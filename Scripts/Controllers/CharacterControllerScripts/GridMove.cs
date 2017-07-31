// Original url: http://wiki.unity3d.com/index.php/GridMove
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/GridMove.cs
// File based on original modification date of: 2 March 2012, at 17:14. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{

Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - GridMove.js 
4 C# - GridMove.cs 

Description This is used for controlling characters that need to move on a 2D map, as typically seen in top-down old-school RPG or strategy games. The character moves smoothly rather than jumping from square to square, but is confined to an user-defined grid. 
Usage Attach this script to the character that should be controlled. The variables are: 
Walk Speed: The speed at which the character normally moves, in units per second. 
Run Speed: The speed at which the character moves if the run button is held down. 
Grid Size: How wide/tall each grid square is, in units. This can only be an integer, so grid sizes such as 1.5 aren't possible. (The grid isn't actually shown; that would be the function of whatever graphics you're using.) 
Grid Orientation: Whether movement is on the horizontal (X/Z) plane or the vertical (X/Y) plane. 
Allow Diagonals: If checked, diagonal movement can be done by holding down the appropriate buttons simultaneously. Otherwise, only straight horizontal or vertical movement is possible. 
Correct Diagonal Speed: If checked, diagonal movement speed is physically the same as straight horizontal/vertical movement speed. That is, movement from (for example) 1,1 to 2,2 takes approximately 1.4 times as long as it would take to move from 1,1 to 1,2. If not checked, then moving from one square diagonally to another takes the same time as moving from one square horizontally/vertically to another. 
The left/right keys or buttons are defined in the "Horizontal" axis as set up in the Input Manager, and likewise up/down is defined by "Vertical". A "Run" button also needs to be defined in the Input Manager. If no separate walk/run speeds are desired, line 34 should be commented out (so setting up the "Run" button won't be necessary). 
JavaScript - GridMove.js var walkSpeed : float = 1.0;
var runSpeed : float = 2.0;
var gridSize : int = 1;
enum Orientation {Horizontal, Vertical}
var gridOrientation = Orientation.Horizontal;
var allowDiagonals = false;
var correctDiagonalSpeed = true;
private var input = Vector2.zero;
 
function Start () {
	var myTransform = transform;
	var startPosition : Vector3;
	var endPosition : Vector3;
	var t : float;
	var tx : float;
	var moveSpeed = walkSpeed;
 
	while (true) {
		while (input == Vector2.zero) {
			GetAxes();
			tx = 0.0;
			yield;
		}
 
		transform.forward = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
		startPosition = myTransform.position;
		endPosition = gridOrientation == Orientation.Horizontal?
			Vector3(Mathf.Round(myTransform.position.x), 0.0, Mathf.Round(myTransform.position.z)) +
			Vector3(System.Math.Sign(input.x)*gridSize, 0.0, System.Math.Sign(input.y)*gridSize)
			:
			Vector3(Mathf.Round(myTransform.position.x), Mathf.Round(myTransform.position.y), 0.0) +
			Vector3(System.Math.Sign(input.x)*gridSize, System.Math.Sign(input.y)*gridSize, 0.0);
		t = tx;
		while (t < 1.0) {
			moveSpeed = Input.GetButton("Run")? runSpeed : walkSpeed;
			t += Time.deltaTime * (moveSpeed/gridSize) * (correctDiagonalSpeed && input.x != 0.0 && input.y != 0.0? .7071 : 1.0);
			myTransform.position = Vector3.Lerp(startPosition, endPosition, t);
			yield;
		}
		tx = t - 1.0;	// Used to prevent slight visual hiccups on "grid lines" due to Time.deltaTime variance
		GetAxes();
	}
}
 
function GetAxes () {
	input = Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	if (allowDiagonals)
		return;
	if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
		input.y = 0.0;
	else
		input.x = 0.0;
}C# - GridMove.cs using System.Collections;
using UnityEngine;
 
class GridMove : MonoBehaviour {
    private float moveSpeed = 3f;
    private float gridSize = 1f;
    private enum Orientation {
        Horizontal,
        Vertical
    };
    private Orientation gridOrientation = Orientation.Horizontal;
    private bool allowDiagonals = false;
    private bool correctDiagonalSpeed = true;
    private Vector2 input;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;
 
    public void Update() {
        if (!isMoving) {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (!allowDiagonals) {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
                    input.y = 0;
                } else {
                    input.x = 0;
                }
            }
 
            if (input != Vector2.zero) {
                StartCoroutine(move(transform));
            }
        }
    }
 
    public IEnumerator move(Transform transform) {
        isMoving = true;
        startPosition = transform.position;
        t = 0;
 
        if(gridOrientation == Orientation.Horizontal) {
            endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
                startPosition.y, startPosition.z + System.Math.Sign(input.y) * gridSize);
        } else {
            endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
                startPosition.y + System.Math.Sign(input.y) * gridSize, startPosition.z);
        }
 
        if(allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0) {
            factor = 0.7071f;
        } else {
            factor = 1f;
        }
 
        while (t < 1f) {
            t += Time.deltaTime * (moveSpeed/gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
 
        isMoving = false;
        yield return 0;
    }
}
}
