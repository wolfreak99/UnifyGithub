// Original url: http://wiki.unity3d.com/index.php/TileSelector
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/TileSelector.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{

Note that the most up-to-date version of this script can be found on my blog: http://cjcurrie.net/blog/2010/10/tileset-selector 
Author: CJCurrie 
Contents [hide] 
1 Description 
2 Usage 
2.1 Step 1 
2.2 Step 2 
2.3 Step 3 
2.4 Step 4 
2.5 Step 5 
2.6 Step N 

Description This is a helper class that makes it a lot easier to pick an individual image from a massive tileset, and apply that image to a sprite. An editor script allows you to pick out an image of any rectangular size from a given tileset and apply it as the mainTexture of Renderer. The point of this script is to avoid having to take a large source tileset and chop it up via Photoshop into a hundred smaller, more obnoxious files. 
 
Usage The TileSet comprises three scripts. TileManagerEditor and TileSelectorEditor are Editor scripts and must be placed in the Editor folder of your project hierarchy. TileManager is the base script that kicks off the magic, and is a component attached to the sprite you want to texture. 
Step 1 Create a GameObject (frequently a plane) and attach a material (frequently with a Transparent Cutout shader) to it. 
Step 2 Attach this script to the GameObject: 
using UnityEngine;
using UnityEditor;
 
public class TileManager : MonoBehaviour {  
 
 
 
}Note that this is an empty class. 
Step 3 Place these two scripts in the Editor folder: 
/*
	Copyright 2010 CJ Currie
	@Date: October 2010
	@Contact: CJCurrie@BlackStormsStudios.com
	@Script: A custom Inspector and popout window for choosing tiles from a tileset
	@Connections: 
	@TODO: 
*/
 
using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor (typeof(TileManager))]
public class TileManagerEditor : Editor {
 
 public static Texture2D tileset;
 int tileSize = 16;
 
 public override void OnInspectorGUI()
 {
  tileset = (Texture2D)EditorGUILayout.ObjectField("Current tileset", tileset, typeof(Texture2D));
 
  GUILayout.BeginHorizontal();
  GUILayout.Label(" Tile Size ");
  tileSize = EditorGUILayout.IntField(tileSize, GUILayout.Width(20));
  GUILayout.EndHorizontal();
 
  if (GUILayout.Button("Open Tile Selector"))
  {
    //tileset = (Texture2D)((TileManager)target).renderer.material.mainTexture;
    TileSelectorEditor window = (TileSelectorEditor) EditorWindow.GetWindow (typeof (TileSelectorEditor));
    window.Init(this, tileSize, tileset);
  }
 }
 
 // =================
 //  Remotely Called
 // =================
 public void SetTile (int x, int y, int width, int height)
  {
    if (!tileset)
      return;
 
    width++;    // Since the EditorWindow uses 0-based, and we can't have zeroes
    height++;
    int tHeight = tileset.height;
    y += - 2 + height;		// Do a little offsetting because the way the pixels are mapped is weird
    y = (tHeight/tileSize) - y;    // Invert since GetPixels is bottom-based
 
    Color[] pixels = tileset.GetPixels (x*tileSize, y*tileSize, width*tileSize, height*tileSize);
    Texture2D newTex = new Texture2D (width*tileSize, height*tileSize);
    newTex.SetPixels(pixels);
    newTex.mipMapBias = -10;
    newTex.Apply(false);
 
    		// @TODO: Suppress this intentional error
	  ((TileManager)target).renderer.material.mainTexture = newTex;
  }
}and 
/*
	Copyright 2010 CJ Currie
	@Date: October 2010
	@Contact: CJCurrie@BlackStormsStudios.com
	@Script: Creates the window that chooses the tiles and submits them to the TileManagerEditor.
	@Connections: 
	@TODO: 
*/
 
using UnityEngine;
using UnityEditor;
 
public class TileSelectorEditor : EditorWindow {
 
  // --- Cache ---
  public Texture2D selectionBox;
 
  // --- Init Values ---
  TileManagerEditor target;
  Vector2 scrollview;
 
  // --- Script vars ---
  Texture2D tileset = null;
  int tileSize;
  Vector2 mousePos;
  float zoom = 1.5f;
  //int[] numberOfTiles = new int[2];
  int[] selectedStart = new int[2];   // The top-left-most tile selected
  int horizTilesSelected;   // How many tiles wide the selection is
  int vertTilesSelected;    // How many tiles tall
  bool isDragging;
  int padding_top = 2;
 
 public void Init(TileManagerEditor incTarget, int incTileSize, Texture2D inctileset)
 {
  wantsMouseMove = true;
 
  target = (TileManagerEditor)incTarget;
  tileSize = incTileSize;
  //numberOfTiles[0] = tileset.width / tileSize;
  //numberOfTiles[1] = tileset.height / tileSize;
  tileset = inctileset;
 }
 
 
  void OnGUI()
  {
    if (!tileset)
    {
      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("NO TILESET SELECTED");
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();
      return;
    }
 
    // Zoom controls
   GUILayout.BeginHorizontal(GUILayout.Width(300), GUILayout.Height(tileSize*padding_top*zoom));
    GUILayout.Label (" Zoom ");
    // Check that we're not zooming too far, too
    if (GUILayout.Button(" + ") && zoom < 4)
    {zoom += .5f;}
    else if (GUILayout.Button(" -  ") && zoom > 1)
    {zoom -= .5f;}
 
    GUILayout.Space(50);
 
    // Submit changes
    if (GUILayout.Button("Set Sprite", GUILayout.Width(200)))
    {
      target.SetTile(selectedStart[0], selectedStart[1], horizTilesSelected, vertTilesSelected);   // So we calculate from the bottom of the padding
    }
 
    GUILayout.Label("Selected: ("+selectedStart[0]+","+selectedStart[1]+") to ("+(selectedStart[0]+horizTilesSelected)+","+(selectedStart[1]+vertTilesSelected)+")" );
 
   GUILayout.EndHorizontal();
 
    // -----------------
    // Begin scroll view
    // Make the scroll bar
    scrollview = EditorGUILayout.BeginScrollView(scrollview, GUILayout.Width (position.width), GUILayout.Height (position.height));
    // Round to the nearest tileSize. Dividing and then multiplying by the same number forces scrollview to be a multiple of tileSize
    scrollview.x = (int)(scrollview.x/tileSize/zoom);
    scrollview.x *= tileSize*zoom;
    scrollview.y = (int)(scrollview.y/tileSize/zoom);
    scrollview.y *= tileSize*zoom;
 
    // Show the tileset
    GUILayout.Label("", GUILayout.Width(tileset.width * zoom), GUILayout.Height(tileset.height * zoom));
    GUI.DrawTexture(new Rect(0,0,tileset.width * zoom,tileset.height * zoom), tileset);
 
    // End scroll view
   EditorGUILayout.EndScrollView ();
   // ------------------
 
    Event e = Event.current;
    int[] currentPos = new int[2];
    currentPos[0] = (int)((e.mousePosition.x + scrollview.x) / tileSize / zoom);
    currentPos[1] = (int)((e.mousePosition.y + scrollview.y) / tileSize / zoom);
 
    // --- Tile select  ---
 
    if (isDragging)
    {
      // We're back on the first tile
      if (currentPos[0] == selectedStart[0])
        horizTilesSelected = 0;
      if (currentPos[1] == selectedStart[1])
        vertTilesSelected = 0;
 
      if (currentPos != selectedStart)
      {      
        // Are we to the left of the selectedStart?
        if (currentPos[0] < selectedStart[0])
        {
          horizTilesSelected ++;    // Change how many tiles wide the selection is
          selectedStart[0] = currentPos[0];   // Change the selected start
        }
        // Else, if we are to the right, change width
        else if (selectedStart[0] < currentPos[0])
          horizTilesSelected = currentPos[0] - selectedStart[0];    // Change the width
 
        // Are we above the selectedStart?
        if (currentPos[1] < selectedStart[1])
        {
          vertTilesSelected = selectedStart[1] - currentPos[1];   // Change how many tiles tall the selection is
          selectedStart[1] = currentPos[1];   // Change the selected start
        }
        // Else, if we are below, change height
        else if (selectedStart[1] < currentPos[1])
          vertTilesSelected = currentPos[1] - selectedStart[1];
        }
    }
 
    if (e.type == EventType.MouseDown && e.button == 0)   // Left mouse button down - first selection
    {    
      selectedStart[0] = currentPos[0];
      selectedStart[1] = currentPos[1];
      horizTilesSelected = 0;
      vertTilesSelected = 0;
 
      isDragging = true;
    }
    else if (isDragging && e.type == EventType.MouseUp && e.button == 0)
    {
      isDragging = false;
    }
 
    int[] offset = new int[2];
    offset[0] = (int)(scrollview.x / tileSize / zoom);
    offset[1] = (int)(scrollview.y / tileSize / zoom);
 
    // Selection Indicator
    GUI.DrawTexture(new Rect((selectedStart[0]-offset[0]) * zoom * tileSize, (selectedStart[1]-offset[1]) * zoom * tileSize, tileSize*zoom * (horizTilesSelected+1), 
      tileSize*zoom * (vertTilesSelected+1)), selectionBox);
 
    // --- If we're in the window, redraw it
    if (EditorWindow.mouseOverWindow == this)
    {
      this.Repaint();
    }
  }
}Once the script compiles, set the default "Selection Box" reference image by clicking on the script and dragging the image to the exposed variable. I used a simple one I made in Photoshop:  Note that it will be stretched over your selection. 
Step 4 Drag your main tileset from your hierarchy to the "Current Tileset" variable exposed in the Tile Manager component of your GameObject from Step 1. Set the "Tile Size" variable exposed directly below it. This is the square root size in pixels of each individual tile. For example, a value of "16" indicates that each tile in your tileset is 16 pixels by 16 pixels (16x16). 
Step 5 Click "Open Tile Selector". An EditorWindow will pop up with your tileset. Use the Zoom buttons to get closer or further from your tileset. Click and drag to select the area you want on your sprite. Click "Set Sprite" to apply the cropped image. You're done! 
Step N To skip all the setup, just duplicate the sprite you made above, or work from a prefab, and the process becomes much faster. 
}
