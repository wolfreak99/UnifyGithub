/*************************
 * Original url: http://wiki.unity3d.com/index.php/IPhoneToMouse
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/IPhoneToMouse.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Kiyaku 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    Description Sometimes, when i am working on an iPhone Application, the Client wants to try the game without using an iPhone. So i wrote a little script that converts some Mouse Functions into iPhoneInput function. You just have to add a few lines but can keep your original gameplay code. 
    Right now it can recognize touchCount 0 or 1, positions and returns "iPhoneTouchPhase Began, Moved, Ended". 
    Feel free to extend it! 
    Usage Create a file named "iPhoneToMouse.cs" and add following code to it: 
    using UnityEngine;
    using System.Collections;
     
    public class iPhoneToMouse
    {
    	public int touchCount
    	{
    		get
    		{
    			if(Input.GetMouseButton(0) || Input.GetMouseButton(1) || 
    			   Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) )
    				return 1;
    			else
    				return 0;
    		}
    	}
     
     
    	public pos GetTouch(int ID)
    	{
    		pos tempPos = new pos();
     
    		tempPos.position = Input.mousePosition;
     
    		if(Input.GetMouseButtonDown(ID))
    			tempPos.phase = iPhoneTouchPhase.Began;
    		else if(Input.GetMouseButton(ID))
    			tempPos.phase = iPhoneTouchPhase.Moved;
     
    		if(Input.GetMouseButtonUp(ID))
    			tempPos.phase = iPhoneTouchPhase.Ended;
     
    		return tempPos;
    	} 
     
     
    	public struct pos
    	{
    		public Vector2 position;
    		public iPhoneTouchPhase phase;
    	}
    }
     
     
    public enum iPhoneTouchPhase
    {
    	Moved,
    	Ended,
    	Began
    }
    Add following lines to your scripts which include iPhoneTouch commands: 
    private iPhoneToMouse iPhoneInput;
     
    void Start()
    {
    	iPhoneInput = new iPhoneToMouse();
}That's it! 
}
