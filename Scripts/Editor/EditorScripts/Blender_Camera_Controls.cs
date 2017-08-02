/*************************
 * Original url: http://wiki.unity3d.com/index.php/Blender_Camera_Controls
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Blender_Camera_Controls.cs
 * File based on original modification date of: 27 January 2014, at 15:15. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Contents [hide] 
    1 Author 
    2 Description 
    3 Usage 
    4 UnityScript - BlenderCameraControls.cs 
    5 Back to editor scripts 
    
    AuthorMarc Kusters (Nighteyes) 
    DescriptionThis adds the default Blender numpad camera controls to Unity. Initial idea from [1]. 
    Note 1: Just so you know, this script changes the Transform inspector to show rotation in quaternions instead of Euler offsets. 
    Note 2: For an improved version of this script checkout Blender Camera Controls Window by Will Traxler. 
    UsageTo start using this script just place it in de editor folder of your project. It is prerequisite to select any object, otherwise the script won't run (this I will hope to fix someday). 
    Numpad1 = Front view
    Control + Numpad1 = Rear view 
    Numpad2 = Rotate view down
    Numpad3 = Right view
    Control + Numpad3 = Left view 
    Numpad4 = Rotate view left
    Numpad5 = Switch between orthographic and perspective
    Numpad6 = Rotate view right
    Numpad7 = Top view
    Control + Numpad7 = Down view
    Numpad8 = Move view up
    Numpad. = Center view on object(s) Note:only works on objects that have the CanEditMultipleObjects property
    Numpad- = Zoom camera out
    Numpad+ = Zoom camera in
    UnityScript - BlenderCameraControls.cs// BlenderCameraControls.cs
    // by Marc Kusters (Nighteyes)
    //
    // Usage: Select any object to use the camera hotkeys. 
    //
     
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
     
    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    public class BlenderCameraControls : Editor
    {
        UnityEditor.SceneView sceneView;
     
        private Vector3 eulerAngles;
        private Event current;
        private Quaternion rotHelper;
     
        public void OnSceneGUI()
        {
     
            current = Event.current;
     
            if (!current.isKey || current.type != EventType.keyDown)
                return;
     
            sceneView = UnityEditor.SceneView.lastActiveSceneView;
            eulerAngles = sceneView.camera.transform.rotation.eulerAngles;
            rotHelper = sceneView.camera.transform.rotation;
     
            switch (current.keyCode)
            {
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
                    else if (Selection.transforms.Length > 1)
                    {
                        Vector3 tempVec = new Vector3();
                        for (int i = 0; i < Selection.transforms.Length; i++)
                        {
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
    }Back to editor scripts
}
