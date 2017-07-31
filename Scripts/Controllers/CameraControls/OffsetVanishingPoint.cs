// Original url: http://wiki.unity3d.com/index.php/OffsetVanishingPoint
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/OffsetVanishingPoint.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Author: Eric Haines (Eric5h5), thanks to the PerspectiveOffCenter function from the Unity docs 
DescriptionOffset the vanishing point of a specified camera by an arbitrary amount. This allows such effects as "Ken Burns" pans over your 3D scene as if it was a 2D image (i.e., no perspective shifting), setting the vanishing point off to the side for special menu effects, etc. 
Usage Call OffsetVanishingPoint by passing a camera and a Vector2 containing the x and y amounts that you want shifted compared to the default vanishing point. The Vector2 is an absolute offset, not a relative offset. A sample usage snippet: 
// Do a pan from left to right
var panSpeed = .15;
var panLimit = .1;
 
function Start () {
	var t = 0.0;
	while (t < 1.0) {
		t += Time.deltaTime * panSpeed;
		var x = Mathf.Lerp(panLimit, -panLimit, t);
		SetVanishingPoint(camera.main, Vector2(x, 0.0));
		yield;
	}
}As the docs say, using projectionMatrix will make the camera no longer update its rendering based on its fieldOfView, so using this function will make the camera stick at whatever FOV it had before you used it. If you want to change the FOV after using this function, you have to call ResetProjectionMatrix first. 
OffsetVanishingPoint.js function SetVanishingPoint (cam : Camera, perspectiveOffset : Vector2) {
	var m = cam.projectionMatrix;
	var w = 2*cam.nearClipPlane/m.m00;
	var h = 2*cam.nearClipPlane/m.m11;
 
	var left = -w/2 - perspectiveOffset.x;
	var right = left+w;
	var bottom = -h/2 - perspectiveOffset.y;
	var top = bottom+h;
 
	cam.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
}
 
static function PerspectiveOffCenter (
	left : float, right : float,
	bottom : float, top : float,
	near : float, far : float ) : Matrix4x4
{
	var x =  (2.0 * near)		/ (right - left);
	var y =  (2.0 * near)		/ (top - bottom);
	var a =  (right + left)		/ (right - left);
	var b =  (top + bottom)		/ (top - bottom);
	var c = -(far + near)		/ (far - near);
	var d = -(2.0 * far * near) / (far - near);
	var e = -1.0;
 
	var m : Matrix4x4;
	m[0,0] =   x;  m[0,1] = 0.0;  m[0,2] = a;   m[0,3] = 0.0;
	m[1,0] = 0.0;  m[1,1] =   y;  m[1,2] = b;   m[1,3] = 0.0;
	m[2,0] = 0.0;  m[2,1] = 0.0;  m[2,2] = c;   m[2,3] =   d;
	m[3,0] = 0.0;  m[3,1] = 0.0;  m[3,2] = e;   m[3,3] = 0.0;
	return m;
}
}
