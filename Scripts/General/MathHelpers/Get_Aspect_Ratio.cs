/*************************
 * Original url: http://wiki.unity3d.com/index.php/Get_Aspect_Ratio
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MathHelpers/Get_Aspect_Ratio.cs
 * File based on original modification date of: 19 April 2014, at 15:22. 
 *
 * Author: Luka Kotar (LukaKotar) 
 *
 * Description 
 *   
 * Usage 
 *   
 * AspectRatio.cs 
 *   
 * AspectRatio.js 
 *   
 * AspectRatioCalculator.cs (Editor Window) 
 *   
 * Download as Unity package 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.MathHelpers
{
    DescriptionThis simple class allows you to quickly find out an aspect ratio between two values. Example: if you pass 1280 and 720 (or 1280 x 720 as Vector2) into the function, it will return 16 x 9 as Vector2. 
    Note: Providing a resolution with an invalid aspect ratio will provide incorrect results, or in rare cases even cause your game/Unity to freeze! Setting the aspect ratio in the "Game" view inside the Unity editor will usually work, but it will sometimes provide incorrect results. For example: setting the aspect ratio to 16:9 in the Game view provides a resolution to 1060 x 569, and the result provided by the script is 265:149. If used to calculate the aspect ratio of the user's screen resolution, this script is only useful for fullscreen usage, and windowed with disabled resizing. (Edit > Project Settings > Player > Resizable Window) Also note that the 16:10 aspect ratio will return 8:5. (16/2=8 & 10/2=5, therefore 16:10=8:5) 
    UsageYou do not need to attach the script to any game object. 
    Allowed syntaxes: 
    AspectRatio.GetAspectRatio (int x, int y) 
    AspectRatio.GetAspectRatio (int x, int y, bool debug) 
    AspectRatio.GetAspectRatio (Vector2 xy) 
    AspectRatio.GetAspectRatio (Vector2 xy, bool debug) 
    Here is a few examples: 
    // Provide two integers, receive the result as Vector2:
    Vector2 aspectRatio = AspectRatio.GetAspectRatio(Screen.width, Screen.height);// Provide two integers, receive the result as Vector2, and print the result in the console:
    Vector2 aspectRatio = AspectRatio.GetAspectRatio(Screen.width, Screen.height, true);// Provide a Vector2, receive the result as Vector2:
    Vector2 aspectRatio = AspectRatio.GetAspectRatio(new Vector2(Screen.width, Screen.height));// Provide a Vector2, receive the result as Vector2, and print the result in the console:
    Vector2 aspectRatio = AspectRatio.GetAspectRatio(new Vector2(Screen.width, Screen.height), true);JavaScript version: 
    // Provide two integers, receive the result as Vector2:
    var aspectRatio : Vector2 = AspectRatio.GetAspectRatio(Screen.width, Screen.height);// Provide two integers, receive the result as Vector2, and print the result in the console:
    var aspectRatio : Vector2 = AspectRatio.GetAspectRatio(Screen.width, Screen.height, true);// Provide a Vector2, receive the result as Vector2:
    var aspectRatio : Vector2 = AspectRatio.GetAspectRatio(new Vector2(Screen.width, Screen.height));// Provide a Vector2, receive the result as Vector2, and print the result in the console:
    var aspectRatio : Vector2 = AspectRatio.GetAspectRatio(new Vector2(Screen.width, Screen.height), true);AspectRatio.csusing UnityEngine;
    using System.Collections;
     
    public static class AspectRatio{
    	public static Vector2 GetAspectRatio(int x, int y){
    		float f = (float)x / (float)y;
    		int i = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
    				break;
    		}
    		return new Vector2((float)System.Math.Round(f * i, 2), i);
    	}
    	public static Vector2 GetAspectRatio(Vector2 xy){
    		float f = xy.x / xy.y;
    		int i = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
    				break;
    		}
    		return new Vector2((float)System.Math.Round(f * i, 2), i);
    	}
    	public static Vector2 GetAspectRatio(int x, int y, bool debug){
    		float f = (float)x / (float)y;
    		int i = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
    				break;
    		}
    		if(debug)
    			Debug.Log("Aspect ratio is "+ f * i +":"+ i +" (Resolution: "+ x +"x"+ y +")");
    		return new Vector2((float)System.Math.Round(f * i, 2), i);
    	}
    	public static Vector2 GetAspectRatio(Vector2 xy, bool debug){
    		float f = xy.x / xy.y;
    		int i = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f*i))
    				break;
    		}
    		if(debug)
    			Debug.Log("Aspect ratio is "+ f * i+":"+ i +" (Resolution: "+ xy.x +"x"+ xy.y +")");
    		return new Vector2((float)System.Math.Round(f * i, 2), i);
    	}
    }AspectRatio.js#pragma strict
    public static class AspectRatio{
    	public static function GetAspectRatio(x : int, y : int) : Vector2{
    		var xf : float = x;
    		var yf : float = y;
    		var f = xf / yf;
    		var i : int = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
    				break;
    		}
    		return new Vector2(System.Math.Round(f * i, 2), i);
    	}
    	public static function GetAspectRatio(xy : Vector2) : Vector2{
    		var f = xy.x / xy.y;
    		var i : int = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
    				break;
    		}
    		return new Vector2(System.Math.Round(f * i, 2), i);
    	}
    	public static function GetAspectRatio(x : int, y : int, debug : boolean) : Vector2{
    		var xf : float = x;
    		var yf : float = y;
    		var f = xf / yf;
    		var i : int = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
    				break;
    		}
    		if(debug)
    			Debug.Log("Aspect ratio is "+ f * i +":"+ i +" (Resolution: "+ x +"x"+ y +")");
    		return new Vector2(System.Math.Round(f * i, 2), i);
    	}
    	public static function GetAspectRatio(xy : Vector2, debug : boolean) : Vector2{
    		var f = xy.x / xy.y;
    		var i : int = 0;
    		while(true){
    			i++;
    			if(System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
    				break;
    		}
    		if(debug)
    			Debug.Log("Aspect ratio is "+ f * i +":"+ i +" (Resolution: "+ xy.x +"x"+ xy.y +")");
    		return new Vector2(System.Math.Round(f * i, 2), i);
    	}
    }AspectRatioCalculator.cs (Editor Window)This editor window was intended for testing purposes, but it can be useful if you want to find out an aspect ratio of a resolution. 
    Note: Once this window is closed or Unity is re-opened, it will be reset to the original values. 
    Usage: 
    The script has to be located in the Editor folder (Assets/Editor), and it requires the AspectRatio script from above to be located in the same project. (C# or the JavaScript version, both will work) 
    Locate the editor window under Window > Aspect Ratio Calculator 
    Enter the values and click "Calculate Aspect Ratio" 
    
    
    using UnityEngine;
    using UnityEditor;
     
    public class AspectRatioCalculator : EditorWindow{
    	Vector2 xy = new Vector2(800, 600);
    	string result = "Aspect Ratio = 4:3 (800x600)";
    	[MenuItem("Window/Aspect Ratio Calculator")]
    	static void Init(){
    		AspectRatioCalculator window = (AspectRatioCalculator)EditorWindow.GetWindow(typeof(AspectRatioCalculator));
    	}
     
    	void OnGUI(){
    		xy = EditorGUI.Vector2Field(new Rect(3, 3, Screen.width - 6, 10), "Resolution", xy);
    		xy = new Vector2(xy.x < 1 ? 1 : (int)xy.x, xy.y < 1 ? 1 : (int)xy.y);
    		if(GUI.Button(new Rect(3, 50, Screen.width - 6, 40), "Calculate Aspect Ratio\n" + result)){
    			Vector2 aspectRatio = AspectRatio.GetAspectRatio((int)xy.x, (int)xy.y);
    			result = "Aspect Ratio = " + aspectRatio.x + ":" + aspectRatio.y + " (" + xy.x + "x" + xy.y + ")";
    		}
    	}
    }Download as Unity packageClick here to download GetAspectRatioCS.unitypackage (2.4 KB) 
Click here to download GetAspectRatioJS.unitypackage (2.4 KB) 
}
