// Original url: http://wiki.unity3d.com/index.php/EdgeAlignerTool
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/EdgeAlignerTool.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Article Original Author: Dreamblur 
EdgeAlignerToolEditorWindow.cs v.1.0 Original Script Author: Dreamblur 
Contents [hide] 
1 Description 
2 Usage 
3 Functionality 
4 Limitation 
5 EdgeAlignerToolEditorWindow.cs 

Description EdgeAlignerTool is a tool which aligns one or more edges of the AABB of a selected game object to those of the AABB of another selected game object. It is similar to the Align tool found in 3ds Max. 
Usage Place the EdgeAlignerToolEditorWindow script in your project's Editor folder. A new sub-menu named Specialized Tools will appear in Unity's menu bar. The Edge Aligner Tool may be accessed from within this sub-menu. 
Functionality This tool aligns a point in the AABB of one game object with a point in the AABB of another game object. Grouped objects (parented game objects) can be aligned and be aligned with. In such cases, the AABB of the group is derived from the combined AABB of the parent object and all its children. 
Limitation There must be at least one skinned mesh renderer or mesh renderer attached to the selected game objects (or any of their children). 
There cannot be both a skinned mesh renderer and a mesh renderer attached to the same game object among the selected game objects. 
A selected game object cannot be a prefab -- an instance of a prefab is accepted. 
EdgeAlignerToolEditorWindow.cs using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
 
public class EdgeAlignerToolEditorWindow : EditorWindow
{
    public enum ENUM_PosInBounds
    {
        Min = 0,
        Center = 1,
        Max = 2
    }
 
    private static GameObject objectToAlignGameObject;
    private static ENUM_PosInBounds enumPosInBoundsToAlign;
    private static bool bAlignAlongXAxis = true;
    private static bool bAlignAlongYAxis = true;
    private static bool bAlignAlongZAxis = true;
 
    private static GameObject objectToAlignWithGameObject;
    private static ENUM_PosInBounds enumPosInBoundsToAlignWith;
 
    private static Transform[] objectToAlignTransforms;
    private static Transform[] objectToAlignWithTransforms;
 
    private static List<Renderer> objectToAlignRenderers = new List<Renderer>(0);
    private static List<Renderer> objectToAlignWithRenderers = new List<Renderer>(0);
 
    private static Bounds boundsToAlignWith;
    private static Vector3[] posInBoundsToAlignWith = new Vector3[3];
 
    private static Bounds boundsToAlign;
    private static Vector3[] posInBoundsToAlign = new Vector3[3];
 
    [MenuItem ("Specialized Tools/Edge Aligner Tool")]
    private static void Init()
    {
        EdgeAlignerToolEditorWindow edgeAlignerToolWindow = (EdgeAlignerToolEditorWindow)EditorWindow.GetWindow(typeof(EdgeAlignerToolEditorWindow));
        edgeAlignerToolWindow.position = new Rect(20, 20, 500, 500);
    }
 
    private void OnDestroy()
    {
        objectToAlignRenderers.Clear();
        objectToAlignRenderers.TrimExcess();
        objectToAlignWithRenderers.Clear();
        objectToAlignWithRenderers.TrimExcess();
        objectToAlignTransforms = null;
        objectToAlignWithTransforms = null;
        objectToAlignGameObject = null;
        objectToAlignWithGameObject = null;
    }
 
    private void OnGUI()
    {
        objectToAlignGameObject = (GameObject)EditorGUILayout.ObjectField("Align this object:", objectToAlignGameObject, typeof(GameObject));
 
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Position In Bounds", GUILayout.Width(125));
        enumPosInBoundsToAlign = (ENUM_PosInBounds)EditorGUILayout.EnumPopup(enumPosInBoundsToAlign);
        EditorGUILayout.EndHorizontal();
 
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Axis To Align", GUILayout.Width(125));
        bAlignAlongXAxis = GUILayout.Toggle(bAlignAlongXAxis, "X", GUILayout.Width(50));
        bAlignAlongYAxis = GUILayout.Toggle(bAlignAlongYAxis, "Y", GUILayout.Width(50));
        bAlignAlongZAxis = GUILayout.Toggle(bAlignAlongZAxis, "Z", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
 
        EditorGUILayout.Separator();
 
        objectToAlignWithGameObject = (GameObject)EditorGUILayout.ObjectField("To this object:", objectToAlignWithGameObject, typeof(GameObject));
 
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Position In Bounds", GUILayout.Width(125));
        enumPosInBoundsToAlignWith = (ENUM_PosInBounds)EditorGUILayout.EnumPopup(enumPosInBoundsToAlignWith);
        EditorGUILayout.EndHorizontal();
 
        EditorGUILayout.Separator();
 
        if(GUILayout.Button("ALIGN"))
        {
            if(ValidateSelections())
            {
                InitCalculableBounds();
                AlignEdge();
            }
        }
    }
 
    private bool ValidateSelections()
    {
        if(!objectToAlignGameObject)
        {
            ShowNotification(new GUIContent("Please select a game object to align."));
            return false;
        }
        else if((EditorUtility.GetPrefabType(objectToAlignGameObject) == PrefabType.Prefab) || (EditorUtility.GetPrefabType(objectToAlignGameObject) == PrefabType.ModelPrefab))
        {
            ShowNotification(new GUIContent("The selected game object to align cannot be a prefab."));
            return false;
        }
        else if(!objectToAlignWithGameObject)
        {
            ShowNotification(new GUIContent("Please select a game object to align with."));
            return false;
        }
        else if((EditorUtility.GetPrefabType(objectToAlignWithGameObject) == PrefabType.Prefab) || (EditorUtility.GetPrefabType(objectToAlignWithGameObject) == PrefabType.ModelPrefab))
        {
            ShowNotification(new GUIContent("The selected game object to align with cannot be a prefab."));
            return false;
        }
        else if(objectToAlignGameObject == objectToAlignWithGameObject)
        {
            ShowNotification(new GUIContent("The selected game objects are the same game object."));
            return false;
        }
        else if(!bAlignAlongXAxis && !bAlignAlongYAxis && !bAlignAlongZAxis)
        {
            ShowNotification(new GUIContent("Please select at least one axis to align along."));
            return false;
        }
        else if(DetIfGameObjectHasDuplicateRenderers(objectToAlignGameObject, ref objectToAlignTransforms))
        {
            ShowNotification(new GUIContent("The selected game object to align has both a mesh renderer and a skinned mesh renderer attached to it or one of its children."));
            return false;
        }
        else if(DetIfGameObjectHasDuplicateRenderers(objectToAlignWithGameObject, ref objectToAlignWithTransforms))
        {
            ShowNotification(new GUIContent("The selected game object to align with has both a mesh renderer and a skinned mesh renderer attached to it or one of its children."));
            return false;
        }
        else if(!DetIfGameObjectHasRenderer(objectToAlignGameObject, ref objectToAlignTransforms, ref objectToAlignRenderers))
        {
            ShowNotification(new GUIContent("The selected game object to align has neither a mesh renderer nor a skinned mesh renderer attached to it or any of its children."));
            return false;
        }
        else if(!DetIfGameObjectHasRenderer(objectToAlignWithGameObject, ref objectToAlignWithTransforms, ref objectToAlignWithRenderers))
        {
            ShowNotification(new GUIContent("The selected game object to align with has neither a mesh renderer nor a skinned mesh renderer attached to it or any of its children."));
            return false;
        }
        else
        {
            return true;
        }
    }
 
    private bool DetIfGameObjectHasDuplicateRenderers(GameObject gameObject, ref Transform[] transforms)
    {
        transforms = gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform transform in transforms)
        {
            if(transform.GetComponent<MeshRenderer>() && transform.GetComponent<SkinnedMeshRenderer>())
            {
                return true;
            }
        }
 
        return false;
    }
 
    private bool DetIfGameObjectHasRenderer(GameObject gameObject, ref Transform[] transforms, ref List<Renderer> renderers)
    {
        Renderer skinnedMeshRenderer, meshRenderer;
        foreach(Transform transform in transforms)
        {
            skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
            meshRenderer = transform.GetComponent<MeshRenderer>();
            renderers.Add(skinnedMeshRenderer != null ? skinnedMeshRenderer : meshRenderer);
        }
        renderers.RemoveAll(IsNull);
 
        if(renderers.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
 
    private void InitCalculableBounds()
    {
        boundsToAlign = objectToAlignRenderers[0].bounds;
        foreach(Renderer renderer in objectToAlignRenderers)
        {
            boundsToAlign.Encapsulate(renderer.bounds);
        }
        posInBoundsToAlign[0] = boundsToAlign.min;
        posInBoundsToAlign[1] = boundsToAlign.center;
        posInBoundsToAlign[2] = boundsToAlign.max;
 
        boundsToAlignWith = objectToAlignWithRenderers[0].bounds;
        foreach(Renderer renderer in objectToAlignWithRenderers)
        {
            boundsToAlignWith.Encapsulate(renderer.bounds);
        }
        posInBoundsToAlignWith[0] = boundsToAlignWith.min;
        posInBoundsToAlignWith[1] = boundsToAlignWith.center;
        posInBoundsToAlignWith[2] = boundsToAlignWith.max;
    }
 
    private void AlignEdge()
    {
        if(bAlignAlongXAxis)
        {
            posInBoundsToAlign[(int)enumPosInBoundsToAlign].x = posInBoundsToAlignWith[(int)enumPosInBoundsToAlignWith].x;
        }
        if(bAlignAlongYAxis)
        {
            posInBoundsToAlign[(int)enumPosInBoundsToAlign].y = posInBoundsToAlignWith[(int)enumPosInBoundsToAlignWith].y;
        }
        if(bAlignAlongZAxis)
        {
            posInBoundsToAlign[(int)enumPosInBoundsToAlign].z = posInBoundsToAlignWith[(int)enumPosInBoundsToAlignWith].z;
        }
 
        switch(enumPosInBoundsToAlign)
        {
            case ENUM_PosInBounds.Min:
            {
                objectToAlignGameObject.transform.position = posInBoundsToAlign[0] + boundsToAlign.extents;
                break;
            }
            case ENUM_PosInBounds.Center:
            {
                objectToAlignGameObject.transform.position = posInBoundsToAlign[1];
                break;
            }
            case ENUM_PosInBounds.Max:
            {
                objectToAlignGameObject.transform.position = posInBoundsToAlign[2] - boundsToAlign.extents;
                break;
            }
        }
    }
 
    private bool IsNull(Renderer renderer)
    {
        if(!renderer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
}
