// Original url: http://wiki.unity3d.com/index.php/ColliderCopier
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/ColliderCopier.cs
// File based on original modification date of: 15 February 2012, at 13:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
DescriptionA simple editor script to copy collider properties. 
UsageYou must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
After that, you find the new functionality in Custom->Collider Copier. 
C# - ColliderCopier.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class ColliderCopier : ScriptableObject
{
	private enum COLLIDER_TYPE {Sphere, Box, Capsule};
 
 	private static COLLIDER_TYPE colliderType;
 
	private	static string	myName; 
 
	// capsule
	private static Vector3 	capsuleCenter;
	private static float 	capsuleRadius;
	private	static float	capsuleHeight;
	private	static int		capsuleDirection;
 
	// sphere
	private	static Vector3	sphereCenter;
	private	static float	sphereRadius;
 
	// box
	private static Vector3	boxCenter;
	private	static Vector3	boxSize;
 
 
 
 
    [MenuItem ("Custom/Collider Copier/Copy Collider")]
    static void DoRecord()
    {
		if(Selection.activeTransform.GetComponent<SphereCollider>()!=null)
		{
			colliderType = COLLIDER_TYPE.Sphere;
			sphereCenter = Selection.activeTransform.GetComponent<SphereCollider>().center;
			sphereRadius = Selection.activeTransform.GetComponent<SphereCollider>().radius;
		}
 
		if(Selection.activeTransform.GetComponent<CapsuleCollider>()!=null)
		{
			colliderType = COLLIDER_TYPE.Capsule;
			capsuleCenter = Selection.activeTransform.GetComponent<CapsuleCollider>().center;
			capsuleRadius = Selection.activeTransform.GetComponent<CapsuleCollider>().radius;
			capsuleHeight = Selection.activeTransform.GetComponent<CapsuleCollider>().height;
			capsuleDirection = Selection.activeTransform.GetComponent<CapsuleCollider>().direction;
		}
 
		if(Selection.activeTransform.GetComponent<BoxCollider>()!=null)
		{
			colliderType = COLLIDER_TYPE.Box;
			boxCenter = Selection.activeTransform.GetComponent<BoxCollider>().center;
			boxSize = Selection.activeTransform.GetComponent<BoxCollider>().size;
		}
 
       myName = Selection.activeTransform.name;       
 
        EditorUtility.DisplayDialog("Collider Copy", "Local settings of " + myName + " collider copied", "OK", "");
    }
 
    [MenuItem ("Custom/Collider Copier/Paste Collider")]
    static void DoApply()
    {
       switch(colliderType)
		{
			case COLLIDER_TYPE.Sphere:
				if(Selection.activeTransform.GetComponent<SphereCollider>()==null)
				{
					EditorUtility.DisplayDialog("DANGER", "Can't paste onto that (not a sphere collider)", "Bah!");
					return;
				}
				Selection.activeTransform.GetComponent<SphereCollider>().center = sphereCenter;
				Selection.activeTransform.GetComponent<SphereCollider>().radius = sphereRadius;
				break;
			case COLLIDER_TYPE.Capsule:
				if(Selection.activeTransform.GetComponent<CapsuleCollider>()==null)
				{
					EditorUtility.DisplayDialog("DANGER", "Can't paste onto that (not a capsule collider)", "Bah!");
					return;
				}
				Selection.activeTransform.GetComponent<CapsuleCollider>().center = capsuleCenter;
				Selection.activeTransform.GetComponent<CapsuleCollider>().radius = capsuleRadius;
				Selection.activeTransform.GetComponent<CapsuleCollider>().height = capsuleHeight;
				Selection.activeTransform.GetComponent<CapsuleCollider>().direction = capsuleDirection;
				break;
			case COLLIDER_TYPE.Box:
				if(Selection.activeTransform.GetComponent<BoxCollider>()==null)
				{
					EditorUtility.DisplayDialog("DANGER", "Can't paste onto that (not a box collider)", "Bah!");
					return;
				}
				Selection.activeTransform.GetComponent<BoxCollider>().center = boxCenter;
				Selection.activeTransform.GetComponent<BoxCollider>().size = boxSize;
				break;
 
		}
        EditorUtility.DisplayDialog("Collider Copy", "Local settings of " + myName + " collider pasted", "OK", "");
    }
 
}
}
