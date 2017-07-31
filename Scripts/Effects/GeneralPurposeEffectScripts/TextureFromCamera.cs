// Original url: http://wiki.unity3d.com/index.php/TextureFromCamera
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/TextureFromCamera.cs
// File based on original modification date of: 20 January 2013, at 00:06. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Description Replaces the given target texture (usually our own) with a texture to be rendered by the given camera. 
by Matt "Trip" Maker, Monstrous Company :: http://monstro.us 
Usage Attach it to the GameObject upon which you wish to see the camera's view as a texture, or attach it anywhere else you like, if you specify the source and destination targets. 


using UnityEngine;
using System.Collections;
 
/// <summary>
/// Replace the given texture (usually our own) with a texture to be rendered by the given camera.
/// You may supply a renderTexture to use, or one will be ceated.
///
/// by Matt "Trip" Maker, Monstrous Company :: http://monstro.us
///
/// </summary>
public class TextureFromCamera : MonoBehaviour {
 
     public Camera cam;
     public Transform target;
     public RenderTexture renderTexture;
     public Vector2 textureSize = new Vector2(512,512);
     public int depth = 16;
 
     void Reset() {
          if (!cam) cam = Camera.main;
          if (!target) target = transform;
     }
 
     void Awake() {
          Reset();
     }
 
     void Start () {
          if ((depth != 0) && (depth != 16) && (depth != 24)) {
               //Debug.LogError(this + " depth must be one of 0, 16, 24");
          }
          if (!renderTexture) {
               renderTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, depth);//depth must be one of 0, 16, 24
               renderTexture.name = "TextureFromCamera_" + cam.name; //optional, but nice
               renderTexture.Create();
               //Debug.Log(renderTexture.depth + " width " + renderTexture.width + " height " + renderTexture.height);
          }
          cam.targetTexture = renderTexture;
          target.renderer.material.mainTexture = renderTexture;
     }
 
     void OnDisable() {
          renderTexture.DiscardContents();    
          renderTexture.Release();    
     }
}
}
