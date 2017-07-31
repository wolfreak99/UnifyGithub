// Original url: http://wiki.unity3d.com/index.php/ObjectCopier
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/DataStructureUtils/ObjectCopier.cs
// File based on original modification date of: 13 March 2012, at 23:41. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.DataStructureUtils
{
Author: Berenger 
Description Instead of implementing the IClonable interface, that script allow you to clone an object with serialization and deserialization. Thus, the object must be serializable. 
Source : http://stackoverflow.com/questions/78536/cloning-objects-in-c-sharp. 
Usage This script uses .Net Extension, so you can use it like that (MyClass and all it's components must be serializable) : 
MyClass obj1 = new MyClass();
MyClass obj2 = obj1.Clone();

CSharp - ObjectCopier.cs using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
 
/// <summary>
/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
/// 
/// Provides a method for performing a deep copy of an object.
/// Binary Serialization is used to perform the copy.
/// </summary>
public static class ObjectCopier
{
    /// <summary>
    /// Perform a deep Copy of the object.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T Clone<T>(this T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }
 
        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }
 
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
     }
}
}
