// Original url: http://wiki.unity3d.com/index.php/ToggleButton
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/ToggleButton.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Keli Hlodverrson (freyr) 
DescriptionThis script uses a GuiTexture and Unity mouse events to implement a regular toggle button that behaves properly like Mac OS X. 
UsageAttach this script to a GuiTexture object. Add a ButtonPressed function to the object pointed to by the messagee variable to catch when the button has been pressed. (You can change the name of the function by changing the message variable.) 
C# - ToggleButton.csThe ToggleButton is created by extending the Button class. You'll need to have both files present in your project for this snippet to work. Note that this script uses features only available in Unity 1.2. Read the comments in Button.cs for more information 
using UnityEngine;
using System.Collections;
 
[AddComponentMenu ("GUI/Toggle Button")] 
public class ToggleButton : Button {
 
    // A second set of textures for showing when the toggle button is selected
    public ButtonTextures selectedTextures; 
    public bool selected=false; // The selected/unselected state of the button
 
    // Override the SetButtonTexture method in Button to make it show a 
    // different set of textures when selected is true.
    protected override void SetButtonTexture(ButtonState state) {
        if(selected)
             myGUITexture.texture=selectedTextures[state];
        else
             myGUITexture.texture=textures[state];
    }
 
    public override void Reset() {
        selected=false;
        base.Reset(); // Calls the overridden Reset function.
    }
 
    // OnMouseUp needs to be overridden so we can flip the selected status 
    // when the user clicks the button.
    public override void OnMouseUp()
    {
 
        if (state == 2)
            selected = !selected;
 
        base.OnMouseUp(); // Do whatever the base class used to do...
    }
 
}
}
