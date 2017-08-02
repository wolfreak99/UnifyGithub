/*************************
 * Original url: http://wiki.unity3d.com/index.php/Tap_to_Move_Drag_to_Look_iPhone
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/Tap_to_Move_Drag_to_Look_iPhone.cs
 * File based on original modification date of: 28 June 2012, at 05:28. 
 *
 * Author: Faikus 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
    DescriptionMove by tapping on the ground in the scenery or with the joystick in the lower left corner. Look around by dragging the screen. These controls are a simple version of the navigation controls in Epic's Epic Citadel demo. 
    UsageAttach this script to a character controller game object. That game object must also have a child object with the main camera attached to it.The ground object must be on the layer 8. 
    
    
    C# - TapMoveDragLook.cs// Move by tapping on the ground in the scenery or with the joystick in the lower left corner.
    // Look around by dragging the screen.
     
    // These controls are a simple version of the navigation controls in Epic's Epic Citadel demo.
     
    // The ground object  must be on the layer 8 (I call it Ground layer).
     
    // Attach this script to a character controller game object. That game object must
    // also have a child object with the main camera attached to it.
     
     
    using UnityEngine;
    using System.Collections;
     
     
     
    [RequireComponent (typeof (CharacterController))]
    public class EpicCitadelControl : MonoBehaviour {
     
    	public bool kJoystikEnabled = true;
    	public float kJoystickSpeed = 0.5f;
    	public bool kInverse = false;
    	public float kMovementSpeed = 10;
     
    	Transform ownTransform;
    	Transform cameraTransform;
    	CharacterController characterController;
    	Camera _camera;
     
    	int leftFingerId = -1;
    	int rightFingerId = -1;
    	Vector2 leftFingerStartPoint;
    	Vector2 leftFingerCurrentPoint;
    	Vector2 rightFingerStartPoint;
    	Vector2 rightFingerCurrentPoint;
    	Vector2 rightFingerLastPoint;
    	bool isRotating;
    	bool isMovingToTarget = false;
    	Vector3 targetPoint;
    	Rect joystickRect;
     
     
     
     
     
    	void MoveFromJoystick()
    	{
    		isMovingToTarget = false;
    		Vector2 offset = leftFingerCurrentPoint - leftFingerStartPoint;
    		if (offset.magnitude > 10)
    			offset = offset.normalized * 10;
     
    		characterController.SimpleMove(kJoystickSpeed * ownTransform.TransformDirection(new Vector3(offset.x, 0, offset.y)));
    	}
     
     
     
    	void MoveToTarget()
    	{
    		Vector3 difference = targetPoint - ownTransform.position;
     
    		characterController.SimpleMove(difference.normalized * kMovementSpeed);
     
    		Vector3 horizontalDifference = new Vector3(difference.x, 0, difference.z);
    		if (horizontalDifference.magnitude < 0.1f)
    			isMovingToTarget = false;	
    	}
     
     
     
    	void SetTarget(Vector2 screenPos)
    	{
    		Ray ray = _camera.ScreenPointToRay (new Vector3 (screenPos.x, screenPos.y));
    		RaycastHit hit;
    		int layerMask = 1 << 8; // Ground
    		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) 
    		{
    			targetPoint = hit.point;
    			isMovingToTarget = true;
    		}
    	}
     
     
     
    	void OnTouchBegan(int fingerId, Vector2 pos)
    	{
    		if (leftFingerId == -1 && kJoystikEnabled && joystickRect.Contains(pos)) {
    			leftFingerId = fingerId;
    			leftFingerStartPoint = leftFingerCurrentPoint = pos;
    		} else if (rightFingerId == -1) {
    			rightFingerStartPoint = rightFingerCurrentPoint = rightFingerLastPoint = pos;
    			rightFingerId = fingerId;
    			isRotating = false;
    		}
    	}
     
     
     
    	void OnTouchEnded(int fingerId)
    	{
    		if (fingerId == leftFingerId)
    			leftFingerId = -1;
    		else if (fingerId == rightFingerId)
    		{
    			rightFingerId = -1;
    			if (false == isRotating)
    				SetTarget(rightFingerStartPoint);
    		}			
    	}
     
     
     
    	void OnTouchMoved(int fingerId, Vector2 pos)
    	{
    		if (fingerId == leftFingerId)
    			leftFingerCurrentPoint = pos;
    		else if (fingerId == rightFingerId)
    		{
    			rightFingerCurrentPoint = pos;
     
    			if ((pos - rightFingerStartPoint).magnitude > 2)
    				isRotating = true;
    		}
    	}
     
     
     
    	void Start ()
    	{
    		joystickRect = new Rect(Screen.width * 0.02f, Screen.height * 0.02f, Screen.width * 0.2f, Screen.height * 0.2f);
    		ownTransform = transform;
    		cameraTransform = Camera.mainCamera.transform;
    		characterController = GetComponent<CharacterController>();
    		_camera = Camera.mainCamera;
    	}
     
     
     
    	void Update ()
    	{
    		if (Application.isEditor)
    		{
    			if (Input.GetMouseButtonDown(0))
    				OnTouchBegan(0, Input.mousePosition);
    			else if (Input.GetMouseButtonUp(0))
    				OnTouchEnded(0);
    			else if (leftFingerId != -1 || rightFingerId != -1)
    				OnTouchMoved(0, Input.mousePosition);
    		}
    		else
    		{
    			int count = Input.touchCount;
     
    			for (int i = 0;  i < count;  i++) 
    			{	
    				Touch touch = Input.GetTouch (i);
     
    				if (touch.phase == TouchPhase.Began)
    					OnTouchBegan(touch.fingerId, touch.position);
    				else if (touch.phase == TouchPhase.Moved)
    					OnTouchMoved(touch.fingerId, touch.position);
    				else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
    					OnTouchEnded(touch.fingerId);
    			}
    		}
     
    		if (leftFingerId != -1)
    			MoveFromJoystick();
    		else if (isMovingToTarget)
    			MoveToTarget();
     
    		if (rightFingerId != -1 && isRotating)
    			Rotate();
     
    	}
     
     
     
    	void Rotate()
    	{
    		Vector3 lastDirectionInGlobal = _camera.ScreenPointToRay(rightFingerLastPoint).direction;
    		Vector3 currentDirectionInGlobal = _camera.ScreenPointToRay(rightFingerCurrentPoint).direction;
     
    		Quaternion rotation = new Quaternion();
    		rotation.SetFromToRotation(lastDirectionInGlobal, currentDirectionInGlobal);
     
    		ownTransform.rotation = ownTransform.rotation * Quaternion.Euler(0, kInverse ? rotation.eulerAngles.y : -rotation.eulerAngles.y, 0);
     
    		// and now the rotation in the camera's local space
    		rotation.SetFromToRotation(	cameraTransform.InverseTransformDirection(lastDirectionInGlobal),
    	                                                cameraTransform.InverseTransformDirection(currentDirectionInGlobal));
    		cameraTransform.localRotation = Quaternion.Euler(kInverse ? rotation.eulerAngles.x : -rotation.eulerAngles.x, 0, 0) * cameraTransform.localRotation;
     
    		rightFingerLastPoint = rightFingerCurrentPoint;
    	}
     
}
}
