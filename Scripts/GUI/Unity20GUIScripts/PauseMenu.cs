/*************************
 * Original url: http://wiki.unity3d.com/index.php/PauseMenu
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/PauseMenu.cs
 * File based on original modification date of: 21 November 2015, at 00:53. 
 *
 * Overview 
 *   
 * Setup 
 *   
 * How It Works 
 *   
 * Javascript Version 
 *   
 * C# version 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    Overview A pause menu with multiple screens for credits and options (audio and visual, system info, stats). Doubles as the start menu and is invoked when ESC is hit. This was used for all my Unity web and widget minigames on Fugu Games. This was not originally run on iOS - if you get the type of errors that you also get from adding #pragma strict then add type declarations where necessary - referring to the C# version (added by another contributor) may help. Also, the C# may run better on iOS (and Android). A sample project with the Javascript version is on the Asset Store (currently, $2) and another version is on github. 
    Setup Attach this to the main camera 
    If a SepiaToneFilter is attached to the main camera, it will be activated on pause. 
    Sepia filter (and image effects in general) are a Pro-only feature. If you have Unity Indie, comment out the sepiaFilter assignment. 
    You need to assign a material to the script's Mat slot for the FPS graph to display - I usually assign an all-white VertexLit material. 
    How It Works Sets Time.timeScale to zero - this pauses anything that depends on time (animation, physics, any Update code that operates on the delta time). For any code that isn't automatically paused, try adding some code that checks if Time.timeScale is zero. 
    Pauses audio by setting AudioListener.pause to true. 
    A more detailed explanation of the code is on http://drupal.technicat.com/games/unity/unitygui/pausemenu.html 
    Direct questions and discussion to http://forum.unity3d.com/threads/74127-UnifyCommunity-PauseMenu 
    Various versions of this are used in projects from Technicat, LLC on the Asset Store. 
    Javascript Version var skin:GUISkin;
     
    private var gldepth = -0.5;
    private var startTime = 0.1;
     
    var mat:Material;
     
    private var tris = 0;
    private var verts = 0;
    private var savedTimeScale:float;
    private var pauseFilter;
     
    private var showfps:boolean;
    private var showtris:boolean;
    private var showvtx:boolean;
    private var showfpsgraph:boolean;
     
    var lowFPSColor = Color.red;
    var highFPSColor = Color.green;
     
    var lowFPS = 30;
    var highFPS = 50;
     
    var start:GameObject;
     
    var url = "unity.html";
     
    var statColor:Color = Color.yellow;
     
    var credits:String[]=[
    	"A Fugu Games Production",
    	"Programming by Phil Chu",
    	"Fugu logo by Shane Nakamura Designs",
    	"Copyright (c) 2007-2008 Technicat, LLC"] ;
    var crediticons:Texture[];
     
    enum Page {
    	None,Main,Options,Credits
    }
     
    private var currentPage:Page;
     
    private var fpsarray:int[];
    private var fps:float;
     
    function Start() {
    	fpsarray = new int[Screen.width];
    	Time.timeScale = 1.0;
    	pauseFilter = Camera.main.GetComponent(SepiaToneEffect);
    	PauseGame();
    }
     
    function OnPostRender() {
    	if (showfpsgraph && mat != null) {
    		GL.PushMatrix ();
    		GL.LoadPixelMatrix();
    		for (var i = 0; i < mat.passCount; ++i)
    		{
    			mat.SetPass(i);
    			GL.Begin( GL.LINES );
    			for (var x=0; x<fpsarray.length; ++x) {
    				GL.Vertex3(x,fpsarray[x],gldepth);
    			}
    		GL.End();
    		}
    		GL.PopMatrix();
    		ScrollFPS();
    	}
    }
     
    function ScrollFPS() {
    	for (var x=1; x<fpsarray.length; ++x) {
    		fpsarray[x-1]=fpsarray[x];
    	}
    	if (fps < 1000) {
    		fpsarray[fpsarray.length-1]=fps;
    	}
    }
     
    static function IsDashboard() {
    	return Application.platform == RuntimePlatform.OSXDashboardPlayer;
    }
     
    static function IsBrowser() {
    	return (Application.platform == RuntimePlatform.WindowsWebPlayer ||
    		Application.platform == RuntimePlatform.OSXWebPlayer);
    }
     
    function LateUpdate () {
    	if (showfps || showfpsgraph) {
    		FPSUpdate();
    	}
    	if (Input.GetKeyDown("escape")) {
    		switch (currentPage) {
    			case Page.None: PauseGame(); break;
    			case Page.Main: if (!IsBeginning()) UnPauseGame(); break;
    			default: currentPage = Page.Main;
    		}
    	}
    }
     
    function OnGUI () {
    	if (skin != null) {
    		GUI.skin = skin;
    	}
    	ShowStatNums();
    	ShowLegal();
    	if (IsGamePaused()) {
    		GUI.color = statColor;
    		switch (currentPage) {
    			case Page.Main: PauseMenu(); break;
    			case Page.Options: ShowToolbar(); break;
    			case Page.Credits: ShowCredits(); break;
    		}
    	}	
    }
     
    function ShowLegal() {
    	if (!IsLegal()) {
    		GUI.Label(Rect(Screen.width-100,Screen.height-20,90,20),
    		"fugugames.com");
    	}
    }
     
    function IsLegal() {
    	return !IsBrowser() || 
    	Application.absoluteURL.StartsWith("http://www.fugugames.com/") ||
    	Application.absoluteURL.StartsWith("http://fugugames.com/");
    }
     
    private var toolbarInt:int=0;
    private var toolbarStrings: String[]= ["Audio","Graphics","Stats","System"];
     
    function ShowToolbar() {
    	BeginPage(300,300);
    	toolbarInt = GUILayout.Toolbar (toolbarInt, toolbarStrings);
    	switch (toolbarInt) {
    		case 0: VolumeControl(); break;
    		case 3: ShowDevice(); break;
    		case 1: Qualities(); QualityControl(); break;
    		case 2: StatControl(); break;
    	}
    	EndPage();
    }
     
    function ShowCredits() {
    	BeginPage(300,300);
    	for (var credit in credits) {
    		GUILayout.Label(credit);
    	}
    	for (var credit in crediticons) {
    		GUILayout.Label(credit);
    	}
    	EndPage();
    }
     
    function ShowBackButton() {
    	if (GUI.Button(Rect(20,Screen.height-50,50,20),"Back")) {
    		currentPage = Page.Main;
    	}
    }
     
     
    function ShowDevice() {
    	GUILayout.Label ("Unity player version "+Application.unityVersion);
    	GUILayout.Label("Graphics: "+SystemInfo.graphicsDeviceName+" "+
    	SystemInfo.graphicsMemorySize+"MB\n"+
    	SystemInfo.graphicsDeviceVersion+"\n"+
    	SystemInfo.graphicsDeviceVendor);
    	GUILayout.Label("Shadows: "+SystemInfo.supportsShadows);
    	GUILayout.Label("Image Effects: "+SystemInfo.supportsImageEffects);
    	GUILayout.Label("Render Textures: "+SystemInfo.supportsRenderTextures);
    }
     
    function Qualities() {
            GUILayout.Label(QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }
     
    function QualityControl() {
    	GUILayout.BeginHorizontal();
    	if (GUILayout.Button("Decrease")) {
    		QualitySettings.DecreaseLevel();
    	}
    	if (GUILayout.Button("Increase")) {
    		QualitySettings.IncreaseLevel();
    	}
    	GUILayout.EndHorizontal();
    }
     
    function VolumeControl() {
    	GUILayout.Label("Volume");
    	AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume,0.0,1.0);
    }
     
    function StatControl() {
    	GUILayout.BeginHorizontal();
    	showfps = GUILayout.Toggle(showfps,"FPS");
    	showtris = GUILayout.Toggle(showtris,"Triangles");
    	showvtx = GUILayout.Toggle(showvtx,"Vertices");
    	showfpsgraph = GUILayout.Toggle(showfpsgraph,"FPS Graph");
    	GUILayout.EndHorizontal();
    }
     
    function FPSUpdate() {
    	var delta = Time.smoothDeltaTime;
    		if (!IsGamePaused() && delta !=0.0) {
    			fps = 1 / delta;
    		}
    }
     
    function ShowStatNums() {
    	GUILayout.BeginArea(Rect(Screen.width-100,10,100,200));
    	if (showfps) {
    		var fpsString= fps.ToString ("#,##0 fps");
    		GUI.color = Color.Lerp(lowFPSColor, highFPSColor,(fps-lowFPS)/(highFPS-lowFPS));
    		GUILayout.Label (fpsString);
    	}
    	if (showtris || showvtx) {
    		GetObjectStats();
    		GUI.color = statColor;
    	}
    	if (showtris) {
    		GUILayout.Label (tris+"tri");
    	}
    	if (showvtx) {
    		GUILayout.Label (verts+"vtx");
    	}
    	GUILayout.EndArea();
    }
     
    function BeginPage(width,height) {
    	GUILayout.BeginArea(Rect((Screen.width-width)/2,(Screen.height-height)/2,width,height));
    }
     
    function EndPage() {
    	GUILayout.EndArea();
    	if (currentPage != Page.Main) {
    		ShowBackButton();
    	}
    }
     
    function IsBeginning() {
    	return Time.time < startTime;
    }
     
     
    function PauseMenu() {
    	BeginPage(200,200);
    	if (GUILayout.Button (IsBeginning() ? "Play" : "Continue")) {
    		UnPauseGame();
     
    	}
    	if (GUILayout.Button ("Options")) {
    		currentPage = Page.Options;
    	}
    	if (GUILayout.Button ("Credits")) {
    		currentPage = Page.Credits;
    	}
    	if (IsBrowser() && !IsBeginning() && GUILayout.Button ("Restart")) {
    		Application.OpenURL(url);
    	}
    	EndPage();
    }
     
    function GetObjectStats() {
    	verts = 0;
    	tris = 0;
    	var ob = FindObjectsOfType(GameObject);
    	for (var obj in ob) {
    		GetObjectStats(obj);
    	}
    }
     
    function GetObjectStats(object) {
    	var filters : Component[];
    	filters = object.GetComponentsInChildren(MeshFilter);
    	for( var f : MeshFilter in filters )
    	{
        	tris += f.sharedMesh.triangles.Length/3;
      		verts += f.sharedMesh.vertexCount;
    	}
    }
     
    function PauseGame() {
    	savedTimeScale = Time.timeScale;
    	Time.timeScale = 0;
    	AudioListener.pause = true;
    	if (pauseFilter) pauseFilter.enabled = true;
    	currentPage = Page.Main;
    }
     
    function UnPauseGame() {
    	Time.timeScale = savedTimeScale;
    	AudioListener.pause = false;
    	if (pauseFilter) pauseFilter.enabled = false;
    	currentPage = Page.None;
    	if (IsBeginning() && start != null) {
    		start.active = true;
    	}
    }
     
    function IsGamePaused() {
    	return Time.timeScale==0;
    }
     
    function OnApplicationPause(pause:boolean) {
    	if (IsGamePaused()) {
    		AudioListener.pause = true;
    	}
    }C# version using UnityEngine;
    using System.Collections;
     
    public class PauseMenu : MonoBehaviour
    {
     
    	public GUISkin skin;
     
    	private float gldepth = -0.5f;
    	private float startTime = 0.1f;
     
    	public Material mat;
     
    	private long tris = 0;
    	private long verts = 0;
    	private float savedTimeScale;
     
    	private bool showfps;
    	private bool showtris;
    	private bool showvtx;
    	private bool showfpsgraph;
     
    	public Color lowFPSColor = Color.red;
    	public Color highFPSColor = Color.green;
     
    	public int lowFPS = 30;
    	public int highFPS = 50;
     
    	public GameObject start;
     
    	public string url = "unity.html";
     
    	public Color statColor = Color.yellow;
     
    	public string[] credits= {
    		"A Fugu Games Production",
    		"Programming by Phil Chu",
    		"Fugu logo by Shane Nakamura Designs",
    		"Copyright (c) 2007-2008 Technicat, LLC"} ;
    	public Texture[] crediticons;
     
    	public enum Page {
    		None,Main,Options,Credits
    	}
     
    	private Page currentPage;
     
    	private float[] fpsarray;
    	private float fps;
     
    	private int toolbarInt = 0;
    	private string[]  toolbarstrings =  {"Audio","Graphics","Stats","System"};
     
     
    	void Start() {
    		fpsarray = new float[Screen.width];
    		Time.timeScale = 1;
    		//Comment the next line out if you don't want it to pause right at the beginning
    		PauseGame();
    	}
     
    	void OnPostRender() {
    		if (showfpsgraph && mat != null) {
    			GL.PushMatrix ();
    			GL.LoadPixelMatrix();
    			for (var i = 0; i < mat.passCount; ++i)
    			{
    				mat.SetPass(i);
    				GL.Begin( GL.LINES );
    				for (int x=0; x < fpsarray.Length; ++x) {
    					GL.Vertex3(x, fpsarray[x], gldepth);
    				}
    				GL.End();
    			}
    			GL.PopMatrix();
    			ScrollFPS();
    		}
    	}
     
    	void ScrollFPS() {
    		for (int x = 1; x < fpsarray.Length; ++x) {
    			fpsarray[x-1]=fpsarray[x];
    		}
    		if (fps < 1000) {
    			fpsarray[fpsarray.Length - 1]=fps;
    		}
    	}
     
    	static bool IsDashboard() {
    		return Application.platform == RuntimePlatform.OSXDashboardPlayer;
    	}
     
    	static bool IsBrowser() {
    		return (Application.platform == RuntimePlatform.WindowsWebPlayer ||
    			Application.platform == RuntimePlatform.OSXWebPlayer);
    	}
     
    	void LateUpdate () {
    		if (showfps || showfpsgraph) {
    			FPSUpdate();
    		}
     
    		if (Input.GetKeyDown("escape")) 
    		{
    			switch (currentPage) 
    			{
    			case Page.None: 
    				PauseGame(); 
    				break;
     
    			case Page.Main: 
    				if (!IsBeginning()) 
    					UnPauseGame(); 
    				break;
     
    			default: 
    				currentPage = Page.Main;
    				break;
    			}
    		}
    	}
     
    	void OnGUI () {
    		if (skin != null) {
    			GUI.skin = skin;
    		}
    		ShowStatNums();
    		ShowLegal();
    		if (IsGamePaused()) {
    			GUI.color = statColor;
    			switch (currentPage) {
    			case Page.Main: MainPauseMenu(); break;
    			case Page.Options: ShowToolbar(); break;
    			case Page.Credits: ShowCredits(); break;
    			}
    		}   
    	}
     
    	void ShowLegal() {
    		if (!IsLegal()) {
    			GUI.Label(new Rect(Screen.width-100,Screen.height-20,90,20),
    				"jdonavan.com");
    		}
    	}
     
    	bool IsLegal() {
    		return !IsBrowser() || 
    			Application.absoluteURL.StartsWith("http://www.jdonavan.com/") ||
    			Application.absoluteURL.StartsWith("http://jdonavan.com/");
     
    	}
     
    	void ShowToolbar() {
    		BeginPage(300,300);
    		toolbarInt = GUILayout.Toolbar (toolbarInt, toolbarstrings);
    		switch (toolbarInt) {
    		case 0: VolumeControl(); break;
    		case 3: ShowDevice(); break;
    		case 1: Qualities(); QualityControl(); break;
    		case 2: StatControl(); break;
    		}
    		EndPage();
    	}
     
    	void ShowCredits() {
    		BeginPage(300,300);
    		foreach(string credit in credits) {
    			GUILayout.Label(credit);
    		}
    		foreach( Texture credit in crediticons) {
    			GUILayout.Label(credit);
    		}
    		EndPage();
    	}
     
    	void ShowBackButton() {
    		if (GUI.Button(new Rect(20, Screen.height - 50, 50, 20),"Back")) {
    			currentPage = Page.Main;
    		}
    	}
     
    	void ShowDevice() {
    		GUILayout.Label("Unity player version "+Application.unityVersion);
    		GUILayout.Label("Graphics: "+SystemInfo.graphicsDeviceName+" "+
    			SystemInfo.graphicsMemorySize+"MB\n"+
    			SystemInfo.graphicsDeviceVersion+"\n"+
    			SystemInfo.graphicsDeviceVendor);
    		GUILayout.Label("Shadows: "+SystemInfo.supportsShadows);
    		GUILayout.Label("Image Effects: "+SystemInfo.supportsImageEffects);
    		GUILayout.Label("Render Textures: "+SystemInfo.supportsRenderTextures);
    	}
     
    	void Qualities() {
    		switch (QualitySettings.GetQualityLevel()) 
    		{
    		case 0:
    			GUILayout.Label("Fastest");
    			break;
    		case 1:
    			GUILayout.Label("Fast");
    			break;
    		case 2:
    			GUILayout.Label("Simple");
    			break;
    		case 3:
    			GUILayout.Label("Good");
    			break;
    		case 4:
    			GUILayout.Label("Beautiful");
    			break;
    		default:
    			GUILayout.Label("Fantastic");
    			break;
    		}
    	}
     
    	void QualityControl() {
    		GUILayout.BeginHorizontal();
    		if (GUILayout.Button("Decrease")) {
    			QualitySettings.DecreaseLevel();
    		}
    		if (GUILayout.Button("Increase")) {
    			QualitySettings.IncreaseLevel();
    		}
    		GUILayout.EndHorizontal();
    	}
     
    	void VolumeControl() {
    		GUILayout.Label("Volume");
    		AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume, 0, 1);
    	}
     
    	void StatControl() {
    		GUILayout.BeginHorizontal();
    		showfps = GUILayout.Toggle(showfps,"FPS");
    		showtris = GUILayout.Toggle(showtris,"Triangles");
    		showvtx = GUILayout.Toggle(showvtx,"Vertices");
    		showfpsgraph = GUILayout.Toggle(showfpsgraph,"FPS Graph");
    		GUILayout.EndHorizontal();
    	}
     
    	void FPSUpdate() {
    		float delta = Time.smoothDeltaTime;
    		if (!IsGamePaused() && delta !=0.0) {
    			fps = 1 / delta;
    		}
    	}
     
    	void ShowStatNums() {
    		GUILayout.BeginArea( new Rect(Screen.width - 100, 10, 100, 200));
    		if (showfps) {
    			string fpsstring= fps.ToString ("#,##0 fps");
    			GUI.color = Color.Lerp(lowFPSColor, highFPSColor,(fps-lowFPS)/(highFPS-lowFPS));
    			GUILayout.Label (fpsstring);
    		}
    		if (showtris || showvtx) {
    			GetObjectStats();
    			GUI.color = statColor;
    		}
    		if (showtris) {
    			GUILayout.Label (tris+"tri");
    		}
    		if (showvtx) {
    			GUILayout.Label (verts+"vtx");
    		}
    		GUILayout.EndArea();
    	}
     
    	void BeginPage(int width, int height) {
    		GUILayout.BeginArea( new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
    	}
     
    	void EndPage() {
    		GUILayout.EndArea();
    		if (currentPage != Page.Main) {
    			ShowBackButton();
    		}
    	}
     
    	bool IsBeginning() {
    		return (Time.time < startTime);
    	}
     
     
    	void MainPauseMenu() {
    		BeginPage(200,200);
    		if (GUILayout.Button (IsBeginning() ? "Play" : "Continue")) {
    			UnPauseGame();
     
    		}
    		if (GUILayout.Button ("Options")) {
    			currentPage = Page.Options;
    		}
    		if (GUILayout.Button ("Credits")) {
    			currentPage = Page.Credits;
    		}
    		if (IsBrowser() && !IsBeginning() && GUILayout.Button ("Restart")) {
    			Application.OpenURL(url);
    		}
    		EndPage();
    	}
     
    	void GetObjectStats() {
    		verts = 0;
    		tris = 0;
    		GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
    		foreach (GameObject obj in ob) {
    			GetObjectStats(obj);
    		}
    	}
     
    	void GetObjectStats(GameObject obj) {
    		Component[] filters;
    		filters = obj.GetComponentsInChildren<MeshFilter>();
    		foreach( MeshFilter f  in filters )
    		{
    			tris += f.sharedMesh.triangles.Length/3;
    			verts += f.sharedMesh.vertexCount;
    		}
    	}
     
    	void PauseGame() {
    		savedTimeScale = Time.timeScale;
    		Time.timeScale = 0;
    		AudioListener.pause = true;
     
    		currentPage = Page.Main;
    	}
     
    	void UnPauseGame() {
    		Time.timeScale = savedTimeScale;
    		AudioListener.pause = false;
    		currentPage = Page.None;
     
    		if (IsBeginning() && start != null) {
    			start.SetActive(true);
    		}
    	}
     
    	bool IsGamePaused() {
    		return (Time.timeScale == 0);
    	}
     
    	void OnApplicationPause(bool pause) {
    		if (IsGamePaused()) {
    			AudioListener.pause = true;
    		}
    	}
    }
}
