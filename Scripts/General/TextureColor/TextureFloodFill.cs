// Original url: http://wiki.unity3d.com/index.php/TextureFloodFill
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/TextureColor/TextureFloodFill.cs
// File based on original modification date of: 28 October 2014, at 13:24. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.TextureColor
{
DescriptionTwo extension methods for Texture2D which allows you to perform a floodfill operation on the texture. 
FloodFillArea will fill the area around the reference point as long as the color stays the same. The reference coler is taken from the start-point-pixel. 
FloodFillBorder will fill the area around the reference point until it reaches the given border color. 
Both functions only perform a 4-direction method (north,east,south,west). This equals the behaviour of usual filling methods like in MS Paint. 
Usage Place this script somewhere in your project or in "Assets/Standard Assets/" if you want to use the functions from a foreign language (not C#). 
To use it you can simply call the desired function like this from any language: 
    texture.FloodFillArea(10,10,Color.red);
    // When you're done call Apply to commit your changes.
    texture.Apply();where "texture" is a Texture2D variable with a texture that is "writable". 
TextureExtension.cs     using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
 
    public static class TextureExtension
    {
        public struct Point
        {
            public short x;
            public short y;
            public Point(short aX, short aY) { x = aX; y = aY; }
            public Point(int aX, int aY) : this((short)aX, (short)aY) { }
        }
 
        public static void FloodFillArea(this Texture2D aTex, int aX, int aY, Color aFillColor)
        {
            int w = aTex.width;
            int h = aTex.height;
            Color[] colors = aTex.GetPixels();
            Color refCol = colors[aX + aY * w];
            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
                for (int i = current.x; i < w; i++)
                {
                    Color C = colors[i + current.y * w];
                    if (C != refCol || C == aFillColor)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    Color C = colors[i + current.y * w];
                    if (C != refCol || C == aFillColor)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }
            aTex.SetPixels(colors);
        }
 
        public static void FloodFillBorder(this Texture2D aTex, int aX, int aY, Color aFillColor, Color aBorderColor)
        {
            int w = aTex.width;
            int h = aTex.height;
            Color[] colors = aTex.GetPixels();
            byte[] checkedPixels = new byte[colors.Length];
            Color refCol = aBorderColor;
            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
 
                for (int i = current.x; i < w; i++)
                {
                    if (checkedPixels[i + current.y * w] > 0 || colors[i + current.y * w] == refCol)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    checkedPixels[i + current.y * w] = 1;
                    if (current.y + 1 < h)
                    {
                        if (checkedPixels[i + current.y * w + w] == 0 && colors[i + current.y * w + w] != refCol)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        if (checkedPixels[i + current.y * w - w] == 0 && colors[i + current.y * w - w] != refCol)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    if (checkedPixels[i + current.y * w] > 0 || colors[i + current.y * w] == refCol)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    checkedPixels[i + current.y * w] = 1;
                    if (current.y + 1 < h)
                    {
                        if (checkedPixels[i + current.y * w + w] == 0 && colors[i + current.y * w + w] != refCol)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        if (checkedPixels[i + current.y * w - w] == 0 && colors[i + current.y * w - w] != refCol)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }
            aTex.SetPixels(colors);
        }
    }Author: Bunny83 
}
