/*************************
 * Original url: http://wiki.unity3d.com/index.php/CameraFacingBillboard
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/CameraFacingBillboard.cs
 * File based on original modification date of: 13 October 2015, at 17:00. 
 *
 * Author: Neil Carter (NCarter) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Technical Discussion 
    4 C# - CameraFacingBillboard.cs 
    5 Mods 
    6 Alternative Mod 
    
    DescriptionThis script makes the object which it is attached to align itself with the camera. This is useful for billboards which should always face the camera and be the same way up as it is. 
    UsagePlace this script on a GameObject that you want to face the camera. Then, with the object selected, use the inspector to select the Camera you want the object to face. 
    You might want to change Vector3.back to Vector3.front, depending on the initial orientation of your object. 
    Technical DiscussionNote that the script doesn't simply point the object at the camera. Instead, it makes the object point in the same direction as the camera's forward axis (that is, the direction the camera is looking in). This might seem intuitively wrong, but it's actually correct for the one-point-perspective world of realtime computer graphics. 
    C# - CameraFacingBillboard.csusing UnityEngine;
    using System.Collections;
     
    public class CameraFacingBillboard : MonoBehaviour
    {
        public Camera m_Camera;
     
        void Update()
        {
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
                m_Camera.transform.rotation * Vector3.up);
        }
    }ModsThis Mod will additionally:
    -Find the default camera in the scene
    -Create an empty "container" GameObject as parent of the billboard, and will rotate this object instead. This allows the user to assign a predefined rotation to the billboard object.
    -Require initialization. (just set "autoInit" to "true")
    Mod Author: juanelo
    
    //cameraFacingBillboard.cs v02
    //by Neil Carter (NCarter)
    //modified by Juan Castaneda (juanelo)
    //
    //added in-between GRP object to perform rotations on
    //added auto-find main camera
    //added un-initialized state, where script will do nothing
    using UnityEngine;
    using System.Collections;
     
     
    public class CameraFacingBillboard : MonoBehaviour
    {
     
        public Camera m_Camera;
    	public bool amActive =false;
    	public bool autoInit =false;
    	GameObject myContainer;	
     
    	void Awake(){
    		if (autoInit == true){
    			m_Camera = Camera.main;
    			amActive = true;
    		}
     
    		myContainer = new GameObject();
    		myContainer.name = "GRP_"+transform.gameObject.name;
    		myContainer.transform.position = transform.position;
    		transform.parent = myContainer.transform;
    	}
     
     
        void Update(){
            if(amActive==true){
            	myContainer.transform.LookAt(myContainer.transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
    	    }
        }
    }
    
    Alternative ModThis Mod will:
    - Find the default camera in the scene
    - Allow default axis to be specified
    Mod Author: Hayden Scott-Baron (dock)
    
    //	CameraFacing.cs 
    //	original by Neil Carter (NCarter)
    //	modified by Hayden Scott-Baron (Dock) - http://starfruitgames.com
    //  allows specified orientation axis
     
     
    using UnityEngine;
    using System.Collections;
     
    public class CameraFacing : MonoBehaviour
    {
    	Camera referenceCamera;
     
    	public enum Axis {up, down, left, right, forward, back};
    	public bool reverseFace = false; 
    	public Axis axis = Axis.up; 
     
    	// return a direction based upon chosen axis
    	public Vector3 GetAxis (Axis refAxis)
    	{
    		switch (refAxis)
    		{
    			case Axis.down:
    				return Vector3.down; 
    			case Axis.forward:
    				return Vector3.forward; 
    			case Axis.back:
    				return Vector3.back; 
    			case Axis.left:
    				return Vector3.left; 
    			case Axis.right:
    				return Vector3.right; 
    		}
     
    		// default is Vector3.up
    		return Vector3.up; 		
    	}
     
    	void  Awake ()
    	{
    		// if no camera referenced, grab the main camera
    		if (!referenceCamera)
    			referenceCamera = Camera.main; 
    	}
     
    	void  Update ()
    	{
    		// rotates the object relative to the camera
    		Vector3 targetPos = transform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back) ;
    		Vector3 targetOrientation = referenceCamera.transform.rotation * GetAxis(axis);
    		transform.LookAt (targetPos, targetOrientation);
    	}
}
}
