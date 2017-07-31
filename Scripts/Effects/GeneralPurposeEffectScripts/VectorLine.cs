// Original url: http://wiki.unity3d.com/index.php/VectorLine
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/VectorLine.cs
// File based on original modification date of: 2 June 2014, at 04:07. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 VectorLine.js 
4 VectorLine.cs 
5 VectorLines.cs 

DescriptionDraws a vector line on the screen based on normalized viewport coordinates. The line can have an arbitrary width, color, and number of segments, and is defined by an array of Vector2s. Requires Unity Pro, since it uses the GL class. 
 
Usage Put this script on a camera. The public variables control the number of points in the line, its color, its thickness, and whether the line should be drawn or not (because disabling the script doesn't stop the OnPostRender function from running). The linePoints array is an array of Vector2s in normalized viewport coordinates (i.e. (0, 0) is the lower-left corner of the screen and (1,1) is the upper-right). Currently only one contiguous line is supported, but it would be easy enough to modify it to use an array of Vector2s in order to draw multiple separate lines, if so inclined. A sample routine, which results in the image above: 
private var origPoints : Vector2[];
 
function Start () {
	linePoints = new Vector2[numberOfPoints];
	origPoints = new Vector2[numberOfPoints];
 
	// Plot points on a circle
	var radians : float = 360.0/(numberOfPoints-1)*Mathf.Deg2Rad;
	var p = 0.0;
	for (i = 0; i < numberOfPoints; i++) {
		linePoints[i] = Vector2(.5 + .25*Mathf.Cos(p), .5 + .25*Mathf.Sin(p));
		origPoints[i] = linePoints[i];
		p += radians;
	}
}
 
function Update () { 
	for (i = 0; i < linePoints.Length; i++) {
		if (i%2 == 0) {var m = .4; var t = 1.0;}
		else {m = .5; t = .5;}
		linePoints[i] = (origPoints[i]-Vector2(.5, .5))*(Mathf.Sin(Time.time*t)+Mathf.PI*m)+Vector2(.5, .5);
	}
}This should be in the same script with the VectorLine code, below. 
VectorLine.js var numberOfPoints = 31;
var lineColor = Color.white;
var lineWidth = 3;
var drawLines = true;
private var lineMaterial : Material;
private var linePoints : Vector2[];
private var cam : Camera;
 
function Awake () {
	lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
		"SubShader { Pass {" +
		"   BindChannels { Bind \"Color\",color }" +
		"   Blend SrcAlpha OneMinusSrcAlpha" +
		"   ZWrite Off Cull Off Fog { Mode Off }" +
		"} } }");
	lineMaterial.hideFlags = HideFlags.HideAndDontSave;
	lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	cam = camera;
}
 
function OnPostRender () {
	if (!drawLines || !linePoints || linePoints.Length < 2) {return;}
 
	var nearClip = cam.nearClipPlane + .00001; // Add a bit, else there's flickering when the camera moves
	var end = linePoints.Length - 1;
	var thisWidth = 1.0/Screen.width * lineWidth * .5;
 
	lineMaterial.SetPass(0);
	GL.Color(lineColor);
 
	if (lineWidth == 1) {
		GL.Begin(GL.LINES);
		for (i = 0; i < end; i++) {
			GL.Vertex(cam.ViewportToWorldPoint(Vector3(linePoints[i].x, linePoints[i].y, nearClip)));
			GL.Vertex(cam.ViewportToWorldPoint(Vector3(linePoints[i+1].x, linePoints[i+1].y, nearClip)));
		}
	}
	else {
		GL.Begin(GL.QUADS);
		for (i = 0; i < end; i++) {
			var perpendicular = (Vector3(linePoints[i+1].y, linePoints[i].x, nearClip) -
								 Vector3(linePoints[i].y, linePoints[i+1].x, nearClip)).normalized * thisWidth;
			var v1 = Vector3(linePoints[i].x, linePoints[i].y, nearClip);
			var v2 = Vector3(linePoints[i+1].x, linePoints[i+1].y, nearClip);
			GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
			GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
			GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
			GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
		}
	}
	GL.End();
}
 
function OnApplicationQuit () {
	DestroyImmediate(lineMaterial);
}
 
@script RequireComponent(Camera)VectorLine.csusing UnityEngine;
using System.Collections;
 
[RequireComponent(typeof (Camera))]
public class VectorLine : MonoBehaviour
{
	public int numberOfPoints = 2;
	public Color lineColor = Color.red;
	public int lineWidth = 3;
	public bool drawLines = true;
 
	private Material lineMaterial;
	private Vector2[] linePoints;
	private Camera cam;
 
	void Awake()
	{
		lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
        "SubShader { Pass {" +
        "   BindChannels { Bind \"Color\",color }" +
        "   Blend SrcAlpha OneMinusSrcAlpha" +
        "   ZWrite Off Cull Off Fog { Mode Off }" +
        "} } }");
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		cam = camera;
	}
 
	// Creates a simple two point line
	void Start()
	{
		linePoints = new Vector2[2];
	}
 
	// Sets line endpoints to center of screen and mouse position
	void Update()
	{
		linePoints[0] = new Vector2(0.5f, 0.5f);
		linePoints[1] = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
	}
 
	void OnPostRender()
	{
		if (!drawLines || linePoints == null || linePoints.Length < 2)
			return;
 
		float nearClip = cam.nearClipPlane + 0.00001f;
		int end = linePoints.Length - 1;
		float thisWidth = 1f/Screen.width * lineWidth * 0.5f;
 
		lineMaterial.SetPass(0);
		GL.Color(lineColor);
 
		if (lineWidth == 1)
		{
	        GL.Begin(GL.LINES);
	        for (int i = 0; i < end; ++i)
			{
	            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i].x, linePoints[i].y, nearClip)));
	            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i+1].x, linePoints[i+1].y, nearClip)));
        	}
    	}
    	else
		{
	        GL.Begin(GL.QUADS);
	        for (int i = 0; i < end; ++i)
			{
	            Vector3 perpendicular = (new Vector3(linePoints[i+1].y, linePoints[i].x, nearClip) -
	                                 new Vector3(linePoints[i].y, linePoints[i+1].x, nearClip)).normalized * thisWidth;
	            Vector3 v1 = new Vector3(linePoints[i].x, linePoints[i].y, nearClip);
	            Vector3 v2 = new Vector3(linePoints[i+1].x, linePoints[i+1].y, nearClip);
	            GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
        	}
    	}
    	GL.End();
	}
 
	void OnApplicationQuit()
	{
		DestroyImmediate(lineMaterial);
	}
}VectorLines.csThe following is a version of the C# script that handles multiple lines at once, updated via UpdateLines() each frame. 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[RequireComponent(typeof (Camera))]
public class VectorLines : MonoBehaviour
{
	// Lists of properties for each line
	public Color lineColor;
	public int lineWidth;
	public bool drawLines = true;
 
	// Material and camera
	private Material lineMaterial;
	private Camera cam;
 
	// List of lines (each a list of vertices) and getter/setter
	private List<List<Vector2>> linePoints;
 
	void Awake()
	{
		lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
        "SubShader { Pass {" +
        "   BindChannels { Bind \"Color\",color }" +
        "   Blend SrcAlpha OneMinusSrcAlpha" +
        "   ZWrite Off Cull Off Fog { Mode Off }" +
        "} } }");
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		cam = camera;
	}
 
	public void InitializeLines(List<List<Vector2>> newPointsList)
	{
		// Creates new list of points
		linePoints = new List<List<Vector2>>();
		for (int i = 0; i < newPointsList.Count; ++i)
		{
			List<Vector2> newList = new List<Vector2>();
			for (int j = 0; j < newPointsList[i].Count; ++j)
			{
				newList.Add(newPointsList[i][j]);
			}
			linePoints.Add(newList);
		}
	}
 
	public void UpdateLines(List<List<Vector2>> updatedPoints)
	{
		// Sets all points of list to update list of points
		for (int i = 0; i < linePoints.Count; ++i)
			for (int j = 0; j < linePoints[i].Count; ++j)
				linePoints[i][j] = updatedPoints[i][j];
	}
 
	void OnPostRender()
	{
		// Cycles through each separate line
		for (int i = 0; i < linePoints.Count; ++i)
		{
			if (!drawLines || linePoints == null || linePoints[i].Count < 2)
				return;
 
			float nearClip = cam.nearClipPlane + 0.00001f;
			int end = linePoints[i].Count - 1;
			float thisWidth = 1f/Screen.width * lineWidth * 0.5f;
 
			lineMaterial.SetPass(0);
			GL.Color(lineColor);
 
			if (lineWidth == 1)
			{
		        GL.Begin(GL.LINES);
		        for (int j = 0; j < end; ++j)
				{
		            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i][j].x, linePoints[i][j].y, nearClip)));
		            GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i][j+1].x, linePoints[i][j+1].y, nearClip)));
	        	}
	    	}
	    	else
			{
		        GL.Begin(GL.QUADS);
		        for (int j = 0; j < end; ++j)
				{
		            Vector3 perpendicular = (new Vector3(linePoints[i][j+1].y, linePoints[i][j].x, nearClip) -
		                                 new Vector3(linePoints[i][j].y, linePoints[i][j+1].x, nearClip)).normalized * thisWidth;
		            Vector3 v1 = new Vector3(linePoints[i][j].x, linePoints[i][j].y, nearClip);
		            Vector3 v2 = new Vector3(linePoints[i][j+1].x, linePoints[i][j+1].y, nearClip);
		            GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
		            GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
		            GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
		            GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
	        	}
	    	}
	    	GL.End();
		}
	}
 
	void OnApplicationQuit()
	{
		DestroyImmediate(lineMaterial);
	}
}
}
