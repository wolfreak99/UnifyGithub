/*************************
 * Original url: http://wiki.unity3d.com/index.php/ShadowAndOutline
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/ShadowAndOutline.cs
 * File based on original modification date of: 25 June 2012, at 21:25. 
 *
 * Author: Bérenger (The idea is not from me, but I can't remember the source) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 C# - ShadowAndOutline.cs 
    4 JS - ShadowAndOutline.js 
    
    Description Draw a label several times in several direction, creating shadows or outlines. 
    Usage Simply call it as you would call the GUI functions. The gui is drawn twice for shadows, four times for outlines. You can choose the distance at which they are drawn, and the color. 
    C# - ShadowAndOutline.cs using UnityEngine;
     
    public static class ShadowAndOutline
    {
            public static void DrawOutline(Rect rect, string text, GUIStyle style, Color outColor, Color inColor, float size)
            {
                float halfSize = size * 0.5F;
                GUIStyle backupStyle = new GUIStyle(style);
                Color backupColor = GUI.color;
     
                style.normal.textColor = outColor;
                GUI.color = outColor;
     
                rect.x -= halfSize;
                GUI.Label(rect, text, style);
     
                rect.x += size;
                GUI.Label(rect, text, style);
     
                rect.x -= halfSize;
                rect.y -= halfSize;
                GUI.Label(rect, text, style);
     
                rect.y += size;
                GUI.Label(rect, text, style);
     
                rect.y -= halfSize;
                style.normal.textColor = inColor;
                GUI.color = backupColor;
                GUI.Label(rect, text, style);
     
                style = backupStyle;
            }
     
            public static void DrawShadow(Rect rect, GUIContent content, GUIStyle style, Color txtColor, Color shadowColor,
                                            Vector2 direction)
            {
                GUIStyle backupStyle = style;
     
                style.normal.textColor = shadowColor;
                rect.x += direction.x;
                rect.y += direction.y;
                GUI.Label(rect, content, style);
     
                style.normal.textColor = txtColor;
                rect.x -= direction.x;
                rect.y -= direction.y;
                GUI.Label(rect, content, style);
     
                style = backupStyle;
            }
            public static void DrawLayoutShadow(GUIContent content, GUIStyle style, Color txtColor, Color shadowColor,
                                            Vector2 direction, params GUILayoutOption[] options)
            {
                DrawShadow(GUILayoutUtility.GetRect(content, style, options), content, style, txtColor, shadowColor, direction);
            }
     
            public static bool DrawButtonWithShadow(Rect r, GUIContent content, GUIStyle style, float shadowAlpha, Vector2 direction)
            {
                GUIStyle letters = new GUIStyle(style);
                letters.normal.background = null;
                letters.hover.background = null;
                letters.active.background = null;
     
                bool result = GUI.Button(r, content, style);
     
                Color color = r.Contains(Event.current.mousePosition) ? letters.hover.textColor : letters.normal.textColor;
     
                DrawShadow(r, content, letters, color, new Color(0f, 0f, 0f, shadowAlpha), direction);
     
                return result;
            }
     
            public static bool DrawLayoutButtonWithShadow(GUIContent content, GUIStyle style, float shadowAlpha,
                                                           Vector2 direction, params GUILayoutOption[] options)
            {
                return DrawButtonWithShadow(GUILayoutUtility.GetRect(content, style, options), content, style, shadowAlpha, direction);
            }
    }
    
    JS - ShadowAndOutline.js #pragma strict
     
    public static class ShadowAndOutline
    {
        function DrawOutline(rect : Rect, text : String, style : GUIStyle, outColor : Color, inColor : Color, size : float)
        {
            var halfSize : float = size * 0.5;
            var backupStyle : GUIStyle = GUIStyle(style);
            var backupColor : Color = GUI.color;
     
            style.normal.textColor = outColor;
            GUI.color = outColor;
     
            rect.x -= halfSize;
            GUI.Label(rect, text, style);
     
            rect.x += size;
            GUI.Label(rect, text, style);
     
            rect.x -= halfSize;
            rect.y -= halfSize;
            GUI.Label(rect, text, style);
     
            rect.y += size;
            GUI.Label(rect, text, style);
     
            rect.y -= halfSize;
            style.normal.textColor = inColor;
            GUI.color = backupColor;
            GUI.Label(rect, text, style);
     
            style = backupStyle;
        }
     
        function DrawShadow(rect : Rect, content : GUIContent, style : GUIStyle, txtColor : Color, shadowColor : Color,
                                        direction : Vector2)
        {
            var backupStyle : GUIStyle = style;
     
            style.normal.textColor = shadowColor;
            rect.x += direction.x;
            rect.y += direction.y;
            GUI.Label(rect, content, style);
     
            style.normal.textColor = txtColor;
            rect.x -= direction.x;
            rect.y -= direction.y;
            GUI.Label(rect, content, style);
     
            style = backupStyle;
        }
        function DrawLayoutShadow(content : GUIContent, style : GUIStyle, txtColor : Color, shadowColor : Color,
                                        direction : Vector2, options : GUILayoutOption[] )
        {
            DrawShadow(GUILayoutUtility.GetRect(content, style, options), content, style, txtColor, shadowColor, direction);
        }
     
        function DrawButtonWithShadow( r : Rect, content : GUIContent, style : GUIStyle, shadowAlpha : float, direction : Vector2) : boolean
        {
            var letters : GUIStyle = GUIStyle(style);
            letters.normal.background = null;
            letters.hover.background = null;
            letters.active.background = null;
     
            var result : boolean = GUI.Button(r, content, style);
     
            var color : Color = r.Contains(Event.current.mousePosition) ? letters.hover.textColor : letters.normal.textColor;
     
            DrawShadow(r, content, letters, color, Color(0.0, 0.0, 0.0, shadowAlpha), direction);
     
            return result;
        }
     
        function DrawLayoutButtonWithShadow(content : GUIContent, style : GUIStyle, shadowAlpha : float,
                                                       direction : Vector2, options : GUILayoutOption[] ) : boolean
        {
            return DrawButtonWithShadow(GUILayoutUtility.GetRect(content, style, options), content, style, shadowAlpha, direction);
        }
}
}
