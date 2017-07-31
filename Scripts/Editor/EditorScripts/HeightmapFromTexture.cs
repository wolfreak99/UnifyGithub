// Original url: http://wiki.unity3d.com/index.php/HeightmapFromTexture
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/HeightmapFromTexture.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{

Author: Eric Haines (Eric5h5) 
Description Uses a texture in your project as a heightmap, which is applied to the active terrain. This way you don't have to import RAW files, but the vertical resolution will be limited to 8-bit. Requires Unity 2.6, although it will work with Unity 2.5 as long the texture is uncompressed. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
To turn a texture into a heightmap, click on the texture in your project, then select the Heightmap From Texture item in the Terrain menu. The texture must be marked "Is Readable" to work. Textures with color are fine, since the colors are evaluated by their grayscale value. If the texture is a different size from the heightmap resolution, it will be resized to fit (internally--the actual texture is untouched). If the texture is resized, it will be scaled using simple nearest-neighbor scaling if the texture is set to have no filtering; otherwise it will be scaled using bilinear filtering. 
JavaScript - HeightmapFromTexture.js @MenuItem ("Terrain/Heightmap From Texture")
 
static function ApplyHeightmap () {
	var heightmap : Texture2D = Selection.activeObject as Texture2D;
	if (heightmap == null) { 
		EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel"); 
		return; 
	}
	Undo.RegisterUndo (Terrain.activeTerrain.terrainData, "Heightmap From Texture");
 
	var terrain = Terrain.activeTerrain.terrainData;
	var w = heightmap.width;
	var h = heightmap.height;
	var w2 = terrain.heightmapWidth;
	var heightmapData = terrain.GetHeights(0, 0, w2, w2);
	var mapColors = heightmap.GetPixels();
	var map = new Color[w2 * w2];
 
	if (w2 != w || h != w) {
		// Resize using nearest-neighbor scaling if texture has no filtering
		if (heightmap.filterMode == FilterMode.Point) {
			var dx : float = parseFloat(w)/w2;
			var dy : float = parseFloat(h)/w2;
			for (y = 0; y < w2; y++) {
				if (y%20 == 0) {
					EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0, w2, y));
				}
				var thisY = parseInt(dy*y)*w;
				var yw = y*w2;
				for (x = 0; x < w2; x++) {
					map[yw + x] = mapColors[thisY + dx*x];
				}
			}
		}
		// Otherwise resize using bilinear filtering
		else {
			var ratioX = 1.0/(parseFloat(w2)/(w-1));
			var ratioY = 1.0/(parseFloat(w2)/(h-1));
			for (y = 0; y < w2; y++) {
				if (y%20 == 0) {
					EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0, w2, y));
				}
				var yy = Mathf.Floor(y*ratioY);
				var y1 = yy*w;
				var y2 = (yy+1)*w;
				yw = y*w2;
				for (x = 0; x < w2; x++) {
					var xx = Mathf.Floor(x*ratioX);
 
					var bl = mapColors[y1 + xx];
					var br = mapColors[y1 + xx+1]; 
					var tl = mapColors[y2 + xx];
					var tr = mapColors[y2 + xx+1];
 
					var xLerp = x*ratioX-xx;
					map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y*ratioY-yy);
				}
			}
		}
		EditorUtility.ClearProgressBar();
	}
	else {
		// Use original if no resize is needed
		map = mapColors;
	}
 
	// Assign texture data to heightmap
	for (y = 0; y < w2; y++) {
		for (x = 0; x < w2; x++) {
			heightmapData[y,x] = map[y*w2+x].grayscale;
		}
	}
	terrain.SetHeights(0, 0, heightmapData);
}
}
