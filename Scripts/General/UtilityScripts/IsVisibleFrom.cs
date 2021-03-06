/*************************
 * Original url: http://wiki.unity3d.com/index.php/IsVisibleFrom
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/IsVisibleFrom.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Michael Garforth 
 *
 * Overview 
 *   
 * Use 
 *   
 * Example - TestRendered.cs 
 *   
 * C# - RendererExtensions.cs 
 *   
 * UnityScript - RendererHelpers.js 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    
    
    OverviewThis C# class gives simple extension access to checking if an Renderer is rendered by a specific Camera. 
    Due to the way UnityScript/javascript compiles extension methods, it's not accessible in the same way, so another javascript specific class is added below it 
    
    
    UseFor the c# extension method: renderer.IsVisibleFrom(cam) 
    For the UnityScript static function: RendererHelper.IsVisibleFrom(renderer, cam) 
    
    
    Example - TestRendered.csusing UnityEngine;
     
    public class TestRendered : MonoBehaviour
    {	
    	void Update()
    	{
    		if (renderer.IsVisibleFrom(Camera.main)) Debug.Log("Visible");
    		else Debug.Log("Not visible");
    	}
    }
    
    C# - RendererExtensions.csThe script should be named RendererExtensions.cs 
    
    
    using UnityEngine;
     
    public static class RendererExtensions
    {
    	public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    	{
    		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
    		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    	}
    }
    
    UnityScript - RendererHelpers.jsThe script should be named RendererHelpers.js Use this only if you won't be using c# 
    static function IsRenderedFrom(renderer : Renderer, camera : Camera) : boolean
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(camera);
    	return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
}
}
