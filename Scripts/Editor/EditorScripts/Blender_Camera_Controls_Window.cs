/*************************
 * Original url: http://wiki.unity3d.com/index.php/Blender_Camera_Controls_Window
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Blender_Camera_Controls_Window.cs
 * File based on original modification date of: 8 December 2013, at 16:41. 
 *
 * Author: Marc Kusters / Will Traxler 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    DescriptionThis is a small extension to the Blender Camera Controls script authored by Marc Kusters. His script allows for the use of Blender camera shortcut keys to quickly center and rotate the editor camera. One issue with this script is that a game object must be selected for the camera controls to function. 
    The script below creates a Unity editor window titled 'Cam Control Window'. As long as this window is open, the camera control script will function even if no object is selected. The camera control window is small and can be docked out of the way. 
    Usage1. Save the script to a folder titled 'Editor'.
    2. From the editor, go to the Window > CamControl Window menu
    3. A small window titled CamControl will appear
    4. Dock this window where it is out of the way and convenient
    5. Use the blender keypad controls to manipulate the editor camera (see original script for controls)
    using UnityEditor;
    using UnityEngine;
     
    public class CamControl : EditorWindow {
    	/*
    	 * This an extension of Marc Kusters BlenderCameraControls script found here:
    	 * 
    	 * http://wiki.unity3d.com/index.php/Blender_Camera_Controls
    	 */
     
     
    	private static bool isEnabled 	= true;
     
    	[MenuItem("Window/" + "CamControl Window")]
    	public static void Init() {
    		CamControl window = GetWindow<CamControl>();
    		window.title = "CamControl";
    		window.minSize = new Vector2(10,10);
    	}	
     
    	public void OnEnable() {
    		SceneView.onSceneGUIDelegate += OnScene;
    	}
     
    	public void OnGUI() {
     
    		GUILayoutOption[] options = {GUILayout.MinWidth(5) };
     
    		if(GUILayout.Button("Close", options))
    			GetWindow<CamControl>().Close();
     
    		//Enable or disable button
    		if (isEnabled) {
    			if(GUILayout.Button("Enabled", options))
    				isEnabled = false;
    		} else {
    			if (GUILayout.Button("Disabled", options))
    				isEnabled = true;
    		}
    	}
     
    	private static void OnScene(SceneView sceneview) {
     
    		if (!isEnabled) return;
     
    		UnityEditor.SceneView sceneView;
    		Vector3 eulerAngles;
         	Event current;
         	Quaternion rotHelper;
     
            current = Event.current;
     
            if (!current.isKey || current.type != EventType.keyDown)
                return;
     
            sceneView = UnityEditor.SceneView.lastActiveSceneView;
     
            eulerAngles = sceneView.camera.transform.rotation.eulerAngles;
            rotHelper = sceneView.camera.transform.rotation;
     
            switch (current.keyCode) {
                case KeyCode.Keypad1:
                    if (current.control == false)
                        sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 360f, 0f)));
                    else
                        sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 180f, 0f)));
                    break;
                case KeyCode.Keypad2:
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, rotHelper * Quaternion.Euler(new Vector3(-15f, 0f, 0f)));
                    break;
                case KeyCode.Keypad3:
                    if (current.control == false)
                        sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 270f, 0f)));
                    else
                        sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
                    break;
                case KeyCode.Keypad4:
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + 15f, eulerAngles.z)));
                    break;
                case KeyCode.Keypad5:
                    sceneView.orthographic = !sceneView.orthographic;
                    break;
                case KeyCode.Keypad6:
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y - 15f, eulerAngles.z)));
                    break;
                case KeyCode.Keypad7:
                    if (current.control == false)
                        sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(90f, 0f, 0f)));
                    else
                        sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(270f, 0f, 0f)));
                    break;
                case KeyCode.Keypad8:
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, rotHelper * Quaternion.Euler(new Vector3(15f, 0f, 0f)));
                    break;
                case KeyCode.KeypadPeriod:
                    if (Selection.transforms.Length == 1)
                        sceneView.LookAtDirect(Selection.activeTransform.position, sceneView.camera.transform.rotation);
                    else if (Selection.transforms.Length > 1) {
                        Vector3 tempVec = new Vector3();
                        for (int i = 0; i < Selection.transforms.Length; i++) {
                            tempVec += Selection.transforms[i].position;
                        }
                        sceneView.LookAtDirect((tempVec / Selection.transforms.Length), sceneView.camera.transform.rotation);
                    }
                    break;
                case KeyCode.KeypadMinus:
                    SceneView.RepaintAll();
                    sceneView.size *= 1.1f;
                    break;
                case KeyCode.KeypadPlus:
                    SceneView.RepaintAll();
                    sceneView.size /= 1.1f;
                    break;
            }
     
    	}
     
    	public void OnDestroy() {
    		SceneView.onSceneGUIDelegate -= OnScene;
    	}
     
}
}
