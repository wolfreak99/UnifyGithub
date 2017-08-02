/*************************
 * Original url: http://wiki.unity3d.com/index.php/SceneViewObjectWindow
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/SceneViewObjectWindow.cs
 * File based on original modification date of: 5 December 2012, at 05:16. 
 *
 * Description 
 *   
 * Install 
 *   
 * Usage 
 *   
 * Code 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    DescriptionThis is a tool window which allows you to pick objects in the sceneview and provide a drag-source for drag&drop operations 
     
    InstallPlace the script in "Assets/editor/" and name the file "SceneViewObjectWindow.cs". 
    UsageOpen the window by clicking "Open SceneView Object Selector" in the Tools menu at the top. You might want to dock the tab somewhere near the inspector for easy drag&drop. 
    To pick an object put the mouse over an object in the sceneview and press the hotkey "ALT + S". The picked object appears now in the editorwindow and can be dragged from there. 
    To change the hotkey you have to replace the "%s" in the menuitem with your desired hotkey. See MenuItem for more information. 
    Codeusing UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
     
    public class SceneViewObjectWindow : EditorWindow
    {
        static GameObject m_LastObject;    
        static List<GameObject> m_Stack = new List<GameObject>();
     
        static bool m_UseStack = false;
        static float m_MaxStackSize = 5;
     
        [MenuItem("Tools/Open SceneView Object Selector")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<SceneViewObjectWindow>();
        }
     
        [MenuItem("Tools/Select Scene Object &s")]
        public static void SelectObject()
        {
            if (m_LastObject != null)
            {
                if (!m_UseStack)
                    m_Stack.Clear();
                m_Stack.Add(m_LastObject);
            }
            EditorWindow.GetWindow<SceneViewObjectWindow>().Repaint();
        }
     
        void OnGUI()
        {
            Event e = Event.current;
            m_UseStack = GUILayout.Toggle(m_UseStack,"Use Stack");
            if (m_UseStack)
                m_MaxStackSize = GUILayout.HorizontalSlider(m_MaxStackSize,1,20);
     
            for(int i = m_Stack.Count-1;i>=0;i--)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(m_Stack[i],typeof(GameObject));
                if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.type == EventType.MouseDrag)
                {
                    DragAndDrop.PrepareStartDrag ();
                    DragAndDrop.objectReferences = new UnityEngine.Object[] {m_Stack[i]};
                    DragAndDrop.StartDrag ("drag");
                    Event.current.Use();
                }
                if(GUILayout.Button("X",GUILayout.Width(20)))
                {
                    m_Stack.RemoveAt(i);
                    Repaint();
                }
                GUILayout.EndHorizontal();
                if (e.type == EventType.Repaint && m_Stack[i] == null)
                {
                    m_Stack.RemoveAt(i);
                    Repaint();
                }
            }
            if (m_UseStack && e.type == EventType.Repaint)
            {
                while(m_Stack.Count > m_MaxStackSize)
                    m_Stack.RemoveAt(0);
            }
        }
     
        [DrawGizmo(GizmoType.NotSelected)]
        static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            if (Event.current == null)
                return;
            Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit))
            {
                m_LastObject = hit.transform.gameObject;
            }
        }
    }
}
