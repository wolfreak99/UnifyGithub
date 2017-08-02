/*************************
 * Original url: http://wiki.unity3d.com/index.php/DeleteMyPlayerPrefs
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/DeleteMyPlayerPrefs.cs
 * File based on original modification date of: 20 January 2013, at 00:41. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description This script adds a menu item that, when selected, deletes the playerprefs within the editor. We found ourselves doing this a lot when working with various ways to unlock levels in our game as the player progressed..... so we made a simple editor script to make it much easier to delete them. 
    
    
    DeleteMyPlayerPrefs.cs using UnityEditor;
    using UnityEngine;
    using System.Collections;
     
    public class DeleteMyPlayerPrefs : MonoBehaviour {
     
    	[MenuItem("Tools/DeleteMyPlayerPrefs")] 
    	static void DeleteMyPlayerPrefs() { 
    		PlayerPrefs.DeleteAll();
    	} 
    }
}
