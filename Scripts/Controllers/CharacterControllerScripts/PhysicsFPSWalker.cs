// Original url: http://wiki.unity3d.com/index.php/PhysicsFPSWalker
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/PhysicsFPSWalker.cs
// File based on original modification date of: 24 October 2012, at 21:48. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Yoggy 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - New FPS Controller.js 
4 C Sharp 

DescriptionSomeone on the forum was struggling to do a really simple thing: make a rotating bridge that an FPS Walker can stand on and be carried around in a circle. 
The FPS Walker / Character Controller that UT provides will not do this so I made a script that works by physics and therefore gets friction information from other objects. 
UsageAssign this script to a rigid body with a collider and it will be able to run, jump and ride on moving platforms etc. 
JavaScript - New FPS Controller.js#pragma strict
 
// These variables are for adjusting in the inspector how the object behaves 
var maxSpeed = 7.000;
var force = 8.000;
var jumpSpeed = 5.000;
 
// These variables are there for use by the script and don't need to be edited
private var state = 0;
private var grounded = false;
private var jumpLimit = 0;
 
// Don't let the Physics Engine rotate this physics object so it doesn't fall over when running
function Awake ()
{ 
	rigidbody.freezeRotation = true;
}
 
// This part detects whether or not the object is grounded and stores it in a variable
function OnCollisionEnter ()
{
	state ++;
	if(state > 0)
	{
		grounded = true;
	}
}
 
 
function OnCollisionExit ()
{
state --;
	if(state < 1)
	{
		grounded = false;
		state = 0;
	}
}
 
// This is called every physics frame
function FixedUpdate ()
{
 
 
	// Get the input and set variables for it
	var jump = Input.GetButtonDown ("Jump");
	var horizontal = Input.GetAxis("Horizontal"); 
	var vertical = Input.GetAxis("Vertical"); 
 
	// Set the movement input to be the force to apply to the player every frame
	horizontal *= force;
	vertical *= force;
 
	// If the object is grounded and isn't moving at the max speed or higher apply force to move it
	if(rigidbody.velocity.magnitude < maxSpeed && grounded == true)
	{
		rigidbody.AddForce (transform.rotation * Vector3.forward * vertical);
		rigidbody.AddForce (transform.rotation * Vector3.right * horizontal);
	}
 
	// This part is for jumping. I only let jump force be applied every 10 physics frames so
	// the player can't somehow get a huge velocity due to multiple jumps in a very short time
	if(jumpLimit < 10) jumpLimit ++;
 
	if(jump && grounded == true && jumpLimit >= 10)
	{
		rigidbody.velocity.y += jumpSpeed;
		jumpLimit = 0;
	}
 }C Sharp Slightly modified version of the above by User:Opless 


using UnityEngine;
using System.Collections;
 
public class PhysicsFPSWalker : MonoBehaviour {
 
 
 
// These variables are for adjusting in the inspector how the object behaves 
public float maxSpeed  = 7;
public float force     = 8;
public float jumpSpeed = 5;
 
// These variables are there for use by the script and don't need to be edited
private int state = 0;
private bool grounded = false;
private float jumpLimit = 0;
 
// Don't let the Physics Engine rotate this physics object so it doesn't fall over when running
void Awake ()
{ 
	rigidbody.freezeRotation = true;
}
 
// This part detects whether or not the object is grounded and stores it in a variable
void OnCollisionEnter ()
{
	state ++;
	if(state > 0)
	{
		grounded = true;
	}
}
 
 
void OnCollisionExit ()
{
	state --;
	if(state < 1)
	{
		grounded = false;
		state = 0;
	}
}
 
 
public virtual bool jump
{
	get 
	{
		return Input.GetButtonDown ("Jump");
	}
}
 
public virtual float horizontal
{
	get
	{
		return Input.GetAxis("Horizontal") * force;
	} 
} 
public virtual float vertical
{
	get
	{
		return Input.GetAxis("Vertical") * force;
	} 
}
// This is called every physics frame
void FixedUpdate ()
{
 
 
 
	// If the object is grounded and isn't moving at the max speed or higher apply force to move it
	if(rigidbody.velocity.magnitude < maxSpeed && grounded == true)
	{
		rigidbody.AddForce (transform.rotation * Vector3.forward * vertical);
		rigidbody.AddForce (transform.rotation * Vector3.right * horizontal);
	}
 
	// This part is for jumping. I only let jump force be applied every 10 physics frames so
	// the player can't somehow get a huge velocity due to multiple jumps in a very short time
	if(jumpLimit < 10) jumpLimit ++;
 
	if(jump && grounded  && jumpLimit >= 10)
	{
		rigidbody.velocity = rigidbody.velocity + (Vector3.up * jumpSpeed);
		jumpLimit = 0;
	}
 }
 
 
}
}
