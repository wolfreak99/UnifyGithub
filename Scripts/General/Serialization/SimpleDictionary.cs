// Original url: http://wiki.unity3d.com/index.php/SimpleDictionary
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/Serialization/SimpleDictionary.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
Save this as SimpleDictionary.js. Usage should be really obvious. 
var d = SimpleDictionary; 
You can directly access values and keys: d.keys, d.values 
You can Set, Get, and Remove keys: d.Set("foo", "bar"); d.Get("foo"); d.Remove("foo"); 
You can load values from and save values to "ini-style" files (i.e. key=value pairs). d.Save("test.ini"); d.Load("test.ini"); // note this overwrites all the existing values in the dictionary 
That's it. 



/*
	SimpleDictionary
	Author: Tonio Loewald
	Date: 4/27/2009
 
	Implements a simple key/value dictionary (of strings)
	Also allows loading and saving of dictionaries from text files
	"=" should not be used in key strings!
 
	Usage:
	var d = new SimpleDictionary();
 
	d.Set( "foo", "bar" );
	print( d.Get( "foo" ) ); // "bar"
 
	d.Set( "bar", "baz" );
	d.Set( "foo", "blah" );
	print( d.Get( "foo" ) ); // "blah"
 
	d.Remove( "foo" );
	print( d.Count() ); // 1
	d.Set( "foxtrot", "uniform" );
	d.Save( "test.ini" ); // file will be bar=baz\nfoxtrot=uniform\n
*/
 
import System;
import System.IO;
 
class SimpleDictionary extends ScriptableObject {
	public var keys = new Array();
	public var values = new Array();
 
	function Get( key : String ) : String {
		for(var i = 0; i < keys.length; i++){
			if( keys[i] == key ){
				return( values[i] );
			}
		}
 
		return "";
	}
 
	function Set( key : String, val : String ) {
		for(var i = 0; i < keys.length; i++){
			if( keys[i] == key ){
				values[i] = val;
				return;
			}
		}
 
		keys.push( key );
		values.push( val );
	}
 
	function Remove( key : String ){
		for(var i = 0; i < keys.length; i++){
			if( keys[i] == key ){
				keys.RemoveAt(i);
				values.RemoveAt(i);
				return;
			}
		}
		print( "SimpleDictionary.Remove failed, key not found: " + key );
	}
 
	function Save( fileName : String ){
		var sw : StreamWriter = new StreamWriter ( Application.dataPath + "/" + fileName );
		for(var i = 0; i < keys.length; i++){
			sw.WriteLine( keys[i] + "=" + values[i] );
		}
		sw.Close ();
		print ( "SimpleDictionary.Saved " + Application.dataPath + "/" + fileName );
	}
 
	function Load( fileName : String ) : SimpleDictionary {
		keys = new Array();
		values = new Array();
 
		var line : String = "-";
		var offset : int;
		try {
			var sr : StreamReader = new StreamReader ( Application.dataPath + "/" + fileName );
			line = sr.ReadLine();
		while (line != null) {
			offset = line.IndexOf("=");
				if( offset > 0 ){
					Set( line.Substring(0, offset), line.Substring(offset+1) );
				}
				line = sr.ReadLine();
		}
			sr.Close();
			print ( "SimpleDictionary.Loaded " + Application.dataPath + "/" + fileName );
		}
		catch (e) {
			print ( "SimpleDictionary.Load failed: " + Application.dataPath + "/" + fileName );
		}
	}
 
	function Count(){
		return keys.length;
	}
}
}
