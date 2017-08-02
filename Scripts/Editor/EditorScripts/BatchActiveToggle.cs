/*************************
 * Original url: http://wiki.unity3d.com/index.php/BatchActiveToggle
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/BatchActiveToggle.cs
 * File based on original modification date of: 27 January 2014, at 10:51. 
 *
 * Author: Desi Quintans (Desi) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Press Ctrl + Shift + A to toggle the active state of all selected game objects in the scene. 
    Usage Place BatchActiveToggle.cs in the Editor folder of your project. 
    Select any game object(s) and press Ctrl + Shift + A to toggle them active/inactive. To undo, press Ctrl + Shift + A again, or press Ctrl + Z. 
    C# - BatchActiveToggle.cs using UnityEditor;
    using UnityEngine;
     
    /*
    Batch Active Toggle
    http://wiki.unity3d.com/index.php/BatchActiveToggle
    Desi Quintans (CowfaceGames.com), 27 Jan 2014.
     
    Press Ctrl + Shift + A to toggle active status of selected game objects.
    This is a non-recursive activation toggle.
    */
     
    public class BatchActiveToggle : Editor
    {
    	[MenuItem("GameObject/Toggle Active %#a")]
    		static void BatchToggleActive ()
    		{
    			foreach (Transform t in Selection.transforms)
    			{
    				// foreach didn't like working on a GameObject array, so I have to get the transforms as the first step.
    				GameObject go = t.gameObject;
    				string undoText;
     
    				if (go.activeSelf)
    				{
    					undoText = "Deactivate";
    				}
    				else
    				{
    					undoText = "Activate";
    				}
     
    				Undo.RecordObject(go, undoText + " " + go.name);
    				go.SetActive (!go.activeSelf);
    				Debug.Log(undoText.TrimEnd('e') + "ing " + go.name + ".");
    			}
    		}
}
}
