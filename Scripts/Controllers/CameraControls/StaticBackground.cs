// Original url: http://wiki.unity3d.com/index.php/StaticBackground
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/StaticBackground.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
This script allows you to have a static image as the background to your scene. Like a skybox that doesn't move. This script can be used only in Unity Pro. 
StaticBackground.cs 
C# // You can place this directly onto your camera.
//
// EDIT: RenderBeforeQueues is not supported anymore in Unity 3.x 
 
using UnityEngine;
 
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Static Background")]
public class StaticBackground : MonoBehaviour {
    public Texture2D background;
 
	// This is not supported anymore in Unity 3.x
	// [RenderBeforeQueues( 1000 )]
    // void OnRenderObject( int queueIndex ) {
	// OnPreRender is used instead ( onPreCull works too )
 
	void OnPreRender (){
        if( background != null )
            Graphics.Blit( background, RenderTexture.active );
    }
 
}
Feel free to ask me any questions regarding this script 
 -Ryan Scott-
}
