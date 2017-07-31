// Original url: http://wiki.unity3d.com/index.php/Create_project_directories
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/Create_project_directories.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Almar (Almar) 
Description This pretty straightforward script will create quite some number of directories under your Assets folder. This is the default structure I use when creating new projects. It can also serve as an example to create your own "startup script" for defining your own directory structure. 
Usage Create a file "MakeFolders.cs" in the Assets\Editor directory (create it, it does not exist by default) and paste the code below. A new item will appear at the "Assets" menu. This code has not been tested at the Mac version of Unity. 
Code using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
 
/// Store this code as "MakeFolders.cs" in the Assets\Editor directory (create it, if it does not exist)
/// Creates a number of directories for storage of various content types.
/// Modify as you see fit.
/// Directories are created in the Assets dir.
/// Not tested on a Mac.
 
 
public class MakeFolders : ScriptableObject
{
 
    [MenuItem ( "Assets/Make Project Folders" )]
    static void MenuMakeFolders()
    {
		CreateFolders();
	}
 
	static void CreateFolders()
	{
		string f = Application.dataPath + "/";
 
		Directory.CreateDirectory(f + "Meshes");
		Directory.CreateDirectory(f + "Fonts");
		Directory.CreateDirectory(f + "Plugins");
		Directory.CreateDirectory(f + "Textures");
		Directory.CreateDirectory(f + "Materials");
		Directory.CreateDirectory(f + "Physics");
		Directory.CreateDirectory(f + "Resources");
		Directory.CreateDirectory(f + "Scenes");
		Directory.CreateDirectory(f + "Music");
		Directory.CreateDirectory(f + "Packages");
		Directory.CreateDirectory(f + "Scripts");
		Directory.CreateDirectory(f + "Shaders");
		Directory.CreateDirectory(f + "Sounds");
 
		Debug.Log("Created directories");
	}
}
}
