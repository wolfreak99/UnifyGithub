// Original url: http://wiki.unity3d.com/index.php/GUIScaler
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/GUIScaler.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Rando Wiltschek (Orion) 
DescriptionThis script scales a GUITexture depending on the screen size / resolution. This allows the interface to be designed independently of a fixed resolution. 
UsagePlace this script on a GameObject with a GUITexture component. The scale and position that you use for "Pixel inset" of the GUITexture will be used as the smallest scale. I usually use 50% of the original size, so it works well in the game view. When the game is run at a higher resolution, the GUITexture will be scaled up to 2x (maxFactor). If you want your GUITexture to be anchored in a corner of the screen, set the object's transform x and y to 0 or 1 (or 0.5 for centered) and then adjust the Pixel inset to move the GUITexture. 
If another script needs to know the scaling factor of the GUI, you can just use GuiScaler.GetSize(); 
JavaScript - GuiScaler.js@script RequireComponent(GUITexture)
 
static private var size = 0.0;
// at what screen height should the texture be it's preset size (as setup in the inspector)?
static private var minAtScreenHeight = 384;
// at what screen height should the texture be fully scaled by maxFactor? 
static private var maxAtScreenHeight = 768;
static private var maxFactor = 2;
 
static function GetSize () : float
{
	if(size == 0)
	{
		var factor = Mathf.InverseLerp(minAtScreenHeight, maxAtScreenHeight, Screen.height);
		size = Mathf.Lerp(1, maxFactor, factor);
	}
	return size;
}
 
 
function Awake () 
{
	var mySize = GetSize();
	gui = GetComponent(GUITexture) as GUITexture;
	gui.pixelInset.x *= mySize;
	gui.pixelInset.y *= mySize;			
	gui.pixelInset.width *= mySize;			
	gui.pixelInset.height *= mySize;			
}
}
