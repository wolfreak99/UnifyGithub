/*************************
 * Original url: http://wiki.unity3d.com/index.php/Volume_Slider
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Sound/Sound/Volume_Slider.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Sound.Sound
{
    JavaScript Volume SliderScript to control a smooth audio fade over a duration of time. 
    //assign the main timer vars
    public var Seconds : int;
    public var StartTime : float;
    public var PlayTime : float;
     
    //assign the sound timer vars
    public var soundStartTime : float;
    public var soundPlayTime : float;
     
    //the sound object that will hold the audio source (say a cube or something)
    var soundSource : GameObject;
     
    //set the Go vars
    static var Go = 0;
    public var soundGo = 0;
     
    var soundBool : boolean = false;
     
    //volume of the sound (if you set this to a higher value than zero, it shall play that volume on start. 1.0 is max)
    var soundVolume : float = 0.00;
     
    //on awake start the main timer
    function Awake() {
     
    	//call start the main timer function
    	start_timer();
     
    }
     
    //function to start the sound timer
    function start_sound_timer() {
     
    	//set the soundStartTime to be equal to generic time
    	soundStartTime = Time.time;
     
    	//set the soundGo to Go (ie: 1 for go, 0 for not go. Leave this value at 1)
    	soundGo = 1;
     
    }
     
    //function to start the main timer
    function start_timer() {
     
    	//set the StartTime to be equal to generic time
    	StartTime =Time.time;
     
    	//set the Go to Go (ie: 1 for go, 0 for not go. Leave this value at 1)	
    	Go = 1;
     
    }
     
    function Update() {
     
    	//if Go of main timer is Go then set the vars
    	if (Go == 1) {
    		//assign PlayTime float to equal the generic StartTime assigned earlier in start_timer function minus the current time 
    		PlayTime =Time.time - StartTime;
     
    		//convert the PlayTime into readable integers ie: 1,2,3,4,5 etc and store them in var Seconds
    		Seconds = PlayTime % 60;
    	}
     
    	//if soundGo of sound timer is Go then set the vars
    	if (soundGo == 1) {
     
    		//assign soundPlayTime float to equal the generic soundStartTime assigned earlier in start_sound_timer function minus the current time 
    		soundPlayTime = Time.time - soundStartTime;
    	}
     
    	//if seconds are between 10 and twenty and the soundBool is false set the soundBool to true and start the sound timer
    	if(Seconds > 10 && Seconds < 20 && soundBool == false) {soundBool = true; start_sound_timer();}
     
    	//you do not have to use Seconds, you can use PlayTime as a seconds call, this sets the soundBool to switch off ie: start decreasing sound when seconds are above 20)
    	if(PlayTime > 20 && soundBool == true) {soundBool = false;}
     
    	//loop the start_sound_timer once the soundPlayTime hits 0.2 (ie: start the timer again) change this soundPlayTime to decrease or increase the time accordingly
    	if(soundBool == true && soundPlayTime == 0.2) {start_sound_timer();}
     
    	//increase the sound as soundPlayTime hits time of 0.1 (keep this below the value in the above line of code and keep this value the same as the value in the next line) and increase the volume accordingly
    	if(soundPlayTime > 0.1 && soundBool == true) {soundVolume = soundVolume + 0.01;}
    	//decrease the sound as soundPlayTime hits time of 0.1 and decrease the volume accordingly
    	if(soundPlayTime > 0.1 && soundBool == false && soundVolume > 0) {soundVolume = soundVolume - 0.01;}
     
    	//so that volume does not max out ie: get more than say 100 and mess up nice speaker system, set a volume cap here
    	if(soundVolume > 0.8) {soundVolume = 0.8;}
    	//keep the volume at bottom when soundBool is false and volume is below a value
    	if(soundVolume < 0.005 && soundBool == false) {soundVolume = 0.0;}
     
    	//set the audiosource gameobject to mirror the sound volume
    	soundSource.audio.volume = soundVolume;
     
    	//when seconds reaches a certain amount restart the timer
    	if(PlayTime > 30) {start_timer();}
     
    }
     
    //All ur base
}
