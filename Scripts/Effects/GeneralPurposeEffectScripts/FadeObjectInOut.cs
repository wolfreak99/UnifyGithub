/*************************
 * Original url: http://wiki.unity3d.com/index.php/FadeObjectInOut
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/FadeObjectInOut.cs
 * File based on original modification date of: 14 July 2015, at 10:31. 
 *
 * Author: Hayden Scott-Baron (Dock) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Contents [hide] 
    1 Description 
    2 Updated 
    3 Usage 
    4 Technical Discussion 
    5 C# - FadeObjectInOut.cs 
    
    DescriptionThis allows you to easily fade an object and its children. If an object is already partially faded it will continue from there. If you choose a different speed, it will use the new speed. 
    It's especially useful for GUI style objects, but can be used on 3D objects in the scene as well. 
    UpdatedUpdated 7th March 2013 with the option of pausing before the fade occurs, and a bunch of fixes to 'Fade In On Start'. 
    UsagePlace this script on the gameobject you wish to fade, and call 'FadeIn()' or 'FadeOut()'. Adjust fading time with 'FadeIn(4.0f)', or by adjusting the public time variable. 
    Technical DiscussionThis requires objects to have materials that allow for alpha transparency. 
    C# - FadeObjectInOut.cs/*
    	FadeObjectInOut.cs
     	Hayden Scott-Baron (Dock) - http://starfruitgames.com
     	6 Dec 2012 
     
    	This allows you to easily fade an object and its children. 
    	If an object is already partially faded it will continue from there. 
    	If you choose a different speed, it will use the new speed. 
     
    	NOTE: Requires materials with a shader that allows transparency through color.  
    */
     
    using UnityEngine;
    using System.Collections;
     
    public class FadeObjectInOut : MonoBehaviour
    {
     
    	// publically editable speed
    	public float fadeDelay = 0.0f; 
    	public float fadeTime = 0.5f; 
    	public bool fadeInOnStart = false; 
    	public bool fadeOutOnStart = false;
    	private bool logInitialFadeSequence = false; 
     
     
     
     
    	// store colours
    	private Color[] colors; 
     
    	// allow automatic fading on the start of the scene
    	IEnumerator Start ()
    	{
    		//yield return null; 
    		yield return new WaitForSeconds (fadeDelay); 
     
    		if (fadeInOnStart)
    		{
    			logInitialFadeSequence = true; 
    			FadeIn (); 
    		}
     
    		if (fadeOutOnStart)
    		{
    			FadeOut (fadeTime); 
    		}
    	}
     
     
     
     
    	// check the alpha value of most opaque object
    	float MaxAlpha()
    	{
    		float maxAlpha = 0.0f; 
    		Renderer[] rendererObjects = GetComponentsInChildren<Renderer>(); 
    		foreach (Renderer item in rendererObjects)
    		{
    			maxAlpha = Mathf.Max (maxAlpha, item.material.color.a); 
    		}
    		return maxAlpha; 
    	}
     
    	// fade sequence
    	IEnumerator FadeSequence (float fadingOutTime)
    	{
    		// log fading direction, then precalculate fading speed as a multiplier
    		bool fadingOut = (fadingOutTime < 0.0f);
    		float fadingOutSpeed = 1.0f / fadingOutTime; 
     
    		// grab all child objects
    		Renderer[] rendererObjects = GetComponentsInChildren<Renderer>(); 
    		if (colors == null)
    		{
    			//create a cache of colors if necessary
    			colors = new Color[rendererObjects.Length]; 
     
    			// store the original colours for all child objects
    			for (int i = 0; i < rendererObjects.Length; i++)
    			{
    				colors[i] = rendererObjects[i].material.color; 
    			}
    		}
     
    		// make all objects visible
    		for (int i = 0; i < rendererObjects.Length; i++)
    		{
    			rendererObjects[i].enabled = true;
    		}
     
     
    		// get current max alpha
    		float alphaValue = MaxAlpha();  
     
     
    		// This is a special case for objects that are set to fade in on start. 
    		// it will treat them as alpha 0, despite them not being so. 
    		if (logInitialFadeSequence && !fadingOut)
    		{
    			alphaValue = 0.0f; 
    			logInitialFadeSequence = false; 
    		}
     
    		// iterate to change alpha value 
    		while ( (alphaValue >= 0.0f && fadingOut) || (alphaValue <= 1.0f && !fadingOut)) 
    		{
    			alphaValue += Time.deltaTime * fadingOutSpeed; 
     
    			for (int i = 0; i < rendererObjects.Length; i++)
    			{
    				Color newColor = (colors != null ? colors[i] : rendererObjects[i].material.color);
    				newColor.a = Mathf.Min ( newColor.a, alphaValue ); 
    				newColor.a = Mathf.Clamp (newColor.a, 0.0f, 1.0f); 				
    				rendererObjects[i].material.SetColor("_Color", newColor) ; 
    			}
     
    			yield return null; 
    		}
     
    		// turn objects off after fading out
    		if (fadingOut)
    		{
    			for (int i = 0; i < rendererObjects.Length; i++)
    			{
    				rendererObjects[i].enabled = false; 
    			}
    		}
     
     
    		Debug.Log ("fade sequence end : " + fadingOut); 
     
    	}
     
     
    	void FadeIn ()
    	{
    		FadeIn (fadeTime); 
    	}
     
    	void FadeOut ()
    	{
    		FadeOut (fadeTime); 		
    	}
     
    	void FadeIn (float newFadeTime)
    	{
    		StopAllCoroutines(); 
    		StartCoroutine("FadeSequence", newFadeTime); 
    	}
     
    	void FadeOut (float newFadeTime)
    	{
    		StopAllCoroutines(); 
    		StartCoroutine("FadeSequence", -newFadeTime); 
    	}
     
     
    	// These are for testing only. 
    //		void Update()
    //		{
    //			if (Input.GetKeyDown (KeyCode.Alpha0) )
    //			{
    //				FadeIn();
    //			}
    //			if (Input.GetKeyDown (KeyCode.Alpha9) )
    //			{
    //				FadeOut(); 
    //			}
    //		}
     
     
}
}
