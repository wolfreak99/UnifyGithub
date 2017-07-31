// Original url: http://wiki.unity3d.com/index.php/StopEmittingAfterDelay
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/StopEmittingAfterDelay.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Jonathan Czeck (aarku) 
DescriptionUse this script on a particle emitter with Autodestruct on. After it stops emitting, the particles will fade naturally then the object will get destroyed. FIXME 
UsageUse this script on a particle emitter with Autodestruct on. After it stops emitting, the particles will fade naturally then the object will get destroyed. FIXME 
C# - StopEmittingAfterDelay.csusing UnityEngine;
 using System.Collections;
 
 public class StopEmittingAfterDelay : MonoBehaviour
 {
 	public float delay = 1F;
 
 	void Start()
 	{
 		Invoke("StopEmitting", delay);
 	}
 
 	void StopEmitting()
  	{
 		particleEmitter.emit = false;
 	}
 }
}
