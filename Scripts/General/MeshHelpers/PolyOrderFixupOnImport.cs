// Original url: http://wiki.unity3d.com/index.php/PolyOrderFixupOnImport
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MeshHelpers/PolyOrderFixupOnImport.cs
// File based on original modification date of: 29 February 2012, at 00:19. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
Contents [hide] 
1 Abstract 
2 Usage 
3 Creating the CSV file 
4 Contributions Accepted 
5 Code 

Abstract Author: Roger Braunstein (rogerimp) 
Updated: This may be completely unnecessary in Unity 3.5+, please see the "Optimize Mesh" checkbox in import settings. See Manual 
This editor script will prevent Unity from modifying the polygon ordering in a model upon import. Lore and evidence suggest that mesh optimizations are applied on import (even when the Mesh Compression import setting is Off). 
In most cases this optimization is a good thing, as Unity seems to create triangle strips that would optimize rendering performance. In some cases, this is a bad thing, such as when you are attempting to render an overlapping discontinuous mesh in a single draw call from bottom-to-top (for instance, drawing a character on the same z-plane in a 2D game using the Unlit/Transparent shader). In this case, the optimizations could reorder your polygons and their carefully-created draw order. 
The script does this by overriding the optimized mesh at import time, using the original polygon ordering. No additional work is required at run time. 
Usage To use this script, place it in the Assets/Editor directory. It will only run on models with an accompanying .csv file containing the original polygon ordering. 
For instance, if your model is in Assets/Kimmy.fbx, it will look for an accompanying Assets/Kimmy.csv file. 
Please ensure that you've triangulated your model before exporting it to FBX. Quads are not supported. 
The script is intended for FBX files that have a single mesh in them. It could be extended to support bigger scenes if you need. 
Creating the CSV file In Cinema 4D, select your Polygon Object (model) in Model mode. Make sure you haven't hidden any polygons (use Selection>Unhide All). Find the Structure Manager or open it up with Window>Structure Manager. Go into polygon mode with Mode>Polygons. File>Export and pick the location next to the exported FBX. C4D will name it .txt, so rename this to .csv. (This extension is easily customizable, maybe we should call it .polygonlist or something more descriptive?) 
Feel free to edit this article to include instructions for other 3d packages. 
Contributions Accepted If your 3d package can export polygon ordering to some kind of text file, it should be pretty easy to accept a new format in this script. Please change it and let me know (PM me on the forums). 
I can provide a python script that reads this information from any FBX, but you'll need to have python and the Autodesk FBX SDK for Python installed. (In theory, we could read this info directly from the FBX in this script, but I'm keeping it simple.) 
Code using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Path = System.IO.Path;
 
public class PolyOrderFixupOnImport : AssetPostprocessor
{
	private readonly char[] CSVDelimiters = new char[]{'\t', ','};
 
	public void OnPostprocessModel(GameObject go)
	{
		string assetPathAbsolute = Path.Combine(Path.Combine(Application.dataPath, ".."), this.assetPath);
		string assetPathNormalized = new DirectoryInfo(assetPathAbsolute).FullName;
		string csvPath = Path.ChangeExtension(assetPathNormalized, "csv");
		if (!File.Exists(csvPath)) return;
 
		try
		{
			Debug.Log(string.Format("Found replacement polygon info for {0}, replacing optimized polys...", go.name));
 
			MeshFilter meshFilterComponent = go.GetComponentInChildren<MeshFilter>();
			SkinnedMeshRenderer skinnedMeshRendererComponent = go.GetComponentInChildren<SkinnedMeshRenderer>();
			Mesh targetMesh;
			if (meshFilterComponent)
			{
				targetMesh = meshFilterComponent.sharedMesh;
			}
			else if (skinnedMeshRendererComponent)
			{
				targetMesh = skinnedMeshRendererComponent.sharedMesh;
			}
			else
			{
				throw new MissingComponentException();
			}
			targetMesh.triangles = ReadTriangleListFromCSV(csvPath).ToArray();
			Debug.Log("Rewrote triangles successfully.");
		} catch (MissingComponentException) {
			Debug.LogWarning("Warning: mesh not found in imported GameObject.");
		}
	}
 
	//We expect a tab separated values file in the following format:
	//Polygon	A	B	C	D
	//0	0	12	14	
	//1	12	1	13
	//
	//and so on. Polygon = just a counter up from 0.
	//All polygons are expected to be triangles, so D column is left blank (and is optional.)
	//By the way, the reasoning behind this format is the way Cinema 4D's Structure manager
	//exports data. Try out Window>Structure Manager, Mode>Polygons, then File>Export ASCII Data.
	//Feel free to extend this class to include formats supported by other 3d tools.
	private List<int> ReadTriangleListFromCSV(string filename)
	{
		var triangles = new List<int>();
		using (StreamReader sr = File.OpenText(filename))
		{
			sr.ReadLine(); //discard first line
			while (!sr.EndOfStream)
			{
				string[] substrings = sr.ReadLine().Split(CSVDelimiters);
				triangles.AddRange(substrings.Skip(1).Take(3).Select(str => int.Parse(str)));
			}
		}
		return triangles;
	}
}
}
