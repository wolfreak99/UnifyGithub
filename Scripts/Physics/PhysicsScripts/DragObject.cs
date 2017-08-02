/*************************
 * Original url: http://wiki.unity3d.com/index.php/DragObject
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/PhysicsScripts/DragObject.cs
 * File based on original modification date of: 20 January 2013, at 00:47. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * Description 
 *   
 * Usage 
 *   
 * JavaScript - DragObject.js 
 *   
 * C# - Dragable.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.PhysicsScripts
{
    DescriptionThis is similar to DragRigidbody, but much more stable and with more direct control. In its current form, it's intended for use with a camera looking down on the X/Z plane and dragging objects on a flat planar surface, but it could be adapted to be more flexible if desired. See here for a demo scene. 
    Usage Attach this script to the object you want to drag. The object should be a non-kinematic rigidbody. Gravity can be on or off; Freeze Rotation can also be on or off. Mass, drag, and angular drag generally don't matter, although mass matters somewhat when dragging into other non-kinematic rigidbodies. There are some public variables: 
    Normal Collision Count: This is an important variable, since how well the script works depends on this being correct. If the object is normally resting on the ground while being dragged, set this variable to 1. If it's not in contact with the ground while being dragged (or if there is no ground at all), then set this variable to 0. 
    Move Limit: Prevents the dragged object from going through other objects when being dragged quickly. This creates some lag when dragging objects at high speeds, since the object won't be able to keep up with the mouse cursor. However, if you want to reduce the lag, it's unlikely you would want to set this above the default 0.5. Instead, you can increase the number of physics frames per second (which increases CPU requirements, so be careful with that...e.g., 0.2 physics timestep is the default 50 fps, and .1 would make it 100 fps). 
    Collision Move Factor: How much dragging force is slowed down when colliding with an object. The default .01 is good for ensuring that the dragged object remains stable when colliding, but makes it somewhat "sticky" when trying to slide it along other objects. You can increase the value to make it less sticky, but larger values may cause some instability when colliding. 
    Add Height When Clicked: Number of units to add to the object's height when clicked on. The default 0 means nothing happens. This can be used for "picking up" objects when clicked. If the object is raised enough so that it's no longer in contact with the floor when it's being dragged, remember to set Normal Collision Count to 0. The object will either be restored to its usual height when let go if it doesn't use gravity, or else it will naturally fall back down to the floor if it does use gravity. 
    Freeze Rotation On Drag: If this is checked, the object will not rotate when being dragged around and colliding with other objects, regardless of whether the rotation is normally frozen. If it's unchecked, then it can rotate when colliding. 
    Cam: The camera being used. If this is not set, the camera tagged "Main Camera" will be used. (And if that's not found, an error will occur.) 
    JavaScript - DragObject.js var normalCollisionCount = 1;
    var moveLimit = .5;
    var collisionMoveFactor = .01;
    var addHeightWhenClicked = 0.0;
    var freezeRotationOnDrag = true;
    var cam : Camera;
    private var myRigidbody : Rigidbody;
    private var myTransform : Transform;
    private var canMove = false;
    private var yPos : float;
    private var gravitySetting : boolean;
    private var freezeRotationSetting : boolean;
    private var sqrMoveLimit : float;
    private var collisionCount = 0;
    private var camTransform : Transform;
     
    function Start () {
    	myRigidbody = rigidbody;
    	myTransform = transform;
    	if (!cam) {
    		cam = Camera.main;
    	}
    	if (!cam) {
    		Debug.LogError("Can't find camera tagged MainCamera");
    		return;
    	}
    	camTransform = cam.transform;
    	sqrMoveLimit = moveLimit * moveLimit;	// Since we're using sqrMagnitude, which is faster than magnitude
    }
     
    function OnMouseDown () {
    	canMove = true;
    	myTransform.Translate(Vector3.up*addHeightWhenClicked);
    	gravitySetting = myRigidbody.useGravity;
    	freezeRotationSetting = myRigidbody.freezeRotation;
    	myRigidbody.useGravity = false;
    	myRigidbody.freezeRotation = freezeRotationOnDrag;
    	yPos = myTransform.position.y;
    }
     
    function OnMouseUp () {
    	canMove = false;
    	myRigidbody.useGravity = gravitySetting;
    	myRigidbody.freezeRotation = freezeRotationSetting;
    	if (!myRigidbody.useGravity) {
    		myTransform.position.y = yPos-addHeightWhenClicked;
    	}
    }
     
    function OnCollisionEnter () {
    	collisionCount++;
    }
     
    function OnCollisionExit () {
    	collisionCount--;
    }
     
    function FixedUpdate () {
    	if (!canMove) return;
     
    	myRigidbody.velocity = Vector3.zero;
    	myRigidbody.angularVelocity = Vector3.zero;
    	myTransform.position.y = yPos;
    	var mousePos = Input.mousePosition;
    	var move = cam.ScreenToWorldPoint(Vector3(mousePos.x, mousePos.y, camTransform.position.y - myTransform.position.y)) - myTransform.position;
    	move.y = 0.0;
    	if (collisionCount > normalCollisionCount) {
    		move = move.normalized*collisionMoveFactor;
    	}
    	else if (move.sqrMagnitude > sqrMoveLimit) {
    		move = move.normalized*moveLimit;
    	}
     
        myRigidbody.MovePosition(myRigidbody.position + move);
    }
     
    @script RequireComponent(Rigidbody)C# - Dragable.cs using UnityEngine;
    using System.Collections;
     
    [RequireComponent(typeof(Rigidbody))]
    public class Dragable : MonoBehaviour
    {
     
    	public int normalCollisionCount = 1;
    	public float moveLimit = .5f;
    	public float collisionMoveFactor = .01f;
    	public float addHeightWhenClicked = 0.0f;
    	public bool freezeRotationOnDrag = true;
    	public Camera cam  ;
    	private Rigidbody myRigidbody ;
    	private Transform myTransform  ;
    	private bool canMove = false;
    	private float yPos;
    	private bool gravitySetting ;
    	private bool freezeRotationSetting ;
    	private float sqrMoveLimit ;
    	private int collisionCount = 0;
    	private Transform camTransform ;
     
    	void Start () 
    	{
    	    myRigidbody = rigidbody;
    	    myTransform = transform;
    	    if (!cam) 
    		{
    	        cam = Camera.main;
    	    }
    	    if (!cam) 
    		{
    	        Debug.LogError("Can't find camera tagged MainCamera");
    	        return;
    	    }
    	    camTransform = cam.transform;
    	    sqrMoveLimit = moveLimit * moveLimit;   // Since we're using sqrMagnitude, which is faster than magnitude
    	}
     
    	void OnMouseDown () 
    	{
    	    canMove = true;
    	    myTransform.Translate(Vector3.up*addHeightWhenClicked);
    	    gravitySetting = myRigidbody.useGravity;
    	    freezeRotationSetting = myRigidbody.freezeRotation;
    	    myRigidbody.useGravity = false;
    	    myRigidbody.freezeRotation = freezeRotationOnDrag;
    	    yPos = myTransform.position.y;
    	}
     
    	void OnMouseUp () 
    	{
    	    canMove = false;
    	    myRigidbody.useGravity = gravitySetting;
    	    myRigidbody.freezeRotation = freezeRotationSetting;
    	    if (!myRigidbody.useGravity) 
    		{
    			Vector3 pos = myTransform.position;
    	        pos.y = yPos-addHeightWhenClicked;
    	        myTransform.position = pos;
    	    }
    	}
     
    	void OnCollisionEnter () 
    	{
    	    collisionCount++;
    	}
     
    	void OnCollisionExit () 
    	{
    	    collisionCount--;
    	}
     
    	void FixedUpdate () 
    	{
    	    if (!canMove)
    		{
    			return;
    		}
     
    	    myRigidbody.velocity = Vector3.zero;
    	    myRigidbody.angularVelocity = Vector3.zero;
     
    		Vector3 pos = myTransform.position;
    	    pos.y = yPos;
    	    myTransform.position = pos;
     
    	    Vector3 mousePos = Input.mousePosition;
    	    Vector3 move = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camTransform.position.y - myTransform.position.y)) - myTransform.position;
    	    move.y = 0.0f;
    	    if (collisionCount > normalCollisionCount)		
    		{
    	        move = move.normalized*collisionMoveFactor;
    	    }
    	    else if (move.sqrMagnitude > sqrMoveLimit) 
    		{
    	        move = move.normalized*moveLimit;
    	    }
     
    	    myRigidbody.MovePosition(myRigidbody.position + move);
    	}
}
}
