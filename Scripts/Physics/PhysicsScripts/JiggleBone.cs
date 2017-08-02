/*************************
 * Original url: http://wiki.unity3d.com/index.php/JiggleBone
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/PhysicsScripts/JiggleBone.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Michael Cook (Fishypants)
 *
 * Description 
 *   
 * Notes 
 *   
 * How it works 
 *   
 * C# - JiggleBone.cs 
 *   
 * End 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.PhysicsScripts
{
    Name: JiggleBone v.1.0.
    Date: 9-25-2011
    
    
    
    Description
    This script will add a secondary jiggle motion to bones. (bouncy breast dynamics, hair, etc) It uses a very crude physics model to approximate the rotation of the bones, with optional squash and stretch. I modified Pegorari's Rubber Simulation Script [1] as the basis for the dynamics. 
    Notes1) This script is meant to be used on bones at the end of a bone hierarchy. It might work on bones in the middle, it might not.
    2) Don't try to chain bones together with this script. It may work, but was not intended for that purpose and may need to be redesigned in order for that to happen.
    3) You have to set "boneAxis" to be that of the bone that you are attaching it to. By default it assumes that the bone will be oriented with its "Z" axis pointed out. Also, try to keep the boneAxis with only 1 forward axis - ex(0,0,1) (1,0,0) etc.
    4) If your bones end up "flipping" 360 degrees, you will have to adjust your "targetDistance" value to be further away from the bone center.
    
    How it works
    The implementation is very simple. Instead of trying to figure out angular velocity, and a bunch of other crazy math stuff we simply goal a dynamic point to a target point, then we orient the bone to the dynamic point. The screenshot above illustrates this. 
    So for the bone, we calculate the up axis (green). Then we calculate the forward axis (blue) with extra length based off "targetDistance". You will need to adjust the target distance if your bones keep flipping, and push it further away from the bone. Then we calculate the "target" (yellow). 
    All these values remain locked to the bone and do not change. They update their orientation based on the bone's orientation. 
    Then we calculate "dynamicPos". Using mass, acceleration, velocity, damping, we try to push this point to the target point. It's very similar to the "Lerp" command, but differs in the fact that we incorporate physics based values. So instead of just going from 1 point to another, "dynamicPos" has the ability to overshoot the target, then bounce back until finally it comes to rest. 
    If you notice that the red target is not in line with the yellow target, this is because of gravity. You can disable it if you like, but it helps to add some realism. 
    Once the "dynamicPos" has been established we simply use the "LookAt" function to orient the bone to the "dynamicPos" point. 
    Simple, eh? :) 
    C# - JiggleBone.cs//	============================================================
    //	Name:		Jiggle Bone v.1.0
    //	Author: 	Michael Cook (Fishypants)
    //	Date:		9-25-2011
    //	License:	Free to use. Any credit would be nice :)
    //
    //	To Use:
    // 		Drag this script onto a bone. (ideally bones at the end)
    //		Set the boneAxis to be the front facing axis of the bone.
    //		Done! Now you have bones with jiggle dynamics.
    //
    //	============================================================
     
    using UnityEngine;
    using System.Collections;
     
    public class jiggleBone : MonoBehaviour {
    	public bool debugMode = true;
     
    	// Target and dynamic positions
    	Vector3 targetPos = new Vector3();
    	Vector3 dynamicPos = new Vector3();
     
    	// Bone settings
    	public Vector3 boneAxis = new Vector3(0,0,1);
    	public float targetDistance = 2.0f;
     
    	// Dynamics settings
    	public float bStiffness = 0.1f;
    	public float bMass = 0.9f;
    	public float bDamping = 0.75f;
    	public float bGravity = 0.75f;
     
    	// Dynamics variables
    	Vector3 force = new Vector3();
    	Vector3 acc = new Vector3();
    	Vector3 vel = new Vector3();
     
    	// Squash and stretch variables
    	public bool SquashAndStretch = true;
    	public float sideStretch = 0.15f;
    	public float frontStretch = 0.2f;
     
    	void Awake(){
    		// Set targetPos and dynamicPos at startup
    		Vector3 targetPos = transform.position + transform.TransformDirection(new Vector3((boneAxis.x * targetDistance),(boneAxis.y * targetDistance),(boneAxis.z * targetDistance)));
    		dynamicPos = targetPos;
    	}
     
    	void LateUpdate(){
    		// Reset the bone rotation so we can recalculate the upVector and forwardVector
    		transform.rotation = new Quaternion();
     
    		// Update forwardVector and upVector
    		Vector3 forwardVector = transform.TransformDirection(new Vector3((boneAxis.x * targetDistance),(boneAxis.y * targetDistance),(boneAxis.z * targetDistance)));
    		Vector3 upVector = transform.TransformDirection(new Vector3(0,1,0));
     
    		// Calculate target position
    		Vector3 targetPos = transform.position + transform.TransformDirection(new Vector3((boneAxis.x * targetDistance),(boneAxis.y * targetDistance),(boneAxis.z * targetDistance)));
     
    		// Calculate force, acceleration, and velocity per X, Y and Z
    		force.x = (targetPos.x - dynamicPos.x) * bStiffness;
    		acc.x = force.x / bMass;
    		vel.x += acc.x * (1 - bDamping);
     
    		force.y = (targetPos.y - dynamicPos.y) * bStiffness;
    		force.y -= bGravity / 10; // Add some gravity
    		acc.y = force.y / bMass;
    		vel.y += acc.y * (1 - bDamping);
     
    		force.z = (targetPos.z - dynamicPos.z) * bStiffness;
    		acc.z = force.z / bMass;
    		vel.z += acc.z * (1 - bDamping);
     
    		// Update dynamic postion
    		dynamicPos += vel + force;
     
    		// Set bone rotation to look at dynamicPos
    		transform.LookAt(dynamicPos, upVector);
     
    		// ==================================================
    		// Squash and Stretch section
    		// ==================================================
    		if(SquashAndStretch){
    			// Create a vector from target position to dynamic position
    			// We will measure the magnitude of the vector to determine
    			// how much squash and stretch we will apply
    			Vector3 dynamicVec = dynamicPos - targetPos;
     
    			// Get the magnitude of the vector
    			float stretchMag = dynamicVec.magnitude;
     
    			// Here we determine the amount of squash and stretch based on stretchMag
    			// and the direction the Bone Axis is pointed in. Ideally there should be
    			// a vector with two values at 0 and one at 1. Like Vector3(0,0,1)
    			// for the 0 values, we assume those are the sides, and 1 is the direction
    			// the bone is facing
    			float xStretch;
    			if(boneAxis.x == 0) xStretch = 1 + (-stretchMag * sideStretch);
    			else xStretch = 1 + (stretchMag * frontStretch);
     
    			float yStretch;
    			if(boneAxis.y == 0) yStretch = 1 + (-stretchMag * sideStretch);
    			else yStretch = 1 + (stretchMag * frontStretch);
     
    			float zStretch;
    			if(boneAxis.z == 0) zStretch = 1 + (-stretchMag * sideStretch);
    			else zStretch = 1 + (stretchMag * frontStretch);
     
    			// Set the bone scale
    			transform.localScale = new Vector3(xStretch, yStretch, zStretch);
    		}
     
    		// ==================================================
    		// DEBUG VISUALIZATION
    		// ==================================================
    		// Green line is the bone's local up vector
    		// Blue line is the bone's local foward vector
    		// Yellow line is the target postion
    		// Red line is the dynamic postion
    		if(debugMode){
    			Debug.DrawRay(transform.position, forwardVector, Color.blue);
    			Debug.DrawRay(transform.position, upVector, Color.green);
    			Debug.DrawRay(targetPos, Vector3.up * 0.2f, Color.yellow);
    			Debug.DrawRay(dynamicPos, Vector3.up * 0.2f, Color.red);
    		}
    		// ==================================================
    	}
}EndCreated by Michael Cook (Fishypants). If you have any questions just ask! 
}
