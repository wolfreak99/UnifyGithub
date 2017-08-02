/*************************
 * Original url: http://wiki.unity3d.com/index.php/ImprovedSelectionList
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/ImprovedSelectionList.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    By Daniel Brauer, inspired by AngryAnt 
    Contents [hide] 
    1 Fix: Double click fix 
    2 Description 
    3 Component code 
    4 Usage 
    
    Fix: Double click fix By Pablo Bollansée (The Oddler) Using Unity 3.4.0f5 
    I noticed a small bug while using this, namely that the double clicking did not work. I was able to fix this by changing a few things: 
    if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1) // added " && Event.current.clickCount == 1"
    {
    	selected = i;
    	Event.current.Use();
    }
    else if (hover && callback != null && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2) //Changed from MouseUp to MouseDown
    {
    	Debug.Log("Works !");
    	callback(i);
    	Event.current.Use();
    }
    //Do the same for the SelectionList function that uses a String list instead of the GUIContent list ;)I tested and for some reason when I check for MouseUp && clickCount == 2 then it never fires, yet it fires 2ce when I just check for clickCount == 2. It also fires once when I check for MouseDown && clickCount == 2. This might just be a little bug and might get fixed later, yet this fix works now. 
    I hope that this helps people who are having the same problem. 
    Description Visualises an array of strings or GUIContents and returns the index selected by the user. A delegate can be provided to capture double-clicking list items. Important: this script depends on your current GUISkin having a custom style called "List Item" with an appropriate OnNormal look. If you don't have this in your skin, you will have to use the custom style versions of the function. 
    Component code using UnityEngine;
     
    public class GUILayoutx {
     
    	public delegate void DoubleClickCallback(int index);
     
    	public static int SelectionList(int selected, GUIContent[] list) {
    		return SelectionList(selected, list, "List Item", null);
    	}
     
    	public static int SelectionList(int selected, GUIContent[] list, GUIStyle elementStyle) {
    		return SelectionList(selected, list, elementStyle, null);
    	}
     
    	public static int SelectionList(int selected, GUIContent[] list, DoubleClickCallback callback) {
    		return SelectionList(selected, list, "List Item", callback);
    	}
     
    	public static int SelectionList(int selected, GUIContent[] list, GUIStyle elementStyle, DoubleClickCallback callback) {
    		for (int i = 0; i < list.Length; ++i) {
    			Rect elementRect = GUILayoutUtility.GetRect(list[i], elementStyle);
    			bool hover = elementRect.Contains(Event.current.mousePosition);
    			if (hover && Event.current.type == EventType.MouseDown) {
    				selected = i;
    				Event.current.Use();
    			} else if (hover && callback != null && Event.current.type == EventType.MouseUp && Event.current.clickCount == 2) {
    				callback(i);
    				Event.current.Use();
    			} else if (Event.current.type == EventType.repaint) {
    				elementStyle.Draw(elementRect, list[i], hover, false, i == selected, false);
    			}
    		}
    		return selected;
    	}
     
    	public static int SelectionList(int selected, string[] list) {
    		return SelectionList(selected, list, "List Item", null);
    	}
     
    	public static int SelectionList(int selected, string[] list, GUIStyle elementStyle) {
    		return SelectionList(selected, list, elementStyle, null);
    	}
     
    	public static int SelectionList(int selected, string[] list, DoubleClickCallback callback) {
    		return SelectionList(selected, list, "List Item", callback);
    	}
     
    	public static int SelectionList(int selected, string[] list, GUIStyle elementStyle, DoubleClickCallback callback) {
    		for (int i = 0; i < list.Length; ++i) {
    			Rect elementRect = GUILayoutUtility.GetRect(new GUIContent(list[i]), elementStyle);
    			bool hover = elementRect.Contains(Event.current.mousePosition);
    			if (hover && Event.current.type == EventType.MouseDown) {
    				selected = i;
    				Event.current.Use();
    			} else if (hover && callback != null && Event.current.type == EventType.MouseUp && Event.current.clickCount == 2) {
    				callback(i);
    				Event.current.Use();
    			} else if (Event.current.type == EventType.repaint) {
    				elementStyle.Draw(elementRect, list[i], hover, false, i == selected, false);
    			}
    		}
    		return selected;
    	}
     
    }Usage Providing a double-click delegate: 
    using UnityEngine;
    using System.Collections;
     
    public class SelectionListTest : MonoBehaviour {
     
    	int currentSelection;
    	public GUISkin skinWithListStyle;
    	string[] myList = new string[] {"First", "Second", "Third"};
     
    	protected void OnGUI() {
    		GUI.skin = skinWithListStyle;
    		currentSelection = GUILayoutx.SelectionList(currentSelection, myList, DoubleClickItem);
    	}
     
    	protected void DoubleClickItem(int index) {
    		Debug.Log("Clicked " + myList[index]);
    	}
    }
}
