// Original url: http://wiki.unity3d.com/index.php/DetectTouchMovement
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/DetectTouchMovement.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Author: Caue Rego (cawas) 
Description Simple script to detect pinch and/or turning with 2 fingers. 
Usage Easier to explain by example. Attach this to the camera: 
void LateUpdate() {
	float pinchAmount = 0;
	Quaternion desiredRotation = transform.rotation;
 
	DetectTouchMovement.Calculate();
 
	if (Mathf.Abs(DetectTouchMovement.pinchDistanceDelta) > 0) { // zoom
		pinchAmount = DetectTouchMovement.pinchDistanceDelta;
	}
 
	if (Mathf.Abs(DetectTouchMovement.turnAngleDelta) > 0) { // rotate
		Vector3 rotationDeg = Vector3.zero;
		rotationDeg.z = -DetectTouchMovement.turnAngleDelta;
		desiredRotation *= Quaternion.Euler(rotationDeg);
	}
 
 
	// not so sure those will work:
	transform.rotation = desiredRotation;
	transform.position += Vector3.forward * pinchAmount;
}DetectTouchMovement.cs using UnityEngine;
using System.Collections;
 
public class DetectTouchMovement : MonoBehaviour {
	const float pinchTurnRatio = Mathf.PI / 2;
	const float minTurnAngle = 0;
 
	const float pinchRatio = 1;
	const float minPinchDistance = 0;
 
	const float panRatio = 1;
	const float minPanDistance = 0;
 
	/// <summary>
	///   The delta of the angle between two touch points
	/// </summary>
	static public float turnAngleDelta;
	/// <summary>
	///   The angle between two touch points
	/// </summary>
	static public float turnAngle;
 
	/// <summary>
	///   The delta of the distance between two touch points that were distancing from each other
	/// </summary>
	static public float pinchDistanceDelta;
	/// <summary>
	///   The distance between two touch points that were distancing from each other
	/// </summary>
	static public float pinchDistance;
 
	/// <summary>
	///   Calculates Pinch and Turn - This should be used inside LateUpdate
	/// </summary>
	static public void Calculate () {
		pinchDistance = pinchDistanceDelta = 0;
		turnAngle = turnAngleDelta = 0;
 
		// if two fingers are touching the screen at the same time ...
		if (Input.touchCount == 2) {
			Touch touch1 = Input.touches[0];
			Touch touch2 = Input.touches[1];
 
			// ... if at least one of them moved ...
			if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
				// ... check the delta distance between them ...
				pinchDistance = Vector2.Distance(touch1.position, touch2.position);
				float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
				                                      touch2.position - touch2.deltaPosition);
				pinchDistanceDelta = pinchDistance - prevDistance;
 
				// ... if it's greater than a minimum threshold, it's a pinch!
				if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance) {
					pinchDistanceDelta *= pinchRatio;
				} else {
					pinchDistance = pinchDistanceDelta = 0;
				}
 
				// ... or check the delta angle between them ...
				turnAngle = Angle(touch1.position, touch2.position);
				float prevTurn = Angle(touch1.position - touch1.deltaPosition,
				                       touch2.position - touch2.deltaPosition);
				turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);
 
				// ... if it's greater than a minimum threshold, it's a turn!
				if (Mathf.Abs(turnAngleDelta) > minTurnAngle) {
					turnAngleDelta *= pinchTurnRatio;
				} else {
					turnAngle = turnAngleDelta = 0;
				}
			}
		}
	}
 
	static private float Angle (Vector2 pos1, Vector2 pos2) {
		Vector2 from = pos2 - pos1;
		Vector2 to = new Vector2(1, 0);
 
		float result = Vector2.Angle( from, to );
		Vector3 cross = Vector3.Cross( from, to );
 
		if (cross.z > 0) {
			result = 360f - result;
		}
 
		return result;
	}
}
}
