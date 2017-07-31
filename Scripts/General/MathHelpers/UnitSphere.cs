// Original url: http://wiki.unity3d.com/index.php/UnitSphere
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MathHelpers/UnitSphere.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MathHelpers
{
Description A small collection of functions that calculates random unit vectors for different purposes. 
UnitSphere.GetPointOnCap(float spotAngle); 
It calculates a random point on the unit sphere surface like Unity's Random.onUnitSphere[1] but it only returns vectors that are within a cone-shaped area. This cone is oriented along the z-axis and coneAngle specifies the size of the area. 90° represents a hemisphere and 180° the full sphere like onUnitSphere. 
UnitSphere.GetPointOnRing(float innerSpotAngle, float outerSpotAngle); 
This one works like GetPointOnCap but it have another, inner cone that is excluded 
Usage The functions are all static functions so you can call it from everywhere. Just save the script as UnitSphere.cs 
UnitSphere.GetPointOnCap(45.0f);
UnitSphere.GetPointOnRing(30.0f,60.0f);
 
UnitSphere.GetPointOnCap(45.0f,transform,10.0f);

C# - UnitSphere.cs using UnityEngine;
using System.Collections;
 
public class UnitSphere
{
    /// <summary>
    /// Returns a point on the unit sphere that is within a cone along the z-axis
    /// </summary>
    /// <param name="spotAngle">[0..180] specifies the angle of the cone. </param>
    public static Vector3 GetPointOnCap(float spotAngle)
    {
        float angle1 = Random.Range(0.0f,Mathf.PI*2);
        float angle2 = Random.Range(0.0f,spotAngle * Mathf.Deg2Rad);
        Vector3 V = new Vector3(Mathf.Sin(angle1),Mathf.Cos(angle1),0);
        V *= Mathf.Sin(angle2);
        V.z = Mathf.Cos(angle2);
	    return V;
    }
 
    public static Vector3 GetPointOnCap(float spotAngle, Quaternion orientation)
    {
	    return orientation * GetPointOnCap(spotAngle);
    }
 
    public static Vector3 GetPointOnCap(float spotAngle, Transform relativeTo, float radius)
    {
	    return relativeTo.TransformPoint( GetPointOnCap(spotAngle)*radius );
    }
 
 
    /// <summary>
    /// Returns a point on the unit sphere that is within the outer cone along the z-axis
    /// but not inside the inner cone. The resulting area describes a ring on the sphere surface.
    /// </summary>
    /// <param name="innerSpotAngle">[0..180] specifies the inner cone that should be excluded.</param>
    /// <param name="outerSpotAngle">[0..180] specifies the outer cone that should be included.</param>
    public static Vector3 GetPointOnRing(float innerSpotAngle, float outerSpotAngle)
    {
        float angle1 = Random.Range(0.0f,Mathf.PI*2);
        float angle2 = Random.Range(innerSpotAngle,outerSpotAngle) * Mathf.Deg2Rad;
        Vector3 V = new Vector3(Mathf.Sin(angle1),Mathf.Cos(angle1),0);
        V *= Mathf.Sin(angle2);
        V.z = Mathf.Cos(angle2);        
	    return V;
    }
 
    public static Vector3 GetPointOnRing(float innerSpotAngle, float outerSpotAngle, Quaternion orientation)
    {
	    return orientation * GetPointOnRing(innerSpotAngle, outerSpotAngle);
    }
 
    public static Vector3 GetPointOnRing(float innerSpotAngle, float outerSpotAngle, Transform relativeTo, float radius)
    {
        return relativeTo.TransformPoint( GetPointOnRing(innerSpotAngle, outerSpotAngle)*radius );
    }
}




--Bunny83 13:27, 16 April 2011 (PDT) 
}
