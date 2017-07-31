// Original url: http://wiki.unity3d.com/index.php/SmoothLookFrame
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/SmoothLookFrame.cs
// File based on original modification date of: 21 February 2012, at 04:31. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Author: David O'Donoghue (a.k.a Trooper from ODD Games) 
Contents [hide] 
1 Description 
2 C# Script 
3 C# Script (Smooth Rotate) 
4 Javascript 

Description Looks at a target whilst keeping another target in view. 
C# Script using UnityEngine;
 
public class SmoothLookFrame : MonoBehaviour {
 
	public Transform lookAtTarget;
	public Transform frameTarget;
	public float distance = 10.0f;
	public float height = 5.0f;
	public float damping = 2.0f;
 
	private Vector3 direction;
	private Vector3 wantedPosition;
 
	void FixedUpdate () {
 
		if (!lookAtTarget || !frameTarget)
			return;
 
		direction = (frameTarget.position - lookAtTarget.position);
 
		wantedPosition = frameTarget.position + (direction.normalized * distance);
		wantedPosition.y = wantedPosition.y + height;
 
		transform.position = Vector3.Lerp(transform.position, wantedPosition, damping * Time.deltaTime);
 
		transform.LookAt (lookAtTarget);
 
	}
 
}C# Script (Smooth Rotate) using UnityEngine;
public class SmoothLookFrame : MonoBehaviour {
 
    public Transform lookAtTarget;
    public Transform frameTarget;
    public float distance = 10.0f;
    public float height = 5.0f;
    public float damping = 2.0f;
 
    private Vector3 direction;
    private Vector3 wantedPosition;
 
    void Update () {
        if (!lookAtTarget || !frameTarget)
            return;
 
        direction = (frameTarget.position - lookAtTarget.position);
        wantedPosition = frameTarget.position + (direction.normalized * distance);
 
        wantedPosition.y = wantedPosition.y + height;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, damping * Time.deltaTime);
 
        Quaternion rotate = Quaternion.LookRotation(lookAtTarget.position - transform.position);
	    transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * damping);
    }
}Javascript var lookAtTarget : Transform;
var frameTarget : Transform;
var distance : float = 10.0;
var height : float = 10.0;
var damping : float = 2.0;
 
private var direction : Vector3;
private var wantedPosition : Vector3;
 
function FixedUpdate () {
 
	if (!lookAtTarget || !frameTarget)
			return;
 
		direction = (frameTarget.position - lookAtTarget.position);
 
		wantedPosition = frameTarget.position + (direction.normalized * distance);
		wantedPosition.y = wantedPosition.y + height;
 
		transform.position = Vector3.Lerp(transform.position, wantedPosition, damping * Time.deltaTime);
 
		transform.LookAt (lookAtTarget);
}
}
