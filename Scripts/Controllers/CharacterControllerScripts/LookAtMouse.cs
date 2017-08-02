/*************************
 * Original url: http://wiki.unity3d.com/index.php/LookAtMouse
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/LookAtMouse.cs
 * File based on original modification date of: 20 July 2014, at 11:17. 
 *
 * Author: capnbishop 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 JavaScript - LookAtMouse.js 
    4 C# - LookAtMouse.cs 
    
    DescriptionThis script will cause an object to rotate towards the mouse cursor along it's y axis. 
    This action could be good for third person games where objects need to point to the cursor but remain parallel with the ground. For instance, a sentry gun located in the center of the screen that rotates toward the mouse to target oncoming enemies. 
    UsageDrop this script onto a GameObject to have it look toward the cursor. Change the speed value to alter how quickly the object rotates. 
    JavaScript - LookAtMouse.js// LookAtMouse will cause an object to rotate toward the cursor, along the y axis.
    //
    // To use, drop on an object that should always look toward the mouse cursor.
    // Change the speed value to alter how quickly the object rotates toward the mouse.
     
    // speed is the rate at which the object will rotate
    var speed = 4.0;
     
    function Update () {
        // Generate a plane that intersects the transform's position with an upwards normal.
        var playerPlane = new Plane(Vector3.up, transform.position);
     
        // Generate a ray from the cursor position
        var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
     
        // Determine the point where the cursor ray intersects the plane.
        // This will be the point that the object must look towards to be looking at the mouse.
        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        //   then find the point along that ray that meets that distance.  This will be the point
        //   to look at.
        var hitdist = 0.0;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast (ray, hitdist)) {
            // Get the point along the ray that hits the calculated distance.
            var targetPoint = ray.GetPoint(hitdist);
     
            // Determine the target rotation.  This is the rotation if the transform looks at the target point.
            var targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
     
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    }C# - LookAtMouse.cs This is a direct copy of the Javascript function implemented in c# 
    using UnityEngine;
    using System.Collections;
     
    public class LookAtMouse : MonoBehaviour
    {
     
    	// speed is the rate at which the object will rotate
    	public float speed;
     
    	void FixedUpdate () 
    	{
        	// Generate a plane that intersects the transform's position with an upwards normal.
        	Plane playerPlane = new Plane(Vector3.up, transform.position);
     
        	// Generate a ray from the cursor position
        	Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
     
        	// Determine the point where the cursor ray intersects the plane.
        	// This will be the point that the object must look towards to be looking at the mouse.
        	// Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        	//   then find the point along that ray that meets that distance.  This will be the point
        	//   to look at.
        	float hitdist = 0.0f;
        	// If the ray is parallel to the plane, Raycast will return false.
        	if (playerPlane.Raycast (ray, out hitdist)) 
    		{
            	// Get the point along the ray that hits the calculated distance.
            	Vector3 targetPoint = ray.GetPoint(hitdist);
     
            	// Determine the target rotation.  This is the rotation if the transform looks at the target point.
            	Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
     
            	// Smoothly rotate towards the target point.
            	transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    		}
        }
}
}
