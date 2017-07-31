// Original url: http://wiki.unity3d.com/index.php/Underwater_Script
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Underwater_Script.cs
// File based on original modification date of: 7 November 2012, at 21:48. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Contents [hide] 
1 Description 
2 Usage 
3 Javascript - Underwater.js 
4 C# - Underwater.cs 

DescriptionCreate simple underwater effects with Unity Indie. 
UsageAttach to your main camera and key in the y-position of your water plane for "underwaterLevel". Leave "noSkybox" blank. 
Javascript - Underwater.js//This script enables underwater effects. Attach to main camera.
 
//Define variables
var underwaterLevel = 7;
 
//The scene's default fog settings
private var defaultFog = RenderSettings.fog;
private var defaultFogColor = RenderSettings.fogColor;
private var defaultFogDensity = RenderSettings.fogDensity;
private var defaultSkybox = RenderSettings.skybox;
var noSkybox : Material;
 
function Start () {
	//Set the background color
	camera.backgroundColor = Color (0, 0.4, 0.7, 1);
}
 
function Update () {
	if (transform.position.y < underwaterLevel) {
		RenderSettings.fog = true;
		RenderSettings.fogColor = Color (0, 0.4, 0.7, 0.6);
		RenderSettings.fogDensity = 0.04;
		RenderSettings.skybox = noSkybox;
	}
 
	else {
		RenderSettings.fog = defaultFog;
		RenderSettings.fogColor = defaultFogColor;
		RenderSettings.fogDensity = defaultFogDensity;
		RenderSettings.skybox = defaultSkybox;
	}
}C# - Underwater.csusing UnityEngine;
using System.Collections;
 
public class Underwater : MonoBehaviour {
 
	//This script enables underwater effects. Attach to main camera.
 
    //Define variable
    public int underwaterLevel = 7;
 
    //The scene's default fog settings
    private bool defaultFog = RenderSettings.fog;
    private Color defaultFogColor = RenderSettings.fogColor;
    private float defaultFogDensity = RenderSettings.fogDensity;
    private Material defaultSkybox = RenderSettings.skybox;
    private Material noSkybox;
 
    void Start () {
	    //Set the background color
	    camera.backgroundColor = new Color(0, 0.4f, 0.7f, 1);
    }
 
    void Update () {
        if (transform.position.y < underwaterLevel)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0, 0.4f, 0.7f, 0.6f);
            RenderSettings.fogDensity = 0.04f;
            RenderSettings.skybox = noSkybox;
        }
        else
        {
            RenderSettings.fog = defaultFog;
            RenderSettings.fogColor = defaultFogColor;
            RenderSettings.fogDensity = defaultFogDensity;
            RenderSettings.skybox = defaultSkybox;
        }
    }
}
}
