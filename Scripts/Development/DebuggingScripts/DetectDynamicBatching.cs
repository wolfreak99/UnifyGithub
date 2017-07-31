// Original url: http://wiki.unity3d.com/index.php/DetectDynamicBatching
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/DebuggingScripts/DetectDynamicBatching.cs
// File based on original modification date of: 9 October 2014, at 17:10. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{
Author: Karel Crombecq (Sileni Studios) 
Contents [hide] 
1 Description 
2 Usage 
3 C# - DynamicBatchingDetector.cs 
4 CG - DetectDynamicBatchingShader.shader 

Description Adding this script to a GameObject in your world will overrule all shaders in your scene, and will instead render all objects that are not dynamically batched color-inverted. This is very useful to find issues with your batching, and can greatly help you optimize your rendering. 
This check works because apparently, objects that are dynamically batched have their _Object2World transformation matrix, which is only available in the shader, set to the identity matrix, even though Transform.localToWorldMatrix still points to the original, proper matrix. This is probably because the dynamic batching happens at a layer below the script level, where all meshes are combined in one single mesh positioned in the origin of the scene. This is not a 100% accurate check, as objects positioned in the origin initially will falsely be flagged, but it's the best and only method of detecting dynamic batching on a per-object basis that I know of. 
Usage First: make sure the shader is in a Resources folder in your Assets library - otherwise the script won't be able to find it! Drop the DynamicBatchingDetector.cs script on any object in your scene. This script will simply overrule your shaders with the dynamic batching detection shader. 
C# - DynamicBatchingDetector.cs using UnityEngine;
using System.Collections;
 
public class DynamicBatchingDetector : MonoBehaviour {
 
	public Camera camera; // inspector
 
 
	public void Start() {
		if (camera == null) camera = Camera.main;
		camera.SetReplacementShader(Shader.Find("Transparent/DetectDynamicBatchingShader"), "");
	}
}

CG - DetectDynamicBatchingShader.shader Shader "Transparent/DetectDynamicBatchingShader" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
 
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	Cull Off
 
CGPROGRAM
#pragma surface surf Lambert alpha
#pragma target 3.0
 
sampler2D _MainTex;
fixed4 _Color;
 
struct Input {
	float2 uv_MainTex;
};
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	float4x4 m = _Object2World;
 
	if (!(
			m[0][0] == 1 && m[0][1] == 0 && m[0][2] == 0 && m[0][3] == 0
		&&	m[1][0] == 0 && m[1][1] == 1 && m[1][2] == 0 && m[1][3] == 0
		&&	m[2][0] == 0 && m[2][1] == 0 && m[2][2] == 1 && m[2][3] == 0
		&&	m[3][0] == 0 && m[3][1] == 0 && m[3][2] == 0 && m[3][3] == 1)) {
 
		o.Albedo.r = 1 - o.Albedo.r;
		o.Albedo.g = 1 - o.Albedo.g;
		o.Albedo.b = 1 - o.Albedo.b;
 
	}
	o.Alpha = c.a;
}
ENDCG
}
 
Fallback "Transparent/VertexLit"
}
}
