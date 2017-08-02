/*************************
 * Original url: http://wiki.unity3d.com/index.php/AnimatedText
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/AnimatedText.cs
 * File based on original modification date of: 19 January 2013, at 23:15. 
 *
 * Author: Bérenger 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Description Display a label animated on position, size and transparency. 
    Usage Place this script onto a GameObject. The movement animation curve determine the lerp between startRect and startrect + movement. You can call the animation from a reference on the script with the function Begin( string text ). 
    
    
    C# - AnimatedText.cs using UnityEngine;
    using System.Collections;
     
    //Display a label animated on position, size and transparency.
    public class AnimatedText : MonoBehaviour 
    {
    	public string txt = "Animated text";
    	public float time = 1f; // The length of the animation
    	public bool animatePos = true; // Does the position need to be animated ?
    	public bool animateAlpha = true; // Does the text need to fade ?
    	public bool animateSize = true; // Does the scale need to be animated ?
    	public bool playOnAwake = true; // Do you want to play the animation at start ?
    	public bool destroyWhenDone = true; // Do you want to destroy the gameObject when the animation is done ?
    	public Rect startRect = new Rect(10 ,10, 100, 100 ); // The start pos
    	public Vector2 movement = new Vector2(200, 0); // The endPos ( startRect + movement );
     
    	public AnimationCurve moveCurve; // The lerp between startRect and ( startRect + movement )
    	public AnimationCurve fadeCurve; // How is alpha behaving ?
    	public AnimationCurve sizeCurve; // How is the scale behaving ?
    	public GUIStyle style;
     
    	private float alpha = 1f;
    	private float scale = 1f;
    	private Rect currRect = new Rect(10 ,10, 100, 100 );
     
    	// Use this for initialization
    	void Awake () 
    	{
    		alpha = 0f;
    		if( playOnAwake ) Begin();
    	}
     
    	// Start the animation with the component's text
    	public void Begin(){ Begin(txt); }
    	// Start the animation with the argument
    	public void Begin(string str)
    	{
    		txt = str;
    		if( destroyWhenDone ) Destroy( gameObject, time );
    		StartCoroutine( Animate() );
    	}
     
    	// The animation coroutine
    	IEnumerator Animate()
    	{
    		float step = 1f / time;
    		float counter = 0f;
    		currRect = startRect;
    		alpha = 1f;
    		scale = 1f;
    		while( counter < time )
    		{
    			float s = counter * step;
    			float t = moveCurve.Evaluate( s );
     
    			// Fading
    			alpha = animateAlpha ? fadeCurve.Evaluate( s ) : 1f;
    			// Scaling
    			scale = animateSize ? sizeCurve.Evaluate( s ) : 1f;
    			// Moving
    			if( animatePos ) currRect = new Rect( Mathf.Lerp( startRect.x, startRect.x + movement.x, t ),
    			                    Mathf.Lerp( startRect.y, startRect.y + movement.y, t ),
    			                    startRect.width, startRect.height );
     
    			yield return null;
    			counter += Time.deltaTime;
    		}
    	}
     
    	void OnGUI()
    	{
    		GUI.color = new Color(1,1,1, alpha );
     
    	    Vector2 pivotPoint = new Vector2( currRect.x + currRect.width * 0.5F, currRect.y + currRect.height * 0.5F);
    	    GUIUtility.ScaleAroundPivot( scale * Vector2.one, pivotPoint ); 
     
    		GUI.Label( currRect, txt, style );
    	}
}
}
