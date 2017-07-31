// Original url: http://wiki.unity3d.com/index.php/Normalize
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/Normalize.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Charles Hinshaw 
DescriptionThis editor script adds the ability to normalize a selected terrain heightmap. 
Javascript - Normalize.js @MenuItem ("CONTEXT/Terrain/Normalize")
@MenuItem ("CONTEXT/TerrainData/Normalize")
@MenuItem ("Terrain/Normalize")
static function Normalize (command) {
	// if this was a context call, get the appropriate TerrainData, otherwise get it from activeTerrain
	if (typeof(command.context) == UnityEngine.TerrainData){
		var data = command.context;
	} else if (typeof(command.context) == UnityEngine.Terrain){
		data = command.context.terrainData;
	} else {
		data = Terrain.activeTerrain.terrainData;	
	}
	// give them something to see
	EditorUtility.DisplayProgressBar("Normalizing Terrain", "Initializing", 0.0);
	var w = data.heightmapWidth;
	var h = data.heightmapHeight;
	var heights = data.GetHeights(0,0,w,h);
	var stepSize = 1.0/(w*h*2);
	var complete = 0.0;
	var min = 100000.0;
	var max = -100000.0;
	var lastComplete = 0.0;
	// get the min and max values for this terrain
	for (var i = 0; i < heights.length; i++){
		if (heights[i] > max) max = heights[i];
		if (heights[i] < min) min = heights[i];
		complete += stepSize;
		if (complete > lastComplete + 0.1){
			lastComplete = complete;
			EditorUtility.DisplayProgressBar("Normalizing Terrain", "Getting Min and Max", complete);
		}
	}
	// normalize the terrain
	for (i = 0; i < heights.length; i++){
		heights[i] = (heights[i]-min)/max;
		complete += stepSize;
		if (complete > lastComplete + 0.1){
			lastComplete = complete;
			EditorUtility.DisplayProgressBar("Normalizing Terrain", "Normalizing TerrainData", complete);
		}
	}
	// set the terrain heights
	EditorUtility.DisplayProgressBar("Normalizing Terrain", "Setting Terrain Heights", 1.0);
	data.SetHeights(0,0,heights);
	// clear the progress bar
	EditorUtility.ClearProgressBar();
}C Sharp - Normalize.cs /*
 * Rewritten in C# by: Matt Greene (aka Deis)
 * Original Javascript Version Author: Charles Hinshaw
 * 
 * This script is an excellent comparison of C# versus JavaScript performance.
 * In all of my testing, this script performs exponentially faster when run
 * as C# in comparison to the Javascript version, even though they are next to
 * identical. A big thank you to Charles for the original JavaScript version.
 *
 * A note about the speed difference: this is because the JavaScript version is
 * using dynamic typing, which is significantly slower.  This does make the script
 * simpler, at the expense of speed.  If it was rewritten to use static typing, then
 * the speed would be similar.
 * 
 */
 
using UnityEngine;
using UnityEditor;
 
public class Normalize
{
    [MenuItem("CONTEXT/Terrain/Normalize")]
    [MenuItem("CONTEXT/TerrainData/Normalize")]
    [MenuItem("Terrain/Normalize")]
    public static void Something(MenuCommand command)
    {
        TerrainData data;
 
        if (command.context is TerrainData)
        {
            data = (TerrainData) command.context;
        }
        else if (command.context is Terrain)
        {
            data = ((Terrain) command.context).terrainData;
        }
        else
        {
            data = Terrain.activeTerrain.terrainData;
        }
 
        EditorUtility.DisplayProgressBar("Normalizing Terrain", "Initializing", 0.0f);
        int w = data.heightmapWidth;
        int h = data.heightmapHeight;
        float[,] heights = data.GetHeights(0, 0, w, h);
        float stepSize = (1.0f / (w*h*2));
        float complete = 0.0f;
        float min = 100000.0f;
        float max = -100000.0f;
        float lastComplete = 0.0f;
 
        // Get the min and max values for this terrain
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                if (heights[x, y] > max) max = heights[x, y];
                if (heights[x, y] < min) min = heights[x, y];
 
                complete += stepSize;
                if (complete > lastComplete + 0.1)
                {
                    lastComplete = complete;
                    EditorUtility.DisplayProgressBar("Normalizing Terrain", "Getting Min and Max", complete);
                }
            }
        }
 
 
        // normalize the terrain
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                heights[x, y] = (heights[x, y] - min) / max;
                complete += stepSize;
 
                if (complete > lastComplete + 0.1)
                {
                    lastComplete = complete;
                    EditorUtility.DisplayProgressBar("Normalizing Terrain", "Normalizing TerrainData", complete);
                }
            }
        }
 
        // set the terrain heights
        EditorUtility.DisplayProgressBar("Normalizing Terrain", "Setting Terrain Heights", 1.0f);
        data.SetHeights(0, 0, heights);
 
        // clear the progress bar
        EditorUtility.ClearProgressBar();
    }
}
}
