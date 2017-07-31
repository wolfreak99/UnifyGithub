// Original url: http://wiki.unity3d.com/index.php/MouseOrbitImproved
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/MouseOrbitImproved.cs
// File based on original modification date of: 14 April 2015, at 09:11. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: Veli V Author Boo: [highway900] 
Contents [hide] 
1 Description 
2 Code Javascript 
3 Code C# 
4 Code Boo 

DescriptionImproved version of the original MouseOrbit script. Zooms with the mousewheel and uses linecast to make sure that object isn't behind anything. 
Feel free to improve it further! 
Code Javascriptvar target : Transform;
var distance = 10.0;
 
var xSpeed = 250.0;
var ySpeed = 120.0;
 
var yMinLimit = -20;
var yMaxLimit = 80;
 
var distanceMin = 3;
var distanceMax = 15;
 
private var x = 0.0;
private var y = 0.0;
 
 
@script AddComponentMenu("Camera-Control/Mouse Orbit")
 
function Start () {
    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;
 
	// Make the rigid body not change rotation
   	if (rigidbody)
		rigidbody.freezeRotation = true;
}
 
function LateUpdate () {
    if (target) {
        x += Input.GetAxis("Mouse X") * xSpeed * distance* 0.02;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
 
 		y = ClampAngle(y, yMinLimit, yMaxLimit);
 
		var rotation = Quaternion.Euler(y, x, 0);
 
		distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
 
		var hit : RaycastHit;
		if (Physics.Linecast (target.position, transform.position, hit)) {
				distance -=  hit.distance;
		}
 
        var position = rotation * Vector3(0.0, 0.0, -distance) + target.position;
 
        transform.rotation = rotation;
        transform.position = position;
 
	}
 
}
 
 
static function ClampAngle (angle : float, min : float, max : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return Mathf.Clamp (angle, min, max);
}Code C#using UnityEngine;
using System.Collections;
 
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {
 
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
 
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
 
    public float distanceMin = .5f;
    public float distanceMax = 15f;
 
    private Rigidbody rigidbody;
 
    float x = 0.0f;
    float y = 0.0f;
 
    // Use this for initialization
    void Start () 
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
 
        rigidbody = GetComponent<Rigidbody>();
 
        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }
 
    void LateUpdate () 
    {
        if (target) 
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
 
            y = ClampAngle(y, yMinLimit, yMaxLimit);
 
            Quaternion rotation = Quaternion.Euler(y, x, 0);
 
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
 
            RaycastHit hit;
            if (Physics.Linecast (target.position, transform.position, out hit)) 
            {
                distance -=  hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;
 
            transform.rotation = rotation;
            transform.position = position;
        }
    }
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}Code Booimport UnityEngine
 
[AddComponentMenu("Camera-Control/Mouse Orbit")]
class MouseOribitImproved (MonoBehaviour): 
 
    public target as Transform
    public distance as single = 5.0
    public xSpeed as single = 120.0
    public ySpeed as single= 120.0
 
    public yMinLimit as single = -80.0
    public yMaxLimit as single = 120.0
 
    public distanceMin as single = 0.5
    public distanceMax as single = 15.0
 
    private x as single  = 0.0
    private y as single  = 0.0
 
    def Start ():
        angles as Vector3 = transform.eulerAngles
        x = angles.y
        y = angles.x
 
        // Make the rigid body not change rotation
        if (rigidbody):
            rigidbody.freezeRotation = true
 
    def LateUpdate ():
 
        if Input.GetMouseButton(2):
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f
            y -= Input.GetAxis("Mouse Y") * ySpeed * distance * 0.02f
 
        if target:
            y = ClampAngle(y, yMinLimit, yMaxLimit)
 
            rotation as Quaternion = Quaternion.Euler(y, x, 0)
 
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax)
 
            negDistance as Vector3 = Vector3(0.0f, 0.0f, -distance)
            position as Vector3 = rotation * negDistance + target.position
 
            transform.rotation = rotation
            transform.position = position
 
    def ClampAngle(angle as single, min as single, max as single):
        if (angle < -360.0f):
            angle += 360.0f
        if (angle > 360.0f):
            angle -= 360f
        return Mathf.Clamp(angle, min, max)
}
