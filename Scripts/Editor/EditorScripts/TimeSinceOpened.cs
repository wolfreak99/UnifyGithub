/*************************
 * Original url: http://wiki.unity3d.com/index.php/TimeSinceOpened
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/TimeSinceOpened.cs
 * File based on original modification date of: 2 September 2013, at 19:06. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    This script allows you to see how long the current Unity editor window has been open for. 
    Place this script in your Editor folder. 
    
    
    TimeSinceEditorOpened.js // add menu item to Unity menu
    @MenuItem ("Window/Time Since Editor Opened")
     
    // this static function is run when menu item is clicked
    static function TimeSinceEditorOpened ()
    {
    	// variables for time in form of minutes and hours
    	var minutes = EditorApplication.timeSinceStartup/60;
    	var hours = EditorApplication.timeSinceStartup/3600;
    	// display dialog telling user amount of time Unity has been open
    	EditorUtility.DisplayDialog ("Time", "This Unity Editor has been open for " + minutes.ToString("f0") + " minutes" + ", which is about " + hours.ToString("f1") + " hours" + ".", "OK", "");
     
    }
}
