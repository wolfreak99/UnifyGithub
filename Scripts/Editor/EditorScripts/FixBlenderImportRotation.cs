// Original url: http://wiki.unity3d.com/index.php/FixBlenderImportRotation
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/FixBlenderImportRotation.cs
// File based on original modification date of: 4 March 2016, at 13:35. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Benjamin Schaaf 
Contents [hide] 
1 Description 
2 Usage 
3 Code 
3.1 UnityScript 
3.2 C# 

Description This script modifies the mesh and object data before and imported blender file is saved to disk. This get's rid of all the problems caused by blenders forward axis to be X rather than Unity's Z. Instead of all the objects in the object to be rotated by x-90 degrees, the mesh data is properly rotated, meaning transform.forward is the real forward axis, and lookAt actually works! 
Usage The script needs to be placed in a folder called Editor. It will run automatically when a blender asset is imported, so if blender files already exist, they should be reimported for the fix to take place. 
Note that this will not change any prefab rotations, so you might have to go through your prefabs and subtract x-90 degrees from their rotation. 
Code Some optimisations could probably be done, but this won't run during runtime anyway (or even be built for that matter), so who really cares? 
UnityScript #pragma strict
 
import System.IO;
import System.Linq;
 
/*
Author: Benjamin Schaaf
*/
class BlenderAssetProcessor extends AssetPostprocessor {
    //After an asset was imported, but before it is saved to disk
    public function OnPostprocessModel(object:GameObject):void {
 
        //only perform corrections with blender files
        var importer : ModelImporter = assetImporter as ModelImporter;
        if (Path.GetExtension(importer.assetPath) == ".blend") {
            RotateObject(object.transform);
        }
 
        //Don't know why we need this...
        //Fixes wrong parent rotation
        object.transform.rotation = Quaternion.identity;
    }
 
    //recursively rotate a object tree individualy
    private function RotateObject(object:Transform):void {
        object.eulerAngles.x += 90;
 
        //if a meshFilter is attached, we rotate the vertex mesh data
        var meshFilter:MeshFilter = object.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (meshFilter) {
            RotateMesh(meshFilter.sharedMesh);
        }
 
        //do this too for all our children
        //Casting is done to get rid of implicit downcast errors
        for (var child:Transform in (object as IEnumerable).Cast.<Transform>()) {
            RotateObject(child);
        }
    }
 
    //"rotate" the mesh data
    private function RotateMesh(mesh:Mesh):void {
        var index:int = 0;
 
        //switch all vertex z values with y values
        var vertices:Vector3[] = mesh.vertices;
        for (index = 0; index < vertices.length; index++) {
            vertices[index] = Vector3(vertices[index].x, vertices[index].z, vertices[index].y);
        }
        mesh.vertices = vertices;
 
        //for each submesh, we invert the order of vertices for all triangles
        //for some reason changing the vertex positions flips all the normals???
        for (var submesh:int = 0; submesh < mesh.subMeshCount; submesh++) {
            var triangles:int[] = mesh.GetTriangles(submesh);
            for (index = 0; index < triangles.length; index += 3) {
                var intermediate:int = triangles[index];
                triangles[index] = triangles[index + 2];
                triangles[index + 2] = intermediate;
            }
            mesh.SetTriangles(triangles, submesh);
        }
 
        //recalculate other relevant mesh data
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}C# using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;
 
public class BlenderAssetProcessor : AssetPostprocessor
{
 
     public void OnPostprocessModel(GameObject obj  )
    {
 
        //only perform corrections with blender files
 
        ModelImporter importer = assetImporter as ModelImporter;
        if (Path.GetExtension(importer.assetPath) == ".blend")
        {
            RotateObject(obj.transform);
        }
 
        //Don't know why we need this...
        //Fixes wrong parent rotation
        obj.transform.rotation = Quaternion.identity;
    }
 
    //recursively rotate a object tree individualy
    private void RotateObject(Transform obj  )
    {
        Vector3 objRotation = obj.eulerAngles;
        objRotation.x += 90f;
        obj.eulerAngles = objRotation;
 
        //if a meshFilter is attached, we rotate the vertex mesh data
        MeshFilter meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (meshFilter)
        {
            RotateMesh(meshFilter.sharedMesh);
        }
 
        //do this too for all our children
        //Casting is done to get rid of implicit downcast errors
        foreach (Transform child in obj)
        {
            RotateObject(child);
        }
    }
 
    //"rotate" the mesh data
    private void RotateMesh(Mesh mesh)
    {
        int index = 0;
 
        //switch all vertex z values with y values
        Vector3[] vertices = mesh.vertices;
        for (index = 0; index < vertices.Length; index++)
        {
            vertices[index] = new Vector3(vertices[index].x, vertices[index].z, vertices[index].y);
        }
        mesh.vertices = vertices;
 
        //for each submesh, we invert the order of vertices for all triangles
        //for some reason changing the vertex positions flips all the normals???
        for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
        {
            int[] triangles = mesh.GetTriangles(submesh);
            for (index = 0; index < triangles.Length; index += 3)
            {
                int intermediate = triangles[index];
                triangles[index] = triangles[index + 2];
                triangles[index + 2] = intermediate;
            }
            mesh.SetTriangles(triangles, submesh);
        }
 
        //recalculate other relevant mesh data
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
}
