// Original url: http://wiki.unity3d.com/index.php/ParallelKeyDictionary
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/DataStructureUtils/ParallelKeyDictionary.cs
// File based on original modification date of: 26 March 2012, at 20:12. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.DataStructureUtils
{
Author : Berenger, Mars 2012 
Contents [hide] 
1 Description 
2 Example JS 
3 Example CSharp 
4 ParallelKeyDictionary.cs 

Description You can use this data structure as a Dictionary. The difference is that you can add the same key several times, then access either the first occurence of that key or a list containing them all. 
Example JS 	var test : ParallelKeyDictionary.< int, String > = new ParallelKeyDictionary.< int, String >();
	test.Add(0, "first");
	test.Add(1, "second");
	test.Add(1, "third");
	test.Add(2, "fourth");
 
	print( test[0] ); // first
	print( test[1] ); // second
	print( test[2] ); // third
	print( test[3] ); // Null
 
	var acc : String = "";
	for( var s : String in test.values )
		acc += s + "\t";
	print( acc ); // first second third fourth
 
	acc = "";
	for( var s : String in test.GetAll(1) )
		acc += s + "\t";		
	print( acc ); // second third
 
	print( test.ContainsKey(0) ); // true
	print( test.ContainsKey(3) ); // false
	var o : String;
	if( test.TryGetFirstValue(3, o) )
		print( o ); // Doesn't go here
	print( test.Count(1) ); // 2
	print( test.Count() ); // 4Example CSharp 	ParallelKeyDictionary< int, string > test = new ParallelKeyDictionary< int, string >();
	test.Add(0, "first");
	test.Add(1, "second");
	test.Add(1, "third");
	test.Add(2, "fourth");
 
	print( test[0] ); // first
	print( test[1] ); // second
	print( test[2] ); // third
	print( test[3] ); // fourth
 
	string acc = "";
	foreach( string s in test.values )
		acc += s + "\t";
	print( acc ); // first second third fourth
 
	acc = "";
	foreach( string s in test.GetAll(1) )
		acc += s + "\t";		
	print( acc ); // second third
 
	print( test.ContainsKey(0) ); // true
	print( test.ContainsKey(3) ); // false
	string o;
	if( test.TryGetFirstValue(3, out o) )
		print( o ); // Doesn't go here
	print( test.Count(1) ); // 2
	print( test.Count() ); // 4ParallelKeyDictionary.cs using System;
using System.Collections.Generic;
 
// http://stackoverflow.com/questions/146204/duplicate-keys-in-net-dictionaries
// KeyValuePair http://msdn.microsoft.com/en-us/library/3db765db.aspx
/// <summary>
/// Identical-Safe-Key Dictionary Class
/// </summary>	
/// <typeparam name="K">Key Type</typeparam>
/// <typeparam name="V">Value Type</typeparam>
public class ParallelKeyDictionary<K, V>
{
    private List<KeyValuePair<K, V>> list;
    public List<V> values;
 
    public ParallelKeyDictionary()
    {
        list = new List<KeyValuePair<K, V>>();
        values = new List<V>();
    }
 
    /// <summary>
    /// Add a value with a key. the key doesn't have to be unique in that dictionary, that's the point.
    /// </summary>	
    /// <typeparam name="key">The key the value will be stored at.</typeparam>
    /// <typeparam name="value">The value that is to be stored.</typeparam>
    public void Add(K key, V value) { list.Add(new KeyValuePair<K, V>(key, value)); values.Add(value); }
    /// <summary>
    /// Remove the first occurence of key.
    /// </summary>	
    /// <typeparam name="key">The key to remove.</typeparam>
    public void RemoveFirst(K key)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].Key.Equals(key))
            {
                values.RemoveAt(i);
                list.RemoveAt(i);
                return;
            }
    }
    /// <summary>
    /// Remove all occurences of key.
    /// </summary>	
    /// <typeparam name="key">The key to remove.</typeparam>
    public void RemoveAll(K key)
    {
        while (ContainsKey(key))
            RemoveFirst(key);
    }
 
    /// <summary>
    /// Get the value at the first occurence of key. Return the default value if key isn't found.
    /// </summary>	
    /// <typeparam name="key">This is the key you're looking for.</typeparam>
    /// <returns>The value if the key is found, default otherwise.</returns>
    public V GetFirst(K key)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].Key.Equals(key))
                return list[i].Value;
 
        return default(V);
    }
    /// <summary>
    /// Get the value at the first occurence of key. Return the default value if key isn't found.
    /// </summary>	
    /// <typeparam name="key">This is the key you're looking for.</typeparam>
    /// <returns>The value if the key is found, default otherwise.</returns>
    public V this[K key] { get { return GetFirst(key); } }
 
    /// <summary>
    /// Get a list of values with that key. The returned list is never null.
    /// </summary>	
    /// <typeparam name="key">This is the key you're looking for.</typeparam>
    /// <returns>A list of values with that key. It is never null.</returns>
    public List<V> GetAll(K key)
    {
        List<V> result = new List<V>();
        for (int i = 0; i < list.Count; i++)
            if (list[i].Key.Equals(key))
                result.Add( list[i].Value );
 
        return result;
    }
    /// <summary>
    /// Count the number of occurences of that key.
    /// </summary>	
    /// <typeparam name="key">This is the key you're looking for.</typeparam>
    /// <returns>The number of occurences of that key.</returns>
    public int Count(K key) { return GetAll(key).Count; }
    /// <summary>
    /// The number of elements in the dictionary.
    /// </summary>	
    /// <returns>The number of elements in the dictionary.</returns>
    public int Count() { return list.Count; }
 
    /// <summary>
    /// Get the first value of that key. If it isn't found, return false.
    /// </summary>	
    /// <typeparam name="key">This is the key you're looking for.</typeparam>
    /// <typeparam name="val">The output val.</typeparam>
    /// <returns>Either it found the key or not.</returns>
    public bool TryGetFirstValue(K key, out V val)
    {
        val = GetFirst(key);
        try { return !val.Equals(default(V)); } // If default is null, exception.
        catch{ return false; }
    }
 
    /// <summary>
    /// Check if the dictionary contains at least one such key.
    /// </summary>	
    /// <typeparam name="key">This is the key you're looking for.</typeparam>
    /// <returns>Either it contains the key or not.</returns>
    public bool ContainsKey(K key)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].Key.Equals(key))
                return true;
 
        return false;
    }
 
    public override string ToString()
    {
        string acc = "";
        for (int i = 0; i < Count(); i++)
            acc += string.Format("[{0}] Key : {1}, Value : {2}\n", i, list[i].Key.ToString(), list[i].Value.ToString());
 
        return acc;
    } 
}
}
