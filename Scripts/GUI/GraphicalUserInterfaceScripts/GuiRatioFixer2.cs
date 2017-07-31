// Original url: http://wiki.unity3d.com/index.php/GuiRatioFixer2
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/GuiRatioFixer2.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Alejandro Gonzalez (zerofractal) 
DescriptionThis is an alternate script to adjust the aspect ratio of a GUIText or GUITexture object by scaling it horizontally to fit the proportions. 
It works with textures of equal or unequal width and height, such as 1024x512 maps. This script takes the aspect from the actual screen making it useful for 4:3 16:9 etc etc. 
It does not know how to align groups of GUI objects after it adjusts their aspect ratio. 
UsagePlace this script on a GameObject with a GUIText and/or GUITexture component. Sacle the object to the correct height in Game View. The width does not matter. Place the object where you want it to appear. Please take into accout that the center will be preserved but not the alignment, so depending on the aspect it will vary. 


JavaScript - AspectCorrection.js//Aspect Correction Script for Unity - Zerofractal 2006
//This script can be placed on any GUI element to make its aspect correct to 1:1. 
//The scale is based on the height of the element. 
//It works with textures of equal or unequal width and height, such as 1024x512 maps. 
//Unlike other scripts, this one takes the aspect from the actual screen making it useful for 4:3 16:9 etc etc.
 
function Awake() {
   RefreshAspect();
}
 
function RefreshAspect() {
   var aspect: float = 1/Camera.main.aspect;
   transform.localScale.x = transform.localScale.y * aspect * guiElement.texture.width/guiElement.texture.height;
   transform.position = transform.localPosition;
}
}
