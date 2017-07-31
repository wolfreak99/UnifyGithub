// Original url: http://wiki.unity3d.com/index.php/DebugLine
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/DebuggingScripts/DebugLine.cs
// File based on original modification date of: 7 November 2013, at 00:26. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{
Author: Spencer Evans 
Contents [hide] 
1 Description 
2 Usage 
3 Notes 
4 Code 
5 Sample Usage 
6 Possible Changes? 

Description DebugLine.cs provides 2 useful functions for debugging : DrawLine() and DrawRay(). These functions are almost identical to those provided in the Unity 3D engine Debug class, except in the following ways: 
The lines are drawn in the Game window using the lineRenderer component. 
A "width" option is provided - allowing the user to choose the width in pixels of the line drawn. 
The "depthTest" parameter in the Debug.DrawLine() and Debug.DrawRay() functions is unavailable; all lines are obscured by objects closer to the camera. 
Usage Create a new folder titled "Plugins", and move it to your project's "/Assets" folder (if you haven't already). 
Create and place "DebugLine.cs" inside the "/Plugins" folder. 
Call the DebugLine.DrawLine() or DebugLine.DrawRay() functions just as you would with the Debug class. 
Notes You must have a Camera in your scene tagged "MainCamera" for these debug functions to work properly. 
You do NOT need Unity Pro for this to work. 
Code /* Author: Spencer Evans
 * 
 * Description:
 * DebugLine.cs provides 2 useful functions for debugging : DrawLine and DrawRay. These functions are almost identical to those provided in the Unity 3D 
 * engine Debug class (http://docs.unity3d.com/Documentation/ScriptReference/Debug.html), except in the following ways:
 * 1. The lines are drawn in the Game window, not the Scene editor window.
 * 2. A "width" option is provided - allowing the user to choose the width in pixels of the line drawn.
 * 3. The "depthTest" parameter in the Debug.DrawLine() and Debug.DrawRay() functions is unavailable; all lines are obscured by objects closer to the camera.
 * 
 * Usage:
 * 1. Create a new folder titled "Plugins", and move it to your project's "/Assets" folder (if you haven't already).
 * 2. Create and place "DebugLine.cs" inside the "/Plugins" folder.
 * 3. Call the DebugLine.DrawLine() or DebugLine.DrawRay() functions just as you would with the Debug class.
 * 
 * Notes:
 * 1. You must have a Camera in your scene tagged "MainCamera" for these debug functions to work properly.
 * 2. You do NOT need Unity Pro for this to work.
 */
 
using UnityEngine;
using System.Collections;
 
public class DebugLine : MonoBehaviour
{
	//used to make calls from both Update and FixedUpdate function properly
	public float destroy_time = float.MaxValue;
	public float fixed_destroy_time = float.MaxValue;
 
	//draw ray functions - http://docs.unity3d.com/Documentation/ScriptReference/Debug.DrawRay.html
	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		DebugLine.DrawLine(start, start + dir, Color.white);
	}
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0, float width = 1)
	{
		DebugLine.DrawLine(start, start + dir, color, duration, width);
	}
 
	//draw line functions - http://docs.unity3d.com/Documentation/ScriptReference/Debug.DrawLine.html
	public static void DrawLine(Vector3 start, Vector3 end)
	{
		DebugLine.DrawLine(start, end, Color.white);
	}
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0, float width = 1)
	{
		//early out if there is no Camera.main to calculate the width
		if (!Camera.main)
			return;
 
		GameObject line = new GameObject("debug_line");
 
		//set the params of the line renderer - http://docs.unity3d.com/Documentation/ScriptReference/LineRenderer.html
		LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Mobile/Particles/Additive"));
		lineRenderer.SetPosition( 0, start );
		lineRenderer.SetPosition( 1, end );
		lineRenderer.SetColors( color, color );
		lineRenderer.SetWidth( DebugLine.CalcPixelHeightAtDist( (start - Camera.main.transform.position).magnitude) * width,
							   DebugLine.CalcPixelHeightAtDist( (end - Camera.main.transform.position).magnitude) * width	);
 
		//add the MonoBehaviour instance
		DebugLine debugLine = line.AddComponent<DebugLine>();
 
		//set the time to expire - default is 0 (will only draw for one update cycle)
		//use Update or FixedUpdate depending on the time of the invoking call
		if (Time.deltaTime == Time.fixedDeltaTime)
			debugLine.fixed_destroy_time = Time.fixedTime + duration;
		else	debugLine.destroy_time = Time.time + duration;
	}
 
	//utility function to calculate the world height of a single pixel given the following info:
	// - Camera.main.fov
	// - Camera.main.pixelHeight
	// - distance to the camera
	public static float CalcPixelHeightAtDist(float dist)
	{
		//early out if there is no Camera.main to calculate the width
		if (!Camera.main)
			return 0;
 
		//http://docs.unity3d.com/Documentation/Manual/FrustumSizeAtDistance.html
		float frustumHeight = 2 * dist * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
 
		return frustumHeight / Camera.main.pixelHeight;
	}
 
	//check to see if should be destroyed yet
	public void Update ()
	{
		if (Time.time > destroy_time)
			Destroy(transform.gameObject);
	}
	public void FixedUpdate ()
	{
		if (Time.fixedTime > fixed_destroy_time)
			Destroy(transform.gameObject);
	}
}Sample Usage using UnityEngine;
using System.Collections;
 
public class MyClassName : MonoBehaviour
{
	public void Start ()
	{
		DebugLine.DrawLine(new Vector3(0, 0, 0), new Vector3(5, 5, 5), Color.cyan, 4, 2);
	}
	public void Update ()
	{
		DebugLine.DrawRay(transform.position, transform.forward * 10, Color.blue);
		DebugLine.DrawRay(transform.position, transform.up * 10, Color.green);
		DebugLine.DrawRay(transform.position, transform.right * 10, Color.red);
	}
}Possible Changes? a static camera member with associated 'set function' 
can be set to modify which camera is being used to calculate the line width 
or pass a camera each DrawLine/DrawRay call?? 
}
