// Original url: http://wiki.unity3d.com/index.php/Loudness
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Sound/Sound/Loudness.cs
// File based on original modification date of: 29 October 2013, at 01:23. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Sound.Sound
{
Author: Jessy 
DescriptionUnity's AudioSource.volume and AudioListener.volume use what is known as a linear taper. Although that is ideal for performance, it means that the 0-1 values that those properties utilize do not match up well with human perception, with loud values taking up a disproportionate amount of the range. This script is designed to make working with loudness more intuitive. 
Loudness is a complex phenomenon, and this simple script does not, for instance, take equal-loudness contours into account, but it should yield better results than a linear taper in every real-world case. I've found the result described here, that "a 10 dB increase in sound level corresponds approximately to a perceived doubling of loudness", leads the way to a highly usable loudness control. 
Instructions (C# only*)Add this script to your project, wherever you like. It's a static class, so you neither need nor have the ability to attach it to a Game Object. 

Loudness.OfListener (which alters AudioListener.volume) is a property, so you can assign a 0-1 value to it, or get one back, using the equals sign. 
Loudness.OfListener = .5F;  // Half as loud as it can get
Extension properties do not exist in C# yet, so for instances of Audio Sources, you have to use separate get and set methods. 
audio.SetLoudness(.5F);  // Affects the AudioSource attached to this Game Object
someAudioSource.GetLoudness(); // gives you a better idea of the portion of maximum loudness than someAudioSource.volume

A UnityScript alternative would have to be more verbose, due to lack of properties and extension methods in that language (at present), but feel free to add a JS solution to this page.  ;-) 
Loudness.csusing UnityEngine;
 
public static class Loudness 
{
	// A 20 dB increase sounds about 4x louder.
	// A signal needs an amplitude that is 10^(dB/20) greater, 
	// to be increased by 'dB' decibels.
	class Exponents 
	{
		public static readonly float Set = Mathf.Log(10, 4);
		public static readonly float Get = 1 / Set;
	}
 
	public static float GetLoudness(this AudioSource audioSource) 
	{
		return Mathf.Pow(audioSource.volume, Exponents.Get);
	}
 
	public static float SetLoudness(this AudioSource audioSource, float value) 
	{
		return audioSource.volume = Mathf.Pow(Mathf.Clamp01(value), Exponents.Set);
	}
 
	public static float OfListener 
	{
		get 
		{
			return Mathf.Pow(AudioListener.volume, Exponents.Get);
		}
		set 
		{
			AudioListener.volume = Mathf.Pow(Mathf.Clamp01(value), Exponents.Set);
		}
	}
}
}
