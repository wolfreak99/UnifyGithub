// Original url: http://wiki.unity3d.com/index.php/Click_To_Move
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/Click_To_Move.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Sakar 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - ClickToMove.js 
4 Behavior 

DescriptionThis script will move an object towards the mouse position when the left mouse button is clicked. 
UsageSimply attach the script towards the object you wish to have move towards the mouse. Change the smooth value to alter the speed at which the object moves. 
JavaScript - ClickToMove.js// Click To Move script
// Moves the object towards the mouse position on left mouse click
 
var smooth:int; // Determines how quickly object moves towards position
 
private var targetPosition:Vector3;
 
function Update () {
	if(Input.GetKeyDown(KeyCode.Mouse0))
	{
		var playerPlane = new Plane(Vector3.up, transform.position);
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		var hitdist = 0.0;
 
		if (playerPlane.Raycast (ray, hitdist)) {
			var targetPoint = ray.GetPoint(hitdist);
			targetPosition = ray.GetPoint(hitdist);
			var targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
			transform.rotation = targetRotation;
		}
	}
 
	transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
}BehaviorIf you want the player to move and correct it's path while holding the mouse button, rather than only doing it when clicking, change "GetKeyDown" to "GetKey" on line 10. 
}
