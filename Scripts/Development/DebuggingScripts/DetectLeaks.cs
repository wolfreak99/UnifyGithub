/*************************
 * Original url: http://wiki.unity3d.com/index.php/DetectLeaks
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/DebuggingScripts/DetectLeaks.cs
 * File based on original modification date of: 15 March 2014, at 21:34. 
 *
 * Author: Joachim Ante, Berenger, Juha 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Development.DebuggingScripts
{
    Contents [hide] 
    1 Description 
    2 C# - DetectLeaks V1.0 
    3 C# - DetectLeaks V1.1 
    4 C# - DetectLeaks V2 
    5 C# - DetectLeaks V3 
    
    Description This script will displays the number of alloctated unity objects by type. This is useful for finding leaks. Knowing the type of object (mesh, texture, sound clip, game object) that is getting leaked is the first step. You could then print the names of all leaked assets of that type. 
    
    
    C# - DetectLeaks V1.0 The script _must_ be named DetectLeaks.cs 
    using UnityEngine;
    using System.Collections;
     
    public class DetectLeaks : MonoBehaviour {
     
    	void OnGUI () {
    		GUILayout.Label("All " + FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
    		GUILayout.Label("Textures " + FindObjectsOfTypeAll(typeof(Texture)).Length);
    		GUILayout.Label("AudioClips " + FindObjectsOfTypeAll(typeof(AudioClip)).Length);
    		GUILayout.Label("Meshes " + FindObjectsOfTypeAll(typeof(Mesh)).Length);
    		GUILayout.Label("Materials " + FindObjectsOfTypeAll(typeof(Material)).Length);
    		GUILayout.Label("GameObjects " + FindObjectsOfTypeAll(typeof(GameObject)).Length);
    		GUILayout.Label("Components " + FindObjectsOfTypeAll(typeof(Component)).Length);
    	}
    }C# - DetectLeaks V1.1 FindObjectsOfTypeAll is obsolete. New version from Berenger on 07/03/2012 (Unity 3.5) 
    using UnityEngine;
    using System.Collections;
     
    public class DetectLeaks : MonoBehaviour 
    {
        void OnGUI () {
            GUILayout.Label("All " + FindObjectsOfType(typeof(UnityEngine.Object)).Length);
            GUILayout.Label("Textures " + FindObjectsOfType(typeof(Texture)).Length);
            GUILayout.Label("AudioClips " + FindObjectsOfType(typeof(AudioClip)).Length);
            GUILayout.Label("Meshes " + FindObjectsOfType(typeof(Mesh)).Length);
            GUILayout.Label("Materials " + FindObjectsOfType(typeof(Material)).Length);
            GUILayout.Label("GameObjects " + FindObjectsOfType(typeof(GameObject)).Length);
            GUILayout.Label("Components " + FindObjectsOfType(typeof(Component)).Length);
        }
    }C# - DetectLeaks V2 The upper one isnt working correctly... Here is the actual Solution -> http://unity3d.com/support/documentation/ScriptReference/Resources.FindObjectsOfTypeAll 
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
     
    public class DetectLeaks : MonoBehaviour
    {
    	void OnGUI()
    	{
    		Object[] objects = FindObjectsOfType(typeof (UnityEngine.Object));
     
    		Dictionary<string, int> dictionary = new Dictionary<string, int>();
     
    		foreach(Object obj in objects)
    		{
    			string key = obj.GetType().ToString();
    			if(dictionary.ContainsKey(key))
    			{
    				dictionary[key]++;
    			} 
    			else
    			{
    				dictionary[key] = 1;
    			}
    		}
     
    		List<KeyValuePair<string, int>> myList = new List<KeyValuePair<string, int>>(dictionary);
    		myList.Sort(
    			delegate(KeyValuePair<string, int> firstPair,
    			KeyValuePair<string, int> nextPair)
    				{
    					return nextPair.Value.CompareTo((firstPair.Value));
    				}
    		);
     
    		foreach (KeyValuePair<string, int> entry in myList)
    		{
    			GUILayout.Label(entry.Key + ": " + entry.Value);
    		}
     
    	}
    }Slightly modified version to show every type of object and ordered (descending) by count. -Juha 
    C# - DetectLeaks V3 Brand new Editor version, kinda inspired by Juha's one. -Berenger 
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Linq;
     
    public class DetectLeaksWindow : EditorWindow
    {
        private Vector2 _scroll;
     
        // To have rich text
        private GUIStyle _missingLabelStyle;
        private GUIStyle _missingFoldoutStyle;
     
        private bool _sceneFoldout = false;
        private bool _projectFoldout = false;
     
        private List<GameObject> _sceneList = null;
        private List<GameObject> _projectList = null;
     
        // To know the color of the foldout label
        private bool _sceneHasMissingScripts = false;
        private bool _projectHasMissingScripts = false;
     
        // Add menu named "DetectLeaks Window" to the Window menu
        [MenuItem("Window/DetectLeaks Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            DetectLeaksWindow window = (DetectLeaksWindow)EditorWindow.GetWindow(typeof(DetectLeaksWindow));
            window.name = "DetectLeaks";
        }
     
        void OnGUI()
        {
            if (GUILayout.Button("SCAN", GUILayout.Height(30f)))
                Scan();
     
     
            _scroll = GUILayout.BeginScrollView(_scroll);
            {
                _missingLabelStyle = _missingLabelStyle ?? new GUIStyle(GUI.skin.label);
                _missingLabelStyle.richText = true;
                _missingFoldoutStyle = _missingFoldoutStyle ?? new GUIStyle(EditorStyles.foldout);
                _missingFoldoutStyle.richText = true;
     
                if ((_sceneList != null && _sceneList.Count > 0) || (_projectList != null && _projectList.Count > 0))
                {
                    _sceneFoldout = EditorGUILayout.Foldout(_sceneFoldout, "<color=" + (_sceneHasMissingScripts ? "red" : "green") + ">Scene GameObjects</color>", _missingFoldoutStyle);
                    if (_sceneFoldout)
                        DrawGameObjectList(_sceneList);
     
                    _projectFoldout = EditorGUILayout.Foldout(_projectFoldout, "<color=" + (_projectHasMissingScripts ? "red" : "green") + ">Project GameObjects</color>", _missingFoldoutStyle);
                    if (_projectFoldout)
                        DrawGameObjectList(_projectList);
                }
                else
                {
                    GUILayout.Label("Hit scan");
                }
            }
            GUILayout.EndScrollView();
        }
     
        /// <summary>
        /// Scan for every game object inside the scene AND the project folder
        /// </summary>
        private void Scan()
        {
            _sceneList = (FindObjectsOfType(typeof(GameObject)) as GameObject[]).ToList();
            _sceneHasMissingScripts = false;
            foreach( GameObject go in _sceneList )
            {
                if( CheckForMissingScripts(go) )
                {
                    _sceneHasMissingScripts = true;
                    continue;
                }
            }
     
            _projectList = new List<GameObject>();
            LoadAllPrefabs(ref _projectList, Application.dataPath);
            _projectHasMissingScripts = false;
            foreach (GameObject go in _projectList)
            {
                if (CheckForMissingScripts(go))
                {
                    _projectHasMissingScripts = true;
                    continue;
                }
            }
        }
     
        /// <summary>
        /// Load every prefabs recursively from the asset folder
        /// </summary>
        /// <param name="prefabs">List of prefab being updated by the recursive function</param>
        /// <param name="path">Current path</param>
        private void LoadAllPrefabs(ref List<GameObject> prefabs, string path)
        {
            string[] directories = System.IO.Directory.GetDirectories(path);
            foreach (string directorie in directories)
                LoadAllPrefabs(ref prefabs, directorie);
     
            path = path.Replace('\\', '/');
            path = "Assets" + path.Substring(Application.dataPath.Length) + "/";
            string[] assetsPath = System.IO.Directory.GetFiles(path, "*.prefab");
            foreach (string assetPath in assetsPath)
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                if (asset != null && PrefabUtility.GetPrefabType(asset) != PrefabType.None)
                    prefabs.Add(asset);
            }
        }
     
        /// <summary>
        /// Display the list of game objects inside the window vertically. If a component is missing, 
        /// the label [Missing script] is drawn in red in front of it.
        /// </summary>
        /// <param name="list">List of game object to display</param>
        private void DrawGameObjectList(List<GameObject> list)
        {
            if (list != null)
            {
                GameObject destroyGO = null;
     
                foreach (GameObject obj in list)
                {
                    if (obj == null)
                        continue;
     
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(20f);
     
                        bool isMissing = CheckForMissingScripts(obj);
                        if (isMissing)
                            GUILayout.Label("<color=red>[Missing script]</color>", _missingLabelStyle, GUILayout.ExpandWidth(false));
     
                        // Selectable, read-only field
                        EditorGUILayout.ObjectField(obj, obj.GetType(), GUILayout.ExpandWidth(true));
     
                        if (isMissing)
                        {
                            // Delete obj, but only after the loop
                            if (GUILayout.Button(new GUIContent("X", "Delete"), GUILayout.Width(20f)))
                            {
                                if (EditorUtility.DisplayDialog("Exterminate !", "Are you sure you want to delete " + obj.name + " ?", "Yes", "No"))
                                    destroyGO = obj;
                            }
     
                            // Select obj in hierarchy
                            if (GUILayout.Button(new GUIContent("S", "Select"), GUILayout.Width(20f)))
                                Selection.activeGameObject = obj;
     
                            // TODO
                            if (GUILayout.Button(new GUIContent("C", "Clear missing scripts"), GUILayout.Width(20f)))
                            {
                                if (!EditorUtility.DisplayDialog("You wish", "Ha ! you thought it would be that easy didn't ya ? You'll have to remove it yourself pal !", "Ok :'(", "More info on UA"))
                                    Application.OpenURL("http://answers.unity3d.com/questions/15225/how-do-i-remove-null-components-ie-missingmono-scr.html");
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
     
     
                }
     
                if( destroyGO != null )
                    GameObject.DestroyImmediate(destroyGO);
            }
        }
     
        /// <summary>
        /// Check if a component as a missing script
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if there is at least one missing component</returns>
        private bool CheckForMissingScripts(GameObject obj)
        {
            if (obj == null)
                return false;
     
            Component[] components = obj.GetComponents<Component>();
            return components != null ? components.Any(c => c == null) : false;
     
        }
}
}
