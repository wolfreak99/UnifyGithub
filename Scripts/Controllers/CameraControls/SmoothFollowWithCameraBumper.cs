// Original url: http://wiki.unity3d.com/index.php/SmoothFollowWithCameraBumper
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/SmoothFollowWithCameraBumper.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Contents [hide] 
1 Description 
2 Contributions 
3 Usage 
4 JavaScript - SmoothFollowWithCameraBumper.js 
5 C# - SmoothFollowWithCameraBumper.cs 

Description Designed to prevent the camera from passing through objects while following the target. A ray is cast out the rear of the target and if it collides, the camera will adjust to prevent passage into the object. 
Contributions (Created CSharp Version) 11/2010: Daniel P. Rossi (DR9885) 
(Added Check for Ray-casting into User) 11/2010: Daniel P. Rossi (DR9885) 
Usage Place this script onto a camera and select camera target in Inspector. Tweak bumper settings to achieve desired affect. 
JavaScript - SmoothFollowWithCameraBumper.js var target : Transform;
var distance : float = 3.0;
var height : float = 1.0;
var damping : float = 5.0;
var smoothRotation : boolean = true;
var rotationDamping : float = 10.0;
 
var targetLookAtOffset : Vector3; 		// allows offsetting of camera lookAt, very useful for low bumper heights
 
var bumperDistanceCheck : float = 2.5;	// length of bumper ray
var bumperCameraHeight : float = 1.0; 	// adjust camera height while bumping
var bumperRayOffset : Vector3; 			// allows offset of the bumper ray from target origin
 
// If the target moves, the camera should child the target to allow for smoother movement. DR
function Awake()
{
    camera.transform.parent = target;
}
 
function FixedUpdate() {
 
	var wantedPosition = target.TransformPoint(0, height, -distance);
 
	// check to see if there is anything behind the target
	var hit : RaycastHit;
	var back = target.transform.TransformDirection(-1 * Vector3.forward);	
 
	// cast the bumper ray out from rear and check to see if there is anything behind
	if (Physics.Raycast(target.TransformPoint(bumperRayOffset), back, hit, bumperDistanceCheck) 
              && hit.transform != target) { // ignore ray-casts that hit the user. DR
        // clamp wanted position to hit position
		wantedPosition.x = hit.point.x;
		wantedPosition.z = hit.point.z;
		wantedPosition.y = Mathf.Lerp(hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * damping);
	} 
 
	transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
 
	var lookPosition : Vector3 = target.TransformPoint(targetLookAtOffset);
 
	if (smoothRotation) {
		var wantedRotation : Quaternion = Quaternion.LookRotation(lookPosition - transform.position, target.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
	} else {
		transform.rotation = Quaternion.LookRotation(lookPosition - transform.position, target.up);
	}
}

C# - SmoothFollowWithCameraBumper.cs //(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 
 
using UnityEngine;
using System.Collections;
 
public class SmoothCameraWithBumper : MonoBehaviour 
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float distance = 3.0f;
    [SerializeField] private float height = 1.0f;
    [SerializeField] private float damping = 5.0f;
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float rotationDamping = 10.0f;
 
    [SerializeField] private Vector3 targetLookAtOffset; // allows offsetting of camera lookAt, very useful for low bumper heights
 
    [SerializeField] private float bumperDistanceCheck = 2.5f; // length of bumper ray
    [SerializeField] private float bumperCameraHeight = 1.0f; // adjust camera height while bumping
    [SerializeField] private Vector3 bumperRayOffset; // allows offset of the bumper ray from target origin
 
    /// <Summary>
    /// If the target moves, the camera should child the target to allow for smoother movement. DR
    /// </Summary>
    private void Awake()
    {
        camera.transform.parent = target;
    }
 
    private void FixedUpdate() 
    {
        Vector3 wantedPosition = target.TransformPoint(0, height, -distance);
 
        // check to see if there is anything behind the target
        RaycastHit hit;
        Vector3 back = target.transform.TransformDirection(-1 * Vector3.forward); 
 
        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(target.TransformPoint(bumperRayOffset), back, out hit, bumperDistanceCheck)
            && hit.transform != target) // ignore ray-casts that hit the user. DR
        {
            // clamp wanted position to hit position
            wantedPosition.x = hit.point.x;
            wantedPosition.z = hit.point.z;
            wantedPosition.y = Mathf.Lerp(hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * damping);
        } 
 
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
 
        Vector3 lookPosition = target.TransformPoint(targetLookAtOffset);
 
        if (smoothRotation)
        {
            Quaternion wantedRotation = Quaternion.LookRotation(lookPosition - transform.position, target.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
        } 
        else 
            transform.rotation = Quaternion.LookRotation(lookPosition - transform.position, target.up);
    }
}
}
