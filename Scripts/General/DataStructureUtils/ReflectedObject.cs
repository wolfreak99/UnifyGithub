// Original url: http://wiki.unity3d.com/index.php/ReflectedObject
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/DataStructureUtils/ReflectedObject.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.DataStructureUtils
{
Contents [hide] 
1 About 
1.1 Script Version 
1.2 Script Revision 
2 Description 
3 Script - C# 
4 References 

About ReflectedObject Script 
Reflects a target object, providing quick access by name to reading and writing its fields and properties, and calling its methods. 
Script Version 0.1.0.0 
Script Revision 5/24/2011 


Description The ReflectedObject is useful when you need to read and write, or call the methods of, a raw object instance. Create an instance of ReflectedObject on a target object to build a collection of that object's fields, properties, and methods which can then be quickly accessed by name. Fields and properties can be read or written to, and fields in particular can return values as bools, colors, floats, integers, strings, and vector3. Fields also have type-safe methods which will return true if the field can be returned as the desired type and will fill a reference parameter with the return value, and will return false if they cannot. In addition to reading and writing fields and properties, methods can be called and allow passing parameters and returning values. 

You should create only one ReflectedObject per object instance and reuse it as long as possible. You should call Dispose() on the ReflectedObject when you are finished with it in order to release the internal reference to the root object that was reflected. 


Script - C# using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
 
/// <summary>
/// Reflects a target object, providing quick access by name to reading and writing its fields and properties, and calling its methods.
/// </summary>
/// <!-- 
/// Version 0.1.0.0
/// By Reed Kimble
/// Last Revision 5/24/2011
/// -->
/// <remarks>
/// The ReflectedObject is useful when you need to read and write, or call the methods of, a raw object instance.  Create an instance
/// of ReflectedObject on a target object to build a collection of that object's fields, properties, and methods which can then be
/// quickly accessed by name.  Fields and properties can be read or written to, and fields in particular can return values as bools,
/// colors, floats, integers, strings, and vector3. Fields also have type-safe methods which will return true if the field can be
/// returned as the desired type and will fill a reference parameter with the return value, and will return false if they cannot.
/// In addition to reading and writing fields and properties, methods can be called and allow passing parameters and returning values.
/// 
/// You should create only one ReflectedObject per object instance and reuse it as long as possible.  Be sure to call Dispose() on the
/// ReflectedObject when you are finished with it in order to release the internal reference to the root object that was reflected.
/// </remarks>
public class ReflectedObject : System.IDisposable
{
    private object _RootObject;
    /// <summary>
    /// The object instance reflected by this ReflectedObject.
    /// </summary>
    public object RootObject { get { return _RootObject; } }
 
    /// <summary>
    /// Stores the list of reflected fields found on the root object.
    /// </summary>
    private Dictionary<string, FieldInfo> _Fields = new Dictionary<string, FieldInfo>();
    /// <summary>
    /// Stores the list of reflected methods found on the root object.
    /// </summary>
    private Dictionary<string, MethodInfo> _Methods = new Dictionary<string, MethodInfo>();
    /// <summary>
    /// Stores the list of reflected properties found on the root object.
    /// </summary>
    private Dictionary<string, PropertyInfo> _Properties = new Dictionary<string, PropertyInfo>();
 
    /// <summary>
    /// Creates a new instance of ReflectedObject for the specified object instance.
    /// </summary>
    /// <param name="rootObject">The object to reflect.</param>
    public ReflectedObject(object rootObject)
    {
        System.Type rootType = rootObject.GetType();
        foreach (FieldInfo field in rootType.GetFields())
        {
            _Fields.Add(field.Name, field);
        }
        foreach (MethodInfo method in rootType.GetMethods())
        {
            _Methods.Add(method.Name, method);
        }
        foreach (PropertyInfo property in rootType.GetProperties())
        {
            _Properties.Add(property.Name, property);
        }
    }
 
    /// <summary>
    /// Checks to see if this object contains the named field.
    /// </summary>
    /// <param name="fieldName">The name of the field to look for.</param>
    /// <returns>True if this object contains the named field, otherwise false.</returns>
    public bool ContainsField(string fieldName)
    { return _Fields.ContainsKey(fieldName); }
 
    /// <summary>
    /// Checks to see if this object contains the named method.
    /// </summary>
    /// <param name="methodName">The name of the method to look for.</param>
    /// <returns>True if this object contains the named method, otherwise false.</returns>
    public bool ContainsMethod(string methodName)
    { return _Methods.ContainsKey(methodName); }
 
    /// <summary>
    /// Checks to see if this object contains the named property.
    /// </summary>
    /// <param name="propertyName">The name of the property to look for.</param>
    /// <returns>True if this object contains the named property, otherwise false.</returns>
    public bool ContainsProperty(string propertyName)
    { return _Properties.ContainsKey(propertyName); }
 
    /// <summary>
    /// Executes the method named using the supplied parameters. Use null if the method takes no parameters.
    /// </summary>
    /// <param name="methodName">The name of the method to execute.</param>
    /// <param name="parameters">The methods parameters in order, or null if the method has no parameters.</param>
    /// <returns>The return value of the method, or null if the method is a void subroutine.</returns>
    public object ExecuteMethod(string methodName, params object[] parameters)
    {
        if (ContainsMethod(methodName))
        {
            return _Methods[methodName].Invoke(_RootObject, parameters);
        }
        return null;
    }
 
    /// <summary>
    /// Gets the expected value type for the named field.
    /// </summary>
    /// <param name="fieldName">The name of the field to check.</param>
    /// <returns>The System.Type of the field's value.</returns>
    public System.Type GetFieldType(string fieldName)
    {
        if (_Fields.ContainsKey(fieldName))
        {
            return _Fields[fieldName].FieldType;
        }
        return null;
    }
 
    /// <summary>
    /// Gets the name of the expected value type for the named field.
    /// </summary>
    /// <param name="fieldName">The name of the field to check.</param>
    /// <returns>The string name of the System.Type of the field's value.</returns>
    public string GetFieldTypeName(string fieldName)
    {
        if (_Fields.ContainsKey(fieldName))
        {
            return _Fields[fieldName].FieldType.Name;
        }
        return null;
    }
 
    /// <summary>
    /// Gets the expected value type for the named property.
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <returns>The System.Type of the property's value.</returns>
    public System.Type GetPropertyType(string propertyName)
    {
        if (_Properties.ContainsKey(propertyName))
        {
            return _Properties[propertyName].PropertyType;
        }
        return null;
    }
 
    /// <summary>
    /// Gets the name of the expected value type for the named property.
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <returns>The string name of the System.Type of the property's value.</returns>
    public string GetPropertyTypeName(string propertyName)
    {
        if (_Properties.ContainsKey(propertyName))
        {
            return _Properties[propertyName].PropertyType.Name;
        }
        return null;
    }
 
    /// <summary>
    /// Returns the value of the named field as a boolean. Be sure the field requested is a boolean type, or use TryGetFieldBool().
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <returns>The boolean value of the field.</returns>
    public bool GetFieldBool(string fieldName)
    { return (bool)GetFieldValue(fieldName); }
 
    /// <summary>
    /// Returns the value of the named field as a color. Be sure the field requested is a color type, or use TryGetFieldColor().
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <returns>The color value of the field.</returns>
    public UnityEngine.Color GetFieldColor(string fieldName)
    { return (UnityEngine.Color)GetFieldValue(fieldName); }
 
    /// <summary>
    /// Returns the value of the named field as a float. Be sure the field requested is a float type, or use TryGetFieldFloat().
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <returns>The float value of the field.</returns>
    public float GetFieldFloat(string fieldName)
    { return (float)GetFieldValue(fieldName); }
 
    /// <summary>
    /// Returns the value of the named field as an integer. Be sure the field requested is an integer type, or use TryGetFieldInteger().
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <returns>The integer value of the field.</returns>
    public float GetFieldInteger(string fieldName)
    { return (int)GetFieldValue(fieldName); }
 
    /// <summary>
    /// Returns the value of the named field as a string. Be sure the field requested is a string type, or use TryGetFieldString().
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <returns>The string value of the field.</returns>
    public string GetFieldString(string fieldName)
    { return (string)GetFieldValue(fieldName); }
 
    /// <summary>
    /// Returns the value of the named field as a Vector3. Be sure the field requested is a Vector3 type, or use TryGetFieldVector3().
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <returns>The Vector3 value of the field.</returns>
    public UnityEngine.Vector3 GetFieldVector3(string fieldName)
    { return (UnityEngine.Vector3)GetFieldValue(fieldName); }
 
    /// <summary>
    /// Returns the value of the named field as an object.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <returns>The object value of the field.</returns>
    public object GetFieldValue(string fieldName)
    {
        if (ContainsField(fieldName))
        {
            return _Fields[fieldName].GetValue(_RootObject);
        }
        return null;
    }
 
    /// <summary>
    /// Returns the value of the named property as an object.
    /// </summary>
    /// <param name="propertyName">The name of the field whose value should be returned.</param>
    /// <returns>The object value of the property.</returns>
    public object GetPropertyValue(string propertyName)
    {
        if (ContainsProperty(propertyName))
        {
            return _Properties[propertyName].GetValue(_RootObject, null);
        }
        return null;
    }
 
    /// <summary>
    /// Sets the value of the named field to the specified object.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be set.</param>
    /// <param name="value">The object value the field is set to.</param>
    /// <returns>True if the field is found and set, otherwise false.</returns>
    public bool SetFieldValue(string fieldName, object value)
    {
        if (ContainsField(fieldName))
        {
            _Fields[fieldName].SetValue(_RootObject, value);
            return true;
        }
        return false;
    }
 
    /// <summary>
    /// Sets the value of the named property to the specified object.
    /// </summary>
    /// <param name="propertyName">The name of the property whose value should be set.</param>
    /// <param name="value">The object value the property is set to.</param>
    /// <returns>True if the property is found and set, otherwise false.</returns>
    public bool SetPropertyValue(string propertyName, object value)
    {
        if (ContainsProperty(propertyName))
        {
            _Properties[propertyName].SetValue(_RootObject, value, null);
        }
        return false;
    }
 
    /// <summary>
    /// Gets the value of the named field as a boolean, if it is of the appropriate type and returns true, otherwise returns false.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <param name="result">The boolean object instance used to return the field value, if found.</param>
    /// <returns>True if the field is found and the result is returned, otherwise false.</returns>
    public bool TryGetFieldBool(string fieldName, ref bool result)
    {
        if (GetFieldType(fieldName) == typeof(bool))
        {
            result = (bool)GetFieldValue(fieldName);
            return true;
        }
        return false;
    }
 
    /// <summary>
    /// Gets the value of the named field as a Color, if it is of the appropriate type and returns true, otherwise returns false.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <param name="result">The Color object instance used to return the field value, if found.</param>
    /// <returns>True if the field is found and the result is returned, otherwise false.</returns>
    public bool TryGetFieldColor(string fieldName, ref UnityEngine.Color result)
    {
        if (GetFieldType(fieldName) == typeof(UnityEngine.Color))
        {
            result = (UnityEngine.Color)GetFieldValue(fieldName);
            return true;
        }
        return false;
    }
 
    /// <summary>
    /// Gets the value of the named field as a float, if it is of the appropriate type and returns true, otherwise returns false.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <param name="result">The float object instance used to return the field value, if found.</param>
    /// <returns>True if the field is found and the result is returned, otherwise false.</returns>
    public bool TryGetFieldFloat(string fieldName, ref float result)
    {
        if (GetFieldType(fieldName) == typeof(float))
        {
            result = (float)GetFieldValue(fieldName);
            return true;
        }
        return false;
    }
 
    /// <summary>
    /// Gets the value of the named field as a integer, if it is of the appropriate type and returns true, otherwise returns false.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <param name="result">The integer object instance used to return the field value, if found.</param>
    /// <returns>True if the field is found and the result is returned, otherwise false.</returns>
    public bool TryGetFieldInteger(string fieldName, ref int result)
    {
        if (GetFieldType(fieldName) == typeof(int))
        {
            result = (int)GetFieldValue(fieldName);
            return true;
        }
        return false;
    }
 
    /// <summary>
    /// Gets the value of the named field as a string, if it is of the appropriate type and returns true, otherwise returns false.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <param name="result">The string object instance used to return the field value, if found.</param>
    /// <returns>True if the field is found and the result is returned, otherwise false.</returns>
    public bool TryGetFieldString(string fieldName, ref string result)
    {
        if (GetFieldType(fieldName) == typeof(string))
        {
            result = (string)GetFieldValue(fieldName);
            return true;
        }
        return false;
    }
 
    /// <summary>
    /// Gets the value of the named field as a Vector3, if it is of the appropriate type and returns true, otherwise returns false.
    /// </summary>
    /// <param name="fieldName">The name of the field whose value should be returned.</param>
    /// <param name="result">The Vector3 object instance used to return the field value, if found.</param>
    /// <returns>True if the field is found and the result is returned, otherwise false.</returns>
    public bool TryGetFieldVector3(string fieldName, ref UnityEngine.Vector3 result)
    {
        if (GetFieldType(fieldName) == typeof(UnityEngine.Vector3))
        {
            result = (UnityEngine.Vector3)GetFieldValue(fieldName);
            return true;
        }
        return false;
    }
 
    /// <summary>
    /// Clears stored reflection information and releases reference to root object.
    /// </summary>
    void System.IDisposable.Dispose()
    {
        _Fields.Clear();
        _Methods.Clear();
        _Properties.Clear();
        _RootObject = null;
    }
}References None. This is an original script. 
}
