// Original url: http://wiki.unity3d.com/index.php/RectangleWipe
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/RectangleWipe.cs
// File based on original modification date of: 17 February 2012, at 03:42. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: Eric Haines (Eric5h5) 
DescriptionMakes an animated rectangular wipe from one camera view to another, that can either zoom in or out. 
 
UsageSee ScreenWipes for an example scene and the actual script that performs the wipe. The script below is an example of usage. It should be attached to an object, such as a manager object, and the ScreenWipes script should also be attached to the manager object. Also needed are two cameras, which must be in the same scene, naturally. In this example, you can press the up and down arrow keys to make the rectangle wipe zoom in or out. 
Drag the two cameras onto the appropriate slots in the inspector after you've attached the script to an object. The script has a WipeTime public variable; this is the time it will take for the wipe to complete. 
Any GUIElements (GUITexts and GUITextures) will move along with the cameras during the transition. To avoid this, make another camera that only renders GUIElements on top of the other cameras, and nothing else. If you're using OnGUI, then the GUI won't be affected. 
The function is a coroutine: 
function RectWipe (camera1 : Camera, camera2 : Camera, wipeTime : float, zoom : ZoomType) : IEnumerator 
camera1 is the camera that you are wiping from and camera2 is the camera you are wiping to. wipeTime is the length of time, in seconds, it takes to complete the wipe. zoom is the ZoomType, which should be either ZoomType.Grow to zoom in or ZoomType.Shrink to zoom out. 
JavaScript - RectWipeExample.jsvar camera1 : Camera;
var camera2 : Camera;
var wipeTime = 2.0;
var curve : AnimationCurve;
private var inProgress = false;
private var swap = false;
 
function Update () {
	if (Input.GetKeyDown("up")) {
		DoWipe(ZoomType.Grow);
	}
	else if (Input.GetKeyDown("down")) {
		DoWipe(ZoomType.Shrink);
	}
}
 
function DoWipe (zoom : ZoomType) {
	if (inProgress) return;
	inProgress = true;
 
	swap = !swap;
	yield ScreenWipe.use.RectWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, zoom);	
	//yield ScreenWipe.use.RectWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, zoom, curve);
 
	inProgress = false;
}To use this you also need the ScreenWipes script. 
}
