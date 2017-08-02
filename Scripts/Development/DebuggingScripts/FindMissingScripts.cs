/*************************
 * Original url: http://wiki.unity3d.com/index.php/FindMissingScripts
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/DebuggingScripts/FindMissingScripts.cs
 * File based on original modification date of: 1 November 2013, at 00:00. 
 *
 * Author: SimTex with slight tweaks by Clement 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Development.DebuggingScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 C# - FindMissingScripts.cs 
    4 C# - FindMissingScriptsRecursively.cs 
    
    Description Allows you to add a search a project for all instances of missing mono script. 
    Usage In the editor, a script that has been assigned to an object, but subsequently deleted has the string "Missing (Mono Script)" where the script class/filename should be. 
    It is possible to search a project to find all missing scripts using this editor script. To use it, save the file as "assets/editor/FindMissingScripts.cs". Note that it is important to save it into the editor directory. 
    Next for each level in your unity project, run the script. It's located under the window menu. 
    C# - FindMissingScripts.cs using UnityEngine;
    using UnityEditor;
    public class FindMissingScripts : EditorWindow
    {
        [MenuItem("Window/FindMissingScripts")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FindMissingScripts));
        }
     
        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected prefabs"))
            {
                FindInSelected();
            }
        }
        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            int go_count = 0, components_count = 0, missing_count = 0;
            foreach (GameObject g in go)
            {
                go_count++;
                Component[] components = g.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    components_count++;
                    if (components[i] == null)
                    {
                        missing_count++;
                        string s = g.name;
                        Transform t = g.transform;
                        while (t.parent != null) 
                        {
                            s = t.parent.name +"/"+s;
                            t = t.parent;
                        }
                        Debug.Log (s + " has an empty script attached in position: " + i, g);
                    }
                }
            }
     
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }
    }
    
    C# - FindMissingScriptsRecursively.cs Just added recursive feature so that children GO's (and their components, etc) will be searched as well, not just children components. -mlc 
    using UnityEngine;
    using UnityEditor;
    public class FindMissingScriptsRecursively : EditorWindow 
    {
        static int go_count = 0, components_count = 0, missing_count = 0;
     
        [MenuItem("Window/FindMissingScriptsRecursively")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
        }
     
        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }
        }
        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            go_count = 0;
    		components_count = 0;
    		missing_count = 0;
            foreach (GameObject g in go)
            {
       			FindInGO(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }
     
        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null) 
                    {
                        s = t.parent.name +"/"+s;
                        t = t.parent;
                    }
                    Debug.Log (s + " has an empty script attached in position: " + i, g);
                }
            }
            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.name  + " " );
                FindInGO(childT.gameObject);
            }
        }
}
}
