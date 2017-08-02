/*************************
 * Original url: http://wiki.unity3d.com/index.php/Trajectory_Simulation
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/Trajectory_Simulation.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Matt Mechtley 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.SimulationScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 BasicTrajectorySimulation.js 
    4 C# Version 
    5 PlayerFire.cs 
    
    DescriptionThere are many cases where it's useful to calculate the trajectory an object will follow -- for instance when firing a ball from a cannon or the like. Perhaps you want to give the player some visual feedback about where their shot might land. The following script represents a very basic trajectory simulation algorithm that I used in the game Splume. It works best for simple spheres, and makes a number of assumptions: 
    The object being fired has 0 drag 
    Any object it bounces off has 0 friction and has friction combine set to Minimum 
    Any object it bounces off has 1 bouncyness and has bouncyness combine set to Maximum 
    That is, the object is assumed not to be losing any energy from drag or collisions. Simulating such cases is possible, but slightly more complicated. 
    UsageIt could potentially be costly to use this script in confined areas using many MeshColliders -- it raycasts 20 times per FixedUpdate. Also, the larger the collider attached to the fired ball, the more inaccurate this simulation becomes (since the actual ball hits with a point on its edge and not its center). This could be improved by using Physics.OverlapSphere instead of Physics.Raycast. 
    BasicTrajectorySimulation.js/* 
    * Controls the Laser Sight for the player's aim
    */
    // Reference to the LineRenderer we will use to display the simulated path
    var sightLine : LineRenderer;
     
    // Reference to a Component that holds information about fire strength, location of cannon, etc.
    var playerFire : PlayerFire;
     
    // Number of vertices to calculate - more gives a smoother line
    var numVertices : int = 20;
     
    // Length scale for each segment
    var segmentScale : float = 1.0;
     
    // Local cache of the line's positions
    private var positions : Vector3[];
     
    // The following may be useful for highlighting a target, etc.
     
    // gameobject we're actually pointing at
    private var hitObject:GameObject;
     
    function Start()
    {
        positions = new Vector3[numVertices];
    }
     
    function FixedUpdate()
    {
        simulatePath();
    }
     
    /**
    * simulate the path of a launched ball. 
    * Slight errors are inherent in the numerical method used
    */
    function simulatePath()
    {
        // The first line point is wherever the player's cannon, etc is
        positions[0] = playerFire.transform.position;
     
        // Time it takes to traverse one segment of length segScale
        var segTime : float;
     
        // The velocity of the current segment
        var segVelocity : Vector3 = playerFire.transform.up * playerFire.fireStrength * Time.fixedDeltaTime;
     
        var hit : RaycastHit;
     
        // reset our hit object
        hitObject = null;
     
        for (var i=1; i<numVertices; i++)
        {
            // worry about if velocity has zero magnitude
            if(segVelocity.sqrMagnitude != 0)
                segTime = segmentScale/segVelocity.magnitude;
            else
                segTime = 0;
            // Add velocity from gravity for this segment's timestep
            segVelocity = segVelocity + Physics.gravity*segTime;
     
            // Check to see if we're going to hit a physics object
            if(Physics.Raycast(positions[i-1], segVelocity, hit, segmentScale))
            {
                // set next position to the position where we hit the physics object
                positions[i] = positions[i-1] + segVelocity.normalized*hit.distance;
                // correct ending velocity, since we didn't actually travel an entire segment
                segVelocity = segVelocity - Physics.gravity*(segmentScale - hit.distance)/segVelocity.magnitude;
                // flip the velocity to simulate a bounce
                segVelocity = Vector3.Reflect(segVelocity, hit.normal);
     
                /*
                 * Here you could check if the object hit by the Raycast had some property - was 
                 * sticky, would cause the ball to explode, or was another ball in the air for 
                 * instance. You could then end the simulation by setting all further points to 
                 * this last point and then breaking this for loop, and setting 
                 * hitObject = hit.collider.gameObject.
                 */
            }
            // If our raycast hit no objects, then set the next position to the last one plus v*t
            else
            {
                positions[i] = positions[i-1] + segVelocity*segTime;
            }
        }
     
        // At the end, apply our simulations to the LineRenderer
     
        // Set the colour of our path to the colour of the next ball
        var startColor : Color = playerFire.nextColor;
        var endColor : Color = startColor;
        startColor.a = 1;
        endColor.a = 0;
        sightLine.SetColors(startColor, endColor);
     
        sightLine.SetVertexCount(numVertices);
        for(i=0; i<numVertices; i++)
        {
            sightLine.SetPosition(i, positions[i]);
        }
    }
     
    function getHitObject()
    {
        return hitObject;
    }C# VersionAuthor: Benoit FOULETIER 
    Basically just regex-translated the above, with a few minor changes (hitObject should reference the Collider itself IMO, not the GameObject).
    More stuff could be cleaned, for example the nextColor thing is very Splume-specific.
    It's unclear what fireStrength is: if it's a force, then mass should be taken into account (even disregarding drag, friction and bouncyness)... trajectory prediction is a complex business!! 
    using UnityEngine;
     
    /// <summary>
    /// Controls the Laser Sight for the player's aim
    /// </summary>
    public class TrajectorySimulation : MonoBehaviour
    {
    	// Reference to the LineRenderer we will use to display the simulated path
    	public LineRenderer sightLine;
     
    	// Reference to a Component that holds information about fire strength, location of cannon, etc.
    	public PlayerFire playerFire;
     
    	// Number of segments to calculate - more gives a smoother line
    	public int segmentCount = 20;
     
    	// Length scale for each segment
    	public float segmentScale = 1;
     
    	// gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
    	private Collider _hitObject;
    	public Collider hitObject { get { return _hitObject; } }
     
    	void FixedUpdate()
    	{
    		simulatePath();
    	}
     
    	/// <summary>
    	/// Simulate the path of a launched ball.
    	/// Slight errors are inherent in the numerical method used.
    	/// </summary>
    	void simulatePath()
    	{
    		Vector3[] segments = new Vector3[segmentCount];
     
    		// The first line point is wherever the player's cannon, etc is
    		segments[0] = playerFire.transform.position;
     
    		// The initial velocity
    		Vector3 segVelocity = playerFire.transform.up * playerFire.fireStrength * Time.deltaTime;
     
    		// reset our hit object
    		_hitObject = null;
     
    		for (int i = 1; i < segmentCount; i++)
    		{
    			// Time it takes to traverse one segment of length segScale (careful if velocity is zero)
    			float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;
     
    			// Add velocity from gravity for this segment's timestep
    			segVelocity = segVelocity + Physics.gravity * segTime;
     
    			// Check to see if we're going to hit a physics object
    			RaycastHit hit;
    			if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale))
    			{
    				// remember who we hit
    				_hitObject = hit.collider;
     
    				// set next position to the position where we hit the physics object
    				segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
    				// correct ending velocity, since we didn't actually travel an entire segment
    				segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;
    				// flip the velocity to simulate a bounce
    				segVelocity = Vector3.Reflect(segVelocity, hit.normal);
     
    				/*
    				 * Here you could check if the object hit by the Raycast had some property - was 
    				 * sticky, would cause the ball to explode, or was another ball in the air for 
    				 * instance. You could then end the simulation by setting all further points to 
    				 * this last point and then breaking this for loop.
    				 */
    			}
    			// If our raycast hit no objects, then set the next position to the last one plus v*t
    			else
    			{
    				segments[i] = segments[i - 1] + segVelocity * segTime;
    			}
    		}
     
    		// At the end, apply our simulations to the LineRenderer
     
    		// Set the colour of our path to the colour of the next ball
    		Color startColor = playerFire.nextColor;
    		Color endColor = startColor;
    		startColor.a = 1;
    		endColor.a = 0;
    		sightLine.SetColors(startColor, endColor);
     
    		sightLine.SetVertexCount(segmentCount);
    		for (int i = 0; i < segmentCount; i++)
    			sightLine.SetPosition(i, segments[i]);
    	}
    }PlayerFire.csThis is the minimum you need to use the above. Attach it to an object pointing in an interesting direction (up is no fun). Strength has to be high enough otherwise all you'll see is a line pointing down. 
    using UnityEngine;
     
    public class PlayerFire : MonoBehaviour
    {
    	public float fireStrength = 500;
    	public Color nextColor = Color.red;
}
}
