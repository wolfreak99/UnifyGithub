/*************************
 * Original url: http://wiki.unity3d.com/index.php/Position_To_Rect
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/Position_To_Rect.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    DescriptionA helper function to convert a GUITexture position to the equivalent Rect. 
    UsagePosToRect (position : Vector2, bounds : Rect, pixelInset : Rect) : Rect; 
    position - the guiTexture's transform.position x and y components as a Vector2. 
    bounds - the equivalent of a custom pixelRect for rendering the guiTexture into. Uses GUI coordinates. 
    pixelInset - the pixelInset (Rect) of the guiTexture. 
    function OnGUI () {
    	var rect = PosToRect(Vector2(pos.x,pos.y),Rect(10,10,200,200),Rect(-10,-10,20,20));
    	// set rect to the GUI Rect of a guiTexture at position pos with a pixelInset of (-10,-10,20,20)
    	// drawn as if by a camera with a pixelRect equivalent to (10,10,200,200)
    	GUI.DrawTexture(rect,texture); // draw the texture at the new rect
    }function PosToRect (pos : Vector2, bounds : Rect, inset : Rect) {
    	i = inset;
    	rect = new Rect(0,0,0,0);
    	rect.x = bounds.x + (bounds.width * pos.x) + i.x;
    	rect.y = bounds.y + (bounds.height * pos.y) + i.y;
    	rect.width = i.width;
    	rect.height = i.height;
    	return rect;
    };--DannyL 09:07, 22 September 2008 (PDT) 
}
