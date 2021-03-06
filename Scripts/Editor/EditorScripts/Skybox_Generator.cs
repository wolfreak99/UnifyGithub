/*************************
 * Original url: http://wiki.unity3d.com/index.php/Skybox_Generator
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Skybox_Generator.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Ethan Vosburgh (with help from Joe ;) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Generates the 6 images necessary to create a Skybox in Unity. 
    Usage You must place the script in a folder named "Editor" in your project's Assets folder for it to work properly. 
    The script will create a menu called "Custom/Render Skybox". When selected a dialog will pop up with a selection for the Transform you want to use as the origin point for the Skybox. Once the Transform is set hit "Render" and wait a few seconds, then Refresh the Project Pane and the 6 generated images will import into Unity in a folder named "Skyboxes". Be warned that each time you run this script it will overwrite previously generated images. 
    JavaScript - SkyBoxGenerator.js #pragma strict
    class SkyBoxGenerator extends ScriptableWizard
    {
        var renderFromPosition : Transform;
     
        var skyBoxImage = new Array ("frontImage", "rightImage", "backImage", "leftImage", "upImage", "downImage");
     
        var skyDirection = new Array (Vector3 (0,0,0), Vector3 (0,-90,0), Vector3 (0,180,0), Vector3 (0,90,0), Vector3 (-90,0,0), Vector3 (90,0,0));
     
     
        function OnWizardUpdate()
        {
            helpString = "Select transform to render from";
            isValid = (renderFromPosition != null);
        }
     
        function OnWizardCreate()
        {
            var go = new GameObject ("SkyboxCamera", Camera);
     
            go.camera.backgroundColor = Color.black;
            go.camera.clearFlags = CameraClearFlags.Skybox;
            go.camera.fieldOfView = 90;    
            go.camera.aspect = 1.0;
     
            go.transform.position = renderFromPosition.position;
     
            if (renderFromPosition.renderer)
            {
                go.transform.position = renderFromPosition.renderer.bounds.center;
            }
     
            go.transform.rotation = Quaternion.identity;
     
            for (var orientation = 0; orientation < skyDirection.length ; orientation++)
            {
                renderSkyImage(orientation, go);
            }
     
            DestroyImmediate (go);
        }
     
        @MenuItem("Custom/Render Skybox", false, 4)
        static function RenderSkyBox()
        {
            ScriptableWizard.DisplayWizard ("Render SkyBox", SkyBoxGenerator, "Render!");
        }
     
        function renderSkyImage(orientation : int, go : GameObject)
        {
    	go.transform.eulerAngles = skyDirection[orientation];
    	var screenSize = 1024;
    	var rt = new RenderTexture (screenSize, screenSize, 24);
    	go.camera.targetTexture = rt;
    	var screenShot = new Texture2D (screenSize, screenSize, TextureFormat.RGB24, false);
    	go.camera.Render();
    	RenderTexture.active = rt;
    	screenShot.ReadPixels (Rect (0, 0, screenSize, screenSize), 0, 0); 
     
    	RenderTexture.active = null;
    	DestroyImmediate (rt);
    	var bytes = screenShot.EncodeToPNG(); 
     
    	var directory = "Assets/Skyboxes";
    	if (!System.IO.Directory.Exists(directory))
    	System.IO.Directory.CreateDirectory(directory);
    	System.IO.File.WriteAllBytes (System.IO.Path.Combine(directory, skyBoxImage[orientation] + ".png"), bytes);   
        }
}
}
