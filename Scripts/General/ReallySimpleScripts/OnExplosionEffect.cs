// Original url: http://wiki.unity3d.com/index.php/OnExplosionEffect
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/OnExplosionEffect.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Opless 
Description Simple Explosion Effect. Instantiate along with particles to throw nearby rigidbodies away from the explosion. 
CSharp - OnExplosionEffect.cs using UnityEngine;
using System.Collections;
 
public class OnExplosionEffect : MonoBehaviour {
 
	public float radius = 5;
	public float power  = 5;
	public float upwardForce = 0;
 
	private float radiusUsed = 0.5F;
 
	// Update is called once per frame
	void FixedUpdate () 
	{
		// Applies an explosion force to all nearby rigidbodies
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere (explosionPos, radius);
 
		foreach (Collider hit in colliders) 
		{
			if(hit == null)
				continue;
			if (hit.rigidbody)
			{
				hit.rigidbody.AddExplosionForce(power, explosionPos, radiusUsed, upwardForce);
			}
		}
		radiusUsed = ((radius-radiusUsed)/2)*Time.deltaTime;
	}
 
// Auto destroy	
public float timeOut = 1.0F;
 
void  Awake ()
{
	Invoke ("DestroyNow", timeOut);
}
 
void DestroyNow ()
{
 
	DestroyObject (gameObject);
}	
 
 
}Javascript - OnExplosionEffect.js  
var radius : float = 5;
var power  : float = 5;
var upwardForce : float = 0;
 
private var radiusUsed : float = 0.5F;
 
// Update is called once per frame
function FixedUpdate () 
{
	// Applies an explosion force to all nearby rigidbodies
	var explosionPos : Vector3 = transform.position;
	var colliders : Collider[] = Physics.OverlapSphere (explosionPos, radius);
 
	for (var hit : Collider in colliders) 
	{
		if(hit == null)
		{
			continue;
		}
		if (hit.rigidbody)
		{
			hit.rigidbody.AddExplosionForce(power, explosionPos, radiusUsed, upwardForce);
		}
	}
	radiusUsed = ((radius-radiusUsed)/2)*Time.deltaTime;
}
 
// Auto destroy	
var timeOut : float = 1.0F;
 
function  Awake ()
{
	Invoke ("DestroyNow", timeOut);
}
 
function DestroyNow ()
{
 
	DestroyObject (gameObject);
}
}
