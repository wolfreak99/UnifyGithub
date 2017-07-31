// Original url: http://wiki.unity3d.com/index.php/PrintPolyCount
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MeshHelpers/PrintPolyCount.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
Author: Aras Pranckevicius 
UsageAttach this script to any game object. It will count the total vertex and triangle count on all Meshes in this game object and all children underneath it and print it on startup. For example, attaching the script to a game level root will calculate the vertex/triangle count in the whole level. 
Note: there is a much better in-editor version of this: MeshInfo script. Thus this one is kind of obsolete. 
JavaScript - PrintPolyCount.jsvar filters : Component[];
filters = GetComponentsInChildren(MeshFilter);
var tris = 0;
var verts = 0;
for( var f : MeshFilter in filters )
{
    tris += f.sharedMesh.triangles.Length / 3;
    verts += f.sharedMesh.vertexCount;
}
print( name + " triangles = " + tris + " vertices = " + verts );C# - PrintPolyCount.csusing UnityEngine;
using System.Collections;
 
public class PrintPolyCount : MonoBehaviour {
 
	// Use this for initialization
	void Start () {
		Component[] filters = GetComponentsInChildren(typeof(MeshFilter));
		int tris = 0;
		int verts = 0;
		foreach (MeshFilter f in filters) {
		    tris += f.sharedMesh.triangles.Length / 3;
		    verts += f.sharedMesh.vertexCount;
		}
		Debug.Log( name + " triangles = " + tris + " vertices = " + verts );
	}
 
}
}
