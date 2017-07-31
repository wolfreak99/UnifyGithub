// Original url: http://wiki.unity3d.com/index.php/MessageDisplayer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/HeadsUpDisplayScripts/MessageDisplayer.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.HeadsUpDisplayScripts
{
Author: Jonathan Czeck (aarku) 
Contents [hide] 
1 Description 
2 Usage 
3 C# - MessageDisplayer.cs 
4 Javascript - MessageDisplayer.js 

DescriptionUse this script on a guiText object to have status messages. Just call messageDisplayerObject.DisplayMessage("hello") and you'll get a line of self disappearing messages. TODO: Make this script more of a singleton. 
UsageUse this script on a guiText object to have status messages. 
C# - MessageDisplayer.csusing UnityEngine;
using System.Collections;
 
// Use this script on a guiText object to have status messages
// Just call messageDisplayerObject.DisplayMessage("hello") and you'll
// get a line of self disappearing messages.
 
public class MessageDisplayer : MonoBehaviour
{
    ArrayList messages = new ArrayList();
 
    public void DisplayMessage(string message)
    {
        messages.Add(message);
        UpdateDisplay();
        Invoke("DeleteOldestMessage", 5F);
    }
 
    void DeleteOldestMessage()
    {
        // The following "if statement" protects this function from
        // getting called by SendMessage from another script and
        // crashing.
        if (messages.Count > 0)
        {
            messages.RemoveAt(0);
            UpdateDisplay();
        }
    }
 
    void UpdateDisplay()
    {
        string formattedMessages = "";
 
        foreach (string message in messages)
        {
            formattedMessages += message + "\n";
        }
 
        guiText.text = formattedMessages;
    }
}
And now, PsychicParrot presents ... the same script! Same usage, just in Javascript, for fun :) 
Javascript - MessageDisplayer.js // Use this script on a guiText object to have status messages
// Just call messageDisplayerObject.DisplayMessage("hello") and you'll
// get a line of self disappearing messages.
 
private var messages = new Array();
 
function Start(){
 
 
}
 
function DisplayMessage(message) {
 
        messages.Add(message);
        UpdateDisplay();
        Invoke("DeleteOldestMessage", 3);
 
}
 
function DeleteOldestMessage() {
        // The following "if statement" protects this function from
        // getting called by SendMessage from another script and
        // crashing.
        if (messages.length > 0)
        {
            messages.Shift();
            UpdateDisplay();
        }
    }
 
function UpdateDisplay() {
 
        formattedMessages = "";
 
        for(l=0;l<messages.length;l++) 
        {
            formattedMessages += messages[l] + "\n";
        }
 
        guiText.text = formattedMessages;
 
}
}
