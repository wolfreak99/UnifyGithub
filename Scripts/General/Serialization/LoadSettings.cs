/*************************
 * Original url: http://wiki.unity3d.com/index.php/LoadSettings
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/Serialization/LoadSettings.cs
 * File based on original modification date of: 4 December 2014, at 12:26. 
 *
 * Author: Hayden Scott-Baron (Dock) : twitter: @docky 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.Serialization
{
    
    Contents [hide] 
    1 Info 
    2 Usage 
    3 Known Issues 
    4 LoadSettings.cs 
    5 SetMyClassValues.cs 
    6 settings.txt 
    
    Info This is a handy script for grabbing a bunch of values from a text file, and storing the values. You can then create small scripts that load the values into your existing components. 
    Usage Put LoadSettings.cs anywhere. If you're using JS, you may need to put this from the Plugins folder. 
    Stick settings.txt (or whatever you prefer to call the file) into the same folder as the build EXE. In the Editor project, put settings.txt into the same folder as Assets, Library, ProjectSettings, etc. 
    You'll also need a script to load these values into your project. See the example of how this can be loaded. 
    Known Issues Seems to behave okay. Should probably be improved to return a bool based on success of GetInt, etc. 
    LoadSettings.cs using UnityEngine;
    using System.Collections;
    using System.Collections.Generic; 
    using System.IO; 
     
    // LoadSettings.cs by Hayden Scott-Baron (@docky)
    // needs a settings.txt file.
    // see the Unity wiki for usage: http://wiki.unity3d.com/index.php/LoadSettings
    // send me a tweet if you use this, please :) 
     
    public class LoadSettings : MonoBehaviour 
    {
    	public string filename = "settings.txt";
     
    	// after 20 seconds the script times out and reports an error; 
    	public float timeOut = 20f; 
     
    	// keep track of when the file has finished loading, and whether an error has occured
    	public bool fileReady = false; 
    	public bool fileError = false; 
     
    	// data for the content
    	public Dictionary<string, string> dataPairs = new Dictionary<string,string>(); 
     
    	// singleton
    	public static LoadSettings instance; 
     
    	void Awake ()
    	{
    		// set the singleton, to allow static references
    		instance = this; 
    	}
     
    	void Start () 
    	{
    		StartCoroutine ("LoadFile", filename); 	
    	}
     
    	// this allows the file loading to time out in case of a file io problem
    	IEnumerator TimeOut (float timer)
    	{
    		yield return new WaitForSeconds (timer); 
     
    		if ( !FileReady() )
    		{
    			fileError = true; 	
    			StopAllCoroutines(); 
    		}
    	}
     
    	// load the file
    	IEnumerator LoadFile (string filename)
    	{
    		StartCoroutine ("TimeOut", timeOut); 
     
    		// reformat the filepath
    		string path = Application.dataPath + "\\..\\" + filename;
     
    		// load the file, and grab the text
    		StreamReader newStream = new StreamReader( path );
    		string allText = newStream.ReadToEnd(); 
     
    		//Debug.Log ("found the following text:\n" + allText); 
     
    		// clean up the text file
    		allText = allText.Replace (" ", ""); 
    		allText = allText.Replace ("\t", ""); 
    		// split the comments onto new lines
    		allText = allText.Replace ("//", "\n//"); 
     
    		// split into lines
    		string[] lines = {}; 
    		lines = allText.Split("\n".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries); 
     
    		// abort if there aren't any lines
    		if (lines.Length < 1)
    		{
    			fileError = true; 
    			yield break; 
    		}
     
    		// keep count of successes
    		int totalLines = 0; 
     
    		// iterate through the lines, and check for valid ones
    		foreach (string item in lines)
    		{
    			if ( !item.Contains("//") )
    			{
    				if (item.Contains ("="))
    				{
    					string[] pieces = item.Split ("=".ToCharArray()); 
    					if (pieces.Length == 2)	
    					{
    						string key 			= pieces[0]; 
    						string dataValue 	= pieces[1]; 
     
    						//Debug.Log ("found data: " + key + " = " + dataValue); 
     
    						totalLines++; 
    						dataPairs.Add (key, dataValue.ToLower() ); 
    					}
    				}
    			}			
    		}
     
    		yield return null; 
     
    		//Debug.Log ("Finished reading file. " + totalLines + " items found.");
    		fileReady = true; 
    	}
     
    	public static bool FileError()
    	{
    		if (!instance)
    			return false; 
     
    		return instance.fileError; 
    	}
     
    	public static bool FileReady()
    	{
    		if (!instance)
    			return false; 
     
    		return instance.fileReady; 
    	}
     
    	public static int GetInt (string id)
    	{
    		id = id.ToLower().Trim(); 
    		//Debug.Log ("id = '" + id + "'"); 
     
    		if (instance == null)
    		{
    			//Debug.Log ("no LoadSettings instance");
    			return -1; 
    		}
     
    		bool keyExists = (instance.dataPairs.ContainsKey (id)); 
    		if (!keyExists)
    		{
    			//Debug.Log ("no matching key");
    			return -1; 
    		}
     
    		string dataForKey = instance.dataPairs[id]; 
    		int outputData = -1; 
     
    		bool canParse = int.TryParse (dataForKey, out outputData); 
    		if (!canParse)
    		{
    			//Debug.Log ("can't parse the line");
    			return -1; 
    		}
     
    		return outputData; 
    	}
     
    	public static float GetFloat (string id)
    	{
    		id = id.ToLower().Trim(); 
     
    		if (instance == null)
    		{
    			//Debug.Log ("no LoadSettings instance");
    			return -1; 
    		}
     
    		bool keyExists = (instance.dataPairs.ContainsKey (id)); 
    		if (!keyExists)
    		{
    			//Debug.Log ("no matching key");
    			return -1; 
    		}
     
    		string dataForKey = instance.dataPairs[id]; 
    		float outputData = -1; 
     
    		bool canParse = float.TryParse (dataForKey, out outputData); 
    		if (!canParse)
    		{
    			//Debug.Log ("can't parse the line");
    			return -1; 
    		}
     
    		return outputData; 
    	}
     
    	public static string GetString (string id)
    	{
    		id = id.ToLower().Trim(); 
     
     
    		if (instance == null)
    			return ""; 
     
    		bool keyExists = (instance.dataPairs.ContainsKey (id)); 
    		if (!keyExists)
    			return ""; 
     
    		string dataForKey = instance.dataPairs[id]; 
     
    		return dataForKey; 
    	}
     
    }SetMyClassValues.cs using UnityEngine;
    using System.Collections;
     
    // you may want several of these to fill in values on different classes, for example
    // GameSettings.cs could have SetGameValues.cs to set the defaults.
    // Monster.cs could have SetMonsterValues.cs to set default information on monsters.
     
    public class SetMyClassValues : MonoBehaviour 
    {
    	private MyClass myClassRef; 
     
    	// set the reference
    	void Awake ()
    	{
    		myClassRef = GetComponent<MyClass>(); 
    	}
     
    	void Start ()
    	{
    		StartCoroutine ("LoadSettingsNow"); 
    	}
     
    	// start to load the data 
    	IEnumerator LoadSettingsNow () 
    	{
    		// wait until ready
    		while ( !LoadSettings.FileError() && !LoadSettings.FileReady() )
    		{
    			yield return null;
    		}
     
    		if ( LoadSettings.FileError() )
    		{
    			Debug.LogWarning ("file error!"); 
    			yield break; 
    		}
     
    		SetValues(); 
    	}
     
    	void SetValues ()
    	{
    		string getFullscreen = LoadSettings.GetString ("fullscreen");  
    		if (getFullscreen == "yes")
    		{
    			myClassRef.SetFullscreen (false);
    		}
    		else
    		{
    			myClassRef.SetFullscreen (false);
    		}
     
    		int getLives = LoadSettings.GetInt ("lives"); 
    		if (getLives >= 0)
    			myClassRef.lives = getLives;
     
    		float getHealth = LoadSettings.GetFloat ("health"); 
    		if (costPerLevel >= 0)
    			myClassRef.health = getHealth;  
    	}
    }settings.txt (can be changed on the public string found on the LoadSettings gameobject) 
    // this is a settings file
    // ignore all the lines with comments
    // tabs, spaces and newlines are trimmed out
     
    // game settings
    defaultResolutionX 	= 800
    defaultResolutionY 	= 600
    fullscreen 		= no		// set to 'yes' or 'no'
     
    // gameplay tweaks
    lives 			= 5			// 1 - 99 are valid
    health			= 5.5		// default is 3.2 
invincible		= yes		// set to 'yes' or 'no'
}
