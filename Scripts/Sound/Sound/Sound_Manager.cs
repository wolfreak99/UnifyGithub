/*************************
 * Original url: http://wiki.unity3d.com/index.php/Sound_Manager
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Sound/Sound/Sound_Manager.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Tom Vogt (tom@lemuria.org) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Sound.Sound
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 JavaScript - SoundManager.js 
    4 JavaScript - MusicTrigger.js 
    
    DescriptionTwo scripts I use to add a bit of dynamic music. The first script takes a list of songs and will play through them, repeating with the first when he reaches the last one. The second script allows interruption of the background music with event-driven clips, after which the background music will continue. 
    
    
    Usage You need 3 game objects for the first script: 
    Music Manager 
    Source 1 
    Source 2 
    The two sources need an audio source component attached. Everything else will be handled by the script. Attach it to the Music Manager gameobject, drag the two source gameobjects into the slots for Source 1 and Source 2 and your music clips into the array for soundtracks. 
    
    The second script should be attached to a collider object with the collider set to trigger. This trigger defines the area where the interrupt will occur. Make it large enough, because if the player leaves the area, the interrupt music will end. One way or the other, it will only play once. It should be easy to modify the script so that it either loops, or does not end upon leaving. 
    
    
    JavaScript - SoundManager.js   /*
    	Sound Manager Scripty by Tom Vogt <tom@lemuria.org>
     
    	attach to a gameobject that has two children (Source1 and Source2) which have audio source components
    	the audio clips used as soundtracks should have "3D sound" DISABLED
    	call "Interrupt()" to fade in event-driven music (see MusicTrigger.js for an example)
      */
     
    var SoundTracks : AudioClip[];
    var FadeLength : float = 2.0;
     
    var Source1 : GameObject;
    var Source2 : GameObject;
    private var CurrentSourceActive : int = 1;
    private var CurrentTrack : int = 0;
    private var SoundRunning : boolean = true;
    private var Interrupted : boolean = false;
     
    function Start() {
    	if (SoundTracks.length==0) return;
    	Source1.audio.volume = 1.0;
    	Source2.audio.volume = 0.0;
     
    	Source1.audio.PlayOneShot(SoundTracks[0]);
    	yield WaitForSeconds(SoundTracks[0].length - FadeLength);
    	Fadeover();
    }
     
    function Fadeover() {
    	while (SoundRunning) {
    		CurrentTrack++;
    		if (CurrentTrack>=SoundTracks.length) CurrentTrack=0;
    		Debug.Log("next track: "+CurrentTrack);
     
    		if (CurrentSourceActive==1) {
    			Debug.Log("switching to source 2");
    			Source2.audio.PlayOneShot(SoundTracks[CurrentTrack]);
    			CurrentSourceActive=2;
    			FadeUpDown(Source2.audio, Source1.audio, FadeLength);
    		} else {
    			Debug.Log("switching to source 1");
    			Source1.audio.PlayOneShot(SoundTracks[CurrentTrack]);
    			CurrentSourceActive=1;
    			FadeUpDown(Source1.audio, Source2.audio, FadeLength);
    		}
    		Debug.Log("waiting for end...");
    		yield WaitForSeconds(SoundTracks[CurrentTrack].length - FadeLength);
    	}
    }
     
    function Interrupt(with:AudioClip) {
    	Debug.Log("interrupting");
    	if (Interrupted) return;
    	Interrupted = true;
    	if (CurrentSourceActive==1) {
    		Source2.audio.PlayOneShot(with);
    		FadeUpDown(Source2.audio, Source1.audio, 1.0);
    	} else {
    		Source1.audio.PlayOneShot(with);
    		FadeUpDown(Source1.audio, Source2.audio, 1.0);
    	}
    	yield WaitForSeconds(1.0);
    	var waitfor = with.length-2.0;
    	if (CurrentSourceActive==1) {
    		Source1.audio.Pause();
    		if (waitfor>0)	yield WaitForSeconds(waitfor);
    		EndInterrupt();
    	} else {
    		Source2.audio.Pause();
    		if (waitfor>0)	yield WaitForSeconds(waitfor);
    		EndInterrupt();
    	}
    }
     
    function EndInterrupt() {
    	if (!Interrupted) return;
    	Interrupted = false;
    	if (CurrentSourceActive==1) {
    		Source1.audio.Play();
    		FadeUpDown(Source1.audio, Source2.audio, 1.0);
    		yield WaitForSeconds(1.0);
    		Source2.audio.Stop();
    	} else {
    		Source2.audio.Play();
    		FadeUpDown(Source2.audio, Source1.audio, 1.0);
    		yield WaitForSeconds(1.0);
    		Source1.audio.Stop();
    	}
    }
     
    function FadeUpDown(up:AudioSource, down:AudioSource, duration:float) {
    	var MyVolume = 0.0;
    	while (MyVolume<1.0) {
    		MyVolume += Time.deltaTime / duration;
    		up.volume = MyVolume;
    		down.volume = 1.0-MyVolume;
    		yield WaitForFixedUpdate();
    	}
    	up.volume = 1.0;
    	down.volume = 0.0;
    }
    
    JavaScript - MusicTrigger.js @script RequireComponent(Collider)
     
    var MusicController:GameObject;
    var MySound : AudioClip;
     
    function OnTriggerEnter() {
    	MusicController.SendMessage("Interrupt", MySound);
    }
     
    function OnTriggerExit() {
    	MusicController.SendMessage("EndInterrupt");
}
}
