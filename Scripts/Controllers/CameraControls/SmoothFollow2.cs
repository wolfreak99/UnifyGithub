// Original url: http://wiki.unity3d.com/index.php/SmoothFollow2
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/SmoothFollow2.cs
// File based on original modification date of: 28 January 2012, at 20:43. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Author: Daniel 
Improved Version 
Author: Vasilis Christopoulos & Daniel Toliaferro 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - SmoothFollow2.js 
4 C# - SmoothFollow2.cs 
5 C# - SmoothFollow2.cs (Improved) 

Description This is designed to make a camera smoothly follow a ship in space. 
Usage Place this script onto a camera. 
JavaScript - SmoothFollow2.js var target : Transform;
var distance = 3.0;
var height = 3.0;
var damping = 5.0;
var smoothRotation = true;
var rotationDamping = 10.0;

function Update () {
	var wantedPosition = target.TransformPoint(0, height, -distance);
	transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * damping);

	if (smoothRotation) {
		var wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
		transform.rotation = Quaternion.Slerp (transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
	}

	else transform.LookAt (target, target.up);
}
C# - SmoothFollow2.cs using UnityEngine;
using System.Collections;

public class SmoothFollow2 : MonoBehaviour {
	public Transform target;
	public float distance = 3.0f;
	public float height = 3.0f;
	public float damping = 5.0f;
	public bool smoothRotation = true;
	public float rotationDamping = 10.0f;
	
	void Update () {
		Vector3 wantedPosition = target.TransformPoint(0, height, -distance);
		transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * damping);

		if (smoothRotation) {
			Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
			transform.rotation = Quaternion.Slerp (transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
		}
		
		else transform.LookAt (target, target.up);
	}
}
C# - SmoothFollow2.cs (Improved) using UnityEngine;
using System.Collections;

        public class SmoothFollow2 : MonoBehaviour {
        public Transform target;
        public float distance = 3.0f;
        public float height = 3.0f;
        public float damping = 5.0f;
        public bool smoothRotation = true;
        public bool followBehind = true;
        public float rotationDamping = 10.0f;

        void Update () {
               Vector3 wantedPosition;
               if(followBehind)
                       wantedPosition = target.TransformPoint(0, height, -distance);
               else
                       wantedPosition = target.TransformPoint(0, height, distance);
     
               transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * damping);

               if (smoothRotation) {
                       Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
                       transform.rotation = Quaternion.Slerp (transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
               }
               else transform.LookAt (target, target.up);
         }
}
}
