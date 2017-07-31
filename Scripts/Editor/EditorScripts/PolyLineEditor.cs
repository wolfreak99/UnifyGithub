// Original url: http://wiki.unity3d.com/index.php/PolyLineEditor
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/PolyLineEditor.cs
// File based on original modification date of: 6 September 2012, at 10:33. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
DescriptionHere is a an exemple of how to use the Unity Handles class and adding custom editing tools. 
 
This tool adds "inScene" controls to add, remove and align a set of points. those points could be used in any way you want (path, curve, animations ....) 

I'd like to thank Dantus from Edelweiss Interactive for 2 very useful trick : 
a way to know which handle I'm currently using 
a way to hide default handle 




UsageAdd the following class in an Editor directory 
C# using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
 
// Thanks to Dantus from Edelweiss Interactive for this trick
public class DefaultHandles
{
    public static bool Hidden
    {
        get
        {
            Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            return ((bool)field.GetValue(null));
        }
        set
        {
            Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, value);
        }
    }
}
 
[CustomEditor(typeof(PolyLine))]
public class PolyLineEditor : Editor {
 
    Vector3[] globalPos;
   // Event current;
 
    int curPointIndex = -1;
    bool hideDefaultHandle = true;
 
    void OnEnable()
    {
        DefaultHandles.Hidden = hideDefaultHandle;
    }
 
    void OnDisable()
    {
        DefaultHandles.Hidden = false;
    }
 
    void Delete(PolyLine polyLine, int index)
    {
        if (index < 0 || index >= polyLine.positions.Length)
            return;
 
        ArrayList list = new ArrayList(polyLine.positions);
        list.RemoveAt(index);
 
        polyLine.positions = (Vector3 []) list.ToArray(typeof(Vector3));
    }
 
    void insert(PolyLine polyLine, int position)
    {
 
        ArrayList list = new ArrayList(polyLine.positions);
 
 
        if (polyLine.positions.Length == 0)
        {
            list.Add(Vector3.zero);
        }
        else if (polyLine.positions.Length == 1)
        {
 
            if (position == 0)
            {
                list.Insert(0, Vector3.zero);      
            }
            else
            {
                list.Add(polyLine.positions[0] * 2);      
            }
 
        }
        else
        {
            Vector3[] newArray = new Vector3[polyLine.positions.Length+1];
            if (position == -1 ) // last
                position = polyLine.positions.Length ;
 
            if (position == 0)
                list.Insert(0, Vector3.zero);      
            else if ( position == polyLine.positions.Length)
                list.Add(2 * polyLine.positions[position - 1] - polyLine.positions[position - 2]);
            else
            {
                // between 2 points, use middle
                list.Insert(position, 0.5f * (polyLine.positions[position - 1] + polyLine.positions[position]));
            }    
        }
 
        polyLine.positions = (Vector3 []) list.ToArray(typeof(Vector3));
    }
 
    void insideSceneGUI(PolyLine polyLine)
    {
        Rect size = new Rect(0, 0, 300, 200);
        float sizeButton = 30;
        Handles.BeginGUI();
      //  Handles.BeginGUI(new Rect(Screen.width - size.width - 10, Screen.height - size.height - 10, size.width, size.height));
 
        GUI.BeginGroup(new Rect(Screen.width - size.width - 10, Screen.height - size.height - 50, size.width, size.height));
        GUI.Box(size, "PolyLine Tool Bar");
 
 
        Rect rc = new Rect(0, sizeButton, size.width, sizeButton);
 
        GUI.Label(rc, "Double Clic on Circles to select a point");
        rc.y += sizeButton;
        if (curPointIndex != -1)
        {
            GUI.Label(rc, "Current Point " + curPointIndex);
            rc.y += sizeButton;
 
            rc.width = size.width / 3;
 
 
            if (GUI.Button(rc, "Insert Before"))
            {
                insert(polyLine, curPointIndex);
            }
            rc.x += rc.width;
            if (GUI.Button(rc, "Delete"))
            {
                Delete(polyLine, curPointIndex);
            }
            rc.x += rc.width;
            if (GUI.Button(rc, "Insert After"))
            {
                insert(polyLine, curPointIndex + 1);
                curPointIndex++;
            }
 
 
        }
        else
        {
            if (polyLine.positions.Length == 0)
            {
                if (GUI.Button(rc, "Insert"))
                {
                    insert(polyLine, 0);
                }
                rc.y += sizeButton;
            }
            else
            {
                rc.width = size.width / 2;
 
                if (GUI.Button(rc, "Insert First"))
                {
                    insert(polyLine, 0);
                    curPointIndex = 0;
                }
                rc.x += rc.width;
                if (GUI.Button(rc, "Insert Last"))
                {
                    insert(polyLine, -1);
                    curPointIndex = polyLine.positions.Length - 1;
                }
                rc.y += sizeButton;
            }
        }
 
        if (polyLine.positions.Length > 0)
        {
            //rc.y += sizeButton;
            rc.width = size.width / 2;
            rc.x = 0;
            rc.y += sizeButton+10;
            if (GUI.Button(rc, "Clear All"))
            {
                polyLine.positions = new Vector3[0];
                curPointIndex = -1;
            }
            rc.x += rc.width;
            if (GUI.Button(rc, "Reset y"))
            {
                for (int i = 0; i < polyLine.positions.Length; i++)
                {
                    polyLine.positions[i].y = 0;
                }
            }
        }
 
        rc.y += sizeButton + 10;
        rc.x = 0;
        rc.width = size.width;
        if (hideDefaultHandle)
        {
            if (GUI.Button(rc, "Show Main Transform"))
            {
                hideDefaultHandle = false;
                DefaultHandles.Hidden = hideDefaultHandle;
            }
        }
        else
        {
            if (GUI.Button(rc, "Hide Main Transform"))
            {
                hideDefaultHandle = true;
                DefaultHandles.Hidden = hideDefaultHandle;
            }
        }
 
        GUI.EndGroup();
        Handles.EndGUI();
    }
 
    void OnSceneGUI () 
    {
        // Thank to Dantus from Edelweiss for this helper
        // You need them somewhere.
        Quaternion handleRotation = Quaternion.identity;
        int someHashCode = GetHashCode();
 
 
        PolyLine polyLine = (PolyLine) target;
        Transform tr = polyLine.transform;
 
        globalPos = new Vector3[polyLine.positions.Length];
 
        for (int i = 0; i < polyLine.positions.Length; i++)
        {   
            Vector3 pos = tr.TransformPoint(polyLine.positions[i]);
            Handles.Label(pos, "   " + i);
 
            float size = HandleUtility.GetHandleSize(pos);
 
            // Get the needed data before the handle
            int controlIDBeforeHandle = GUIUtility.GetControlID(someHashCode, FocusType.Passive);
            bool isEventUsedBeforeHandle = (Event.current.type == EventType.used);
 
            //
           // pos = Handles.PositionHandle(pos, Quaternion.identity);
 
            Handles.ScaleValueHandle(0, pos, Quaternion.identity, size, Handles.SphereCap, 0);
            if (curPointIndex == i)
            {
                pos = Handles.PositionHandle(pos, Quaternion.identity);
            }
 
 
            // Get the needed data after the handle
            int controlIDAfterHandle = GUIUtility.GetControlID(someHashCode, FocusType.Passive);
            bool isEventUsedByHandle = !isEventUsedBeforeHandle && (Event.current.type == EventType.used);
 
            if
             ((controlIDBeforeHandle < GUIUtility.hotControl &&
               GUIUtility.hotControl < controlIDAfterHandle) ||
               isEventUsedByHandle)
            {
                curPointIndex = i;
            }
 
            polyLine.positions[i] = tr.InverseTransformPoint(pos);
            globalPos[i] = pos;
        }
        insideSceneGUI(polyLine);
        Handles.DrawPolyLine(globalPos);
    }
 
    override public void  OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
 
}
}
