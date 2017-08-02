/*************************
 * Original url: http://wiki.unity3d.com/index.php/GUIBuilder
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/GUIBuilder.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Shaun Lelacheur-Sales (shaunls) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Example Screenshots 
    4 C# - GenericGUIMethods.cs 
    5 C# - GenericTextFieldMethods.cs 
    
    DescriptionThe following scripts can be added to GUITextures/Texts to automate some of the simple Win/OSX common UI controls. 
    There are 2 main scripts used for different functions: 
    1. GenericGUIMethods - Use this script for panels, buttons, checkboxes etc. 
    2. GenericTextFieldMethods - Use this script for getting text input. 
    UsageSetting up GenericGUIMethods.cs 
    1. Create your GUI hierarchy (look at the first screenshot for an example) - the top object should ideally be a 'Panel' type. 
    2. Add the script to all non-static GUITextures in the hierarchy and set the following options: 
    VisibleOnAwake = Can we see this object when the player starts? 
    DontDestroyOnSceneLoad = Will be kept between scene changes - be careful as it could be loaded twice. 
    ButtonDisabledColor = the color of a button (or any object) when its its state is set to disabled. 
    ButtonEnabled = is the button (object) enabled or not (i.e can receive input) 
    TextureMouseSelected = Texture of selected state 
    TextureMouseOver = Texture of over state 
    TextureMouseNormal = Texture of normal state 
    ContextHelpText = The GUITexture that is used for popup help (on mouse over) 
    TopLevelParent = The uppermost parent in the hierarchy (should be the container panel) 
    LinkToGUITextGameObject = The GUIText object that this GUITexture is linked to (used for textbox graphics that sit behind a GUIText) 
    ObjectGlow = makes the object slowly glow (alternating glowing icons in a progress panel are a neat use for this option) 
    ButtonSelected = Is the button currently selected (generally used for checkboxes and radiobuttons) 
    Frozen = The frozen state is used to lock and fade a whole hierarcy (i use it for freezing the login panel when processing a login request) 
    TextFieldTabCount = Number of text fields in form hierarchy (COMING SOON) 
    TextFieldTabActive = Currently active tab (COMING SOON) 
    
    Setting up GenericTextFieldMethods.cs 
    1. In your form(panel) hierarchy you probably have some GUITextures that represent textboxes. You will need GUITexts to actually read the keyboard input from your user, so this script is used to 'link' the textbox GUITexture and GUIText together so that they will appear to operate as one. 
    2. Make the GUIText object a child to your textbox GUITexture (you can check the 2nd screenshot to see how its done) 
    3. Add the script to the GUIText and set the following options: 
    TopLevelParent = The uppermost parent in the hierarchy (should be the container panel) 
    TabID = This textfields TabID (COMING SOON) 
    MaximumWithInPixels = the maximum width in pixels that the text field can extend to 
    strDefaultText = the default text in the textfield 
    IsPasswordField = is this a normal text entry field, or password style (masked characters) 
    PasswordMaskingCharacter = if a password field, this character will be used to mask input (i generally just use "*" 
    IsLabel = Labels cannot receive input - set to false if you want to 'capture' typing 
    strRealText = This is the 'real' text contained in the textfield (not the displayed text) - generally can just be blank. 
    
    
    Good luck! 
    Example Screenshots 
    Above: Setting the options for a Radio button using the GenericGUIMethods Script. 
    
     
    Above: Setting the options for a password field using the GenericTextFieldMethods Script. 
    
    
    C# - GenericGUIMethods.csusing UnityEngine;
    using System.Collections;
     
    public class GenericGUIMethods : MonoBehaviour
    {
     
    //Init User Controlled Variables
    public bool VisibleOnAwake = true;
    public bool DontDestroyOnSceneLoad = false;
    public Color ButtonDisabledColor;
    public bool ButtonEnabled = true;
    public Texture TextureMouseSelected = null;
    public Texture TextureMouseOver = null;
    public Texture TextureMouseNormal = null;
    public GameObject ContextHelpText = null;
    public GameObject TopLevelParent = null;
    public GameObject LinkToGUITextGameObject = null;
    public bool ObjectGlow = false;
    public bool ButtonSelected = false;
    public bool Frozen = false;
    public int TextFieldTabCount = 0;
    public int TextFieldTabActive = 1;
     
     
    public enum SelectGUI_ObjectType
        {
            Panel,
            Button,
            Checkbox,
            RadioButton,
            Textbox,
            Icon
        }
    public SelectGUI_ObjectType m_SelectGUI_ObjectType = SelectGUI_ObjectType.Panel;
     
    public enum SelectScreenPosition
        {
            NoChange,
            Centered,
            TopMiddle,
            BottomMiddle
        }
    public SelectScreenPosition m_SelectScreenPosition = SelectScreenPosition.NoChange;
     
    //---Init Private Variables
    private bool MouseIsOver = false;
    private float CurrentOpacity = 0.5f;
    private int CurrentGlowDirection = 1;
    private int FadeDirection = 0;
     
     
    //--- SCREEN ALIGNMENT SCRIPT
    //Notes: need to add other basic screen positioning options (Top/Bottom Left, Top/Bottom Right etc) 
    void fncAlignToScreen () {
      	Rect rctCurrentPosition = guiTexture.pixelInset;
      	int intThisHeight = (int)Mathf.Abs((rctCurrentPosition.yMax - rctCurrentPosition.yMin));
        int intThisWidth = (int)Mathf.Abs((rctCurrentPosition.xMax - rctCurrentPosition.xMin));
    	float fltScreenUnitsWidth = 1.0f/Screen.width;
    	float fltScreenUnitsHeight = 1.0f/Screen.height;
    	float fltInsetAmount = 0.0f;
    	switch(m_SelectScreenPosition)
    	{
    	case SelectScreenPosition.Centered:
     
    		transform.position = new Vector3(0.5f,0.5f,transform.position.z);
      		break;
        case SelectScreenPosition.TopMiddle:
      		fltInsetAmount = 1 - ((intThisHeight/2.0f) * fltScreenUnitsHeight);
      		transform.position = new Vector3(0.5f,fltInsetAmount,transform.position.z);
    		break;
    	case SelectScreenPosition.BottomMiddle:
      		fltInsetAmount = (intThisHeight/2.0f) * fltScreenUnitsHeight;
      		transform.position = new Vector3(0.5f,fltInsetAmount,transform.position.z);
     
    		break;
    	case SelectScreenPosition.NoChange:
    		//Do Nothing
    		break;
    	default:
    		//Do Nothing
    		break;
    	}
    }
    //--- FIXED UPDATE SCRIPT
    //Notes: move glow script into IEnumerator function
    void FixedUpdate(){
    	//Do Glow
    	if(ObjectGlow==true){
    		if(CurrentOpacity < 0.5f && CurrentGlowDirection == 1){
    			guiTexture.color = new Color(0.5f,0.5f,0.5f,CurrentOpacity);
    			CurrentOpacity += 0.006f;
    		}else{
    			CurrentGlowDirection = 0;
    		}
    		if(CurrentOpacity > 0.15f && CurrentGlowDirection == 0){
    			guiTexture.color = new Color(0.5f,0.5f,0.5f,CurrentOpacity);
    			CurrentOpacity -= 0.006f;
    		}else{
    			CurrentGlowDirection = 1;
    		}
     
    	}
    }
     
    //--- AWAKE SCRIPT
    void Awake () {
    	TextFieldTabActive = 1;
    	//Set Dont Destroy on Load Flag
    	if(DontDestroyOnSceneLoad==true) DontDestroyOnLoad (this);
    	//Init Button States
    	fncInitButton();
    	//Init Screen Alignement function
    	fncAlignToScreen();
    	//Set visibility based on inspector flag
    	if(VisibleOnAwake==true){
    		guiTexture.enabled=true;
    	}else{
    		guiTexture.enabled=false;
    	}
    }
     
    //--- INIT BUTTONS
    //Notes: init scripts for other UI types should be added
    void fncInitButton(){
    		//Init Button Textures
    	switch(m_SelectGUI_ObjectType)
    	{
    	case SelectGUI_ObjectType.Button:
    		guiTexture.texture=TextureMouseNormal;
    		if(ButtonEnabled==false){
    			guiTexture.color=ButtonDisabledColor;
    		}
    		break;
    	case SelectGUI_ObjectType.Checkbox:
    		guiTexture.texture=TextureMouseNormal;
    		if(ButtonEnabled==false){
    			guiTexture.color=ButtonDisabledColor;
    		}
    		break;
    	case SelectGUI_ObjectType.RadioButton:
    		guiTexture.texture=TextureMouseNormal;
    		if(ButtonEnabled==false){
    			guiTexture.color=ButtonDisabledColor;
    		}
    		break;
    	default:
    		//Do Nothing
    		break;
    	}
    }
     
    //--- MESSAGE RECEIVER
    //Notes: Freeze GUITexture settings should not be hard coded - move to Public members later...
    void fncMessageReceiver (string strInternalCommand) {
     
     
    switch(strInternalCommand)
    	{
    	case "DisableSelectedButtons":
     
    		if(ButtonSelected==true && MouseIsOver != true){
    			fncSetButtonState("Normal");
    			ButtonSelected=false;
    		}
      		break;
      	case "FadeIn":
     
    		StartCoroutine ("fncFadeInOut", "FadeIn");
      		break;
      	case "FadeOut":
     
    		StartCoroutine ("fncFadeInOut", "FadeOut");
      		break;
    	case "MouseOverTextField":
     
    		if(m_SelectGUI_ObjectType == SelectGUI_ObjectType.Textbox) fncSetButtonState("Over");
      		break;	
      	case "MouseExitTextField":
     
    		if(m_SelectGUI_ObjectType == SelectGUI_ObjectType.Textbox) fncSetButtonState("Normal");
      		break;
      	case "ToggleFreeze":
      		if(Frozen==false){
    			guiTexture.color = new Color(0.5f,0.5f,0.5f,0.1f);
    		}else{
    			guiTexture.color = new Color(0.5f,0.5f,0.5f,0.5f);
    		}
    		Frozen = !Frozen;
      		break;
      	case "ToggleGuiEnabled":
      		guiTexture.enabled = !guiTexture.enabled;
      		break;
    	default:
    		//Do Nothing
    		break;
    	}
    }
     
    //--- FADE IN/OUT FUNCTION
    //Notes: the fading and glowing script could probably be merged - later...
    IEnumerator fncFadeInOut(string strFadeType){
    int intRunFade = 1;
    float fltFadeOpacity = 0.0f;
    float fltMaxOpacity = 0.5f;
    bool RestartGlow = false;
    if(ObjectGlow==true){
    	RestartGlow=true;
    	ObjectGlow=false;
    }
    if(strFadeType=="FadeIn"){
    	fltFadeOpacity = 0.0f;
    	guiTexture.enabled=true;
    	if(ButtonEnabled==false) fltMaxOpacity = 0.15f;
    }
     
    if(strFadeType=="FadeOut"){
    	fltFadeOpacity=guiTexture.color.a;
    	ObjectGlow=false;
    }
    while (intRunFade!=0)
            {
                switch (strFadeType)
                {
                    case "FadeIn":
                    	if(fltFadeOpacity <= fltMaxOpacity){
                    		guiTexture.color = new Color(0.5f,0.5f,0.5f,fltFadeOpacity);
                    		fltFadeOpacity += 0.008f;
                    	}else{
                    		intRunFade=0;
                    		fncInitButton();
                    		if(RestartGlow==true){
    							ObjectGlow=true;
    						}	
                    	}
    	                break;
                	case "FadeOut":
                	    if(fltFadeOpacity >= 0.0f){
                    		guiTexture.color = new Color(0.5f,0.5f,0.5f,fltFadeOpacity);
                    		fltFadeOpacity -= 0.008f;
                    	}else{
                    		intRunFade=0;
                    	}
     
                    	break;
     
                }
                yield return 0;
            }
    if(strFadeType=="FadeIn"){ 
    	guiTexture.color = new Color(0.5f,0.5f,0.5f,fltMaxOpacity);
    }
    	if(strFadeType=="FadeOut"){
    		guiTexture.color = new Color(0.5f,0.5f,0.5f,0.0f);
    		guiTexture.enabled=false;
    	}
    }
     
    //--- SET BUTTON STATE
    //Notes: buttons with no GUITextures use hardcoded alpha values - should be user controlled
    void fncSetButtonState (string strState) {
    	switch(strState)
    	{
    	case "Normal":
     
    		if(TextureMouseNormal!=null){
    			guiTexture.texture=TextureMouseNormal;
    		}
    		guiTexture.color = new Color(0.5f,0.5f,0.5f,0.5f);
      		break;
      	case "Over":
     
    		if(TextureMouseOver!=null){
    			guiTexture.texture=TextureMouseOver;
    		}else{
    			guiTexture.color = new Color(0.6f,0.6f,0.6f,0.6f);	
    		}
      		break;
      	case "Selected":
     
    		if(TextureMouseSelected!=null){
    			guiTexture.texture=TextureMouseSelected;
    			guiTexture.color = new Color(0.5f,0.5f,0.5f,0.5f);
    		}
      		break;
      	case "Active":
    			guiTexture.color = new Color(0.8f,0.8f,0.8f,0.5f);
      		break;
       	default:
      		//No Action
      		break;
     
    	}
    }
     
    //--- MOUSE ENTER
    void OnMouseEnter(){
    if(Frozen==false){
    	MouseIsOver = true;
    	if(ContextHelpText != null){
    	ContextHelpText.guiTexture.enabled = true;
    	}
    	switch(m_SelectGUI_ObjectType)
    	{
    	case SelectGUI_ObjectType.Button:
    		fncSetButtonState("Over");
    		break;
    	case SelectGUI_ObjectType.Panel:
    		//No Action
    		break;
    	case SelectGUI_ObjectType.Checkbox:
    		fncSetButtonState("Over");
    		break;
    	case SelectGUI_ObjectType.RadioButton:
    		if(ButtonSelected==false){
    		fncSetButtonState("Over");
    		}
    		break;
    	case SelectGUI_ObjectType.Textbox:
    		fncSetButtonState("Over");
    		break;		
    	default:
    		//No Action
    		break;	
    	}
    }
    }	
     
    //--- MOUSE EXIT
    void OnMouseExit(){
    if(Frozen==false){
    	MouseIsOver = false;
    	if(ContextHelpText != null){
    	ContextHelpText.guiTexture.enabled = false;
    	}
    	switch(m_SelectGUI_ObjectType)
    	{
    	case SelectGUI_ObjectType.Button:
    		fncSetButtonState("Normal");
    		break;
    	case SelectGUI_ObjectType.Panel:
    		//No Action
    		break;
    	case SelectGUI_ObjectType.Checkbox:
    		if(ButtonSelected==false){
    			fncSetButtonState("Normal");
    		}else{
    			fncSetButtonState("Selected");
    		}
    		break;
    	case SelectGUI_ObjectType.RadioButton:
    		if(ButtonSelected==false){
    		fncSetButtonState("Normal");
    		}
    		break;
    	case SelectGUI_ObjectType.Textbox:
    		fncSetButtonState("Normal");
    		break;		
    	default:
    		//No Action
    		break;	
    	}
    }
    }
     
    //--- MOUSE DOWN
    void OnMouseDown(){
    	switch(m_SelectGUI_ObjectType)
    	{
    	case SelectGUI_ObjectType.Button:
    		fncSetButtonState("Active");
    		break;
    	case SelectGUI_ObjectType.Panel:
    		//No Action
    		break;
    	case SelectGUI_ObjectType.Checkbox:
    		//No Action
    		break;
    	case SelectGUI_ObjectType.RadioButton:
    		//No Action
    		break;
    	default:
    		//No Action
    		break;	
    	}
    }
     
    //--- MOUSE UP
    //Bug: If users mouse downs over a button, moves off and mouse ups, things get messy - probably can be solved by checking Mouse down instead
    void OnMouseUp(){
    	switch(m_SelectGUI_ObjectType)
    	{
    	case SelectGUI_ObjectType.Button:
    		fncSetButtonState("Normal");
    		break;
    	case SelectGUI_ObjectType.Panel:
    		//No Action
    		break;
    	case SelectGUI_ObjectType.Checkbox:
    		if(ButtonSelected==false){
    	 		fncSetButtonState("Selected");
    	 		ButtonSelected=true;
    		}else{ 
    	 		fncSetButtonState("Normal");
    	 		ButtonSelected=false;
    		}
    		break;
    	case SelectGUI_ObjectType.RadioButton:
    		if(ButtonSelected==false){
    	 		fncSetButtonState("Selected");
    	 		ButtonSelected=true;
    			TopLevelParent.BroadcastMessage("fncMessageReceiver","DisableSelectedButtons");	
    	 	}
    		break;
    	case SelectGUI_ObjectType.Textbox:
    		GenericTextFieldMethods genericTextFieldMethods = (GenericTextFieldMethods)LinkToGUITextGameObject.GetComponent(typeof(GenericTextFieldMethods));
    		genericTextFieldMethods.fncSetActiveTabID("This");
    		break;
    	default:
    		//DoNothing
    		break;	
    	}
    }
    ///////////END OF GENERIC GUI METHODS CLASS
    }C# - GenericTextFieldMethods.csusing UnityEngine;
    using System.Collections;
     
    public class GenericTextFieldMethods : MonoBehaviour {
     
    //Init Public Variables
    	public GameObject TopLevelParent = null;
    	public int TabID = 0;
    	public int MaximumWithInPixels = 100;
    	public string strDefaultText = "";
    	public bool IsPasswordField = false;
    	public string PasswordMaskingCharacter = "*";
    	public bool IsLabel = false;
    	public string strRealText = "";
     
    //Init Private Variables
    	private int intCurrentActiveTab = 0;
    	private bool blnIsFrozen = false;
     
    // AWAKE
    	void Awake () {
    	guiText.text = strDefaultText;
    	}
     
    // GET ACTIVE TABID
    	int fncGetActiveTabID(){
    		GenericGUIMethods genericGUIMethods = (GenericGUIMethods)TopLevelParent.GetComponent(typeof(GenericGUIMethods));
    		return genericGUIMethods.TextFieldTabActive;
    	}
     
    // SET ACTIVE TABID
    //Bug: currently using the TAB key to change fields doesnt work - will be fixed ASAP
    	public void fncSetActiveTabID(string strFunction){
    		if(IsLabel!=true){
    			if(guiText.text == strDefaultText) guiText.text = "";
    			GenericGUIMethods genericGUIMethods = (GenericGUIMethods)TopLevelParent.GetComponent(typeof(GenericGUIMethods));
    			switch(strFunction)
    			{
    			case "This":
    				genericGUIMethods.TextFieldTabActive=TabID;
    				break;
    			/*case "Next": //TO BE FIXED
    				if(TabID < genericGUIMethods.TextFieldTabCount){
    					genericGUIMethods.TextFieldTabActive++;
    				}else{
    					genericGUIMethods.TextFieldTabActive=1;
    				}
    				break;*/
    			}
    		}
    	}
     
    // UPDATE
    	void Update () {
    		if(fncGetActiveTabID()==TabID && IsLabel!=true){	
    			foreach(char c in Input.inputString) {
    	        if(guiText.text == strDefaultText) guiText.text = "";	
    	        	switch(c)
    				{
    				case '\b':  // Backspace - Remove the last character
                		if (strRealText.Length != 0){
                    		strRealText = strRealText.Substring(0,strRealText.Length - 1);
            			}
            			if (guiText.text.Length != 0){
                    		guiText.text = guiText.text.Substring(0, guiText.text.Length - 1);
            			}
    					break;
    				case '\t':  // Tab
                		//fncSetActiveTabID("Next");
    					break;
    				case '\n':  // Enter
                		//No Action
    					break;
    				default: //normal character input
    	        		//if the text will overflow, ignore further user input
    	        		Rect tbRect = guiText.GetScreenRect();
    					if(tbRect.width < MaximumWithInPixels){
    	            		if(IsPasswordField==true){
    	            			guiText.text += PasswordMaskingCharacter;
    	            			strRealText += c;
    	            		}else{
    	            			guiText.text += c;
    	            			strRealText += c;
    	            		}
    	    			}
    	    			break;
    				}
    			}
    		}
    	}
     
    	void OnMouseUp () {
    		if(IsLabel!=true) fncSetActiveTabID("This");		
    	}
     
    	void OnMouseOver () {
    		if(IsLabel!=true) gameObject.SendMessageUpwards ("fncMessageReceiver", "MouseOverTextField");	
    	}
    	void OnMouseExit () {
    		if(IsLabel!=true) gameObject.SendMessageUpwards ("fncMessageReceiver", "MouseExitTextField");	
    	}
    //--- MESSAGE RECEIVER
    	void fncMessageReceiver (string strInternalCommand) {
     
     
    	switch(strInternalCommand)
    		{
     
    	  	case "ToggleFreeze":
    	  		guiText.enabled = !guiText.enabled;
    	  		if(guiText.enabled==true){ 
    	  			blnIsFrozen=false;
    	  		}else{
    	  			blnIsFrozen=true;
    	  		}
    	   		break;
    	  	case "ToggleGuiEnabled":
    			if(blnIsFrozen==false) guiText.enabled = !guiText.enabled;
    	  		break;
    		default:
    			//No Action
    			break;
    		}
    	}
     
    // END OF GENERIC TEXT FIELD METHODS CLASS
}
}
