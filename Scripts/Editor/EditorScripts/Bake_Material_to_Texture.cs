// Original url: http://wiki.unity3d.com/index.php/Bake_Material_to_Texture
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Bake_Material_to_Texture.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: ReJ aka Renaldas Zioma 
Description Allows to bake complex materials into a single texture. Can be useful for converting assets from Desktop to Mobile 
Usage You must place the script in a folder named "Editor" in your project's Assets folder for it to work properly. 
Select Material asset, then choose "Custom/Bake Material ..." menu item. Editor window will open where you can tweak additional lighting and setup texture parameters. Press "Bake" button to save results. Script will generate Texture asset next to original Material asset. 
Select multiple Material assets, then choose "Custom/Bake Materials" menu item to bake them all. 
 
JavaScript - BakeMaterial.js class BakeMaterialSettings
{
	private static var kEditorPrefsName = "BakeMaterialSettings";
 
	static var kBakingLayerShouldBeUnusedInScene = 30;
	static var kStandardTexNames = new Array ("_MainTex", "_BumpMap", "_Detail", "_ParallaxMap", "_Parallax");
 
	var bakeAlpha = false;
	var bakeMainTexAsWhite = false;
	var minTextureResolution = 8;
	var maxTextureResolution = 2048;
 
	var emptyScene = false;
	var useCustomLights = false;
	var ambient = Color.black;
 
	static var kLights = 3;
	var enableLight = new boolean[kLights];
	var colorLight = new Color[kLights];
	var dirLight = new Vector2[kLights];
 
	function BakeMaterialSettings ()
	{
		Load ();
	}
 
	function Load ()
	{
		bakeAlpha = EditorPrefs.GetBool(kEditorPrefsName + ".bakeAlpha");
		bakeMainTexAsWhite = EditorPrefs.GetBool(kEditorPrefsName + ".bakeMainTexAsWhite");
		minTextureResolution = EditorPrefs.GetInt(kEditorPrefsName + ".minTextureResolution", 8);
		maxTextureResolution = EditorPrefs.GetInt(kEditorPrefsName + ".maxTextureResolution", 2048);
 
		emptyScene = EditorPrefs.GetBool(kEditorPrefsName + ".emptyScene");
		useCustomLights = EditorPrefs.GetBool(kEditorPrefsName + ".useCustomLights");
		ambient.r = EditorPrefs.GetFloat(kEditorPrefsName + ".ambient.r");
		ambient.g = EditorPrefs.GetFloat(kEditorPrefsName + ".ambient.g");
		ambient.b = EditorPrefs.GetFloat(kEditorPrefsName + ".ambient.b");
		ambient.a = EditorPrefs.GetFloat(kEditorPrefsName + ".ambient.a", 1.0f);
 
		for (var q = 0; q < kLights; ++q)
		{
			enableLight[q] = EditorPrefs.GetBool(kEditorPrefsName + ".enableLight" + q);
			colorLight[q].r = EditorPrefs.GetFloat(kEditorPrefsName + ".color.r" + q, 0.5f);
			colorLight[q].g = EditorPrefs.GetFloat(kEditorPrefsName + ".color.g" + q, 0.5f);
			colorLight[q].b = EditorPrefs.GetFloat(kEditorPrefsName + ".color.b" + q, 0.5f);
			colorLight[q].a = EditorPrefs.GetFloat(kEditorPrefsName + ".color.a" + q, 1.0f);
			dirLight[q].x = EditorPrefs.GetFloat(kEditorPrefsName + ".dir.x" + q);
			dirLight[q].y = EditorPrefs.GetFloat(kEditorPrefsName + ".dir.y" + q);
		}
	}
 
	function Save ()
	{
		EditorPrefs.SetBool(kEditorPrefsName + ".bakeAlpha", bakeAlpha);
		EditorPrefs.SetBool(kEditorPrefsName + ".bakeMainTexAsWhite", bakeMainTexAsWhite);
		EditorPrefs.SetInt(kEditorPrefsName + ".minTextureResolution", minTextureResolution);
		EditorPrefs.SetInt(kEditorPrefsName + ".maxTextureResolution", maxTextureResolution);
 
		EditorPrefs.GetBool(kEditorPrefsName + ".emptyScene", emptyScene);
		EditorPrefs.SetBool(kEditorPrefsName + ".useCustomLights", useCustomLights);
		EditorPrefs.SetFloat(kEditorPrefsName + ".ambient.r", ambient.r);
		EditorPrefs.SetFloat(kEditorPrefsName + ".ambient.g", ambient.g);
		EditorPrefs.SetFloat(kEditorPrefsName + ".ambient.b", ambient.b);
		EditorPrefs.SetFloat(kEditorPrefsName + ".ambient.a", ambient.a);
 
		for (var q = 0; q < kLights; ++q)
		{
			EditorPrefs.SetBool(kEditorPrefsName + ".enableLight" + q, enableLight[q]);
			EditorPrefs.SetFloat(kEditorPrefsName + ".color.r" + q, colorLight[q].r);
			EditorPrefs.SetFloat(kEditorPrefsName + ".color.g" + q, colorLight[q].g);
			EditorPrefs.SetFloat(kEditorPrefsName + ".color.b" + q, colorLight[q].b);
			EditorPrefs.SetFloat(kEditorPrefsName + ".color.a" + q, colorLight[q].a);
			EditorPrefs.SetFloat(kEditorPrefsName + ".dir.x" + q, dirLight[q].x);
			EditorPrefs.SetFloat(kEditorPrefsName + ".dir.y" + q, dirLight[q].y);
		}
	}
}
 
class BakeMaterial extends EditorWindow
{
	private static var kMateriBakeNodeName = "__MateriaBakeSetup";
	private static var kWindowMinSize = Vector2 (300, 386);
 
	private static var settings : BakeMaterialSettings;
	private static var visible : boolean = false;
 
	private var camera : GameObject;
	private var plane : GameObject;
	private var previewTexture : Texture;
	private var lights : GameObject[] = new GameObject[BakeMaterialSettings.kLights];
	private var stateChanged = false;
 
	private var texViewScrollPosition = Vector2.zero;
	private var lastMaterial : Material;
 
	private var originalScene = "";
 
	private var scheduleBakeOnNextUpdate = false;
 
 
	private function SetupScene ()
	{
		DestroyScene ();
		var oldGo = GameObject.Find(kMateriBakeNodeName);
		if (oldGo)
			DestroyImmediate (oldGo);
		camera = new GameObject (kMateriBakeNodeName, Camera);
		plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
 
		var cam = camera;
		cam.camera.backgroundColor = Color.black;
		cam.camera.clearFlags = CameraClearFlags.SolidColor;
		cam.camera.orthographic = true;
		cam.camera.orthographicSize = 5.0;
		cam.camera.cullingMask = 1 << settings.kBakingLayerShouldBeUnusedInScene;
 
		plane.transform.parent = cam.transform;
		plane.transform.position = Vector3.forward * 10.0;
		plane.transform.rotation = Quaternion.Euler (0, 0, 180) * Quaternion.Euler (-90, 0, 0);
		plane.layer = settings.kBakingLayerShouldBeUnusedInScene;
 
		for (var l in lights)
		{
			l = new GameObject ("Light", Light);
			l.light.type = LightType.Directional;
			l.light.cullingMask = 1 << settings.kBakingLayerShouldBeUnusedInScene;
			l.transform.parent = cam.transform;
			l.active = false;
		}
	}
 
	private function UpdateScene (m : Material)
	{
		for (q = 0; q < settings.kLights; ++q)
		{
			lights[q].active = settings.useCustomLights & settings.enableLight[q];
			lights[q].light.color = settings.colorLight[q];
			lights[q].transform.rotation = 
				Quaternion.AngleAxis(settings.dirLight[q].x, Vector3.up) *
				Quaternion.AngleAxis(settings.dirLight[q].y, Vector3.right);
		}
 
		if (settings.useCustomLights)
			RenderSettings.ambientLight = settings.ambient;
		else if (settings.emptyScene)
			RenderSettings.ambientLight = Color.white;
 
		plane.renderer.material = m;
	}
 
	private function DestroyScene ()
	{
		GameObject.DestroyImmediate (camera);
		GameObject.DestroyImmediate (plane);
		GameObject.DestroyImmediate (previewTexture);
	}
 
	function UpdateMaterialPreview (m : Material) : RenderTexture
	{
		if (!m)
			return;
 
		var saveAmbientLight = RenderSettings.ambientLight;
		var saveMainTexture = m.mainTexture;
		if (settings.bakeMainTexAsWhite)
			m.mainTexture = null;
 
 
		// setup
		if (!camera)
			SetupScene ();
		camera.SetActiveRecursively(true);
		UpdateScene (m);
 
		var res = FindLargestTextureResolution (plane.renderer.sharedMaterial, settings.minTextureResolution, settings.maxTextureResolution);
		var rt = RenderCameraToRenderTexture (camera.camera, res.x, res.y);
 
		// restore
		camera.SetActiveRecursively(false);
		RenderSettings.ambientLight = saveAmbientLight;
		m.mainTexture = saveMainTexture;
 
		previewTexture = rt;
		return rt;
	}
 
 	function CaptureMaterial(m : Material)
	{
		var matAssetPath = AssetDatabase.GetAssetPath (m);
		var assetPath = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (matAssetPath), System.IO.Path.GetFileNameWithoutExtension (matAssetPath));
 
		var rt = UpdateMaterialPreview (m);
		RenderTextureToPNG (rt, settings.bakeAlpha, assetPath + ".png");
	}
 
	function OnEnable ()
	{
		if (!settings)
			settings = new BakeMaterialSettings ();
		SetupScene ();
		visible = true;
	}
 
	function OnDisable ()
	{
		DestroyScene ();
		settings.Save ();
		visible = false;
	}
 
	static function GetTargetMaterial () : Material
	{
		return EditorUtility.InstanceIDToObject (Selection.activeInstanceID) as Material;
	}
 
	function OnSelectionChange ()
	{
		Repaint ();
	}
 
	function Update ()
	{
		var rebuildScene = false;
		if (scheduleBakeOnNextUpdate)
		{
			Bake ();
			scheduleBakeOnNextUpdate = false;
			rebuildScene = true;
		}
 
		if (originalScene == "" && EditorApplication.currentScene == "")
			settings.emptyScene = true;
 
		if (settings.emptyScene && originalScene == "" && EditorApplication.currentScene != "")
		{
			DestroyScene ();
			if (EditorApplication.SaveCurrentSceneIfUserWantsTo ())
			{
				originalScene = EditorApplication.currentScene;
				EditorApplication.NewScene ();
			}
			else
				settings.emptyScene = false;
			rebuildScene = true;			
		}
		else if (!settings.emptyScene && originalScene != "")
		{
			EditorApplication.OpenScene (originalScene);
			rebuildScene = true;
			originalScene = "";
		}
 
		if (rebuildScene)
		{
			SetupScene ();
		}
 
		if (rebuildScene || stateChanged || !settings.emptyScene)
		{
			UpdateMaterialPreview (lastMaterial);
			Repaint ();
			stateChanged = false;
		}
	}
 
	function OnGUI ()
	{
		var material = GetTargetMaterial ();
		if (lastMaterial != material)
			UpdateMaterialPreview (material);
		if (material)
			lastMaterial = material;
 
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(200));
				if (!(originalScene == "" && EditorApplication.currentScene == ""))
				{
					settings.emptyScene = !EditorGUILayout.BeginToggleGroup("Scene ligthing", !settings.emptyScene);
					EditorGUILayout.EndToggleGroup();
				}
				settings.useCustomLights = EditorGUILayout.BeginToggleGroup("Custom lighting", settings.useCustomLights);
				if (settings.useCustomLights)
				{
					EditorGUI.indentLevel = 1;
					settings.ambient = EditorGUILayout.ColorField("Ambient", settings.ambient);
					for (var q = 0; q < settings.kLights; ++q)
					{
						settings.enableLight[q] = EditorGUILayout.BeginToggleGroup("Light", settings.enableLight[q]);
						EditorGUI.indentLevel = 2;
							settings.colorLight[q] = EditorGUILayout.ColorField("Color", settings.colorLight[q]);
							settings.dirLight[q] = EditorGUILayout.Vector2Field("Direction", settings.dirLight[q]);
						EditorGUILayout.EndToggleGroup();
					}
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.EndToggleGroup();
 
				settings.bakeAlpha = EditorGUILayout.Toggle("Bake Alpha", settings.bakeAlpha);
				settings.bakeMainTexAsWhite = !EditorGUILayout.Toggle("MainTex", !settings.bakeMainTexAsWhite);
				settings.minTextureResolution = EditorGUILayout.IntField("Min Resolution", settings.minTextureResolution);
				settings.maxTextureResolution = EditorGUILayout.IntField("Max Resolution", settings.maxTextureResolution);
				settings.minTextureResolution = Mathf.Max(2, settings.minTextureResolution);
				settings.maxTextureResolution = Mathf.Max(settings.minTextureResolution, settings.maxTextureResolution);
 
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Bake"))
				{
					CaptureMaterial (lastMaterial);
				}
				if (GUILayout.Button("Bake Selected"))
				{
					scheduleBakeOnNextUpdate = true;
				}
				EditorGUILayout.EndHorizontal();
 
			EditorGUILayout.EndVertical();
 
			texViewScrollPosition = EditorGUILayout.BeginScrollView (texViewScrollPosition);
				var r = GUILayoutUtility.GetAspectRect(1.0f);
				if (previewTexture)
					EditorGUI.DrawPreviewTexture(r, previewTexture);
			EditorGUILayout.EndScrollView();		
		EditorGUILayout.EndHorizontal();
 
		if (GUI.changed)
		{
			stateChanged = true;
		}
	}
 
	@MenuItem("Custom/Bake Material ...", false, 5)
	static function CreateBakeEditor()
	{
		var window = EditorWindow.GetWindow(BakeMaterial);
		window.title = "Bake Material";
		window.minSize = kWindowMinSize;
		window.Show();
	}
 
	@MenuItem("Custom/Bake Selected Materials", false, 4)
	static function Bake()
	{
		var instanceIDs = Selection.instanceIDs;
		var currentScene = EditorApplication.currentScene;
 
		var wasAlreadyVisible = BakeMaterial.visible;
		var window = EditorWindow.GetWindow(BakeMaterial);
 
		if (window.settings.emptyScene)
		{
			if (!EditorApplication.SaveCurrentSceneIfUserWantsTo ())
				return;
			EditorApplication.NewScene ();
		}
 
		window.SetupScene ();
		for (var i in instanceIDs)
		{
			var m : Material = EditorUtility.InstanceIDToObject (i) as Material;
			if (m)
				window.CaptureMaterial (m);
		}
		window.DestroyScene ();
 
		if (window.settings.emptyScene && currentScene)
		{
			EditorApplication.OpenScene (currentScene);
		}
 
		if (!wasAlreadyVisible)
			window.Close ();
	}
 
	static function FindLargestTextureResolution (m : Material, minTexRes : int, maxTexRes : int) : Vector2
	{
		var res = Vector2 (minTexRes, minTexRes);
		for (var n in BakeMaterialSettings.kStandardTexNames)
		{
			if (!m.HasProperty (n))
				continue;
 
			var t : Texture = m.GetTexture (n);
			if (!t)
				continue;
 
			res.x = Mathf.Max (res.x, t.width);
			res.y = Mathf.Max (res.y, t.height);
		}
		res.x = Mathf.Min (res.x, maxTexRes);
		res.y = Mathf.Min (res.y, maxTexRes);
		return res;
	}
 
	static function RenderCameraToRenderTexture (cam : Camera, width : int, height : int) : RenderTexture
	{
		var rt = cam.camera.targetTexture;
		if (rt && rt.width != width && rt.height != height)
			DestroyImmediate(rt);
		if (!rt)
			rt = new RenderTexture (width, height, 24);
		cam.camera.targetTexture = rt;
		cam.camera.Render ();
		return rt;
	}
 
	static function RenderTextureToPNG (rt : RenderTexture, bakeAlpha : boolean, assetPath : String)
	{
		RenderTexture.active = rt;
 
		var screenShot = new Texture2D (rt.width, rt.height, bakeAlpha? TextureFormat.ARGB32 : TextureFormat.RGB24, false);
		screenShot.ReadPixels (Rect (0, 0, rt.width, rt.height), 0, 0); 
 
		RenderTexture.active = null;
 
		var bytes = screenShot.EncodeToPNG (); 
		System.IO.File.WriteAllBytes (assetPath, bytes);
 
		AssetDatabase.ImportAsset (assetPath, ImportAssetOptions.ForceUpdate);
	}
}
}
