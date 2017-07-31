// Original url: http://wiki.unity3d.com/index.php/LightBehavior
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/LightBehavior.cs
// File based on original modification date of: 24 June 2012, at 17:17. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
This script allows you create a flashing light and flashing every few seconds. You can also set colors for it to alternate between. This can be used as a siren, for instance, with a good sound effect. Can be put on any game object with a Light component. By SevenBits. 
using UnityEngine;
 
public class LightBehavior : MonoBehaviour {
 
	private Light light;
	private int curFluxInd = 0;
 
	public bool flashing;
	public float flashEvery = 1.0F;
	public float flashDelay = 0.0F;
	public bool colorFlux;
	public Color[] colors;
	public float fluxEvery = 0.5F;
	public float fluxDelay = 0.0F;
 
	// Use this for initialization
	void Start () {
		light = GetComponent<Light>();
 
		if (flashing) {
			InvokeRepeating("FlashLight", flashDelay, flashEvery);
		}
 
		if (colorFlux) {
			InvokeRepeating("ColorFlux", fluxDelay, fluxEvery);
		}
	}
 
	void FlashLight() {
		light.enabled = !light.enabled;
	}
 
	void ColorFlux () {
		curFluxInd++;
		if (curFluxInd >= colors.Length) curFluxInd = 0;
 
		//Set the color to the new color.
		light.color = colors[curFluxInd];
	}
}
}
