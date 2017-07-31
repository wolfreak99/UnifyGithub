// Original url: http://wiki.unity3d.com/index.php/FlyThrough
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/FlyThrough.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Author: Slin 
DescriptionThis is just a very basic fly through camera movement javascript code. 
var lookSpeed = 15.0;
var moveSpeed = 15.0;
 
var rotationX = 0.0;
var rotationY = 0.0;
 
function Update ()
{
	rotationX += Input.GetAxis("Mouse X")*lookSpeed;
	rotationY += Input.GetAxis("Mouse Y")*lookSpeed;
	rotationY = Mathf.Clamp (rotationY, -90, 90);
 
	transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
	transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
 
	transform.position += transform.forward*moveSpeed*Input.GetAxis("Vertical");
	transform.position += transform.right*moveSpeed*Input.GetAxis("Horizontal");
}
}
