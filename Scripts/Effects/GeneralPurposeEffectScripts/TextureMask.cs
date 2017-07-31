// Original url: http://wiki.unity3d.com/index.php/TextureMask
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/TextureMask.cs
// File based on original modification date of: 20 January 2013, at 00:05. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Created by Ablaze 

This simple shader will mask off areas of a texture. This is useful for, say, when you really want to make your minimap round. I've designed it with GUI elements in mind. To make a round object you would: 
put a black border around a white circle and import that into unity. 
Change the texture format to Alpha 8 bit and check "Build Alpha from Grayscale" 
Create a shader with the code below. 
Create a new plane and import your texture on to it. 
Select the shader you created. 
Drop your culling mask in to it. 
That easy! 
Invalid language.
You need to specify a language like this: <source lang="html4strict">...</source>
Supported languages for syntax highlighting:
 [Expand] 4cs, 6502acme, 6502kickass, 6502tasm, 68000devpac, abap, actionscript, actionscript3, ada, algol68, apache, applescript, apt_sources, asm, asp, autoconf, autohotkey, autoit, avisynth, awk, bascomavr, bash, basic4gl, bf, bibtex, blitzbasic, bnf, boo, c, c_loadrunner, c_mac, caddcl, cadlisp, cfdg, cfm, chaiscript, cil, clojure, cmake, cobol, coffeescript, cpp, cpp-qt, csharp, css, cuesheet, d, dcs, delphi, diff, div, dos, dot, e, ecmascript, eiffel, email, epc, erlang, euphoria, f1, falcon, fo, fortran, freebasic, fsharp, gambas, gdb, genero, genie, gettext, glsl, gml, gnuplot, go, groovy, gwbasic, haskell, hicest, hq9plus, html4strict, html5, icon, idl, ini, inno, intercal, io, j, java, java5, javascript, jquery, kixtart, klonec, klonecpp, latex, lb, lisp, llvm, locobasic, logtalk, lolcode, lotusformulas, lotusscript, lscript, lsl2, lua, m68k, magiksf, make, mapbasic, matlab, mirc, mmix, modula2, modula3, mpasm, mxml, mysql, newlisp, nsis, oberon2, objc, objeck, ocaml, ocaml-brief, oobas, oracle11, oracle8, oxygene, oz, pascal, pcre, per, perl, perl6, pf, php, php-brief, pic16, pike, pixelbender, pli, plsql, postgresql, povray, powerbuilder, powershell, proftpd, progress, prolog, properties, providex, purebasic, pycon, python, q, qbasic, rails, rebol, reg, robots, rpmspec, rsplus, ruby, sas, scala, scheme, scilab, sdlbasic, smalltalk, smarty, sql, systemverilog, tcl, teraterm, text, thinbasic, tsql, typoscript, unicon, uscript, vala, vb, vbnet, verilog, vhdl, vim, visualfoxpro, visualprolog, whitespace, whois, winbatch, xbasic, xml, xorg_conf, xpp, yaml, z80, zxbasic


Shader "MaskedTexture"
{
   Properties
   {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Mask ("Culling Mask", 2D) = "white" {}
      _Cutoff ("Alpha cutoff", Range (0,1)) = 0.1
   }
   SubShader
   {
      Tags {"Queue"="Transparent"}
      Lighting Off
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      AlphaTest GEqual [_Cutoff]
      Pass
      {
         SetTexture [_Mask] {combine texture}
         SetTexture [_MainTex] {combine texture, previous}
      }
   }
}
}
