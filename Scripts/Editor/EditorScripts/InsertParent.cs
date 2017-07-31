// Original url: http://wiki.unity3d.com/index.php/InsertParent
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/InsertParent.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Neil Carter (NCarter) 
Description This script is an alternative to the built-in Make Parent command. Whereas the Make Parent command makes the first object you select the parent of subsequently selected objects, this script inserts a new GameObject which becomes the parent of all selected objects. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Select some objects in the Scene view or Hierarchy window, then choose GameObject→Insert Parent from the menu (or press control P). Depending on the nature of the selected objects, it will have one of the following effects: 
If only one object is selected... 
...and it has no parent, a new GameObject will be added, and the selected object will become a child of it. 
...and it already has a parent, a new GameObject will be inserted between the object and its original parent. 
If more than one object is selected, irrespective of whether those objects already have parents, all those objects will become children of the newly created GameObject. 
The newly created parent object is named _Parent to make it sort towards the top of the Hierarchy list. 
C# - InsertParent.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class InsertParent : ScriptableObject
{
    [MenuItem ("GameObject/Insert Parent ^p")]
    static void MenuInsertParent()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel |
            SelectionMode.OnlyUserModifiable);
 
        GameObject newParent = new GameObject("_Parent");
        Transform newParentTransform = newParent.transform;
 
        if(transforms.Length == 1)
        {
            Transform originalParent = transforms[0].parent;
            transforms[0].parent = newParentTransform;
            if(originalParent)
                newParentTransform.parent = originalParent;
        }
        else
        {
            foreach(Transform transform in transforms)
                transform.parent = newParentTransform;
        }
    }
}
}
