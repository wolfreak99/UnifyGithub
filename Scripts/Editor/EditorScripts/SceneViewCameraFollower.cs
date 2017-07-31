// Original url: http://wiki.unity3d.com/index.php/SceneViewCameraFollower
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/SceneViewCameraFollower.cs
// File based on original modification date of: 30 October 2012, at 03:51. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Description Allows multiple SceneView cameras in the editor to be setup to follow gameobjects. This is particularly useful when you are in play mode and you want the scene view window to continue to stay on a certain gameobject. It works for multiple SceneViews, so you can have different angles and objects up all at the same time. 
Usage Place 'SceneViewCameraFollower.cs' on any gameobject in the scene. The script does not need to be in an Editor folder. You can change the array length of the SceneViewFollower array to the number of SceneViews you have in the editor. 
For each SceneViewFollower, you can specify: 
the target transform to be followed (by default it is the current gameobject the script is on) 
the position offset (relative to the target transform's position) 
if you want the rotation to be fixed, and if so, what rotation (uses euler angles; good for 2D games) 
the size of the camera (changing the aforementioned position offset's z will produce something similar) 
orthographic or perspective 
the index for the specific SceneView these settings should be applied to. 
A global setting can be toggled to have the SceneViews only follow in play mode. 
To be sure it updates the SceneViews in edit mode, make sure the script component is unfolded, since it updates via 'OnDrawGizmos'. 
C# Script - SceneViewCameraFollower.cs //Allows multiple SceneView cameras in the editor to be setup to follow gameobjects.
//October 2012 - Joshua Berberick
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[ExecuteInEditMode]
public class SceneViewCameraFollower : MonoBehaviour
{
#if UNITY_EDITOR
 
	public bool on = true;
	public bool onlyInPlayMode = false;
	public SceneViewFollower[] sceneViewFollowers;
	private ArrayList sceneViews;
 
	void LateUpdate()
	{
		if(sceneViewFollowers != null && sceneViews != null)
		{
			foreach(SceneViewFollower svf in sceneViewFollowers)
			{
				if(svf.targetTransform == null) svf.targetTransform = transform;
				svf.size = Mathf.Clamp(svf.size, .01f, float.PositiveInfinity);
				svf.sceneViewIndex = Mathf.Clamp(svf.sceneViewIndex, 0, sceneViews.Count-1);
			}
		}
 
		if(Application.isPlaying)
			Follow();
	}
 
	public void OnDrawGizmos()
	{
		if(!Application.isPlaying)
			Follow();
	}
 
	void Follow()
	{
		sceneViews = UnityEditor.SceneView.sceneViews;
		if(sceneViewFollowers == null || !on || sceneViews.Count == 0) return;
 
		foreach(SceneViewFollower svf in sceneViewFollowers)
		{	
			if(!svf.enable) continue;
			UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[svf.sceneViewIndex];
			if(sceneView != null)
			{
				if((Application.isPlaying && onlyInPlayMode) || !onlyInPlayMode)
				{
					sceneView.orthographic = svf.orthographic;
					sceneView.LookAtDirect(svf.targetTransform.position + svf.positionOffset, (svf.enableFixedRotation) ? Quaternion.Euler(svf.fixedRotation) : svf.targetTransform.rotation, svf.size);	
				}
			}
		}	
	}
 
	[System.Serializable]
	public class SceneViewFollower
	{
		public bool enable;
		public Vector3 positionOffset;
		public bool enableFixedRotation;
		public Vector3 fixedRotation;
		public Transform targetTransform;
		public float size;
		public bool orthographic;
		public int sceneViewIndex;
 
		SceneViewFollower()
		{
			enable = false;
			positionOffset = Vector3.zero;
			enableFixedRotation = false;
			fixedRotation = Vector3.zero;
			size = 5;
			orthographic = true;
			sceneViewIndex = 0;
		}
	}
 
#endif
}
}
