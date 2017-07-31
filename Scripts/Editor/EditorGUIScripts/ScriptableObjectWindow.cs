// Original url: http://wiki.unity3d.com/index.php/ScriptableObjectWindow
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/ScriptableObjectWindow.cs
// File based on original modification date of: 19 April 2017, at 12:49. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorGUIScripts
{
Author: Fredrik Ludvigsen (Steinbitglis) 
Description Adds a scriptable object window under Window/Scriptable objects. 
 
Usage Place the script in a folder named Editor in your project's Assets folder. 
Open the window by selecting Window/Scriptable objects. The objects can be dragged out of, but not into this window. 
Background: 
ScriptableObjects can be saved either as independent assets in separate files, or together with a scene. 
This window will give you a full overview for your currently open scenes. 
Tested with Unity 5.5, probably needs adjustment for Unity 5.6. 
C# - ScriptableObjectWindow.cs using Type = System.Type;
using Assembly = System.Reflection.Assembly;
 
using System.Collections.Generic;
using System.Text;
 
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
 
public class ScriptableObjectWindow : EditorWindow {
 
    private static GenericMenu newObjectMenu;
 
    public Vector2 scrollPosition = Vector2.zero;
 
    private ScriptableObject[] assetObjects;
    private ScriptableObject[] orphanObjects;
    private Dictionary<string, ScriptableObject[]> sceneScriptableObjects;
    private int numSceneObjects;
 
    // Caches
    private HashSet<ScriptableObject> orphanObjectSet;
    private HashSet<ScriptableObject>  assetObjectSet;
    private Dictionary<string, SerializedObject[]> allSerializedObjects;
    private List<Component> components;
    private Dictionary<Component,          SerializedObject> fullSerializedComponentCache;
    private Dictionary<UnityEngine.Object, SerializedObject> fullSerializedObjectCache;
    private List<ScriptableObject> finalResult;
    private HashSet<UnityEngine.Object> checkedObjects;
    private List<SerializedObject> currentSerializedObjects;
    private List<SerializedObject> nextSerializedObjects;
 
    [System.NonSerialized]
    private bool dirty = true;
 
    [MenuItem("Window/Scriptable objects")]
    public static void Init() {
        EditorWindow.GetWindow<ScriptableObjectWindow>("Scriptable objects", true);
    }
 
    private void OnEnable() {
        CreateNewObjectMenu();
    }
 
    private static void CreateNewObjectMenu() {
        newObjectMenu = new GenericMenu();
 
        Assembly[] referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i< referencedAssemblies.Length; i++) {
            var assemblyName = referencedAssemblies[i].FullName;
            if (!((assemblyName.StartsWith("Assembly") &&
                  (assemblyName.StartsWith("Assembly-CSharp-Editor") ||
                   assemblyName.StartsWith("Assembly-CSharp-Editor-firstpass") ||
                   assemblyName.StartsWith("Assembly-UnityScript-Editor") ||
                   assemblyName.StartsWith("Assembly-UnityScript-Editor-firstpass") ||
                   assemblyName.StartsWith("Assembly-Boo-Editor") ||
                   assemblyName.StartsWith("Assembly-Boo-Editor-firstpass"))))) {
                foreach (Type t in referencedAssemblies[i].GetTypes()) {
                    if (typeof(ScriptableObject).IsAssignableFrom(t) && !t.IsAbstract) {
                        if (t.Namespace == null ||
                            (!(t.Namespace.StartsWith("UnityEditorInternal") ||
                               t.Namespace.StartsWith("UnityEditor") ||
                               t.Namespace.StartsWith("UnityEngine") ||
                               t.Namespace.StartsWith("Unity") ||
                               t.Namespace.StartsWith("TreeEditor")))) {
                            newObjectMenu.AddItem(new GUIContent(t.FullName.Replace('.', '/')), false, CreateNewObjectHandler, t);
                        }
                    }
                }
            }
        }
    }
 
    private static void CreateNewObjectHandler(object data) {
        ScriptableObject.CreateInstance((Type) data);
    }
 
    private void OnInspectorUpdate() {
        dirty = true;
        Repaint();
    }
 
    private void OnGUI() {
        if (dirty) {
            dirty = false;
 
            if (sceneScriptableObjects == null)
                sceneScriptableObjects = new Dictionary<string, ScriptableObject[]>();
            else
                sceneScriptableObjects.Clear();
 
            numSceneObjects = 0;
 
            if (orphanObjectSet == null)
                orphanObjectSet = new HashSet<ScriptableObject>();
            else
                orphanObjectSet.Clear();
 
            if (assetObjectSet == null)
                assetObjectSet = new HashSet<ScriptableObject>();
            else
                assetObjectSet.Clear();
 
            if (fullSerializedComponentCache == null)
                fullSerializedComponentCache = new Dictionary<Component,          SerializedObject>();
            if (fullSerializedObjectCache    == null)
                fullSerializedObjectCache    = new Dictionary<UnityEngine.Object, SerializedObject>();
 
            foreach (ScriptableObject o in FindObjectsOfType(typeof(ScriptableObject))) {
                if (!AssetDatabase.Contains(o))
                    orphanObjectSet.Add(o);
            }
 
            if (allSerializedObjects == null)
                allSerializedObjects = new Dictionary<string, SerializedObject[]>();
            else
                allSerializedObjects.Clear();
 
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                var ro = scene.GetRootGameObjects();
                if (components == null)
                    components = new List<Component>();
                else
                    components.Clear();
 
                for (int j = 0; j < ro.Length; j++) {
                    GameObject go = ro[j];
                    components.AddRange(go.GetComponentsInChildren<Component>(true));
                }
                SerializedObject[] aso;
                if (!allSerializedObjects.ContainsKey(scene.name))
                    aso = allSerializedObjects[scene.name] = new SerializedObject[components.Count];
                else {
                    var prevSerializedObjects = allSerializedObjects[scene.name];
                    if (prevSerializedObjects.Length != components.Count)
                        aso = allSerializedObjects[scene.name] = new SerializedObject[components.Count];
                    else
                        aso = prevSerializedObjects;
                }
                for (int j = 0; j < components.Count; j++) {
                    var c = components[j];
                    SerializedObject cachedSO;
                    if (fullSerializedComponentCache.TryGetValue(c, out cachedSO)) {
                        cachedSO.Update();
                        aso[j] = cachedSO;
                    } else {
                        fullSerializedComponentCache[c] = (aso[j] = new SerializedObject(c));
                    }
                }
            }
 
            var allSerializedObjectsEnumerator = allSerializedObjects.GetEnumerator();
            try { while (allSerializedObjectsEnumerator.MoveNext()) {
                var sceneSerializedObjects = allSerializedObjectsEnumerator.Current;
                if (finalResult == null)
                    finalResult = new List<ScriptableObject>();
                else
                    finalResult.Clear();
 
                if (checkedObjects == null)
                    checkedObjects = new HashSet<UnityEngine.Object>();
                else
                    checkedObjects.Clear();
 
                if (currentSerializedObjects == null)
                    currentSerializedObjects = new List<SerializedObject>(sceneSerializedObjects.Value);
                else {
                    currentSerializedObjects.Clear();
                    for (int i = 0; i < sceneSerializedObjects.Value.Length; i++)
                        currentSerializedObjects.Add(sceneSerializedObjects.Value[i]);
                }
                while (currentSerializedObjects.Count > 0) {
                    if (nextSerializedObjects == null)
                        nextSerializedObjects = new List<SerializedObject>();
                    else
                        nextSerializedObjects.Clear();
                    for (int i = 0; i < currentSerializedObjects.Count; i++) {
                        SerializedObject so = currentSerializedObjects[i];
                        if (!checkedObjects.Contains(so.targetObject)) {
                            checkedObjects.Add(so.targetObject);
                            var sp = so.GetIterator();
                            do {
                                if (sp.propertyType == SerializedPropertyType.ObjectReference) {
                                    var scriptableObjectReference = sp.objectReferenceValue as ScriptableObject;
                                    if (scriptableObjectReference != null) {
                                        if (!assetObjectSet.Contains(scriptableObjectReference)) {
                                            if (AssetDatabase.Contains(scriptableObjectReference)) {
                                                assetObjectSet.Add(scriptableObjectReference);
                                            } else {
                                                numSceneObjects++;
                                                finalResult.Add(scriptableObjectReference);
                                                orphanObjectSet.Remove(scriptableObjectReference);
                                            }
                                        }
                                    }
                                    if (sp.objectReferenceValue != null) {
                                        var or = sp.objectReferenceValue;
                                        SerializedObject cachedSO;
                                        if (fullSerializedObjectCache.TryGetValue(or, out cachedSO)) {
                                            cachedSO.Update();
                                            nextSerializedObjects.Add(cachedSO);
                                        } else {
                                            SerializedObject newSO = new SerializedObject(or);
                                            nextSerializedObjects.Add(newSO);
                                            fullSerializedObjectCache[or] = newSO;
                                        }
                                    }
                                }
                            } while (sp.Next(true));
                        }
                    }
                    var unusedList = currentSerializedObjects; // Reuse list for GC benefit
                    currentSerializedObjects = nextSerializedObjects;
                    nextSerializedObjects = unusedList;
                }
 
                sceneScriptableObjects[sceneSerializedObjects.Key] = finalResult.ToArray();
            } } finally {
                allSerializedObjectsEnumerator.Dispose();
            }
            if ( assetObjects == null ||  assetObjects.Length !=  assetObjectSet.Count)  assetObjects = new ScriptableObject[ assetObjectSet.Count];
            if (orphanObjects == null || orphanObjects.Length != orphanObjectSet.Count) orphanObjects = new ScriptableObject[orphanObjectSet.Count];
 
            assetObjectSet.CopyTo (assetObjects);
            orphanObjectSet.CopyTo(orphanObjects);
        }
 
        var lineHeight = EditorGUIUtility.singleLineHeight;
 
        var wantedHeightReal = lineHeight * (sceneScriptableObjects.Count + numSceneObjects + 1 + orphanObjects.Length + 1 + assetObjects.Length);
        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPosition, new Rect(0, 0, position.width, wantedHeightReal));
 
        index = 0;
        indent = 0f;
 
        ScriptableObjectsField(sceneScriptableObjects, false);
        ScriptableObjectsField("Unreferenced", orphanObjects, false);
        NewScriptableObjectField();
        ScriptableObjectsField("Assets",  assetObjects, true);
 
        GUI.EndScrollView();
    }
 
    private void ScriptableObjectsField(Dictionary<string, ScriptableObject[]> objects, bool isAsset) {
        var enumerator = objects.GetEnumerator();
        try { while (enumerator.MoveNext()) {
                var kvp = enumerator.Current;
                ScriptableObjectsField(kvp.Key, kvp.Value, isAsset);
        } } finally {
            enumerator.Dispose();
        }
    }
 
    private int index = 0;
    private float indent = 0f;
    private Rect lastRect;
 
    private void NewScriptableObjectField() {
        indent = 14f;
        var lineHeight = EditorGUIUtility.singleLineHeight;
        if (GUI.Button(new Rect(indent, index++ * lineHeight, position.width, lineHeight), "+")) {
            newObjectMenu.ShowAsContext();
        }
        indent = 0f;
    }
 
    private void ScriptableObjectsField(string name, ScriptableObject[] objects, bool isAsset) {
        var lineHeight = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField((lastRect = new Rect(indent, index++ * lineHeight, position.width, lineHeight)), name, UnityEditor.EditorStyles.boldLabel);
        indent = 14f;
        for (int i = 0; i < objects.Length; i++)
            ScriptableObjectField(objects[i], isAsset);
        indent = 0f;
    }
 
    private void ScriptableObjectField(ScriptableObject o, bool isAsset) {
        BeginDrag();
        var lineHeight = EditorGUIUtility.singleLineHeight;
        var bw = 64f;
        if (isAsset) {
            EditorGUI.ObjectField((lastRect = new Rect(indent,              index   * lineHeight, position.width - (indent + bw), lineHeight)), o, typeof(ScriptableObject), true);
            if  (GUI.Button                  (new Rect(position.width - bw, index++ * lineHeight,                            bw , lineHeight) , "Delete"))
                if (EditorUtility.DisplayDialog("Delete asset?", "Are you sure you want to delete the asset from disk and memory?", "Delete", "Cancel"))
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(o));
        }
        else {
            EditorGUI.ObjectField((lastRect = new Rect(indent,              index   * lineHeight, position.width - (indent + bw), lineHeight)), o, typeof(ScriptableObject), true);
            if  (GUI.Button                  (new Rect(position.width - bw, index++ * lineHeight,                            bw , lineHeight) , "To asset")) {
                var path = EditorUtility.SaveFilePanelInProject("Save to asset", string.Format("{0}.asset", o.GetType().Name), "asset", "Enter a file name to save into");
                if (path.Length != 0) AssetDatabase.CreateAsset(o, path);
            }
        }
        EndDrag(o);
    }
 
    private void BeginDrag() {
        if (Event.current.type == EventType.DragUpdated) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            Event.current.Use();
        }
    }
 
    private void EndDrag(UnityEngine.Object o) {
        if (Event.current.type == EventType.MouseDrag) {
            if (lastRect.Contains(Event.current.mousePosition)) {
                DragAndDrop.PrepareStartDrag ();
                DragAndDrop.objectReferences = new UnityEngine.Object[] {o};
                DragAndDrop.StartDrag (string.Format("Dragging {0}", o.ToString()));
                Event.current.Use();
            }
        }
    }
}
}
