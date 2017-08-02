/*************************
 * Original url: http://wiki.unity3d.com/index.php/SkinMeshCombineUtility
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/PerformanceScripts/SkinMeshCombineUtility.cs
 * File based on original modification date of: 10 January 2012, at 20:47. 
 *
 * Author: Gabriel Santos 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Development.PerformanceScripts
{
    Description Class used in order to combine multiple Skinned Meshes into just one Skinned Mesh, it works with CombineSkinnedMeshes Script. 
    Feel free to clean up the code and improve it. :D 
    C# Code /*IMPORTANT: READ !!!!!!
    @Autor: Gabriel Santos
    @Description Class that Combine the Meshes and create SubMeshes for Meshes which uses different material.
    @IMPORTANT: This script is used by CombineSkinnedMeshes script!
    This script was based on the MeshCombineUtility provided by Unity, I have just modified in order to create new submeshes for meshes which uses different materials
    PS: It was tested with FBX files exported from 3D MAX*/
     
    using UnityEngine;
    using System.Collections;
     
    public class SkinMeshCombineUtility {
     
    	public struct MeshInstance
    	{
    		public Mesh      mesh;
    		public int       subMeshIndex;            
    		public Matrix4x4 transform;
    	}
     
    	public static Mesh Combine (MeshInstance[] combines)
    	{
    		int vertexCount = 0;
    		int triangleCount = 0;
     
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    			{
    				vertexCount += combine.mesh.vertexCount;						
    			}
    		}
     
    		// Precomputed how many triangles we need instead
     
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    			{
    				triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
    			}
    		}
     
    		Vector3[] vertices = new Vector3[vertexCount] ;
    		Vector3[] normals = new Vector3[vertexCount] ;
    		Vector4[] tangents = new Vector4[vertexCount] ;
    		Vector2[] uv = new Vector2[vertexCount];
    		Vector2[] uv1 = new Vector2[vertexCount];		
     
    		int offset;
     
    		offset=0;
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    				Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);			
    		}		
     
    		offset=0;
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    			{
    				Matrix4x4 invTranspose = combine.transform;
    				invTranspose = invTranspose.inverse.transpose;
    				CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
    			}
     
    		}
    		offset=0;
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    			{
    				Matrix4x4 invTranspose = combine.transform;
    				invTranspose = invTranspose.inverse.transpose;
    				CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
    			}
     
    		}
    		offset=0;
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    				Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset);
    		}
     
    		offset=0;
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    				Copy(combine.mesh.vertexCount, combine.mesh.uv1, uv1, ref offset);
    		}
     
    		int triangleOffset=0;
    		int vertexOffset=0;
     
    		int j=0;
     
    		Mesh mesh = new Mesh();
    		mesh.vertices = vertices;
    		mesh.normals = normals;
    		mesh.uv = uv;
    		mesh.uv1 = uv1;
    		mesh.tangents = tangents;
     
    		//Setting SubMeshes
    		mesh.subMeshCount = combines.Length;
     
    		foreach( MeshInstance combine in combines )
    		{		
    			int[]  inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
    			int[] trianglesx = new int[inputtriangles.Length];
    			for (int i=0;i<inputtriangles.Length;i++)
    			{
    				//triangles[i+triangleOffset] = inputtriangles[i] + vertexOffset;
    				trianglesx[i] = inputtriangles[i] + vertexOffset;						
    			}
    			triangleOffset += inputtriangles.Length;
    			mesh.SetTriangles(trianglesx,j++);
     
    			vertexOffset += combine.mesh.vertexCount;			
    		}
     
    		mesh.name = "Combined Mesh";
     
    		return mesh;
    	}
     
    	static void Copy (int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    	{
    		for (int i=0;i<src.Length;i++)
    		{
    			dst[i+offset] = transform.MultiplyPoint(src[i]);
    		}
    		offset += vertexcount;
    	}
     
    	static void CopyBoneWei (int vertexcount, BoneWeight[] src, BoneWeight[] dst, ref int offset, Matrix4x4 transform)
    	{
    		for (int i=0;i<src.Length;i++)
    			dst[i+offset] =src[i];
    		offset += vertexcount;
    	}
     
    	static void CopyNormal (int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    	{
    		for (int i=0;i<src.Length;i++)
    			dst[i+offset] = transform.MultiplyVector(src[i]).normalized;
    		offset += vertexcount;
    	}
     
    	static void Copy (int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
    	{
    		for (int i=0;i<src.Length;i++)
    			dst[i+offset] = src[i];
    		offset += vertexcount;
    	}
     
    	static void CopyTangents (int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
    	{
    		for (int i=0;i<src.Length;i++)
    		{
    			Vector4 p4 = src[i];
    			Vector3 p = new Vector3(p4.x, p4.y, p4.z);
    			p = transform.MultiplyVector(p).normalized;
    			dst[i+offset] = new Vector4(p.x, p.y, p.z, p4.w);
    		}
     
    		offset += vertexcount;
    	}
}
}
