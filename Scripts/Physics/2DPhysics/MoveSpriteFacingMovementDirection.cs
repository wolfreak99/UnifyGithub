// Original url: http://wiki.unity3d.com/index.php/MoveSpriteFacingMovementDirection
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/2DPhysics/MoveSpriteFacingMovementDirection.cs
// File based on original modification date of: 22 May 2015, at 05:30. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.2DPhysics
{
Contents [hide] 
1 Author 
2 Description 
3 Usage 
4 Script C# 
5 Category 

Author    Ahmad Sami


Description    Move the Sprite facing the direction of movement.





Usage     Add following script to your sprite and assign your public variables. Also attach "Rigidbody 2D" to your sprite. 





Script C# 


	public Camera camera;          // Assign you camera    
	public float speed;            // Define speed of sprite movement
	public float stopValue;        // Define Value before you want to stop (0.1f is better)
 
	private Vector3 newPosition;
	private bool isMoving;
	private Rigidbody2D rigidBody2d;
	void Start ()
	{
		// so sprite not move at start
		newPosition = transform.position;
		// rigidbody 2D refrence
		rigidBody2d = GetComponent<Rigidbody2D> ();
	}
 
	void Update ()
	{
		// Get click position and assign it.
		if (Input.GetMouseButtonDown (0)) {
			newPosition = camera.ScreenToWorldPoint (Input.mousePosition);
			newPosition.z = transform.position.z;
			isMoving = true;
		}
 
		// here we stop the sprite if its near target position other wise sprite will act strangely when reached target position. 
		float dis = Vector3.Distance (newPosition, transform.position);
		if (dis > stopValue) {
			Vector3 dir = (newPosition - transform.position).normalized * speed;
			rigidBody2d.velocity = dir;
		} else {
			isMoving = false;
			rigidBody2d.velocity = Vector2.zero;
			rigidBody2d.angularVelocity = 0f;
		}
 
		// Get angle of rotaion and apply
		Vector2 moveDirection = rigidBody2d.velocity;
		if (moveDirection != Vector2.zero && isMoving) {
			float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}
	}

Category   2D Physics
}
