// Original url: http://wiki.unity3d.com/index.php/ArcBall
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/ArcBall.cs
// File based on original modification date of: 18 July 2013, at 09:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Vlad Chifor 
Description Use this script to rotate an object in an intuitive way. The mouse input is used to compute a rotation axis and an angular velocity. This idea is based on the ArcBall Rotation Control technique presented in the book Graphics Gems IV by Ken Shoemake. 
Usage Attach this script and a sphere collider to the object. Adjust the sphere radius to your needs. Use the speed and damping variables to tweak the rotation speed. 
C Sharp - ArcBall.cs // Attach this script and a sphere collider to the object. Adjust the sphere radius to your needs. Use the speed and damping variables to tweak the rotation speed.
//
// Author: Vlad Chifor - racocvr [at] gmail (dot) com
 
using UnityEngine;
using System.Collections;
 
public class ArcBall : MonoBehaviour
{
	public float damping = 0.9f;
	public float speed = 0.1f;
 
	private Vector3 vDown;
	private Vector3 vDrag;
	private bool dragging;
	private float angularVelocity;
	private Vector3 rotationAxis;
 
	void Start ()
	{
		dragging = false;
		angularVelocity = 0;
		rotationAxis = Vector3.zero;
	}
 
	void Update ()
	{	
		// on mouse down
		if( Input.GetMouseButton(0) ) 
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
 
			// if the object was clicked
			if( Physics.Raycast(ray, out hit) ) 
			{
				if( !dragging )
				{
					// extract vDown from the RaycastHit
					vDown = hit.point - transform.position;
 
					// start dragging
					dragging = true;
				}
				else
				{
					// extract vDrag from the RaycastHit
					vDrag = hit.point - transform.position;
 
					// compute the rotation axis and angular velocity from vDown and vDrag
					rotationAxis = Vector3.Cross( vDown, vDrag );
					angularVelocity = Vector3.Angle( vDown, vDrag ) * speed;
				}
			}
			else
				dragging = false;
		}
 
		// on mouse up stop dragging
		if( Input.GetMouseButtonUp(0) )
			dragging = false;
 
		// apply the angular velocity
		if( angularVelocity > 0 )
		{
			transform.RotateAround( rotationAxis, angularVelocity * Time.deltaTime );
			angularVelocity = ( angularVelocity > 0.01f ) ? angularVelocity * damping : 0;
		}
	}
}
}
