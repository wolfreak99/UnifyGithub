// Original url: http://wiki.unity3d.com/index.php/Radar
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/HeadsUpDisplayScripts/Radar.cs
// File based on original modification date of: 24 May 2013, at 20:31. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.HeadsUpDisplayScripts
{
This page needs to be cleaned up. 
Please refer to the Formatting Guidlines. 
Contents [hide] 
1 Radar Script 
1.1 JavaScript 
1.2 CSharp Version 
1.3 Modified to allow many radar locations based on the CSharp Version 
1.4 Updated JavaScript Version 
1.5 Updated CSharp Version 
1.6 Simple CSharp Version 

Radar ScriptAuthor: PsychicParrot (Jeff Murray) http://www.psychicparrotgames.com 
After seeing requests for radars quite frequently in the forums, I decided to post this up for everyone to enjoy and hopefully build on :) 
Don't know if it's the most efficient way to do this or whatever, but it works and it seems to work pretty well! 
USAGE: 
Put this script on a GameObject. Select a texture for a 'blip'. Select a texture for the radar background (radarBG) Set the map center (on screen coordinates) Set the GameObject to use as your center object (usually the player) Make sure that all enemies are tagged with 'Enemy' Watch your radar goodness in your game ;) 
I included the GUI Matrix scaling line just incase ... if you want it to auto-scale (which is a good thing to do!) un-comment it and change the values to your own game screen resolution. 
Enjoy! Jeff. 
JavaScript@script ExecuteInEditMode()
 
// radar! by PsychicParrot, adapted from a Blitz3d script found in the public domain online somewhere ..
//
 
public var blip : Texture;
public var radarBG : Texture;
 
public var centerObject : Transform;
public var mapScale = 0.3;
public var mapCenter = Vector2(50,50);
 
function OnGUI () {
 
	//	GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3(Screen.width / 600.0, Screen.height / 450.0, 1));
 
	// Draw player blip (centerObject)
	bX=centerObject.transform.position.x * mapScale;
    bY=centerObject.transform.position.z * mapScale;
 
    bX=centerObject.transform.position.x * mapScale;
    bY=centerObject.transform.position.z * mapScale;
 
 	GUI.DrawTexture(Rect(mapCenter.x-32,mapCenter.y-32,64,64),radarBG);
 
	// Draw blips for zombies
	DrawBlipsForEnemies();
 
}
 
function DrawBlipsForCows(){
 
	 // Find all game objects with tag Cow
    var gos : GameObject[];
    gos = GameObject.FindGameObjectsWithTag("Cow"); 
 
    var distance = Mathf.Infinity; 
    var position = transform.position; 
 
    // Iterate through them
    for (var go : GameObject in gos)  { 
 
		drawBlip(go,blip2);
 
    }
 
}
 
function drawBlip(go,aTexture){
 
	centerPos=centerObject.position;
	extPos=go.transform.position;
 
	// first we need to get the distance of the enemy from the player
	dist=Vector3.Distance(centerPos,extPos);
	if(dist<=200) { // New - should be more optimal 
	  dx=centerPos.x-extPos.x; // how far to the side of the player is the enemy?
	  dz=centerPos.z-extPos.z; // how far in front or behind the player is the enemy?
 
	  // what's the angle to turn to face the enemy - compensating for the player's turning?
	  deltay=Mathf.Atan2(dx,dz)*Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
 
	  // just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
	  bX=dist*Mathf.Cos(deltay * Mathf.Deg2Rad);
	  bY=dist*Mathf.Sin(deltay * Mathf.Deg2Rad);
 
	  bX=bX*mapScale; // scales down the x-coordinate by half so that the plot stays within our radar
	  bY=bY*mapScale; // scales down the y-coordinate by half so that the plot stays within our radar
 
	// Old if(dist<=200){ // this is the diameter of our largest radar circle
	  GUI.DrawTexture(Rect(mapCenter.x+bX,mapCenter.y+bY,2,2),aTexture);
 
	}
 
}
 
function DrawBlipsForEnemies(){
 
    // Find all game objects tagged Enemy
    var gos : GameObject[];
    gos = GameObject.FindGameObjectsWithTag("Enemy"); 
 
    var distance = Mathf.Infinity; 
    var position = transform.position; 
 
    // Iterate through them and call drawBlip function
    for (var go : GameObject in gos)  { 
 
	drawBlip(go,blip);
 
    }
 
}CSharp VersionAuthor: oPless A C# version of the script above (slightly modified) 
using UnityEngine;
using System.Collections;
 
public class Radar : MonoBehaviour 
{
 
// radar! by oPless from the original javascript by PsychicParrot, 
// who in turn adapted it from a Blitz3d script found in the
// public domain online somewhere ....
//
 
public Texture blip  ;
public Texture radarBG ;
 
	public Transform centerObject ;
	public float mapScale = 0.3f;
	public Vector2 mapCenter = new Vector2(50,50);
	public string tagFilter =  "enemy";
	public float maxDist = 200;
 
	void OnGUI() 
	{
 
		//	GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3(Screen.width / 600.0, Screen.height / 450.0, 1));
 
		// Draw player blip (centerObject)
//		float bX=centerObject.transform.position.x * mapScale;
//	    float bY=centerObject.transform.position.z * mapScale;
 
 
	 	GUI.DrawTexture(new Rect(mapCenter.x-32,mapCenter.y-32,64,64),radarBG);
		DrawBlipsFor(tagFilter);
 
	}
 
	private void DrawBlipsFor(string tagName)
	{
 
		 // Find all game objects with tag 
	    GameObject[] gos = GameObject.FindGameObjectsWithTag(tagName); 
 
	    // Iterate through them
	    foreach (GameObject go in gos)  
	    { 
			drawBlip(go,blip);
	    }
	}
 
	private void drawBlip(GameObject go,Texture aTexture)
	{
		Vector3 centerPos=centerObject.position;
		Vector3 extPos=go.transform.position;
 
		// first we need to get the distance of the enemy from the player
		float dist=Vector3.Distance(centerPos,extPos);
 
		float dx=centerPos.x-extPos.x; // how far to the side of the player is the enemy?
		float dz=centerPos.z-extPos.z; // how far in front or behind the player is the enemy?
 
		// what's the angle to turn to face the enemy - compensating for the player's turning?
		float deltay=Mathf.Atan2(dx,dz)*Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
 
		// just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
		float bX=dist*Mathf.Cos(deltay * Mathf.Deg2Rad);
		float bY=dist*Mathf.Sin(deltay * Mathf.Deg2Rad);
 
		bX=bX*mapScale; // scales down the x-coordinate by half so that the plot stays within our radar
		bY=bY*mapScale; // scales down the y-coordinate by half so that the plot stays within our radar
 
		if(dist<= maxDist)
		{ 
			// this is the diameter of our largest radar circle
		   GUI.DrawTexture(new Rect(mapCenter.x+bX,mapCenter.y+bY,2,2),aTexture);
		}
 
	}
 
 
}

Modified to allow many radar locations based on the CSharp VersionAuthor: Zumwalt 
  public class CLSRadar
  {
    // expanded radar! by zumwalt, alteration of oPress's version which is
    // from the original javascript by PsychicParrot, 
    // who in turn adapted it from a Blitz3d script found in the
    // public domain online somewhere ....
    //
    public Texture blip;
    public Texture radarBG;
    public Transform centerObject;
    public float mapScale = 0.3f;
    public Vector2 mapCenter;
    public float screenWidth=1024.0f;
    public float screenHeight=768.0f;
    public int mapLocation = 0;
    // 0 = upper left
    // 1 = upper right
    // 2 = lower left
    // 3 = lower right
    public string tagFilter = "enemy";
    public float maxDist = 200;
 
    /// <summary>
    /// Switch DrawRadar() to OnGui if you use this directly in a unity C# Script object outside of this class
    /// otherwise, within the OnGui() call DrawRadar() on the class object.
    /// </summary>
    public void DrawRadar()
    {
      //UnityEngine.GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 600.0f, Screen.height / 450.0f, 1.0f));
      // Draw player blip (centerObject)
      switch (mapLocation)
      {
        case 0:
          UnityEngine.GUI.DrawTexture(new Rect(0, 0, radarBG.width,radarBG.height), radarBG);
          mapCenter = new Vector2(radarBG.width / 2, radarBG.height / 2);
          break;
        case 1:
          UnityEngine.GUI.DrawTexture(new Rect(screenWidth - radarBG.width, 0, radarBG.width, radarBG.height), radarBG);
          mapCenter = new Vector2(screenWidth-(radarBG.width / 2), radarBG.height / 2);
          break;
        case 2:
          UnityEngine.GUI.DrawTexture(new Rect(0,screenHeight-radarBG.height, radarBG.width, radarBG.height), radarBG);
          mapCenter = new Vector2(radarBG.width / 2, screenHeight-(radarBG.height / 2));
          break;
        case 3:
          UnityEngine.GUI.DrawTexture(new Rect(screenWidth - radarBG.width, screenHeight - radarBG.height, radarBG.width, radarBG.height), radarBG);
          mapCenter = new Vector2(screenWidth-(radarBG.width / 2),screenHeight-(radarBG.height / 2));
          break;
      }
 
      DrawBlipsFor(tagFilter);
    }
 
    private void DrawBlipsFor(string tagName)
    {
      // Find all game objects with tag 
      GameObject[] gos = GameObject.FindGameObjectsWithTag(tagName);
      // Iterate through them
      foreach (GameObject go in gos)
      {
        drawBlip(go, blip);
      }
    }
 
    private void drawBlip(GameObject go, Texture aTexture)
    {
      Vector3 centerPos = centerObject.position;
      Vector3 extPos = go.transform.position;
 
      // first we need to get the distance of the enemy from the player
      float dist = Vector3.Distance(centerPos, extPos);
 
      float dx = centerPos.x - extPos.x; // how far to the side of the player is the enemy?
      float dz = centerPos.z - extPos.z; // how far in front or behind the player is the enemy?
 
      // what's the angle to turn to face the enemy - compensating for the player's turning?
      float deltay = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
 
      // just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
      float bX = dist * Mathf.Cos(deltay * Mathf.Deg2Rad);
      float bY = dist * Mathf.Sin(deltay * Mathf.Deg2Rad);
 
      bX = bX * mapScale; // scales down the x-coordinate by half so that the plot stays within our radar
      bY = bY * mapScale; // scales down the y-coordinate by half so that the plot stays within our radar
 
      if (dist <= maxDist)
      {
        // this is the diameter of our largest radar circle
        UnityEngine.GUI.DrawTexture(new Rect(mapCenter.x + bX, mapCenter.y + bY, 2, 2), aTexture);
      }
    }
  }Updated JavaScript VersionAuthor: DastardlyBanana 

This update makes the radar location more flexible, since you can easily define its location in the inspector using one of 9 preset locations or by defining your own. We have also made the radar interact with the enemies it tracks, seeing whether or not they are aware of the player and displaying aware enemies in a different color (this is dependent on the AI script of the enemies- in our game the enemies turn red when chasing the player, but this could be changed to fit the needs of the game it is in). It is also easily possible to change the size of the radar now, based on a percentage of the screen ( so it scales with the screen). 
A unity package containing the radar script, demo scene and example script for interaction with enemy AI can be found at our website: [[1]] 
@script ExecuteInEditMode()
// radar! by PsychicParrot, adapted from a Blitz3d script found in the public domain online somewhere ..
 
//Modified by Dastardly Banana to add radar size configuration, different colors for enemies in different states (patrolling or chasing), ability to move radar to either one of 9 preset locations or to custom location.
 
//some lines are particular to our AI script, you will need to change "EnemyAINew" to the name of your AI script, and change "isChasing" to the boolean within that AI script that is true when the enemy is active/can see the player/is chasing the player.
 
var blip : Texture; // texture to use when the enemy isn't chasing
var blipChasing : Texture; //When Chasing
var radarBG : Texture;
 
var centerObject : Transform;
var mapScale = 0.3;
var mapSizePercent = 15;
 
var checkAIscript : boolean = true;
var enemyTag = "Enemy";
 
enum radarLocationValues {topLeft, topCenter, topRight, middleLeft, middleCenter, middleRight, bottomLeft, bottomCenter, bottomRight, custom}
var radarLocation : radarLocationValues = radarLocationValues.bottomLeft;
 
private var mapWidth : float;
private var mapHeight : float;
private var mapCenter : Vector2;
var mapCenterCustom : Vector2;
 
function Start () {
	setMapLocation();	
}
 
function OnGUI () {
//	GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3(Screen.width / 600.0, Screen.height / 450.0, 1));
 
 	// Draw player blip (centerObject)
 	bX=centerObject.transform.position.x * mapScale;
 	bY=centerObject.transform.position.z * mapScale;	
 	GUI.DrawTexture(Rect(mapCenter.x - mapWidth/2,mapCenter.y-mapHeight/2,mapWidth,mapHeight),radarBG);
 
	// Draw blips for Enemies
	DrawBlipsForEnemies();
 
}
 
function drawBlip(go,aTexture){
 
	centerPos=centerObject.position;
	extPos=go.transform.position;
 
	// first we need to get the distance of the enemy from the player
	dist=Vector3.Distance(centerPos,extPos);
 
	dx=centerPos.x-extPos.x; // how far to the side of the player is the enemy?
	dz=centerPos.z-extPos.z; // how far in front or behind the player is the enemy?
 
	// what's the angle to turn to face the enemy - compensating for the player's turning?
	deltay=Mathf.Atan2(dx,dz)*Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
 
	// just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
	bX=dist*Mathf.Cos(deltay * Mathf.Deg2Rad);
	bY=dist*Mathf.Sin(deltay * Mathf.Deg2Rad);
 
	bX=bX*mapScale; // scales down the x-coordinate so that the plot stays within our radar
	bY=bY*mapScale; // scales down the y-coordinate so that the plot stays within our radar
 
	if(dist<=mapWidth*.5/mapScale){ 
		// this is the diameter of our largest radar circle
	   GUI.DrawTexture(Rect(mapCenter.x+bX,mapCenter.y+bY,4,4),aTexture);
 
	}
 
}
 
function DrawBlipsForEnemies(){
	//You will need to replace isChasing with a variable from your AI script that is true when 				the enemy is chasing the player, or doing watever you want it to be doing when it is red on 			the radar.
 
	//You will need to replace "EnemyAINew with the name of your AI script
 
    // Find all game objects tagged Enemy
    var gos : GameObject[];
    gos = GameObject.FindGameObjectsWithTag(enemyTag); 
 
    var distance = Mathf.Infinity; 
    var position = transform.position; 
 
    // Iterate through them and call drawBlip function
    for (var go : GameObject in gos)  { 
   		 	var blipChoice : Texture = blip;
   		   	if(checkAIscript){
    			var aiScript : EnemyAI = go.GetComponent("EnemyAI");
    		if(aiScript.isChasing)
    				blipChoice = blipChasing;
    	}
		drawBlip(go,blipChoice);
    }
 
}
 
function setMapLocation () {
	mapWidth = Screen.width*mapSizePercent/100.0;
	mapHeight = mapWidth;
 
	//sets mapCenter based on enum selection
	if(radarLocation == radarLocationValues.topLeft){
		mapCenter = Vector2(mapWidth/2, mapHeight/2);
	} else if(radarLocation == radarLocationValues.topCenter){
		mapCenter = Vector2(Screen.width/2, mapHeight/2);
	} else if(radarLocation == radarLocationValues.topRight){
		mapCenter = Vector2(Screen.width-mapWidth/2, mapHeight/2);
	} else if(radarLocation == radarLocationValues.middleLeft){
		mapCenter = Vector2(mapWidth/2, Screen.height/2);
	} else if(radarLocation == radarLocationValues.middleCenter){
		mapCenter = Vector2(Screen.width/2, Screen.height/2);
	} else if(radarLocation == radarLocationValues.middleRight){
		mapCenter = Vector2(Screen.width-mapWidth/2, Screen.height/2);
	} else if(radarLocation == radarLocationValues.bottomLeft){
		mapCenter = Vector2(mapWidth/2, Screen.height - mapHeight/2);
	} else if(radarLocation == radarLocationValues.bottomCenter){
		mapCenter = Vector2(Screen.width/2, Screen.height - mapHeight/2);
	} else if(radarLocation == radarLocationValues.bottomRight){
		mapCenter = Vector2(Screen.width-mapWidth/2, Screen.height - mapHeight/2);
	} else if(radarLocation == radarLocationValues.custom){
		mapCenter = mapCenterCustom;
	}
 
}An example of an AI script to interact with the radar: 
//   created by Dastardly Banana Productions
//   http://www.dastardlybanana.com/
 
//This is a dummy script to fool the radar, since our AI script is not ready to be posted at this time.        
// "isChasing" is true
 
var isChasing : boolean;
var seeDistance : float = 50;
 
function Start () {
}
function Update () {
	var gos : GameObject[];
    gos = GameObject.FindGameObjectsWithTag("Player"); 
    //assumes only one player
    var thePlayer:GameObject = gos[0];
	var dist = Vector3.Distance(thePlayer.transform.position,transform.position);
 
	if(dist<seeDistance){
		isChasing= true;
	} else {
		isChasing=false;
	}
}

Updated CSharp VersionI've created a radar script that I find easy to use and is customizable. You can show up to 4 kinds of objects on the radar but it would be easy to add more blip types. 
//
// Radar - Based off of the other radar scripts I've seen on the wiki
// By: Justin Zaun
//
// Attach this wherever you like, I recommend you attach it where the rest of your GUI is. Once
// attached take a look at the inspector for the object. You are going to see a bunch of options
// to setup. I've tried to give a set of defaults that will work with little messing around.
//
// The first item that should be set is the "Radar Center Tag" for anything of interesting to
// happen. This tag should be the object at the center of the radar, typically this is the local
// player's object. Place a check in the "Radar Center Active" box to diplay the play on the radar.
//
// The second item that should be set is the "Radar Blip1 Tag." This is the tag given to the
// objects you want to show on the radar. Typically this would be the remote player's tag or
// the bad guy's tag.
//
// To turn on the display of the blip place a check in the "Radar Blip1 Active" box.
//
// If you run your game at this point you will see a radar in the bottom center of your screen
// that is showing you all the remote players/bad guys that are around you.
//
// Now that you have seen a quick example of the radar I'll explain all the options. There looks
// like a lot but they are split into two sections. At the top are general radar settings that
// determin how you would like the radar to look. On the bottom there are settings for the blips
//
// I'll explain the blips first. This radar supports up to 4 types of blips. Each blip has an
// "Radar Blip# Active" option for turning on or off the blip. This allows you to have everything
// setup and then in game based on events to turn on the display of different types on blips.
//
// The second options is the "Radar Blip# Color" setting. This is the color of the blip. Not hard
// to explain, it changed the color of the object's blips for a given Tag. The last option for a
// blip is "Radar Blip# Tag." This is the tag for the object you would like to have on the radar.
//
// Some examples would be using Blip1 for bad guys, Blip2 for items the play is trying to find,
// Blip3 for forts and Blip4 could be the level's exits. Having the items on the radar in differnt
// colors will let the player identify the type of object the blip represents.
//
// The top options are the overall settings. First is the location of the radar. There are several
// options to choose from. If you choose "Custom" you have to fill in the "Radar Location Custom"
// to define the location. When you are defining the location please note this is the center of the
// radar.
//
// The third option "Radar Type" is how you would like your radar to look. You have a choice of
// Textured, Round and Transparent. The default is Round and is the colored bullseye you
// saw at the start if you played with the first example. If you choose Textured you MUST set
// "Radar Texture" to some image for the background. If you choose Round you can change the colors
// used in the bullseye with "Radar BackroundA" and "Radar BackgroundB"
//
// The last two options are about the size and zoom of the radar. The "Radar Size" is a percent
// of the screen the radar will take up, for example .2 is 20% of the screen. The "Radar Zoom"
// determines how much of the map should be displayed on the radar. Making this number smaller
// will zoom out and show you more stuff.
 
using UnityEngine;
using System.Collections;
 
public class Radar : MonoBehaviour
{
 
	public enum RadarTypes : int {Textured, Round, Transparent};
	public enum RadarLocations : int {TopLeft, TopCenter, TopRight, BottomLeft, BottomCenter, BottomRight, Left, Center, Right, Custom};
 
	// Display Location
	public RadarLocations radarLocation = RadarLocations.BottomCenter;
	public Vector2 radarLocationCustom;
	public RadarTypes radarType = RadarTypes.Round;
	public Color radarBackgroundA = new Color(255, 255, 0);
	public Color radarBackgroundB = new Color(0, 255, 255);
	public Texture2D radarTexture;
	public float radarSize = 0.20f;  // The amount of the screen the radar will use
	public float radarZoom = 0.60f;
 
	// Center Object information
	public bool   radarCenterActive;
	public Color  radarCenterColor = new Color(255, 255, 255);
	public string radarCenterTag;
 
	// Blip information
	public bool   radarBlip1Active;
	public Color  radarBlip1Color = new Color(0, 0, 255);
	public string radarBlip1Tag;
 
	public bool   radarBlip2Active;
	public Color  radarBlip2Color = new Color(0, 255, 0);
	public string radarBlip2Tag;
 
	public bool   radarBlip3Active;
	public Color  radarBlip3Color = new Color(255, 0, 0);
	public string radarBlip3Tag;
 
	public bool   radarBlip4Active;
	public Color  radarBlip4Color = new Color(255, 0, 255);
	public string radarBlip4Tag;
 
	// Internal vars
	private GameObject _centerObject;
	private int        _radarWidth;
	private int        _radarHeight;
	private Vector2    _radarCenter;
	private Texture2D  _radarCenterTexture;
	private Texture2D  _radarBlip1Texture;
	private Texture2D  _radarBlip2Texture;
	private Texture2D  _radarBlip3Texture;
	private Texture2D  _radarBlip4Texture;
 
	// Initialize the radar
	void Start ()
	{
		// Determine the size of the radar
    	_radarWidth = (int)(Screen.width * radarSize);
    	_radarHeight = _radarWidth;
 
    	// Get the location of the radar
    	setRadarLocation();
 
		// Create the blip textures
		_radarCenterTexture = new Texture2D(3, 3, TextureFormat.RGB24, false);
		_radarBlip1Texture = new Texture2D(3, 3, TextureFormat.RGB24, false);
		_radarBlip2Texture = new Texture2D(3, 3, TextureFormat.RGB24, false);
		_radarBlip3Texture = new Texture2D(3, 3, TextureFormat.RGB24, false);
		_radarBlip4Texture = new Texture2D(3, 3, TextureFormat.RGB24, false);
 
		CreateBlipTexture(_radarCenterTexture, radarCenterColor);
		CreateBlipTexture(_radarBlip1Texture, radarBlip1Color);
		CreateBlipTexture(_radarBlip2Texture, radarBlip2Color);
		CreateBlipTexture(_radarBlip3Texture, radarBlip3Color);
		CreateBlipTexture(_radarBlip4Texture, radarBlip4Color);
 
		// Setup the radar background texture
		if (radarType != RadarTypes.Textured)
		{
			radarTexture = new Texture2D(_radarWidth, _radarHeight, TextureFormat.RGB24, false);
			CreateRoundTexture(radarTexture, radarBackgroundA, radarBackgroundB);
		}
 
		// Get our center object
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(radarCenterTag);
		_centerObject = gos[0];
	}
 
	// Update is called once per frame
	void OnGUI ()
	{
		GameObject[] gos;
 
		// Draw th radar background
		if (radarType != RadarTypes.Transparent)
		{
			Rect radarRect = new Rect(_radarCenter.x - _radarWidth / 2, _radarCenter.y - _radarHeight / 2, _radarWidth, _radarHeight);
			GUI.DrawTexture(radarRect, radarTexture);
		}
 
		// Draw blips
		if (radarBlip1Active)
		{
			// Find all game objects
			gos = GameObject.FindGameObjectsWithTag(radarBlip1Tag); 
 
			// Iterate through them and call drawBlip function
			foreach (GameObject go in gos)
			{
				drawBlip(go, _radarBlip1Texture);
			}
		}
		if (radarBlip2Active)
		{
			gos = GameObject.FindGameObjectsWithTag(radarBlip2Tag); 
 
			foreach (GameObject go in gos)
			{
				drawBlip(go, _radarBlip2Texture);
			}
		}
		if (radarBlip3Active)
		{
			gos = GameObject.FindGameObjectsWithTag(radarBlip3Tag); 
 
			foreach (GameObject go in gos)
			{
				drawBlip(go, _radarBlip3Texture);
			}
		}
		if (radarBlip4Active)
		{
			gos = GameObject.FindGameObjectsWithTag(radarBlip4Tag); 
 
			foreach (GameObject go in gos)
			{
				drawBlip(go, _radarBlip4Texture);
			}
		}
 
		// Draw center oject
		if (radarCenterActive)
		{
			Rect centerRect = new Rect(_radarCenter.x - 1.5f, _radarCenter.y - 1.5f, 3, 3);
			GUI.DrawTexture(centerRect, _radarCenterTexture);
		}
	}
 
	// Draw a blip for an object
	void drawBlip(GameObject go, Texture2D blipTexture)
	{
		if (_centerObject)
		{
			Vector3 centerPos = _centerObject.transform.position;
			Vector3 extPos = go.transform.position;
 
			// Get the distance to the object from the centerObject
			float dist = Vector3.Distance(centerPos, extPos);
 
			// Get the object's offset from the centerObject
			float bX = centerPos.x - extPos.x;
			float bY = centerPos.z - extPos.z;
 
			// Scale the objects position to fit within the radar
			bX = bX * radarZoom;
			bY = bY * radarZoom;
 
			// For a round radar, make sure we are within the circle
			if(dist <= (_radarWidth - 2) * 0.5 / radarZoom)
			{
				Rect clipRect = new Rect(_radarCenter.x - bX - 1.5f, _radarCenter.y + bY - 1.5f, 3, 3);
				GUI.DrawTexture(clipRect, blipTexture);
			}
		}
	}
 
	// Create the blip textures
	void CreateBlipTexture(Texture2D tex, Color c)
	{
		Color[] cols = {c, c, c, c, c, c, c, c, c};
		tex.SetPixels(cols, 0);
		tex.Apply();
	}
 
	// Create a round bullseye texture
	void CreateRoundTexture(Texture2D tex, Color a, Color b)
	{
		Color c = new Color(0, 0, 0);
		int size = (int)((_radarWidth / 2) / 4);
 
		// Clear the texture
		for (int x = 0; x < _radarWidth; x++)
		{
			for (int y = 0; y < _radarWidth; y++)
			{
				tex.SetPixel(x, y, c);
			}
		}
 
		for (int r = 4; r > 0; r--)
		{
			if (r % 2 == 0)
			{
				c = a;
			}
			else
			{
				c = b;
			}
			DrawFilledCircle(tex, (int)(_radarWidth / 2), (int)(_radarHeight / 2), (r * size), c);
		}
 
		tex.Apply();
	}
 
	// Draw a filled colored circle onto a texture
	void DrawFilledCircle(Texture2D tex, int cx, int cy, int r, Color c)
	{
		for (int x = -r; x < r ; x++)
		{
			int height = (int)Mathf.Sqrt(r * r - x * x);
 
			for (int y = -height; y < height; y++)
				tex.SetPixel(x + cx, y + cy, c);
		}
	}
 
	// Figure out where to put the radar
	void setRadarLocation()
	{
		// Sets radarCenter based on enum selection
		if(radarLocation == RadarLocations.TopLeft)
		{
			_radarCenter = new Vector2(_radarWidth / 2, _radarHeight / 2);
		}
		else if(radarLocation == RadarLocations.TopCenter)
		{
			_radarCenter = new Vector2(Screen.width / 2, _radarHeight / 2);
		}
		else if(radarLocation == RadarLocations.TopRight)
		{
			_radarCenter = new Vector2(Screen.width - _radarWidth / 2, _radarHeight / 2);
		}
		else if(radarLocation == RadarLocations.Left)
		{
			_radarCenter = new Vector2(_radarWidth / 2, Screen.height / 2);
		}
		else if(radarLocation == RadarLocations.Center)
		{
			_radarCenter = new Vector2(Screen.width / 2, Screen.height / 2);
		}
		else if(radarLocation == RadarLocations.Right)
		{
			_radarCenter = new Vector2(Screen.width - _radarWidth / 2, Screen.height / 2);
		}
		else if(radarLocation == RadarLocations.BottomLeft)
		{
			_radarCenter = new Vector2(_radarWidth / 2, Screen.height - _radarHeight / 2);
		}
		else if(radarLocation == RadarLocations.BottomCenter)
		{
			_radarCenter = new Vector2(Screen.width / 2, Screen.height - _radarHeight / 2);
		}
			else if(radarLocation == RadarLocations.BottomRight)
		{
			_radarCenter = new Vector2(Screen.width - _radarWidth / 2, Screen.height - _radarHeight / 2);
		}
			else if(radarLocation == RadarLocations.Custom)
		{
			_radarCenter = radarLocationCustom;
		}
	} 
 
 
}Simple CSharp Version it supports n types of objects on radar, simple and usefull to use it: crate empty gameobject and add the script to it, fill the proprietis for the centerObject variable if you want to set it up dinamicly, in your player class scripts use something like: 
void Start(){
   ((Radar)FindObjectOfType(typeof(Radar))).centerObject=this.transform;
}c# verison of script: 
using UnityEngine;
using System.Collections;
 
public class Radar : MonoBehaviour 
{  // from the original javascript by PsychicParrot, 
    // who in turn adapted it from a Blitz3d script found in the
    // public domain online somewhere ....
    //
    public Texture radarBG;
    public Transform centerObject;
    public float mapScale = 0.3f;
 
	public float RadarSize=150f;
    public float maxDist = 200;
	public RComp[] RadaObjects;
	private Vector2 mapCenter;
	[System.Serializable]
	public class RComp{
		public Texture OnRadar;
		public string TagName;
	}
 
    public void OnGUI()
    {
		if(centerObject){
		     Rect r=new Rect(Screen.width-50 - RadarSize, 50, RadarSize, RadarSize);
 
		      UnityEngine.GUI.DrawTexture(r, radarBG,ScaleMode.StretchToFill);
		      mapCenter = new Vector2(Screen.width-50-RadarSize/2,50+RadarSize/2);
		    foreach(RComp c in RadaObjects){      
	     	 	GameObject[] gos = GameObject.FindGameObjectsWithTag(c.TagName);
			    foreach (GameObject go in gos){
			        drawBlip(go, c.OnRadar);
			    }
			}
		}
    }
 
    private void drawBlip(GameObject go, Texture aTexture)
    {
      Vector3 centerPos = centerObject.position;
      Vector3 extPos = go.transform.position;
 
      // first we need to get the distance of the enemy from the player
      float dist = Vector3.Distance(centerPos, extPos);
 
      float dx = centerPos.x - extPos.x; // how far to the side of the player is the enemy?
      float dz = centerPos.z - extPos.z; // how far in front or behind the player is the enemy?
 
      // what's the angle to turn to face the enemy - compensating for the player's turning?
      float deltay = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
 
      // just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
      float bX = dist * Mathf.Cos(deltay * Mathf.Deg2Rad);
      float bY = dist * Mathf.Sin(deltay * Mathf.Deg2Rad);
 
      bX = bX * mapScale; // scales down the x-coordinate by half so that the plot stays within our radar
      bY = bY * mapScale; // scales down the y-coordinate by half so that the plot stays within our radar
 
      if (dist <= maxDist)
      {
        // this is the diameter of our largest radar circle
        UnityEngine.GUI.DrawTexture(new Rect(mapCenter.x + bX, mapCenter.y + bY, 2, 2), aTexture);
      }
    }
  }
}
