// Original url: http://wiki.unity3d.com/index.php/Take3DScreenshot
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/Take3DScreenshot.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Contents [hide] 
1 Description 
2 Usage 
3 Examples 
4 C# - Take3DScreenshot.cs 

DescriptionThis editor script lets you take "3D Screenshots" of a specified object using the Game View. A 3D Screenshot is special type of movie that gives a "bullet-time" type camera rotation effect around a paused scene. Useful for the creation of QuicktimeVR. 
UsagePlace this script in YourProject/Assets/Editor and a menu item will automatically appear in the Custom menu after it is compiled. 
If capturing an action scene, pause your game at the exact moment you want to capture. Make sure the camera is the distance you want it to be from the object during capture. If it isn't, you can usually just move it into position while paused. 
Select Custom -> Take 3D Screenshot of Game View from the Custom menu. 
In the wizard, change the folder name to match your system (make sure the folder exists). 
Next, select the camera you want to use with useCamera, and the object to rotate the camera around with rotateAround. 
The everyXdegrees parameter let's you choose how many screenshots you'll take and the fluidity of the final sequence. A lower number here means more images will be created; for example a value of 1 means 360 images will be created, 1 for each degree in a circle, default of 30 means 12 images will be made, and so on. More images result in a more fluid or smooth sequence. 
The captureDelayMs value is the time in milliseconds that will be delayed between each screenshot (to allow the script to save the image to your hard disk). Best to leave captureDelayMs as is, but if you have a slower system and/or a low everyXdegrees number and are finding that some images in the sequence are missing, then you might increase this value. 
Click Create and sit back and wait until the process has finished. 
Now you will have a series of images. "What you do from here is up to you" as the tools to create QTVR are currently pretty bad (especially on OS X, ironically). The exact procedure for making an object movie depends on the tool you choose. You generally specify the number of rows, the degrees of rotation and number of images per row, the initial view, and the folder that contains the images, and then the software creates the movie. Reluctantly, I recommend a windows program to do this: Pano2QTVR Microsoft ICE also works well: ICE 
Special thanks to NCarter for helping with the threading. 
ExamplesYou can find 3d screenshots for many popular games here: PanoGames 
C# - Take3DScreenshot.csusing UnityEngine;
using UnityEditor;
using System.Threading;
using System.Collections;
 
public class Take3DScreenshot : ScriptableWizard {
    public static string fileName = "Unity 3D Screenshot _";
    public static string folder = "/Users/Casemon/Desktop/Screens/";
    public GameObject useCamera;
    public GameObject rotateAround;
    public int everyXdegrees = 30;
	public int captureDelayMs = 500;
 
    [MenuItem ("Custom/Take 3D Screenshot of Game View")]
 
    static void DoSet () {
        ScriptableWizard.DisplayWizard("Set params for 3D Screenshot", typeof(Take3DScreenshot), "Create");
    }
 
    void OnWizardUpdate () {
        helpString = "Set the parameters to create a series of pictures in a circle... \n\nThese settings will create: " + (360 / everyXdegrees) + " images";
 
    }
 
    void OnWizardCreate () {
        Thread thread = new Thread (TakeScreenshot);
        thread.Start ();
    }
 
    void TakeScreenshot () {
        for (int number = 0; number < (360 / everyXdegrees); number++) {
            Application.CaptureScreenshot(folder +fileName +number+"_"+number.ToString("d4") +".png");
 
            if (rotateAround)
                useCamera.transform.RotateAround(rotateAround.transform.position, Vector3.up, everyXdegrees);
            else
                useCamera.transform.Rotate(Vector3.up, everyXdegrees);
 			Debug.Log ("Saving " +folder +fileName +number.ToString("d4") +".png... DO NOT use the Unity Editor!!");
            Thread.Sleep (captureDelayMs);
        }
		Debug.Log ("All Done! We now return you to your previously scheduled Unity work...");
    }
}
}
