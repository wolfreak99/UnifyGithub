// Original url: http://wiki.unity3d.com/index.php/KeyCombo
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/KeyCombo.cs
// File based on original modification date of: 15 January 2014, at 22:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
This class makes it easy to detect multi-key combos in, for example, fighting games. Created by StarManta. 
Javascript: KeyCombo.jsclass KeyCombo
{
	var buttons : String[];
	private var currentIndex : int=0; //moves along the array as buttons are pressed
	var allowedTimeBetweenButtons : float = 0.3; //tweak as needed
	private var timeLastButtonPressed : float;
 
	function KeyCombo(b : String[])
	{
		buttons = b;
	}
 
	//usage: call this once a frame. when the combo has been completed, it will return true
	function Check() : boolean
	{
		if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex=0;
		if (currentIndex < buttons.length)
		{
			if ((buttons[currentIndex] == "down" && Input.GetAxisRaw("Vertical") == -1) ||
			(buttons[currentIndex] == "up" && Input.GetAxisRaw("Vertical") == 1) ||
			(buttons[currentIndex] == "left" && Input.GetAxisRaw("Horizontal") == -1) ||
			(buttons[currentIndex] == "right" && Input.GetAxisRaw("Horizontal") == 1) ||
			(buttons[currentIndex] != "down" &&  buttons[currentIndex] != "up" &&  buttons[currentIndex] != "left" &&  buttons[currentIndex] != "right" && Input.GetButtonDown(buttons[currentIndex])) )
			{
				timeLastButtonPressed = Time.time;
				currentIndex++;
			}
 
			if (currentIndex >= buttons.length)
			{
				currentIndex = 0;
				return true;
			}
			else return false;
		}
	}
}

Javascript Implementation (No filename requirement)private var falconPunch : KeyCombo = KeyCombo(["down","right","right"]);
private var falconKick : KeyCombo = KeyCombo(["down", "right", "Fire1"]);
 
function Update()
{
	if (falconPunch.Check())
	{
		// do the falcon punch
		Debug.Log("PUNCH");	
	}
	if (falconKick.Check())
	{
		// do the falcon kick
		Debug.Log("KICK");
	}
}This is C# version (AmIr_bobo) 
using UnityEngine;
public class KeyCombo
{
    public string[] buttons;
    private int currentIndex = 0; //moves along the array as buttons are pressed
 
    public float allowedTimeBetweenButtons = 0.3f; //tweak as needed
    private float timeLastButtonPressed;
 
    public KeyCombo(string[] b)
    {
        buttons = b;
    }
 
    //usage: call this once a frame. when the combo has been completed, it will return true
    public bool Check()
    {
        if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex = 0;
        {
            if (currentIndex < buttons.Length)
            {
                if ((buttons[currentIndex] == "down" && Input.GetAxisRaw("Vertical") == -1) ||
                (buttons[currentIndex] == "up" && Input.GetAxisRaw("Vertical") == 1) ||
                (buttons[currentIndex] == "left" && Input.GetAxisRaw("Vertical") == -1) ||
                (buttons[currentIndex] == "right" && Input.GetAxisRaw("Horizontal") == 1) ||
                (buttons[currentIndex] != "down" && buttons[currentIndex] != "up" && buttons[currentIndex] != "left" && buttons[currentIndex] != "right" && Input.GetButtonDown(buttons[currentIndex])))
                {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;
                }
 
                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    return true;
                }
                else return false;
            }
        }
 
        return false;
    }
}Usage: 
using UnityEngine;
using System.Collections;
 
public class Test : MonoBehaviour {
 
	private KeyCombo falconPunch= new KeyCombo(new string[] {"down", "right","right"});
	private KeyCombo falconKick= new KeyCombo(new string[] {"down", "right","Fire1"});
 
	void Update () {
		if (falconPunch.Check())
		{
			// do the falcon punch
			Debug.Log("PUNCH"); 
		}		
		if (falconKick.Check())
		{
			// do the falcon punch
			Debug.Log("KICK"); 
		}
	}
}


Here's a multi-player edit to allow flexible input across combos; same usage as the JS implementation above, just pass some more info to the constructor. Improvements by Ethan Redd 
Javascript: MultiKeyCombo.jsclass MultiKeyCombo{
 
        //Input axes as strings. Default to player 1.
	var vert:String = "P1Vertical";
	var hori:String = "P1Horizontal";
	var other:String = "P1Fire1";
 
	var buttons : String[];
	private var currentIndex : int=0; //moves along the array as buttons are pressed
	var allowedTimeBetweenButtons : float = 1; //tweak as needed
	private var timeLastButtonPressed : float;
 
	function MultiKeyCombo(b : String[], n:int){
		buttons = b;
		myPlayer = n;
 
                 //Which player did the combo?
		 switch(n){
			case 1: 
					hori = "P1Horizontal";
					vert = "P1Vertical";
					other = "P1Fire1";
				break;			
			case 2: 
				hori = "P2Horizontal";
				vert = "P2Vertical";
				other = "P2Fire1";	
				break;
                       //Add as many cases as you like :)
		}
	}
 
	// function KeyCombo(b : String[]){
		//If you forget the new constructor, it'll default to player 1
                // KeyCombo(b,1);
	// }
 
 
	//usage: call this once a frame. when the combo has been completed, it will return true
	function Check() : boolean
	{
		if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex=0;
		if (currentIndex < buttons.length)
		{
			if ((buttons[currentIndex] == "down" && Input.GetAxisRaw(vert) == -1) ||
			(buttons[currentIndex] == "up" && Input.GetAxisRaw(vert) == 1) ||
			(buttons[currentIndex] == "left" && Input.GetAxisRaw(hori) == -1) ||
			(buttons[currentIndex] == "right" && Input.GetAxisRaw(hori) == 1) ||
			(buttons[currentIndex] == "other"&&Input.GetButtonDown("Fire1"))||		
			(buttons[currentIndex] != "down" &&  buttons[currentIndex] != "up" &&  buttons[currentIndex] != "left" &&  buttons[currentIndex] != "right" && Input.GetButtonDown(buttons[currentIndex])) */)
			{
				timeLastButtonPressed = Time.time;
				currentIndex++;
			}
 
			if (currentIndex >= buttons.length)
			{
				currentIndex = 0;
				return true;
			}
			else return false;
		}
	}
}
}
