// Original url: http://wiki.unity3d.com/index.php/ReplaceSelection
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/ReplaceSelection.cs
// File based on original modification date of: 19 July 2013, at 06:36. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Dave A and yesfish 
DescriptionSelect some objects, menu "Game Object / Replace Selection" will prompt for what to replace that with. Preserves parentage, position, scale, rotation. 
UsagePlace this script in YourProject/Assets/Editor and a menu item will automatically appear in the 'Game Object' menu after it is compiled. 
C# - ReplaceSelection.cs/* This wizard will replace a selection with an object or prefab.
 * Scene objects will be cloned (destroying their prefab links).
 * Original coding by 'yesfish', nabbed from Unity Forums
 * 'keep parent' added by Dave A (also removed 'rotation' option, using localRotation
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class ReplaceSelection : ScriptableWizard
{
    static GameObject replacement = null;
    static bool keep = false;
 
    public GameObject ReplacementObject = null;
    public bool KeepOriginals = false;
 
    [MenuItem("GameObject/-Replace Selection...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard(
            "Replace Selection", typeof(ReplaceSelection), "Replace");
    }
 
    public ReplaceSelection()
    {
        ReplacementObject = replacement;
        KeepOriginals = keep;
    }
 
    void OnWizardUpdate()
    {
        replacement = ReplacementObject;
        keep = KeepOriginals;
    }
 
    void OnWizardCreate()
    {
        if (replacement == null)
            return;
 
        Undo.RegisterSceneUndo("Replace Selection");
 
        Transform[] transforms = Selection.GetTransforms(
            SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
 
        foreach (Transform t in transforms)
        {
            GameObject g;
            PrefabType pref = EditorUtility.GetPrefabType(replacement);
 
            if (pref == PrefabType.Prefab || pref == PrefabType.ModelPrefab)
            {
                g = (GameObject)EditorUtility.InstantiatePrefab(replacement);
            }
            else
            {
                g = (GameObject)Editor.Instantiate(replacement);
            }
 
            Transform gTransform = g.transform;
            gTransform.parent = t.parent;
            g.name = replacement.name;
            gTransform.localPosition = t.localPosition;
            gTransform.localScale = t.localScale;
            gTransform.localRotation = t.localRotation;
        }
 
        if (!keep)
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                GameObject.DestroyImmediate(g);
            }
        }
    }
}
}
