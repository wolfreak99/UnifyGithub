/*************************
 * Original url: http://wiki.unity3d.com/index.php/AssetPathPrinter
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/AssetPathPrinter.cs
 * File based on original modification date of: 10 January 2012, at 20:44. 
 *
 * Author: Dave Buchhofer 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    DescriptionSimple editor script that will copy the path to any selected project pane item to the clipboard, and also print to the console. 
    UsagePlace this script in YourProject/Assets/Editor and the menu items will appear in Custom \ Selection \ Print Asset Location, currently it is set to use a hotkey Control-J. 
    C# - AssetLocPrinter.cs using UnityEngine;
    using UnityEditor;
     
    public class AssetLocPrinter : ScriptableObject {
    	[MenuItem ("Custom/Selection/Print Asset Location %j")]
    	static void CopyAssetLocations() {
    		string path=string.Empty;
    		foreach (Object o in Selection.objects)
    		{
    			path += "\n" + AssetDatabase.GetAssetPath(o);
    		}
    		Debug.Log(path);
    		EditorGUIUtility.systemCopyBuffer = path;
        }
}
}
