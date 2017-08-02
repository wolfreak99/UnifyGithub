/*************************
 * Original url: http://wiki.unity3d.com/index.php/Notes
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Notes.cs
 * File based on original modification date of: 15 November 2015, at 04:38. 
 *
 * C# Author: Jeremy Hollingsworth (jeremyace) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 C# - NoteEditor.cs 
    4 C# - Note.cs 
    5 JS - NoteEditor.js 
    6 JS - Note.js 
    
    Description Allows you to add a custom note field to any GameObject. 
    Usage Place NoteEditor in the Editor folder in your assets folder. 
    Place Note in your chosen script folder. 
    To add a note, simply drag Note.cs or Note.js to your object and type away. 
    Warning: The C# script uses unsupported code, so use at your own risk. 
    C# - NoteEditor.cs using UnityEditor;
    using UnityEngine;
    using System;
     
    [CustomEditor(typeof(Note))]
    public class NoteEditor : Editor
    {
        private Note note;
     
        private Vector2 scrollPos;
     
        private void Init()
        {
            note = base.target as Note;        
        }
     
        void OnInspectorGUI()
        {
            Init();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(100.0F));
            GUIStyle myStyle = new GUIStyle();
            myStyle.wordWrap = true;
            myStyle.stretchWidth = false;
            myStyle.normal.textColor = Color.gray;
            note.text = GUILayout.TextArea(note.text, myStyle, GUILayout.Width(200.0F), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            Repaint();       
        }          
    }C# - Note.cs using UnityEngine;
     
    public class Note : MonoBehaviour
    {
        public string text = "Type your note here";
    }JS - NoteEditor.js CustomEditor(Note);
     
        private var note : Note;
     
        private var scrollPos : Vector2;
     
        function OnInspectorGUI ()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(100.0));
            var myStyle : GUIStyle;
            myStyle.wordWrap = true;
            myStyle.stretchWidth = false;
            myStyle.normal.textColor = Color.gray;
            note.text = GUILayout.TextArea(note.text, myStyle, GUILayout.Width(200.0), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            this.Repaint();       
        }
    
    JS - Note.js public class Note extends MonoBehaviour
    {
        public var text : String = "Type your note here";
}
}
