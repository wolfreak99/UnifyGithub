/*************************
 * Original url: http://wiki.unity3d.com/index.php/DrawGizmoGrid
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/DrawGizmoGrid.cs
 * File based on original modification date of: 17 January 2016, at 06:25. 
 *
 * Author: Hayden Scott-Baron (@docky) - http://twitter.com/docky 
 *   
 * Description 
 *   This adds a handy reference grid to the scene. 
 * Usage 
 *   Place this script anywhere in your project, then drag it onto a blank gameobject. 
 *   note: It will automatically rename and centre this gameobject upon first use. 
 * 
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    using UnityEngine;
    using System.Collections;
     
    // DrawGizmoGrid.cs
    // draws a useful reference grid in the editor in Unity. 
    // 09/01/15 - Hayden Scott-Baron
    // twitter.com/docky 
    // no attribution needed, but please tell me if you like it ^_^
     
    public class DrawGizmoGrid : MonoBehaviour
    {
    	// universal grid scale
    	public float gridScale = 1f; 
     
    	// extents of the grid
    	public int minX = -15; 
    	public int minY = -15; 
    	public int maxX = 15; 
    	public int maxY = 15; 
     
    	// nudges the whole grid rel
    	public Vector3 gridOffset = Vector3.zero; 
     
    	// is this an XY or an XZ grid?
    	public bool topDownGrid = true; 
     
    	// choose a colour for the gizmos
    	public int gizmoMajorLines = 5; 
    	public Color gizmoLineColor = new Color (0.4f, 0.4f, 0.3f, 1f);  
     
    	// rename + centre the gameobject upon first time dragging the script into the editor. 
    	void Reset ()
    	{
    		if (name == "GameObject")
    			name = "~~ GIZMO GRID ~~"; 
     
    		transform.position = Vector3.zero; 
    	}
     
    	// draw the grid :) 
    	void OnDrawGizmos ()
    	{
    		// orient to the gameobject, so you can rotate the grid independently if desired
    		Gizmos.matrix = transform.localToWorldMatrix;
     
    		// set colours
    		Color dimColor = new Color(gizmoLineColor.r, gizmoLineColor.g, gizmoLineColor.b, 0.25f* gizmoLineColor.a); 
    		Color brightColor = Color.Lerp (Color.white, gizmoLineColor, 0.75f); 
     
    		// draw the horizontal lines
    		for (int x = minX; x < maxX+1; x++)
    		{
    			// find major lines
    			Gizmos.color = (x % gizmoMajorLines == 0 ? gizmoLineColor : dimColor); 
    			if (x == 0)
    				Gizmos.color = brightColor;
     
    			Vector3 pos1 = new Vector3(x, minY, 0) * gridScale;  
    			Vector3 pos2 = new Vector3(x, maxY, 0) * gridScale;  
     
    			// convert to topdown/overhead units if necessary
    			if (topDownGrid)
    			{
    				pos1 = new Vector3(pos1.x, 0, pos1.y); 
    				pos2 = new Vector3(pos2.x, 0, pos2.y); 
    			}
     
    			Gizmos.DrawLine ((gridOffset + pos1), (gridOffset + pos2)); 
    		}
     
    		// draw the vertical lines
    		for (int y = minY; y < maxY+1; y++)
    		{
    			// find major lines
    			Gizmos.color = (y % gizmoMajorLines == 0 ? gizmoLineColor : dimColor); 
    			if (y == 0)
    				Gizmos.color = brightColor;
     
    			Vector3 pos1 = new Vector3(minX, y, 0) * gridScale;  
    			Vector3 pos2 = new Vector3(maxX, y, 0) * gridScale;  
     
    			// convert to topdown/overhead units if necessary
    			if (topDownGrid)
    			{
    				pos1 = new Vector3(pos1.x, 0, pos1.y); 
    				pos2 = new Vector3(pos2.x, 0, pos2.y); 
    			}
     
    			Gizmos.DrawLine ((gridOffset + pos1), (gridOffset + pos2)); 
    		}
    	}
    }
}
