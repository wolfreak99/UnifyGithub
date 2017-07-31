// Original url: http://wiki.unity3d.com/index.php/PlayerSave
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/Serialization/PlayerSave.cs
// File based on original modification date of: 25 January 2012, at 16:19. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
Author: Sophie Houlden 
Description This class is an alternative to PlayerPrefs, it has all the same functions and behaves like PlayerPrefs, except you can specify where your save files get put, on a platform by platform basis, (you can also specify which platforms the system should just use PlayerPrefs for instead). 
Usage Just put the code in your project, and then use the class just like you would PlayerPrefs, it's functions are the same: 
PlayerSave.GetInt("a integer key", 99); //returns 99
PlayerSave.SetInt("a integer key", 1234);
PlayerSave.GetInt("a integer  key", 99); //returns 1234
 
PlayerSave.GetFloat("a float key", 99.99f); //returns 99.99
PlayerSave.SetFloat("a float key", 1234.56f);
PlayerSave.GetFloat("a float key", 99.99f); //returns 1234.56
 
PlayerSave.GetString("a string key", "Ninety-nine"); //returns Ninety-nine
PlayerSave.SetString("a string key", "One, Two, Three");
PlayerSave.GetString("a string key", "Ninety-nine"); //returns One, Two, ThreeMake sure to set each platform's save directory in the Prepare() function, and set fallback to true for any platforms you'd rather PlayerPrefs does the saving. 
Code (PlayerSave.cs) using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
 
 
/*
 * 
 * Class works almost exactly like playerprefs, but you can specify save locations by platform
 * 
 * ~~~ features ~~~
 * - option to fall back to player prefs (this is a must for some platforms)
 * - all functions of PlayerPrefs exist here, use the scripts the same way
 * 
 * ~~~ Imperfections ~~~
 * - to be able to save when the game closes (like playerprefs does), the script needs 
 *     to instantiate a GO called "_SaveCaller"
 * - no scrambling/obfusimacation of the save data right now, so users can easily hack it
 * - as with player prefs, there's no built in way to have seperate save 'slots' for the
 *     same game and save directory, add this yourself if you want it :)
 * - you might not like the way I load/save from the file, I'm no expert so
 *     if you can do better feel free :)
 * 
 * 
 * ~~~ Instructions ~~~
 * you must go through the Prepare() function, and set all relevant stuff, for all platforms
 * that you want to target.
 * don't forget to change the 'gameName' variable if your path uses it, or
 * you could overwrite other game's saves :P
 * 
 * ~~~ Licence ~~~
 * Licence for this is whatever I guess, do what you like with it,
 * I don't care about credit either. full free use :)
 * 
 * */
static public class PlayerSave {
 
	//loads all data, happens first time you set or get, but can be called earlier to avoid a jump
	static public void Prepare (){
		if (dataLoaded) return;
		dataLoaded = true;
 
 
		//default path
		savePath = Application.persistentDataPath;
		saveFile = "Save.sav";
		gameName = "Change this";
 
		//don't fallback by default
		fallback = false;
 
 
 
		//platform dependent settings:
		#if UNITY_STANDALONE_OSX
	    	fallback = false;
	    #endif
		#if UNITY_STANDALONE_WIN
			//saves to where some rockpapershotgun folk want the pc save standard to be: http://www.rockpapershotgun.com/2012/01/24/start-it-the-place-to-put-save-games
	    	fallback = false;
			savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\" + gameName + "\\Saves";
	    #endif
		#if UNITY_DASHBOARD_WIDGET
	    	fallback = true;
	    #endif
		#if UNITY_WEBPLAYER
	    	fallback = true;
	    #endif
		#if UNITY_WII
	    	fallback = true;
	    #endif
		#if UNITY_IPHONE
	    	fallback = true;
	    #endif
		#if UNITY_ANDROID
	    	fallback = true;
	    #endif
		#if UNITY_PS3
	    	fallback = true;
	    #endif
		#if UNITY_XBOX360
	    	fallback = true;
	    #endif
		#if UNITY_FLASH
	    	fallback = true;
	    #endif
 
		//if we aren't going to fallback to playerprefs, let's load the data
		if (!fallback){
			//load from file here
			LoadSavedData();
 
			//create empty gameobject that calls EndNow when the application quits
			GameObject saveCaller = new GameObject();
			saveCaller.AddComponent<SaveCaller>();
			saveCaller.name = "_SaveCaller";
		}
 
	}
 
 
	static public void SetInt (string key, int val) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			PlayerPrefs.SetInt(key, val);
			return;
		}
 
		//set the int if it already exists
		for (int i=0; i<intKeys.Count; i++){
			if (intKeys[i] == key){
				ints[i] = val;
				return;
			}
		}
 
		//int does not exist, we create it
		ints.Add(val);
		intKeys.Add(key);
	}
	static public int GetInt (string key, int defaultVal) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			return PlayerPrefs.GetInt(key, defaultVal);
		}
 
		//return int if it exists
		for (int i=0; i<intKeys.Count; i++) if (intKeys[i] == key) return ints[i];
 
		//otherwise return default value
		return defaultVal;
	}
 
	static public void SetFloat (string key, float val) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			PlayerPrefs.SetFloat(key, val);
			return;
		}
 
		//set the float if it already exists
		for (int i=0; i<floatKeys.Count; i++){
			if (floatKeys[i] == key){
				floats[i] = val;
				return;
			}
		}
 
		//float does not exist, we create it
		floats.Add(val);
		floatKeys.Add(key);
	}
	static public float  GetFloat (string key, float defaultVal) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			return PlayerPrefs.GetFloat(key, defaultVal);
		}
 
		//return float if it exists
		for (int i=0; i<floatKeys.Count; i++) if (floatKeys[i] == key) return floats[i];
 
		//otherwise return default value
		return defaultVal;
	}
 
	static public void SetString (string key, string val) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			PlayerPrefs.SetString(key, val);
			return;
		}
 
		//set the string if it already exists
		for (int i=0; i<stringKeys.Count; i++){
			if (stringKeys[i] == key){
				strings[i] = val;
				return;
			}
		}
 
		//string does not exist, we create it
		strings.Add(val);
		stringKeys.Add(key);
	}
	static public string GetString (string key, string defaultVal) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			return PlayerPrefs.GetString(key, defaultVal);
		}
 
		//return string if it exists
		for (int i=0; i<stringKeys.Count; i++) if (stringKeys[i] == key) return strings[i];
 
		//otherwise return default value
		return defaultVal;
	}
 
	static public bool HasKey (string key) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			return PlayerPrefs.HasKey(key);
		}
 
		for (int i=0; i<intKeys.Count; i++) if (intKeys[i] == key) return true;
		for (int i=0; i<floatKeys.Count; i++) if (floatKeys[i] == key) return true;
		for (int i=0; i<stringKeys.Count; i++) if (stringKeys[i] == key) return true;
 
		return false;
	}
 
	static public void DeleteKey (string key) {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			PlayerPrefs.DeleteKey(key);
			return;
		}
 
		//find relevant keys and remove them
		for (int i=0; i<ints.Count; i++){
			if (intKeys[i] == key){
				ints.RemoveAt(i);
				intKeys.RemoveAt(i);
				i--;
			}
		}
		for (int i=0; i<floats.Count; i++){
			if (floatKeys[i] == key){
				floats.RemoveAt(i);
				floatKeys.RemoveAt(i);
				i--;
			}
		}
		for (int i=0; i<strings.Count; i++){
			if (stringKeys[i] == key){
				strings.RemoveAt(i);
				stringKeys.RemoveAt(i);
				i--;
			}
		}
 
	}
 
	static public void DeleteAll () {
		Prepare();
 
		//playerprefs fallback
		if (fallback){
			PlayerPrefs.DeleteAll();
			return;
		}
 
		//reset all keys
		ints = new List<int>();
		intKeys = new List<string>();
		floats = new List<float>();
		floatKeys = new List<string>();
		strings = new List<string>();
		stringKeys = new List<string>();
	}
 
	static public void Save () {
		//playerprefs fallback
		if (fallback){
			PlayerPrefs.Save();
			return;
		}
 
		//only need to save if something has changed
		if (!dataLoaded) return;
 
 
		//save now
		string outputString = "I\n";
		for (int i=0; i<ints.Count; i++){
			outputString += intKeys[i] + "\n" + ints[i].ToString() + "\n";
		}
		outputString += "F\n";
		for (int i=0; i<floats.Count; i++){
			outputString += floatKeys[i] + "\n" + floats[i].ToString() + "\n";
		}
		outputString += "S\n";
		for (int i=0; i<strings.Count; i++){
			outputString += stringKeys[i] + "\n" + strings[i] + "\n";
		}
 
		if (!File.Exists(savePath)){
			Directory.CreateDirectory(savePath);
		}
		File.WriteAllText(savePath + "\\" + saveFile,outputString);
 
 
 
	}
 
	//data and keys
	static List<int> ints = new List<int>();
	static List<string> intKeys = new List<string>();
	static List<float> floats = new List<float>();
	static List<string> floatKeys = new List<string>();
	static List<string> strings = new List<string>();
	static List<string> stringKeys = new List<string>();
 
	//have we loaded the data yet?
	static bool dataLoaded = false;
 
	static bool fallback;
	static string savePath;
	static string saveFile;
	static string gameName;
 
 
	static void LoadSavedData(){
 
 
		if (!File.Exists(savePath + "\\" + saveFile)) return;
 
		string[] loadedFile = File.ReadAllLines(savePath + "\\" + saveFile);
 
		string loadState = "";
 
		for (int i=0; i<loadedFile.Length; i++){
			if (loadedFile[i] == "I"){
				//start loading the ints
				loadState = "ints"; 
			}else if (loadedFile[i] == "F"){
				//start loading the floats
				loadState = "floats";
			}else if (loadedFile[i] == "S"){
				//start loading the strings
				loadState = "strings";
			}else{
				//load a key
				string loadKey = loadedFile[i];
				string loadVar = loadedFile[i+1];
 
				if (loadState == "ints"){
					intKeys.Add(loadKey);
					ints.Add(MakeInt(loadVar));
				}else if (loadState == "floats"){
					floatKeys.Add(loadKey);
					floats.Add(MakeFloat(loadVar));
				}else if (loadState == "strings"){
					stringKeys.Add(loadKey);
					strings.Add(loadVar);
				}
 
				i++; //skip a line (next line is the var for this key)
			}
		}
	}	
 
	static float MakeFloat(string v) {
		return System.Convert.ToSingle(v.Trim(), new System.Globalization.CultureInfo("en-US"));
	}
 
	static int MakeInt(string v) {
		return System.Convert.ToInt32(v.Trim(), new System.Globalization.CultureInfo("en-US"));
	}
 
}
 
//class to go on object to let us know when the game is quitting:
public class SaveCaller : MonoBehaviour {
	void Start(){
		DontDestroyOnLoad(gameObject);
	}
	void OnApplicationQuit(){
		PlayerSave.Save();
	}
}
}
