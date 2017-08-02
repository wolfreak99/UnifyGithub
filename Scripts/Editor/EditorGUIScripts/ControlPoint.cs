/*************************
 * Original url: http://wiki.unity3d.com/index.php/ControlPoint
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/ControlPoint.cs
 * File based on original modification date of: 1 November 2012, at 11:48. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    Contents [hide] 
    1 Author 
    2 Description 
    3 Usage 
    4 UnityScript - ControlPoint.js 
    
    AuthorHayden Scott-Baron (Dock) - http://starfruitgames.com 
    DescriptionThis adds a handle to GameObjects which helps show the position and orientation in the Editor. Also allows clicking on invisible objects. 
    UsagePlace this script anywhere in your project, and drag it onto any gameobject. Change the size, and the size of the centre sphere. 
    UnityScript - ControlPoint.js// ControlPoint.js 
    // by Hayden Scott-Baron (Dock)
    // dock@starfruitgames.com
    //
    // This is just a helper object, it draws a fake Axis gizmo.
    // Usage: Add Component to any GameObject. 
    //
    // Tip: Set a large fully transparent sphere to make gameobject clickable in the scene.
     
    var gizmoSize = 0.5;
    var spherePoint = true;
    private var sphereColor = new Color(0, 0, 0, 0.1); 
    var sphereScale = 0.1; 
     
    function OnDrawGizmos()
    {
    	if (this.enabled == false) { return; }
     
    	if (spherePoint)
    	{
    		Gizmos.color = sphereColor; 
    		Gizmos.DrawSphere (transform.position, sphereScale * gizmoSize);
    	}
     
    	Gizmos.color = Color.blue;
    	Gizmos.DrawLine (transform.position, transform.position + (transform.forward * gizmoSize * 1.0));
    	Gizmos.DrawLine (transform.position + (transform.forward * gizmoSize * 1.0), (transform.position + (transform.forward * gizmoSize * 0.8) + (transform.up * gizmoSize * 0.2)));
    	Gizmos.DrawLine (transform.position + (transform.forward * gizmoSize * 1.0), (transform.position + (transform.forward * gizmoSize * 0.8) + (transform.up * gizmoSize * -0.2)));
    	Gizmos.DrawLine (transform.position + (transform.forward * gizmoSize * 1.0), (transform.position + (transform.forward * gizmoSize * 0.8) + (transform.right * gizmoSize * 0.2)));
    	Gizmos.DrawLine (transform.position + (transform.forward * gizmoSize * 1.0), (transform.position + (transform.forward * gizmoSize * 0.8) + (transform.right * gizmoSize * -0.2)));
     
    	Gizmos.color = Color.green;
    	Gizmos.DrawLine (transform.position, transform.position + (transform.up * gizmoSize));
    	Gizmos.DrawLine (transform.position + (transform.up * gizmoSize * 1.0), (transform.position + (transform.up * gizmoSize * 0.8) + (transform.forward * gizmoSize * 0.2)));
    	Gizmos.DrawLine (transform.position + (transform.up * gizmoSize * 1.0), (transform.position + (transform.up * gizmoSize * 0.8) + (transform.forward * gizmoSize * -0.2)));
    	Gizmos.DrawLine (transform.position + (transform.up * gizmoSize * 1.0), (transform.position + (transform.up * gizmoSize * 0.8) + (transform.right * gizmoSize * 0.2)));
    	Gizmos.DrawLine (transform.position + (transform.up * gizmoSize * 1.0), (transform.position + (transform.up * gizmoSize * 0.8) + (transform.right * gizmoSize * -0.2)));
     
    	Gizmos.color = Color.red;
    	Gizmos.DrawLine (transform.position, transform.position + (transform.right * gizmoSize));
    	Gizmos.DrawLine (transform.position + (transform.right * gizmoSize * 1.0), (transform.position + (transform.right * gizmoSize * 0.8) + (transform.up * gizmoSize * 0.2)));
    	Gizmos.DrawLine (transform.position + (transform.right * gizmoSize * 1.0), (transform.position + (transform.right * gizmoSize * 0.8) + (transform.up * gizmoSize * -0.2)));
    	Gizmos.DrawLine (transform.position + (transform.right * gizmoSize * 1.0), (transform.position + (transform.right * gizmoSize * 0.8) + (transform.forward * gizmoSize * 0.2)));
    	Gizmos.DrawLine (transform.position + (transform.right * gizmoSize * 1.0), (transform.position + (transform.right * gizmoSize * 0.8) + (transform.forward * gizmoSize * -0.2)));	
    }
}
