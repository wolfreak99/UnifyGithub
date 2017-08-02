/*************************
 * Original url: http://wiki.unity3d.com/index.php/SceneField
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SceneField.cs
 * File based on original modification date of: 5 July 2016, at 09:34. 
 *
 * Author: Fredrik Ludvigsen (Steinbitglis) 
 *
 * Description 
 *   
 * Usage 
 *   
 * C# - SceneField.cs 
 *   
 * C# - Usage.cs 
 *   
 * C# - Editor/SceneFieldDrawer.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description This script lets you refer to any scene by reference, instead of name or index. 
    Usage To refer to a scene, add SceneField.cs to your project and create a SceneField as demonstraded in Usage.cs. 
    Add SceneFieldDrawer.cs to an Editor folder for extra convenience. 
    
    
    C# - SceneField.cs using UnityEngine;
     
    [System.Serializable]
    public class SceneField : ISerializationCallbackReceiver {
     
    #if UNITY_EDITOR
        public UnityEditor.SceneAsset sceneAsset;
    #endif
     
    #pragma warning disable 414
        [SerializeField, HideInInspector]
        private string sceneName = "";
    #pragma warning restore 414
     
        // Makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField) {
    #if UNITY_EDITOR
            return System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(sceneField.sceneAsset));
    #else
            return sceneField.sceneName;
    #endif
        }
     
        public void OnBeforeSerialize() {
    #if UNITY_EDITOR
            sceneName = this;
    #endif
        }
        public void OnAfterDeserialize() {}
    }
    
    C# - Usage.cs using UnityEngine;
    using UnityEngine.SceneManagement;
     
    public class Usage: MonoBehaviour {
     
        public SceneField sceneField;
     
        void Start() {
            SceneManager.LoadScene(sceneField);
        }
    }
    
    C# - Editor/SceneFieldDrawer.cs using System.Collections.Generic;
     
    using UnityEngine;
    using UnityEditor;
     
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
     
            EditorGUI.BeginProperty(position, label, sceneAssetProp);
            EditorGUI.PropertyField(position, sceneAssetProp, label);
            EditorGUI.EndProperty();
        }
}
}
