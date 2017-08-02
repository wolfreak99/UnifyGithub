/*************************
 * Original url: http://wiki.unity3d.com/index.php/MultipleCameraSwitcher2
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/MultipleCameraSwitcher2.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: cgf 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CameraControls
{
    
    DescriptionA script that allows you to switch between different cameras in your scene. This does what MultipleCameraSwitcher does without hard-coding the number of cameras. 
    UsageDrag the script onto a GameObject and view it in the inspector. Size the Cameras array to how many cameras you need, then drag the cameras on it's entries. Finally, set Camera Index to the index (zero-based) of the camera slot to start with. 
    If you don't want the script to change the audio listener to the current camera, uncheck Switch Audio Listener. 
    To switch between camera views, the default button is C. 
    JavaScript - MultipleCameraSwitcher2.js#pragma strict
     
    var cameras : Camera[];
    var cameraIndex : int = 0;
    var switchAudioListener = true;
     
    function Start () 
    {
    	if (cameras.length < 1) {
    		Debug.LogError("No cameras set.");
    		return;
    	}
     
    	for (var c : Camera in cameras) {
    		ToggleCam(c, false);
    	}	
     
    	if ((cameraIndex < 0) || (cameraIndex >= cameras.length)) {
    		Debug.LogError("Invalid camera index.");
    		cameraIndex = 0;
    	}
    	ToggleCam(cameras[cameraIndex], true);
    } 
     
    function ToggleCam(cam : Camera, enabled : boolean)
    {
    	cam.enabled = enabled;
    	if (switchAudioListener) {
    		var listener = cam.GetComponent(AudioListener);
    		if (listener) {
    			listener.enabled = enabled;
    		}
    	}
    }
     
    function Update () 
    { 
    	if (Input.GetKeyDown ("c"))
    	{ 
    		ToggleCam(cameras[cameraIndex], false);
    		cameraIndex = (cameraIndex + 1) % cameras.length;
    		ToggleCam(cameras[cameraIndex], true);
    	}
}
}
