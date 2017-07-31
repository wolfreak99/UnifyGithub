// Original url: http://wiki.unity3d.com/index.php/SnapToGrid
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/SnapToGrid.cs
// File based on original modification date of: 6 July 2014, at 10:20. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Hayden Peake (Hayden) 
Contents [hide] 
1 Description 
2 Usage 
3 C# - SnapToGrid.cs 
4 Javascript - SnapToGrid.js 

Description Snaps objects to a grid in 3 dimensions. The grid spacing may be different for each axis. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Select some objects in the Scene view or Hierarchy window, then choose GameObject→Snap to Grid from the menu (or press control G). Each selected object will be independently snapped to a unit grid. 
If you require a different grid spacing, change them in Edit→Snap Settings. 
C# - SnapToGrid.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class SnapToGrid : ScriptableObject {
 
	[MenuItem ("Window/Snap to Grid %g")]
	static void MenuSnapToGrid() {
		foreach (Transform t in Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable)) {
			t.position = new Vector3 (
				Mathf.Round(t.position.x / EditorPrefs.GetFloat("MoveSnapX")) * EditorPrefs.GetFloat("MoveSnapX"),
				Mathf.Round(t.position.y / EditorPrefs.GetFloat("MoveSnapY")) * EditorPrefs.GetFloat("MoveSnapY"),
				Mathf.Round(t.position.z / EditorPrefs.GetFloat("MoveSnapZ")) * EditorPrefs.GetFloat("MoveSnapZ")
			);
		}
	}
 
}Javascript - SnapToGrid.js @MenuItem ("GameObject/Snap to Grid %g")
static function MenuSnapToGrid() {
 
	for (var t : Transform in Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable)) {
		t.position = new Vector3 (
			Mathf.Round(t.position.x / EditorPrefs.GetFloat("MoveSnapX")) * EditorPrefs.GetFloat("MoveSnapX"),
			Mathf.Round(t.position.y / EditorPrefs.GetFloat("MoveSnapY")) * EditorPrefs.GetFloat("MoveSnapY"),
			Mathf.Round(t.position.z / EditorPrefs.GetFloat("MoveSnapZ")) * EditorPrefs.GetFloat("MoveSnapZ")
		);
	}
 
}
}
