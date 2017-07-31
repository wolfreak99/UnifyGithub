// Original url: http://wiki.unity3d.com/index.php/AddRemoveComponentRecursively
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/AddRemoveComponentRecursively.cs
// File based on original modification date of: 11 December 2012, at 16:26. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Toomas 
Description Inspired by AddComponentRecursively and DeleteComponentsInChildren editor scripts, this version provides a more solid implementation. 
Usage Copy it under /Assets/Editor folder and access from GameObject menu. 
AddRemoveComponentsRecursively.cs using UnityEditor;
using UnityEngine;
using System.Collections;
 
/**
 * Adds/removes components by name to selected game object and its children (no duplicates are added)
 *
 * For this to work, copy it under Editor folder. You can access the functionality
 * from "GameObject/Add or remove components recursivly..." menu.
 *
 * @author toomas008
 */
public class AddRemoveComponentsRecursively : ScriptableWizard {
 
	public string componentType = null;
 
	// disabled menu item when no Transform is selected
	[MenuItem ("GameObject/Add or remove components recursively...", true)]
	static bool CreateWindowDisabled() {
		return Selection.activeTransform;
	}
 
	// menu item
	[MenuItem ("GameObject/Add or remove components recursively...")]
	static void CreateWindow()
	{
		ScriptableWizard.DisplayWizard<AddRemoveComponentsRecursively>(
			"Add or remove components recursivly",
			"Add", "Remove");
	}
 
	void OnWizardUpdate()
	{
		helpString = "Note: Duplicates are not created";
 
		if(string.IsNullOrEmpty(componentType)) {
			errorString = "Please enter component class name";
			isValid = false;
		} else {
			errorString = "";
			isValid = true;    
		}
	}
 
	// adding
	void OnWizardCreate()
	{
		int c = 0;
		Transform[] ts = Selection.GetTransforms(SelectionMode.Deep);
		foreach(Transform t in ts)
		{
			if(t.gameObject.GetComponent(componentType) == null) {
				if(t.gameObject.AddComponent(componentType) == null) {
					Debug.LogWarning("Component of type " + componentType + " does not exist");
					return;
				}
				c++;
			}
		}
		Debug.Log("Added " + c + " components of type " + componentType);
	}
 
	// removing
	void OnWizardOtherButton()
	{
		int c = 0;
		Transform[] ts = Selection.GetTransforms(SelectionMode.Deep);
		foreach(Transform t in ts)
		{
			if(t.GetComponent(componentType) != null)
			{
				DestroyImmediate(t.GetComponent(componentType));
				c++;
			}
		}
		Debug.Log("Removed " + c + " components of type " + componentType);
		Close();
	}
}
}
