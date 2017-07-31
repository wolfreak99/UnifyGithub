// Original url: http://wiki.unity3d.com/index.php/CookieCutter
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/WWWScripts/CookieCutter.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
Author: KeliHlodversson 
Contents [hide] 
1 Description 
2 Usage 
3 Notes 
4 JavaScript - CookieCutter.js 
5 Example - CookieTest.js 

DescriptionThis is a utility script for accessing the browser cookies from the web player 
UsagePlace this script on a GameObject. Make sure the GameObject has a unique name. 
After the game has started, cookies can be read by calling CookieCutter.GetCookie(name) and set using CookieCutter.SetCookie(name, value, days), where name is the name of the cookie key, value is the string value to be stored and days is the number of days until the browser will delete the cookie. 
NotesDue to the asynchronous way the Unity Plug-In communicates with the browser, the cookies are not available straight away. Before calling Get or SetCookie, inspect CookieCutter.loaded, which will be set to true when the data is available. 
This script only works in the web player plug-in. 
Safari requires the embedding html to be served from an http: url. When testing on a local file url, no cookies will be saved or returned. Either enable personal web sharing and open the player through http://localhost/... or use a browser that supports cookies on file:// urls (eg. Firefox) 
Watch the size of the cookies. All cookies defined using this module will be sent to the web server serving the web player on every web request. This increases the amount of data sent to the server, but might also compromise privacy if the cookies contain sensitive data. Especially if you are hosting the player on a domain name shared by others. You can limit the scope of the cookies created by this module by setting the path variable to limit the cookie to a subdirectory on the web server. (Ie. if the web player is located at http://myhost.com/myProjects/coolGame/webPlayer.html, set path to /myProjects/coolGame to ensure the cookies are not available to urls outside that directory on the web server.) 
JavaScript - CookieCutter.jsimport UnityEngine.Application; // Imports ExternalEval(), ExternalCall() and platform into the current namespace
import UnityEngine.RuntimePlatform; // Imports WindowsWebPlayer and OSXWebPlayer
 
private static var _cookies : Hashtable; 
private static var _instance : CookieCutter;
static var loaded = false;
var path = "/"; // Change this to limit the scope of the cookies created
 
function Awake () { 
  _cookies = new Hashtable(); 
  _instance = this;
  // Define helper functions 
  ExternalEval( 
    "function CookieCutter_init(name, callback){" + 
    "    var unity=GetUnity();" + // Old Unity versions (before 2.0) should have this line changed to: "var unity=document.Unity;"
    "    if ( unity ) { " +
    "      unity.SendMessage( name, callback, " + 
    "            ''+document.cookie);" + 
    "    } " +
    "    else { " +
    "      alert('Could not find Unity plugin. Cookies will not be loaded'); " +
    "    } " +
    "}" + 
    "function  CookieCutter_set(name, value, days){" + 
    "  var expires='';" + 
    "  if (days) {" + 
    "    var date = new Date();" + 
    "    date.setTime(date.getTime()+ " + 
    "      (days*24*60*60*1000));" + 
    "    expires = '; expires='+date.toUTCString();" + 
    "  }" + 
    "  document.cookie = name+'='+value+expires+'; path="+path+"';" + 
    "}"); 
 
  ReadCookiesFromBrowser(); 
} 
 
// Called by the browser javascript during initialisation 
function _ReadCookiesCallback(cookie_string) { 
  _cookies = new Hashtable(); 
  loaded=true;
  for(var element in cookie_string.Split([";"[0]]) ) { 
      var key_value = element.Split(["="[0]],2); 
      if(key_value.Length == 2)
          _cookies[key_value[0].Trim()] = key_value[1]; 
 
  }
} 
 
function ReadCookiesFromBrowser() { 
   loaded=false;
   if(platform == WindowsWebPlayer || platform == OSXWebPlayer)
       ExternalCall("CookieCutter_init", name, "_ReadCookiesCallback"); 
   else
       _ReadCookiesCallback("test1=1; testCookie=NotWebPlayer;");
} 
 
static function ReloadCookies(){
  _instance.ReadCookiesFromBrowser();
}
 
static function SetCookie(name, value, days) { 
   _cookies[name]=value; 
   ExternalCall("CookieCutter_set", name, value, days); 
} 
static function SetCookie(name, value) { 
   _cookies[name]=value; 
   ExternalCall("CookieCutter_set", name, value); 
} 
 
static function GetCookie(name) { 
   return _cookies[name]; 
}Example - CookieTest.js@script RequireComponent(GUIText)
 
var cookieValue : String;
 
// Wait until cookies are loaded
while(!CookieCutter.loaded)
 yield WaitForSeconds(0.1);
 
// Read testCookie
cookieValue=CookieCutter.GetCookie("testCookie");
 
// SetDefault
if(!cookieValue)
   cookieValue="!Cookie";
else // Else modify value
   cookieValue+="!";
 
// Display the cookie value
guiText.text=cookieValue;
 
// And save it in the browser
CookieCutter.SetCookie("testCookie",cookieValue,100);
}
