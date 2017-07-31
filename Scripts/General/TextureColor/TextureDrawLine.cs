// Original url: http://wiki.unity3d.com/index.php/TextureDrawLine
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/TextureColor/TextureDrawLine.cs
// File based on original modification date of: 13 August 2013, at 04:17. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.TextureColor
{
Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 TextureDraw.js 
4 TextureDraw.cs 

DescriptionDraws single-pixel-wide, non-anti-aliased lines inside a texture. This isn't for drawing lines on the screen (unless the texture fills the entire screen, of course). Line drawing code adapted from some example code in another web page somewhere on teh intarnets. 
 
Usage Have this script somewhere in your project. Call the function by the name of the script ("TextureDraw" in this case) plus ".Line". The arguments are: 
function Line (texture : Texture2D, x1 : int, y1 : int, x2 : int, y2 : int, color : Color) 
"texture" is the texture you want to draw the line in, "x1" and "y1" are the x and y coordinates of the starting point of the line, "x2" and "y2" are the x and y coordinates of the ending point of the line, "color" is the color of the line. Lines drawn in the texture won't show up until Apply() is called; for speed reasons, this should be done last if more than one line is being drawn at once. A code example, which draws some random lines in a texture and results in something similar to the image above: 
var texSize = 256;
var lines = 20;
 
function Start () {
	var tex = new Texture2D(texSize, texSize);
	for (i = 0; i < lines; i++) {
		TextureDraw.Line(tex, Random.Range(0, texSize), Random.Range(0, texSize), Random.Range(0, texSize), Random.Range(0, texSize),
			Color(Random.Range(0.25, 1.0), Random.Range(0.25, 1.0), Random.Range(0.25, 1.0)) );
	}
	tex.Apply();
	renderer.material.mainTexture = tex;
}What happens to points drawn outside the texture boundaries depends on whether the texture is set to clamp or repeat: clamp means the line won't draw past the edge, and repeat makes the line start over from the other side (this behavior is inherent to SetPixel). 
TextureDraw.js static function Line (tex : Texture2D, x0 : int, y0 : int, x1 : int, y1 : int, col : Color) {
	var dy = y1-y0;
	var dx = x1-x0;
 
	if (dy < 0) {dy = -dy; var stepy = -1;}
	else {stepy = 1;}
	if (dx < 0) {dx = -dx; var stepx = -1;}
	else {stepx = 1;}
	dy <<= 1;
	dx <<= 1;
 
	tex.SetPixel(x0, y0, col);
	if (dx > dy) {
		var fraction = dy - (dx >> 1);
		while (x0 != x1) {
			if (fraction >= 0) {
				y0 += stepy;
				fraction -= dx;
			}
			x0 += stepx;
			fraction += dy;
			tex.SetPixel(x0, y0, col);
		}
	}
	else {
		fraction = dx - (dy >> 1);
		while (y0 != y1) {
			if (fraction >= 0) {
				x0 += stepx;
				fraction -= dy;
			}
			y0 += stepy;
			fraction += dx;
			tex.SetPixel(x0, y0, col);
		}
	}
}

TextureDraw.cs void DrawLine(Texture2D tex, int x1, int y1, int x2, int y2, Color col)
{
 	int dy = (int)(y1-y0);
	int dx = (int)(x1-x0);
 	int stepx, stepy;
 
	if (dy < 0) {dy = -dy; stepy = -1;}
	else {stepy = 1;}
	if (dx < 0) {dx = -dx; stepx = -1;}
	else {stepx = 1;}
	dy <<= 1;
	dx <<= 1;
 
	float fraction = 0;
 
	tex.SetPixel(x0, y0, col);
	if (dx > dy) {
		fraction = dy - (dx >> 1);
		while (Mathf.Abs(x0 - x1) > 1) {
			if (fraction >= 0) {
				y0 += stepy;
				fraction -= dx;
			}
			x0 += stepx;
			fraction += dy;
			tex.SetPixel(x0, y0, col);
		}
	}
	else {
		fraction = dx - (dy >> 1);
		while (Mathf.Abs(y0 - y1) > 1) {
			if (fraction >= 0) {
				x0 += stepx;
				fraction -= dy;
			}
			y0 += stepy;
			fraction += dx;
			tex.SetPixel(x0, y0, col);
		}
	}
}
}
