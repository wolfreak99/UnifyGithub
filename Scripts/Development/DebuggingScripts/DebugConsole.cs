// Original url: http://wiki.unity3d.com/index.php/DebugConsole
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Development/DebuggingScripts/DebugConsole.cs
// File based on original modification date of: 1 December 2015, at 03:38. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{
Author: Jeremy Hollingsworth (jeremyace) 
Modifications by: 
Simon Waite (opless) 22 Feb 2006 
Shinsuke Sugita (shinriyo) Dec 2015 
Description This script will create a scrolling debug console. Allows you to set one of three colors (normal, warning, error) to each line to get more feedback and is mouse draggable at runtime. 
Usage Drop the DebugConsole script into your Standard Assets folder (This is a requirement). It is now fully usable. 
If you want to change the default settings (like colors, or change the GUIText object), then create an empty GameObject and drag this script onto it from the standard assets folder. You now have access to all of the default settings in the inspector. The settings are explained below. 

Settings: 
Debug GUI : Drag the guiText you created here if you wish to override the default. 
defaultGuiPosition : A vector for the starting position of the first line of output. If you provide your own GUIText then it's position overrides this property. 
defaultGuiScale : A vector for the scale of the individual lines. Again, this is overridden if you provide your own GUIText. 
The three colors : are for normal, warning, and error. Set to whatever you like or leave alone. 
Max messages : How many message lines we will put on the screen at once. 
Line Spacing : A float for how far down we will move each new line. 
Draggable : True means you can drag the output around with the mouse at runtime, false means position is locked from mouse input. To drag output at runtime, click once on the uppermost output line to connect it to the mouse, then move to where you want the output displayed and click the mouse again to release it. 
Visible : The default setting for visibility of the debug output. If set to false (unchecked), you can send data but it wont show until you tell it to, if set to true (checked) it will show all debug output until you tell it not to. 
PixelCorrect : If set to true and LineSpacing to be on-screen pixels or not, very much like the GUIText property of the same name. Default: false LineSpacing will be in screen coordinates (0.0 - 1.0) 

Using the DebugConsole script from your scripts: 
To call the DebugConsole script from your scripts, just type: 

DebugConsole.someFunction() 

Replace someFunction() with any of the following: 

Log (string message, string color) 
Adds a message to the list. The color is a string either "normal", "warning" or "error". The color argument is optional and if omitted, the color will default to "normal". 

Clear() 
Clears all of the messages from the list and the screen. 

To toggle the visibility use: 
isVisible (true, false) 
The property to set the visiblility of the debug output. This property _not_ clear the message list, just hides it. You need to use the Clear() method to clear the list. 

To toggle the mouse drag functionality use: 
isDraggable (true, false) 

So for a simple example, to add hello world in warning color (yellow), we type: DebugConsole.Log("hello world", "warning"); 

To hide the output we type: DebugConsole.isVisible = false; 
C# - DebugConsole.cs The script _must_ be named DebugConsole.cs 
/*==== DebugConsole.cs ====================================================
 * Class for handling multi-line, multi-color debugging messages.
 * Original Author: Jeremy Hollingsworth
 * Based On: Version 1.2.1 Mar 02, 2006
 * 
 * Modified: Simon Waite
 * Date: 22 Feb 2007
 *
 * Modified: Shinsuke Sugita
 * Date: 1 Dec 2015
 * 
 * Modification to original script to allow pixel-correct line spacing
 *
 * Setting the boolean pixelCorrect changes the units in lineSpacing property
 * to pixels, so you have a pixel correct gui font in your console.
 *
 * It also checks every frame if the screen is resized to make sure the 
 * line spacing is correct (To see this; drag and let go in the editor 
 * and the text spacing will snap back)
 *
 * USAGE:
 * ::Drop in your standard assets folder (if you want to change any of the
 * default settings in the inspector, create an empty GameObject and attach
 * this script to it from you standard assets folder.  That will provide
 * access to the default settings in the inspector)
 * 
 * ::To use, call DebugConsole.functionOrProperty() where 
 * functionOrProperty = one of the following:
 * 
 * -Log(string message, string color)  Adds "message" to the list with the
 * "color" color. Color is optional and can be any of the following: "error",
 * "warning", or "normal".  Default is normal.
 * 
 * Clear() Clears all messages
 * 
 * isVisible (true,false)  Toggles the visibility of the output.  Does _not_
 * clear the messages.
 * 
 * isDraggable (true, false)  Toggles mouse drag functionality
 * =========================================================================*/
 
 
using UnityEngine;
using System.Collections;
 
 
public class DebugConsole : MonoBehaviour
{
	public GameObject DebugGui = null;             // The GUI that will be duplicated
	public Vector3 defaultGuiPosition = new Vector3(0.01F, 0.98F, 0F);
	public Vector3 defaultGuiScale = new Vector3(0.5F, 0.5F, 1F);
	public Color normal = Color.green;
	public Color warning = Color.yellow;
	public Color error = Color.red;
	public int maxMessages = 30;                   // The max number of messages displayed
	public float lineSpacing = 0.02F;              // The amount of space between lines
	public ArrayList messages = new ArrayList();
	public ArrayList guis = new ArrayList();
	public ArrayList colors = new ArrayList();
	public bool draggable = true;                  // Can the output be dragged around at runtime by default? 
	public bool visible = true;                    // Does output show on screen by default or do we have to enable it with code? 
	public bool pixelCorrect = false; // set to be pixel Correct linespacing
	public static bool isVisible
	{                                      
		get
		{
			return DebugConsole.instance.visible;
		}
 
		set
		{
			DebugConsole.instance.visible = value;
			if (value == true)
			{
				DebugConsole.instance.Display();
			}
			else if (value == false)
			{
				DebugConsole.instance.ClearScreen();
			}
		}
	}
 
	public static bool isDraggable
	{                                      
		get
		{
			return DebugConsole.instance.draggable;
		}
 
		set
		{
			DebugConsole.instance.draggable = value;
 
		}
	}
 
 
	private static DebugConsole s_Instance = null;   // Our instance to allow this script to be called without a direct connection.
	public static DebugConsole instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
				if (s_Instance == null)
				{
					GameObject console = new GameObject();
					console.AddComponent<DebugConsole>();
					console.name = "DebugConsoleController";
					s_Instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
					DebugConsole.instance.InitGuis();
				}
 
			}
 
			return s_Instance;
		}
	}
 
	void Awake()
	{
		s_Instance = this;
		InitGuis();
 
	}
 
	protected bool guisCreated = false;
	protected float screenHeight =-1;
	public void InitGuis()
	{
		float usedLineSpacing = lineSpacing;
		screenHeight = Screen.height;
		if(pixelCorrect)
			usedLineSpacing = 1.0F / screenHeight * usedLineSpacing;  
 
		if (guisCreated == false)
		{
			if (DebugGui == null)  // If an external GUIText is not set, provide the default GUIText
			{
				DebugGui = new GameObject();
				DebugGui.AddComponent<GUIText>();
				DebugGui.name = "DebugGUI(0)";
				DebugGui.transform.position = defaultGuiPosition;
				DebugGui.transform.localScale = defaultGuiScale;
			}
 
			// Create our GUI objects to our maxMessages count
			Vector3 position = DebugGui.transform.position;
			guis.Add(DebugGui);
			int x = 1;
 
			while (x < maxMessages)
			{
				position.y -= usedLineSpacing;
				GameObject clone = null;
				clone = (GameObject)Instantiate(DebugGui, position, transform.rotation);
				clone.name = string.Format("DebugGUI({0})", x);
				guis.Add(clone);
				position = clone.transform.position;
				x += 1;
			}
 
			x = 0;
			while (x < guis.Count)
			{
				GameObject temp = (GameObject)guis[x];
				temp.transform.parent = DebugGui.transform;
				x++;
			}
			guisCreated = true;
		} else {
			// we're called on a screensize change, so fiddle with sizes
			Vector3 position = DebugGui.transform.position;
			for(int x=0;x < guis.Count; x++)
			{
				position.y -= usedLineSpacing;
				GameObject temp = (GameObject)guis[x];
				temp.transform.position= position;
			}    	
		}
	}
 
 
 
	bool connectedToMouse = false;  
	void Update()
	{
		// If we are visible and the screenHeight has changed, reset linespacing
		if (visible == true && screenHeight != Screen.height)
		{
			InitGuis();
		}
		if (draggable == true)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (connectedToMouse == false && DebugGui.GetComponent<GUIText>().HitTest((Vector3)Input.mousePosition) == true)
				{
					connectedToMouse = true;
				}
				else if (connectedToMouse == true)
				{
					connectedToMouse = false;
				}
 
			}
 
			if (connectedToMouse == true)
			{
				float posX = DebugGui.transform.position.x;
				float posY = DebugGui.transform.position.y;
				posX = Input.mousePosition.x / Screen.width;
				posY = Input.mousePosition.y / Screen.height;
				DebugGui.transform.position = new Vector3(posX, posY, 0F);
			}
		}
 
	}
	//+++++++++ INTERFACE FUNCTIONS ++++++++++++++++++++++++++++++++
	public static void Log(string message, string color)
	{
		DebugConsole.instance.AddMessage(message, color);
 
	}
	//++++ OVERLOAD ++++
	public static void Log(string message)
	{
		DebugConsole.instance.AddMessage(message);
	}
 
	public static void Clear()
	{
		DebugConsole.instance.ClearMessages();
	}
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
 
 
	//---------- void AddMesage(string message, string color) ------
	//Adds a mesage to the list
	//--------------------------------------------------------------
 
	public void AddMessage(string message, string color)
	{
		messages.Add(message);
		colors.Add(color);
		Display();
	}
	//++++++++++ OVERLOAD for AddMessage ++++++++++++++++++++++++++++
	// Overloads AddMessage to only require one argument(message)
	//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	public void AddMessage(string message)
	{
		messages.Add(message);
		colors.Add("normal");
		Display();
	}
 
 
	//----------- void ClearMessages() ------------------------------
	// Clears the messages from the screen and the lists
	//---------------------------------------------------------------
	public void ClearMessages()
	{
		messages.Clear();
		colors.Clear();
		ClearScreen();
	}
 
 
	//-------- void ClearScreen() ----------------------------------
	// Clears all output from all GUI objects
	//--------------------------------------------------------------
	void ClearScreen()
	{
		if (guis.Count < maxMessages)
		{
			//do nothing as we haven't created our guis yet
		}
		else
		{
			int x = 0;
			while (x < guis.Count)
			{
				GameObject gui = (GameObject)guis[x];   
				gui.GetComponent<GUIText>().text = "";
				//increment and loop
				x += 1;
			}
		}
	}   
 
 
	//---------- void Prune() ---------------------------------------
	// Prunes the array to fit within the maxMessages limit
	//---------------------------------------------------------------
	void Prune()
	{
		int diff;
		if (messages.Count > maxMessages)
		{
			if (messages.Count <= 0)
			{
				diff = 0;
			}
			else
			{
				diff = messages.Count - maxMessages;
			}
			messages.RemoveRange(0, (int)diff);
			colors.RemoveRange(0, (int)diff);
		}
 
	}
 
	//---------- void Display() -------------------------------------
	// Displays the list and handles coloring
	//---------------------------------------------------------------
	void Display()
	{
		//check if we are set to display
		if (visible == false)
		{
			ClearScreen();
		}
		else if (visible == true)
		{
 
 
			if (messages.Count > maxMessages)
			{
				Prune();
			}
 
			// Carry on with display
			int x = 0;
			if (guis.Count < maxMessages)
			{
				//do nothing as we havent created our guis yet
			}
			else
			{
				while (x < messages.Count)
				{
					GameObject gui = (GameObject)guis[x];   
 
					//set our color
					switch ((string)colors[x])
					{
					case "normal": gui.GetComponent<GUIText>().material.color = normal;
						break;
					case "warning": gui.GetComponent<GUIText>().material.color = warning;
						break;
					case "error": gui.GetComponent<GUIText>().material.color = error;
						break;
					}
 
					//now set the text for this element
					gui.GetComponent<GUIText>().text = (string)messages[x];
 
					//increment and loop
					x += 1;
				}
			}
 
		}
	}
 
 
}// End DebugConsole Class
}
