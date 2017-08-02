/*************************
 * Original url: http://wiki.unity3d.com/index.php/Camera_view_window
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Camera_view_window.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Original Author: Luis Correa (Ratamorph) 
 *
 * Description 
 *   
 * Usage 
 *   
 * Javascript - SelectedCameraViewScene.js 
 *   
 * Javascript - SelectedCameraView.js 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    
    DescriptionSometimes you have multiple cameras in your scene each looking at a different place, toggling between those cameras is a bit tedious. This is where this set of scripts come in handy. You can look at the rendered image of any selected camera that has the SelectedCameraView script. 
    Usage1.Place the SelectedCameraViewScene script in YourProject/Assets/Editor. 2.Place the SelectedCameraView script on any camera you want to view. 
    Now when you select a camera that has the SelectedCameraView a window on the scene view window will display the rendered image from that camera. You can turn the window on/off for each camera aswell as modify the position and size of the view window. 
    
    
    Javascript - SelectedCameraViewScene.js@CustomEditor (SelectedCameraView)
    class SelectedCameraViewScene extends Editor {
     
      	function OnInspectorGUI()
      	{
      		EditorGUILayout.BeginHorizontal();
      		GUILayout.Label("Display camera view");
      		target.showView = EditorGUILayout.Toggle(target.showView);
      	  	EditorGUILayout.EndHorizontal();	
     
      		target.ViewRect = EditorGUILayout.RectField("View Rect", target.ViewRect);
     
      		if (GUI.changed)
     
                EditorUtility.SetDirty (target);  		
      	}
     
        function OnSceneGUI () {
     
            if(!target.showView)
            	return;
     
            if(target.camera)
            {
            	Handles.BeginGUI();
            	GUILayout.BeginArea(target.ViewRect);
     
            	GUILayout.Box(target.name);
     
            	var viewCameraRect : Rect = target.ViewRect;
            	viewCameraRect.y = target.ViewRect.y + 80;
            	DrawCamera(viewCameraRect, target.transform.camera);
     
            	GUILayout.EndArea();
            	Handles.EndGUI();	
            }
        }
     
        private function DrawCamera (position : Rect, camera : Camera) 
     
    	{ 
     
        	if (Event.current.type == EventType.Repaint) 
     
       		{ 
     
            	var cameraRect : Rect = new Rect (position.xMin, Screen.height - position.yMax, position.width, position.height);
     
            	var cameraOriginalRect : Rect = camera.pixelRect;
     
     
            	camera.pixelRect = cameraRect; 
     
     
     
            	camera.Render ();
     
            	camera.pixelRect = cameraOriginalRect;
     
        	} 
     
    	}
    }Javascript - SelectedCameraView.js@script ExecuteInEditMode()
    @script RequireComponent(Camera)
     
    //YES, it only holds a couple of variables...
    var showView : boolean = true;
var ViewRect : Rect = Rect(0,0,100,100);
}
