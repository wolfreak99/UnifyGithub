// Original url: http://wiki.unity3d.com/index.php/ScaledRect
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/ScaledRect.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Nick Tziamihas 
Contents [hide] 
1 Summary 
2 Integration 
3 Usage 
4 Notes 
5 Javascript- ScaledRect.js 

Summary OnGUI elements require Rectangle Structure coordinates (x, y, width, height) in order to display anything on the screen, which forces applications to be designed with only 1 resolution in mind. This static class allows the user to hard-code his coordinates for a single resolution without worrying about rescaling them. 
Integration Choose your default resolution, and set the Editor's Game Tab to that resolution. Preferably, choose a big resolution (such as 1920x1080), with the option to scale down from that, so you don't loose texture quality. Create a Javascript file called ScaledRect and paste the code beneath in it. Edit the customWidth and customHeight variables in the script to the values that you've decided to use in the Editor. ScaledRect.js is not a MonoBehaviour script and therefore does not need to be attached to an object in the hierarchy to work. It works straight from the Project tab. 
Usage Use the OnGUI functions (like GUI.DrawTexture or GUI.Button) normally. However instead of Rect, now type ScaledRect.scaledRect. 
Example 1: 
GUI.DrawTexture(Rect(0,0,1024,768),backgroundImage); 
becomes 
GUI.DrawTexture(ScaledRect.scaledRect(0,0,1024,768),backgroundImage); 
Example 2: 
GUI.Button(Rect(80,45,200,100),"BACK"); 
becomes 
GUI.Button(ScaledRect.scaledRect(80,45,200,100),"BACK"); 
Notes This feature allows projects that are near completion with hard-coded coordinates to be easily converted to dynamic scaling with a simple "Find and Replace" on the Script Editor. In addition, this function can also be used to scale non-GUI elements that use the Rect structure, such as Mouse or Touch inputs. Note however that this function doesn't change the font size on any GUI Text elements. Other work-arounds need to be used for this problem. 
Javascript- ScaledRect.jsstatic var customWidth : float = 960; //Set this value to the Width of your Game Tab in the Editor
static var customHeight : float = 640; //Set this value to the Height of your Game Tab in the Editor
 
static function scaledRect (x : float, y : float, width : float, height : float) {
	var returnRect : Rect;
	x = (x*Screen.width) / customWidth;
	y = (y*Screen.height) / customHeight;
	width = (width*Screen.width) / customWidth;
	height = (height*Screen.height) / customHeight;
 
	returnRect = Rect(x,y,width,height);
	return returnRect;
}
}
