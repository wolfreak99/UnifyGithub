// Original url: http://wiki.unity3d.com/index.php/Object2Terrain
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/Object2Terrain.cs
// File based on original modification date of: 17 August 2014, at 04:34. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{

Authors:
Eric Haines (Eric5h5): original.
Jessy: C# conversion.
Bit Barrel Media: added functionality. 
Contents [hide] 
1 Description 
2 Usage 
3 Settings (C# only) 
4 C# - Object2Terrain.cs 
5 JavaScript - Object2Terrain.js 

Description Converts an object mesh to a heightmap. This way you can create terrain meshes in a standard 3D app such as Blender or Maya and convert it to a Unity terrain. It uses raycasting instead of file conversion, so it works with any object which has a mesh.

See also TerrainObjExporter, which saves a Unity terrain as an .obj file. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. The source object must contain a mesh. Selecting a parent object will not work.

If in doubt, simply open the mesh asset in the project view, then drag the bare mesh into the scene or hierarchy: 

Note: The script uses the axis-aligned bounding box of the mesh, so object rotations on the x and z axis other than 0, or rotations on the y axis other than multiples of 90 degrees, may give somewhat odd results.


C# (updated):
-Click on an object in the scene view or hierarchy.
-Select from the top menu: Terrain->Object to Terrain.
-A new terrain GameObject is automatically created.


Java (original):
-Add a default terrain GameObject to the scene.
-Click on an object in the scene view or hierarchy.
-Select from the top menu: Terrain->Object to Terrain.
-The object is then converted to the heightmap in the active terrain.

Settings (C# only) -Resolution: the resolution of the generated terrain.
-Add terrain: add blank terrain to the front/back, left/right and above. Handy if you want to create additional content. Increase this if the edge of the terrain is cut off.
-Shift height: move the terrain up or down. The terrain GameObject will stay in the same position. Range limited to the vertical terrain size.
-Bottom up: terrain is generated from the bottom up. This will ensure a 1 to 1 resemblance of the source object.
-Top down: terrain is generated from top to bottom. This will stretch the terrain if a larger y value of "Add terrain" is used. This is the original mode but gives somewhat odd results.
-Create terrain: start generating the terrain. Depending on the resolution, this might take a while.

C# - Object2Terrain.cs using UnityEngine;
using UnityEditor;
 
public class Object2Terrain : EditorWindow {
 
	[MenuItem("Terrain/Object to Terrain", false, 2000)] static void OpenWindow () {
 
		EditorWindow.GetWindow<Object2Terrain>(true);
	}
 
	private int resolution = 512;
	private Vector3 addTerrain;
	int bottomTopRadioSelected = 0;
	static string[] bottomTopRadio = new string[] { "Bottom Up", "Top Down"};
	private float shiftHeight = 0f;
 
	void OnGUI () {
 
		resolution = EditorGUILayout.IntField("Resolution", resolution);
		addTerrain = EditorGUILayout.Vector3Field("Add terrain", addTerrain);
		shiftHeight = EditorGUILayout.Slider("Shift height", shiftHeight, -1f, 1f);
		bottomTopRadioSelected = GUILayout.SelectionGrid(bottomTopRadioSelected, bottomTopRadio, bottomTopRadio.Length, EditorStyles.radioButton);
 
		if(GUILayout.Button("Create Terrain")){
 
			if(Selection.activeGameObject == null){
 
				EditorUtility.DisplayDialog("No object selected", "Please select an object.", "Ok");
				return;
			}
 
			else{
 
				CreateTerrain();
			}
		}
	}
 
	delegate void CleanUp();
 
	void CreateTerrain(){	
 
		//fire up the progress bar
		ShowProgressBar(1, 100);
 
		TerrainData terrain = new TerrainData();
		terrain.heightmapResolution = resolution;
		GameObject terrainObject = Terrain.CreateTerrainGameObject(terrain);
 
		Undo.RegisterCreatedObjectUndo(terrainObject, "Object to Terrain");
 
		MeshCollider collider = Selection.activeGameObject.GetComponent<MeshCollider>();
		CleanUp cleanUp = null;
 
		//Add a collider to our source object if it does not exist.
		//Otherwise raycasting doesn't work.
		if(!collider){
 
			collider = Selection.activeGameObject.AddComponent<MeshCollider>();
			cleanUp = () => DestroyImmediate(collider);
		}
 
		Bounds bounds = collider.bounds;	
		float sizeFactor = collider.bounds.size.y / (collider.bounds.size.y + addTerrain.y);
		terrain.size = collider.bounds.size + addTerrain;
		bounds.size = new Vector3(terrain.size.x, collider.bounds.size.y, terrain.size.z);
 
		// Do raycasting samples over the object to see what terrain heights should be
		float[,] heights = new float[terrain.heightmapWidth, terrain.heightmapHeight];	
		Ray ray = new Ray(new Vector3(bounds.min.x, bounds.max.y + bounds.size.y, bounds.min.z), -Vector3.up);
		RaycastHit hit = new RaycastHit();
		float meshHeightInverse = 1 / bounds.size.y;
		Vector3 rayOrigin = ray.origin;
 
		int maxHeight = heights.GetLength(0);
		int maxLength = heights.GetLength(1);
 
		Vector2 stepXZ = new Vector2(bounds.size.x / maxLength, bounds.size.z / maxHeight);
 
		for(int zCount = 0; zCount < maxHeight; zCount++){
 
			ShowProgressBar(zCount, maxHeight);
 
			for(int xCount = 0; xCount < maxLength; xCount++){
 
				float height = 0.0f;
 
				if(collider.Raycast(ray, out hit, bounds.size.y * 3)){
 
					height = (hit.point.y - bounds.min.y) * meshHeightInverse;
					height += shiftHeight;
 
					//bottom up
					if(bottomTopRadioSelected == 0){
 
						height *= sizeFactor;
					}
 
					//clamp
					if(height < 0){
 
						height = 0;
					}
				}
 
				heights[zCount, xCount] = height;
           		rayOrigin.x += stepXZ[0];
           		ray.origin = rayOrigin;
			}
 
			rayOrigin.z += stepXZ[1];
      		rayOrigin.x = bounds.min.x;
      		ray.origin = rayOrigin;
		}
 
		terrain.SetHeights(0, 0, heights);
 
		EditorUtility.ClearProgressBar();
 
		if(cleanUp != null){
 
			cleanUp();    
		}
	}
 
    void ShowProgressBar(float progress, float maxProgress){
 
		float p = progress / maxProgress;
		EditorUtility.DisplayProgressBar("Creating Terrain...", Mathf.RoundToInt(p * 100f)+ " %", p);
	}
}JavaScript - Object2Terrain.js @MenuItem ("Terrain/Object to Terrain")
 
static function Object2Terrain () {
	// See if a valid object is selected
	var obj = Selection.activeObject as GameObject;
	if (obj == null) { 
		EditorUtility.DisplayDialog("No object selected", "Please select an object.", "Cancel");
		return;
	}
	if (obj.GetComponent(MeshFilter) == null) {
		EditorUtility.DisplayDialog("No mesh selected", "Please select an object with a mesh.", "Cancel");
		return;
	}
	else if ((obj.GetComponent(MeshFilter) as MeshFilter).sharedMesh == null) {
		EditorUtility.DisplayDialog("No mesh selected", "Please select an object with a valid mesh.", "Cancel");
		return;		
	}
	if (Terrain.activeTerrain == null) {
		EditorUtility.DisplayDialog("No terrain found", "Please make sure a terrain exists.", "Cancel");
		return;
	}	
	var terrain = Terrain.activeTerrain.terrainData;
 
	// If there's no mesh collider, add one (and then remove it later when done)
	var addedCollider = false;
	var addedMesh = false;
	var objCollider = obj.collider as MeshCollider;
	if (objCollider == null) {
		objCollider = obj.AddComponent(MeshCollider);
		addedCollider = true;
	}
	else if (objCollider.sharedMesh == null) {
		objCollider.sharedMesh = (obj.GetComponent(MeshFilter) as MeshFilter).sharedMesh;
		addedMesh = true;
	}
 
	Undo.RegisterUndo (terrain, "Object to Terrain");
 
	var resolutionX = terrain.heightmapWidth;
	var resolutionZ = terrain.heightmapHeight;
	var heights = terrain.GetHeights(0, 0, resolutionX, resolutionZ);
 
	// Use bounds a bit smaller than the actual object; otherwise raycasting tends to miss at the edges
	var objectBounds = objCollider.bounds;
	var leftEdge = objectBounds.center.x - objectBounds.extents.x + .01;
	var bottomEdge = objectBounds.center.z - objectBounds.extents.z + .01;
	var stepX = (objectBounds.size.x - .019) / resolutionX;
	var stepZ = (objectBounds.size.z - .019) / resolutionZ;
 
	// Set up raycast vars
	var y = objectBounds.center.y + objectBounds.extents.y + .01;
	var hit : RaycastHit;
	var ray = new Ray(Vector3.zero, -Vector3.up);
	var rayDistance = objectBounds.size.y + .02;
	var heightFactor = 1.0 / rayDistance;
 
	// Do raycasting samples over the object to see what terrain heights should be
	var z = bottomEdge;
	for (zCount = 0; zCount < resolutionZ; zCount++) {
		var x = leftEdge;
		for (xCount = 0; xCount < resolutionX; xCount++) {
			ray.origin = Vector3(x, y, z);
			if (objCollider.Raycast(ray, hit, rayDistance)) {
				heights[zCount, xCount] = 1.0 - (y - hit.point.y)*heightFactor;
			}
			else {
				heights[zCount, xCount] = 0.0;
			}
			x += stepX;
		}
		z += stepZ;
	}
 
	terrain.SetHeights(0, 0, heights);
 
	if (addedMesh) {
		objCollider.sharedMesh = null;
	}
	if (addedCollider) {
		DestroyImmediate(objCollider);
	}
}
}
