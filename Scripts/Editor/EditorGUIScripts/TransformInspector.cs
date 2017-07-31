// Original url: http://wiki.unity3d.com/index.php/TransformInspector
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorGUIScripts/TransformInspector.cs
// File based on original modification date of: 26 September 2015, at 21:25. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorGUIScripts
{
DescriptionTransformInspector.cs is a reverse engineered version of Unity's built-in UnityEditor.TransformInspector class from Unity v5.1.1f. It should both look and function nearly identical to the original. Once loaded into your project, all Transform components will be routed through TransformInspector.cs allowing you to easily extend or modify the inspector. 
For simplification and to honor copyright, this class was completely rewritten compared to the Unity internal code so may not behave exactly like the original, however it should perform as expected, including multi-selection, tooltips, warnings and localization. The rotation field property drawer is handled a bit differently but the differences should be insignificant. 
This is currently a much better alternative to using Editor.DrawDefaultInspector in your custom Transform inspector class since DrawDefaultInspector does not preserve the inspector's unique GUI layout. 
UsagePlace TransformInspector.cs into an Editor folder inside of your project's Assets folder. 
TransformInspector.cs// Reverse engineered UnityEditor.TransformInspector
 
[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
public class TransformInspector : Editor {
 
    private const float FIELD_WIDTH = 212.0f;
    private const bool WIDE_MODE = true;
 
    private const float POSITION_MAX = 100000.0f;
 
    private static GUIContent positionGUIContent = new GUIContent(LocalString("Position")
                                                                 ,LocalString("The local position of this Game Object relative to the parent."));
    private static GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation")
                                                                 ,LocalString("The local rotation of this Game Object relative to the parent."));
    private static GUIContent scaleGUIContent    = new GUIContent(LocalString("Scale")
                                                                 ,LocalString("The local scaling of this Game Object relative to the parent."));
 
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
        this.scaleProperty    = this.serializedObject.FindProperty("m_LocalScale");
    }
 
    public override void OnInspectorGUI() {
        EditorGUIUtility.wideMode = TransformInspector.WIDE_MODE;
        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - TransformInspector.FIELD_WIDTH; // align field to right of inspector
 
        this.serializedObject.Update();
 
        EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);
        this.RotationPropertyField(   this.rotationProperty, rotationGUIContent);
        EditorGUILayout.PropertyField(this.scaleProperty,    scaleGUIContent);
 
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
}
}
