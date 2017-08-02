/*************************
 * Original url: http://wiki.unity3d.com/index.php/Object_Lock_Window
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/Object_Lock_Window.cs
 * File based on original modification date of: 1 June 2013, at 15:06. 
 *
 * Author: ChemiKhazi 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    This editor window simply helps set the HideFlags of game objects to NotEditable and optionally lets you ignore selecting NotEditable objects in the scene. Useful if you have objects you don't want to edit in the scene but still want visible. Requires EditorGUIExtension. 
    Installation Save EditorGUIExtension.cs and then save the script below to ObjectLockWindow.cs and put them in an Editor directory. 
    Usage instructions The window appears under the menu "Window>Object Lock". Pressing "Lock Selection" locks any selected objects and objects in its hierarchy. "Lock Selected Only" will only lock the selected object but not the objects under it. Toggle "Ignore Locked Objects" to ensure you won't select locked objects in the scene view. 
    ObjectLockWindow.cs using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
     
    public class ObjectLockWindow : EditorWindow
    {
    	[MenuItem ("Window/Object Lock")]
    	static void Init()
    	{
    		ObjectLockWindow lockWin = (ObjectLockWindow) EditorWindow.GetWindow(typeof(ObjectLockWindow));
    		lockWin.title = "Object Lock";
    	}
     
    	bool doIgnoreLocked = false;
     
    	void OnEnable()
    	{
    		doIgnoreLocked = false;
    	}
     
    	void OnSelectionChange()
    	{
    		Repaint();
    	}
     
    	void OnGUI()
    	{
    		GUILayout.BeginHorizontal();
     
    		string hierarchyText = "Lock Selection";
    		string soloText = "Lock Selected Only";
     
    		bool selectionLocked = ObjectIsLocked(Selection.activeGameObject);
     
    		if (selectionLocked)
    		{
    			hierarchyText = "Unlock Selection";
    			soloText = "Unlock Selected Only";
    		}
     
    		bool doHierarchy = GUILayout.Button(hierarchyText);
    		bool doSolo = GUILayout.Button(soloText);
     
    		GUILayout.EndHorizontal();
     
    		doIgnoreLocked = EditorGUIExtension.ToggleButton(doIgnoreLocked, "Ignore Locked Objects");
     
    		if (Selection.activeGameObject != null)
    		{
    			if (doHierarchy || doSolo)
    			{
    				foreach (GameObject targetObject in Selection.gameObjects)
    				{
    					if (selectionLocked)
    						UnlockObject(targetObject, doHierarchy);
    					else
    						LockObject(targetObject, doHierarchy);
    				}
    			}
    		}
    	}
     
    	void Update()
    	{
    		if (Selection.activeGameObject != null && doIgnoreLocked)
    		{
    			List<Object> newSelectedObjects = new List<Object>();
     
    			foreach (Object testObject in Selection.objects)
    			{
    				bool doAdd = true;
     
    				// Check if selected object is a game object and then if it is locked
    				if (testObject is GameObject)
    				{
    					if (ObjectIsLocked(testObject as GameObject))
    						doAdd = false;
    				}
     
    				if (doAdd)
    					newSelectedObjects.Add(testObject);
    			}
     
    			Selection.objects = newSelectedObjects.ToArray();
    		}
    	}
     
    	void LockObject(GameObject targetObject, bool recursive)
    	{
    		targetObject.hideFlags = targetObject.hideFlags | HideFlags.NotEditable;
    		if (recursive)
    		{
    			foreach (Transform child in targetObject.transform)
    				LockObject(child.gameObject, true);
    		}
    	}
     
    	void UnlockObject(GameObject targetObject, bool recursive)
    	{
    		targetObject.hideFlags = targetObject.hideFlags & ~HideFlags.NotEditable;
    		if (recursive)
    		{
    			foreach (Transform child in targetObject.transform)
    				UnlockObject(child.gameObject, true);
    		}
    	}
     
    	bool ObjectIsLocked(GameObject testObject)
    	{
    		if (testObject == null)
    			return false;
    		return ((int) testObject.hideFlags & (int)HideFlags.NotEditable) != 0;
    	}
}
}
