/*************************
 * Original url: http://wiki.unity3d.com/index.php/BoolPrefs
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/Serialization/BoolPrefs.cs
 * File based on original modification date of: 14 May 2014, at 17:39. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.Serialization
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 PlayerPrefsX.js 
    4 PlayerPrefsX.cs 
    
    DescriptionSo you're missing SetBool and GetBool from PlayerPrefs? Worry not, here they are! 
    Usage Have this script somewhere in your project. Ideally it should be in Standard Assets/Scripts so it can easily be used by C# and Boo scripts. Call it PlayerPrefsX, and then you can use PlayerPrefsX.GetBool and PlayerPrefsX.SetBool as you would expect. Normally GetBool returns false if the key doesn't exist, but you can override that by optionally specifying a default value. i.e., "var foo = PlayerPrefsX.GetBool("blah", true);" will cause foo to be true if the key "blah" doesn't exist. These functions use PlayerPrefs.SetInt and GetInt behind the scenes (0 = false, 1 = true), so they're basically convenience functions. 
    PlayerPrefsX.js static function SetBool (name : String, value : boolean) {
    	PlayerPrefs.SetInt(name, value? 1 : 0);
    }
     
    static function GetBool (name : String) : boolean {
    	return PlayerPrefs.GetInt(name) == 1;
    }
     
    static function GetBool (name : String, defaultValue : boolean) : boolean {
    	if (PlayerPrefs.HasKey(name)) {
    		return GetBool(name);
    	}
    	return defaultValue;
    }PlayerPrefsX.cs public class PlayerPrefsX
    {
    	public static void SetBool(string name, bool booleanValue) 
    	{
    		PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
    	}
     
    	public static bool GetBool(string name)  
    	{
    	    return PlayerPrefs.GetInt(name) == 1 ? true : false;
    	}
     
    	public static bool GetBool(string name, bool defaultValue)
    	{
    	    if(PlayerPrefs.HasKey(name)) 
    		{
    	        return GetBool(name);
    	    }
     
    	    return defaultValue;
    	}
}
}
