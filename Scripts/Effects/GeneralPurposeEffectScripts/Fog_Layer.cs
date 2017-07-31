// Original url: http://wiki.unity3d.com/index.php/Fog_Layer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/Fog_Layer.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Joachim Ante 
DescriptionThis script lets you enable and disable fog per camera. 
For changing more render settings, see CameraRenderSettings 
UsageAttach the script to the camera you want to have fog enabled or disabled. Then click on the enabled checkbox in the title of the inspector to turn fog on or off for this camera. 
Javascript- FogLayer.js/*
 This script lets you enable and disable per camera.
 By enabling or disabling the script in the title of the inspector, you can turn fog on or off per camera.
*/
 
private var revertFogState = false;
 
function OnPreRender () {
	revertFogState = RenderSettings.fog;
	RenderSettings.fog = enabled;
}
 
function OnPostRender () {
	RenderSettings.fog = revertFogState;
}
 
@script AddComponentMenu ("Rendering/Fog Layer")
@script RequireComponent (Camera)
}
