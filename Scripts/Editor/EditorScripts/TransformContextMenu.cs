// Original url: http://wiki.unity3d.com/index.php/TransformContextMenu
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/TransformContextMenu.cs
// File based on original modification date of: 12 January 2013, at 00:09. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Zach Aikman (Zach Aikman) 
DescriptionMy goal was to add copy/paste functionality to the Transform component and to make it as intuitive as possible to use. Thankfully, Unity provides a method of adding context menus to specific component types, including the mandatory Transform. 
Transform data can be copy/pasted as a whole, or you can choose to copy/paste the position, rotation or scale individually. 
UsageThis script need to be placed in a folder called "Editor" in the root of your project's Assets folder -> Assets/Editor/TransformContextMenu.cs. 
Right click on a Transform component to see the context menu of copy/paste operations. 
C# - TransformContextMenu.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
/// <summary>
/// Extends the Transform context menu to provide support for copying, pasting
/// and pushing position/rotation/scale data around.
/// 
/// Code by Zach Aikman - zachaikman@gmail.com
/// </summary>
public class TransformContextMenu
{
    private class TransformClipboard
    {
        public Vector3      position;
        public Quaternion   rotation;
        public Vector3      scale;
 
        public bool isPositionSet = false;
        public bool isRotationSet = false;
        public bool isScaleSet = false;
    }
 
    private static TransformClipboard clipboard = new TransformClipboard();
 
    #region Copy Methods
    [MenuItem("CONTEXT/Transform/Copy Transform", false, 150)]
    static void CopyTransform()
    {
        CopyPosition();
        CopyRotation();
        CopyScale();
    }
 
    [MenuItem("CONTEXT/Transform/Copy Position", false, 151)]
    static void CopyPosition()
    {
        clipboard.position = Selection.activeTransform.localPosition;
        clipboard.isPositionSet = true;
    }
 
    [MenuItem("CONTEXT/Transform/Copy Rotation", false, 152)]
    static void CopyRotation()
    {
        clipboard.rotation = Selection.activeTransform.localRotation;
        clipboard.isRotationSet = true;
    }
 
    [MenuItem("CONTEXT/Transform/Copy Scale", false, 153)]
    static void CopyScale()
    {
        clipboard.scale = Selection.activeTransform.localScale;
        clipboard.isScaleSet = true;
    }
    #endregion
 
    #region Paste Methods
    [MenuItem("CONTEXT/Transform/Paste Transform", false, 200)]
    static void PasteTransform()
    {
        PastePosition();
        PasteRotation();
        PasteScale();
    }
 
    [MenuItem("CONTEXT/Transform/Paste Position", false, 201)]
    static void PastePosition()
    {
        Undo.RegisterUndo(Selection.activeTransform, "Paste Position");
        Selection.activeTransform.localPosition = clipboard.position;
    }
 
    [MenuItem("CONTEXT/Transform/Paste Rotation", false, 202)]
    static void PasteRotation()
    {
        Undo.RegisterUndo(Selection.activeTransform, "Paste Rotation");
        Selection.activeTransform.localRotation = clipboard.rotation;
    }
 
    [MenuItem("CONTEXT/Transform/Paste Scale", false, 203)]
    static void PasteScale()
    {
        Undo.RegisterUndo(Selection.activeTransform, "Paste Scale");
        Selection.activeTransform.localScale = clipboard.scale;
    }
    #endregion
 
    #region Validation
    [MenuItem("CONTEXT/Transform/Paste Transform", true)]
    static bool ValidatePasteTransform()
    {
        return ValidatePastePosition() && ValidatePasteRotation() && ValidatePasteScale();
    }
 
    [MenuItem("CONTEXT/Transform/Paste Position", true)]
    static bool ValidatePastePosition()
    {
        return clipboard.isPositionSet;
    }
 
    [MenuItem("CONTEXT/Transform/Paste Rotation", true)]
    static bool ValidatePasteRotation()
    {
        return clipboard.isRotationSet;
    }
 
    [MenuItem("CONTEXT/Transform/Paste Scale", true)]
    static bool ValidatePasteScale()
    {
        return clipboard.isScaleSet;
    }
 
    [MenuItem("CONTEXT/Transform/Push To Children", true)]
    static bool ValidatePushToChildren()
    {
        return Selection.activeTransform.childCount > 0;
    }
 
    [MenuItem("CONTEXT/Transform/Push To Parent", true)]
    static bool ValidatePushToParent()
    {
        return Selection.activeTransform.parent != null;
    }
    #endregion
}
}
