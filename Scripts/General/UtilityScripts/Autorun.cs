// Original url: http://wiki.unity3d.com/index.php/Autorun
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/Autorun.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Save this script as Autorun.cs in your Editor folder: 
using UnityEngine;
using UnityEditor;
// from the excellent http://answers.unity3d.com/questions/45186/can-i-auto-run-a-script-when-editor-launches-or-a.html
 
///
/// This must be added to "Editor" folder: http://unity3d.com/support/documentation/ScriptReference/index.Script_compilation_28Advanced29.html
/// Execute some code exactly once, whenever the project is opened, recompiled, or run.
///
[InitializeOnLoad]
public class Autorun
{
    static Autorun()
    {
        EditorApplication.update += RunOnce;
    }
 
    static void RunOnce()
    {
        Debug.Log("RunOnce!");
        EditorApplication.update -= RunOnce;
 
		// do something here. You could open an EditorWindow, for example.
    }
}
}
