/*************************
 * Original url: http://wiki.unity3d.com/index.php/DelayedMessage
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MessagingSystems/DelayedMessage.cs
 * File based on original modification date of: 7 March 2013, at 12:11. 
 *
 * Author: Hayden Scott-Baron (Dock) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.MessagingSystems
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Technical Discussion 
    4 C# - DelayedMessage.cs 
    
    DescriptionA simple script to call a message on an object after a while. 
    UsagePlace this script on the gameobject you wish to have call a function later on. Useful for Destroying an old object, for example, or for allowing objects to repeatedly fade in/out using the FadeObjectInOut script. 
    Technical DiscussionSet the initial time to negative to reduce the initial wait. 
    C# - DelayedMessage.cs/*
    	DelayedMessage.cs
     	Hayden Scott-Baron (Dock) - http://starfruitgames.com
     	7 Mar 2013 
     
    	This uses 'SendMessage' on an object after a delay. 
     	You can send a value such as a string, int or float
     	You may also set this to repeat a message on an object. 
    */
     
    using UnityEngine;
    using System.Collections;
     
     
    public class DelayedMessage : MonoBehaviour 
    {
     
     
    	public enum MessageType
    	{
    		No_Values,
    		Send_Int,
    		Send_String,
    		Send_Float,
    	}
    	public MessageType typeOfMessage = MessageType.No_Values;  
     
    	public float delayTime = 1.0f; 
    	public string message = ""; 
     
    	public int valueInt; 
    	public float valueFloat; 
    	public string valueString; 
     
    	public SendMessageOptions messageOptions = SendMessageOptions.DontRequireReceiver; 
     
    	public float initialDelay = 0.0f; 	
    	public bool repeatMessage = false; 
     
     
    	private bool startNow = true; 
    	private float timer = 0.0f; 
     
    	IEnumerator MessageSequence ()
    	{
    		timer += delayTime; 
     
    		while (timer > 0.0f)
    		{
    			yield return null; 
    			timer -= Time.deltaTime; 
    		}
     
    		SendMessageNow (); 
     
    		if (repeatMessage)
    			startNow = true; 
    	}
     
    	void SendMessageNow()
    	{
    		switch (typeOfMessage)
    		{
    		case MessageType.No_Values:
    			SendMessage (message, messageOptions); 
    			break; 
     
    		case MessageType.Send_Float:
    			SendMessage (message, valueFloat, messageOptions); 
    			break; 
     
    		case MessageType.Send_Int:
    			SendMessage (message, valueInt, messageOptions); 
    			break; 
     
    		case MessageType.Send_String:
    			SendMessage (message, valueString, messageOptions); 
    			break; 
    		}
    	}
     
    	void Awake ()
    	{
    		if (message == "")
    			Debug.LogError ("Warning! No message on 'Delayed Message' object!", gameObject); 
     
    		timer += initialDelay;
    		startNow = true; 
    	}
     
    	void Update ()
    	{
    		if (startNow)
    		{
    			startNow = false; 
    			StartCoroutine ( "MessageSequence" ); 
    		}
    	}
     
     
}
}
