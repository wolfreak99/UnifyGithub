// Original url: http://wiki.unity3d.com/index.php/AddParent
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/AddParent.cs
// File based on original modification date of: 26 October 2013, at 11:51. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Neil Carter (NCarter) 
Description Parents selected objects under new GameObject. Its useful for very fast reorganization of scene content. 
Usage You must place the script in a folder named "Editor" in your project's Assets folder. 
It will show in GameObject → +AddParent 
The newly created object is named "_Parent" 
AddParent.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class AddParent : ScriptableObject
{
    [MenuItem ("GameObject/+Add Parent")]
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
