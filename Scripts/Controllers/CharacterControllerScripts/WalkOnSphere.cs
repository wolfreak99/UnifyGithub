// Original url: http://wiki.unity3d.com/index.php/WalkOnSphere
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/WalkOnSphere.cs
// File based on original modification date of: 7 March 2014, at 18:00. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Contents [hide] 
1 WalkOnSphere.cs 
1.1 Author 
1.2 Description 
1.2.1 Variables 
1.3 WalkOnSphere.cs code 

WalkOnSphere.csAuthorzombience aka Jason Araujo 
DescriptionWalkOnSphere.cs is intended to allow a first person camera to walk around a sphere and always maintain the appropriate orientation. The script includes raycasting to allow for terrain variation, although it is not designed to handle extreme terrain variation. 
WalkOnSphere.cs should be placed on a Camera that is the child of an otherwise empty object. The parent object should be at the same location as the "planet" or sphere that you want to navigate. 
WalkOnSphere.cs was adapted from code posted by Unity Answers user Statement in response to a question about spherical movement 


Variablesfloat rotSpeed - the speed for rotating the camera horizontally 
float moveSpeed - the speed to navigate forward, back, and side to side
float rotDamp - how quickly the rotation slows to zero on no input
float moveDamp - how quickly the player slows down on no input


float jumpheight - how high is a jump?
float gravity - how "fast" is a jump? 
float radius - this will be automatically set by raycast.


Transform planet - the object around which the player will walk
Transform trans - a direct reference to the player transform for more efficient position updating
Transform parent - a reference to the camera parent, in case you want the planet to move, and the player to move with the planet


float angle - used for calculating movement
float curJumpHeight - used for jumps in progress
float jumpTimer - calculating jump progress
bool jumping - are we currently jumping?


Vector3 direction - used for movement updating
Quaternion rotation - used for determining rotation around planet

WalkOnSphere.cs code 
using UnityEngine;
using System.Collections;
 
public class WalkOnSphere : MonoBehaviour 
{
	#region vars
	public float rotSpeed = 50;
	public float moveSpeed;
	public float rotDamp;
	public float moveDamp;
	public float height;
	public float jumpHeight;
	[Range(.1f, 10f)]
	public float gravity;
	public float radius;
 
	public Transform planet;
	protected Transform trans;
	protected Transform parent;
 
	protected float angle = 90f;
	protected float curJumpHeight = 0;
	protected float jumpTimer;
	protected bool jumping;
 
 
 
	protected Vector3 direction;
	protected Quaternion rotation = Quaternion.identity;
 
	#endregion
 
	#region Unity methods
	void Start () 
	{	
		trans = transform;
		parent = transform.parent;
	}
 
	void Update () 
	{
 
		//parent.position = planet.position; // If you want to have a moving planet
 
		direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
 
		if(Input.GetKey(KeyCode.LeftShift))
			Position(Input.GetAxis("Horizontal") * -moveSpeed, 0);
		else
			Rotation(Input.GetAxis("Horizontal") * -rotSpeed);
 
		if(Input.GetButtonDown("Jump") && !jumping)
		{
			jumping = true;
			jumpTimer = Time.time;
		}
 
		if(jumping)
		{
			curJumpHeight = Mathf.Sin((Time.time - jumpTimer) * gravity) * jumpHeight; 
			if(curJumpHeight <= -.01f)
			{
				curJumpHeight = 0;
				jumping = false;
			}
		}
 
		Position (0, Input.GetAxis("Vertical") * moveSpeed);
		Movement();
	}
	#endregion
 
	#region Actions
	protected void Rotation(float amt)
	{
		angle += amt * Mathf.Deg2Rad * Time.fixedDeltaTime;
	}
 
	protected void Position(float x, float y)
	{
		Vector2 perpendicular = new Vector2(-direction.y, direction.x);
		Quaternion vRot = Quaternion.AngleAxis(y, perpendicular);
		Quaternion hRot = Quaternion.AngleAxis(x, direction);
		rotation *= hRot * vRot;
	}
 
	protected void Movement()
	{
		trans.localPosition = Vector3.Lerp(trans.localPosition, rotation * Vector3.forward * GetHeight(), Time.fixedDeltaTime * moveDamp);
		trans.rotation = Quaternion.Lerp(trans.rotation, rotation * Quaternion.LookRotation(direction, Vector3.forward), Time.fixedDeltaTime * rotDamp);
	}
 
	protected float GetHeight()
	{
		Ray ray = new Ray(trans.position, planet.position - trans.position);
		RaycastHit hit;
 
		if(Physics.Raycast(ray, out hit))
			radius = Vector3.Distance(planet.position, hit.point) + height + curJumpHeight;
 
		return radius;
	}
 
	#endregion
}
}
