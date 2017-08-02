/*************************
 * Original url: http://wiki.unity3d.com/index.php/SICK_Laser_Scanner
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/SICK_Laser_Scanner.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: (Jeff Craighead) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.SimulationScripts
{
    Thanks to StarManta for help with the Quaternion math. 
    DescriptionThis script simulates the SICK LMS laser scanner by doing a bunch of raycasts around a single point. There are two functions each behaves slightly differently. See the comments in the code. 
    UsageAttach this to an empty object, which becomes the scanner. You'll need some sort of query function to retrieve the range data for use in other parts of the game/simulation. 
    JavaScriptvar arcAngle = 180.0;
    var numLines = 181;
    var maxDist = 8.0;
    var scansPerSec = 3;
    private var ranges : float[];
    private var timer = 0.0;
     
    function LateUpdate(){
    	DoScan2();
    }
     
    //Centers scan of arcAngle degrees with numLines rays around "forward". Centering accounts for the "missing" last ray by arcAngle/(2*numLines) degrees.
    //Start and end lines are not exactly on 0 and n degrees but offset by arcAngle/(2*numLines) degrees.
    function DoScan1(){
    	if(timer>1.0/scansPerSec){
    		ranges = new float[numLines];
    		for (l=0;l<numLines;l++) {
    			var shootVec : Vector3 = transform.rotation * Quaternion.AngleAxis(-1*arcAngle/2+(l*arcAngle/numLines)+arcAngle/(2*numLines), Vector3.up) * Vector3.forward;
    			var hit : RaycastHit;
    				Debug.DrawRay(transform.position,shootVec,Color.red);
    			if (Physics.Raycast(transform.position, shootVec, hit, maxDist)) {
    				Debug.DrawLine(transform.position, hit.point, Color.red);
    				ranges[l]=hit.distance;
    			}
    			else ranges[l]=maxDist;
    		}
    		timer=0;
    	}
    	else timer+=Time.deltaTime;
    }
     
    //Centers scan of arcAngle degrees with numLines rays around "forward". Centering accounts for the "missing" last ray by adding the last "extra" ray.
    //Start and end lines are exactly on 0 and n degrees but there are numLines+1 rays.
    function DoScan2(){
    	nLines=numLines+1;
    	if(timer>1.0/scansPerSec){
    		ranges = new float[nLines];
    		for (l=0;l<nLines;l++) {
    			var shootVec : Vector3 = transform.rotation * Quaternion.AngleAxis(-1*arcAngle/2+(l*arcAngle/numLines), Vector3.up) * Vector3.forward;
    			var hit : RaycastHit;
    				Debug.DrawRay(transform.position,shootVec,Color.blue);
    			if (Physics.Raycast(transform.position, shootVec, hit, maxDist)) {
    				Debug.DrawLine(transform.position, hit.point, Color.blue);
    				ranges[l]=hit.distance;
    			}
    			else ranges[l]=maxDist;
    		}
    		timer=0;
    	}
    	else timer+=Time.deltaTime;
}
}
