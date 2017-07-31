// Original url: http://wiki.unity3d.com/index.php/OpenVisualStudioProject
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/OpenVisualStudioProject.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Description A simple script that will open the current visual studio project, useful when combined with the Classexec trick from Setting_up_Visual_Studio_for_Unity to open the visual studio file created by the unity 2.6+ Assets/Sync VisualStudio Project 


Usage Create a file "OpenVisualStudioProject.cs" in the Assets\Editor directory (create it, it does not exist by default) and paste the code below. A new item will appear at the "Assets" menu. This code has not been tested at the Mac version of Unity. 


Code using UnityEngine;
using UnityEditor;
using System.IO;
 
public class OpenVisualStudioProject : ScriptableObject {
    [MenuItem("Assets/Open Visual Studio Project")]
    static void MenuLaunchProject()
    {
        LaunchProject();
    }
    static void LaunchProject()
    {
        string dir = Directory.GetCurrentDirectory();
        string path2 = dir + "/" +Path.GetFileNameWithoutExtension(dir) + ".csproj";
        FileInfo info = new FileInfo(path2);
        System.Diagnostics.Process.Start(info.FullName);
    }
}
}
