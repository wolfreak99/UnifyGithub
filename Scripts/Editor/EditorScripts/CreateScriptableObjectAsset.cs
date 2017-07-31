// Original url: http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CreateScriptableObjectAsset.cs
// File based on original modification date of: 13 October 2014, at 12:55. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Brandon Edmark 
Contents [hide] 
1 Description 
2 Usage 
2.1 ScriptableObjectUtility.cs 
2.2 YourClassAsset.cs 
3 See also 

Description This is a method to easily create a new asset file instance of a ScriptableObject-derived class. The asset is uniquely named and placed in the currently selected project path; this mimics the way Unity's built-in assets are created. 
ScriptableObject asset files are useful for storing data that doesn't fit naturally within the MonoBehaviour/GameObject Prefab system. Since they use Unity's built-in serialization, they are guaranteed to map perfectly to an existing class; therefore, ScriptableObject assets are much easier to work with in Unity than XML, CSV, or other traditional ways of storing such data. 
Usage Copy ScriptableObjectUtility.cs into your project. Then create a class like YourClassAsset.cs, replacing YourClass with the name of your ScriptableObject-inheriting class, and place it in an Editor folder. You will now be able to create a uniquely named YourClass asset file from the Asset menu. 
ScriptableObjectUtility.cs using UnityEngine;
using UnityEditor;
using System.IO;
 
public static class ScriptableObjectUtility
{
	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
 
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
 
		AssetDatabase.CreateAsset (asset, assetPathAndName);
 
		AssetDatabase.SaveAssets ();
        	AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}YourClassAsset.cs using UnityEngine;
using UnityEditor;
 
public class YourClassAsset
{
	[MenuItem("Assets/Create/YourClass")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<YourClass> ();
	}
}See alsoCreateScriptableObjectAsset2 
}
