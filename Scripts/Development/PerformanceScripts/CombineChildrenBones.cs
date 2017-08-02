/*************************
 * Original url: http://wiki.unity3d.com/index.php/CombineChildrenBones
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/PerformanceScripts/CombineChildrenBones.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author 
 *   
 * Description 
 *   
 * Caveats 
 *   
 * CombineChildrenBones 
 *   
 * MeshCombineBonesUtility 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Development.PerformanceScripts
{
    Author Mike Laurence (http://mikelaurence.com), with much of the code extracted from everyone's favorite CombineChildren script. 
    Description This script combines all MeshFilter meshes (e.g., static meshes without bones) in its children into a single SkinnedMeshRenderer. Thereafter, it automatically updates the transforms of the bones to match any changes to the original GameObjects. 
    Caveats This script appears to have very similar functionality to the MeshMerger script, though with much more code. Since I used CombineChildren as the base for this script, I'm not sure how much is really necessary. Perhaps someone can do a comparison between the two (hopefully with benchmarks!) 
    Multiple materials are not yet supported 
    The destroy function is not yet implemented 
    Cutting down to a single draw call does not necessarily give you a performance increase. In fact, in my own case, this script didn't help at all! Constantly transforming bone vertices is cpu-intensive, and on the iPhone I had a significant frame rate drop using this script as opposed to just having 10 draw calls of my individual objects. 
    The script is kind of ugly, as I've left in comments of what I changed from the original CombineChildren script. Those can probably be erased, but at least they're here in the wiki history for posterity! 
    CombineChildrenBones using UnityEngine;
    using System.Collections;
    /*
    Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
    This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.
     
    Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
    */
     
    [AddComponentMenu("Mesh/Combine Children Bones")]
    public class CombineChildrenBones : MonoBehaviour {
     
    	/// Usually rendering with triangle strips is faster.
    	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
    	/// Best is to try out which value is faster in practice.
    	public bool generateTriangleStrips = true;
     
    	private SkinnedMeshRenderer skinned;
    	private GameObject[] childrenObjects;
    	private Transform[] childrenTransforms;
     
    	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
    	void Start () {
     
    		float startTime = Time.realtimeSinceStartup;
     
    		Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
    		childrenObjects = new GameObject[filters.Length];
    		childrenTransforms = new Transform[filters.Length];
    		Matrix4x4 myTransform = transform.worldToLocalMatrix;
    		Hashtable materialToMesh = new Hashtable();
     
    		for (int i=0;i<filters.Length;i++) {
    			MeshFilter filter = (MeshFilter)filters[i];
    			childrenObjects[i] = filter.gameObject;
    			childrenTransforms[i] = filter.transform;
     
    			Renderer curRenderer  = filters[i].renderer;
    			MeshCombineBonesUtility.MeshInstance instance = new MeshCombineBonesUtility.MeshInstance();
    			instance.mesh = filter.sharedMesh;
    			if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
    				instance.transform = myTransform * filter.transform.localToWorldMatrix;
    				instance.objectTransform = filter.transform;
     
    				Material[] materials = curRenderer.sharedMaterials;
    				for (int m=0;m<materials.Length;m++) {
    					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
     
    					ArrayList objects = (ArrayList)materialToMesh[materials[m]];
    					if (objects != null) {
    						objects.Add(instance);
    					}
    					else
    					{
    						objects = new ArrayList ();
    						objects.Add(instance);
    						materialToMesh.Add(materials[m], objects);
    					}
    				}
     
    				curRenderer.enabled = false;
    			}
    		}
     
    		foreach (DictionaryEntry de  in materialToMesh) {
    			ArrayList elements = (ArrayList)de.Value;
    			MeshCombineBonesUtility.MeshInstance[] instances = (MeshCombineBonesUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineBonesUtility.MeshInstance));
     
    			// We have a maximum of one material, so just attach the mesh to our own game object
    			if (materialToMesh.Count == 1)
    			{
    				// Make sure we have a mesh filter & renderer
    				//if (GetComponent(typeof(MeshFilter)) == null)
    				//	gameObject.AddComponent(typeof(MeshFilter));
    //				if (!GetComponent("MeshRenderer"))
    //					gameObject.AddComponent("MeshRenderer");
    				if (!GetComponent(typeof(SkinnedMeshRenderer)))
    					gameObject.AddComponent(typeof(SkinnedMeshRenderer));
     
    				//MeshFilter filter = (MeshFilter)GetComponent(typeof(MeshFilter));
    				skinned = (SkinnedMeshRenderer) GetComponent(typeof(SkinnedMeshRenderer));
    				skinned.sharedMesh = MeshCombineBonesUtility.Combine(instances, generateTriangleStrips, skinned, transform);
    //				skinned.sharedMesh = MeshCombineBonesUtility.Combine(instances, generateTriangleStrips);
    //				renderer.material = (Material)de.Key;
    //				renderer.enabled = true;
    				skinned.quality = SkinQuality.Bone1;
    				skinned.sharedMaterial = (Material)de.Key;
    				skinned.enabled = true;
     
    			}
    			// We have multiple materials to take care of, build one mesh / gameobject for each material
    			// and parent it to this object
    			else
    			{
    				Debug.LogError("Multi-material combines not supported yet!");
    				/*
    				GameObject go = new GameObject("Combined mesh");
    				go.transform.parent = transform;
    				go.transform.localScale = Vector3.one;
    				go.transform.localRotation = Quaternion.identity;
    				go.transform.localPosition = Vector3.zero;
    				go.AddComponent(typeof(MeshFilter));
    				go.AddComponent("MeshRenderer");
    				go.renderer.material = (Material)de.Key;
    				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
    				filter.mesh = MeshCombineBonesUtility.Combine(instances, generateTriangleStrips, null, transform);
    				*/
    			}
    		}
     
    		//((GUIText) GameObject.Find("Debug").GetComponent(typeof(GUIText))).text = (Time.realtimeSinceStartup - startTime) + "";
    	}
     
    	private Vector3 hiddenPosition = new Vector3(0, -100000, 0);
     
    	void Update() {
    		for (int b = 0; b < skinned.bones.Length; b++) {
    			if (childrenTransforms[b].gameObject.active) {
    				skinned.bones[b].position = childrenTransforms[b].position;
    				skinned.bones[b].rotation = childrenTransforms[b].rotation;
    			}
    			//else
    			//	skinned.bones[b].position = hiddenPosition;
    		}
    	}
     
    	void Destroy(GameObject go) {
    		for (int b = 0; b < childrenObjects.Length; b++) {
    			if (childrenObjects[b] == go) {
    				// TODO!
    			}
    		}
    	}
    }MeshCombineBonesUtility Note: this script is a required additional component to the CombineChildrenBones script. 
    using UnityEngine;
    using System.Collections;
     
    public class MeshCombineBonesUtility {
     
    	public struct MeshInstance
    	{
    		public Mesh      mesh;
    		public int       subMeshIndex;            
    		public Matrix4x4 transform;
    		public Transform objectTransform;
    		public int		 startVertex;
    		public int		 endVertex;
    	}
     
    	public static Mesh Combine (MeshInstance[] combines, bool generateStrips, SkinnedMeshRenderer skinnedMeshRenderer, Transform masterTransform)
    	{
    		int vertexCount = 0;
    		int triangleCount = 0;
    		int stripCount = 0;
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    			{
    				vertexCount += combine.mesh.vertexCount;
     
    				if (generateStrips)
    				{
    					// SUBOPTIMAL FOR PERFORMANCE
    					int curStripCount = combine.mesh.GetTriangleStrip(combine.subMeshIndex).Length;
    					if (curStripCount != 0)
    					{
    						if( stripCount != 0 )
    						{
    							if ((stripCount & 1) == 1 )
    								stripCount += 3;
    							else
    								stripCount += 2;
    						}
    						stripCount += curStripCount;
    					}
    					else
    					{
    						generateStrips = false;
    					}
    				}
    			}
    		}
     
    		// Precomputed how many triangles we need instead
    		if (!generateStrips)
    		{
    			foreach( MeshInstance combine in combines )
    			{
    				if (combine.mesh)
    				{
    					triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
    				}
    			}
    		}
     
    		Vector3[] vertices = new Vector3[vertexCount] ;
    		Vector3[] normals = new Vector3[vertexCount] ;
    		Vector4[] tangents = new Vector4[vertexCount] ;
    		Vector2[] uv = new Vector2[vertexCount];
    		Vector2[] uv1 = new Vector2[vertexCount];
    		Color[] colors = new Color[vertexCount];
     
    		int[] triangles = new int[triangleCount];
    		int[] strip = new int[stripCount];
     
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
     
    		offset=0;
    		foreach( MeshInstance combine in combines )
    		{
    			if (combine.mesh)
    				CopyColors(combine.mesh.vertexCount, combine.mesh.colors, colors, ref offset);
    		}
     
    		int triangleOffset=0;
    		int stripOffset=0;
    		int vertexOffset=0;
    		for (int comb = 0; comb < combines.Length; comb++)
    		{
    			MeshInstance combine = combines[comb];
    			if (combine.mesh)
    			{
    				if (generateStrips)
    				{
    					int[] inputstrip = combine.mesh.GetTriangleStrip(combine.subMeshIndex);
    					if (stripOffset != 0)
    					{
    						if ((stripOffset & 1) == 1)
    						{
    							strip[stripOffset+0] = strip[stripOffset-1];
    							strip[stripOffset+1] = inputstrip[0] + vertexOffset;
    							strip[stripOffset+2] = inputstrip[0] + vertexOffset;
    							stripOffset+=3;
    						}
    						else
    						{
    							strip[stripOffset+0] = strip[stripOffset-1];
    							strip[stripOffset+1] = inputstrip[0] + vertexOffset;
    							stripOffset+=2;
    						}
    					}
     
    					for (int i=0;i<inputstrip.Length;i++)
    					{
    						strip[i+stripOffset] = inputstrip[i] + vertexOffset;
    					}
    					stripOffset += inputstrip.Length;
    				}
    				else
    				{
    					int[]  inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
    					for (int i=0;i<inputtriangles.Length;i++)
    					{
    						triangles[i+triangleOffset] = inputtriangles[i] + vertexOffset;
    					}
    					triangleOffset += inputtriangles.Length;
    				}
    				combines[comb].startVertex = vertexOffset;
    				vertexOffset += combine.mesh.vertexCount;
    				combines[comb].endVertex = vertexOffset;
    			}
    		}
     
    		Mesh mesh = new Mesh();
    		mesh.name = "Combined Mesh";
    		mesh.vertices = vertices;
    		mesh.normals = normals;
    		mesh.colors = colors;
    		mesh.uv = uv;
    		mesh.uv1 = uv1;
    		mesh.tangents = tangents;
    		if (generateStrips)
    			mesh.SetTriangleStrip(strip, 0);
    		else
    			mesh.triangles = triangles;
     
     
    		// -----===== Bone calculation! =====-----
     
    		BoneWeight[] weights = new BoneWeight[vertices.Length];
    		Transform[] bones = new Transform[combines.Length];
    		Matrix4x4[] bindPoses = new Matrix4x4[combines.Length];
     
    		for (int c = 0; c < combines.Length; c++) {
    			MeshInstance combine = combines[c];
    			Debug.Log("Mesh #" + c + " - vertices: " + combine.startVertex + ", " + combine.endVertex);
    			for (int v = combine.startVertex; v < combine.endVertex; v++) {
    				weights[v].boneIndex0 = c;   // Assign bone index as original mesh index
    				weights[v].weight0 = 1;   // Assign full 100% weight to bone
    			}
     
    			// Set up bones & bind poses
    			bones[c] = combine.objectTransform;
    			bones[c].parent = masterTransform;
    			bindPoses[c] = bones[c].worldToLocalMatrix * masterTransform.localToWorldMatrix;
    		}
     
    		mesh.boneWeights = weights;
    		mesh.bindposes = bindPoses;
    		skinnedMeshRenderer.bones = bones;
     
    		return mesh;
    	}
     
    	static void Copy (int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    	{
    		for (int i=0;i<src.Length;i++)
    			dst[i+offset] = transform.MultiplyPoint(src[i]);
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
     
    	static void CopyColors (int vertexcount, Color[] src, Color[] dst, ref int offset)
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
