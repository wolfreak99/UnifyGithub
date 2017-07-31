// Original url: http://wiki.unity3d.com/index.php/Tween
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/AnimationControllers/Tween.cs
// File based on original modification date of: 31 March 2015, at 13:28. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.AnimationControllers
{
DescriptionAn adaptation of the Tweener class of FLASH for unity. 
UsageExample use (attach this script to a game object): 
The array that will apear on your Object as Positions/Rotations/Alphas have 5 parameter that works this way: 
Total Time: How long this action will take. 
Delay: How long this action will wait to be started. (if this action have one or more actions before it, you might wanna add the amount of Total Time of those here) 
Ease: What kind of behaviour would you like to have on this "tween" 
Tween Value: The final Translation/Rotation/Alpha of the element. 
Loop Array: Will it loop? 


using UnityEngine;
using System.Collections;
 
//Tween position object class
[System.Serializable]
public class TweenPositionObject : BaseTweenObject
{
	public Vector3 tweenValue;
 
	private Vector3 _startValue;
	public Vector3 startValue
	{
		set{_startValue = value;}
		get{return _startValue;}
	}
 
	public TweenPositionObject ()
	{
		this.tweenType = TweenType.TweenPosition;
	}	
}
 
//Tween rotation object class
[System.Serializable]
public class TweenRotationObject : BaseTweenObject
{
	public Vector3 tweenValue;
 
	private Vector3 _startValue;
	public Vector3 startValue
	{
		set{_startValue = value;}
		get{return _startValue;}
	}
 
	public TweenRotationObject ()
	{
		this.tweenType = TweenType.TweenRotation;
	}	
}
 
//Tween rotation object class
[System.Serializable]
public class TweenAlphaObject : BaseTweenObject
{
	public float tweenValue;
 
	private float _startValue;
	public float startValue
	{
		set{_startValue = value;}
		get{return _startValue;}
	}
 
	public TweenAlphaObject ()
	{
		this.tweenType = TweenType.TweenAlpha;
	}	
}
 
 
 
//Engine class
public class UnityTween : MonoBehaviour {
 
 
	public TweenPositionObject[] positions =  new TweenPositionObject[0];
	public TweenRotationObject[] rotations =  new TweenRotationObject[0];
	public TweenAlphaObject[] alphas =  new TweenAlphaObject[0];
 
	public bool loopArray;
 
	private ArrayList tweens;
 
 
	void Start () {
 
		this.tweens = new ArrayList();
		this.AddTweens();
	}
 
	private void AddTweens ()
	{
		foreach(TweenPositionObject tween in positions)
		{
			TweenPosition(tween);
		}
		foreach(TweenRotationObject tween in rotations)
		{
			TweenRotation(tween);
		}
		foreach(TweenAlphaObject tween in alphas)
		{
			TweenAlpha(tween);
		}
	}
	//Add Tween in arrayList
	//Tween position
	public void TweenPosition (TweenPositionObject obj)
	{
		TweenPositionObject tween = new TweenPositionObject();
 
		tween.startTime = Time.time;
		tween.CopyTween(obj);
		tween.tweenValue = obj.tweenValue;
		tween.Init();
 
		this.tweens.Add(tween);
	}
	//Tween rotation
	public void TweenRotation (TweenRotationObject obj)
	{
		TweenRotationObject tween = new TweenRotationObject();
 
		tween.startTime = Time.time;
		tween.CopyTween(obj);
		tween.tweenValue = obj.tweenValue;
		tween.Init();
 
		this.tweens.Add(tween);
	}
	//Tween alpha
	public void TweenAlpha (TweenAlphaObject obj)
	{
		TweenAlphaObject tween = new TweenAlphaObject();
 
		tween.startTime = Time.time;
		tween.CopyTween(obj);
		tween.tweenValue = obj.tweenValue;
		tween.Init();
 
		this.tweens.Add(tween);
	}
 
	//Clear Tweens with the same type
	private void ClearTweensSameType (BaseTweenObject obj)
	{
		foreach (BaseTweenObject tween in tweens)
		{
			if(tween.id != obj.id && tween.tweenType == obj.tweenType)
				tween.ended = true;
		}
	}
 
	//Updates
	void Update ()
	{
		this.DetectDelay();
		this.UpdateTween();
	}
	//Detect when delay was passed
	private void DetectDelay ()
	{
		foreach (BaseTweenObject tween in tweens)
		{
			if(Time.time > tween.startTime + tween.delay && !tween.canStart)
			{
				if(tween.tweenType == TweenType.TweenPosition)
				{
					TweenPositionObject tweenPos = tween as TweenPositionObject;
					tweenPos.startValue = this.transform.position;
				}
				else if(tween.tweenType == TweenType.TweenRotation)
				{
					TweenRotationObject tweenRot = tween as TweenRotationObject;
					tweenRot.startValue = this.transform.rotation.eulerAngles;
				}
				else if(tween.tweenType == TweenType.TweenAlpha)
				{
					TweenAlphaObject tweenAlpha = tween as TweenAlphaObject;
					if(guiTexture != null)
						tweenAlpha.startValue = guiTexture.color.a;
					else
						tweenAlpha.startValue = this.renderer.material.color.a;
				}
 
				this.ClearTweensSameType(tween);
 
				tween.canStart = true;
			}
		}
	}
	//Update tween by type
	private void UpdateTween ()
	{
 
		int tweenCompleted = 0;
		foreach (BaseTweenObject tween in tweens)
		{
			if(tween.canStart && !tween.ended)
			{
				if(tween.tweenType == TweenType.TweenPosition)
					UpdatePosition(tween as TweenPositionObject);
				else if(tween.tweenType == TweenType.TweenRotation)
					UpdateRotation(tween as TweenRotationObject);
				else if(tween.tweenType == TweenType.TweenAlpha)
					UpdateAlpha(tween as TweenAlphaObject);	
 
			}
			if(tween.ended)
				tweenCompleted++;
 
			if(tweenCompleted == tweens.Count && loopArray)
				this.MakeLoop ();
 
 
		}
	}
 
	private void MakeLoop ()
	{
		foreach (BaseTweenObject tween in tweens)
		{
			tween.ended = false;
			tween.canStart = false;
			tween.startTime = Time.time;			
		}
	}
 
 
 
	//Update Position
	private void UpdatePosition(TweenPositionObject tween)
	{
		Vector3 begin = tween.startValue;
		Vector3 finish =  tween.tweenValue; 
		Vector3 change =  finish - begin;
		float duration = tween.totalTime; 
		float currentTime = Time.time - (tween.startTime + tween.delay);	
 
		if(duration == 0)
		{
			this.EndTween(tween);
			this.transform.position = finish;
			return;
		}
 
 
		if(Time.time  > tween.startTime + tween.delay + duration)
			this.EndTween(tween);
 
		this.transform.position = Equations.ChangeVector(currentTime, begin, change ,duration, tween.ease);
	}
	//Update Rotation
	private void UpdateRotation(TweenRotationObject tween)
	{
		Vector3 begin = tween.startValue;
		Vector3 finish =  tween.tweenValue; 
		Vector3 change =  finish - begin;
		float duration = tween.totalTime; 
		float currentTime = Time.time - (tween.startTime + tween.delay);	
 
		if(duration == 0)
		{
			this.EndTween(tween);
			this.transform.position = finish;
			return;
		}
 
 
		if(Time.time  > tween.startTime + tween.delay + duration)
			this.EndTween(tween);
 
		this.transform.rotation = Quaternion.Euler(Equations.ChangeVector(currentTime, begin, change ,duration, tween.ease));
	}
	//Update Alpha
	private void UpdateAlpha(TweenAlphaObject tween)
	{
		float begin = tween.startValue;
		float finish =  tween.tweenValue; 
		float change =  finish - begin;
		float duration = tween.totalTime; 
		float currentTime = Time.time - (tween.startTime + tween.delay);	
 
		float alpha = Equations.ChangeFloat(currentTime, begin, change ,duration, tween.ease);
		float redColor;
		float redGreen;
		float redBlue;
 
		if(guiTexture != null)
		{
			redColor = guiTexture.color.r;
			redGreen = guiTexture.color.g;
			redBlue = guiTexture.color.b;
 
			guiTexture.color = new Color(redColor,redGreen,redBlue,alpha);
 
			if(duration == 0)
			{
				this.EndTween(tween);
				guiTexture.color = new Color(redColor,redGreen,redBlue,finish);
				return;
			}
		}
		else
		{
			redColor = this.renderer.material.color.r;
			redGreen = this.renderer.material.color.g;
			redBlue = this.renderer.material.color.b;
 
			this.renderer.material.color = new Color(redColor,redGreen,redBlue,alpha);
 
			if(duration == 0)
			{
				this.EndTween(tween);
				this.renderer.material.color = new Color(redColor,redGreen,redBlue,finish);
				return;
			}
		}
 
		if(Time.time  > tween.startTime + tween.delay + duration)
			this.EndTween(tween);
	}	
 
	private void EndTween (BaseTweenObject tween)
	{
		tween.ended = true;
	}
}
You must have this classes on you script folder too. 


using UnityEngine;
using System.Collections;
 
 
public enum Ease {
	Linear = 0,
	EaseInQuad = 1,
	EaseOutQuad = 2,
	EaseInOutQuad = 3,
	EaseOutInQuad = 4,
	EaseInCubic = 5,
	EaseOutCubic = 6,
	EaseInOutCubic = 7,
	EaseOutInCubic = 8,
	EaseInQuart = 9, 
	EaseOutQuart = 10,  
	EaseInOutQuart = 11,  
	EaseOutInQuart = 12,  
	EaseInQuint = 13,  
	EaseOutQuint = 14,  
	EaseInOutQuint = 15,  
	EaseOutInQuint = 16,
	EaseInSine = 17, 
	EaseOutSine = 18, 
	EaseInOutSine = 19, 
	EaseOutInSine = 20, 
	EaseInExpo = 21,  
	EaseOutExpo = 22,  
	EaseInOutExpo = 23,  
	EaseOutInExpo = 24,
	EaseInCirc = 25,  
	EaseOutCirc = 26,  
	EaseInOutCirc = 27,  
	EaseOutInCirc = 28,  
	EaseInElastic = 29,  
	EaseOutElastic = 30,  
	EaseInOutElastic = 31,  
	EaseOutInElastic = 32,
	EaseInBack = 33, 
	EaseOutBack = 34, 
	EaseInOutBack = 35, 
	EaseOutInBack = 36, 
	EaseInBounce = 37, 
	EaseOutBounce = 38, 
	EaseInOutBounce = 39,  
	EaseOutInBounce = 40
}
 
public class Equations {
 
 
	// TWEENING EQUATIONS floats -----------------------------------------------------------------------------------------------------
	// (the original equations are Robert Penner's work as mentioned on the disclaimer)
 
	/**
	 * Easing equation float for a simple linear tweening, with no easing.
	 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
 
	public static float EaseNone (float t, float b, float c, float d) {
		return c * t / d + b;
	}
 
	/**
	 * Easing equation float for a quadratic (t^2) easing in: accelerating from zero velocity.
	 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseInQuad (float t, float b, float c, float d) {
		return c * (t/=d) * t + b;
	}
 
	/**
	 * Easing equation float for a quadratic (t^2) easing out: decelerating to zero velocity.
	 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseOutQuad (float t, float b, float c, float d) {
		return -c *(t/=d)*(t-2) + b;
	}
 
	/**
	 * Easing equation float for a quadratic (t^2) easing in/out: acceleration until halfway, then deceleration.
	 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseInOutQuad  (float t, float b, float c, float d) {
 
		if ((t/=d/2) < 1) return c/2*t*t + b;
 
		return -c/2 * ((--t)*(t-2) - 1) + b;
	}
 
	/**
	 * Easing equation float for a quadratic (t^2) easing out/in: deceleration until halfway, then acceleration.
	 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseOutInQuad (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutQuad (t*2, b, c/2, d);
		return EaseInQuad((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
	 * Easing equation float for a cubic (t^3) easing in: accelerating from zero velocity.
		 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseInCubic (float t, float b, float c, float d) {
		return c*(t/=d)*t*t + b;
	}
 
	/**
	 * Easing equation float for a cubic (t^3) easing out: decelerating from zero velocity.
		 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseOutCubic (float t, float b, float c, float d) {
		return c*((t=t/d-1)*t*t + 1) + b;
	}
 
	/**
	 * Easing equation float for a cubic (t^3) easing in/out: acceleration until halfway, then deceleration.
		 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseInOutCubic (float t, float b, float c, float d) {
		if ((t/=d/2) < 1) return c/2*t*t*t + b;
		return c/2*((t-=2)*t*t + 2) + b;
	}
 
	/**
	 * Easing equation float for a cubic (t^3) easing out/in: deceleration until halfway, then acceleration.
		 *
	 * @param t		Current time (in frames or seconds).
	 * @param b		Starting value.
	 * @param c		Change needed in value.
	 * @param d		Expected easing duration (in frames or seconds).
	 * @return		The correct value.
	 */
	public static float EaseOutInCubic (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutCubic (t*2, b, c/2, d);
		return EaseInCubic((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for a quartic (t^4) easing in: accelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInQuart (float t, float b, float c, float d) {
		return c*(t/=d)*t*t*t + b;
	}
 
	/**
		 * Easing equation float for a quartic (t^4) easing out: decelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutQuart (float t, float b, float c, float d) {
		return -c * ((t=t/d-1)*t*t*t - 1) + b;
	}
 
	/**
		 * Easing equation float for a quartic (t^4) easing in/out: acceleration until halfway, then deceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInOutQuart (float t, float b, float c, float d) {
		if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
		return -c/2 * ((t-=2)*t*t*t - 2) + b;
	}
 
	/**
		 * Easing equation float for a quartic (t^4) easing out/in: deceleration until halfway, then acceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutInQuart (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutQuart (t*2, b, c/2, d);
		return EaseInQuart((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for a quintic (t^5) easing in: accelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInQuint (float t, float b, float c, float d) {
		return c*(t/=d)*t*t*t*t + b;
	}
 
	/**
		 * Easing equation float for a quintic (t^5) easing out: decelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutQuint (float t, float b, float c, float d) {
		return c*((t=t/d-1)*t*t*t*t + 1) + b;
	}
 
	/**
		 * Easing equation float for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInOutQuint (float t, float b, float c, float d) {
		if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
		return c/2*((t-=2)*t*t*t*t + 2) + b;
	}
 
	/**
		 * Easing equation float for a quintic (t^5) easing out/in: deceleration until halfway, then acceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutInQuint (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutQuint (t*2, b, c/2, d);
		return EaseInQuint((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInSine (float t, float b, float c, float d) {
		return -c * Mathf.Cos(t/d * (Mathf.PI/2)) + c + b;
	}
 
	/**
		 * Easing equation float for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutSine (float t, float b, float c, float d) {
		return c * Mathf.Sin(t/d * (Mathf.PI/2)) + b;
	}
 
	/**
		 * Easing equation float for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInOutSine (float t, float b, float c, float d) {
		return -c/2 * (Mathf.Cos(Mathf.PI*t/d) - 1) + b;
	}
 
	/**
		 * Easing equation float for a sinusoidal (sin(t)) easing out/in: deceleration until halfway, then acceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutInSine (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutSine (t*2, b, c/2, d);
		return EaseInSine((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for an exponential (2^t) easing in: accelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInExpo (float t, float b, float c, float d) {
		return (t==0) ? b : c * Mathf.Pow(2, 10 * (t/d - 1)) + b - c * 0.001f;
	}
 
	/**
		 * Easing equation float for an exponential (2^t) easing out: decelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutExpo (float t, float b, float c, float d) {
		return (t==d) ? b+c : c * 1.001f * (-Mathf.Pow(2, -10 * t/d) + 1) + b;
	}
 
	/**
		 * Easing equation float for an exponential (2^t) easing in/out: acceleration until halfway, then deceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInOutExpo (float t, float b, float c, float d) {
		if (t==0) return b;
		if (t==d) return b+c;
		if ((t/=d/2) < 1) return c/2 * Mathf.Pow(2, 10 * (t - 1)) + b - c * 0.0005f;
		return c/2 * 1.0005f * (-Mathf.Pow(2, -10 * --t) + 2) + b;
	}
 
	/**
		 * Easing equation float for an exponential (2^t) easing out/in: deceleration until halfway, then acceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutInExpo (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutExpo (t*2, b, c/2, d);
		return EaseInExpo((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInCirc (float t, float b, float c, float d) {
		return -c * (Mathf.Sqrt(1 - (t/=d)*t) - 1) + b;
	}
 
	/**
		 * Easing equation float for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutCirc (float t, float b, float c, float d) {
		return c * Mathf.Sqrt(1 - (t=t/d-1)*t) + b;
	}
 
	/**
		 * Easing equation float for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.
 		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInOutCirc (float t, float b, float c, float d) {
		if ((t/=d/2) < 1) return -c/2 * (Mathf.Sqrt(1 - t*t) - 1) + b;
		return c/2 * (Mathf.Sqrt(1 - (t-=2)*t) + 1) + b;
	}
 
	/**
		 * Easing equation float for a circular (sqrt(1-t^2)) easing out/in: deceleration until halfway, then acceleration.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutInCirc (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutCirc (t*2, b, c/2, d);
		return EaseInCirc((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param a		Amplitude.
		 * @param p		Period.
		 * @return		The correct value.
		 */
	public static float EaseInElastic (float t, float b, float c, float d) {
		if (t==0) return b;
		if ((t/=d)==1) return b+c;
		float p =  d *.3f;
		float s = 0;
		float a = 0;
		if (a == 0f || a < Mathf.Abs(c)) {
			a = c;
			s = p/4;
		} else {
			s = p/(2*Mathf.PI) * Mathf.Asin (c/a);
		}
		return -(a*Mathf.Pow(2,10*(t-=1)) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )) + b;
	}
 
	/**
		 * Easing equation float for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param a		Amplitude.
		 * @param p		Period.
		 * @return		The correct value.
		 */
	public static float EaseOutElastic (float t, float b, float c, float d) {
		if (t==0) return b;
		if ((t/=d)==1) return b+c;
		float p = d*.3f;
		float s = 0;
		float a = 0;
		if (a == 0f || a < Mathf.Abs(c)) {
			a = c;
			s = p/4;
		} else {
			s = p/(2*Mathf.PI) * Mathf.Asin (c/a);
		}
		return (a*Mathf.Pow(2,-10*t) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p ) + c + b);
	}
 
	/**
		 * Easing equation float for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param a		Amplitude.
		 * @param p		Period.
		 * @return		The correct value.
		 */
	public static float EaseInOutElastic (float t, float b, float c, float d) {
		if (t==0) return b;
		if ((t/=d/2)==2) return b+c;
		float p =  d*(.3f*1.5f);
		float s = 0;
		float a = 0;
		if (a == 0f || a < Mathf.Abs(c)) {
			a = c;
			s = p/4;
		} else {
			s = p/(2*Mathf.PI) * Mathf.Asin (c/a);
		}
		if (t < 1) return -.5f*(a*Mathf.Pow(2,10*(t-=1)) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )) + b;
		return a*Mathf.Pow(2,-10*(t-=1)) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )*.5f + c + b;
	}
 
	/**
		 * Easing equation float for an elastic (exponentially decaying sine wave) easing out/in: deceleration until halfway, then acceleration.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param a		Amplitude.
		 * @param p		Period.
		 * @return		The correct value.
		 */
	public static float EaseOutInElastic (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutElastic (t*2, b, c/2, d);
		return EaseInElastic((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
		 * @return		The correct value.
		 */
	public static float EaseInBack (float t, float b, float c, float d) {
		float s = 1.70158f;
		return c*(t/=d)*t*((s+1)*t - s) + b;
	}
 
	/**
		 * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
		 * @return		The correct value.
		 */
	public static float EaseOutBack (float t, float b, float c, float d) {
		float s = 1.70158f;
		return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
	}
 
	/**
		 * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway, then deceleration.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
		 * @return		The correct value.
		 */
	public static float EaseInOutBack (float t, float b, float c, float d) {
		float s =  1.70158f;
		if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525f))+1)*t - s)) + b;
		return c/2*((t-=2)*t*(((s*=(1.525f))+1)*t + s) + 2) + b;
	}
 
	/**
		 * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: deceleration until halfway, then acceleration.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param s		Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
		 * @return		The correct value.
		 */
	public static float EaseOutInBack (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutBack (t*2, b, c/2, d);
		return EaseInBack((t*2)-d, b+c/2, c/2, d);
	}
 
	/**
		 * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInBounce (float t, float b, float c, float d) {
		return c - EaseOutBounce (d-t, 0, c, d) + b;
	}
 
	/**
		 * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutBounce (float t, float b, float c, float d) {
		if ((t/=d) < (1/2.75f)) {
			return c*(7.5625f*t*t) + b;
		} else if (t < (2/2.75f)) {
			return c*(7.5625f*(t-=(1.5f/2.75f))*t + .75f) + b;
		} else if (t < (2.5f/2.75f)) {
			return c*(7.5625f*(t-=(2.25f/2.75f))*t + .9375f) + b;
		} else {
			return c*(7.5625f*(t-=(2.625f/2.75f))*t + .984375f) + b;
		}
	}
 
	/**
		 * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseInOutBounce (float t, float b, float c, float d) {
		if (t < d/2) return EaseInBounce (t*2, 0, c, d) * .5f + b;
		else return EaseOutBounce (t*2-d, 0, c, d) * .5f + c*.5f + b;
	}
 
	/**
		 * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing out/in: deceleration until halfway, then acceleration.
		 *
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @return		The correct value.
		 */
	public static float EaseOutInBounce (float t, float b, float c, float d) {
		if (t < d/2) return EaseOutBounce (t*2, b, c/2, d);
		return EaseInBounce((t*2)-d, b+c/2, c/2, d);
	}
 
 
 
	public static Vector3 ChangeVector(float t , Vector3 b , Vector3 c , float d , Ease ease)
	{
		float x = methods[(int)ease](t, b.x, c.x,d);
		float y = methods[(int)ease](t, b.y, c.y,d);
		float z = methods[(int)ease](t, b.z, c.z,d);
		return new Vector3(x,y,z);
	}
 
	delegate float EaseDelegate(float t, float b, float c, float d);
 
	static EaseDelegate[] methods = new EaseDelegate[]{
		EaseNone,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseOutInQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseOutInCubic,
		EaseInQuart, 
		EaseOutQuart,  
		EaseInOutQuart,  
		EaseOutInQuart,  
		EaseInQuint,  
		EaseOutQuint,  
		EaseInOutQuint,  
		EaseOutInQuint,
		EaseInSine, 
		EaseOutSine, 
		EaseInOutSine, 
		EaseOutInSine, 
		EaseInExpo,  
		EaseOutExpo,  
		EaseInOutExpo,  
		EaseOutInExpo,
		EaseInCirc,  
		EaseOutCirc,  
		EaseInOutCirc,  
		EaseOutInCirc,  
		EaseInElastic,  
		EaseOutElastic,  
		EaseInOutElastic,  
		EaseOutInElastic,
		EaseInBack, 
		EaseOutBack, 
		EaseInOutBack, 
		EaseOutInBack, 
		EaseInBounce, 
		EaseOutBounce, 
		EaseInOutBounce,  
		EaseOutInBounce
	};
 
		/**
		 * 
		 * @param t		Current time (in frames or seconds).
		 * @param b		Starting value.
		 * @param c		Change needed in value.
		 * @param d		Expected easing duration (in frames or seconds).
		 * @param Ease	EaseType
		 * @return		The correct value.
		 */
	public static float ChangeFloat(float t , float b , float c , float d , Ease ease)
	{
		return methods[(int)ease](t,b,c,d);
	}
 
 
}

using UnityEngine;
using System.Collections;
 
public enum TweenType {
	TweenPosition = 0,
	TweenRotation = 1,
	TweenAlpha = 2
}
 
 
public class BaseTweenObject {
 
	public float totalTime = 0;
	public float delay = 0;
	public Ease ease = Ease.Linear;	
 
	private TweenType _tweenType;
	public TweenType tweenType
	{
		set{_tweenType = value;}
		get{return _tweenType;}
	}
 
	private float _startTime;
	public float startTime
	{
		set{_startTime = value;}
		get{return _startTime;}
	}
 
	private bool _ended = false;
	public bool ended
	{
		set{_ended = value;}
		get{return _ended;}
	}
 
	private bool _canStart = false;
	public bool canStart
	{
		set{_canStart = value;}
		get{return _canStart;}
	}
 
	private string _id = "";
	public string id
	{
		set{_id = value;}
		get{return _id;}
	}
 
	public BaseTweenObject ()
	{
 
	}
	public void Init()
	{
		this.id = "tween" + Time.time.ToString();
	}
 
	public void CopyTween (BaseTweenObject tween)
	{
		this.totalTime = tween.totalTime;
		this.delay = tween.delay;
		this.ease = tween.ease;
		this.tweenType = tween.tweenType;
	}
 
 
 
}
}
