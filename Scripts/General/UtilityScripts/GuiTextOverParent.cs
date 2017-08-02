/*************************
 * Original url: http://wiki.unity3d.com/index.php/GuiTextOverParent
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/GuiTextOverParent.cs
 * File based on original modification date of: 7 September 2015, at 21:24. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    What is it A simple way of placing text over a parent object, say a 2D sprite, by Opless. 
    Code using UnityEngine;
    using System.Collections;
     
    [ExecuteInEditMode]
    public class GuiTextOverParent : MonoBehaviour
    {
     
    	// Use this for initialization
    	void Start ()
    	{
     
    	}
     
    	// Update is called once per frame
    	void Update ()
    	{
    		var parent = transform.parent;
     
    		if (parent == null) {
    			Debug.LogError ("No parent");
    			return;
    		}
    		var screenpoint = Camera.main.WorldToViewportPoint (parent.transform.position);
    		this.transform.position = screenpoint;
    	}
    }Instructions Create your GuiText component in a game object (you'll have to do this manually in Unity5) 
    Make it a child of a sprite 
    Voila it works 
}
