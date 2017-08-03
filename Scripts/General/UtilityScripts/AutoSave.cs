/*************************
 * Original url: http://wiki.unity3d.com/index.php/AutoSave
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/AutoSave.cs
 * File based on original modification date of: 10 January 2012, at 20:44. 
 *
 * Author: Melle Heeres 
 *
 * Description 
 *   This script creates a new window in the editor with a autosave function. It is saving your current scene with an interval from 1 minute to 10 minutes. 
 * Usage 
 *   Create a new script called AutoSave.cs in the folder: Assets/Editor. Activate autosave via window > autosave. 
 *   This script is tested, but usage is at your own risk. 
 * Update 
 *   Update 19-04: After some testing i discovered a problem with the assets saving, so remove the 
 *   rule: EditorApplication.SaveAssets(); at line 52 
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    using UnityEngine;
    using UnityEditor;
    using System;
     
    public class AutoSave : EditorWindow {
     
    	private bool autoSaveScene = true;
    	private bool showMessage = true;
    	private bool isStarted = false;
    	private int intervalScene;	
    	private DateTime lastSaveTimeScene = DateTime.Now;
     
    	private string projectPath = Application.dataPath;
    	private string scenePath;
     
    	[MenuItem ("Window/AutoSave")]
    	static void Init () {
    		AutoSave saveWindow = (AutoSave)EditorWindow.GetWindow (typeof (AutoSave));
    		saveWindow.Show();
    	}
     
    	void OnGUI () {	
    		GUILayout.Label ("Info:", EditorStyles.boldLabel);
    		EditorGUILayout.LabelField ("Saving to:", ""+projectPath);
    		EditorGUILayout.LabelField ("Saving scene:", ""+scenePath);
    		GUILayout.Label ("Options:", EditorStyles.boldLabel);
    		autoSaveScene = EditorGUILayout.BeginToggleGroup ("Auto save", autoSaveScene);
    		intervalScene = EditorGUILayout.IntSlider ("Interval (minutes)", intervalScene, 1, 10);
    		if(isStarted) {
    			EditorGUILayout.LabelField ("Last save:", ""+lastSaveTimeScene);
    		}
    		EditorGUILayout.EndToggleGroup();
    		showMessage = EditorGUILayout.BeginToggleGroup ("Show Message", showMessage);
    		EditorGUILayout.EndToggleGroup ();
    	}
     
     
    	void Update(){
    		scenePath = EditorApplication.currentScene;
    		if(autoSaveScene) {
    			if(DateTime.Now.Minute >= (lastSaveTimeScene.Minute+intervalScene) || DateTime.Now.Minute == 59 && DateTime.Now.Second == 59){
    				saveScene();
    			}
    		} else {
    			isStarted = false;
    		}
     
    	}
     
    	void saveScene() {
    		EditorApplication.SaveScene(scenePath);
    		lastSaveTimeScene = DateTime.Now;
    		isStarted = true;
    		if(showMessage){
    			Debug.Log("AutoSave saved: "+scenePath+" on "+lastSaveTimeScene);
    		}
    		AutoSave repaintSaveWindow = (AutoSave)EditorWindow.GetWindow (typeof (AutoSave));
    		repaintSaveWindow.Repaint();
    	}
}
}
