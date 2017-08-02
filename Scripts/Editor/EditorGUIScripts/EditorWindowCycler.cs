/*************************
 * Original url: http://wiki.unity3d.com/index.php/EditorWindowCycler
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/EditorWindowCycler.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System;
     
    /// <summary>
    /// I hear that it has been a longstanding request for the Window menu shortcuts (Command + 0-9)
    /// cycle through all open windows of the indicated type one by one. This script does that,
    /// using the same shortcuts with the Alt/Option key added.
    ///
    /// by Matt "Trip" Maker, Monstrous Company :: http://monstro.us
    /// 
    /// from http://unifycommunity.com/wiki/index.php?title=EditorWindowCycler
    /// 
    /// 
    /// </summary>
    public class EditorWindowCycler {
     
    	static Dictionary<string, EditorWindow> lastWindow = new Dictionary<string, EditorWindow>();
     
    	static void openDefaultEditorWindow(string windowType) {
            switch(windowType)
            {
            case "UnityEditor.SceneView":
    			EditorApplication.ExecuteMenuItem("Window/Scene");
                break; 
            case "UnityEditor.GameView":
    			EditorApplication.ExecuteMenuItem("Window/Game");
                break; 
            case "UnityEditor.InspectorWindow":
    			EditorApplication.ExecuteMenuItem("Window/Inspector");
                break; 
    		case "UnityEditor.HierarchyWindow":
    			EditorApplication.ExecuteMenuItem("Window/Hierarchy");
    			break;
            case "UnityEditor.ProjectWindow":
    			EditorApplication.ExecuteMenuItem("Window/Project");	//see also EditorUtility.FocusProjectWindow();
    			break;
            case "UnityEditor.AnimationWindow":
    			EditorApplication.ExecuteMenuItem("Window/Animation");
    			break;
    		case "UnityEditor.ConsoleWindow":
    			EditorApplication.ExecuteMenuItem("Window/Console");
    			break;
    		default: 
                break; 
            }		
    	}
     
    	static bool focusNextEditorWindowType(string windowType) {
    		EditorWindow last; 
    		lastWindow.TryGetValue(windowType, out last);
    		EditorWindow next = getNextWindow(last);
    		if (!next) {
    			//in the case where no windows of this type are open, use Unity's standard menu commands to open one and try again.
    			//this is also fine in the case where we are just starting out and don't know what the last focused windows were.
    			openDefaultEditorWindow(windowType);
    			//EditorWindow.FocusWindowIfItsOpen(Type.GetType(type));//if we didn't want to open a new one, we could use this.
    			//this fails (type is too far removed I guess: EditorWindow.GetWindow(Type.GetType(windowType));
    			next = getFirstEditorWindowOfType(windowType);
    		}
    		if (next) {
    			next.Focus();
    			lastWindow[windowType] = next;
    			Debug.Log("WindowCycler Focusing " + windowType + " " + next.GetInstanceID());
    		}
    		return (next != last);
    	}
     
    	//TODO validation functions that make sure at least two such windows are open.
    	//nah, that would be expensive...
     
    	[MenuItem("Window/Next Scene &%1", priority = 1044)]
    	static void focusNextScene() {
    		focusNextEditorWindowType("UnityEditor.SceneView");
    	}
     
    	[MenuItem("Window/Next Game &%2", priority = 1044)]
    	static void focusNextGame() {
    		focusNextEditorWindowType("UnityEditor.GameView");
    	}
     
    	[MenuItem("Window/Next Inspector &%3", priority = 1044)]
    	static void focusNextInspector3() {
    		focusNextEditorWindowType("UnityEditor.InspectorWindow");
    	}
     
    	[MenuItem("Window/Next Heirarchy &%4", priority = 1044)]
    	static void focusNextHeirarchy() {
    		focusNextEditorWindowType("UnityEditor.HierarchyWindow");
    	}
     
    	[MenuItem("Window/Next Project &%5", priority = 1044)]
    	static void focusNextProject() {
    		focusNextEditorWindowType("UnityEditor.ProjectWindow");
    	}
     
    	[MenuItem("Window/Next Animation &%6", priority = 1044)]
    	static void focusNextAnimation() {
    		focusNextEditorWindowType("UnityEditor.AnimationWindow");
    	}
     
    	static EditorWindow getNextWindow(EditorWindow prev) {
    		if (!prev) return null;
    		IEnumerable<EditorWindow> wins;
    		wins = getEditorWindowsOfType(prev);
    		//add a second copy so we "loop around"
    		return wins.Concat(wins).SkipWhile(x => x != prev).Skip(1).FirstOrDefault();
    	}
     
    	static EditorWindow getFirstEditorWindowOfType(string type) {
    		return getEditorWindowsOfType(type).FirstOrDefault();
    	}
     
    	static IEnumerable<EditorWindow> getEditorWindowsOfType(EditorWindow win) {
    		return getEditorWindowsOfType(win.GetType().ToString());
    	}
    	static IEnumerable<EditorWindow> getEditorWindowsOfType(string type) {
    		return (Resources.FindObjectsOfTypeAll(typeof(EditorWindow)) as EditorWindow[]).Where(x => x.GetType().ToString() == type);
    	}
     
    }
}
