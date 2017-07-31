// Original url: http://wiki.unity3d.com/index.php/GetSize
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/GetSize.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Charles Hinshaw 
DescriptionThis editor script adds the option to get the dimensions of an object's renderer in game units. 
C# - GetSize.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class GetSize : ScriptableObject {
	private static Quaternion rotation;
	private static Vector3 size;
	private static GameObject go;
	[MenuItem ("GameObject/Get Renderer Size")]
	static void MenuGetSize(){
		rotation = Selection.activeTransform.localRotation;
		Selection.activeTransform.localRotation = Quaternion.identity;
		if (Selection.activeTransform.gameObject.renderer != null){
			size = Selection.activeTransform.gameObject.renderer.bounds.size;
			Selection.activeTransform.localRotation = rotation;
			EditorUtility.DisplayDialog("Object Scale", "X: "+size.x+", Y: "+size.y+", Z: "+size.z, "OK", "");
		} else {
			EditorUtility.DisplayDialog("Oops", "There is no renderer available on this object.", "OK", "");	
		}
	}
}
}
