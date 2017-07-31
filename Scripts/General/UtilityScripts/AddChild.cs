// Original url: http://wiki.unity3d.com/index.php/AddChild
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/AddChild.cs
// File based on original modification date of: 1 June 2012, at 14:17. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Author: Neil Carter (NCarter) 
Description Adds an empty GameObject as a child of each selected object. This may sometimes be preferable to the built-in GameObject→Create Empty command, as it saves you from having to hunt in the hierarchy for the newly created object. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Select some objects in the Scene view or Hierarchy window, then choose GameObject→+Add Child from the menu (or press control N). Each selected object will have a new child object added to it. 
The newly created child objects are named _null to make them sort towards the top of the Hierarchy list. 
C# - AddChild.cs using UnityEngine;
using UnityEngine;
using UnityEditor;
using System.Collections;
 
 
public class AddChild : ScriptableObject
{
 
    [MenuItem ("GameObject/+Add Child")]
    static void MenuAddChild()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
 
        foreach(Transform transform in transforms)
        {
            GameObject newChild = new GameObject("_null");
            newChild.transform.parent = transform;
		newChild.transform.localPosition = Vector3.zero;
        }
 
    }
 
}
}
