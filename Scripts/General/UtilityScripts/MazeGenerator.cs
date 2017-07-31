// Original url: http://wiki.unity3d.com/index.php/MazeGenerator
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/MazeGenerator.cs
// File based on original modification date of: 6 November 2012, at 13:31. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Overview This Javascript maze generator is used in the Fugu Maze player/widget. It's based on a recursive subdivision algorithm described in the Wikipedia article on maze generation. The complete project is available on the Unity Asset Store 
Implementation Each cell of the maze is a cube with a tile on each side (a textured plane). InstantiateFloor and InstantiateWalls instantiates the tiles - this is the time-consuming part, hence the use of coroutines and a progress indicator (I attach the script to a GameObject with a GUIText that acts as the progress display) 
MakeMaze runs the maze generation algorithm, deactivating all the walls and then recursively subdividing the by activating walls and knocking holes in corridors to connect regions. 
The Start function runs the wall/floor instantiation functions and then optionally activates another game object to start the game. In the case of FuguMaze, that start object is a first person controller with a script that places itself in the maze. 
The maze generation can be made generic by changing the Room class to, say, use booleans for the walls. Then you can run MakeMaze and build a physical realization from the resulting maze structure. 


/* Copyright (c) 2007-2008 Technicat, LLC */
 
// a square plane used for walls, floor, and ceiling
var tile:Transform;
var tileSize:int = 10;
 
// next state
var next:GameObject;
 
// maze dimensions
var height:int = 1;
var width:int = 1;
 
private var maze;
private var tileCounter = 0;
 
private var facingDown:Quaternion = Quaternion.Euler(180,0,0);
 
class Room {
	var east:Transform;
	var west:Transform;
	var north:Transform;
	var south:Transform;
}
 
function Start() {
	maze = new Array(width*height);
	yield StartCoroutine("InstantiateFloor");
	yield StartCoroutine("InstantiateWalls");
	guiText.enabled = false;
	if (next != null) {
		next.active = true;
	}
}
 
function InstantiateWalls() {
	var mid = tileSize/2;
	var pos = new Vector3(0,mid,0);
	for (var x:int =0; x< width; ++x) {
		var xpos:int = x*tileSize;
		for (var y:int =0; y< height; ++y) {
			var ypos:int = y*tileSize;
			var room = new Room();
			pos.x = xpos;
			pos.z = ypos-mid;
			room.south = InstantiateTile(pos,Quaternion.identity);
			room.south.Rotate(90,0,0);
			pos.x = xpos-mid;
			pos.z = ypos;
			room.west = InstantiateTile(pos,Quaternion.identity);
			room.west.Rotate(0,0,270);
			room.west.Rotate(0,90,0);
			pos.x = xpos;
			pos.z = ypos+mid;
			room.north = InstantiateTile(pos,Quaternion.identity);
			room.north.Rotate(270,0,0);
			room.north.Rotate(0,180,0);
			pos.x = xpos+mid;
			pos.z = ypos;
			room.east =InstantiateTile(pos,Quaternion.identity);
			room.east.Rotate(0,0,90);
			room.east.Rotate(0,270,0);
			maze[MazeIndex(x,y)]=room;
			ShowProgress();
			yield;
		}
	}
}
 
// floor and ceiling
function InstantiateFloor() {
	var floorpos:Vector3 = Vector3.zero;
	for (var x:int=0; x< width; ++x) {
		floorpos.x = x*tileSize;
		for (var y:int=0; y< height; ++y) {
			floorpos.z = y*tileSize;
			floorpos.y = 0;
			InstantiateTile(floorpos,Quaternion.identity);
			floorpos.y = tileSize;
			InstantiateTile(floorpos,facingDown);
			ShowProgress();
			yield;
		}
	}
}
 
function ShowProgress() {
	var progress:float = tileCounter/(height*width*6.0);
	guiText.text = progress.ToString("maze generated: #0%");	
}
 
function InstantiateTile(pos:Vector3,rot:Quaternion):Transform {
	++tileCounter;
	return Instantiate(tile,pos,rot);
}
 
function MakeMaze() {
	ClearMaze();
	SetOuterWalls();
	SubDivideMaze(0,width-1,0,height-1);
}
 
function ClearMaze() {
	for (var x:int=0; x< width; ++x) {
		for (var y:int=0; y< height; ++y) {
			maze[MazeIndex(x,y)].west.active = false;
			maze[MazeIndex(x,y)].east.active = false;
			maze[MazeIndex(x,y)].north.active = false;
			maze[MazeIndex(x,y)].south.active = false;
		}
	}
}
 
function SubDivideMaze(left,right,bottom,top) {
	if (left!=right && bottom != top) {
		var x:int = Random.Range(left,right);
		var leftdoor:int = Random.Range(left,x+1);
		var rightdoor:int = Random.Range(x+1,right+1);
		var y:int = Random.Range(bottom,top);
		var bottomdoor:int = Random.Range(bottom,y+1);
		var topdoor:int = Random.Range(y+1,top+1);
		AddNorthWall(left,right,y);
		AddEastWall(bottom,top,x);
		var doors = Random.value;
		if (doors < 0.25) {
			SetNorthWall(MazeIndex(leftdoor,y),false);
			SetNorthWall(MazeIndex(rightdoor,y),false);
			SetEastWall(MazeIndex(x,bottomdoor),false);
		} else {
			if (doors < 0.5) {
				SetNorthWall(MazeIndex(leftdoor,y),false);
				SetNorthWall(MazeIndex(rightdoor,y),false);
				SetEastWall(MazeIndex(x,topdoor),false);
			} else {
					if (doors < 0.75) {
						SetNorthWall(MazeIndex(rightdoor,y),false);
						SetEastWall(MazeIndex(x,bottomdoor),false);
						SetEastWall(MazeIndex(x,topdoor),false);
					} else {
							SetNorthWall(MazeIndex(leftdoor,y),false);
							SetEastWall(MazeIndex(x,bottomdoor),false);
							SetEastWall(MazeIndex(x,topdoor),false);
					}
			}
		}
		SubDivideMaze(left,x,y+1,top);
		SubDivideMaze(x+1,right,y+1,top);
		SubDivideMaze(left,x,bottom,y);
		SubDivideMaze(x+1,right,bottom,y);
	}
}
 
function SetOuterWalls() {
	AddNorthWall(0,width-1,height-1);
	AddSouthWall(0,width-1,0);
	AddEastWall(0,height-1,width-1);
	AddWestWall(0,height-1,0);
	SetNorthWall(MazeIndex(width-1,height-1),false);
}
 
function MazeIndex(x:int,y:int):int {
	return y*width+x;
}
 
 
function SetNorthWall(room,value) {
	maze[room].north.active = value;
	var neighbor:int = RoomNorth(room);
	if (neighbor !=-1) {
		maze[neighbor].south.active = value;
	}
}
 
function SetSouthWall(room,value) {
	maze[room].south.active = value;
	var neighbor:int = RoomSouth(room);
	if (neighbor !=-1) {
		maze[neighbor].north.active = value;
	}
}
 
function SetEastWall(room,value) {
	maze[room].east.active = value;
	var neighbor:int = RoomEast(room);
	if (neighbor !=-1) {
		maze[neighbor].west.active = value;
	}
}
 
function SetWestWall(room,value) {
	maze[room].west.active = value;
	var neighbor:int = RoomWest(room);
	if (neighbor !=-1) {
		maze[neighbor].east.active = value;
	}
}
 
function AddNorthWall(left:int,right:int,y:int) {
	for (var hwall:int = left; hwall<=right; ++hwall) {
			SetNorthWall(MazeIndex(hwall,y),true);
		}
}
 
function AddEastWall(bottom:int,top:int,x:int) {
	for (var vwall:int = bottom; vwall<=top; ++vwall) {
		SetEastWall(MazeIndex(x,vwall),true);
	}
}
 
function AddSouthWall(left:int,right:int,y:int) {
	for (var hwall:int = left; hwall<=right; ++hwall) {
		SetSouthWall(MazeIndex(hwall,y),true);
	}
}
 
function AddWestWall(bottom:int,top:int,x:int) {
	for (var vwall:int = bottom; vwall<=top; ++vwall) {
		SetWestWall(MazeIndex(x,vwall),true);
	}
}
 
function RoomEast(index:int) {
	var y:int = index/width;
	var x:int = index-y*width;
	if (x==width-1) {
		return -1;
	} else {
		return MazeIndex(x+1,y);
	}
}
 
function RoomWest(index:int) {
	var y:int = index/width;
	var x:int = index-y*width;
	if (x==0) {
		return -1;
	} else {
		return MazeIndex(x-1,y);
	}
}
 
function RoomNorth(index:int) {
	var y:int = index/width;
	var x:int = index-y*width;
	if (y==height-1) {
		return -1;
	} else {
		return MazeIndex(x,y+1);
	}
}
 
function RoomSouth(index:int) {
	var y:int = index/width;
	var x:int = index-y*width;
	if (y==0) {
		return -1;
	} else {
		return MazeIndex(x,y-1);
	}
}
 
function GetRoom(x:int,y:int) {
	return maze[MazeIndex(x,y)];
}
After using the base script in Unity 2.6 I found some issues. I cannot get the 2nd plane to activate no matter how hard I try. I also modified the code above to actually run the MakeMaze() routine. In this way, the generator is working, but with a single tile as its base - hoping someone with more experience can take a look and see what else can be addressed to get this fully working. 


/* Copyright (c) 2007-2008 Technicat, LLC */
/* Minor Editing ACFalk */
 
// a square plane used for walls, floor, and ceiling
var tile:Transform;
var tileSize:int = 10;
 
// next state
// Changed this to bypass the deprecated methods transitioning to Unity 2.6 : ACF
var next:Transform;
 
// maze dimensions
var height:int = 12;
var width:int = 12;
 
private var maze;
private var tileCounter = 0;
 
private var facingDown:Quaternion = Quaternion.Euler(180,0,0);
 
class Room {
    var east:Transform;
    var west:Transform;
    var north:Transform;
    var south:Transform;
}
 
function Start() {
    maze = new Array(width*height);
    yield StartCoroutine("InstantiateFloor");
    yield StartCoroutine("InstantiateWalls");
    guiText.enabled = false;
    if (next != null) {
        next.gameObject.active = true;
 
    }
 
     // Added a Call to actually Make your Maze : ACF
     MakeMaze();
}
 
function InstantiateWalls() {
    var mid = tileSize/2;
    var pos = new Vector3(0,mid,0);
    for (var x:int =0; x< width; ++x) {
        var xpos:int = x*tileSize;
        for (var y:int =0; y< height; ++y) {
            var ypos:int = y*tileSize;
            var room = new Room();
            pos.x = xpos;
            pos.z = ypos-mid;
            room.south = InstantiateTile(pos,Quaternion.identity);
            room.south.Rotate(90,0,0);
            pos.x = xpos-mid;
            pos.z = ypos;
            room.west = InstantiateTile(pos,Quaternion.identity);
            room.west.Rotate(0,0,270);
            room.west.Rotate(0,90,0);
            pos.x = xpos;
            pos.z = ypos+mid;
            room.north = InstantiateTile(pos,Quaternion.identity);
            room.north.Rotate(270,0,0);
            room.north.Rotate(0,180,0);
            pos.x = xpos+mid;
            pos.z = ypos;
            room.east =InstantiateTile(pos,Quaternion.identity);
            room.east.Rotate(0,0,90);
            room.east.Rotate(0,270,0);
            maze[MazeIndex(x,y)]=room;
            ShowProgress();
            yield;
        }
    }
}
 
// floor and ceiling
function InstantiateFloor() {
    var floorpos:Vector3 = Vector3.zero;
    for (var x:int=0; x< width; ++x) {
        floorpos.x = x*tileSize;
        for (var y:int=0; y< height; ++y) {
            floorpos.z = y*tileSize;
            floorpos.y = 0;
            InstantiateTile(floorpos,Quaternion.identity);
            floorpos.y = tileSize;
            InstantiateTile(floorpos,facingDown);
            ShowProgress();
            yield;
        }
    }
}
 
function ShowProgress() {
    var progress:float = tileCounter/(height*width*6.0);
    guiText.text = progress.ToString("maze generated: #0%");   
}
 
function InstantiateTile(pos:Vector3,rot:Quaternion):Transform {
    ++tileCounter;
    return Instantiate(tile,pos,rot);
}
 
function MakeMaze() {
    ClearMaze();
    SetOuterWalls();
 
	SubDivideMaze(0,width-1,0,height-1);
 
}
 
function ClearMaze() {
    for (var x:int=0; x< width; ++x) {
        for (var y:int=0; y< height; ++y) {
            maze[MazeIndex(x,y)].west.active = false;
            maze[MazeIndex(x,y)].east.active = false;
            maze[MazeIndex(x,y)].north.active = false;
            maze[MazeIndex(x,y)].south.active = false;
        }
    }
}
 
function SubDivideMaze(left,right,bottom,top) {
    if (left!=right && bottom != top) {
        var x:int = Random.Range(left,right);
        var leftdoor:int = Random.Range(left,x+1);
        var rightdoor:int = Random.Range(x+1,right+1);
        var y:int = Random.Range(bottom,top);
        var bottomdoor:int = Random.Range(bottom,y+1);
        var topdoor:int = Random.Range(y+1,top+1);
        AddNorthWall(left,right,y);
        AddEastWall(bottom,top,x);
        var doors = Random.value;
        if (doors < 0.25) {
            SetNorthWall(MazeIndex(leftdoor,y),false);
            SetNorthWall(MazeIndex(rightdoor,y),false);
            SetEastWall(MazeIndex(x,bottomdoor),false);
        } else {
            if (doors < 0.5) {
                SetNorthWall(MazeIndex(leftdoor,y),false);
                SetNorthWall(MazeIndex(rightdoor,y),false);
                SetEastWall(MazeIndex(x,topdoor),false);
            } else {
                    if (doors < 0.75) {
                        SetNorthWall(MazeIndex(rightdoor,y),false);
                        SetEastWall(MazeIndex(x,bottomdoor),false);
                        SetEastWall(MazeIndex(x,topdoor),false);
                    } else {
                            SetNorthWall(MazeIndex(leftdoor,y),false);
                            SetEastWall(MazeIndex(x,bottomdoor),false);
                            SetEastWall(MazeIndex(x,topdoor),false);
                    }
            }
        }
        SubDivideMaze(left,x,y+1,top);
        SubDivideMaze(x+1,right,y+1,top);
        SubDivideMaze(left,x,bottom,y);
        SubDivideMaze(x+1,right,bottom,y);
    }
}
 
function SetOuterWalls() {
    AddNorthWall(0,width-1,height-1);
    AddSouthWall(0,width-1,0);
    AddEastWall(0,height-1,width-1);
    AddWestWall(0,height-1,0);
    SetNorthWall(MazeIndex(width-1,height-1),false);
}
 
function MazeIndex(x:int,y:int):int {
    return y*width+x;
}
 
 
function SetNorthWall(room,value) {
    maze[room].north.active = value;
    var neighbor:int = RoomNorth(room);
    if (neighbor !=-1) {
        maze[neighbor].south.active = value;
    }
}
 
function SetSouthWall(room,value) {
    maze[room].south.active = value;
    var neighbor:int = RoomSouth(room);
    if (neighbor !=-1) {
        maze[neighbor].north.active = value;
    }
}
 
function SetEastWall(room,value) {
    maze[room].east.active = value;
    var neighbor:int = RoomEast(room);
    if (neighbor !=-1) {
        maze[neighbor].west.active = value;
    }
}
 
function SetWestWall(room,value) {
    maze[room].west.active = value;
    var neighbor:int = RoomWest(room);
    if (neighbor !=-1) {
        maze[neighbor].east.active = value;
    }
}
 
function AddNorthWall(left:int,right:int,y:int) {
    for (var hwall:int = left; hwall<=right; ++hwall) {
            SetNorthWall(MazeIndex(hwall,y),true);
        }
}
 
function AddEastWall(bottom:int,top:int,x:int) {
    for (var vwall:int = bottom; vwall<=top; ++vwall) {
        SetEastWall(MazeIndex(x,vwall),true);
    }
}
 
function AddSouthWall(left:int,right:int,y:int) {
    for (var hwall:int = left; hwall<=right; ++hwall) {
        SetSouthWall(MazeIndex(hwall,y),true);
    }
}
 
function AddWestWall(bottom:int,top:int,x:int) {
    for (var vwall:int = bottom; vwall<=top; ++vwall) {
        SetWestWall(MazeIndex(x,vwall),true);
    }
}
 
function RoomEast(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (x==width-1) {
        return -1;
    } else {
        return MazeIndex(x+1,y);
    }
}
 
function RoomWest(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (x==0) {
        return -1;
    } else {
        return MazeIndex(x-1,y);
    }
}
 
function RoomNorth(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (y==height-1) {
        return -1;
    } else {
        return MazeIndex(x,y+1);
    }
}
 
function RoomSouth(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (y==0) {
        return -1;
    } else {
        return MazeIndex(x,y-1);
    }
}
 
function GetRoom(x:int,y:int) {
    return maze[MazeIndex(x,y)];
}I modified version 2 of this script to accept Blender meshes. As you may know, meshes imported into Unity from Blender end up with a default Euler rotation of (270,0,0) instead of (0,0,0) when the model is in its default upright position. This version of the code adds a check to see if Euler x == 270, and adjusts accordingly. In theory you could use any floor mesh to generate a maze, not just a plane. 
/* Copyright (c) 2007-2008 Technicat, LLC */
/* Minor Editing ACFalk */
/* Blender model fix by WarpZone */
 
// a square plane OR any floor mesh prefab used for walls, floor, and ceiling.
// If the prefab's euler X rotation is 270, script will assume we are dealing with a Blender mesh and adjust accordingly.
var tile: Transform;
var tileSize:int = 4;
 
// next state
// Changed this to bypass the deprecated methods transitioning to Unity 2.6 : ACF
var next:Transform;
 
// maze dimensions
var height:int = 12;
var width:int = 12;
 
private var maze;
private var tileCounter = 0;
 
private var facingDown:Quaternion = Quaternion.Euler(180,0,0);
 
 
class Room {
    var east:Transform;
    var west:Transform;
    var north:Transform;
    var south:Transform;
}
 
function Start() {
    maze = new Array(width*height);
    yield StartCoroutine("InstantiateFloor");
    yield StartCoroutine("InstantiateWalls");
    if (next != null) {
        next.gameObject.active = true;
 
    }
     // Added a Call to actually Make your Maze : ACF
     MakeMaze();
}
 
function InstantiateWalls() {
    var mid = tileSize/2;
    var pos = new Vector3(0,mid,0);
    for (var x:int =0; x< width; ++x) {
        var xpos:int = x*tileSize;
        for (var y:int =0; y< height; ++y) {
            var ypos:int = y*tileSize;
            var room = new Room();
            pos.x = xpos;
            pos.z = ypos-mid;
            // BlLENDER fix
            if(tile.rotation.eulerAngles.x == 270){
            	room.south = InstantiateTile(pos,tile.rotation);
				room.south.Rotate(90,0,0);
				pos.x = xpos-mid;
				pos.z = ypos;
				room.west = InstantiateTile(pos,tile.rotation);
				room.west.Rotate(90,0,0);
				room.west.Rotate(0,90,0);
				pos.x = xpos;
				pos.z = ypos+mid;
				room.north = InstantiateTile(pos,tile.rotation);
				room.north.Rotate(90,0,0);
				room.north.Rotate(0,180,0);
				pos.x = xpos+mid;
				pos.z = ypos;
				room.east =InstantiateTile(pos,tile.rotation);
				room.east.Rotate(90,0,0);
				room.east.Rotate(0,270,0);
			} else {
				room.south = InstantiateTile(pos,tile.rotation);
				room.south.Rotate(90,0,0);
				pos.x = xpos-mid;
				pos.z = ypos;
				room.west = InstantiateTile(pos,tile.rotation);
				room.west.Rotate(90,90,0);
				pos.x = xpos;
				pos.z = ypos+mid;
				room.north = InstantiateTile(pos,tile.rotation);
				room.north.Rotate(90,180,0);
				pos.x = xpos+mid;
				pos.z = ypos;
				room.east =InstantiateTile(pos,tile.rotation);
				room.east.Rotate(90,270,0);				
			}
			maze[MazeIndex(x,y)]=room;
			UpdateProgress();
			yield;
		}
	}
}
 
// floor and ceiling
function InstantiateFloor() {
	//Instead of assuming a zeroed prefab, let's grab the pos and rot of our prefab!
    var floorpos:Vector3 = tile.position;
    for (var x:int=0; x< width; ++x) {
        floorpos.x = x*tileSize;
        for (var y:int=0; y< height; ++y) {
            floorpos.z = y*tileSize;
            floorpos.y = 0;
            InstantiateTile(floorpos,tile.rotation);
            floorpos.y = tileSize;
            InstantiateTile(floorpos,tile.rotation * facingDown);
            UpdateProgress();
            yield;
        }
    }
}
 
 var updateString: String = "";
function UpdateProgress() {
    var progress:float = tileCounter/(height*width*6.0);
    updateString = progress.ToString("maze generated: #0%");   
}
 
function OnGUI(){
	GUI.Box (Rect (0,0,600,50),updateString );
}
 
function InstantiateTile(pos:Vector3,rot:Quaternion):Transform {
    ++tileCounter;
    return Instantiate(tile,pos,rot);
}
 
function MakeMaze() {
    ClearMaze();
    SetOuterWalls();
 
	SubDivideMaze(0,width-1,0,height-1);
 
}
 
function ClearMaze() {
    for (var x:int=0; x< width; ++x) {
        for (var y:int=0; y< height; ++y) {
            maze[MazeIndex(x,y)].west.active = false;
            maze[MazeIndex(x,y)].east.active = false;
            maze[MazeIndex(x,y)].north.active = false;
            maze[MazeIndex(x,y)].south.active = false;
        }
    }
}
 
function SubDivideMaze(left,right,bottom,top) {
    if (left!=right && bottom != top) {
        var x:int = Random.Range(left,right);
        var leftdoor:int = Random.Range(left,x+1);
        var rightdoor:int = Random.Range(x+1,right+1);
        var y:int = Random.Range(bottom,top);
        var bottomdoor:int = Random.Range(bottom,y+1);
        var topdoor:int = Random.Range(y+1,top+1);
        AddNorthWall(left,right,y);
        AddEastWall(bottom,top,x);
        var doors = Random.value;
        if (doors < 0.25) {
            SetNorthWall(MazeIndex(leftdoor,y),false);
            SetNorthWall(MazeIndex(rightdoor,y),false);
            SetEastWall(MazeIndex(x,bottomdoor),false);
        } else {
            if (doors < 0.5) {
                SetNorthWall(MazeIndex(leftdoor,y),false);
                SetNorthWall(MazeIndex(rightdoor,y),false);
                SetEastWall(MazeIndex(x,topdoor),false);
            } else {
                    if (doors < 0.75) {
                        SetNorthWall(MazeIndex(rightdoor,y),false);
                        SetEastWall(MazeIndex(x,bottomdoor),false);
                        SetEastWall(MazeIndex(x,topdoor),false);
                    } else {
                            SetNorthWall(MazeIndex(leftdoor,y),false);
                            SetEastWall(MazeIndex(x,bottomdoor),false);
                            SetEastWall(MazeIndex(x,topdoor),false);
                    }
            }
        }
        SubDivideMaze(left,x,y+1,top);
        SubDivideMaze(x+1,right,y+1,top);
        SubDivideMaze(left,x,bottom,y);
        SubDivideMaze(x+1,right,bottom,y);
    }
}
 
function SetOuterWalls() {
    AddNorthWall(0,width-1,height-1);
    AddSouthWall(0,width-1,0);
    AddEastWall(0,height-1,width-1);
    AddWestWall(0,height-1,0);
    SetNorthWall(MazeIndex(width-1,height-1),false);
}
 
function MazeIndex(x:int,y:int):int {
    return y*width+x;
}
 
 
function SetNorthWall(room,value) {
    maze[room].north.active = value;
    var neighbor:int = RoomNorth(room);
    if (neighbor !=-1) {
        maze[neighbor].south.active = value;
    }
}
 
function SetSouthWall(room,value) {
    maze[room].south.active = value;
    var neighbor:int = RoomSouth(room);
    if (neighbor !=-1) {
        maze[neighbor].north.active = value;
    }
}
 
function SetEastWall(room,value) {
    maze[room].east.active = value;
    var neighbor:int = RoomEast(room);
    if (neighbor !=-1) {
        maze[neighbor].west.active = value;
    }
}
 
function SetWestWall(room,value) {
    maze[room].west.active = value;
    var neighbor:int = RoomWest(room);
    if (neighbor !=-1) {
        maze[neighbor].east.active = value;
    }
}
 
function AddNorthWall(left:int,right:int,y:int) {
    for (var hwall:int = left; hwall<=right; ++hwall) {
            SetNorthWall(MazeIndex(hwall,y),true);
        }
}
 
function AddEastWall(bottom:int,top:int,x:int) {
    for (var vwall:int = bottom; vwall<=top; ++vwall) {
        SetEastWall(MazeIndex(x,vwall),true);
    }
}
 
function AddSouthWall(left:int,right:int,y:int) {
    for (var hwall:int = left; hwall<=right; ++hwall) {
        SetSouthWall(MazeIndex(hwall,y),true);
    }
}
 
function AddWestWall(bottom:int,top:int,x:int) {
    for (var vwall:int = bottom; vwall<=top; ++vwall) {
        SetWestWall(MazeIndex(x,vwall),true);
    }
}
 
function RoomEast(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (x==width-1) {
        return -1;
    } else {
        return MazeIndex(x+1,y);
    }
}
 
function RoomWest(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (x==0) {
        return -1;
    } else {
        return MazeIndex(x-1,y);
    }
}
 
function RoomNorth(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (y==height-1) {
        return -1;
    } else {
        return MazeIndex(x,y+1);
    }
}
 
function RoomSouth(index:int) {
    var y:int = index/width;
    var x:int = index-y*width;
    if (y==0) {
        return -1;
    } else {
        return MazeIndex(x,y-1);
    }
}
 
function GetRoom(x:int,y:int) {
    return maze[MazeIndex(x,y)];
}
}
