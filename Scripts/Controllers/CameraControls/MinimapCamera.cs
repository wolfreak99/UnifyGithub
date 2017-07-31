// Original url: http://wiki.unity3d.com/index.php/MinimapCamera
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/MinimapCamera.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Contents [hide] 
1 Description 
2 Usage 
3 Bugs 
4 JavaScript - MinimapCamera.js 

Description This script can be attached to a camera to provide some basic minimap functionality such as zooming and a full-screen toggle. It should be noted that this was developed to work in combination with a standard TPS view and AI pathfinder, so if you wish to use it in other contexts, it will require some tweaking. 
Developed by stringbot. Feel free to update/improve/fix the code and add yourself to this line. 
Usage This script should be attached to a Camera GameObject. After this, the correct values need to be set in the inspector. You'll also need to configure two inputs: a "MinimapToggle" button and a "Mouse Wheel" axis. 
Target: The object to track. 
Scroll Sensitivity: How fast to move in and out whilst zooming. 
Max Height: How high the camera can go above the target. 
Min Height: The closest that the camera can get to the target. 
Rotate With Target: Should the map rotate when the target does? 
Bugs None that I'm aware of. Feel free to correct me here! 
JavaScript - MinimapCamera.js// Target for the minimap.
var target : GameObject;
 
// Default height to view from.
private var height = 30;
 
// Is the map currently open?
static var mapOpen = false;
 
// How much should we move for each small movement on the mouse wheel?
var scrollSensitivity = 3;
 
// Maximum and minimum heights that the camera can reach.
var maxHeight = 80;
var minHeight = 5;
 
// Should the minimap rotate with the player?
var rotateWithTarget = true;
 
function Start() {
    Screen.lockCursor = true;
    height = PlayerPrefs.GetInt("MinimapCameraHeight");
}
 
// Where the action is :D
function Update () {
 
    // If the minimap button is pressed then toggle the map state.
    if(Input.GetButtonDown("MinimapToggle")) {
        toggleMinimap();
    }
 
    // Update the transformation of the camera as per the target's position.
    transform.position.x = target.transform.position.x;
    transform.position.z = target.transform.position.z;
    // For this, we add the predefined (but variable, see below) height var.
    transform.position.y = target.transform.position.y + height;        
 
    // If the minimap should rotate as the target does, the rotateWithTarget var should be true.
    // An extra catch because rotation with the fullscreen map is a bit weird.
    if(rotateWithTarget && !mapOpen) {
        transform.eulerAngles.y = target.transform.eulerAngles.y;
    }
 
    // Get the movement of the mouse wheel as an axis. 
    // Needs configuring in your project's input setup.
    var mw : float = Input.GetAxis("Mouse Wheel");
 
    // If the value is positive, add the height as defined by the sensitivity.
    // Also, save the height to player prefs in both cases with the call to saveHeight().
    if(mw > 0 && height < maxHeight) {
        height += scrollSensitivity;
        saveHeight();
    } 
    // Opposite for negative, just sub the value instead.
    else if(mw < 0 && height > minHeight) {
        height -= scrollSensitivity;
        saveHeight();
    }
 
}
 
 
// Function to open/close the minimap.
function toggleMinimap() {
    if(!mapOpen) {
        // Set the camera to use the entire screen.
        camera.rect = Rect (0,0,1,1);
        // Update the global so other scripts can know.
        mapOpen = true;
        // Unlock the cursor for proper point/click navigation.
        Screen.lockCursor = false;
        // Update the relevant PlayerPref key, could be useful for persistence.
        PlayerPrefs.SetInt("mapOpen",1);
    }
    else {
        // Set the camera to use a small portion of the screen.
        camera.rect = Rect (0.8,0.8,1,1);
        // Update the global so other scripts can know.
        mapOpen = false;
        // Lock the cursor for TPS control.
        Screen.lockCursor = true;
        // Update the relevant PlayerPref key, could be useful for persistence.
        PlayerPrefs.SetInt("mapOpen",0);
    }
 
    // Debug print the current state of the map.
    print("mapOpen = " + mapOpen);
 
}
 
// Method to store the height of the camera as a PlayerPref.
function saveHeight() {
    PlayerPrefs.SetInt("MinimapCameraHeight",height);
}
}
