/*************************
 * Original url: http://wiki.unity3d.com/index.php/ToggleActiveRecursivelyAllGameObjects
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/ToggleActiveRecursivelyAllGameObjects.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Martin Schultz (MartinSchultz) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Screenshot 
    4 JavaScript - ToggleActiveAllSelectedGO.js 
    
    Description This little helper scripts toggles the active status of all selected game objects to the opposite and does as well for any linked children of the selected game objects, so a full recursive active toggle for all selected GOs. 
    The script can be found under the menu Custom→Toggle Active All Active Selected GO's. 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
    Select some game objects and call select this script from the menu. It will toggle the active state instantly. 
    Screenshot  
    
    
    JavaScript - ToggleActiveAllSelectedGO.js @MenuItem ("Custom/Toggle Active All Selected GO's")
    static function ToggleAllSelected() {
    	for (t in Selection.transforms) {
    		t.gameObject.SetActiveRecursively(! t.gameObject.active);
    	}
}
}
