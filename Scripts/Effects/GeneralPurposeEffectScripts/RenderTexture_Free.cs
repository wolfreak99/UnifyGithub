// Original url: http://wiki.unity3d.com/index.php/RenderTexture_Free
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/RenderTexture_Free.cs
// File based on original modification date of: 18 June 2012, at 01:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Description Simulate a render texture by copying what is displayed on screen into a texture, GUI included. A Texture.Apply() is performed, which is extremely slow, so it is a bad idea to capture the screen every frames. 
Usage Attach the following script to the object receiving the texture. 
using UnityEngine;
using System.Collections;
 
public class RTtest : MonoBehaviour 
{	
	public float refresh = 1f;
 
	// Update is called once per frame
	void Start (){
		InvokeRepeating( "SimulateRenderTexure", 0f, refresh );
	}
 
	void SimulateRenderTexure(){		
		renderer.material.mainTexture = RenderTextureFree.Capture();
	}
}RenderTextureFree.cs using UnityEngine;
 
/// <summary>
/// Simulate a RenderTexture. The process need a Texture.Apply, which takes a lot of time,
/// so don't use this every frames.
///
/// by Berenger Mantoue, www.berengermantoue.fr
///
/// </summary>
 
public class RenderTextureFree
{
	private static Texture2D tex2D;
	private static Texture tex;
 
	// Return the entire screen in a texture
	public static Texture Capture(){ return Capture( new Rect( 0f, 0f, Screen.width, Screen.height ), 0, 0 ); }
 
	// Return part of the screen in a texture.
	public static Texture Capture( Rect captureZone, int destX, int destY )
	{
		Texture2D result;
		result = new Texture2D( Mathf.RoundToInt( captureZone.width ) + destX,
					Mathf.RoundToInt( captureZone.height ) + destY,
					TextureFormat.RGB24, false);
		result.ReadPixels(captureZone, destX, destY, false);
 
		// That's the heavy part, it takes a lot of time.
		result.Apply();
 
		return result;
	}
}
}
