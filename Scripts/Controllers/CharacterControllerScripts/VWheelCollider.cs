// Original url: http://wiki.unity3d.com/index.php/VWheelCollider
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/VWheelCollider.cs
// File based on original modification date of: 21 November 2014, at 16:33. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Venryx (venryx) 
Contents [hide] 
1 Description 
2 Main script - VWheelCollider 
2.1 C# - VWheelCollider.cs 
3 Dependencies - ClassExtensions, VPhysics 
3.1 C# - VExtras.cs 
4 Usage 
4.1 C# - VCarController.cs 

Description A basic replacement for the built-in WheelCollider component. One of the main things it lacks is a realistic friction model; all it currently does is apply forward and sideways forces to counteract the 'slip', which is itself just a measure of how much the wheel is moving. That said, it's working fine for basic movement, and can serve as the base for a more comprehensive replacement. 
Main script - VWheelCollider Create a new C# script named "VWheelCollider" and paste the following code: 
C# - VWheelCollider.cs using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
public class VWheelCollider : MonoBehaviour
{
	public float radius;
	public float forwardFriction;
	public float sidewaysFriction;
 
	public List<string> terrainLayers;
 
	public float motorTorque;
	public float steerAngle;
 
	GameObject carObj;
	Rigidbody carRigidbody;
 
	Vector3? lastPosition;
	float forwardSlip;
	float sidewaysSlip;
	bool onGround;
 
	void Awake()
	{
		carObj = gameObject.GetParents().First(a=>a.GetComponent<Rigidbody>());
		carRigidbody = carObj.GetComponent<Rigidbody>();
	}
 
	Vector3 lastFinalForward;
	void FixedUpdate()
	{
		// store the forward and sideways direction to improve performance
		var finalForward = Quaternion.AngleAxis(steerAngle, Vector3.up) * transform.forward;
		var finalRight = Quaternion.AngleAxis(steerAngle, Vector3.up) * transform.right;
 
		float nearestSurfaceAngle_angle = -1;
		float nearestSurfaceAngle_distance = float.MaxValue;
		for (var angleOffset = -90; angleOffset <= 90; angleOffset += 10)
		{
			RaycastHit hit2;
			if (VPhysics.Raycast(new Ray(transform.position, Quaternion.AngleAxis(angleOffset, transform.right) * -transform.up), out hit2, float.MaxValue, terrainLayers))
				if (hit2.distance < nearestSurfaceAngle_distance)
				{
					nearestSurfaceAngle_angle = angleOffset;
					nearestSurfaceAngle_distance = hit2.distance;
				}
		}
		if (nearestSurfaceAngle_distance != float.MaxValue)
		{
			finalForward = Quaternion.AngleAxis(nearestSurfaceAngle_angle, transform.right) * finalForward;
			if (nearestSurfaceAngle_distance <= radius * 1.1f)
				onGround = true;
			else
				onGround = false;
		}
		else
			onGround = false;
 
		CalculateSlips(finalForward, finalRight);
		ApplyForce(finalForward, finalRight);
 
		lastFinalForward = finalForward;
	}
	void CalculateSlips(Vector3 finalForward, Vector3 finalRight)
	{
		// calculate the wheel's linear velocity
		if (!lastPosition.HasValue)
			lastPosition = transform.position;
		Vector3 velocity = (transform.position - lastPosition.Value) / Time.deltaTime;
		lastPosition = transform.position;
 
		// calculate the forward and sideways velocity components relative to the wheel
		Vector3 forwardVelocity = Vector3.Dot(velocity, finalForward) * finalForward;
		Vector3 sidewaysVelocity = Vector3.Dot(velocity, finalRight) * finalRight;
 
		// calculate the slip velocities; note that these values are different from the standard slip calculation
		forwardSlip = Mathf.Sign(Vector3.Dot(finalForward, forwardVelocity)) * forwardVelocity.magnitude; // - motorTorque; // + (wheelAngularVelocity * Mathf.PI / 180f * radius);
		sidewaysSlip = Mathf.Sign(Vector3.Dot(finalRight, sidewaysVelocity)) * sidewaysVelocity.magnitude; // - motorTorque * (sidewaysFriction / forwardFriction);
	}
	Vector3 lastFinalForce;
	void ApplyForce(Vector3 finalForward, Vector3 finalRight)
	{
		var finalForce = new Vector3();
 
		// forward friction
		finalForce += finalForward * -forwardSlip * forwardFriction;
		// sideways friction
		finalForce += finalRight * -sidewaysSlip * sidewaysFriction;
 
		// forward force
		finalForce += finalForward * motorTorque;
 
		if (onGround)
			carRigidbody.AddForceAtPosition(finalForce, transform.position, ForceMode.Impulse);
 
		lastFinalForce = finalForce;
	}
 
	void OnDrawGizmos()
	{
		if (!Application.isPlaying)
			return;
		if (onGround)
			Gizmos.DrawSphere(transform.position, .1f);
		Gizmos.DrawSphere(carRigidbody.worldCenterOfMass, .1f);
		Gizmos.DrawLine(transform.position, transform.position + lastFinalForce);
		Gizmos.DrawLine(transform.position, transform.position + lastFinalForward);
	}
}Dependencies - ClassExtensions, VPhysics The ClassExtensions class just adds a couple methods for finding parents/children. 
The VPhysics class is just a wrapper for the built-in Physics.Raycast and Physics.RaycastAll functions, that handles layer-masks in a way I prefer (passing layer names in instead of bit-shifted integers). 
Assuming you don't want to take out the helper-classes before you've even tried the system, create a new C# script named "VExtras" and paste the following code: 
C# - VExtras.cs using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
public static class ClassExtensions
{
	// GameObject
	public static GameObject GetChild(this GameObject obj, string name) { return obj.transform.FindChild(name) ? obj.transform.FindChild(name).gameObject : null; }
	public static List<GameObject> GetParents(this GameObject obj, bool addSelf = false)
	{
		var result = new List<GameObject>();
		if (addSelf)
			result.Add(obj);
		Transform currentParent = obj.transform.parent;
		while (currentParent)
		{
			result.Add(currentParent.gameObject);
			currentParent = currentParent.parent;
		}
		return result;
	}
 
	// Vector3
	public static Vector3 NewX(this Vector3 obj, float val) { return new Vector3(val, obj.y, obj.z); }
	public static Vector3 NewY(this Vector3 obj, float val) { return new Vector3(obj.x, val, obj.z); }
	public static Vector3 NewZ(this Vector3 obj, float val) { return new Vector3(obj.x, obj.y, val); }
}
 
public static class VPhysics
{
	static int GetLayerMask(List<string> includeLayers = null, string excludeLayer1 = "Ignore Raycast", string excludeLayer2 = "SpaceCollider", params string[] extraExcludeLayers)
	{
		int result = 0; // no layers
		if (includeLayers == null)
			result = ~0; // all layers
		else
			foreach (string layer in includeLayers)
				result = result | (1 << LayerMask.NameToLayer(layer)); // add layer
		if (excludeLayer1 != null)
			result = result & ~(1 << LayerMask.NameToLayer(excludeLayer1)); // remove layer
		if (excludeLayer2 != null)
			result = result & ~(1 << LayerMask.NameToLayer(excludeLayer2)); // remove layer
		foreach (string layer in extraExcludeLayers)
			result = result & ~(1 << LayerMask.NameToLayer(layer)); // remove layer
		return result;
	}
	public static bool Raycast(Ray ray, out RaycastHit hit, float distance = float.MaxValue, List<string> includeLayers = null, string excludeLayer1 = "Ignore Raycast", string excludeLayer2 = "SpaceCollider", params string[] extraExcludeLayers)
		{ return Physics.Raycast(ray, out hit, distance, GetLayerMask(includeLayers, excludeLayer1, excludeLayer2, extraExcludeLayers)); }
	public static List<RaycastHit> RaycastAll(Ray ray, float distance = float.MaxValue, List<string> includeLayers = null, string excludeLayer1 = "Ignore Raycast", string excludeLayer2 = "SpaceCollider", params string[] extraExcludeLayers)
		{ return Physics.RaycastAll(ray, distance, GetLayerMask(includeLayers, excludeLayer1, excludeLayer2, extraExcludeLayers)).ToList(); }
}Usage There are a couple things you have to do to get the VWheelCollider working for a vehicle: 
The VWheelColliders do not add any spring forces, so to keep the wheels from falling through the terrain, you need to place a sphere-collider (or equivalent) for each wheel, with its radius set to the wheel's radius. 
Some sort of controller script is needed (as with the built-in WheelColliders). Here's an example: 
C# - VCarController.cs using UnityEngine;
 
class VCarController : MonoBehaviour
{
	public VWheelCollider backLeft;
	public VWheelCollider backRight;
	public VWheelCollider frontLeft;
	public VWheelCollider frontRight;
 
	public float steerMax = 30;
	public float motorMax = 500;
 
	float steer;
	float motor;
 
	void FixedUpdate()
	{
		steer = Input.GetKey(KeyCode.LeftArrow) ? - 1 : (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
		motor = Input.GetKey(KeyCode.DownArrow) ? -1 : (Input.GetKey(KeyCode.UpArrow) ? 1 : 0);
 
		backLeft.motorTorque = motorMax * motor;
		backRight.motorTorque = motorMax * motor;
		frontLeft.motorTorque = motorMax * motor;
		frontRight.motorTorque = motorMax * motor;
 
		frontLeft.steerAngle = steerMax * steer;
		frontRight.steerAngle = steerMax * steer;
 
		//var backLeftTire = backLeft.transform.GetChild(0);
		//var backRightTire = backRight.transform.GetChild(0);
		var frontLeftTire = frontLeft.transform.GetChild(0);
		var frontRightTire = frontRight.transform.GetChild(0);
 
		frontLeftTire.localEulerAngles = frontLeftTire.localEulerAngles.NewY(steerMax * steer);
		frontRightTire.localEulerAngles = frontRightTire.localEulerAngles.NewY(steerMax * steer);
	}
}
}
