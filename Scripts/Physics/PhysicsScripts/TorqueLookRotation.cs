// Original url: http://wiki.unity3d.com/index.php/TorqueLookRotation
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/PhysicsScripts/TorqueLookRotation.cs
// File based on original modification date of: 27 January 2012, at 22:26. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.PhysicsScripts
{
Author: Ducketts (from #unity) 
DescriptionThis script basically implements a torque-based lookrotation. 
UsageAttach the script to any gameobject with a rigidbody, set the drag desired level, set the target and away you go. 
C# - TorqueLookRotation.csusing UnityEngine;
using System.Collections;
 
// @robotduck 2011
// set the object's rigidbody angular drag to a high value, like 10
 
public class TorqueLookRotation : MonoBehaviour {
 
	public Transform target;
	public float force = 0.1f;
 
	void FixedUpdate () {
 
		Vector3 targetDelta = target.position - transform.position;
 
		//get the angle between transform.forward and target delta
		float angleDiff = Vector3.Angle(transform.forward, targetDelta);
 
		// get its cross product, which is the axis of rotation to
		// get from one vector to the other
		Vector3 cross = Vector3.Cross(transform.forward, targetDelta);
 
		// apply torque along that axis according to the magnitude of the angle.
		rigidbody.AddTorque(cross * angleDiff * force);
	}
}
}
