// Original url: http://wiki.unity3d.com/index.php/Scrolling_UVs
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Scrolling_UVs.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Overview A C# script that smoothly scrolls a material's UVs in an arbitrary direction/speed given by "Uv Animation Rate". Supports changing which material index and texture name, but the defaults will work with single material, single texture renderers. 
C# using UnityEngine;
using System.Collections;
 
public class ScrollingUVs : MonoBehaviour 
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
    public string textureName = "_MainTex";
 
    Vector2 uvOffset = Vector2.zero;
 
    void LateUpdate() 
    {
        uvOffset += ( uvAnimationRate * Time.deltaTime );
        if( renderer.enabled )
        {
            renderer.materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
        }
    }
}
}
