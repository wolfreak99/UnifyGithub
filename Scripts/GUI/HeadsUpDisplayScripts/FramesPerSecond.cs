/*************************
 * Original url: http://wiki.unity3d.com/index.php/FramesPerSecond
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/HeadsUpDisplayScripts/FramesPerSecond.cs
 * File based on original modification date of: 8 May 2015, at 20:50. 
 *
 * FPSDisplay.cs Author: Dave Hampson 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.HeadsUpDisplayScripts
{
    
    
    Contents [hide] 
    1 FPSDisplay.cs 
    2 Description 
    3 JavaScript - FramesPerSecond.js 
    4 CSharp HUDFPS.cs 
    5 Boo FPS_Display.boo 
    6 Other CSharp HUDFPS.cs 
    7 Other UnityScript HUDFPS.js 
    8 FPSCounter.cs 
    
    Here is a FPS Display script I created. It doesn't require a GUIText element and it also shows milliseconds, so just drop it into a GameObject and you should be ready to go. 
    Hope someone finds it useful! 
    using UnityEngine;
    using System.Collections;
     
    public class FPSDisplay : MonoBehaviour
    {
    	float deltaTime = 0.0f;
     
    	void Update()
    	{
    		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    	}
     
    	void OnGUI()
    	{
    		int w = Screen.width, h = Screen.height;
     
    		GUIStyle style = new GUIStyle();
     
    		Rect rect = new Rect(0, 0, w, h * 2 / 100);
    		style.alignment = TextAnchor.UpperLeft;
    		style.fontSize = h * 2 / 100;
    		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
    		float msec = deltaTime * 1000.0f;
    		float fps = 1.0f / deltaTime;
    		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    		GUI.Label(rect, text, style);
    	}
    }
    
    
    
    
    Author: Aras Pranckevicius (NeARAZ) 
    DescriptionUse this script on a GUIText object to display a FPS (frames per second) indicator. 
    It calculates frames/second over a defined interval, so the displayed number does not keep changing wildly. It is also fairly accurate at very low FPS counts (<10). The frames per second remain accurate if the time scale of the game is changed. 
    
    Note that in the web player, the frame rate is capped at 60fps by default - you can increase this by using Application.targetFrameRate. 
    
    
    JavaScript - FramesPerSecond.js// Attach this to a GUIText to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.
     
    var updateInterval = 0.5;
     
    private var accum = 0.0; // FPS accumulated over the interval
    private var frames = 0; // Frames drawn over the interval
    private var timeleft : float; // Left time for current interval
     
    function Start()
    {
        if( !guiText )
        {
            print ("FramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;  
    }
     
    function Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;
     
        // Interval ended - update GUI text and start new interval
        if( timeleft <= 0.0 )
        {
            // display two fractional digits (f2 format)
            guiText.text = "" + (accum/frames).ToString("f2");
            timeleft = updateInterval;
            accum = 0.0;
            frames = 0;
        }
    }
    
    CSharp HUDFPS.cs A C# implementation of the above converted by Opless. The main difference is the colour change when FPS dips too low. 
    using UnityEngine;
    using System.Collections;
     
    public class HUDFPS : MonoBehaviour 
    {
     
    // Attach this to a GUIText to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.
     
    public  float updateInterval = 0.5F;
     
    private float accum   = 0; // FPS accumulated over the interval
    private int   frames  = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
     
    void Start()
    {
        if( !guiText )
        {
            Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;  
    }
     
    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;
     
        // Interval ended - update GUI text and start new interval
        if( timeleft <= 0.0 )
        {
            // display two fractional digits (f2 format)
    	float fps = accum/frames;
    	string format = System.String.Format("{0:F2} FPS",fps);
    	guiText.text = format;
     
    	if(fps < 30)
    		guiText.material.color = Color.yellow;
    	else 
    		if(fps < 10)
    			guiText.material.color = Color.red;
    		else
    			guiText.material.color = Color.green;
    	//	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
    }Boo FPS_Display.boo A Boo implementation of the above converted by Philbywhizz. 
    import UnityEngine
     
    class FPS_Display (MonoBehaviour): 
     
        public updateInterval as single = 0.5
     
        private accum as single = 0 // FPS accumulated over the interval
        private frames as int = 0 // Frames drawn over the interval
        private timeleft as single // Left time for current interval
     
        def Start ():
            if(not guiText):
                Debug.Log("FPS Display needs a GUIText component!")
                enabled = false
            timeleft = updateInterval
     
        def Update ():
            timeleft -= Time.deltaTime
            accum += Time.timeScale/Time.deltaTime
            ++frames
     
            if (timeleft <= 0.0):
                fps = accum/frames
                format = System.String.Format("FPS: {0:F2}", fps);
                guiText.text = format
     
                if(fps < 30):
                    guiText.material.color = Color.yellow
                elif(fps < 10):
                    guiText.material.color = Color.red
                else:
                    guiText.material.color = Color.green
     
                timeleft = updateInterval
                accum = 0.0
                frames = 0Other CSharp HUDFPS.cs A different implementation of HUDFPS. Just attach it to a gameobject. 
    using UnityEngine;
    using System.Collections;
     
    [AddComponentMenu( "Utilities/HUDFPS")]
    public class HUDFPS : MonoBehaviour
    {
    	// Attach this to any object to make a frames/second indicator.
    	//
    	// It calculates frames/second over each updateInterval,
    	// so the display does not keep changing wildly.
    	//
    	// It is also fairly accurate at very low FPS counts (<10).
    	// We do this not by simply counting frames per interval, but
    	// by accumulating FPS for each frame. This way we end up with
    	// corstartRect overall FPS even if the interval renders something like
    	// 5.5 frames.
     
    	public Rect startRect = new Rect( 10, 10, 75, 50 ); // The rect the window is initially displayed at.
    	public bool updateColor = true; // Do you want the color to change if the FPS gets low
    	public bool allowDrag = true; // Do you want to allow the dragging of the FPS window
    	public  float frequency = 0.5F; // The update frequency of the fps
    	public int nbDecimal = 1; // How many decimal do you want to display
     
    	private float accum   = 0f; // FPS accumulated over the interval
    	private int   frames  = 0; // Frames drawn over the interval
    	private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
    	private string sFPS = ""; // The fps formatted into a string.
    	private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.
     
    	void Start()
    	{
    	    StartCoroutine( FPS() );
    	}
     
    	void Update()
    	{
    	    accum += Time.timeScale/ Time.deltaTime;
    	    ++frames;
    	}
     
    	IEnumerator FPS()
    	{
    		// Infinite loop executed every "frenquency" secondes.
    		while( true )
    		{
    			// Update the FPS
    		    float fps = accum/frames;
    		    sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
     
    			//Update the color
    			color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.red : Color.yellow);
     
    	        accum = 0.0F;
    	        frames = 0;
     
    			yield return new WaitForSeconds( frequency );
    		}
    	}
     
    	void OnGUI()
    	{
    		// Copy the default label skin, change the color and the alignement
    		if( style == null ){
    			style = new GUIStyle( GUI.skin.label );
    			style.normal.textColor = Color.white;
    			style.alignment = TextAnchor.MiddleCenter;
    		}
     
    		GUI.color = updateColor ? color : Color.white;
    		startRect = GUI.Window(0, startRect, DoMyWindow, "");
    	}
     
    	void DoMyWindow(int windowID)
    	{
    		GUI.Label( new Rect(0, 0, startRect.width, startRect.height), sFPS + " FPS", style );
    		if( allowDrag ) GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    	}
    }Other UnityScript HUDFPS.js A different implementation of HUDFPS. Just attach it to a gameobject. 
    // Attach this to any object to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // corstartRect overall FPS even if the interval renders something like
    // 5.5 frames.
     
    var startRect : Rect = Rect( 10, 10, 75, 50 ); // The rect the window is initially displayed at.
    var updateColor : boolean = true; // Do you want the color to change if the FPS gets low
    var allowDrag : boolean = true; // Do you want to allow the dragging of the FPS window
    var frequency : float = 0.5; // The update frequency of the fps
    var nbDecimal : int = 1; // How many decimal do you want to display
     
    private var accum : float = 0; // FPS accumulated over the interval
    private var frames : int = 0; // Frames drawn over the interval
    private var color : Color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
    private var sFPS : String= ""; // The fps formatted into a string.
    private var style : GUIStyle; // The style the text will be displayed at, based en defaultSkin.label.
     
    function Start()
    {
    	accum = 0f;
    	frame = 1;
     
        // Infinite loop executed every "frenquency" secondes.
        while( Application.isPlaying )
        {
            // Update the FPS
            var fps : float = accum / (frames > 0 ? frame : 1);
            sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
     
            //Update the color
            color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.red : Color.yellow);
     
            accum = 0;
            frames = 0;
     
            yield WaitForSeconds( frequency );
        }
    }
     
    function Update()
    {
        accum += Time.timeScale/ Time.deltaTime;
        ++frames;
    }
     
    function OnGUI()
    {
        // Copy the default label skin, change the color and the alignement
        if( style == null ){
            style = GUIStyle( GUI.skin.label );
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
        }
     
        GUI.color = updateColor ? color : Color.white;
        startRect = GUI.Window(0, startRect, DoMyWindow, "");
    }
     
    function DoMyWindow( windowID : int )
    {
        GUI.Label( Rect(0, 0, startRect.width, startRect.height), sFPS + " FPS", style );
        if( allowDrag ) GUI.DragWindow(Rect(0, 0, Screen.width, Screen.height));
    }FPSCounter.cs Yet another FPS counter. This one output fps value to a property as well as GUItext component attached to the same gameobject. It use no Update() event and need only one division to perform the calculation. It seem to provide good enough data to me. Please let me know if it's not? 
    /* **************************************************************************
     * FPS COUNTER
     * **************************************************************************
     * Written by: Annop "Nargus" Prapasapong
     * Created: 7 June 2012
     * *************************************************************************/
     
    using UnityEngine;
    using System.Collections;
     
    /* **************************************************************************
     * CLASS: FPS COUNTER
     * *************************************************************************/ 
    [RequireComponent(typeof(GUIText))]
    public class FPSCounter : MonoBehaviour {
    	/* Public Variables */
    	public float frequency = 0.5f;
     
    	/* **********************************************************************
    	 * PROPERTIES
    	 * *********************************************************************/
    	public int FramesPerSec { get; protected set; }
     
    	/* **********************************************************************
    	 * EVENT HANDLERS
    	 * *********************************************************************/
    	/*
    	 * EVENT: Start
    	 */
    	private void Start() {
    		StartCoroutine(FPS());
    	}
     
    	/*
    	 * EVENT: FPS
    	 */
    	private IEnumerator FPS() {
    		for(;;){
    			// Capture frame-per-second
    			int lastFrameCount = Time.frameCount;
    			float lastTime = Time.realtimeSinceStartup;
    			yield return new WaitForSeconds(frequency);
    			float timeSpan = Time.realtimeSinceStartup - lastTime;
    			int frameCount = Time.frameCount - lastFrameCount;
     
    			// Display it
    			FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
    			gameObject.guiText.text = FramesPerSec.ToString() + " fps";
    		}
    	}
}
}
