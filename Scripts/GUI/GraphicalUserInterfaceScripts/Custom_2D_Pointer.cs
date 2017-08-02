/*************************
 * Original url: http://wiki.unity3d.com/index.php/Custom_2D_Pointer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/Custom_2D_Pointer.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    
    DescriptionThis script lets you use a custom 2D mouse cursor instead of the system mouse pointer. It also lets you have different images for when the pointer hits the edges of the screen. Typically these would be up/down/left/right arrows of some sort. 
    UsageAttach this script to a GUI Texture object, then drag your pointer images onto the appropriate slots for the script in the Inspector. 
    JavaScript - Mouse Cursor.jsvar normalCursor : Texture2D;        // The texture for when the cursor isn't near a screen edge
    var leftCursor : Texture2D;          // The texture for the cursor when it's at the left edge
    var rightCursor : Texture2D;         // Ditto, right edge
    var upCursor : Texture2D;            // Top edge
    var downCursor : Texture2D;          // ...And bottom edge
    var nativeRatio = 1.3333333333333;   // Aspect ratio of the monitor used to set up GUI elements
    private var lastPosition : Vector3;  // Where the mouse position was last
    var normalAlpha = .5;                // Normal alpha value of the cursor ... .5 is full
    var fadeTo = .2;                     // The alpha value the cursor fades to if not moved
    var fadeRate = .22;                  // The rate at which the cursor fades
    private var cursorIsFading = true;   // Whether we should fade the cursor
    private var fadeValue : float;
     
    // Scale the cursor so it should look right in any aspect ratio, and turn off the OS mouse pointer
    function Start() {
        // Slightly weird but necessary way of forcing float evaluation
        var currentRatio : float = (0.0 + Screen.width) / Screen.height;
        transform.localScale.x *= nativeRatio / currentRatio;
        Screen.showCursor = false;
        fadeValue = normalAlpha;
        lastPosition = Input.mousePosition;
    }
     
    function Update() {
        var mousePos = Input.mousePosition;
        // If the mouse has moved since the last update
        if (mousePos != lastPosition) {
        	lastPosition = mousePos;
            // Get mouse X and Y position as a percentage of screen width and height
            MoveMouse(mousePos.x/Screen.width, mousePos.y/Screen.height);
        }
        // Fade the alpha of the cursor
        if (cursorIsFading) {
            guiElement.color.a = fadeValue;
            fadeValue -= fadeRate * Time.deltaTime;
            if (fadeValue < fadeTo) {
                fadeValue = fadeTo;
                cursorIsFading = false;
            }
        }
    }
     
    function MoveMouse(mousePosX : float, mousePosY : float) {
        // Make the cursor solid, and set fading to start in case mouse movement stops
        guiElement.color.a = fadeValue = normalAlpha;
        guiElement.texture = normalCursor;
        cursorIsFading = true;
     
        // If the mouse is on a screen edge, first make sure the cursor doesn't go off the screen, then give it the appropriate cursor
        if (mousePosX < .005) {
            mousePosX = .005;
            guiElement.texture = leftCursor;
        }
        if (mousePosX > .995) {
            mousePosX = .995;
            guiElement.texture = rightCursor;
        }
        if (mousePosY < .005) {
            mousePosY = .005;
            guiElement.texture = downCursor;
        }
        if (mousePosY > .995) {
            mousePosY = .995;
            guiElement.texture = upCursor;
        }
     
        transform.position.x = mousePosX;
        transform.position.y = mousePosY;
}
}
