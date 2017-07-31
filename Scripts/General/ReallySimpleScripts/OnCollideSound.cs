// Original url: http://wiki.unity3d.com/index.php/OnCollideSound
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/OnCollideSound.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Opless 
Description Simple Prefab spawning collision, great for collision sounds 
CSharp - OnCollideSound.cs using UnityEngine;
 
public class OnCollideSound : MonoBehaviour 
{
	public GameObject collisionSoundPrefab;
	public float      triggerMagnitude = 2;
	public bool       attachToMe = true;
 
	void OnCollisionEnter( Collision collision ) 
	{
		if (collision.relativeVelocity.magnitude > triggerMagnitude)
		{
			GameObject o = (GameObject) Instantiate(collisionSoundPrefab, this.transform.position, this.transform.rotation);
			if(attachToMe)
				o.transform.parent = transform;
		}	
	}
}Javascript - OnCollideSound.js var collisionSoundPrefab : GameObject;
var triggerMagnitude : float = 2;
var attachToMe : boolean = true;
 
function OnCollisionEnter( collision : Collision ) 
{
	if (collision.relativeVelocity.magnitude > triggerMagnitude)
	{
		var o : GameObject = Instantiate(collisionSoundPrefab, this.transform.position, this.transform.rotation);
		if(attachToMe) {
			o.transform.parent = transform;
		}	
	}
}
}
