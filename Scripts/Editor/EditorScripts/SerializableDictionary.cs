/*************************
 * Original url: http://wiki.unity3d.com/index.php/SerializableDictionary
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SerializableDictionary.cs
 * File based on original modification date of: 20 September 2016, at 13:09. 
 *
 * Author: Fredrik Ludvigsen (Steinbitglis) 
 *
 * Description 
 *   
 * All files as a unitypackage 
 *   
 * Usage 
 *   
 * C# - Example.cs 
 *   
 * C# - SerializableDictionaryImplementations.cs 
 *   
 * C# - SerializableDictionaryDrawerImplementations.cs 
 *   
 * C# - SerializableDictionary.cs 
 *   
 * C# - SerializableDictionaryDrawer.cs 
 *   
 * C# - SerializedPropertyExtension.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description  
    This script lets you use serializable dictionaries in Unity with little boilerplate per new type. 
    You can also choose if you want to add a property drawer for the type very easily. 
    The property drawer is primarily not for complex structures, but they too work in theory. 
    All files as a unitypackage File:SerializableDictionary.zip 
    Usage For each new dictionary type, add the appropriate code snippets to SerializableDictionaryImplementations and SerializableDictionaryDrawerImplementations. 
    For convenient usage, implement the fields and getters as shown in Example. 
    C# - Example.cs using System.Collections.Generic;
     
    using UnityEngine;
     
    [CreateAssetMenu(menuName = "Example Asset")]
    public class Example : ScriptableObject {
     
        [SerializeField]
        private StringIntDictionary stringIntegerStore = StringIntDictionary.New<StringIntDictionary>();
        private Dictionary<string, int> stringIntegers {
            get { return stringIntegerStore.dictionary; }
        }
     
        [SerializeField]
        private GameObjectFloatDictionary gameObjectFloatStore = GameObjectFloatDictionary.New<GameObjectFloatDictionary>();
        private Dictionary<GameObject, float> gameObjectFloats {
            get { return gameObjectFloatStore.dictionary; }
        }
    }C# - SerializableDictionaryImplementations.cs using System;
     
    using UnityEngine;
     
    // ---------------
    //  String => Int
    // ---------------
    [Serializable]
    public class StringIntDictionary : SerializableDictionary<string, int> {}
     
    // ---------------
    //  GameObject => Float
    // ---------------
    [Serializable]
    public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}C# - SerializableDictionaryDrawerImplementations.cs using UnityEngine;
    using UnityEngine.UI;
     
    using UnityEditor;
     
    // ---------------
    //  String => Int
    // ---------------
    [UnityEditor.CustomPropertyDrawer(typeof(StringIntDictionary))]
    public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<string, int> {
        protected override SerializableKeyValueTemplate<string, int> GetTemplate() {
            return GetGenericTemplate<SerializableStringIntTemplate>();
        }
    }
    internal class SerializableStringIntTemplate : SerializableKeyValueTemplate<string, int> {}
     
    // ---------------
    //  GameObject => Float
    // ---------------
    [UnityEditor.CustomPropertyDrawer(typeof(GameObjectFloatDictionary))]
    public class GameObjectFloatDictionaryDrawer : SerializableDictionaryDrawer<GameObject, float> {
        protected override SerializableKeyValueTemplate<GameObject, float> GetTemplate() {
            return GetGenericTemplate<SerializableGameObjectFloatTemplate>();
        }
    }
    internal class SerializableGameObjectFloatTemplate : SerializableKeyValueTemplate<GameObject, float> {}C# - SerializableDictionary.cs using System.Collections.Generic;
     
    using UnityEngine;
     
    abstract public class SerializableDictionary<K, V> : ISerializationCallbackReceiver {
        [SerializeField]
        private K[] keys;
        [SerializeField]
        private V[] values;
     
        public Dictionary<K, V> dictionary;
     
        static public T New<T>() where T : SerializableDictionary<K, V>, new() {
            var result = new T();
            result.dictionary = new Dictionary<K, V>();
            return result;
        }
     
        public void OnAfterDeserialize() {
            var c = keys.Length;
            dictionary = new Dictionary<K, V>(c);
            for (int i = 0; i < c; i++) {
                dictionary[keys[i]] = values[i];
            }
            keys = null;
            values = null;
        }
     
        public void OnBeforeSerialize() {
            var c = dictionary.Count;
            keys = new K[c];
            values = new V[c];
            int i = 0;
            using (var e = dictionary.GetEnumerator())
            while (e.MoveNext()) {
                var kvp = e.Current;
                keys[i] = kvp.Key;
                values[i] = kvp.Value;
                i++;
            }
        }
    }C# - SerializableDictionaryDrawer.cs using System.Collections.Generic;
     
    using UnityEngine;
    using UnityEditor;
     
    public abstract class SerializableKeyValueTemplate<K, V> : ScriptableObject {
        public K key;
        public V value;
    }
     
    public abstract class SerializableDictionaryDrawer<K, V> : PropertyDrawer {
     
        protected abstract SerializableKeyValueTemplate<K, V> GetTemplate();
        protected T GetGenericTemplate<T>() where T: SerializableKeyValueTemplate<K, V> {
            return ScriptableObject.CreateInstance<T>();
        }
     
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
     
            EditorGUI.BeginProperty(position, label, property);
     
            var firstLine = position;
            firstLine.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(firstLine, property);
     
            if (property.isExpanded) {
                var secondLine = firstLine;
                secondLine.y += EditorGUIUtility.singleLineHeight;
     
                EditorGUIUtility.labelWidth = 50f;
     
                secondLine.x += 15f; // indentation
                secondLine.width -= 15f;
     
                var secondLine_key = secondLine;
     
                var buttonWidth = 60f;
                secondLine_key.width -= buttonWidth; // assign button
                secondLine_key.width /= 2f;
     
                var secondLine_value = secondLine_key;
                secondLine_value.x += secondLine_value.width;
                if (GetTemplateValueProp(property).hasVisibleChildren) { // if the value has children, indent to make room for fold arrow
                    secondLine_value.x += 15;
                    secondLine_value.width -= 15;
                }
     
                var secondLine_button = secondLine_value;
                secondLine_button.x += secondLine_value.width;
                secondLine_button.width = buttonWidth;
     
                var kHeight = EditorGUI.GetPropertyHeight(GetTemplateKeyProp(property));
                var vHeight = EditorGUI.GetPropertyHeight(GetTemplateValueProp(property));
                var extraHeight = Mathf.Max(kHeight, vHeight);
     
                secondLine_key.height = kHeight;
                secondLine_value.height = vHeight;
     
                EditorGUI.PropertyField(secondLine_key, GetTemplateKeyProp(property), true);
                EditorGUI.PropertyField(secondLine_value, GetTemplateValueProp(property), true);
     
                var keysProp = GetKeysProp(property);
                var valuesProp = GetValuesProp(property);
     
                var numLines = keysProp.arraySize;
     
                if (GUI.Button(secondLine_button, "Assign")) {
                    bool assignment = false;
                    for (int i = 0; i < numLines; i++) { // Try to replace existing value
                        if (SerializedPropertyExtension.EqualBasics(GetIndexedItemProp(keysProp, i), GetTemplateKeyProp(property))) {
                            SerializedPropertyExtension.CopyBasics(GetTemplateValueProp(property), GetIndexedItemProp(valuesProp, i));
                            assignment = true;
                            break;
                        }
                    }
                    if (!assignment) { // Create a new value
                        keysProp.arraySize += 1;
                        valuesProp.arraySize += 1;
                        SerializedPropertyExtension.CopyBasics(GetTemplateKeyProp(property), GetIndexedItemProp(keysProp, numLines));
                        SerializedPropertyExtension.CopyBasics(GetTemplateValueProp(property), GetIndexedItemProp(valuesProp, numLines));
                    }
                }
     
                for (int i = 0; i < numLines; i++) {
                    secondLine_key.y += extraHeight;
                    secondLine_value.y += extraHeight;
                    secondLine_button.y += extraHeight;
     
                    kHeight = EditorGUI.GetPropertyHeight(GetIndexedItemProp(keysProp, i));
                    vHeight = EditorGUI.GetPropertyHeight(GetIndexedItemProp(valuesProp, i));
                    extraHeight = Mathf.Max(kHeight, vHeight);
     
                    secondLine_key.height = kHeight;
                    secondLine_value.height = vHeight;
     
                    EditorGUI.PropertyField(secondLine_key, GetIndexedItemProp(keysProp, i), true);
                    EditorGUI.PropertyField(secondLine_value, GetIndexedItemProp(valuesProp, i), true);
     
                    if (GUI.Button(secondLine_button, "Remove")) {
                        keysProp.DeleteArrayElementAtIndex(i);
                        valuesProp.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            EditorGUI.EndProperty ();
        }
     
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;
     
            var total = EditorGUIUtility.singleLineHeight;
     
            var kHeight = EditorGUI.GetPropertyHeight(GetTemplateKeyProp(property));
            var vHeight = EditorGUI.GetPropertyHeight(GetTemplateValueProp(property));
            total += Mathf.Max(kHeight, vHeight);
     
            var keysProp = GetKeysProp(property);
            var valuesProp = GetValuesProp(property);
            int numLines = keysProp.arraySize;
            for (int i = 0; i < numLines; i++) {
                kHeight = EditorGUI.GetPropertyHeight(GetIndexedItemProp(keysProp, i));
                vHeight = EditorGUI.GetPropertyHeight(GetIndexedItemProp(valuesProp, i));
                total += Mathf.Max(kHeight, vHeight);
            }
            return total;
        }
     
        private SerializedProperty GetTemplateKeyProp(SerializedProperty mainProp) {
            return GetTemplateProp(templateKeyProp, mainProp);
        }
        private SerializedProperty GetTemplateValueProp(SerializedProperty mainProp) {
            return GetTemplateProp(templateValueProp, mainProp);
        }
     
        private SerializedProperty GetTemplateProp(Dictionary<int, SerializedProperty> source, SerializedProperty mainProp) {
            SerializedProperty p;
            if (!source.TryGetValue(mainProp.GetObjectCode(), out p)) {
                var templateObject = GetTemplate();
                var templateSerializedObject = new SerializedObject(templateObject);
                var kProp = templateSerializedObject.FindProperty("key");
                var vProp = templateSerializedObject.FindProperty("value");
                templateKeyProp[mainProp.GetObjectCode()] = kProp;
                templateValueProp[mainProp.GetObjectCode()] = vProp;
                p = source == templateKeyProp ? kProp : vProp;
            }
            return p;
        }
        private Dictionary<int, SerializedProperty> templateKeyProp = new Dictionary<int, SerializedProperty>();
        private Dictionary<int, SerializedProperty> templateValueProp = new Dictionary<int, SerializedProperty>();
     
        private SerializedProperty GetKeysProp(SerializedProperty mainProp) {
            return GetCachedProp(mainProp, "keys", keysProps); }
        private SerializedProperty GetValuesProp(SerializedProperty mainProp) {
            return GetCachedProp(mainProp, "values", valuesProps); }
     
        private SerializedProperty GetCachedProp(SerializedProperty mainProp, string relativePropertyName, Dictionary<int, SerializedProperty> source) {
            SerializedProperty p;
            int objectCode = mainProp.GetObjectCode();
            if (!source.TryGetValue(objectCode, out p))
                source[objectCode] = p = mainProp.FindPropertyRelative(relativePropertyName);
            return p;
        }
     
        private Dictionary<int, SerializedProperty> keysProps = new Dictionary<int, SerializedProperty>();
        private Dictionary<int, SerializedProperty> valuesProps = new Dictionary<int, SerializedProperty>();
     
        private Dictionary<int, Dictionary<int, SerializedProperty>> indexedPropertyDicts = new Dictionary<int, Dictionary<int, SerializedProperty>>();
     
        private SerializedProperty GetIndexedItemProp(SerializedProperty arrayProp, int index) {
            Dictionary<int, SerializedProperty> d;
            if (!indexedPropertyDicts.TryGetValue(arrayProp.GetObjectCode(), out d))
                indexedPropertyDicts[arrayProp.GetObjectCode()] = d = new Dictionary<int, SerializedProperty>();
            SerializedProperty result;
            if (!d.TryGetValue(index, out result))
                d[index] = result = arrayProp.FindPropertyRelative(string.Format("Array.data[{0}]", index));
            return result;
        }
     
    }C# - SerializedPropertyExtension.cs using UnityEditor;
     
    public static class SerializedPropertyExtension {
     
        public static int GetObjectCode(this SerializedProperty p) { // Unique code per serialized object and property path
            return p.propertyPath.GetHashCode() ^ p.serializedObject.GetHashCode();
        }
     
        public static bool EqualBasics(SerializedProperty left, SerializedProperty right) {
            if (left.propertyType != right.propertyType)
                return false;
            if (left.propertyType == SerializedPropertyType.Integer) {
                if (left.type == right.type) {
                    if (left.type == "int")
                        return left.intValue == right.intValue;
                    else
                        return left.longValue == right.longValue;
                } else {
                    return false;
                }
            } else if (left.propertyType == SerializedPropertyType.String) {
                return left.stringValue == right.stringValue;
            } else if (left.propertyType == SerializedPropertyType.ObjectReference) {
                return left.objectReferenceValue == right.objectReferenceValue;
            } else if (left.propertyType == SerializedPropertyType.Enum) {
                return left.enumValueIndex == right.enumValueIndex;
            } else if (left.propertyType == SerializedPropertyType.Boolean) {
                return left.boolValue == right.boolValue;
            } else if (left.propertyType == SerializedPropertyType.Float) {
                if (left.type == right.type) {
                    if (left.type == "float")
                        return left.floatValue == right.floatValue;
                    else
                        return left.doubleValue == right.doubleValue;
                } else {
                    return false;
                }
            } else if (left.propertyType == SerializedPropertyType.Color) {
                return left.colorValue == right.colorValue;
            } else if (left.propertyType == SerializedPropertyType.LayerMask) {
                return left.intValue == right.intValue;
            } else if (left.propertyType == SerializedPropertyType.Vector2) {
                return left.vector2Value == right.vector2Value;
            } else if (left.propertyType == SerializedPropertyType.Vector3) {
                return left.vector3Value == right.vector3Value;
            } else if (left.propertyType == SerializedPropertyType.Vector4) {
                return left.vector4Value == right.vector4Value;
            } else if (left.propertyType == SerializedPropertyType.Rect) {
                return left.rectValue == right.rectValue;
            } else if (left.propertyType == SerializedPropertyType.ArraySize) {
                return left.arraySize == right.arraySize;
            } else if (left.propertyType == SerializedPropertyType.Character) {
                return left.intValue == right.intValue;
            } else if (left.propertyType == SerializedPropertyType.AnimationCurve) {
                return false;
            } else if (left.propertyType == SerializedPropertyType.Bounds) {
                return left.boundsValue == right.boundsValue;
            } else if (left.propertyType == SerializedPropertyType.Gradient) {
                return false;
            } else if (left.propertyType == SerializedPropertyType.Quaternion) {
                return left.quaternionValue == right.quaternionValue;
            } else {
                return false;
            }
        }
     
        public static void CopyBasics(SerializedProperty source, SerializedProperty target) {
            if (source.propertyType != target.propertyType)
                return;
            if (source.propertyType == SerializedPropertyType.Integer) {
                if (source.type == target.type) {
                    if (source.type == "int")
                        target.intValue = source.intValue;
                    else
                        target.longValue = source.longValue;
                }
            } else if (source.propertyType == SerializedPropertyType.String) {
                target.stringValue = source.stringValue;
            } else if (source.propertyType == SerializedPropertyType.ObjectReference) {
                target.objectReferenceValue = source.objectReferenceValue;
            } else if (source.propertyType == SerializedPropertyType.Enum) {
                target.enumValueIndex = source.enumValueIndex;
            } else if (source.propertyType == SerializedPropertyType.Boolean) {
                target.boolValue = source.boolValue;
            } else if (source.propertyType == SerializedPropertyType.Float) {
                if (source.type == target.type) {
                    if (source.type == "float")
                        target.floatValue = source.floatValue;
                    else
                        target.doubleValue = source.doubleValue;
                }
            } else if (source.propertyType == SerializedPropertyType.Color) {
                target.colorValue = source.colorValue;
            } else if (source.propertyType == SerializedPropertyType.LayerMask) {
                target.intValue = source.intValue;
            } else if (source.propertyType == SerializedPropertyType.Vector2) {
                target.vector2Value = source.vector2Value;
            } else if (source.propertyType == SerializedPropertyType.Vector3) {
                target.vector3Value = source.vector3Value;
            } else if (source.propertyType == SerializedPropertyType.Vector4) {
                target.vector4Value = source.vector4Value;
            } else if (source.propertyType == SerializedPropertyType.Rect) {
                target.rectValue = source.rectValue;
            } else if (source.propertyType == SerializedPropertyType.ArraySize) {
                target.arraySize = source.arraySize;
            } else if (source.propertyType == SerializedPropertyType.Character) {
                target.intValue = source.intValue;
            } else if (source.propertyType == SerializedPropertyType.AnimationCurve) {
                target.animationCurveValue = source.animationCurveValue;
            } else if (source.propertyType == SerializedPropertyType.Bounds) {
                target.boundsValue = source.boundsValue;
            } else if (source.propertyType == SerializedPropertyType.Gradient) {
                // TODO?
            } else if (source.propertyType == SerializedPropertyType.Quaternion) {
                target.quaternionValue = source.quaternionValue;
            } else {
                if (source.hasChildren && target.hasChildren) {
                    var sourceIterator = source.Copy();
                    var targetIterator = target.Copy();
                    while (true) {
                        if (sourceIterator.propertyType == SerializedPropertyType.Generic) {
                            if (!sourceIterator.Next(true) || !targetIterator.Next(true))
                                break;
                        } else if (!sourceIterator.Next(false) || !targetIterator.Next(false)) {
                            break;
                        }
                        SerializedPropertyExtension.CopyBasics(sourceIterator, targetIterator);
                    }
                }
            }
        }
}
}
