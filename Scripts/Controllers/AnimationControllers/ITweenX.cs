/*************************
 * Original url: http://wiki.unity3d.com/index.php/ITweenX
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/AnimationControllers/ITweenX.cs
 * File based on original modification date of: 19 November 2012, at 17:10. 
 *
 * Author: Berenger 
 *
 * Description 
 *   
 * Usage 
 *   
 * Example 
 *   
 * C# - iTweenX.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.AnimationControllers
{
    
    Description Bunch of variables to use with iTween instead of searching the keywords every times. Here is a usefull link to preview the easetypes : http://www.robertpenner.com/easing/easing_demo.html 
    Usage Put this script in your Standard Assets/Scripts folder; this way it can be easily used from javascript or Boo. If you declare a public var of type EaseType, you can easily pick the right easetype from the inspector. 
    
    
    Example public class MyClass: MonoBehaviour
    {
    	public EaseType easeType;
     
    	void Start()
    	{
    		iTween.ValueTo( gameObject, iTween.Hash( iTweenX.from, 0f, iTweenX.to, 1f, iTweenX.time, 1f,
                                                     iTweenX.onUpdate, "OnUpdateValue",
                                                     iTweenX.onStart, "OnStart",
                                                     iTweenX.onComplete, "OnComplete",
                                                     iTweenX.easeType, iTweenX.Ease(easeType),
                                                     iTweenX.ignoreTimescale, true ) );
    	}
    	void OnUpdateValue(float value){ print( "OnUpdateValue : " + value.ToString() ); }
    	void OnStart(){ print( "OnStart" ); }
    	void OnComplete(){ print( "OnComplete" ); }
    }
    
    C# - iTweenX.cs public enum EaseType
    {
    	EaseInQuad = 0,
    	EaseOutQuad,    
    	EaseInOutQuad,  
    	EaseInCubic,    
    	EaseOutCubic,   
    	EaseInOutCubic, 
    	EaseInQuart,    
    	EaseOutQuart,   
    	EaseInOutQuart, 
    	EaseInQuint,    
    	EaseOutQuint,   
    	EaseInOutQuint, 
    	EaseInSine,     
    	EaseOutSine,    
    	EaseInOutSine,  
    	EaseInExpo,     
    	EaseOutExpo,    
    	EaseInOutExpo,  
    	EaseInCirc,     
    	EaseOutCirc,    
    	EaseInOutCirc,  
    	Linear,         
    	Spring,         
    	EaseInBounce,   
    	EaseOutBounce,  
    	EaseInOutBounce,
    	EaseInBack,     
    	EaseOutBack,    
    	EaseInOutBack,  
    	EaseInElastic,  
    	EaseOutElastic, 
    	EaseInOutElastic
    }
     
    public static class iTweenX
    {	
    	public readonly static string[] easeTypes = new string[32]
    	{
    		"easeInQuad",
    		"easeOutQuad",
    		"easeInOutQuad",
    		"easeInCubic",
    		"easeOutCubic",
    		"easeInOutCubic",
    		"easeInQuart",
    		"easeOutQuart",
    		"easeInOutQuart",
    		"easeInQuint",
    		"easeOutQuint",
    		"easeInOutQuint",
    		"easeInSine",
    		"easeOutSine",
    		"easeInOutSine",
    		"easeInExpo",
    		"easeOutExpo",
    		"easeInOutExpo",
    		"easeInCirc",
    		"easeOutCirc",
    		"easeInOutCirc",
    		"linear",
    		"spring",
    		"easeInBounce",
    		"easeOutBounce",
    		"easeInOutBounce",
    		"easeInBack",
    		"easeOutBack",
    		"easeInOutBack",
    		"easeInElastic",
    		"easeOutElastic",
    		"easeInOutElastic"
    	};
     
    	public const string time =		"time";
    	public const string speed =		"speed";
    	public const string from =		"from";
    	public const string to =		"to";
    	public const string onStart =          	"onstart";
    	public const string onStartTarget =	"onstarttarget";
    	public const string onStartParams =	"onstartparams";
    	public const string onUpdate =		"onupdate";
    	public const string onUpdateTarget =	"onupdatetarget";
    	public const string onUpdateParams =	"onupdateparams";
    	public const string onComplete =	"oncomplete";
    	public const string onCompleteTarget =	"oncompletetarget";
    	public const string onCompleteParams =	"oncompleteparams";
    	public const string ignoreTimescale =	"ignoretimescale";
    	public const string easeType =		"easetype";
     
    	public static string Ease( EaseType type )
    	{
    		return easeTypes[ (int)type ];
    	}
}
}
