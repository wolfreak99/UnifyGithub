// Original url: http://wiki.unity3d.com/index.php/TileManager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/TileManager.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
A simple tile editor script. 
Here's how you use it, first import those two scripts, the first one can be anywhere, you probably want to place it with your game scripts. 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class Tiles : MonoBehaviour
{
	//the size of the grid, sometimes non-square grid may be useful
	public float width = 32.0f;
	public float height = 32.0f;
 
	//offset, just in case someone wants to adjust the position of the grid
	public float offsetX = 0.0f;
	public float offsetY = 0.0f;
 
	//the color of the lines, someone it has to be adjusted for better visibility
	public Color color = Color.white;
 
	//shortcuts 
	public string drawKey = "";
	public string deleteKey = "";
	public string disableKey = "";
	public string alignKey = "";
	public string setParentKey = "";
 
	//depth += 1.0f shortcut
	public string incDepthKey = "";
	//depth -= 1.0f shortcut
	public string decDepthKey = "";
 
	//depth at which the tiles are placed
	public float depth = 0.0f;
 
	//objects' offset from grid
	public float objOffsetX = 0.0f;
	public float objOffsetY = 0.0f;
 
	//is tile placing enabled
	public bool enabled = false;
 
	//the tiles' parent object
	public Transform parent;
 
	void OnDrawGizmos()
	{
                if (!enabled)
	            return;
 
		//Camera.current gets us the sceneview's camera
		Camera c = Camera.current;
		Vector3 cPos = c.transform.position;
 
		Gizmos.color = color;
 
		//draw horizontal lines... I cheat length and how many lines are drawn a bit here, ugly but works
		//float.MinValue, float.MaxValue and float.NegativeInfinity, float.PositiveInfinity didn't work
		for (float y = cPos.y - c.orthographicSize*4.0f; y < cPos.y + c.orthographicSize*4.0f; y+= height)
		{
 
			Gizmos.DrawLine(new Vector3(-1000000.0f, Mathf.Floor(y/height) * height + offsetY, 0.0f), 
							new Vector3(1000000.0f, Mathf.Floor(y/height) * height + offsetY, 0.0f));
		}
 
		//pretty much the same thing for the vertical lines
		for (float x = cPos.x - c.orthographicSize*4.0f; x < cPos.x + c.orthographicSize*4.0f; x+= height)
		{
 
			Gizmos.DrawLine(new Vector3(Mathf.Floor(x/width) * width + offsetX, -1000000.0f, 0.0f), 
							new Vector3(Mathf.Floor(x/width) * width + offsetX, 1000000.0f, 0.0f));
		}
	}
}The second script you need to place in the Editor folder, it extends the Tiles class. 
using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor (typeof(Tiles))]
public class TilesEditor : Editor
{
	Tiles tileMgr;
 
	//last aligned pos, we'll use it to _NOT_ create multiple tiles in the same place
	bool lastPosSet = false;
	Vector3 lastPos;
 
	//have any tiles been moved? We'll use that for Undo
	bool moved = false;
 
	//have tiles been painted? We'll use that for Undo
	bool painted = false;
 
	//have tiles been deleted? We'll use that for Undo
	bool deleted = false;
 
	void OnEnable()
	{
		tileMgr = (Tiles)target;
		SceneView.onSceneGUIDelegate += UpdateTiles;
	}
 
	void UpdateTiles(SceneView sceneview)
	{
		//get the current event
		Event e = Event.current;
 
		//if the toggle key is set and was pressed, change then toggle activation
		if ((tileMgr.disableKey != "") && (e.isKey && e.character == tileMgr.disableKey[0]))
			tileMgr.enabled = false;
 
		//if disabled, return
		if (tileMgr.enabled == false)
			return;
 
		//calculating the world space mouse position in the scene view
		Vector3 tmp = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
		Ray r = Camera.current.ScreenPointToRay(tmp);
		Vector3 mousePos = new Vector3(r.origin.x, r.origin.y, tileMgr.depth);
 
		//aligning to grid
		Vector3 aligned = new Vector3(
		Mathf.Floor((mousePos.x - tileMgr.offsetX)/tileMgr.width)*tileMgr.width + tileMgr.width/2.0f + (tileMgr.offsetX) + tileMgr.objOffsetX,
		Mathf.Floor((mousePos.y - tileMgr.offsetY)/tileMgr.height)*tileMgr.height + tileMgr.height/2.0f + (tileMgr.offsetY) + tileMgr.objOffsetY,
					tileMgr.depth);
 
		//if drawKey is set then draw the aligned tile
		if ((tileMgr.setParentKey != "") && (e.isKey && e.character == tileMgr.setParentKey[0]))
		{
			if (Selection.activeObject != null)
				tileMgr.parent = (Transform)Selection.activeObject;
		}
		else if ((tileMgr.drawKey != "") && (e.isKey && e.character == tileMgr.drawKey[0]))
		{	
			if ((lastPos == aligned) && lastPosSet)
				return;
 
			if (Selection.activeObject == null)
				return;
 
			if (!painted)
			{
				Undo.IncrementCurrentEventIndex(); //Create undo state for the tile painting
				painted = true;		
			}
 
			//if there's already a tile, delete it
			if (tileMgr.parent != null)
			{
				foreach (Transform child in tileMgr.parent)
				{
					//we don't care about depth while deleting
					if (child.position.x == aligned.x && child.position.y == aligned.y)
					{
						DestroyImmediate(child.gameObject);
						break;
					}
				}
			}
 
			//our new tile
			GameObject obj;
 
			//get the tile's prefab
			Object prefab = EditorUtility.GetPrefabParent(Selection.activeObject);
 
			//if prefab exists, create from prefab, if not the simply clone
			if (prefab)
			{
                                obj = (GameObject)EditorUtility.InstantiatePrefab(prefab);
				obj.transform.localScale = Selection.activeTransform.localScale;
				obj.transform.eulerAngles = Selection.activeTransform.eulerAngles;
			}
			else
				obj = (GameObject)Instantiate(Selection.activeObject);
 
			Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
			//set the tile's position and parent
			obj.transform.position = aligned;
			obj.transform.parent = tileMgr.parent;
 
			//Debug.Log("Created Tile at " + aligned); //logging where the tile was placed in world space
			lastPos = aligned;
			lastPosSet = true;
		}
		else if ((tileMgr.deleteKey != "") && (e.isKey && e.character == tileMgr.deleteKey[0]))
		{
			//we delete only inside the parent object, it could be a mess to use hierarchy for that
			if (tileMgr.parent != null)
			{
				if (!deleted)
				{
					Undo.IncrementCurrentEventIndex();
					Undo.RegisterSceneUndo("Delete Tiles");
					deleted = true;
				}
 
				foreach (Transform child in tileMgr.parent)
				{
					//we don't care about depth while deleting
					if (child.position.x == aligned.x && child.position.y == aligned.y) 
					{
						DestroyImmediate(child.gameObject);
						break;
					}
				}
 
			}
		}
		else if ((tileMgr.alignKey != "") && (e.isKey && e.character == tileMgr.alignKey[0]))
		{	
			//we align only if there are selected objects
			if (Selection.transforms.Length > 0)
			{
				if (!moved)
				{
					Undo.IncrementCurrentEventIndex();
					Undo.RegisterSceneUndo("Move Tiles");
					moved = true;
				}
 
				Vector3 posOffset = aligned - Selection.transforms[0].position;
				Selection.transforms[0].position = aligned;
 
				for (int i = 1; i < Selection.transforms.Length; ++i)
					Selection.transforms[i].position += posOffset;
			}
		}
		else if ((tileMgr.incDepthKey != "") && (e.isKey && e.character == tileMgr.incDepthKey[0]))
		{
			tileMgr.depth += 1.0f;
			Debug.Log("Now placing tiles at Z equal to " + tileMgr.depth);
		}
		else if ((tileMgr.decDepthKey != "") && (e.isKey && e.character == tileMgr.decDepthKey[0]))
		{
			tileMgr.depth -= 1.0f;
			Debug.Log("Now placing tiles at Z equal to " + tileMgr.depth);
		}
		else if (e.type == EventType.KeyUp)
		{
			//if the key is up and we were either moving, painting or deleting, the we se the appropiate variables
			//to their appropiate values
			if (moved && tileMgr.parent)
			{
				moved = false;
 
				//we need to delete the tiles that has been replaced
				for (int i = 0; i < Selection.transforms.Length; ++i)
				{
					foreach (Transform child in tileMgr.parent)
					{
						if (Selection.transforms[i] == child)
							continue;
 
						if (Selection.transforms[i].position == child.position)
						{
							DestroyImmediate(child.gameObject);
							break;
						}
					}
				}
			}
			else if (painted)
			{
				painted = false;
			}
			else if (deleted)
				deleted = false;
		}
 
	}
 
	//just a couple of simple fields
	public override void OnInspectorGUI()
 	{	
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Tile Width ");
		tileMgr.width = EditorGUILayout.FloatField(tileMgr.width, GUILayout.Width(50));
		GUILayout.Label(" Tile Height ");
		tileMgr.height = EditorGUILayout.FloatField(tileMgr.height, GUILayout.Width(50));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Grid Offset X ");
		tileMgr.offsetX = EditorGUILayout.FloatField(tileMgr.offsetX, GUILayout.Width(40));
		GUILayout.Label(" Grid Offset Y ");
		tileMgr.offsetY = EditorGUILayout.FloatField(tileMgr.offsetY, GUILayout.Width(40));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Object Offset X ");
		tileMgr.objOffsetX = EditorGUILayout.FloatField(tileMgr.objOffsetX, GUILayout.Width(30));
		GUILayout.Label(" Object Offset Y ");
		tileMgr.objOffsetY = EditorGUILayout.FloatField(tileMgr.objOffsetY, GUILayout.Width(30));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Grid Color ");
		tileMgr.color = EditorGUILayout.ColorField(tileMgr.color, GUILayout.Width(150));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Z Position ");
		tileMgr.depth = EditorGUILayout.FloatField(tileMgr.depth, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Draw Key ");
		tileMgr.drawKey = EditorGUILayout.TextField(tileMgr.drawKey, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Delete Key ");
		tileMgr.deleteKey = EditorGUILayout.TextField(tileMgr.deleteKey, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Disable Key ");
		tileMgr.disableKey = EditorGUILayout.TextField(tileMgr.disableKey, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Align Key ");
		tileMgr.alignKey = EditorGUILayout.TextField(tileMgr.alignKey, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Increase Depth Key ");
		tileMgr.incDepthKey = EditorGUILayout.TextField(tileMgr.incDepthKey, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Decrease Depth Key ");
		tileMgr.decDepthKey = EditorGUILayout.TextField(tileMgr.decDepthKey, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Is Enabled? ");
		tileMgr.enabled = EditorGUILayout.Toggle(tileMgr.enabled, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Tiles' Parent ");
		tileMgr.parent = (Transform)EditorGUILayout.ObjectField(tileMgr.parent, typeof(Transform), true, GUILayout.Width(100));
		GUILayout.EndHorizontal();
 
 
		//button to align the grid to the tiles' parent
		if (GUILayout.Button("Align Grid Offset With Parent", GUILayout.Width(255)))
		{	
			tileMgr.offsetX = tileMgr.parent.position.x;
			tileMgr.offsetY = tileMgr.parent.position.y;
		}
 
		//button to align the grid to the tiles' parent and offset it by 0.5 to deter renderer's .f point errors
		if (GUILayout.Button("Align Grid Offset With Parent + 0.5", GUILayout.Width(255)))
		{	
			tileMgr.offsetX = Mathf.Floor(tileMgr.parent.position.x) + 0.5f;
			tileMgr.offsetY = Mathf.Floor(tileMgr.parent.position.y) + 0.5f;
		}
 
		//repaint the scene to see the changes immediately
		SceneView.RepaintAll();
	}
}Here's how you use it: 
Attach a "Tiles" script to any object, then the grid will be drawn. In the inspector you can find those types of fields: 
   -Tile width, height -> those impact the size of your grid (in units).
   -Grid Offset X,Y -> this offsets the grid position (in units).
   -Object Offset X,Y -> this offsets the tiles from the center of the grid square
   -Grid Color            -> The color of the grid
   -Z position            -> the Z position at which the tiles will be placed
   -Draw Key              -> painta the selected tile on the grid
   -Delete Key            -> deletes a tile from the grid
   -Disable Key           -> disables the tile manager
   -Align Key             -> aligns anything selected to the grid
   -Set Parent Key        -> sets the parent of the created
   -Increase Depth Key    -> increases the Z position by one
   -Decrease Depth Key    -> decreases the Z position by one
   -Is Enabled?           -> enables or disables the manager
   -Tiles' Parent         -> a reference to a parent of the created tiles, if reference is not set the tiles will be created in the top of Hierarchy
Remember that you have to use keys when the scene view is active. Also, if you reimport the assets you need to select the tile manager for the script to work. 
There are also two buttons, the first one is for aligning the grid with exact parent position, which helps to align the grid when the parent of the tiles is moved together with them. The second one does the same thing but it also offsets the grid by 0.5, you may want to use that to solve the floating point errors with rendering 2D sprites if your parent isn't placed properly. 
}
