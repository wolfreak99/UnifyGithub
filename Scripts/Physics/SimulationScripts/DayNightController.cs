// Original url: http://wiki.unity3d.com/index.php/DayNightController
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/DayNightController.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.SimulationScripts
{
Contents [hide] 
1 About 
1.1 Script Version 
1.2 Script Revision 
2 Description 
3 To-Do 
3.1 Enhancement: Skybox Material Transition 
3.2 Option: Allow GameObject Translation Instead of Rotation 
4 Script - C# 
5 References 
5.1 See Also 

About DayNightController Script 
Implements a Day/Night cycle relative to the game world, with a World-Time clock, and optional Direcitonal Light control. 
Script Version 0.0.1.0 
Script Revision 5/19/2011 


Description Add this script to a new GameObject to create a Day/Night cycle for the scene. The day/night cycle effect is achieved by modifying the scene ambient light color, fog color, and skybox material. The script will also rotate, fade, and enable/disable a directional light if one is attached to the same GameObject as the DayNightController script. The length of a complete day (in seconds) and the number of hours per day are modifiable in the script fields and allow calculation of the World-time hour-of-day. Each 'phase' of the day is considered to be 1/4 of the dayCycleLength. 

Note that the script will rotate the GameObject transform it is attached to, even if no directional light is attached. You will probably want to use a dedicated GameObject for this script and any attached directional light. 

The GameObject with this script should be placed roughly in the center of your scene, with a height of about 2/3 the length (or width) of the terrain. If that GameObject has a light, it should be a directional light pointed straight down (x:90, y:0, z:0). This light will then be rotated around its x-axis (relative to the scene; eg. as if you used the rotation tool locked on the green x axis) and will reach its horizontal peeks during the end of dusk and beginning of dawn, turning off during the night (upside-down rotation). 

The reset command will attempt to use the default skybox assets DawnDusk, Sunny2, and StarryNight if that package has been imported. The command will also choose acceptable color values and set the day cycle to two minutes. It is suggested that the directional light be a light-yellow or peach in color with a roughly 0.33f intensity. The script will not set any default values for the light, if one exists, so the light must be configured manually. 


To-Do Enhancement: Skybox Material Transition The script is fully-functional in its current state, and the default values produce a day/night cycle effect which is acceptable for many situations. 

However, a better effect could be achieved through an interpolated transition of the skybox materials during each phase change. The framework code to support such an update exists in the script as comments, but lacks a worker routine which can successfully perform the interpolation. This is an enhancement which should be added. 

Even without a material interpolation, it would be possible to create custom skybox materials which used the same textures set to various brightness levels. This would make the skybox change less drastic, but could still benefit from a fade effect. The existing, yet unused, skybox material change framework in the script could be adapted to implement either enhancement. 
Option: Allow GameObject Translation Instead of Rotation Another enhancement would be the option to rotate the light by moving the GameObject's position around the terrain at a radius of the current height, while rotating the GameObject to keep the light pointed at the center of the terrain. This could allow attaching a model to the GameObject and rendering a visible sun which tracks across the sky. This functionality would need to toggle with the existing rotation code in an either-or configuration. 


Script - C# using UnityEngine;
using System.Collections;
 
/// <summary>
/// Implements a Day/Night cycle relative to the game world, with a World-Time clock, and optional Direcitonal Light control.
/// </summary>
/// <!-- 
/// Version 0.0.1.0 (beta)
/// By Reed Kimble
/// Last Revision 5/19/2011
/// -->
/// <remarks>
/// Add this script to a new GameObject to create a Day/Night cycle for the scene. The day/night cycle effect is achieved by modifying the
/// scene ambient light color, fog color, and skybox material.  The script will also rotate, fade, and enable/disable a directional
/// light if one is attached to the same GameObject as the DayNightController script.  The length of a complete day (in seconds) and the number of
/// hours per day are modifiable in the script fields and allow calculation of the World-time hour-of-day.  Each 'phase' of the day is considered
/// to be 1/4 of the dayCycleLength.
/// 
/// Note that the script will rotate the GameObject transform it is attached to, even if no directional light is attached. You will probably want to 
/// use a dedicated GameObject for this script and any attached directional light.
/// 
/// The GameObject with this script should be placed roughly in the center of your scene, with a height of about 2/3 the length (or width) of the terrain.
/// If that GameObject has a light, it should be a directional light pointed straight down (x:90, y:0, z:0).  This light will then be rotated around its
/// x-axis (relative to the scene; eg. as if you used the rotation tool locked on the green x axis) and will reach its horizontal peeks during the
/// end of dusk and beginning of dawn, turning off during the night (upside-down rotation).
/// 
/// The reset command will attempt to use the default skybox assets DawnDusk, Sunny2, and StarryNight if that package has been imported.  The
/// command will also choose acceptable color values and set the day cycle to two minutes. It is suggested that the directional light be a light-
/// yellow or peach in color with a roughly 0.33f intensity.  The script will not set any default values for the light, if one exists, so the light
/// must be configured manually.
/// </remarks>
public class DayNightController : MonoBehaviour
{
    /// <summary>
    /// The number of real-world seconds in one game day.
    /// </summary>
    public float dayCycleLength;
 
    /// <summary>
    /// The current time within the day cycle. Modify to change the World Time.
    /// </summary>
    public float currentCycleTime;
 
    //Would be the amount of time the sky takes to transition if UpdateSkybox were used.
    //public float skyTransitionTime;
 
    /// <summary>
    /// The current 'phase' of the day; Dawn, Day, Dusk, or Night
    /// </summary>
    public DayPhase currentPhase;
 
    /// <summary>
    /// The number of hours per day used in the WorldHour time calculation.
    /// </summary>
    public float hoursPerDay;
 
    /// <summary>
    /// Dawn occurs at currentCycleTime = 0.0f, so this offsets the WorldHour time to make
    /// dawn occur at a specified hour. A value of 3 results in a 5am dawn for a 24 hour world clock.
    /// </summary>
    public float dawnTimeOffset;
 
    /// <summary>
    /// The calculated hour of the day, based on the hoursPerDay setting. Do not set this value.
    /// Change the time of day by calculating and setting the currentCycleTime.
    /// </summary>
    public int worldTimeHour;
 
    /// <summary>
    /// The scene ambient color used for full daylight.
    /// </summary>
    public Color fullLight;
 
    /// <summary>
    /// The scene ambient color used for full night.
    /// </summary>
    public Color fullDark;
 
    /// <summary>
    /// The scene skybox material to use at dawn and dusk.
    /// </summary>
    public Material dawnDuskSkybox;
 
    /// <summary>
    /// The scene fog color to use at dawn and dusk.
    /// </summary>
    public Color dawnDuskFog;
 
    /// <summary>
    /// The scene skybox material to use during the day.
    /// </summary>
    public Material daySkybox;
 
    /// <summary>
    /// The scene fog color to use during the day.
    /// </summary>
    public Color dayFog;
 
    /// <summary>
    /// The scene skybox material to use at night.
    /// </summary>
    public Material nightSkybox;
 
    /// <summary>
    /// The scene fog color to use at night.
    /// </summary>
    public Color nightFog;
 
    /// <summary>
    /// The calculated time at which dawn occurs based on 1/4 of dayCycleLength.
    /// </summary>
    private float dawnTime; 
 
    /// <summary>
    /// The calculated time at which day occurs based on 1/4 of dayCycleLength.
    /// </summary>
    private float dayTime;
 
    /// <summary>
    /// The calculated time at which dusk occurs based on 1/4 of dayCycleLength.
    /// </summary>
    private float duskTime;
 
    /// <summary>
    /// The calculated time at which night occurs based on 1/4 of dayCycleLength.
    /// </summary>
    private float nightTime;
 
    /// <summary>
    /// One quarter the value of dayCycleLength.
    /// </summary>
    private float quarterDay;
 
    //Would be the amount of time remaining in the skybox transition if UpdateSkybox were used.
    //private float remainingTransition;
 
    /// <summary>
    /// The specified intensity of the directional light, if one exists. This value will be
    /// faded to 0 during dusk, and faded from 0 back to this value during dawn.
    /// </summary>
    private float lightIntensity;
 
    /// <summary>
    /// Initializes working variables and performs starting calculations.
    /// </summary>
    void Initialize()
    {
        //remainingTransition = skyTransitionTime; //Would indicate that the game should start with an active transition, if UpdateSkybox were used.
        quarterDay = dayCycleLength * 0.25f;
        dawnTime = 0.0f;
        dayTime = dawnTime + quarterDay;
        duskTime = dayTime + quarterDay;
        nightTime = duskTime + quarterDay;
        if (light != null)
        { lightIntensity = light.intensity; }
    }
 
    /// <summary>
    /// Sets the script control fields to reasonable default values for an acceptable day/night cycle effect.
    /// </summary>
    void Reset()
    {
        dayCycleLength = 120.0f;
        //skyTransitionTime = 3.0f; //would be set if UpdateSkybox were used.
        hoursPerDay = 24.0f;
        dawnTimeOffset = 3.0f;
        fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);
        fullLight = new Color(253.0f / 255.0f, 248.0f / 255.0f, 223.0f / 255.0f);
        dawnDuskFog = new Color(133.0f / 255.0f, 124.0f / 255.0f, 102.0f / 255.0f);
        dayFog = new Color(180.0f / 255.0f, 208.0f / 255.0f, 209.0f / 255.0f);
        nightFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);
        Skybox[] skyboxes = AssetBundle.FindObjectsOfTypeIncludingAssets(typeof(Skybox)) as Skybox[];
        foreach (Skybox box in skyboxes)
        {
            if (box.name == "DawnDusk Skybox")
            { dawnDuskSkybox = box.material; }
            else if (box.name == "StarryNight Skybox")
            { nightSkybox = box.material; }
            else if (box.name == "Sunny2 Skybox")
            { daySkybox = box.material; }
        }
    }
 
    // Use this for initialization
    void Start()
    {
        Initialize();
    }
 
    // Update is called once per frame
    void Update()
    {
        // Rudementary phase-check algorithm:
        if (currentCycleTime > nightTime && currentPhase == DayPhase.Dusk)
        {
            SetNight();
        }
        else if (currentCycleTime > duskTime && currentPhase == DayPhase.Day)
        {
            SetDusk();
        }
        else if (currentCycleTime > dayTime && currentPhase == DayPhase.Dawn)
        {
            SetDay();
        }
        else if (currentCycleTime > dawnTime && currentCycleTime < dayTime && currentPhase == DayPhase.Night)
        {
            SetDawn();
        }
 
        // Perform standard updates:
        UpdateWorldTime();
        UpdateDaylight();
        UpdateFog();
        //UpdateSkybox(); //would be called if UpdateSkybox were used.
 
        // Update the current cycle time:
        currentCycleTime += Time.deltaTime;
        currentCycleTime = currentCycleTime % dayCycleLength;
    }
 
    /// <summary>
    /// Sets the currentPhase to Dawn, turning on the directional light, if any.
    /// </summary>
    public void SetDawn()
    {
        RenderSettings.skybox = dawnDuskSkybox; //would be commented out or removed if UpdateSkybox were used.
        //remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
        if (light != null)
        { light.enabled = true; }
        currentPhase = DayPhase.Dawn;
    }
 
    /// <summary>
    /// Sets the currentPhase to Day, ensuring full day color ambient light, and full
    /// directional light intensity, if any.
    /// </summary>
    public void SetDay()
    {
        RenderSettings.skybox = daySkybox; //would be commented out or removed if UpdateSkybox were used.
        //remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
        RenderSettings.ambientLight = fullLight;
        if (light != null)
        { light.intensity = lightIntensity; }
        currentPhase = DayPhase.Day;
    }
 
    /// <summary>
    /// Sets the currentPhase to Dusk.
    /// </summary>
    public void SetDusk()
    {
        RenderSettings.skybox = dawnDuskSkybox; //would be commented out or removed if UpdateSkybox were used.
        //remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
        currentPhase = DayPhase.Dusk;
    }
 
    /// <summary>
    /// Sets the currentPhase to Night, ensuring full night color ambient light, and
    /// turning off the directional light, if any.
    /// </summary>
    public void SetNight()
    {
        RenderSettings.skybox = nightSkybox; //would be commented out or removed if UpdateSkybox were used.
        //remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
        RenderSettings.ambientLight = fullDark;
        if (light != null)
        { light.enabled = false; }
        currentPhase = DayPhase.Night;
    }
 
    /// <summary>
    /// If the currentPhase is dawn or dusk, this method adjusts the ambient light color and direcitonal
    /// light intensity (if any) to a percentage of full dark or full light as appropriate. Regardless
    /// of currentPhase, the method also rotates the transform of this component, thereby rotating the
    /// directional light, if any.
    /// </summary>
    private void UpdateDaylight()
    {
        if (currentPhase == DayPhase.Dawn)
        {
            float relativeTime = currentCycleTime - dawnTime;
            RenderSettings.ambientLight = Color.Lerp(fullDark, fullLight, relativeTime / quarterDay);
            if (light != null)
            { light.intensity = lightIntensity * (relativeTime / quarterDay); }
        }
        else if (currentPhase == DayPhase.Dusk)
        {
            float relativeTime = currentCycleTime - duskTime;
            RenderSettings.ambientLight = Color.Lerp(fullLight, fullDark, relativeTime / quarterDay);
            if (light != null)
            { light.intensity = lightIntensity * ((quarterDay - relativeTime) / quarterDay); }
        }
 
        transform.Rotate(Vector3.up * ((Time.deltaTime / dayCycleLength) * 360.0f), Space.Self);
   }
 
    /// <summary>
    /// Interpolates the fog color between the specified phase colors during each phase's transition.
    /// eg. From DawnDusk to Day, Day to DawnDusk, DawnDusk to Night, and Night to DawnDusk
    /// </summary>
    private void UpdateFog()
    {
        if (currentPhase == DayPhase.Dawn)
        {
            float relativeTime = currentCycleTime - dawnTime;
            RenderSettings.fogColor = Color.Lerp(dawnDuskFog, dayFog, relativeTime / quarterDay);
        }
        else if (currentPhase == DayPhase.Day)
        {
            float relativeTime = currentCycleTime - dayTime;
            RenderSettings.fogColor = Color.Lerp(dayFog, dawnDuskFog, relativeTime / quarterDay);
        }
        else if (currentPhase == DayPhase.Dusk)
        {
            float relativeTime = currentCycleTime - duskTime;
            RenderSettings.fogColor = Color.Lerp(dawnDuskFog, nightFog, relativeTime / quarterDay);
        }
        else if (currentPhase == DayPhase.Night)
        {
            float relativeTime = currentCycleTime - nightTime;
            RenderSettings.fogColor = Color.Lerp(nightFog, dawnDuskFog, relativeTime / quarterDay);
        }
    }
 
    //Not yet implemented, but would be nice to allow a smoother transition of the Skybox material.
    //private void UpdateSkybox()
    //{
    //    if (remainingTransition > 0.0f)
    //    {
    //        if (currentPhase == DayCycle.Dawn)
    //        {
    //            //RenderSettings.skybox.Lerp(dawnDuskSkybox, nightSkybox, remainingTransition / skyTransitionTime);
    //        }
    //        if (currentPhase == DayCycle.Day)
    //        {
 
    //        }
    //        if (currentPhase == DayCycle.Dusk)
    //        {
 
    //        }
    //        if (currentPhase == DayCycle.Night)
    //        {
 
    //        }
    //        remainingTransition -= Time.deltaTime;
    //    }
    //}
 
    /// <summary>
    /// Updates the World-time hour based on the current time of day.
    /// </summary>
    private void UpdateWorldTime()
    {
        worldTimeHour = (int)((Mathf.Ceil((currentCycleTime / dayCycleLength) * hoursPerDay) + dawnTimeOffset) % hoursPerDay) + 1;
    }
 
    public enum DayPhase
    {
        Night = 0,
        Dawn = 1,
        Day = 2,
        Dusk = 3
    }
}References None. This is an original script. 


See Also These scripts provide a similar functionality, but in a different fashion. 
SunLight 
GameTime 
}
