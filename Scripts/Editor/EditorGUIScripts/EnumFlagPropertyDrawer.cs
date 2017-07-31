// Original url: http://wiki.unity3d.com/index.php/EnumFlagPropertyDrawer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorGUIScripts/EnumFlagPropertyDrawer.cs
// File based on original modification date of: 29 April 2014, at 10:38. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorGUIScripts
{

Unity3d property drawer for automatically making enums flags into mask fields in the inspector. 
Usage: Put the .cs files somewhere in your project. When you have an enum field you want to turn into a mask field in the inspector, add the EnumFlag attribute over the field. Eg: 
[EnumFlag]
MyCustomEnum thisEnum;
// This lets you give the field a custom name in the inspector.
[EnumFlag("Custom Inspector Name")]
MyCustomEnum anotherEnum;GitHub Gist 
EnumFlagAttribute.cs 
using UnityEngine;
 
public class EnumFlagAttribute : PropertyAttribute
{
	public string enumName;
 
	public EnumFlagAttribute() {}
 
	public EnumFlagAttribute(string name)
	{
		enumName = name;
	}
}EnumFlagDrawer.cs 
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
 
[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
		Enum targetEnum = GetBaseProperty<Enum>(property);
 
		string propName = flagSettings.enumName;
		if (string.IsNullOrEmpty(propName))
			propName = property.name;
 
		EditorGUI.BeginProperty(position, label, property);
		Enum enumNew = EditorGUI.EnumMaskField(position, propName, targetEnum);
		property.intValue = (int) Convert.ChangeType(enumNew, targetEnum.GetType());
		EditorGUI.EndProperty();
	}
 
	static T GetBaseProperty<T>(SerializedProperty prop)
	{
		// Separate the steps it takes to get to this property
		string[] separatedPaths = prop.propertyPath.Split('.');
 
		// Go down to the root of this serialized property
		System.Object reflectionTarget = prop.serializedObject.targetObject as object;
		// Walk down the path to get the target object
		foreach (var path in separatedPaths)
		{
			FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
			reflectionTarget = fieldInfo.GetValue(reflectionTarget);
		}
		return (T) reflectionTarget;
	}
}
}
