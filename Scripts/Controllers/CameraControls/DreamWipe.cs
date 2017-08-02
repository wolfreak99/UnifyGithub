/*************************
 * Original url: http://wiki.unity3d.com/index.php/DreamWipe
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/DreamWipe.cs
 * File based on original modification date of: 18 January 2013, at 01:56. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CameraControls
{
    
    DescriptionFades from one camera to another, with a wavy distortion effect like those seen in transitions to dream sequences or flashbacks (usually accompanied by cheesy harp music). Requires Unity Pro. 
     
    UsageSee ScreenWipes for an example scene and the actual script that performs the wipe. The script below is an example of usage. It should be attached to an object, such as a manager object, and the ScreenWipes script should also be attached to the manager object. Also needed are two cameras, which must be in the same scene, naturally. In this example, you can press space to swap between the two cameras. 
    Drag the two cameras onto the appropriate slots in the inspector after you've attached the script to an object. The script has a FadeTime public variable; this is the time it will take for the wipe to complete. It also has WaveScale, which is the horizontal size of the wavy effect, and WaveFrequency, which is the vertical number of waves. 
    The ScreenWipe script itself has a public variable called PlaneResolution. In order to achieve the wavy effect, a plane is created with a number of segments. The higher the number for PlaneResolution, the more segments are made and the smoother the effect will look. The default of 90 should generally be fine for most situations. 
    The function is a coroutine: 
    function DreamWipe (camera1 : Camera, camera2 : Camera, fadeTime : float, waveScale : float = .07, waveFrequency : float = 25.0) : IEnumerator 
    camera1 is the camera that you are fading from and camera2 is the camera you are fading to. After the fade is complete, camera1 will automatically be set to inactive and camera2 will be set to active. fadeTime is the length of time, in seconds, it takes to complete the fade. waveScale is the horizontal size of the "bulges" in the wavy effect. This is clamped between -.5 and .5; a size of 0 means the effect won't be visible (which means it will look very similar to CrossFadePro). Negative values are the inverse of positive values, but in practice look pretty much the same. waveFrequency affects how many of the wavy bits there are vertically, with a higher number resulting in smaller waves. You can choose to leave out waveScale and waveFrequency, in which case the defaults of .07 and 25.0 respectively will be used. 
    Additionally, there is a function called InitializeDreamWipe: 
    function InitializeDreamWipe () : void 
    This is called automatically if needed, but you may want to call it manually (one time only, like in a Start function) before using the DreamWipe function. Not doing so may cause a slight hiccup in the framerate the first time the DreamWipe function is used, since some necessary elements are created. You may experience a hiccup in the framerate anyway if the second camera is looking at objects with textures that have not yet been uploaded to the graphics card. 
    Notes: 
    If you see objects turning transparent or disappearing during the fade when they shouldn't, check the opacity on the color of their materials. Solid, non-transparent objects should have opacity set to 100%. This applies to any solid background colors and skyboxes you may be using as well. They may look fine normally with less opacity, but the rendertexture picks up their alpha and does funky things with it. 
    Any GUIElements (GUITexts and GUITextures) will move along with the cameras during the transition. To avoid this, make another camera that only renders GUIElements on top of the other cameras, and nothing else. If you're using OnGUI, then the GUI won't be affected. 
    JavaScript - DreamWipeExample.jsvar camera1 : Camera;
    var camera2 : Camera;
    var fadeTime = 4.0;
    var waveScale = .07;				// Higher numbers make the effect more exaggerated. Can be negative, .5/-.5 is the max
    var waveFrequency = 25.0;			// Higher numbers make more waves in the effect
    private var inProgress = false;
    private var swap = false;
     
    function Start () {
    	ScreenWipe.use.InitializeDreamWipe();
    }
     
    function Update () {
    	if (Input.GetKeyDown("space")) {
    		DoFade();
    	}
    }
     
    function DoFade () {
    	if (inProgress) return;
    	inProgress = true;
     
    	swap = !swap;
    	yield ScreenWipe.use.DreamWipe (swap? camera1 : camera2, swap? camera2 : camera1, fadeTime, waveScale, waveFrequency);
     
    	inProgress = false;
}To use this you also need the ScreenWipes script. 
}
