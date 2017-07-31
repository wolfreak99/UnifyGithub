// Original url: http://wiki.unity3d.com/index.php/SplashScreen
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/SplashScreen.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
SplashScreenFades in a splash picture on the screen, waits for a few seconds (or waits for user input), then fades out while the next scene is loaded. 
TODO: 
Console still reports the error message: "There are 2 audio listeners in the scene..." Its not critical but it would be nice to get rid of this. 
Any additions to this script are always welcome! 
Just create a new scene with a camera and add the script to the camera and set properties. 
C# Codeusing UnityEngine;
using System.Collections;
 
//
// SplashScreen Script
//
// Version 0.1 by Martijn Dekker
// martijn.pixelstudio@gmail.com
//
// Version 0.2 by Ferdinand Joseph Fernandez, 2010Sep7 16:45 GMT + 8
// Changes:
//  * changed levelToLoad to a string, for easier usage
//  * added waitTime, which adds a pause after fade in, and before fade
//    out (during fade waiting)
//  * added option to either automatically fade out after waitTime
//    seconds (default), or wait for user input (press any key to continue)
//  * added option to wait until fade out is complete before loading next
//    level, instead of the default, which is to load the next level
//    before fade out
//
// Version 0.3 by Ferdinand Joseph Fernandez, 2010Sep8 01:13 GMT + 8
// Changes:
//  * splash screen itself is now fading without the need for a solid
//    background color
//  * optimized some code
//
// Version 0.4 by Ferdinand Joseph Fernandez, 2010Sep14 14:09 GMT + 8
// Changes:
//  * splash screen picture can now be either centered (default) or
//    stretched on the screen
//
// Version 0.5 by Ferdinand Joseph Fernandez, 2010Sep15 18:27 GMT + 8
// Changes:
//  * now has option to start automatically or not. if not started
//    automatically, the splash screen can be started by calling
//    the StartSplash function
//  * code acknowledges if the levelToLoad is blank, in that case,
//    the code simply does not attempt to load a level
//
// Version 0.6 by Ferdinand Joseph Fernandez, 2010Sep29 13:43 GMT + 8
// Changes:
//  * added the property "gui depth" so you can control at which depth the
//    splash screen shows in
//
 
public class SplashScreen : MonoBehaviour
{
	public int guiDepth = 0;
	public string levelToLoad = ""; // this has to correspond to a level (file>build settings)
	public Texture2D splashLogo; // the logo to splash;
	public float fadeSpeed = 0.3f;
	public float waitTime = 0.5f; // seconds to wait before fading out
	public bool waitForInput = false; // if true, this acts as a "press any key to continue"
	public bool startAutomatically = true;
	private float timeFadingInFinished = 0.0f;
 
	public enum SplashType
	{
		LoadNextLevelThenFadeOut,
		FadeOutThenLoadNextLevel
	}
	public SplashType splashType;
 
	private float alpha = 0.0f;
 
	private enum FadeStatus
	{
		Paused,
		FadeIn,
		FadeWaiting,
		FadeOut
	}
	private FadeStatus status = FadeStatus.FadeIn;
 
	private Camera oldCam;
	private GameObject oldCamGO;
 
	private Rect splashLogoPos = new Rect();
	public enum LogoPositioning
	{
		Centered,
		Stretched
	}
	public LogoPositioning logoPositioning;
 
	private bool loadingNextLevel = false;
 
	void Start()
	{
		if (startAutomatically)
		{
			status = FadeStatus.FadeIn;
		}
		else
		{
			status = FadeStatus.Paused;
		}
		oldCam = Camera.main;
		oldCamGO = Camera.main.gameObject;
 
		if (logoPositioning == LogoPositioning.Centered)
		{
			splashLogoPos.x = (Screen.width * 0.5f) - (splashLogo.width * 0.5f);
			splashLogoPos.y = (Screen.height * 0.5f) - (splashLogo.height * 0.5f);
 
			splashLogoPos.width = splashLogo.width;
			splashLogoPos.height = splashLogo.height;
		}
		else
		{
			splashLogoPos.x = 0;
			splashLogoPos.y = 0;
 
			splashLogoPos.width = Screen.width;
			splashLogoPos.height = Screen.height;
		}
 
 
 
		if (splashType == SplashType.LoadNextLevelThenFadeOut)
		{
			DontDestroyOnLoad(this);
			DontDestroyOnLoad(Camera.main);
		}
		if ((Application.levelCount <= 1) || (levelToLoad == ""))
		{
			Debug.LogWarning("Invalid levelToLoad value.");
		}
	}
 
	public void StartSplash()
	{
		status = FadeStatus.FadeIn;
	}
 
	void Update()
	{
		switch(status)
		{
			case FadeStatus.FadeIn:
				alpha += fadeSpeed * Time.deltaTime;
			break;
			case FadeStatus.FadeWaiting:
				if ((!waitForInput && Time.time >= timeFadingInFinished + waitTime) || (waitForInput && Input.anyKey))
				{
					status = FadeStatus.FadeOut;
				}
			break;
			case FadeStatus.FadeOut:
				alpha += -fadeSpeed * Time.deltaTime;
			break;
		}
	}
 
	void OnGUI()
	{
		GUI.depth = guiDepth;
		if (splashLogo != null)
		{
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp01(alpha));
			GUI.DrawTexture(splashLogoPos, splashLogo);
			if (alpha > 1.0f)
			{
				status = FadeStatus.FadeWaiting;
				timeFadingInFinished = Time.time;
				alpha = 1.0f;
				if (splashType == SplashType.LoadNextLevelThenFadeOut)
				{
					oldCam.depth = -1000;
					loadingNextLevel = true;
					if ((Application.levelCount >= 1) && (levelToLoad != ""))
					{
						Application.LoadLevel(levelToLoad);
					}
				}
			}
			if (alpha < 0.0f)
			{
				if (splashType == SplashType.FadeOutThenLoadNextLevel)
				{
					if ((Application.levelCount >= 1) && (levelToLoad != ""))
					{
						Application.LoadLevel(levelToLoad);
					}
				}
				else
				{
					Destroy(oldCamGO); // somehow this doesn't work
					Destroy(this);
				}
			}
		}
	}
 
	void OnLevelWasLoaded(int lvlIdx)
	{
		if (loadingNextLevel)
		{
			Destroy(oldCam.GetComponent<AudioListener>());
			Destroy(oldCam.GetComponent<GUILayer>());
		}
	}
 
	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, .5f);
		Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
	}
}

UnityScript Code//
// SplashScreen Script
//
// This is the same script as above (only a few redundant things were removed), just converted to UnityScript
// to help out those sticking to unity script, and to help those just starting out with it.
// Quantum Fusion Studios
 
var guiDepth : int = 0;
var levelToLoad : String = "";
var splashLogo : Texture2D;
var fadeSpeed : float = 0.3;
var waitTime : float = 0.5;
var waitForInput : boolean = false;
var startAutomatically : boolean = true;
var timeFadingInFinished : float = 0.0;
var logoPositioning : LogoPositioning;
var splashType : SplashType;
enum SplashType {LoadNextLevelThenFadeOut, FadeOutThenLoadNextLevel}
enum LogoPositioning {Centered, Stretched}
private var alpha : float = 0.0;
private var status : FadeStatus = FadeStatus.FadeIn;
private var oldCam : Camera;
private var oldCamGO : GameObject;
private var splashLogoPos : Rect;
private var loadingNextLevel : boolean = false;
private enum FadeStatus {Paused, FadeIn, FadeWaiting, FadeOut}
 
 
function Start()
{
	if (startAutomatically)
	{
		status = FadeStatus.FadeIn;
	}
 
	else
	{
		status = FadeStatus.Paused;
	}
 
	oldCam = Camera.main;
	oldCamGO = Camera.main.gameObject;
 
	if (logoPositioning == LogoPositioning.Centered)
	{
		splashLogoPos.x = (Screen.width * 0.5) - (splashLogo.width * 0.5);
		splashLogoPos.y = (Screen.height * 0.5) - (splashLogo.height * 0.5);
 
		splashLogoPos.width = splashLogo.width;
		splashLogoPos.height = splashLogo.height;
	}
 
	else
	{
		splashLogoPos.x = 0;
		splashLogoPos.y = 0;
 
		splashLogoPos.width = Screen.width;
		splashLogoPos.height = Screen.height;
	}
 
	if (splashType == SplashType.LoadNextLevelThenFadeOut)
	{
		DontDestroyOnLoad(this);
		DontDestroyOnLoad(Camera.main);
	}
 
 
	if ((Application.levelCount <= 1) || (levelToLoad == ""))
	{
		Debug.LogWarning("Invalid levelToLoad value.");
	}
}
 
 
 
function Update()
{
	switch(status)
	{
		case FadeStatus.FadeIn:
			alpha += fadeSpeed * Time.deltaTime;
		break;
 
		case FadeStatus.FadeWaiting:
			if ((!waitForInput && Time.time >= timeFadingInFinished + waitTime) || (waitForInput && Input.anyKey))
			{
				status = FadeStatus.FadeOut;
			}
		break;
 
		case FadeStatus.FadeOut:
			alpha += -fadeSpeed * Time.deltaTime;
		break;
	}
}
 
 
 
function OnGUI()
{
	GUI.depth = guiDepth;
 
	if (splashLogo != null)
	{
		GUI.color = Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp01(alpha));
		GUI.DrawTexture(splashLogoPos, splashLogo);
 
			if (alpha > 1.0)
			{
				status = FadeStatus.FadeWaiting;
				timeFadingInFinished = Time.time;
				alpha = 1.0;
 
				if (splashType == SplashType.LoadNextLevelThenFadeOut)
				{
					oldCam.depth = -1000;
					loadingNextLevel = true;
 
					if ((Application.levelCount >= 1) && (levelToLoad != ""))
					{
						Application.LoadLevel(levelToLoad);
					}
				}
			}
 
			if (alpha < 0.0)
			{
				if (splashType == SplashType.FadeOutThenLoadNextLevel)
				{
					if ((Application.levelCount >= 1) && (levelToLoad != ""))
					{
						Application.LoadLevel(levelToLoad);
					}
				}
 
				else
				{
					Destroy(oldCamGO);
				}
			}
	}
}
 
 
 
function OnLevelWasLoaded(lvlIdx : int)
{
	if (loadingNextLevel)
	{
		Destroy(oldCam);
	}
}
 
 
function OnDrawGizmos()
{
	Gizmos.color = Color(1, 0, 0, 0.5);
	Gizmos.DrawCube(transform.position, Vector3(1, 1, 1));
}
 
 
function StartSplash()
{
	status = FadeStatus.FadeIn;
}
}
