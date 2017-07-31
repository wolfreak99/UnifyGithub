// Original url: http://wiki.unity3d.com/index.php/ArrayPrefs2
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/Serialization/ArrayPrefs2.cs
// File based on original modification date of: 20 June 2014, at 08:55. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
Author: Eric Haines 
Contents [hide] 
1 Description 
2 Usage 
3 Vector2, Vector3, Quaternion, Color, Bool 
4 Arrays 
5 ShowArrayType 
6 Usage Note 
7 How it Works 
8 EditorPrefsX 
9 JavaScript - PlayerPrefsX.js 
10 C# - PlayerPrefsX.cs 

DescriptionProvides a way to save and load arrays of various types from PlayerPrefs, plus Vector2, Vector3, Quaternion, and Color. Also integrates boolean saving from BoolPrefs, with a bit of spiffing up. ArrayPrefs2 supersedes the original ArrayPrefs because it's generally faster, more robust, and usually takes less space when saving. (Except string arrays, but they're still more robust anyway.) Note that preferences saved with ArrayPrefs are not compatible with ArrayPrefs2. Requires Unity 3.0. 
Usage Have this script somewhere in your project. Ideally it should be in the Standard Assets folder; this way it can be called from C# and Boo scripts. Call it PlayerPrefsX, and then you can use the following functions: 
PlayerPrefsX.SetBool 
PlayerPrefsX.GetBool 
PlayerPrefsX.SetLong (fixme: C# only) 
PlayerPrefsX.GetLong (fixme: C# only) 
PlayerPrefsX.SetVector2 
PlayerPrefsX.GetVector2 
PlayerPrefsX.SetVector3 
PlayerPrefsX.GetVector3 
PlayerPrefsX.SetQuaternion 
PlayerPrefsX.GetQuaternion 
PlayerPrefsX.SetColor 
PlayerPrefsX.GetColor 
PlayerPrefsX.SetIntArray 
PlayerPrefsX.GetIntArray 
PlayerPrefsX.SetFloatArray 
PlayerPrefsX.GetFloatArray 
PlayerPrefsX.SetBoolArray 
PlayerPrefsX.GetBoolArray 
PlayerPrefsX.SetStringArray 
PlayerPrefsX.GetStringArray 
PlayerPrefsX.SetVector2Array 
PlayerPrefsX.GetVector2Array 
PlayerPrefsX.SetVector3Array 
PlayerPrefsX.GetVector3Array 
PlayerPrefsX.SetQuaternionArray 
PlayerPrefsX.GetQuaternionArray 
PlayerPrefsX.SetColorArray 
PlayerPrefsX.GetColorArray 
Vector2, Vector3, Quaternion, Color, Bool GetVector2/SetVector2, plus Vector3, Quaternion, Color, and Bool, work pretty much like they would if they were part of PlayerPrefs. Namely, setting a value with a key stores that value, which can be retrieved later. 
var coordinates = Vector2(.4, .2);
PlayerPrefsX.SetVector2 ("Coords", coordinates);Save a string: explicitly declare the string data type or you may get missing method exception error (u3d 4.1) 
function Update()
{
  var stringy : String = "blablibla boink";
  if (Input.GetKeyDown ("home"))
  {
    PlayerPrefs.SetString("bla",  stringy );
    print ("save");
  }
}The functions return true if they succeeded, or false if a PlayerPrefsException occurred (like trying to save more than 1MB of data when using the web player). So you might want to check the return value. 
// Save the player's position and rotation
var player : GameObject;
if (!PlayerPrefsX.SetVector3 ("PlayerPosition", player.transform.position))
	print ("Saving position failed");
if (!PlayerPrefsX.SetQuaternion ("PlayerRotation", player.transform.rotation))
	print ("Saving rotation failed");When reading keys, they return the default values for the appropriate type if the key doesn't exist. 
var coordinates = PlayerPrefsX.GetVector2 ("Coords");
// coordinates = Vector2.zero if "Coords" doesn't existIf you'd rather have a different default, you can specify that instead: 
// Load the player's position and rotation
var player : GameObject;
player.transform.position = PlayerPrefsX.GetVector3 ("PlayerPosition", Vector3(100, 50, 0));
player.transform.rotation = PlayerPrefsX.GetQuaternion ("PlayerRotation", Quaternion.Euler(90, 45, 0));Arrays You can also save arrays of int, float, boolean, String, Vector2, Vector3, Quaternion, and Color. Saving works the same as above when setting variables: 
var numberArray = new int[10];
for (n in numberArray) n = Random.Range(-10, 11);
PlayerPrefsX.SetIntArray ("Numbers", numberArray);Again, you can check the return value to see if the save succeeded. Note that these functions work with built-in arrays rather than Lists, but you can convert Lists to built-in arrays easily enough: 
var aList = new List.<int>();
for (i = 0; i < 10; i++) aList.Add(i);
var anArray : int[] = aList.ToArray();
PlayerPrefsX.SetIntArray ("Numbers", anArray);Loading variables is also basically the same: 
var numberArray = PlayerPrefsX.GetIntArray ("Numbers");There are a couple of differences. If the key doesn't exist, then an empty array of the appropriate type is returned. In the example above, if "Numbers" doesn't exist, then numberArray will be the same as if you did "var numberArray = new int[0];". You can specify a default value for the array if the key doesn't exist, but since it's an array, you also have to specify how many entries it should have as the third parameter: 
var numberArray = PlayerPrefsX.GetIntArray ("Numbers", -1, 10);In this case, if "Numbers" doesn't exist, then numberArray will be an int array with 10 elements, and each element will contain -1. 
Note: when saving string arrays, each string can have a maximum of 255 characters. Also, the type of the array is saved along with the array, so trying to do "PlayerPrefsX.GetFloatArray ("Numbers")" won't work, assuming you used an int array when saving to "Numbers". 
ShowArrayType In the event that you have a saved array and you're not sure what type it is, you can use ShowArrayType and it will tell you: 
PlayerPrefsX.ShowArrayType ("Numbers");
// prints '"Numbers" is a Int32 array', assuming "Numbers" was saved using SetIntArrayUsage Note Even though these functions make it easy to save arrays, PlayerPrefs is more suitable for small amounts of data, like locally-stored top 10 lists and that sort of thing. PlayerPrefsX isn't intended to turn PlayerPrefs into a database system. If you have large amounts of data, you should use standard disk IO functions instead. 
How it Works PlayerPrefsX is built on PlayerPrefs.SetString, which is really the only way in Unity to save a preferences entry as an array (specifically, an array of chars). So, when saving arrays of floats and ints, each number is converted to 4 bytes and stored in a byte array. The total byte array is then converted to base64, and saved as a string. When saving a boolean array, each boolean is converted to a single bit in a byte array, which again is converted to a base64 string and saved. With string arrays, first an index array of bytes is built, where each byte contains the length of the respective string entry from the string array. This is converted to a base64 string, then all the entries in the string array are smooshed into a single string and appended to the index array. Vector2, Vector3, Quaternion, and Color are built on PlayerPrefsX.SetFloatArray. 
Even though System.BitConverter is not endian-aware, PlayerPrefsX is. If you load a preferences file from a big-endian system into a little-endian system (or vice versa), it will still work. While mostly theoretical, this is actually a real, if small, possibility, such as someone upgrading from an old PPC Mac to a newer Intel Mac. Anyway, you don't have to worry about it, but that's what all the funky stuff in the code with endianDiff and byteBlock is all about. 
EditorPrefsX This script will also work for saving arrays to the EditorPrefs. Take the latest copy of PlayerPrefsX below and do a search and replace for all instances of "PlayerPrefs", renaming them to "EditorPrefs". Also, because only one Enum can exist in the project with one name, do a search and replace "ArrayType" with "EditorArrayType" (be careful not to rename the function 'ShowArrayType' near the bottom). Save the file as EditorPrefsX.js and place in Editor folder. 
JavaScript - PlayerPrefsX.js // ArrayPrefs2 v 1.3
// todo: modify PlayerPrefx.js as PlayerPrefs.cs has been modified, to allow 0-length arrays.
import System.Collections.Generic;
 
static private var endianDiff1 : int;
static private var endianDiff2 : int;
static private var idx : int;
static private var byteBlock : byte[];
 
enum ArrayType {Float, Int32, Bool, String, Vector2, Vector3, Quaternion, Color}
 
static function SetBool (name : String, value : boolean) : boolean {
	try {
		PlayerPrefs.SetInt(name, value? 1 : 0);
	}
	catch (err) {
		return false;
	}
	return true;
}
 
static function GetBool (name : String) : boolean {
	return PlayerPrefs.GetInt(name) == 1;
}
 
static function GetBool (name : String, defaultValue : boolean) : boolean {
	if (PlayerPrefs.HasKey(name)) {
		return GetBool(name);
	}
	return defaultValue;
}
 
static function SetVector2 (key : String, vector : Vector2) : boolean {
	return SetFloatArray(key, [vector.x, vector.y]);
}
 
static function GetVector2 (key : String) : Vector2 {
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 2) {
		return Vector2.zero;
	}
	return Vector2(floatArray[0], floatArray[1]);
}
 
static function GetVector2 (key : String, defaultValue : Vector2) : Vector2 {
	if (PlayerPrefs.HasKey(key)) {
		return GetVector2(key);
	}
	return defaultValue;
}
 
static function SetVector3 (key : String, vector : Vector3) : boolean {
	return SetFloatArray(key, [vector.x, vector.y, vector.z]);
}
 
static function GetVector3 (key : String) : Vector3 {
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 3) {
		return Vector3.zero;
	}
	return Vector3(floatArray[0], floatArray[1], floatArray[2]);
}
 
static function GetVector3 (key : String, defaultValue : Vector3) : Vector3 {
	if (PlayerPrefs.HasKey(key)) {
		return GetVector3(key);
	}
	return defaultValue;
}
 
static function SetQuaternion (key : String, vector : Quaternion) : boolean {
	return SetFloatArray(key, [vector.x, vector.y, vector.z, vector.w]);
}
 
static function GetQuaternion (key : String) : Quaternion {
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 4) {
		return Quaternion.identity;
	}
	return Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
}
 
static function GetQuaternion (key : String, defaultValue : Quaternion) : Quaternion {
	if (PlayerPrefs.HasKey(key)) {
		return GetQuaternion(key);
	}
	return defaultValue;
}
 
static function SetColor (key : String, color : Color) : boolean {
	return SetFloatArray(key, [color.r, color.g, color.b, color.a]);
}
 
static function GetColor (key : String) : Color {
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 4) {
		return Color(0.0, 0.0, 0.0, 0.0);
	}
	return Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
}
 
static function GetColor (key : String, defaultValue : Color) : Color {
	if (PlayerPrefs.HasKey(key)) {
		return GetColor(key);
	}
	return defaultValue;
}
 
static function SetBoolArray (key : String, boolArray : boolean[]) : boolean {
	if (boolArray.Length == 0) {
		Debug.LogError ("The bool array cannot have 0 entries when setting " + key);
		return false;
	}
	// Make a byte array that's a multiple of 8 in length, plus 5 bytes to store the number of entries as an int32 (+ identifier)
	// We have to store the number of entries, since the boolArray length might not be a multiple of 8, so there could be some padded zeroes
	var bytes = new byte[(boolArray.Length + 7)/8 + 5];
	bytes[0] = System.Convert.ToByte (ArrayType.Bool);	// Identifier
	var bits = new BitArray(boolArray);
	bits.CopyTo (bytes, 5);
	Initialize();
	ConvertInt32ToBytes (boolArray.Length, bytes); // The number of entries in the boolArray goes in the first 4 bytes
 
	return SaveBytes (key, bytes);	
}
 
static function GetBoolArray (key : String) : boolean[] {
	if (PlayerPrefs.HasKey(key)) {
		var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
		if (bytes.Length < 6) {
			Debug.LogError ("Corrupt preference file for " + key);
			return new boolean[0];
		}
		if (bytes[0] != ArrayType.Bool) {
			Debug.LogError (key + " is not a boolean array");
			return new boolean[0];
		}
		Initialize();
 
		// Make a new bytes array that doesn't include the number of entries + identifier (first 5 bytes) and turn that into a BitArray
		var bytes2 = new byte[bytes.Length-5];
		System.Array.Copy(bytes, 5, bytes2, 0, bytes2.Length);
		var bits = new BitArray(bytes2);
		// Get the number of entries from the first 4 bytes after the identifier and resize the BitArray to that length, then convert it to a boolean array
		bits.Length = ConvertBytesToInt32 (bytes);
		var boolArray = new boolean[bits.Count];
		bits.CopyTo (boolArray, 0);
 
		return boolArray;
	}
	return new boolean[0];
}
 
static function GetBoolArray (key : String, defaultValue : boolean, defaultSize : int) : boolean[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetBoolArray(key);
	}
	var boolArray = new boolean[defaultSize];
	for (b in boolArray) {
		b = defaultValue;
	}
	return boolArray;
}
 
static function SetStringArray (key : String, stringArray : String[]) : boolean {
	if (stringArray.Length == 0) {
		Debug.LogError ("The string array cannot have 0 entries when setting " + key);
		return false;
	}
	var bytes = new byte[stringArray.Length + 1];
	bytes[0] = System.Convert.ToByte (ArrayType.String);	// Identifier
	Initialize();
 
	// Store the length of each string that's in stringArray, so we can extract the correct strings in GetStringArray
	for (var i = 0; i < stringArray.Length; i++) {
		if (stringArray[i] == null) {
			Debug.LogError ("Can't save null entries in the string array when setting " + key);
			return false;
		}
		if (stringArray[i].Length > 255) {
			Debug.LogError ("Strings cannot be longer than 255 characters when setting " + key);
			return false;
		}
		bytes[idx++] = stringArray[i].Length;
	}
 
	try {
		PlayerPrefs.SetString (key, System.Convert.ToBase64String (bytes) + "|" + String.Join("", stringArray));
	}
	catch (err) {
		return false;
	}
	return true;
}
 
static function GetStringArray (key : String) : String[] {
	if (PlayerPrefs.HasKey(key)) {
		var completeString = PlayerPrefs.GetString(key);
		var separatorIndex = completeString.IndexOf("|"[0]);
		if (separatorIndex < 4) {
			Debug.LogError ("Corrupt preference file for " + key);
			return new String[0];
		}
		var bytes = System.Convert.FromBase64String (completeString.Substring(0, separatorIndex));
		if (bytes[0] != ArrayType.String) {
			Debug.LogError (key + " is not a string array");
			return new String[0];
		}
		Initialize();
 
		var numberOfEntries = bytes.Length-1;
		var stringArray = new String[numberOfEntries];
		var stringIndex = separatorIndex + 1;
		for (var i = 0; i < numberOfEntries; i++) {
			var stringLength : int = bytes[idx++];
			if (stringIndex + stringLength > completeString.Length) {
				Debug.LogError ("Corrupt preference file for " + key);
				return new String[0];
			}
			stringArray[i] = completeString.Substring(stringIndex, stringLength);
			stringIndex += stringLength;
		}
 
		return stringArray;
	}
	return new String[0];
}
 
static function GetStringArray (key : String, defaultValue : String, defaultSize : int) : String[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetStringArray(key);
	}
	var stringArray = new String[defaultSize];
	for (s in stringArray) {
		s = defaultValue;
	}
	return stringArray;
}
 
static function SetIntArray (key : String, intArray : int[]) : boolean {
	return SetValue (key, intArray, ArrayType.Int32, 1, ConvertFromInt);
}
 
static function SetFloatArray (key : String, floatArray : float[]) : boolean {
	return SetValue (key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
}
 
static function SetVector2Array (key : String, vector2Array : Vector2[]) : boolean {
	return SetValue (key, vector2Array, ArrayType.Vector2, 2, ConvertFromVector2);
}
 
static function SetVector3Array (key : String, vector3Array : Vector3[]) : boolean {
	return SetValue (key, vector3Array, ArrayType.Vector3, 3, ConvertFromVector3);
}
 
static function SetQuaternionArray (key : String, quaternionArray : Quaternion[]) : boolean {
	return SetValue (key, quaternionArray, ArrayType.Quaternion, 4, ConvertFromQuaternion);
}
 
static function SetColorArray (key : String, colorArray : Color[]) : boolean {
	return SetValue (key, colorArray, ArrayType.Color, 4, ConvertFromColor);
}
 
private static function SetValue (key : String, array : IList, arrayType : ArrayType, vectorNumber : int, convert : function(IList, byte[], int)) : boolean {
	if (array.Count == 0) {
		Debug.LogError ("The " + arrayType.ToString() + " array cannot have 0 entries when setting " + key);
		return false;
	}
	var bytes = new byte[(4*array.Count)*vectorNumber + 1];
	bytes[0] = System.Convert.ToByte (arrayType);	// Identifier
	Initialize();
 
	for (var i = 0; i < array.Count; i++) {
		convert (array, bytes, i);	
	}
	return SaveBytes (key, bytes);
}
 
private static function ConvertFromInt (array : int[], bytes : byte[], i : int) {
	ConvertInt32ToBytes (array[i], bytes);
}
 
private static function ConvertFromFloat (array : float[], bytes : byte[], i : int) {
	ConvertFloatToBytes (array[i], bytes);
}
 
private static function ConvertFromVector2 (array : Vector2[], bytes : byte[], i : int) {
	ConvertFloatToBytes (array[i].x, bytes);
	ConvertFloatToBytes (array[i].y, bytes);
}
 
private static function ConvertFromVector3 (array : Vector3[], bytes : byte[], i : int) {
	ConvertFloatToBytes (array[i].x, bytes);
	ConvertFloatToBytes (array[i].y, bytes);
	ConvertFloatToBytes (array[i].z, bytes);
}
 
private static function ConvertFromQuaternion (array : Quaternion[], bytes : byte[], i : int) {
	ConvertFloatToBytes (array[i].x, bytes);
	ConvertFloatToBytes (array[i].y, bytes);
	ConvertFloatToBytes (array[i].z, bytes);
	ConvertFloatToBytes (array[i].w, bytes);
}
 
private static function ConvertFromColor (array : Color[], bytes : byte[], i : int) {
	ConvertFloatToBytes (array[i].r, bytes);
	ConvertFloatToBytes (array[i].g, bytes);
	ConvertFloatToBytes (array[i].b, bytes);
	ConvertFloatToBytes (array[i].a, bytes);
}
 
static function GetIntArray (key : String) : int[] {
	var intList = new List.<int>();
	GetValue (key, intList, ArrayType.Int32, 1, ConvertToInt);
	return intList.ToArray();
}
 
static function GetIntArray (key : String, defaultValue : int, defaultSize : int) : int[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetIntArray(key);
	}
	var intArray = new int[defaultSize];
	for (i in intArray) {
		i = defaultValue;
	}
	return intArray;
}
 
static function GetFloatArray (key : String) : float[] {
	var floatList = new List.<float>();
	GetValue (key, floatList, ArrayType.Float, 1, ConvertToFloat);
	return floatList.ToArray();
}
 
static function GetFloatArray (key : String, defaultValue : float, defaultSize : int) : float[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetFloatArray(key);
	}
	var floatArray = new float[defaultSize];
	for (f in floatArray) {
		f = defaultValue;
	}
	return floatArray;
}
 
static function GetVector2Array (key : String) : Vector2[] {
	var vector2List = new List.<Vector2>();
	GetValue (key, vector2List, ArrayType.Vector2, 2, ConvertToVector2);
	return vector2List.ToArray();
}
 
static function GetVector2Array (key : String, defaultValue : Vector2, defaultSize : int) : Vector2[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetVector2Array(key);
	}
	var vector2Array = new Vector2[defaultSize];
	for (v in vector2Array) {
		v = defaultValue;
	}
	return vector2Array;
}
 
static function GetVector3Array (key : String) : Vector3[] {
	var vector3List = new List.<Vector3>();
	GetValue (key, vector3List, ArrayType.Vector3, 3, ConvertToVector3);
	return vector3List.ToArray();
}
 
static function GetVector3Array (key : String, defaultValue : Vector3, defaultSize : int) : Vector3[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetVector3Array(key);
	}
	var vector3Array = new Vector3[defaultSize];
	for (v in vector3Array) {
		v = defaultValue;
	}
	return vector3Array;
}
 
static function GetQuaternionArray (key : String) : Quaternion[] {
	var quaternionList = new List.<Quaternion>();
	GetValue (key, quaternionList, ArrayType.Quaternion, 4, ConvertToQuaternion);
	return quaternionList.ToArray();
}
 
static function GetQuaternionArray (key : String, defaultValue : Quaternion, defaultSize : int) : Quaternion[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetQuaternionArray(key);
	}
	var quaternionArray = new Quaternion[defaultSize];
	for (v in quaternionArray) {
		v = defaultValue;
	}
	return quaternionArray;
}
 
static function GetColorArray (key : String) : Color[] {
	var colorList = new List.<Color>();
	GetValue (key, colorList, ArrayType.Color, 4, ConvertToColor);
	return colorList.ToArray();
}
 
static function GetColorArray (key : String, defaultValue : Color, defaultSize : int) : Color[] {
	if (PlayerPrefs.HasKey(key)) {
		return GetColorArray(key);
	}
	var colorArray = new Color[defaultSize];
	for (v in colorArray) {
		v = defaultValue;
	}
	return colorArray;
}
 
private static function GetValue (key : String, list : IList, arrayType : ArrayType, vectorNumber : int, convert : function(IList, byte[])) {
	if (PlayerPrefs.HasKey(key)) {
		var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
		if ((bytes.Length-1) % (vectorNumber*4) != 0) {
			Debug.LogError ("Corrupt preference file for " + key);
			return;
		}
		if (bytes[0] != arrayType) {
			Debug.LogError (key + " is not a " + arrayType.ToString() + " array");
			return;
		}
		Initialize();
 
		var end = (bytes.Length-1) / (vectorNumber*4);
		for (var i = 0; i < end; i++) {
			convert (list, bytes);
		}
	}
}
 
private static function ConvertToInt (list : List.<int>, bytes : byte[]) {
	list.Add (ConvertBytesToInt32(bytes));
}
 
private static function ConvertToFloat (list : List.<float>, bytes : byte[]) {
	list.Add (ConvertBytesToFloat(bytes));
}
 
private static function ConvertToVector2 (list : List.<Vector2>, bytes : byte[]) {
	list.Add (Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
private static function ConvertToVector3 (list : List.<Vector3>, bytes : byte[]) {
	list.Add (Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
private static function ConvertToQuaternion (list : List.<Quaternion>, bytes : byte[]) {
	list.Add (Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
private static function ConvertToColor (list : List.<Color>, bytes : byte[]) {
	list.Add (Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
static function ShowArrayType (key : String) {
	var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
	if (bytes.Length > 0) {
		var arrayType : ArrayType = bytes[0];
		Debug.Log (key + " is a " + arrayType.ToString() + " array");
	}
}
 
private static function Initialize () {
	if (System.BitConverter.IsLittleEndian) {
		endianDiff1 = 0;
		endianDiff2 = 0;
	}
	else {
		endianDiff1 = 3;
		endianDiff2 = 1;
	}
	if (byteBlock == null) {
		byteBlock = new byte[4];
	}
	idx = 1;
}
 
private static function SaveBytes (key : String, bytes : byte[]) : boolean {
	try {
		PlayerPrefs.SetString (key, System.Convert.ToBase64String (bytes));
	}
	catch (err) {
		return false;
	}
	return true;
}
 
private static function ConvertFloatToBytes (f : float, bytes : byte[]) {
	byteBlock = System.BitConverter.GetBytes (f);
	ConvertTo4Bytes (bytes);
}
 
private static function ConvertBytesToFloat (bytes : byte[]) : float {
	ConvertFrom4Bytes (bytes);
	return System.BitConverter.ToSingle (byteBlock, 0);
}
 
private static function ConvertInt32ToBytes (i : int, bytes : byte[]) {
	byteBlock = System.BitConverter.GetBytes (i);
	ConvertTo4Bytes (bytes);
}
 
private static function ConvertBytesToInt32 (bytes : byte[]) : int {
	ConvertFrom4Bytes (bytes);
	return System.BitConverter.ToInt32 (byteBlock, 0);
}
 
private static function ConvertTo4Bytes (bytes : byte[]) {
	bytes[idx  ] = byteBlock[    endianDiff1];
	bytes[idx+1] = byteBlock[1 + endianDiff2];
	bytes[idx+2] = byteBlock[2 - endianDiff2];
	bytes[idx+3] = byteBlock[3 - endianDiff1];
	idx += 4;
}
 
private static function ConvertFrom4Bytes (bytes : byte[]) {
	byteBlock[    endianDiff1] = bytes[idx  ];
	byteBlock[1 + endianDiff2] = bytes[idx+1];
	byteBlock[2 - endianDiff2] = bytes[idx+2];
	byteBlock[3 - endianDiff1] = bytes[idx+3];
	idx += 4;
}C# - PlayerPrefsX.cs For those who prefer C# I've transcribed it over. There may be some transcription errors in there as I haven't tested all code paths, but what I have tested does work. 
// ArrayPrefs2 v 1.4
 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
 
public class PlayerPrefsX
{
static private int endianDiff1;
static private int endianDiff2;
static private int idx;
static private byte [] byteBlock;
 
enum ArrayType {Float, Int32, Bool, String, Vector2, Vector3, Quaternion, Color}
 
public static bool SetBool ( String name, bool value)
{
	try
	{
		PlayerPrefs.SetInt(name, value? 1 : 0);
	}
	catch
	{
		return false;
	}
	return true;
}
 
public static bool GetBool (String name)
{
	return PlayerPrefs.GetInt(name) == 1;
}
 
public static bool GetBool (String name, bool defaultValue)
{
	return (1==PlayerPrefs.GetInt(name, defaultValue?1:0));
}
 
public static long GetLong(string key, long defaultValue)
{
	int lowBits, highBits;
	SplitLong(defaultValue, out lowBits, out highBits);
	lowBits = PlayerPrefs.GetInt(key+"_lowBits", lowBits);
	highBits = PlayerPrefs.GetInt(key+"_highBits", highBits);
 
	// unsigned, to prevent loss of sign bit.
	ulong ret = (uint)highBits;
	ret = (ret << 32);
	return (long)(ret | (ulong)(uint)lowBits);
}
 
public static long GetLong(string key)
{
	int lowBits = PlayerPrefs.GetInt(key+"_lowBits");
	int highBits = PlayerPrefs.GetInt(key+"_highBits");
 
	// unsigned, to prevent loss of sign bit.
	ulong ret = (uint)highBits;
	ret = (ret << 32);
	return (long)(ret | (ulong)(uint)lowBits);
}
 
private static void SplitLong(long input, out int lowBits, out int highBits)
{
	// unsigned everything, to prevent loss of sign bit.
	lowBits = (int)(uint)(ulong)input;
	highBits = (int)(uint)(input >> 32);
}
 
public static void SetLong(string key, long value)
{
	int lowBits, highBits;
	SplitLong(value, out lowBits, out highBits);
	PlayerPrefs.SetInt(key+"_lowBits", lowBits);
	PlayerPrefs.SetInt(key+"_highBits", highBits);
}
 
public static bool SetVector2 (String key, Vector2 vector)
{
	return SetFloatArray(key, new float[]{vector.x, vector.y});
}
 
static Vector2 GetVector2 (String key)
{
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 2)
	{
		return Vector2.zero;
	}
	return new Vector2(floatArray[0], floatArray[1]);
}
 
public static Vector2 GetVector2 (String key, Vector2 defaultValue)
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetVector2(key);
	}
	return defaultValue;
}
 
public static bool SetVector3 (String key, Vector3 vector)
{
	return SetFloatArray(key, new float []{vector.x, vector.y, vector.z});
}
 
public static Vector3 GetVector3 (String key)
{
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 3)
	{
		return Vector3.zero;
	}
	return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
}
 
public static Vector3 GetVector3 (String key, Vector3 defaultValue)
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetVector3(key);
	}
	return defaultValue;
}
 
public static bool SetQuaternion (String key, Quaternion vector)
{
	return SetFloatArray(key, new float[]{vector.x, vector.y, vector.z, vector.w});
}
 
public static Quaternion GetQuaternion (String key)
{
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 4)
	{
		return Quaternion.identity;
	}
	return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
}
 
public static Quaternion GetQuaternion (String key, Quaternion defaultValue )
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetQuaternion(key);
	}
	return defaultValue;
}
 
public static bool SetColor (String key, Color color)
{
	return SetFloatArray(key, new float[]{color.r, color.g, color.b, color.a});
}
 
public static Color GetColor (String key)
{
	var floatArray = GetFloatArray(key);
	if (floatArray.Length < 4)
	{
		return new Color(0.0f, 0.0f, 0.0f, 0.0f);
	}
	return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
}
 
public static Color GetColor (String key , Color defaultValue )
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetColor(key);
	}
	return defaultValue;
}
 
public static bool SetBoolArray (String key, bool[] boolArray)
{
	// Make a byte array that's a multiple of 8 in length, plus 5 bytes to store the number of entries as an int32 (+ identifier)
	// We have to store the number of entries, since the boolArray length might not be a multiple of 8, so there could be some padded zeroes
	var bytes = new byte[(boolArray.Length + 7)/8 + 5];
	bytes[0] = System.Convert.ToByte (ArrayType.Bool);	// Identifier
	var bits = new BitArray(boolArray);
	bits.CopyTo (bytes, 5);
	Initialize();
	ConvertInt32ToBytes (boolArray.Length, bytes); // The number of entries in the boolArray goes in the first 4 bytes
 
	return SaveBytes (key, bytes);	
}
 
public static bool[] GetBoolArray (String key)
{
	if (PlayerPrefs.HasKey(key))
	{
		var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
		if (bytes.Length < 5)
		{
			Debug.LogError ("Corrupt preference file for " + key);
			return new bool[0];
		}
		if ((ArrayType)bytes[0] != ArrayType.Bool)
		{
			Debug.LogError (key + " is not a boolean array");
			return new bool[0];
		}
		Initialize();
 
		// Make a new bytes array that doesn't include the number of entries + identifier (first 5 bytes) and turn that into a BitArray
		var bytes2 = new byte[bytes.Length-5];
		System.Array.Copy(bytes, 5, bytes2, 0, bytes2.Length);
		var bits = new BitArray(bytes2);
		// Get the number of entries from the first 4 bytes after the identifier and resize the BitArray to that length, then convert it to a boolean array
		bits.Length = ConvertBytesToInt32 (bytes);
		var boolArray = new bool[bits.Count];
		bits.CopyTo (boolArray, 0);
 
		return boolArray;
	}
	return new bool[0];
}
 
public static bool[] GetBoolArray (String key, bool defaultValue, int defaultSize) 
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetBoolArray(key);
	}
	var boolArray = new bool[defaultSize];
	for(int i=0;i<defaultSize;i++)
	{
		boolArray[i] = defaultValue;
	}
	return boolArray;
}
 
public static bool SetStringArray (String key, String[] stringArray)
{
	var bytes = new byte[stringArray.Length + 1];
	bytes[0] = System.Convert.ToByte (ArrayType.String);	// Identifier
	Initialize();
 
	// Store the length of each string that's in stringArray, so we can extract the correct strings in GetStringArray
	for (var i = 0; i < stringArray.Length; i++)
	{
		if (stringArray[i] == null)
		{
			Debug.LogError ("Can't save null entries in the string array when setting " + key);
			return false;
		}
		if (stringArray[i].Length > 255)
		{
			Debug.LogError ("Strings cannot be longer than 255 characters when setting " + key);
			return false;
		}
		bytes[idx++] = (byte)stringArray[i].Length;
	}
 
	try
	{
		PlayerPrefs.SetString (key, System.Convert.ToBase64String (bytes) + "|" + String.Join("", stringArray));
	}
	catch
	{
		return false;
	}
	return true;
}
 
public static String[] GetStringArray (String key)
{
	if (PlayerPrefs.HasKey(key)) {
		var completeString = PlayerPrefs.GetString(key);
		var separatorIndex = completeString.IndexOf("|"[0]);
		if (separatorIndex < 4) {
			Debug.LogError ("Corrupt preference file for " + key);
			return new String[0];
		}
		var bytes = System.Convert.FromBase64String (completeString.Substring(0, separatorIndex));
		if ((ArrayType)bytes[0] != ArrayType.String) {
			Debug.LogError (key + " is not a string array");
			return new String[0];
		}
		Initialize();
 
		var numberOfEntries = bytes.Length-1;
		var stringArray = new String[numberOfEntries];
		var stringIndex = separatorIndex + 1;
		for (var i = 0; i < numberOfEntries; i++)
		{
			int stringLength = bytes[idx++];
			if (stringIndex + stringLength > completeString.Length)
			{
				Debug.LogError ("Corrupt preference file for " + key);
				return new String[0];
			}
			stringArray[i] = completeString.Substring(stringIndex, stringLength);
			stringIndex += stringLength;
		}
 
		return stringArray;
	}
	return new String[0];
}
 
public static String[] GetStringArray (String key, String defaultValue, int defaultSize)
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetStringArray(key);
	}
	var stringArray = new String[defaultSize];
	for(int i=0;i<defaultSize;i++)
	{
		stringArray[i] = defaultValue;
	}
	return stringArray;
}
 
public static bool SetIntArray (String key, int[] intArray)
{
	return SetValue (key, intArray, ArrayType.Int32, 1, ConvertFromInt);
}
 
public static bool SetFloatArray (String key, float[] floatArray)
{
	return SetValue (key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
}
 
public static bool SetVector2Array (String key, Vector2[] vector2Array )
{
	return SetValue (key, vector2Array, ArrayType.Vector2, 2, ConvertFromVector2);
}
 
public static bool SetVector3Array (String key, Vector3[] vector3Array)
{
	return SetValue (key, vector3Array, ArrayType.Vector3, 3, ConvertFromVector3);
}
 
public static bool SetQuaternionArray (String key, Quaternion[] quaternionArray )
{
	return SetValue (key, quaternionArray, ArrayType.Quaternion, 4, ConvertFromQuaternion);
}
 
public static bool SetColorArray (String key, Color[] colorArray)
{
	return SetValue (key, colorArray, ArrayType.Color, 4, ConvertFromColor);
}
 
private static bool SetValue<T> (String key, T array, ArrayType arrayType, int vectorNumber, Action<T, byte[],int> convert) where T : IList
{
	var bytes = new byte[(4*array.Count)*vectorNumber + 1];
	bytes[0] = System.Convert.ToByte (arrayType);	// Identifier
	Initialize();
 
	for (var i = 0; i < array.Count; i++) {
		convert (array, bytes, i);	
	}
	return SaveBytes (key, bytes);
}
 
private static void ConvertFromInt (int[] array, byte[] bytes, int i)
{
	ConvertInt32ToBytes (array[i], bytes);
}
 
private static void ConvertFromFloat (float[] array, byte[] bytes, int i)
{
	ConvertFloatToBytes (array[i], bytes);
}
 
private static void ConvertFromVector2 (Vector2[] array, byte[] bytes, int i)
{
	ConvertFloatToBytes (array[i].x, bytes);
	ConvertFloatToBytes (array[i].y, bytes);
}
 
private static void ConvertFromVector3 (Vector3[] array, byte[] bytes, int i)
{
	ConvertFloatToBytes (array[i].x, bytes);
	ConvertFloatToBytes (array[i].y, bytes);
	ConvertFloatToBytes (array[i].z, bytes);
}
 
private static void ConvertFromQuaternion (Quaternion[] array, byte[] bytes, int i)
{
	ConvertFloatToBytes (array[i].x, bytes);
	ConvertFloatToBytes (array[i].y, bytes);
	ConvertFloatToBytes (array[i].z, bytes);
	ConvertFloatToBytes (array[i].w, bytes);
}
 
private static void ConvertFromColor (Color[] array, byte[] bytes, int i)
{
	ConvertFloatToBytes (array[i].r, bytes);
	ConvertFloatToBytes (array[i].g, bytes);
	ConvertFloatToBytes (array[i].b, bytes);
	ConvertFloatToBytes (array[i].a, bytes);
}
 
public static int[] GetIntArray (String key)
{
	var intList = new List<int>();
	GetValue (key, intList, ArrayType.Int32, 1, ConvertToInt);
	return intList.ToArray();
}
 
public static int[] GetIntArray (String key, int defaultValue, int defaultSize)
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetIntArray(key);
	}
	var intArray = new int[defaultSize];
	for (int i=0; i<defaultSize; i++)
	{
		intArray[i] = defaultValue;
	}
	return intArray;
}
 
public static float[] GetFloatArray (String key)
{
	var floatList = new List<float>();
	GetValue (key, floatList, ArrayType.Float, 1, ConvertToFloat);
	return floatList.ToArray();
}
 
public static float[] GetFloatArray (String key, float defaultValue, int defaultSize)
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetFloatArray(key);
	}
	var floatArray = new float[defaultSize];
	for (int i=0; i<defaultSize; i++)
	{
		floatArray[i] = defaultValue;
	}
	return floatArray;
}
 
public static Vector2[] GetVector2Array (String key)
{
	var vector2List = new List<Vector2>();
	GetValue (key, vector2List, ArrayType.Vector2, 2, ConvertToVector2);
	return vector2List.ToArray();
}
 
public static Vector2[] GetVector2Array (String key, Vector2 defaultValue, int defaultSize)
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetVector2Array(key);
	}
	var vector2Array = new Vector2[defaultSize];
	for(int i=0; i< defaultSize;i++)
	{
		vector2Array[i] = defaultValue;
	}
	return vector2Array;
}
 
public static Vector3[] GetVector3Array (String key)
{
	var vector3List = new List<Vector3>();
	GetValue (key, vector3List, ArrayType.Vector3, 3, ConvertToVector3);
	return vector3List.ToArray();
}
 
public static Vector3[] GetVector3Array (String key, Vector3 defaultValue, int defaultSize)
{
	if (PlayerPrefs.HasKey(key))
 
	{
		return GetVector3Array(key);
	}
	var vector3Array = new Vector3[defaultSize];
	for (int i=0; i<defaultSize;i++)
	{
		vector3Array[i] = defaultValue;
	}
	return vector3Array;
}
 
public static Quaternion[] GetQuaternionArray (String key)
{
	var quaternionList = new List<Quaternion>();
	GetValue (key, quaternionList, ArrayType.Quaternion, 4, ConvertToQuaternion);
	return quaternionList.ToArray();
}
 
public static Quaternion[] GetQuaternionArray (String key, Quaternion defaultValue, int defaultSize)
{
	if (PlayerPrefs.HasKey(key))
	{
		return GetQuaternionArray(key);
	}
	var quaternionArray = new Quaternion[defaultSize];
	for(int i=0;i<defaultSize;i++)
	{
		quaternionArray[i] = defaultValue;
	}
	return quaternionArray;
}
 
public static Color[] GetColorArray (String key)
{
	var colorList = new List<Color>();
	GetValue (key, colorList, ArrayType.Color, 4, ConvertToColor);
	return colorList.ToArray();
}
 
public static Color[] GetColorArray (String key, Color defaultValue, int defaultSize)
{
	if (PlayerPrefs.HasKey(key)) {
		return GetColorArray(key);
	}
	var colorArray = new Color[defaultSize];
	for(int i=0;i<defaultSize;i++)
	{
		colorArray[i] = defaultValue;
	}
	return colorArray;
}
 
private static void GetValue<T> (String key, T list, ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
{
	if (PlayerPrefs.HasKey(key))
	{
		var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
		if ((bytes.Length-1) % (vectorNumber*4) != 0)
		{
			Debug.LogError ("Corrupt preference file for " + key);
			return;
		}
		if ((ArrayType)bytes[0] != arrayType)
		{
			Debug.LogError (key + " is not a " + arrayType.ToString() + " array");
			return;
		}
		Initialize();
 
		var end = (bytes.Length-1) / (vectorNumber*4);
		for (var i = 0; i < end; i++)
		{
			convert (list, bytes);
		}
	}
}
 
private static void ConvertToInt (List<int> list, byte[] bytes)
{
	list.Add (ConvertBytesToInt32(bytes));
}
 
private static void ConvertToFloat (List<float> list, byte[] bytes)
{
	list.Add (ConvertBytesToFloat(bytes));
}
 
private static void ConvertToVector2 (List<Vector2> list, byte[] bytes)
{
	list.Add (new Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
private static void ConvertToVector3 (List<Vector3> list, byte[] bytes)
{
	list.Add (new Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
private static void ConvertToQuaternion (List<Quaternion> list,byte[] bytes)
{
	list.Add (new Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
private static void ConvertToColor (List<Color> list, byte[] bytes)
{
	list.Add (new Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
}
 
public static void ShowArrayType (String key)
{
	var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
	if (bytes.Length > 0)
	{
		ArrayType arrayType = (ArrayType)bytes[0];
		Debug.Log (key + " is a " + arrayType.ToString() + " array");
	}
}
 
private static void Initialize ()
{
	if (System.BitConverter.IsLittleEndian)
	{
		endianDiff1 = 0;
		endianDiff2 = 0;
	}
	else
	{
		endianDiff1 = 3;
		endianDiff2 = 1;
	}
	if (byteBlock == null)
	{
		byteBlock = new byte[4];
	}
	idx = 1;
}
 
private static bool SaveBytes (String key, byte[] bytes)
{
	try
	{
		PlayerPrefs.SetString (key, System.Convert.ToBase64String (bytes));
	}
	catch
	{
		return false;
	}
	return true;
}
 
private static void ConvertFloatToBytes (float f, byte[] bytes)
{
	byteBlock = System.BitConverter.GetBytes (f);
	ConvertTo4Bytes (bytes);
}
 
private static float ConvertBytesToFloat (byte[] bytes)
{
	ConvertFrom4Bytes (bytes);
	return System.BitConverter.ToSingle (byteBlock, 0);
}
 
private static void ConvertInt32ToBytes (int i, byte[] bytes)
{
	byteBlock = System.BitConverter.GetBytes (i);
	ConvertTo4Bytes (bytes);
}
 
private static int ConvertBytesToInt32 (byte[] bytes)
{
	ConvertFrom4Bytes (bytes);
	return System.BitConverter.ToInt32 (byteBlock, 0);
}
 
private static void ConvertTo4Bytes (byte[] bytes)
{
	bytes[idx  ] = byteBlock[    endianDiff1];
	bytes[idx+1] = byteBlock[1 + endianDiff2];
	bytes[idx+2] = byteBlock[2 - endianDiff2];
	bytes[idx+3] = byteBlock[3 - endianDiff1];
	idx += 4;
}
 
private static void ConvertFrom4Bytes (byte[] bytes)
{
	byteBlock[    endianDiff1] = bytes[idx  ];
	byteBlock[1 + endianDiff2] = bytes[idx+1];
	byteBlock[2 - endianDiff2] = bytes[idx+2];
	byteBlock[3 - endianDiff1] = bytes[idx+3];
	idx += 4;
}
}
}
