// Original url: http://wiki.unity3d.com/index.php/PlayerPrefsx
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Networking/WWWScripts/PlayerPrefsx.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
Author: StarManta; built on CookieCutter by KeliHlodversson 
Widget Support: Casemon (with thanks to StarManta & Eric5h5) 
This script has been made obsolete by Unity 2.1; PlayerPrefs now works in Web players. 
As is, it works with Unity 2.0 Web Players. It has not been tested with 1.x. If it doesn't work, line 24 is probably the line that would need to be changed. 
Contents [hide] 
1 Description 
2 Usage 
3 Notes 
4 JavaScript - PlayerPrefsx.js 

DescriptionCookieCutter is a utility script for accessing the browser cookies from the web player. PlayerPrefsx takes that base, and adds functions so that, to make your game web-game-save compatible, you can simply find and replace PlayerPrefs with PlayerPrefsx. PlayerPrefsx will automatically determine whether to use CookieCutter code or PlayerPrefs code so your game can always save. 
For Dashboard widgets, use exactly like you would with webplayers. PlayerPrefsx with Dashboard players will create a single widget preferenceKey (in your game's widget plist in ~/Library/Preferences) that contains all of your prefs. Recommend against saving prefs every frame, otherwise you'll notice a slow down (especially in widgets). 
UsagePlace this script on a GameObject. Make sure the GameObject has a unique name. 
After the game has started, cookies can be read by calling PlayerPrefs.SetInt("Player Count", 5), etc - just like PlayerPrefs. PPX also has SetBool and GetBool methods that work online and off. 
For best reliability, put the following code before you use PlayerPrefsx: 
while (!PlayerPrefsx.loaded) yield;It shouldn't take any significant amount of time to load the cookies - a frame at most, probably. (Non-Web player builds will be loaded instantaneously) In any case this will make sure PPX function are not called when they shouldn't be. 
NotesSafari requires the embedding html to be served from an http: url. When testing on a local file url, no cookies will be saved or returned. Either enable personal web sharing and open the player through http://localhost/... or use a browser that supports cookies on file:// urls (eg. Firefox) 
Watch the size of the cookies. All cookies defined using this module will be sent to the web server serving the web player on every web request. This increases the amount of data sent to the server, but might also compromise privacy if the cookies contain sensitive data. Especially if you are hosting the player on a domain name shared by others. You can limit the scope of the cookies created by this module by setting the path variable to limit the cookie to a subdirectory on the web server. (Ie. if the web player is located at http://myhost.com/myProjects/coolGame/webPlayer.html, set path to /myProjects/coolGame to ensure the cookies are not available to urls outside that directory on the web server.) 
The building of dashCookieData could probable be written more effectively (any takers?) 
The string.Split in the callback of cookiecutter requires System.?. To make this work without that big dependency replace the for with something like: 
while(cookie_string.IndexOf(";")>0) {
      var stripText = cookie_string.Substring(0,cookie_string.IndexOf(";"));
      cookie_string=cookie_string.Substring(cookie_string.IndexOf(";"));     
      if(stripText.Length >= 2){
         var is_pos=stripText.IndexOf("=");
         var name=stripText.Substring(0,is_pos);
         var value=stripText.Substring(is_pos+1);
          _cookies[name] = value;
      }
  }JavaScript - PlayerPrefsx.jsimport UnityEngine.Application; // Imports ExternalEval(), ExternalCall() and platform into the current namespace
// import UnityEngine.RuntimePlatform; // Imports WindowsWebPlayer and OSXWebPlayer
 
private static var _cookies : Hashtable; 
private static var instance : PlayerPrefsx;
// private static var cookieDash : String;
 
static var loaded = false;
var path : String = "/"; // Change this to limit the scope of the cookies created
var dashCookiePrefix = "MyGameName"; // Change this something unique for your game
static var cookieDays : int = 60;
 
private static var isWebPlayer : boolean = false;
private static var isDashPlayer : boolean = false;
private static var dashCookieName : String = "DashCookie";
 
 
function Awake () {
	if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
    	isWebPlayer = true;
	else if (Application.platform == RuntimePlatform.OSXDashboardPlayer)
    	isDashPlayer = true;
 
	_cookies = new Hashtable(); 
 
	// Define helper functions 
	if (isWebPlayer && !isDashPlayer) {
		ExternalEval( 
			"function PlayerPrefsx_init(name, callback, unused){ " + 
			"	var unity=GetUnity(); " + 
			"	if ( unity ) { " +
			"		unity.SendMessage( name, callback, " + 
			"			''+document.cookie); " + 
			"	} " +
			"	else { " +
			"		alert('Could not find Unity plugin. Cookies will not be loaded'); " +
			"	} " +
			"}" + 
			"function PlayerPrefsx_set(name, value, days){ " + 
			"	var expires=''; " + 
			"	if (days) { " + 
			"		var date = new Date(); " + 
			"		date.setTime(date.getTime()+ " + 
			"		(days*24*60*60*1000)); " + 
			"		expires = '; expires='+date.toUTCString(); " + 
			"	} " + 
			"	document.cookie = name+'='+value+expires+'; path="+path+"';" + 
			"}"); 
	}
 
    // Define helper functions 
    else if (isDashPlayer) {
		dashCookieName = dashCookiePrefix + dashCookieName;
 
		ExternalEval( 
			"function GetUnity () {" +
			"	return document.getElementById('UnityEmbed'); " +
			"}" +
			"function PlayerPrefsx_init(name, callback, cookieName){ " + 
			"	var unity=GetUnity(); " + 
			"	if ( unity ) { " +
			"		unity.SendMessage( name, callback, " +
			"			''+widget.preferenceForKey(cookieName) " +
			"	); " + 
			"	} " +
			"	else { " +
			"		alert('Could not find Unity plugin. Preferences will not be loaded'); " +
			"	} " +
			"}" + 
			"function PlayerPrefsx_set(name, value){ " + 
			"	widget.setPreferenceForKey(value, name); " + 
			"}");
	}
 
	ReadCookiesFromBrowser();
} 
 
//DEBUG CONSOLE
 
// function OnGUI() {
// 	if (isWebPlayer || isDashPlayer) {
// 		for (var element in _cookies) {
// 			GUILayout.Label(element);
// 		}
// 	}
// }
 
// Called by the browser javascript during initialisation 
function _ReadCookiesCallback(cookie_string : String) { 
	if (!_cookies) _cookies = new Hashtable();
    loaded=true;
    var split : Array = cookie_string.Split(";"[0]);
    for(var element in split ) { 
        var key_value = element.Split(["="[0]],2); 
        if(key_value.Length == 2)
        _cookies[key_value[0].Trim()] = key_value[1]; 
    }
} 
 
function ReadCookiesFromBrowser() { 
	loaded=false;
	if (isWebPlayer || isDashPlayer) {
		ExternalCall("PlayerPrefsx_init", transform.name, "_ReadCookiesCallback", dashCookieName); 
	}
	else
		_ReadCookiesCallback("test1=1; testCookie=NotWebPlayer;");
} 
 
static function ReloadCookies(){
    instance.ReadCookiesFromBrowser();
}
 
static function SetCookie(name : String, value : String, days : int) { 
	_cookies[name]=value; 
	if (isDashPlayer) {
		var dashCookieData : String = "";
		for (var entry : DictionaryEntry in _cookies) { 
			dashCookieData = dashCookieData + entry.Key + "=" + entry.Value + ";";
		}
		ExternalCall("PlayerPrefsx_set", dashCookieName, dashCookieData); 
		// ExternalCall("PlayerPrefsx_set", name, value); 
	}
	else
		ExternalCall("PlayerPrefsx_set", name, value, days); 
}
 
static function GetCookie(name : String) : String { 
    if (_cookies[name] != null) {
        return _cookies[name]; 
    }
    else {
        return "";
    }
}
 
//PLAYERPREFS COMPATIBILITY FUNCTIONS
 
static function SetBool(name : String, value : boolean) {
    SetInt(name, value ? 1 : 0);
}
 
static function SetInt(name : String, value : int) {
    if (isWebPlayer || isDashPlayer)
        SetCookie(name, value.ToString(), cookieDays);
    else
        PlayerPrefs.SetInt(name, value);
}
 
static function SetFloat(name : String, value : float) {
    if (isWebPlayer || isDashPlayer)
        SetCookie(name, value.ToString(), cookieDays);
    else
        PlayerPrefs.SetFloat(name, value);
}
 
static function SetString(name : String, value : String) {
    if (isWebPlayer || isDashPlayer)
        SetCookie(name, value, cookieDays);
    else
        PlayerPrefs.SetString(name, value);
}
 
static function GetBool(name : String, defaultValue : boolean) : boolean {
    return (GetInt(name, defaultValue ? 1 : 0) == 1);
}
 
static function GetInt(name:  String, defaultValue : int) : int {
    if (name=="") return defaultValue;
	var temp : String;
	if (isWebPlayer || isDashPlayer) {
        temp = GetCookie(name);
        if (temp != "") {
            return int.Parse(temp);
        }
        else {
            SetCookie(name, ""+defaultValue, cookieDays);
            return defaultValue;
        }
    }
    else return PlayerPrefs.GetInt(name, defaultValue);
}
 
static function GetFloat(name:  String, defaultValue : float) : float {
    if (name=="") return defaultValue;
    if (isWebPlayer || isDashPlayer) {
        var temp : String = GetCookie(name);
        if (temp != "") return float.Parse(temp);
        else {
            SetCookie(name, defaultValue.ToString("f2"), cookieDays);
            return defaultValue;
        }
    }
    else return PlayerPrefs.GetFloat(name, defaultValue);
}
 
static function GetString(name:  String, defaultValue : String) : String {
    if (name=="") return defaultValue;
    if (isWebPlayer || isDashPlayer) {
        var temp : String = GetCookie(name);
        if (temp != "") return temp;
        else {
            SetCookie(name, defaultValue, cookieDays);
            return defaultValue;
        }
    }
    else return PlayerPrefs.GetString   (name, defaultValue);
}
}
