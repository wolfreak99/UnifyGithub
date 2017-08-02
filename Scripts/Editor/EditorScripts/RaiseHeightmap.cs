/*************************
 * Original url: http://wiki.unity3d.com/index.php/RaiseHeightmap
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/RaiseHeightmap.cs
 * File based on original modification date of: 15 January 2013, at 19:25. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * Description 
 *   
 * Usage 
 *   
 * JavaScript - RaiseHeightmap.js 
 *   
 * C# - AdjustHeights.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    
    Description Raises or lowers the terrain. Normally terrains are created near the lowest level possible, which in effect prevents you from digging valleys or trenches below that height. A work-around is to use Flatten Terrain from the Terrain menu before you start working on it to set the base terrain level to a greater height, which gives you "leg room" to dig down. But if you didn't do this, you're basically stuck unless you export the heightmap to an image editor and brighten it. This will effectively raise the terrain, but can be problematic. Fortunately, now you don't have to worry about that, since you can just use RaiseTerrain to raise or lower the entire terrain at any time. 
    Requires Unity 2.1 or later, but is slightly nicer with Unity 2.5. 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. The script also must be called "RaiseHeightmap" or it won't run. 
    To raise or lower a heightmap, click on the terrain you want and select Raise or Lower Heightmap... from the Terrain menu. If you don't select a terrain, the active terrain will be used (if one exists). The amount you can enter ranges from -1.0 through 1.0. 1.0 is the entire available vertical space available for the heightmap, so using that amount is basically the same thing as choosing Flatten Heightmap... and using a 100% height value. Since there's a fixed amount of vertical space available, raising or lowering too much can flatten part or all of your terrain. If this happens, undo and try a smaller amount. 
    JavaScript - RaiseHeightmap.js class RaiseHeightmap extends ScriptableWizard {
    	var addHeight = .1;
    	static var terrain : TerrainData;
     
    	@MenuItem ("Terrain/Raise or Lower Heightmap...")
    	static function CreateWizard () {
    		terrain = null;
    		var terrainObject : Terrain = (Selection.activeObject as GameObject).GetComponent(Terrain) as Terrain;
    		if (!terrainObject) {
    			terrainObject = Terrain.activeTerrain;
    		}
    		if (terrainObject) {
    			terrain = terrainObject.terrainData;
    			var buttonText = "Apply Height";
    		}
    		else {
    			buttonText = "Cancel";
    		}
    		ScriptableWizard.DisplayWizard("Raise/Lower Heightmap", RaiseHeightmap, buttonText);
    	}
     
    	function OnWizardUpdate () {
    		if (!terrain) {
    			helpString = "No terrain found";
    			return;
    		}
     
    		addHeight = Mathf.Clamp(addHeight, -1.0, 1.0);
    		helpString = (terrain.size.y*addHeight) + " meters (" + parseInt(addHeight*100.0) + "%)";
    	}
     
    	function OnWizardCreate () {
    		if (!terrain) {
    			return;
    		}
    		Undo.RegisterUndo(terrain, "Raise or Lower Heightmap");
     
    		var heights = terrain.GetHeights(0, 0, terrain.heightmapWidth, terrain.heightmapHeight);
    		for (var y = 0; y < terrain.heightmapHeight; y++) {
    			for (var x = 0; x < terrain.heightmapWidth; x++) {
    				heights[y,x] = heights[y,x] + addHeight;
    			}
    		}
    		terrain.SetHeights(0, 0, heights);
    		terrain = null;
    	}
    }C# - AdjustHeights.cs using UnityEditor;
    using UnityEngine;
     
    internal class AdjustHeights : ScriptableWizard
    {
        private static TerrainData _terrainData;
        public float HeightAdjustment = 0.1f;
     
        [MenuItem("Terrain/Adjust Heights")]
        public static void CreateWizard()
        {
            string buttonText = "Cancel";
            _terrainData = null;
     
            Terrain terrainObject = Selection.activeObject as Terrain ?? Terrain.activeTerrain;
     
            if (terrainObject)
            {
                _terrainData = terrainObject.terrainData;
                buttonText = "Adjust Heights";
            }
     
            DisplayWizard<AdjustHeights>("Adjust Heights", buttonText);
        }
     
        private void OnWizardUpdate()
        {
            if (!_terrainData)
            {
                helpString = "No terrain found";
                return;
            }
     
            HeightAdjustment = Mathf.Clamp(HeightAdjustment, -1.0f, 1.0f);
            helpString = (_terrainData.size.y*HeightAdjustment) + " meters (" + (HeightAdjustment*100.0) + "%)";
        }
     
        private void OnWizardCreate()
        {
            if (!_terrainData) return;
     
            Undo.RegisterUndo(_terrainData, "Adjust Heights");
     
            float[,] heights = _terrainData.GetHeights(0, 0, _terrainData.heightmapWidth, _terrainData.heightmapHeight);
     
            for (int y = 0; y < _terrainData.heightmapHeight; y++)
            {
                for (int x = 0; x < _terrainData.heightmapWidth; x++)
                {
                    heights[y, x] = heights[y, x] + HeightAdjustment;
                }
            }
     
            _terrainData.SetHeights(0, 0, heights);
            _terrainData = null;
        }
}
}
