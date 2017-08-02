/*************************
 * Original url: http://wiki.unity3d.com/index.php/CreatePrefabFromSelected
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CreatePrefabFromSelected.cs
 * File based on original modification date of: 27 April 2015, at 13:03. 
 *
 * Author: Matthew Miner (matthew@matthewminer.com) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 To Do 
    4 C# - CreatePrefabFromSelected.cs 
    5 C# - Updated CreatePrefabFromSelected.cs 
    6 C# - CreatePrefabFromSelected.cs Procedural Mesh Version 
    
    Description Creates a prefab containing the contents of the currently selected game object. 
    Usage Place the script inside the Editor folder. Select a GameObject and choose 'GameObject > Create Prefab From Selection'. A new prefab will be created in the root directory bearing the same name as the GameObject. 
    Faults: 
    To Do Currently the script replaces any existing prefab with the same name; more desirable would be to append a number to the filename of the new prefab 
    The selected GameObject should become an instance of the newly created prefab 
    C# - CreatePrefabFromSelected.cs using UnityEditor;
    using UnityEngine;
     
    class CreatePrefabFromSelected
    {
    	const string menuTitle = "GameObject/Create Prefab From Selected";
     
    	/// <summary>
    	/// Creates a prefab from the selected game object.
    	/// </summary>
    	[MenuItem (menuTitle)]
    	static void CreatePrefab ()
    	{
    		GameObject obj = Selection.activeGameObject;
    		string name = obj.name;
     
    		Object prefab = EditorUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
    		EditorUtility.ReplacePrefab(obj, prefab);
    		AssetDatabase.Refresh();
    	}
     
    	/// <summary>
    	/// Validates the menu.
    	/// </summary>
    	/// <remarks>The item will be disabled if no game object is selected.</remarks>
    	[MenuItem (menuTitle, true)]
    	static bool ValidateCreatePrefab ()
    	{
    		return Selection.activeGameObject != null;
    	}
    }C# - Updated CreatePrefabFromSelected.cs I decided to update this script in can anybody is intewrested. It now will give a popup if the prefab already exits and you can choose to overwrite the prefab or cancel. It also replaces the selected gameObject with the new prefab. 
    Cameron Bonde 
    - Updated to accept multiple selected objects and not use /prefabs folder (without it it deletes the GO without creating prefab) 
    - Fixed the script to make sure the new object have the same parent as the old one in the scene. --Blikstad 06:11, 20 May 2012 (PDT) 
    - Extended with choice of save folder by winxalex 
    using UnityEditor;
    using UnityEngine;
    using System.Collections;
     
    class CreatePrefabFromSelected : ScriptableObject
    {
        const string menuTitle = "GameObject/Create Prefab From Selected";
     
        /// <summary>
        /// Creates a prefab from the selected game object.
        /// </summary>
        [MenuItem(menuTitle)]
        static void CreatePrefab()
        {
                 var objs = Selection.gameObjects;
     
    			string pathBase = EditorUtility.SaveFolderPanel ("Choose save folder", "Assets", "");
     
    			if (!String.IsNullOrEmpty (pathBase)) {
     
    				pathBase=pathBase.Remove(0,pathBase.IndexOf("Assets"))+Path.DirectorySeparatorChar;
     
    								foreach (var go in objs) {
    										String localPath = pathBase + go.name + ".prefab";
     
    										if (AssetDatabase.LoadAssetAtPath (localPath, typeof(GameObject))) {
    												if (EditorUtility.DisplayDialog ("Are you sure?", 
    						                                "The prefab already exists. Do you want to overwrite it?", 
    						                                "Yes", 
    						                                "No"))
    														CreateNew (go, localPath);
    										} else
    												CreateNew (go, localPath);
    								}
    						}
        }
     
        static void createNew(GameObject obj, string localPath)
        {
            Object prefab = PrefabUtility.CreatePrefab (localPath, obj);
            EditorUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
        }
     
        /// <summary>
        /// Validates the menu.
        /// </summary>
        /// <remarks>The item will be disabled if no game object is selected.</remarks>
        [MenuItem(menuTitle, true)]
        static bool ValidateCreatePrefab()
        {
            return Selection.activeGameObject != null;
        }
    }C# - CreatePrefabFromSelected.cs Procedural Mesh VersionSmall update for procedural mesh creations: it saves the created mesh to disk and inside a prefab, instead of losing it, and keeps all prefab size, shader, collider, and script settings in 1 prefab. 
    Requires that you have a folder called Assets/savedMesh 
    
    
    using UnityEditor;
    using UnityEngine;
     
    /// <summary>
    /// Saves mesh and entire prefab from gameview, your procedural mesh prefab is saved.
    /// </summary>
    class CreatePrefabFromSelected
    {
    	const string menuName = "GameObject/Create Prefab From Selected";
     
    	/// <summary>
    	/// Adds a menu named "Create Prefab From Selected" to the GameObject menu.
    	/// </summary>
    	[MenuItem(menuName)]
    	static void CreatePrefabMenu ()
    	{
    		var go = Selection.activeGameObject;
     
    		Mesh m1 = go.GetComponent<MeshFilter>().mesh;//update line1
    		AssetDatabase.CreateAsset(m1, "Assets/savedMesh/" + go.name +"_M" + ".asset"); // update line2
     
     
    		var prefab = EditorUtility.CreateEmptyPrefab("Assets/savedMesh/" + go.name + ".prefab");
    		EditorUtility.ReplacePrefab(go, prefab);
    		AssetDatabase.Refresh();
    	}
     
    	/// <summary>
    	/// Validates the menu.
    	/// The item will be disabled if no game object is selected.
    	/// </summary>
    	/// <returns>True if the menu item is valid.</returns>
    	[MenuItem(menuName, true)]
    	static bool ValidateCreatePrefabMenu ()
    	{
    		return Selection.activeGameObject != null;
    	}
}
}
