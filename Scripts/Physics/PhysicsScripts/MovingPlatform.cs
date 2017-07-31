// Original url: http://wiki.unity3d.com/index.php/MovingPlatform
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/PhysicsScripts/MovingPlatform.cs
// File based on original modification date of: 30 June 2013, at 19:16. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.PhysicsScripts
{
/** MOVING PLATFORM
 *
 * This script move a gameObject like a moving platform/elevator and moves the others gameObjects while 
 * they are in contact with this gameObject.
 * 
 * To use this script there's two ways. Anyway chosen, mark the collider as Convex make sure that 
 * the "Enter" methods are called (a tip is to use a debug message for confirmation). 
 * 
 * 1. Using OnCollision methods: Add a Rigidbody (disable gravity) and don't make the Collider as trigger.
 * 2. Using OnTrigger methods: Make the Collider as trigger.
 * 
 * OnCollision methods only moves the object with script component and other objects in contact that
 * have a Collider and a Rigidbody.
 *
 * OnTrigger methods only moves the object with script component and other objects in contact that
 * have a Collider and one of the two objects (the object that has this script component or the object
 * that is colliding with the one that have the script component) have a Rigidbody or Character
 * Controller, but with the object with this script as a trigger, so the objects will go through the
 * moving platform (is ignored in physics engine). The solution is to duplicate the moving platform
 * and make one as a child, using a parent a little bigger as the trigger. Follow these steps:
 * 
 * 1. Duplicate the object making object A and object B.
 * 2. Change the A y scale a little, some big objects works with 1.1 scale, some others smaller
 * objects needs 5 scale.
 * 3. Delete the A renderer and other things, only keep the collider (that will be used as trigger)
 * and the script (that only needs to be in A).
 * 4-Add B as child of A.
 * 5-You can place the B a little lower, but make sure that B is totally inside in A. Keep in mind
 * that anything while is in contact with A will move too.
 *
 *
 * If you wish to only move the platform in one direction, wait and return to the previous direction,
 * just put only one route and mark the box 'ReverseAtEnd'. 
 * 
 * Example: A script with a route with Distance=Vector3(4,0,0), Speed=Vector3(0.5,0,0) and 
 * Wait Time=3.0 goes 0.5 distance per second in x until goes through 4 distance. After this you wait
 * for 3 seconds and go to the next route or restarts if the route is the last one. 
 *
 * If you marks 'ReverseAtEnd', after done all routes the object do the routes backward with
 * reverse (*-1) distances and restart the circle. Example: If you made three routes with distances
 * 3, -2 and 7 (disregarding the y and z values) and marks 'ReverseAtEnd' the the distance that
 * platforms do are: 3, -2, 7, -7, 2, -3, 3, -2, 7, -7, 2, -3, 3, -2, 7, -7, etc...
 *
 * @author FL
 * @version 1.0
**/
 
class Route{
	var distance : Vector3 = Vector3(0,0,0);
	var speed : Vector3 = Vector3(-1,-1,-1); // Put a negative number to use the default value.
	var waitTime : float=-1; // Put a negative number to use the default value.
}
 
var reverseAtEnd : boolean = true; // Reverse the route when it ends, example TODO
var speedDefault : Vector3 = Vector3(0,0,0); // Must be positive
var waitTimeDefault : float =  1.0f;
var route : Route[] = new Route[1];
 
private var distanceNow : Vector3;
private var speedNow : Vector3;
private var waitTimeNow : float = 0f;
private var waitUntil : float = 0f;
private var index : int = -1;
private var routeIndex : int = 0;
private var way : int = 1;
private var destiny = Vector3(0,0,0);
private var objectsInPlatform : Array = new Array();
 
function Start(){
	if (route.Length==0)
		Debug.Log("MovingPlatform: ERROR! NO ROUTE DEFINED!");
	destiny = transform.position;
	objectsInPlatform.Add(gameObject);
}
 
function Update(){
	if(waitUntil>=0){ // If is waiting. In the first call, go to this part.
		if(waitUntil>Time.timeSinceLevelLoad){
			// Keep waiting
			return;
		}else{
			// Stops waiting and prepares the next index
			waitUntil=-1;
			index++;
			if(index==route.length){
				index=0;
				if(reverseAtEnd) 
					way *= -1;
			}	
			routeIndex = (way==1) ? index : (route.length - 1 - index);
			distanceNow = route[routeIndex].distance;
			speedNow = (route[routeIndex].speed==null || route[routeIndex].speed[0]<0 || route[routeIndex].speed[1]<0
				|| route[routeIndex].speed[2]<0) ? speedDefault : route[routeIndex].speed;
			for(var dv : int = 0;dv<3;dv++){ // Calculates the way of velocity with basis of distance.
				speedNow[dv] *= (distanceNow[dv]*way>0.0) ? 1 : -1;
			}
			waitTimeNow = (route[routeIndex].waitTime<0) ? waitTimeDefault : route[routeIndex].waitTime;
			destiny = distanceNow*way+transform.position;
		}	
	}
	//Calculates the distance to go.
	var distanceToGo : Vector3 = speedNow*Time.deltaTime; 
	var destinyWithoutFix : Vector3 = distanceToGo+transform.position;
	for(var d : int = 0;d<3;d++){
		distanceToGo[d] = (distanceToGo[d]>0) ? 
			Mathf.Min(destinyWithoutFix[d],destiny[d]) : Mathf.Max(destinyWithoutFix[d],destiny[d]);
	}
	distanceToGo-=transform.position;
 
	// Move the objects
	for(var i : int = 0;i<objectsInPlatform.length;i++){ 
		if(objectsInPlatform[i]){
			objectsInPlatform[i].transform.position+=distanceToGo;
		}else{
			objectsInPlatform.RemoveAt(i);
			i--;
		}
	}
 
	// Self explain
	if(destiny==transform.position)
		waitUntil = Time.timeSinceLevelLoad+waitTimeNow;
}
 
function OnTriggerEnter(collider : Collider){
	//Debug.Log("MovingPlatform OnTriggerEnter "+collider.gameObject);
	AddObject(collider);
}
 
function OnTriggerExit(collider : Collider){
	//Debug.Log("OnTriggerExit");
	RemoveObject(collider);
}
 
function OnCollisionEnter(collision : Collision) {
	//Debug.Log("MovingPlatform OnCollisionEnter "+collision.collider.gameObject);
	AddObject(collision.collider);
}
 
function OnCollisionExit(collision : Collision) {
	//Debug.Log("OnCollisionExit");
	RemoveObject(collision.collider);
}
 
function AddObject(collider : Collider) {
	objectsInPlatform.Add(collider.gameObject);
}
 
function RemoveObject(collider : Collider) {
	// Starts on 1 because that position 0 is always this gameObject  
	for(var i : int = 1;i<objectsInPlatform.length;i++){ 
		if(objectsInPlatform[i]==collider.gameObject){
			objectsInPlatform.RemoveAt(i);
			i--;
		}
	}
}
 
@script RequireComponent(Collider)
}
