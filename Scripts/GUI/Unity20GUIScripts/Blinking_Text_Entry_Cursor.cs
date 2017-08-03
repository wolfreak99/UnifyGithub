/*************************
 * Original url: http://wiki.unity3d.com/index.php/Blinking_Text_Entry_Cursor
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/Blinking_Text_Entry_Cursor.cs
 * File based on original modification date of: 19 January 2012, at 19:03. 
 *
 * Author: Joseph Quigley (CPUFreak91) 
 *
 * Description 
 *   This script will allow you to display a blinking cursor (which can be any character, but I'm using an underline). 
 * Usage 
 *   Assign this behavior to any GameObject. It was designed to work with the example in the Input String Example 
 *   in the Unity docs, so you may want to move this code into your input-related scripts. Append the cursorChar 
 *   variable to the end of your input string. 
 * Example
     GUI.Label (Rect(400,35, 200, 70), "Type Your Name: " + nameString + cursorChar);
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    using UnityEngine;
    using System.Collections;

    public class BlinkingCursor : MonoBehaviour
    {
        private float m_TimeStamp;
        private bool cursor = false;
        private string cursorChar = "";
        private int maxStringLength = 24;
        
        // TODO UnifyGithub: figure out what "enteredString" was supposed to be. this line was originally missing
        public string enteredString;

        void Update()
        {
            if (Time.time - m_TimeStamp >= 0.5) {
                m_TimeStamp = Time.time;
                if (cursor == false) {
                    cursor = true;
                    if (enteredString.Length < maxStringLength) {
                        cursorChar += "_";
                    }
                }
                else {
                    cursor = false;
                    if (cursorChar.Length != 0) {
                        cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
                    }
                }
            }
        }
    }
}
