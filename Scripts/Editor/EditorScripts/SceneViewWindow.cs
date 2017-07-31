// Original url: http://wiki.unity3d.com/index.php/SceneViewWindow
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SceneViewWindow.cs
// File based on original modification date of: 1 July 2013, at 23:00. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Kevin_Tarchenski 
Description A simple Editor window to quickly switch between build scenes. If you use a lot of different scenes in your build, it can get annoying constantly hunting around for them in the Project View. This script enumerates all the scenes in a build and allows you to switch between them with a single click. 
 
Usage Place SceneViewWindow.cs in your Assets/Editor folder. 
Open via Window > Scene View, then dock and enjoy! 
C# - SceneViewWindow.cs using System.IO;
using UnityEditor;
using UnityEngine;
 
/// <summary>
/// SceneViewWindow class.
/// </summary>
public class SceneViewWindow : EditorWindow
{
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;
 
    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("Window/Scene View")]
    internal static void Init()
    {
        // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
        // instance if it can't find one. The second parameter is a flag for creating the window as a
        // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
        var window = (SceneViewWindow)GetWindow(typeof(SceneViewWindow), false, "Scene View");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }
 
    /// <summary>
    /// Called on GUI events.
    /// </summary>
    internal void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);
 
        GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);
        for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            if (scene.enabled)
            {
                var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                var pressed = GUILayout.Button(i + ": " + sceneName, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });
                if (pressed)
                {
                    if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
                    {
                        EditorApplication.OpenScene(scene.path);
                    }
                }
            }
        }
 
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
}
