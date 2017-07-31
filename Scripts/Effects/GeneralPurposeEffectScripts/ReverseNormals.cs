// Original url: http://wiki.unity3d.com/index.php/ReverseNormals
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/ReverseNormals.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Joachim Ante 
DescriptionThis script lets you reverse normals. The normals will only be reversed in playmode so it will still look inverted when viewing in edit mode. 
UsageAttach the script to the mesh you want normals to be reversed 
CSharp- ReverseNormals.csusing UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(MeshFilter))]
public class ReverseNormals : MonoBehaviour {
 
	void Start () {
		MeshFilter filter = GetComponent(typeof (MeshFilter)) as MeshFilter;
		if (filter != null)
		{
			Mesh mesh = filter.mesh;
 
			Vector3[] normals = mesh.normals;
			for (int i=0;i<normals.Length;i++)
				normals[i] = -normals[i];
			mesh.normals = normals;
 
			for (int m=0;m<mesh.subMeshCount;m++)
			{
				int[] triangles = mesh.GetTriangles(m);
				for (int i=0;i<triangles.Length;i+=3)
				{
					int temp = triangles[i + 0];
					triangles[i + 0] = triangles[i + 1];
					triangles[i + 1] = temp;
				}
				mesh.SetTriangles(triangles, m);
			}
		}		
	}
}
}
