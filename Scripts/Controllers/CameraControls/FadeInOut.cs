/*************************
 * Original url: http://wiki.unity3d.com/index.php/FadeInOut
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/FadeInOut.cs
 * File based on original modification date of: 9 November 2012, at 23:54. 
 *
 * JavaScript Introduction Author: PsychicParrot 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CameraControls
{
    
    
    Contents [hide] 
    1 JavaScript 
    1.1 Introduction 
    1.2 Source (FadeInOut.js) 
    2 Boo 
    2.1 Introduction 
    2.2 Source (CameraFade.boo) 
    3 Another Fade Script in C# 
    3.1 Introduction 
    3.2 Source (CameraFade.cs) 
    4 Another Fade Script in C#, Extended Singleton 
    4.1 Introduction 
    4.2 Source (CameraFade.cs) 
    5 C# 
    5.1 Introduction 
    5.2 Source (CameraFade.cs) 
    
    Little script to fade an image (stretched to fit the screen, so use a 1x1 black pixel for a simple black fade in/out). 
    Simply apply it to your camera (or an empty GameObject), set the texture to use, set the fadeSpeed and call fadeIn() or fadeOut() 
    Easiest way is probably to apply this script to your main camera in the scene, then wherever you want to fade use something like: 
    Camera.main.SendMessage("fadeOut"); 
    or 
    Camera.main.SendMessage("fadeIn"); 
    Enjoy! 
    Source (FadeInOut.js) // FadeInOut
    //
    //--------------------------------------------------------------------
    //                        Public parameters
    //--------------------------------------------------------------------
     
    public var fadeOutTexture : Texture2D;
    public var fadeSpeed = 0.3;
     
    var drawDepth = -1000;
     
    //--------------------------------------------------------------------
    //                       Private variables
    //--------------------------------------------------------------------
     
    private var alpha = 1.0; 
     
    private var fadeDir = -1;
     
    //--------------------------------------------------------------------
    //                       Runtime functions
    //--------------------------------------------------------------------
     
    //--------------------------------------------------------------------
     
    function OnGUI(){
     
    	alpha += fadeDir * fadeSpeed * Time.deltaTime;	
    	alpha = Mathf.Clamp01(alpha);	
     
    	GUI.color.a = alpha;
     
    	GUI.depth = drawDepth;
     
    	GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }
     
    //--------------------------------------------------------------------
     
    function fadeIn(){
    	fadeDir = -1;	
    }
     
    //--------------------------------------------------------------------
     
    function fadeOut(){
    	fadeDir = 1;	
    }
     
    function Start(){
    	alpha=1;
    	fadeIn();
    }Boo Introduction Author: Adrian 
    Extended version written in Boo (put it into "Standard Assets/Scripts" to use it from within JavaScript files). 
    Main change is that rather than defining a fade speed you can set the duration of the fade in seconds. 
    Also provides methods to set the fade duration when the method is called and static methods to call the fade script on the main camera (by calling CameraFade.FadeInMain()). 
    Source (CameraFade.boo) import UnityEngine
     
    class CameraFade (MonoBehaviour): 
     
    	# ---------------------------------------- #
    	# PUBLIC FIELDS
     
    	# Alpha start value
    	public startAlpha as single = 1
     
    	# Texture used for fading
    	public fadeTexture as Texture2D
     
    	# Default time a fade takes in seconds
    	public fadeDuration as single = 2
     
    	# Depth of the gui element
    	public guiDepth as int = -1
     
    	# Fade into scene at start
    	public fadeIntoScene as bool = true
     
    	# ---------------------------------------- #
    	# PRIVATE FIELDS
     
    	# Current alpha of the texture
    	currentAlpha as single = 1
     
    	# Current duration of the fade
    	currentDuration as single
     
    	# Direction of the fade
    	fadeDirection as int = -1
     
    	# Fade alpha to
    	targetAlpha as single = 0
     
    	# Alpha difference
    	alphaDifference as single = 0
     
    	# Style for background tiling
    	private backgroundStyle as GUIStyle = GUIStyle()
    	private dummyTex as Texture2D
     
    	# ---------------------------------------- #
    	# START FADE METHODS
     
    	def FadeIn(duration as single, to as single):
    		# Set fade duration
    		currentDuration = duration
    		# Set target alpha
    		targetAlpha = to
    		# Difference
    		alphaDifference = Mathf.Clamp01(currentAlpha - targetAlpha)
    		# Set direction to Fade in
    		fadeDirection = -1
     
    	def FadeIn():
    		FadeIn(fadeDuration, 0)
     
    	def FadeIn(duration as single):
    		FadeIn(duration, 0)
     
    	def FadeOut(duration as single, to as single):
    		# Set fade duration
    		currentDuration = duration
    		# Set target alpha
    		targetAlpha = to
    		# Difference
    		alphaDifference = Mathf.Clamp01(targetAlpha - currentAlpha)
    		# Set direction to fade out
    		fadeDirection = 1
     
    	def FadeOut():
    		FadeOut(fadeDuration, 1)
     
    	def FadeOut(duration as single):
    		FadeOut(duration, 1)
     
    	# ---------------------------------------- #
    	# STATIC FADING FOR MAIN CAMERA
     
    	static def FadeInMain(duration as single, to as single):
    		GetInstance().FadeIn(duration, to)
     
    	static def FadeInMain():
    		GetInstance().FadeIn()
     
    	static def FadeInMain(duration as single):
    		GetInstance().FadeIn(duration)
     
    	static def FadeOutMain(duration as single, to as single):
    		GetInstance().FadeOut(duration, to)
     
    	static def FadeOutMain():
    		GetInstance().FadeOut()
     
    	static def FadeOutMain(duration as single):
    		GetInstance().FadeOut(duration)
     
    	# Get script fom Camera
    	static def GetInstance() as CameraFade:
    		# Get Script
    		fader as CameraFade = Camera.main.GetComponent(CameraFade)
    		# Check if script exists
    		if (fader == null):
    			raise System.Exception("No CameraFade attached to the main camera.")
    		return fader
     
    	# ---------------------------------------- #
    	# SCENE FADEIN
     
    	def Start():
    		dummyTex = Texture2D(1,1)
    		dummyTex.SetPixel(0,0,Color.clear)
    		backgroundStyle.normal.background = fadeTexture
    		currentAlpha = startAlpha
    		if fadeIntoScene:
    			FadeIn()
     
    	# ---------------------------------------- #
    	# FADING METHOD
     
    	def OnGUI():
     
    		# Fade alpha if active
    		if ((fadeDirection == -1 and currentAlpha > targetAlpha)
    				or
    			(fadeDirection == 1 and currentAlpha < targetAlpha)):
    			# Advance fade by fraction of full fade time
    			currentAlpha += (fadeDirection * alphaDifference) * (Time.deltaTime / currentDuration)
    			# Clamp to 0-1
    			currentAlpha = Mathf.Clamp01(currentAlpha)
     
    		# Draw only if not transculent
    		if (currentAlpha > 0):
    			# Draw texture at depth
    			GUI.color.a = currentAlpha;
    			GUI.depth = guiDepth;
    			GUI.Label(Rect(-10, -10, Screen.width + 10, Screen.height + 10), dummyTex, backgroundStyle)
    
    Another Fade Script in C# Introduction Author: Kentyman 
    Hello everybody! I was looking for an easy way to fade the screen and found this page. The scripts here didn't do quite what I wanted, so I rewrote the C# version. This one will fade from any color to any color. Usage: use "SetScreenOverlayColor" to set the initial color, then use "StartFade" to set the target color and the fade duration (in seconds) and start the fade. 
    Source (CameraFade.cs)  
    // simple fading script
    // A texture is stretched over the entire screen. The color of the pixel is set each frame until it reaches its target color.
     
     
    using UnityEngine;
     
     
    public class CameraFade : MonoBehaviour
    {   
    	private GUIStyle m_BackgroundStyle = new GUIStyle();		// Style for background tiling
    	private Texture2D m_FadeTexture;				// 1x1 pixel texture used for fading
    	private Color m_CurrentScreenOverlayColor = new Color(0,0,0,0);	// default starting color: black and fully transparrent
    	private Color m_TargetScreenOverlayColor = new Color(0,0,0,0);	// default target color: black and fully transparrent
    	private Color m_DeltaColor = new Color(0,0,0,0);		// the delta-color is basically the "speed / second" at which the current color should change
    	private int m_FadeGUIDepth = -1000;				// make sure this texture is drawn on top of everything
     
     
    	// initialize the texture, background-style and initial color:
    	private void Awake()
    	{		
    		m_FadeTexture = new Texture2D(1, 1);        
            m_BackgroundStyle.normal.background = m_FadeTexture;
    		SetScreenOverlayColor(m_CurrentScreenOverlayColor);
     
    		// TEMP:
    		// usage: use "SetScreenOverlayColor" to set the initial color, then use "StartFade" to set the desired color & fade duration and start the fade
    		//SetScreenOverlayColor(new Color(0,0,0,1));
    		//StartFade(new Color(1,0,0,1), 5);
    	}
     
     
    	// draw the texture and perform the fade:
    	private void OnGUI()
        {   
    		// if the current color of the screen is not equal to the desired color: keep fading!
    		if (m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor)
    		{			
    			// if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
    			if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * Time.deltaTime)
    			{
    				m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
    				SetScreenOverlayColor(m_CurrentScreenOverlayColor);
    				m_DeltaColor = new Color(0,0,0,0);
    			}
    			else
    			{
    				// fade!
    				SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime);
    			}
    		}
     
    		// only draw the texture when the alpha value is greater than 0:
    		if (m_CurrentScreenOverlayColor.a > 0)
    		{			
                		GUI.depth = m_FadeGUIDepth;
                		GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
    		}
        }
     
     
    	// instantly set the current color of the screen-texture to "newScreenOverlayColor"
    	// can be usefull if you want to start a scene fully black and then fade to opague
    	public void SetScreenOverlayColor(Color newScreenOverlayColor)
    	{
    		m_CurrentScreenOverlayColor = newScreenOverlayColor;
    		m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
    		m_FadeTexture.Apply();
    	}
     
     
    	// initiate a fade from the current screen color (set using "SetScreenOverlayColor") towards "newScreenOverlayColor" taking "fadeDuration" seconds
    	public void StartFade(Color newScreenOverlayColor, float fadeDuration)
    	{
    		if (fadeDuration <= 0.0f)		// can't have a fade last -2455.05 seconds!
    		{
    			SetScreenOverlayColor(newScreenOverlayColor);
    		}
    		else					// initiate the fade: set the target-color and the delta-color
    		{
    			m_TargetScreenOverlayColor = newScreenOverlayColor;
    			m_DeltaColor = (m_TargetScreenOverlayColor - m_CurrentScreenOverlayColor) / fadeDuration;
    		}
    	}
    }Another Fade Script in C#, Extended Singleton Introduction Original Source: Kentyman Extended Source: dannyskim 
    I wanted to extend the functionality of Kentyman's implementation, so I decided to do a few things. 
       - Make it into a Singleton so I don't have to write various amounts of code everywhere to do the same thing.  
          The singleton also takes care of cleanup, which also involves not having to worry about a loose cannon OnGUI() sucking up resources.
       - I almost never use Color to Color fades.  I typically use Alpha fades of one color, feel free to add the Color to Color fade if you wish.
       - Add an OnComplete() Action. I typically will use fades in scene transitions, so it's handy to pass in the instruction to 
          load a level instead of having to write a method every time you want to.
    To use this Singleton, all you have to do is: 
    CameraFade.StartAlphaFade( Color.white, false, 2f, 2f, () => { Application.LoadLevel(1); } );The above will fade out to a White screen, after a delay of two seconds. The fade will take two seconds, and then it will Load Level 1, and then automatically destroy itself, cleaning up the private static reference. It will also destroy itself after the OnFinish() call, preventing OnGUI() from running anymore. 
    Note: As of now, Lambda Expressions are not currently supported in the Flash beta of Unity3D, but regular Actions still are. To forego the possibility of Lambda Expressions breaking your .swf, simply pass in a void method instead of the Lambda Expression for OnComplete(). 
    CameraFade.StartAlphaFade( Color.white, false, 2f, 2f, loadLevelOnComplete(1) );
     
    void loadLevelOnComplete( int levelIndex )
    {
        Application.LoadLevel(levelIndex);
    }Since it is a singleton, the above line of code is all you have to do. No instantiating, no disabling the script, no finding the script via GetComponent<>(), etc. Nice and easy. 
    Source (CameraFade.cs)  
    using UnityEngine;
    using System;
     
    public class CameraFade : MonoBehaviour
    {   
    	private static CameraFade mInstance = null;
     
    	private static CameraFade instance
    	{
    		get
    		{
    			if( mInstance == null )
    			{
    				mInstance = GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;
     
    				if( mInstance == null )
    				{
    					mInstance = new GameObject("CameraFade").AddComponent<CameraFade>();
    				}
    			}
     
    			return mInstance;
    		}
    	}
     
    	void Awake()
    	{
    		if( mInstance == null )
    		{
    			mInstance = this as CameraFade;
    			instance.init();
    		}
    	}
     
    	public GUIStyle m_BackgroundStyle = new GUIStyle();						// Style for background tiling
    	public Texture2D m_FadeTexture;											// 1x1 pixel texture used for fading
    	public Color m_CurrentScreenOverlayColor = new Color(0,0,0,0);			// default starting color: black and fully transparrent
    	public Color m_TargetScreenOverlayColor = new Color(0,0,0,0);			// default target color: black and fully transparrent
    	public Color m_DeltaColor = new Color(0,0,0,0);							// the delta-color is basically the "speed / second" at which the current color should change
    	public int m_FadeGUIDepth = -1000;										// make sure this texture is drawn on top of everything
     
    	public float m_FadeDelay = 0;
    	public Action m_OnFadeFinish = null;
     
    	// Initialize the texture, background-style and initial color:
    	public void init()
    	{		
    		instance.m_FadeTexture = new Texture2D(1, 1);        
            instance.m_BackgroundStyle.normal.background = instance.m_FadeTexture;
    	}
     
    	// Draw the texture and perform the fade:
    	void OnGUI()
        {   
    		// If delay is over...
    		if( Time.time > instance.m_FadeDelay )
    		{
    			// If the current color of the screen is not equal to the desired color: keep fading!
    			if (instance.m_CurrentScreenOverlayColor != instance.m_TargetScreenOverlayColor)
    			{			
    				// If the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
    				if (Mathf.Abs(instance.m_CurrentScreenOverlayColor.a - instance.m_TargetScreenOverlayColor.a) < Mathf.Abs(instance.m_DeltaColor.a) * Time.deltaTime)
    				{
    					instance.m_CurrentScreenOverlayColor = instance.m_TargetScreenOverlayColor;
    					SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor);
    					instance.m_DeltaColor = new Color( 0,0,0,0 );
     
    					if( instance.m_OnFadeFinish != null )
    						instance.m_OnFadeFinish();
     
    					Die();
    				}
    				else
    				{
    					// Fade!
    					SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor + instance.m_DeltaColor * Time.deltaTime);
    				}
    			}
    		}
    		// Only draw the texture when the alpha value is greater than 0:
    		if (m_CurrentScreenOverlayColor.a > 0)
    		{			
        		GUI.depth = instance.m_FadeGUIDepth;
        		GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), instance.m_FadeTexture, instance.m_BackgroundStyle);
    		}
        }
     
     
    	/// <summary>
    	/// Sets the color of the screen overlay instantly.  Useful to start a fade.
    	/// </summary>
    	/// <param name='newScreenOverlayColor'>
    	/// New screen overlay color.
    	/// </param>
    	private static void SetScreenOverlayColor(Color newScreenOverlayColor)
    	{
    		instance.m_CurrentScreenOverlayColor = newScreenOverlayColor;
    		instance.m_FadeTexture.SetPixel(0, 0, instance.m_CurrentScreenOverlayColor);
    		instance.m_FadeTexture.Apply();
    	}
     
     	/// <summary>
     	/// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent.
     	/// </summary>
     	/// <param name='newScreenOverlayColor'>
     	/// Target screen overlay Color.
     	/// </param>
     	/// <param name='fadeDuration'>
     	/// Fade duration.
     	/// </param>
    	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration )
    	{
    		if (fadeDuration <= 0.0f)		
    		{
    			SetScreenOverlayColor(newScreenOverlayColor);
    		}
    		else					
    		{
    			if( isFadeIn )
    			{
    				instance.m_TargetScreenOverlayColor = new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 );
    				SetScreenOverlayColor( newScreenOverlayColor );
    			} else {
    				instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
    				SetScreenOverlayColor( new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 ) );
    			}
     
    			instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;	
    		}
    	}
     
    	/// <summary>
    	/// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent, after a delay.
    	/// </summary>
    	/// <param name='newScreenOverlayColor'>
    	/// New screen overlay color.
    	/// </param>
    	/// <param name='fadeDuration'>
    	/// Fade duration.
    	/// </param>
    	/// <param name='fadeDelay'>
    	/// Fade delay.
    	/// </param>
    	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay )
    	{
    		if (fadeDuration <= 0.0f)		
    		{
    			SetScreenOverlayColor(newScreenOverlayColor);
    		}
    		else					
    		{
    			instance.m_FadeDelay = Time.time + fadeDelay;			
     
    			if( isFadeIn )
    			{
    				instance.m_TargetScreenOverlayColor = new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 );
    				SetScreenOverlayColor( newScreenOverlayColor );
    			} else {
    				instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
    				SetScreenOverlayColor( new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 ) );
    			}
     
    			instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
    		}
    	}
     
    	/// <summary>
    	/// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent, after a delay, with Action OnFadeFinish.
    	/// </summary>
    	/// <param name='newScreenOverlayColor'>
    	/// New screen overlay color.
    	/// </param>
    	/// <param name='fadeDuration'>
    	/// Fade duration.
    	/// </param>
    	/// <param name='fadeDelay'>
    	/// Fade delay.
    	/// </param>
    	/// <param name='OnFadeFinish'>
    	/// On fade finish, doWork().
    	/// </param>
    	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay, Action OnFadeFinish )
    	{
    		if (fadeDuration <= 0.0f)		
    		{
    			SetScreenOverlayColor(newScreenOverlayColor);
    		}
    		else					
    		{
    			instance.m_OnFadeFinish = OnFadeFinish;
    			instance.m_FadeDelay = Time.time + fadeDelay;
     
    			if( isFadeIn )
    			{
    				instance.m_TargetScreenOverlayColor = new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 );
    				SetScreenOverlayColor( newScreenOverlayColor );
    			} else {
    				instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
    				SetScreenOverlayColor( new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 ) );
    			}
    			instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
    		}
    	}
     
    	void Die()
    	{
    		mInstance = null;
    		Destroy(gameObject);
    	}
     
    	void OnApplicationQuit()
    	{
    		mInstance = null;
    	}
    }C# Introduction Author: ratmat2000 
    Extended version written in C# (put it into "Standard Assets/Scripts" to use it from within JavaScript files). 
    This version is identical to the Boo version but written in C#. 
    Source (CameraFade.cs) using UnityEngine;
     
    public class CameraFade : MonoBehaviour
    {    
        // ---------------------------------------- 
        // 	PUBLIC FIELDS
        // ----------------------------------------
     
        // Alpha start value
        public float startAlpha = 1;
     
        // Texture used for fading
        public Texture2D fadeTexture;
     
        // Default time a fade takes in seconds
        public float fadeDuration = 2;
     
        // Depth of the gui element
        public int guiDepth = -1000;
     
        // Fade into scene at start
        public bool fadeIntoScene = true;
     
        // ---------------------------------------- 
        // 	PRIVATE FIELDS
        // ----------------------------------------
     
        // Current alpha of the texture
        private float currentAlpha = 1;
     
        // Current duration of the fade
        private float currentDuration;
     
        // Direction of the fade
        private int fadeDirection = -1;
     
        // Fade alpha to
        private float targetAlpha = 0;
     
        // Alpha difference
        private float alphaDifference = 0;
     
        // Style for background tiling
        private GUIStyle backgroundStyle = new GUIStyle();
        private Texture2D dummyTex;
     
        // Color object for alpha setting
        Color alphaColor = new Color();
     
        // ---------------------------------------- 
        // 	FADE METHODS
        // ----------------------------------------
     
        public void FadeIn(float duration, float to)
        {
            // Set fade duration
            currentDuration = duration;
            // Set target alpha
            targetAlpha = to;
            // Difference
            alphaDifference = Mathf.Clamp01(currentAlpha - targetAlpha);
            // Set direction to Fade in
            fadeDirection = -1;
        }
     
        public void FadeIn()
        {
            FadeIn(fadeDuration, 0);
        }
     
        public void FadeIn(float duration)
        {
            FadeIn(duration, 0);
        }
     
        public void FadeOut(float duration, float to)
        {
            // Set fade duration
            currentDuration = duration;
            // Set target alpha
            targetAlpha = to;
            // Difference
            alphaDifference = Mathf.Clamp01(targetAlpha - currentAlpha);
            // Set direction to fade out
            fadeDirection = 1;
        }
     
        public void FadeOut()
        {
            FadeOut(fadeDuration, 1);
        }    
     
        public void FadeOut(float duration)
        {
            FadeOut(duration, 1);
        }
     
        // ---------------------------------------- 
        // 	STATIC FADING FOR MAIN CAMERA
        // ----------------------------------------
     
        public static void FadeInMain(float duration, float to)
        {
            GetInstance().FadeIn(duration, to);
        }
     
        public static void FadeInMain()
        {
            GetInstance().FadeIn();
        }
     
        public static void FadeInMain(float duration)
        {
            GetInstance().FadeIn(duration);
        }
     
        public static void FadeOutMain(float duration, float to)
        {
            GetInstance().FadeOut(duration, to);
        }
     
        public static void FadeOutMain()
        {
            GetInstance().FadeOut();
        }
     
        public static void FadeOutMain(float duration)
        {
            GetInstance().FadeOut(duration);
        }
     
        // Get script fom Camera
        public static FadeInOut GetInstance()
        {
        	// Get Script
            FadeInOut fader = (FadeInOut)Camera.main.GetComponent("FadeInOut");
            // Check if script exists
            if (fader == null) 
            {
                Debug.LogError("No FadeInOut attached to the main camera.");
            }    
            return fader;
    	}
     
        // ---------------------------------------- 
        // 	SCENE FADEIN
        // ----------------------------------------
     
        public void Start()
        {
        	Debug.Log("Starting FadeInOut");
     
            dummyTex = new Texture2D(1,1);
            dummyTex.SetPixel(0,0,Color.clear);
            backgroundStyle.normal.background = fadeTexture;
            currentAlpha = startAlpha;
            if (fadeIntoScene)
            {
                FadeIn();
            }
        }
     
        // ---------------------------------------- 
        // 	FADING METHOD
        // ----------------------------------------
     
        public void OnGUI()
        {   
            // Fade alpha if active
            if ((fadeDirection == -1 && currentAlpha > targetAlpha) ||
                (fadeDirection == 1 && currentAlpha < targetAlpha))
            {
                // Advance fade by fraction of full fade time
                currentAlpha += (fadeDirection * alphaDifference) * (Time.deltaTime / currentDuration);
                // Clamp to 0-1
                currentAlpha = Mathf.Clamp01(currentAlpha);
            }
     
            // Draw only if not transculent
            if (currentAlpha > 0)
            {
                // Draw texture at depth
                alphaColor.a = currentAlpha;
                GUI.color = alphaColor;
                GUI.depth = guiDepth;
                GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), dummyTex, backgroundStyle);
            }
        }
    }
    The following bugs have been identified in the code segment above 
    LINE 133: FadeInOut should read CameraFade 
    LINE 136: Replace FadeInOut occurances with CameraFade 
    Also when I was trying to use it, if I didn't FadeIn on start the value for currentAlpha would end up NaN. I had to do some rearranging, but when I was done there were conditions that would cause currentAlpha to equal the targetAlpha when I issued a FadeIn command. I corrected this by placing the following code in the FadeIn method and an equivalent one in the FadeOut. 
    
    
        	//Check to see if currentAlpha is set to 1.  It will need to be 1 to fade properly
        	if (currentAlpha != 1){
              currentAlpha = 1;	
        }I would replace this code with mine, but I've already changed my so much I am hesitant to do so, I'll leave that exercise to someone else. 
}
