// Original url: http://wiki.unity3d.com/index.php/DrawLine
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/Unity20GUIScripts/DrawLine.cs
// File based on original modification date of: 20 August 2013, at 23:40. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.Unity20GUIScripts
{
Author: capnbishop 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - DrawLine.js 
4 JavaScript - DrawLine.js - Editor version 
5 C# - DrawLine.cs 

DescriptionThis script allows you to draw a line between two points in the GUI system. 
UsageRender a line in the GUI system by calling DrawLine and passing it a set of Vector2's for point A and point B of the line. 
Optionally, you can pass DrawLine a color and width for the line, as well as using a Rect instead of Vector2's. If you do not pass a width, the line will have a width of 1. If you do not pass a color, the line will be rendered with the GUI.contentColor. 
Example 1: 
// Render a line from the center of the screen to the mouse position
 
var width = 1.0;
var color = Color.black;
 
function OnGUI () {
    var pointA = Vector2(Screen.width/2, Screen.height/2);
    var pointB = Event.current.mousePosition;
    DrawLine.DrawLine(pointA, pointB, color, width);
}JavaScript - DrawLine.js//****************************************************************************************************
//  static function DrawLine(rect : Rect) : void
//  static function DrawLine(rect : Rect, color : Color) : void
//  static function DrawLine(rect : Rect, width : float) : void
//  static function DrawLine(rect : Rect, color : Color, width : float) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, width : float) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color, width : float) : void
//  
//  Draws a GUI line on the screen.
//  
//  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
//  This function works by drawing a 1x1 texture filled with a color, which is then scaled
//   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
//****************************************************************************************************
 
static var lineTex : Texture2D;
 
static function DrawLine(rect : Rect) { DrawLine(rect, GUI.contentColor, 1.0); }
static function DrawLine(rect : Rect, color : Color) { DrawLine(rect, color, 1.0); }
static function DrawLine(rect : Rect, width : float) { DrawLine(rect, GUI.contentColor, width); }
static function DrawLine(rect : Rect, color : Color, width : float) { DrawLine(Vector2(rect.x, rect.y), Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
static function DrawLine(pointA : Vector2, pointB : Vector2) { DrawLine(pointA, pointB, GUI.contentColor, 1.0); }
static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color) { DrawLine(pointA, pointB, color, 1.0); }
static function DrawLine(pointA : Vector2, pointB : Vector2, width : float) { DrawLine(pointA, pointB, GUI.contentColor, width); }
static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color, width : float) {
    // Save the current GUI matrix, since we're going to make changes to it.
    var matrix = GUI.matrix;
 
    // Generate a single pixel texture if it doesn't exist
    if (!lineTex) {
    	lineTex = Texture2D(1, 1);
    	lineTex.SetPixel(0, 0, Color.white);
    	lineTex.Apply();
    }
 
    // Store current GUI color, so we can switch it back later,
    // and set the GUI color to the color parameter
    var savedColor = GUI.color;
    GUI.color = color;
 
    // Determine the angle of the line.
    var angle = Vector3.Angle(pointB-pointA, Vector2.right);
 
    // Vector3.Angle always returns a positive number.
    // If pointB is above pointA, then angle needs to be negative.
    if (pointA.y > pointB.y) { angle = -angle; }
 
    // Use ScaleAroundPivot to adjust the size of the line.
    // We could do this when we draw the texture, but by scaling it here we can use
    //  non-integer values for the width and length (such as sub 1 pixel widths).
    // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
    //  is centered on the origin at pointA.
    GUIUtility.ScaleAroundPivot(Vector2((pointB-pointA).magnitude, width), Vector2(pointA.x, pointA.y + 0.5));
 
    // Set the rotation for the line.
    //  The angle was calculated with pointA as the origin.
    GUIUtility.RotateAroundPivot(angle, pointA);
 
    // Finally, draw the actual line.
    // We're really only drawing a 1x1 texture from pointA.
    // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
    //  render with the proper width, length, and angle.
    GUI.DrawTexture(Rect(pointA.x, pointA.y, 1, 1), lineTex);
 
    // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
    GUI.matrix = matrix;
    GUI.color = savedColor;
}JavaScript - DrawLine.js - Editor version//****************************************************************************************************
//  static function DrawLine(rect : Rect) : void
//  static function DrawLine(rect : Rect, color : Color) : void
//  static function DrawLine(rect : Rect, width : float) : void
//  static function DrawLine(rect : Rect, color : Color, width : float) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, width : float) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color, width : float) : void
//  
//  Draws a GUI line on the screen in an Editor window.
//  
//  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
//  This function works by drawing a 1x1 texture filled with a color, which is then scaled
//   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
//  There seems to a bug in how GUI.matrix is applied within Editor windows, so this version
//  of the script employs a little hack to work around it
//****************************************************************************************************
 
static var lineTex : Texture2D;
 
static function DrawLine(rect : Rect) { DrawLine(rect, GUI.contentColor, 1.0); }
static function DrawLine(rect : Rect, color : Color) { DrawLine(rect, color, 1.0); }
static function DrawLine(rect : Rect, width : float) { DrawLine(rect, GUI.contentColor, width); }
static function DrawLine(rect : Rect, color : Color, width : float) { DrawLine(Vector2(rect.x, rect.y), Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
static function DrawLine(pointA : Vector2, pointB : Vector2) { DrawLine(pointA, pointB, GUI.contentColor, 1.0); }
static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color) { DrawLine(pointA, pointB, color, 1.0); }
static function DrawLine(pointA : Vector2, pointB : Vector2, width : float) { DrawLine(pointA, pointB, GUI.contentColor, width); }
static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color, width : float) {
    // Save the current GUI matrix, since we're going to make changes to it.
    var matrix = GUI.matrix;
    GUI.matrix = Matrix4x4.identity;
 
    // Generate a single pixel texture if it doesn't exist
    if (!lineTex) {
    	lineTex = Texture2D(1, 1);
    	lineTex.SetPixel(0, 0, Color.white);
    	lineTex.Apply();
    }
 
    // Store current GUI color, so we can switch it back later,
    // and set the GUI color to the color parameter
    var savedColor = GUI.color;
    GUI.color = color;
 
    // Use Scale to adjust the size of the line.
    // We could do this when we draw the texture, but by scaling it here we can use
    //  non-integer values for the width and length (such as sub 1 pixel widths).
    // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
    //  is centered on the origin at pointA.
 
    // Set the rotation for the line.
    //  The angle was calculated with pointA as the origin.
 
 
   // For some reason, when used in an Editor window, everything is rotated around the wrong place until some kind of offset is used. The exact value of this was a bit of guesswork - your mileage may vary. Think this is probably a bug in how GUI.matrix is applied.
    var offset : Vector2 = new Vector2(0, -20);
 
    // Set the GUI matrix to rotate around a pivot point
    // We're doing a 3-stage matrix multiplication since we don't want to rotate around the origin - there's a weird offset bug in the way the GUI is rendered that needs to be worked around   
    offset += Vector2(0, 0.5); // Compensate for line width
    var guiRot : Quaternion = Quaternion.FromToRotation(Vector2.right, pointB-pointA);
    var guiRotMat : Matrix4x4 = Matrix4x4.TRS(pointA, guiRot, new Vector3((pointB-pointA).magnitude, width, 1));
    var guiTransMat : Matrix4x4 = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
    var guiTransMatInv : Matrix4x4 = Matrix4x4.TRS(-offset, Quaternion.identity, Vector3.one);
 
    GUI.matrix = guiTransMatInv * guiRotMat * guiTransMat;
 
    // Finally, draw the actual line.
    // We're really only drawing a 1x1 texture from pointA.
    // The matrix operations done with Scale, Rotate and Translate will make this
    //  render with the proper width, length, and angle and position
    GUI.DrawTexture(Rect(0, 0, 1, 1), lineTex);
 
    // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
    GUI.matrix = matrix;
    GUI.color = savedColor;
}C# - DrawLine.cs A version of the C# script suitable for editor scripts exists at http://forum.unity3d.com/threads/71979-Drawing-lines-in-the-editor. There is also an optimized version of the script later in that forum post. 
using System;
using UnityEngine;
 
public class Drawing
{
    //****************************************************************************************************
    //  static function DrawLine(rect : Rect) : void
    //  static function DrawLine(rect : Rect, color : Color) : void
    //  static function DrawLine(rect : Rect, width : float) : void
    //  static function DrawLine(rect : Rect, color : Color, width : float) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, width : float) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color, width : float) : void
    //  
    //  Draws a GUI line on the screen.
    //  
    //  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
    //  This function works by drawing a 1x1 texture filled with a color, which is then scaled
    //   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
    //****************************************************************************************************
 
    public static Texture2D lineTex;
 
    public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
    public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
    public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
    public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        // Save the current GUI matrix, since we're going to make changes to it.
        Matrix4x4 matrix = GUI.matrix;
 
        // Generate a single pixel texture if it doesn't exist
        if (!lineTex) { lineTex = new Texture2D(1, 1); }
 
        // Store current GUI color, so we can switch it back later,
        // and set the GUI color to the color parameter
        Color savedColor = GUI.color;
        GUI.color = color;
 
        // Determine the angle of the line.
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);
 
        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if (pointA.y > pointB.y) { angle = -angle; }
 
        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
 
        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        GUIUtility.RotateAroundPivot(angle, pointA);
 
        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);
 
        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }
}
}
