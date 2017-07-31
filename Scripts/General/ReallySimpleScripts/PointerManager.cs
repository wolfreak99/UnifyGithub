// Original url: http://wiki.unity3d.com/index.php/PointerManager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/PointerManager.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Shannon 
Description This script will hide the pointer after two minutes if the mouse does not move for a designated period of time. It is also an example of how to use enum and switch define a state machine. 
Usage To use this script, attach it to any game object in your scene. 
CSharp - PointerManager.cs // PointerManager.cs
// July 30, 2008
// Shannon Ware (cc) 2008 Some Rights Reserved
 
using UnityEngine;
using System.Collections;
 
public class PointerManager : MonoBehaviour {
	// STATIC PARAMETERS
	public static enum PointerState {
		stateInvisible,
		stateVisible,
		stateSetInvisible,
		stateSetVisible
	}
 
	// PUBLIC VARIABLES
	public string version = "1.0.0";	// "[ReleaseID].[BetaID].[BuildID]"
	public PointerState pointerState = PointerState.stateSetInvisible;
	public float timeOutReset = 2f;
 
	// PRIVATE VARIABLES
	private bool hideOverride = false;
	private float timeOutCounter;
	private Vector3 currentMousePosition;
 
	// CLASS METHODS
 
 
	// EVENT HANDLERS
	void Start () {
		currentMousePosition = Input.mousePosition;
	}
 
	void Update () {
		switch (pointerState) {
			case PointerState.stateInvisible:
				if (!hideOverride)
					if (Input.mousePosition != currentMousePosition) 
						pointerState = PointerState.stateSetVisible;
			break;
			case PointerState.stateVisible:
				if (timeOutCounter > 0f) timeOutCounter -= Time.deltaTime;
				else pointerState = PointerState.stateSetInvisible;
			break;	
			case PointerState.stateSetInvisible:
				Screen.showCursor = false;
				currentMousePosition = Input.mousePosition;
				pointerState = PointerState.stateInvisible;
			break;
			case PointerState.stateSetVisible:
				timeOutCounter = timeOutReset;
				Screen.showCursor = true;
				pointerState = PointerState.stateVisible;
			break;	
 
		}
	}
}
}
