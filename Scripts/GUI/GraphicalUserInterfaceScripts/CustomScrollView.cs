// Original url: http://wiki.unity3d.com/index.php/CustomScrollView
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/CustomScrollView.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
DescriptionThis is just a custom implementation of a ScrollView that works like GUI.BeginScrollView but you have full control whether the scrollbars are shown or not. 
UsageAlmost like the original function: 
    pos = GUIX.BeginScrollView(new Rect(10,10,100,100), pos, new Rect(0,0,1000,1000), true, true);
    // content here
    GUIX.EndScrollView();
 
    // or with custom styles
    pos = GUIX.BeginScrollView(new Rect(10,10,100,100), pos, new Rect(0,0,1000,1000), true, true,GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);If you have already a static tool class for your custom GUI stuff just add the 3 methods 


Codeusing UnityEngine;
using System;
using System.Collections.Generic;
 
public static class GUIX
{
    public static Vector2 BeginScrollView(
        Rect position,
        Vector2 scrollPosition,
        Rect contentRect,
        bool useHorizontal,
        bool useVertical,
        GUIStyle hStyle,
        GUIStyle vStyle)
    {
 
        Vector2 scrollbarSize = new Vector2(hStyle.CalcSize(GUIContent.none).y,vStyle.CalcSize(GUIContent.none).x);
        Rect viewArea = position;
        if (useHorizontal)
            viewArea.height -= scrollbarSize.x;
        if (useVertical)
            viewArea.width -= scrollbarSize.y;
        if (useHorizontal)
        {
            Rect hScrRect = new Rect(position.x, position.y + viewArea.height, viewArea.width, scrollbarSize.x);
            scrollPosition.x = GUI.HorizontalScrollbar(hScrRect,scrollPosition.x,viewArea.width,0,contentRect.width);
        }
        if (useVertical)
        {
            Rect vScrRect = new Rect(position.x + viewArea.width, position.y, scrollbarSize.y, viewArea.height);
            scrollPosition.y = GUI.VerticalScrollbar(vScrRect,scrollPosition.y,viewArea.height,0,contentRect.height);
        }
        GUI.BeginGroup(viewArea);
        contentRect.x = -scrollPosition.x;
        contentRect.y = -scrollPosition.y;
        GUI.BeginGroup(contentRect);
        return scrollPosition;
    }
 
    public static Vector2 BeginScrollView(
        Rect position,
        Vector2 scrollPosition,
        Rect contentRect,
        bool useHorizontal,
        bool useVertical)
    {
        return BeginScrollView(position, scrollPosition, contentRect, useHorizontal, useVertical, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);
    }
 
    public static void EndScrollView()
    {
        GUI.EndGroup();
        GUI.EndGroup();
    }
}
}
