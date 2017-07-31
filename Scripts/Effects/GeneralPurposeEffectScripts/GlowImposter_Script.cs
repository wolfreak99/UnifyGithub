// Original url: http://wiki.unity3d.com/index.php/GlowImposter_Script
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/GlowImposter_Script.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Forest (yoggy) 
DescriptionThis script uses a flat plane to generate a cool graphical effect that mimics a glowing volume by rotating and scaling a plane with a circle texture on it. This can be used for glowing bullets, lasers etc. 
UsageUse this script on both a flat plane object with an Additive shader and circle texture and a blank object that parents the flat plane. Set the "parent" variable accordingly. 


JavaScript - GlowImposter.jsvar parent = true;
 
private var normalScale = Vector3(0, 0, 0);
 
function Update ()
{
	target = Camera.main.transform;
 
	// Get the rotation tword the camera
	vector = target.position - transform.position;
 
	/* parent should be true on the blank game object that parents the flat plane object
	parent should be false on the flat plane object parented by the blank object*/
 
	if(parent)
	{
		// rotate the parent's z axis toward the camera
		vector.z = 0;
		transform.rotation = Quaternion.FromToRotation (Vector3.up, vector);
	}
	else
	{
		// rotate the child object's x axis toward the camera
		z = vector.normalized.z;
		if(normalScale == Vector3.zero) normalScale = transform.localScale;
		transform.localEulerAngles.x = z * 90;
 
		// get a range for relative z distance
		z = Mathf.Abs(z);
		z = 1 - z;		
 
		// scale by relative z distance
		transform.localScale.z = z * normalScale.z;
		transform.localScale.z += normalScale.x;
	}
}
}
