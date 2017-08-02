/*************************
 * Original url: http://wiki.unity3d.com/index.php/ConstantVelocity
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/ConstantVelocity.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Opless 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.ReallySimpleScripts
{
    
    Description Useful for missiles, grenades, etc 
    CSharp - ConstantVelocity.cs using UnityEngine;
    using System.Collections;
     
    public class ConstantVelocity : MonoBehaviour {
     
    	public Vector3  direction      = Vector3.forward;
     
    	// Use this for initialization
    	void Start () 
    	{
    	}
     
    	// Update is called once per frame
    	void Update () 
    	{
    		rigidbody.velocity =  transform.rotation * direction;
    	}
    }Javascript - ConstantVelocity.js     var direction : Vector3 = Vector3.forward;
     
        function Update ()
        {
            rigidbody.velocity =  transform.rotation * direction;
    }
}
