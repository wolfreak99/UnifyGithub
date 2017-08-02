/*************************
 * Original url: http://wiki.unity3d.com/index.php/SelectObjectsIteratively
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SelectObjectsIteratively.cs
 * File based on original modification date of: 1 February 2014, at 16:24. 
 *
 * Author: Erdin Kacan (Nidre) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Iterates Over The Selected Prefabs/All Prefabs In Project Folder and Select each one of them.Waits 1 UnityEditor frame between each select to let Unity render the GUI and update the Components if necessary.After selecting all the Prefabs script Applies all prefabs and if Project mode is selected removes all of the created instances from the Hierarchy. 
    Beaware : I have created this code piece for my Project and i am not planning to Support it.(I might update it as i develop it further if my project requires so.) 
    Beaware 2 : I haven't really optimized it so this is not the most optimized way of doing that(But worked well for my needs).It might contain bugs but nothing major. 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
    Navigate to "Custom/Nidre/Select Objects Iter. In Scene" from the Unity Menu Bar to Iterate over the selected GameObject(s)(Prefabs).[This might also work work non Prefab objects but did'nt tried.Try at your own risk.] 
    Navigate to "Custom/Nidre/Select Objects Iter. In Project" from the Unity Menu Bar to Iterate over all All Prefabs in the project folder. 
    
    
    C# - SelectObjectsIteratively.cs //Author : Erdin Kacan (Nidre)
    //Webpage : http://erdinkacan.tumblr.com/
     
    //SelectObjectsIteratively by Erdin Kacan (Nidre) is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
    //To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/3.0/.
     
    using UnityEngine;
    using System.Collections;
    using UnityEditor;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
     
    [InitializeOnLoad]
    public class SelectObjectsIteratively : MonoBehaviour
    {
        static List<Object> _prefabObjects = new List<Object>();
        static List<Object> _prefabIntances = new List<Object>();
        static List<Object> _objectsToBeSelected = new List<Object>();
        static Object[] _initialSelection;
        //static int _totalSteps;
        //static int _stepsCompleted;
        static float _percentage;
        static bool _isApplying;
        static EditorApplication.CallbackFunction s_backgroundUpdateCB;
        static bool _deleteObjectAfter = false;
        static bool _skipApply;
        static Mode _mode;
        enum Mode
        {
            Scene,
            Project
        }
     
        struct Steps
        {
            internal int _totalSteps;
            internal int _stepsCompleted;
            internal float Percentage()
            {
                return _stepsCompleted / (float)_totalSteps;
            }
            internal void Clear()
            {
                _totalSteps = 0;
                _stepsCompleted = 0;
            }
        }
     
        static Steps _processing = new Steps();
        static Steps _applying = new Steps();
        static Steps _clearing = new Steps();
     
        static List<string> _logs = new List<string>();
     
        static SelectObjectsIteratively()
        {
            s_backgroundUpdateCB = new EditorApplication.CallbackFunction(UpdateCallBack);
            EditorApplication.update += s_backgroundUpdateCB;
        }
     
        [MenuItem("Custom/Nidre/Misc/Optimize/Select Objects Iter. In Scene")]
        public static void ForceUpdateAtlasReadersInScene()
        {
            _mode = Mode.Scene;
            _deleteObjectAfter = false;
            Clear();
            _initialSelection = Selection.objects;
            LoopAndUpdateSelectedObjects();
        }
     
        [MenuItem("Custom/Nidre/Misc/Optimize/Select Objects Iter. In Project")]
        public static void ForceUpdateAtlasReadersInProject()
        {
            _mode = Mode.Project;
            _deleteObjectAfter = true;
            Clear();
            LoopAllObjectsOfType();
            LoopAndUpdateSelectedObjects();
        }
     
        static void UpdateCallBack()
        {
            if (_objectsToBeSelected.Count > 0)
            {
                _processing._stepsCompleted++;
                //_percentage = _stepsCompleted / (float)_totalSteps;
                //if ((_objectsToBeSelected[0] as GameObject).transform.parent != null)
                //     _logs.Add("Processing : " + (_objectsToBeSelected[0] as GameObject).transform.parent.name + "." + _objectsToBeSelected[0].name);
                //else
                //     _logs.Add("Processing : " + _objectsToBeSelected[0].name);
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Processing... ({0}/{1})", _processing._stepsCompleted, _processing._totalSteps), _objectsToBeSelected[0].name, _processing.Percentage()))
                {
                    _isApplying = false;
                    _skipApply = true;
                    _processing._stepsCompleted += _objectsToBeSelected.Count;
                    _objectsToBeSelected.Clear();
                    _prefabObjects.Clear();
                    _prefabIntances.Clear();
                    WriteLog();
                    _logs.Clear();
                }
                else
                {
                    Selection.objects = new GameObject[] { _objectsToBeSelected[0] as GameObject };
                    ArrayList sceneViews = UnityEditor.SceneView.sceneViews;
                    foreach (SceneView sceneView in sceneViews)
                    {
                        sceneView.LookAt((_objectsToBeSelected[0] as GameObject).transform.position);
                    }
                    _objectsToBeSelected.RemoveAt(0);
                    _isApplying = false;
                }
            }
            else if (!_isApplying && _initialSelection != null)
            {
                _isApplying = true;
                if (!_skipApply)
                {
                    ApplyChanges();
                    //for (int i = 0; i < _initialSelection.Length; i++)
                    //{
                    //    _stepsCompleted++;
                    //    _percentage = _stepsCompleted / (float)_totalSteps;
                    //    EditorUtility.DisplayProgressBar(string.Format("Applying... ({0}/{1})", _stepsCompleted, _totalSteps), _initialSelection[i].name, _percentage);
                    //    TraverseHierarchyAndApplyChanges(_initialSelection[i] as GameObject);
                    //}
                }
                if (_deleteObjectAfter)
                {
                    DeleteLeftOvers();
                }
                WriteLog();
                Clear();
            }
        }
     
        private static void WriteLog()
        {
            int _steps = 0;
            foreach (var item in _logs)
            {
                EditorUtility.DisplayProgressBar(string.Format("Writin Log... ({0}/{1})", _steps, _logs.Count), "", _steps / (float)_logs.Count);
                print(item);
                _steps++;
            }
        }
     
        private static void DeleteLeftOvers()
        {
            foreach (var item in _initialSelection)
            {
                _clearing._stepsCompleted++;
                EditorUtility.DisplayProgressBar(string.Format("Clearing... ({0}/{1})", _clearing._stepsCompleted, _clearing._totalSteps), item.name, _clearing.Percentage());
                DestroyImmediate(item);
            }
        }
     
        private static void LoopAndUpdateSelectedObjects()
        {
            //Clear();
            for (int i = 0; i < _initialSelection.Length; i++)
            {
                //_stepsCompleted++;
                //_percentage = _stepsCompleted / (float)_totalSteps;
                EditorUtility.DisplayProgressBar("Adding Objects to Update Queue...", _initialSelection[i].name, 1);
                TraverseHierarchyAndMakeChanges(_initialSelection[i] as GameObject);
            }
            if (_prefabIntances.Count + _prefabObjects.Count == 0)
            {
                Clear();
                if (_mode == Mode.Project)
                {
                    DeleteLeftOvers();
                }
            }
        }
     
        private static void TraverseHierarchyAndMakeChanges(GameObject go)
        {
            if (go)
            {
                bool originalActiveState = go.activeSelf;
                bool originalActiveInHierState = go.activeInHierarchy;
                if (!originalActiveInHierState)
                {
                    GameObject prefabInstance = PrefabUtility.FindPrefabRoot(PrefabUtility.FindRootGameObjectWithSameParentPrefab(go));
                    prefabInstance.SetActive(true);
                }
                if (!originalActiveState)
                {
                    go.SetActive(true);
                }
                for (int j = 0; j < go.transform.childCount; j++)
                {
                    FindPrefabRoot(go.transform.GetChild(j).gameObject);
                    TraverseHierarchyAndMakeChanges(go.transform.GetChild(j).gameObject);
                }
                FindPrefabRoot(go);
                if (!originalActiveInHierState)
                {
                    GameObject prefabInstance = PrefabUtility.FindPrefabRoot(PrefabUtility.FindRootGameObjectWithSameParentPrefab(go));
                    prefabInstance.SetActive(originalActiveInHierState);
                }
                if (!originalActiveState)
                    go.SetActive(originalActiveState);
            }
        }
     
        private static void Clear()
        {
            _skipApply = false;
            //_stepsCompleted = 0;
            //_totalSteps = 0;
            _clearing.Clear();
            _applying.Clear();
            _processing.Clear();
            _objectsToBeSelected.Clear();
            _prefabObjects.Clear();
            _prefabIntances.Clear();
            _logs.Clear();
            EditorUtility.ClearProgressBar();
        }
     
        private static void ApplyChanges()
        {
            foreach (var item in _prefabIntances)
            {
                _applying._stepsCompleted++;
                //_percentage = _stepsCompleted / (float)_totalSteps;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Applying... ({0}/{1})", _applying._stepsCompleted, _applying._totalSteps), item.name, _applying.Percentage()))
                {
                    break;
                }
                ComparePrefabAndApply(item);
     
            }
        }
     
        [ExecuteInEditMode]
        private static void FindPrefabRoot(GameObject go)
        {
            Object prefabObject;
            GameObject prefabInstance;
            //bool originalActiveState;
            if (!_objectsToBeSelected.Contains(go))
            {
                //Those two lines were used to select only the object i have needed.You can replace them with anything you want.Commented out for now.
                //if (go.GetComponentInChildren<AtlasReaderBase>())
                //{
                //    if (go.GetComponent<AtlasReaderBase>())
                //    {
                        _objectsToBeSelected.Add(go);
                        _processing._totalSteps++;
     
                        //originalActiveState = go.activeSelf;
                        //if (!originalActiveState)
                        //{
                        //    go.SetActive(true);
                        //}
                        prefabInstance = PrefabUtility.FindPrefabRoot(PrefabUtility.FindRootGameObjectWithSameParentPrefab(go));
                        prefabObject = PrefabUtility.GetPrefabParent(prefabInstance);
                        //go.SetActive(originalActiveState);
                        if (prefabObject != null)
                        {
                            if (!_prefabObjects.Contains(prefabObject))
                            {
                                _prefabObjects.Add(prefabObject);
                            }
                            if (!_prefabIntances.Contains(prefabInstance))
                            {
                                _prefabIntances.Add(prefabInstance);
                                _applying._totalSteps++;
                            }
                            // _logs.Add("Prefab Root Added : " + prefabRoot.name);
                        }
                        else
                        {
                            if (prefabInstance != null)
                                _logs.Add("No Prefab Parent Found For : " + prefabInstance.name + "-" + go.name);
                            else
                                _logs.Add("No Prefab Parent Found For : " + go.name);
                        }
                //    }
                //}
                //else  _logs.Add("No Atlas Reader In Children Found For : " + go.name);
            }
            //else _logs.Add("_objectsToBeSelected already contains : " + go.name);
            //Selection.objects = new GameObject[] { go };
        }
     
     
        private static void ComparePrefabAndApply(Object prefabInstance)
        {
            GameObject temp = prefabInstance as GameObject;
            bool originalActiveState = temp.activeSelf;
            if (!originalActiveState)
            {
                temp.SetActive(true);
            }
            Object prefabObject = PrefabUtility.GetPrefabParent(prefabInstance);
            if (prefabObject == null)
            {
                if (prefabInstance != null)
                    _logs.Add("prefabParent is NUll for " + prefabInstance.name);
                return;
            }
            if (_prefabObjects.Contains(prefabObject))
            {
                //If a prefab is passed, make sure it is valid
                PrefabType prefabType = PrefabUtility.GetPrefabType(prefabObject);
                if (prefabType != PrefabType.Prefab)
                {
                    Debug.LogError("The prefab is not a valid reference to a Prefab : " + prefabObject.name + "(" + prefabType.ToString() + ")");
                    return;
                }
                if (prefabObject.name.Equals(prefabInstance.name))
                {
                    //Following lines were used to select only the object i have needed.You can replace them with anything you want.Commented out for now.
                    //if ((prefabInstance as GameObject).transform.GetComponentInChildren<AtlasReaderBase>() != null)
                    //{
                        temp.SetActive(originalActiveState);
                        PrefabUtility.ReplacePrefab(
                        prefabInstance as GameObject,
                        prefabObject,
                        ReplacePrefabOptions.ReplaceNameBased
                        );
                        _logs.Add("Applied : " + prefabObject.name);
                        _prefabObjects.Remove(prefabObject);
                    //}
                    //else _logs.Add("AtlasReaderBase is NUll for children of " + prefabInstance.name);
                }
                else _logs.Add(prefabObject.name + " is not eq to " + prefabInstance.name);
            }
            else _logs.Add("prefabParent is not in _prefabParents list " + prefabObject.name);
            temp.SetActive(originalActiveState);
        }
     
        static void LoopAllObjectsOfType()
        {
            EditorUtility.DisplayProgressBar("Finding...", "", 1);
            IEnumerable<string> assetFolderPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab"));
            IEnumerable<GameObject> assets = assetFolderPaths.Select(item => AssetDatabase.LoadAssetAtPath(item, typeof(GameObject))).Cast<GameObject>().Where(obj => obj != null);
            List<Object> objectsFound = new List<Object>();
            foreach (GameObject obj in assets)
            {
                objectsFound.Add(PrefabUtility.InstantiatePrefab(obj));
            }
            _initialSelection = objectsFound.ToArray();
            //_processing._totalSteps = objectsFound.Count;
            _clearing._totalSteps = objectsFound.Count;
            //_totalSteps = objectsFound.Count;// *2;
            objectsFound.Clear();
        }
}
}
