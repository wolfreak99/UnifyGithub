// Original url: http://wiki.unity3d.com/index.php/AStarHelper
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/GeneralConcepts/AStarHelper.cs
// File based on original modification date of: 12 September 2012, at 15:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
Author: Rod Green 


Contents [hide] 
1 Description 
2 Usage 
3 AStarHelper class 
4 IPathNode Interface 
5 Usage Example 
5.1 PathNode Class 
5.2 DrawHelper Class 
5.3 PathNodeTester Class 

Description A text book implementation of the A* network pathing routine. Based of the wikipedia detailing on the method. A* Wikipedia Article 
Usage The code is broken into a class (AStarHelper) and the base interface (IPathNode) needed for the Helper class 
See below for usage example (tester). 


AStarHelper classusing UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public static class AStarHelper
{
 
 
	// Validator for path nodes
	// Needed to cope with nodes that might be GameObjects and therefore
	// not 'acutally' null when compared in generic methods
	public static bool Invalid<T>(T inNode) where T: IPathNode<T>
	{
		if(inNode == null || inNode.Invalid)
			return true;
		return false;
	}
 
	// Distance between Nodes
	static float Distance<T>(T start, T goal) where T: IPathNode<T>
	{
		if(Invalid(start) || Invalid(goal))
			return float.MaxValue;
		return Vector3.Distance(start.Position, goal.Position);
	}
 
	// Base cost Estimate - this would need to be evoled for your project based on true cost
	// to move between nodes
	static float HeuristicCostEstimate<T>(T start, T goal) where T: IPathNode<T>
	{
		return Distance(start, goal);
	}
 
	// Find the current lowest score path
	static T LowestScore<T>(List<T> openset, Dictionary<T, float> scores) where T: IPathNode<T>
	{
		int index = 0;
		float lowScore = float.MaxValue;
 
		for(int i = 0; i < openset.Count; i++)
		{
			if(scores[openset[i]] > lowScore)
				continue;
			index = i;
			lowScore = scores[openset[i]];
		}
 
		return openset[index];
	}
 
 
	// Calculate the A* path
	public static List<T> Calculate<T>(T start, T goal) where T: IPathNode<T>
	{
		List<T> closedset = new List<T>();    // The set of nodes already evaluated.
		List<T> openset = new List<T>();    // The set of tentative nodes to be evaluated.
		openset.Add(start);
     	Dictionary<T, T> came_from = new Dictionary<T, T>();    // The map of navigated nodes.
 
		Dictionary<T, float> g_score = new Dictionary<T, float>();
		g_score[start] = 0.0f; // Cost from start along best known path.
 
		Dictionary<T, float> h_score = new Dictionary<T, float>();
		h_score[start] = HeuristicCostEstimate(start, goal); 
 
		Dictionary<T, float> f_score = new Dictionary<T, float>();
		f_score[start] = h_score[start]; // Estimated total cost from start to goal through y.
 
		while(openset.Count != 0)
		{
			T x = LowestScore(openset, f_score);
			if(x.Equals(goal))
			{
				List<T> result = new List<T>();
				ReconstructPath(came_from, x, ref result);
				return result;
			}
			openset.Remove(x);
			closedset.Add(x);
			foreach(T y in x.Connections)
			{
				if(AStarHelper.Invalid(y) || closedset.Contains(y))
					continue;
				float tentative_g_score = g_score[x] + Distance(x, y);
 
				bool tentative_is_better = false;
				if(!openset.Contains(y))
				{
					openset.Add(y);
					tentative_is_better = true;
				}
				else if (tentative_g_score < g_score[y])
					tentative_is_better = true;
 
				if(tentative_is_better)
				{
					came_from[y] = x;
					g_score[y] = tentative_g_score;
					h_score[y] = HeuristicCostEstimate(y, goal);
					f_score[y] = g_score[y] + h_score[y];
				}
			}
		}
 
     return null;
 
	}
 
	// Once the goal has been found we now reconstruct the steps taken to get to the path
	static void ReconstructPath<T>(Dictionary<T, T> came_from, T current_node, ref List<T> result) where T: IPathNode<T>
	{
		if(came_from.ContainsKey(current_node))
		{
			ReconstructPath(came_from, came_from[current_node], ref result);
			result.Add(current_node);
			return;
		}
		result.Add(current_node);
	}
}IPathNode Interfaceusing UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
 
public interface IPathNode<T>
{
	List<T> Connections { get; }
	Vector3 Position { get; }
	bool Invalid {get;}
}Usage ExampleThis is a very quick and dirty implementation of the AStartHelper class. It's really meant to allow fast debugging of the routine and not a 'true' implementation of the technique into a game library. 
PathNode class (MonoBehaviour) is an implementation of the IPathNode interface 
PathNodeTester (MonoBehaviour) is an object that's used to handle the globals and execution of the path calculations 
PathNode Classusing UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class PathNode : MonoBehaviour, IPathNode<PathNode>
{
	public List<PathNode> connections;
 
	public static int pnIndex;
 
	public Color nodeColor = new Color(0.05f, 0.3f, 0.05f, 0.1f);
 
	public bool Invalid
	{
		get { return (this == null); }
	}
 
	public List<PathNode> Connections
	{
		get { return connections; }
	}
 
	public Vector3 Position
	{
		get
		{
 
			return transform.position;
		}
	}
 
	public void Update()
	{
		DrawHelper.DrawCube(transform.position, Vector3.one, nodeColor );
		if(connections == null)
			return;
		for(int i = 0; i < connections.Count; i++)
		{
			if(connections[i] == null)
				continue;
			Debug.DrawLine(transform.position, connections[i].Position, nodeColor);
		}
	}
 
	public void Awake()
	{
		if(connections == null)
			connections = new List<PathNode>();
 
	}
 
	public static PathNode Spawn(Vector3 inPosition)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		obj.name = "pn_" + pnIndex;
		obj.transform.position = inPosition;
		pnIndex++;
 
		PathNode newNode = obj.AddComponent<PathNode>();
		return newNode;
	}
 
	public static List<PathNode> CreateGrid(Vector3 center, Vector3 spacing, int[] dim, float randomSpace)
	{
		GameObject groupObject = new GameObject("grid");
		Random.seed = 1337;
		int xCount = dim[0];
		int yCount = dim[1];
		float xWidth = spacing.x * xCount;
		float yWidth = spacing.z * yCount;
 
		float xStart = center.x - (xWidth / 2.0f) + (spacing.x / 2.0f);
		float yStart = center.z - (yWidth / 2.0f) + (spacing.z / 2.0f);
 
		List<PathNode> result = new List<PathNode>();
 
		for(int x = 0; x < xCount; x++)
		{
			float xPos = (x * spacing.x) + xStart;
 
			for(int y = 0; y < yCount; y++)
			{
				if(randomSpace > 0.0f)
				{
					if(Random.value <= randomSpace)
					{
						result.Add(null);
						continue;
 
					}
				}
				float yPos = (y * spacing.z) + yStart;
				PathNode newNode = Spawn(new Vector3(xPos, 0.0f, yPos));
 
				result.Add(newNode);
				newNode.transform.parent = groupObject.transform;
 
			}
		}
 
		for(int x = 0; x < xCount; x++)
		{
 
 
			for(int y = 0; y < yCount; y++)
			{
				int thisIndex = (x * yCount) + y;
				List<int> connectedIndicies = new List<int>();
				PathNode thisNode = result[thisIndex];
				if(AStarHelper.Invalid(thisNode)) continue;
				if(x != 0)
					connectedIndicies.Add(((x - 1) * yCount) + y);
				if(x != xCount - 1)
					connectedIndicies.Add(((x + 1) * yCount) + y);
				if(y != 0)
					connectedIndicies.Add((x * yCount) + (y - 1));
				if(y != yCount - 1)
					connectedIndicies.Add((x * yCount) + (y + 1));
 
				if(x != 0 && y != 0)
					connectedIndicies.Add(((x - 1) * yCount) + (y - 1));
 
				if(x != xCount - 1 && y != yCount - 1)
					connectedIndicies.Add(((x + 1) * yCount) + (y + 1));
 
				if(x != 0 && y != yCount - 1)
					connectedIndicies.Add(((x - 1) * yCount) + (y + 1));
 
				if(x != xCount - 1 && y != 0)
					connectedIndicies.Add(((x + 1) * yCount) + (y - 1));
 
				for(int i = 0; i < connectedIndicies.Count; i++)
				{
					PathNode thisConnection = result[connectedIndicies[i]];
					if(AStarHelper.Invalid(thisConnection))
						continue;
					thisNode.Connections.Add(thisConnection);
				}
 
			}
		}
 
		return result;
 
	}
}

DrawHelper Classusing System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DrawHelper
{
	public static void DrawCube(Vector3 position, Vector3 size, Color color)
	{
		Vector3 leftFrontDown 	= new Vector3( -size.x / 2.0f, -size.y / 2.0f, -size.z / 2.0f );
		Vector3 rightFrontDown 	= new Vector3( 	size.x / 2.0f, -size.y / 2.0f, -size.z / 2.0f );
		Vector3 rightFrontUp 	= new Vector3( 	size.x / 2.0f, 	size.y / 2.0f, -size.z / 2.0f );
		Vector3 leftFrontUp 	= new Vector3( -size.x / 2.0f, 	size.y / 2.0f, -size.z / 2.0f );
 
		Vector3 leftBackDown 	= new Vector3( -size.x / 2.0f, -size.y / 2.0f, size.z / 2.0f );
		Vector3 rightBackDown 	= new Vector3( 	size.x / 2.0f, -size.y / 2.0f, size.z / 2.0f );
		Vector3 rightBackUp 	= new Vector3( 	size.x / 2.0f, 	size.y / 2.0f, size.z / 2.0f );
		Vector3 leftBackUp 		= new Vector3( -size.x / 2.0f, 	size.y / 2.0f, size.z / 2.0f );
 
		Vector3[] arr = new Vector3[8];
 
		arr[0] = leftFrontDown;
		arr[1] = rightFrontDown;
		arr[2] = rightFrontUp;
		arr[3] = leftFrontUp;
 
		arr[4] = leftBackDown;
		arr[5] = rightBackDown;
		arr[6] = rightBackUp;
		arr[7] = leftBackUp;
 
		for (int i = 0; i < arr.Length; i++)
			arr[i] += position;
 
		for (int i = 0; i < arr.Length; i++)
		{
			for (int j = 0; j < arr.Length; j++)
			{
				if (i != j)
				{
					Debug.DrawLine(arr[i], arr[j], color);	
				}
			}
		}
	}
}PathNodeTester Classusing UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
 
public class PathNodeTester : MonoBehaviour
{
	public List<PathNode> sources;
	public GameObject start;
	public GameObject end;
	public Color nodeColor = new Color(0.05f, 0.3f, 0.05f, 0.1f);
	public Color pulseColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
	public Color pathColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
	public bool reset;
 
	public bool gridCreated;
	int startIndex;
	int endIndex;
 
	int lastEndIndex;
	int lastStartIndex;
 
	bool donePath;
	public List<PathNode> solvedPath = new List<PathNode>();
 
 
 
	public void Awake()
	{
 
		if(gridCreated)
			return;
		sources = PathNode.CreateGrid(Vector3.zero, Vector3.one * 5.0f, new int[] { 50, 50}, 0.0f);
		gridCreated = true;
 
	}
 
	public void PulsePoint(int index)
	{
		if(AStarHelper.Invalid(sources[index]))
			return;
		DrawHelper.DrawCube(sources[index].Position, Vector3.one * 2.0f, pulseColor);
	}
 
 
	public void Draw(int startPoint, int endPoint, Color inColor)
	{
		Debug.DrawLine(sources[startPoint].Position, sources[endPoint].Position, inColor);
	}
 
	static int Closest(List<PathNode> inNodes, Vector3 toPoint)
	{
		int closestIndex = 0;
		float minDist = float.MaxValue;
		for(int i = 0; i < inNodes.Count; i++)
		{
			if(AStarHelper.Invalid(inNodes[i]))
				continue;
			float thisDist = Vector3.Distance(toPoint, inNodes[i].Position);
			if(thisDist > minDist)
				continue;
 
			minDist = thisDist;
			closestIndex = i;
		}
 
		return closestIndex;
	}
 
 
	public void Update()
	{
		if(reset)
		{
			donePath = false;
			ArrayFunc.Clear(ref solvedPath);
			reset = false;
		}
 
		if(start == null || end == null)
		{
			Debug.LogWarning("Need 'start' and or 'end' defined!");
			enabled = false;
			return;
		}
 
		startIndex = Closest(sources, start.transform.position);
 
		endIndex = Closest(sources, end.transform.position);
 
 
		if(startIndex != lastStartIndex || endIndex != lastEndIndex)
		{
			reset = true;
			lastStartIndex = startIndex;
			lastEndIndex = endIndex;
			return;
		}
 
		for(int i = 0; i < sources.Count; i++)
		{
			if(AStarHelper.Invalid(sources[i]))
				continue;
			sources[i].nodeColor = nodeColor;
		}
 
		PulsePoint(lastStartIndex);
		PulsePoint(lastEndIndex);
 
 
		if(!donePath)
		{
 
			solvedPath = AStarHelper.Calculate(sources[lastStartIndex], sources[lastEndIndex]);
 
			donePath = true;
		}
 
		// Invalid path
		if(solvedPath == null || solvedPath.Count < 1)
		{
			Debug.LogWarning("Invalid path!");
			reset = true;
			enabled = false;
			return;
		}
 
 
		//Draw path	
		for(int i = 0; i < solvedPath.Count - 1; i++)
		{
			if(AStarHelper.Invalid(solvedPath[i]) || AStarHelper.Invalid(solvedPath[i + 1]))
			{
				reset = true;
 
				return;
			}
			Debug.DrawLine(solvedPath[i].Position, solvedPath[i + 1].Position, Color.cyan * new Color(1.0f, 1.0f, 1.0f, 0.5f)); 
		}
 
 
 
	}
 
}
}
