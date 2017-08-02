/*************************
 * Original url: http://wiki.unity3d.com/index.php/AutoType
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/AutoType.cs
 * File based on original modification date of: 11 March 2015, at 13:37. 
 *
 * Author: Daniel 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Javascript - AutoType.js 
    4 C# - AutoType.cs 
    5 JavaScript: Rich Text per character 
    
    Description Automatically types a string of text typewriter style. 
    Usage Place this script onto a GUIText. The text in the GUIText object's Text field will be used. 
    Javascript - AutoType.js var letterPause = 0.2;
    var sound : AudioClip;
     
    private var word;
     
    function Start () {
    	word = guiText.text;
    	guiText.text = "";
    	TypeText ();
    }
     
    function TypeText () {
    	for (var letter in word.ToCharArray()) {
    		guiText.text += letter;
    		if (sound)
    			audio.PlayOneShot (sound);
    		yield WaitForSeconds (letterPause);
    	}		
    }C# - AutoType.cs Conversion: paste120 
    using UnityEngine;
    using System.Collections;
     
    public class AutoType : MonoBehaviour {
     
    	public float letterPause = 0.2f;
    	public AudioClip sound;
     
    	string message;
     
    	// Use this for initialization
    	void Start () {
    		message = guiText.text;
    		guiText.text = "";
    		StartCoroutine(TypeText ());
    	}
     
    	IEnumerator TypeText () {
    		foreach (char letter in message.ToCharArray()) {
    			guiText.text += letter;
    			if (sound)
    				audio.PlayOneShot (sound);
    				yield return 0;
    			yield return new WaitForSeconds (letterPause);
    		}      
    	}
    }
    
    JavaScript: Rich Text per character This is a JS version of AutoType which allows for styling on a per character level. Simply wrap your words in a pair of symbols to achieve the styling effect. This requires that richText is set to true on your GUIText. For bold text, wrap your string with "¬"- eg: "This text is ¬bold¬. 
    var letterPause : float = 0.2;
    var word : String;
     
    function Start () {
    	word= guiText.text;
    	guiText.text = "";
    	TypeText(word);
    }
     
     
    function TypeText () {
     
    	var bold : boolean = false; //toggles the style for bold;
    	var red : boolean = false; // toggle red
    	var italics : boolean = false;
     
    	var ignore : boolean = false; //for ignoring special characters that toggle styles
     
    	for (var nextletter in word.ToCharArray()) {
     
    		switch (nextletter) {
     
    			case "@":
    				ignore = true; //make sure this character isn't printed by ignoring it
    				red = !red; //toggle red styling
    			break;
    			case "¬":
    				ignore = true; //make sure this character isn't printed by ignoring it
    				bold = !bold; //toggle bold styling
    			break;
    			case "/":
    				ignore = true; //make sure this character isn't printed by ignoring it
    				italics = !italics; //toggle italic styling
    			break;
    		}
     
     
    		var letter : String = nextletter.ToString();
     
    		if (!ignore) {
     
    			if (bold){
     
    				letter = "<b>"+letter+"</b>";
     
    			}
    			if (italics){
     
    				letter = "<i>"+letter+"</i>";
     
    			}
    			if (red){
     
    				letter = "<color=#ff0000>"+letter+"</color>";
     
    			}
     
    			guiText.text += letter; 
     
    		}
                    //make sure the next character isn't ignored
                    ignore = false;
    		yield WaitForSeconds (letterPause);
    	}
}--Fishman92 (talk) 14:37, 11 March 2015 (CET) 
}
