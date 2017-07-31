// Original url: http://wiki.unity3d.com/index.php/Animated_Color_Procedural_Texture
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Animated_Color_Procedural_Texture.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Antony Stewart 
Description Variation of the procedural perlin noise demo script, to generate textures with mixes of RGB by blending 3 independant RGB x,y, graphs. Also HSBColor. 
Usage Attach this script to the object that you wish to give a special effects color. Write maths functions for the red, blue, green variables that compute between 0 and 1, based on x,y, graphs. pastel colors are closer to .7 and bright colors closer to 1. for example if you want a psycadelically angry robot, make a black and red dot graph and animate with Time.time*30, i.e. warping very fast. You need to know abit about 2D graphs. 
straight =sin(x*.3) 
vertical =sin(y*.3) 
oblique = sin(x+y) or (x-y) 
wiggle = sin(x*.2+sin(y*.5)*.3) 
complex wiggles = add multiple wiggles together. 
Note: x and y =128 so sensible sine multipliers are 0.1,0.345 etc, not 20. 

JavaScript - AnimatedTextureUV.js var gray = true;
var width = 128;
var height = 128;
 
private var texture : Texture2D;
 
function Start ()
{
	texture = new Texture2D(width, height, TextureFormat.RGB24, false);
	renderer.material.mainTexture = texture;
}
 
function Update()
{
	Calculate();
}
 
function Calculate()
{
 
 
	for (var y = 0;y<height;y++)
	{
		for (var x = 0;x<width;x++)
		{
			if (gray)
			{
			var red = 0;
			var green = Mathf.Sin(x*.5+Time.time+Mathf.Sin(y*.23)*.8)/5+.5;
			var blue = Mathf.Sin(x*.3+Time.time+Mathf.Sin(x*.43)*.4)/5+.5;
 
				texture.SetPixel(x, y, Color (red, green, blue, 1));
			}
			else
			{
 
 
				texture.SetPixel(x, y, Color (1, 0, 0, 1));
			}
		}	
	}
 
	texture.Apply();
}
}
