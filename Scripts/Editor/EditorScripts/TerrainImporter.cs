// Original url: http://wiki.unity3d.com/index.php/TerrainImporter
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/TerrainImporter.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Warwick Allison (WarwickAllison) 
DescriptionImports a set of 16-bit RAW heightmaps and 8-bit RAW alphamaps to build TerrainData for a Terrain. See documentation in the script itself. It's similar to using the Import Heightmap - Raw menu option, except it keeps all the settings so that you can reimport whenever you change the input files. 
Place the script in a folder named Editor in your project's Assets folder. Call it TerrainImporter.js. 
TerrainImporter.js /*
** Terrain Importer for Unity
**
** by Warwick Allison
** Licensed under Creative Commons license - Attribution-ShareAlike 3.0 CC BY-SA
** See license information at http://creativecommons.org/licenses/by-sa/3.0/au/
** (private or commercial use permitted, with attribution - i.e. retain this header,
** and republish any changes you make)
**
** Please post changes here:
** http://www.unifycommunity.com/wiki/index.php?title=TerrainImporter
**
** Version 0.2 - bug fixes (close file)
** Version 0.1 - initial version
** Tested with Unity 3.0beta5
**
** Place this script in the Assets/Editor/ directory of your project.
**
** With this script installed, any .txt file which starts with "[Terrain Importer]"
** is processed to define a new TerrainData (or modify an existing one).
** The resulting TerrainData can then be dragged into the Hierarchy.
**
** The file has the following format:
**
** [Terrain Importer]
** <default settings>
** [<output name>]
** <specific settings>
**
** See further below for examples.
**
** Each "setting" is <parameter>=<value> where <parameter> is one of those described below.
**
** Heightfield file parameters:
**   heightFormat = r16littleendian (16 bit words, Windows byte order, the default)
**   heightFormat = r16bigendian (16 bit words, Mac byte order)
**   heightFile = file name of heightfield data (formated according to heightformat)
**   heightFileWidth = width of heightfield (default derived from file size or heightFileHeight)
**   heightFileHeight = height of heightfield (default derived from file size or heightFileWidth)
**
** Terrain physical parameters:
**   terrainWidth = width of terrain in world units on X axis (default 1000)
**   terrainHeight = height of terrain in world units on Y axis for height of 1.0 (default 200)
**   terrainLength = length of terrain in world units on Z axis (default 1000)
**
** Texture layering parameters:
**   equalizeLayers = set to 1 to force alphamaps to add up to 1.0 (default off)
**   layer<n>file = filename of raw heightfield (8 bit)
**   layer<n>Width = tiling width for texture <n> (TextureData.splatPrototypes[n].tileSize)
**   layer<n>Height = tiling height for texture <n>
**   layer<n>OffsetX = tiling X offset for texture <n> (TextureData.splatPrototypes[n].tileOffset)
**   layer<n>OffsetY = tiling Y offset for texture <n>
**   layer<n>Texture = asset name of tile for texture <n> (eg. a png file)
** If only the texture<n>file is provided, *temporary* red/green/blue/grey textures are
** created. You should then set them in the normal manner within Unity and they will be
** maintained when the height file is modified.
** The layer<n>Texture file is either relative to the path of the text asset, or
** from the root (Assets) by starting with "/"). You cannot currently use ".." notation.
**
** The texture tiling values above refer to the in-game tiling of the texture, not
** tiling in the input (see the next section for those parameters).
**
** Terrain tile selection (the heightmap can be divided up into tiles)
**   terrainTileSize = width (and height) of heightmap to extract (default heightfileWidth)
**   terrainTileX = X tile number to extract (default 0)
**   terrainTileY = Y tile number to extract (default 0)
** Note that tiled terrains are correctly stitched, so a 513x513 heightfile can be
** divided into 4 tiles each of 257x257, since the centerline heights are shared.
** The terrainTileSize should be 1 plus a power of 2 (a Unity3D requirement).
** The layer<n>file files must have the same aspect ratio as the height field, as they
** are tiled in the same manner (except they do not need stitching).
** To use tiles effectively in Unity, after adding them to the scene, you will need to
** correctly set their transform and their neighbors (see Terrain.SetNeighbors).
**
**
** Examples:
**
** Simplest terrain definition:
 
[Terrain Importer]
heightfile=heightdata.r16
 
** Textured terrain definition:
 
[Terrain Importer]
[MyTerrainExample]
heightFile=heightdata.r16
terrainWidth=100
terrainHeight=50
terrainLength=100
equalizeLayers=1
layer1File=slope.raw
layer2File=flow.raw
 
** Tiled terrain definition:
 
[Terrain Importer]
# This is 3x1 tiles
heightFile=heightdata.r16
heightFileWidth=769
terrainSize=257
terrainWidth=100
terrainHeight=50
terrainLength=100
[LeftTerrain]
terrainOffsetX=0
[MiddleTerrain]
terrainOffsetX=1
[RightTerrain]
terrainOffsetX=2
 
**
** Lines except the first may also be a comment, preceded by "#" or "//", or be blank.
** All texts are case sensitive.
** If no [<output name>] line is given, a default name is generated from the text file name.
**
** Unity will only reimport if you chnage the txt file or explicitly use
** the Reimport context menu option. When the terrain is reimported, conflicting changes
** you have made within Unity will be lost - so make changes in the source files. You may
** however freely modify textures and add trees and other details from within Unity.
**/
 
class TerrainImporter extends AssetPostprocessor {
	static function OnPostprocessAllAssets (
		importedAssets : String[],
		deletedAssets : String[],
		movedAssets : String[],
		movedFromAssetPaths : String[])
	{
		for (var ass in importedAssets) {
			if (ass.EndsWith(".txt")) {
				var sr = new StreamReader(ass);
				var s = sr.ReadLine();
				if (s == "[Terrain Importer]") {
					var baseparam = new Hashtable();
					baseparam["terrainWidth"] = "1000";
					baseparam["terrainHeight"] = "200";
					baseparam["terrainLength"] = "1000";
					baseparam["terrainTileX"] = "0";
					baseparam["terrainTileY"] = "0";
					baseparam["equalizeLayers"] = 0;
					baseparam["heightFormat"] = "r16littleendian";
					var param = baseparam.Clone();
					var currentoutput = "";
					while (sr.Peek() >= 0) {
						s = sr.ReadLine();
						if (s.StartsWith("[") && s.EndsWith("]")) {
							if (currentoutput == "") {
								baseparam = param.Clone();
							} else {
								GenerateTerrain(currentoutput,param,Path.GetDirectoryName(ass));
							}
							currentoutput = s.Substring(1,s.Length-2);
							param = baseparam.Clone();
						} else if (s == "" || s.StartsWith("//") || s.StartsWith("#")) {
							// Ignore (comment)
						} else {
							var kv = s.Split("="[0]);
							param[kv[0]]=kv[1];
						}
					}
					if (currentoutput == "") {
						GenerateTerrain(Path.GetFileNameWithoutExtension(ass)+"-terrain",param,Path.GetDirectoryName(ass));
					} else {
						GenerateTerrain(currentoutput,param,Path.GetDirectoryName(ass));
					}
				}
				sr.Close();
			}
		}
	}
 
	static function GenerateTerrain(output : String, param : Hashtable, path : String)
	{
		var terraindatapath = Path.Combine(path,output + ".asset");
		Debug.Log("Generate Terrain: " + terraindatapath);
 
		var terrainData : TerrainData = AssetDatabase.LoadAssetAtPath(terraindatapath,TerrainData);
		if (!terrainData) {
			terrainData = new TerrainData();
			AssetDatabase.CreateAsset(terrainData, terraindatapath);
		}
 
		var fi = new FileInfo(Path.Combine(path,param["heightFile"]));
 
		var hfSamples : int = fi.Length/2;
		var hfWidth : int;
		var hfHeight : int;
		if (!int.TryParse(param["heightFileWidth"],hfWidth))
			hfWidth = 0;
		if (!int.TryParse(param["heightFileHeight"],hfHeight) || hfHeight <= 0) {
			if (hfWidth > 0)
				hfHeight = hfSamples/hfWidth;
			else
				hfHeight = hfWidth = Mathf.CeilToInt(Mathf.Sqrt(hfSamples));
		} else {
			if (hfWidth <= 0)
				hfWidth = hfSamples/hfHeight;
		}
		var size : int;
		if (!int.TryParse(param["terrainTileSize"],size))
			size = hfWidth;
		var tOffX : int;
		if (!int.TryParse(param["terrainTileX"],tOffX))
			tOffX = 0;
		var tOffY : int;
		if (!int.TryParse(param["terrainTileY"],tOffY))
			tOffY = 0;
 
		if (tOffX < 0 || tOffY < 0 || (size-1)*tOffX > hfWidth || (size-1)*tOffY > hfHeight) {
			Debug.LogError("terrainTile ("+tOffX+","+tOffY+") of size "+size+"x"+size+" "
					+"is outside heightFile size "+hfWidth+"x"+hfHeight);
			return; // We don't want to Seek/Read outside file bounds.
		}
 
		// Stitching reuses right/bottom edges.
		tOffX = (size-1)*tOffX;
		tOffY = (size-1)*tOffY;
 
		var bpp = 2; // only word formats are currently supported
 
		var x;
		var y;
 
		var fs = fi.OpenRead();
		var b = new byte[size*size*bpp];
		fs.Seek((tOffX+tOffY*hfWidth)*bpp, SeekOrigin.Current);
		if (size == hfWidth) {
			fs.Read(b,0,size*size*bpp);
		} else {
			for (y=0; y<size; ++y) {
				fs.Read(b,y*size*bpp,size*bpp);
				if (y+1<size)
					fs.Seek((hfWidth-size)*bpp, SeekOrigin.Current);
			}
		}
		fs.Close();
 
		var h = MultiDim.FloatArray(size,size);
		var i=0;
 
		if (param["heightFormat"] == "r16bigendian") {
			for (x=0; x<size; ++x) {
				for (y=0; y<size; ++y) {
					h[size-1-x,y] = (b[i++]*256.0+b[i++])/65535.0;
				}
			}
		} else { // r16littleendian
			for (x=0; x<size; ++x) {
				for (y=0; y<size; ++y) {
					h[size-1-x,y] = (b[i++]+b[i++]*256.0)/65535.0;
				}
			}
		}
 
		terrainData.heightmapResolution = size-1;
 
		if (param["layer0File"] || param["layer1File"]) {
			var nlayers = 2;
			while (param["layer"+nlayers+"File"]) nlayers++;
 
			var alphas = MultiDim.FloatArray(1,1,1);
			var asize = 0;
			var amWidth = 0;
			for (var lay=0; lay<nlayers; ++lay) {
				if (param["layer"+lay+"File"]) {
					fi = new FileInfo(Path.Combine(path,param["layer"+lay+"File"]));
					if (asize==0) {
						var amSamples = fi.Length;
						asize = size * amSamples / hfSamples;
						amWidth = hfWidth * amSamples / hfSamples;
						terrainData.alphamapResolution = asize;
						alphas = MultiDim.FloatArray(asize,asize,nlayers);
					}
 
 
					fs = fi.OpenRead();
					b = new byte[asize*asize];
					fs.Seek(tOffX+tOffY*amWidth, SeekOrigin.Current);
					if (asize == amWidth) {
						fs.Read(b,0,asize*asize);
					} else {
						for (y=0; y<asize; ++y) {
							fs.Read(b,y*asize,asize);
							if (y+1<asize)
								fs.Seek(amWidth-asize, SeekOrigin.Current);
						}
					}
 
					fs.Close();
					i=0;
					for (x=0; x<asize; ++x) {
						for (y=0; y<asize; ++y) {
							alphas[asize-1-x,y,lay] = b[i++]/256.0;
						}
					}
				}
			}
			if (param["equalizeLayers"]) {
				if (!param["layer0File"]) {
					// create layer0 by remainder
					for (x=0; x<asize; ++x) {
						for (y=0; y<asize; ++y) {
							var rem=1.0;
							for (lay=1; lay<nlayers; ++lay)
								rem -= alphas[asize-1-x,y,lay];
							if (rem > 0.0)
								alphas[asize-1-x,y,0] = rem;
						}
					}
				}
				// Equalize by rescaling
				for (x=0; x<asize; ++x) {
					for (y=0; y<asize; ++y) {
						var tot=0.0;
						for (lay=0; lay<nlayers; ++lay)
							tot += alphas[asize-1-x,y,lay];
						if (tot > 0.0) {
							for (lay=0; lay<nlayers; ++lay)
								alphas[asize-1-x,y,lay] = alphas[asize-1-x,y,lay]/tot;
						}
					}
				}
			}
			var oldsp = terrainData.splatPrototypes;
			var sp = new SplatPrototype[nlayers];
			for (lay=0; lay<nlayers; ++lay) {
				var splat : String = "layer"+lay;
				sp[lay] = new SplatPrototype();
 
				var ts : Vector2;
				if (!float.TryParse(param[splat+"Width"],ts.x)) {
					if (lay >= oldsp.Length) ts.x = 15;
					else ts.x = oldsp[lay].tileSize.x;
				}
				if (!float.TryParse(param[splat+"Height"],ts.y)) {
					if (lay >= oldsp.Length) ts.y = 15;
					else ts.y = oldsp[lay].tileSize.y;
				}
 
				var to : Vector2;
				if (!float.TryParse(param[splat+"OffsetX"],to.x)) {
					if (lay >= oldsp.Length) to.x = 0;
					else to.x = oldsp[lay].tileOffset.x;
				}
				if (!float.TryParse(param[splat+"OffsetY"],to.y)) {
					if (lay >= oldsp.Length) to.y = 0;
					else to.y = oldsp[lay].tileOffset.y;
				}
				var tex : Texture2D;
				var texfile = param[splat+"Texture"];
				if (texfile) {
					if (texfile.StartsWith("/") || texfile.StartsWith("\\")) {
						tex = AssetDatabase.LoadMainAssetAtPath("Assets"+param[splat+"Texture"]);
					} else {
						tex = AssetDatabase.LoadMainAssetAtPath(Path.Combine(path,param[splat+"Texture"]));
					}
				} else if (lay < oldsp.Length) {
					tex = oldsp[lay].texture;
				}
				if (!tex) {
					tex = new Texture2D(1,1);
					var g = (lay-1)/nlayers;
					tex.SetPixel(0,0,
						lay==0 ? Color.red :
						lay==1 ? Color.green :
						lay==2 ? Color.blue : Color(g,g,g));
					tex.Apply();
				}
				sp[lay].texture = tex;
				sp[lay].tileSize = ts;
				sp[lay].tileOffset = to;
			}
			terrainData.splatPrototypes = sp;
			terrainData.SetAlphamaps(0,0,alphas);
		}
 
		var sz : Vector3;
		sz.x = float.Parse(param["terrainWidth"]);
		sz.y = float.Parse(param["terrainHeight"]);
		sz.z = float.Parse(param["terrainLength"]);
 
		terrainData.size = sz;
		terrainData.SetHeights(0,0,h);
		terrainData.RecalculateTreePositions();
	}
}
}
