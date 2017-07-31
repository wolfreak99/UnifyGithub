// Original url: http://wiki.unity3d.com/index.php/MultipleObjectsToLayer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/MultipleObjectsToLayer.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
By: Sommer. 
Description Editor Utility that lets you move multiple selected objects into a layer at once. 
Put this script into your project in the Editor folder. You will get the menu option Tools->Move selection to layer. Alternatively you can press CMD+ALT+L (CTRL+ALT+L on Windows) to open the window. 
Code using UnityEngine;
using UnityEditor;
public class MultipleObjectsToLayer : EditorWindow {
 
	static int selection = 0;
	static bool includeChildren = true;
 
    [MenuItem ("Tools/Move selection to layer %&l")]
    public static void Run () 
	{
 
		if (Selection.gameObjects.Length > 0)
		{
        		GetWindow (typeof (MultipleObjectsToLayer)).Show ();
		}
 
    }
 
    void OnGUI () {
        GUILayout.Label ("Move selection to layer", EditorStyles.boldLabel);
 
		includeChildren = GUILayout.Toggle(includeChildren, "Include children");
 
		selection = EditorGUILayout.LayerField(selection);
 
		if (GUILayout.Button("Move to layer"))
		{
			MoveSelectionToLayer();
		}
    }
 
	static void MoveSelectionToLayer()
	{
		Object[] selectedObjects;
 
		if (includeChildren)
		{
			selectedObjects = Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep);
		}
		else
		{
			selectedObjects = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
		}
 
		foreach (GameObject go in selectedObjects)
		{
			go.layer = selection;
		}
	}
}
}
