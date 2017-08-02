/*************************
 * Original url: http://wiki.unity3d.com/index.php/Box_Script
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/Box_Script.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Forest (yoggy) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    
    Contents [hide] 
    1 Description 
    2 Usage 
    3 JavaScript - Aspect.js 
    4 JavaScript - Box.js 
    5 JavaScript - Button.js 
    6 JavaScript - ButtonAction.js 
    7 JavaScript - MenuStyle.js 
    
    DescriptionA system with multiple scripts I set up to handle GUI boxes with different content and buttons with varying effects. You can set up the box border, font and other things once in the MenuStyle and then use it all over on any scale. This makes simple GUI objects less of a pain so you can get started quick. 
    A nice example unity project using this script can be found Here 
    UsageI recommend you download the project folder and check out how it is set up before trying to put it into your game. It is pretty complex but not all that hard to figure out. 
     
    
    (the fat lines don't look bad because of how the the code works... 3 minute programmer art ;) ) 
    
    
    JavaScript - Aspect.jsfunction Start ()
    {
    	aspect = 1 / Camera.main.aspect;
    	transform.localScale.x = transform.localScale.y * aspect;
    }JavaScript - Box.jsvar button = true;
    var lineWidth = 0.10;
     
    var startHeight = 0.00;
    var startWidth = 0.00;
     
    var contentString = "";
    var contentImage : Texture2D;
    var customContent : GameObject;
    var scaleContentByBoxSize = false;
    var adjustWidthForAspect = false;
    var contentScale = 5.00;
     
    private var content : GameObject;
    private var sens : GameObject;
    private var parts = new GameObject[9];
    private var style : MenuStyle;
     
    private var time = 0.00;
     
    private var newHeight = 0.00;
    private var newWidth = 0.00;
     
    private var height = 0.00;
    private var width = 0.00;
     
    function Start ()
    {
    	s = gameObject.Find("MenuStyle");
    	if(s) style = s.GetComponent(MenuStyle);
     
    	if(style)
    	{
    		transform.localScale = Vector3.one * style.boxScale;
     
    		if(adjustWidthForAspect) startWidth = startWidth * Camera.main.aspect;
     
    		height = startHeight;
    		width = startWidth;
     
    		i = 0;
    		while(i < 9)
    		{
    			n = new GameObject("part");
    			n.transform.parent = transform;
    			if(i != 0) n.transform.localPosition.z = 2;
    			n.AddComponent("GUITexture");
    			n.guiElement.texture = style.textures[i];
     
    			parts[i] = n;
    			i ++;
    		}
     
    		if(button)
    		{
    			sens = new GameObject("sens");
    			sens.transform.parent = transform;
    			sens.transform.localPosition.z = 10;
    			sens.AddComponent("GUITexture");
     
    			b = sens.AddComponent(Button);
    			b.box = this;
    			b.buttonAction = gameObject.GetComponent(ButtonAction);
    		}
     
    		if(contentString != "")
    		{
    			content = Instantiate(style.text);
    			content.guiText.text = contentString;
    		}
    		else if(contentImage)
    		{
    			content = new GameObject("BoxImage");
    			t = content.AddComponent("GUITexture");
    			t.texture = contentImage;
    		}
    		else if(customContent)
    		{
    			content = Instantiate(customContent);
    		}
     
    		if(content)
    		{
    			content.transform.parent = transform;
    			content.transform.localPosition.z = 1;
    		}
     
    		aspect = 1 / Camera.main.aspect;
    		transform.localScale.x = transform.localScale.y * aspect;
     
    		Align();
    	}
    }
     
    function Align()
    {
    	i = 0;
    	while(i < 9)
    	{
    		parts[i].transform.localPosition = Vector3(style.positions[i].x * width, style.positions[i].y * height, parts[i].transform.localPosition.z);
     
    		if(style.positions[i].y && style.positions[i].x)
    		{
    			parts[i].transform.localScale = Vector3.one * lineWidth;
    		}
    		else
    		{
    			parts[i].transform.localScale = (Vector3(Mathf.Abs(style.positions[i].y) * width, Mathf.Abs(style.positions[i].x) * height, 0) * 2) - Vector3.one * lineWidth;
    		}
     
    		if(i == 0)
    		{
    			parts[i].transform.localScale = Vector3(width, height, 0) * 2;
    			if(style.backgroundScaleByLineWidth) parts[i].transform.localScale -= Vector3.one * lineWidth;
    		}
     
    		i ++;
    	}
     
    	if(button)
    	{
    		sens.transform.localPosition = Vector3(0, 0, sens.transform.localPosition.z);
    		sens.transform.localScale = Vector3(width, height, 0) * 2;
    		if(style.backgroundScaleByLineWidth) sens.transform.localScale -= Vector3.one * lineWidth;
    	}
     
    	if(content)
    	{
    		content.transform.localPosition = Vector3(0, 0, content.transform.localPosition.z);
    		content.transform.localScale = Vector3.one * contentScale;
    		if(scaleContentByBoxSize)
    		{
    			content.transform.localScale = Vector3(width, height, 0) * contentScale;
    			if(style.backgroundScaleByLineWidth) content.transform.localScale -= Vector3.one * lineWidth;
    		}
    	}
    }
     
    function LerpSize ()
    {
    	var x : float;
    	while(x < 1 && width != newWidth || height != newHeight)
    	{
    		time += Time.deltaTime;
    		x = time / style.lerpTime; 
     
    		width = Mathf.Lerp(width, newWidth, x);
    		height = Mathf.Lerp(height, newHeight, x);
     
    		Align();
    		yield;
    	}
     
    	time = 0;
    }
     
    function Enter ()
    {
    	newHeight = startHeight * style.rolloverSize;
    	newWidth = startWidth * style.rolloverSize;
    	LerpSize();
    }
     
    function Down ()
    {
    	height = startHeight * style.clickedSize;
    	width = startWidth * style.clickedSize;
    	newHeight = startHeight * style.rolloverSize;
    	newWidth = startWidth * style.rolloverSize;
    	LerpSize();
    }
     
    function Up ()
    {
    	newHeight = startHeight;
    	newWidth = startWidth;
    	LerpSize();
    }
     
    function Exit ()
    {
    	newHeight = startHeight;
    	newWidth = startWidth;
    	LerpSize();
    }JavaScript - Button.jsvar box : Box;
    var buttonAction : ButtonAction;
     
    private var state = 0;
     
    function OnMouseEnter()
     {
     	state++;
     	if (state == 1)
    	{
     		if(box) box.Enter();
    	}
     }
     
     function OnMouseDown()
     {
     	state++;
     	if (state == 2)
    	{
     		if(box) box.Down();
    	}
     }
     
     function OnMouseUp()
     {
     	if (state == 2)
     	{
     		state--;
     
     		if(box) box.Up();
     		if(buttonAction) buttonAction.Click();
     	}
     	else
     	{
     		state --;
     		if (state < 0)
     			state = 0;
     	}
     	if(box) box.Exit();
     }
     
     function OnMouseExit()
     {
     	if (state > 0)
     		state--;
     	if (state == 0)
    	{
     		if(box) box.Exit();	
    	}
     }
    
    JavaScript - ButtonAction.jsvar deleteObject : GameObject;
    var deleteObjectWithName = "";
    var spawnObject : GameObject;
     
    var startLevel = false;
    var levelToStart = 1;
     
    var quit = false;
     
    function Click ()
    {
    	if(deleteObjectWithName != "") deleteObject = gameObject.Find(deleteObjectWithName);
     
    	if(deleteObject)
    	{
    		Destroy(deleteObject);
    	}
     
    	if(spawnObject)
    	{
    		Instantiate(spawnObject);
    	}
     
    	if(startLevel)
    	{
    		Application.LoadLevel(levelToStart);
    	}
     
    	if(quit)
    	{
    		Application.Quit();
    	}
    }
    
    JavaScript - MenuStyle.js// recomended configuration of textures: 0 background, 1 leftside, 2 top left corner, 3 top,
    // 4 topright corner, 5 right side, 6 bottom right corner, 7 bottom, 8 bottom left corner
     
    var textures = new Texture2D[9];
    var positions = new Vector3[9];
    var boxScale = 0.02;
    var rolloverSize = 1.20;
    var clickedSize = 0.80;
    var lerpTime = 1.00;
    var backgroundScaleByLineWidth = false;
var text : GameObject;
}
