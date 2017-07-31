// Original url: http://wiki.unity3d.com/index.php/MoveObject
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/AnimationControllers/MoveObject.cs
// File based on original modification date of: 27 June 2016, at 02:08. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.AnimationControllers
{

Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 Example 
4 JavaScript - MoveObject.js 
5 C# - MoveObject.cs 
6 Usage Example in C# 

Description Simple routines for moving an object from point A to point B (either over a specified time, or at a certain rate) and rotating by a given number of degrees over time. For more advanced animation, try AniMate, Tween, or iTween. 
Usage READ THIS ENTIRE PARAGRAPH AND DO EVERYTHING IT SAYS! Put this script in your Plugins folder; this way it can be easily used from C# or Boo. The script should be named "MoveObject". The script must be attached to some object in the scene, such as an empty object used for game manager scripts. You then use the coroutines by calling MoveObject.use.Translation or MoveObject.use.Rotation. Since they're coroutines, just call them when needed; no need for Update. 

function Translation (transform : Transform, startPosition : Vector3, endPosition : Vector3, value : float, moveType : MoveType) : IEnumerator 
Moves transform from startPosition to endPosition. value is either the number of seconds it takes to complete the translation (if moveType is set to MoveType.Time) or the number of units per second that the transform will move at (if moveType is set to MoveType.Speed). 

function Translation (transform : Transform, endPosition : Vector3, value : float, moveType : MoveType) : IEnumerator 
Same as above, except the starting position is whatever position the transform happens to be at when you start the routine, and endPosition is relative to the starting position. So using Vector3.right*2.0 would move the transform 2 units to the right. 

function Rotation (transform : Transform, degrees : Vector3, time : float) : IEnumerator 
Rotates transform by degrees over time seconds. The degrees is a Vector3 so you can specify the axis. i.e., Vector3(0.0, 0.0, 180.0) or Vector3.forward*180.0 would rotate 180 degrees along the Z axis. 
Example function Start () {
	// Starting from the origin, move this object to 5 units along X by 10 units along Z, at 2.5 units per second
	yield MoveObject.use.Translation(transform, Vector3.zero, Vector3(5.0, 0.0, 10.0), 2.5, MoveType.Speed);
	// When that's done, simultaneously move up one unit and flip 180 degrees along the Z axis, doing both in half a second
	MoveObject.use.Translation(transform, Vector3.up, .5, MoveType.Time);
	MoveObject.use.Rotation(transform, Vector3.forward * 180.0, .5);
}

JavaScript - MoveObject.js enum MoveType {Time, Speed}
static var use : MoveObject;
 
function Awake () {
	if (use) {
		Debug.LogWarning("Only one instance of the MoveObject script in a scene is allowed");
		return;
	}
	use = this;
}
 
function Translation (thisTransform : Transform, endPos : Vector3, value : float, moveType : MoveType) {
	yield Translation (thisTransform, thisTransform.position, thisTransform.position + endPos, value, moveType);
}
 
function Translation (thisTransform : Transform, startPos : Vector3, endPos : Vector3, value : float, moveType : MoveType) {
	var rate = (moveType == MoveType.Time)? 1.0/value : 1.0/Vector3.Distance(startPos, endPos) * value;
	var t = 0.0;
	while (t < 1.0) {
		t += Time.deltaTime * rate;
		thisTransform.position = Vector3.Lerp(startPos, endPos, t);
		yield; 
	}
}
 
function Rotation (thisTransform : Transform, degrees : Vector3, time : float) {
	var startRotation = thisTransform.rotation;
	var endRotation = thisTransform.rotation * Quaternion.Euler(degrees);
	var rate = 1.0/time;
	var t = 0.0;
	while (t < 1.0) {
		t += Time.deltaTime * rate;
		thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
		yield;
	}
}C# - MoveObject.cs This version is provided by Nathan St. Pierre. 
using UnityEngine;
using System.Collections;
 
public class MoveObject : MonoBehaviour
{
 
    public enum MoveType { Time, Speed }
    public static MoveObject use = null;
 
    void Awake()
    {
        if (use)
        {
            Debug.LogWarning("Only one instance of the MoveObject script in a scene is allowed");
            return;
        }
        use = this;
    }
 
    public IEnumerator TranslateTo(Transform thisTransform, Vector3 endPos, float value, MoveType moveType)
    {
        yield return Translation(thisTransform, thisTransform.position, endPos, value, moveType);
    }
 
    public IEnumerator Translation(Transform thisTransform, Vector3 endPos, float value, MoveType moveType)
    {
        yield return Translation(thisTransform, thisTransform.position, thisTransform.position + endPos, value, moveType);
    }
 
    public IEnumerator Translation(Transform thisTransform, Vector3 startPos, Vector3 endPos, float value, MoveType moveType)
    {
        float rate = (moveType == MoveType.Time) ? 1.0f / value : 1.0f / Vector3.Distance(startPos, endPos) * value;
        float t = 0.0f;
        while (t < 1.0)
        {
            t += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
    }
 
    public IEnumerator Rotation(Transform thisTransform, Vector3 degrees, float time)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(degrees);
        float rate = 1.0f / time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
    }
}Usage Example in C# // move object up one unit over 0.5 seconds
yield return StartCoroutine(MoveObject.use.Translation(gameObject.transform, Vector3.up, 0.5f, MoveObject.MoveType.Time));
}
