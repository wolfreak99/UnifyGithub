// Original url: http://wiki.unity3d.com/index.php/CameraGradientBackground
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/CameraGradientBackground.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 GradientBackground.js 
4 GradientBackground.cs 

DescriptionCreates a background for the camera, which is a simple gradient blend between two colors. 
Usage Attach this script to your camera, and change the top and bottom colors in the inspector as desired. When run, the clear flags for your camera is set to Depth Only, allowing a newly created background camera to show through. A plane with the gradient colors is created, which only the background camera can see. This is done with GradientLayer, which is the only layer that the background camera sees, and your camera is set to ignore. The default, 7, is a built-in (non-user-editable) layer that's not used for anything as of Unity 3.1. It can be changed as necessary. 
GradientBackground.js var topColor = Color.blue;
var bottomColor = Color.white;
var gradientLayer = 7;
 
function Awake () {
	gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
	if (!camera) {
		Debug.LogError ("Must attach GradientBackground script to the camera");
		return;
	}
 
	camera.clearFlags = CameraClearFlags.Depth;
	camera.cullingMask = camera.cullingMask & ~(1 << gradientLayer);
	var gradientCam = new GameObject("Gradient Cam", Camera).camera;
	gradientCam.depth = camera.depth-1;
	gradientCam.cullingMask = 1 << gradientLayer;
 
	var mesh = new Mesh();
	mesh.vertices = [Vector3(-100, .577, 1), Vector3(100, .577, 1), Vector3(-100, -.577, 1), Vector3(100, -.577, 1)];
	mesh.colors = [topColor, topColor, bottomColor, bottomColor];
	mesh.triangles = [0, 1, 2, 1, 3, 2];
 
	var mat = new Material("Shader \"Vertex Color Only\"{Subshader{BindChannels{Bind \"vertex\", vertex Bind \"color\", color}Pass{}}}");
	var gradientPlane = new GameObject("Gradient Plane", MeshFilter, MeshRenderer);
	(gradientPlane.GetComponent(MeshFilter) as MeshFilter).mesh = mesh;
	gradientPlane.renderer.material = mat;
	gradientPlane.layer = gradientLayer;
}GradientBackground.cs using UnityEngine;
 
public class GradientBackground : MonoBehaviour {
 
	public Color topColor = Color.blue;
	public Color bottomColor = Color.white;
	public int gradientLayer = 7;
 
	void Awake () {	
		gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
   		if (!camera) {
        	Debug.LogError ("Must attach GradientBackground script to the camera");
        	return;
    	}
 
    	camera.clearFlags = CameraClearFlags.Depth;
    	camera.cullingMask = camera.cullingMask & ~(1 << gradientLayer);
    	Camera gradientCam = new GameObject("Gradient Cam",typeof(Camera)).camera;
    	gradientCam.depth = camera.depth-1;
    	gradientCam.cullingMask = 1 << gradientLayer;
 
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[4]
						{new Vector3(-100f, .577f, 1f), new Vector3(100f, .577f, 1f), new Vector3(-100f, -.577f, 1f), new Vector3(100f, -.577f, 1f)};
 
		mesh.colors = new Color[4] {topColor,topColor,bottomColor,bottomColor};
 
		mesh.triangles = new int[6] {0, 1, 2, 1, 3, 2};
 
		Material mat = new Material("Shader \"Vertex Color Only\"{Subshader{BindChannels{Bind \"vertex\", vertex Bind \"color\", color}Pass{}}}");
    	GameObject gradientPlane = new GameObject("Gradient Plane", typeof(MeshFilter), typeof(MeshRenderer));
 
		((MeshFilter)gradientPlane.GetComponent(typeof(MeshFilter))).mesh = mesh;
    	gradientPlane.renderer.material = mat;
    	gradientPlane.layer = gradientLayer;
	}
 
}
}
