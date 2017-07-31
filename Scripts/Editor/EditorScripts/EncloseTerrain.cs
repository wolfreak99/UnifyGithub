// Original url: http://wiki.unity3d.com/index.php/EncloseTerrain
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/EncloseTerrain.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Description An easy and quick script for encapsulating your terrain in a wall. 
Future: More advanced versions to come... 
/*
 * Written by: Matt Greene (aka Deis)
 * Last Revision: 3/11/2010 12:50 EST
 *
 * A very simply script for encapsulating your terrain
 * in a wall. I suggest combining this script with others
 * such as the Terrain Toolkit and blending.
 * 
 * NOTICE: You need to manually set your Height and Depth
 * in this script for now.
 * 
 */
 
using UnityEngine;
using UnityEditor;
 
public class EncloseTerrain : MonoBehaviour
{
    // The desired height of the wall
    public static float EncloseHeight = 5;
    // How thick the wall needs to be
    public static float EncloseDepth = 10;
 
    [MenuItem("CONTEXT/Terrain/Enclose")]
    [MenuItem("CONTEXT/TerrainData/Enclose")]
    [MenuItem("Terrain/Enclose")]
    public static void Enclose(MenuCommand command)
    {
        TerrainData terrainData;
 
        if (command.context is TerrainData)
            terrainData = (TerrainData) command.context;
        else if (command.context is Terrain)
            terrainData = ((Terrain) command.context).terrainData;
        else
            terrainData = Terrain.activeTerrain.terrainData;
 
        Undo.RegisterUndo(terrainData, "Enclose Terrain");
 
        EditorUtility.DisplayProgressBar("Enclose Terrain", "Initializing", 0.0f);
 
 
        int w = terrainData.heightmapWidth;
        int h = terrainData.heightmapHeight;
        float size = terrainData.size.y;
        float[,] heights = terrainData.GetHeights(0, 0, w, h);
 
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                if (x < EncloseDepth || x > w - EncloseDepth)
                {
                    heights[x, y] = EncloseHeight / size;
                }
 
                if (y < EncloseDepth || y > h - EncloseDepth)
                {
                    heights[x, y] = EncloseHeight / size;
                }
            }
        }
 
        terrainData.SetHeights(0, 0, heights);
 
        EditorUtility.DisplayProgressBar("Enclose Terrain", "Finalizing", 1.0f);
        EditorUtility.ClearProgressBar();
    }
}
}
