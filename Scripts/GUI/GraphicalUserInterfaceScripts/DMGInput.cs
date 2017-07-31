// Original url: http://wiki.unity3d.com/index.php/DMGInput
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/DMGInput.cs
// File based on original modification date of: 30 January 2012, at 20:04. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
DescriptionAn alternative to normal Input script that allows you to register 'dead zone' Rects Initially used for UI rects 
UsageExaples     void Update()
     {
          if (DMGInput.CursorIsOverAnyWindow())
                return;
 
          //Things here that we want to ignore if we're over the UI, ex raycasts into the scene, etc..
     }
 
//instead of Input
     if (DMGInput.GetMouseButtonDown(0))
     {
          //Stuff we only want to happen if we didn't click within the RECT..
     }C# - DMGInput.cs/* - DMGInput.cs v1.0 by Dave Buchhofer
 * 
 * - An alternative to normal Input script that allows you to register 'dead zone' Rects
 * - Initially used for UI rects, and for demonstration/learning of usage of the Messenger script
 * 
 * 
 *      
 *      if(DMGInput.GetMouseButton(1)) { Debug.Log("We clicked and it wasn't on the UI!"); }
 */
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class DMGInput : MonoBehaviour {
 
    static Dictionary<string,Rect> rects = new Dictionary<string,Rect>();
 
    public static bool GetMouseButtonDown(int btn)
    {
        if (!Input.GetMouseButtonDown(btn)) return false;
        if (CursorIsOverAnyWindow()) return false;
        return true;
    }
    public static bool GetMouseButton(int btn)
    {
        if (!Input.GetMouseButton(btn)) return false;
        if (CursorIsOverAnyWindow()) return false;
        return true;
    }
    public static bool CursorIsOverAnyWindow()
    {
        //the mouse position in GUI screen space
        Vector2 guiCursorPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
 
        //only check against rects if we are inside of the window at all
        if (new Rect(0, 0, Screen.width, Screen.height).Contains(guiCursorPos))
        {
            //check to see if we have registered any rects to click against..
            //if none, just return that we're not over any windows
            if (rects.Count == 0) return false;
 
            //if we have rects, cycle through the list and check against the cursor position
            foreach (KeyValuePair<string,Rect> usedRect in rects)
            {
                if (usedRect.Value.Contains(guiCursorPos)) return true;
            }
        }
        //we didn't hit anything if we made it this far, FALSE it up!
        return false;
    }
 
    public static void AddRect(string index, Rect newRect)
    {
        //if we dont already have this rect
        if (!rects.ContainsKey(index))
        {
            //then add it to the array
            rects.Add(index, newRect);
        }
        else
        //else remove the old entry and update it with the new rect
        {
            rects.Remove(index);
            rects.Add(index, newRect);
        }
    }
 
    public static void RemoveRect(string index)
    {
        if (rects.ContainsKey(index))
        {
            rects.Remove(index);
        }
    }
}
}
