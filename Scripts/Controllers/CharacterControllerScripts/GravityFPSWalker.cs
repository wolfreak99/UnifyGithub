// Original url: http://wiki.unity3d.com/index.php/GravityFPSWalker
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/GravityFPSWalker.cs
// File based on original modification date of: 3 November 2013, at 13:59. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Donitz (Original FPSWalker by Yoggy and ?) 
DescriptionThe GravityFPSWalker component is based on the RigidbodyFPSWalker and PhysicsFPSWalker components. It adds a Gravity vector which allows the FPSWalker to spin around freely while the player keeps control of local horizontal movement. Ground detection is done using raycasting. 
With this controller the Camera should rotate in its own local space and convey its transform to the FPSWalker via the LookTransform variable. 
UsageAttach the GravityFPSWalker component to an object with a RigidBody. Add a Camera as a child GameObject and attach it to the LookTransform variable as a reference for the FPSWalker movement vectors. 
GravityFPSWalker.csusing UnityEngine;
 
/* -------------------------------------------------------------------------------
	GravityFPSWalker
 
	This component is added to a GameObject with a RigidBody. It allows the player
	to move the RigidBody using the vertical and horizontal inputs, and to jump
	using the jump button.
 
	The RigidBody is pushed towards its own custom Gravity vector. The body will
	rotate to stay upright with the RotationRate.
 
	This component uses a raycast to determine if the RigidBody is standing on 
	the ground. The GroundHeight variable should be the distance between the
	GameObject transform and a little further than the bottom of the RigidBody.
 
	The LookTransform should be a child GameObject which points in the direction
	that the player is looking at. This could for example be a child GameObject 
	with a camera. The LookTransform is used to determine the movement veectors.
 ------------------------------------------------------------------------------ */
[RequireComponent(typeof(Rigidbody))]
public class GravityFPSWalker : MonoBehaviour {
 
	public Transform LookTransform;
	public Vector3 Gravity = Vector3.down * 9.81f;
	public float RotationRate = 0.1f;
	public float Velocity = 8;
	public float GroundControl = 1.0f;
	public float AirControl = 0.2f;
	public float JumpVelocity = 5;
	public float GroundHeight = 1.1f;
	private bool jump;
 
	void Start() { 
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
	}
 
	void Update() {
		jump = jump || Input.GetButtonDown("Jump");
	}
 
	void FixedUpdate() {
		// Cast a ray towards the ground to see if the Walker is grounded
		bool grounded = Physics.Raycast(transform.position, Gravity.normalized, GroundHeight);
 
		// Rotate the body to stay upright
		Vector3 gravityForward = Vector3.Cross(Gravity, transform.right);
		Quaternion targetRotation = Quaternion.LookRotation(gravityForward, -Gravity);
		rigidbody.rotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, RotationRate);
 
		// Add velocity change for movement on the local horizontal plane
		Vector3 forward = Vector3.Cross(transform.up, -LookTransform.right).normalized;
		Vector3 right = Vector3.Cross(transform.up, LookTransform.forward).normalized;
		Vector3 targetVelocity = (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")) * Velocity;
		Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		Vector3 velocityChange = transform.InverseTransformDirection(targetVelocity) - localVelocity;
 
		// The velocity change is clamped to the control velocity
		// The vertical component is either removed or set to result in the absolute jump velocity
		velocityChange = Vector3.ClampMagnitude(velocityChange, grounded ? GroundControl : AirControl);
		velocityChange.y = jump && grounded ? -localVelocity.y + JumpVelocity : 0;
		velocityChange = transform.TransformDirection(velocityChange);
		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
 
		// Add gravity
		rigidbody.AddForce(Gravity * rigidbody.mass);
 
		jump = false;
	}
 
}
}
