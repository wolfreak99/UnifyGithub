// Original url: http://wiki.unity3d.com/index.php/GUITextField
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/GUITextField.cs
// File based on original modification date of: 20 January 2013, at 00:26. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Contents [hide] 
1 GUITextField 
1.1 Description 
1.2 Usage 
1.3 GUITextField.js 

GUITextField Description The Unity provided GUI.TextField and GUILayout.TextField are of very limited use for editing strings that need to be sent over a network (e.g. player names). The new text is returned after every call to the TextField function, and even testing whether it's changed or not before sending it means that a new string will be sent over the network with each character change. 
In addition, where an authoritative server is used to check the name and confirm that the name has been changed, this effectively means the client is typing "over the network" with all the latency issues that causes (and hence problems with a confirmed name arriving after new characters have been typed and scrambling the intended characters). 
The following script therefore keeps track of when the text field is being edited, and returns true from the display call only when new text is commited (i.e. the text field is changed, then unfocused by a mouse click elsewhere or by pressing return / enter). It also returns false, and restores the unedited text if the user presses escape. 
Unfortunately, provision for focusing of controls is a bit sketchy in Unity (it's possible to focus a specific control, but not to unfocus one), so a "fake" control is provided, and GUITextField.FocusHack() must be called before TextField.Display in an OnGUI function. In addition, a text field must be identified with a unique control name. 
Usage // Usage example:
 
// "Test" is a control name unique to this control used for focusing / unfocusing.
// "testing text" is the initial text field display text.
var testField : TextField = new TextField("Test", "testing text");
 
function OnGUI() {
	// This is a static function that must be called once in the OnGUI function before any text fields are displayed.
	GUITextField.FocusHack();
 
	// Display the text field.
	// Further overloads are provided to correspond with the GUI.TextField and GUILayout.TextField functions.
	if (testField.Display()) {
		Debug.Log("text change committed: " + testField.GetText());
	}
}GUITextField.js static var hackControlName : String = "hackery129578835432342";
 
// call this function in OnGUI before calling TextField (only needs to be called once)
static function FocusHack() {
	// fake control used for unfocussing things, since we can't call
	// FocusControl with no arguments and an empty string doesn't work
	GUI.SetNextControlName(hackControlName);
	GUI.Button(Rect(-10000,-10000, 0,0), GUIContent.none);
}
 
class TextField {
 
	var name : String;
 
	private var text : String;
	private var lastCommittedText : String;
	private var focused : boolean;
 
	function TextField(name : String, text : String) {
		this.name = name;
		this.text = text;
		lastCommittedText = text;
		focused = false;
	}
 
	function GetText() : String {
		return text;
	}
	function SetText(text : String) {
		this.text = text;
		lastCommittedText = text;
	}
 
	// GUI function overloads
	function Display(position : Rect) : boolean {
		GUI.SetNextControlName(name);
		text = GUI.TextField(position, text);
 
		return CheckFocus();
	}
	function Display(position : Rect, style : GUIStyle) : boolean {
		GUI.SetNextControlName(name);
		text = GUI.TextField(position, text, style);
 
		return CheckFocus();
	}
	function Display(position : Rect, style : String) : boolean {
		GUI.SetNextControlName(name);
		text = GUI.TextField(position, text, style);
 
		return CheckFocus();
	}
 
	function Display(position : Rect, maxLength : int) : boolean {
		GUI.SetNextControlName(name);
		text = GUI.TextField(position, text, maxLength);
 
		return CheckFocus();
	}
	function Display(position : Rect, maxLength : int, style : GUIStyle) : boolean {
		GUI.SetNextControlName(name);
		text = GUI.TextField(position, text, maxLength, style);
 
		return CheckFocus();
	}
	function Display(position : Rect, maxLength : int, style : String) : boolean {
		GUI.SetNextControlName(name);
		text = GUI.TextField(position, text, maxLength, style);
 
		return CheckFocus();
	}
 
	// GUILayout function overloads
	function Display() : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text);
 
		return CheckFocus();
	}
	function Display(options : GUILayoutOption[]) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, options);
 
		return CheckFocus();
	}
	function Display(style : GUIStyle) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, style);
 
		return CheckFocus();
	}
	function Display(style : GUIStyle, options : GUILayoutOption[]) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, style, options);
 
		return CheckFocus();
	}
	function Display(style : String) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, style);
 
		return CheckFocus();
	}
	function Display(style : String, options : GUILayoutOption[]) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, style, options);
 
		return CheckFocus();
	}
 
	function Display(maxLength : int) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, maxLength);
 
		return CheckFocus();
	}
	function Display(maxLength : int, options : GUILayoutOption[]) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, maxLength, options);
 
		return CheckFocus();
	}
	function Display(maxLength : int, style : GUIStyle) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, maxLength, style);
 
		return CheckFocus();
	}
	function Display(maxLength : int, style : GUIStyle, options : GUILayoutOption[]) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, maxLength, style, options);
 
		return CheckFocus();
	}
	function Display(maxLength : int, style : String) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, maxLength, style);
 
		return CheckFocus();
	}
	function Display(maxLength : int, style : String, options : GUILayoutOption[]) : boolean {
		GUI.SetNextControlName(name);
		text = GUILayout.TextField(text, maxLength, style, options);
 
		return CheckFocus();
	}
 
	private function CheckFocus() : boolean {
		if (GUI.GetNameOfFocusedControl() == name) {
			if (!focused) {
				// name field was focused
				focused = true;
			}
		}
		else {
			if (focused) {
				// name field was unfocused
				focused = false;
 
				if (text != lastCommittedText) {
					lastCommittedText = text;
					return true;
				}
				return false;
			}
		}
 
		if (Event.current.isKey) {
			if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
				// unfocus the textfield (focus fake control)
				if (GUI.GetNameOfFocusedControl() == name) {
					GUI.FocusControl(GUITextField.hackControlName);
				}
			}
			else if (Event.current.keyCode == KeyCode.Escape) {
				// unfocus textfield (focus fake control) and change text back to last committed version
				if (GUI.GetNameOfFocusedControl() == name) {
					GUI.FocusControl(GUITextField.hackControlName);
					text = lastCommittedText;
				}
			}
		}
 
		return false;
	}
}
}
