// Original url: http://wiki.unity3d.com/index.php/TorqueStabilizer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/PhysicsScripts/TorqueStabilizer.cs
// File based on original modification date of: 6 February 2012, at 22:51. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.PhysicsScripts
{
Author: Rune Skovbo Johansen 
DescriptionThis script basically implements a torque-based stabilizer. 
UsageAttach the script to any gameobject with a rigidbody. 
C# - TorqueStabilizer.csusing UnityEngine;
using System.Collections;
 
public class TorqueStabilizer : MonoBehaviour {
 
    public float stability = 0.3f;
    public float speed = 2.0f;
 
    // Update is called once per frame
    void FixedUpdate () {
        Vector3 predictedUp = Quaternion.AngleAxis(
            rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
            rigidbody.angularVelocity
        ) * transform.up;
 
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        // Uncomment the next line to stabilize on only 1 axis.
        //torqueVector = Vector3.Project(torqueVector, transform.forward);
        rigidbody.AddTorque(torqueVector * speed);
    }
}
}
