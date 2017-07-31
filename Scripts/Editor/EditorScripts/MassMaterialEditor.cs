// Original url: http://wiki.unity3d.com/index.php/MassMaterialEditor
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/MassMaterialEditor.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
file: MassMaterialEditor.cs 
from: http://unity3d.com/support/resources/example-projects/charactercustomization 
slightly modified to include a mass shader 
using System;
using UnityEditor;
using UnityEngine;
 
class MassMaterialEditor : EditorWindow
{
    static MassMaterialEditor window;
 
	String oldShaderName;
    Color oldMainColor;
    Color oldSpecColor;
    float oldShininess;
    Texture2D oldLightmap;
 
    [MenuItem("Character Generator/Mass Material Editor")]
    static void Execute()
    {
        if (window == null)
            window = (MassMaterialEditor)GetWindow(typeof (MassMaterialEditor));
        window.Show();
    }
 
    void OnGUI()
    {
        Shader shader = Shader.Find(EditorGUILayout.TextField("Shader", oldShaderName));
		if (shader != null) if (shader.name != oldShaderName)
		{
			oldShaderName = shader.name;
			SetProperty("_Shader", shader);
		}
 
        EditorGUILayout.Separator();
        GUILayout.Label("--- Render Settings ---");
 
        RenderSettings.fog = EditorGUILayout.Toggle("Fog", RenderSettings.fog);
        RenderSettings.fogColor = EditorGUILayout.ColorField("Fog Color", RenderSettings.fogColor, GUILayout.Width(140));
        RenderSettings.fogDensity = EditorGUILayout.Slider("Fog Density", RenderSettings.fogDensity, 0, 1);
        RenderSettings.ambientLight = EditorGUILayout.ColorField("Ambient", RenderSettings.ambientLight, GUILayout.Width(140));
 
        EditorGUILayout.Separator();
        GUILayout.Label("--- Material Settings ---");
        GUILayout.Label("Selected Materials are modified");
 
        Color mainColor = EditorGUILayout.ColorField("Main Color", oldMainColor, GUILayout.Width(140));
        if (mainColor != oldMainColor)
        {
            oldMainColor = mainColor;
            SetProperty("_Color", mainColor);
        }
 
        Color specColor = EditorGUILayout.ColorField("Spec Color", oldSpecColor, GUILayout.Width(140));
        if (specColor != oldSpecColor)
        {
            oldSpecColor = specColor;
            SetProperty("_SpecColor", specColor);
        }
 
        float shininess = EditorGUILayout.Slider("Shininess", oldShininess, .01f, 1, GUILayout.Width(250));
        if (shininess != oldShininess)
        {
            oldShininess = shininess;
            SetProperty("_Shininess", shininess);
        }
 
        Texture2D lightmap = (Texture2D)EditorGUILayout.ObjectField("Lightmap", oldLightmap, typeof(Texture2D));
        if (lightmap != oldLightmap)
        {
            oldLightmap = lightmap;
            SetProperty("_LightMap", lightmap);
        }
    }
 
    static void SetProperty(string prop, object value)
    {
        foreach (Material m in Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets))
        {
            if (value is Shader)
            {
                m.shader = (Shader)value;
                continue;
            }
 
            if (!m.HasProperty(prop)) continue;
 
            if (value is float)
            {
                m.SetFloat(prop, (float)value);
                continue;
            }
            if (value is Color)
            {
                m.SetColor(prop, (Color)value);
                continue;
            }
            if (value is Texture)
            {
                m.SetTexture(prop, (Texture)value);
                continue;
            }
            throw new Exception("Unexpected type for " + prop + ": " + value.GetType());
        }
    }
}
}
