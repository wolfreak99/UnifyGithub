/*************************
 * Original url: http://wiki.unity3d.com/index.php/Selection-Grid
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/Selection-Grid.cs
 * File based on original modification date of: 9 June 2014, at 10:26. 
 *
 * Author: Bérenger 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Description makes a grid using a texture of your choice, 1 draw call, make the grid in art program, returns integer of button pressed. 
    Usage get / make a grid texture representing your buttons. change grid pixels of x an y to those of your grid. 
    //this example uses a texture2d of x = 12 squares and y = 12 squares, 
    //i made the texture 360 but it turned to 240 in button so each button is 20 pixels.
     
     
    function OnGUI(){
    				var gridpixels = 20;//pixels per grid square
    				var gidxsquares = 12;//num squares in x direction
    				if (GUI.Button(Rect(320,10,240,60),fctbutton,GUIStyle.none))
    		{
    				var xpos = Input.mousePosition.x - 320 ;
    				var ypos = Screen.height - Input.mousePosition.y -10;
     
    				var result =  Mathf.Floor(xpos / gridpixels) + Mathf.Floor(ypos / gridpixels)*gidxsquares + 1;//plus 1 at end for not zero first square
    				//
    				print (result);
    		}	
}
}
