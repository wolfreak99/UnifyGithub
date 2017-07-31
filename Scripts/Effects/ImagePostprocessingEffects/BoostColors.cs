// Original url: http://wiki.unity3d.com/index.php/BoostColors
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/ImagePostprocessingEffects/BoostColors.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.ImagePostprocessingEffects
{
Author: Nicholas Francis 
DescriptionThis simple image effect subtracts a certain amount of color from your rendering, then doubles the strength. 
 
Example of the BoostColors image effect in action.UsageThis script (and all image effects) use render textures by need and therefore require Unity Pro. There isn't a way around this! 
Simply attach this script to a Camera and watch your colors go off the radar. 
C# - BoostColors.csusing UnityEngine;
using System.Collections;
 
[AddComponentMenu("Image Effects/Boost Colors")]
public class BoostColors : MonoBehaviour {  
    static Material s_BoostMaterial = null;
    public float blackLevel = .15f;
 
    void OnRenderImage (RenderTexture source, RenderTexture dest) {
        if (!s_BoostMaterial) {
            s_BoostMaterial = new Material (
                "Shader \"\" {" +
                "   Properties {" +
                "       _Color (\"Color\", Color) = (.3,.3,.3,.3)" +
                "       _RTex (\"RenderTex\", RECT) = \"\" {}" +
                "   }" +
                "   SubShader {" +
                "       Cull Off ZWrite Off ZTest Always" +
                "       Pass {" +
                "           SetTexture [_RTex] { constantColor [_Color] combine texture - constant DOUBLE }" +
                "       }" +
                "   }" +
                "}"
            );
        }
        RenderTexture.active = dest;
        s_BoostMaterial.SetTexture ("_RTex", source);
        s_BoostMaterial.SetColor ("_Color", new Color (blackLevel,blackLevel,blackLevel,blackLevel));
        s_BoostMaterial.SetPass (0);
 
        GL.PushMatrix ();
        GL.LoadOrtho ();
 
        GL.Begin (GL.QUADS);
        GL.TexCoord2 (0,0); GL.Vertex3 (0,0,0.1f);
        GL.TexCoord2 (1,0); GL.Vertex3 (1,0,0.1f);
        GL.TexCoord2 (1,1); GL.Vertex3 (1,1,0.1f);
        GL.TexCoord2 (0,1); GL.Vertex3 (0,1,0.1f);
        GL.End ();
        GL.PopMatrix ();
    }
}
}
