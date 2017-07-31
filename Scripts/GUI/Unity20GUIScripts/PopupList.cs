// Original url: http://wiki.unity3d.com/index.php/PopupList
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/Unity20GUIScripts/PopupList.cs
// File based on original modification date of: 8 January 2015, at 23:51. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.Unity20GUIScripts
{

Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 Javascript - PopupListUsageExample.js 
4 C# - Popup.cs 
5 C# - Popup.cs - Updated 
6 C# - Popup.cs - Multi-Instance Support 
7 C# - ComboBox - Update 
8 C# Example Class 
9 C# Example Class 

Description Creates a popup list button as an extension to OnGUI. When the button is clicked, a selectable list pops up, and disappears when the mouse button is released. 
 
Usage Put the Popup.cs script below in your Standard Assets/Scripts folder (if you don't have one, make one). This way it can be accessed from any language easily without having to worry about compilation order problems. You can then access the List function by calling Popup.List, which must be called from within OnGUI. 
When called, it returns true if the user has selected an item from the list and false otherwise. It requires an existing boolean to be passed in, which reflects whether the list is currently being shown or not. It also requires an existing integer to be passed in, which is the index of the list entry that's selected when the user releases the mouse button. 

function List (position : Rect, ref showList : boolean, ref listEntry : int, buttonContent : GUIContent, listContent : GUIContent[], buttonStyle : GUIStyle, boxStyle : GUIStyle, listStyle : GUIStyle) : boolean 
position is the rect where the button should be located. showList is a boolean that's true if the list is being shown. This is a reference boolean, so when used with C#, it must be preceded with "ref". listEntry is an integer that holds the value of the list entry that the user selected. This is a reference integer, so when used with C#, it must be preceded with "ref". buttonContent is the GUIContent that's displayed on the button. listContent is a GUIContent array that contains the list that's displayed when the button is clicked. 
Of the last three GUIStyle variables, the first two are optional. If they are not supplied, the current "button" and "box" styles from the current GUI skin are used. Otherwise, buttonStyle is the GUIStyle for the button, and boxStyle is the GUIStyle for the box that the list is displayed in. listStyle is not optional, and is the style used for displaying the list contents. 
Note that the default Unity GUI skin isn't entirely suitable for this function. The GUIStyle used for the list needs to have a texture background for the Hover and OnHover entries, so that the user has some kind of visual feedback as to which item in the list will be selected, and none of the default styles look particularly good for this ("button" might do in a pinch). However, it's pretty easy to add a custom style, and a small solid-colored texture will do nicely for the background. See the usage example below for an example of this. Also remember that GUIContent can have both text and an image, so the button and list can have associated icons if desired (see screenshot at the top of this article). 



Update John H. (Kentu) 8/11/2010 
I changed this C# script to accept an abstract array, and also be able to assign a delegate upon mouse up. To pass in a List<T> you would for example do this YourList.ToArray() simple right. You will also want to pass in your function as the delegate callback. This can be extended to not take a callback delegate, this is just mainly for example. For the item to return the correct name, the objects you are creating your class/List<T> from, you just simple Overide the ToString method, Example code file below. 

Update Hyungseok Seo. (Jerry) 28/12/2010 
I changed this C# code, because when I clicked this button, I want to see seperate gui for ComboBox and List. (you can see the result from Decription's last picture). And also I want to use PopupList like ComboBox control. That mean's control have some data. (for example, selected item index.). so you don't need to care for that kind of data anymore, when you want to use PopupList. 
Update MarkGX, 13/01/2011 
The ComboBox has an error where you have to click it twice before it appears with the drop down. Here is the code fix 
        if( GUI.Button( rect, buttonContent, buttonStyle ) )
        {
			if (useControlID == -1)
			{
				useControlID = controlID;
				isClickedComboButton = false;
			}
 
            if( useControlID != controlID )
            {
                forceToUnShow = true;
                useControlID = controlID;
            }
            isClickedComboButton = true;
        }Update Jerry 25/01/2011 
Fixed bug. - Thanks MarkGX. :) 
Javascript - PopupListUsageExample.js //intended for use with original popup.cs by Eric5h5 (not updated version)
//works with multiple Popub buttons, if you duplicate the arrays used in this example. 
//But dont forget to make the first line. showlist = false, duplicate, 
//have as many of this boolean value as you have buttons.
//picked variable isnt essential, in fact if you use multiple popuplists it wont reset to falst 
//so may as well omit it.
 
private var showList = false;
private var listEntry = 0;
private var list : GUIContent[];
private var listStyle : GUIStyle;
private var picked = false;
 
function Start () {
	// Make some content for the popup list
	list = new GUIContent[5];
	list[0] = new GUIContent("Foo");
	list[1] = new GUIContent("Bar");
	list[2] = new GUIContent("Thing1");
	list[3] = new GUIContent("Thing2");
	list[4] = new GUIContent("Thing3");
 
	// Make a GUIStyle that has a solid white hover/onHover background to indicate highlighted items
	listStyle = new GUIStyle();
	listStyle.normal.textColor = Color.white;
	var tex = new Texture2D(2, 2);
	var colors = new Color[4];
	for (color in colors) color = Color.white;
	tex.SetPixels(colors);
	tex.Apply();
	listStyle.hover.background = tex;
	listStyle.onHover.background = tex;
	listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;
}
 
function OnGUI () {
	if (Popup.List (Rect(50, 100, 100, 20), showList, listEntry, GUIContent("Click me!"), list, listStyle)) {
		picked = true;
	}
	if (picked) {
		GUI.Label (Rect(50, 70, 400, 20), "You picked " + list[listEntry].text + "!");
	}
}C# - Popup.cs using UnityEngine;
 
public class Popup {
	static int popupListHash = "PopupList".GetHashCode();
 
	public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, GUIContent[] listContent,
							 GUIStyle listStyle) {
		return List(position, ref showList, ref listEntry, buttonContent, listContent, "button", "box", listStyle);
	}
 
	public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, GUIContent[] listContent,
							 GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle) {
		int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
		bool done = false;
		switch (Event.current.GetTypeForControl(controlID)) {
			case EventType.mouseDown:
				if (position.Contains(Event.current.mousePosition)) {
					GUIUtility.hotControl = controlID;
					showList = true;
				}
				break;
			case EventType.mouseUp:
				if (showList) {
					done = true;
				}
				break;
		}
 
		GUI.Label(position, buttonContent, buttonStyle);
		if (showList) {
			Rect listRect = new Rect(position.x, position.y, position.width, listStyle.CalcHeight(listContent[0], 1.0f)*listContent.Length);
			GUI.Box(listRect, "", boxStyle);
			listEntry = GUI.SelectionGrid(listRect, listEntry, listContent, 1, listStyle);
		}
		if (done) {
			showList = false;
		}
		return done;
	}
}

C# - Popup.cs - Updated // Popup list created by Eric Haines
// Popup list Extended by John Hamilton. john@nutypeinc.com
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class Popup {
    static int popupListHash = "PopupList".GetHashCode();
	// Delegate
	public delegate void ListCallBack();
 
 
 
    public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, object[] list ,
                             GUIStyle listStyle, ListCallBack callBack) {
 
 
 
        return List(position, ref showList, ref listEntry, buttonContent, list, "button", "box", listStyle, callBack);
	}
 
    public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent,  object[] list,
                             GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle, ListCallBack callBack) {
 
 
        int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
        bool done = false;
        switch (Event.current.GetTypeForControl(controlID)) {
            case EventType.mouseDown:
                if (position.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = controlID;
                    showList = true;
                }
                break;
            case EventType.mouseUp:
                if (showList) {
                    done = true;
                     // Call our delegate method
				callBack();
                }
                break;
        }
 
        GUI.Label(position, buttonContent, buttonStyle);
        if (showList) {
 
			// Get our list of strings
			string[] text = new string[list.Length];
			// convert to string
			for (int i =0; i<list.Length; i++)
			{
				text[i] = list[i].ToString();
			}
 
            Rect listRect = new Rect(position.x, position.y, position.width, list.Length * 20);
            GUI.Box(listRect, "", boxStyle);
            listEntry = GUI.SelectionGrid(listRect, listEntry, text, 1, listStyle);
        }
        if (done) {
            showList = false;
        }
        return done;
    }
}

C# - Popup.cs - Multi-Instance Support // Popup list with multi-instance support created by Xiaohang Miao. (xmiao2@ncsu.edu)
 
using UnityEngine;
public class Popup{
 
	// Represents the selected index of the popup list, the default selected index is 0, or the first item
	private int selectedItemIndex = 0;
 
	// Represents whether the popup selections are visible (active)
	private bool isVisible = false;
 
	// Represents whether the popup button is clicked once to expand the popup selections
	private bool isClicked = false;
 
	// If multiple Popup objects exist, this static variable represents the active instance, or a Popup object whose selection is currently expanded
	private static Popup current;
 
	// This function is ran inside of OnGUI()
	// For usage, see http://wiki.unity3d.com/index.php/PopupList#Javascript_-_PopupListUsageExample.js
	public int List(Rect box, GUIContent[] items, GUIStyle boxStyle, GUIStyle listStyle) {
 
		// If the instance's popup selection is visible
		if(isVisible) {
 
			// Draw a Box
			Rect listRect = new Rect( box.x, box.y + box.height, box.width, box.height * items.Length);
			GUI.Box( listRect, "", boxStyle );
 
			// Draw a SelectionGrid and listen for user selection
			selectedItemIndex = GUI.SelectionGrid( listRect, selectedItemIndex, items, 1, listStyle );
 
			// If the user makes a selection, make the popup list disappear
			if(GUI.changed) {
				current = null;
			}
		}
 
		// Get the control ID
		int controlID = GUIUtility.GetControlID( FocusType.Passive );
 
		// Listen for controls
		switch( Event.current.GetTypeForControl(controlID) )
		{
			// If mouse button is clicked, set all Popup selections to be retracted
			case EventType.mouseUp:
			{
				current = null;
				break;
			}	
		}	
 
		// Draw a button. If the button is clicked
		if(GUI.Button(new Rect(box.x,box.y,box.width,box.height),items[selectedItemIndex])) {
 
			// If the button was not clicked before, set the current instance to be the active instance
			if(!isClicked) {
				current = this;
				isClicked = true;
			}
			// If the button was clicked before (it was the active instance), reset the isClicked boolean
			else {
				isClicked = false;
			}
		}
 
		// If the instance is the active instance, set its popup selections to be visible
		if(current == this) {
			isVisible = true;
		}
 
		// These resets are here to do some cleanup work for OnGUI() updates
		else {
			isVisible = false;
			isClicked = false;
		}
 
		// Return the selected item's index
		return selectedItemIndex;
	}
 
	// Get the instance variable outside of OnGUI()
	public int GetSelectedItemIndex()
	{
		return selectedItemIndex;
	}
}C# - ComboBox - Update // Popup list created by Eric Haines
// ComboBox Extended by Hyungseok Seo.(Jerry) sdragoon@nate.com
// 
// -----------------------------------------------
// This code working like ComboBox Control.
// I just changed some part of code, 
// because I want to seperate ComboBox button and List.
// ( You can see the result of this code from Description's last picture )
// -----------------------------------------------
//
// === usage ======================================
//
// public class SomeClass : MonoBehaviour
// {
//	GUIContent[] comboBoxList;
//	private ComboBox comboBoxControl = new ComboBox();
//	private GUIStyle listStyle = new GUIStyle();
//
//	private void Start()
//	{
//	    comboBoxList = new GUIContent[5];
//	    comboBoxList[0] = new GUIContent("Thing 1");
//	    comboBoxList[1] = new GUIContent("Thing 2");
//	    comboBoxList[2] = new GUIContent("Thing 3");
//	    comboBoxList[3] = new GUIContent("Thing 4");
//	    comboBoxList[4] = new GUIContent("Thing 5");
//
//	    listStyle.normal.textColor = Color.white; 
//	    listStyle.onHover.background =
//	    listStyle.hover.background = new Texture2D(2, 2);
//	    listStyle.padding.left =
//	    listStyle.padding.right =
//	    listStyle.padding.top =
//	    listStyle.padding.bottom = 4;
//	}
//
//	private void OnGUI () 
//	{
//	    int selectedItemIndex = comboBoxControl.GetSelectedItemIndex();
//	    selectedItemIndex = comboBoxControl.List( 
//			new Rect(50, 100, 100, 20), comboBoxList[selectedItemIndex].text, comboBoxList, listStyle );
//          GUI.Label( new Rect(50, 70, 400, 21), 
//			"You picked " + comboBoxList[selectedItemIndex].text + "!" );
//	}
// }
//
// =================================================
 
using UnityEngine;
 
public class ComboBox
{
	private static bool forceToUnShow = false; 
	private static int useControlID = -1;
	private bool isClickedComboButton = false;	
 
	private int selectedItemIndex = 0;	
 
	public int List( Rect rect, string buttonText, GUIContent[] listContent, GUIStyle listStyle )
	{
	    return List( rect, new GUIContent( buttonText ), listContent, "button", "box", listStyle );
	}
 
	public int List( Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle )
	{
	    return List( rect, buttonContent, listContent, "button", "box", listStyle);
	}
 
	public int List( Rect rect, string buttonText, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle )
	{
	    return List( rect, new GUIContent( buttonText ), listContent, buttonStyle, boxStyle, listStyle );
	}
 
	public int List( Rect rect, GUIContent buttonContent, GUIContent[] listContent,
                                    GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle )
	{
	    if( forceToUnShow )
	    {
	        forceToUnShow = false;
	        isClickedComboButton = false;			
	    }
 
	    bool done = false;
 	    int controlID = GUIUtility.GetControlID( FocusType.Passive );		
 
 	    switch( Event.current.GetTypeForControl(controlID) )
	    {
   	        case EventType.mouseUp:
   	        {
   	            if( isClickedComboButton )
   	            {
   	                done = true;
   	            }
   	        }
   	        break;
	    }		
 
 	    if( GUI.Button( rect, buttonContent, buttonStyle ) )
   	    {
   	        if( useControlID == -1 )
   	        {
   	            useControlID = controlID;
   	            isClickedComboButton = false;
   	        }
 
   	        if( useControlID != controlID )
   	        {
   	            forceToUnShow = true;
   	            useControlID = controlID;
   	        }
   	        isClickedComboButton = true;
	    }
 
	    if( isClickedComboButton )
	    {
	        Rect listRect = new Rect( rect.x, rect.y + listStyle.CalcHeight(listContent[0], 1.0f),
					  rect.width, listStyle.CalcHeight(listContent[0], 1.0f) * listContent.Length );
 
	        GUI.Box( listRect, "", boxStyle );
	        int newSelectedItemIndex = GUI.SelectionGrid( listRect, selectedItemIndex, listContent, 1, listStyle );
	        if( newSelectedItemIndex != selectedItemIndex )
	            selectedItemIndex = newSelectedItemIndex;
	    }
 
	    if( done )
	        isClickedComboButton = false;
 
	    return GetSelectedItemIndex();
	}
 
	public int GetSelectedItemIndex()
	{
	    return selectedItemIndex;
	}
}
I just refactored Hyungseok Seo.(Jerry) sdragoon@nate.com's ComboBox to oop. I don't know how the "button" and "box" string be converted to GUIStyle, so leave the constructor's prameter as "string buttonStyle" and "string boxStyle". 
This version doesn't allow the presenting script to control the `selectedItemIndex`, so there is no capability to return back to the default value.  This became problematic for me when an action took place because of certain returned index that hid the `ComboBox`, and a later action tried to restore the original layout re-showing the `ComboBox`, resulting in “ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint” errors (because the state was being changes to re-show the `ComboBox`, but during the Layout event phase the `ComboBox`'s persistent selected index triggers hiding itself again.  CapnSlipp (talk) 00:46, 9 January 2015 (CET) 
C# Example Class /*
 * 
// Popup list created by Eric Haines
// ComboBox Extended by Hyungseok Seo.(Jerry) sdragoon@nate.com
// this oop version of ComboBox is refactored by zhujiangbo jumbozhu@gmail.com
// 
// -----------------------------------------------
// This code working like ComboBox Control.
// I just changed some part of code, 
// because I want to seperate ComboBox button and List.
// ( You can see the result of this code from Description's last picture )
// -----------------------------------------------
//
// === usage ======================================
using UnityEngine;
using System.Collections;
 
public class ComboBoxTest : MonoBehaviour
{
	GUIContent[] comboBoxList;
	private ComboBox comboBoxControl;// = new ComboBox();
	private GUIStyle listStyle = new GUIStyle();
 
	private void Start()
	{
		comboBoxList = new GUIContent[5];
		comboBoxList[0] = new GUIContent("Thing 1");
		comboBoxList[1] = new GUIContent("Thing 2");
		comboBoxList[2] = new GUIContent("Thing 3");
		comboBoxList[3] = new GUIContent("Thing 4");
		comboBoxList[4] = new GUIContent("Thing 5");
 
		listStyle.normal.textColor = Color.white; 
		listStyle.onHover.background =
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.left =
		listStyle.padding.right =
		listStyle.padding.top =
		listStyle.padding.bottom = 4;
 
		comboBoxControl = new ComboBox(new Rect(50, 100, 100, 20), comboBoxList[0], comboBoxList, "button", "box", listStyle);
	}
 
	private void OnGUI () 
	{
		int selectedItemIndex = comboBoxControl.Show();
		GUI.Label( new Rect(50, 70, 400, 21), "dfdsfYou picked " + comboBoxList[selectedItemIndex].text + "!" );
	}
}
 
*/
 
using UnityEngine;
 
public class ComboBox
{
    private static bool forceToUnShow = false; 
    private static int useControlID = -1;
    private bool isClickedComboButton = false;
    private int selectedItemIndex = 0;
 
	private Rect rect;
	private GUIContent buttonContent;
	private GUIContent[] listContent;
	private string buttonStyle;
	private string boxStyle;
	private GUIStyle listStyle;
 
    public ComboBox( Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle ){
		this.rect = rect;
		this.buttonContent = buttonContent;
		this.listContent = listContent;
		this.buttonStyle = "button";
		this.boxStyle = "box";
		this.listStyle = listStyle;
    }
 
	public ComboBox(Rect rect, GUIContent buttonContent, GUIContent[] listContent, string buttonStyle, string boxStyle, GUIStyle listStyle){
		this.rect = rect;
		this.buttonContent = buttonContent;
		this.listContent = listContent;
		this.buttonStyle = buttonStyle;
		this.boxStyle = boxStyle;
		this.listStyle = listStyle;
	}
 
    public int Show()
    {
        if( forceToUnShow )
        {
            forceToUnShow = false;
            isClickedComboButton = false;
        }
 
        bool done = false;
        int controlID = GUIUtility.GetControlID( FocusType.Passive );       
 
        switch( Event.current.GetTypeForControl(controlID) )
        {
            case EventType.mouseUp:
            {
                if( isClickedComboButton )
                {
                    done = true;
                }
            }
            break;
        }       
 
        if( GUI.Button( rect, buttonContent, buttonStyle ) )
        {
            if( useControlID == -1 )
            {
                useControlID = controlID;
                isClickedComboButton = false;
            }
 
            if( useControlID != controlID )
            {
                forceToUnShow = true;
                useControlID = controlID;
            }
            isClickedComboButton = true;
        }
 
        if( isClickedComboButton )
        {
            Rect listRect = new Rect( rect.x, rect.y + listStyle.CalcHeight(listContent[0], 1.0f),
                      rect.width, listStyle.CalcHeight(listContent[0], 1.0f) * listContent.Length );
 
            GUI.Box( listRect, "", boxStyle );
            int newSelectedItemIndex = GUI.SelectionGrid( listRect, selectedItemIndex, listContent, 1, listStyle );
            if( newSelectedItemIndex != selectedItemIndex )
                selectedItemIndex = newSelectedItemIndex;
        }
 
        if( done )
            isClickedComboButton = false;
 
        return selectedItemIndex;
    }
 
    public int SelectedItemIndex{
		get{
        	return selectedItemIndex;
		}
		set{
			selectedItemIndex = value;
		}
    }
}
I just wanted the button to automatically show what you have selected instead of a label 
C# Example Class /*
 * 
// Popup list created by Eric Haines
// ComboBox Extended by Hyungseok Seo.(Jerry) sdragoon@nate.com
// Refactored by zhujiangbo jumbozhu@gmail.com
// Slight edit for button to show the previously selected item AndyMartin458 www.clubconsortya.blogspot.com
// 
// -----------------------------------------------
// This code working like ComboBox Control.
// I just changed some part of code, 
// because I want to seperate ComboBox button and List.
// ( You can see the result of this code from Description's last picture )
// -----------------------------------------------
//
// === usage ======================================
using UnityEngine;
using System.Collections;
 
public class ComboBoxTest : MonoBehaviour
{
	GUIContent[] comboBoxList;
	private ComboBox comboBoxControl;// = new ComboBox();
	private GUIStyle listStyle = new GUIStyle();
 
	private void Start()
	{
		comboBoxList = new GUIContent[5];
		comboBoxList[0] = new GUIContent("Thing 1");
		comboBoxList[1] = new GUIContent("Thing 2");
		comboBoxList[2] = new GUIContent("Thing 3");
		comboBoxList[3] = new GUIContent("Thing 4");
		comboBoxList[4] = new GUIContent("Thing 5");
 
		listStyle.normal.textColor = Color.white; 
		listStyle.onHover.background =
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.left =
		listStyle.padding.right =
		listStyle.padding.top =
		listStyle.padding.bottom = 4;
 
		comboBoxControl = new ComboBox(new Rect(50, 100, 100, 20), comboBoxList[0], comboBoxList, "button", "box", listStyle);
	}
 
	private void OnGUI () 
	{
		comboBoxControl.Show();
	}
}
 
*/
 
 
using UnityEngine;
 
public class ComboBox
{
    private static bool forceToUnShow = false; 
    private static int useControlID = -1;
    private bool isClickedComboButton = false;
    private int selectedItemIndex = 0;
 
	private Rect rect;
	private GUIContent buttonContent;
	private GUIContent[] listContent;
	private string buttonStyle;
	private string boxStyle;
	private GUIStyle listStyle;
 
    public ComboBox( Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle ){
		this.rect = rect;
		this.buttonContent = buttonContent;
		this.listContent = listContent;
		this.buttonStyle = "button";
		this.boxStyle = "box";
		this.listStyle = listStyle;
    }
 
	public ComboBox(Rect rect, GUIContent buttonContent, GUIContent[] listContent, string buttonStyle, string boxStyle, GUIStyle listStyle){
		this.rect = rect;
		this.buttonContent = buttonContent;
		this.listContent = listContent;
		this.buttonStyle = buttonStyle;
		this.boxStyle = boxStyle;
		this.listStyle = listStyle;
	}
 
    public int Show()
    {
        if( forceToUnShow )
        {
            forceToUnShow = false;
            isClickedComboButton = false;
        }
 
        bool done = false;
        int controlID = GUIUtility.GetControlID( FocusType.Passive );       
 
        switch( Event.current.GetTypeForControl(controlID) )
        {
            case EventType.mouseUp:
            {
                if( isClickedComboButton )
                {
                    done = true;
                }
            }
            break;
        }       
 
        if( GUI.Button( rect, buttonContent, buttonStyle ) )
        {
            if( useControlID == -1 )
            {
                useControlID = controlID;
                isClickedComboButton = false;
            }
 
            if( useControlID != controlID )
            {
                forceToUnShow = true;
                useControlID = controlID;
            }
            isClickedComboButton = true;
        }
 
        if( isClickedComboButton )
        {
            Rect listRect = new Rect( rect.x, rect.y + listStyle.CalcHeight(listContent[0], 1.0f),
                      rect.width, listStyle.CalcHeight(listContent[0], 1.0f) * listContent.Length );
 
            GUI.Box( listRect, "", boxStyle );
            int newSelectedItemIndex = GUI.SelectionGrid( listRect, selectedItemIndex, listContent, 1, listStyle );
            if( newSelectedItemIndex != selectedItemIndex )
            {
                selectedItemIndex = newSelectedItemIndex;
                buttonContent = listContent[selectedItemIndex];
            }
        }
 
        if( done )
            isClickedComboButton = false;
 
        return selectedItemIndex;
    }
 
    public int SelectedItemIndex{
		get{
        	return selectedItemIndex;
		}
		set{
			selectedItemIndex = value;
		}
    }
}
}
