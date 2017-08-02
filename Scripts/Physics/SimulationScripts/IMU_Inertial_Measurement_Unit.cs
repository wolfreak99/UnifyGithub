/*************************
 * Original url: http://wiki.unity3d.com/index.php/IMU_Inertial_Measurement_Unit
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/IMU_Inertial_Measurement_Unit.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.SimulationScripts
{
    IMU ScriptThis script simulates an IMU. You can specify the update rate in the editor. It can use a Sender script to send the readings to a remote machine. 
    Codevar updateFreq = 2.0;
    var angVel : Vector3;
    var angAccel : Vector3;
    var linVel : Vector3;
    var linAccel : Vector3;
    private var lastPos : Vector3;
    private var lastAng : Vector3;
    private var lastLinVel : Vector3;
    private var lastAngVel : Vector3;
    private var timer = 0.0;
    private var sender : Sender;
     
    function Start(){
    	Init();
    }
     
    function Update () {
    	timer += Time.deltaTime;
    	if(timer>(1/updateFreq)){
     
    		lastLinVel = linVel;
    		lastAngVel = angVel;
     
    		var lastPosInv = transform.InverseTransformPoint(lastPos);	
     
    		linVel.x = (0-lastPosInv.x)/timer;
    		linVel.y = (0-lastPosInv.y)/timer;
    		linVel.z = (0-lastPosInv.z)/timer;
     
    		var deltaX = Mathf.Abs((transform.rotation.eulerAngles).x)-lastAng.x;
    		if(Mathf.Abs(deltaX)<180 && deltaX>-180) angVel.x = deltaX/timer;
    		else{
    			if(deltaX>180) angVel.x = (360-deltaX)/timer;
    			else angVel.x = (360+deltaX)/timer;
    		}
     
    		var deltaY = Mathf.Abs((transform.rotation.eulerAngles).y)-lastAng.y;
    		if(Mathf.Abs(deltaY)<180 && deltaY>-180) angVel.y = deltaY/timer;
    		else{
    			if(deltaY>180) angVel.y = (360-deltaY)/timer;
    			else angVel.y = (360-deltaY)/timer;
    		}
     
    		var deltaZ = Mathf.Abs((transform.rotation.eulerAngles).z)-lastAng.z;
    		if(Mathf.Abs(deltaZ)<180 && deltaZ>-180) angVel.z = deltaZ/timer;
    		else{
    			if(deltaZ>180) angVel.z = (360-deltaZ)/timer;
    			else angVel.z = (360+deltaZ)/timer;
    		}
     
     
    		linAccel.x = (linVel.x-lastLinVel.x)/timer;
    		linAccel.y = (linVel.y-lastLinVel.y)/timer;
    		linAccel.z = (linVel.z-lastLinVel.z)/timer;
    		angAccel.x = ((angVel.x-lastAngVel.x)/timer)/9.81;
    		angAccel.y = ((angVel.y-lastAngVel.y)/timer)/9.81;
    		angAccel.z = ((angVel.z-lastAngVel.z)/timer)/9.81;
     
    		lastPos = transform.position;
     
    		lastAng.x = Mathf.Abs((transform.rotation.eulerAngles).x);
    		lastAng.y = Mathf.Abs((transform.rotation.eulerAngles).y);
    		lastAng.z = Mathf.Abs((transform.rotation.eulerAngles).z);
     
    		timer=0;
     
    		sender.Send(linVel + "," + linAccel + "," + angVel + "," + angAccel);
    	}
     
    }
     
    function OnEnable(){
    	Init();
    }
     
    function Init(){
    	sender = gameObject.GetComponent("Sender");
    }
}
