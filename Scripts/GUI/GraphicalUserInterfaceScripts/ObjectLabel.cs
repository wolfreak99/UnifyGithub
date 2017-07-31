// Original url: http://wiki.unity3d.com/index.php/ObjectLabel
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/ObjectLabel.cs
// File based on original modification date of: 23 November 2014, at 15:23. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{

Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - ObjectLabel.js 
4 Description 
5 C# - ObjectLabel.cs 

DescriptionMakes a GUIText label follow an object in 3D space. Useful for things like having name tags over players' heads. 
UsageAttach this script to a GUIText object, and drag the object it should follow into the Target slot. For best results, the anchor of the GUIText should probably be set to lower center, depending on what you're doing. Offset is used to position the label somewhere relative to the actual target's position. The default of (0, 1, 0) is useful for having the label appear above the object, rather than appearing right on top of it. If ClampToScreen is on, the label will never disappear even if the target is off the screen, but will attempt to follow as best it can (for example, if the target is off to the left of the camera out of sight, the label will still be visible on the left). ClampBorderSize sets how much space will be left at the borders if the label is being clamped to the screen, to help ensure that the label is still readable and not partially cut off. This is in viewport space, so the default .05 is 5% of the screen's size. If UseMainCamera is checked, the first camera in the scene tagged MainCamera will be used. If it's not checked, you should drag the desired camera onto the CameraToUse slot, which is otherwise unused if UseMainCamera is true. 
Note: This script also works with GUITextures. 
JavaScript - ObjectLabel.jsvar target : Transform;		// Object that this label should follow
var offset = Vector3.up;	// Units in world space to offset; 1 unit above object by default
var clampToScreen = false;	// If true, label will be visible even if object is off screen
var clampBorderSize = .05;	// How much viewport space to leave at the borders when a label is being clamped
var useMainCamera = true;	// Use the camera tagged MainCamera
var cameraToUse : Camera;	// Only use this if useMainCamera is false
private var cam : Camera;
private var thisTransform : Transform;
private var camTransform : Transform;
 
function Start () {
	thisTransform = transform;
	if (useMainCamera)
		cam = Camera.main;
	else
		cam = cameraToUse;
	camTransform = cam.transform;
}
 
function Update () {
	if (clampToScreen) {
		var relativePosition = camTransform.InverseTransformPoint(target.position + offset);
		relativePosition.z = Mathf.Max(relativePosition.z, 1.0);
		thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition));
		thisTransform.position = Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1.0-clampBorderSize),
										 Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1.0-clampBorderSize),
										 thisTransform.position.z);
	}
	else {
		thisTransform.position = cam.WorldToViewportPoint(target.position + offset);
	}
}
 
@script RequireComponent(GUIText)DescriptionConverted to C# by Roidz (Ward Dewaele). Use in the same way as the javascript. 
C# - ObjectLabel.csusing UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (GUIText))]
public class ObjectLabel : MonoBehaviour {
 
public Transform target;  // Object that this label should follow
public Vector3 offset = Vector3.up;    // Units in world space to offset; 1 unit above object by default
public bool clampToScreen = false;  // If true, label will be visible even if object is off screen
public float clampBorderSize = 0.05f;  // How much viewport space to leave at the borders when a label is being clamped
public bool useMainCamera = true;   // Use the camera tagged MainCamera
public Camera cameraToUse ;   // Only use this if useMainCamera is false
Camera cam ;
Transform thisTransform;
Transform camTransform;
 
	void Start () 
    {
	    thisTransform = transform;
    if (useMainCamera)
        cam = Camera.main;
    else
        cam = cameraToUse;
    camTransform = cam.transform;
	}
 
 
    void Update()
    {
 
        if (clampToScreen)
        {
            Vector3 relativePosition = camTransform.InverseTransformPoint(target.position + offset);
            relativePosition.z =  Mathf.Max(relativePosition.z, 1.0f);
            thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition));
            thisTransform.position = new Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1.0f - clampBorderSize),
                                             Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1.0f - clampBorderSize),
                                             thisTransform.position.z);
 
        }
        else
        {
            thisTransform.position = cam.WorldToViewportPoint(target.position + offset);
        }
    }
}
}
