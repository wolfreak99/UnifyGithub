// Original url: http://wiki.unity3d.com/index.php/MultipleCameraSwitcher
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/MultipleCameraSwitcher.cs
// File based on original modification date of: 10 January 2012, at 20:48. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: StephenL 
DescriptionA script that allows you to switch between different cameras in your scene. Supports four camera views that can be toggled with a button. Requires Unity 2.6 or later. 
UsageDrag four cameras into the appropriate slots in the inspector after you've attached the script to an object. The script has a startCamera public variable; this is the camera that scene will start with. To switch between camera views, the default button is C. 
JavaScript - MultipleCameraSwitcher.jsvar camera1 : Camera; 
var camera2 : Camera; 
var camera3 : Camera;
var camera4 : Camera;
public var startCamera : int = 1;
 
function Start () 
{ 
   camera1.enabled = true; 
   camera2.enabled = false; 
   camera3.enabled = false;
   camera4.enabled = false;
   startCamera = 1;
} 
 
function Update () 
{ 
   if (Input.GetKeyDown ("c") && (startCamera == 1))
   { 
	  startCamera = 2;
      camera1.enabled = false; 
      camera2.enabled = true; 
	  camera3.enabled = false;
	  camera4.enabled = false;
   } 
 
   else if (Input.GetKeyDown ("c") && (startCamera == 2))
   { 
	  startCamera = 3;
      camera1.enabled = false; 
      camera2.enabled = false; 
	  camera3.enabled = true;
	  camera4.enabled = false;
   } 
 
   else if (Input.GetKeyDown ("c") && (startCamera == 3))
   { 
	  startCamera = 4;
      camera1.enabled = false; 
      camera2.enabled = false; 
	  camera3.enabled = false;
	  camera4.enabled = true;
   } 
 
   else if (Input.GetKeyDown ("c") && (startCamera == 4))
   { 
	  startCamera = 1;
      camera1.enabled = true; 
      camera2.enabled = false; 
	  camera3.enabled = false;
	  camera4.enabled = false;
   } 
 
}
}
