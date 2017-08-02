/*************************
 * Original url: http://wiki.unity3d.com/index.php/TerrainObjExporter
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/TerrainObjExporter.cs
 * File based on original modification date of: 17 August 2014, at 04:32. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    
    Authors:
    Eric Haines (Eric5h5): original.
    Yun Kyu Choi: C# conversion.
    Bit Barrel media: progress bar fix. 
    
    
    Contents [hide] 
    1 Description 
    2 Usage 
    3 ExportTerrain.js 
    4 ExportTerrain.cs 
    
    DescriptionExports a Unity terrain object as an .obj file that can be loaded into various 3D apps. Requires Unity 2.5 or later. (See also Object2Terrain, which converts a mesh object to a Unity terrain.) 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. Also it must be called "ExportTerrain" or it won't run. 
    To export, first select a terrain object in your scene. If none is selected, it will use the active terrain (if any). Select Export To Obj... from the Terrain menu, and in the resulting window, select whether you want the object to use triangles or quads when exported, and also select the resolution to use for the exported mesh (full, half, quarter, eighth or sixteenth). Then click Export, choose a file name and location, and the file will be exported. Note that high-res terrains exported at full resolution will result in very large .obj files and may take a while to process. 
    ExportTerrain.js import System.IO;
    import System.Text;
     
    enum SaveFormat {Triangles, Quads}
    enum SaveResolution {Full, Half, Quarter, Eighth, Sixteenth}
     
    class ExportTerrain extends EditorWindow {
    	var saveFormat = SaveFormat.Triangles;
    	var saveResolution = SaveResolution.Half;
    	static var terrain : TerrainData;
    	static var terrainPos : Vector3;
     
    	var tCount : int;
    	var counter : int;
    	var totalCount : int;
            var progressUpdateInterval = 10000;
     
    	@MenuItem ("Terrain/Export To Obj...")
    	static function Init () {
    		terrain = null;
    		var terrainObject : Terrain = Selection.activeObject as Terrain;
    		if (!terrainObject) {
    			terrainObject = Terrain.activeTerrain;
    		}
    		if (terrainObject) {
    			terrain = terrainObject.terrainData;
    			terrainPos = terrainObject.transform.position;
    		}
    		EditorWindow.GetWindow(ExportTerrain).Show();
    	}
     
    	function OnGUI () {
    		if (!terrain) {
    			GUILayout.Label("No terrain found");
    			if (GUILayout.Button("Cancel")) {
    				EditorWindow.GetWindow(ExportTerrain).Close();
    			}
    			return;
    		}
    		saveFormat = EditorGUILayout.EnumPopup("Export Format", saveFormat);
    		saveResolution = EditorGUILayout.EnumPopup("Resolution", saveResolution);
     
    		if (GUILayout.Button("Export")) {
    			Export();
    		}
    	}
     
    	function Export () {
    		var fileName = EditorUtility.SaveFilePanel("Export .obj file", "", "Terrain", "obj");
    		var w = terrain.heightmapWidth;
    		var h = terrain.heightmapHeight;
    		var meshScale = terrain.size;
    		var tRes = Mathf.Pow(2, parseInt(saveResolution));
    		meshScale = Vector3(meshScale.x/(w-1)*tRes, meshScale.y, meshScale.z/(h-1)*tRes);
    		var uvScale = Vector2(1.0/(w-1), 1.0/(h-1));
    		var tData = terrain.GetHeights(0, 0, w, h);
     
    		w = (w-1) / tRes + 1;
    		h = (h-1) / tRes + 1;
    		var tVertices = new Vector3[w * h];
    		var tUV = new Vector2[w * h];
    		if (saveFormat == SaveFormat.Triangles) {
    			var tPolys = new int[(w-1) * (h-1) * 6];
    		}
    		else {
    			tPolys = new int[(w-1) * (h-1) * 4];
    		}
     
    		// Build vertices and UVs
    		for (y = 0; y < h; y++) {
    			for (x = 0; x < w; x++) {
    				tVertices[y*w + x] = Vector3.Scale(meshScale, Vector3(-y, tData[x*tRes,y*tRes], x)) + terrainPos;
    				tUV[y*w + x] = Vector2.Scale(Vector2(x*tRes, y*tRes), uvScale);
    			}
    		}
     
    		var index = 0;
    		if (saveFormat == SaveFormat.Triangles) {
    			// Build triangle indices: 3 indices into vertex array for each triangle
    			for (y = 0; y < h-1; y++) {
    				for (x = 0; x < w-1; x++) {
    					// For each grid cell output two triangles
    					tPolys[index++] = (y	 * w) + x;
    					tPolys[index++] = ((y+1) * w) + x;
    					tPolys[index++] = (y	 * w) + x + 1;
     
    					tPolys[index++] = ((y+1) * w) + x;
    					tPolys[index++] = ((y+1) * w) + x + 1;
    					tPolys[index++] = (y	 * w) + x + 1;
    				}
    			}
    		}
    		else {
    			// Build quad indices: 4 indices into vertex array for each quad
    			for (y = 0; y < h-1; y++) {
    				for (x = 0; x < w-1; x++) {
    					// For each grid cell output one quad
    					tPolys[index++] = (y	 * w) + x;
    					tPolys[index++] = ((y+1) * w) + x;
    					tPolys[index++] = ((y+1) * w) + x + 1;
    					tPolys[index++] = (y	 * w) + x + 1;
    				}
    			}	
    		}
     
    		// Export to .obj
    		try {
    			var sw = new StreamWriter(fileName);
    			sw.WriteLine("# Unity terrain OBJ File");
     
    			// Write vertices
    			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
    			counter = tCount = 0;
    			totalCount = (tVertices.Length*2 + (saveFormat == SaveFormat.Triangles? tPolys.Length/3 : tPolys.Length/4)) / progressUpdateInterval;
    			for (i = 0; i < tVertices.Length; i++) {
    				UpdateProgress();
    				var sb = StringBuilder("v ", 20);
    				// StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
    				// Which is important when you're exporting huge terrains.
    				sb.Append(tVertices[i].x.ToString()).Append(" ").
    				   Append(tVertices[i].y.ToString()).Append(" ").
    				   Append(tVertices[i].z.ToString());
    				sw.WriteLine(sb);
    			}
    			// Write UVs
    			for (i = 0; i < tUV.Length; i++) {
    				UpdateProgress();
    				sb = StringBuilder("vt ", 22);
    				sb.Append(tUV[i].x.ToString()).Append(" ").
    				   Append(tUV[i].y.ToString());
    				sw.WriteLine(sb);
    			}
    			if (saveFormat == SaveFormat.Triangles) {
    				// Write triangles
    				for (i = 0; i < tPolys.Length; i += 3) {
    					UpdateProgress();
    					sb = StringBuilder("f ", 43);
    					sb.Append(tPolys[i]+1).Append("/").Append(tPolys[i]+1).Append(" ").
    					   Append(tPolys[i+1]+1).Append("/").Append(tPolys[i+1]+1).Append(" ").
    					   Append(tPolys[i+2]+1).Append("/").Append(tPolys[i+2]+1);
    					sw.WriteLine(sb);
    				}
    			}
    			else {
    				// Write quads
    				for (i = 0; i < tPolys.Length; i += 4) {
    					UpdateProgress();
    					sb = StringBuilder("f ", 57);
    					sb.Append(tPolys[i]+1).Append("/").Append(tPolys[i]+1).Append(" ").
    					   Append(tPolys[i+1]+1).Append("/").Append(tPolys[i+1]+1).Append(" ").
    					   Append(tPolys[i+2]+1).Append("/").Append(tPolys[i+2]+1).Append(" ").
    					   Append(tPolys[i+3]+1).Append("/").Append(tPolys[i+3]+1);
    					sw.WriteLine(sb);
    				}		
    			}
    		}
    		catch (err) {
    			Debug.Log("Error saving file: " + err.Message);
    		}
    		sw.Close();
     
    		terrain = null;
    		EditorUtility.DisplayProgressBar("Saving file to disc.", "This might take a while...", 1);		
    		EditorWindow.GetWindow(ExportTerrain).Close();
    		EditorUtility.ClearProgressBar();
    	}
     
    	function UpdateProgress () {
    		if (counter++ == progressUpdateInterval) {
    			counter = 0;
    			EditorUtility.DisplayProgressBar("Saving...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
    		}
    	}
    }ExportTerrain.cs // Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
    // C # manual conversion work by Yun Kyu Choi
     
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
     
    enum SaveFormat { Triangles, Quads }
    enum SaveResolution { Full=0, Half, Quarter, Eighth, Sixteenth }
     
    class ExportTerrain : EditorWindow
    {
       SaveFormat saveFormat = SaveFormat.Triangles;
       SaveResolution saveResolution = SaveResolution.Half;
     
       static TerrainData terrain;
       static Vector3 terrainPos;
     
       int tCount;
       int counter;
       int totalCount;
       int progressUpdateInterval = 10000;
     
       [MenuItem("Terrain/Export To Obj...")]
       static void Init()
       {
          terrain = null;
          Terrain terrainObject = Selection.activeObject as Terrain;
          if (!terrainObject)
          {
             terrainObject = Terrain.activeTerrain;
          }
          if (terrainObject)
          {
             terrain = terrainObject.terrainData;
             terrainPos = terrainObject.transform.position;
          }
     
          EditorWindow.GetWindow<ExportTerrain>().Show();
       }
     
       void OnGUI()
       {
          if (!terrain)
          {
             GUILayout.Label("No terrain found");
             if (GUILayout.Button("Cancel"))
             {
                EditorWindow.GetWindow<ExportTerrain>().Close();
             }
             return;
          }
          saveFormat = (SaveFormat) EditorGUILayout.EnumPopup("Export Format", saveFormat);
     
          saveResolution = (SaveResolution) EditorGUILayout.EnumPopup("Resolution", saveResolution);
     
          if (GUILayout.Button("Export"))
          {
             Export();
          }
       }
     
       void Export()
       {
          string fileName = EditorUtility.SaveFilePanel("Export .obj file", "", "Terrain", "obj");
          int w = terrain.heightmapWidth;
          int h = terrain.heightmapHeight;
          Vector3 meshScale = terrain.size;
          int tRes = (int)Mathf.Pow(2, (int)saveResolution );
          meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
          Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
          float[,] tData = terrain.GetHeights(0, 0, w, h);
     
          w = (w - 1) / tRes + 1;
          h = (h - 1) / tRes + 1;
          Vector3[] tVertices = new Vector3[w * h];
          Vector2[] tUV = new Vector2[w * h];
     
          int[] tPolys;
     
          if (saveFormat == SaveFormat.Triangles)
          {
             tPolys = new int[(w - 1) * (h - 1) * 6];
          }
          else
          {
             tPolys = new int[(w - 1) * (h - 1) * 4];
          }
     
          // Build vertices and UVs
          for (int y = 0; y < h; y++)
          {
             for (int x = 0; x < w; x++)
             {
                tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(-y, tData[x * tRes, y * tRes], x)) + terrainPos;
                tUV[y * w + x] = Vector2.Scale( new Vector2(x * tRes, y * tRes), uvScale);
             }
          }
     
          int  index = 0;
          if (saveFormat == SaveFormat.Triangles)
          {
             // Build triangle indices: 3 indices into vertex array for each triangle
             for (int y = 0; y < h - 1; y++)
             {
                for (int x = 0; x < w - 1; x++)
                {
                   // For each grid cell output two triangles
                   tPolys[index++] = (y * w) + x;
                   tPolys[index++] = ((y + 1) * w) + x;
                   tPolys[index++] = (y * w) + x + 1;
     
                   tPolys[index++] = ((y + 1) * w) + x;
                   tPolys[index++] = ((y + 1) * w) + x + 1;
                   tPolys[index++] = (y * w) + x + 1;
                }
             }
          }
          else
          {
             // Build quad indices: 4 indices into vertex array for each quad
             for (int y = 0; y < h - 1; y++)
             {
                for (int x = 0; x < w - 1; x++)
                {
                   // For each grid cell output one quad
                   tPolys[index++] = (y * w) + x;
                   tPolys[index++] = ((y + 1) * w) + x;
                   tPolys[index++] = ((y + 1) * w) + x + 1;
                   tPolys[index++] = (y * w) + x + 1;
                }
             }
          }
     
          // Export to .obj
          StreamWriter sw = new StreamWriter(fileName);
          try
          {
     
             sw.WriteLine("# Unity terrain OBJ File");
     
             // Write vertices
             System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
             counter = tCount = 0;
             totalCount = (tVertices.Length * 2 + (saveFormat == SaveFormat.Triangles ? tPolys.Length / 3 : tPolys.Length / 4)) / progressUpdateInterval;
             for (int i = 0; i < tVertices.Length; i++)
             {
                UpdateProgress();
                StringBuilder sb = new StringBuilder("v ", 20);
                // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
                // Which is important when you're exporting huge terrains.
                sb.Append(tVertices[i].x.ToString()).Append(" ").
                   Append(tVertices[i].y.ToString()).Append(" ").
                   Append(tVertices[i].z.ToString());
                sw.WriteLine(sb);
             }
             // Write UVs
             for (int i = 0; i < tUV.Length; i++)
             {
                UpdateProgress();
                StringBuilder sb = new StringBuilder("vt ", 22);
                sb.Append(tUV[i].x.ToString()).Append(" ").
                   Append(tUV[i].y.ToString());
                sw.WriteLine(sb);
             }
             if (saveFormat == SaveFormat.Triangles)
             {
                // Write triangles
                for (int i = 0; i < tPolys.Length; i += 3)
                {
                   UpdateProgress();
                   StringBuilder sb = new StringBuilder("f ", 43);
                   sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                      Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                      Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1);
                   sw.WriteLine(sb);
                }
             }
             else
             {
                // Write quads
                for (int i = 0; i < tPolys.Length; i += 4)
                {
                   UpdateProgress();
                   StringBuilder sb = new StringBuilder("f ", 57);
                   sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                      Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                      Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1).Append(" ").
                      Append(tPolys[i + 3] + 1).Append("/").Append(tPolys[i + 3] + 1);
                   sw.WriteLine(sb);
                }
             }
          }
          catch(Exception err)
          {
             Debug.Log("Error saving file: " + err.Message);
          }
          sw.Close();
     
          terrain = null;
          EditorUtility.DisplayProgressBar("Saving file to disc.", "This might take a while...", 1f);
          EditorWindow.GetWindow<ExportTerrain>().Close();      
          EditorUtility.ClearProgressBar();
       }
     
       void UpdateProgress()
       {
          if (counter++ == progressUpdateInterval)
          {
             counter = 0;
             EditorUtility.DisplayProgressBar("Saving...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
          }
       }
    }
}
