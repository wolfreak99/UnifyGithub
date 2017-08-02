/*************************
 * Original url: http://wiki.unity3d.com/index.php/OpenStreetMap_for_unity_iPhone
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/OpenStreetMap_for_unity_iPhone.cs
 * File based on original modification date of: 28 November 2012, at 09:41. 
 *
 * Author: Mark Hessburg (VIC20) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.SimulationScripts
{
    Contents [hide] 
    1 Description 
    2 To do List (please help) 
    3 Project download 
    4 Thread 
    5 Code 
    6 Related Links 
    
    Description This is a simple OpenStreetMap script, it requires unity iPhone but should work in simulation mode with the normal unity. It loads OpenStreetMap Tiles based on your current (or while running in the Editor simulated) position. It scrolls the map by following GPS movements (or simulated location updates). You can set the useFAKEDmovement Variable to simulate movement of the iPhone in the Editor. Set Variable OSMZoom for different zoom levels (1-18). Just download the prepared Project, do a quick Build&Run and make a little test drive, it's pretty accurate (after one or two kilometers you might discover a map scrolling bug which still needs a fix.) 
    To do List (please help) Add OSM Icon - REMEMBER: OpenStreetMap license requires to display an OpenStreetMap-Icon when showing the Map 
    Fix Map Scrolling Bug 
    Delete OS-Tiles from memory (otherwise iPhone will crash due to memory leak) 
    Add User-TouchInterface 
    Add a smooth touch Zoom 
    Cache Tiles to disk (to avoid useless internet access) 
    Rotate Tiles and add Compass/Heading - in other words: don't use GUITexture anymore. (Rotation will require the unity expansion pack by Rob terrel to access the 3GS Compass or GPS Heading add "newLocation.course" to the enhancement pack to get the GPS heading - or CoreLocation which is able to handle both GPS and compass) 
    Find a way to increase download speed of the tiles and try to reduce tiles from 25 to 15 
    Get rid of the terrible Spaghetti-Code! :-) 
    Project download Click here to DOWNLOAD the Original Project (javascript). 
    link New C# Version by silentchujo 
    Thread link Forum thread 
    Code#pragma strict
     
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // 	OpenStreetMap for unity iPhone 0.1
    //
    // 	Project started december 2009 by Mark Hessburg as upcoming Feature for the App "SunGPS"
    // 	This project is open source - feel free to expand and to use for any purposes.
    //
    // 	WHAT IT DOES:
    //	Shows OpenStreetMap Tiles based on your current (or simulated) position. 
    //  Follows location updates (or simulated location updates).
    //
    //
    //	TO DO LIST: 
    //
    //  1. Add OSM Icon - REMEMBER: OpenStreetMap license requires to display an OpenStreetMap-Icon when showing the Map!
    //
    //	2. Fix Map Scrolling Bug 
    //
    //  3. delete OS-Tiles from memory (otherwise iPhone will crash due to memory overload)
    //
    //	4. Add User-TouchInterface
    //
    //	5. Add Zoom
    //
    //	6. Cache Tiles (to avoid useless internet access)
    //
    //  7. Rotate Tiles and add Compass/Heading 
    //  (Rotation will require the unity iPhone expansion pack by Rob terrel to access the 3GS Compass or GPS Heading
    //  add "newLocation.course" to the enhancement pack to get the GPS heading)
    // 
    //	8. Get rid of the terrible Spaghetti-Code! :-)
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
     
    private var useFAKEDcoordinates : int = 0; 
    var useFAKEDmovement : int = 1;
    private var FAKEDmovementLat : double = Random.Range(-0.00001, 0.00001);
    private var FAKEDmovementLon : double = Random.Range(-0.00001, 0.00001);
    private var FAKEDlatitude : double = 52.52845620905931;  // Chausseestr. 8C, 10115 Berlin - Germany
    private var FAKEDlongitude : double = 13.38696790757584; // (Should display exact position of my Appartement in the Backyard) 
     
    static var lat : double;
    static var lon : double;
    private var OldLat : double;
    private var OldLon : double;
     
    private var OSMtileX : int; 
    private var OSMtileY : int; 
    private var OldOSMtileX : int; 
    private var OldOSMtileY : int; 
    private var x : int;
    private var y : int;
    private var xpos : int;
    private var ypos : int;
    var OSMZoom : int = 16; // Set Zoom manually in Editor (1-18)
    private var TileSize : double = 256; // Change this ONLY when creating a stepless/smooth Zoom by changing the Size and Position of the GUITextures!
    private var CheckHeadingX : int; // 
    private var CheckHeadingY : int; // has nothing to do with GPS heading - used to find the scroll-direction of the Map
     
    // 	GUItextures for the Map-Tiles	5 rows a 5 Tiles - Examples: Tile 1 = -640,384 (upper left Tile) - Tile 13 = -128,-128 (center Tile) - Tile 25 = -640, 384 (lower right Tile) - (Tile 0 = nothing)
    var Tile = new Transform[26];
    var TileBuffer = new Texture[26];
     
    private var i : int;
    private var ii : int;
    private var number : int;
    private var TileNumber : int;
    private var buffer : int;
    var ready : int = 0;
     
    private var Xscroll : int;
    private var Yscroll : int;
    private var Tileposx : int;
    private var Tileposy : int;
    private var PositionOnTileX : int; // GPS Position's X,Y on the center Tile 
    private var PositionOnTileY : int;
     
     
     
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
    function Start () {
     
    	// automatically use a faked GPS Position when running in the editor  
    	if (iPhoneSettings.generation==iPhoneSettings.generation.Unknown)
    	{
    		useFAKEDcoordinates = 1;    
    	}
     
        // Start service before querying location
        iPhoneSettings.StartLocationServiceUpdates();
     
        // Wait until service initializes 
        var maxWait : int = 20;
        while (iPhoneSettings.locationServiceStatus == LocationServiceStatus.Initializing && maxWait > 0) {
            yield WaitForSeconds(1);
            maxWait--;
        } 
     
        // Service didn't initialize in 20 seconds
        if (maxWait < 1) {
            print("Timed out");
            return;
        } 
     
        // User denied access to device location 
        if (iPhoneSettings.locationServiceStatus == LocationServiceStatus.Failed) {
                print("User denied access to device location");
                return;        
            }
     
        // Stop service if there is no need to query location updates continously
        // iPhoneSettings.StopLocationServiceUpdates();
     
        if(useFAKEDcoordinates==1)
    	{
    		lat=FAKEDlatitude;
    		lon=FAKEDlongitude; // Use Faked coordinates when running in the editor
    	}
    	else
    	{
    		lat=iPhoneInput.lastLocation.latitude;
    		lon=iPhoneInput.lastLocation.longitude; // use real LocationService Position when running on the device
    	}	
     
    	CalculateTileXY();
    	Scroll();
    	LoadTiles();
     
    	OldLat=lat;
    	Oldlon=lon;
     
    	// Save calculated Tile X&Y for the FIRST TIME, this way we can decide if we need to load new Tiles from the Web
    	OldOSMtileX=OSMtileX;
    	OldOSMtileY=OSMtileY;
    }
     
     
     
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
     
     
    function Update () 
    {
    if(ready==1) // wait till first set of tiles is loaded
    {		
    	if(useFAKEDcoordinates==0)  
    	{
    		lat=iPhoneInput.lastLocation.latitude;
    		lon=iPhoneInput.lastLocation.longitude; // use real LocationServices Position when running on the device
    	}
    	else
    	{
    		if(useFAKEDmovement==1)  // SET useFAKEDmovement in the EDITOR to simulate GPS-Device movement
    		{
    			lat=lat+FAKEDmovementLat; 
    			lon=lon+FAKEDmovementLon;
    		}	
    	}
     
     
    	// Check if we moved and reload or recenter the map
    	if(OldLat != lat || OldLon != lon)
    	{
    		CalculateTileXY();
     
    		if(OldOSMtileX != OSMtileX || OldOSMtileY != OSMtileY)
    		{
    			if(OSMtileX > OldOSMtileX+1 || OSMtileX < OldOSMtileX-1  || OSMtileY > OldOSMtileY+1 || OSMtileY < OldOSMtileY-1)
    			{
    				LoadTiles();  // load all 25 tiles again
    			}
    			else
    			{
    				ReLoadTiles();  // only load necessary tiles (5 or 9 tiles) to reduce network connections
    			}	
    		}
     
    		Scroll();
     
    		OldLat=lat;
    		Oldlon=lon;
    	}
    }
    }
     
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
     
     
    function LoadTiles()  // Loads ALL 25 Tiles from the web
    {
    	xpos=-3;  // x & y needed for the tiles around the center tile - 0,0 = center
    	ypos=-2;
     
    	// Load tiles from OpenStreetMap
     
    	for (TileNumber=1;TileNumber<26;TileNumber++)
    	{		
    		xpos=xpos+1;
     
    		if(xpos==3)
    		{
    			xpos=-2;
    			ypos=ypos+1;
    		}	
     
    		url = "http://tile.openstreetmap.org/"+OSMZoom+"/"+(OSMtileX+xpos)+"/"+(OSMtileY+ypos)+".png";
    		var Download : WWW = new WWW (url);
     		yield Download; // Wait for download to complete
     
     		Tile[TileNumber].guiTexture.texture=Download.texture;
     		TileBuffer[TileNumber]=Tile[TileNumber].guiTexture.texture; // write buffer - not so smart to use twice the ammount of memory :-(
     
     		if(TileNumber==25)
     		{
     			ready=1;
     		}	
     
    	}
     
    }    
     
     
     
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
     
     
    function ReLoadTiles() // Loads only necessary tiles from the web
    {
    	x=-3;  // x & y -  0,0 = center
    	y=-2;
     
    	CheckHeadingY = OSMtileY - OldOSMtileY; // find scroll direction (-1, 0 or +1)  
    	CheckHeadingX = OSMtileX - OldOSMtileX; 
     
     
    	for (i=1;i<26;i++)
    	{			
    		x=x+1;
     
    		if(x==3)
    		{
    			x=-2;
    			y=y+1;
    		}	
     
    		if(CheckHeadingY*y!=2 || CheckHeadingX*x!=2)  // We load the Tile from the buffer
    		{
    			ii= (i+(5*CheckHeadingY))+CheckHeadingX;
    			Tile[i].guiTexture.texture=TileBuffer[ii];	
    		}		
    	}
     
    	x=-3;  
    	y=-2;
     
    	for (i=1;i<26;i++)
    	{			
    		x=x+1;
     
    		if(x==3)
    		{
    			x=-2;
    			y=y+1;
    		}	
     
    		if(CheckHeadingY*y==2 || CheckHeadingX*x==2)  // We have to download the Tile from the internet
    		{
    			url = "http://tile.openstreetmap.org/"+OSMZoom+"/"+(OSMtileX+x)+"/"+(OSMtileY+y)+".png";
    			var Download : WWW = new WWW (url);
     			yield Download; // Wait for download to complete
     			Tile[i].guiTexture.texture=Download.texture;
    		}
    	}
     
    	for (buffer=1;buffer<26;buffer++)
    	{
    		TileBuffer[buffer]=Tile[buffer].guiTexture.texture; // write new buffer
    	}		
     
    }    
     
     
     
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
     
     
    function CalculateTileXY()
    {    
    		// Save old calculated Tile X&Y, this way we can decide if we need to load new Tiles from the Web
    		OldOSMtileX=OSMtileX;
    		OldOSMtileY=OSMtileY;
     
    		// calculate center tile
    		OSMtileX=Mathf.FloorToInt((lon+180)/360*Mathf.Pow(2,OSMZoom));
    		OSMtileY=Mathf.FloorToInt((1-Mathf.Log(Mathf.Tan(lat*Mathf.PI/180) + 1/Mathf.Cos(lat*Mathf.PI/180))/Mathf.PI)/2 *Mathf.Pow(2,OSMZoom));	
     
    		// calculate the position ON the tile itself - (due to variable "TileSize" this is ready for a yet to be done new smooth zoom)
    		PositionOnTileX=Mathf.FloorToInt((((lon+180)/360*Mathf.Pow(2,OSMZoom))-OSMtileX) * TileSize);
    	    PositionOnTileY=Mathf.FloorToInt((((1-Mathf.Log(Mathf.Tan(lat*Mathf.PI/180) + 1/Mathf.Cos(lat*Mathf.PI/180))/Mathf.PI)/2 *Mathf.Pow(2,OSMZoom))-OSMtileY) * TileSize);
     
    }
     
     
     
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
     
     
    function Scroll()
    {
    	Tileposx=-896;
    	Tileposy=384;
     
    	for (number=1;number<26;number++)
    	{	
    		Tileposx=Tileposx+256;
     
    		if(Tileposx==640)
    		{
    			Tileposx=-640;
    			Tileposy=Tileposy-256;
    		}	
     
    		Xscroll=Tileposx-PositionOnTileX+(TileSize/2);
    		Yscroll=Tileposy+PositionOnTileY-(TileSize/2);
     
    		Tile[number].guiTexture.pixelInset = Rect (Xscroll, Yscroll, TileSize, TileSize);
     
    	}
}Related Links
}
