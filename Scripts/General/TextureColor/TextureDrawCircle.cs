/*************************
 * Original url: http://wiki.unity3d.com/index.php/TextureDrawCircle
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/TextureColor/TextureDrawCircle.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.TextureColor
{
    DescriptionDraws single-pixel-wide, non-anti-aliased circles inside a texture. This isn't for drawing circles on the screen (unless the texture fills the entire screen, of course). Circle drawing code adapted from some example code in another web page somewhere on teh intarnets. 
     
    Usage Have this script somewhere in your project. Call the function by the name of the script ("TextureDraw" in this case) plus ".Circle". The arguments are: 
    function Circle (texture : Texture2D, x : int, y : int, radius : int, color : Color) 
    "texture" is the texture you want to draw the circle in, "x" and "y" are the x and y coordinates of the midpoint of the circle, "radius" is the radius of the circle in pixels, "color" is the color of the circle. Circles drawn in the texture won't show up until Apply() is called; for speed reasons, this should be done last if more than one circle is being drawn at once. A code example, which draws some random circles in a texture and results in something similar to the image above: 
    var texSize = 256;
    var circles = 10;
     
    function Start () {
    	var tex = new Texture2D(texSize, texSize);
    	for (i = 0; i < circles; i++) {
    		TextureDraw.Circle(tex, Random.Range(0, texSize), Random.Range(0, texSize), Random.Range(1, texSize/4),
    			Color(Random.Range(0.25, 1.0), Random.Range(0.25, 1.0), Random.Range(0.25, 1.0)) );
    	}
    	tex.Apply();
    	renderer.material.mainTexture = tex;
    }What happens to points drawn outside the texture boundaries depends on whether the texture is set to clamp or repeat: clamp means the circle won't draw past the edges, and repeat makes the circle start over from the other side (this behavior is inherent to SetPixel). 
    TextureDraw.js static function Circle (tex : Texture2D, cx : int, cy : int, r : int, col : Color) {
    	var y = r;
    	var d = 1/4 - r;
    	var end = Mathf.Ceil(r/Mathf.Sqrt(2));
     
    	for (x = 0; x <= end; x++) {
    		tex.SetPixel(cx+x, cy+y, col);
    		tex.SetPixel(cx+x, cy-y, col);
    		tex.SetPixel(cx-x, cy+y, col);
    		tex.SetPixel(cx-x, cy-y, col);
    		tex.SetPixel(cx+y, cy+x, col);
    		tex.SetPixel(cx-y, cy+x, col);
    		tex.SetPixel(cx+y, cy-x, col);
    		tex.SetPixel(cx-y, cy-x, col);
     
    		d += 2*x+1;
    		if (d > 0) {
    			d += 2 - 2*y--;
    		}
    	}
}
}
