/*************************
 * Original url: http://wiki.unity3d.com/index.php/BloomEffect
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/ImagePostprocessingEffects/BloomEffect.cs
 * File based on original modification date of: 10 January 2012, at 20:44. 
 *
 * Author: Jonathan Czeck (aarku) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.ImagePostprocessingEffects
{
    DescriptionThis script is made obsolete by the GlowEffect shipped with Unity 1.5. This script makes areas of white alpha glow. The amount of effect and the blurryness of the glow can be controlled by the two public variables of the script. 
     
    A test of OverWhelmed Arena by Graveck Interactive showing the bloom effect. The effect had not been tweaked and as a result there are serious graphical problems, but the bloom effect is sound.UsageThis script (and all image effects) use render textures by need and therefore require Unity Pro. There isn't a way around this! 
    Set your scene view to show the alpha channel of your scene. This is a menu in your scene view. Then, set up your scene so that areas you want to glow are white and areas that you do not want to glow are black, and anywhere inbetween. For most shaders, you can alter this by either including an alpha channel in your texture, or by altering the material's main color's alpha. Some shaders will even put the specular highlights (gloss) in the alpha, so that glints of light will glow. This should work with most if not all of the regular default shaders, but that does not seem to be the case as of Unity 1.2.2. Make sure to also set the alpha of the tint color in your skybox to an appropriate value. This means that you can even change the amount of skybox glow at runtime just like "Shadow Of The Colossus"! 
    When you are done setting up your scene, attach this script to your main camera. Adjust the number of blurs to match the performance you want and the effect you want. The more iterations the more computation it takes. It is usually fill rate limited. 
    C# - BloomEffect.csusing UnityEngine;
    using System.Collections;
     
    // This class implements a bloom effect.
    // It is based off of OTEE's BlurEffect.cs
    // It uses texture combiners, so it should run on older gfx. cards too.
    // It is based on a four tap cone kernel.
    [AddComponentMenu("Image Effects/Bloom")]
    public class BloomEffect : MonoBehaviour {
     
        /// 0 to 1... the amount of bloom effect
        public float bloomAmount = 1F;
     
        /// How many levels of blurryness we want
        public int blurLevels = 2;
     
        static Material m_BloomMaterial = null;
        protected static Material bloomMaterial {
            get {
                if (m_BloomMaterial == null) 
                    m_BloomMaterial = new Material (
                        "Shader \"Bloom Combine Shader\" {" +
                        "   Properties { _Color (\"Bloom Amount\", Color) = (1,1,1,1) }" +
                        "   SubShader { Pass {" +
                        "       ZTest Always Cull Off ZWrite Off" +
                        "       Blend SrcAlpha One" +
                        "       SetTexture [__RenderTex] {" +
                        "           constantColor [_Color]" +
                        "           combine constant * texture QUAD" +
                        "}}}}"
                );
                return m_BloomMaterial;
            } 
        }
     
        /// The blur shader material.
        static Material m_BlurMaterial = null;  
        protected static Material blurMaterial {
            get {
                if (m_BlurMaterial == null) 
                    m_BlurMaterial = new Material (
                    "Shader \"ConeTab\" {\n"    +
                    "   SubShader { Pass {\n" +
                    "       ZTest Always Cull Off ZWrite Off\n" +
                    "       SetTexture [__RenderTex] { combine texture * constant alpha " +
                    "           constantColor (0,0,0,0.25) } \n"    +
                    "       SetTexture [__RenderTex] { combine texture * constant + previous " +
                    "           constantColor (0,0,0,0.25) }\n" +
                    "       SetTexture [__RenderTex] { combine texture * constant + previous " +
                    "           constantColor (0,0,0,0.25) }\n" +
                    "       SetTexture [__RenderTex] { combine texture * constant + previous " +
                    "           constantColor (0,0,0,0.25) }\n" +
                    "   }}\n"    +
                    "}"
                );
                return m_BlurMaterial;
            } 
        }
     
        protected void Start()
        {
            // Disable the image effect if the shader can't
            // run on the users graphics card
            if (!blurMaterial.shader.isSupported)
                enabled = false;
        }
     
        // Function that actually renders the screenspace passes, offsetting the texture coordinates
        // based one the size of the screen.
        public static void FourTapCone (RenderTexture source, Rect sourceRect, RenderTexture dest, Rect destRect, int iteration) {
     
            RenderTexture.active = dest;
            source.SetGlobalShaderProperty ("__RenderTex");
     
            float offsetX = (.5F+iteration) / (float)source.width;
            float offsetY = (.5F+iteration) / (float)source.height;
     
            GL.PushMatrix ();
            GL.LoadOrtho ();    
     
            for (int i = 0; i < blurMaterial.passCount; i++) {
                blurMaterial.SetPass (i);
     
                GL.Begin (GL.QUADS);
                GL.MultiTexCoord2 (0, sourceRect.xMin - offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMin + offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMin + offsetX,    sourceRect.yMin + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMin - offsetX,    sourceRect.yMin + offsetY);
                GL.Vertex3 (destRect.xMin,destRect.yMin, .1f);
     
                GL.MultiTexCoord2 (0, sourceRect.xMax - offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMax + offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMax + offsetX,    sourceRect.yMin + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMax - offsetX,    sourceRect.yMin + offsetY);
                GL.Vertex3 (destRect.xMax,destRect.yMin, .1f);
     
                GL.MultiTexCoord2 (0, sourceRect.xMax - offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMax + offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMax + offsetX,    sourceRect.yMax + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMax - offsetX,    sourceRect.yMax + offsetY);
                GL.Vertex3 (destRect.xMax,destRect.yMax,.1f);
     
                GL.MultiTexCoord2 (0, sourceRect.xMin - offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMin + offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMin + offsetX,    sourceRect.yMax + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMin - offsetX,    sourceRect.yMax + offsetY);
                GL.Vertex3 (destRect.xMin,destRect.yMax,.1f);
                GL.End();
     
            }
            GL.PopMatrix ();
     
        }
     
        // Downsamples the texture to a quarter resolution.
        static void DownSample4x (RenderTexture source, Rect sourceRect, RenderTexture dest, Rect destRect) {
     
            //if (dest.width * (destRect.xMax - destRect.xMin) *4 != source.width * (sourceRect.xMax - sourceRect.xMin))
            //  Debug.Log ("Warning: DownSample4x called with non-matching rectangles");
            RenderTexture.active = dest;
            source.SetGlobalShaderProperty ("__RenderTex");
     
            float offsetX = 1F / (float)source.width;
            float offsetY = 1F / (float)source.height;
     
            GL.PushMatrix ();
            GL.LoadOrtho ();        
            for (int i = 0; i < blurMaterial.passCount; i++) {
                blurMaterial.SetPass (i);
     
                GL.Begin (GL.QUADS);
                GL.MultiTexCoord2 (0, sourceRect.xMin - offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMin + offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMin + offsetX,    sourceRect.yMin + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMin - offsetX,    sourceRect.yMin + offsetY);
                GL.Vertex3 (destRect.xMin,destRect.yMin, .1f);
     
                GL.MultiTexCoord2 (0, sourceRect.xMax - offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMax + offsetX,    sourceRect.yMin - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMax + offsetX,    sourceRect.yMin + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMax - offsetX,    sourceRect.yMin + offsetY);
                GL.Vertex3 (destRect.xMax,destRect.yMin, .1f);
     
                GL.MultiTexCoord2 (0, sourceRect.xMax - offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMax + offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMax + offsetX,    sourceRect.yMax + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMax - offsetX,    sourceRect.yMax + offsetY);
                GL.Vertex3 (destRect.xMax,destRect.yMax,.1f);
     
                GL.MultiTexCoord2 (0, sourceRect.xMin - offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (1, sourceRect.xMin + offsetX,    sourceRect.yMax - offsetY);
                GL.MultiTexCoord2 (2, sourceRect.xMin + offsetX,    sourceRect.yMax + offsetY); 
                GL.MultiTexCoord2 (3, sourceRect.xMin - offsetX,    sourceRect.yMax + offsetY);
                GL.Vertex3 (destRect.xMin,destRect.yMax,.1f);
                GL.End();
            }
            GL.PopMatrix ();
        }
     
        // Update is called once per frame
        void OnRenderImage (RenderTexture source, RenderTexture destination) {      
            RenderTexture buffer = RenderTexture.GetTemporary(source.width/4, source.height/4, 0);
            RenderTexture buffer2 = RenderTexture.GetTemporary(source.width/4, source.height/4, 0);
     
            // Blur the source image. Since blurring is an expensive process, we want to do it at
            // quater resolution.
            // Copy the main screen image into an image at half size
            DownSample4x (source, new Rect (0,0,1,1), buffer, new Rect (0,0,1,1));  
     
            // Blur the image by running the FourTapCone function on it i = blurLevels numbers of time.
            bool oddEven = true;
            for(int i = 0; i < blurLevels; i++) {
                if(oddEven) {
                    FourTapCone (buffer, new Rect (0,0,1,1), buffer2, new Rect (0,0,1,1), i);
                    oddEven = false;
                } else {
                    FourTapCone (buffer2, new Rect (0,0,1,1), buffer, new Rect (0,0,1,1), i);
                    oddEven = true;
                }
            }
            ImageEffects.Blit(source,destination);
     
            //bloomAmount = Mathf.Clamp01(bloomAmount);
            bloomMaterial.color = new Color(1F, 1F, 1F, bloomAmount);
     
            if(oddEven)
                BlitBloom(buffer, destination);
            else
                BlitBloom(buffer2, destination);
     
            RenderTexture.ReleaseTemporary(buffer);
            RenderTexture.ReleaseTemporary(buffer2);
        }
     
        public void BlitBloom (RenderTexture source, RenderTexture dest)
        {
            // Make the destination texture the target for all rendering
            RenderTexture.active = dest;        
            // Assign the source texture to a property from a shader
            source.SetGlobalShaderProperty ("__RenderTex"); 
            // Set up the simple Matrix
            GL.PushMatrix ();
            GL.LoadOrtho ();
            bloomMaterial.SetPass (0);
            ImageEffects.DrawGrid(1,1);
            GL.PopMatrix ();
        }
     
}
}
