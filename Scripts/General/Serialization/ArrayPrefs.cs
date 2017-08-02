/*************************
 * Original url: http://wiki.unity3d.com/index.php/ArrayPrefs
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/Serialization/ArrayPrefs.cs
 * File based on original modification date of: 17 April 2012, at 19:56. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.Serialization
{
    Contents [hide] 
    1 Contributions 
    2 Description 
    3 Usage 
    4 SetVector3 
    5 GetVector3 
    6 SetIntArray 
    7 GetIntArray 
    8 SetFloatArray 
    9 GetFloatArray 
    10 SetStringArray 
    11 GetStringArray 
    12 JavaScript - PlayerPrefsX.js 
    13 C# - PlayerPrefsX.cs 
    14 C# - EditorPrefsX.cs 
    
    Contributions (Set/Get Vector3) 03/2010: Mario Madureira Fontes (fontmaster) 
    (Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 
    (Inherit PlayerPrefs Functionality) 10/2010: Daniel P. Rossi (DR9885) 
    DescriptionProvides a way to save and load arrays from PlayerPrefs. (Note: You probably want to use ArrayPrefs2 instead, which is generally faster and better.) 
    Usage Have this script somewhere in your project. Ideally it should be in the Scripts folder in Standard Assets; this way it can be called from C# and Boo scripts. Call it PlayerPrefsX, and then you can use the following functions: 
    PlayerPrefsX.SetVector3 
    PlayerPrefsX.GetVector3 
    PlayerPrefsX.SetIntArray 
    PlayerPrefsX.GetIntArray 
    PlayerPrefsX.SetFloatArray 
    PlayerPrefsX.GetFloatArray 
    PlayerPrefsX.SetStringArray 
    PlayerPrefsX.GetStringArray 
    
    
    SetVector3 static function SetVector3 (key : string, value : Vector3) : boolean 
    Description 
    Sets the value of the preference identified by key. The value in this case is a Vector3. Returns false if a PlayerPrefsException occured (for example, trying to save more than 1MB of data when using the web player), otherwise returns true. 
    // Try to save a player object position
    var player : GameObject;
    if (!PlayerPrefsX.SetVector3("PlayerPosition", player.transform.position))
    	print("Can't save a player position!");
    
    GetVector3 static function GetVector3 (key : string) : Vector3 
    Description 
    Returns the value corresponding to key in the preference file if it exists. 
    If it doesn't exist, it will return Vector3(0,0,0). 
    var player : GameObject;
    player.transform.position = PlayerPrefsX.GetVector3("PlayerPosition");
    
    SetIntArray static function SetIntArray (key : string, value : int[]) : boolean 
    Description 
    Sets the value of the preference identified by key. The value in this case is an array of integers (int[]). Returns false if a PlayerPrefsException occured (for example, trying to save more than 1MB of data when using the web player), otherwise returns true. 
    // Makes an array of integers from 1 through 10, then saves the array to a PlayerPrefs entry named "Scores"
    var myScores = new int[10];
    for (i = 0; i < myScores.Length; i++)
    	myScores[i] = i+1;
    if (!PlayerPrefsX.SetIntArray("Scores", myScores))
    	print("Can't save scores");
    
    GetIntArray static function GetIntArray (key : string) : int[] 
    Description 
    Returns the value corresponding to key in the preference file if it exists. 
    If it doesn't exist, it will return int[0]. 
    var scores = PlayerPrefsX.GetIntArray("Scores");
    static function GetIntArray (key : string, defaultValue : int, defaultSize : int) : int[] 
    Description 
    Returns the value corresponding to key in the preference file if it exists. 
    If it doesn't exist, it will return an integer array of size defaultSize, filled with value defaultValue. 
    // If the PlayerPrefs entry "Scores" doesn't exist, initialize entries to 0 and make the array have 10 elements
    var scores = PlayerPrefsX.GetIntArray("Scores", 0, 10);
    
    SetFloatArray static function SetFloatArray (key : string, value : float[]) : boolean 
    Description 
    Sets the value of the preference identified by key. The value in this case is an array of floats (float[]). Returns false if a PlayerPrefsException occured (for example, trying to save more than 1MB of data when using the web player), otherwise returns true. 
    // Makes an array of floats from 1.5 through 10.5, then saves the array to a PlayerPrefs entry named "Coordinates"
    var myCoords = new float[10];
    for (i = 0; i < myCoords.Length; i++)
    	myCoords[i] = i+1.5;
    if (!PlayerPrefsX.SetFloatArray("Coordinates", myCoords))
    	print("Can't save coordinates");
    
    GetFloatArray static function GetFloatArray (key : string) : float[] 
    Description 
    Returns the value corresponding to key in the preference file if it exists. 
    If it doesn't exist, it will return float[0]. 
    var myCoords = PlayerPrefsX.GetFloatArray("Coordinates");
    static function GetFloatArray (key : string, defaultValue : float, defaultSize : int) : float[] 
    Description 
    Returns the value corresponding to key in the preference file if it exists. 
    If it doesn't exist, it will return a float array of size defaultSize, filled with value defaultValue. 
    // If the PlayerPrefs entry "Coordinates" doesn't exist, initialize entries to .5 and make the array have 10 elements
    var myCoords = PlayerPrefsX.GetFloatArray("Coordinates", .5, 10);
    
    SetStringArray static function SetStringArray (key : string, value : String[]) : boolean 
    Description 
    Sets the value of the preference identified by key. The value in this case is an array of strings (String[]). Returns false if a PlayerPrefsException occured (for example, trying to save more than 1MB of data when using the web player), otherwise returns true. 
    // Makes an array of names, then saves the array to a PlayerPrefs entry named "Names"
    var names = new String[10];
    for (i = 0; i < names.Length; i++)
    	names[i] = "Player"+(i+1);
    if (!PlayerPrefsX.SetStringArray("Names", names))
    	print("Can't save names");
    static function SetStringArray (key : string, value : String[], separator : char) : boolean 
    Description 
    By default, a line break is used as a separator when storing array entries. If your string array contains strings that include line breaks, then reading the array back would fail. Therefore, you can choose to supply a different separator character. 
    // Makes an array of names, then saves the array to a PlayerPrefs entry named "Names"
    var names = new String[10];
    for (i = 0; i < names.Length; i++)
    	names[i] = "FirstName\nLastName"+(i+1);
    if (!PlayerPrefsX.SetStringArray("Names", names, "#"[0]))
    	print("Can't save names");
    
    GetStringArray static function GetStringArray (key : string) : string[] 
    Description 
    Returns the value corresponding to key in the preference file if it exists. 
    If it doesn't exist, it will return String[0]. 
    var names = PlayerPrefsX.GetStringArray("Names");
    static function GetStringArray (key : string, separator : char) : string[] 
    Description 
    If you specified a separator character when saving a string array, you can supply it when reading that array back. 
    var names = PlayerPrefsX.GetStringArray("Names", "#"[0]);
    static function GetStringArray (key : string, defaultValue : String, defaultSize : int) : string[] 
    Description 
    If key doesn't exist, it will return a String array of size defaultSize, filled with value defaultValue. 
    var names = PlayerPrefsX.GetStringArray("Names", "Player", 10);
    static function GetStringArray (key : string, separator : char, defaultValue : String, defaultSize : int) : string[] 
    Description 
    If you specified a separator character when saving a string array, you can supply it when reading that array back, and if key doesn't exist, it will return a String array of size defaultSize, filled with value defaultValue. 
    var names = PlayerPrefsX.GetStringArray("Names", "#"[0], "FirstName\nLastName", 10);
    
    JavaScript - PlayerPrefsX.js // Site of this script: http://www.unifycommunity.com/wiki/index.php?title=ArrayPrefs
    // Created by: Eric Haines (Eric5h5)
    // Contribution (Set/Get Vector3) 03/2010: Mario Madureira Fontes (fontmaster)
     
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
     
    static function SetIntArray (key : String, intArray : int[]) : boolean {
    	if (intArray.Length == 0) return false;
     
    	var sb = new System.Text.StringBuilder();
    	for (i = 0; i < intArray.Length-1; i++) {
    		sb.Append(intArray[i]).Append("|");
    	}
    	sb.Append(intArray[i]);
     
    	try {
    		PlayerPrefs.SetString(key, sb.ToString());
    	}
    	catch (err) {
    		return false;
    	}
    	return true;
    }
     
    static function GetIntArray (key : String) : int[] {
    	if (PlayerPrefs.HasKey(key)) {
    		var stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
    		var intArray = new int[stringArray.Length];
    		for (i = 0; i < stringArray.Length; i++) {
    			intArray[i] = parseInt(stringArray[i]);
    		}
    		return intArray;
    	}
    	return new int[0];
    }
     
    static function GetIntArray (key : String, defaultValue : int, defaultSize : int) : int[] {
    	if (PlayerPrefs.HasKey(key)) {
    		return GetIntArray(key);
    	}
    	var intArray = new int[defaultSize];
    	for (i = 0; i < defaultSize; i++) {
    		intArray[i] = defaultValue;
    	}
    	return intArray;
    }
     
    static function SetFloatArray (key : String, floatArray : float[]) : boolean {
    	if (floatArray.Length == 0) return false;
     
    	var sb = new System.Text.StringBuilder();
    	for (i = 0; i < floatArray.Length-1; i++) {
    		sb.Append(floatArray[i]).Append("|");
    	}
    	sb.Append(floatArray[i]);
     
    	try {
    		PlayerPrefs.SetString(key, sb.ToString());
    	}
    	catch (err) {
    		return false;
    	}
    	return true;
    }
     
    static function GetFloatArray (key : String) : float[] {
    	if (PlayerPrefs.HasKey(key)) {
    		var stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
    		var floatArray = new float[stringArray.Length];
    		for (i = 0; i < stringArray.Length; i++) {
    			floatArray[i] = parseFloat(stringArray[i]);
    		}
    		return floatArray;
    	}
    	return new float[0];
    }
     
    static function GetFloatArray (key : String, defaultValue : float, defaultSize : int) : float[] {
    	if (PlayerPrefs.HasKey(key)) {
    		return GetFloatArray(key);
    	}
    	var floatArray = new float[defaultSize];
    	for (i = 0; i < defaultSize; i++) {
    		floatArray[i] = defaultValue;
    	}
    	return floatArray;
    }
     
    static function SetStringArray (key : String, stringArray : String[], separator : char) : boolean {
    	if (stringArray.Length == 0) return false;
     
    	try {
    		PlayerPrefs.SetString(key, String.Join(separator.ToString(), stringArray));
    	}
    	catch (err) {
    		return false;
    	}
    	return true;
    }
     
    static function SetStringArray (key : String, stringArray : String[]) : boolean {
    	if (!SetStringArray(key, stringArray, "\n"[0])) {
    		return false;
    	}
    	return true;
    }
     
    static function GetStringArray (key : String, separator : char) : String[] {
    	if (PlayerPrefs.HasKey(key)) {
    		return PlayerPrefs.GetString(key).Split(separator);
    	}
    	return new String[0];
    }
     
    static function GetStringArray (key : String) : String[] {
    	if (PlayerPrefs.HasKey(key)) {
    		return PlayerPrefs.GetString(key).Split("\n"[0]);
    	}
    	return new String[0];
    }
     
    static function GetStringArray (key : String, separator : char, defaultValue : String, defaultSize : int) : String[] {
    	if (PlayerPrefs.HasKey(key)) {
    		return PlayerPrefs.GetString(key).Split(separator);
    	}
    	var stringArray = new String[defaultSize];
    	for (i = 0; i < defaultSize; i++) {
    		stringArray[i] = defaultValue;
    	}
    	return stringArray;
    }
     
    static function GetStringArray (key : String, defaultValue : String, defaultSize : int) : String[] {
    	return GetStringArray(key, "\n"[0], defaultValue, defaultSize);
    }C# - PlayerPrefsX.cs // Contribution (Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885)
    // Contribution (Created Bool Array) 10/2010: Daniel P. Rossi (DR9885)
    // Contribution (Made functions public) 01/2011: Bren
     
    using UnityEngine;
    using System;
     
    public static class PlayerPrefsX
    {
        #region Vector 3
     
        /// <summary>
        /// Stores a Vector3 value into a Key
        /// </summary>
        public static bool SetVector3(string key, Vector3 vector)
        {
            return SetFloatArray(key, new float[3] { vector.x, vector.y, vector.z });
        }
     
        /// <summary>
        /// Finds a Vector3 value from a Key
        /// </summary>
        public static Vector3 GetVector3(string key)
        {
            float[] floatArray = GetFloatArray(key);
            if (floatArray.Length < 3)
                return Vector3.zero;
            return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
        }
     
        #endregion
     
        #region Bool Array
     
        /// <summary>
        /// Stores a Bool Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetBoolArray(string key, params bool[] boolArray)
        {
            if (boolArray.Length == 0) return false;
     
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < boolArray.Length - 1; i++)
                sb.Append(boolArray[i]).Append("|");
            sb.Append(boolArray[boolArray.Length - 1]);
     
            try { PlayerPrefs.SetString(key, sb.ToString()); }
            catch (Exception e) { return false; }
            return true;
        }
     
        /// <summary>
        /// Returns a Bool Array from a Key
        /// </summary>
        public static bool[] GetBoolArray(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string[] stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
                bool[] boolArray = new bool[stringArray.Length];
                for (int i = 0; i < stringArray.Length; i++)
                    boolArray[i] = Convert.ToBoolean(stringArray[i]);
                return boolArray;
            }
            return new bool[0];
        }
     
        /// <summary>
        /// Returns a Bool Array from a Key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static bool[] GetBoolArray(string key, bool defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
                return GetBoolArray(key);
            bool[] boolArray = new bool[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                boolArray[i] = defaultValue;
            return boolArray;
        }
     
        #endregion
     
        #region Int Array
     
        /// <summary>
        /// Stores a Int Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetIntArray(string key, params int[] intArray)
        {
            if (intArray.Length == 0) return false;
     
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < intArray.Length - 1; i++)
                sb.Append(intArray[i]).Append("|");
            sb.Append(intArray[intArray.Length - 1]);
     
            try { PlayerPrefs.SetString(key, sb.ToString()); }
            catch (Exception e) { return false; }
            return true;
        }
     
        /// <summary>
        /// Returns a Int Array from a Key
        /// </summary>
        public static int[] GetIntArray(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string[] stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
                int[] intArray = new int[stringArray.Length];
                for (int i = 0; i < stringArray.Length; i++)
                    intArray[i] = Convert.ToInt32(stringArray[i]);
                return intArray;
            }
            return new int[0];
        }
     
        /// <summary>
        /// Returns a Int Array from a Key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static int[] GetIntArray(string key, int defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
                return GetIntArray(key);
            int[] intArray = new int[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                intArray[i] = defaultValue;
            return intArray;
        }
     
        #endregion
     
        #region Float Array
     
        /// <summary>
        /// Stores a Float Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetFloatArray(string key, params float[] floatArray)
        {
            if (floatArray.Length == 0) return false;
     
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < floatArray.Length - 1; i++)
                sb.Append(floatArray[i]).Append("|");
            sb.Append(floatArray[floatArray.Length - 1]);
     
            try
            {
                PlayerPrefs.SetString(key, sb.ToString());
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
     
        /// <summary>
        /// Returns a Float Array from a Key
        /// </summary>
        public static float[] GetFloatArray(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string[] stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
                float[] floatArray = new float[stringArray.Length];
                for (int i = 0; i < stringArray.Length; i++)
                    floatArray[i] = Convert.ToSingle(stringArray[i]);
                return floatArray;
            }
            return new float[0];
        }
     
        /// <summary>
        /// Returns a String Array from a Key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static float[] GetFloatArray(string key, float defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
                return GetFloatArray(key);
            float[] floatArray = new float[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                floatArray[i] = defaultValue;
            return floatArray;
        }
     
        #endregion
     
        #region String Array
     
        /// <summary>
        /// Stores a String Array or Multiple Parameters into a Key w/ specific char seperator
        /// </summary>
        public static bool SetStringArray(string key, char separator, params string[] stringArray)
        {
            if (stringArray.Length == 0) return false;
            try
            { PlayerPrefs.SetString(key, String.Join(separator.ToString(), stringArray)); }
            catch (Exception e)
            { return false; }
            return true;
        }
     
        /// <summary>
        /// Stores a Bool Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetStringArray(string key, params string[] stringArray)
        {
            if (!SetStringArray(key, "\n"[0], stringArray))
                return false;
            return true;
        }
     
        /// <summary>
        /// Returns a String Array from a key & char seperator
        /// </summary>
        public static string[] GetStringArray(string key, char separator)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key).Split(separator);
            return new string[0];
        }
     
        /// <summary>
        /// Returns a Bool Array from a key
        /// </summary>
        public static string[] GetStringArray(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key).Split("\n"[0]);
            return new string[0];
        }
     
        /// <summary>
        /// Returns a String Array from a key & char seperator
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static string[] GetStringArray(string key, char separator, string defaultValue, int defaultSize)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key).Split(separator);
            string[] stringArray = new string[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                stringArray[i] = defaultValue;
            return stringArray;
        }
     
        /// <summary>
        /// Returns a String Array from a key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static String[] GetStringArray(string key, string defaultValue, int defaultSize)
        {
            return GetStringArray(key, "\n"[0], defaultValue, defaultSize);
        }
     
        #endregion
    }C# - EditorPrefsX.cs // Contribution (Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885)
    // Contribution (Created Bool Array) 10/2010: Daniel P. Rossi (DR9885)
    // Contribution (Made functions public) 01/2011: Bren
    // Contribution (Changed PlayerPrefs to EditorPrefs from PlayerPrefsX.cs) 17/04/2012 Mentalogicus 
     
    using UnityEditor;
    using UnityEngine;
    using System;
     
    public static class EditorPrefsX
    {
        #region Vector 3
     
        /// <summary>
        /// Stores a Vector3 value into a Key
        /// </summary>
        public static bool SetVector3(string key, Vector3 vector)
        {
            return SetFloatArray(key, new float[3] { vector.x, vector.y, vector.z });
        }
     
        /// <summary>
        /// Finds a Vector3 value from a Key
        /// </summary>
        public static Vector3 GetVector3(string key)
        {
            float[] floatArray = GetFloatArray(key);
            if (floatArray.Length < 3)
                return Vector3.zero;
            return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
        }
     
        #endregion
     
        #region Bool Array
     
        /// <summary>
        /// Stores a Bool Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetBoolArray(string key, params bool[] boolArray)
        {
            if (boolArray.Length == 0) return false;
     
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < boolArray.Length - 1; i++)
                sb.Append(boolArray[i]).Append("|");
            sb.Append(boolArray[boolArray.Length - 1]);
     
            try { EditorPrefs.SetString(key, sb.ToString()); }
            catch (Exception e) { return false; }
            return true;
        }
     
        /// <summary>
        /// Returns a Bool Array from a Key
        /// </summary>
        public static bool[] GetBoolArray(string key)
        {
            if (EditorPrefs.HasKey(key))
            {
                string[] stringArray = EditorPrefs.GetString(key).Split("|"[0]);
                bool[] boolArray = new bool[stringArray.Length];
                for (int i = 0; i < stringArray.Length; i++)
                    boolArray[i] = Convert.ToBoolean(stringArray[i]);
                return boolArray;
            }
            return new bool[0];
        }
     
        /// <summary>
        /// Returns a Bool Array from a Key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static bool[] GetBoolArray(string key, bool defaultValue, int defaultSize)
        {
            if (EditorPrefs.HasKey(key))
                return GetBoolArray(key);
            bool[] boolArray = new bool[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                boolArray[i] = defaultValue;
            return boolArray;
        }
     
        #endregion
     
        #region Int Array
     
        /// <summary>
        /// Stores a Int Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetIntArray(string key, params int[] intArray)
        {
            if (intArray.Length == 0) return false;
     
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < intArray.Length - 1; i++)
                sb.Append(intArray[i]).Append("|");
            sb.Append(intArray[intArray.Length - 1]);
     
            try { EditorPrefs.SetString(key, sb.ToString()); }
            catch (Exception e) { return false; }
            return true;
        }
     
        /// <summary>
        /// Returns a Int Array from a Key
        /// </summary>
        public static int[] GetIntArray(string key)
        {
            if (EditorPrefs.HasKey(key))
            {
                string[] stringArray = EditorPrefs.GetString(key).Split("|"[0]);
                int[] intArray = new int[stringArray.Length];
                for (int i = 0; i < stringArray.Length; i++)
                    intArray[i] = Convert.ToInt32(stringArray[i]);
                return intArray;
            }
            return new int[0];
        }
     
        /// <summary>
        /// Returns a Int Array from a Key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static int[] GetIntArray(string key, int defaultValue, int defaultSize)
        {
            if (EditorPrefs.HasKey(key))
                return GetIntArray(key);
            int[] intArray = new int[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                intArray[i] = defaultValue;
            return intArray;
        }
     
        #endregion
     
        #region Float Array
     
        /// <summary>
        /// Stores a Float Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetFloatArray(string key, params float[] floatArray)
        {
            if (floatArray.Length == 0) return false;
     
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < floatArray.Length - 1; i++)
                sb.Append(floatArray[i]).Append("|");
            sb.Append(floatArray[floatArray.Length - 1]);
     
            try
            {
                EditorPrefs.SetString(key, sb.ToString());
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
     
        /// <summary>
        /// Returns a Float Array from a Key
        /// </summary>
        public static float[] GetFloatArray(string key)
        {
            if (EditorPrefs.HasKey(key))
            {
                string[] stringArray = EditorPrefs.GetString(key).Split("|"[0]);
                float[] floatArray = new float[stringArray.Length];
                for (int i = 0; i < stringArray.Length; i++)
                    floatArray[i] = Convert.ToSingle(stringArray[i]);
                return floatArray;
            }
            return new float[0];
        }
     
        /// <summary>
        /// Returns a String Array from a Key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static float[] GetFloatArray(string key, float defaultValue, int defaultSize)
        {
            if (EditorPrefs.HasKey(key))
                return GetFloatArray(key);
            float[] floatArray = new float[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                floatArray[i] = defaultValue;
            return floatArray;
        }
     
        #endregion
     
        #region String Array
     
        /// <summary>
        /// Stores a String Array or Multiple Parameters into a Key w/ specific char seperator
        /// </summary>
        public static bool SetStringArray(string key, char separator, params string[] stringArray)
        {
            if (stringArray.Length == 0) return false;
            try
            { EditorPrefs.SetString(key, String.Join(separator.ToString(), stringArray)); }
            catch (Exception e)
            { return false; }
            return true;
        }
     
        /// <summary>
        /// Stores a Bool Array or Multiple Parameters into a Key
        /// </summary>
        public static bool SetStringArray(string key, params string[] stringArray)
        {
            if (!SetStringArray(key, "\n"[0], stringArray))
                return false;
            return true;
        }
     
        /// <summary>
        /// Returns a String Array from a key & char seperator
        /// </summary>
        public static string[] GetStringArray(string key, char separator)
        {
            if (EditorPrefs.HasKey(key))
                return EditorPrefs.GetString(key).Split(separator);
            return new string[0];
        }
     
        /// <summary>
        /// Returns a Bool Array from a key
        /// </summary>
        public static string[] GetStringArray(string key)
        {
            if (EditorPrefs.HasKey(key))
                return EditorPrefs.GetString(key).Split("\n"[0]);
            return new string[0];
        }
     
        /// <summary>
        /// Returns a String Array from a key & char seperator
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static string[] GetStringArray(string key, char separator, string defaultValue, int defaultSize)
        {
            if (EditorPrefs.HasKey(key))
                return EditorPrefs.GetString(key).Split(separator);
            string[] stringArray = new string[defaultSize];
            for (int i = 0; i < defaultSize; i++)
                stringArray[i] = defaultValue;
            return stringArray;
        }
     
        /// <summary>
        /// Returns a String Array from a key
        /// Note: Uses default values to initialize if no key was found
        /// </summary>
        public static String[] GetStringArray(string key, string defaultValue, int defaultSize)
        {
            return GetStringArray(key, "\n"[0], defaultValue, defaultSize);
        }
     
        #endregion
}
}
