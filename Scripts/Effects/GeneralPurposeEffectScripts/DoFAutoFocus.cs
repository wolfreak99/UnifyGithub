// Original url: http://wiki.unity3d.com/index.php/DoFAutoFocus
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/DoFAutoFocus.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author : Frank Otto | http://fosion.de 
Description Simple Autofocus System, with linear Interpolation of the Focus Point. 
Dependencies : Unity Pro Depth of Field (DoF) Image Effect 
Usage Drop DoFAutoFocus onto your Camera, be sure you have imported the ImageEffects. Adjust your DoF Settings. Script creates a new Gameobject "DoFFocusTarget" and asigns it to your DoF Image Effect. Enable interpolateFocus to use linear interpolation for the focus point. Switch Quality between NORMAL and HIGH changes the focus call from the FixedUpdate to the Update Function. Have Fun! --fosiOn a true vision 08:10, 16 May 2011 (PDT) 
C# - DoFAutoFocus.cs using UnityEngine;
using System.Collections;
using System;
 
/// <summary>
/// PlayFM
/// DoFAutofocus.cs
/// 
/// HIGH Quality checks every Frame
/// NORMAL Quality in Fixed Update
/// 
/// BMBF Researchproject
/// PlayFM - Serious Games für den IT-gestützten Wissenstransfer im Facility Management 
///	Gefördert durch das bmb+f - Programm Forschung an Fachhochschulen profUntFH
/// http://playFM.htw-berlin.de
///	
///	<author>Frank.Otto@htw-berlin.de</author>
///
/// </summary>
 
 
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(DepthOfField))]
public class DoFAutoFocus : MonoBehaviour
{
 
	private GameObject doFFocusTarget;
	private Vector3 lastDoFPoint;
	private DepthOfField dofComponent;
 
	public DoFAFocusQuality focusQuality = DoFAutoFocus.DoFAFocusQuality.NORMAL;
	public LayerMask hitLayer = 1;
	public float maxDistance = 100.0f;
	public bool interpolateFocus = false;
	public float interpolationTime = 0.7f;
 
	public enum DoFAFocusQuality
	{
		NORMAL,
		HIGH
	}
 
	/// <summary>
	/// Init all needed objects
	/// </summary>
	void Start ()
	{
		doFFocusTarget = new GameObject ("DoFFocusTarget");
		dofComponent = gameObject.GetComponent<DepthOfField> ();
		dofComponent.objectFocus = doFFocusTarget.transform;
	}
	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
 
		// switch between Modes Test Focus every Frame
		if (focusQuality == DoFAutoFocus.DoFAFocusQuality.HIGH) {
			Focus ();
		}
 
	}
 
	void FixedUpdate ()
	{
		// switch between modes Test Focus like the Physicsupdate
		if (focusQuality == DoFAutoFocus.DoFAFocusQuality.NORMAL) {
			Focus ();
		}
	}
 
 
	/// <summary>
	/// Interpolate DoF Target
	/// </summary>
	/// <param name="targetPosition">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="IEnumerator"/>
	/// </returns>	
	IEnumerator InterpolateFocus (Vector3 targetPosition)
	{
 
		Vector3 start = this.doFFocusTarget.transform.position;
		Vector3 end = targetPosition;
		float dTime = 0;
 
		Debug.DrawLine (start, end,Color.green);
 
		while (dTime < 1) {
			yield return null;
			//new WaitForEndOfFrame();
			dTime += Time.deltaTime / this.interpolationTime;
			this.doFFocusTarget.transform.position = Vector3.Lerp (start, end, dTime);
		}
		this.doFFocusTarget.transform.position = end;
	}
 
	/// <summary>
	/// Raycasts the focus point
	/// </summary>
	void Focus ()
	{
		// our ray
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, this.maxDistance, this.hitLayer)) {
			Debug.DrawLine (ray.origin, hit.point);
 
			// do we have a new point?					
			if (this.lastDoFPoint == hit.point) {
				return;
				// No, do nothing
			} else if (this.interpolateFocus) { // Do we interpolate from last point to the new Focus Point ?
				// stop the Coroutine
				StopCoroutine ("InterpolateFocus");
				// start new Coroutine
				StartCoroutine (InterpolateFocus (hit.point));
 
			} else {
				this.doFFocusTarget.transform.position = hit.point;
			}
			// asign the last hit
			this.lastDoFPoint = hit.point;
		}
	}
 
}
}
