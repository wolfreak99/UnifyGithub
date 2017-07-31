// Original url: http://wiki.unity3d.com/index.php/HierarchySelectObject
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/HierarchySelectObject.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Martin Schultz (MartinSchultz) 
Description Editor script that automatically selects and scrolls to a specific gameObject in Unity's hierarchy window. This is very useful if you have a large scale project with lots of objects in the hierarchy window and you want to quickly jump to a specific object you often need to jump to. This script scrolls automatically the hierarchy window to your object, pings it (gets highlighted in yellow) and gets also automatically selected so you can edit it in the inspector. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Define a gameObject from the hierarchy window that you often need, for example a "RaceManager" object or an "Boss Enemy". 
JavaScript - SelectMyObject.js @MenuItem ("Tools/Select My Object")
static function SelectMyObject() {
	var obj = GameObject.Find("your object here from the hierarchy window");
	EditorGUIUtility.PingObject(obj);
	Selection.activeGameObject = obj;
}
 
 
// if you want to find a specific object identified by its class, like when an object has a RaceManager.js script attached to it.
@MenuItem ("Tools/Select My Specific Object")
static function SelectMySpecificObject() {
	var obj = GameObject.FindObjectOfType(RaceManager);
	EditorGUIUtility.PingObject(obj);
	Selection.activeGameObject = obj.gameObject;
}
}
