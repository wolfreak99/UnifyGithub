// Original url: http://wiki.unity3d.com/index.php/SkinnedMeshCombiner
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/PerformanceScripts/SkinnedMeshCombiner.cs
// File based on original modification date of: 12 December 2012, at 21:26. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.PerformanceScripts
{
Contents [hide] 
1 Description 
2 Use 
3 Requirements 
4 SkinnedMeshCombiner.cs 

DescriptionThis script will combine skinned meshes whilst retaining their animation and texture properties. Currently it's not tested with multiple materials, as that was not required for what we needed it for. 
UseAdd this script to a master GO, with numerous skinned mesh child GOs. Run scene. Profit. 
RequirementsYour child GOs must have the skinned mesh, on a separate GO, for example the hierarchy should look like this: GO - This script on here. 
CHILD GO - A gameobject that serves as a container for the other components, can have animation data, or, misc components on that do not affect skinned meshes. 
SMC GO - A gameobject with -just- the skinned mesh renderer on it, I mean, you can put other things on here, but they will get destroyed when the script deletes this object. 
Another GO - Misc stuff, this GO will remain intact/functional after you run the scene/simulation. 
CHILD GO 
CHILD GO 
SkinnedMeshCombiner.csusing UnityEngine;
using System.Collections.Generic;
 
public class SkinnedMeshCombiner : MonoBehaviour {
 
    void Start() {        
        SkinnedMeshRenderer[] smRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        List<Transform> bones = new List<Transform>();        
        List<BoneWeight> boneWeights = new List<BoneWeight>();        
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Texture2D> textures = new List<Texture2D>();
        int numSubs = 0;
 
        foreach(SkinnedMeshRenderer smr in smRenderers)
            numSubs += smr.sharedMesh.subMeshCount;
 
        int[] meshIndex = new int[numSubs];
        int boneOffset = 0;
        for( int s = 0; s < smRenderers.Length; s++ ) {
            SkinnedMeshRenderer smr = smRenderers[s];          
 
            BoneWeight[] meshBoneweight = smr.sharedMesh.boneWeights;
 
            // May want to modify this if the renderer shares bones as unnecessary bones will get added.
            foreach( BoneWeight bw in meshBoneweight ) {
                BoneWeight bWeight = bw;
 
                bWeight.boneIndex0 += boneOffset;
                bWeight.boneIndex1 += boneOffset;
                bWeight.boneIndex2 += boneOffset;
                bWeight.boneIndex3 += boneOffset;                
 
                boneWeights.Add( bWeight );
            }
            boneOffset += smr.bones.Length;
 
            Transform[] meshBones = smr.bones;
            foreach( Transform bone in meshBones )
                bones.Add( bone );
 
            if( smr.material.mainTexture != null )
                textures.Add( smr.renderer.material.mainTexture as Texture2D );
 
            CombineInstance ci = new CombineInstance();
            ci.mesh = smr.sharedMesh;
            meshIndex[s] = ci.mesh.vertexCount;
            ci.transform = smr.transform.localToWorldMatrix;
            combineInstances.Add( ci );
 
            Object.Destroy( smr.gameObject );
        }
 
        List<Matrix4x4> bindposes = new List<Matrix4x4>();
 
        for( int b = 0; b < bones.Count; b++ ) {
            bindposes.Add( bones[b].worldToLocalMatrix * transform.worldToLocalMatrix );
        }
 
        SkinnedMeshRenderer r = gameObject.AddComponent<SkinnedMeshRenderer>();
        r.sharedMesh = new Mesh();
        r.sharedMesh.CombineMeshes( combineInstances.ToArray(), true, true );
 
        Texture2D skinnedMeshAtlas = new Texture2D( 128, 128 );
        Rect[] packingResult = skinnedMeshAtlas.PackTextures( textures.ToArray(), 0 );
        Vector2[] originalUVs = r.sharedMesh.uv;
        Vector2[] atlasUVs = new Vector2[originalUVs.Length];
 
        int rectIndex = 0;
        int vertTracker = 0;
        for( int i = 0; i < atlasUVs.Length; i++ ) {
            atlasUVs[i].x = Mathf.Lerp( packingResult[rectIndex].xMin, packingResult[rectIndex].xMax, originalUVs[i].x );
            atlasUVs[i].y = Mathf.Lerp( packingResult[rectIndex].yMin, packingResult[rectIndex].yMax, originalUVs[i].y );            
 
            if( i >= meshIndex[rectIndex] + vertTracker ) {                
                vertTracker += meshIndex[rectIndex];
                rectIndex++;                
            }
        }
 
        Material combinedMat = new Material( Shader.Find( "Diffuse" ) );
        combinedMat.mainTexture = skinnedMeshAtlas;
        r.sharedMesh.uv = atlasUVs;
        r.sharedMaterial = combinedMat;
 
        r.bones = bones.ToArray();
        r.sharedMesh.boneWeights = boneWeights.ToArray();
        r.sharedMesh.bindposes = bindposes.ToArray();
        r.sharedMesh.RecalculateBounds();
    }
}
}
