// Original url: http://wiki.unity3d.com/index.php/FollowTrackingCamera
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CameraControls/FollowTrackingCamera.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: --Digitalos 18:57, 9 July 2009 (PDT)[Digitalos] 
DescriptionA pretty basic C# camera that allows you to turn on/of zoom or rotation as you desire. I plan to add to this to include mousetracking and a few other options, feel free to contribute too. This was a rewrite of the included orbit cam as to allow some toggles and generally make it more readable. 
Codeusing UnityEngine;
 
public class FollowTrackingCamera : MonoBehaviour
{
    // Camera target to look at.
    public Transform target;
 
    // Exposed vars for the camera position from the target.
    public float height = 20f;
    public float distance = 20f;
 
    // Camera limits.
    public float min = 10f;
    public float max = 60;
 
    // Rotation.
    public float rotateSpeed = 1f;
 
    // Options.
    public bool doRotate;
    public bool doZoom;
 
    // The movement amount when zooming.
    public float zoomStep = 30f;
    public float zoomSpeed = 5f;
    private float heightWanted;
    private float distanceWanted;
 
    // Result vectors.
    private Vector3 zoomResult;
    private Quaternion rotationResult;
    private Vector3 targetAdjustedPosition;
 
    void Start(){
        // Initialise default zoom vals.
        heightWanted = height;
        distanceWanted = distance;
 
        // Setup our default camera.  We set the zoom result to be our default position.
        zoomResult = new Vector3(0f, height, -distance);
    }
 
    void LateUpdate(){
        // Check target.
        if( !target ){
            Debug.LogError("This camera has no target, you need to assign a target in the inspector.");
            return;
        }
 
        if( doZoom ){
            // Record our mouse input.  If we zoom add this to our height and distance.
            float mouseInput = Input.GetAxis("Mouse ScrollWheel");
            heightWanted -= zoomStep * mouseInput;
            distanceWanted -= zoomStep * mouseInput;
 
            // Make sure they meet our min/max values.
            heightWanted = Mathf.Clamp(heightWanted, min, max);
            distanceWanted = Mathf.Clamp(distanceWanted, min, max);
 
            height = Mathf.Lerp(height, heightWanted, Time.deltaTime * zoomSpeed);
            distance = Mathf.Lerp(distance, distanceWanted, Time.deltaTime * zoomSpeed);
 
            // Post our result.
            zoomResult = new Vector3(0f, height, -distance);
        }
 
        if( doRotate ){
            // Work out the current and wanted rots.
            float currentRotationAngle = transform.eulerAngles.y;
            float wantedRotationAngle = target.eulerAngles.y;
 
            // Smooth the rotation.
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotateSpeed * Time.deltaTime);
 
            // Convert the angle into a rotation.
            rotationResult = Quaternion.Euler(0f, currentRotationAngle, 0f);
        }
 
        // Set the camera position reference.
        targetAdjustedPosition = rotationResult * zoomResult;
        transform.position = target.position + targetAdjustedPosition;
 
        // Face the desired position.
        transform.LookAt(target);
    }
}
}
