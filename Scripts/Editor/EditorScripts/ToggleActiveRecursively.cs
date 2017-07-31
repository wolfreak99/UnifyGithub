// Original url: http://wiki.unity3d.com/index.php/ToggleActiveRecursively
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/ToggleActiveRecursively.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Original Author: Jonathan Czeck (aarku) 
Javascript version: Daniel Brauer 
Contents [hide] 
1 Description 
2 Usage 
3 C# - ToggleActiveRecursively.cs 
4 Javascript - ToggleActiveRecursively.js 

DescriptionThis editor script takes the current object selected in your hierarchy and toggles whether or not it is active. Then it sets the activeness of all its children to this. 
UsagePlace this script in YourProject/Assets/Editor and a menu item will automatically appear in the Custom menu after it is compiled.

The Javascript version places the menu item in the GameObject menu, and assigns it a shortcut of Command-T. The menu item will be disabled if nothing is selected. 
C# - ToggleActiveRecursively.csusing UnityEngine;
using UnityEditor;
 
public class ToggleActiveRecursively : ScriptableObject
{
    [MenuItem ("Custom/Toggle Active And Send Recursively %i")]
    static void DoToggle()
    {
        GameObject activeGO = Selection.activeGameObject;
 
        activeGO.SetActiveRecursively(!activeGO.active);
    }
}Javascript - ToggleActiveRecursively.js// Add menu item named "Toggle Active Recursively" to GameObject menu
@MenuItem ("GameObject/Toggle Active Recursively %t")
 
static function ToggleActiveRecursively() {
	for (var currentTransform : Transform in Selection.transforms)
		currentTransform.gameObject.SetActiveRecursively(!currentTransform.gameObject.active);
}
 
// The menu item will be disabled if no transform is selected.
@MenuItem ("GameObject/Toggle Active Recursively %t", true)
 
static function ValidateToggleActiveRecursively() : boolean {
	return Selection.activeTransform;
}
}
