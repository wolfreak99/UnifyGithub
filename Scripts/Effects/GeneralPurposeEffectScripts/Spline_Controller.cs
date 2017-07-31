// Original url: http://wiki.unity3d.com/index.php/Spline_Controller
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/Spline_Controller.cs
// File based on original modification date of: 7 September 2013, at 07:59. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Contents [hide] 
1 Author 
2 Download 
3 Intro 
4 Usage 
5 Classes 
5.1 Spline 
5.2 SplineNode 
5.3 SplineController 

AuthorMatt Schoen of Defective Studios 
DownloadDownload SplineController.unitypackage 
This is an external link because the wiki seems to be preventing me from uploading packages, despite listing unityPackage as a supported filetype. Please e-mail me if this is fixed or if there's a trick to it. 
Intro Note: The author of this page moved its old contents to Hermite Spline Controller. The Spline Controller scripts above allow users to create pre-defined spline paths for objects to follow, either automatically at set speeds, or responding to keyboard and mouse input. The spline is defined by a connected collection of GameObjects which define a set of positions and orientations for a Controller script to follow. For automatic followers, each node defines an action, which will be one of the following: 
Continue (default) 
Pause 
Stop 
Reverse 
The actions are pretty self-explanatory, and can be set for each node in the Unity Editor. The package also includes editor scripts to override the default inspector for the Spline object (which contains the nodes) and each SplineNode. The Spline inspector adds some buttons to affect every node in the spline, such as setting parameters and toggling the display of some "helper" models, which wants to be hidden when the user isn't manipulating the spline. 
There are many improvements still to be made: 
Non-linear position/rotation/speed interpolation 
Can you do 3D Bezier interpretation? If not, I'll probably implement something like the interpolation used in the Hermite Spline Controller. If you want a smooth follower (without player control), check that page out. 
Re-integration of "Gravity" spline 
The original design of this controller involved a second spline which the character would refer to for it's definition of world space while in the air. In other words, the spline defines "up" and "forward" depending on the character's current position. This feature was abandoned for convenience but important vestigial functions have remained, and will be put back into use in time. 
Better "drawing" controls 
The add next/prev buttons are a temporary convenience and will eventually make way to a "brush"-like interface. I'll see how easy this is to do in-editor, because it would be inconvenient to have to create splines in-game for many reasons. 
Usage  
The Spline ControllerThe involved classes are described in detail below, I won't bother to elaborate certain details. Please refer to the information below if any specifics are confusing. 
You'll want to use the provided prefab to get started, as setting up a spline node by hand is a bit tedious. 
Select one of the two nodes created by default, and click "Add Next" or "Add Previous" to create more. 
You'll have to position each node as you like manually. 
Rotating the node will set the rotation the follower will take upon reaching it. If you enable Rotation Freedom you can rotate the node however you like. 
To remove nodes from the spline, simply delete them. They'll take their collider with them, and all links will be respected, via the OnDestroy function in SplineNode. 
Add the SplineController script to the object you would like to follow the spline. 
Add a rigidbody (and collider if you want it) to the object, and turn off it's gravity. If this will be keyboard controlled, you might want to set it's collision mode to Continuous Dynamic since it will be flying around quite a bit. 
Further setup of the controller will depend on what type you want 
"Auto" mode 
Drag the Spline object into the Current Spline field 
"Mouse" mode 
Mouse mode assumes that something else, presumably the mouse, is setting the position of the object 
Set the Snap Distance to choose how close (above) the spline the object has to be before it attaches to the spline 
Set the hover offset to specify where, in relation to the position along the spline, the object should be 
"Keyboard" mode 
After setting the Mode to Keyboard, if you're happy with the default settings (which make good sense for a character about 2 units tall) you're done! 
ClassesI'll admit my comments are sparse (this will get better over time :) so here's a rundown on the important classes in the package. 
SplineThe spline goes on the parent object to the whole system. As an aside, the controller will set this as its parent when it lands on a spline. For creating an auto-controller, I advise you make the controller a child of the spline, so that the spline (and its nodes) cannot be "pulled out from under" the controller, as you will see happen if it is setup otherwise. You will also find it useful to take advantage of the utilities provided by its inspector while manipulating the splines. The controller will also use a reference to this object for various purposes. 
 
The Spline InspectorBegin 
The first node in the spline. (See End) 
End 
The Add buttons and SplineNode.OnDestroy() do a good job of keeping these parameters set correctly, but if you're having trouble it wouldn't hurt to verify that they're correct manually. Click on the SplineNode reference to highlight it in the Heirarchy. 
Next 
The next spline in the chain. (See Previous) 
Previous 
The previous spline in the chain. If for whatever reason, you want to chain splines together, you can. This can be a finicky process and should be avoided where possible, but followers will check for a next or previous spline before running off the end of their current spline. If a next or previous spline is found, the second or penultimate node are chosen for the next or previous node, respectively. As an example, a follower going "forward," upon passing the a node which has no "Next" (see below) will then check the Spline's "Next" field. If a next spline is found, the first node of that spline (begin) is chosen as the current node, the spline is set as the current spline and the follower continues as normal. Connected splines are meant to "overlap"--the end of the first either in the same location as or "behind" the beginning of the second--for a variety of reasons. For one, the colliders (discussed below) must span any "walkable" area for followers to land on them. A set of connected splines which do not overlap will behave as if not connected. The character will simply fall off of the spline it was following when reaching the beginning or end. 
Global Collider Type 
This determines which type of collider will be applied when clicking "Reset Colliders" below. (See SplineNode.Collider Type) 
Length 
The length of the spline. Again, this is more-or-less handled by the scripts, but it's possible for length to be wrong. If you're having trouble, check that this is correct. 
Set Pause Time 
The pause time which will be used when clicking "Set Pause Time" below. 
Set Speed 
The Speed which will be used when clicking "Set Pause Time" below. 
Set Speed 
Clicking this will set the speed of every node in the spline to the "Set speed" value above. 
Set Pause Time 
Clicking this will set the pause time of every node in the spline to the "Set pause time" value above. 
Toggle handles 
This will toggle the display of rotation handles for all nodes. The rotation handles are meant to resemble the rotation handles of any 3D package, with an added arrow indicating the "up" direction of the node. The rotation handles themselves are a child object whose hideflags prevent you from selecting or editing them. I also got the colors wrong. I'll fix this eventually. Sorry :) 
Togggle nodes 
This will toggle the display of node meshes for all nodes. The mesh will change to indicate the node's action, and provides an object to click on in the editor. 
Global Collider Radius 
This slider sets the radius of all colliders in the spline. (See SplineNode.Collider Radius) 
Toggle Colliders 
Toggles the display of the cube gizmo approximating each collider 
Reset Colliders 
Remakes all colliders on the spline. Use this the display mode of the colliders is out of sync, or if for whatever reason you're missing one. 
SplineNode 
The SplineNode InspectorNum 
NOT AN INDEX -- This is a variable I included for debug purposes. 
Collider Radius 
The radius (or width) of the collider. This will have a different effect depending on the type of collider. (See Collider Type) Don't set this variable here, because when you select the Spline and the Inspector is rendered, the inspector script will reset all collider radii to the global value. 
Type 
In retrospect, this should be "action" or something. Defines what an auto-follower will do upon reaching this node. The options are: 
Continue 
Just keep going 
Pause 
Pause for a certain amount of time. (See Pause Time) 
Stop 
Stop indefinitely, or until the user or a script restarts the following 
Reverse 
Pause and reverse direction. If no pause is desired, set Pause Time to 0 
Collider Type 
What type the span collider can be. This collider exists to be found by the follower's prediction linecasts so that the follower can land on a spline. It is a trigger, and thus doesn't interfere with physics colliders. The types are (same as PrimitiveType) 
Sphere 
A sphere collider would be pretty silly. Currently, setting Collider Type to sphere will get it reset to Capsule. 
Capsule 
This creates a capsule collider with a radius of Collider Radius. The center of each cap is positioned at the begin/end nodes of the span, so that the collider caps will overlap perfectly. 
Cylinder 
There's no cylinder collider. This also resets to Capsule. 
Cube 
Cubes would also be a bit weird. This setting defaults to plane (which is actually a cube collider) 
Plane 
A plane collider actually isn't a plane. For a few reasons, it's better to use a box collider here, which for our purposes has a height of 0.1. This will be a settable variable in the next revision. 
Collider Freedom 
Don't constrain the node's collider's position, rotation, and size, as is the default behavior. If this is false, each collider is positioned exactly between two nodes, is scaled to be as long as their span in the forward direction, and rotated to "look at" the second node. If it is true, this procedure is ignored, and you can manipulate the collider however you please. 
Rotation Freedom 
Don't constrain the node's rotation. If this is true, the node will always "look at" the next node (free if there is no next node). The node can still be rotated 360 degrees around the forward (look) axis, but will always point forward to the next node. If this is false, you can rotate the node however you want. 
Hide Handles 
If true, don't display the handle object. This is toggled by the "Toggle Handles" button above. 
Pause Time 
The length of time to wait when pausing or reversing. As a note, I've found it is useful, when using an animated follower, to use animation length as pause time. To do this, set the Pause Time to infinity (this can be done by just typing "Infinity" for its value), and use your own controller logic to set the speed back to the node's speed when the animation has finished. You could also use a stop node instead of setting time = Infinity, but I like the node is marked as "pause" so it's clear what will happen. 
Speed 
The speed which the follower will be set to upon hitting this node. Use the above "Set Speed" to normalize this across the spline. 
Add Offset 
The length of offset (to left or right) when a next/previous node is added to this node. 
From Previous 
Whether this node's action will be done on a controller coming from the previous node. (See From Next) 
From Next 
Whether this node's action will be done on a controller coming from the next node. This lets a follower "bypass" the node when travelling in a certain direction, giving you more control of the behavior of "auto" followers. Obviously, this has no impact on keyboard or mouse followers. 
Next 
The next node in the spline (See Previous) 
Previous 
The previous node in the spline. These connections are the only way that the spline is connected (and thus defined). If the spline were a data structure, it would be a doubly-linked list. 
Span Collider 
The reference to the collider that spans the gap between this node and the next. The collider is referenced in the "left" node, meaning there will be one in the beginning node, and not in the end node. The collider objects themselves exist as a direct child of the Spline. (See Collider Type, above) 
Spline 
The reference to the spline, which will generally but not necessarily be the node's parent 
Destroy 
An incidental Boolean to be set when the parent is destroyed, so as to avoid an error on destroying the span collider twice. (will be hidden from inspector) 
Node 
Reference to the object which provides the mesh and material for the Continue nodes 
Pause 
Reference to the object which provides the mesh and material for the Pause nodes 
Stop 
Reference to the object which provides the mesh and material for Stop nodes 
Reverse 
Reference to the object which provides the mesh and material for Reverse nodes 
Next Arrow 
Reference to the object which provides the mesh and material for the next arrow 
Prev Arrow 
Reference to the object which provides the mesh and material for the previous arrow 
Handles 
Reference to the object which provides the mesh and material for the rotation handles 
SplineController 
The SplineController Inspector 
The SplineController Inspector, Keyboard mode 
The SplineController Inspector, Auto modeGo 
This boolean encapuslates everything in the FixedUpdate method of SplineController. In other words, if Go is true, the follower doesn't move on its own accord. I might remove this in light of using the "enable" field of the component instead. 
Rev Orientation 
If this is true, the model will be rotated (in Y) by 180 degrees, so that it will face backwards as it moves. Do this if the model faces the wrong direction. The controller will orient the model so it "looks forward" along its forward z vector. 
Current Spline 
The spline to which the controller is currently attached. For keyboard or mouse followers, this should remain unset. If you would like to set up an automatic follower, you have a choice to make. If no Current Spline is set, the follower will fall down until it hits a spline (forever, if it has to). If you want a follower to start on-spline, set this variable in the editor before the game runs. 
Gravity Spline 
This variable is ignored for all intents and purposes. It's a vestige of when the controller used a spline to determine its local space while not attached to a spline (where to face, where to fall), but its use hasn't been tested recently. It will be included in future revisions. 
Gravity Force 
The force amount applied to gravity, while the follower isn't attached to a spline. If no gravity spline is set, the controller falls relative to Vector3.up (so a negative value will fall down) 
Loose Follow 
This variable has a subtle effect. The difference is between whether the follower will "snap" to the spline, or simply head toward it given its current position. There will probably be changes regarding its affect in future revisions. 
Snap Distance 
This variable belongs solely to Mouse mode followers, now, since it is automatically set with Keyboard and Auto followers. In a mouse follower, Snap distance is used for the length of the linecast along Vector3.down, which searches for the spline nearest the follower. In Keyboard and Auto followers, the snap distance is a threshold given to the Closest Point of Approach algorithm which determines where along the spline the controller has landed. In practice, this value is set automatically to ensure the right position is found. This will be discussed below regarding the controller's "workings." 
Mode 
What mode the follower will assume. There are three options: 
Mouse 
As stated above, mouse node assumes that another script is in control of the follwer's position. Strictly speaking, Mouse mode disables the controller, but here is it's purpose: Whatever script moves the object should call SplineController.FindNextSplineMouse every frame (or whenever position is updated). The function takes two parameters: an Vector3 for the intended position, and an out Vector3 for the position at which it should be placed. Thus, your placement function should look like this 
MoveObject(Vector3 position, GameObject obj){
	SplineController sc = obj.GetComponent<SplineController>();
	Vector3 pos;
	if(sc){
		if(sc.findNextSplineMouse(out pos, data.drag.point))
			sc.transform.position = pos + sc.hoverOffset;
		else
			obj.transform.position = position;
	} else {
		obj.transform.position = position;
	}
}The function returns a boolean, equal to whether or not the controller is in range of a spline. This is determined by casting a line downward from the controller's position, with a length of Snap Distance. The controller object will be placed at the position along the spline closest to the ray, offset by the Hover Offset vector. Thus, as you move it around, when the position is above the spline, the object will be placed at a consistent offset, along the spline. 



Hover Offset 
The offset by which the controller will hover. 
Keyboard 
This means the forward/backward progress will be determined by keyboard input, namely according to the default "Horizontal" axis. By default, the controller will fall straight down, casting rays along it's forecasted position (See Prediction Length/Step). As soon as it falls through a spline, it will attach. While falling, the left/right arrow keys will cause an acceleration (equal to AirForce) along Vector3.Left/Right (until Gravity Spline functionality is replaced). When attached to the spline, the acceleration will be applied along the forward direction of GroundNode (See GroundNode). 
Air Force 
The amount of force applied so the controller in response to the "Horizontal" axis, when falling through the air. 
Max Air Speed 
The speed above which key control will not apply acceleration, when falling through the air. 
Run Force 
The amount of force applied so the controller in response to the "Horizontal" axis, when attached to a spline. 
Max Run Speed 
The speed above which key control will not apply acceleration, when attached to a spline. 
Stop Force 
The force used to stop the controller when moving in one direction while the user presses the button to move in the other. 
Jump Velocity 
The speed at which the controller will move upward when space bar is pressed. 
Initial Speed 
The speed at which the controller will move at the start of the game. 
Auto 
Go In Reverse 
Whether the controller will start moving "left" toward the previous nodes. 
Initial Speed 
The speed at which the controller will move at the start of the game. 
Advanced 
Opening this foldout will show the default inspector, revealing a few more parameters. 
}
