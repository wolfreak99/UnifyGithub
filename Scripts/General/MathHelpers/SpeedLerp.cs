/*************************
 * Original url: http://wiki.unity3d.com/index.php/SpeedLerp
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MathHelpers/SpeedLerp.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * Description 
 *   
 * Usage 
 *   
 * New Functions 
 *   
 * Unclamped Variations 
 *   
 * Benchmarks 
 *   
 * C# - MathS.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.MathHelpers
{
    DescriptionIn Unity 2.6, several functions, namely Vector3.Lerp, Vector4.Lerp, and Color.Lerp, were sped up a fair amount compared to the previous versions of Unity. Unfortunately, they—along with other Lerp functions—still aren't as fast as they could be. Perhaps this will be changed in the future, but as of Unity 3.5, replacing these with user-made functions results in a noticeable speed increase. 
    "Noticeable" is relative, though...in general usage, it's unlikely to be worth the bother of using this script. On the other hand, if you're running routines that operate on large datasets and/or typically loop hundreds of thousands of times or more, then it can be worthwhile. Basically, any time you really want to squeeze more speed out of your code. 
    This also adds two new functions: Vector2Lerp and SuperLerp. (Vector2.Lerp was added in Unity 3.0, but this implementation is a little faster.) 
    Usage Put the MathS script in Standard Assets; this way it can be called easily from Javascript and Boo (if you're not using Standard Assets, you'll have to make this folder). Several of the functions are drop-in replacements...Mathf.Lerp, for example, can be replaced with MathS.Lerp and nothing will change except for faster operation. (As in the iPhone 3GS, the "S" stands for "Speed". Cheesy, eh?) 
    The directly replaced functions are: MathS.Lerp, MathS.InverseLerp, and MathS.SmoothStep. In addition, you can replace Color.Lerp with MathS.ColorLerp. Vector3.Lerp can be replaced with MathS.Vector3Lerp. Vector4.Lerp can be replaced with MathS.Vector4Lerp. 
    New Functions Vector2.Lerp doesn't exist prior to Unity 3.0, but is included here in case you need it: 
    static function Vector2Lerp (from : float, to : float, value : float) : Vector2 
    Linearly interpolates between two vectors: from towards to by amount value, where value is clamped between [0...1]. 
    
    Another new function is MathS.SuperLerp. This takes the form: 
    static function SuperLerp (from : float, to : float, from2 : float, to2 : float, value : float) : float 
    It's the equivalent of doing this: 
    Mathf.Lerp (from, to, Mathf.InverseLerp (from2, to2, value)) 
    In other words, it's like Lerp, except the control parameter is an arbitrary range instead of only 0 through 1. The arbitrary range itself is controlled by value, which is clamped between from2 and to2. The reason for using SuperLerp is that it's faster and a little simpler than using the Lerp/InverseLerp combo. An example of usage, which would be useful for underwater effects: 
    function Update () {
    	// Lerps fog density from .02 when player is at y position 0 and above,
    	// though .4 when player is at y position -100 and below
    	RenderSettings.fogDensity = MathS.SuperLerp(.02, .4, 0.0, -100.0, transform.position.y);
    }Unclamped Variations Namely: MathS.LerpUnclamped, MathS.InverseLerpUnclamped, MathS.SmoothStepUnclamped, and MathS.SuperLerpUnclamped. As the names suggest, these are unclamped versions of the respective functions. Normally with Lerp and SmoothStep, the control (third) parameter is clamped between 0 and 1. With InverseLerp, the third parameter is clamped between the first two, and with SuperLerp, the fifth parameter is clamped between the third and fourth. But with the unclamped versions, there are no contraints. Other than that, they work the same. For example: 
    var foo = MathS.LerpUnclamped (100.0, 200.0, .5);As for the purpose, as you might guess, this is in the interest of still more speed. For example, if your code using Lerp is written in such a way that it guarantees the control parameter is never below 0 or above 1 anyway, then there's no reason for the Lerp function to waste time checking this every time it's called. 
    Warning: since there is no clamping, control values outside the 0..1 range will naturally return incorrect values. In the above example, using 1.5 instead of .5 will result in 250, instead of 200 like you'd normally get with Lerp. Of course, it's possible for this to actually be a feature rather than a limitation, depending on what you want to do with your code. 
    If you're wondering why there are no unclamped versions of ColorLerp, Vector3Lerp, and Vector4Lerp, see the benchmarks below...for some quite strange reason, the unclamped versions are actually a little slower. So there doesn't seem to be much point in including them. 
    Benchmarks The numbers below were generated by a simple loop iterating 10 million times, with an even mix of control variables inside and outside the allowed ranges. For example, Mathf.Lerp (100.0, 200.0, .5) and Mathf.Lerp (100.0, 200.0, 1.5). The tests were run a number of times and the results were averaged from all runs. Actual speed differences ouside of synthetic benchmarks will, of course, depend on a number of factors, such as what else your code is doing, CPU speed and type, version of Unity, etc. Making your own tests is encouraged in order to confirm that using this script is actually beneficial for your projects. (One of my apps makes heavy use of SuperLerp in particular, and switching from Mathf.Lerp/InverseLerp to the MathS.SuperLerp function was a big reason for some substantial speed gains between versions—the 3.5X faster rate does in fact seem to apply.) 
    X faster 
    Mathf.Lerp vs MathS.Lerp: 1.60 MathS.LerpUnclamped: 3.09 
    Mathf.SmoothStep vs MathS.SmoothStep: 1.70 MathS.SmoothStepUnclamped: 2.07 
    Mathf.InverseLerp vs MathS.InverseLerp: 1.56 MathS.InverseLerpUnclamped: 2.06 
    Color.Lerp vs MathS.ColorLerp: 1.36 MathS.ColorLerpUnclamped: 1.31 
    Vector3.Lerp vs MathS.Vector3Lerp: 1.49 MathS.Vector3LerpUnclamped: 1.36 
    Vector4.Lerp vs MathS.Vector4Lerp: 1.37 MathS.Vector4LerpUnclamped: 1.32 
    Mathf.Lerp(Mathf.InverseLerp) vs MathS.SuperLerp: 2.92 MathS.SuperLerpUnclamped: 3.52 
    C# - MathS.cs using UnityEngine;
     
    public class MathS {
     
    	public static float Lerp (float from, float to, float value) {
    		if (value < 0.0f)
    			return from;
    		else if (value > 1.0f)
    			return to;
    		return (to - from) * value + from;
    	}
     
    	public static float LerpUnclamped (float from, float to, float value) {
    		return (1.0f - value)*from + value*to;
    	}
     
    	public static float InverseLerp (float from, float to, float value) {
    		if (from < to) {
    			if (value < from)
    				return 0.0f;
    			else if (value > to)
    				return 1.0f;
    		}
    		else {
    			if (value < to)
    				return 1.0f;
    			else if (value > from)
    				return 0.0f;
    		}
    		return (value - from) / (to - from);
    	}
     
    	public static float InverseLerpUnclamped (float from, float to, float value) {
    		return (value - from) / (to - from);
    	}
     
    	public static float SmoothStep (float from, float to, float value) {
    		if (value < 0.0f)
    			return from;
    		else if (value > 1.0f)
    			return to;
    		value = value*value*(3.0f - 2.0f*value);
    		return (1.0f - value)*from + value*to;
    	}
     
    	public static float SmoothStepUnclamped (float from, float to, float value) {
    		value = value*value*(3.0f - 2.0f*value);
    		return (1.0f - value)*from + value*to;
    	}
     
    	public static float SuperLerp (float from, float to, float from2, float to2, float value) {
    		if (from2 < to2) {
    			if (value < from2)
    				value = from2;
    			else if (value > to2)
    				value = to2;
    		}
    		else {
    			if (value < to2)
    				value = to2;
    			else if (value > from2)
    				value = from2;	
    		}
    		return (to - from) * ((value - from2) / (to2 - from2)) + from;
    	}
     
    	public static float SuperLerpUnclamped (float from, float to, float from2, float to2, float value) {
    		return (to - from) * ((value - from2) / (to2 - from2)) + from;
    	}
     
    	public static Color ColorLerp (Color c1, Color c2, float value) {
    		if (value > 1.0f)
    			return c2;
    		else if (value < 0.0f)
    			return c1;
    		return new Color (	c1.r + (c2.r - c1.r)*value, 
    							c1.g + (c2.g - c1.g)*value, 
    							c1.b + (c2.b - c1.b)*value, 
    							c1.a + (c2.a - c1.a)*value );
    	}
     
    	public static Vector2 Vector2Lerp (Vector2 v1, Vector2 v2, float value) {
    		if (value > 1.0f)
    			return v2;
    		else if (value < 0.0f)
    			return v1;
    		return new Vector2 (v1.x + (v2.x - v1.x)*value, 
    							v1.y + (v2.y - v1.y)*value );		
    	}
     
    	public static Vector3 Vector3Lerp (Vector3 v1, Vector3 v2, float value) {
    		if (value > 1.0f)
    			return v2;
    		else if (value < 0.0f)
    			return v1;
    		return new Vector3 (v1.x + (v2.x - v1.x)*value, 
    							v1.y + (v2.y - v1.y)*value, 
    							v1.z + (v2.z - v1.z)*value );
    	}
     
    	public static Vector4 Vector4Lerp (Vector4 v1, Vector4 v2, float value) {
    		if (value > 1.0f)
    			return v2;
    		else if (value < 0.0f)
    			return v1;
    		return new Vector4 (v1.x + (v2.x - v1.x)*value, 
    							v1.y + (v2.y - v1.y)*value, 
    							v1.z + (v2.z - v1.z)*value,
    							v1.w + (v2.w - v1.w)*value );
    	}
     
}
}
