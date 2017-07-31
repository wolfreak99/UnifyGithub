// Original url: http://wiki.unity3d.com/index.php/WorldUVs
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/WorldUVs.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Aubrey Falconer (Robur) 
Special recognition: Mike_Mac, Azupko 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - WorldUV.js 
4 JavaScript - WorldUVAdvanced.js 

Description  
Cubes UV mapped in world spaceRecursively generates UV maps for mesh faces so that they all line up with each other in world space. Want to create a world out of scaled and rotated cubes, then add a seamless texture to them all? This script is what you need! 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
When you are ready to generate new UVs for your world; select your parent world object in the Inspector, and then run the "GameObject > World Space UVMap" menu command. 
JavaScript - WorldUV.js This algorithm is quite simple, but also very effective. It determines the nearest cardinal direction to each face, then textures the face in that direction's plane. 
@MenuItem ("GameObject/World Space UVMap")
static function doMeshes() {
	if(!EditorUtility.DisplayDialog("UV Remap Confirmation", "This tool will recursively alter the UV map(s) of the mesh(es) in your selected object, altering them to all line up with each other in world space.", "Remap UVs of Selected Mesh(es)!", "Cancel")) return;
	var meshes = Selection.activeGameObject.GetComponentsInChildren(MeshFilter);
	for (var mf : MeshFilter in meshes) DoMesh(mf);
}
 
static function DoMesh (mf : MeshFilter) {
	mesh = mf.sharedMesh;
	var uvs = new Vector2[mesh.vertices.Length];
	var tris = mesh.triangles;
	var go = mf.gameObject;
	for (var i=0;i<tris.Length;i+=3) {
		var a : Vector3 = go.transform.TransformPoint(mesh.vertices[tris[i]]);
		var b : Vector3 = go.transform.TransformPoint(mesh.vertices[tris[i+1]]);
		var c : Vector3 = go.transform.TransformPoint(mesh.vertices[tris[i+2]]);
		var n : Vector3 = Vector3.Cross(a-c, b-c).normalized;
		if(Vector3.Dot(Vector3.up, n) > .5 || Vector3.Dot(-Vector3.up, n) > .5) {
			uvs[tris[i]]	= Vector2(a.x, a.z);
			uvs[tris[i+1]]	= Vector2(b.x, b.z);
			uvs[tris[i+2]]	= Vector2(c.x, c.z);
		}
		else if(Vector3.Dot(Vector3.right, n) > .5 || Vector3.Dot(Vector3.left, n) > .5) {
			uvs[tris[i]]	= Vector2(a.y, a.z);
			uvs[tris[i+1]]	= Vector2(b.y, b.z);
			uvs[tris[i+2]]	= Vector2(c.y, c.z);
		}
		else {
			uvs[tris[i]]	= Vector2(a.y, a.x);
			uvs[tris[i+1]]	= Vector2(b.y, b.x);
			uvs[tris[i+2]]	= Vector2(c.y, c.x);
		}
	}
 	mesh.uv = uvs;
}JavaScript - WorldUVAdvanced.js This algorithm is more advanced than the preceding one, but it doesn't always work. It attempts to texture each mesh face in it's own plane. If you can figure out what is wrong with this script, please post your revised code! 
@MenuItem ("GameObject/World Space UVMap")
static function doMeshes() {
	if(!EditorUtility.DisplayDialog("UV Remap Confirmation", "This tool will recursively alter the UV map(s) of the mesh(es) in your selected object, altering them to all line up with each other in world space.", "Remap UVs of Selected Mesh(es)!", "Cancel")) return;
	var meshes = Selection.activeGameObject.GetComponentsInChildren(MeshFilter);
	for (var mf : MeshFilter in meshes) DoMesh(mf);
}
 
static function DoMesh (mf : MeshFilter) {
	var uvs = new Vector2[mf.mesh.vertices.Length];
	var tris = mf.mesh.triangles;
	for (var i=0;i<tris.Length;i+=3) {
		var a : Vector3 = mf.transform.TransformPoint(mf.mesh.vertices[tris[i]]);
		var b : Vector3 = mf.transform.TransformPoint(mf.mesh.vertices[tris[i+1]]);
		var c : Vector3 = mf.transform.TransformPoint(mf.mesh.vertices[tris[i+2]]);
		var center : Vector3 = (a + b + c) / 3;
		var normal : Vector3 = Vector3.Cross(a-c, b-c).normalized;
 
		a = Quaternion.LookRotation(normal) * a;
		b = Quaternion.LookRotation(normal) * b;
		c = Quaternion.LookRotation(normal) * c;
 
		uvs[tris[i]]	= Vector2(a.x, a.y);
		uvs[tris[i+1]]	= Vector2(b.x, b.y);
		uvs[tris[i+2]]	= Vector2(c.x, c.y);
	}
 	mf.mesh.uv = uvs;
}
}
