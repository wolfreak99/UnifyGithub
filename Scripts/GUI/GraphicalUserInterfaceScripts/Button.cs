// Original url: http://wiki.unity3d.com/index.php/Button
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/Button.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Jonathan Czeck (aarku) 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - Button.js 
4 C# - Button.cs 
5 C# - Button.cs (iPhone Compatible) 

DescriptionThis script uses a GUITexture and Unity mouse events to implement a regular push button that behaves properly like Mac OS X. 
Warning: As of Unity 3.0.0, this script does not work on iOS devices, as OnMouseUp functions and the like do not work on iOS devices. Use the iPhone compatible version further below. 
UsageAttach this script to a GUITexture object. Add a ButtonPressed function to the object pointed to by the messagee variable to catch when the button has been pressed. (You can change the name of the function by changing the message variable.) 
JavaScript - Button.js var normalTexture : Texture;
 var hoverTexture : Texture;
 var pressedTexture : Texture;
 var messagee : GameObject;
 var message = "ButtonPressed";
 
 private var state = 0;
 private var myGUITexture : GUITexture;
 
 myGUITexture = GetComponent(GUITexture); 
 
 function OnMouseEnter()
 {
    state++;
    if (state == 1)
        myGUITexture.texture = hoverTexture;
 }
 
 function OnMouseDown()
 {
    state++;
    if (state == 2)
        myGUITexture.texture = pressedTexture;
 }
 
 function OnMouseUp()
 {
    if (state == 2)
    {
        state--;
        if (messagee)
            messagee.SendMessage(message, gameObject);
    }
    else
    {
        state --;
        if (state < 0)
            state = 0;
    }
    myGUITexture.texture = normalTexture;
 }
 
 function OnMouseExit()
 {
    if (state > 0)
        state--;
    if (state == 0)
        myGUITexture.texture = normalTexture;
 }C# - Button.csThe C# version is a bit different from the JS version. It shows off some new features in Unity 1.2 for automatically attaching the GUITexture component when attaching the Button (using the RequireComponent attribute) and placing the Button in a custom submenu in the Components menu (using the AddComponentMenu attribute.) 
It also groups the textures together into a separate object and has a separate method for changing the texture to ease customisation. Note that some properties and methods are marked protected and/or virtual to make it possible to customise the Button class by creating a new subclass. See the ToggleButton snippet for an example on how to subclass the Button class. 
using UnityEngine;
using System.Collections;
 
public enum ButtonState {
    normal,
    hover,
    armed
}
 
[System.Serializable] // Required so it shows up in the inspector 
public class ButtonTextures {
    public Texture normal=null;
    public Texture hover=null;
    public Texture armed=null;
    public ButtonTextures() {}
 
    public Texture this [ButtonState state] {
        get {
            switch(state) {
                case ButtonState.normal:
                    return normal;
                case ButtonState.hover:
                    return hover;
                case ButtonState.armed:
                    return armed;
                default:
                    return null;
            }
        }
    }
}
 
 
[RequireComponent(typeof(GUITexture))]
[AddComponentMenu ("GUI/Button")]    
public class Button : MonoBehaviour {
 
    public GameObject messagee;
    public string message = "ButtonPressed";
    public ButtonTextures textures;
 
    protected int state = 0;
    protected GUITexture myGUITexture;
 
    protected virtual void SetButtonTexture(ButtonState state) {
        myGUITexture.texture=textures[state];
    }
 
    public virtual void Reset() {
        messagee = gameObject;
        message = "ButtonPressed";
    }
 
    public virtual void Start() {
        myGUITexture = GetComponent(typeof(GUITexture)) as GUITexture; 
        SetButtonTexture(ButtonState.normal);
    }
 
    public virtual void OnMouseEnter()
    {
        state++;
        if (state == 1)
            SetButtonTexture(ButtonState.hover);
    }
 
    public virtual void OnMouseDown()
    {
        state++;
        if (state == 2)
            SetButtonTexture(ButtonState.armed);
    }
 
    public virtual void OnMouseUp()
    {
        if (state == 2)
        {
            state--;
            if (messagee != null)
                messagee.SendMessage(message, this);
        }
        else
        {
            state --;
            if (state < 0)
                state = 0;
        }
        SetButtonTexture(ButtonState.normal);
    }
 
    public virtual void OnMouseExit()
    {
        if (state > 0)
            state--;
        if (state == 0)
            SetButtonTexture(ButtonState.normal);
    }
}

C# - Button.cs (iPhone Compatible)Same as the C# version above, but added support for recognizing touch input from iPhone or Android devices. Also adds callback function for when the button is double-clicked/double-tapped. 
using UnityEngine;
using System.Collections;
 
public enum ButtonState
{
    normal,
    hover,
    armed
}
 
[System.Serializable] // Required so it shows up in the inspector 
public class ButtonTextures
{
    public Texture normal=null;
    public Texture hover=null;
    public Texture armed=null;
    public ButtonTextures() {}
 
    public Texture this [ButtonState state]
	{
        get
		{
            switch(state)
			{
                case ButtonState.normal:
                    return normal;
                case ButtonState.hover:
                    return hover;
                case ButtonState.armed:
                    return armed;
                default:
                    return null;
            }
        }
    }
}
 
 
[RequireComponent(typeof(GUITexture))]
[AddComponentMenu ("GUI/Button")]    
public class GuiButton : MonoBehaviour
{
    public GameObject messagee;
    public string message = "";
	public string messageDoubleClick = "";
    public ButtonTextures textures;
 
    protected int state = 0;
    protected GUITexture myGUITexture;
 
	private int clickCount = 1;
	private float lastClickTime = 0.0f;
	static private float doubleClickSensitivity = 0.5f;
 
    protected virtual void SetButtonTexture(ButtonState state)
	{
		if (textures[state] != null)
		{
        	myGUITexture.texture = textures[state];
		}
    }
 
    public virtual void Reset()
	{
        messagee = gameObject;
        message = "";
		messageDoubleClick = "";
    }
 
	public bool HitTest(Vector2 pos)
	{
		return myGUITexture.HitTest(new Vector3(pos.x, pos.y, 0));
	}
 
    public virtual void Start()
	{
        myGUITexture = GetComponent(typeof(GUITexture)) as GUITexture; 
        SetButtonTexture(ButtonState.normal);
    }
 
    public virtual void OnMouseEnter()
    {
        state++;
        if (state == 1)
            SetButtonTexture(ButtonState.hover);
    }
 
    public virtual void OnMouseDown()
    {
        state++;
        if (state == 2)
            SetButtonTexture(ButtonState.armed);
    }
 
    public virtual void OnMouseUp()
    {
		if (Time.time - lastClickTime <= doubleClickSensitivity)
		{
			++clickCount;
		}
		else
		{
			clickCount = 1;
		}
 
        if (state == 2)
        {
            state--;
			if (clickCount == 1)
			{
				if (messagee != null && message != "")
				{
					messagee.SendMessage(message, this);
				}
			}
			else
			{
				if (messagee != null && messageDoubleClick != "")
				{
					messagee.SendMessage(messageDoubleClick, this);
				}
			}
        }
        else
        {
            state --;
            if (state < 0)
                state = 0;
        }
        SetButtonTexture(ButtonState.normal);
		lastClickTime = Time.time;
    }
 
    public virtual void OnMouseExit()
    {
        if (state > 0)
            state--;
        if (state == 0)
            SetButtonTexture(ButtonState.normal);
    }
 
#if (UNITY_IPHONE || UNITY_ANDROID)
	void Update()
	{
		int count = Input.touchCount;
		for (int i = 0; i < count; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (HitTest(touch.position))
			{
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					SetButtonTexture(ButtonState.normal);
				}
				else
				{
					SetButtonTexture(ButtonState.armed);
				}
				if (touch.phase == TouchPhase.Began)
				{
					if (touch.tapCount == 1)
					{
						if (messagee != null && message != "")
						{
							messagee.SendMessage(message, this);
						}
					}
					else if (touch.tapCount == 2)
					{
						if (messagee != null && messageDoubleClick != "")
						{
							messagee.SendMessage(messageDoubleClick, this);
						}
					}
				}
				break;
			}
		}
	}
#endif
}
}
