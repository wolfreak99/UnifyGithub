// Original url: http://wiki.unity3d.com/index.php/SimpleTankController
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/SimpleTankController.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Description A non-physics simple tank movement controller that includes acceleration and optional auto-parenting of the object directly below the controlled object. 
Usage Place this script onto a gameobject. If it currently does not have a CharacterController one will be added. Horizontal/Vertical inputs are used for movement. Tweak various public variables to obtain desired behaviour. Turning on the Parent To Ground option will enable the controlled object to step on a moving object and begin moving with that object until the controlled object is moved off the moving object. NOTE: It is important that you remove any colliders from the gameobject being controlled. 
JavaScript - SimpleTankController.js enum direction {forward, reverse, stop};
 
public var parentToGround : boolean = false;				// automatically updates the parent to the object below to allow for moving objects to affect object
public var groundRayOffset : Vector3 = Vector3.zero;		// adjust where the parentToGround ray is cast from
public var groundRayCastLength : float = 1.0;				// how far does the parentToGround ray cast
 
public var topSpeedForward : float = 3.0; 					// top speed of forward
public var topSpeedReverse : float = 1.0; 					// top speed of reverse
public var accelerationRate : float = 3; 					// rate at which top speed is reached
public var decelerationRate : float = 2; 					// rate at which speed is lost when not accelerating
public var brakingDecelerationRate : float = 4;				// rate at which speed is lost when braking (input opposite of current direction)
public var stoppedTurnRate : float = 2.0;					// rate at which object turns when stopped
public var topSpeedForwardTurnRate : float = 1.0;			// rate at which object turns at top forward speed
public var topSpeedReverseTurnRate : float = 2.0;			// rate at which object turns at top reverse speed
public var gravity = 10.0;									// gravity for object
public var stickyThrottle : boolean = false; 				// true to disable loss of speed if no input is provided
public var stickyThrottleDelay : float = .35;				// delay between change of direction when sticky throttle is enabled
 
private var currentSpeed : float = 0.0; 					// stores current speed
private var currentTopSpeed : float = topSpeedForward; 		// stores top speed of current direction
private var currentDirection : direction = direction.stop; 	// stores current direction
private var isBraking : boolean = false; 					// true if input is braking
private var isAccelerating : boolean = false; 				// true if input is accelerating
private var stickyDelayCount : float = 9999.0;				// current sticky delay count
private var characterController : CharacterController;
 
function Start() {
    characterController = GetComponent(CharacterController);
}
 
function FixedUpdate() {
 
	// direction to move this update
	var moveDirection : Vector3 = Vector3.zero;
	// direction requested this update
	var requestedDirection : direction = direction.stop;
 
    if(characterController.isGrounded == true) {
		// simulate loss of turn rate at speed
		var currentTurnRate = Mathf.Lerp((currentDirection == direction.forward ? topSpeedForwardTurnRate : topSpeedReverseTurnRate), stoppedTurnRate, (1- (currentSpeed/currentTopSpeed)));		
		transform.eulerAngles.y += Input.GetAxis("Horizontal") * currentTurnRate;
 
		// based on input, determine requested action
		if (Input.GetAxis("Vertical") > 0) { // requesting forward
			requestedDirection = direction.forward;
			isAccelerating = true;
		} else if (Input.GetAxis("Vertical") < 0) { // requesting reverse
			requestedDirection = direction.reverse;
			isAccelerating = true;
		} else {
			requestedDirection = currentDirection;
			isAccelerating = false;
		}
 
		isBraking = false;
 
		if (currentDirection == direction.stop) { // engage new direction
			stickyDelayCount += Time.deltaTime;
			// if we are not sticky throttle or if we have hit the delay then change direction
			if (!stickyThrottle || stickyDelayCount > stickyThrottleDelay) {
				// make sure we can go in the requsted direction
				if (requestedDirection == direction.reverse && topSpeedReverse > 0 || 
					requestedDirection == direction.forward && topSpeedForward > 0) {
 
					currentDirection = requestedDirection;
				}
			}
		} else if (currentDirection != requestedDirection) { // requesting a change of direction, but not stopped so we are braking
			isBraking = true;
			isAccelerating = false;
		}
 
		// setup top speeds and move direction
		if (currentDirection == direction.forward) {
			moveDirection = Vector3.forward;
			currentTopSpeed = topSpeedForward;
		} else if (currentDirection == direction.reverse) {
			moveDirection = (-1 * Vector3.forward);
			currentTopSpeed = topSpeedReverse;
		} else if (currentDirection == direction.stop) {
			moveDirection = Vector3.zero;
		}
 
		if (isAccelerating) {
			//if we havent hit top speed yet, accelerate
		   if (currentSpeed < currentTopSpeed){ 
				currentSpeed += (accelerationRate * Time.deltaTime);     
		   }
		} else {
			// if we are not accelerating and still have some speed, decelerate
			if (currentSpeed > 0) {
				// adjust deceleration for braking and implement sticky throttle
				var currentDecelerationRate : float = (isBraking ? brakingDecelerationRate : (!stickyThrottle ? decelerationRate : 0));
				currentSpeed -= (currentDecelerationRate * Time.deltaTime);  
			}
		}
 
		// if our speed is below zero, stop and initialize
		if (currentSpeed < 0 || (currentSpeed == 0 && currentDirection != direction.stop)) {
			SetStopped();
		} else if (currentSpeed > currentTopSpeed) { // limit the speed to the current top speed
			currentSpeed = currentTopSpeed;
		}
 
        moveDirection = transform.TransformDirection(moveDirection);
	}
 
	// implement gravity so we can stay grounded
    moveDirection.y -= gravity * Time.deltaTime;
	moveDirection.z = moveDirection.z * (Time.deltaTime * currentSpeed);
	moveDirection.x = moveDirection.x * (Time.deltaTime * currentSpeed);
    characterController.Move(moveDirection);
 
	if (parentToGround) {
		var hit : RaycastHit;
		var down = transform.TransformDirection (-1 * Vector3.up);	
 
		// cast the bumper ray out from bottom and check to see if there is anything below
		if (Physics.Raycast (transform.TransformPoint(groundRayOffset), down, hit, groundRayCastLength)) {
			// clamp wanted position to hit position
			transform.parent = hit.transform;
		}
 
		// if we are currently stopped, move just a tad to allow for collisions while parent is moving
		if (currentDirection == direction.stop) {
			characterController.SimpleMove(transform.TransformDirection(Vector3.forward) * 0.000000000001);		
		}
	}
 
}
 
function GetCurrentSpeed() {
	return currentSpeed;
}
 
function GetCurrentTopSpeed() {
	return currentTopSpeed;
}
 
function GetCurrentDirection() {
	return currentDirection;
}
 
function GetIsBraking() {
	return isBraking;
}
 
function GetIsAccelerating() {
	return isAccelerating;
}
 
function SetStopped() {
	currentSpeed = 0;
	currentDirection = direction.stop;
	isAccelerating = false;
	isBraking = false;
	stickyDelayCount = 0;
}
 
@script RequireComponent(CharacterController)
}
