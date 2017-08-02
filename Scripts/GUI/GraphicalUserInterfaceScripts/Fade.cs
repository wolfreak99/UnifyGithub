/*************************
 * Original url: http://wiki.unity3d.com/index.php/Fade
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/Fade.cs
 * File based on original modification date of: 31 October 2013, at 03:59. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * Description 
 *   
 * Usage 
 *   
 * Examples 
 *   
 * JavaScript - Fade.js 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Description Fade GUITextures or anything that has a material (including GUIText objects) at any time with these simple coroutines that can be called from anywhere. Fade alpha values in and out, or fade from one color to another, with optional ease in/out. You can also use an array of colors. 
    Usage These functions are coroutines, so you normally don't use them in Update (which always runs once every frame and is therefore not suitable for scheduling events). The exception is if you're just launching a Fade routine once at a specific time; see below for an example of making a GUIText fade when hitting a key. Normally you'd use Fade functions from a coroutine. 
    Put this script in your Standard Assets/Scripts folder or Plugins folder; this way it can be easily used from C# or Boo. It should be named "Fade". The script must be attached to some object in the scene, such as an empty object used for game manager scripts. If you don't feel like reading a bunch of text, see below for some examples. Otherwise keep reading for all the technical details.... 
    To do alpha fading, call Fade.use.Alpha(), which has these parameters: 
    function Alpha(object : Object, start : float, end : float, timer : float, easeType : EaseType = EaseType.None) : IEnumerator 
    The object should be either a GUITexture or a Material. Any other type will print a warning message at run-time, and the function won't do anything. This works fine on the default material for GUIText objects, but other objects should have a material which uses a Transparent shader of some kind in order for it to have any visible effect. 
    The alpha value will fade from start to end, over timer seconds. Therefore, fading in would go from 0.0 through 1.0, and fading out would go from 1.0 through 0.0. You don't have to use 0 or 1; you could fade in part way, for example, by going from 0.0 through 0.5. 
    The optional easeType is None, In, Out, or InOut. EaseType.None is the default and is a straight linear fade. EaseType.In will more gradually start the fade. The effect is slightly subtle, but noticeable, since fading in alpha with a linear fade tends to look a little abrupt. EaseType.Out will more gradually end the fade, and would obviously be best used at the end of a fadeout for the same reason. As you would expect, EaseType.InOut eases at both ends. 
    To do color fading, call Fade.use.Colors(): 
    function Colors(object : Object, start : Color, end : Color, timer : float, easeType : EaseType = EaseType.None) : IEnumerator 
    This works the same as Alpha, except that start and end are colors instead of alpha values (although the colors themselves can contain transparency if desired). 
    To use a range of colors: 
    function Colors(object : Object, colorRange : Color[], timer : float, repeat : boolean) : IEnumerator 
    In this case, colorRange is an array of colors (Color[]), timer is how many seconds it takes to cycle through back to the first color, and repeat is whether the colors should be cycled through once (if false), or infinitely (if true). To stop an infinitely cycling Colors function, call Fade.use.StopAllCoroutines(). If colorRange has fewer than 2 entries, a warning message will be printed at run-time and the function won't do anything. 
    Note on GUITextures: these are capable of overbrightening colors, where a value of .5 means 100%, and 1.0 means 200%. It's likely that this feature isn't actually used much, and it's not always easy to remember that an alpha of, say, .25 is actually 50% transparent when using a GUITexture. So all the Fade functions compensate for this, which allows you to use 1.0 to mean 100% as usual. You can still overbrighten if desired by using values from 1.0 - 2.0, though colors like this can't be specified in the Inspector and must be created in code. 
    Examples Fade out a GUITexture over 3 seconds: 
    Fade.use.Alpha(guiTexture, 1.0, 0.0, 3.0);
    Fade in a GUITexture over 5 seconds, easing in at the beginning: 
    Fade.use.Alpha(guiTexture, 0.0, 1.0, 5.0, EaseType.In);
    Fade in another object which has a GUITexture component over 2 seconds, wait 1.5 seconds, then fade out again and deactivate the object: 
    var title : GameObject;
     
    function Start () {
    	yield Fade.use.Alpha(title.guiTexture, 0.0, 1.0, 2.0, EaseType.In);
    	yield WaitForSeconds(1.5);
    	yield Fade.use.Alpha(title.guiTexture, 1.0, 0.0, 2.0, EaseType.Out);
    	title.SetActive (false);
    }
    Fade a GUIText from green to blue over 1 second: 
    Fade.use.Colors(guiText.material, Color.green, Color.blue, 1.0);
    Make a GUIText pulse from red to yellow several times, where each pulse takes 1.5 seconds: 
    function Start () {
    	guiText.text = "Warning!";
    	var colors = [Color.red, Color.yellow];
    	for (i = 0; i < 3; i++) {
    		yield Fade.use.Colors(guiText.material, colors, 1.5, false);
    	}
    }
    Make a GameObject continuously cycle through an array of colors defined in the Inspector, but stop after 30 seconds: 
    var colors : Color[];
     
    function Start () {
    	Fade.use.Colors(renderer.material, colors, 5.0, true);
    	yield WaitForSeconds(30.0);
    	Fade.use.StopAllCoroutines();
    }
    Make a GUIText object fade out over one second after pressing the "A" key: 
    private var faded = false;
     
    function Start () {
    	guiText.text = "Press 'A'";
    }
     
    function Update () {
    	if (!faded && Input.GetKeyDown(KeyCode.A)) {
    		faded = true;
    		Fade.use.Alpha(guiText.material, 1.0, 0.0, 1.0);
    	}
    }
    Make a GameObject fade from black to white with easing, in C#: 
    using UnityEngine;
     
    public class Test : MonoBehaviour {
    	void Start () {
    		StartCoroutine(Fade.use.Colors(renderer.material, Color.black, Color.white, 2.0f, EaseType.InOut));
    	}
    }Note: To start a coroutine you probably want to do it in the Fade.use (So instead of StartCoroutine you just call Fade.use.StartCoroutine), and then call Fade.use.StopAllCoroutines() when you want to stop them again. 
    This prevents you from stopping other coroutines in your script, while still being able to stop the fading. 
    JavaScript - Fade.js enum EaseType {None, In, Out, InOut}
    static var use : Fade;
     
    function Awake () {
    	if (use) {
    		Debug.LogWarning("Only one instance of the Fade script in a scene is allowed");
    		return;
    	}
    	use = this;
    }
     
    function Alpha (object : Object, start : float, end : float, timer : float) {
    	yield Alpha(object, start, end, timer, EaseType.None);
    }
     
    function Alpha (object : Object, start : float, end : float, timer : float, easeType : EaseType) {
    	if (!CheckType(object)) return;
    	var t = 0.0;
    	var objectType = typeof(object);
    	while (t < 1.0) {
    		t += Time.deltaTime * (1.0/timer);
    		if (objectType == GUITexture)
    			(object as GUITexture).color.a = Mathf.Lerp(start, end, Ease(t, easeType)) * .5;
    		else
    			(object as Material).color.a = Mathf.Lerp(start, end, Ease(t, easeType));
    		yield;
    	}
    }
     
    function Colors (object : Object, start : Color, end : Color, timer : float) {
    	yield Colors(object, start, end, timer, EaseType.None);
    }
     
    function Colors (object : Object, start : Color, end : Color, timer : float, easeType : EaseType) {
    	if (!CheckType(object)) return;
    	var t = 0.0;
    	var objectType = typeof(object);
    	while (t < 1.0) {
    		t += Time.deltaTime * (1.0/timer);
    		if (objectType == GUITexture)
    			(object as GUITexture).color = Color.Lerp(start, end, Ease(t, easeType)) * .5;
    		else
    			(object as Material).color = Color.Lerp(start, end, Ease(t, easeType));
    		yield;
    	}
    }
     
    function Colors (object : Object, colorRange : Color[], timer : float, repeat : boolean) {
    	if (!CheckType(object)) return;
    	if (colorRange.Length < 2) {
    		Debug.LogError("Error: color array must have at least 2 entries");
    		return;
    	}
    	timer /= colorRange.Length;
    	var i = 0;
    	var objectType = typeof(object);
    	while (true) {
    		var t = 0.0;
    		while (t < 1.0) {
    			t += Time.deltaTime * (1.0/timer);
    			if (objectType == GUITexture)
    				(object as GUITexture).color = Color.Lerp(colorRange[i], colorRange[(i+1) % colorRange.Length], t) * .5;
    			else
    				(object as Material).color = Color.Lerp(colorRange[i], colorRange[(i+1) % colorRange.Length], t);
    			yield;
    		}
    		i = ++i % colorRange.Length;
    		if (!repeat && i == 0) break;
    	}	
    }
     
    private function Ease (t : float, easeType : EaseType) : float {
    	if (easeType == EaseType.None)
    		return t;
    	else if (easeType == EaseType.In)
    		return Mathf.Lerp(0.0, 1.0, 1.0 - Mathf.Cos(t * Mathf.PI * .5));
    	else if (easeType == EaseType.Out)
    		return Mathf.Lerp(0.0, 1.0, Mathf.Sin(t * Mathf.PI * .5));
    	else
    		return Mathf.SmoothStep(0.0, 1.0, t);
    }
     
    private function CheckType (object : Object) : boolean {
    	if (typeof(object) != GUITexture && typeof(object) != Material) {
    		Debug.LogError("Error: object is a " + typeof(object) + ". It must be a GUITexture or a Material");
    		return false;
    	}
    	return true;
}
}
