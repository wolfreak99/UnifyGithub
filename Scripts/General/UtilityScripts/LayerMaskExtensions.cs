/*************************
 * Original url: http://wiki.unity3d.com/index.php/LayerMaskExtensions
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/LayerMaskExtensions.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Michael Garforth 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    
    
    Contents [hide] 
    1 Overview 
    2 Use 
    3 Example - TestLayers.cs 
    4 C# - LayerMaskExtensions.cs 
    
    OverviewThis C# class gives simple extension access to manipulating and debugging LayerMasks. 
    Due to the way UnityScript/Javascript compiles extension methods, it's not accessible in the same way, so you need to call the functions with the class name and the first parameter as the instance, for example LayerMaskExtensions.AddToMask(mask, names) 
    
    
    UseNote - layerMask below refers to a LayerMask instance, LayerMaskExtensions to the class name itself 
    
    LayerMaskExtensions.Create(params string[] names) - Creates a new LayerMask from a variable number of layer names 
    LayerMaskExtensions.Create(params int[] layerNumbers) - Creates a new LayerMask from a variable number of layer numbers 
    LayerMaskExtensions.NamesToMask(params string[] names) - Same as Create 
    LayerMaskExtensions.LayerNumbersToMask(params int[] layerNumbers) - Same as Create 
    layerMask.Inverse() - Returns the inverse of the mask 
    layerMask.AddToMask(params string[] names) - Returns a new LayerMask with the specified layers added 
    layerMask.RemoveFromMask(params string[] names) - Returns a new LayerMask with the specified layers removed 
    layerMask.MaskToNames() - Returns a string array with the layer names from the mask 
    layerMask.MaskToString() - Returns a string with the layer names from the mask, delimited by comma 
    layerMask.MaskToString(string delimiter) - Returns a string with the layer names from the mask, delimited by the specified delimiter 
    Example - TestLayers.csusing UnityEngine;
     
    public class TestLayers : MonoBehaviour
    {	
    	void Start()
    	{
    		LayerMask mask = LayerMaskExtensions.Create("Ignore Raycast", "TransparentFX", "Water");
    		Debug.Log(mask.MaskToString()); //prints out Ignore Raycast, TransparentFX, Water
     
    		mask = mask.RemoveFromMask("TransparentFX");
    		Debug.Log(mask.MaskToString()); //prints out Ignore Raycast, Water
     
    		mask = mask.AddToMask("TransparentFX");
    		Debug.Log(mask.MaskToString()); //prints out Ignore Raycast, TransparentFX, Water
     
    		Debug.Log(mask.Inverse().MaskToString()); //prints out everything except Ignore Raycast, TransparentFX, Water
    	}
    }
    
    C# - LayerMaskExtensions.csThe script should be named LayerMaskExtensions.cs 
    
    
    using UnityEngine;
    using System.Collections.Generic;
     
    public static class LayerMaskExtensions
    {
    	public static LayerMask Create(params string[] layerNames)
    	{
    		return NamesToMask(layerNames);
    	}
     
    	public static LayerMask Create(params int[] layerNumbers)
    	{
    		return LayerNumbersToMask(layerNumbers);
    	}
     
    	public static LayerMask NamesToMask(params string[] layerNames)
    	{
    		LayerMask ret = (LayerMask)0;
    		foreach(var name in layerNames)
    		{
    			ret |= (1 << LayerMask.NameToLayer(name));
    		}
    		return ret;
    	}
     
    	public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
    	{
    		LayerMask ret = (LayerMask)0;
    		foreach(var layer in layerNumbers)
    		{
    			ret |= (1 << layer);
    		}
    		return ret;
    	}
     
    	public static LayerMask Inverse(this LayerMask original)
    	{
    		return ~original;
    	}
     
    	public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
    	{
    		return original | NamesToMask(layerNames);
    	}
     
    	public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
    	{
    		LayerMask invertedOriginal = ~original;
    		return ~(invertedOriginal | NamesToMask(layerNames));
    	}
     
    	public static string[] MaskToNames(this LayerMask original)
    	{
    		var output = new List<string>();
     
    		for (int i = 0; i < 32; ++i)
    		{
    			int shifted = 1 << i;
    			if ((original & shifted) == shifted)
    			{
    				string layerName = LayerMask.LayerToName(i);
    				if (!string.IsNullOrEmpty(layerName))
    				{
    					output.Add(layerName);
    				}
    			}
    		}
    		return output.ToArray();
    	}
     
    	public static string MaskToString(this LayerMask original)
    	{
    		return MaskToString(original, ", ");
    	}
     
    	public static string MaskToString(this LayerMask original, string delimiter)
    	{
    		return string.Join(delimiter, MaskToNames(original));
    	}
}
}
