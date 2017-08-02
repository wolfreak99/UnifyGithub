/*************************
 * Original url: http://wiki.unity3d.com/index.php/CameraRelativeScale
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/CameraRelativeScale.cs
 * File based on original modification date of: 19 October 2012, at 13:43. 
 *
 * Author: Hayden Scott-Baron (Dock) 
 *
 * Description 
 *   
 * Usage 
 *   
 * Technical Discussion 
 *   
 * C# - ScaleRelativeToCamera.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    DescriptionThis script scales an object relative to a camera's distance. This gives the appearance of the object size being the same. Useful for GUI objects that appear within the game scene. Often useful when combined with CameraFacingBillboard. 
    UsagePlace this script on the gameobject you wish to keep a constant size. 
    Technical DiscussionThis measures the distance from the Camera plane, rather than the camera itself, and uses the initial scale as a basis. Use the public objectScale variable to adjust the object size. 
    C# - ScaleRelativeToCamera.cs/// ScaleRelativeToCamera.cs
    /// Hayden Scott-Baron (Dock) - http://starfruitgames.com
    /// 19 Oct 2012
    /// 
    /// Scales object relative to camera. 
    /// Useful for GUI and items that appear in the world space. 
     
    using UnityEngine;
    using System.Collections;
     
    public class ScaleRelativeToCamera : MonoBehaviour 
    {
    	public Camera cam; 
    	public float objectScale = 1.0f; 
    	private Vector3 initialScale; 
     
    	// set the initial scale, and setup reference camera
    	void Start ()
    	{
    		// record initial scale, use this as a basis
    		initialScale = transform.localScale; 
     
    		// if no specific camera, grab the default camera
    		if (cam == null)
    			cam = Camera.main; 
    	}
     
    	// scale object relative to distance from camera plane
    	void Update () 
    	{
    		Plane plane = new Plane(cam.transform.forward, cam.transform.position); 
    		float dist = plane.GetDistanceToPoint(transform.position); 
    		transform.localScale = initialScale * dist * objectScale; 
    	}
}
}
