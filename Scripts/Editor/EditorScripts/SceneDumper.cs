// Original url: http://wiki.unity3d.com/index.php/SceneDumper
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/SceneDumper.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Contents [hide] 
1 Description 
2 Usage 
3 Future Work 
4 C# Script - SceneDumper.cs 

Description This script dumps the scene hierarchy to a text file. It dumps all game object recursively, starting with the selected objects. The text dump contains object names, hierarchy, and the names of all components. 
This is useful for things like: 
quick review of your scene structure 
comparing the state of the scene from day to day or milestone to milestone 
comparing two scenes 
comparing the scene from before and during playback to see what objects and components are created dynamically 
finding missing scripts in your scene (search the dump for "(null)") 
For a more full-featured version of this script, including a custom editor window for better IDE integration, see http://zeroandone.ca/unity. 
Usage Save it as "SceneDumper.cs" below your project's Editor folder. You will now have a Debug menu containing a "Dump Scene" menu item. Select one or more objects in your scene hierarchy and select the menu item. 
Future Work Potential useful extensions to this script include: 
a GUI for selecting what to dump 
control over location of output file 
use of System.Reflection to dump the fields and properties of components 
C# Script - SceneDumper.cs // SceneDumper.cs
//
// History:
// version 1.0 - December 2010 - Yossarian King
 
using StreamWriter = System.IO.StreamWriter;
 
using UnityEngine;
using UnityEditor;
 
public static class SceneDumper
{
    [MenuItem("Debug/Dump Scene")]
    public static void DumpScene()
    {
        if ((Selection.gameObjects == null) || (Selection.gameObjects.Length == 0))
        {
            Debug.LogError("Please select the object(s) you wish to dump.");
            return;
        }
 
        string filename = @"C:\unity-scene.txt";
 
        Debug.Log("Dumping scene to " + filename + " ...");
        using (StreamWriter writer = new StreamWriter(filename, false))
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                DumpGameObject(gameObject, writer, "");
            }
        }
        Debug.Log("Scene dumped to " + filename);
    }
 
    private static void DumpGameObject(GameObject gameObject, StreamWriter writer, string indent)
    {
        writer.WriteLine("{0}+{1}", indent, gameObject.name);
 
        foreach (Component component in gameObject.GetComponents<Component>())
        {
            DumpComponent(component, writer, indent + "  ");
        }
 
        foreach (Transform child in gameObject.transform)
        {
            DumpGameObject(child.gameObject, writer, indent + "  ");
        }
    }
 
    private static void DumpComponent(Component component, StreamWriter writer, string indent)
    {
        writer.WriteLine("{0}{1}", indent, (component == null ? "(null)" : component.GetType().Name));
    }
}
}
