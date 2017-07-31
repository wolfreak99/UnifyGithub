// Original url: http://wiki.unity3d.com/index.php/MeshCreationHelper
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MeshHelpers/MeshCreationHelper.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
Author: Evelyn Liu (evepoe) 
MeshCreationHelper.cs  
 
/* ------------------------------------------------------------------------------------
   | Author:  Evelyn Liu (evelyn.liuqi@gmail.com).  
   | Created: 4/24/2010. (Modified 4/28/2010. Corrected a bug as suggested by Mark.)
   | Usage:   Mesh creation helper. To create a new mesh with one of the submesh only.
   ------------------------------------------------------------------------------------ */
 
using System.Collections.Generic;
using UnityEngine;
 
public static class MeshCreationHelper
{
	///------------------------------------------------------------
	/// <summary>
	/// Create a new mesh with one of oldMesh's submesh
	/// </summary>
	///--------------~~~~------------------------------------------------
	public static Mesh CreateMesh(Mesh oldMesh, int subIndex)
	{
		Mesh newMesh = new Mesh();
 
		List<int> triangles = new List<int>();
		triangles.AddRange(oldMesh.GetTriangles(subIndex)); // the triangles of the sub mesh
 
		List<Vector3> newVertices = new List<Vector3>();
		List<Vector2> newUvs = new List<Vector2>();
 
		// Mark's method. 
		Dictionary<int, int> oldToNewIndices = new Dictionary<int, int>();
		int newIndex = 0;
 
		// Collect the vertices and uvs
		for (int i = 0; i < oldMesh.vertices.Length; i++)
		{
			if (triangles.Contains(i))
			{
				newVertices.Add(oldMesh.vertices[i]);
				newUvs.Add(oldMesh.uv[i]);
				oldToNewIndices.Add(i, newIndex);
				++newIndex;
			}
		}
 
		int[] newTriangles = new int[triangles.Count];
 
		// Collect the new triangles indecies
		for (int i = 0; i < newTriangles.Length; i++)
		{
			newTriangles[i] = oldToNewIndices[triangles[i]];
		}
		// Assemble the new mesh with the new vertices/uv/triangles.
		newMesh.vertices = newVertices.ToArray();
		newMesh.uv = newUvs.ToArray();
		newMesh.triangles = newTriangles;
 
		// Re-calculate bounds and normals for the renderer.
		newMesh.RecalculateBounds();
		newMesh.RecalculateNormals();
 
		return newMesh;
	}
}js versionIppokratis Bournellis Oct 2011
The above code in js (is a little faster now, thanks to huge help from fholm and Tenebrous, thanks guys )!
Keep the transform at the origin to avoid searching for it.

function CombineSubMeshes( oldMesh : Mesh, subIndex: int ):Mesh//Optimus
{
 
    var newMesh : Mesh = new Mesh();
    var triangles : int[] = oldMesh.GetTriangles(subIndex);
    var trianglesLength : int = triangles.Length;
 
    var newVertices : Vector3[] = new Vector3[trianglesLength];
    var newUvs :  Vector2[] = new Vector2[trianglesLength];
    var oldToNewIndices : int[] = new int[oldMesh.vertices.Length];
    var newIndex : int = 0;
 	var v :Vector3[]= oldMesh.vertices;
    var uv :Vector2[]= oldMesh.uv;
    // Collect the vertices and uvs
 
     var vertexIndex : int;
     var i : int= 0;
    for ( i = 0; i < trianglesLength; ++i)
    {
        vertexIndex = triangles[i];
        if( oldToNewIndices[vertexIndex] == 0 )
        {
            newVertices[newIndex] = v[vertexIndex];
            newUvs[newIndex] = uv[vertexIndex];
            oldToNewIndices[vertexIndex] = newIndex + 1;
            ++newIndex;
        }
    }
 
 
    var newTriangles : int[] = new int[trianglesLength];
    var newTrianglesLength = newTriangles.Length;
 
    // Collect the new triangles indecies
    for ( i = 0; i < newTrianglesLength; ++i)
    {
        newTriangles[i] = oldToNewIndices[triangles[i]] - 1;
    }
    // Assemble the new mesh with the new vertices/uv/triangles.
    newMesh.vertices = newVertices;
    newMesh.uv = newUvs;
    newMesh.triangles = newTriangles;
 
    // Re-calculate bounds and normals for the renderer.
    newMesh.RecalculateBounds();
    newMesh.RecalculateNormals();
    newMesh.Optimize();
        Debug.Log("submesh " + subIndex.ToString() + "is ready");
    return newMesh;
}
}
