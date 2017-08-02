/*************************
 * Original url: http://wiki.unity3d.com/index.php/CombineSkinnedMeshes
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/PerformanceScripts/CombineSkinnedMeshes.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Gabriel Santos 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Development.PerformanceScripts
{
    Description Class Model that allows you combine multiple skinned meshes at runtime. IMPORTANT: The number of vertices and bones is very particular, you must set it up according to your own model. 
    This Class uses the SkinMeshCombineUtility ! 
    Feel free to clean up the code and add to it. :D 
    C# Code /*IMPORTANT: READ !!!!!!
    @Autor: Gabriel Santos
    @Description Class that Combine Multilple SkinnedMeshes in just one Skinned Mesh Renderer
    @Usage: Just AddComponent("CombineSkinnedMeshes") to your root component;
    @IMPORTANT: This script uses the SkinMeshCombineUtility Script!
     
    PS: It was tested with FBX files exported from 3D MAX
    This Vertex Number and Bone Number must be configured according to your own AVATAR...
    You can make a Counter to get the Number of Vertices from your imported character, I choice not do it since 
    this is script is just executed one time... */
     
    using UnityEngine;
    using System.Collections;
     
     
    public class CombineSkinnedMeshes : MonoBehaviour {
     
    		/// Usually rendering with triangle strips is faster.
    		/// However when combining objects with very low triangle counts, it can be faster to use triangles.
    		/// Best is to try out which value is faster in practice.
    		public bool castShadows = true;
    		public bool receiveShadows = true;
     
    	/* This is for very particular use, you must set it regarding to your Character */
    		public static int VERTEX_NUMBER= [Your Vertices Number]; //The number of vertices total of the character
    		public static int BONE_NUMBER =[Your Bones Number];		//The number of bones total
     
    	// Use this for initialization
    	void Start () {
     
    		//Getting all Skinned Renderer from Children
    		Component[] allsmr = GetComponentsInChildren(typeof(SkinnedMeshRenderer));
    		Matrix4x4 myTransform = transform.worldToLocalMatrix;		
     
    		//The hash with the all bones references: it will be used for set up the BoneWeight Indexes
    		Hashtable boneHash =new Hashtable();
     
    		/* If you want make a counter in order to get the total of Vertices and Bones do it here ... */
     
    		//The Sum of All Child Bones
    		Transform[] totalBones = new Transform[BONE_NUMBER];//Total of Bones for my Example
    		//The Sum of the BindPoses
    		Matrix4x4[] totalBindPoses = new Matrix4x4[BONE_NUMBER];//Total of BindPoses
    		//The Sum of BoneWeights
    		BoneWeight[] totalBoneWeight = new BoneWeight[VERTEX_NUMBER];//total of Vertices for my Example
     
    		int offset=0;
    		int b_offset=0;
    		Transform[] usedBones= new Transform[totalBones.Length];
     
    		ArrayList myInstances = new ArrayList();
     
    		//Setting my Arrays for copies		
    		ArrayList myMaterials=new ArrayList();
     
    		for(int i=0;i<allsmr.Length;i++)
    		{
    		//Getting one by one
    			SkinnedMeshRenderer smrenderer  =  (SkinnedMeshRenderer)allsmr[i];
    			//Making changes to the Skinned Renderer
    			SkinMeshCombineUtility.MeshInstance instance = new SkinMeshCombineUtility.MeshInstance ();
     
    			//Setting the Mesh for the instance
    			instance.mesh = smrenderer.sharedMesh;
     
    			//Getting All Materials
    			for(int t=0;t<smrenderer.sharedMaterials.Length;t++)
    			{
    				myMaterials.Add(smrenderer.sharedMaterials[t]);
    			}
     
    			if (smrenderer != null && smrenderer.enabled && instance.mesh != null) {							
     
    				instance.transform = myTransform * smrenderer.transform.localToWorldMatrix;
     
    				//Material == null
    				smrenderer.sharedMaterials =new  Material[1];
     
    				//Getting  subMesh
    				for(int t=0;t<smrenderer.sharedMesh.subMeshCount;t++)
    				{
    					instance.subMeshIndex = t; 
    					myInstances.Add(instance);
    				}
     
    				//Copying Bones
    				for(int x=0;x<smrenderer.bones.Length;x++)
    				{
     
    				bool flag = false;
    					for(int j=0;j<totalBones.Length;j++)
    					{
    						if(usedBones[j]!=null)
    							//If the bone was already inserted
    							if((smrenderer.bones[x]==usedBones[j]))
    							{
    								flag = true;
    								break;
    							}
    					}
     
    					//If Bone is New ...
    					if(!flag)
    					{
    						//Debug.Log("Inserted bone:"+smrenderer.bones[x].name);
    						for(int f=0;f<totalBones.Length;f++)
    						{
    							//Insert bone at the firs free position
    							if(usedBones[f]==null)
    							{
    								usedBones[f] = smrenderer.bones[x];
    								break;
    							}
    						}
    						//inserting bones in totalBones
    						totalBones[offset]=smrenderer.bones[x];		
    						//Reference HashTable
    						boneHash.Add(smrenderer.bones[x].name,offset);
     
    						//Recalculating BindPoses
    						//totalBindPoses[offset] = smrenderer.sharedMesh.bindposes[x] ;						
    						totalBindPoses[offset] = smrenderer.bones[x].worldToLocalMatrix * transform.localToWorldMatrix ;
    						offset++;						
    					}					
    				}
     
    				//RecalculateBoneWeights
    				for(int x=0;x<smrenderer.sharedMesh.boneWeights.Length ;x++)
    					{	
    						//Just Copying and changing the Bones Indexes !!						
    						totalBoneWeight[b_offset] =  recalculateIndexes(smrenderer.sharedMesh.boneWeights[x],boneHash,smrenderer.bones);
    						b_offset++;
    					}
    				//Disabling current SkinnedMeshRenderer
    				((SkinnedMeshRenderer)allsmr[i]).enabled = false;					
    			}
    		}		
     
    		SkinMeshCombineUtility.MeshInstance[] instances = (SkinMeshCombineUtility.MeshInstance[])myInstances.ToArray(typeof(SkinMeshCombineUtility.MeshInstance));			
     
    		// Make sure we have a SkinnedMeshRenderer
    		if (GetComponent(typeof(SkinnedMeshRenderer)) == null)
    		{
    			gameObject.AddComponent(typeof(SkinnedMeshRenderer));
    		}
     
    		//Setting Skinned Renderer
    		SkinnedMeshRenderer objRenderer = (SkinnedMeshRenderer)GetComponent(typeof(SkinnedMeshRenderer));
    		//Setting Mesh
    		objRenderer.sharedMesh = SkinMeshCombineUtility.Combine(instances);					
     
    		objRenderer.castShadows = castShadows;
    		objRenderer.receiveShadows = receiveShadows;
     
    		//Setting Bindposes
    		objRenderer.sharedMesh.bindposes = totalBindPoses;
     
    		//Setting BoneWeights
    		objRenderer.sharedMesh.boneWeights = totalBoneWeight;
     
    		//Setting bones
    		objRenderer.bones =totalBones;
     
    		//Setting Materials
    		objRenderer.sharedMaterials = (Material[])myMaterials.ToArray(typeof(Material));
     
    		objRenderer.sharedMesh.RecalculateNormals();
    		objRenderer.sharedMesh.RecalculateBounds();
     
    		//Enable Mesh
    		objRenderer.enabled = true;					
     
    		/* 	Debug.Log("############################################");
    		Debug.Log("M Materials "+myMaterials.Count);
    		Debug.Log("bindPoses "+objRenderer.sharedMesh.bindposes.Length);
    		Debug.Log("boneWeights "+objRenderer.sharedMesh.boneWeights.Length);
    		Debug.Log("Bones "+objRenderer.bones.Length);
    		Debug.Log("Vertices "+objRenderer.sharedMesh.vertices.Length); */
     
    	}
     
    	/* 
    	@Description: Revert the order of an array of components
    	(NOT USED)
    	*/
    	static Component[] revertComponent(Component[] comp )
    	{
    		Component[] result = new Component[comp.Length];
    		int x=0;
    		for(int i=comp.Length-1;i>=0;i--)
    		{
    				result[x++]=comp[i];
    		}
     
    		return result;
    	}	
     
    	/* 
    	@Description: Setting the Indexes for the new bones	
    	*/
    	static BoneWeight recalculateIndexes(BoneWeight bw,Hashtable boneHash,Transform[] meshBones )
    	{
    		BoneWeight retBw = bw;
    		retBw.boneIndex0 = (int)boneHash[meshBones[bw.boneIndex0].name];
    		retBw.boneIndex1 = (int)boneHash[meshBones[bw.boneIndex1].name];
    		retBw.boneIndex2 = (int)boneHash[meshBones[bw.boneIndex2].name];
    		retBw.boneIndex3 = (int)boneHash[meshBones[bw.boneIndex3].name];
    		return retBw;
    	}
}
}
