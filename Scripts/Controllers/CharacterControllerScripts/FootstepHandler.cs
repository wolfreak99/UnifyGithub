// Original url: http://wiki.unity3d.com/index.php/FootstepHandler
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/FootstepHandler.cs
// File based on original modification date of: 13 July 2013, at 18:03. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Jake Bayer (BakuJake14) 
DescriptionSimple C# script using CharacterController to create footstep sounds. 
UsageUse the sounds variable to assign the audio clip you want to use. Use minInterval, maxVelocity, and bias to change how the audio works. 
FootstepHandler.csusing UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class FootstepHandler : MonoBehaviour {
	public AudioClip[] sounds;			//An array that stores the footstep sounds
	public float minInterval = 0.1f;
	public float maxVelocity = 8.0f;
	public float bias = 1.1f;
 
	private CharacterController _controller;
 
	void Awake() {
		_controller = GetComponent<CharacterController>();
	}
 
	// Use this for initialization
	IEnumerator Start () {
		while(true) {
			float vel = _controller.velocity.magnitude;
			if(_controller.isGrounded && vel > 0.2f) {
				audio.clip = sounds[Random.Range(0, sounds.Length)];
				audio.Play();
				float interval = minInterval * (maxVelocity + bias) / (vel + bias);
				yield return new WaitForSeconds(interval);
			}
			else {
				yield return 0;
			}
		}
 
 
	}
}
}
