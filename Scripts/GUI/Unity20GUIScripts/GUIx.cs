// Original url: http://wiki.unity3d.com/index.php/GUIx
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/Unity20GUIScripts/GUIx.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.Unity20GUIScripts
{
This script serves as an extension to the existing GUI class in Unity. 
Usage MinMaxRect - Construct a MinMaxRect from Vector2s 
Vector2Rect - Construct a Rect from Vector2s 
DrawTexture - Draws a texture using UV co-ordinates 
DrawScaleTexture - Draws a texture scaled to fill the supplied rect but without scaling the corners and edges 
Javascript - GUIx.js // GUIx.MinMaxRect: construct a minmax rect from Vector2s
static function MinMaxRect(min : Vector2, max : Vector2) : Rect
{
	return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
}
 
// GUIx.Vector2Rect: construct a rect from Vector2s
static function Vector2Rect(topLeft : Vector2, size : Vector2) : Rect
{
	return Rect(topLeft.x, topLeft.y, size.x, size.y);
}
 
// GUIx.DrawTexture - DrawTexture implemented with UVs
static function DrawTexture(screenRect : Rect, uvMin : Vector2, uvMax : Vector2, texture : Texture)
{
	DrawTexture(screenRect, GUIx.MinMaxRect(uvMin, uvMax), texture, false);
}
 
// GUIx.DrawTexture - DrawTexture implemented with UVs
static function DrawTexture(screenRect : Rect, uvMin : Vector2, uvMax : Vector2, texture : Texture, texelAdjust : boolean)
{
	DrawTexture(screenRect, GUIx.MinMaxRect(uvMin, uvMax), texture, texelAdjust);
}
 
// GUIx.DrawTexture - DrawTexture implemented with UVs
static function DrawTexture(screenRect : Rect, uvs : Rect, texture : Texture)
{
	DrawTexture(screenRect, uvs, texture, false);
}
 
// GUIx.DrawTexture - DrawTexture implemented with UVs
static function DrawTexture(screenRect : Rect, uvs : Rect, texture : Texture, texelAdjust : boolean)
{
	// sanity check
	if (uvs.width == 0 || uvs.height == 0 || screenRect.width == 0 || screenRect.height == 0 || (texture == null))
	{
		return;
	}
 
	var uvsAdjusted : Rect;
 
	if (texelAdjust)
	{
		// adjust the uvs by half a pixel - sometimes useful when textures are closely packed
		var texelAdjustPixels = 0.5;
		var uvHalfPixel : Vector2 = Vector2(texelAdjustPixels / texture.width, texelAdjustPixels / texture.height);
		uvsAdjusted = Rect.MinMaxRect(	uvs.xMin + uvHalfPixel.x,
										uvs.yMin + uvHalfPixel.y,
										uvs.xMax - uvHalfPixel.x,
										uvs.yMax - uvHalfPixel.y);
	}
	else
	{
		// do not adjust
		uvsAdjusted = uvs;
	}
 
	// create Vec2 sizes for the inputs
	var screenRectSize : Vector2 = Vector2(screenRect.width, screenRect.height);
	var uvSize : Vector2 = Vector2(uvsAdjusted.width, uvsAdjusted.height);
	var textureSize : Vector2 = Vector2(texture.width, texture.height);
 
	// convert the UV size into pixels
	var uvSizeInPixels : Vector2 = Vector2.Scale(textureSize, uvSize);
	// divide the screenRect size (pixels) by the uv size (pixels) to get a scale ration
	var textureScale : Vector2 = Vector2(screenRectSize.x / uvSizeInPixels.x, screenRectSize.y / uvSizeInPixels.y);
	// convert the UV offset into pixels
	var uvOffsetInPixels : Vector2 = Vector2(uvsAdjusted.xMin * textureSize.x, uvsAdjusted.yMin * textureSize.y);
	// multiply the uv offset (pixels) by the texture scale to get the offset of the texture
	// this result will be positive but we need to move the texture left and up - which is the negative direction so *=-1
	var textureOffset : Vector2 = Vector2(uvOffsetInPixels.x * textureScale.x, uvOffsetInPixels.y * textureScale.y) * -1;
	// multiply the texture size by the texture scale to get the actual width and height the texture should be drawn at
	var textureRectSize : Vector2 = Vector2.Scale(textureScale, textureSize);
	// set up the rect used to draw the texture
	var textureRect : Rect = GUIx.Vector2Rect(textureOffset, textureRectSize);
 
	// set up a GUI clipping rect
	GUI.BeginGroup(screenRect);
 
	// rendering the texture directly always results in scaling - use a guiStyle
	var guiStyle : GUIStyle = GUIStyle();
	guiStyle.normal.background = texture;
 
	// render the texture - use GUI.Box instead of GUI.DrawTexture as GUI.DrawTexture does not obey Group clipping
	GUI.Box(textureRect, null, guiStyle);
 
	// end GUI clipping rect
	GUI.EndGroup();
}
 
// GUIx.DrawScaleTexture - Scale a texture to screenRect size, keeping the corners at their original pixel size
static function DrawScaleTexture(screenRect : Rect, texture : Texture)
{
	// sanity check
	if (	screenRect.width == 0 || 
			screenRect.height == 0 || 
			texture == null ||
			texture.width == 0 || 
			texture.height == 0)
	{
		return;
	}
 
	// calculate sizes used when drawing
	var screenRectSize : Vector2 = Vector2(screenRect.width, screenRect.height);
	var halfScreenRectSize : Vector2 = screenRectSize/2.0;
	var textureSize : Vector2 = Vector2(1.0, 1.0);				// use 1.0 instead of actual size as we are passing in UVs
	var textureThirdSize : Vector2 = textureSize / 3.0;
	var textureCornerPixels : Vector2 = Vector2(texture.width/3.0, texture.height/3.0);
 
	var cornerSize : Vector2 = Vector2(	Mathf.Min(halfScreenRectSize.x, textureCornerPixels.x),
										Mathf.Min(halfScreenRectSize.y, textureCornerPixels.y));
 
	var edgeSize : Vector2 = Vector2(	screenRectSize.x - (textureCornerPixels.x * 2.0),
										screenRectSize.y - (textureCornerPixels.y * 2.0));
 
	// use a GUI group to get correct offsetting
	GUI.BeginGroup(screenRect);
 
	// draw the corners
	// top left									
	GUIx.DrawTexture(	GUIx.Vector2Rect(Vector2.zero, cornerSize), 
						GUIx.Vector2Rect(Vector2.zero, textureThirdSize), 
						texture);
 
	// top right
	GUIx.DrawTexture(	GUIx.Vector2Rect(Vector2(screenRectSize.x - cornerSize.x, 0.0), cornerSize),
						GUIx.Vector2Rect(Vector2(textureSize.x - textureThirdSize.x, 0.0), textureThirdSize),
						texture);
 
	// bottom left					
	GUIx.DrawTexture(	GUIx.Vector2Rect(Vector2(0.0, screenRectSize.y - cornerSize.y), cornerSize),
						GUIx.Vector2Rect(Vector2(0.0, textureSize.y - textureThirdSize.y), textureThirdSize),
						texture);
 
	// bottom right
	GUIx.DrawTexture(	GUIx.Vector2Rect(screenRectSize - cornerSize, cornerSize),
						GUIx.Vector2Rect(textureSize - textureThirdSize, textureThirdSize),
						texture);
 
	// will be set to false if one or both of the edges does not draw
	var drawCentre = true;
 
	// should the horizontal edges be drawn?
	if (edgeSize.x>0.0)
	{
		// top
		GUIx.DrawTexture(	GUIx.Vector2Rect(Vector2(cornerSize.x, 0.0), Vector2(edgeSize.x,cornerSize.y)),
							GUIx.Vector2Rect(Vector2(textureThirdSize.x, 0.0), textureThirdSize),
							texture);
 
		// bottom
		GUIx.DrawTexture(	GUIx.Vector2Rect(Vector2(cornerSize.x, screenRectSize.y-cornerSize.y), Vector2(edgeSize.x, cornerSize.y)),
							GUIx.Vector2Rect(Vector2(textureThirdSize.x, textureSize.y-textureThirdSize.y), textureThirdSize),
							texture);		
	}
	else
	{
		drawCentre = false;
	}
 
	// should the vertical edges be drawn?
	if (edgeSize.y>0.0)
	{
		// left
		GUIx.DrawTexture(	GUIx.Vector2Rect(Vector2(0.0, cornerSize.y), Vector2(cornerSize.x, edgeSize.y)),
							GUIx.Vector2Rect(Vector2(0.0, textureThirdSize.y), textureThirdSize),
							texture);
 
		// right
		GUIx.DrawTexture(	GUIx.Vector2Rect(Vector2(screenRectSize.x-cornerSize.x, cornerSize.y), Vector2(cornerSize.x, edgeSize.y)),
							GUIx.Vector2Rect(Vector2(textureSize.x-textureThirdSize.x, textureThirdSize.y), textureThirdSize),
							texture);
 
	}
	else
	{
		drawCentre = false;
	}
 
	// if both edges were drawn, draw the centre texture
	if (drawCentre)
	{
		GUIx.DrawTexture(	GUIx.Vector2Rect(cornerSize, edgeSize),
							GUIx.Vector2Rect(textureThirdSize, textureThirdSize),
							texture);
	}
 
	// end the GUI group
	GUI.EndGroup();
}C# - GUIx.cs using UnityEngine;
 
public class GUIx
{
	// GUIx.MinMaxRect: construct a minmax rect from Vector2s
	public static Rect MinMaxRect(Vector2 min, Vector2 max)
	{
		return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
	}
 
	// GUIx.Vector2Rect: construct a rect from Vector2s
	public static Rect Vector2Rect(Vector2 topLeft, Vector2 size)
	{
		return new Rect(topLeft.x, topLeft.y, size.x, size.y);
	}
 
	// GUIx.DrawTexture - DrawTexture implemented with UVs
	public static void DrawTexture(Rect screenRect, Vector2 uvMin, Vector2 uvMax, Texture texture)
	{
		DrawTexture(screenRect, MinMaxRect(uvMin, uvMax), texture, false);
	}
 
	// GUIx.DrawTexture - DrawTexture implemented with UVs
	public static void DrawTexture(Rect screenRect, Vector2 uvMin, Vector2 uvMax, Texture texture, bool texelAdjust)
	{
		DrawTexture(screenRect, GUIx.MinMaxRect(uvMin, uvMax), texture, texelAdjust);
	}
 
	// GUIx.DrawTexture - DrawTexture implemented with UVs
	public static void DrawTexture(Rect screenRect, Rect uvs, Texture texture)
	{
		DrawTexture(screenRect, uvs, texture, false);
	}
 
	// GUIx.DrawTexture - DrawTexture implemented with UVs
	private static void DrawTexture(Rect screenRect, Rect uvs, Texture texture, bool texelAdjust)
	{
		// sanity check
		if (uvs.width == 0 || uvs.height == 0 || screenRect.width == 0 || screenRect.height == 0 || (texture == null))
		{
			return;
		}
 
		Rect uvsAdjusted;
 
		if (texelAdjust)
		{
			// adjust the uvs by half a pixel - sometimes useful when textures are closely packed
			const float texelAdjustPixels = 0.5f;
			Vector2 uvHalfPixel = new Vector2(texelAdjustPixels/texture.width, texelAdjustPixels/texture.height);
			uvsAdjusted = Rect.MinMaxRect(uvs.xMin + uvHalfPixel.x,
			                              uvs.yMin + uvHalfPixel.y,
			                              uvs.xMax - uvHalfPixel.x,
			                              uvs.yMax - uvHalfPixel.y);
		}
		else
		{
			// do not adjust
			uvsAdjusted = uvs;
		}
 
		// create Vec2 sizes for the inputs
		Vector2 screenRectSize = new Vector2(screenRect.width, screenRect.height);
		Vector2 uvSize = new Vector2(uvsAdjusted.width, uvsAdjusted.height);
		Vector2 textureSize = new Vector2(texture.width, texture.height);
 
		// convert the UV size into pixels
		Vector2 uvSizeInPixels = Vector2.Scale(textureSize, uvSize);
		// divide the screenRect size (pixels) by the uv size (pixels) to get a scale ration
		Vector2 textureScale = new Vector2(screenRectSize.x/uvSizeInPixels.x, screenRectSize.y/uvSizeInPixels.y);
		// convert the UV offset into pixels
		Vector2 uvOffsetInPixels = new Vector2(uvsAdjusted.xMin*textureSize.x, uvsAdjusted.yMin*textureSize.y);
		// multiply the uv offset (pixels) by the texture scale to get the offset of the texture
		// this result will be positive but we need to move the texture left and up - which is the negative direction so *=-1
		Vector2 textureOffset = new Vector2(uvOffsetInPixels.x*textureScale.x, uvOffsetInPixels.y*textureScale.y)*-1;
		// multiply the texture size by the texture scale to get the actual width and height the texture should be drawn at
		Vector2 textureRectSize = Vector2.Scale(textureScale, textureSize);
		// set up the rect used to draw the texture
		Rect textureRect = GUIx.Vector2Rect(textureOffset, textureRectSize);
 
		// set up a GUI clipping rect
		GUI.BeginGroup(screenRect);
 
		// rendering the texture directly always results in scaling - use a guiStyle
		GUIStyle guiStyle = new GUIStyle();
		guiStyle.normal.background = (Texture2D) texture;
 
		// render the texture - use GUI.Box instead of GUI.DrawTexture as GUI.DrawTexture does not obey Group clipping
		GUI.Box(textureRect, (string) null, guiStyle);
 
		// end GUI clipping rect
		GUI.EndGroup();
	}
 
	// GUIx.DrawScaleTexture - Scale a texture to screenRect size, keeping the corners at their original pixel size
	private static void DrawScaleTexture(Rect screenRect, Texture texture)
	{
		// sanity check
		if (screenRect.width == 0 ||
		    screenRect.height == 0 ||
		    texture == null ||
		    texture.width == 0 ||
		    texture.height == 0)
		{
			return;
		}
 
		// calculate sizes used when drawing
		Vector2 screenRectSize = new Vector2(screenRect.width, screenRect.height);
		Vector2 halfScreenRectSize = screenRectSize/2.0f;
		Vector2 textureSize = new Vector2(1.0f, 1.0f); // use 1.0 instead of actual size as we are passing in UVs
		Vector2 textureThirdSize = textureSize/3.0f;
		Vector2 textureCornerPixels = new Vector2(texture.width/3.0f, texture.height/3.0f);
 
		Vector2 cornerSize = new Vector2(Mathf.Min(halfScreenRectSize.x, textureCornerPixels.x),
		                                 Mathf.Min(halfScreenRectSize.y, textureCornerPixels.y));
 
		Vector2 edgeSize = new Vector2(screenRectSize.x - (textureCornerPixels.x*2.0f),
		                               screenRectSize.y - (textureCornerPixels.y*2.0f));
 
		// use a GUI group to get correct offsetting
		GUI.BeginGroup(screenRect);
 
		// draw the corners
		// top left         
		GUIx.DrawTexture(GUIx.Vector2Rect(Vector2.zero, cornerSize),
		                 GUIx.Vector2Rect(Vector2.zero, textureThirdSize),
		                 texture);
 
		// top right
		GUIx.DrawTexture(GUIx.Vector2Rect(new Vector2(screenRectSize.x - cornerSize.x, 0.0f), cornerSize),
		                 GUIx.Vector2Rect(new Vector2(textureSize.x - textureThirdSize.x, 0.0f), textureThirdSize),
		                 texture);
 
		// bottom left         
		GUIx.DrawTexture(GUIx.Vector2Rect(new Vector2(0.0f, screenRectSize.y - cornerSize.y), cornerSize),
		                 GUIx.Vector2Rect(new Vector2(0.0f, textureSize.y - textureThirdSize.y), textureThirdSize),
		                 texture);
 
		// bottom right
		GUIx.DrawTexture(GUIx.Vector2Rect(screenRectSize - cornerSize, cornerSize),
		                 GUIx.Vector2Rect(textureSize - textureThirdSize, textureThirdSize),
		                 texture);
 
		// will be set to false if one or both of the edges does not draw
		bool drawCentre = true;
 
		// should the horizontal edges be drawn?
		if (edgeSize.x > 0.0)
		{
			// top
			GUIx.DrawTexture(GUIx.Vector2Rect(new Vector2(cornerSize.x, 0.0f), new Vector2(edgeSize.x, cornerSize.y)),
			                 GUIx.Vector2Rect(new Vector2(textureThirdSize.x, 0.0f), textureThirdSize),
			                 texture);
 
			// bottom
			GUIx.DrawTexture(
				GUIx.Vector2Rect(new Vector2(cornerSize.x, screenRectSize.y - cornerSize.y), new Vector2(edgeSize.x, cornerSize.y)),
				GUIx.Vector2Rect(new Vector2(textureThirdSize.x, textureSize.y - textureThirdSize.y), textureThirdSize),
				texture);
		}
		else
		{
			drawCentre = false;
		}
 
		// should the vertical edges be drawn?
		if (edgeSize.y > 0.0)
		{
			// left
			GUIx.DrawTexture(GUIx.Vector2Rect(new Vector2(0.0f, cornerSize.y), new Vector2(cornerSize.x, edgeSize.y)),
			                 GUIx.Vector2Rect(new Vector2(0.0f, textureThirdSize.y), textureThirdSize),
			                 texture);
 
			// right
			GUIx.DrawTexture(
				GUIx.Vector2Rect(new Vector2(screenRectSize.x - cornerSize.x, cornerSize.y), new Vector2(cornerSize.x, edgeSize.y)),
				GUIx.Vector2Rect(new Vector2(textureSize.x - textureThirdSize.x, textureThirdSize.y), textureThirdSize),
				texture);
		}
		else
		{
			drawCentre = false;
		}
 
		// if both edges were drawn, draw the centre texture
		if (drawCentre)
		{
			GUIx.DrawTexture(GUIx.Vector2Rect(cornerSize, edgeSize),
			                 GUIx.Vector2Rect(textureThirdSize, textureThirdSize),
			                 texture);
		}
 
		// end the GUI group
		GUI.EndGroup();
	}
}
}
