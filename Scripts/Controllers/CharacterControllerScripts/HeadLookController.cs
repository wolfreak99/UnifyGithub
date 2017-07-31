// Original url: http://wiki.unity3d.com/index.php/HeadLookController
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/HeadLookController.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: runevision 
Contents [hide] 
1 Description 
2 Usage 
3 Troubleshooting 
4 C# - HeadLookController.cs 
5 JavaScript - HeadLookController.js 

DescriptionThis script can be used to make a character look towards a specified point in space, smoothly turning towards it with for example the eyes, head, upper body, or whatever is specified. Multiple ranges of bones can be specified, each with different settings for responsiveness, angle constraints etc. 
UsageDrop this script onto a GameObject; typically the root. In the inspector, set the length of the Segments array and specify the settings for each segment. Optionally a list of joints to not be affected can be specified in the Non Affected Joints array. This is to make certain transforms keep their original orientation even though they are children of transforms specified in one of the segments. For example you can have the upper body bend, but set the joints for the shoulders to not be affected (or to be only affected a little). 
There is more information and a demo video at the Unity Blog here: http://blogs.unity3d.com/2009/07/10/just-looking-around/ 
TroubleshootingPer default, the character needs to have animations that drive all the bones that the script modifies. If a body part turns or twists around out of control, it might be because there is currently no animation playing that has curves for those bones. In that case, the setting Override Animation can be turned on. This make the affected bones ignore any animation that may be affecting them and instead they are set in each frame to be identical to how they were when the object was instantiated. Normally this is not a good idea to turn on. For example, if an idle animation has a subtle breathing animation of the upper body and head, this would be overridden. However, it can be useful for enabling the use of the Head Look Controller on models that do not have any animation on the affected bones at all. 
C# - HeadLookController.csusing UnityEngine;
using System.Collections;
 
[System.Serializable]
public class BendingSegment {
    public Transform firstTransform;
    public Transform lastTransform;
    public float thresholdAngleDifference = 0;
    public float bendingMultiplier = 0.6f;
    public float maxAngleDifference = 30;
    public float maxBendingAngle = 80;
    public float responsiveness = 5;
    internal float angleH;
    internal float angleV;
    internal Vector3 dirUp;
    internal Vector3 referenceLookDir;
    internal Vector3 referenceUpDir;
    internal int chainLength;
    internal Quaternion[] origRotations;
}
 
[System.Serializable]
public class NonAffectedJoints {
    public Transform joint;
    public float effect = 0;
}
 
public class HeadLookController : MonoBehaviour {
 
    public Transform rootNode;
    public BendingSegment[] segments;
    public NonAffectedJoints[] nonAffectedJoints;
    public Vector3 headLookVector = Vector3.forward;
    public Vector3 headUpVector = Vector3.up;
    public Vector3 target = Vector3.zero;
    public float effect = 1;
    public bool overrideAnimation = false;
 
    void Start () {
        if (rootNode == null) {
            rootNode = transform;
        }
 
        // Setup segments
        foreach (BendingSegment segment in segments) {
            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);
            segment.referenceLookDir =
                parentRotInv * rootNode.rotation * headLookVector.normalized;
            segment.referenceUpDir =
                parentRotInv * rootNode.rotation * headUpVector.normalized;
            segment.angleH = 0;
            segment.angleV = 0;
            segment.dirUp = segment.referenceUpDir;
 
            segment.chainLength = 1;
            Transform t = segment.lastTransform;
            while (t != segment.firstTransform && t != t.root) {
                segment.chainLength++;
                t = t.parent;
            }
 
            segment.origRotations = new Quaternion[segment.chainLength];
            t = segment.lastTransform;
            for (int i=segment.chainLength-1; i>=0; i--) {
                segment.origRotations[i] = t.localRotation;
                t = t.parent;
            }
        }
    }
 
    void LateUpdate () {
        if (Time.deltaTime == 0)
            return;
 
        // Remember initial directions of joints that should not be affected
        Vector3[] jointDirections = new Vector3[nonAffectedJoints.Length];
        for (int i=0; i<nonAffectedJoints.Length; i++) {
            foreach (Transform child in nonAffectedJoints[i].joint) {
                jointDirections[i] = child.position - nonAffectedJoints[i].joint.position;
                break;
            }
        }
 
        // Handle each segment
        foreach (BendingSegment segment in segments) {
            Transform t = segment.lastTransform;
            if (overrideAnimation) {
                for (int i=segment.chainLength-1; i>=0; i--) {
                    t.localRotation = segment.origRotations[i];
                    t = t.parent;
                }
            }
 
            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);
 
            // Desired look direction in world space
            Vector3 lookDirWorld = (target - segment.lastTransform.position).normalized;
 
            // Desired look directions in neck parent space
            Vector3 lookDirGoal = (parentRotInv * lookDirWorld);
 
            // Get the horizontal and vertical rotation angle to look at the target
            float hAngle = AngleAroundAxis(
                segment.referenceLookDir, lookDirGoal, segment.referenceUpDir
            );
 
            Vector3 rightOfTarget = Vector3.Cross(segment.referenceUpDir, lookDirGoal);
 
            Vector3 lookDirGoalinHPlane =
                lookDirGoal - Vector3.Project(lookDirGoal, segment.referenceUpDir);
 
            float vAngle = AngleAroundAxis(
                lookDirGoalinHPlane, lookDirGoal, rightOfTarget
            );
 
            // Handle threshold angle difference, bending multiplier,
            // and max angle difference here
            float hAngleThr = Mathf.Max(
                0, Mathf.Abs(hAngle) - segment.thresholdAngleDifference
            ) * Mathf.Sign(hAngle);
 
            float vAngleThr = Mathf.Max(
                0, Mathf.Abs(vAngle) - segment.thresholdAngleDifference
            ) * Mathf.Sign(vAngle);
 
            hAngle = Mathf.Max(
                Mathf.Abs(hAngleThr) * Mathf.Abs(segment.bendingMultiplier),
                Mathf.Abs(hAngle) - segment.maxAngleDifference
            ) * Mathf.Sign(hAngle) * Mathf.Sign(segment.bendingMultiplier);
 
            vAngle = Mathf.Max(
                Mathf.Abs(vAngleThr) * Mathf.Abs(segment.bendingMultiplier),
                Mathf.Abs(vAngle) - segment.maxAngleDifference
            ) * Mathf.Sign(vAngle) * Mathf.Sign(segment.bendingMultiplier);
 
            // Handle max bending angle here
            hAngle = Mathf.Clamp(hAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
            vAngle = Mathf.Clamp(vAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
 
            Vector3 referenceRightDir =
                Vector3.Cross(segment.referenceUpDir, segment.referenceLookDir);
 
            // Lerp angles
            segment.angleH = Mathf.Lerp(
                segment.angleH, hAngle, Time.deltaTime * segment.responsiveness
            );
            segment.angleV = Mathf.Lerp(
                segment.angleV, vAngle, Time.deltaTime * segment.responsiveness
            );
 
            // Get direction
            lookDirGoal = Quaternion.AngleAxis(segment.angleH, segment.referenceUpDir)
                * Quaternion.AngleAxis(segment.angleV, referenceRightDir)
                * segment.referenceLookDir;
 
            // Make look and up perpendicular
            Vector3 upDirGoal = segment.referenceUpDir;
            Vector3.OrthoNormalize(ref lookDirGoal, ref upDirGoal);
 
            // Interpolated look and up directions in neck parent space
            Vector3 lookDir = lookDirGoal;
            segment.dirUp = Vector3.Slerp(segment.dirUp, upDirGoal, Time.deltaTime*5);
            Vector3.OrthoNormalize(ref lookDir, ref segment.dirUp);
 
            // Look rotation in world space
            Quaternion lookRot = (
                (parentRot * Quaternion.LookRotation(lookDir, segment.dirUp))
                * Quaternion.Inverse(
                    parentRot * Quaternion.LookRotation(
                        segment.referenceLookDir, segment.referenceUpDir
                    )
                )
            );
 
            // Distribute rotation over all joints in segment
            Quaternion dividedRotation =
                Quaternion.Slerp(Quaternion.identity, lookRot, effect / segment.chainLength);
            t = segment.lastTransform;
            for (int i=0; i<segment.chainLength; i++) {
                t.rotation = dividedRotation * t.rotation;
                t = t.parent;
            }
        }
 
        // Handle non affected joints
        for (int i=0; i<nonAffectedJoints.Length; i++) {
            Vector3 newJointDirection = Vector3.zero;
 
            foreach (Transform child in nonAffectedJoints[i].joint) {
                newJointDirection = child.position - nonAffectedJoints[i].joint.position;
                break;
            }
 
            Vector3 combinedJointDirection = Vector3.Slerp(
                jointDirections[i], newJointDirection, nonAffectedJoints[i].effect
            );
 
            nonAffectedJoints[i].joint.rotation = Quaternion.FromToRotation(
                newJointDirection, combinedJointDirection
            ) * nonAffectedJoints[i].joint.rotation;
        }
    }
 
    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis) {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);
 
        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);
 
        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
}JavaScript - HeadLookController.js #pragma strict
 
class BendingSegment {
	var firstTransform : Transform;
	var lastTransform : Transform ;
	var thresholdAngleDifference = 0.0;
	var bendingMultiplier = 0.6;
	var maxAngleDifference = 30.0;
	var maxBendingAngle = 80.0;
	var responsiveness = 5.0;
	internal var angleH : float;
	internal var angleV : float;
	internal var dirUp : Vector3;
	internal var referenceLookDir : Vector3;
	internal var referenceUpDir : Vector3;
	internal var chainLength : int;
	internal var origRotations : Quaternion[];
}
 
class NonAffectedJoints {
	var joint : Transform;
	var effect = 0.0;
}
 
var rootNode : Transform;
var segments : BendingSegment[];
var nonAffectedJoints : NonAffectedJoints[];
var headLookVector = Vector3.forward;
var headUpVector = Vector3.up;
var target = Vector3.zero;
var effect = 1.0;
var overrideAnimation = false;
 
function Start ()
{
	if (rootNode == null) {
		rootNode = transform;
	}
 
	// Setup segments
	for (var segment : BendingSegment in segments) {
		var parentRot : Quaternion = segment.firstTransform.parent.rotation;
		var parentRotInv : Quaternion = Quaternion.Inverse(parentRot);
		segment.referenceLookDir =
			parentRotInv * rootNode.rotation * headLookVector.normalized;
		segment.referenceUpDir =
			parentRotInv * rootNode.rotation * headUpVector.normalized;
		segment.angleH = 0.0;
		segment.angleV = 0.0;
		segment.dirUp = segment.referenceUpDir;
 
		segment.chainLength = 1;
		var t : Transform = segment.lastTransform;
		while (t != segment.firstTransform && t != t.root) {
			segment.chainLength++;
			t = t.parent;
		}
 
		segment.origRotations = new Quaternion[segment.chainLength];
		t = segment.lastTransform;
		for (var i=segment.chainLength-1; i>=0; i--) {
			segment.origRotations[i] = t.localRotation;
			t = t.parent;
		}
	}
}
 
function LateUpdate ()
{
	if (Time.deltaTime == 0)
		return;
 
	// Remember initial directions of joints that should not be affected
	var jointDirections : Vector3[] = new Vector3[nonAffectedJoints.Length];
	for (var i=0; i<nonAffectedJoints.Length; i++) {
		for (var child : Transform in nonAffectedJoints[i].joint) {
			jointDirections[i] = child.position - nonAffectedJoints[i].joint.position;
			break;
		}
	}
 
	// Handle each segment
	for (var segment : BendingSegment in segments) {
		var t : Transform = segment.lastTransform;
		if (overrideAnimation) {
			for (i=segment.chainLength-1; i>=0; i--) {
				t.localRotation = segment.origRotations[i];
				t = t.parent;
			}
		}
 
		var parentRot : Quaternion = segment.firstTransform.parent.rotation;
		var parentRotInv : Quaternion = Quaternion.Inverse(parentRot);
 
		// Desired look direction in world space
		var lookDirWorld : Vector3 = (target - segment.lastTransform.position).normalized;
 
		// Desired look directions in neck parent space
		var lookDirGoal : Vector3 = (parentRotInv * lookDirWorld);
 
		// Get the horizontal and vertical rotation angle to look at the target
		var hAngle : float = AngleAroundAxis(
			segment.referenceLookDir, lookDirGoal, segment.referenceUpDir
		);
 
		var rightOfTarget : Vector3 = Vector3.Cross(segment.referenceUpDir, lookDirGoal);
 
		var lookDirGoalinHPlane : Vector3 =
			lookDirGoal - Vector3.Project(lookDirGoal, segment.referenceUpDir);
 
		var vAngle : float  = AngleAroundAxis(
			lookDirGoalinHPlane, lookDirGoal, rightOfTarget
		);
 
		// Handle threshold angle difference, bending multiplier,
		// and max angle difference here
		var hAngleThr : float = Mathf.Max(
			0, Mathf.Abs(hAngle) - segment.thresholdAngleDifference
		) * Mathf.Sign(hAngle);
 
		var vAngleThr : float = Mathf.Max(
			0, Mathf.Abs(vAngle) - segment.thresholdAngleDifference
		) * Mathf.Sign(vAngle);
 
		hAngle = Mathf.Max(
			Mathf.Abs(hAngleThr) * Mathf.Abs(segment.bendingMultiplier),
			Mathf.Abs(hAngle) - segment.maxAngleDifference
		) * Mathf.Sign(hAngle) * Mathf.Sign(segment.bendingMultiplier);
 
		vAngle = Mathf.Max(
			Mathf.Abs(vAngleThr) * Mathf.Abs(segment.bendingMultiplier),
			Mathf.Abs(vAngle) - segment.maxAngleDifference
		) * Mathf.Sign(vAngle) * Mathf.Sign(segment.bendingMultiplier);
 
		// Handle max bending angle here
		hAngle = Mathf.Clamp(hAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
		vAngle = Mathf.Clamp(vAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
 
		var referenceRightDir : Vector3 =
			Vector3.Cross(segment.referenceUpDir, segment.referenceLookDir);
 
		// Lerp angles
		segment.angleH = Mathf.Lerp(
			segment.angleH, hAngle, Time.deltaTime * segment.responsiveness
		);
		segment.angleV = Mathf.Lerp(
			segment.angleV, vAngle, Time.deltaTime * segment.responsiveness
		);
 
		// Get direction
		lookDirGoal = Quaternion.AngleAxis(segment.angleH, segment.referenceUpDir)
			* Quaternion.AngleAxis(segment.angleV, referenceRightDir)
			* segment.referenceLookDir;
 
		// Make look and up perpendicular
		var upDirGoal : Vector3 = segment.referenceUpDir;
		Vector3.OrthoNormalize(lookDirGoal, upDirGoal);
 
		// Interpolated look and up directions in neck parent space
		var lookDir : Vector3 = lookDirGoal;
		segment.dirUp = Vector3.Slerp(segment.dirUp, upDirGoal, Time.deltaTime*5);
		Vector3.OrthoNormalize(lookDir, segment.dirUp);
 
		// Look rotation in world space
		var lookRot : Quaternion = (
			(parentRot * Quaternion.LookRotation(lookDir, segment.dirUp))
			* Quaternion.Inverse(
				parentRot * Quaternion.LookRotation(
					segment.referenceLookDir, segment.referenceUpDir
				)
			)
		);
 
		// Distribute rotation over all joints in segment
		var dividedRotation : Quaternion =
			Quaternion.Slerp(Quaternion.identity, lookRot, effect / segment.chainLength);
		t = segment.lastTransform;
		for (i=0; i<segment.chainLength; i++) {
			t.rotation = dividedRotation * t.rotation;
			t = t.parent;
		}
	}
 
	// Handle non affected joints
	for (i=0; i<nonAffectedJoints.Length; i++) {
		var newJointDirection : Vector3 = Vector3.zero;
 
		for (var child : Transform in nonAffectedJoints[i].joint) {
			newJointDirection = child.position - nonAffectedJoints[i].joint.position;
			break;
		}
 
		var combinedJointDirection : Vector3 = Vector3.Slerp(
			jointDirections[i], newJointDirection, nonAffectedJoints[i].effect
		);
 
		nonAffectedJoints[i].joint.rotation = Quaternion.FromToRotation(
			newJointDirection, combinedJointDirection
		) * nonAffectedJoints[i].joint.rotation;
	}
}
 
// The angle between dirA and dirB around axis
static function AngleAroundAxis (dirA : Vector3, dirB : Vector3, axis : Vector3)
{
	// Project A and B onto the plane orthogonal target axis
	dirA = dirA - Vector3.Project(dirA, axis);
	dirB = dirB - Vector3.Project(dirB, axis);
 
	// Find (positive) angle between A and B
	var angle : float = Vector3.Angle(dirA, dirB);
 
	// Return angle multiplied with 1 or -1
	return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
}
}
