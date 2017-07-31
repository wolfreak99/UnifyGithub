// Original url: http://wiki.unity3d.com/index.php/PasswordField
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/PasswordField.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.Unity20GUIScripts
{
by Shaun le Lacheur and StarManta 
Usage Insert the function into any .js file. You can then call this as you would call GUILayout.Textfield. TODO: allow for input of GUIStyles. For now, just hardcode them into the TextFeld call in the function. 


static function PasswordField(password : String, maskChar : String) : String {
	if (Event.current.type == EventType.repaint || Event.current.type == EventType.mouseDown)
	{
	    strPasswordMask = "";
	    for (i = 0; i <password.Length; i++)
	    {
	        strPasswordMask += maskChar;
	    }
	}
	else
	{
	    strPasswordMask = password;
	}
	GUI.changed = false;
	strPasswordMask = GUILayout.TextField(strPasswordMask, GUILayout.Width(150));
	if (GUI.changed)
	{
	    password = strPasswordMask;
	}
return password;
}C# Equivalent by Opless 
string PasswordField(string password , string maskChar) 
{
	string strPasswordMask="";
	if (Event.current.type == EventType.repaint || Event.current.type == EventType.mouseDown)
	{
	    strPasswordMask = "";
	    for (int i = 0; i <password.Length; i++)
	    {
	        strPasswordMask += maskChar;
	    }
	}
	else
	{
	    strPasswordMask = password;
	}
	GUI.changed = false;
	strPasswordMask = GUILayout.TextField(strPasswordMask, GUILayout.Width(150));
	if (GUI.changed)
	{
	    password = strPasswordMask;
	}
return password;
}
}
