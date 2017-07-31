// Original url: http://wiki.unity3d.com/index.php/ClickCharacterController
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/ClickCharacterController.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
By: DaveA 
Contents [hide] 
1 Description 
2 Usage 
3 Code 
4 C#- ClickCharacterController.cs 

Description Character controller script to use with the Locomotion system, which will move a character to the point indicted by left mouse click. Rudimentary! Please feel free to expound and expand upon, but please post anything good. 
Usage Put this script into your project. Follow the Locomotion tutorial to add LegController, LegAnimator, etc. to your character. Use this character controller on your character instead of Platform or AimLook etc. 
Code C#- ClickCharacterController.cs using UnityEngine;
using System.Collections;
 
public class ClickCharacterController : MonoBehaviour {
 
	private CharacterMotor motor;
	private Vector3 targetPosition;
	private Vector3 directionVector;
	private Camera mainCamera;
 
	public float walkMultiplier = 1f;
	public bool defaultIsWalk = false;
	public float smooth = 0.0005F;
 
	void Start () {
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		if (motor==null) Debug.Log("Motor is null!!");
	}
 
	void Update ()
	{		
		// see if user pressed the mouse down
		if (Input.GetMouseButtonDown (0))
		{
			mainCamera = FindCamera();
 
			// We need to actually hit an object
			RaycastHit hit;
			if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),  out hit, 100))
				return;
			// We need to hit something (with a collider on it)
			if (!hit.transform)
				return;
 
			// Get input vector from kayboard or analog stick and make it length 1 at most
			targetPosition = hit.point;
			directionVector = hit.point - transform.position;
			directionVector.y = 0;
			if (directionVector.magnitude>1)
				directionVector = directionVector.normalized;
		}
 
		if (walkMultiplier!=1)
		{
			if ( (Input.GetKey("left shift") || Input.GetKey("right shift") || Input.GetButton("Sneak")) != defaultIsWalk ) {
				directionVector *= walkMultiplier;
			}
		}
 
		// Apply direction
		Vector3 diff = targetPosition - transform.position;
		motor.desiredFacingDirection = diff.normalized;
		motor.desiredMovementDirection = Vector3.forward;
		transform.position = Vector3.MoveTowards (transform.position, targetPosition, smooth);
		if (diff.magnitude < .1f)
		{
			transform.position = targetPosition;
			motor.desiredMovementDirection = Vector3.zero;
		}
	}
 
	Camera FindCamera ()
	{
		if (camera)
			return camera;
		else
			return Camera.main;
	}
 
}
}
