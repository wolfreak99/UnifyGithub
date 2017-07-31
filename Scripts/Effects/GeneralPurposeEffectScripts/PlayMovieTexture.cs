// Original url: http://wiki.unity3d.com/index.php/PlayMovieTexture
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/PlayMovieTexture.cs
// File based on original modification date of: 15 July 2013, at 15:17. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{


Author: Jake Bayer (BakuJake14) 
DescriptionThis is a simple script that tells a texture (in this case, a Movie Texture) to play at the start of your game. Requires Unity Pro. 
UsageThe process goes as followed: 
If you haven't done so already, set up a Movie Texture in the Unity editor. Documentation for Movie Textures can be found here. 
Once you have a Movie Texture set up, attach the script below to the object holding the Movie Texture. 
Hit Play to see your Movie Texture. 
PlayMovie.cs//Written by Jake Bayer
//Posted July 15, 2013
 
//This is a simple script that plays a Movie Texture at the start of your game.
 
using UnityEngine;
using System.Collections;
 
public class PlayMovie : MonoBehaviour {
	private MovieTexture _texture;
 
	void Awake() {
		_texture = renderer.material.mainTexture as MovieTexture;
	}
 
	// Use this for initialization
	void Start () {
		_texture.Play();
 
	}
 
}
}
