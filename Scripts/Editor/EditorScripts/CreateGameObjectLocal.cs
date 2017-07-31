// Original url: http://wiki.unity3d.com/index.php/CreateGameObjectLocal
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CreateGameObjectLocal.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Antony Blackett 
Description Adds an empty GameObject to the currently active GameObject (GameObject in the inspector). If no GameObject is selected then it will be added to the scene like the Create Empty command. If creating the new object breaks a prefab then you will be warned like normal. 
Undo works as you'd expect. 
There is a keyboard shortcut as well Alt + Shift + N. 
Usage Place the script inside a folder named Editor. e.g. Assets/Editor 
Select an object in the scene and select GameObject -> Create Empty Local or use the shortcut Alt + Shift + N. 
Enjoy! 
C# - CreateGameObjectLocal.cs using UnityEngine;
using UnityEditor;
 
public class CreateGameObjectLocal : Editor
{
	[MenuItem ("GameObject/Create Empty Local #&n")]
	static void CreateEmptyLocal()
	{
		if(Selection.activeTransform != null)
		{
 
			// Check if the selected object is a prefab instance and display a warning
			PrefabType type = EditorUtility.GetPrefabType( Selection.activeGameObject );
			if(type == PrefabType.PrefabInstance)
			{
				if(!EditorUtility.DisplayDialog("Losing prefab", 
				                               "This action will lose the prefab connection. Are you sure you wish to continue?", 
				                               "Continue", "Cancel"))
				{
					return; // The user does not want to break the prefab connection so do nothing.
				}
			}
		}
 
		// Make this action undoable
		Undo.RegisterSceneUndo("Create Empty Local");
 
		// Create our new GameObject
		GameObject newGameObject = new GameObject();
		newGameObject.name = "GameObject";
 
		// If there is a selected object in the scene then make the new object its child.
		if(Selection.activeTransform != null)
		{
			newGameObject.transform.parent = Selection.activeTransform;
			newGameObject.name = Selection.activeTransform.gameObject.name + "Child";
 
			// Place the new GameObject at the same position as the parent.
			newGameObject.transform.localPosition = Vector3.zero;
			newGameObject.transform.localRotation = Quaternion.identity;
			newGameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
		}
 
		// Select our newly created GameObject
		Selection.activeTransform = newGameObject.transform;
	}
}
}
