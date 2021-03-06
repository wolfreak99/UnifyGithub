/*************************
 * Original url: http://wiki.unity3d.com/index.php/DebugX
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/DebugX.cs
 * File based on original modification date of: 8 February 2013, at 22:28. 
 *
 * Author 
 *   
 * Description 
 *   
 * Usage 
 *   
 * DebugX.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    AuthorHayden Scott-Baron (Dock) - http://starfruitgames.com 
    DescriptionAdds a number of useful Debug Draw features, useful for drawing gizmos. 
    UsageDebugX.DrawPoint(transform.position. Color.red, float 1.0f); 
    DebugX.cs// DebugX.cs
    // Hayden Scott-Baron (Dock) - http://starfruitgames.com
    // Adds a number of useful Debug Draw features
     
    using UnityEngine;
    using System.Collections;
     
    public class DebugX : MonoBehaviour
    {
    	public static void DrawCube (Vector3 pos, Color col, Vector3 scale)
    	{
    		Vector3 halfScale = scale * 0.5f; 
     
    		Vector3[] points = new Vector3 []
    		{
    			pos + new Vector3(halfScale.x, 		halfScale.y, 	halfScale.z),
    			pos + new Vector3(-halfScale.x, 	halfScale.y, 	halfScale.z),
    			pos + new Vector3(-halfScale.x, 	-halfScale.y, 	halfScale.z),
    			pos + new Vector3(halfScale.x, 		-halfScale.y, 	halfScale.z),			
    			pos + new Vector3(halfScale.x, 		halfScale.y, 	-halfScale.z),
    			pos + new Vector3(-halfScale.x, 	halfScale.y, 	-halfScale.z),
    			pos + new Vector3(-halfScale.x, 	-halfScale.y, 	-halfScale.z),
    			pos + new Vector3(halfScale.x, 		-halfScale.y, 	-halfScale.z),
    		};
     
    		Debug.DrawLine (points[0], points[1], col ); 
    		Debug.DrawLine (points[1], points[2], col ); 
    		Debug.DrawLine (points[2], points[3], col ); 
    		Debug.DrawLine (points[3], points[0], col ); 
    	}
     
    	public static void DrawRect (Rect rect, Color col)
    	{
    		Vector3 pos = new Vector3( rect.x + rect.width/2, rect.y + rect.height/2, 0.0f );
    		Vector3 scale = new Vector3 (rect.width, rect.height, 0.0f );
     
    		DebugX.DrawRect (pos, col, scale); 
    	}	
     
    	public static void DrawRect  (Vector3 pos, Color col, Vector3 scale)
    	{		
    		Vector3 halfScale = scale * 0.5f; 
     
    		Vector3[] points = new Vector3 []
    		{
    			pos + new Vector3(halfScale.x, 		halfScale.y, 	halfScale.z),
    			pos + new Vector3(-halfScale.x, 	halfScale.y, 	halfScale.z),
    			pos + new Vector3(-halfScale.x, 	-halfScale.y, 	halfScale.z),
    			pos + new Vector3(halfScale.x, 		-halfScale.y, 	halfScale.z),	
    		};
     
    		Debug.DrawLine (points[0], points[1], col ); 
    		Debug.DrawLine (points[1], points[2], col ); 
    		Debug.DrawLine (points[2], points[3], col ); 
    		Debug.DrawLine (points[3], points[0], col ); 
    	}
     
    	public static void DrawPoint (Vector3 pos, Color col, float scale)
    	{
    		Vector3[] points = new Vector3[] 
    		{
    			pos + (Vector3.up * scale), 
    			pos - (Vector3.up * scale), 
    			pos + (Vector3.right * scale), 
    			pos - (Vector3.right * scale), 
    			pos + (Vector3.forward * scale), 
    			pos - (Vector3.forward * scale)
    		}; 		
     
    		Debug.DrawLine (points[0], points[1], col ); 
    		Debug.DrawLine (points[2], points[3], col ); 
    		Debug.DrawLine (points[4], points[5], col ); 
     
    		Debug.DrawLine (points[0], points[2], col ); 
    		Debug.DrawLine (points[0], points[3], col ); 
    		Debug.DrawLine (points[0], points[4], col ); 
    		Debug.DrawLine (points[0], points[5], col ); 
     
    		Debug.DrawLine (points[1], points[2], col ); 
    		Debug.DrawLine (points[1], points[3], col ); 
    		Debug.DrawLine (points[1], points[4], col ); 
    		Debug.DrawLine (points[1], points[5], col ); 
     
    		Debug.DrawLine (points[4], points[2], col ); 
    		Debug.DrawLine (points[4], points[3], col ); 
    		Debug.DrawLine (points[5], points[2], col ); 
    		Debug.DrawLine (points[5], points[3], col ); 
     
    	}
    }
}
