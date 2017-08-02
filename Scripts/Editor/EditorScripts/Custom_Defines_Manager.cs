/*************************
 * Original url: http://wiki.unity3d.com/index.php/Custom_Defines_Manager
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Custom_Defines_Manager.cs
 * File based on original modification date of: 24 January 2016, at 20:43. 
 *
 * 	Author: Jon Kenkel (nonathaj)
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Usage for Assets 
    4 Note about deleting the manager file 
    5 C# Script - AssetDefineManager.cs 
    
    Description Script for adding/removing #define's in Unity that are dependent on a file. When that file exists, this script will create the desired defines for it. When that file is removed, those defines will be removed. 
    Unity does not support standard #define's, but they can be added to each build type's player settings using editor scripts (http://docs.unity3d.com/Manual/PlatformDependentCompilation.html) to allow #if and #elif preprocessor directives for platform dependent code. Of course it is often more useful to create your own, so this script simplifies that process, especially for imported assets. 
    Usage Simply add a new AssetDefine object to the list of CustomDefines that the manager uses in your project. The AssetDefine requires a file to be dependent on, and at least 1 define for that dependency. You may define as many additional defines as you wish. Optionally, you may replace the null, with a list of the platforms you wish your define to work for 
    private static List<AssetDefine> CustomDefines = new List<AssetDefine>
    {
    	new AssetDefine("MyScriptThatOthersUse.cs", null, "MyScriptThatOthersUseDefine"),
    	new AssetDefine("AnotherAsset.unity", new BuildTargetGroup[] { BuildTargetGroup.Standalone }, "AnotherAsset"),
    	new AssetDefine("AssetDefineManager.cs", null, "AssetCustomDefine", "MyOtherDefine", "ThirdDefine", "FourthDefine", "andAnother"),
    };Usage for Assets It is recommended to to rename AssetDefineManager to something like MyAssetNameDefineManager, so that it does not interfere if another asset should use this script. 
    With an asset, it is easy to include this file along with your asset, then allow the manager to have the define based on the Manager existing. It will properly handle the creation of the defines when your asset is added, and the removal if your asset is deleted from the project. 
    Note about deleting the manager file If the manager has a define dependency file set as the manager file itself, it WILL properly handle it's own deletion. This is because Unity runs all AssetPostProcessors THEN recompiles scripts, meaning that the manager will properly remove the defines that depend on itself. 
    C# Script - AssetDefineManager.cs /*
    	Created: 1/23/2016
    */
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
     
    [InitializeOnLoad]
    public class AssetDefineManager : AssetPostprocessor
    {
    	/// <summary>
    	/// Custom defines to add based on the file to detect the asset by, and the desired platforms
    	/// </summary>
    	private static List<AssetDefine> CustomDefines = new List<AssetDefine>
    	{
    		//new AssetDefine("AssetDefineManager.cs", null, "AssetCustomDefine"),
    		new AssetDefine("AssetDefineManager.cs", null, "AssetCustomDefine", "MyOtherDefine"),
    	};
     
    	private struct AssetDefine
    	{
    		public readonly string assetDetectionFile;              //the file used to detect if the asset exists
    		public readonly string[] assetDefines;                  //series of defines for this asset
    		public readonly BuildTargetGroup[] definePlatforms;     //platform this define will be used for (null is all platforms)
     
    		public AssetDefine(string fileToDetectAsset, BuildTargetGroup[] platformsForDefine, params string[] definesForAsset)
    		{
    			assetDetectionFile = fileToDetectAsset;
    			definePlatforms = platformsForDefine;
    			assetDefines = definesForAsset;
    		}
     
    		public bool IsValid { get { return assetDetectionFile != null && assetDefines != null; } }
    		public static AssetDefine Invalid = new AssetDefine(null, null, null);
     
    		public void RemoveAllDefines()
    		{
    			foreach (string define in assetDefines)
    				RemoveCompileDefine(define, definePlatforms);
    		}
     
    		public void AddAllDefines()
    		{
    			foreach (string define in assetDefines)
    				AddCompileDefine(define, definePlatforms);
    		}
    	}
     
    	static AssetDefineManager()
    	{
    		ValidateDefines();
    	}
     
    	private static void ValidateDefines()
    	{
    		foreach(AssetDefine def in CustomDefines)
    		{
    			string[] fileCodes = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(def.assetDetectionFile));
    			foreach(string fileCode in fileCodes)
    			{
    				string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(fileCode));
    				if(fileName == def.assetDetectionFile)
    				{
    					if (def.IsValid)        //this is an asset we are tracking for defines
    						def.AddAllDefines();
    				}
    			}
    		}		
    	}
     
    	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    	{
    		foreach (string deletedFile in deletedAssets)
    		{
    			AssetDefine def = AssetDefine.Invalid;
             {
    				string file = Path.GetFileName(deletedFile);
    				foreach (AssetDefine define in CustomDefines)
    				{
    					if (define.assetDetectionFile == file)
    					{
    						def = define;
    						break;
    					}
    				}
    			}
     
    			if (def.IsValid)			//this is an asset we are tracking for defines
    				def.RemoveAllDefines();
    		}
    	}
     
    	/// <summary>
    	/// Attempts to add a new #define constant to the Player Settings
    	/// </summary>
    	/// <param name="newDefineCompileConstant">constant to attempt to define</param>
    	/// <param name="targetGroups">platforms to add this for (null will add to all platforms)</param>
    	public static void AddCompileDefine(string newDefineCompileConstant, BuildTargetGroup[] targetGroups = null)
    	{
    		if (targetGroups == null)
    			targetGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
     
    		foreach (BuildTargetGroup grp in targetGroups)
    		{
    			if (grp == BuildTargetGroup.Unknown)        //the unknown group does not have any constants location
    				continue;
     
    			string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(grp);
    			if (!defines.Contains(newDefineCompileConstant))
    			{
    				if (defines.Length > 0)         //if the list is empty, we don't need to append a semicolon first
    					defines += ";";
     
    				defines += newDefineCompileConstant;
    				PlayerSettings.SetScriptingDefineSymbolsForGroup(grp, defines);
    			}
    		}
    	}
     
    	/// <summary>
    	/// Attempts to remove a #define constant from the Player Settings
    	/// </summary>
    	/// <param name="defineCompileConstant"></param>
    	/// <param name="targetGroups"></param>
    	public static void RemoveCompileDefine(string defineCompileConstant, BuildTargetGroup[] targetGroups = null)
    	{
    		if (targetGroups == null)
    			targetGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
     
    		foreach (BuildTargetGroup grp in targetGroups)
    		{
    			string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(grp);
    			int index = defines.IndexOf(defineCompileConstant);
    			if (index < 0)
    				continue;           //this target does not contain the define
    			else if (index > 0)
    				index -= 1;         //include the semicolon before the define
    									//else we will remove the semicolon after the define
     
    			//Remove the word and it's semicolon, or just the word (if listed last in defines)
    			int lengthToRemove = Math.Min(defineCompileConstant.Length + 1, defines.Length - index);
     
    			//remove the constant and it's associated semicolon (if necessary)
    			defines = defines.Remove(index, lengthToRemove);
     
    			PlayerSettings.SetScriptingDefineSymbolsForGroup(grp, defines);
    		}
    	}
}
}
