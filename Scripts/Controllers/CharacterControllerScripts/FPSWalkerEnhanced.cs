// Original url: http://wiki.unity3d.com/index.php/FPSWalkerEnhanced
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/FPSWalkerEnhanced.cs
// File based on original modification date of: 10 January 2012, at 20:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript Version (FPSWalkerEnhanced.js) 
4 C# Version 
5 Boo Version 

Description This is an enhanced version of the FPSWalker script found in Standard Assets. It allows for both walking and running (either by toggle or by holding down a run key), the capability of detecting falling distances, optionally sliding down slopes (both those above the Slope Limit and/or objects specifically tagged "Slide"), optional air control, optional anti-bunny hopping control, and special patented* Anti-Bump™ code that eliminates that irritating "bumpity bumpity bumpa bump" you get when trying to walk down moderately inclined slopes when using the default script. 
*It's not really patented...it's just a small change actually. 
Note that the new first-person controller prefab in Unity 3 handles most of this, as well as having other functionality. 
Usage In any situation where you'd normally use the standard FPSWalker script, you can replace it with this instead. The original parameters are still there and work as usual (although "speed" is now "walk speed"). 
Walk Speed: How fast the player moves when walking (the default). 
Run Speed: How fast the player moves when running. 
Limit Diagonal Speed: If checked, strafing combined with moving forward or backward can't exceed the normal movement rate. The horizontal and vertical movement axes are computed independently, so if this isn't checked, then diagonal speed is about 1.4 times faster than normal. 
Toggle Run: If checked, the player can toggle between running and walking by pressing the run button (there must be a button set up in the Input Manager called "Run"). If not checked, the player normally walks unless holding down the run button. 
Jump Speed: How high the player jumps when hitting the jump button (there must be a button set up in the Input Manager called "Jump", which there is by default). 
Gravity: How fast the player falls when not standing on anything. 
Falling Damage Threshold: How many units the player can fall before taking damage when landing. The script as-is merely prints the total number of units that the player fell if this happens, but the FallingDamageAlert function can be changed to do anything the programmer desires. The "fallDistance" local variable in this function contains the number of vertical units between the initial point of "catching air" and the final impact point. If falling damage is irrelevant, change the number to "Infinity" in the inspector, and the function will never be called. 
Slide When Over Slope Limit: If checked, the player will slide down slopes that are greater than the Slope Limit as defined by the Character Controller. Attempting to jump up such slopes will also fail. The player has no control until resting on a surface that is under the Slope Limit. 
Slide On Tagged Objects: If checked, the player will slide down any objects tagged "Slide" when standing on them, regardless of how much or little they are sloped. (The tag "Slide" must be added to the Tag Manager.) This can be used to create chutes, icy surfaces, etc. Note that tagged objects with zero slope will cause sliding in an undefined direction. 
Slide Speed: How fast the player slides when on slopes as defined above. 
Air Control: If checked, the player will be able to control movement while in the air, except when Slide When Over Slope Limit/Slide On Tagged Objects is enabled and the player is jumping off a slope over the limit (otherwise the player would be able to jump up supposedly inaccessible slopes). 
Anti Bump Factor: An amount used to reduce or eliminate the "bumping" that can occur when walking down slopes, which is a result of the player constantly toggling between walking and falling small distances. The default of .75 is sufficient for most cases, although a little bit can still occur on steep slopes, assuming sliding isn't enabled. Larger amounts will stop this from ever happening, but too much can result in excessive falling speeds when stepping over an edge. Very small amounts will enable bumping, if that's desired for some reason. 
Anti Bunny Hop Factor: Bunny hopping is repeated jumping by virtue of continuously holding down the jump button. Often considered annoying and silly, especially in multiplayer games. If the anti-hop value is at least 1, the player must release the jump button (and the release-and-hold-in-the-air trick is ineffective), and be grounded for the specified number of physics frames before being able to jump again. If the value is 0, then bunny hopping is allowed. If using noticeably large values, implementing some kind of visual indicator of jump availability is recommended to avoid player frustration. 
The slope sliding is quite basic: the player is either sliding or not, and has no lateral control when sliding. One issue that may surface is that, under some circumstances, attempting to force oneself up a slippery slope will result in annoying jittering, as the character controller moves forward a bit one frame only to slide back the next frame, then moves forward a bit again, etc. Possible solutions are: add code to the script so this doesn't happen, design levels so as to minimize this, or ignore it and repeat to yourself "If it's good enough for UT2004, it's good enough for me." It's also not a good idea to create situations where the player slides into a V-shaped wedge valley where both slopes are unscalable, since the player will be stuck at the bottom, jittering for eternity. Not fun. 
JavaScript Version (FPSWalkerEnhanced.js) var walkSpeed = 6.0;
var runSpeed = 11.0;
 
// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
var limitDiagonalSpeed = true;
 
// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
// There must be a button set up in the Input Manager called "Run"
var toggleRun = false;
 
var jumpSpeed = 8.0;
var gravity = 20.0;
 
// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
var fallingDamageThreshold = 10.0;
 
// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
var slideWhenOverSlopeLimit = false;
 
// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
var slideOnTaggedObjects = false;
 
var slideSpeed = 12.0;
 
// If checked, then the player can change direction while in the air
var airControl = false;
 
// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
var antiBumpFactor = .75;
 
// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping 
var antiBunnyHopFactor = 1;
 
private var moveDirection = Vector3.zero;
private var grounded = false;
private var controller : CharacterController;
private var myTransform : Transform;
private var speed : float;
private var hit : RaycastHit;
private var fallStartLevel : float;
private var falling = false;
private var slideLimit : float;
private var rayDistance : float;
private var contactPoint : Vector3;
private var playerControl = false;
private var jumpTimer : int;
 
function Start () {
	controller = GetComponent(CharacterController);
	myTransform = transform;
	speed = walkSpeed;
	rayDistance = controller.height * .5 + controller.radius;
	slideLimit = controller.slopeLimit - .1;
	jumpTimer = antiBunnyHopFactor;
	oldPos = transform.position;
}
 
function FixedUpdate() {
	var inputX = Input.GetAxis("Horizontal");
	var inputY = Input.GetAxis("Vertical");
	// If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
	var inputModifyFactor = (inputX != 0.0 && inputY != 0.0 && limitDiagonalSpeed)? .7071 : 1.0;
 
	if (grounded) {
		var sliding = false;
		// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
		// because that interferes with step climbing amongst other annoyances
		if (Physics.Raycast(myTransform.position, -Vector3.up, hit, rayDistance)) {
			if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
				sliding = true;
		}
		// However, just raycasting straight down from the center can fail when on steep slopes
		// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
		else {
			Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, hit);
			if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
				sliding = true;
		}
 
		// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
		if (falling) {
			falling = false;
			if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
				FallingDamageAlert (fallStartLevel - myTransform.position.y);
		}
 
		// If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
		if (!toggleRun) 
			speed = Input.GetButton("Run")? runSpeed : walkSpeed;
 
		// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
		if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
			var hitNormal = hit.normal;
			moveDirection = Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
			Vector3.OrthoNormalize (hitNormal, moveDirection);
			moveDirection *= slideSpeed;
			playerControl = false;
		}
		// Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
		else {
			moveDirection = Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
			moveDirection = myTransform.TransformDirection(moveDirection) * speed;
			playerControl = true;
		}
 
		// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
		if (!Input.GetButton("Jump"))
			jumpTimer++;
		else if (jumpTimer >= antiBunnyHopFactor) {
			moveDirection.y = jumpSpeed;
			jumpTimer = 0;
		}
	}
	else {
		// If we stepped over a cliff or something, set the height at which we started falling
		if (!falling) {
			falling = true;
			fallStartLevel = myTransform.position.y;
		}
 
		// If air control is allowed, check movement but don't touch the y component
		if (airControl && playerControl) {
			moveDirection.x = inputX * speed * inputModifyFactor;
			moveDirection.z = inputY * speed * inputModifyFactor;
			moveDirection = myTransform.TransformDirection(moveDirection);
		}
	}
 
	// Apply gravity
	moveDirection.y -= gravity * Time.deltaTime;
 
	// Move the controller, and set grounded true or false depending on whether we're standing on something
	grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
}
 
function Update () {
	// If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
	// FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
	if (toggleRun && grounded && Input.GetButtonDown("Run"))
		speed = (speed == walkSpeed? runSpeed : walkSpeed);
}
 
// Store point that we're in contact with for use in FixedUpdate if needed
function OnControllerColliderHit (hit : ControllerColliderHit) {
	contactPoint = hit.point;
}
 
// If falling damage occured, this is the place to do something about it. You can make the player
// have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
function FallingDamageAlert (fallDistance : float) {
	Debug.Log ("Ouch! Fell " + fallDistance + " units!");	
}
 
@script RequireComponent(CharacterController)C# Version Identical script, just converted to C#. 
Converted by: --TwiiK 09:40, 1 May 2010 (PDT) 
using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (CharacterController))]
public class FPSWalkerEnhanced: MonoBehaviour {
 
    public float walkSpeed = 6.0f;
 
    public float runSpeed = 11.0f;
 
    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    public bool limitDiagonalSpeed = true;
 
    // If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
    // There must be a button set up in the Input Manager called "Run"
    public bool toggleRun = false;
 
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
 
    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    public float fallingDamageThreshold = 10.0f;
 
    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;
 
    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;
 
    public float slideSpeed = 12.0f;
 
    // If checked, then the player can change direction while in the air
    public bool airControl = false;
 
    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;
 
    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public int antiBunnyHopFactor = 1;
 
    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;
 
    void Start() {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
    }
 
    void FixedUpdate() {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;
 
        if (grounded) {
            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
 
            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling) {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert (fallStartLevel - myTransform.position.y);
            }
 
            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            if (!toggleRun)
                speed = Input.GetButton("Run")? runSpeed : walkSpeed;
 
            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }
 
            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!Input.GetButton("Jump"))
                jumpTimer++;
            else if (jumpTimer >= antiBunnyHopFactor) {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
            }
        }
        else {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling) {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }
 
            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControl) {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }
 
        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
 
        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }
 
    void Update () {
        // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
        // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
        if (toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed? runSpeed : walkSpeed);
    }
 
    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit) {
        contactPoint = hit.point;
    }
 
    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance) {
        print ("Ouch! Fell " + fallDistance + " units!");   
    }
}Boo Version import UnityEngine
 
[RequireComponent(CharacterController)]
 
class FPSWalkerEnhanced (MonoBehaviour): 
 
	public walkSpeed = 6f
	public runSpeed = 11f
 
	# If limitDiagonalSpeed is true, diagonal speed (when strafing + moving forward or back) 
	# can't exceed normal move speed; otherwise it's about 1.4 times faster
 
	public limitDiagonalSpeed = true
 
	# If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
	# There must be a button set up in the Input Manager called "Run"
	toggleRun = false;
 
	jumpSpeed = 8.0;
	gravity = 20.0;
 
	# Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	fallingDamageThreshold = 10.0;
 
	# If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
	slideWhenOverSlopeLimit = false
 
	# If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
	slideOnTaggedObjects = false
 
	slideSpeed = 12.0
 
	# If checked, then the player can change direction while in the air
	airControl = false
 
	# Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	antiBumpFactor = .75
 
	# Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping 
	antiBunnyHopFactor = 1
 
	private moveDirection = Vector3.zero
	private grounded = false
	private controller as CharacterController
	private myTransform as Transform
	private speed as single
	private hit as RaycastHit
	private fallStartLevel as single
	private falling = false
	private slideLimit as single
	private rayDistance as single
	private contactPoint as Vector3
	private playerControl = false
	private jumpTimer as int
 
	def Start ():
	    controller = GetComponent(CharacterController)
	    myTransform = transform
	    speed = walkSpeed
	    rayDistance = controller.height * .5 + controller.radius
	    slideLimit = controller.slopeLimit - .1
	    jumpTimer = antiBunnyHopFactor
	    oldPos = transform.position
 
	def FixedUpdate():
	    inputX = Input.GetAxis("Horizontal");
	    inputY = Input.GetAxis("Vertical");
	    # If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
	    inputModifyFactor = (.7071 if (inputX != 0.0 and inputY != 0.0 and limitDiagonalSpeed) else 1.0)
 
	    if grounded:
	        sliding = false
	        # See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
	        # because that interferes with step climbing amongst other annoyances
	        if Physics.Raycast(myTransform.position, -Vector3.up, hit, rayDistance):
	            if Vector3.Angle(hit.normal, Vector3.up) > slideLimit:
	                sliding = true
	        # However, just raycasting straight down from the center can fail when on steep slopes
	        # So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
        	else:
	            Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, hit)
	            if Vector3.Angle(hit.normal, Vector3.up) > slideLimit:
	                sliding = true
 
 
	        # If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
	        if falling:
	            falling = false
	            if myTransform.position.y < fallStartLevel - fallingDamageThreshold:
	                FallingDamageAlert(fallStartLevel - myTransform.position.y)
 
	        # If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
	        if not toggleRun:
	            speed = (runSpeed if Input.GetButton("Run") else walkSpeed)
 
	        # If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
	        if (sliding and slideWhenOverSlopeLimit) or (slideOnTaggedObjects and hit.collider.tag == "Slide"):
	            hitNormal = hit.normal
	            moveDirection = Vector3(hitNormal.x, -hitNormal.y, hitNormal.z)
	            Vector3.OrthoNormalize (hitNormal, moveDirection)
	            moveDirection *= slideSpeed
	            playerControl = false
 
	        # Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
	        else:
	            moveDirection = Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor)
	            moveDirection = myTransform.TransformDirection(moveDirection) * speed
	            playerControl = true
 
 
	        # Jump! But only if the jump button has been released and player has been grounded for a given number of frames
	        if not Input.GetButton("Jump"):
	            jumpTimer++
	        elif jumpTimer >= antiBunnyHopFactor:
	            moveDirection.y = jumpSpeed;
	            jumpTimer = 0;
 
 
	    else:
	        # If we stepped over a cliff or something, set the height at which we started falling
	        if not falling:
	            falling = true;
	            fallStartLevel = myTransform.position.y;
 
 
	        # If air control is allowed, check movement but don't touch the y component
	        if airControl and playerControl:
	            moveDirection.x = inputX * speed * inputModifyFactor
	            moveDirection.z = inputY * speed * inputModifyFactor
	            moveDirection = myTransform.TransformDirection(moveDirection)
 
 
	    # Apply gravity
	    moveDirection.y -= gravity * Time.deltaTime;
 
	    # Move the controller, and set grounded true or false depending on whether we're standing on something
	    grounded = ((controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0)
 
 
	def Update ():
	    # If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
	    # FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
	    if toggleRun and grounded and Input.GetButtonDown("Run"):
	        speed = (runSpeed if speed == walkSpeed else walkSpeed)
 
 
	# Store point that we're in contact with for use in FixedUpdate if needed
	def OnControllerColliderHit (hit as ControllerColliderHit):
	    contactPoint = hit.point
 
 
	# If falling damage occured, this is the place to do something about it. You can make the player
	# have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
	def FallingDamageAlert (fallDistance as single):
	    Debug.Log ("Ouch! Fell " + fallDistance + " units!")
}
