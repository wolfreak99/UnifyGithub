/*************************
 * Original url: http://wiki.unity3d.com/index.php/MouseOrbitInfiteRotateZoom
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/MouseOrbitInfiteRotateZoom.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: VeggieTribe 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CameraControls
{
    
    DescriptionThis orbital camera will rotate infinitely with zoom controls on the mousewheel. The zoom can be limited via min/max. 
    Please let me know if you have any questions or want to show me how you use it in your project. 
    
    
    Code C#using UnityEngine;
    using System.Collections;
     
    [AddComponentMenu("Infinite Camera-Control/Mouse Orbit with zoom")]
    public class MouseOrbitInfiniteRotateZoom : MonoBehaviour {
     
        public Transform target;
        public float xSpeed = 12.0f;
        public float ySpeed = 12.0f;
        public float scrollSpeed = 10.0f;
     
    	public float zoomMin = 1.0f;
     
    	public float zoomMax = 20.0f;
     
    	public float distance ;
     
    	public Vector3 position;
     
        public bool isActivated;
     
     
     
        float x = 0.0f;
     
        float y = 0.0f;
     
     
     
        // Use this for initialization
     
        void Start () {
     
            Vector3 angles = transform.eulerAngles;
     
            x = angles.y;
     
            y = angles.x;
     
     
     
        }
     
     
     
        void LateUpdate () {
     
     
     
    		// only update if the mousebutton is held down
     
    		if (Input.GetMouseButtonDown(1)){
     
    			isActivated = true;
     
    		} 
     
    		// if mouse button is let UP then stop rotating camera
     
    		if (Input.GetMouseButtonUp(1))
     
    		{
     
    			isActivated = false;
     
    		} 
     
     
     
    	    if (target && isActivated) { 
     
    			//  get the distance the mouse moved in the respective direction
     
    			x += Input.GetAxis("Mouse X") * xSpeed;
     
    	        y -= Input.GetAxis("Mouse Y") * ySpeed;	 
     
     
     
    			// when mouse moves left and right we actually rotate around local y axis	
     
    			transform.RotateAround(target.position,transform.up, x);
     
     
     
    			// when mouse moves up and down we actually rotate around the local x axis	
     
    			transform.RotateAround(target.position,transform.right, y);
     
     
     
    			// reset back to 0 so it doesn't continue to rotate while holding the button
     
    			x=0;
     
    			y=0; 	
     
    		} else {		
     
    			// see if mouse wheel is used 	
     
    	 		if (Input.GetAxis("Mouse ScrollWheel") != 0) 
     
    			{	
     
    				// get the distance between camera and target
     
    				distance = Vector3.Distance (transform.position , target.position);	
     
     
     
    				// get mouse wheel info to zoom in and out	
     
    				distance = ZoomLimit(distance - Input.GetAxis("Mouse ScrollWheel")*scrollSpeed, zoomMin, zoomMax);
     
     
     
    				// position the camera FORWARD the right distance towards target
     
    		        position = -(transform.forward*distance) + target.position;
     
     
     
    				// move the camera
     
    				transform.position = position; 
     
    			}
     
    		}
     
    }
     
     
     
    	public static float ZoomLimit(float dist, float min, float max)
     
        {
     
            if (dist < min)
     
                dist= min;
     
            if (dist > max)
     
                dist= max; 
     
    		return dist;
     
        }
     
     
     
}
}
