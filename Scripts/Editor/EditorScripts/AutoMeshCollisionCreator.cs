// Original url: http://wiki.unity3d.com/index.php/AutoMeshCollisionCreator
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/AutoMeshCollisionCreator.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Sean Watson of JackalBorn (Jackalborn) 
Contents [hide] 
1 Description 
2 Usage 
3 C# - BoundingBoxAdder.cs 
4 Credit 

Description Allows the creation of a Mesh Collider inside your 3D Package(3DMax tested) to be automatically added as an non rendering Mesh Collider to your main object upon import into Unity. 
Usage Setup includes creating 2 folders in your main Asset folder of your project, 1 called "Editor" and 1 called "environment" (the second can be changed in the script if needed). 
Place BoundingBoxAdder.cs inside the "Editor". 
Simpley create your Mesh Collider around your main object in your 3D Package and name it something with "_collider" in it and import to the "environment" folder! That should be it! 


C# - BoundingBoxAdder.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class BoundingBoxAdder : AssetPostprocessor {
	void OnPostprocessModel(GameObject g)
    {
        // filter out only animations.
        string lowerCaseAssetPath = assetPath.ToLower();
 
        if (lowerCaseAssetPath.IndexOf("/environment/") == -1)  //do this ONLY if we are in the ENVIRONMENT FOLDER, assets/environment/...
            return;
 
		Apply(g.transform);
 
	}
 
 
	// Add a mesh collider to each game object that contains collider in its name
	void Apply (Transform transform){
		if (transform.name.ToLower().Contains("collider")){
			transform.gameObject.AddComponent(typeof(MeshCollider));
 
			Object[] smr = transform.gameObject.GetComponentsInChildren(typeof(MeshRenderer), false);
			Object[] mfs = transform.gameObject.GetComponentsInChildren(typeof(MeshFilter), false);
 
			 foreach (MeshRenderer o in smr){
				Object.DestroyImmediate(o, true);
			}
			foreach (MeshFilter mf in mfs){
				Object.DestroyImmediate(mf, true);
			}
		}
 
		// Recurse
		foreach(Transform child in transform)
			Apply(child);
	}
}Credit Everyone in #unity3d and the Unity Script Reference page for giving me enough stuff to Frankenstein this tool together. And anyone that needs a hug. 
}
