// Original url: http://wiki.unity3d.com/index.php/IOSTextField
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/IOSTextField.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.Unity20GUIScripts
{
This script provides emulation of a GUI.TextField but allows specialized keyboards. It is also configured to work in non-iOS code with standard GUI.TextFields. A few basic configurations are provided as well a fully configurable base function. 
Usage The functions are used in a similar fashion to the GUI methods, with the exception that they need to maintain state, where the Unity immediate mode UI is generally stateless. This is where the fieldID comes in. If you have more than one of these text fields in an OnGUI, you should give each a positive unique identifier (much like the window ID in GUI.Window) 
InputField - the basic army knife function, allows you to specify the iPhoneKeyboardType as well as if you want a password field or enable/disable autocorrect. 
var field1text : String;
var field2text : String;
 
function OnGUI()
{
    field1text = iOSUI.InputField(1, Rect(0,0, 100, 20), field1text, 0, iPhoneKeyboardType.URL, false, false);
    field2text = iOSUI.InputField(2, Rect(0,30, 100, 20), field2text, 10, iPhoneKeyboardType.NumberPad, true, false);
}PWField - hidden text in both the keyboard and field text 
var password : String;
 
function OnGUI()
{
    password = iOSUI.PWField(1, Rect(0,0, 100, 20), password, 10);
}EMailField - uses the email keyboard and doesn't try to autocorrect 
var email : String;
 
function OnGUI()
{
    email = iOSUI.EMailField(1, Rect(0,0, 100,20), email);
}NameField - disables autocorrect 
var name : String;
 
function OnGUI()
{
    name = iOSUI.NameField(1, Rect(0,0, 100,20), name, 16);
}Javascript - iOSUI.js #pragma strict
#pragma downcast
 
private static var kb : iPhoneKeyboard = null;
private static var currentIOSInputField : int = -1;
 
public static function InputField(fieldID : int    //positive unique to current DoUI
    , rect : Rect    // standard positioning rectangle
    , text : String    // the string to start with
    , maxLength : int	// 0 indicates infinite
    , keyboardType : iPhoneKeyboardType
    , secure : boolean    // true will cause the keyboard to go in to password mode and the field to show only asterisks
    , autocorrect : boolean    // false will disable autocorrect, useful for fields where a user types in a name
) : String    // returns the modified string
{
#if UNITY_IPHONE && !UNITY_EDITOR
 
    var style = GUIStyle("textfield");
    var displayText : String;
    if(secure)
    {
        displayText = "";
        for(var i : int = 0; i < text.Length; ++i) {displayText += "*";}
    }
    else
    {
        displayText = text;
    }
 
    if(GUI.Button(rect, displayText, style))
    {
        kb = iPhoneKeyboard.Open(text, keyboardType, autocorrect, false, secure, false, ""); 
        currentIOSInputField = fieldID;
    }
 
    if(kb && (currentIOSInputField == fieldID))
    {
        if(maxLength == 0 || text.Length <= maxLength)
        {
             text = kb.text;
        }
        else
        {
            text = kb.text.Substring(0, maxLength);
        }
 
        if(kb.done)
        {
            kb = null;
            currentIOSInputField = -1;
        }
    }
 
    return text;
#else
    if(secure)
    {
        if(maxLength > 0)
        {
            return GUI.PasswordField(rect, text, "*"[0], maxLength);
        }
        else
        {
            return GUI.PasswordField(rect, text, "*"[0]);
        }
    }
    else
    {
        if(maxLength > 0)
        {
            return GUI.TextField(rect, text, maxLength);
        }
        else
        {
            return GUI.TextField(rect, text);
        }
    }
#endif 
}
 
public static function PWField(fieldID : int, rect : Rect, text : String, maxLength : int) : String
{
    return InputField(fieldID, rect, text, maxLength, iPhoneKeyboardType.Default, true, false);
}
 
public static function EMailField(fieldID : int, rect : Rect, text : String) : String
{
    return InputField(fieldID, rect, text, 0, iPhoneKeyboardType.EmailAddress, false, false);
}
 
public static function NameField(fieldID : int, rect : Rect, text : String, maxLength : int) : String
{
    return InputField(fieldID, rect, text, maxLength, iPhoneKeyboardType.Default, false, false);
}
}
