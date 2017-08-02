/*************************
 * Original url: http://wiki.unity3d.com/index.php/ToggleGravity
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/ToggleGravity.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Jonathan Czeck (aarku) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.GeneralConcepts
{
    Description This script illustrates how to toggle some property by clicking the mouse button. The script waits for the left mouse button to be pressed down. When this occurs, two courses of action can be taken dependant on the state of the gravityOn variable. If the gravity is currently on, save the gravity value in a variable, set the global gravity to zero, and change the state of gravityOn to false. If the gravity is currently off, restore the global gravity setting and set the state of the gravityOn variable to true. 
    Usage Place this script on any GameObject in your scene. Logically it should go on a master control GameObject named something like "GameController." 
    JavaScript - ToggleGravity.js private var gravitySave : Vector3;
    private var gravityOn = true;
     
    function Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (gravityOn)
            {
                gravitySave = Physics.gravity;
                Physics.gravity = Vector3.zero;
                gravityOn = false;
            }
            else
            {
                Physics.gravity = gravitySave;
                gravityOn = true;
            }
        }
}
}
