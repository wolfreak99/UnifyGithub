/*************************
 * Original url: http://wiki.unity3d.com/index.php/DontGoThroughThings
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/PhysicsScripts/DontGoThroughThings.cs
 * File based on original modification date of: 17 May 2015, at 21:56. 
 *
 * Author: Daniel Brauer, Adrian 
 *
 * Description 
 *   
 * Changes 
 *   
 * Usage 
 *   
 * JavaScript - DontGoThroughThings.js 
 *   
 * Boo - DontGoThroughThings.boo 
 *   
 * C# - DontGoThroughThings.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.PhysicsScripts
{
    DescriptionThis script uses raycasting to avoid the physics engine letting fast-moving objects go through other objects (particularly meshes). 
    Changesv1.1 (Boo): 
    Added "Switch To Layer" convenience option to move the game object to a given layer and remove that layer from the layer mask (the original layer is restored if used together with "Time to Live"). 
    Added "Time to Live" option to automatically destroy the script after a given time to reduce raycasts and instances. 
    Moved initialization to an Init method to make the script more usable if added through script. 
    UsageAttach the script to any object that might move fast enough to go through other colliders. Make sure that the LayerMask does not include the layer of the object the script is attached to, otherwise the object will collide with itself. Leave Skin Width at 0.1 unless the object still passes through other colliders. If this happens, increase the skin width until the issue stops or you reach a value of 1.0. 
    JavaScript - DontGoThroughThings.js#pragma strict 
     
    var layerMask : LayerMask; //make sure we aren't in this layer 
    var skinWidth : float = 0.1; //probably doesn't need to be changed 
    private var minimumExtent : float; 
    private var partialExtent : float; 
    private var sqrMinimumExtent : float; 
    private var previousPosition : Vector3; 
    private var myRigidbody : Rigidbody; 
    //initialize values 
    function Awake() { 
       myRigidbody = rigidbody; 
       previousPosition = myRigidbody.position; 
       minimumExtent = Mathf.Min(Mathf.Min(collider.bounds.extents.x, collider.bounds.extents.y), collider.bounds.extents.z); 
       partialExtent = minimumExtent*(1.0 - skinWidth); 
       sqrMinimumExtent = minimumExtent*minimumExtent; 
    } 
     
    function FixedUpdate() { 
       //have we moved more than our minimum extent? 
       var movementThisStep : Vector3 = myRigidbody.position - previousPosition; 
       var movementSqrMagnitude : float = movementThisStep.sqrMagnitude;
       if (movementSqrMagnitude > sqrMinimumExtent) { 
          var movementMagnitude : float = Mathf.Sqrt(movementSqrMagnitude);
          var hitInfo : RaycastHit; 
          //check for obstructions we might have missed 
          if (Physics.Raycast(previousPosition, movementThisStep, hitInfo, movementMagnitude, layerMask.value)) 
             myRigidbody.position = hitInfo.point - (movementThisStep/movementMagnitude)*partialExtent; 
       } 
       previousPosition = myRigidbody.position; 
    }Boo - DontGoThroughThings.boo(See Script Compilation as for how Boo scripts can be used from JavaScript and C#). 
    import UnityEngine
     
    # DontGoThroughThings v1.1
    # Created by Adrian on 2008-10-29.
    # Original Script by Daniel Brauer
     
    class DontGoThroughThings (MonoBehaviour):
     
    	# ---------------------------------------- #
    	# PUBLIC FIELDS
     
    	# Layers the Raycast checks against
    	# The game object of this script should not be on 
    	# a layer set in this mask or it will collide with itself.
    	public layerMask as LayerMask = System.Int32.MaxValue
     
    	# How far the object is set into the object it 
    	# shoud have collided with to force a physics collision
    	# (Should probably be fine at 0.1 and must be between 0 and 1)
    	public skinWidth as single = 0.1
     
    	# Move the game object to this layer and remove 
    	# the layer from the layer mask. This is a convenience 
    	# feature to make sure the game object doesn't collide 
    	# with itself. (set to -1 to disable)
    	public switchToLayer as int = -1
     
    	# Time in seconds before the script destroys itself 
    	# after it has been created or initialized.
    	# Convenient to protected the game object only for 
    	# a critical time to avoid useless raycasts.
    	# (set to -1 to disable)
    	public timeToLive as single = -1
     
    	# ---------------------------------------- #
    	# PRIVATE FIELDS
     
    	private startTime as single
     
    	private originalLayer as int = -1
     
    	private minimumExtent as single
    	private partialExtent as single
    	private sqrMinimumExtent as single
    	private previousPosition as Vector3
     
    	private myRigidbody as Rigidbody
     
    	# ---------------------------------------- #
    	# METHODS
     
    	# Initialize the script on awake.
    	def Awake():
    		Init(timeToLive, switchToLayer)
     
    	# Initialize method to be used when adding 
    	# this component from a script.
    	def Init(ttl as single, layer as int):
    		# Switch layer of game object
    		if (layer >= 0):
    			originalLayer = gameObject.layer
    			gameObject.layer = layer
    			switchToLayer = layer
    			# Clear the layer in the layer mask
    			layerMask = layerMask.value & ~(1 << gameObject.layer)
     
    		# Time to live
    		if (ttl >= 0):
    			startTime = Time.time
    			timeToLive = ttl
     
    		# Initialize variables
    		myRigidbody = rigidbody
    		previousPosition = myRigidbody.position
    		minimumExtent = Mathf.Min(Mathf.Min(
    							cast(single,collider.bounds.extents.x), 
    							cast(single,collider.bounds.extents.y)), 
    							cast(single,collider.bounds.extents.z))
    		sqrMinimumExtent = minimumExtent ** 2
    		partialExtent = minimumExtent*(1.0 - skinWidth)
     
    	# Collision checking
    	def FixedUpdate():
    		# Check time to live
    		if (timeToLive > 0 and Time.time > startTime + timeToLive):
    			# Restore original layer
    			if (originalLayer >= 0):
    				gameObject.layer = originalLayer
    			Destroy(self)
    			return
     
    		# Only check for missed collisions if the game object moved more than its minimum extent
    		if (myRigidbody):
    			movementThisStep = myRigidbody.position - previousPosition
    			movementSqrMagnitude = movementThisStep.sqrMagnitude
    			if (movementSqrMagnitude > sqrMinimumExtent):
    				movementMagnitude = Mathf.Sqrt(movementSqrMagnitude)
    				hitInfo as RaycastHit
    				if (Physics.Raycast(previousPosition, movementThisStep, hitInfo, 
    										movementMagnitude, layerMask.value)):
    					# Move rigidbody back to right before the collision was missed
    					myRigidbody.MovePosition(
    						hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent)
     
    		previousPosition = myRigidbody.position
    
    C# - DontGoThroughThings.csusing UnityEngine;
    using System.Collections;
     
    public class DontGoThroughThings : MonoBehaviour
    {
           // Careful when setting this to true - it might cause double
           // events to be fired - but it won't pass through the trigger
           public bool sendTriggerMessage = false; 	
     
    	public LayerMask layerMask = -1; //make sure we aren't in this layer 
    	public float skinWidth = 0.1f; //probably doesn't need to be changed 
     
    	private float minimumExtent; 
    	private float partialExtent; 
    	private float sqrMinimumExtent; 
    	private Vector3 previousPosition; 
    	private Rigidbody myRigidbody;
    	private Collider myCollider;
     
    	//initialize values 
    	void Start() 
    	{ 
    	   myRigidbody = GetComponent<Rigidbody>();
    	   myCollider = GetComponent<Collider>();
    	   previousPosition = myRigidbody.position; 
    	   minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z); 
    	   partialExtent = minimumExtent * (1.0f - skinWidth); 
    	   sqrMinimumExtent = minimumExtent * minimumExtent; 
    	} 
     
    	void FixedUpdate() 
    	{ 
    	   //have we moved more than our minimum extent? 
    	   Vector3 movementThisStep = myRigidbody.position - previousPosition; 
    	   float movementSqrMagnitude = movementThisStep.sqrMagnitude;
     
    	   if (movementSqrMagnitude > sqrMinimumExtent) 
    		{ 
    	      float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
    	      RaycastHit hitInfo; 
     
    	      //check for obstructions we might have missed 
    	      if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
                  {
                     if (!hitInfo.collider)
                         return;
     
                     if (hitInfo.collider.isTrigger) 
                         hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);
     
                     if (!hitInfo.collider.isTrigger)
                         myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent; 
     
                  }
    	   } 
     
    	   previousPosition = myRigidbody.position; 
    	}
}
}
