// Original url: http://wiki.unity3d.com/index.php/CrossFadePro
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/CrossFadePro.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: Eric Haines (Eric5h5) 
DescriptionMakes a smooth cross-fade from one camera view to another. Unlike the original CrossFade, the image in the first camera can be moving during the transition. Also it's faster since it doesn't require manually reading the entire screen into a texture and applying it, but uses a rendertexture instead, like the full-screen image effects do. Requires Unity Pro and Unity 2.6 or later. 
 
UsageSee ScreenWipes for an example scene and the actual script that performs the wipe. The script below is an example of usage. It should be attached to an object, such as a manager object, and the ScreenWipes script should also be attached to the manager object. Also needed are two cameras, which must be in the same scene, naturally. In this example, you can press space to swap between the two cameras. 
Drag the two cameras onto the appropriate slots in the inspector after you've attached the script to an object. The script has a FadeTime public variable; this is the time it will take for the wipe to complete. 
The function is a coroutine: 
function CrossFadePro (camera1 : Camera, camera2 : Camera, fadeTime : float) : IEnumerator 
camera1 is the camera that you are fading from, camera2 is the camera you are fading to, and fadeTime is the length of time, in seconds, it takes to complete the fade. 
Notes: 
If you see objects turning transparent or disappearing during the fade when they shouldn't, check the opacity on the color of their materials. Solid, non-transparent objects should have opacity set to 100%. This applies to any solid background colors and skyboxes you may be using as well. They may look fine normally with less opacity, but the rendertexture picks up their alpha and does funky things with it. 
Any GUIElements (GUITexts and GUITextures) will overbrighten during the transition, since the two cameras are rendering them on top of each other, if both cameras have a GUILayer. To avoid this, make another camera that only renders GUIElements on top of the other cameras, and nothing else. If you're using OnGUI, then the GUI won't be affected. 
JavaScript - CrossFadeProExample.jsvar camera1 : Camera;
var camera2 : Camera;
var fadeTime = 2.0;
private var inProgress = false;
private var swap = false;
 
function Update () {
	if (Input.GetKeyDown("space")) {
		DoFade();
	}
}
 
function DoFade () {
	if (inProgress) return;
	inProgress = true;
 
	swap = !swap;
	yield ScreenWipe.use.CrossFadePro (swap? camera1 : camera2, swap? camera2 : camera1, fadeTime);
 
	inProgress = false;
}To use this you also need the ScreenWipes script. 
}
