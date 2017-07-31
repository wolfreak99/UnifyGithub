// Original url: http://wiki.unity3d.com/index.php/OnMouseDown
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/OnMouseDown.cs
// File based on original modification date of: 18 November 2014, at 19:11. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{

Spin Off: OnTouch 
This Javascript sends OnMouseDown messages based on iPhone taps, so you can use OnMouseDown callbacks. Attach the script to the camera that is rendering the clickable (tappable) objects. Add a layer argument to the Raycast call if you need to avoid unnecessary intersections. A similar script can be implemented for OnMouseUp (an exercise for the reader). This is only for 3D objects with colliders (e.g. this is used for the 3D menus in HyperBowl), not for GUIText and GUITexture. 
function Update () {
   var hit : RaycastHit;
   for (var i = 0; i < iPhoneInput.touchCount; ++i) {
      if (iPhoneInput.GetTouch(i).phase == iPhoneTouchPhase.Began) {
      // Construct a ray from the current touch coordinates
      var ray = camera.ScreenPointToRay (iPhoneInput.GetTouch(i).position);
      if (Physics.Raycast (ray,hit)) {
         hit.transform.gameObject.SendMessage("OnMouseDown");
      }
   }
   }
}- 
Unity.js Updated for Unity3.0: 
//	OnTouchDown.js
//	Allows "OnMouseDown()" events to work on the iPhone.
//	Attack to the main camera.
 
#pragma strict
#pragma implicit
#pragma downcast
 
function Update () {
	// Code for OnMouseDown in the iPhone. Unquote to test.
	var hit : RaycastHit;
	for (var i = 0; i < Input.touchCount; ++i) {
		if (Input.GetTouch(i).phase == TouchPhase.Began) {
		// Construct a ray from the current touch coordinates
		var ray = camera.ScreenPointToRay (Input.GetTouch(i).position);
		if (Physics.Raycast (ray,hit)) {
			hit.transform.gameObject.SendMessage("OnMouseDown");
	      }
	   }
   }
}
C# Updated for Unity3.0: 
//	OnTouchDown.cs
//	Allows "OnMouseDown()" events to work on the iPhone.
//	Attack to the main camera.
 
#if UNITY_IPHONE
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class OnTouchDown : MonoBehaviour
{
	void Update () {
		// Code for OnMouseDown in the iPhone. Unquote to test.
		RaycastHit hit = new RaycastHit();
		for (int i = 0; i < Input.touchCount; ++i) {
			if (Input.GetTouch(i).phase.Equals(TouchPhase.Began)) {
			// Construct a ray from the current touch coordinates
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
			if (Physics.Raycast(ray, out hit)) {
				hit.transform.gameObject.SendMessage("OnMouseDown");
		      }
		   }
	   }
	}
}
#endif
}
