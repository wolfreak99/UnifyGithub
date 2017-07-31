// Original url: http://wiki.unity3d.com/index.php/FakeGizmoDemo
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/FakeGizmoDemo.cs
// File based on original modification date of: 14 March 2010, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
Introduction This is a simple demo package that demonstrates two things: creating a fake Runtime Gizmo, and procedural creation of an in-game mesh object. 
The fake Gizmo is a three-arrow axis object that can be attached to an in-Hierarchy object. By clicking on one of the arrows, the Gizmo moves itself and the parent object in that axis direction. Note - it's not mouse-draggable as such, just holding the mouse down generates the movement. 
To use it, you would drag/drop the script, s_GizmoCreate.cs onto your GameObject. This script procedurally creates the actual Gizmo, which is built by two other scripts, s_Gizmo.cs and s_GizmoClick.cs. 
To use the Procedural Mesh creation code, I first created an arrow in Blender, exported it to a .fbx, then wrote a Python script to convert all the vertex/triangle numbers into a C# statement like new Vector3 (0.01, 0.02, 0.03). The arrow looked fine when I imported the .fbx file straight into Unity - I'm not sure why the vertex/triangle list shows up as glitchy. :) 
The Unity package can be downloaded from Media:FakeGizmoDemo.zip 
What it looks like:  
}
