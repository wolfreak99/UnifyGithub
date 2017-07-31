// Original url: http://wiki.unity3d.com/index.php/DebuggerX
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Development/DebuggingScripts/DebuggerX.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{
This simple Javascript class wraps the Unity Debug object. Use it in place of Debug. methods directly and you can leave it in your published code. It will check for the 'Strip Debug Symbols' build flag by default, and disable debugging. All Debug methods are wrapped and provided with same arguments. 
This can help with the Mac OS X Leopard problem where eats your CPU because Unity is outputting a lot of messages to the Mac's Console. 
var consoleDebug : boolean = true;
var gameDebug : boolean = true;
var lastDebugMessage : String = null;
 
static var _instance : DebuggerX;
 
function Start() {
    if (!Debug.isDebugBuild) {
        consoleDebug = false;
        gameDebug = false;
    }
}
 
static function Break() {
    _instance.DoBreak();
}
 
static function Log(val : Object) {
    if (val != null) {
        _instance.DoLog(val);
    }
}
 
static function Error(val : Object, source : Object) {
    if (val != null) {
        _instance.DoError(val, source);
    }
}
 
static function Warning(val : Object, source : Object) {
    if (val != null) {
        _instance.DoWarning(val, source);
    }
}
 
static function DrawLine(start : Vector3, end : Vector3, col : Color) {
    _instance.DoDrawLine(start, end, col);
}
 
static function DrawRay(start : Vector3, dir : Vector3, col : Color) {
    _instance.DoDrawRay(start, dir, col);
}
 
function DoBreak() {
    if (consoleDebug) {
        Debug.Break();
    }
}
 
function DoLog(val : Object) {
    if (consoleDebug) {
        Debug.Log(val);
    }
    lastDebugMessage = "" + val;       
}
 
function DoError(val : Object, source : Object) {
    if (consoleDebug) {
        Debug.LogError(val, source);
    }
    lastDebugMessage = "" + val;
}
 
function DoWarning(val : Object, source : Object) {
    if (consoleDebug) {
        Debug.LogWarning(val, source);
    }
    lastDebugMessage = "" + val;
}
function DoDrawLine(start : Vector3, end : Vector3, col : Color) {
    if (consoleDebug) {
        Debug.DrawLine(start, end, col);
    }
}
 
function DoDrawRay(start : Vector3, dir : Vector3, col : Color) {
    if (consoleDebug) {
        Debug.DrawRay(start, dir, col);
    }
}
 
function Awake() { 
   if (_instance == null) { 
      DontDestroyOnLoad(gameObject); 
      _instance = this;    
   } else {
      Destroy(this.gameObject); 
   }
} 
 
function Update() {
    if (_instance == null) {
        _instance = this;
    }
 
    if (Input.GetKey("left ctrl") && Input.GetKeyUp("=")) {
        gameDebug = !gameDebug;
        /*
        var msgs0 = GameObject.Find("Messages");
        if (msgs0 != null) {
            msgs0.SendMessage("LocalMsg", "Game Debug " + (gameDebug ? "Enabled" : "Disabled"));
        }
        */
    }
    if (Input.GetKey("left ctrl") && Input.GetKeyUp("-")) {
        consoleDebug = !consoleDebug;
        /*
        var msgs1 = GameObject.Find("Messages");
        if (msgs1 != null) {
            msgs1.SendMessage("LocalMsg", "Console Debug " + (consoleDebug ? "Enabled" : "Disabled"));
        }
        */
    }
}
 
function OnGUI() {
    if (gameDebug && lastDebugMessage) {
        GUI.Label(Rect(5, -3, Screen.width, 20), lastDebugMessage);
    }
}
}
