/*************************
 * Original url: http://wiki.unity3d.com/index.php/BuildAssetBundlesFromDirectory
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/BuildAssetBundlesFromDirectory.cs
 * File based on original modification date of: 27 March 2014, at 00:09. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Author Brad Nelson (playemgames) 
    DescriptionThis script eases the pain of creating asset bundles from all files in a directory. Useful for textures or other simple files that don't need dependency tracking. 
    UsageDrop this script in the Editor folder in your Project and then right click on a folder and select "Build AssetBundles From Directory of Files - No dependency tracking". It will make all the files in that directory asset bundles. Use with Unity Pro only. This file and page can be edited and improved to suit the community better. Free for non-commercial and commercial use. 
    CSharp - BuildAssetBundlesFromDirectory.cs using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
     
    public class BuildAssetBundlesFromDirectory
    {
    	[@MenuItem("Assets/Build AssetBundles From Directory of Files - No dependency tracking")]
    	static void ExportAssetBundles () 
    	{
    		// Get the selected directory
    		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    		Debug.Log(path);
    		if (path.Length != 0)
    		{
    			path = path.Replace("Assets/", "");
    			Debug.Log(path);
    			string [] fileEntries = Directory.GetFiles(Application.dataPath+"/"+path);
    			foreach(string fileName in fileEntries)
    			{
    				Debug.Log(fileName);
    				string filePath = fileName.Replace("\\", "/");
    				int index = filePath.LastIndexOf("/");
    				filePath = filePath.Substring(index);
    				Debug.Log(filePath);
    				string localPath = "Assets/" + path;
    				// Debug.Log(localPath);
    				if (index > 0)
    					localPath += filePath;
    					Debug.Log(localPath);
    				Object t = AssetDatabase.LoadMainAssetAtPath(localPath);
    				if (t != null)
    				{
    					Debug.Log(t.name);
    					string bundlePath = "Assets/" + path + "/" + t.name + ".unity3d";
    					Debug.Log(bundlePath);
    					// Build the resource file from the active selection.
    					BuildPipeline.BuildAssetBundle(t, null, bundlePath, BuildAssetBundleOptions.CompleteAssets);
    				}
     
    			}
    		}
    	}
    }
}
