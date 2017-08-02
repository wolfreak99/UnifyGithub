/*************************
 * Original url: http://wiki.unity3d.com/index.php/ShapeWipe
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/ShapeWipe.cs
 * File based on original modification date of: 17 February 2012, at 03:43. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CameraControls
{
    
    DescriptionMakes an animated wipe from one camera view to another, where an arbitrary shape containing the second view either zooms in or out, with optional rotation. 
     
    UsageSee ScreenWipes for an example scene and the actual script that performs the wipe. The script below is an example of usage. It should be attached to an object, such as a manager object, and the ScreenWipes script should also be attached to the manager object. Also needed are two cameras, which must be in the same scene, naturally. In this example, you can press the up and down arrow keys to make the shape wipe zoom in or out, and press either 1, 2, or 3 to select different shapes (a circle, a diamond, and a star respectively). 
    Drag the two cameras onto the appropriate slots in the inspector after you've attached the script to an object. The script has a WipeTime public variable; this is the time it will take for the wipe to complete. Additionally, it has a RotateAmount public variable, which is the number of degrees that the shape will rotate over the course of the wipe. Therefore 0 is no rotation, 360 is one full rotation, 720 is two rotations, etc. A positive value means clockwise rotation, and a negative value means counter-clockwise rotation. Finally, there is a ShapeMesh array, that contains an array of meshes that correspond to the 1, 2, and 3 keys. 
    You can make whatever shapes you like in your 3D app. They must be centered on 0,0,0, and they must fill the screen when positioned just slightly beyond the camera's near clip plane (by default this is .3). The easiest way to test this is to drag the mesh into the hierarchy as a child of the camera, zero out its local rotation and position, and then change its z position to .301. 
    This wipe has a limitation in that it doesn't entirely play nicely with skyboxes or solid camera background colors, since one of the cameras has to get temporarily changed to "clear flags = depth only" during the wipe. It works with skyboxes/solid backgrounds under two situations: You're doing a grow and the first camera has no skybox or background color visible in the view frustum. Or you're doing a shrink and the second camera has no skybox or background color visible. Otherwise you get wacky behavior. 
    Also, any GUIElements (GUITexts and GUITextures) will overbrighten during the transition, since the two cameras are rendering them on top of each other, if both cameras have a GUILayer. To avoid this, make another camera that only renders GUIElements on top of the other cameras, and nothing else. If you're using OnGUI, then the GUI won't be affected. 
    The function is a coroutine: 
    function ShapeWipe (camera1 : Camera, camera2 : Camera, wipeTime : float, zoom : ZoomType, mesh : Mesh, rotateAmount : float) : IEnumerator 
    camera1 is the camera that you are wiping from and camera2 is the camera you are wiping to. wipeTime is the length of time, in seconds, it takes to complete the wipe. zoom is the ZoomType, which should be either ZoomType.Grow to zoom in or ZoomType.Shrink to zoom out. mesh is an arbitrary, bare Mesh (not a prefab). rotateAmount is the number of degrees (positive or negative) that the shape will rotate during the course of the zoom. 
    JavaScript - ShapeWipeExample.jsvar camera1 : Camera;
    var camera2 : Camera;
    var wipeTime = 2.0;
    var rotateAmount = 360.0;
    var shapeMesh : Mesh[];
    var curve : AnimationCurve;
    private var inProgress = false;
    private var swap = false;
    private var useShape = 0;
     
    function Update () {
    	if (Input.GetKeyDown("up")) {
    		DoWipe(ZoomType.Grow);
    	}
    	else if (Input.GetKeyDown("down")) {
    		DoWipe(ZoomType.Shrink);
    	}
     
    	if (Input.GetKeyDown("1")) {useShape = 0;}
    	if (Input.GetKeyDown("2")) {useShape = 1;}
    	if (Input.GetKeyDown("3")) {useShape = 2;}
    }
     
    function DoWipe (zoom : ZoomType) {
    	if (inProgress) return;
    	inProgress = true;
     
    	swap = !swap;
    	yield ScreenWipe.use.ShapeWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, zoom, shapeMesh[useShape], rotateAmount);
    	//yield ScreenWipe.use.ShapeWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, zoom, shapeMesh[useShape], rotateAmount, curve);
     
    	inProgress = false;
}To use this you also need the ScreenWipes script. 
}
