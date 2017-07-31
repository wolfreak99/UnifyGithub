// Original url: http://wiki.unity3d.com/index.php/PlayerPrefsX2
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/Serialization/PlayerPrefsX2.cs
// File based on original modification date of: 9 January 2016, at 07:33. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
DescriptionIt uses Reflection. You can serialize class object. Requires Unity 5.0. 
Usage Download PlayerPrefsX.cs from ArrayPrefs. And you also put PlayerPrefsX2.cs in your Project folder. If you troublesome to make file and write it, you can get it from my gist[1]] 
snippet 
using UnityEngine;
using System.Collections;
 
class Hoge
{
	public string hogePub;
	public int intPub;
	private int bar;
	private string fuga;
	private string hoge;
}
 
public class Test : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
 
		var hoge = new Hoge{hogePub="hoge", intPub=100};
		PlayerPrefsX2.SetObject("test", hoge);
 
		var a = PlayerPrefsX2.GetObject<Hoge> ("test", null);
		Debug.Log (a.hogePub);
		Debug.Log (a.intPub);
	}
}C# - PlayerPrefsX2.cs Download it and put in your Project folder. 
/*
	PlayerPrefsX2
	Author: shinriyo 
	Date: 1/9/2015
 */
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
 
public class PlayerPrefsX2 
{		
	public static T GetObject<T>(string key, T defaultValue) where T : new()
	{
		Type t = typeof(T);
		FieldInfo[] fieldInfos = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.Instance | BindingFlags.Static |
			BindingFlags.DeclaredOnly);
 
		var newObj = new T();
 
		foreach (var fieldInfo in fieldInfos)
		{
			var fieldName = fieldInfo.Name;
			var fieldKey = string.Format("{0}-{1}-{2}", t, fieldInfo.MemberType, fieldName);
 
			if(fieldInfo.FieldType == typeof(int))
			{
				var val = PlayerPrefs.GetInt (fieldKey);
				Debug.Log (val);
				fieldInfo.SetValue(newObj, (int)val);
			}	
			else if(fieldInfo.FieldType == typeof(float))
			{
				var val = PlayerPrefs.GetFloat (fieldKey);
				fieldInfo.SetValue(newObj, (float)val);
			}
			else if(fieldInfo.FieldType == typeof(string))
			{
				var val = PlayerPrefs.GetString (fieldKey);
				fieldInfo.SetValue(newObj, (string)val);
			}
			else if(fieldInfo.FieldType == typeof(long))
			{
				var val = PlayerPrefsX.GetLong (fieldKey);
				fieldInfo.SetValue(newObj, (long)val);
			}
			else if(fieldInfo.FieldType == typeof(Vector2))
			{
				var val = PlayerPrefsX.GetVector2 (fieldKey, Vector2.zero);
				fieldInfo.SetValue(newObj, (Vector2)val);
			}
			else if(fieldInfo.FieldType == typeof(Quaternion))
			{
				var val = PlayerPrefsX.GetQuaternion (fieldKey);
				fieldInfo.SetValue(newObj, (Quaternion)val);
			}
			else if(fieldInfo.FieldType == typeof(Color))
			{
				var val = PlayerPrefsX.GetColor (fieldKey);
				fieldInfo.SetValue(newObj, (Color)val);
			}
			// array.
			else if(fieldInfo.FieldType == typeof(int[]))
			{
				var val = PlayerPrefsX.GetIntArray (fieldKey);
				fieldInfo.SetValue(newObj, (int[])val);
			}
			else if(fieldInfo.FieldType == typeof(float[]))
			{
				var val = PlayerPrefsX.GetFloatArray (fieldKey);
				fieldInfo.SetValue(newObj, (float[])val);
			}
			else if(fieldInfo.FieldType == typeof(string[]))
			{
				var val = PlayerPrefsX.GetStringArray (fieldKey);
				fieldInfo.SetValue(newObj, (string[])val);
			}
			else if(fieldInfo.FieldType == typeof(Quaternion[]))
			{
				var val = PlayerPrefsX.GetQuaternionArray (fieldKey);
				fieldInfo.SetValue(newObj, (Quaternion[])val);
			}
			else if(fieldInfo.FieldType == typeof(Color[]))
			{
				var val = PlayerPrefsX.GetColorArray (fieldKey);
				fieldInfo.SetValue(newObj, (Color[])val);
			}
			// list.
			else if(fieldInfo.FieldType == typeof(List<int>))
			{
				var val = PlayerPrefsX.GetIntArray (fieldKey);
				fieldInfo.SetValue(newObj, val.OfType<int>().ToList());
			}
			else if(fieldInfo.FieldType == typeof(List<float>))
			{
				var val = PlayerPrefsX.GetFloatArray (fieldKey);
				fieldInfo.SetValue(newObj, val.OfType<float>().ToList());
			}
			else if(fieldInfo.FieldType == typeof(List<string>))
			{
				var val = PlayerPrefsX.GetStringArray (fieldKey);
				fieldInfo.SetValue(newObj, val.OfType<string>().ToList());
			}
			else if(fieldInfo.FieldType == typeof(List<Quaternion>))
			{
				var val = PlayerPrefsX.GetQuaternionArray (fieldKey);
				fieldInfo.SetValue(newObj, (Quaternion[])val);
			}
			else if(fieldInfo.FieldType == typeof(List<Color>))
			{
				var val = PlayerPrefsX.GetColorArray (fieldKey);
				fieldInfo.SetValue(newObj, (Color[])val);
			}
			// other.
			else
			{
				Debug.LogError("Un known type.");
			}
		}
 
		if (newObj != null)
		{
			return newObj;
		}
 
		return defaultValue;
	}
 
	public static bool SetObject(string name, object value)
	{
		Type t = value.GetType();
		FieldInfo[] fieldInfos = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.Instance | BindingFlags.Static |
			BindingFlags.DeclaredOnly);
 
		try
		{
			foreach (var fieldInfo in fieldInfos)
			{
				var fieldName = fieldInfo.Name;
				var fieldKey = string.Format("{0}-{1}-{2}-{3}", name, t, fieldInfo.MemberType, fieldName);
 
				if(fieldInfo.FieldType == typeof(int))
				{
					var val = (int)fieldInfo.GetValue(value);
					PlayerPrefs.SetInt (fieldKey, val);
				}	
				else if(fieldInfo.FieldType == typeof(float))
				{
					var val = (float)fieldInfo.GetValue(value);
					PlayerPrefs.SetFloat (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(string))
				{
					var val = (string)fieldInfo.GetValue(value);
					PlayerPrefs.SetString (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(long))
				{
					var val = (long)fieldInfo.GetValue(value);
					PlayerPrefsX.SetLong (fieldKey, val);
				}
				// Unity val.
				else if(fieldInfo.FieldType == typeof(Vector2))
				{
					var val = (Vector2)fieldInfo.GetValue(value);
					PlayerPrefsX.SetVector2 (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(Vector3))
				{
					var val = (Vector3)fieldInfo.GetValue(value);
					PlayerPrefsX.SetVector3 (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(Quaternion))
				{
					var val = (Quaternion)fieldInfo.GetValue(value);
					PlayerPrefsX.SetQuaternion (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(Color))
				{
					var val = (Color)fieldInfo.GetValue(value);
					PlayerPrefsX.SetColor (fieldKey, val);
				}
				// array.
				else if(fieldInfo.FieldType == typeof(int[]))
				{
					var val = (int[])fieldInfo.GetValue(value);
					PlayerPrefsX.SetIntArray (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(float[]))
				{
					var val = (float[])fieldInfo.GetValue(value);
					PlayerPrefsX.SetFloatArray (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(string[]))
				{
					var val = (string[])fieldInfo.GetValue(value);
					PlayerPrefsX.SetStringArray (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(Quaternion[]))
				{
					var val = (Quaternion[])fieldInfo.GetValue(value);
					PlayerPrefsX.SetQuaternionArray (fieldKey, val);
				}
				else if(fieldInfo.FieldType == typeof(Color[]))
				{
					var val = (Color[])fieldInfo.GetValue(value);
					PlayerPrefsX.SetColorArray (fieldKey, val);
				}
				// list.
				else if(fieldInfo.FieldType == typeof(List<int>))
				{
					var val = (List<int>)fieldInfo.GetValue(value);
					PlayerPrefsX.SetIntArray (fieldKey, val.ToArray());
				}
				else if(fieldInfo.FieldType == typeof(List<float>))
				{
					var val = (List<float>)fieldInfo.GetValue(value);
					PlayerPrefsX.SetFloatArray (fieldKey, val.ToArray());
				}
				else if(fieldInfo.FieldType == typeof(List<string>))
				{
					var val = (List<string>)fieldInfo.GetValue(value);
					PlayerPrefsX.SetStringArray (fieldKey, val.ToArray());
				}
				else if(fieldInfo.FieldType == typeof(List<Quaternion>))
				{
					var val = (Quaternion[])fieldInfo.GetValue(value);
					PlayerPrefsX.SetQuaternionArray (fieldKey, val.ToArray());
				}
				else if(fieldInfo.FieldType == typeof(List<Color>))
				{
					var val = (Color[])fieldInfo.GetValue(value);
					PlayerPrefsX.SetColorArray (fieldKey, val.ToArray());
				}
				else
				{
					Debug.LogError("Un known type.");
				}
			}
		}
		catch
		{
			return false;
		}
		return true;
	}
}
}
