// Original url: http://wiki.unity3d.com/index.php/TransformInspectorWithSize
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/TransformInspectorWithSize.cs
// File based on original modification date of: 1 October 2015, at 07:25. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorGUIScripts
{
DescriptionThe following script adds a Size field to the Transform inspector to show the scaled mesh size of an object. 
Provides a convenient way to see the actual size of a mesh in the scene or update the scale of an object to define a precise object size after scaling. 
Accounts for child objects with meshes as well, showing the total size in the parent. 
The Size field can be edited and will update the Scale property in relation the objects mesh size. 
Derived from TransformInspector. 
UsagePlace TransformInspectorWithSize.cs into an Editor folder inside of your project's Assets folder. 
TransformInspectorWithSize.cs// Reverse engineered UnityEditor.TransformInspector
 
using UnityEngine;
using UnityEditor;
 
[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
public class TransformInspector : UnityEditor.Editor {
 
    private const float FIELD_WIDTH = 212.0f;
    private const bool WIDE_MODE = true;
 
    private const float POSITION_MAX = 100000.0f;
 
    private static GUIContent positionGUIContent = new GUIContent(LocalString("Position"),  LocalString("The local position of this Game Object relative to the parent."));
    private static GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation"),  LocalString("The local rotation of this Game Object relative to the parent."));
    private static GUIContent scaleGUIContent    = new GUIContent(LocalString("Scale"),     LocalString("The local scaling of this Game Object relative to the parent."));
    private static GUIContent sizeGUIContent     = new GUIContent(LocalString("Size"),      LocalString("The local scaled mesh size of this Game Object relative to the parent."));
 
    private static string positionWarningText = LocalString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");
 
    private SerializedProperty positionProperty;
    private SerializedProperty rotationProperty;
    private SerializedProperty scaleProperty;
 
    private static string LocalString(string text) {
        return LocalizationDatabase.GetLocalizedString(text);
    }
 
    public void OnEnable() {
        this.positionProperty = this.serializedObject.FindProperty("m_LocalPosition");
        this.rotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
        this.scaleProperty = this.serializedObject.FindProperty("m_LocalScale");
    }
 
    public override void OnInspectorGUI() {
        EditorGUIUtility.wideMode = TransformInspector.WIDE_MODE;
        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - TransformInspector.FIELD_WIDTH; // align field to right of inspector
 
        this.serializedObject.Update();
 
        EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);
        this.RotationPropertyField(this.rotationProperty, rotationGUIContent);
        EditorGUILayout.PropertyField(this.scaleProperty, scaleGUIContent);
        this.ScaledSizePropertyField(this.scaleProperty, sizeGUIContent);
 
        if (!ValidatePosition(((Transform) this.target).position)) {
            EditorGUILayout.HelpBox(positionWarningText, MessageType.Warning);
        }
 
        this.serializedObject.ApplyModifiedProperties();
    }
 
    private bool ValidatePosition(Vector3 position) {
        if (Mathf.Abs(position.x) > TransformInspector.POSITION_MAX) return false;
        if (Mathf.Abs(position.y) > TransformInspector.POSITION_MAX) return false;
        if (Mathf.Abs(position.z) > TransformInspector.POSITION_MAX) return false;
        return true;
    }
 
    private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content) {
        Transform transform = (Transform) this.targets[0];
        Quaternion localRotation = transform.localRotation;
        foreach (UnityEngine.Object t in (UnityEngine.Object[]) this.targets) {
            if (!SameRotation(localRotation, ((Transform) t).localRotation)) {
                EditorGUI.showMixedValue = true;
                break;
            }
        }
 
        EditorGUI.BeginChangeCheck();
 
        Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);
 
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObjects(this.targets, "Rotation Changed");
            foreach (UnityEngine.Object obj in this.targets) {
                Transform t = (Transform) obj;
                t.localEulerAngles = eulerAngles;
            }
            rotationProperty.serializedObject.SetIsDifferentCacheDirty();
        }
 
        EditorGUI.showMixedValue = false;
    }
 
    private bool SameRotation(Quaternion rot1, Quaternion rot2) {
        if (rot1.x != rot2.x) return false;
        if (rot1.y != rot2.y) return false;
        if (rot1.z != rot2.z) return false;
        if (rot1.w != rot2.w) return false;
        return true;
    }
 
    private void ScaledSizePropertyField(SerializedProperty scaleProperty, GUIContent content) {
        bool guiEnabled = GUI.enabled;
        bool enabled = GUI.enabled;
        Vector3 meshSize = GetWorldMeshSize((Transform) this.target);
        if (this.targets.Length > 1) {
            foreach (Transform obj1 in this.targets) {
                foreach (Transform obj2 in this.targets) {
                    if (obj1.parent == obj2) enabled = false;
                }
            }
            foreach (Transform obj1 in this.targets) {
                if (meshSize != GetWorldMeshSize((Transform) obj1)) {
                    EditorGUI.showMixedValue = true;
                    break;
                }
            }
        }
 
        if (meshSize == Vector3.zero) return;
 
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
 
        // convert to local scale
        Transform t = (Transform) this.target;
        Vector3 lossy2local = new Vector3(t.localScale.x / t.lossyScale.x, t.localScale.y / t.lossyScale.y, t.localScale.z / t.lossyScale.z);
 
        meshSize = Vector3.Scale(meshSize, lossy2local);
 
        enabled = enabled && !EditorGUI.showMixedValue;
        GUI.enabled = enabled;
        Vector3 size = EditorGUILayout.Vector3Field(content, meshSize);
        GUI.enabled = guiEnabled;
        if (!enabled) return;
 
        // don't update if any component is 0
        if (size.x == 0) return;
        if (size.y == 0) return;
        if (size.z == 0) return;
 
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObjects(this.targets, "Scale Changed");
            foreach (UnityEngine.Object obj in this.targets) {
                t = (Transform) obj;
                meshSize = GetWorldMeshSize(t);
                if (meshSize == Vector3.zero) continue;
                lossy2local = new Vector3(t.localScale.x / t.lossyScale.x, t.localScale.y / t.lossyScale.y, t.localScale.z / t.lossyScale.z);
                meshSize = Vector3.Scale(meshSize, lossy2local);
                t.localScale = Vector3.Scale(t.localScale, new Vector3(size.x / meshSize.x, size.y / meshSize.y, size.z / meshSize.z));
            }
            scaleProperty.serializedObject.SetIsDifferentCacheDirty();
        }
    }
 
 
    private Vector3 GetWorldMeshSize(Transform t) {
        Vector3 size = Vector3.zero;
        MeshFilter[] filters = t.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter filter in filters) {
            if (filter.sharedMesh == null) continue;
            Vector3 meshSize = Vector3.Scale(filter.sharedMesh.bounds.size, filter.transform.lossyScale);
            size = Vector3.Max(size, meshSize);
        }
        return size;
    }
}
}
