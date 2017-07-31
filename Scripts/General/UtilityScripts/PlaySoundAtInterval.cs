// Original url: http://wiki.unity3d.com/index.php/PlaySoundAtInterval
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/PlaySoundAtInterval.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Author: Will Preston (Sigma) 
DescriptionThis script allows for a sound to be played at intervals specified by the user. NOTE: The script does not take into account the time length of the sound; likewise, if you wanted to play an 11-second sound every 60 seconds, you would set the interval to be 71 (60sec + 11sec). 
UsagePlace this script in a C# file named PlaySoundAtInterval.cs, and drag it onto the object with the AudioSource you wish for it to affect. The audio source does not have to have any sound loaded into it, this script handles that as well. You may encounter problems if the AudioSource has looped playback and/or Play On Awake enabled, so it is recommended that you disable those options for the AudioSource in question. 
C# - PlaySoundAtInterval.csusing UnityEngine;
using System.Collections;
 
// PlaySoundAtInterval.cs
// Copyright (c) 2010-2011 Sigma-Tau Productions (http://www.sigmatauproductions.com/).
// This script is free to be used in both free and commercial projects as long as this
// notice is retained.
 
[RequireComponent (typeof (AudioSource))]
public class PlaySoundAtInterval : MonoBehaviour {
 
	// Public variables
	// Will the sound play on startup?
	public bool playAtStartup = false;
 
	// The interval of time (in seconds) that the sound will be played.
	public float interval = 3.0f;
 
	// The sound itself.
	public AudioClip clipToPlay;
 
	// Private variables
	// A modifier that will prevent the script from running in the event of an error
	private bool disableScript = false;
 
	// The amount of time that has passed since the last initial playback of the sound.
	private float trackedTime = 0.0f;
 
	// Tracks to see if we've played this at startup.
	private bool playedAtStartup = false;
 
	// Use this for initialization
	void Start () {
		if (interval < 1.0f) { // Make sure the interval isn't 0, or we'll be constantly playing the sound!
			Debug.LogError("Interval base must be at least 1.0!");
			disableScript = true;
		}
	}
 
	// Update is called once per frame
	void Update () {
		if (!disableScript) {
 
			// Play the sound when the scene starts
			if (playAtStartup && !playedAtStartup) {
				audio.PlayOneShot(clipToPlay);
				playedAtStartup = true;
			}
 
			// Increment the timer
			trackedTime += Time.deltaTime;
 
			// Check to see that the proper amount of time has passed
			if (trackedTime >= interval) {
				// Play the sound, reset the timer
				audio.PlayOneShot(clipToPlay);
				trackedTime = 0.0f;
			}
		}
	}
 
 
}
}
