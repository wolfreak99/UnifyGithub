// Original url: http://wiki.unity3d.com/index.php/Mesh2Collider
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/Mesh2Collider.cs
// File based on original modification date of: 18 October 2013, at 18:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Jessy, with special thanks to Delly (potan on the forum) 
Contents [hide] 
1 Description 
2 Usage 
3 Tips for working with each type of mesh/collider in your modeling app 
3.1 Sphere 
3.2 Box 
3.3 Capsule 
4 C# - Mesh2ColliderMenu.cs 

Description 
A scene with the collider meshes in a modeling app. 
Actual colliders in Unity, after object selection and this Menu Item.The purpose of this script is to make the creation of primitive colliders faster and more accurate. It allows you to use an external 3D modeling program, instead of Unity, to make the colliders. I don't know of a system to allow Unity to import colliders directly, so with this, you create primitive-shaped meshes externally, and then the meshes are deleted via this script, in Unity, leaving colliders of the appropriate size, shape, and orientation. 
Here is some useful information about the performance of colliders. You should use colliders that do a good job of matching the shape of your meshes, but spheres yield the best performance, and capsules are a little faster than boxes. Mesh colliders are the slowest, but this script doesn't have anything to do with mesh colliders. 
UsageLike all Editor scripts, this has to be put into a folder named Editor, somewhere in your Assets folder. 
The basic gist of it is, if you have a mesh that contains the name "box", "sphere", or "capsule", in your Hierarchy selection, a primitive collider of the type that matches the name will be added to the scene, in place of the mesh. You can select as many Game Objects as you want, and anything that doesn't have a MeshFilter component, and contains one of those names, will be skipped over. The only requirement to get good results is that your mesh actually matches the shape of what it is named, and that it is oriented along one of its own axes (more information on this latter requirement in the following section). 
Tips for working with each type of mesh/collider in your modeling appAfter you create them, positioning the meshes is one thing you'll obviously need to do. There should never be any issues with that. It also won't make any difference if you move vertices directly, or scale the mesh for the same effect. You don't have to pay any mind to pivot points, either; it does not matter at all if the center of the collider mesh is not at the mesh's pivot point. (See below for more rotation and scaling tips that are pertinent to boxes and capsules.) 
It helps to use a material that allows you to see the mesh for which you are making colliders, along with the colliders themselves. I've opted to use a transparent material, with visible wireframe. 


I can only speak from using Blender, in terms of this next tip. There may be better options for other software.
Keep a file that contains only the three types of colliders, centered at (0,0,0), and with no rotation or scaling. Then, you can append in the mesh(es) that you need, into other files. After something is appended, you can just duplicate it as needed in the new file. Here is what I use. Make sure you have Blender GLSL Materials turned on, in the Game menu. Otherwise, these won't look right in Textured mode. 
SphereAlthough "box" is the first check I make in the script below, because I believe it is the most used, sphere is definitely the simplest type. 
Creation 
All you need to do is create a sphere primitive, with as many subdivisions as you like. I recommend a diameter of 1 world unit, so you can more easily match the size of other objects numerically. It helps to use a sphere type that has a circular equator; that way, you can use it to make the capsule collider. (The appropriate type is called "UVsphere" in Blender.) 
Usage 
Rotation doesn't really matter, with a sphere, but feel free to rotate it, if you like the way the resulting topology jibes with whatever you're adding it to. 
Uniformly scale it. 


BoxCreation 
Make a unit cube. 
Usage 
Only use object-level rotation. If you rotate the vertices, so that they don't form a box that is aligned with the object's axes, then it won't be oriented correctly in Unity. 
To shape the box, you can use non-uniform scaling, and/or mesh-level edits. Just make sure, if you're working directly with the faces, that you only move each face along the object-level axis to which it is perpendicular. 


CapsuleThe capsule will work just as well, oriented along the "left-right" or "front-back" axis. Feel free to orient the vertices along a different axis than "up-down", as described next, if you want. 
Creation 
Take what I recommended you make for the sphere collider. Separate the vertices at the equator. Select each hemisphere individually, move it away from the origin by half a unit, and assign the selection to a vertex group. (I called mine "top" and "bottom".) Then, create a cylinder with the same number of subdivisions as your sphere, and a height and diameter of 1 unit. Delete the two vertices that represent the centers of the circular "pies" on the top and bottom of the cylinder, leaving you with a "tube". Merge the non-manifold vertices of the equators with the non-manifold vertices of the cylinder, which should already match up exactly in space, prior to the merge. 
Usage 
As with the box, use object-level rotation only. You can, however, rotate 90 degrees along an object axis, if you like the way the axes of the collider line up better with the axes of the mesh you're using them with. (The vertex group names "top" and "bottom" from the previous paragraph probably wouldn't make sense if you do this, however.) 
Scaling can be used, but it must be uniform. Unity's capsule colliders are hemispheres with cylinders in-between. If that's not what your mesh looks like, it won't translate accurately to Unity. 
After getting the position and diameter about right, move the hemispheres of the capsule along a single object-space axis, in the same fashion as moving faces of the box collider. (This is why the vertex groups were created; it is not as fast or easy to select a hemisphere as it is to select a single face.) 
Note: if you pull the hemispheres of the capsule far enough in, that the equators touch, or go through each other, there will be no cylinder component in Unity. It will still be turned into a capsule collider, however, but it will appear as a sphere. If this is the shape you want, use a sphere collider instead. 
C# - Mesh2ColliderMenu.csClick here to download the script. 
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
 
class Mesh2ColliderMenu {
 
static List<string> primitiveNames = new List<string>(new string[]{"box", "sphere", "capsule"});
static List<Transform> selection;
 
// Grey out the menu item if there are no non-skinned meshes in the selection,
// or if no mesh names contain the name of any primitive collider type.
[MenuItem("GameObject/Create Colliders from Meshes", true)] static bool ValidateMesh2Collider () {
	selection = new List<Transform>(Selection.GetTransforms(SelectionMode.Unfiltered | SelectionMode.ExcludePrefab));	
	return selection.Exists(transform => transform.GetComponent<MeshFilter>() 
		&& primitiveNames.Exists(primitiveName => transform.name.ToLower().Contains(primitiveName))
	);
}
 
[MenuItem("GameObject/Create Colliders from Meshes")] static void Mesh2Collider () {
	Undo.RegisterSceneUndo("Create Colliders from Meshes");	
	selection.ForEach(transform => {
		MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
		if (!meshFilter) return;
 
		string name = meshFilter.name.ToLower();
		if (name.Contains("box")) ChangeMeshTo<BoxCollider>(meshFilter);
		else if (name.Contains("sphere")) ChangeMeshTo<SphereCollider>(meshFilter);
		else if (name.Contains("capsule")) ChangeMeshTo<CapsuleCollider>(meshFilter);
	});
}
 
static void ChangeMeshTo <T> (MeshFilter meshFilter) where T : Collider {
	T collider = meshFilter.GetComponent<T>();
	if (collider) Object.DestroyImmediate(collider);
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
		    } else capsuleCollider.radius = meshBounds.extents[xyOrZ];
	}
 
	Object.DestroyImmediate(meshFilter.renderer);
	Object.DestroyImmediate(meshFilter);
}
 
}
}
