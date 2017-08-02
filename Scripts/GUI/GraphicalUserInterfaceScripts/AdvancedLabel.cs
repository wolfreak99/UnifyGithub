/*************************
 * Original url: http://wiki.unity3d.com/index.php/AdvancedLabel
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/AdvancedLabel.cs
 * File based on original modification date of: 19 January 2013, at 23:06. 
 *
 * Author: Bérenger 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 C# - AdvancedLabel.cs 
    4 JS - AdvancedLabel.js 
    
    Description Display a label the way you're used to, and add the modifiers you need. You can draw a label with a specific size, color, font, color or alignement (feel free to add new ones), any changes are reversed after the label is drawn. This is a much simpler but less powerfull alternative to fancy labels (http://forum.unity3d.com/threads/9549-FancyLabel-Multicolor-and-Multifont-label/) 
    Usage Just use the static class AdvancedLabel as you would use GUI or GUILayout. The functions are Draw and DrawLayout. 
    Here is an example : 
    C# 
    GUILayout.BeginArea( rect );
        AdvancedLabel.DrawLayout( "Big size", new GUILayoutOption[]{ GUILayout.MinHeight(20) }, new NewFontSize(20), new NewColor(Color.yellow) );
        AdvancedLabel.DrawLayout( "Small size", new NewFontSize(12) );
    GUILayout.EndArea();JS 
    #pragma strict
    var skin : GUISkin;
     
    function OnGUI()
    {
    	GUI.skin = skin;
    	GUILayout.BeginArea( Rect( 0, 0, Screen.width, Screen.height ) );
    		AdvancedLabel.DrawLayout( "TITLE", (new Array( NewFontSize(50), NewColor(Color.yellow) ).ToBuiltin(ILabelParameter)) as ILabelParameter[] );
    		AdvancedLabel.DrawLayout( "Subtitle", (new Array( NewFontSize(30), NewFontStyle(FontStyle.Bold) ).ToBuiltin(ILabelParameter)) as ILabelParameter[] );
    		GUILayout.Label( "Normal" );
    		AdvancedLabel.DrawLayout( "Middle Center", (new Array( NewAlignement(TextAnchor.MiddleCenter) ).ToBuiltin(ILabelParameter)) as ILabelParameter[] );
    		GUILayout.FlexibleSpace();
    	GUILayout.EndArea();
    }
    
    C# - AdvancedLabel.cs using UnityEngine;
     
    public static class AdvancedLabel
    {
        public static void Draw(Rect r, string txt, params ILabelParameter[] parameters) { Draw(r, new GUIContent(txt), GUI.skin.label, parameters); }
        public static void Draw(Rect r, Texture2D tex, params ILabelParameter[] parameters) { Draw(r, new GUIContent(tex), GUI.skin.label, parameters); }
        public static void Draw(Rect r, GUIContent content, params ILabelParameter[] parameters) { Draw(r, content, GUI.skin.label, parameters); }
        public static void Draw(Rect r, string txt, GUIStyle style, params ILabelParameter[] parameters) { Draw(r, new GUIContent(txt), style, parameters); }
        public static void Draw(Rect r, Texture2D tex, GUIStyle style, params ILabelParameter[] parameters) { Draw(r, new GUIContent(tex), style, parameters); }
        public static void Draw(Rect r, GUIContent content, GUIStyle style, params ILabelParameter[] parameters)
        {
            foreach( ILabelParameter L in parameters )
                L.Update(style);
     
            GUI.Label(r, content, style);
     
            foreach( ILabelParameter L in parameters )
                L.Backup(style);
        }
     
        public static void DrawLayout(string txt, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(txt), GUI.skin.label, null, parameters); }
        public static void DrawLayout(Texture2D tex, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(tex), GUI.skin.label, null, parameters); }
        public static void DrawLayout(GUIContent content, params ILabelParameter[] parameters) { DrawLayout(content, GUI.skin.label, null, parameters); }
        public static void DrawLayout(string txt, GUIStyle style, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(txt), style, null, parameters); }
        public static void DrawLayout(Texture2D tex, GUIStyle style, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(tex), style, null, parameters); }
        public static void DrawLayout(GUIContent content, GUIStyle style, params ILabelParameter[] parameters) { DrawLayout(content, style, null, parameters); }
        public static void DrawLayout(string txt, GUILayoutOption[] options, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(txt), GUI.skin.label, options, parameters); }
        public static void DrawLayout(Texture2D tex, GUILayoutOption[] options, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(tex), GUI.skin.label, options, parameters); }
        public static void DrawLayout(GUIContent content, GUILayoutOption[] options, params ILabelParameter[] parameters) { DrawLayout(content, GUI.skin.label, options, parameters); }
        public static void DrawLayout(string txt, GUIStyle style, GUILayoutOption[] options, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(txt), style, options, parameters); }
        public static void DrawLayout(Texture2D tex, GUIStyle style, GUILayoutOption[] options, params ILabelParameter[] parameters) { DrawLayout(new GUIContent(tex), style, options, parameters); }
        public static void DrawLayout(GUIContent content, GUIStyle style, GUILayoutOption[] options, params ILabelParameter[] parameters)
        {
            foreach( ILabelParameter L in parameters )
                L.Update(style);
     
            GUILayout.Label(content, style, options);
     
            foreach( ILabelParameter L in parameters )
                L.Backup(style);
        }
    }
     
     
    public interface ILabelParameter
    {
        void Backup(GUIStyle style);
        void Update(GUIStyle style);
    }
     
     
    public class NewFont : ILabelParameter
    {
        Font font, backupFont;
        public NewFont(Font _font) { font = _font; }
        public void Backup(GUIStyle style) { style.font = backupFont; }
        public void Update(GUIStyle style) { backupFont = style.font; style.font = font; }
    }
     
    public class NewFontSize : ILabelParameter
    {
        int fontSize, backupFontSize;
        public NewFontSize(int _fontSize) { fontSize = _fontSize;  }
        public void Backup(GUIStyle style) { style.fontSize = backupFontSize; }
        public void Update(GUIStyle style) { backupFontSize = style.fontSize; style.fontSize = fontSize; }
    }
     
    public class NewFontStyle : ILabelParameter
    {
        FontStyle fontStyle, backupFontStyle;
        public NewFontStyle(FontStyle _fontStyle) { fontStyle = _fontStyle; }
        public void Backup(GUIStyle style) { style.fontStyle = backupFontStyle; }
        public void Update(GUIStyle style) { backupFontStyle = style.fontStyle; style.fontStyle = fontStyle; }
    }
     
    public class NewColor : ILabelParameter
    {
        Color color, backupColor;
        public NewColor(Color _color) { color = _color; }
        public void Backup(GUIStyle style) { GUI.color = backupColor; }
        public void Update(GUIStyle style) { backupColor = GUI.color; GUI.color = color; }
    }
     
    public class NewAlignement : ILabelParameter
    {
        TextAnchor alignment, backupAlignment;
        public NewAlignement(TextAnchor _alignment) { alignment = _alignment; }
        public void Backup(GUIStyle style) { style.alignment = backupAlignment; }
        public void Update(GUIStyle style) { backupAlignment = style.alignment; style.alignment = alignment; }
    }
    
    JS - AdvancedLabel.js #pragma strict
     
    public static class AdvancedLabel
    {
        public function Draw(r : Rect, txt : String, parameters : ILabelParameter[]) { Draw(r, GUIContent(txt), GUI.skin.label, parameters); }
        public function Draw(r : Rect, tex : Texture2D, parameters : ILabelParameter[]) { Draw(r, GUIContent(tex), GUI.skin.label, parameters); }
        public function Draw(r : Rect, content : GUIContent, parameters : ILabelParameter[]) { Draw(r, content, GUI.skin.label, parameters); }
        public function Draw(r : Rect, txt : String, style : GUIStyle, parameters : ILabelParameter[]) { Draw(r, GUIContent(txt), style, parameters); }
        public function Draw(r : Rect, tex : Texture2D, style : GUIStyle, parameters : ILabelParameter[]) { Draw(r, GUIContent(tex), style, parameters); }
        public function Draw(r : Rect, content : GUIContent, style : GUIStyle, parameters : ILabelParameter[])
        {
            for( var L :  ILabelParameter in parameters )
                L.Update(style);
     
            GUI.Label(r, content, style);
     
            for( var L :  ILabelParameter in parameters )
                L.Backup(style);
        }
     
        public function DrawLayout(txt : String, parameters : ILabelParameter[]) { DrawLayout(GUIContent(txt), GUI.skin.label, null, parameters); }
        public function DrawLayout(tex : Texture2D, parameters : ILabelParameter[]) { DrawLayout(GUIContent(tex), GUI.skin.label, null, parameters); }
        public function DrawLayout(content : GUIContent, parameters : ILabelParameter[]) { DrawLayout(content, GUI.skin.label, null, parameters); }
        public function DrawLayout(txt : String, style : GUIStyle, parameters : ILabelParameter[]) { DrawLayout(GUIContent(txt), style, null, parameters); }
        public function DrawLayout(tex : Texture2D, style : GUIStyle, parameters : ILabelParameter[]) { DrawLayout(GUIContent(tex), style, null, parameters); }
        public function DrawLayout(content : GUIContent, style : GUIStyle, parameters : ILabelParameter[]) { DrawLayout(content, style, null, parameters); }
        public function DrawLayout(txt : String, options : GUILayoutOption[], parameters : ILabelParameter[]) { DrawLayout(GUIContent(txt), GUI.skin.label, options, parameters); }
        public function DrawLayout(tex : Texture2D, options : GUILayoutOption[], parameters : ILabelParameter[]) { DrawLayout(GUIContent(tex), GUI.skin.label, options, parameters); }
        public function DrawLayout(content : GUIContent, options : GUILayoutOption[], parameters : ILabelParameter[]) { DrawLayout(content, GUI.skin.label, options, parameters); }
        public function DrawLayout(txt : String, style : GUIStyle, options : GUILayoutOption[], parameters : ILabelParameter[]) { DrawLayout(GUIContent(txt), style, options, parameters); }
        public function DrawLayout(tex : Texture2D, style : GUIStyle, options : GUILayoutOption[], parameters : ILabelParameter[]) { DrawLayout(GUIContent(tex), style, options, parameters); }
        public function DrawLayout(content : GUIContent, style : GUIStyle, options : GUILayoutOption[], parameters : ILabelParameter[])
        {
            for( var L :  ILabelParameter in parameters )
                L.Update(style);
     
            GUILayout.Label(content, style, options);
     
            for( var L :  ILabelParameter in parameters )
                L.Backup(style);
        }
    }
     
     
    public interface ILabelParameter
    {
        function Backup(style : GUIStyle);
        function Update(style : GUIStyle);
    }
     
     
    public class NewFont implements ILabelParameter
    {
        var font : Font;
        var backupFont : Font;
        public function NewFont(_font : Font) { font = _font; }
        public function Backup(style : GUIStyle) { style.font = backupFont; }
        public function Update(style : GUIStyle) { backupFont = style.font; style.font = font; }
    }
     
    public class NewFontSize implements ILabelParameter
    {
        var fontSize : int;
        var backupFontSize : int;
        public function NewFontSize(_fontSize : int) { fontSize = _fontSize;  }
        public function Backup(style : GUIStyle) { style.fontSize = backupFontSize; }
        public function Update(style : GUIStyle) { backupFontSize = style.fontSize; style.fontSize = fontSize; }
    }
     
    public class NewFontStyle implements ILabelParameter
    {
        var fontStyle : FontStyle;
        var backupFontStyle : FontStyle;
        public function NewFontStyle(_fontStyle : FontStyle) { fontStyle = _fontStyle; }
        public function Backup(style : GUIStyle) { style.fontStyle = backupFontStyle; }
        public function Update(style : GUIStyle) { backupFontStyle = style.fontStyle; style.fontStyle = fontStyle; }
    }
     
    public class NewColor implements ILabelParameter
    {
        var color : Color;
        var backupColor : Color;
        public function NewColor(_color : Color) { color = _color; }
        public function Backup(style : GUIStyle) { GUI.color = backupColor; }
        public function Update(style : GUIStyle) { backupColor = GUI.color; GUI.color = color; }
    }
     
    public class NewAlignement implements ILabelParameter
    {
        var alignment : TextAnchor;
        var backupAlignment : TextAnchor;
        public function NewAlignement(_alignment : TextAnchor) { alignment = _alignment; }
        public function Backup(style : GUIStyle) { style.alignment = backupAlignment; }
        public function Update(style : GUIStyle) { backupAlignment = style.alignment; style.alignment = alignment; }
}
}
