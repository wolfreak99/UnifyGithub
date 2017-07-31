// Original url: http://wiki.unity3d.com/index.php/CameraViewer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CameraViewer.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
By AngryAnt. 
Description Note: Unity Pro only. 
Install in Assets/Editor to add a "Camera viewer" item to the "Window" menu. Clicking this will launch a window which renders the output of the selected camera GameObject. 
Editor script code using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class CameraViewer : EditorWindow
{
	bool liveUpdate = false;
	Camera camera;
	RenderTexture renderTexture, originalTarget;
 
	[ MenuItem( "Window/Camera viewer" ) ]
	static void Launch()
	{
		EditorWindow editorWindow = GetWindow( typeof( CameraViewer ) );
 
		editorWindow.Show();
	}
 
	void Update()
	{
		if( camera != null )
		{
			camera.Render();
			if( liveUpdate )
			{
				Repaint();
			}
		}
	}
 
	void OnSelectionChange()
	{
		Camera newCamera = ( Selection.activeTransform == null ) ? null : Selection.activeTransform.gameObject.camera;
 
		if( newCamera != camera )
		{
			if( originalTarget != null )
			{
				camera.targetTexture = originalTarget;
			}
 
			camera = newCamera;
			if( camera != null )
			{
				originalTarget = camera.targetTexture;
				camera.targetTexture = renderTexture;
			}
			else
			{
				originalTarget = null;
			}
		}
	}
 
	void OnGUI()
	{
		if( camera == null )
		{
			ToolbarGUI( "No camera selection" );
			return;	
		}
 
		if( renderTexture == null || renderTexture.width != position.width || renderTexture.height != position.height )
		{
			renderTexture = new RenderTexture( ( int )position.width, ( int )position.height, ( int )RenderTextureFormat.ARGB32 );
			camera.targetTexture = renderTexture;
		}
 
		GUI.DrawTexture( new Rect( 0.0f, 0.0f, position.width, position.height ), renderTexture );
 
		ToolbarGUI( camera.gameObject.name );
	}
 
	void ToolbarGUI( string title )
	{
		GUILayout.BeginHorizontal( "Toolbar" );
			GUILayout.Label( title );
			GUILayout.FlexibleSpace();
			liveUpdate = GUILayout.Toggle( liveUpdate, "Live update", "ToolbarButton" );
		GUILayout.EndHorizontal();
	}
}
}
