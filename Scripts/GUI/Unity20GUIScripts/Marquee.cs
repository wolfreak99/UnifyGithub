/*************************
 * Original url: http://wiki.unity3d.com/index.php/Marquee
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/Marquee.cs
 * File based on original modification date of: 10 February 2012, at 18:13. 
 *
 * Author: Matthew Miner (matthew@matthewminer.com) 
 *
 * Description 
 *   
 * Usage 
 *   
 * C# - Marquee.cs 
 *   
 * AdvancedMarquee Description 
 *   
 * AdvancedMarquee Usage 
 *   
 * C# - AdvancedMarquee.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    Description Creates a scrolling label that moves across the screen horizontally left-to-right, looping back once it reaches the edge of the screen. 
    Usage Attach the script to a game object and change the message and scrollSpeed fields in the inspector. 
    C# - Marquee.cs using UnityEngine;
     
    public class Marquee : MonoBehaviour
    {
    	public string message	 = "Where we're going, we don't need roads.";
    	public float scrollSpeed = 50;
     
    	Rect messageRect;
     
    	void OnGUI ()
    	{
    		// Set up the message's rect if we haven't already
    		if (messageRect.width == 0) {
    			Vector2 dimensions = GUI.skin.label.CalcSize(new GUIContent(message));
     
    			// Start the message past the left side of the screen
    			messageRect.x      = -dimensions.x;
    			messageRect.width  =  dimensions.x;
    			messageRect.height =  dimensions.y;
    		}
     
    		messageRect.x += Time.deltaTime * scrollSpeed;
     
    		// If the message has moved past the right side, move it back to the left
    		if (messageRect.x > Screen.width) {
    			messageRect.x = -messageRect.width;
    		}
     
    		GUI.Label(messageRect, message);
    	}
    }
    
    AdvancedMarquee Description Modified marquee script from above. 
    There are conditions for movement from left to right, right to left, up and down 
    AdvancedMarquee Usage Same as previous 
    C# - AdvancedMarquee.cs Author: SirGive 
    using UnityEngine;
     
    [RequireComponent (typeof (GUIText))]
     
    public class AdvancedMarquee: MonoBehaviour
    {
    	string message = "Scrolling... Check me out";
    	public bool leftToRight = true;
    	public bool rightToLeft = false;
    	public bool upward = false;
    	public bool downward = false;
    	public float xScrollSpeed = 50;
    	private float xPosition	= 0;
    	public float yScrollSpeed = 0;
    	private float yPosition	= 0;
    	public GUIText outputText;
     
     
        Rect messageRect;
     
     
        void OnGUI ()
        {
          	//this sets the size of the text (width and height) into the dimensions variable
    			Vector2 dimensions = GUI.skin.label.CalcSize(new GUIContent(message));
     
    			//sets the size of the rectangle that the text is printed on
    			//helpful to have accurate scrolling off screen
                messageRect.width  =  dimensions.x;
                messageRect.height =  dimensions.y;
     
    	        //limits these events to every frame. Otherwise, the scrolling text will speed up with any input
    		if (Event.current.type == EventType.Repaint)
                    {
    		//This sets the string to the text of the GUIText
    		outputText.text = message;
     
    		//This increments the positions	accordingly
    		scrollingText();
     
    		//This sets the positions changed by the scrollingText() function to the offset pixel values
    		outputText.pixelOffset = new Vector2(xPosition, yPosition);
     
    		//this functions checks to see if the text has gone past the screen
    		//if it has, then it resets it
    		reset();
    	        }
        }	
     
     
    	void scrollingText()
    	{		
    		if(leftToRight == true)
            xPosition += -2*(xScrollSpeed/100);
    		if(rightToLeft == true)
            xPosition += 2*(xScrollSpeed/100);
    		if(downward == true)
            yPosition += -2*(yScrollSpeed/100);
    		if(upward == true)
            yPosition += 2*(yScrollSpeed/100);
    	}
     
     
    	void reset()
    	{
    		 // If the message has moved past the right side, move it back to the left
            if (xPosition > 0) 
                xPosition = -Screen.width - messageRect.width;
    	if(xPosition < -Screen.width - messageRect.width)
    	    xPosition = 0;
            if (yPosition > Screen.height + messageRect.height)
    	    yPosition = 0 + -messageRect.height;
            if (yPosition < -messageRect.height)
    	    yPosition = Screen.height + messageRect.height;
    	}
}
}
