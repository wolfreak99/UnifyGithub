/*************************
 * Original url: http://wiki.unity3d.com/index.php/TextManager
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/TextManager.cs
 * File based on original modification date of: 21 January 2012, at 17:04. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    Contents [hide] 
    1 Disclaimer 
    2 Description 
    3 Usage 
    4 TextManager.cs 
    5 TextManager.js 
    
    Disclaimer Basically all credits go to kenlem from the forums: http://forum.unity3d.com/threads/35617-TextManager-Localization-Script 
    Description Uses gettext along the lines of poEdit for translating text the GNU way! 
    The concept is really simple as you can see under Usage and all you need is POT and PO files created by any po editor, or even just use your text editor of choice with the standards in mind. 
    Usage // This will reset to default POT Language
    TextManager.LoadLanguage(null);
     
    // This will load filename-en.po
    TextManager.LoadLanguage("filename-en");
     
    // Considering the PO file has a msgid "Ola Mundo" and msgstr "Hello World" this will print "Hello World"
    print(TextManager.GetText("Ola Mundo"));
    
    TextManager.cs using UnityEngine;
    using System.Collections;
    using System.IO;
     
    // originally found in: http://forum.unity3d.com/threads/35617-TextManager-Localization-Script
     
    /// <summary>
    /// TextManager
    /// 
    /// Reads PO files in the Assets\Resources\Languages directory into a Hashtable.
    /// Look for "PO editors" as that's a standard for translating software.
    /// 
    /// Example:
    /// 
    /// load the language file:
    ///   TextManager.LoadLanguage("helptext-pt-br");
    /// 
    /// which has to contain at least two lines as such:
    ///   msgid "HELLO WORLD"
    ///   msgstr "OLA MUNDO"
    /// 
    /// then we can retrieve the text by calling:
    ///   TextManager.GetText("HELLO WORLD");
    /// </summary>
    public class TextManager : MonoBehaviour {
     
    	private static TextManager instance;
    	private static Hashtable textTable;
    	private TextManager () {} 
     
    	private static TextManager Instance 
    	{
    		get 
    		{
    			if (instance == null) 
    			{
            		// Because the TextManager is a component, we have to create a GameObject to attach it to.
           	 		GameObject notificationObject = new GameObject("Default TextManager");
     
            		// Add the DynamicObjectManager component, and set it as the defaultCenter
           			instance = (TextManager) notificationObject.AddComponent(typeof(TextManager));
        		}
    			return instance;
    		}
    	}
     
    	public static TextManager GetInstance ()
    	{
    		return Instance;
    	}	
     
    	public static bool LoadLanguage (string filename)
    	{
    		GetInstance();
     
    		if (filename == null)
    		{
    			DebugConsole.Log("[TextManager] loading default language.");
    			textTable = null; // reset to default
    			return false; // this means: call LoadLanguage with null to reset to default
    		}
     
    		string fullpath = "Languages/" +  filename + ".po"; // the file is actually ".txt" in the end
     
    		TextAsset textAsset = (TextAsset) Resources.Load(fullpath, typeof(TextAsset));
    		if (textAsset == null) 
    		{
    			DebugConsole.LogError("[TextManager] "+ fullpath +" file not found.");
    			return false;
    		}
     
    		DebugConsole.Log("[TextManager] loading: "+ fullpath);
     
    		if (textTable == null) 
    		{
    			textTable = new Hashtable();
    		}
     
    		textTable.Clear();
     
    		StringReader reader = new StringReader(textAsset.text);
    		string key = null;
    		string val = null;
    		string line;
    		while ( (line = reader.ReadLine()) != null)
    		{
    			if (line.StartsWith("msgid \""))
    			{
        			key = line.Substring(7, line.Length - 8);
    			}
    			else if (line.StartsWith("msgstr \""))
    			{
        			val = line.Substring(8, line.Length - 9);
    			}
    			else
    			{
    	    		if (key != null && val != null) 
    	    		{
    	    			// TODO: add error handling here in case of duplicate keys
    	    			textTable.Add(key, val);
    					key = val = null;
    	    		} 
        		}
    		}
     
    		reader.Close();
     
    		return true;
    	}
     
     
    	public static string GetText (string key)
    	{
    		if (key != null && textTable != null)
    		{
    			if (textTable.ContainsKey(key))
    			{
    				string result = (string)textTable[key];
    				if (result.Length > 0)
    				{
    					key = result;
    				}
    			}
    		}
    		return key;
    	}
    }
    
    TextManager.js Added this js port. Also fixed one error (don't remember what it was). 
    import System.IO;//using System.IO;
    import UnityEngine;
    import System.Collections;
     
    // originally found in: http://forum.unity3d.com/threads/35617-TextManager-Localization-Script
     
    /// <summary>
    /// TextManager
    /// 
    /// Reads PO files in the Assets\Resources\Languages directory into a Hashtable.
    /// Look for "PO editors" as that's a standard for translating software.
    /// 
    /// Example:
    /// 
    /// load the language file:
    ///   TextManager.LoadLanguage("helptext-pt-br");
    /// 
    /// which has to contain at least two lines as such:
    ///   msgid "HELLO WORLD"
    ///   msgstr "OLA MUNDO"
    /// 
    /// then we can retrieve the text by calling:
    ///   TextManager.GetText("HELLO WORLD");
    /// </summary>
    class TextManager extends MonoBehaviour {
     
        static var instance:TextManager ;
        static var textTable:Hashtable ;
     
     
        static function Instance() 
        {
     
                if (instance == null) 
                {
                    // Because the TextManager is a component, we have to create a GameObject to attach it to.
                    var notificationObject:GameObject  = new GameObject("Default TextManager");
     
                    // Add the DynamicObjectManager component, and set it as the defaultCenter
              	instance = notificationObject.AddComponent(typeof(TextManager));
                }
                return instance;
     
        }
     
        public static function GetInstance ()
        {
            return Instance();
        }   
     
        public static function LoadLanguage (filename:String)
        {
            GetInstance();
     
            if (filename == null)
            {
                Debug.Log("[TextManager] loading default language.");
                textTable = null; // reset to default
                return false; // this means: call LoadLanguage with null to reset to default
            }
     
            var fullpath:String = "Languages/" +  filename ; // the file is actually ".txt" in the end
     
            var textAsset:TextAsset =  Resources.Load(fullpath, typeof(TextAsset));
            if (textAsset == null) 
            {
                Debug.Log("[TextManager] "+ fullpath +" file not found.");
                return false;
            }
     
            Debug.Log("[TextManager] loading: "+ fullpath);
     
            if (textTable == null) 
            {
                textTable = new Hashtable();
            }
     
            textTable.Clear();
     
            var reader:StringReader  = new StringReader(textAsset.text);
            var key:String = null;
            var val:String = null;
            var line:String;
            line = reader.ReadLine();
            while ( (line ) != null)
            { 
     
                if (line.StartsWith("msgid \""))
                {
                    key = line.Substring(7, line.Length - 8);
                }
                else if (line.StartsWith("msgstr \""))
                {
                    val = line.Substring(8, line.Length - 9);
                }
     
                if (key != null && val != null) 
                {
                        // TODO: add error handling here in case of duplicate keys
                        textTable.Add(key, val);
     
                        key = val = null;
                } 
     
                line = reader.ReadLine();
            }
     
            reader.Close();
     
            return true;
        }
     
     
        public static function GetText (key:String)
        {
            if (key != null && textTable != null)
            {
                if (textTable.ContainsKey(key))
                {
                    var result:String = textTable[key];
                    if (result.Length > 0)
                    {
                        key = result;
                    }
                }
            }
            return key;
        }
    }
}
