/*************************
 * Original url: http://wiki.unity3d.com/index.php/MoveToOrigin
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/MoveToOrigin.cs
 * File based on original modification date of: 18 January 2014, at 23:55. 
 *
 * Author: Matthew Miner (matthew@matthewminer.com) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Moves the selected GameObject(s) to (0, 0, 0). 
    Usage Place the script inside the Editor folder. Select one or more GameObjects and choose GameObject > Move To Origin or press cmd-0 (Mac) / ctrl-0 (Windows). This is functionally equivalent to invoking Reset on a GameObject's transform. 
    C# - MoveToOrigin.cs using UnityEditor;
    using UnityEngine;
     
    class MoveToOrigin {
    	/// <summary>
    	/// Moves the selected game object(s) to (0, 0, 0).
    	/// <summary>
    	/// <remarks>Keyboard shortcut: cmd-0 (Mac), ctrl-0 (Windows).</remarks>
    	[MenuItem ("GameObject/Move To Origin %0")]
    	static void MenuMoveToOrigin () {
    		// Move each selected transform to (0, 0, 0)
    		foreach (Transform t in Selection.transforms) {
    			Undo.RecordObject(t, "Move " + t.name);
    			t.position = Vector3.zero;
    			Debug.Log("Moving " + t.name + " to origin");
    		}
        }
     
    	/// <summary>
    	/// Validates the "Move To Origin" menu item.
    	/// </summary>
    	/// <remarks>The menu item will be disabled if no transform is selected.</remarks>
    	[MenuItem ("GameObject/Move To Origin %0", true)]
    	static bool ValidateMoveToOrigin () {
    		return Selection.activeTransform != null;
    	}
}
}
