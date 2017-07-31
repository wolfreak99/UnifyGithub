// Original url: http://wiki.unity3d.com/index.php/UUniCG
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/UUniCG.cs
// File based on original modification date of: 19 June 2014, at 20:25. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Based on the Mesh2Collider script by Jessy. 
Contents [hide] 
1 Description 
2 Installation 
3 Usage basics 
3.1 In depth tutorial 
4 Model preparation 
4.1 Examples 
5 C# - UUniCG.cs 

Description 
A few meshes before and after the generation.Much like the script this extension is based on, it allows you to generate colliders automatically. Unlike the original script however, this script was designed to allow the generation of colliders for entire, complex scenes. As such it handles not only box-, sphere- and capsule-colliders, but also mesh-colliders and can automatically turn them into triggers. It can go through the entire parenting hierarchy recursively and, if you wish, preserve the mesh renderers. 
The question why we'd want to do that is answered thoroughly here: Mesh2Collider 
It is highly recommended to at least read through the "Tips for working with each type of mesh/collider in your modeling app" section. This page will only cover the usage of this particular script. 
InstallationPlace the script in the Editor folder in your Assets directory. 
Usage basics 
The user interface.After selecting at least one GameObject from your hierarchy the "Windows/Collider Generation" option will become available. The UI should be pretty self-explanatory. You select the types of colliders you want to generate, whether the script should handle triggers, remove the mesh renderers (and filters), run recursively and runs in write-mode. 
The last option is the most important one. When write-mode is disabled, nothing will happen when you press the button to start the generation. The question why it's there in the first place answered itself upon enabling it. 
After changing all the settings to your liking just hit the button and the script will automatically do the work for you. For this to work however, it is required that the script can detect which collider to generate and whether it is supposed to be a trigger or not. This is covered in the "Model preparation" section. 
The pattern that is used is shown in form of a regular expression. This was already put in place for the future, where I want to add more complex keywords that allow a greater degree of control (like overriding the "Remove mesh renderer" option or specifying layers/tags). 
In depth tutorialLevel-Creation using UUniCG, Blender and SketchUp 
Model preparationThe original Mesh2Collider script used the mesh name to determine which collider to generate. We do it the same way here, but with different keywords since the old ones could cause problems. Say you have a boxing game and the enemy would be called "Boxer". The original script would turn the name into a lowercase string and compare it to the primitive names. In this case the enemy would have its meshrenderer and filter removed, and a box-collider added. For a script that is meant to generate colliders for all objects in entire scenes this is unacceptable. Keywords like "box" or "mesh" are far too generic and long. 
This script compares the name without turning it into a lowercase string and expects a "--", followed by a letter describing the mesh/option. A full list can be found below: 
Keyword: Meaning: 
--B Box-collider 
--S Sphere-collider 
--C Capsule-collider 
--MMesh-collider 
--T Collider is trigger 

This list is ordered. If there are multiple matches, only the first one is used 
ExamplesName: Result: 
door_generic_--B_001a Box-collider 
door_generic_--b_001a Nothing 
door_trigger_--T Nothing 
door_trigger_--B_banana_#T Box-collider as trigger 
freshprince_--S_--B Sphere-collider 
C# - UUniCG.cs// Yes, this code wasn't commended yet. I'll do this soon; I promise. :)
 
using UnityEngine;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
 
public class UUniCG : EditorWindow {
    private static List<Transform> selection;
 
    [MenuItem("Window/Collider Generation", true)]
    public static bool EnableWindow() {
        selection = new List<Transform>(Selection.GetTransforms(SelectionMode.Unfiltered | SelectionMode.ExcludePrefab));
        if (selection.Count > 0)
            return true;
        else
            return false;
    }
 
    [MenuItem("Window/Collider Generation")]
    public static void ShowWindow() {
        Mesh2Collider.selection = selection;
        EditorWindow.GetWindow<UUniCG>();
    }
 
    bool[] colliderTypes = { true, true, true, true };
    bool[] options = { true, true, false, false };
 
    private void OnGUI() {
        GUILayout.Label("Collider types:", EditorStyles.boldLabel);
 
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Primitives:");
 
            GUILayout.BeginVertical();
            {
                colliderTypes[0] = GUILayout.Toggle(colliderTypes[0], "Box");
                colliderTypes[1] = GUILayout.Toggle(colliderTypes[1], "Sphere");
                colliderTypes[2] = GUILayout.Toggle(colliderTypes[2], "Capsule");
            }
            GUILayout.EndVertical();
            GUILayout.Label("Complex:");
            colliderTypes[3] = GUILayout.Toggle(colliderTypes[3], "Mesh");
        }
        GUILayout.EndHorizontal();
 
        GUILayout.Space(20.0f);
 
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Options:", EditorStyles.boldLabel);
            GUILayout.BeginVertical();
            {
                options[0] = GUILayout.Toggle(options[0], "Generate triggers");
                options[1] = GUILayout.Toggle(options[1], "Remove mesh renderers");
                options[2] = GUILayout.Toggle(options[2], "Enable recursive mode");
                options[3] = GUILayout.Toggle(options[3], "Enable write mode");
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
 
        GUILayout.Space(20.0f);
        if (options[3]) {
            GUI.backgroundColor = Color.red;
            GUILayout.Box("WARNING:\nWrite mode is enabled. Operations cannot be undone! Work with a copy or make sure you have a backup!");
            GUI.backgroundColor = Color.white;
        }
 
        GUI.enabled = !string.IsNullOrEmpty(GenerateRegEx());
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Pattern:", EditorStyles.boldLabel);
            GUILayout.Label(GenerateRegEx(), EditorStyles.largeLabel);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20.0f);
 
        if (GUILayout.Button("Do black magic")) {
            if (options[3]) {
                Mesh2Collider.GenerateColliders(colliderTypes[0], colliderTypes[1], colliderTypes[2], colliderTypes[3], options[0], options[1], options[2]);
            }
        }
    }
 
    private string GenerateRegEx() {
        string regularExpression = "*\u2218--\u2218";
 
        string settings = "(";
        for (int i = 0; i < colliderTypes.Length; i++) {
            if (colliderTypes[i]) {
                switch (i) {
                    case 0:
                        settings += "B+";
                        break;
                    case 1:
                        settings += "S+";
                        break;
                    case 2:
                        settings += "C+";
                        break;
                    case 3:
                        settings += "M";
                        break;
                    default:
                        break;
                }
            }
        }
        if (settings.Equals("("))
            return "";
 
        if (settings.EndsWith("+")) settings = settings.Remove(settings.Length - 1);
        settings += ")\u2218*";
 
 
        if (options[0]) {
            settings += "\u2218(--T)?\u2218*";
        }
 
        return regularExpression + settings;
    }
 
 
    class Mesh2Collider {
        public static List<Transform> selection;
        static List<Transform> selectionRecursive;
 
 
        public static void GenerateColliders(bool box, bool sphere, bool capsule, bool mesh, bool trigger, bool removeRenderers, bool recursive) {
            if (recursive) {
                selectionRecursive = new List<Transform>();
 
                foreach (Transform t in selection) {
                    SearchForMeshes(t);
                }
 
                selection = selectionRecursive;
            }
 
            selection.ForEach(transform => {
                MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
                if (!meshFilter)
                    return;
 
                string name = meshFilter.name;
                if (box && name.Contains("--B")) ChangeMeshTo<BoxCollider>(meshFilter, removeRenderers, trigger && name.Contains("--T"));
                else if (box && name.Contains("--S")) ChangeMeshTo<SphereCollider>(meshFilter, removeRenderers, trigger && name.Contains("--T"));
                else if (box && name.Contains("--C")) ChangeMeshTo<CapsuleCollider>(meshFilter, removeRenderers, trigger && name.Contains("--T"));
                else if (box && name.Contains("--M")) ChangeMeshTo<MeshCollider>(meshFilter, removeRenderers, trigger && name.Contains("--T"));
            });
        }
 
        static void SearchForMeshes(Transform input) {
            Transform[] t_child = input.GetComponentsInChildren<Transform>();
 
            if (t_child.Length > 0) {
                foreach (Transform child_transform in t_child) {
                    selectionRecursive.Add(child_transform);
                }
            }
        }
 
        static void ChangeMeshTo<T>(MeshFilter meshFilter, bool removeRenderer, bool trigger) where T : Collider {
            T collider = meshFilter.GetComponent<T>();
            if (collider)
                Object.DestroyImmediate(collider);
            collider = meshFilter.gameObject.AddComponent<T>();
 
            // The capsule is not created as nicely as the other types of colliders are.
            // Instead of actually being a capsule, AddComponent(typeof(CapsuleCollider))
            // creates a sphere that fully encompases the mesh.
            // I suggested to Unity Technologies, on April 1, 2010, via the Bug Reporter,
            // that they implement automatic orientation, similar to what follows,
            // so a workaround like this can be avoided.
            if (typeof(T) == typeof(CapsuleCollider)) {
                CapsuleCollider capsuleCollider = collider as CapsuleCollider;
                Bounds meshBounds = meshFilter.sharedMesh.bounds;
                for (int xyOrZ = 0; xyOrZ < 3; ++xyOrZ)
                    if (meshBounds.size[xyOrZ] > capsuleCollider.height) {
                        capsuleCollider.direction = xyOrZ;
                        capsuleCollider.height = meshBounds.size[xyOrZ];
                    }
                    else capsuleCollider.radius = meshBounds.extents[xyOrZ];
            }
            else if (typeof(T) == typeof(MeshCollider)) {
                MeshCollider meshcollider = collider as MeshCollider;
                meshcollider.sharedMesh = meshFilter.sharedMesh;
            }
 
            if (trigger)
                collider.isTrigger = true;
 
            if (removeRenderer) {
                Object.DestroyImmediate(meshFilter.renderer);
                Object.DestroyImmediate(meshFilter);
            }
        }
    }
}
}
