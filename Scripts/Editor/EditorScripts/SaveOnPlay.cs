/*************************
 * Original url: http://wiki.unity3d.com/index.php/SaveOnPlay
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SaveOnPlay.cs
 * File based on original modification date of: 9 December 2015, at 01:34. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Paste the script into a file named SaveOnPlay.cs under a folder named Editor, e.g. Assets/Editor/SaveOnPlay.cs . Click play, the debug message appearing before your scene runs is a sign the save worked. 
    C# - SaveOnPlay.cs using UnityEditor;
    using UnityEngine;
     
    [InitializeOnLoad]
    public class SaveOnPlay
    {
        static SaveOnPlay()
        {
            EditorApplication.playmodeStateChanged += SaveCurrentScene;
        }
     
        static void SaveCurrentScene()
        {
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
    #if UNITY_5_3
                Debug.Log("Saving open scenes on play...");
                UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
    #else
                    Debug.Log("Saving scene on play...");
                    EditorApplication.SaveScene();
    #endif
            }
        }
    }
}
