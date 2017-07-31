// Original url: http://wiki.unity3d.com/index.php/CustomGetMouseButtonDown
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/Unity20GUIScripts/CustomGetMouseButtonDown.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.Unity20GUIScripts
{
If you need to use the likes of Input.OnMouseButtonDown(0), but when your user clicks on buttons and text fields need it to not shoot whatever happens to be behind the window.... just use this class. It's written so you can simply replace "Input." with "YourGUIClass." and it will just work. 
You do, however, need to customize the last function per game - it needs to check each window manually, as there doesn't seem to be an automated way of looping through all the windows. 
static function GetMouseButtonDown(btn : int) : boolean {
	if (!Input.GetMouseButtonDown(btn)) return false;
	if (CursorIsOverAnyWindow()) return false;
	return true;
}
static function GetMouseButton(btn :int) : boolean {
	if (!Input.GetMouseButton(btn)) return false;
	if (CursorIsOverAnyWindow()) return false;
	return true;
}
 
static function CursorIsOverAnyWindow() : boolean {
	var guiCursorPos : Vector2 = Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y);
//TODO: check all your windows here
	if (GUIScriptClass1.main.window.Contains(guiCursorPos)) return true;
	if (GUIScriptClass2.main.window.Contains(guiCursorPos)) return true;
//etcetera
	return false;
}
}
