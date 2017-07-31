// Original url: http://wiki.unity3d.com/index.php/3DMenu
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/3DMenu.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Description This is a very basic example implementation of a menu-like mouse input detector. This script detects mouse overs and mouse clicks, the object this script is applied to will change to a red color when the mouse is over it. The game will quit or load level 1 depending on the value of isQuitBtn. 
Script var isQuitBtn : boolean = false;
 
private var startColor : Color = renderer.material.color;
 
function OnMouseEnter ()
{
	renderer.material.color = Color.red;
}
 
function OnMouseExit ()
{
	renderer.material.color = startColor;
}
 
function OnMouseUp()
{
	if(isQuitBtn)
	{
		Application.Quit();
	}
	else
	{
		Application.LoadLevel(1);
}
}
}
