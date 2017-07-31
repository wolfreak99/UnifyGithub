// Original url: http://wiki.unity3d.com/index.php/OnCollideExplode
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/OnCollideExplode.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Opless 
Description Borrowed from the documentation, if we collide with anything, we spawn an explosion prefab. 
Useful for missiles, grenades, etc 
CSharp - OnCollideExplode.cs using UnityEngine;
using System.Collections;
 
public class OnCollideExplode : MonoBehaviour 
{
 
// A grenade
// - instantiates a explosion prefab when hitting a surface
// - then destroys itself
public GameObject explosionPrefab;
public float      explodeSecs = -1;
 
void Awake()
{
		if(explodeSecs > -1) Invoke ("DestroyNow", explodeSecs);
}
 
void OnCollisionEnter( Collision collision ) 
{
	// Rotate the object so that the y-axis faces along the normal of the surface
	ContactPoint contact = collision.contacts[0];
	Quaternion   rot     = Quaternion.FromToRotation(Vector3.up, contact.normal);
	Vector3      pos     = contact.point;
	Instantiate(explosionPrefab, pos, rot);
	// Destroy the projectile
	Destroy (gameObject);
}
 
void DestroyNow()
{
	Instantiate(explosionPrefab, transform.position, transform.rotation);
	Destroy (gameObject);
}
 
 
 
 
}Javascript - OnCollideExplode.js // A grenade
// - instantiates a explosion prefab when hitting a surface
// - then destroys itself
var  explosionPrefab : GameObject;
var explodeSecs : float = -1;
 
function Awake()
{
	if (explodeSecs > -1) {
		Invoke ("DestroyNow", explodeSecs);
	}
}
 
function OnCollisionEnter( collision : Collision ) 
{
	// Rotate the object so that the y-axis faces along the normal of the surface
	var contact : ContactPoint = collision.contacts[0];
	var rot : Quaternion = Quaternion.FromToRotation (Vector3.up, contact.normal);
	var pos : Vector3 = contact.point;
	Instantiate(explosionPrefab, pos, rot);
	// Destroy the projectile
	Destroy (gameObject);
}
 
function DestroyNow()
{
	Instantiate(explosionPrefab, transform.position, transform.rotation);
	Destroy (gameObject);
}
}
