/*************************
 * Original url: http://wiki.unity3d.com/index.php/CrossFade
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/CrossFade.cs
 * File based on original modification date of: 31 October 2013, at 05:07. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CameraControls
{
    
    DescriptionMakes a smooth cross-fade from one camera view to another. Requires Unity 2.6 or Unity 3.x. (Doesn't work on Unity 4.) 
     
    UsageSee ScreenWipes for an example scene and the actual script that performs the wipe. The script below is an example of usage. It should be attached to an object, such as a manager object, and the ScreenWipes script should also be attached to the manager object. Also needed are two cameras, which must be in the same scene, naturally. In this example, you can press space to swap between the two cameras. 
    Drag the two cameras onto the appropriate slots in the inspector after you've attached the script to an object. The script has a FadeTime public variable; this is the time it will take for the wipe to complete. It uses a fake "render texture" technique involving Texture2D.ReadPixels(), which means it doesn't need Unity Pro, but the downside is that the image in the first camera will become static during the fade, although the second camera can have motion. Also, it doesn't really work right in the editor. This is just cosmetic, and doesn't affect an actual game build. 
    The function is a coroutine: 
    function CrossFade (camera1 : Camera, camera2 : Camera, fadeTime : float) : IEnumerator 
    camera1 is the camera that you are fading from, camera2 is the camera you are fading to, and fadeTime is the length of time, in seconds, it takes to complete the fade. 
    JavaScript - CrossFadeExample.jsvar camera1 : Camera;
    var camera2 : Camera;
    var fadeTime = 2.0;
    var curve : AnimationCurve;
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
    	yield ScreenWipe.use.CrossFade (swap? camera1 : camera2, swap? camera2 : camera1, fadeTime);
    	//yield ScreenWipe.use.CrossFade (swap? camera1 : camera2, swap? camera2 : camera1, fadeTime, curve);
     
    	inProgress = false;
}To use this you also need the ScreenWipes script. 
}
