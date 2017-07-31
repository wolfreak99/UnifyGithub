// Original url: http://wiki.unity3d.com/index.php/CameraRenderSettings
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/CameraRenderSettings.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Keli Hlodversson 
Contents [hide] 
1 Description 
2 Usage 
3 Javascript - CameraRenderSettings.js 
4 Variation 
4.1 Javascript - CameraRenderSettings2.js 

DescriptionThis script lets you change render settings per camera. This is an extended version of Joe Ante's Fog Layer script which allows enabling and disabling fog on different cameras. 
UsageAttach the script to the camera you want to have fog enabled or disabled. Then modify the render settings in the inspector instead of changing the global render settings. 
Javascript - CameraRenderSettings.js/*
 This script lets you change all render settings per camera.
 
*/
 
// Public variables -- set these in the inspector
var fog = RenderSettings.fog;
var fogColor = RenderSettings.fogColor;
var fogDensity = RenderSettings.fogDensity;
var ambientLight = RenderSettings.ambientLight;
var haloStrength = RenderSettings.haloStrength;
var flareStrength = RenderSettings.flareStrength;
 
// Private variables -- used to reset the render settings after the current camera has been rendered
private var _global_fog = RenderSettings.fog;
private var _global_fogColor = RenderSettings.fogColor;
private var _global_fogDensity = RenderSettings.fogDensity;
private var _global_ambientLight = RenderSettings.ambientLight;
private var _global_haloStrength = RenderSettings.haloStrength;
private var _global_flareStrength = RenderSettings.flareStrength;
 
private var dirty = false; // Used to flag that the render settings have been overridden and need a restore
 
function OnPreRender () {
    if (! enabled ) return; // If the component is disabled, use the global render settings
 
    // Save global render state:
    _global_fog = RenderSettings.fog;
    _global_fogColor = RenderSettings.fogColor;
    _global_fogDensity = RenderSettings.fogDensity;
    _global_ambientLight = RenderSettings.ambientLight;
    _global_haloStrength = RenderSettings.haloStrength;
    _global_flareStrength = RenderSettings.flareStrength;
 
 
    // Set local settings:
    RenderSettings.fog = fog;
    RenderSettings.fogColor = fogColor;
    RenderSettings.fogDensity = fogDensity;
    RenderSettings.ambientLight = ambientLight;
    RenderSettings.haloStrength = haloStrength;
    RenderSettings.flareStrength = flareStrength;
 
    dirty=true;
}
 
function OnPostRender () {
    if (! dirty ) return; // If the component was disabled in OnPreRender, then don't restore
 
    // Restore global settings:
    RenderSettings.fog = _global_fog;
    RenderSettings.fogColor = _global_fogColor;
    RenderSettings.fogDensity = _global_fogDensity;
    RenderSettings.ambientLight = _global_ambientLight;
    RenderSettings.haloStrength = _global_haloStrength;
    RenderSettings.flareStrength = _global_flareStrength;
 
    dirty=false;
}
 
// Reset the component to revert to the current global settings
function Reset () {
    fog = RenderSettings.fog;
    fogColor = RenderSettings.fogColor;
    fogDensity = RenderSettings.fogDensity;
    ambientLight = RenderSettings.ambientLight;
    haloStrength = RenderSettings.haloStrength;
    flareStrength = RenderSettings.flareStrength;
}
 
 
@script AddComponentMenu ("Rendering/Per Camera Render Settings")
@script RequireComponent (Camera)VariationIf you are going to have seperate render settings for each and every camera, you can simplify the above script to not to restore the global render settings on post render, as the next camera will set it anyway to its own settings: 
Javascript - CameraRenderSettings2.js/*
 This script lets you change all render settings per camera.
 Note: This variant of the script requires that all cameras have this component 
 attached, as it does not preserve the global render settings!
 
*/
 
// Public variables -- set these in the inspector
var fog = RenderSettings.fog;
var fogColor = RenderSettings.fogColor;
var fogDensity = RenderSettings.fogDensity;
var ambientLight = RenderSettings.ambientLight;
var haloStrength = RenderSettings.haloStrength;
var flareStrength = RenderSettings.flareStrength;
 
function OnPreRender () {
    // Set local settings:
    RenderSettings.fog = fog;
    RenderSettings.fogColor = fogColor;
    RenderSettings.fogDensity = fogDensity;
    RenderSettings.ambientLight = ambientLight;
    RenderSettings.haloStrength = haloStrength;
    RenderSettings.flareStrength = flareStrength;
}
 
// Reset the component to revert to the current global settings
function Reset () {
    fog = RenderSettings.fog;
    fogColor = RenderSettings.fogColor;
    fogDensity = RenderSettings.fogDensity;
    ambientLight = RenderSettings.ambientLight;
    haloStrength = RenderSettings.haloStrength;
    flareStrength = RenderSettings.flareStrength;
}
 
 
@script AddComponentMenu ("Rendering/Per Camera Render Settings2")
@script RequireComponent (Camera)
}
