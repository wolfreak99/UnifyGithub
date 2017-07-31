// Original url: http://wiki.unity3d.com/index.php/JoystickButtonMenu
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/JoystickButtonMenu.cs
// File based on original modification date of: 21 July 2012, at 06:19. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Alex Hackl (AKA Silix) 
Contents [hide] 
1 Description 
2 Usage 
3 C# - JoystickButton.cs 
4 C# - JoystickButtonMenu.cs 

Description Unity GUI does not fully support joystick navigation which sucks 
so here is two custom classes i wrote for just that reason the JoystickButton and JoystickButtonMenu 
working together they provide functionality to create buttons that can be navigated by either horizontal or vertical movement and joystick button presses 
please note: I am a self taught programmer so this is relatively crude and i have only tested it's functionality against my specific needs. 
Usage First Declare the menus you want 
private JoystickButtonMenu mainMenu;then under Start initialize the menu: 
mainMenu = new JoystickButtonMenu(int numberOfButtons,Rect[] rectangleArray,string[] stringArray,string inputJoyButton,JoyAxis joystickInputAxis);the numberOfbuttons is self explanatory but be sure to use that same number as the size of both your rectangle and string array. 
the rectangleArray is an array of rectangles that define the position and size of each button, be sure to keep the size the same as the number of buttons. 
the stringArray is an array of labels that correspond to the rectangles and will show up as button labels. 
the joystickInputAxis is of type JoyAxis which is an enum in the JoystickButtonMenu, syntax goes as such JoystickButtonMenu.JoyAxis.Horizontal 

to display the buttons: 
void OnGUI(){
		mainMenu.DisplayButtons();
	}to detect input: 
void Update(){
		mainMenu.CheckJoystickAxis();
		mainMenu.CheckJoystickButton();
	}API: 
JoystickButtonMenu.DisplayButtons(); //displays the buttons in OnGUI(), returns void
 
JoystickButtonMenu.CheckJoystickAxis(); //execute each update to check whether the joystick has been moved, returns a bool
 
JoystickButtonMenu.CheckJoystickButton(); //execute each update to check whether the joystick button defined during initialization is pressed, returns an int corresponding to numberOfButtons(-1 = no button pressed)(0 = first button pressed, 1 = second button pressed, etc...)Here is an example which is the main menu for my current game: 
using UnityEngine;
using System.Collections;
 
public class MainMenu : MonoBehaviour {
 
 
	public GUISkin mySkin;
	public float delayBetweenFocusChanges = .5f;
 
	private Rect[] myRects = new Rect[3];
	private string[] mainMenuLabels = new string[3];
	private string[] optionMenuLabels = new string[3];
	private JoystickButtonMenu mainMenu,optionMenu;
 
	private int currentlyPressedButton = -1;
 
	void Start(){
		myRects[0] = new Rect(Screen.width/2 -30,Screen.height/2 -40,60,30);
		myRects[1] = new Rect(Screen.width/2 -30,Screen.height/2,60,30);
		myRects[2] = new Rect(Screen.width/2 -30,Screen.height/2 +40,60,30);
 
		mainMenuLabels[0] = "Play";
		mainMenuLabels[1] = "Options";
		mainMenuLabels[2] = "Exit";
 
		optionMenuLabels[0] = "Go";
		optionMenuLabels[1] = "Fuck";
		optionMenuLabels[2] = "Yourself";
 
		mainMenu = new JoystickButtonMenu(3,myRects,mainMenuLabels,"Fire1",JoystickButtonMenu.JoyAxis.Vertical);
		optionMenu = new JoystickButtonMenu(3,myRects,optionMenuLabels,"Fire1",JoystickButtonMenu.JoyAxis.Vertical);
 
		optionMenu.enabled = false;
	}
 
	void OnGUI(){
		GUI.skin = mySkin;
 
		mainMenu.DisplayButtons();
		optionMenu.DisplayButtons();
	}
 
	void Update(){
		if(mainMenu.enabled){
			if(mainMenu.CheckJoystickAxis()){
				Invoke("Delay",delayBetweenFocusChanges);
			}
			currentlyPressedButton = mainMenu.CheckJoystickButton();
 
			switch(currentlyPressedButton){
			case 0:
				Application.LoadLevel(Application.loadedLevel+1);
				return;
			case 1:
				optionMenu.enabled = true;
				mainMenu.enabled = false;
				return;
			case 2:
				Application.Quit();
				return;
		}
		}
		if(optionMenu.enabled){
			if(optionMenu.CheckJoystickAxis()){
				Invoke("Delay",delayBetweenFocusChanges);
			}
			currentlyPressedButton = optionMenu.CheckJoystickButton();
 
			switch(currentlyPressedButton){
			case 0:
				mainMenu.enabled = true;
				optionMenu.enabled = false;
				return;
			case 1:
				mainMenu.enabled = true;
				optionMenu.enabled = false;
				return;
			case 2:
				mainMenu.enabled = true;
				optionMenu.enabled = false;
				return;
			}
		}
	}
 
	private void Delay(){
		mainMenu.isCheckingJoy = false;
		optionMenu.isCheckingJoy = false;
	}	
}

C# - JoystickButton.cs using UnityEngine;
 
public class JoystickButton{
	public Texture Up, Over, Down;
	public string text;
	public Rect buttonRect;
	public bool isPressed, isFocused, enabled;
 
	public JoystickButton(Rect rect,string label){
		enabled = true;
 
		text = label;
 
		buttonRect = rect;
 
		isPressed = false;
		isFocused = false;
	}
 
	public void Display(){
		if(enabled){
			Up = (Texture)GUI.skin.button.normal.background;
			Over = (Texture)GUI.skin.button.hover.background;
			Down = (Texture)GUI.skin.button.active.background;
 
			if(!isFocused && !isPressed){
				GUI.DrawTexture(buttonRect,Up);
				GUI.skin.label.normal.textColor = GUI.skin.button.normal.textColor;
				GUI.Label(buttonRect,text);
			}else if(isFocused && !isPressed){
				GUI.DrawTexture(buttonRect,Over);
				GUI.skin.label.normal.textColor = GUI.skin.button.hover.textColor;
				GUI.Label(buttonRect,text);
			}else if(isFocused && isPressed){
				GUI.DrawTexture(buttonRect,Down);
				GUI.skin.label.normal.textColor = GUI.skin.button.active.textColor;
				GUI.Label(buttonRect,text);
			}
		}
	}
 
	public void Focus(bool fo){
		isFocused = fo;
	}
	public void Focus(){
		isFocused = true;
	}
 
	public bool Click(){
		if(isFocused){
			isPressed = true;
			return true;
		}
		return false;
	}
 
	public void UnClick(){
		if(isPressed){
			isPressed = false;
		}
	}
}C# - JoystickButtonMenu.cs using UnityEngine;
 
public class JoystickButtonMenu{
	public enum JoyAxis{
		Horizontal = 0,
		Vertical = 1
	}
 
	public JoyAxis JoystickAxis = JoyAxis.Vertical;
 
	public string joystickInputNamePrefix = "", joystickInputName = "";
	public bool enabled = true;
 
	private int numberOfButtons;
	private JoystickButton[] buttons;
	public bool isCheckingJoy;
	public int currentFocus;
	private string actionButton;
 
 
	public JoystickButtonMenu(int numOfButtons,Rect[] rectangles,string[] labels, string inputActionButton,JoyAxis axis){
		if(axis == JoyAxis.Horizontal){
			joystickInputName = "Horizontal";
		}else if(axis == JoyAxis.Vertical){
			joystickInputName = "Vertical";
		}
 
		numberOfButtons = numOfButtons;
		actionButton = inputActionButton;
 
		buttons = new JoystickButton[numOfButtons];
		for (int i = 0; i<numOfButtons; i++){
			buttons[i] = new JoystickButton(rectangles[i],labels[i]);
		}
 
		buttons[0].Focus();
		currentFocus = 0;
	}
 
	public bool CheckJoystickAxis(){
		if(Mathf.Abs(Input.GetAxis(joystickInputNamePrefix+joystickInputName)) == 1 && !isCheckingJoy && enabled){
			if(Input.GetAxis(joystickInputNamePrefix+joystickInputName) > .1f){
				SetFocus(1);
			}
			if(Input.GetAxis(joystickInputNamePrefix+joystickInputName) < -.1f){
				SetFocus(-1);
			}
			isCheckingJoy = true;
			return true;
		}
		return false;
	}
 
	public int CheckJoystickButton(){
		int pressedButton = -1;
		if(enabled){
			if(Input.GetButtonDown(actionButton)){
				for(int i = 0; i<numberOfButtons; i++){
					if(buttons[i].Click()){
						pressedButton = i;
					}
				}
			}
			if(Input.GetButtonUp(actionButton)){
				foreach (JoystickButton butt in buttons){
					butt.UnClick();
				}
			}
		}
		return pressedButton;
	}
 
	public void SetFocus(int change){
		if(enabled){
			if(change == -1){
				currentFocus ++;
				if(currentFocus == numberOfButtons){
					currentFocus = 0;
				}
			}else if(change == 1){
				currentFocus --;
				if(currentFocus == -1){
					currentFocus = numberOfButtons-1;
				}
			}
 
			for(int i = 0;i<numberOfButtons;i++){
				buttons[i].Focus(false);
				if(currentFocus == i){
					buttons[i].Focus(true);
				}
			}
		}
	}
 
	public void DisplayButtons(){
		if(enabled){
			foreach(JoystickButton butt in buttons){
				butt.Display();
			}
		}
	}
}
}
