/*************************
 * Original url: http://wiki.unity3d.com/index.php/Blinking_Text_Entry_Cursor
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/Blinking_Text_Entry_Cursor.cs
 * File based on original modification date of: 19 January 2012, at 19:03. 
 *
 * Author: Joseph Quigley (CPUFreak91) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 JavaScript - BlinkingCursor.js 
    4 C# - BlinkingCursor.cs 
    
    DescriptionThis script will allow you to display a blinking cursor (which can be any character, but I'm using an underline). 
    UsageAssign this behavior to any GameObject. It was designed to work with the example in the Input String Example in the Unity docs, so you may want to move this code into your input-related scripts. Append the cursorChar variable to the end of your input string. Example: 
    GUI.Label (Rect(400,35, 200, 70), "Type Your Name: " + nameString + cursorChar);JavaScript - BlinkingCursor.js//These are made private so that they don't appear in the Inspector
    private var m_TimeStamp = Time.time;
    private var cursor = false;
    private var cursorChar = "";
    var maxStringLength : int; // Maximum characters to allow in the text entry field
     
    function Update () {
        if (Time.time - m_TimeStamp >= 0.5) { // Display/remove the cursor ever 1/2 second
            m_TimeStamp = Time.time; // Reset the time stamp
            if (cursor == false) { //If the cursor is off, enable it
                cursor = true;
                if (enteredString.Length < maxStringLength) // Only show the cursor character, though, if the entered string is less than the maximum length.
                //If it's the same as, or more than the max length don't display the cursor character
                    cursorChar += "_";
            }
            else {
                cursor = false;
                if (cursorChar.Length != 0)
                    cursorChar = cursorChar.Substring(0, cursorChar.Length - 1); //Remove the cursor character. cursorChar = ""; would also work
            }
        }
    }C# - BlinkingCursor.csusing UnityEngine;
    using System.Collections;
     
    public class BlinkingCursor : MonoBehaviour {      
     
    	private float m_TimeStamp;
    	private bool cursor = false;
    	private string cursorChar = "";
    	private int maxStringLength = 24;
     
            void update() {
                 if (Time.time - m_TimeStamp >= 0.5)
           		{
           			m_TimeStamp = Time.time;
           			if (cursor == false)
           			{
                                    cursor = true;
           				if (enteredString.Length < maxStringLength)
           				{
           					cursorChar += "_";
           				}
           			}
           			else
           			{
           				cursor = false;
           				if (cursorChar.Length != 0)
           				{
           					cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
           				}
           			}
           		}
            }
}
}
