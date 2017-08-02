/*************************
 * Original url: http://wiki.unity3d.com/index.php/Mic_Input
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Sound/Sound/Mic_Input.cs
 * File based on original modification date of: 8 January 2016, at 23:04. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Sound.Sound
{
    ATTENTION! THIS PROJECT IS NO LONGER OPEN SOURCE I WILL NO LONGER BE UPDATING THE JAVASCRIPT VERSION ON THIS WIKI! However don't worry the version you find on this wiki is still free to use for every one, even in commercial projects or to use as a base for your own scripts, I will just no longer be working on this project as an open source project. you are still free to use the scripts on this wiki as a base for your own commercial projects. The latest (close source) version will be updated frequently can be bought here[1] 
    Note that neither of these scripts (the original javascript and the translated C#) are up to date and contain several errors. 
    Hello my name is Mark Duisters and I came across allot of people having problems with getting audio from the machine's microphone. 
    I have made a ready to use script, just place it on an empty GameObject and you are good to go. 
    from external scripts: 
    Call MicInput.loudness to get the audio data in a float format. (this can be used for scaling objects, chaning color, etc...). 
    Call gameObject.Find("MicInput").SendMessage("StopMicrophone"); This will terminate any data coming trough the mic in-game. 
    Call gameObject.Find("MicInput").SendMessage("StartMicrophone"); to start the mic from an external script (you don't need to call this as it is already running by default). 
    Note! StartMicrophone can only be called in the start function of a script (as that is the point where the mic is selected). 
    To use this script across all languages place it in the standard assets folder (not your own asset folder). 
    You are free to use this script under the creative commons restriction policy handled by the unify community. 
    This script can also be downloaded from the asset store under the name Mic Control (still pending for clearance). 
    Note that some functions are script depended, for instance the java version does not have a push to talk option. 
    Update Log: 
    1.2 
    C# 
       - Added 3 Talking methods:
           ~ Hold To Talk: Hold down a button to activate the mic.
           ~ Push To Talk: Press a button once to activate the mic, and press it again to stop it. 
           ~ Constant Talk: The microphone will constantly be activated. 
       - Improved the GUI functionality built into the script.
       - Editor GUI now shows which mic you are using.
       - General Optimization and bug fixes
    1.1 
       - Fixed some bugs pointed out by djfunkey from the community: http://forum.unity3d.com/members/118660-djfunkey
       - Expanded the editor functionality: now you can choose the audio device which you want to use. Up to 6 devices are supported. 
         If no device is found at the selected slot then the script will spit out an error message.
       - Added ShowDeviceName toggle. If true it wil print the device name and slot number to the console.
       - C# version of the script by djfunkey
       - Changed naming of several inputs to be more clear.
       - Added Source volume selection.(converted from djfunkey).
       - Added ingame menu (selected by user whether he wants to use it or not).(converted from djfunkey).
       - Added memory cleaning to prevent audio lag, both in java and C#(djfunkey).
    UnityScript (java) features: 
    1: An Editor menu and an ingame menu so the user/player can pick the microphone they wish to use (handy if hey have a web-cam microphone, and a headset microphone). 
    2: The user can now adjust the volume of their microphone, it will also affect the loudness variable proportionally. 
    3: If there is only 1 device detected it will make that device default. 
    4: Call input data from any script trough MicInput.loudness. 
    5: Option for showing all device names in the console + debugging option for visually reading the input data. 
    6: Option to listen to your audio input by toggling the mute button. 
    Example: var Myfloat:float=MicInput.loudness; This could be used to detect when a player is speaking, breathing or to implement interactive environments. Good luck and have fun. 
    #pragma strict
    @script RequireComponent (AudioSource);
    //if true a menu will apear ingame with all the microphones
    var SelectIngame:boolean=false;
    //if false the below will override and set the mic selected in the editor
     //Select the microphone you want to use (supported up to 6 to choose from). If the device has number 1 in the console, you should select default as it is the first defice to be found.
    enum Devices2 {DefaultDevice, Second, Third, Fourth, Fifth, Sixth}
     
    var InputDevice : Devices2;
    private var selectedDevice:String;
     
     
     
     
    var audioSource:AudioSource;
    //The maximum amount of sample data that gets loaded in, best is to leave it on 256, unless you know what you are doing. A higher number gives more accuracy but 
    //lowers performance allot, it is best to leave it at 256.
    var amountSamples:float=256;
    static var loudness:float;
    var sensitivity:float=1;
    var sourceVolume:float=100;
    private var minFreq: int;
    private var maxFreq: int;
     
    var Mute:boolean=true;
    var debug:boolean=false;
    var ShowDeviceName:boolean=false;
    private var micSelected:boolean=false; 
     
    private var mTimer:float=10;
    private var mRefTime:float=10; 
     
     
    function Start () {
     
    if(!audioSource){
      audioSource = GetComponent(AudioSource);
    	} 
     
    var i=0;
    //count amount of devices connected
    for(device in Microphone.devices){
    i++;
    if(ShowDeviceName){
    Debug.Log ("Devices number "+i+" Name"+"="+device);
     
    }
    }
    if(SelectIngame==false){
    //select the device if possible else give error
    if(InputDevice==Devices2.DefaultDevice){
    if(i>=1){
    selectedDevice= Microphone.devices[0];
    }
    else{
    Debug.LogError ("No device detected on this slot");
    }
     
    }
     
     
    if(InputDevice==Devices2.Second){
    if(i>=2){
    selectedDevice= Microphone.devices[1];
    }
    else{
    Debug.LogError ("No device detected on this slot");
    }
     
    }
     
     
     
    if(InputDevice==Devices2.Third){
    if(i>=3){
    selectedDevice= Microphone.devices[2];
    }
    else{
    Debug.LogError ("No device detected on this slot");
    return;
    }
    }
     
     
    if(InputDevice==Devices2.Fourth){
    if(i>=4){
    selectedDevice= Microphone.devices[2];
    }
    else{
    Debug.LogError ("No device detected on this slot");
    return;
    }
    }
    if(InputDevice==Devices2.Fifth){
    if(i>=5){
    selectedDevice= Microphone.devices[2];
    }
    else{
    Debug.LogError ("No device detected on this slot");
    return;
    }
    }
     
    if(InputDevice==Devices2.Sixth){
    if(i>=6){
    selectedDevice= Microphone.devices[2];
    }
    else{
    Debug.LogError ("No device detected on this slot");
    return;
    }
    }
     
    }
    //detect the default microphone
    audio.clip = Microphone.Start(selectedDevice, true, 10, 44100);
    //loop the playing of the recording so it will be realtime
    audio.loop = true;
    //if you only need the data stream values  check Mute, if you want to hear yourself ingame don't check Mute. 
    audio.mute = Mute;
    //don't do anything until the microphone started up
    while (!(Microphone.GetPosition(selectedDevice) > 0)){
     
    }
    //Put the clip on play so the data stream gets ingame on realtime
    audio.Play();
     
    }
     
     
     
    //apply the mic input data stream to a float;
    function Update () {
    //set timer for refreshing memory.
     mTimer += Time.deltaTime;
     //refresh the memory
    if (micSelected == true){
     if (mTimer >= mRefTime) {
    				StopMicrophone();
    				StartMicrophone();
    				mTimer = 0;
    			}
    		   }	
     
     
    if(Microphone.IsRecording(selectedDevice)){
      loudness = GetDataStream()*sensitivity*(sourceVolume/10);
      if(debug){
      Debug.Log(loudness);
      }
      }
     
      //the source volume
      if (sourceVolume > 100){
           sourceVolume = 100;
     }
     
      if (sourceVolume < 0){
       sourceVolume = 0;
       }
      audio.volume = (sourceVolume/100);
     
     
    }
     
     
    function GetDataStream(){
    if(Microphone.IsRecording(selectedDevice)){
     
       var dataStream: float[]  = new float[amountSamples];
           var audioValue: float = 0;
            audio.GetOutputData(dataStream,0);
     
            for(var i in dataStream){
                audioValue += Mathf.Abs(i);
            }
            return audioValue/amountSamples;
            }
     
     
     
     
     
     
    }
     
     
     
     
     
    //select device ingame
     
        function OnGUI () {
     if(SelectIngame==true){
            if (Microphone.devices.Length > 1 && micSelected == false)//If there is more than one device, choose one.
                 for (var i:int= 0; i < Microphone.devices.Length; ++i)
                     if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), Microphone.devices[i].ToString())) {
                         StopMicrophone();
                         selectedDevice = Microphone.devices[i].ToString();
                         GetMicCaps();
                        StartMicrophone();
                         micSelected = true;
     
                    }
     
            if (Microphone.devices.Length < 2 && micSelected == false) {//If there is only 1 decive make it default
                 selectedDevice = Microphone.devices[0].ToString();
                GetMicCaps();
                 micSelected = true;
     
            }
     }
        }
     
     
     
        //for the above control the mic start or stop
     
     
     public function StartMicrophone () {
             audio.clip = Microphone.Start(selectedDevice, true, 10, maxFreq);//Starts recording
             while (!(Microphone.GetPosition(selectedDevice) > 0)){} // Wait until the recording has started
             audio.Play(); // Play the audio source!
     
        }
     
     
     
     public function StopMicrophone () {
             audio.Stop();//Stops the audio
             Microphone.End(selectedDevice);//Stops the recording of the device  
     
        }
     
     
          function GetMicCaps () {
             Microphone.GetDeviceCaps(selectedDevice,  minFreq,  maxFreq);//Gets the frequency of the device
             if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
                 maxFreq = 44100;
     
        }
     
     
     
     
     
     
        //Create a gui button in another script that calls to this script
            public function MicDeviceGUI (left:float , top:float, width:float, height:float, buttonSpaceTop:float, buttonSpaceLeft:float) {
    	if (Microphone.devices.Length > 1 && micSelected == false)//If there is more than one device, choose one.
    		for (var i:int=0; i < Microphone.devices.Length; ++i)
    			if (GUI.Button(new Rect(left + (buttonSpaceLeft * i), top + (buttonSpaceTop * i), width, height), Microphone.devices[i].ToString())) {
    				StopMicrophone();
    				selectedDevice = Microphone.devices[i].ToString();
    				GetMicCaps();
    				StartMicrophone();
    				micSelected = true;
    			}
    	if (Microphone.devices.Length < 2 && micSelected == false) {//If there is only 1 decive make it default
    		selectedDevice = Microphone.devices[0].ToString();
    		GetMicCaps();
    		micSelected = true;
    	}
        }
    djfunkey did an excelent job in converting the script to C#. Thanks to him even more people can take advantage of the microphone in game. http://forum.unity3d.com/members/118660-djfunkey 
    The Editor Script for the C# version is below. 
    To talk in game using either of the methods press "T"* 
    C# features: 
    1: A menu so that the user can pick the microphone they wish to use (handy if hey have a web-cam microphone, and a headset microphone) 
    2: The user can now adjust the volume of their microphone, it will also affect the loudness variable proportionally. 
    3: The user can choose from 3 settings: PushtToTalk, HoldToTalk, ConstantTalk. These determine what the user has to do for the microphone to be activated. 
    4: If there is only 1 device detected it will make that device default. 
    5: Call microphone data from any script with MicrophoneInput.loudness. 
    6: The microphone RAM will still be flushed even if Time.timeScale = 0; 
    7: You can set how often the michrophone RAM is flushed. 
    
    
    using UnityEngine;
    using System.Collections;
     
    [RequireComponent(typeof(AudioSource))]
    public class MicControlC : MonoBehaviour {
     
    	public enum micActivation {
    		HoldToSpeak,
    		PushToSpeak,
    		ConstantSpeak
    	}
     
    	public float sensitivity = 100;
    	public float ramFlushSpeed = 5;//The smaller the number the faster it flush's the ram, but there might be performance issues...
    	[Range(0,100)]
    	public float sourceVolume = 100;//Between 0 and 100
    	public bool GuiSelectDevice = true;
    	public micActivation micControl;
    	//
    	public string selectedDevice { get; private set; }	
    	public float loudness { get; private set; } //dont touch
    	//
    	private bool micSelected = false;
    	private float ramFlushTimer;
    	private int amountSamples = 256; //increase to get better average, but will decrease performance. Best to leave it
    	private int minFreq, maxFreq; 
     
        void Start() {
    		audio.loop = true; // Set the AudioClip to loop
    		audio.mute = false; // Mute the sound, we don't want the player to hear it
    		selectedDevice = Microphone.devices[0].ToString();
    		micSelected = true;
    		GetMicCaps();
        }
     
    	void OnGUI() {
    		MicDeviceGUI((Screen.width/2)-150, (Screen.height/2)-75, 300, 100, 10, -300);
    		if (Microphone.IsRecording(selectedDevice)) {
    			ramFlushTimer += Time.fixedDeltaTime;
    			RamFlush();
    		}
    	}
     
    	public void MicDeviceGUI (float left, float top, float width, float height, float buttonSpaceTop, float buttonSpaceLeft) {
    		if (Microphone.devices.Length > 1 && GuiSelectDevice == true || micSelected == false)//If there is more than one device, choose one.
    			for (int i = 0; i < Microphone.devices.Length; ++i)
    				if (GUI.Button(new Rect(left + ((width + buttonSpaceLeft) * i), top + ((height + buttonSpaceTop) * i), width, height), Microphone.devices[i].ToString())) {
    					StopMicrophone();
    					selectedDevice = Microphone.devices[i].ToString();
    					GetMicCaps();
    					StartMicrophone();
    					micSelected = true;
    				}
    		if (Microphone.devices.Length < 2 && micSelected == false) {//If there is only 1 decive make it default
    			selectedDevice = Microphone.devices[0].ToString();
    			GetMicCaps();
    			micSelected = true;
    		}
    	}
     
    	public void GetMicCaps () {
    		Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
    		if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
    			maxFreq = 44100;
    	}
     
    	public void StartMicrophone () {
    		audio.clip = Microphone.Start(selectedDevice, true, 10, maxFreq);//Starts recording
    		while (!(Microphone.GetPosition(selectedDevice) > 0)){} // Wait until the recording has started
    		audio.Play(); // Play the audio source!
    	}
     
    	public void StopMicrophone () {
    		audio.Stop();//Stops the audio
    		Microphone.End(selectedDevice);//Stops the recording of the device	
    	}		
     
        void Update() {
    		audio.volume = (sourceVolume/100);
    		loudness = GetAveragedVolume() * sensitivity * (sourceVolume/10);
    		//Hold To Speak!!
    		if (micControl == micActivation.HoldToSpeak) {
    			if (Microphone.IsRecording(selectedDevice) && Input.GetKey(KeyCode.T) == false)
    				StopMicrophone();
    			//
    			if (Input.GetKeyDown(KeyCode.T)) //Push to talk
    				StartMicrophone();
    			//
    			if (Input.GetKeyUp(KeyCode.T))
    				StopMicrophone();
    			//
    		}
    		//Push To Talk!!
    		if (micControl == micActivation.PushToSpeak) {
    			if (Input.GetKeyDown(KeyCode.T)) {
    				if (Microphone.IsRecording(selectedDevice)) 
    					StopMicrophone();
     
    				else if (!Microphone.IsRecording(selectedDevice)) 
    					StartMicrophone();
    			}
    			//
    		}
    		//Constant Speak!!
    		if (micControl == micActivation.ConstantSpeak)
    			if (!Microphone.IsRecording(selectedDevice)) 
    				StartMicrophone();
    		//
    		if (Input.GetKeyDown(KeyCode.G))
    			micSelected = false; 
        }
     
    	private void RamFlush () {
    		if (ramFlushTimer >= ramFlushSpeed && Microphone.IsRecording(selectedDevice)) {
    			StopMicrophone();
    			StartMicrophone();
    			ramFlushTimer = 0;
    		}
    	}
     
    	float GetAveragedVolume() {
            float[] data = new float[amountSamples];
            float a = 0;
            audio.GetOutputData(data,0);
            foreach(float s in data) {
                a += Mathf.Abs(s);
            }
            return a/amountSamples;
        }
    }
}
