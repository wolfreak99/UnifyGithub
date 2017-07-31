// Original url: http://wiki.unity3d.com/index.php/SkinnedMeshTools
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MeshHelpers/SkinnedMeshTools.cs
// File based on original modification date of: 3 April 2012, at 14:39. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
Author: Bérenger, from masterprompt's code. 
Description Copy one skinnedmesh renderer onto another with same bones. They bones must have the same names. The point is to have only one set of bones and one animation component for a changing number of skinned mesh renderer. Think swappable equipment pieces. 
See the discussion here : http://forum.unity3d.com/threads/16485-quot-stitch-multiple-body-parts-into-one-character-quot 
Usage Use the function AddSkinnedMeshTo. The first parameter is the root of the object containing the animation component, the bones and the skinned mesh renderer you want to add to the main object. The second parameter is the transform of that main object. The last one determine if SetActiveRecursively(false) is used on the first parameter or not. The return value is a list of all the gameobjects added to the main object. 
C# - SkinnedMeshTools.cs using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public static class SkinnedMeshTools
{
	// Return the list of all the new skinned mesh renderer added to root. Set recursively obj as inactive.
	public static List<GameObject> AddSkinnedMeshTo( GameObject obj, Transform root ){ return AddSkinnedMeshTo(obj, root, true); }
	// Return the list of all the new skinned mesh renderer added to root. Set recursively obj as inactive if hideFromObj is true.
	public static List<GameObject> AddSkinnedMeshTo( GameObject obj, Transform root, bool hideFromObj )
	{
		List<GameObject> result = new List<GameObject>();
 
		// Here, boneObj must be instatiated and active (at least the one with the renderer),
		// or else GetComponentsInChildren won't work.
		SkinnedMeshRenderer[] BonedObjects = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach( SkinnedMeshRenderer smr in BonedObjects )
			result.Add( ProcessBonedObject( smr, root ) );
 
		if( hideFromObj )
			obj.SetActiveRecursively( false );
 
		return result;
	}
 
	private static GameObject ProcessBonedObject( SkinnedMeshRenderer ThisRenderer, Transform root )
	{		
	    // Create the SubObject
		GameObject newObject = new GameObject( ThisRenderer.gameObject.name );	
	    newObject.transform.parent = root;
 
	    // Add the renderer
	    SkinnedMeshRenderer NewRenderer = newObject.AddComponent( typeof( SkinnedMeshRenderer ) ) as SkinnedMeshRenderer;
 
	    // Assemble Bone Structure	
	    Transform[] MyBones = new Transform[ ThisRenderer.bones.Length ];
 
		// As clips are using bones by their names, we find them that way.
	    for( int i = 0; i < ThisRenderer.bones.Length; i++ )
	        MyBones[ i ] = FindChildByName( ThisRenderer.bones[ i ].name, root );
 
	    // Assemble Renderer	
	    NewRenderer.bones = MyBones;	
	    NewRenderer.sharedMesh = ThisRenderer.sharedMesh;	
	    NewRenderer.materials = ThisRenderer.materials;
 
		return newObject;
	}
 
	// Recursive search of the child by name.
	private static Transform FindChildByName( string ThisName, Transform ThisGObj )	
	{	
	    Transform ReturnObj;
 
		// If the name match, we're return it
	    if( ThisGObj.name == ThisName )	
	        return ThisGObj.transform;
 
		// Else, we go continue the search horizontaly and verticaly
	    foreach( Transform child in ThisGObj )	
	    {	
	        ReturnObj = FindChildByName( ThisName, child );
 
	        if( ReturnObj != null )	
	            return ReturnObj;	
	    }
 
	    return null;	
	}
}
}
