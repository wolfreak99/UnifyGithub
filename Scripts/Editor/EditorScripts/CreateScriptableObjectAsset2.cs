// Original url: http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset2
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/CreateScriptableObjectAsset2.cs
// File based on original modification date of: 16 October 2014, at 14:25. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Lea Hayes 
Contents [hide] 
1 Description 
2 Screenshot 
3 Warning 
4 Usage 
4.1 Assets/Editor/ScriptableObjectUtility2.cs 
4.2 Assets/Editor/YourUnityIntegration.cs 
4.3 Assets/YourScriptableObject.cs 
5 See also 

Description This page demonstrates an alternative method of adding a menu item which can be used to create an asset file for a ScriptableObject type class. As with the original topic the asset is uniquely named and placed in the currently selected project path but better mimics the way in which Unity's built-in assets are created since the asset is instantly in "rename" mode. 
This topic is an extension of CreateScriptableObjectAsset by Brandon Edmark. 
Screenshot  
Warning This implementation makes use of the non-documented method ProjectWindowUtil.CreateAsset and was created by way of trial-and-error but seems to work. Use this at your own risk. 
Usage 1. Copy the following editor script into the path "Assets\Editor\ScriptableObjectUtility2.cs". 
2. You can then use this utility script when adding new items to the "Assets/Create" menu. 
Assets/Editor/ScriptableObjectUtility2.cs using UnityEngine;
using UnityEditor;
 
public static class ScriptableObjectUtility2 {
 
	/// <summary>
	/// Create new asset from <see cref="ScriptableObject"/> type with unique name at
	/// selected folder in project window. Asset creation can be cancelled by pressing
	/// escape key when asset is initially being named.
	/// </summary>
	/// <typeparam name="T">Type of scriptable object.</typeparam>
	public static void CreateAsset<T>() where T : ScriptableObject {
		var asset = ScriptableObject.CreateInstance<T>();
		ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
	}
 
}Assets/Editor/YourUnityIntegration.cs using UnityEngine;
using UnityEditor;
 
static class YourUnityIntegration {
 
	[MenuItem("Assets/Create/YourScriptableObject")]
	public static void CreateYourScriptableObject() {
		ScriptableObjectUtility2.CreateAsset<YourScriptableObject>();
	}
 
}Assets/YourScriptableObject.cs using UnityEngine;
 
public class YourScriptableObject : ScriptableObject {
 
}See alsoCreateScriptableObjectAsset 
}
