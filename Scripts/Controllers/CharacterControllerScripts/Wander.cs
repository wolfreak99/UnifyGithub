/*************************
 * Original url: http://wiki.unity3d.com/index.php/Wander
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/Wander.cs
 * File based on original modification date of: 8 June 2012, at 12:23. 
 *
 * Author: Matthew Miner (matthew@matthewminer.com) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 C# - Wander.cs 
    4 JavaScript Version Wander.js 
    
    Description Creates a wandering behaviour for NPCs. 
    Usage Attach the script to your NPC, ensuring a CharacterController component is also attached. Tweak the inspector variables to get the desired behaviour. 
    C# - Wander.cs using UnityEngine;
    using System.Collections;
     
    /// <summary>
    /// Creates wandering behaviour for a CharacterController.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Wander : MonoBehaviour
    {
    	public float speed = 5;
    	public float directionChangeInterval = 1;
    	public float maxHeadingChange = 30;
     
    	CharacterController controller;
    	float heading;
    	Vector3 targetRotation;
     
    	void Awake ()
    	{
    		controller = GetComponent<CharacterController>();
     
    		// Set random initial rotation
    		heading = Random.Range(0, 360);
    		transform.eulerAngles = new Vector3(0, heading, 0);
     
    		StartCoroutine(NewHeading());
    	}
     
    	void Update ()
    	{
    		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
    		var forward = transform.TransformDirection(Vector3.forward);
    		controller.SimpleMove(forward * speed);
    	}
     
    	/// <summary>
    	/// Repeatedly calculates a new direction to move towards.
    	/// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
    	/// </summary>
    	IEnumerator NewHeading ()
    	{
    		while (true) {
    			NewHeadingRoutine();
    			yield return new WaitForSeconds(directionChangeInterval);
    		}
    	}
     
    	/// <summary>
    	/// Calculates a new direction to move towards.
    	/// </summary>
    	void NewHeadingRoutine ()
    	{
    		var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
    		var ceil  = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
    		heading = Random.Range(floor, ceil);
    		targetRotation = new Vector3(0, heading, 0);
    	}
    }
    
    JavaScript Version Wander.js #pragma strict
     
    /// <summary>
    /// Creates wandering behaviour for a CharacterController.
    /// </summary>
    @script RequireComponent(CharacterController)
     
        var speed:float = 5;
        var directionChangeInterval:float = 1;
        var maxHeadingChange:float = 30;
     
        var heading: float=0;
        var targetRotation: Vector3 ;
     
        function Awake (){
     
            // Set random initial rotation
    		transform.eulerAngles = Vector3(0, Random.Range(0,360), 0);  // look in a random direction at start of frame.
     
            //StartCoroutine
    		NewHeadingRoutine ();
        }
     
        function Update (){
    		var controller : CharacterController = GetComponent(CharacterController);
     
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
            var forward = transform.TransformDirection(Vector3.forward);
            controller.SimpleMove(forward * speed);
        }
     
        /// <summary>
        /// Repeatedly calculates a new direction to move towards.
        /// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
        /// </summary>
    	while (true){
    		NewHeadingRoutine();
    		yield WaitForSeconds(directionChangeInterval);
    	}
     
        /// <summary>
        /// Calculates a new direction to move towards.
        /// </summary>
        function NewHeadingRoutine (){
            var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
            var ceil  = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
            heading = Random.Range(floor, ceil);
            targetRotation = new Vector3(0, heading, 0);
    }
}
