// Original url: http://wiki.unity3d.com/index.php/GameTime
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Physics/SimulationScripts/GameTime.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.SimulationScripts
{
GameTimeGameTime rotates a directional light to match the appropriate direction of the sun for the system time of day. 
TODO: It doesn't change the length of the day for seasons nor the angle of the sun. It also doesn't turn off the light at night, as would be accurate. 
Codeimport System;
var date = DateTime.Now;
var timeDisplay : GUIText;
 
function Start() {
	InvokeRepeating("Increment", 1.0, 1.0);
}
function Update () {
	var seconds : float = date.TimeOfDay.Ticks / 10000000;
	transform.rotation = Quaternion.LookRotation(Vector3.up);
	transform.rotation *= Quaternion.AngleAxis(seconds/86400*360,Vector3.down);
	if (timeDisplay) timeDisplay.text = date.ToString("f");
}
 
function Increment() {
	date += TimeSpan(0,0,0, 1);
}
}
