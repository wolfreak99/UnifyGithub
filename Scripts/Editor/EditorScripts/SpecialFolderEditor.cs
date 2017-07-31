// Original url: http://wiki.unity3d.com/index.php/SpecialFolderEditor
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SpecialFolderEditor.cs
// File based on original modification date of: 6 September 2016, at 09:15. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Fredrik Ludvigsen (Steinbitglis) 
Contents [hide] 
1 Description 
2 Usage 
3 Examples 
3.1 MyAsset.cs 
3.2 MyAssetFolderInspector.cs 
4 Core 
4.1 ObjectInspector.cs 
4.2 ObjectInspectorInitiator.cs 
5 References 

Description A good way of writing editors for special folders. 

 

One benefit is that you can edit a large set of assets without having to lump them together in a large file. 
The implementation relies on reflection, but the resulting work process is very good, and makes up for it. 
This technique eliminates any doubt about where the a given asset type should be stored in your project. Folders now have an explicit meaning and associated set of utilities! 
Usage For usage beyond the #Examples... just derive from ObjectInspector every time you need a new type of inspector. It can be special folders, like in the example, or it can be custom file types etc. It's up to you. 
Examples The example scripts are MyAsset.cs and MyAssetFolderInspector.cs. 
The example editor is based on the GUID 8bf3d03..., you must change this in order for the example to work. 


MyAsset.cs using UnityEngine;
 
public class MyAsset : ScriptableObject {
    public string description;
}MyAssetFolderInspector.cs using System.Collections.Generic;
using System.IO;
 
using UnityEngine;
 
using UnityEditor;
 
using Object = UnityEngine.Object;
 
public class MyAssetFolderInspector : ObjectInspector {
    override public bool IsValid() { return HasGUID("8bf3d033a45b77c43bb8412e2dea5cf6"); }
 
    private MyAsset[] assets;
    private SerializedObject[] serializedObjects;
 
    private SerializedProperty[] fileNameProps;
    private SerializedProperty[] descriptionProps;
 
    private GUIContent descriptionGUIContent;
    private GUIContent fileNameGUIContent;
 
    override public void OnEnable() {
        Read();
    }
 
    private void Read() {
        var assetList = new List<MyAsset>();
        foreach (string f in Directory.GetFiles(AssetDatabase.GetAssetPath(target), "*.asset")) {
            var o = (MyAsset) AssetDatabase.LoadAssetAtPath(f, typeof(MyAsset));
            if (o != null) assetList.Add(o);
        }
        assets = assetList.ToArray();
 
        serializedObjects = new SerializedObject[assets.Length];
 
        fileNameProps     = new SerializedProperty[assets.Length];
        descriptionProps  = new SerializedProperty[assets.Length];
 
        fileNameGUIContent    = new GUIContent("File Name");
        descriptionGUIContent = new GUIContent("Description");
 
        for (int i = 0; i < assets.Length; i++) {
            var s = serializedObjects[i] = new SerializedObject(assets[i]);
            fileNameProps[i]    = s.FindProperty("m_Name");
            descriptionProps[i] = s.FindProperty("description");
        }
    }
 
    override public void OnInspectorGUI() {
        var singleHeight = EditorGUIUtility.singleLineHeight;
        var unitHeight = singleHeight * 5 + 5f;
        var controlRect = EditorGUILayout.GetControlRect(false, unitHeight * assets.Length);
        controlRect.height = unitHeight;
        var labelWidth = 100f;
        var otherWidth = controlRect.width - labelWidth;
        var noGUIContent = GUIContent.none;
        var fileMoved = false;
 
        for (int i = 0; i < assets.Length; i++) {
            serializedObjects[i].Update();
            var fileNameLabelTotalRect = new Rect(controlRect.x, controlRect.y, labelWidth + otherWidth, singleHeight);
            var fileNameLabelRect      = new Rect(controlRect.x, controlRect.y, labelWidth             , singleHeight);
 
            var descriptionLabelTotalRect = new Rect(controlRect.x, controlRect.y + singleHeight, labelWidth+otherWidth, singleHeight);
            var descriptionLabelRect      = new Rect(controlRect.x, controlRect.y + singleHeight, labelWidth, singleHeight);
 
            var fileNameRect    = new Rect(controlRect.x + labelWidth,              controlRect.y               , otherWidth,   singleHeight);
            var descriptionRect = new Rect(controlRect.x + labelWidth,              controlRect.y + singleHeight, otherWidth, 4*singleHeight);
 
            var selectRect = new Rect(controlRect.x, controlRect.y+unitHeight-singleHeight-8, labelWidth, singleHeight+2);
 
            EditorGUI.HandlePrefixLabel(fileNameLabelTotalRect, fileNameLabelRect, fileNameGUIContent);
            EditorGUI.BeginChangeCheck();
            var newFileName = EditorGUI.DelayedTextField(fileNameRect, noGUIContent, fileNameProps[i].stringValue);
            if (EditorGUI.EndChangeCheck()) {
                fileNameProps[i].stringValue = newFileName;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(assets[i]), newFileName);
                fileMoved = true;
            }
 
            EditorGUI.HandlePrefixLabel(descriptionLabelTotalRect, descriptionLabelRect, descriptionGUIContent);
            EditorGUI.PropertyField(descriptionRect, descriptionProps[i], noGUIContent);
 
            if (GUI.Button(selectRect, "Ping")) {
                EditorGUIUtility.PingObject(serializedObjects[i].targetObject);
            }
 
            serializedObjects[i].ApplyModifiedProperties();
 
            controlRect.y += unitHeight;
        }
 
        if (GUILayout.Button("Add")) {
            var newInstance = ScriptableObject.CreateInstance<MyAsset>();
            var standardPath = Path.Combine( AssetDatabase.GetAssetPath(target), "MyAsset 1.asset" );
            var newPath      = AssetDatabase.GenerateUniqueAssetPath(standardPath);
            AssetDatabase.CreateAsset(newInstance, newPath);
            Read();
        } else if (fileMoved) {
            Read();
        }
    }
}

Core ObjectInspector.cs using System;
 
using UnityEngine;
 
using UnityEditor;
 
using Object = UnityEngine.Object;
 
abstract public class ObjectInspector {
    protected Object target;
    abstract public void OnEnable();
    abstract public bool IsValid();
    abstract public void OnInspectorGUI();
 
    protected bool HasGUID(string guid) {
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target)).Equals(guid, StringComparison.Ordinal);
    }
}ObjectInspectorInitiator.cs using System;
using System.Reflection;
 
using UnityEngine;
 
using UnityEditor;
 
using Object = UnityEngine.Object;
 
[CustomEditor(typeof(Object), true)]
public class ObjectInspectorInitiator : Editor {
    private ObjectInspector inspectorObject;
    private static object[] noArgs = new object[0];
 
    private void OnEnable() {
        inspectorObject = null;
        try {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes()) {
                if (typeof(ObjectInspector).IsAssignableFrom(t) && !t.IsAbstract) {
                    var constructorInfo = t.GetConstructor(Type.EmptyTypes);
                    inspectorObject = (ObjectInspector) constructorInfo.Invoke(noArgs);
                    var isValidInfo  = t.GetMethod("IsValid",  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var targetInfo   = t.GetField("target",  BindingFlags.Instance | BindingFlags.NonPublic);
                    if (targetInfo != null && isValidInfo != null) {
                        targetInfo.SetValue(inspectorObject, target);
                        if ((bool)isValidInfo.Invoke(inspectorObject, noArgs))
                            t.GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(inspectorObject, noArgs);
                        else
                            inspectorObject = null;
                    } else inspectorObject = null;
                    break;
                }
            }
        } catch (Exception) {
            inspectorObject = null;
        }
    }
 
    public override void OnInspectorGUI() {
        if (inspectorObject != null) {
            var prevEnabledGUI = GUI.enabled;
            GUI.enabled = true;
            try {
                inspectorObject.OnInspectorGUI();
            } finally {
                GUI.enabled = prevEnabledGUI;
            }
        } else {
            DrawDefaultInspector();
        }
    }
}

References Ryan Hipple - Unite 2014 - The Hack Spectrum: Tips, Tricks and Hacks for Unity 
}
