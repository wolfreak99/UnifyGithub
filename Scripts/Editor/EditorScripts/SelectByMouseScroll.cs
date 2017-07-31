// Original url: http://wiki.unity3d.com/index.php/SelectByMouseScroll
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SelectByMouseScroll.cs
// File based on original modification date of: 28 June 2016, at 13:03. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Fredrik Ludvigsen (Steinbitglis) 
Description This script lets you select any GameObject with a renderer under the mouse cursor by scrolling. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Hold the alt button while scrolling to select one of the GameObjects under your mouse cursor. 
C# - SelectByMouseScroll.cs using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
[InitializeOnLoad]
public static class SelectByMouseScroll {
    private static Renderer[] renderers;
    private static int index;
 
    static SelectByMouseScroll() {
        HierarchyUpdate();
        EditorApplication.hierarchyWindowChanged += HierarchyUpdate;
        SceneView.onSceneGUIDelegate += HighlightUpdate;
    }
 
    private static void HierarchyUpdate() { renderers = Object.FindObjectsOfType<Renderer>(); }
 
    private static void HighlightUpdate(SceneView sceneview) {
        if (Event.current.type != EventType.ScrollWheel || !Event.current.alt)
            return;
        var mp = Event.current.mousePosition;
        var ccam = Camera.current;
        var mouseRay = ccam.ScreenPointToRay(new Vector3(mp.x, ccam.pixelHeight - mp.y, 0f));
 
        index += Event.current.delta.y >= 0f ? -1 : 1;
 
        var pointedRenderers = new List<Renderer>();
        foreach (Renderer r in renderers)
            if (r.bounds.IntersectRay(mouseRay))
                pointedRenderers.Add(r);
 
        if (pointedRenderers.Count > 0) {
            index = (index + pointedRenderers.Count) % pointedRenderers.Count;
            Selection.objects = new Object[] {pointedRenderers[index].gameObject};
            Event.current.Use();
        }
    }
}
}
