/*************************
 * Original url: http://wiki.unity3d.com/index.php/TransformRotation2D
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/TransformRotation2D.cs
 * File based on original modification date of: 12 September 2013, at 02:27. 
 *
 * Author: Jessy 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.ReallySimpleScripts
{
    DescriptionQuaternions are overkill for rotations if you're working in 2D. These extension methods allow you to represent Transform rotations around an axis of your choosing (Z by default) with just a float. 
    UsageEither add this stuff to your already-existing Transform extension class, or copy and paste the whole thing into a C# script named TransformExtensions. By default, you work with it using world space, and the XY plane, but you can use the optional parameters if you want to work with local space, or other planes. The angles are in radians; make other extension methods that call these ones, after converting from degrees to radians, if you like. 
    using UnityEngine;
     
    public static class TransformExtensions 
    {
    	public enum Axis { X, Y, Z }
     
    	public static void SetRotation2D(this Transform transform, float angle, 
    		Space space = Space.World, Axis axis = Axis.Z)
    	{
    		var rotation = new Quaternion();
    		float halfAngle = angle % (2 * Mathf.PI) * .5F;
    		rotation[(int)axis] = Mathf.Sin(halfAngle);
    		rotation.w = Mathf.Cos(halfAngle);
    		switch (space)
    		{
    			case Space.Self:
    				transform.localRotation = rotation;
    				break;
    			default:
    				transform.rotation = rotation;
    				break;
    		}
    	}
     
    	public static float GetRotation2D(this Transform transform, 
    		Space space = Space.World, Axis axis = Axis.Z)
    	{
    		Quaternion rotation;	
    		switch (space)
    		{
    			case Space.Self:
    				rotation = transform.localRotation;
    				break;
    			default:
    				rotation = transform.rotation;
    				break;
    		}
    		return Mathf.Asin( rotation[(int)axis] * Mathf.Sign(rotation.w) ) * 2;
    	}
}
}
