// Original url: http://wiki.unity3d.com/index.php/RigidbodyFPSWalker
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/RigidbodyFPSWalker.cs
// File based on original modification date of: 7 May 2012, at 10:25. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - RigidbodyFPSController.js 
4 C# - RigidbodyFPSController.cs 

DescriptionThis is a rigidbody based first person controller. Traditionally first person controllers are done using the character controller and this is the recommended way, but sometimes you want it to use real physics. So forces automatically affect the rigidbody, and joints can be used to eg. make a rope swinging game. 
The script works by adding a force in the direction of desired movement, it subtracts the current velocity from it, thus when letting go of all keys the character will stop. A maximum velocity change can be specified which will make the character come to reset slower faster and in effect apply more or less force to object the character runs into. 
UsageYou can download a sample project with the complete setup here: 
RigidbodyFPS.zip 
JavaScript - RigidbodyFPSController.jsvar speed = 10.0;
var gravity = 10.0;
var maxVelocityChange = 10.0;
var canJump = true;
var jumpHeight = 2.0;
private var grounded = false;
 
@script RequireComponent(Rigidbody, CapsuleCollider)
 
function Awake ()
{
	rigidbody.freezeRotation = true;
	rigidbody.useGravity = false;
}
 
function FixedUpdate ()
{
	if (grounded)
	{
		// Calculate how fast we should be moving
		var targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		targetVelocity = transform.TransformDirection(targetVelocity);
		targetVelocity *= speed;
 
		// Apply a force that attempts to reach our target velocity
		var velocity = rigidbody.velocity;
		var velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;
		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
 
		// Jump
		if (canJump && Input.GetButton("Jump"))
		{
			rigidbody.velocity = Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
		}
	}
 
	// We apply gravity manually for more tuning control
	rigidbody.AddForce(Vector3 (0, -gravity * rigidbody.mass, 0));
 
	grounded = false;
}
 
function OnCollisionStay ()
{
	grounded = true;	
}
 
function CalculateJumpVerticalSpeed ()
{
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt(2 * jumpHeight * gravity);
}



C# - RigidbodyFPSController.csusing UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class CharacterControls : MonoBehaviour {
 
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	private bool grounded = false;
 
 
 
	void Awake () {
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
	}
 
	void FixedUpdate () {
	    if (grounded) {
	        // Calculate how fast we should be moving
	        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	        targetVelocity = transform.TransformDirection(targetVelocity);
	        targetVelocity *= speed;
 
	        // Apply a force that attempts to reach our target velocity
	        Vector3 velocity = rigidbody.velocity;
	        Vector3 velocityChange = (targetVelocity - velocity);
	        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
	        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
	        velocityChange.y = 0;
	        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
 
	        // Jump
	        if (canJump && Input.GetButton("Jump")) {
	            rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
	        }
	    }
 
	    // We apply gravity manually for more tuning control
	    rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
 
	    grounded = false;
	}
 
	void OnCollisionStay () {
	    grounded = true;    
	}
 
	float CalculateJumpVerticalSpeed () {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}
}
