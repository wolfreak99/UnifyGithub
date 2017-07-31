// Original url: http://wiki.unity3d.com/index.php/TextureScale
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/TextureColor/TextureScale.cs
// File based on original modification date of: 14 October 2016, at 21:02. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.TextureColor
{
Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 TextureScale.js 
4 TextureScale.cs 

DescriptionIf you've used Texture2D.Resize, you may have been disappointed to find that it only resizes the texture; it doesn't scale the contents. Using TextureScale, however, you can in fact do just that, using either bilinear or point (nearest neighbor) filtering. It also uses multi-threading to speed up the process on CPUs with more than one core. (Although SetPixels() and Apply() cause a pretty big hit, so the multi-threading is perhaps limited in usefulness, but it can still be nice for large textures, and in any case it's faster than not having it.) 
Usage Name this script TextureScale and have it somewhere in your project, preferably in a folder such as Standard Assets that compiles before other folders, so you can call it from C# or Boo. You can then call TextureScale.Bilinear to use bilinear scaling, and TextureScale.Point to use point scaling. The arguments are: 
function Bilinear (texture : Texture2D, newWidth : int, newHeight : int) : void 
texture is the texture that you want to scale using bilinear filtering. newWidth and newHeight are the desired dimensions for the texture. It will be resized using Texture2D.Resize, and the contents scaled. Texture2D.Apply is called when done so that the changes take effect immediately. The texture must be RGBA32, RGB24, or Alpha8 in order for the function to work, and also must be read/write enabled. 
function Point (texture : Texture2D, newWidth : int, newHeight : int) : void 
The same as above, except point filtering is used instead of bilinear. 
Note that the functions resize the texture that's passed in; they don't return a new texture. An example of usage: 
var tex : Texture2D;
 
function Start () {
	var newTex = Instantiate (tex);
	renderer.material.mainTexture = newTex;
	TextureScale.Bilinear (newTex, tex.width*2, tex.height*2);
}Note that you probably don't want to scale the actual textures in your project. Instantiate a copy first, as shown in the code above. 
TextureScale.js // Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable
 
import System.Threading;
 
class ThreadData {
	var start : int;
	var end : int;
	var i : int;
 
	function ThreadData (start : int, end : int, i : int) {
		this.start = start;
		this.end = end;
		this.i = i;
	}
}
 
private static var texColors : Color32[];
private static var newColors : Color32[];
private static var w : int;
private static var ratioX : float;
private static var ratioY : float;
private static var w2 : int;
private static var finishCounts : int[];
 
static function Point (tex : Texture2D, newWidth : int, newHeight : int) {
	ThreadedScale (tex, newWidth, newHeight, false);
}
 
static function Bilinear (tex : Texture2D, newWidth : int, newHeight : int) {
	ThreadedScale (tex, newWidth, newHeight, true);
}
 
private static function ThreadedScale (tex : Texture2D, newWidth : int, newHeight : int, useBilinear : boolean) {
	texColors = tex.GetPixels32();
	newColors = new Color32[newWidth * newHeight];
	if (useBilinear) {
		ratioX = 1.0 / (parseFloat(newWidth) / (tex.width-1));
		ratioY = 1.0 / (parseFloat(newHeight) / (tex.height-1));
	}
	else {
		ratioX = parseFloat(tex.width) / newWidth;
		ratioY = parseFloat(tex.height) / newHeight;
	}
	w = tex.width;
	w2 = newWidth;
	var cores = Mathf.Min (SystemInfo.processorCount, newHeight);
	if (finishCounts == null || finishCounts.Length != cores) {
		finishCounts = new int[cores];
	}
	else {
		for (var count in finishCounts) count = 0;
	}
 
	if (cores > 1) {
		var slice = newHeight/cores;
		for (var i = 0; i < cores-1; i++) {
			var threadData = new ThreadData(slice*i, slice*(i+1), i);
			var thread = useBilinear? new Thread(BilinearScale) : new Thread(PointScale);
			thread.Start (threadData);
		}
		threadData = new ThreadData(slice*i, newHeight, i);
		if (useBilinear) {
			BilinearScale (threadData);
		}
		else {
			PointScale (threadData);
		}
 
		var totalCount : int;
		while (totalCount < cores) {
			totalCount = 0;
			for (i = 0; i < cores; i++) {
				totalCount += finishCounts[i];
			}
		}
	}
	else {
		threadData = new ThreadData(0, newHeight, 0);
		if (useBilinear) {
			BilinearScale (threadData);
		}
		else {
			PointScale (threadData);
		}
	}
 
	tex.Resize (newWidth, newHeight);
	tex.SetPixels32 (newColors);
	tex.Apply();
 
	texColors = null;
	newColors = null;
}
 
private static function BilinearScale (threadData : ThreadData) {
	for (var y = threadData.start; y < threadData.end; y++) {
		var yFloor = Mathf.Floor (y * ratioY);
		var y1 = yFloor * w;
		var y2 = (yFloor+1) * w;
		var yw = y * w2;
 
		for (var x = 0; x < w2; x++) {
			var xFloor = Mathf.Floor (x * ratioX);
			var xLerp = x * ratioX-xFloor;
			newColors[yw + x] = ColorLerpUnclamped (ColorLerpUnclamped (texColors[y1 + xFloor], texColors[y1 + xFloor+1], xLerp),
												    ColorLerpUnclamped (texColors[y2 + xFloor], texColors[y2 + xFloor+1], xLerp),
												    y*ratioY-yFloor);
		}
	}
 
	finishCounts[threadData.i]++;
}
 
private static function PointScale (threadData : ThreadData) {
	for (var y = threadData.start; y < threadData.end; y++) {
		var thisY = parseInt(ratioY * y) * w;
		var yw = y * w2;
		for (var x = 0; x < w2; x++) {
			newColors[yw + x] = texColors[thisY + ratioX*x];
		}
	}
 
	finishCounts[threadData.i]++;
}
 
private static function ColorLerpUnclamped (c1 : Color32, c2 : Color32, value : float) : Color32 {
	return new Color32 (c1.r + (c2.r - c1.r)*value, 
					    c1.g + (c2.g - c1.g)*value, 
					    c1.b + (c2.b - c1.b)*value, 
					    c1.a + (c2.a - c1.a)*value);
}TextureScale.cs // Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable
 
using System.Threading;
using UnityEngine;
 
public class TextureScale
{
	public class ThreadData
	{
	    public int start;
	    public int end;
		public ThreadData (int s, int e) {
			start = s;
			end = e;
		}
	}
 
    private static Color[] texColors;
    private static Color[] newColors;
    private static int w;
    private static float ratioX;
    private static float ratioY;
    private static int w2;
    private static int finishCount;
    private static Mutex mutex;
 
	public static void Point (Texture2D tex, int newWidth, int newHeight)
    {
		ThreadedScale (tex, newWidth, newHeight, false);
	}
 
	public static void Bilinear (Texture2D tex, int newWidth, int newHeight)
    {
		ThreadedScale (tex, newWidth, newHeight, true);
	}
 
	private static void ThreadedScale (Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
		texColors = tex.GetPixels();
		newColors = new Color[newWidth * newHeight];
		if (useBilinear)
        {
			ratioX = 1.0f / ((float)newWidth / (tex.width-1));
			ratioY = 1.0f / ((float)newHeight / (tex.height-1));
		}
		else {
			ratioX = ((float)tex.width) / newWidth;
			ratioY = ((float)tex.height) / newHeight;
		}
		w = tex.width;
		w2 = newWidth;
		var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
		var slice = newHeight/cores;
 
		finishCount = 0;
		if (mutex == null) {
			mutex = new Mutex(false);
		}
		if (cores > 1)
		{
		    int i = 0;
		    ThreadData threadData;
			for (i = 0; i < cores-1; i++) {
                threadData = new ThreadData(slice * i, slice * (i + 1));
                ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
			    Thread thread = new Thread(ts);
				thread.Start(threadData);
			}
			threadData = new ThreadData(slice*i, newHeight);
			if (useBilinear)
            {
				BilinearScale(threadData);
			}
			else
            {
				PointScale(threadData);
			}
			while (finishCount < cores)
            {
                Thread.Sleep(1);
            }
		}
		else
        {
			ThreadData threadData = new ThreadData(0, newHeight);
			if (useBilinear)
            {
				BilinearScale(threadData);
			}
			else
            {
				PointScale(threadData);
			}
		}
 
		tex.Resize(newWidth, newHeight);
		tex.SetPixels(newColors);
		tex.Apply();
 
		texColors = null;
		newColors = null;
	}
 
	public static void BilinearScale (System.Object obj)
	{
	    ThreadData threadData = (ThreadData) obj;
		for (var y = threadData.start; y < threadData.end; y++)
        {
			int yFloor = (int)Mathf.Floor(y * ratioY);
			var y1 = yFloor * w;
			var y2 = (yFloor+1) * w;
			var yw = y * w2;
 
			for (var x = 0; x < w2; x++) {
				int xFloor = (int)Mathf.Floor(x * ratioX);
				var xLerp = x * ratioX-xFloor;
				newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor+1], xLerp),
													   ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor+1], xLerp),
													   y*ratioY-yFloor);
			}
		}
 
		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
	}
 
	public static void PointScale (System.Object obj)
	{
	    ThreadData threadData = (ThreadData) obj;
		for (var y = threadData.start; y < threadData.end; y++)
        {
			var thisY = (int)(ratioY * y) * w;
			var yw = y * w2;
			for (var x = 0; x < w2; x++) {
				newColors[yw + x] = texColors[(int)(thisY + ratioX*x)];
			}
		}
 
		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
	}
 
	private static Color ColorLerpUnclamped (Color c1, Color c2, float value)
    {
        return new Color (c1.r + (c2.r - c1.r)*value, 
						  c1.g + (c2.g - c1.g)*value, 
						  c1.b + (c2.b - c1.b)*value, 
						  c1.a + (c2.a - c1.a)*value);
    }
}
}
