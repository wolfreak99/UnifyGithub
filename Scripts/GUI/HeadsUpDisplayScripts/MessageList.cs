/*************************
 * Original url: http://wiki.unity3d.com/index.php/MessageList
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/HeadsUpDisplayScripts/MessageList.cs
 * File based on original modification date of: 24 January 2012, at 01:57. 
 *
 * Author: capnbishop 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.HeadsUpDisplayScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Javascript - MessageList.js 
    4 Javascript - TimedFadeText.js 
    5 Javascript - TimedFadeTextMESH.js 
    6 C# - MessageList.cs 
    7 C# - TimedFadeText.cs 
    
    DescriptionThese two scripts can be used to create a list of automatic self-destroying messages that fade away. Each added message is put at the top or bottom of a list, moving the rest down or up (respectively) depending on the direction. The message will last for a designated period of time, then fade away and destroy itself. 
    UsageUse MessageList.js on an empty game object in the scene. Use the TimedFadeText.js script on a GUIText prefab, then assign that prefab to MessageList. Call AddMessage() on MessageList to create a message and have it added to the list. New messages will be instantiated at the pixel coordinate position specified in startPos (origin at the bottom left corner), and existing messages will be moved a set number of pixels along the y axis by the lineSize property. The duration of the life of the message and fading period can be set in TimedFadeText. 
    Included at the end is the modified code for the use on a TextMESH rather than a GUIText object. Use this code if you need to use a 3D Text Mesh rather than a 2D GUI Text Object. This code is called "TimedFadeTextMESH.js". 
    The exact syntax for calling AddMessage is as follows: 
    MessageList.Instance().AddMessage("Your message text here!");Javascript - MessageList.js //	MessageList.js
    //	From the Unity Wiki
    //	Use with TimedFadeText.js
    //	Attach to an emtpy Game Object
     
    var messagePrefab : GUIText;					// The prefab for our text object
     
    var lineSize : float = 20.0;					// The pixel spacing between text objects
    var startPos : Vector3 = Vector3 (20, 20, 0);	// The position GUIText objects will be instantiated
    var layerTag : int = 0;
    var insertAbove : boolean = true;
     
    private var messages = new Array();				// The array that holds all our text objects
    private var directionFactor : float = 1.0;
     
    //	Provide singleton support for this class.
    //	The script must still be attached to a game object, but this will allow it to be called
    // 	from anywhere without specifically identifying that game object.
    static private var messageListInstance : MessageList;
    static var instance : MessageList;
     
    static function Instance () {
    	if (!instance) {
    		instance = FindObjectOfType (MessageList);
    		if (!instance)
    			Debug.LogError ("There needs to be one active MessageList script on a GameObject in your scene.");
    	}
    	return instance;
    }
     
    function Awake () {
    	//	First make sure that we have a prefab set.  If not, then disable the script
    	if (!messagePrefab) {
    		enabled = false;
    		Debug.Log("Must set the GUIText prefab for MessageList");
    	}
     
    	if (insertAbove) {
    		directionFactor = 1.0;
    	}
     
    	else {
    		directionFactor = -1.0;
    	}
    }
     
    //	AddMessage() accepts a text value and adds it as a status message.
    //	All other status messages will be moved along the y axis by a normalized distance of lineSize.
    //	AddMessage() also handles automatic removing of any GUIText objects that automatically destroy
    //	themselves.
    function AddMessage (messageText : String) {
    	//	Itterate through the messages, removing any that don't exist anymore, and moving the rest
    	for (var i = 0; i < messages.length; i++) {
    		//	If this message is null, remove it, drop back the i count, and jump back to the begining
    		//	of the loop.
    		if (!(messages[i] as GUIText)) {
    			messages.RemoveAt(i);
    			i--;
    			continue;
    		}
     
    		//	If this message object does exist, then move it along the y axis by lineSize.
    		//	The y axis uses normalized coordinates, so we divide by the screen height to convert
    		//	pixel coordinates into normalized screen coordinates.
     
    		(messages[i] as GUIText).transform.position.y += directionFactor * (lineSize/Screen.height);
    	}
     
    	//	All the existing messages have been moved, making room for the new one.
    	//	Instantiate a new message from the prefab, set it's text value, and add it to the
    	//	array of messages.
    	var newMessage : GUIText = Instantiate(messagePrefab, Vector3(startPos.x/Screen.width, startPos.y/Screen.height, startPos.z), transform.rotation);
    	newMessage.text = messageText;
    	newMessage.gameObject.layer = layerTag;
    	messages.Add(newMessage);
    }Javascript - TimedFadeText.js //	TimedFadeText.js
    //	From the Unity Wiki
    //	Use with MessageList.js
    //	Use on a GUIText Object saved as a PreFab
    //	This PreFab will be instantiated by MessageList.js
    //	Check the GUIText Object settings for alignment
    //	Check the MessageList.js settings for placement on screen
     
    var liveTime : float = 5.0;					//	The number of seconds the GUIText will last before starting to fade
    var fadeTime : float = 2.0;					//	The number of seconds to fade until totally transparent
     
    private var time : float = 0.0;				//	Static var to track how much time has passed
    private var isFading : boolean = false;		//	Static var to track if we're in the fading stage
    private var startAlpha : float = 1.0;		//	Static var to keep track of the initial amount of alpha
     
    function Start () {
    	//	This script uses the GUIText's material to set the alpha fade.
    	//	If the font doesn't have a material, then this won't work, so disable the script.
    	if (!guiText.material) {
    		Debug.Log ("You're Screwed!");
    		enabled = false;
    	}
     
    	//	Get the starting alpha value.
    	//	If the developer has the text start transparent, then we need to fade from that point.
    	startAlpha = guiText.material.color.a;
    }
     
    function Update () {
    	//	Update our time var to keep track of how much time has passed.
    	time += Time.deltaTime;
     
    	if (isFading) {
    		//	We're in the fading stage. If we've reached the end of this stage, then destroy the gameObject.
    		if (time >= fadeTime) {
    			Destroy(gameObject);
    		}
     
    		//	We're still fading, so update the material's alpha color to make it fade a little more.
    		guiText.material.color.a = CalculateAlpha();
    	}
     
    	else if (time >= liveTime) {
    		//	If we're not fading yet, but should be, then update our values to proceed to the fading stage.
    		isFading = true;
    		time = 0.0;
    	}
     
    	//	If we're not fading yet, and don't need to be yet, then nothign will happen at this point. The
    	//	text will just exist, and the timer will keep incrementing until there's a state change.
    }
     
    //	CalculateAlpha() simple takes the static global vars we're using to keep track of everything
    //	to figure out our current alpha value from 0 to 1.
    function CalculateAlpha() {
    	//	Find out the percent of time from 0 to 1 that has gone between when the text starts and stops fading
    	var timePercent : float = Mathf.Clamp01((fadeTime - time) / fadeTime);
    	//	Generate a nice, smooth value from 1 to 0 to represent how faded the text is
    	var smoothAlpha : float = Mathf.SmoothStep(0.0, startAlpha, timePercent);
     
    	//	We actually could just return the timePercent value for a linear fade, but we want it to be smooth,
    	//	so return the smoothAlpha.
    	return smoothAlpha;
    }
     
    @script RequireComponent(GUIText)Javascript - TimedFadeTextMESH.js //	TimedFadeTextMESH.js
    //	From the Unity Wiki (modified for TextMesh)
    //	Use with MessageList.js
    //	Use on a TextMesh PreFab
    //  This PreFab will be instantiated by MessageList.js
     
    var liveTime : float = 5.0;					//	The number of seconds the GUIText will last before starting to fade
    var fadeTime : float = 2.0;					//	The number of seconds to fade until totally transparent
     
    private var time : float = 0.0;				//	Static var to track how much time has passed
    private var isFading : boolean = false;		//	Static var to track if we're in the fading stage
    private var startAlpha : float = 1.0;		//	Static var to keep track of the initial amount of alpha
     
    function Start () {
    	//	This script uses the GUIText's material to set the alpha fade.
    	//	If the font doesn't have a material, then this won't work, so disable the script.
    	if (!renderer.material) {
    		Debug.Log ("You're Screwed!");
    		enabled = false;
    	}
     
    	//	Get the starting alpha value.
    	//	If the developer has the text start transparent, then we need to fade from that point.
    	startAlpha = renderer.material.color.a;
    //	renderer.material
    }
     
    function Update () {
    	//	Update our time var to keep track of how much time has passed.
    	time += Time.deltaTime;
     
    	if (isFading) {
    		//	We're in the fading stage. If we've reached the end of this stage, then destroy the gameObject.
    		if (time >= fadeTime) {
    			Destroy(gameObject);
    		}
     
    		//	We're still fading, so update the material's alpha color to make it fade a little more.
    		renderer.material.color.a = CalculateAlpha();
    	}
     
    	else if (time >= liveTime) {
    		//	If we're not fading yet, but should be, then update our values to proceed to the fading stage.
    		isFading = true;
    		time = 0.0;
    	}
     
    	//	If we're not fading yet, and don't need to be yet, then nothign will happen at this point. The
    	//	text will just exist, and the timer will keep incrementing until there's a state change.
    }
     
    //	CalculateAlpha() simple takes the static global vars we're using to keep track of everything
    //	to figure out our current alpha value from 0 to 1.
    function CalculateAlpha() {
    	//	Find out the percent of time from 0 to 1 that has gone between when the text starts and stops fading
    	var timePercent : float = Mathf.Clamp01((fadeTime - time) / fadeTime);
    	//	Generate a nice, smooth value from 1 to 0 to represent how faded the text is
    	var smoothAlpha : float = Mathf.SmoothStep(0.0, startAlpha, timePercent);
     
    	//	We actually could just return the timePercent value for a linear fade, but we want it to be smooth,
    	//	so return the smoothAlpha.
    	return smoothAlpha;
    }C# - MessageList.cs Conversion : CorrodedSoul
    
    The exact syntax of this version is: 
    MessageList.Instance.AddMessage(string "string")
    
    //  MessageList.cs
    //  From the Unity Wiki
    //  Use with TimedFadeText.cs
    //  Attach to an emtpy Game Object
    //  Based on the work of capnbishop
    //  Conversion to csharp by CorrodedSoul
     
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;									//needed to replace the Javascript arrays with List<>.
     
    public class MessageList : MonoBehaviour {
    	public GUIText messagePrefab;									//The prefab for our text object;
     
    	public float lineSize = 20.0f;									//Pixel spacing between lines;
    	public Vector3 startingPos = new  Vector3(20, 20, 0);
    	public int layerTag = 0;
    	public bool insertAbove = true;
     
    	private List<GUIText> _messages;								//Using a List<> instead of a JS dynamic array
    	private float _directionFactor = 1.0f;
     
    #region Singleton
    	/// <summary>
    	///   Provide singleton support for this class.
        ///   The script must still be attached to a game object, but this will allow it to be called
        ///   from anywhere without specifically identifying that game object.
    	/// </summary>
    	private static MessageList instance;
     
    	public static MessageList Instance{
    		get{
    			if (instance == null){
    				instance = (MessageList)FindObjectOfType(typeof(MessageList));
    				if (instance == null)
    					Debug.LogError("There needs to be one active MessageList script on a GameObject in your scene.");
    			}
    			return instance;
    		}
    	}
    #endregion
     
    	void Awake() {
    		// First make sure we have a prefab set. If not, disable the script
    		if (messagePrefab == null){
    			enabled = false;
    			Debug.LogWarning("Must set the GUIText prefab for MessageList");
    		}
     
    		if (insertAbove) {
    			_directionFactor = 1.0f;
    		}else{
    			_directionFactor = -1.0f;
    		}
    		_messages = new List<GUIText>();
    	}
     
     
    /// <summary>
    /// AddMessage() accepts a text value and adds it as a status message.
    /// All other status messages will be moved along the y axis by a normalized distance of lineSize.
    /// AddMessage() also handles automatic removing of any GUIText objects that automatically destroy
    /// themselves.
    /// </summary>
    	public void AddMessage(string messageText) {
    		// Iterate though the messages, removing any that don't exist anymore, and moving the rest
    		for (int i = 0; i < _messages.Count; i++){
    			// If this message is null, remove it, drop back the index count, and jump back to the begining of the loop
    			if (_messages[i]== null){
    				_messages.RemoveAt(i);
    				i--;
    				continue;
    			}
     
    			_messages[i].transform.position += new Vector3(0,_directionFactor * (lineSize/Screen.height),0);
    		}
    		//  All the existing messages have been moved, making room for the new one.
    		//  Instantiate a new message from the prefab, set it's text value, and add it to the
    		//  array of messages.
     
    		GUIText newMessage;
    		newMessage = Instantiate(messagePrefab, new Vector3(startingPos.x/Screen.width, startingPos.y/Screen.height,startingPos.z),transform.rotation) as GUIText;
    		newMessage.text = messageText;
    		newMessage.gameObject.layer = layerTag;
    		_messages.Add(newMessage);
    	}
    }C# - TimedFadeText.cs //  TimedFadeText.cs
    //  From the Unity Wiki
    //  Use with MessageList.cs
    //  Use on a GUIText Object saved as a PreFab
    //  This PreFab will be instantiated by MessageList.cs
    //  Check the GUIText Object settings for alignment
    //  Check the MessageList.cs settings for placement on screen
    //  Based on the work of capnbishop
    //  Conversion to csharp by CorrodedSoul
     
    using UnityEngine;
    using System.Collections;
     
    [RequireComponent (typeof (GUIText))]
    public class TimedFadeText : MonoBehaviour{
     
    	public float lifeTime = 5.0f;			// The number of seconds the GUIText will last before starting to fade
    	public float fadeTime = 2.0f;			// The number of seconds to fade until totally transparent
     
    	private float _time = 0.0f;				// Static var to track how much time has passed
    	private bool _isFading = false;			// Static var to track if we're in the fading stage
    	private float _startAlpha = 1.0f;		// Static var to keep track of the initial amount of alpha
    	private GUIText _guiText;
    //	private float _timePercent;
     
    	void Start() {
     
    		// This script uses the GUIText's material to set the alpha fade.
    		// If the font doesn't have a material, then this won't work, so disable the script.
    		_guiText = this.gameObject.GetComponent<GUIText>();
    		if (_guiText.material == null){
    			Debug.LogWarning("GUIText material missing");	//Changed the comment from original
    			enabled = false;
    		}
     
    		// Get the starting alpha value.
    		// If the developer has the text start transparent, then we need to fade from that point.
    		_startAlpha = _guiText.material.color.a;
    	}
     
    	void Update() {
    		_time += Time.deltaTime;
     
    		if (_isFading){
    			//  We're in the fading stage. If we've reached the end of this stage, then destroy the gameObject.
    			if ( _time >= fadeTime ) {
    				Destroy(gameObject);
    			}
     
    			//  We're still fading, so update the material's alpha color to make it fade a little more.
    			_guiText.material.color = new Color(_guiText.material.color.r,_guiText.material.color.g,_guiText.material.color.b,CalcutateAlpha());
     
    		}else if (_time >= lifeTime ) {
    			//  If we're not fading yet, but should be, then update our values to proceed to the fading stage.
    			_isFading = true;
    			_time = 0.0f;
    		}
     
    		//  If we're not fading yet, and don't need to be yet, then nothign will happen at this point. The
        	//  text will just exist, and the timer will keep incrementing until there's a state change.
    	}
     
    	private float CalcutateAlpha() {
    		float timePercent;
    		float smoothAlpha;
     
    		timePercent = Mathf.Clamp01((fadeTime-_time)/fadeTime);
    		smoothAlpha = Mathf.SmoothStep(0.0f,_startAlpha,timePercent);
     
    		return smoothAlpha;
     
    	}
}
}
