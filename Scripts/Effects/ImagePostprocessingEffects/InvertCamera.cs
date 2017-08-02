/*************************
 * Original url: http://wiki.unity3d.com/index.php/InvertCamera
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/ImagePostprocessingEffects/InvertCamera.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: (Joachim Ante) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.ImagePostprocessingEffects
{
    DescriptionThis script inverts the view of the camera. So everything rendered by the camera is flipped. This will help you to implement a rear mirror camera. 
    UsageAttach the script to a camera. 
    JavaScript - InvertCamera.js// EXAMPLE WITH CAMERA UPSIDEDOWN
    function OnPreCull () {
    	camera.ResetWorldToCameraMatrix ();
    	camera.ResetProjectionMatrix ();
    	camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(Vector3 (1, -1, 1));
    }
     
    function OnPreRender () {
    	GL.SetRevertBackfacing (true);
    }
     
    function OnPostRender () {
    	GL.SetRevertBackfacing (false);
    }
     
@script RequireComponent (Camera)
}
