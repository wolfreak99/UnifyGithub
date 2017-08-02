/*************************
 * Original url: http://wiki.unity3d.com/index.php/EnumeratedDelegate
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/EnumeratedDelegate.cs
 * File based on original modification date of: 6 April 2012, at 23:50. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.GeneralConcepts
{
    A method to setup a quick and dirty Finite State Machine of sorts by means of generics, delegates and an enum. 
    Contents [hide] 
    1 Description 
    2 Usage 
    3 EnumeratedDelegate class members of interest 
    3.1 Properties 
    3.2 Constructors 
    3.2.1 public EnumeratedDelegate() 
    3.2.2 public EnumeratedDelegate(int intialState) 
    3.2.3 public EnumeratedDelegate(DelegateType[] arrFuncs) 
    3.2.4 public EnumeratedDelegate(DelegateType[] arrFuncs, int intialState) 
    4 Code 
    4.1 EnumeratedDelegate.cs 
    4.2 GUISetup.cs 
    5 Coming Soon 
    
    DescriptionA C# generic class that takes an enum and a delegate as parameters and lightly emulates a finite state machine. Just provide an enum representing the different states, a delegate representing the type of function that all of the states will be represented by, and then one function for each member of the enum, matching the delegate signature. 
    Usage(For this example, I am using a GUI navigation menu system) 
    First create an enum representing the different states (names and naming scheme are irrelevant, it's the order that's important): 
    public enum EMenuMode
    {
    	eMenu_MainMenu,
    	eMenu_Review,
    	eMenu_Preview,
    	eMenu_Teach,
    	eMenu_Practice,
    	eMenu_Apply,
    	eMenu_Play,
    	eMenu_StudentDB
    };
    
    Then create the delegate to which all your state functions match: 
    public delegate void GUIMethod();
    
    Next, declare the member variable representing the EnumeratedDelegate itself: 
    protected EnumeratedDelegate<EMenuMode, GUIMethod> m_MenuEnum = null;
    
    Meow, create all the functions that will represent the different states, making sure that they all have the same signature: (Meow, what is so damn funny?!) 
    void DoMainMenuGUI()
    {
    	//Do stuff
    }
     
    void DoReviewGUI()
    {
    	//Do stuff
    }
     
    void DoPreviewGUI()
    {
    	//Do stuff
    }
     
    void DoTeachGUI()
    {
    	//Do stuff
    }
     
    void DoPracticeGUI()
    {
    	//Do stuff
    }
     
    void DoApplyGUI()
    {
    	//Do stuff
    }
     
    void DoPlayGUI()
    {
    	//Do stuff
    }
     
    void DoStudentDBGUI()
    {
    	//Do stuff
    }
    
    At this point, you only need to instantiate the EnumeratedDelegate object: 
    void Awake()
    {
    	//Create the enumerated delegate and pass in the correct functions,
    	//in the exact order specified by the enum
    	m_MenuEnum = new EnumeratedDelegate<EMenuMode, GUIMethod>
    	(
    		new GUIMethod[]
    		{
    			DoMainMenuGUI,	//eMenu_MainMenu,
    			DoReviewGUI,    //eMenu_Review,
    			DoPreviewGUI,   //eMenu_Preview,
    			DoTeachGUI,     //eMenu_Teach,
    			DoPracticeGUI,  //eMenu_Practice,
    			DoApplyGUI,     //eMenu_Apply,
    			DoPlayGUI,      //eMenu_Play,
    			DoStudentDBGUI  //eMenu_StudentDB
    		}
    	);
    }Note there are 4 different constructors for EnumeratedDelegate that take either the functions and/or the initial state. Initial state is the very first in the enum by default. 
    
    
    Last but definitely not least, you must actually call the current state function of the EnumeratedDelegate from whereever you need it (this is usually in Update() or OnGUI()): 
    void OnGUI()
    {
    	//Call the current GUI (enumerated delegate) function
    	m_MenuEnum.CurrentFunction();
    }
    
    You can change the current state of the EnumeratedDelegate through its CurrentMode property. 
    m_MenuEnum.CurrentMode = EMenuMode.eMenu_MainMenu;EnumeratedDelegate class members of interestPropertiesProperty Get/Set Description 
    Count Get Returns the number of states represented by the enum. 
    CurrentFunction Get Returns the function delegate of the current state. 
    CurrentMode Get/Set Gets and sets the current mode. Takes a value from within the enum as only correct input. 
    Functions Get/Set Gets and sets the array of functions representing the states enumerated in the enum. Takes delegate[] as the value type. 
    
    
    
    Constructorspublic EnumeratedDelegate()Parameter Description 
    (None) (N/A) 
    
    Default Constructor. Reads in the names of the enum and stores them. Sets CurrentMode to the first value declared in the enum (technically whatever value is assigned to zero). 
    Note: The state functions are still null at this point, so you will still need to set the Functions property like so: 
    m_MenuEnum.Functions = new GUIMethod[]
    {
    	DoMainMenuGUI,	//eMenu_MainMenu,
    	DoReviewGUI,    //eMenu_Review,
    	DoPreviewGUI,   //eMenu_Preview,
    	DoTeachGUI,     //eMenu_Teach,
    	DoPracticeGUI,  //eMenu_Practice,
    	DoApplyGUI,     //eMenu_Apply,
    	DoPlayGUI,      //eMenu_Play,
    	DoStudentDBGUI  //eMenu_StudentDB
    };
    
    public EnumeratedDelegate(int intialState)Parameter Description 
    initialState The initial value of CurrentMode. 
    
    Reads in the names of the enum and stores them. Sets CurrentMode to the enum value specified by initialState (technically whatever value is assigned to initialState). 
    Note: The state functions are still null at this point. See note above. 
    
    
    public EnumeratedDelegate(DelegateType[] arrFuncs)Parameter Description 
    arrFuncs The state functions representing the enum values. Sets via Functions property. 
    
    Reads in the names of the enum and stores them. Sets CurrentMode to the first value declared in the enum (technically whatever value is assigned to zero). Sets the state functions specified by arrFuncs. 
    
    
    public EnumeratedDelegate(DelegateType[] arrFuncs, int intialState)Parameter Description 
    arrFuncs The state functions representing the enum values. Sets via Functions property. 
    initialState The initial value of CurrentMode. 
    
    Reads in the names of the enum and stores them. Sets CurrentMode to the enum value specified by initialState (technically whatever value is assigned to initialState). Sets the state functions specified by arrFuncs. 
    CodeEnumeratedDelegate.csusing UnityEngine;
    using System;
    using System.Collections;
     
    //Needs to be included because conversion to an int doesn't work any other way as far as I can tell,
    //and apparently it can't perform ENUM == ENUM or ENUM == int in generic functions
    //If you can solve, please feel free to update the Unify page for this class
    using System.Globalization;
     
    public class EnumeratedDelegate<ENUM, DelegateType> 
    						where ENUM : struct, IConvertible, IComparable, IFormattable
    						where DelegateType : class
    {
    	private ENUM			m_eCurrentMode;			//The current enum value
    	private DelegateType	m_fnCurrentFunction;	//The current function being called
    	private string[]		m_arrNames;				//The names of the enum values
    	private DelegateType[]	m_arrFunctions;			//The functions corresponding to the enum values
     
    	//////////////////////////////////////////////////////////////////////////////////////////////
     
    	public int Count
    	{
    		get {	return m_arrNames.Length;	}
    	}
     
    	public DelegateType		CurrentFunction	//Get the current function
    	{
    		get	{	return m_fnCurrentFunction;	}
    	}
     
    	public DelegateType[]	Functions		//Set the functions safely
    	{
    		get	{	return m_arrFunctions;		}
     
    		set
    		{
    			if (m_arrNames == null || m_arrNames.Length == 0)
    				throw new UnityException("Must set enum first!");
    			if (value.Length == 0)
    				throw new UnityException("Empty arrays not allowed!");
    			if (value.Length != m_arrNames.Length)
    				throw new UnityException("Must have same number of functions as enum values!");
     
    			m_arrFunctions = value;
    		}
    	}
     
    	public ENUM				CurrentMode		//Get and set the current mode. When setting, change the function delegate appropriately
    	{
    		get	{	return m_eCurrentMode;		}
     
    		set
    		{
    			//Make sure that the value being passed in even corresponds to an existing enum value
    			if (!Enum.IsDefined(typeof(ENUM), value))
    				throw new UnityException("'" + value + "' is not a valid member of the enum '" + typeof(ENUM).Name + "'!");
     
    			for (int i = 0; i < this.Count; i++)	
    			{
    				//TODO: Check to see if we should try and set the index by name instead and avoid the CultureInfo
    				//print(value.ToString());
     
    				//Must convert because conversion to an int doesn't work any other way as far as I can tell,
    				//and apparently it can't perform ENUM == ENUM or ENUM == int in generic functions
    				int nValue = value.ToInt32(new CultureInfo("en-us"));
     
    				if (i == nValue)
    				{
    					SetFunctionByIndex(i);
    					m_eCurrentMode = value;
    					return;
    				}
    			}
     
    			throw new UnityException("Couldn't find the enum somehow!");
    		}
    	}
     
    	//////////////////////////////////////////////////////////////////////////////////////////////
    	//////////////////////////////////////////////////////////////////////////////////////////////
     
    	static EnumeratedDelegate()
    	{
    		//Assert that this class's first generic type is an enum
    		if (!typeof(ENUM).IsEnum)
    			throw new UnityException("First argument of EnumeratedDelegate must be an enum!");
     
    		//Debug.Log(typeof(DelegateType).);
    		//Debug.Log(typeof(Delegate).Name);
     
    		//Assert that this class's second generic type is a delegate
    		//if (!(typeof(DelegateType).Name.Equals(typeof(Delegate).Name)))
    		//	throw new UnityException("Second argument of EnumeratedDelegate must be a Delegate!");
    	}
     
    	public EnumeratedDelegate()
    	{
    		m_arrNames	= Enum.GetNames(typeof(ENUM));		
    		CurrentMode	= (ENUM)Enum.GetValues(typeof(ENUM)).GetValue(0);
    	}
     
    	public EnumeratedDelegate(int intialState)
    	{
    		m_arrNames	= Enum.GetNames(typeof(ENUM));		
    		CurrentMode	= (ENUM)Enum.ToObject(typeof(ENUM), intialState);
    	}
     
    	public EnumeratedDelegate(DelegateType[] arrFuncs)
    	{
    		m_arrNames	= Enum.GetNames(typeof(ENUM));
    		this.Functions = arrFuncs;
    		CurrentMode	= (ENUM)Enum.GetValues(typeof(ENUM)).GetValue(0);
    	}
     
    	public EnumeratedDelegate(DelegateType[] arrFuncs, int intialState)
    	{
    		m_arrNames	= Enum.GetNames(typeof(ENUM));
    		this.Functions = arrFuncs;
    		CurrentMode	= (ENUM)Enum.ToObject(typeof(ENUM), intialState);
    	}
     
    	void SetFunctionByIndex(int index)
    	{
    		Debug.Log("Setting function to '" + ((m_arrFunctions[index]) as Delegate).Method.ToString() + "'");
    		m_fnCurrentFunction = m_arrFunctions[index];
    	}
     
    	void SetFunctionByName(string name)
    	{
    		int i = 0;
    		foreach (string _name in m_arrNames)
    		{
    			if (_name == name)
    			{
    				Debug.Log("Setting function to '" + name + "'");
    				m_fnCurrentFunction = m_arrFunctions[i];
    				return;
    			}
     
    			i++;
    		}
     
    		throw new UnityException("Couldn't find an enum state representing given name: " + name);
    	}
    }Example usage as a GUI system for several different screens: 
    GUISetup.csusing UnityEngine;
    using System.Collections;
     
    public class GUISetup : MonoBehaviour
    {
    	/////////////////////////////////////////////////////
    	//The enumatered delegate setup
    	/////////////////////////////////////////////////////
     
    	//Delegate declaration for EnumeratedDelegate
    	public delegate void GUIMethod();
     
    	//The enum itself
    	public enum EMenuMode
    	{
    		eMenu_MainMenu,
    		eMenu_Review,
    		eMenu_Preview,
    		eMenu_Teach,
    		eMenu_Practice,
    		eMenu_Apply,
    		eMenu_Play,
    		eMenu_StudentDB
    	};
     
    	protected EnumeratedDelegate<EMenuMode, GUIMethod> m_MenuEnum = null;
     
    	//QADS (quick and dirty singleton, not fully
    	//necessary if you don't plan on changing levels) 
    	private static bool mk_bAlreadyCreated = false;
     
    	/////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////
     
    	private GUILayout m_Layout;
     
    	public bool BGFullScreen = true;
    	public Texture2D BGImage;
     
    	private bool m_bMainMenuVert = true;
     
    	// Use this for initialization
    	void Awake()
    	{
    		//If object has been created, but the menu deleted or destroyed,
    		//destroy it
    		if (mk_bAlreadyCreated && m_MenuEnum == null)
    		{
    			Destroy(this);
    			return;
    		}
     
    		//Create the enumerated delegate and pass in the correct functions,
    		//in the exact order specified by the enum
    		m_MenuEnum = new EnumeratedDelegate<EMenuMode, GUIMethod>
    		(
    			new GUIMethod[]
    			{
    				DoMainMenuGUI,	//eMenu_MainMenu,
    				DoReviewGUI,    //eMenu_Review,
    				DoPreviewGUI,   //eMenu_Preview,
    				DoTeachGUI,     //eMenu_Teach,
    				DoPracticeGUI,  //eMenu_Practice,
    				DoApplyGUI,     //eMenu_Apply,
    				DoPlayGUI,      //eMenu_Play,
    				DoStudentDBGUI  //eMenu_StudentDB
    			}
    		);
     
    		//We want this game object to stick around
    		DontDestroyOnLoad(this.gameObject);
     
    		//QADS creation
    		mk_bAlreadyCreated = true;
     
    		PreviewGUI.Init();
    	}
     
    	// Update is called once per frame
    	void Update()
    	{
    	}
     
    	void OnGUI()
    	{
    		int width = this.BGImage.width,
    			height = this.BGImage.height;
     
    		//if (BGFullScreen)
    			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), BGImage);
    		//else
    			//GUI.Box(new Rect(512 - width / 2, 384 - height / 2, width, height), BGImage);
     
    		//Call the current GUI (enumerated delegate) function
    		m_MenuEnum.CurrentFunction();
    	}
     
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
     
    	void DoMainMenuGUI()
    	{
    		DoMainMenuButtons();
     
    		if (GUI.Button(new Rect(10, Screen.height * 0.9f, 100, 100), "Students"))
    		{
    			m_MenuEnum.CurrentMode = EMenuMode.eMenu_StudentDB;
    			Application.LoadLevel("StudentDB");
    		}
    	}
     
    	void OnClickMainMenuButton(EMenuMode eMenuMode, string label)
    	{
    		m_MenuEnum.CurrentMode = eMenuMode;
    		Application.LoadLevel(label);
    	}
     
    	void DoMainMenuButtons()
    	{
    		int ix = 512 - 150, iy = 250, dx = 5, dy = dx, w = 290, h = 30;
    		string [] arrLabels = {"Main Menu", "Review", "Preview", "Teach", "Practice", "Apply", "Play"};
    		int numButtons = arrLabels.Length;
     
    		if (!m_bMainMenuVert)
    		{
    			ix = 212;
    			iy = 20;
    			w = 80;
    			//arrLabels = new string[]{"Main Menu", "Review", "Preview", "Teach", "Practice", "Apply", "Play"};
    		}
    		else
    			numButtons--;
     
    		GUI.Box(new Rect(ix + dx, iy + dy, w + (2 * dx) + (m_bMainMenuVert ? 0 : (numButtons - 1) * (w + dx)), 5 * dy + (m_bMainMenuVert ? (numButtons * (h + dy)) : h + dy)), arrLabels[0/*(int)m_eMenuMode*/]);
     
    		int index = 0;
     
    		foreach (string label in arrLabels)
    		{
    			if (index > 0 || (index == 0 && !m_bMainMenuVert))
    				if (GUI.Button(new Rect(ix + 2 * dx + (m_bMainMenuVert ? 0 : (index - 0) * (w + dx)), iy + 6 * dy + (m_bMainMenuVert ? (index - 1) * (h + dy) : 0), w, h), label))
    					OnClickMainMenuButton((EMenuMode)index, label);
     
    			index++;
    		}
    	}
     
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
     
    	void DoReviewGUI()
    	{
    		m_bMainMenuVert = false;
    		DoMainMenuGUI();
    		ReviewGUI.DoGUI();
    	}
     
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
     
    	void DoPreviewGUI()
    	{
    		m_bMainMenuVert = false;
    		DoMainMenuGUI();
    		PreviewGUI.DoGUI();
    	}
     
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
     
    	void DoTeachGUI()
    	{
    		m_bMainMenuVert = false;
    		DoMainMenuGUI();
    	}
     
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
     
    	void DoPracticeGUI()
    	{
    		m_bMainMenuVert = false;
    		DoMainMenuGUI();
    	}
     
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
     
    	void DoApplyGUI()
    	{
    		m_bMainMenuVert = false;
    		DoMainMenuGUI();
    	}
     
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
    	/////////////////////////////////////////////////////////////////////////////////////////////
     
    	void DoPlayGUI()
    	{
    		m_bMainMenuVert = false;
    		DoMainMenuGUI();
    		PlayGUI.DoGUI();
    	}
     
    	void DoStudentDBGUI()
    	{
    		if (GUI.Button(new Rect(10, Screen.height - 55, 200, 50), "Back To Main Menu"))
    		{
    			//m_bMainMenuVert
    			m_MenuEnum.CurrentMode = EMenuMode.eMenu_MainMenu;
    			Application.LoadLevel("Main Menu");
    		}
    		else
    		{
    			StudentDBGUI.DoGUI();
    		}
    	}
    }Coming SoonState transition in/out support 
    More/better error checking 
}
