/*************************
 * Original url: http://wiki.unity3d.com/index.php/Expose_properties_in_inspector
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Expose_properties_in_inspector.cs
 * File based on original modification date of: 22 February 2016, at 19:18. 
 *
 * Author: Mift (mift) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Note: An updated version of this script package (which only calls a property's setter when the value has actually changed) can be found here: ExposePropertiesInInspector_SetOnlyWhenChanged 
    Contents [hide] 
    1 Description 
    2 The Sausage 
    2.1 C# - ExposePropertyAttribute.cs 
    3 The magic 
    3.1 C# - ExposeProperties.cs 
    4 Note 
    5 Example component 
    5.1 C# - MyType.cs 
    6 The custom editor 
    6.1 C# - MyTypeEditor.cs 
    7 Conclusion 
    
    Description The code here provided is intended for those that want to hold their variables private and let them access only through get/set accessors, so further processing becomes possible. 
    The Sausage Create a new c# Script named "ExposePropertyAttribute" and paste following code : 
    C# - ExposePropertyAttribute.cs using UnityEngine;
    using System;
    using System.Collections;
     
    [AttributeUsage( AttributeTargets.Property )]
    public class ExposePropertyAttribute : Attribute
    {
     
    }The magic Now create another script in the "Assets/Editor" folder named "ExposeProperties" and paste following code: 
    C# - ExposeProperties.cs using UnityEditor;
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
     
     
    public static class ExposeProperties
    {
    	public static void Expose( PropertyField[] properties )
    	{
     
    		GUILayoutOption[] emptyOptions = new GUILayoutOption[0];
     
    		EditorGUILayout.BeginVertical( emptyOptions );
     
    		foreach ( PropertyField field in properties )
    		{
     
    			EditorGUILayout.BeginHorizontal( emptyOptions );
     
    			switch ( field.Type )
    			{
    			case SerializedPropertyType.Integer:
    					field.SetValue( EditorGUILayout.IntField( field.Name, (int)field.GetValue(), emptyOptions ) ); 
    				break;
     
    			case SerializedPropertyType.Float:
    					field.SetValue( EditorGUILayout.FloatField( field.Name, (float)field.GetValue(), emptyOptions ) );
    				break;
     
    			case SerializedPropertyType.Boolean:
    					field.SetValue( EditorGUILayout.Toggle( field.Name, (bool)field.GetValue(), emptyOptions ) );
    				break;
     
    			case SerializedPropertyType.String:
    					field.SetValue( EditorGUILayout.TextField( field.Name, (String)field.GetValue(), emptyOptions ) );
    				break;
     
    			case SerializedPropertyType.Vector2:
    					field.SetValue( EditorGUILayout.Vector2Field( field.Name, (Vector2)field.GetValue(), emptyOptions ) );
    				break;
     
    			case SerializedPropertyType.Vector3:
    					field.SetValue( EditorGUILayout.Vector3Field( field.Name, (Vector3)field.GetValue(), emptyOptions ) );
    				break;
     
     
     
    			case SerializedPropertyType.Enum:
       				        field.SetValue(EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(), emptyOptions));
    				break;
     
                            case SerializedPropertyType.ObjectReference:
    					field.SetValue(EditorGUILayout.ObjectField(field.Name,(UnityEngine.Object)field.GetValue(), field.GetPropertyType(), true, emptyOptions));
    			        break;
     
    			default:
     
    				break;
     
    			}
     
    			EditorGUILayout.EndHorizontal();
     
    		}
     
    		EditorGUILayout.EndVertical();
     
    	}
     
    	public static PropertyField[] GetProperties( System.Object obj )
    	{
     
    		List< PropertyField > fields = new List<PropertyField>();
     
    		PropertyInfo[] infos = obj.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );
     
    		foreach ( PropertyInfo info in infos )
    		{
     
    			if ( ! (info.CanRead && info.CanWrite) )
    				continue;
     
    			object[] attributes = info.GetCustomAttributes( true );
     
    			bool isExposed = false;
     
    			foreach( object o in attributes )
    			{
    				if ( o.GetType() == typeof( ExposePropertyAttribute ) )
    				{
    					isExposed = true;
    					break;
    				}
    			}
     
    			if ( !isExposed )
    				continue;
     
    			SerializedPropertyType type = SerializedPropertyType.Integer;
     
    			if( PropertyField.GetPropertyType( info, out type ) )
    			{
    				PropertyField field = new PropertyField( obj, info, type );
    				fields.Add( field );
    			}
     
    		}
     
    		return fields.ToArray();
     
    	}
     
    }
     
     
    public class PropertyField
    {
    	System.Object m_Instance;
    	PropertyInfo m_Info;
    	SerializedPropertyType m_Type;
     
    	MethodInfo m_Getter;
    	MethodInfo m_Setter;
     
    	public SerializedPropertyType Type
    	{
    		get
    		{
    			return m_Type;	
    		}
    	}
     
    	public String Name
    	{	
    		get
    		{
    			return ObjectNames.NicifyVariableName( m_Info.Name );	
    		}
    	}
     
    	public PropertyField( System.Object instance, PropertyInfo info, SerializedPropertyType type )
    	{	
     
    		m_Instance = instance;
    		m_Info = info;
    		m_Type = type;
     
    		m_Getter = m_Info.GetGetMethod();
    		m_Setter = m_Info.GetSetMethod();
    	}
     
    	public System.Object GetValue() 
    	{
    		return m_Getter.Invoke( m_Instance, null );
    	}
     
    	public void SetValue( System.Object value )
    	{
    		m_Setter.Invoke( m_Instance, new System.Object[] { value } );
    	}
     
            public Type GetPropertyType() {
    		return m_Info.PropertyType;
    	}
     
    	public static bool GetPropertyType( PropertyInfo info, out SerializedPropertyType propertyType )
    	{
     
    		propertyType = SerializedPropertyType.Generic;
     
    		Type type = info.PropertyType;
     
    		if ( type == typeof( int ) )
    		{
    			propertyType = SerializedPropertyType.Integer;
    			return true;
    		}
     
    		if ( type == typeof( float ) )
    		{
    			propertyType = SerializedPropertyType.Float;
    			return true;
    		}
     
    		if ( type == typeof( bool ) )
    		{
    			propertyType = SerializedPropertyType.Boolean;
    			return true;
    		}
     
    		if ( type == typeof( string ) )
    		{
    			propertyType = SerializedPropertyType.String;
    			return true;
    		}	
     
    		if ( type == typeof( Vector2 ) )
    		{
    			propertyType = SerializedPropertyType.Vector2;
    			return true;
    		}
     
    		if ( type == typeof( Vector3 ) )
    		{
    			propertyType = SerializedPropertyType.Vector3;
    			return true;
    		}
     
    		if ( type.IsEnum )
    		{
    			propertyType = SerializedPropertyType.Enum;
    			return true;
    		}
                    // COMMENT OUT to NOT expose custom objects/types
                    propertyType = SerializedPropertyType.ObjectReference;
    		return true;
     
    		//return false;
     
    	}
     
    }Note As you probably have seen in the code above, only following types are currently exposed, but it should be easy enough to implement other ones: 
    - Integer
    - Float
    - Boolean
    - String
    - Vector2
    - Vector3
    - Enum
    - UnityEngine.Object
    Example component Now create your type, i.e. MyType. Properties MUST have both getter and setter accessors AND [ExposeProperty] attribute set otherwise the property will not be shown. 
    In order for the property to maintain values when you hit play, you must add [SerializeField] to the field that the property is changing. Unfortunately, this exposes the field to the Editor (editing this value does not call the property getter/setter), so you have to explicitly hide it with [HideInInspector]. 
    C# - MyType.cs using UnityEngine;
    using System.Collections;
     
     
    public class MyType : MonoBehaviour {
     
    	[HideInInspector] [SerializeField] int m_SomeInt;
    	[HideInInspector] [SerializeField] float m_SomeFloat;
    	[HideInInspector] [SerializeField] bool m_SomeBool;
    	[HideInInspector] [SerializeField] string m_Etc;
     
    	[ExposeProperty]
    	public int SomeInt
    	{
    		get
    		{
    			return m_SomeInt;
    		}
    		set
    		{
    			m_SomeInt = value;	
    		}
    	}
     
    	[ExposeProperty]
    	public float SomeFloat
    	{
    		get
    		{
    			return m_SomeFloat;
    		}
    		set
    		{
    			m_SomeFloat = value;	
    		}
    	}
     
    	[ExposeProperty]
    	public bool SomeBool
    	{
    		get
    		{
    			return m_SomeBool;
    		}
    		set
    		{
    			m_SomeBool = value;	
    		}
    	}
     
    	[ExposeProperty]
    	public string SomeString
    	{
    		get
    		{
    			return m_Etc;
    		}
    		set
    		{
    			m_Etc = value;	
    		}
    	}
     
    }The custom editor Finally, create a custom editor Script in "Assets/Editor" for the type which properties you want to see exposed. I.e.: 
    C# - MyTypeEditor.cs using UnityEditor;
    using UnityEngine;
    using System.Collections;
     
    [CustomEditor( typeof( MyType ) )]
    public class MyTypeEditor : Editor {
     
     
    	MyType m_Instance;
    	PropertyField[] m_fields;
     
     
    	public void OnEnable()
    	{
    		m_Instance = target as MyType;
    		m_fields = ExposeProperties.GetProperties( m_Instance );
    	}
     
    	public override void OnInspectorGUI () {
     
    		if ( m_Instance == null )
    			return;
     
    		this.DrawDefaultInspector();
     
    		ExposeProperties.Expose( m_fields );
     
    	}
}Conclusion Well, thats pretty much the basic system, Im sure some ppl that might touch this "sauce code" will be able to make it better ( and prettier ). It does what I need it for, so Im not gonna improve it. 
}
