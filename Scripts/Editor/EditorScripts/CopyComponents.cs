/*************************
 * Original url: http://wiki.unity3d.com/index.php/CopyComponents
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CopyComponents.cs
 * File based on original modification date of: 6 February 2012, at 21:39. 
 *
 * Author: Yuri Kunde Schlesner (yuriks) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    DescriptionThis script matches GameObjects from one hierarchy to the other, and then clones all components, transferring them to the destination. 
    UsagePlace this script in Assets/Editor and select the option in the tools menu. Specify the source and destination GameObject hierarchies, as well as an additional prefix on the destination objects. GameObjects in the source without a match in the destination will be skipped and a warning printed. NOTE: Save your scene before running this script. The functions used tend to mess up Unity's internal data structures and make it crash ocasionally. 
    C# - CopyComponents.csusing UnityEngine;
    using UnityEditor;
    using System.Collections;
     
    public class CopyComponents : ScriptableWizard {
    	public GameObject source;
    	public GameObject destination;
    	public string namePrefix = "skin_";
     
    	static GameObject findChildWithName(GameObject go, string name) {
    		foreach (Transform child_transform in go.transform) {
    			GameObject child = child_transform.gameObject;
     
    			if (child.name == name)
    				return child;
    		}
     
    		return null;
    	}
     
    	void RecursiveCopy(GameObject src, GameObject dst) {
    		Debug.Log("Copying from " + src.name + " to " + dst.name);
     
    		Component[] src_components = src.GetComponents(typeof(Component));
     
    		foreach (Component comp in src_components) {
    			if (comp is Transform || comp is Renderer || comp is Animation)
    				continue;
     
    			//Debug.Log("Adding " + comp.GetType().Name + " to " + dst.name);
    			Component dst_comp = dst.AddComponent(comp.GetType());
     
    			SerializedObject src_ser_obj = new SerializedObject(comp);
    			SerializedObject dst_ser_obj = new SerializedObject(dst_comp);
    			src_ser_obj.Update();
    			dst_ser_obj.Update();
     
    			SerializedProperty ser_prop = src_ser_obj.GetIterator();
     
    			bool enterChildren = true;
    			while (ser_prop.Next(enterChildren)) {
    				enterChildren = true;
    				string path = ser_prop.propertyPath;
     
    				bool skip = false;
    				foreach (string blacklisted_path in propertyBlacklist) {
    					if (path.EndsWith(blacklisted_path)) {
    						skip = true;
    						break;
    					}
    				}
    				if (skip) {
    					enterChildren = false;
    					continue;
    				}
     
    				//Debug.Log(path);
    				SerializedProperty dst_ser_prop = dst_ser_obj.FindProperty(path);
    				AssignSerializedProperty(ser_prop, dst_ser_prop);
    			}
     
    			dst_ser_obj.ApplyModifiedProperties();
    		}
     
    		foreach (Transform child_transform in src.transform) {
    			GameObject child = child_transform.gameObject;
     
    			string dst_object_name = namePrefix + child.name;
    			GameObject dst_object = findChildWithName(dst, dst_object_name);
     
    			if (dst_object == null) {
    				Debug.LogWarning("Didn't find matching GameObject for " + child.name);
    				continue;
    			}
     
    			RecursiveCopy(child, dst_object);
    		}
    	}
     
    	static string[] propertyBlacklist = new string[] {
    		"m_ObjectHideFlags", "m_PrefabParentObject", "m_PrefabInternal", "m_GameObject", "m_EditorHideFlags", "m_FileID", "m_PathID"
    	};
     
    	static void AssignSerializedProperty(SerializedProperty from, SerializedProperty to) {
    		if (from.propertyType != to.propertyType)
    			Debug.LogError("SerializedPropertys have different types!");
     
    		switch (from.propertyType) {
    			case SerializedPropertyType.Integer        : to.intValue             = from.intValue;             break;
    			case SerializedPropertyType.Boolean        : to.boolValue            = from.boolValue;            break;
    			case SerializedPropertyType.Float          : to.floatValue           = from.floatValue;           break;
    			case SerializedPropertyType.String         : to.stringValue          = from.stringValue;          break;
    			case SerializedPropertyType.Color          : to.colorValue           = from.colorValue;           break;
    			case SerializedPropertyType.ObjectReference: to.objectReferenceValue = from.objectReferenceValue; break;
    			case SerializedPropertyType.LayerMask      : to.intValue             = from.intValue;             break;
    			case SerializedPropertyType.Enum           : to.enumValueIndex       = from.enumValueIndex;       break;
    			case SerializedPropertyType.Vector2        : to.vector2Value         = from.vector2Value;         break;
    			case SerializedPropertyType.Vector3        : to.vector3Value         = from.vector3Value;         break;
    			case SerializedPropertyType.Rect           : to.rectValue            = from.rectValue;            break;
    			case SerializedPropertyType.ArraySize      : to.intValue             = from.intValue;             break;
    			case SerializedPropertyType.Character      : to.intValue             = from.intValue;             break;
    			case SerializedPropertyType.AnimationCurve : to.animationCurveValue  = from.animationCurveValue;  break;
    			case SerializedPropertyType.Bounds         : to.boundsValue          = from.boundsValue;          break;
    		}
    	}
     
    	void OnWizardCreate() {
    		RecursiveCopy(source, destination);
    	}
     
    	[MenuItem("Tools/Copy Components")]
    	static void DoCopyComponents() {
    		ScriptableWizard.DisplayWizard<CopyComponents>("Copy Components", "Copy");
    	}
}
}
