// Original url: http://wiki.unity3d.com/index.php/Simple_planetary_orbits
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/Simple_planetary_orbits.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.SimulationScripts
{
These scripts are designed for quickly setting up basic rigidbody-driven 2D orbital physics. 
Included are two Monobehaviours (Orbiter and OrbitRenderer) and two supporting classes (OrbitalEllipse and OrbitState). Attach Orbiter.js to the object that you would like to put into orbit. Optionally add the OrbitRenderer component for visual feedback while adjusting Orbiter properties. All objects must lie on the same x/y plane. 


Contents [hide] 
1 Orbiter.js 
2 OrbitRenderer.js 
3 OrbitalEllipse.js 
4 OrbitState.js 

Orbiter.js #pragma strict
@script RequireComponent(Rigidbody)
 
//==============================//
//===        Orbiter         ===//
//==============================//
 
/*
  Required component. Add Orbiter.js to the object that you would like to put into orbit.
 
  Dependencies:
    OrbitalEllipse.js - calculates the shape, orientation, and offset of an orbit
    OrbitState.js - calculates the initial state of the orbiter
*/
 
var orbitAround : Transform;
var orbitSpeed : float = 10.0; // In the original orbital equations this is gravity, not speed
var apsisDistance : float; // By default, this is the periapsis (closest point in its orbit)
var startingAngle : float = 0; // 0 = starting apsis, 90 = minor axis, 180 = ending apsis
var circularOrbit : boolean = false;
var counterclockwise : boolean = false;
 
private var gravityConstant : float = 100;
private var rb : Rigidbody;
private var trans : Transform;
private var ellipse : OrbitalEllipse;
private var orbitState : OrbitState;
 
// Accessor
function Ellipse () : OrbitalEllipse {
	return ellipse;
}
 
function Transform() : Transform {
	return trans;
}
function GravityConstant () : float {
	return gravityConstant;
}
 
 
// Setup the orbit when the is added
function Reset () {
	if (!orbitAround)
		return;
	ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
	apsisDistance = ellipse.endingApsis; // Default to a circular orbit by setting both apses to the same value
}
function OnApplicationQuit () {
	ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
}
 
function OnDrawGizmosSelected () {
	if (!orbitAround)
		return;
	// This is required for the OrbitRenderer. For some reason the ellipse var is always null
	// if it's set anywhere else, even including OnApplicationQuit;
	if (!ellipse)
		ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
	// Never allow 0 apsis. Start with a circular orbit.
	if (apsisDistance == 0) {
		apsisDistance = ellipse.startingApsis;
	}
}
 
 
function Start () {
	// Cache transform
	trans = transform;	
	// Cache & set up rigidbody
	rb = rigidbody;
	rb.drag = 0;
	rb.useGravity = false;
	rb.isKinematic = false;
 
	// Bail out if we don't have an object to orbit around
	if (!orbitAround) {
		Debug.LogWarning("Satellite has no object to orbit around");
		return;
	}
 
	// Update the ellipse with initial value
	if (!ellipse)
		Reset();
	ellipse.Update(orbitAround.position, transform.position, apsisDistance, circularOrbit);
 
	// Calculate starting orbit state
	orbitState = new OrbitState(startingAngle, this, ellipse);
 
	// Position the orbiter
	trans.position = ellipse.GetPosition(startingAngle, orbitAround.position);
 
	// Add starting velocity
	rb.AddForce(orbitState.velocity, ForceMode.VelocityChange);
	StartCoroutine("Orbit");
}
 
// Coroutine to apply gravitational forces on each fixed update to keep the object in orbit
function Orbit () {
	while (true) {
		// Debug.DrawLine(orbitState.position - orbitState.tangent*4, orbitState.position + orbitState.tangent*4);
		var difference = trans.position - orbitAround.position;
		rb.AddForce(-difference.normalized * orbitSpeed * gravityConstant * Time.fixedDeltaTime / difference.sqrMagnitude, ForceMode.VelocityChange);
		yield WaitForFixedUpdate();
	}
}

OrbitRenderer.js #pragma strict
@script RequireComponent(Orbiter)
 
//===============================//
//===     Orbit Renderer      ===//
//===============================//
 
/*
  Optional component. Display the Orbiter component's properties in the editor. Does nothing in-game.
*/
 
var orbitPointsColor : Color = Color(1,1,0,0.5); // Yellow
var orbitPointsSize : float = 0.5;
var ellipseResolution : float = 24;
//var renderAsLines : boolean = false;
 
var startPointColor : Color = Color(1,0,0,0.7); // Red
var startPointSize : float = 1.0;
 
private var orbiter : Orbiter;
private var ellipse : OrbitalEllipse;
 
function Awake () {
	// Remove the component in the compiled game. Likely not a noticeable optimization, just an experiment.
	if (!Application.isEditor)
		Destroy(this);
}
 
function Reset () {
	orbiter = GetComponent(Orbiter);
}
function OnApplicationQuit () {
	orbiter = GetComponent(Orbiter);
}
 
 
function OnDrawGizmosSelected () {
	if (!orbiter)
		orbiter = GetComponent(Orbiter);
 
	// Bail out if there is no object to orbit around
	if (!orbiter.orbitAround)
		return;
 
	// Recalculate the ellipse only when in the editor
	if (!Application.isPlaying) {
		if (!orbiter.Ellipse())
			return;
		orbiter.Ellipse().Update(orbiter.orbitAround.position, transform.position, orbiter.apsisDistance, orbiter.circularOrbit);
	}
 
	DrawEllipse();
	DrawStartingPosition();
}
 
function DrawEllipse () {
	for (var angle = 0; angle < 360; angle += 360 / ellipseResolution) {
		Gizmos.color = orbitPointsColor;
		Gizmos.DrawSphere(orbiter.Ellipse().GetPosition(angle, orbiter.orbitAround.position), orbitPointsSize);
	}
}
 
function DrawStartingPosition () {	
	Gizmos.color = startPointColor;
	Gizmos.DrawSphere(orbiter.Ellipse().GetPosition(orbiter.startingAngle, orbiter.orbitAround.position), startPointSize);
}

OrbitalEllipse.js #pragma strict
 
//===================================//
//===  Elliptical orbit datatype  ===//
//===================================//
 
/*
  Calculates an ellipse to use as an orbital path
*/
 
class OrbitalEllipse extends Object {
 
	// "Starting" apsis is the position of the transform.position of the orbiter.
	// "Ending" apsis is the distance that we've defined in the inspector.
	// Each apsis defines the distance from the object we're orbiting to the orbiter
	var startingApsis : float;
	var endingApsis : float;
 
	var semiMajorAxis : float;
	var semiMinorAxis : float;
	var focalDistance : float;
	var difference : Vector3; // difference between the object we're orbiting and the orbiter
 
 
	//==== Instance Methods ====//
 
	// Constructor
	function OrbitalEllipse (orbitAroundPos : Vector3, orbiterPos : Vector3, endingApsis : float, circular : boolean) {
		Update(orbitAroundPos, orbiterPos, endingApsis, circular);
	}
 
	// Update ellipse when orbiter properties change
	function Update (orbitAroundPos : Vector3, orbiterPos : Vector3, endingApsis : float, circular : boolean) {
		this.difference = orbiterPos - orbitAroundPos;
		this.startingApsis = difference.magnitude;
		if (endingApsis == 0 || circular)
			this.endingApsis = this.startingApsis;
		else
			this.endingApsis = endingApsis;
		this.semiMajorAxis = CalcSemiMajorAxis(this.startingApsis, this.endingApsis);
		this.focalDistance = CalcFocalDistance(this.semiMajorAxis, this.endingApsis);
		this.semiMinorAxis = CalcSemiMinorAxis(this.semiMajorAxis, this.focalDistance);
	}
 
	// The global position
	function GetPosition (degrees : float, orbitAroundPos : Vector3) : Vector3 {
		// Use the difference between the orbiter and the object it's orbiting around to determine the direction
		// that the ellipse is aimed
		// Angle is given in degrees
		var ellipseDirection : float = Vector3.Angle(Vector3.left, difference); // the direction the ellipse is rotated
		if (difference.y < 0) {
			ellipseDirection = 360-ellipseDirection; // Full 360 degrees, rather than doubling back after 180 degrees
		}
 
		var beta : float = ellipseDirection * Mathf.Deg2Rad;
		var sinBeta : float = Mathf.Sin(beta);
		var cosBeta : float = Mathf.Cos(beta);
 
		var alpha = degrees * Mathf.Deg2Rad;
		var sinalpha = Mathf.Sin(alpha);
		var cosalpha = Mathf.Cos(alpha);
 
		// Position the ellipse relative to the "orbit around" transform
		var ellipseOffset : Vector3 = difference.normalized * (semiMajorAxis - endingApsis);
 
		var finalPosition : Vector3 = new Vector3();
		finalPosition.x = ellipseOffset.x + (semiMajorAxis * cosalpha * cosBeta - semiMinorAxis * sinalpha * sinBeta) * -1;
		finalPosition.y = ellipseOffset.y + (semiMajorAxis * cosalpha * sinBeta + semiMinorAxis * sinalpha * cosBeta);
 
		// Offset entire ellipse proportional to the position of the object we're orbiting around
		finalPosition += orbitAroundPos;
 
		return finalPosition;
	}
 
 
	//==== Private Methods ====//
 
	private function CalcSemiMajorAxis (startingApsis : float, endingApsis : float) : float {
		return (startingApsis + endingApsis) * 0.5;
	}
	private function CalcSemiMinorAxis (semiMajorAxis : float, focalDistance : float) : float {
		var distA : float = semiMajorAxis + focalDistance*0.5;
		var distB : float = semiMajorAxis - focalDistance*0.5;
		return Mathf.Sqrt( Mathf.Pow(distA+distB,2) - focalDistance*focalDistance ) * 0.5;
	}
	// private function CalcEccentricity (semiMajorAxis : float, focalDistance : float) : float {
	// 	return focalDistance / (semiMajorAxis * 2);
	// }
	private function CalcFocalDistance (semiMajorAxis : float, endingApsis : float) : float {
		return (semiMajorAxis - endingApsis) * 2;
	}			
}

OrbitState.js #pragma strict
 
//================================//
//===   Orbit State datatype   ===//
//================================//
 
/*
 The OrbitState is the initial state of the orbiter at a particular point along the ellipse
 The state contains all of the information necessary to apply a force to get the orbiter moving along the ellipse
*/
 
class OrbitState extends Object {
	var position : Vector3; // local position relative to the object we're orbiting around
	var normal : Vector3;
	var tangent : Vector3;
	var velocity : Vector3;
	private var orbiter : Orbiter;
	private var ellipse : OrbitalEllipse;	
 
	//==== Instance Methods ====//
 
	// Constructor
	function OrbitState (angle : float, orbiter : Orbiter, ellipse : OrbitalEllipse) {
		Update(angle, orbiter, ellipse);
	}
 
	// Update the state of the orbiter when its position along the ellipse changes
	// Note: Make sure the ellipse is up to date before updating the orbit state
	function Update (orbiterAngle : float, orbiter : Orbiter, ellipse : OrbitalEllipse) {
		this.orbiter = orbiter;
		this.ellipse = ellipse;
		this.normal = CalcNormal(orbiterAngle);
		this.tangent = CalcTangent(normal);
		this.position = ellipse.GetPosition(orbiterAngle, orbiter.orbitAround.position);
		this.velocity = CalcVelocity(orbiter.orbitSpeed * orbiter.GravityConstant(), position, orbiter.orbitAround.position);
	}
 
 
	//==== Private Methods ====//
 
	// Returns the normal on the ellipse at the given angle
	// Assumes a vertical semi-major axis, and a rotation of 0 at the top of the ellipse, going clockwise
	private function CalcNormal (rotationAngle : float) : Vector3 {
		// Part 1: Find the normal for the orbiter at its starting angle
		// Rotate an upward vector by the given starting angle around the ellipse. Gives us the normal for a circle.
		var localNormal : Vector3 = Quaternion.AngleAxis(rotationAngle, Vector3.forward*-1) * Vector3.up;
		// Sqash the normal into the shape of the ellipse
		localNormal.x *= ellipse.semiMajorAxis/ellipse.semiMinorAxis;
 
		// Part 2: Find the global rotation of the ellipse
		var ellipseAngle : float = Vector3.Angle(Vector3.up, ellipse.difference);
		if (ellipse.difference.x < 0)
			ellipseAngle = 360-ellipseAngle; // Full 360 degrees, rather than doubling back after 180 degrees
 
		// Part 3: Rotate our normal to match the rotation of the ellipse
		var globalNormal : Vector3 = Quaternion.AngleAxis(ellipseAngle, Vector3.forward*-1) * localNormal;
		return globalNormal.normalized;
	}
 
	private function CalcTangent (normal : Vector3) : Vector3 {
		var angle : float = 90;
		var direction : int = orbiter.counterclockwise ? -1 : 1;
		var tangent = Quaternion.AngleAxis(angle*direction, Vector3.forward*-1) * normal;
		return tangent;
	}
 
	private function CalcVelocity (gravity : float, orbiterPos : Vector3, orbitAroundPos : Vector3) : Vector3 {
		// Vis Viva equation
		var speed : float = Mathf.Sqrt( gravity * (2/Vector3.Distance(orbiterPos, orbitAroundPos) - 1/ellipse.semiMajorAxis ) );
		var velocityVec : Vector3 = tangent * speed;
		return velocityVec;
	}	
}
}
