/*************************
 * Original url: http://wiki.unity3d.com/index.php/Force2D
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/Force2D.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Jonathan Czeck (aarku) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 JavaScript - Force2D.js 
    4 CShapr - Force2D.cs 
    
    DescriptionThis script will allow you to constrain Unity's physics engine to two dimensions. It works best for rigidbodies that are not connected by physics joints. 
    Technically, this script constrains the GameObject to the Y-Z plane with rotations only allowed on the X axis. 
    UsageAssign this behavior to any GameObject. Do not assign this to RigidBodies that are linked together with joints if you want to keep the physics engine more stable. 
    Note that using this script as is will prevent rigidbodies from sleeping . the workaround will be to check whether the object needs alignment before assigning a value to it. 
    EDIT: the JS can now be made to constrain to x y or z, and will do so there original non constrained rotations 
    JavaScript - Force2D.js//By Nik Gabo
    //now its modular ;)
    public var forceBetweenXYZ:String="x";
    private var Axis:Vector3;//desired depth
    private var Rotation:Vector3;//desired rotation
     
    function Awake(){
        Axis=transform.position;//defines the starting X position as the desired X position
    	Rotation=transform.eulerAngles;//defines the starting rotation as the desired rotation
    }
     function Update(){//each frame
    	var spinforce:Vector3=gameObject.rigidbody.angularVelocity;//is the rotational force on this frame
    	var place:Vector3=gameObject.transform.position;//is the target position in world space on this frame
     
    	if(forceBetweenXYZ=="x"){
    		if(place.x!=Axis.x){//if the current X position IS NOT the desired X position then...
    			transform.position.x=Axis.x;//go to the desired X position
    		}
    		gameObject.rigidbody.angularVelocity.y=0;//stop rotating around y axis
    		gameObject.rigidbody.angularVelocity.z=0;//stop rotating around z axis
    		gameObject.transform.rotation.y=Rotation.y;//resets it to the desired y rotation
    		gameObject.transform.rotation.z=Rotation.z;//resets it to the desired y rotation
    	}else if(forceBetweenXYZ=="y"){
    		if(place.y!=Axis.y){//if the current X position IS NOT the desired X position then...
    			transform.position.y=Axis.y;//go to the desired X position
    		}
    		gameObject.rigidbody.angularVelocity.y=0;//stop rotating around y axis
    		gameObject.rigidbody.angularVelocity.x=0;//stop rotating around z axis
    		gameObject.transform.rotation.x=Rotation.x;//resets it to the desired y rotation
    		gameObject.transform.rotation.z=Rotation.z;//resets it to the desired y rotation
    	}else{
    		if(place.z!=Axis.z){//if the current X position IS NOT the desired X position then...
    			transform.position.z=Axis.z;//go to the desired X position
    		}
    		gameObject.rigidbody.angularVelocity.y=0;//stop rotating around y axis
    		gameObject.rigidbody.angularVelocity.x=0;//stop rotating around z axis
    		gameObject.transform.rotation.y=Rotation.y;//resets it to the desired y rotation
    		gameObject.transform.rotation.x=Rotation.x;//resets it to the desired y rotation
    		if(forceBetweenXYZ!="z"){
    			print("FORCING Z BY DEFAULT, USE X, Y, OR Z LOWERCASE");
    		}
    	}
    }CShapr - Force2D.csusing UnityEngine;
     
    public class Force2D:MonoBehaviour{
    	private Transform trans;
    	private Rigidbody body;
    	//historical x-position
    	private float x;
     
    	public void Start ()
    	{
    		//cashe components for good performance
    		trans = transform;
    		body = rigidbody;
     
    		x = trans.position.x;
    	}
     
    	public void FixedUpdate ()
    	{
    		//reset x-position
    		Vector3 pos = trans.position;
    		pos.x = x;
    		trans.position = pos;
     
    		//clear Y and Z rotation
    		Vector3 dir = trans.localRotation * Vector3.forward;
    		dir.x = 0;
    		trans.localRotation = Quaternion.LookRotation (dir, -Vector3.Cross (Vector3.right, dir));
     
    		//clear X-velocity
    		Vector3 vel = body.velocity;
    		vel.x = 0;
    		body.velocity = vel;
     
    		//clear Y and Z angular velocity
    		body.angularVelocity = new Vector3(body.angularVelocity.x, 0, 0);
    	}
}
}
