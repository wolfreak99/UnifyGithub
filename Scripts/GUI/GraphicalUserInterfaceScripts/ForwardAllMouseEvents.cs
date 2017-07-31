// Original url: http://wiki.unity3d.com/index.php/ForwardAllMouseEvents
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/ForwardAllMouseEvents.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Jonathan Czeck (aarku) 
Contents [hide] 
1 Description 
2 Usage 
3 Advanced Usage 
4 C# - ForwardAllMouseEvents.cs 

DescriptionThis script will send mouse events to a target GameObject. 
UsageAssign this script to an object with a Collider, GUITexture, or GUIText. Then set the target property to the GameObject you want the mouse events forwarded to. 
Advanced UsageSee DraggableGUIElement. 
C# - ForwardAllMouseEvents.csusing UnityEngine;
using System.Collections;
 
public class ForwardAllMouseEvents : MonoBehaviour
{
    public GameObject target;
 
    void OnMouseEnter()
    {
        target.SendMessage("OnMouseEnter", null, SendMessageOptions.DontRequireReceiver);
    }
 
    void OnMouseOver()
    {
        target.SendMessage("OnMouseOver", null, SendMessageOptions.DontRequireReceiver);
    }
 
    void OnMouseExit()
    {
        target.SendMessage("OnMouseExit", null, SendMessageOptions.DontRequireReceiver);
    }
 
    void OnMouseDown()
    {
        target.SendMessage("OnMouseDown", null, SendMessageOptions.DontRequireReceiver);
    }
 
    void OnMouseUp()
    {
        target.SendMessage("OnMouseUp", null, SendMessageOptions.DontRequireReceiver);
    }
 
    void OnMouseDrag()
    {
        target.SendMessage("OnMouseDrag", null, SendMessageOptions.DontRequireReceiver);
    }
}
}
