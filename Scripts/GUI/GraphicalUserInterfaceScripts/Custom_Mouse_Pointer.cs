// Original url: http://wiki.unity3d.com/index.php/Custom_Mouse_Pointer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/Custom_Mouse_Pointer.cs
// File based on original modification date of: 7 July 2012, at 00:22. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Kiyaku 
Contents [hide] 
1 Description 
2 Usage 
3 C# - mousePointer.cs 
4 C# - mousePointer.cs - with default and for colliders 

DescriptionIf you want a custom image for your mouse pointer but still have it in front of the OnGUI() elements, use this little script. 
UsageAttach this script to any gameObject you want, preferable a new empty one. Assign your custom Texture to the script. 
C# - mousePointer.csusing UnityEngine;
using System.Collections;
 
public class mousePointer : MonoBehaviour 
{
    public Texture2D cursorImage;
 
    private int cursorWidth = 32;
    private int cursorHeight = 32;
 
    void Start()
    {
        Screen.showCursor = false;
    }
 
 
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursorWidth, cursorHeight), cursorImage);
    }
}

C# - mousePointer.cs - with default and for colliders Uses a default resource and also only changes the cursor if it collides with the gameobject. 
/*
 * http://www.unifycommunity.com/wiki/index.php?title=Custom_Mouse_Pointer
 * Authors: Kiyaku, ananasblau
 */
using UnityEngine;
using System.Collections;
 
public class MousePointer : MonoBehaviour 
{
    public Texture2D cursorImage;
 
    private int cursorWidth = 16;
    private int cursorHeight = 16;
    private bool showCursor = false;
   private string defaultResource = "MousePointer";
 
    void Start()
    {
		if(!cursorImage) {
			cursorImage = (Texture2D) Resources.Load(defaultResource);
			Debug.Log(cursorImage);
		}
		//cursorImage = (Texture2D) Instantiate(cursorImage);
    }
 
 
    void OnMouseEnter()
    {
		Debug.Log("Entered");
		Screen.showCursor = false;
		showCursor = true;
	}
	void OnGUI() {
		if(showCursor) {
        	GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursorWidth, cursorHeight), cursorImage);
		}
    }
	void OnMouseExit()
	{
		Debug.Log("Left");
		showCursor = false;
		Screen.showCursor = true;
	}
}
}
