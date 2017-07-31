// Original url: http://wiki.unity3d.com/index.php/SeekSteer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/SeekSteer.cs
// File based on original modification date of: 19 October 2012, at 18:20. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Matthew Hughes (he wears an argyle lab coat nowadays) 
Contents [hide] 
1 Description 
2 Usage 
3 C# - SeekSteer.cs 
4 Javascript - SeekSteer.js 

DescriptionThis script will allow you to set multiple waypoints, and have an object follow them loosely, turning smoothing to get to the next one. The resulting effect is like a missile seeking out targets and getting 'close enough' then moving on to the next target. 
UsageAssign this behavior to the GameObject you want to see follow the waypoints. Set the size of the waypoint array to the number of waypoints required and attach each waypoint GameObject to its respective location in the array. 
C# - SeekSteer.cs// SeekSteer.cs
// Written by Matthew Hughes
// 19 April 2009
// Uploaded to Unify Community Wiki on 19 April 2009
// URL: http://www.unifycommunity.com/wiki/index.php?title=SeekSteer
using UnityEngine;
using System.Collections;
 
public class SeekSteer : MonoBehaviour
{
 
    public Transform[] waypoints;
    public float waypointRadius = 1.5f;
    public float damping = 0.1f;
    public bool loop = false;
    public float speed = 2.0f;
    public bool faceHeading = true;
 
    private Vector3 currentHeading,targetHeading;
    private int targetwaypoint;
    private Transform xform;
    private bool useRigidbody;
    private Rigidbody rigidmember;
 
 
    // Use this for initialization
    protected void Start ()
    {
        xform = transform;
        currentHeading = xform.forward;
        if(waypoints.Length<=0)
        {
            Debug.Log("No waypoints on "+name);
            enabled = false;
        }
        targetwaypoint = 0;
        if(rigidbody!=null)
        {
        	useRigidbody = true;
        	rigidmember = rigidbody;
        }
        else
        {
        	useRigidbody = false;
        }
    }
 
 
    // calculates a new heading
    protected void FixedUpdate ()
    {
        targetHeading = waypoints[targetwaypoint].position - xform.position;
 
        currentHeading = Vector3.Lerp(currentHeading,targetHeading,damping*Time.deltaTime);
    }
 
    // moves us along current heading
    protected void Update()
    {
    	if(useRigidbody)
    		rigidmember.velocity = currentHeading * speed;
    	else
	        xform.position +=currentHeading * Time.deltaTime * speed;
        if(faceHeading)
            xform.LookAt(xform.position+currentHeading);
 
        if(Vector3.Distance(xform.position,waypoints[targetwaypoint].position)<=waypointRadius)
        {
            targetwaypoint++;
            if(targetwaypoint>=waypoints.Length)
            {
                targetwaypoint = 0;
                if(!loop)
                    enabled = false;
            }
        }
    }
 
 
    // draws red line from waypoint to waypoint
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(waypoints==null)
            return;
        for(int i=0;i< waypoints.Length;i++)
        {
            Vector3 pos = waypoints[i].position;
            if(i>0)
            {
                Vector3 prev = waypoints[i-1].position;
                Gizmos.DrawLine(prev,pos);
            }
        }
    }
 
}

Javascript - SeekSteer.jsI made this in JavaScript just for fun... 
// SeekSteer.cs
// Written by Matthew Hughes
// 19 April 2009
// Uploaded to Unify Community Wiki on 19 April 2009
// URL: http://www.unifycommunity.com/wiki/index.php?title=SeekSteer
 
//remade in JavaScript by Pier Shaw 
// URL: www.firecg.com 
//,,see my other sites http://www.industrialclubs.com/
 
    var waypoints : Transform[];
    var waypointRadius : float  = 1.5;
    var damping : float = 0.1;
    var loop : boolean = false;
    var speed : float = 2.0;
    var faceHeading : boolean = true;
 
    private var targetHeading : Vector3;
    private var currentHeading : Vector3;
    private var targetwaypoint : int;
    private var xform : Transform;
    private var useRigidbody : boolean;
    private var rigidmember : Rigidbody;
 
 
    // Use this for initialization
   function Start() {
        xform = transform;
        currentHeading = xform.forward;
        if(waypoints.Length<=0)
        {
            Debug.Log("No waypoints on "+name);
            enabled = false;
        }
        targetwaypoint = 0;
        if(rigidbody!=null)
        {
            useRigidbody = true;
            rigidmember = rigidbody;
        }
        else
        {
            useRigidbody = false;
        }
    }
 
 
    // calculates a new heading
    function FixedUpdate() {
        targetHeading = waypoints[targetwaypoint].position - xform.position;
 
        currentHeading = Vector3.Lerp(currentHeading,targetHeading,damping*Time.deltaTime);
    }
 
    // moves us along current heading
    function Update(){
 
        if(useRigidbody)
            rigidmember.velocity = currentHeading * speed;
        else
            xform.position +=currentHeading * Time.deltaTime * speed;
        if(faceHeading)
            xform.LookAt(xform.position+currentHeading);
 
        if(Vector3.Distance(xform.position,waypoints[targetwaypoint].position)<=waypointRadius)
        {
            targetwaypoint++;
            if(targetwaypoint>=waypoints.Length)
            {
                targetwaypoint = 0;
                if(!loop)
                    enabled = false;
            }
        }
    }
 
 
    // draws red line from waypoint to waypoint
    function OnDrawGizmos(){
 
        Gizmos.color = Color.red;
        for(var i : int = 0; i< waypoints.Length;i++)
        {
           var pos : Vector3 = waypoints[i].position;
            if(i>0)
            {
                var prev : Vector3 = waypoints[i-1].position;
                Gizmos.DrawLine(prev,pos);
            }
        }
    }
}
