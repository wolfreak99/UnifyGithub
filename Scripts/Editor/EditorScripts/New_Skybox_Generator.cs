/*************************
 * Original url: http://wiki.unity3d.com/index.php/New_Skybox_Generator
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/New_Skybox_Generator.cs
 * File based on original modification date of: 31 May 2013, at 22:18. 
 *
 * Author: ReJ aka Renaldas Zioma 
 *
 * Description 
 *   
 * Usage 
 *   
 * JavaScript - NewSkyBoxGenerator.js 
 *   
 * CSharp - NewSkyBoxGenerator.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Generates Skybox by rendering 6 images and combining them with skybox material. Select multiple scene objects to render Skyboxes from multiple locations. 
    Usage You must place the script in a folder named "Editor" in your project's Assets folder for it to work properly. 
    Select one or more scene objects which you want to use as the origin point(s) for the Skybox(es), then choose "Custom/Render Skybox" menu item and wait for a few seconds. Generated skybox images will appear under folder named "Skyboxes". You will find Skybox material in the same directory. Such Skybox material can be used in "Edit/Render Settings". 
    JavaScript - NewSkyBoxGenerator.js class BakeSkybox
    {
    	static var faceSize = 512;
    	static var directory = "Assets/Skyboxes";
    	static var skyboxShader = "RenderFX/Skybox";
     
     
    	static var skyBoxImage = new Array ("front", "right", "back", "left", "up", "down");
    	static var skyBoxProps = new Array ("_FrontTex", "_RightTex", "_BackTex", "_LeftTex", "_UpTex", "_DownTex");
     
    	static var skyDirection = new Array (Vector3 (0,0,0), Vector3 (0,-90,0), Vector3 (0,180,0), Vector3 (0,90,0), Vector3 (-90,0,0), Vector3 (90,0,0));
     
    	@MenuItem("Custom/Bake Skybox", false, 4)
    	static function Bake()
    	{
    		if (Selection.transforms.Length == 0)
    		{
    			Debug.LogWarning ("Select at least one scene object as a skybox center!");
    			return;
    		}
     
    		if (!System.IO.Directory.Exists(directory))
    			System.IO.Directory.CreateDirectory(directory);
     
    		for (var t in Selection.transforms)
    			RenderSkyboxTo6PNG(t);
    	}
     
    	static function RenderSkyboxTo6PNG(t : Transform)
    	{
    		var go = new GameObject ("SkyboxCamera", Camera);
     
    		go.camera.backgroundColor = Color.black;
    		go.camera.clearFlags = CameraClearFlags.Skybox;
    		go.camera.fieldOfView = 90;    
    		go.camera.aspect = 1.0;
     
    		go.transform.position = t.position;
    		go.transform.rotation = Quaternion.identity;
     
    		// render skybox        
    		for (var orientation = 0; orientation < skyDirection.length ; orientation++)
    		{
    			var assetPath = System.IO.Path.Combine(directory, t.name + "_" + skyBoxImage[orientation] + ".png");
    			RenderSkyBoxFaceToPNG(orientation, go.camera, assetPath);
    		}
    		GameObject.DestroyImmediate (go);
     
    		// wire skybox material
    		AssetDatabase.Refresh();
     
    		var skyboxMaterial = new Material (Shader.Find(skyboxShader));        
    		for (orientation = 0; orientation < skyDirection.length ; orientation++)
    		{
    			var texPath = System.IO.Path.Combine(directory, t.name + "_" + skyBoxImage[orientation] + ".png");
    			var tex : Texture2D = AssetDatabase.LoadAssetAtPath(texPath, Texture2D) as Texture2D;
    			tex.wrapMode = TextureWrapMode.Clamp;
    			skyboxMaterial.SetTexture(skyBoxProps[orientation] as String, tex);
    		}
     
    		// save material
    		var matPath = System.IO.Path.Combine(directory, t.name + "_skybox" + ".mat");
    		AssetDatabase.CreateAsset(skyboxMaterial, matPath);
    	}
     
    	static function RenderSkyBoxFaceToPNG(orientation : int, cam : Camera, assetPath : String)
    	{
    		cam.transform.eulerAngles = skyDirection[orientation];
    		var rt = new RenderTexture (faceSize, faceSize, 24);
    		cam.camera.targetTexture = rt;
    		cam.camera.Render();
    		RenderTexture.active = rt;
     
    		var screenShot = new Texture2D (faceSize, faceSize, TextureFormat.RGB24, false);
    		screenShot.ReadPixels (Rect (0, 0, faceSize, faceSize), 0, 0); 
     
    		RenderTexture.active = null;
    		GameObject.DestroyImmediate (rt);
     
    		var bytes = screenShot.EncodeToPNG(); 
    		System.IO.File.WriteAllBytes (assetPath, bytes);
     
    		AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    	}
    }CSharp - NewSkyBoxGenerator.cs using UnityEngine;
    using UnityEditor;
    using System;
    using System.IO;
     
    class NewSkyBoxGenerator
    {
    	static int faceSize = 512;
    	static string directory = "Assets/Skyboxes";
    	static string skyboxShader = "RenderFX/Skybox";
     
    	static string[] skyBoxImage = new string[]{"front", "right", "back", "left", "up", "down"};
    	static string[] skyBoxProps = new string[]{"_FrontTex", "_RightTex", "_BackTex", "_LeftTex", "_UpTex", "_DownTex"};
     
    	static Vector3[] skyDirection = new Vector3[]{new Vector3(0,0,0), new Vector3(0,-90,0), new Vector3(0,180,0), new Vector3(0,90,0), new Vector3(-90,0,0), new Vector3(90,0,0)};
     
    	[MenuItem("GameObject/Custom/Bake Skybox", false, 4)]
    	static void Bake()
    	{
    		if (Selection.transforms.Length == 0)
    		{
    			Debug.LogWarning ("Select at least one scene object as a skybox center!");
    			return;
    		}
     
    		if (!Directory.Exists(directory))
    			Directory.CreateDirectory(directory);
     
    		foreach(Transform t in Selection.transforms)
    			RenderSkyboxTo6PNG(t);
    	}
     
    	static void RenderSkyboxTo6PNG(Transform t)
    	{
    		GameObject go = new GameObject("SkyboxCamera", typeof(Camera));
     
    		go.camera.backgroundColor = Color.black;
    		go.camera.clearFlags = CameraClearFlags.Skybox;
    		go.camera.fieldOfView = 90;    
    		go.camera.aspect = 1.0f;
     
    		go.transform.position = t.position;
    		go.transform.rotation = Quaternion.identity;
     
    		//Render skybox        
    		for (int orientation = 0; orientation < skyDirection.Length ; orientation++)
    		{
    			string assetPath = Path.Combine(directory, t.name + "_" + skyBoxImage[orientation] + ".png");
    			RenderSkyBoxFaceToPNG(orientation, go.camera, assetPath);
    		}
    		GameObject.DestroyImmediate(go);
     
    		//Wire skybox material
    		AssetDatabase.Refresh();
     
    		Material skyboxMaterial = new Material(Shader.Find(skyboxShader));        
    		for (int orientation = 0; orientation < skyDirection.Length ; orientation++)
    		{
    			string texPath = Path.Combine(directory, t.name + "_" + skyBoxImage[orientation] + ".png");
    			Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D));
    			tex.wrapMode = TextureWrapMode.Clamp;
    			skyboxMaterial.SetTexture(skyBoxProps[orientation], tex);
    		}
     
    		//Save material
    		string matPath = Path.Combine(directory, t.name + "_skybox" + ".mat");
    		AssetDatabase.CreateAsset(skyboxMaterial, matPath);
    	}
     
    	static void RenderSkyBoxFaceToPNG(int orientation, Camera cam, string assetPath)
    	{
    		cam.transform.eulerAngles = skyDirection[orientation];
    		RenderTexture rt = new RenderTexture(faceSize, faceSize, 24);
    		cam.camera.targetTexture = rt;
    		cam.camera.Render();
    		RenderTexture.active = rt;
     
    		Texture2D screenShot = new Texture2D(faceSize, faceSize, TextureFormat.RGB24, false);
    		screenShot.ReadPixels(new Rect(0, 0, faceSize, faceSize), 0, 0); 
     
    		RenderTexture.active = null;
    		GameObject.DestroyImmediate(rt);
     
    		byte[] bytes = screenShot.EncodeToPNG(); 
    		File.WriteAllBytes(assetPath, bytes);
     
    		AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    	}
}
}
