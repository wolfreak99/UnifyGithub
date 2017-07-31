// Original url: http://wiki.unity3d.com/index.php/MouseTorque
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/MouseTorque.cs
// File based on original modification date of: 5 December 2012, at 14:21. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: Robert Grant (rhgrant10) 
Description This is just a mouse look script, but instead of rotating the object directly, moving the mouse applies torque to the object, thereby rotating it indirectly using the physics engine. Useful for very smooth rotation of the camera, as well as applications involving spaceship-like control. 
Usage Basically, use this script just like any other mouse look script. There are horizontal and vertical sensitivity settings that can be adjusted to give the desired effect. Negative sensitivities invert the axis, and zero will disable rotation on that axis. Since coordinated movement through the application of forces is not always straightforward (in this case, the object tends to wobble away from being upright), there is a corrective force that acts to keep the object upright. Just like the sensitivity settings, setting the strength to zero will disable this corrective force. Some of the adjustments that control the resulting rotations are the properties of the rigidbody, mainly angular drag. If using the default sensitivity and strength values in the script, a good angular drag is probably around 5 or 6, but I've only tested that with my mouse on my computer, so your results may vary! 
Note: it is possible for the object's rotation to become slightly unstable. Using higher angular drag and corrective strength reduces the effects and frequency of the instability. 
Play with the sensitivities, strength, and angular drag settings until you get the desired result. 
using UnityEngine;
using System.Collections;
 
[AddComponentMenu("Camera-Control/Mouse Torque")]
[RequireComponent(typeof(Rigidbody))]
 
/**
 * MouseTorque.cs - a mouselook implementation using torque
 * 
 * All of the smooth mouse look scripts I found used input averages over the last several frames, which
 * ends up not being all that smooth really.  This camera script uses rotational forces (torques) to
 * rotate the camera in response to mouse movement.
 * 
 * The caveat of this camera is that it is possible for the camera to become slightly unstable.  Use
 * the rigidbody properties (mainly angular drag) as well as the correctiveStrength variable to affect
 * the stability of the rotations.
 * 
 * Note: make sure that "Use Gravity" is unchecked in the rigidbody settings.
 * Note: use an angular drag of about 5 or 6.
 * Note: setting a sensitivity value to a negative value inverts that axis.
 * 
 * Author: Robert Grant
 */
public class MouseTorque : MonoBehaviour {
    /** Controls how sensitive the horizontal axis is. */
    public float horizontalSensitivity = 30;
 
    /** Controls how sensitive the vertical axis is. */
    public float verticalSensitivity = 30;
 
    /** Controls how strongly the camera tries to keep itself upright. */
    public float correctiveStrength = 20;
 
    void FixedUpdate () {
        rigidbody.AddTorque(0, Input.GetAxis("Mouse X") * horizontalSensitivity, 0);
        rigidbody.AddRelativeTorque(Input.GetAxis("Mouse Y") * verticalSensitivity, 0, 0);
 
        // Adding the two forces above creates some wobble that causes the camera to become
        // less than perfectly upright.  Set the corrective strength to zero to see what I'm
        // talking about.  The following lines help keep the camera upright.
        Vector3 properRight = Quaternion.Euler(0, 0, -transform.localEulerAngles.z) * transform.right;
        Vector3 uprightCorrection = Vector3.Cross(transform.right, properRight);
        rigidbody.AddRelativeTorque(uprightCorrection * correctiveStrength);
    }
}
}
