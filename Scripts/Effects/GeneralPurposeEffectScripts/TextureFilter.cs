// Original url: http://wiki.unity3d.com/index.php/TextureFilter
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/TextureFilter.cs
// File based on original modification date of: 29 September 2014, at 09:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author : Berenger 
Contents [hide] 
1 Description 
2 Usage 
3 Todo 
4 C# 

Description This static class contains a set of methods to modify a texture 2d. This isn't meant to replace the pro features of post process though. 
Available filters : 
- Grayscale
- Threshold
- Convolutions (edge detection, sharpen, linear blur, gaussian blur)
- Sepia
- Sobel filter to create a normal map
- Possibility to add more ...


Usage using UnityEngine;
public class TextFilterExample : MonoBehaviour 
{
    public Texture2D tex;
    public Renderer a, b, c, d, e;
 
    void Start () 
    {
        a.material.mainTexture = tex;
        b.material.mainTexture = TextureFilter.SobelFilter(tex);
        c.material.mainTexture = TextureFilter.Convolution(tex, TextureFilter.SHARPEN_KERNEL, 1);
        d.material.mainTexture = TextureFilter.Convolution(tex, TextureFilter.LINEAR_KERNEL, 10);
        e.material.mainTexture = TextureFilter.Threshold(tex, .5f);
    }
}If you wan to add more, create two function, a public one that takes at least a Texture2D parameter, and a private one that takes (int, int, Color[], ref Color[], ...), like this : 
    public static Texture2D NewFilter(Texture2D inTex, float myParameter)
    {
        return Filter(inTex, NewFilterFunc);
    }
 
    private static void NewFilterFunc(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters)
    {
        float myParameter = (float)parameters[0];
 
        // Here is the code of the filter
    }

Todo This code could be improved by adding the possibility to cumulate x filter without having to call tex.Apply() until the end, or even SetPixels, just by manipulating an array of Color. 


C# using UnityEngine;
using System;
 
/// <summary>
/// Simple implementation of image processing.
/// See this link to improve this code : https://code.google.com/p/aforge/
/// </summary>
public static class TextureFilter
{
    #region General
    private delegate void FilterMethod(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters);
 
    private static Texture2D Filter(Texture2D inTex, FilterMethod method, params object[] parameters)
    {
        float startTime = Time.realtimeSinceStartup;
 
        Color[] inPixels = null;
 
        try { inPixels = inTex.GetPixels(); }
        catch (UnityException e)
        {
            Debug.LogError("Error while reading the texture : " + e);
            return inTex;
        }
 
        Color[] outPixels = new Color[inPixels.Length];
 
        if (method != null)
            method(inTex.width, inTex.height, inPixels, ref outPixels, parameters);
 
        float endTime = Time.realtimeSinceStartup;
 
        Debug.Log(endTime - startTime);
 
        Texture2D outTex = new Texture2D(inTex.width, inTex.height);
        outTex.SetPixels(outPixels);
        outTex.Apply();
 
        return outTex;
    }
 
    private static int GetIndex(int x, int y, int width, int height)
    {
        int i = Mathf.Clamp(x, 0, width - 1) + Mathf.Clamp(y, 0, height - 1) * width;
        return i;
    }
    #endregion
 
    #region Grayscale
    public static Texture2D Grayscale(Texture2D inTex)
    {
        return Filter(inTex, GrayscaleFunc);
    }
 
    private static void GrayscaleFunc(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters)
    {
        for (int i = 0; i < inPixels.Length; i++)
        {
            float gray = inPixels[i].grayscale;
            outPixels[i] = new Color(gray, gray, gray, inPixels[i].a);
        }
    }
    #endregion
 
    #region Threshold
    public static Texture2D Threshold(Texture2D inTex, float threshold)
    {
        return Filter(inTex, ThresholdFunc, threshold);
    }
 
    private static void ThresholdFunc(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters)
    {
        float threshold = (float)parameters[0];
        for (int i = 0; i < inPixels.Length; i++)
        {
            float g = inPixels[i].grayscale;
            outPixels[i] = g > threshold ? Color.white : Color.black;
        }
    }
    #endregion
 
    #region Convolutions
    private static float Norme(float[][] kernel)
    {
        float norme = 0f;
        foreach (float[] line in kernel)
            foreach (float f in line)
                norme += f;
        return norme;
    }
 
    private static float[][] Normalize(float[][] kernel)
    {
        float sum = Norme(kernel);
        if (!Mathf.Approximately(sum, 1f))
        {
            for (int x = 0; x < kernel.Length; x++)
                for (int y = 0; y < kernel[x].Length; y++)
                    kernel[x][y] /= sum;
        }
        else
            Debug.LogWarning("Kernel is already normalized");
 
        return kernel;
    }
 
    public static float[][] Normalized(this float[][] kernel)
    {
        return Normalize(kernel);
    }
 
    #region Kernels
    /// <summary>
    /// Procedural linear kernel, filled with ones
    /// </summary>
    /// <param name="size">Width or length of the kernel</param>
    /// <param name="normalize">Weither the kernel must be normalized</param>
    /// <returns>Kernel size*size filled with ones</returns>
    public static float[][] LinearKernel(int size, bool normalize = true)
    {
        float[][] kernel = new float[size][];
        for (int x = 0; x < size; x++)
        {
            kernel[x] = new float[size];
            for (int y = 0; y < size; y++)
                kernel[x][y] = 1f;
        }
 
        if (normalize)
            kernel = kernel.Normalized();
 
        return kernel;
    }
 
    /// <summary>
    /// Procedural gaussian kernel. More accurate than the const ones below, but it takes time to create it.
    /// </summary>
    /// <param name="sigma">Variance, not the square</param>
    /// <param name="size">Width or length of the kernel</param>
    /// <param name="normalize">Weither the kernel must be normalized</param>
    /// <returns>Gaussian kernel</returns>
    public static float[][] GaussianKernel(float sigma, int size, bool normalize = true)
    {
        float[][] kernel = new float[size][];
        float mean = (float)size/2;
 
        float sigmaSqr2 = sigma * sigma * 2f;
        float sigmaSqr2piInv = 1f / Mathf.Sqrt((float)Math.PI * sigmaSqr2);
 
        int k = size / 2;
 
        for (int x = 0; x < size; x++)
        {
            kernel[x] = new float[size];
            int kx = x - k;
 
            for (int y = 0; y < size; y++)
            {
                int ky = y - k;
                float g = 0f;
 
                if (x > k && y < k)
                {
                    g = kernel[size - x - 1][y];
                }
                else if (x < k && y > k)
                {
                    g = kernel[x][size - y - 1];
                }
                else if (x > k && y > k)
                {
                    g = kernel[size - x - 1][size - y - 1];
                }
                else
                {
                    float exp = Mathf.Exp(-((kx * kx + ky * ky) / sigmaSqr2));
                    g = sigmaSqr2piInv * exp;
                }
 
                kernel[x][y] = g;
            }
        }
 
        if (normalize)
            kernel = kernel.Normalized();
 
        return kernel;
    }
 
    // Do not normalize
    public static readonly float[][] EDGEDETECT_KERNEL_1 = new float[3][]
    {
        new float[3]{ 1, 0, -1 },
        new float[3]{ 0, 0, 0 },
        new float[3]{ -1, 0, 1 }
    };
 
    // Do not normalize
    public static readonly float[][] EDGEDETECT_KERNEL_2 = new float[3][]
    {
        new float[3]{ 0, 1, 0 },
        new float[3]{ 1, -4, 1 },
        new float[3]{ 0, 1, 0 }
    };
 
    // Do not normalize
    // Vertical * horizontal
    public static readonly float[][] EDGEDETECT_KERNEL_3 = new float[3][]
    {
        new float[3]{ -1, -1, -1 },
        new float[3]{ -1,  8, -1 },
        new float[3]{ -1, -1, -1 }
    };
 
    // Do not normalize
    public static readonly float[][] EDGEDETECT_KERNEL_HORIZONTAL = new float[3][]
    {
        new float[3]{ -1, -1, -1 },
        new float[3]{ 2, 2, 2 },
        new float[3]{ -1, -1, -1 }
    };
 
    // Do not normalize
    public static readonly float[][] EDGEDETECT_KERNEL_VERTICAL = new float[3][]
    {
        new float[3]{ -1, 2, -1 },
        new float[3]{ -1, 2, -1 },
        new float[3]{ -1, 2, -1 }
    };
 
    // Do not normalize
    public static readonly float[][] SHARPEN_KERNEL = new float[3][]
    {
        new float[3]{ 0, -1, 0 },
        new float[3]{ -1, 5, -1 },
        new float[3]{ 0, -1, 0 }
    };
 
    // Do not normalize
    public static readonly float[][] LINEAR_KERNEL = new float[3][]
    {
        new float[3]{ 1f /9, 1f /9, 1f /9 },
        new float[3]{ 1f /9, 1f /9, 1f /9 },
        new float[3]{ 1f /9, 1f /9, 1f /9 }
    };
 
    // Do not normalize
    public static readonly float[][] GAUSSIAN_KERNEL_3 = new float[3][]
    {
        new float[3]{ 0.07511361f, 0.1238414f, 0.07511361f },
        new float[3]{ 0.1238414f, 0.20418f, 0.1238414f },
        new float[3]{ 0.07511361f, 0.1238414f, 0.07511361f }
    };
 
    // Do not normalize
    public static readonly float[][] GAUSSIAN_KERNEL_5 = new float[5][]
    {
        new float[5]{ 0.002969016f, 0.01330621f, 0.02193823f, 0.01330621f, 0.002969016f },
        new float[5]{ 0.01330621f, 0.05963429f, 0.09832032f, 0.05963429f, 0.01330621f },
        new float[5]{ 0.02193823f, 0.09832032f, 0.1621028f, 0.09832032f, 0.02193823f },
        new float[5]{ 0.01330621f, 0.05963429f, 0.09832032f, 0.05963429f, 0.01330621f },
        new float[5]{ 0.002969016f, 0.01330621f, 0.02193823f, 0.01330621f, 0.002969016f }
    };
 
    // Do not normalize
    public static readonly float[][] GAUSSIAN_KERNEL_7 = new float[7][]
    {
        new float[7]{ 1.965191E-05f, 0.0002394093f, 0.001072958f, 0.001769009f, 0.001072958f, 0.0002394093f, 1.965191E-05f },
        new float[7]{ 0.0002394093f, 0.002916602f, 0.01307131f, 0.02155094f, 0.01307131f, 0.002916602f, 0.0002394093f },
        new float[7]{ 0.001072958f, 0.01307131f, 0.05858152f, 0.0965846f, 0.05858152f, 0.01307131f, 0.001072958f },
        new float[7]{ 0.001769009f, 0.02155094f, 0.0965846f, 0.1592411f, 0.0965846f, 0.02155094f, 0.001769009f },
        new float[7]{ 0.001072958f, 0.01307131f, 0.05858152f, 0.0965846f, 0.05858152f, 0.01307131f, 0.001072958f },
        new float[7]{ 0.0002394093f, 0.002916602f, 0.01307131f, 0.02155094f, 0.01307131f, 0.002916602f, 0.0002394093f },
        new float[7]{ 1.965191E-05f, 0.0002394093f, 0.001072958f, 0.001769009f, 0.001072958f, 0.0002394093f, 1.965191E-05f },
    };
 
    // Do not normalize
    public static readonly float[][] GAUSSIAN_KERNEL_9 = new float[9][]
    {
        new float[9]{ 1.791064E-08f, 5.931188E-07f, 7.225666E-06f, 3.238319E-05f, 5.339085E-05f, 3.238319E-05f, 7.225666E-06f, 5.931188E-07f, 1.791064E-08f },
        new float[9]{ 5.931188E-07f, 1.96414E-05f, 0.0002392812f, 0.001072384f, 0.001768062f, 0.001072384f, 0.0002392812f, 1.96414E-05f, 5.931188E-07f },
        new float[9]{ 7.225666E-06f, 0.0002392812f, 0.002915042f, 0.01306431f, 0.02153941f, 0.01306431f, 0.002915042f, 0.0002392812f, 7.225666E-06f },
        new float[9]{ 3.238319E-05f, 0.001072384f, 0.01306431f, 0.05855018f, 0.09653293f, 0.05855018f, 0.01306431f, 0.001072384f, 3.238319E-05f },
        new float[9]{ 5.339085E-05f, 0.001768062f, 0.02153941f, 0.09653293f, 0.1591559f, 0.09653293f, 0.02153941f, 0.001768062f, 5.339085E-05f },
        new float[9]{ 3.238319E-05f, 0.001072384f, 0.01306431f, 0.05855018f, 0.09653293f, 0.05855018f, 0.01306431f, 0.001072384f, 3.238319E-05f },
        new float[9]{ 7.225666E-06f, 0.0002392812f, 0.002915042f, 0.01306431f, 0.02153941f, 0.01306431f, 0.002915042f, 0.0002392812f, 7.225666E-06f },
        new float[9]{ 5.931188E-07f, 1.96414E-05f, 0.0002392812f, 0.001072384f, 0.001768062f, 0.001072384f, 0.0002392812f, 1.96414E-05f, 5.931188E-07f },
        new float[9]{ 1.791064E-08f, 5.931188E-07f, 7.225666E-06f, 3.238319E-05f, 5.339085E-05f, 3.238319E-05f, 7.225666E-06f, 5.931188E-07f, 1.791064E-08f },
    };
 
    // Do not normalize
    public static readonly float[][] LAPLACIAN_GAUSSIAN_KERNEL = new float[5][]
    {
        new float[5]{ 0,  0,  -1, 0,  0 },
        new float[5]{ 0,  -1, -2, -1, 0 },
        new float[5]{ -1, -2, 16, -2, -1 },
        new float[5]{ 0,  -1, -2, -1, 0 },
        new float[5]{ 0,  0,  -1, 0,  0 }
    };
    #endregion
 
    public static Texture2D Convolution(Texture2D inTex, float[][] kernel, int iteration)
    {
        return Filter(inTex, ConvolutionFunc, kernel, iteration);
    }
 
    private static void ConvolutionFunc(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters)
    {
        float[][] kernel = (float[][])parameters[0];
        int kernelSize = kernel.Length;
        int iteration = (int)parameters[1];
 
        for (int ite = 0; ite < iteration; ite++)
        {
            for (int i = 0; i < inPixels.Length; i++)
            {
                int px = i % width;
                int py = i / width;
 
                Color c = new Color(0f, 0f, 0f, 0f);
                for (int y = 0; y < kernelSize; y++)
                {
                    int ky = y - kernelSize / 2;
                    for (int x = 0; x < kernelSize; x++)
                    {
                        int kx = x - kernelSize / 2;
                        c += inPixels[GetIndex(px + kx, py + ky, width, height)] * kernel[x][y];
                    }
                }
                outPixels[i] = c;
 
            }
 
            // Copy array for next iteration
            if (ite < iteration - 1)
            {
                for (int i = 0; i < inPixels.Length; i++)
                    inPixels[i] = outPixels[i];
            }
        }
    }
    #endregion
 
    #region Color correction
    #region Sepia
    public static Texture2D Sepia(Texture2D inTex)
    {
        return Filter(inTex, SepiaFunc);
    }
 
    private static void SepiaFunc(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters)
    {
        for (int i = 0; i < inPixels.Length; i++)
        {
            float y = Vector3.Dot( new Vector3(0.299f, 0.587f, 0.114f), new Vector3(inPixels[i].r, inPixels[i].g, inPixels[i].b));
 
	        // Convert to Sepia Tone by adding constant
            Vector3 sepiaConvert = new Vector3(0.191f, -0.054f, -0.221f);
            Vector3 output = sepiaConvert + new Vector3(y, y, y);
            outPixels[i] = new Color(output.x, output.y, output.z, inPixels[i].a);
        }
    }
    #endregion
 
    public static Texture2D ContrasteSaturationBrightness(Texture2D inTex, float contrast, float saturation, float brightness)
    {
        return Filter(inTex, ContrasteSaturationBrightnessFunc, saturation, brightness, contrast);
    }
 
    private static void ContrasteSaturationBrightnessFunc(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters)
    {
        float saturation = (float)parameters[0];
        float brightness = (float)parameters[1];
        float contrast = (float)parameters[2];
 
        for (int i = 0; i < inPixels.Length; i++)
        {
            Color color = inPixels[i];
 
            //RGB Color Channels
            float AvgLumR = .5f;
            float AvgLumG = .5f;
            float AvgLumB = .5f;
 
            //Luminace Coefficients for brightness of image
            Vector3 LuminaceCoeff = new Vector3(0.2125f, 0.7154f, 0.0721f);
 
            //Brigntess calculations
            Vector3 AvgLumin = new Vector3(AvgLumR, AvgLumG, AvgLumB);
            Vector3 brtColor = new Vector3(color.r, color.g, color.b) * brightness;
            float intensityf = Vector3.Dot(brtColor, LuminaceCoeff);
            Vector3 intensity = new Vector3(intensityf, intensityf, intensityf);
 
            //Saturation calculation
            Vector3 satColor = Vector3.Lerp(intensity, brtColor, saturation);
 
            //Contrast calculations
            Vector3 conColor = Vector3.Lerp(AvgLumin, satColor, contrast);
 
            outPixels[i] = new Color(conColor.x, conColor.y, conColor.z, color.a);
        }
    }
    #endregion
 
    #region SobelFilter
    public static Texture2D SobelFilter(Texture2D inTex)
    {
        return Filter(inTex, SobelFilterFunc);
    }
 
    // http://forum.unity3d.com/threads/5714-bumpmap-(grayscale)-to-normalmap-(rgb)
    private static void SobelFilterFunc(int width, int height, Color[] inPixels, ref Color[] outPixels, params object[] parameters)
    {
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                float xLeft = inPixels[GetIndex(x - 1, y, width, height)].grayscale;
                float xRight = inPixels[GetIndex(x + 1, y, width, height)].grayscale;
                float yUp = inPixels[GetIndex(x, y - 1, width, height)].grayscale;
                float yDown = inPixels[GetIndex(x, y + 1, width, height)].grayscale;
 
                float xDelta = ((xLeft - xRight) + 1) * .5f;
                float yDelta = ((yUp - yDown) + 1) * .5f;
 
                outPixels[GetIndex(x, y, width, height)] = new Color(xDelta, yDelta, 1f, 1f);
            }
        }
    }
    #endregion
}
}
