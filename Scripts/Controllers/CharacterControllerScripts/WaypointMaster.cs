// Original url: http://wiki.unity3d.com/index.php/WaypointMaster
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/WaypointMaster.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
This is implementation of A* algorithm, based on waypoints. 
UsageTo use it you should attach "WaypointMaster" script to empty game object, and add waypoints around obstacles in such way, that each waypoint will be able to see any another waypoint. Waypoint can be any game object, this script use only position component of waypoint game objects, and waypoints should be without collider component. All this waypoints should be children of WyapointMaster game object. Download Demo If you have questions, you can ask them here 
Screenshot 
JavaScript - WaypointMaster.jsvar LayerToIgnore : LayerMask = -1;	// Layer to ignore, when do raycasting
private var layerMask : int;
 
var drawPathLines : boolean = false;
 
private var waypointList = new Array();	// List of all waypoints 
private var opendList = new Array();		
private var closedList = new Array();
var pathList = new Array();	
 
private var WaypointID : int = 0;	
 
class Waypoint 
{
	var af : float = 0.0;		// the sum of g and h
	var ag : float = 0.0;	// the actual shortest distance traveled from initial node to current node
	var ah : float = 0.0;	// the estimated (or "heuristic") distance from current node to goal
	var ID : int = 0;
 
	var Pos : Vector3;	
	var creator : Waypoint = null;		// Parent of this waypoint
	var childList = new Array();	// List of child nodes
}
 
function Start () 
{
	layerMask = ~LayerToIgnore.value;
 
	// Iterate through childs and create waypoint list
	for (var child : Transform in transform) 
	{
		var waypInst = new Waypoint();
		waypInst.Pos = child.transform.position;
		WaypointID++;
		waypInst.ID = WaypointID;
		waypointList.Push(waypInst);
 
		//Destroy (child.transform.gameObject);
	}
}
 
// Find waypoint nodes, that current node can see
function FindChild(waypInst : Waypoint)
{
	//	Clear previous data first
	waypInst.childList.Clear();
 
	//iterate through each node of the list
	var Dir : Vector3;
	for (var node : Waypoint in waypointList)
	{
		if(node.ID != waypInst.ID)
		{
			Dir = node.Pos - waypInst.Pos;
			Dir.Normalize();
			if((Physics.Raycast(waypInst.Pos, Dir, Vector3.Distance(waypInst.Pos, node.Pos), layerMask)) == false)
			{
				waypInst.childList.Push(node);
			}
		}
	}
}
 
// Clear list of child nodes
function FreeChild()
{
	for (var node : Waypoint in waypointList)
	{
		node.childList.clear();
	}
}
 
// Find path
function FindPath(start : Vector3, dest : Vector3)
{
	var resultPath : int = -1;
	var tempNode : Waypoint;
 
	// Clear previous data, before path finding	
	opendList.Clear();
	closedList.Clear();
	pathList.Clear();
	for (var node : Waypoint in waypointList)
	{
		node.creator = null;
	}
 
	// Add the starting location to the open list
	var StartLoc = new Waypoint();
	StartLoc.ah = Vector3.Distance(start, dest);
	StartLoc.af = StartLoc.ah;
	StartLoc.Pos = start;
	opendList.Push(StartLoc);
 
	// Check to see if we can trace ray to the destination node without path finding
	var Dir : Vector3 = dest - start;
	Dir.Normalize();
	if((Physics.Raycast(start, Dir, Vector3.Distance(start, dest), layerMask)) == false)
	{
		// Add start location
		pathList.Push(start);
		// Add destination location
		pathList.Push(dest);
 
		// Path is found
		resultPath = 1;
	}
 
	// Repeat until path is found, or it doesn't exist
	if(resultPath == -1)
	{
		// If opend list isn't empty
		while(opendList.length != 0)
		{
			// Find lowest f value in opend list
			var f = opendList[0].af;
			var index : int = 0;
			for(var i : int = 0; i < opendList.length-1; i++)
			{
				if(opendList[i].af < f)
				{
					f = opendList[i].af;
					index = i;
				}
			}
 
			// if current node already has list of children, do nothing
			if(opendList[index].childList.length == 0)
			{
				FindChild(opendList[index]);
			}
 
			// Add current node to the closed list
			closedList.Push(opendList[index]);
			// And remove it from the opend list
			opendList.RemoveAt(index);
 
			// Check all child nodes of node we currently added to closed list
			for (var child : Waypoint in closedList[closedList.length-1].childList)
			{
				var skip : int = 0;
 
				// If current node in the closed list skip this loop cycle
				for (var node : Waypoint in closedList)
				{
					if(child.ID == node.ID)
					{
						skip = 1;
						break;
					}
				}
 
				// Should we skip this loop cycle?
				if(skip == 0)
				{
					// If this node is already in the opend list, check to see if this path to that node is better
					// (if the G score for that node is lower if we use the current node to get there)
					for (var node : Waypoint in opendList)
					{
						if(child.ID == node.ID)
						{
							skip = 1;
							var dist = Vector3.Distance(closedList[closedList.length-1].Pos, child.Pos);
							if((closedList[closedList.length-1].ag + dist) < child.ag)
							{
								// If new G score better, recalculate F and G scores for this node
								// and change the "creator" of this node
								child.creator = closedList[closedList.length-1];
								child.ag = closedList[closedList.length-1].ag + dist;
								child.ah = Vector3.Distance(child.Pos, dest);
								child.af = child.ag + child.ah;
							}
						}
					}
 
					// If current node isn't in the opend list, add it there
					if(skip == 0)
					{
						child.creator = closedList[closedList.length-1];
						child.ag = closedList[closedList.length-1].ag + Vector3.Distance(closedList[closedList.length-1].Pos, child.Pos);
						child.ah = Vector3.Distance(child.Pos, dest);
						child.af = child.ag + child.ah;
						opendList.Push(child);
					}
 
					// If current node can see target
					Dir = dest - child.Pos;
					Dir.Normalize();
					if((Physics.Raycast(child.Pos, Dir, Vector3.Distance(child.Pos, dest), layerMask)) == false)
					{
						// Remember this node 
						if(tempNode == null)
						{
							tempNode = child;
						}
						else
						{
							// If another child node has lower f value  
							if(child.af < tempNode.af)
							{
								tempNode = child;
							}
						}
					}
				}				
			}
		}
 
		// if there was any node, which was able to see destination position
		if(tempNode != null)
		{
			//  Push destination coordinates to the path list			
			pathList.Push(dest);
 
			// Start from the destination node, go from each node to its 
			// creator node until we reach the starting node
			var next : Waypoint = tempNode;
			while(next)
			{
				pathList.Unshift(next.Pos);
				next = next.creator; 					
			}
 
			resultPath = 1;
		}
		else // Path doesn't exist
		{
			resultPath = 0;
		}
	}
 
	return resultPath;
}
 
function OnDrawGizmos()
{
	if((pathList.length != 0) && drawPathLines == true)
	{	
		Gizmos.color = Color.green;
		for (var i : int =0; i < pathList.length-1; i++) 
		{
			Gizmos.DrawLine(pathList[i], pathList[i+1]); 
		}
	}
}
}
