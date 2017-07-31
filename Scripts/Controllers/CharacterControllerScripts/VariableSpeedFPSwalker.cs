// Original url: http://wiki.unity3d.com/index.php/VariableSpeedFPSwalker
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/VariableSpeedFPSwalker.cs
// File based on original modification date of: 9 April 2014, at 08:42. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: (Mr. Animator) 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - VariableSpeedFPSwalker.js 
4 C# - VariableSpeedFPSwalker.cs 

Description1: Script allows you to have different speeds for walking forward, strafing, and backpeddling (I hate shooters that let you RUN backwards) 
2: Because it uses rigidbody.velocity instead of rigidbody.AddRelativeForce, it's more directly tied to the settings you choose for the Horizontal/Vertical axes, and your speed plateaus nice and dependably. I found the default FPS walker to be a little too loosey goosey 
3: To keep you from magically gaining extra speed for walking forward and strafing at the same time (a little side-effect from my method of doing things) there's a semi-sloppy math solution in there that "rounds off" the corners. So if you're moving on a 45 degree angle, it properly adjusts your forward and lateral movement so that they don't add up to more than you would get for just walking forward. 
UsageAssign this script to any RigidBody you want to be user controlled. 
JavaScript - VariableSpeedFPSwalker.js var forwardSpeed = 10; 
 var backwardSpeed = 5; 
 var strafeSpeed = 8; 
 
 function FixedUpdate () { 
    // Step1: Get your input values 
    var horizontal = Input.GetAxis("Horizontal"); 
    var vertical = Input.GetAxis("Vertical"); 
    // Step 2: Limit the movement on angles and then multiply those results 
    // by their appropriate speed variables 
    var percentofpercent = Mathf.Abs(horizontal) + Mathf.Abs(vertical) - 1.0; 
    if (percentofpercent > 0.1) { 
       // if we're here, then we're not moving in a straight line 
       // my math here might be kinda confusing and sloppy...so don't look! 
       percentofpercent = percentofpercent * 10000; 
       percentofpercent = Mathf.Sqrt(percentofpercent); 
       percentofpercent = percentofpercent / 100; 
       var finalMultiplier = percentofpercent * .25; 
       horizontal = horizontal - (horizontal * finalMultiplier); 
       vertical = vertical - (vertical * finalMultiplier); 
    } 
    if (vertical > 0) { 
       vertical = vertical * forwardSpeed; 
    } 
    if (vertical < 0) { 
       vertical = vertical * backwardSpeed; 
    } 
    horizontal = horizontal * strafeSpeed; 
    // Step 3: Derive a vector on which to travel, based on the combined 
    // influence of BOTH axes 
    var tubeFinalVector = transform.TransformDirection (Vector3(horizontal,0,vertical)); 
    // Step 4: Apply the final movement in world space 
    rigidbody.velocity.z = tubeFinalVector.z; 
    rigidbody.velocity.x = tubeFinalVector.x; 
 } 
 function Awake () { 
    rigidbody.freezeRotation = true; 
 }C# - VariableSpeedFPSwalker.csusing UnityEngine;
 
class VariableSpeedFPSWalker : MonoBehaviour
{
	[SerializeField]
	private float forwardSpeed = 10;
	[SerializeField]
	private float backwardSpeed = 5;
	[SerializeField]
	private float strafeSpeed = 8;
 
	void Awake()
	{
		rigidbody.freezeRotation = true;
	}
 
	void FixedUpdate()
	{
		// Step1: Get your input values
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		// Step 2: Limit the movement on angles and then multiply those results
		// by their appropriate speed variables
		float percentofpercent = Mathf.Abs(horizontal) + Mathf.Abs(vertical) - 1;
		if (percentofpercent > 0.1f)
		{
			// if we're here, then we're not moving in a straight line
			// my math here might be kinda confusing and sloppy...so don't look!
			percentofpercent = percentofpercent * 10000;
			percentofpercent = Mathf.Sqrt(percentofpercent);
			percentofpercent = percentofpercent / 100;
			float finalMultiplier = percentofpercent * .25f;
			horizontal = horizontal - (horizontal * finalMultiplier);
			vertical = vertical - (vertical * finalMultiplier);
		}
 
		if (vertical > 0)
			vertical = vertical * forwardSpeed;
		else if (vertical < 0)
			vertical = vertical * backwardSpeed;
 
		horizontal = horizontal * strafeSpeed;
 
		// Step 3: Derive a vector on which to travel, based on the combined
		// influence of BOTH axes (ignoring any y movement)
		Vector3 tubeFinalVector = transform.TransformDirection(new Vector3(horizontal, rigidbody.velocity.y, vertical));
 
		// Step 4: Apply the final movement in world space
		rigidbody.velocity = tubeFinalVector;
	}
}
}
