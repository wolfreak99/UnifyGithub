// Original url: http://wiki.unity3d.com/index.php/Shadow_Volumes_in_Alpha
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Shadow_Volumes_in_Alpha.cs
// File based on original modification date of: 6 February 2013, at 09:18. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Aras Pranckevicius 
Contents [hide] 
1 Description 
2 Usage 
3 The package 
4 Behind the scenes 
5 History 
6 JavaScript - RenderShadowVolume.js 
7 ShaderLab - ShadowVolumeExtrusion.shader 

Description 
Shadow Volume in actionThis package implements very basic shadows using shadow volume algorithm. Like all shadow volume based shadows, it's very fillrate hungry, and this script only implements very basic functionality, but on the plus side it should work on pretty much all graphics cards and performance might be enough for simple cases. Requires Unity Pro as it uses GL class. (note: as of Unity 4.0, it works with Unity Basic as well) 
Note that in Unity 2.0 and up there are easier to use built-in shadows based on shadow mapping algorithm. But this technique still works if you want shadow volumes. 
Usage 
Typical shadow volume setupThe provided unity package contains the example scene inside. In general, the usage is this: 
Use closed and shadow volume friendly meshes for shadow casters. 
Meshes should be completely closed and each edge should be between exactly two triangles. 
Meshes generally should not have hard shading creases, i.e. be smooth. Unless you implement the following point, meshes should be fairly highly tessellated as well. 
Meshes should be highly tesselated, or each hard edge should actually have two zero-area triangles that join both sides of the hard edge. Imagine that if you move one face away, no holes should appear - the invisible triangles should stretch out and fill the gap. 
Attach RenderShadowVolume.js to a Camera. Setup shadow casting Objects, shadow casting light, volume extrusion distance and the shader for extrusion (ShadowVolumeExtrusion) in Inspector. 
Using no ambient lighting is probably a good idea as well. 
Hit play and enjoy. 
This script uses GL class and therefore requires Unity Pro. (note: as of Unity 4.0, it works with Unity Basic as well) It should run on about any hardware. 
The packageZipped Unity package for Unity 2.x: Media:ShadowVolumeAlpha.zip 
Behind the scenesThe script implements shadow volumes without using the stencil buffer, as described in "Shadow Volumes Revisited" paper by Roettger, Irion and Ertl; 2002. 
Shadow is rendered by darkening the screen twice in shadowed regions. The paper describes alternative ways of applying the shadow, this is just one of them. 
Shadow volume extrusion is done in a vertex shader, by just moving away vertices that have normals facing away from the light source. This is not 100% correct way of doing the extrusion, but it often works fine for highly tesselated objects. Even with them, sometimes it introduces small 1 pixel shadow artifacts. The extrusion can be improved in two ways: 
Do extrusion on the CPU, extruding away faces (not vertices!) that face away from the light, and fill in shadow volume side faces. Or: 
Preprocess shadow casting meshes so that all vertices actually contain face normals, and between each non-planar triangle pair there are degenerate triangles included. Pretty much as described in "Shadow Volume Extrusion using a Vertex Shader" article on ShaderX by Brennan; 2002 (can be found on AMD developer site). 
As this script uses alpha channel of the screen itself, it does not play nicely with Glow and other effects that also use alpha channel of the screen. A minor change could be made to make it render volumes into alpha channel of a screen-sized render texture. 
History2008 Mar 11 Initial version 
JavaScript - RenderShadowVolume.jsvar objects : MeshFilter[];
var shadowLight : Light;
var extrusionDistance = 20.0;
var extrusionShader : Shader;
 
private var setAlphaMat : Material;
private var extrusionMat : Material;
 
function Start() {
    if (!shadowLight) {
        Debug.LogWarning ("no shadow casting light set, disabling script");
        enabled = false;
    }
    if (!extrusionShader) {
        Debug.LogWarning ("no shadow casting shader set, disabling script");
        enabled = false;
    }
    if (!camera) {
        Debug.LogWarning ("script must be attached to camera, disabling script");
        enabled = false;
    }
}
 
function OnPostRender() {
    if (!enabled)
        return;
 
    if (!setAlphaMat) {
        setAlphaMat = new Material ("Shader \"Hidden/ShadowVolumeAlpha\" {" +
            "SubShader { " +
            " ColorMask A " +
            " ZTest Always Cull Off ZWrite Off" +
            " Pass {" +
            "    Color (0.25,0.25,0.25,0.25)" +
            " } " +
            " Pass {" +
            "    Blend DstColor One" +
            "    Color (1,1,1,1)" +
            " } " +
            " Pass {" +
            "    Blend OneMinusDstColor Zero" +
            "    Color (1,1,1,1)" +
            " } " +
            " Pass {" +
            "    Blend One One" +
            "    Color (0.5,0.5,0.5,0.5)" +
            " } " +
            " Pass {" +
            "    ColorMask RGB" +
            "    Blend Zero DstAlpha" +
            "    Color (1,1,1,1)" +
            " } " +
            "}}"
        );
        setAlphaMat.shader.hideFlags = HideFlags.HideAndDontSave;
        setAlphaMat.hideFlags = HideFlags.HideAndDontSave;
    }
    if (!extrusionMat) {
        extrusionMat = new Material (extrusionShader);
        extrusionMat.hideFlags = HideFlags.HideAndDontSave;
    }
 
    // clear screen's alpha to 1/4
    GL.PushMatrix ();
    GL.LoadOrtho ();
    setAlphaMat.SetPass (0);
    DrawQuad();
    GL.PopMatrix ();
 
    // setup extrusion shader properties
    extrusionMat.SetFloat ("_Extrusion", extrusionDistance);
    var lightPos : Vector4;
    if (shadowLight.type == LightType.Directional) {
        var dir = -shadowLight.transform.forward;
        dir = transform.InverseTransformDirection(dir);
        lightPos = Vector4(dir.x,dir.y,-dir.z,0.0);
    } else {
        var pos = shadowLight.transform.position;
        pos = transform.InverseTransformPoint(pos);
        lightPos = Vector4(pos.x,pos.y,-pos.z,1.0);
    }
    extrusionMat.SetVector("_LightPosition", lightPos);
 
    // render shadow volumes of all objects
    for( var filter : MeshFilter in objects ) {
        var m : Mesh = filter.sharedMesh;
        var tr : Transform = filter.transform;
        extrusionMat.SetPass(0);
        Graphics.DrawMesh (m, tr.localToWorldMatrix);
        extrusionMat.SetPass(1);
        Graphics.DrawMesh (m, tr.localToWorldMatrix);
    }
 
    // normalize and apply shadow mask
    GL.PushMatrix ();
    GL.LoadOrtho ();
    setAlphaMat.SetPass (1);
    DrawQuad();
    setAlphaMat.SetPass (2);
    DrawQuad();
    setAlphaMat.SetPass (3);
    DrawQuad();
    setAlphaMat.SetPass (4);
    DrawQuad();
    GL.PopMatrix ();
}
 
private static function DrawQuad() : void {
    GL.Begin (GL.QUADS);
    GL.Vertex3 (0, 0, 0.1);
    GL.Vertex3 (1, 0, 0.1);
    GL.Vertex3 (1, 1, 0.1);
    GL.Vertex3 (0, 1, 0.1);
    GL.End ();
}ShaderLab - ShadowVolumeExtrusion.shaderInvalid language.
You need to specify a language like this: <source lang="html4strict">...</source>
Supported languages for syntax highlighting:
 [Expand] 4cs, 6502acme, 6502kickass, 6502tasm, 68000devpac, abap, actionscript, actionscript3, ada, algol68, apache, applescript, apt_sources, asm, asp, autoconf, autohotkey, autoit, avisynth, awk, bascomavr, bash, basic4gl, bf, bibtex, blitzbasic, bnf, boo, c, c_loadrunner, c_mac, caddcl, cadlisp, cfdg, cfm, chaiscript, cil, clojure, cmake, cobol, coffeescript, cpp, cpp-qt, csharp, css, cuesheet, d, dcs, delphi, diff, div, dos, dot, e, ecmascript, eiffel, email, epc, erlang, euphoria, f1, falcon, fo, fortran, freebasic, fsharp, gambas, gdb, genero, genie, gettext, glsl, gml, gnuplot, go, groovy, gwbasic, haskell, hicest, hq9plus, html4strict, html5, icon, idl, ini, inno, intercal, io, j, java, java5, javascript, jquery, kixtart, klonec, klonecpp, latex, lb, lisp, llvm, locobasic, logtalk, lolcode, lotusformulas, lotusscript, lscript, lsl2, lua, m68k, magiksf, make, mapbasic, matlab, mirc, mmix, modula2, modula3, mpasm, mxml, mysql, newlisp, nsis, oberon2, objc, objeck, ocaml, ocaml-brief, oobas, oracle11, oracle8, oxygene, oz, pascal, pcre, per, perl, perl6, pf, php, php-brief, pic16, pike, pixelbender, pli, plsql, postgresql, povray, powerbuilder, powershell, proftpd, progress, prolog, properties, providex, purebasic, pycon, python, q, qbasic, rails, rebol, reg, robots, rpmspec, rsplus, ruby, sas, scala, scheme, scilab, sdlbasic, smalltalk, smarty, sql, systemverilog, tcl, teraterm, text, thinbasic, tsql, typoscript, unicon, uscript, vala, vb, vbnet, verilog, vhdl, vim, visualfoxpro, visualprolog, whitespace, whois, winbatch, xbasic, xml, xorg_conf, xpp, yaml, z80, zxbasic


Shader "Hidden/ShadowVolume/Extrusion" {
Properties {
    _Extrusion ("Extrusion", Range(0,30)) = 5.0
}

CGINCLUDE
#pragma vertex vert
#include "UnityCG.cginc"

struct appdata {
    float4 vertex;
    float3 normal;
};

float _Extrusion;

// camera space light position
// xyz = position, w = 1 for point/spot lights
// xyz = direction, w = 0 for directional lights
float4 _LightPosition;

float4 vert( appdata v ) : POSITION {
    
    // point to light vector
    float4 objLightPos = mul( _LightPosition, glstate.matrix.invtrans.modelview[0] );
    float3 toLight = normalize(objLightPos.xyz - v.vertex.xyz * objLightPos.w);
    
    // dot with normal
    float backFactor = dot( toLight, v.normal );
    
    float extrude = (backFactor < 0.0) ? 1.0 : 0.0;
    v.vertex.xyz -= toLight * (extrude * _Extrusion);
    
    return mul( glstate.matrix.mvp, v.vertex );
}
ENDCG


SubShader {
    Tags { "Queue" = "Transparent+10" }
    ZWrite Off
    ColorMask A
    Offset 1,1
    
    // Draw front faces, doubling the value in alpha channel
    Pass {
        Cull Back
        Blend DstColor One
        
        CGPROGRAM
        ENDCG
    
        SetTexture[_MainTex] { constantColor(1,1,1,1) combine constant }        
    }
    
    // Draw back faces, halving the value in alpha channel
    Pass {
        Cull Front
        Blend DstColor Zero
        
        CGPROGRAM
        ENDCG
    
        SetTexture[_MainTex] { constantColor(0.5,0.5,0.5,0.5) combine constant }
        
    }
} 

FallBack Off
}
}
