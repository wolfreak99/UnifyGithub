// Original url: http://wiki.unity3d.com/index.php/WithPrefs
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/WithPrefs.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.Unity20GUIScripts
{
by StarManta 
Instead of coding many switches and sliders and manually hook them up with preferences, these functions hook up easily with preferences. These use PlayerPrefsx but can easily be converted to use plain PlayerPrefs if desired. 
Usage: 
myVar = ToggleWithPref("My Variable", myVar); myFloat = SliderWithPref("My Float", myFloat, 0.0, 1.0); 
static function ToggleWithPref(name : String, defaultValue : boolean) : boolean {
	var oldValue : boolean = PlayerPrefsx.GetBool(name, defaultValue);
	var newValue : boolean = GUILayout.Toggle(oldValue, name);
	if (newValue != oldValue) {
		PlayerPrefsx.SetBool(name, newValue);
	}
	return newValue;
}
 
 
static function SliderWithPref(name : String, defaultValue : float, min : float, max : float) : float {
	var oldValue : float = PlayerPrefsx.GetFloat(name, defaultValue);
	var newValue : float = GUILayout.Slider(oldValue, min, max);
	if (newValue != oldValue) {
		PlayerPrefsx.SetFloat(name, newValue);
	}
	return newValue;
}
}
