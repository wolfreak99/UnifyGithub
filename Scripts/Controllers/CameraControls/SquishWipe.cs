// Original url: http://wiki.unity3d.com/index.php/SquishWipe
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/SquishWipe.cs
// File based on original modification date of: 17 February 2012, at 03:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: Eric Haines (Eric5h5) 
DescriptionMakes an animated wipe from one camera view to another, where the first image is squished out of view as the second image expands to take its place. It can go up, down, left, or right. 
 
UsageSee ScreenWipes for an example scene and the actual script that performs the wipe. The script below is an example of usage. It should be attached to an object, such as a manager object, and the ScreenWipes script should also be attached to the manager object. Also needed are two cameras, which must be in the same scene, naturally. In this example, you can press any of the arrow keys to make the wipe scroll left, right, up, or down. 
Drag the two cameras onto the appropriate slots in the inspector after you've attached the script to an object. The script has a WipeTime public variable; this is the time it will take for the wipe to complete. 
Any GUIElements (GUITexts and GUITextures) will move along with the cameras during the transition. To avoid this, make another camera that only renders GUIElements on top of the other cameras, and nothing else. If you're using OnGUI, then the GUI won't be affected. 
The function is a coroutine: 
function SquishWipe (camera1 : Camera, camera2 : Camera, wipeTime : float, transitionType : TransitionType) : IEnumerator 
camera1 is the camera that you are wiping from and camera2 is the camera you are wiping to. wipeTime is the length of time, in seconds, it takes to complete the wipe. transitionType is the TransitionType, which should be either TransitionType.Left, TransitionType.Right, TransitionType.Up, or TransitionType.Down to wipe in the respective direction. 
JavaScript - SquishWipeExample.jsvar camera1 : Camera;
var camera2 : Camera;
var wipeTime = 2.0;
var curve : AnimationCurve;
private var inProgress = false;
private var swap = false;
 
function Update () {
	if (Input.GetKeyDown("up")) {
		DoWipe(TransitionType.Up);
	}
	else if (Input.GetKeyDown("down")) {
		DoWipe(TransitionType.Down);
	}
	else if (Input.GetKeyDown("left")) {
		DoWipe(TransitionType.Left);
	}
	else if (Input.GetKeyDown("right")) {
		DoWipe(TransitionType.Right);
	}
}
 
function DoWipe (transitionType : TransitionType) {
	if (inProgress) return;
	inProgress = true;
 
	swap = !swap;
	yield ScreenWipe.use.SquishWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, transitionType);
	//yield ScreenWipe.use.SquishWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, transitionType, curve);
 
	inProgress = false;
}To use this you also need the ScreenWipes script. 
}
