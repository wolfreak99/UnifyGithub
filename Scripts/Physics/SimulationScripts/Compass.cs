// Original url: http://wiki.unity3d.com/index.php/Compass
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/Compass.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.SimulationScripts
{
CompassThe compass script allows you to get 0-360 degree heading with respect to an arbitrary North reference object named "GPS Reference" of which the Z axis is pointed North. The compass uses a Sender to send the heading over a network. 
Codevar updateFreq = 1.0;
private var heading = 0.0;
private var gpsRef : Transform;
private var gpsRefN : int;
private var sender : Sender;
private var timer = 0.0;
 
function Start(){
	Init();	
}
function Update () {	
	timer+=Time.deltaTime;
	heading = transform.rotation.eulerAngles.y - gpsRefN;
	if(heading<0)heading+=360;
	if(timer>(1/updateFreq)){
		sender.Send(heading);
		timer=0;
	}
//	Debug.Log(transform.rotation.eulerAngles.y + " - " + gpsRefN + " = " + heading);
}
 
function OnEnable(){
	Init();
}
 
function Init(){
	var gpsRefGO = GameObject.Find("GPS Reference");
	if(gpsRefGO!=null){
		gpsRef = gpsRefGO.transform;
		gpsRefN = gpsRef.rotation.eulerAngles.y;
		sender = gameObject.GetComponent("Sender");
	}
}
}
