// Original url: http://wiki.unity3d.com/index.php/TerrainPerlinNoise
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/TerrainPerlinNoise.cs
// File based on original modification date of: 11 January 2013, at 20:25. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
This Editor wizard generates the terrain heights from a perlin noise function. Access it by clicking on the "Terrain/Generate from Perlin Noise" menu item. There is only one option, tiling, which affects the tiling of the perlin noise functions. Increasing the tiling increases the number of hills. I didn't add an option for height for this simple script, but if you feel you need it it should be pretty straightforward to add. 
C# Code using UnityEditor;
using UnityEngine;
using System.Collections;
 
public class TerrainPerlinNoise : ScriptableWizard {
 
    public float Tiling = 10.0f;
 
    [MenuItem("Terrain/Generate from Perlin Noise")]
    public static void CreateWizard(MenuCommand command)
    {
        ScriptableWizard.DisplayWizard("Perlin Noise Generation Wizard", typeof(TerrainPerlinNoise));
    }
 
    void OnWizardUpdate()
    {
        helpString = "This small generation tool allows you to generate perlin noise for your terrain.";
    }
 
    void OnWizardCreate()
    {
        GameObject obj = Selection.activeGameObject;
 
        if (obj.GetComponent<Terrain>())
        {
            GenerateHeights(obj.GetComponent<Terrain>(), Tiling);
        }
    }
 
    public void GenerateHeights(Terrain terrain, float tileSize)
    {
        float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
 
        for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
        {
            for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
            {
                heights[i, k] = Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize, ((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize)/10.0f;
            }
        }
 
        terrain.terrainData.SetHeights(0, 0, heights);
    }
}
}
