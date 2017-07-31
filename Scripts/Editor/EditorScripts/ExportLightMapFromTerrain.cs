// Original url: http://wiki.unity3d.com/index.php/ExportLightMapFromTerrain
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/ExportLightMapFromTerrain.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
DescriptionThis editor script for export lightmap from selected terrain and save it image in Asset folder. Based on LightmapExport.cs fro Island Demo. 
UsagePlace this script in YourProject/Assets/Editor and the "Export LightMap" menu items will appear in the Terrain menu once it is compiled. 
Java - NeoLightMapExport.jsimport System.IO;
class NeoLightmapExport extends ScriptableWizard
{
	var terrain : Terrain;
	private var terrainData : TerrainData;
 
	@MenuItem("Terrain/ExportLightmap...")
	static function CreateWizard ()
	{
		ScriptableWizard.DisplayWizard("Export Lightmap", NeoLightmapExport, "Export"); 
	}
 
	function OnWizardUpdate ()
	{
		helpString = "Well, let's go Rock!";
		if(!terrain) {
            helpString = "I need a terrain!";
        } else {
            terrainData = terrain.terrainData;
        }
	}
 
	function OnWizardCreate ()
    {
		var myBytes : byte[];
		var lightmapTexture : Texture2D = terrainData.lightmap;
		myBytes = lightmapTexture.EncodeToPNG();
		var filename : String = terrain.name + "Duplicate.png";
		File.WriteAllBytes(Application.dataPath + "/"+filename, myBytes);
		EditorUtility.DisplayDialog("Texture Duplicated", "Selected Texture2D saved in Assets/ as: " + filename, "See You Again!");
	}
}
}
