/*************************
 * Original url: http://wiki.unity3d.com/index.php/SwitchCamera
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/SwitchCamera.cs
 * File based on original modification date of: 24 December 2013, at 20:59. 
 *
 * Author: JakeH 
 *
 * Description 
 *   
 * Usage 
 *   
 * JavaScript- SwitchCamera.js 
 *   
 * CSharp- SwitchCamera.cs 
 *   
 * More cameras 
 *   
 * CSharp - SwitchCameras.cs 
 *   
 * Javascript - SwitchCameras.js 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.ReallySimpleScripts
{
    
    DescriptionThis script lets you switch between two cameras in your scene. 
    UsageAttach the script to amy GameObject and define the variables. 
    JavaScript- SwitchCamera.jsvar camera1 : Camera; 
    var camera2 : Camera; 
     
    function Start () { 
       camera1.enabled = true; 
       camera2.enabled = false; 
    } 
     
    function Update () { 
       if (Input.GetKeyDown ("2")){ 
          camera1.enabled = false; 
          camera2.enabled = true; 
       } 
       if (Input.GetKeyDown ("1")){ 
          camera1.enabled = true; 
          camera2.enabled = false; 
       }     
    }CSharp- SwitchCamera.csusing UnityEngine;
    using System.Collections;
     
    public class SwitchCamera : MonoBehaviour {
     
    	public Camera camera1;
    	public Camera camera2;
     
    	void Start () {
    		camera1.enabled = true; 
    		camera2.enabled = false; 
    	}
     
    	void Update () {
    		if (Input.GetKeyDown("2"))
    		{
    			camera1.enabled = false;
    			camera2.enabled = true;
    		}
    		if (Input.GetKeyDown("1"))
    		{
    			camera1.enabled = true;
    			camera2.enabled = false;
    		}
    	}
    }More camerasThe scripts below use arrays to store as many cameras as wanted. The script must be placed on a camera for the controls to be displayed. 
    CSharp - SwitchCameras.cs/** Author: Douglas Barcelos **/
     
    using UnityEngine;
    using System.Collections;
     
    public class SwitchCameras : MonoBehaviour {
     
    	public Camera[] cameras;
    	private int cameraIndex = 0;
     
    	void Start () {
    		cameraIndex = 0;
    		SelectCamera(cameraIndex);
    	}
     
    	void OnGUI()
    	{
    		if(GUI.Button(new Rect(Screen.width - 120, 20, 100, 100), "Next >>"))
    		{
    			if(cameraIndex >= cameras.Length - 1)
    				cameraIndex = 0;
    			else
    				cameraIndex++;
     
    			SelectCamera(cameraIndex);
    		}
     
    		if(GUI.Button(new Rect(20, 20, 100, 100), "<< Prev"))
    		{
    			if(cameraIndex <= 0)
    				cameraIndex = cameras.Length - 1;
    			else
    				cameraIndex--;
     
    			SelectCamera(cameraIndex);
    		}
    	}
     
     
    	void SelectCamera(int idCamera)
    	{		
    		if (cameras[idCamera] != null)
    		{
    			foreach(Camera cam in cameras)
    				cam.enabled = false;
     
    			cameras[idCamera].camera.enabled = true;
    		}
    	}	
    }Javascript - SwitchCameras.js//////////////\\\\\\\\\\\\\\\
    // Author: Douglas Barcelos 
    //  Translator: William Stott 
    /////////////\\\\\\\\\\\\\\\
     
    var cameras : Camera[];
    private int cameraIndex = 0;
     
    function Start () {
    	cameraIndex = 0;
    	SelectCamera(cameraIndex);
    }
     
    function OnGUI()
    {
    	if(GUI.Button(new Rect(Screen.width - 120, 20, 100, 100), "Next >>"))
    	{
    		if (cameraIndex >= cameras.Length - 1) {
    			cameraIndex = 0;
    		} else {
    			cameraIndex++;
    		}
     
    		SelectCamera(cameraIndex);
    	}
     
    	if(GUI.Button(new Rect(20, 20, 100, 100), "<< Prev"))
    	{
    		if (cameraIndex <= 0) {
    			cameraIndex = cameras.Length - 1;
    		} else {
    			cameraIndex--;
    		}
     
    		SelectCamera(cameraIndex);
    	}
    }
     
     
    function SelectCamera (idCamera : int)
    {		
    	if (cameras[idCamera] != null)
    	{
    		for (cam : Camera in cameras)
    			cam.enabled = false;
     
    		cameras[idCamera].camera.enabled = true;
    	}
}
}
