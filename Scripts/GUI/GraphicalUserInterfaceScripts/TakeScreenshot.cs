// Original url: http://wiki.unity3d.com/index.php/TakeScreenshot
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/TakeScreenshot.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Contents [hide] 
1 Description 
2 Usage 
3 C# - TakeScreenshot.cs 
4 C# - Screenshot.cs 

DescriptionCaptures sequentially numbered screenshots when a function key is pressed. Existing screenshots are not overwritten. 
UsageJust attach this script to an empty game object. 
C# - TakeScreenshot.cs// TODO:
// By default, screenshot files are placed next to the executable bundle -- we don't want this in a
// shipping game, as it will fail if the user doesn't have write access to the Applications folder.
// Instead we should place the screenshots on the user's desktop. However, the ~/ notation doesn't
// work, and Unity doesn't have a mechanism to return special paths. Therefore, the correct way to
// solve this is probably with a plug-in to return OS specific special paths.
 
// Mono/.NET has functions to get special paths... see discussion page. --Aarku
 
using UnityEngine;
using System.Collections;
 
public class TakeScreenshot : MonoBehaviour
{    
    private int screenshotCount = 0;
 
    // Check for screenshot key each frame
    void Update()
    {
        // take screenshot on up->down transition of F9 key
        if (Input.GetKeyDown("f9"))
        {        
            string screenshotFilename;
            do
            {
                screenshotCount++;
                screenshotFilename = "screenshot" + screenshotCount + ".png";
 
            } while (System.IO.File.Exists(screenshotFilename));
 
            Application.CaptureScreenshot(screenshotFilename);
        }
    }
}

C# - Screenshot.cs// ******  Notice : It doesn't works in Wep Player environment.  ******
// ******    It works in PC environment.                         ******
// Default method have some problem, when you take a Screen shot for your game. 
// So add this script.
// CF Page : http://technology.blurst.com/unity-jpg-encoding-javascript/
// made by Jerry ( sdragoon@nate.com )
 
using UnityEngine;
using System.Collections;
using System.IO;
 
public class ScreenShot : MonoBehaviour
{
    private int count = 0;
 
    void Update()
    {
        if (Input.GetKeyDown("k"))
            StartCoroutine(ScreenshotEncode());
    }
 
    IEnumerator ScreenshotEncode()
    {
        // wait for graphics to render
        yield return new WaitForEndOfFrame();
 
        // create a texture to pass to encoding
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
 
        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
 
        // split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
        yield return 0;
 
        byte[] bytes = texture.EncodeToPNG();
 
        // save our test image (could also upload to WWW)
        File.WriteAllBytes(Application.dataPath + "/../testscreen-" + count + ".png", bytes);
        count++;
 
        // Added by Karl. - Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
        DestroyObject( texture );
 
        //Debug.Log( Application.dataPath + "/../testscreen-" + count + ".png" );
    }
}
}
