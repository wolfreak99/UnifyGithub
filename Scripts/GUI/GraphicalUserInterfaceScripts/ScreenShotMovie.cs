// Original url: http://wiki.unity3d.com/index.php/ScreenShotMovie
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/ScreenShotMovie.cs
// File based on original modification date of: 18 March 2014, at 01:09. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Description This script saves a sequence of images when you hit play. Framerate will run at a constant rate (And game time adjusted). When complete you can import the image sequence into quicktime pro to create a movie out of it. 
Use Create a new javascript file in your Unity Project. (It does not have to be in an "Editor" folder.) 
Name it "ScreenshotMovie". 
Attach it to the camera you wish to record from. 
Adjust settings to your liking. 
Hit play and it should dump out image files. They will be in your project's root folder, so they will not appear in the Unity Editor. 
// The folder we place all screenshots inside.
// If the folder exists we will append numbers to create an empty folder.
var folder = "ScreenshotMovieOutput";
var frameRate = 25;
var sizeMultiplier : int = 1;
 
private var realFolder = "";
 
function Start () {
    // Set the playback framerate!
    // (real time doesn't influence time anymore)
    Time.captureFramerate = frameRate;
 
    // Find a folder that doesn't exist yet by appending numbers!
    realFolder = folder;
    count = 1;
    while (System.IO.Directory.Exists(realFolder)) {
        realFolder = folder + count;
        count++;
    }
    // Create the folder
    System.IO.Directory.CreateDirectory(realFolder);
}
 
function Update () {
    // name is "realFolder/shot 0005.png"
    var name = String.Format("{0}/shot {1:D04}.png", realFolder, Time.frameCount );
 
    // Capture the screenshot
    Application.CaptureScreenshot (name, sizeMultiplier);
}




CSharp version. 
using UnityEngine;
 
public class ScreenshotMovie : MonoBehaviour
{
	// The folder we place all screenshots inside.
	// If the folder exists we will append numbers to create an empty folder.
	public string folder = "ScreenshotMovieOutput";
	public int frameRate = 25;
	public int sizeMultiplier = 1;
 
	private string realFolder = "";
 
	void Start()
	{
		// Set the playback framerate!
		// (real time doesn't influence time anymore)
		Time.captureFramerate = frameRate;
 
		// Find a folder that doesn't exist yet by appending numbers!
		realFolder = folder;
		int count = 1;
		while (System.IO.Directory.Exists(realFolder))
		{
			realFolder = folder + count;
			count++;
		}
		// Create the folder
		System.IO.Directory.CreateDirectory(realFolder);
	}
 
	void Update()
	{
		// name is "realFolder/shot 0005.png"
		var name = string.Format("{0}/shot {1:D04}.png", realFolder, Time.frameCount);
 
		// Capture the screenshot
		Application.CaptureScreenshot(name, sizeMultiplier);
	}
}




Boo version. 
import UnityEngine
 
class ScreenshotMovie (MonoBehaviour): 
 
	public folder as string = "ScreenshotMovieOutput"
	public frameRate as int = 25
	public sizeMultiplier as int = 1
 
	private realFolder as string
 
	def Start ():
		# Set the playback framerate
		# (real time doesn't influence time anymore)
		Time.captureFramerate = frameRate
 
		# Find a folder that doesn't exist yet by appending numbers
		realFolder = folder
		count as int = 1
		while System.IO.Directory.Exists(realFolder):
 
			realFolder = folder + count
			count += 1
 
		# Create the folder
		System.IO.Directory.CreateDirectory(realFolder)
 
	def Update ():
		# name is realFolder/shot 0005.png
		name = string.Format("{0}/shot {1:D04}.png", realFolder, Time.frameCount)
 
		# Capture the screenshot
		Application.CaptureScreenshot(name, sizeMultiplier)
}
