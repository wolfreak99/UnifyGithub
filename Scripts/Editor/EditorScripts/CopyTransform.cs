// Original url: http://wiki.unity3d.com/index.php/CopyTransform
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CopyTransform.cs
// File based on original modification date of: 2 May 2017, at 20:11. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Changelog: 
v1.0 [Steve Allison-Bunnell / aarku] 
   - script created
v1.1 [AndrewRaphaelLukasik] 
   - Paste transform options broken into categories
   - Swap Y and Z Scale added (useful when dealing with Y-Z axis issues)
   - localRotation+90 added
v1.2 [Nicolas Chicunque] 
   - Edit Undo (ctrl + Z) options added 
   - Changed MenuItem attribute
   - Changed path from Window/ to CONTEXT/Transform
DescriptionAllows you to copy the local position, rotation, and scale of the currently selected object in the scene. Then you can paste this transform information into another object. Useful for positioning new prefabs in the location of a different prefab if you want to swap them. 
UsageYou must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Select an object in the scene hierarchy, then click gear of Transform component and select Copy Independent Values. This copies the local position, rotation, and scale of the selected object. Then select another object and click gear of Transform component and select Paste (option). This will apply the copied transform to the new object. Both objects must either be at the root of the scene hierarchy or within the same parent for the transform to be applied properly. 
File:TransformCopier.zip 

 
C# - TransformCopier.cs // TransformCopier.cs v 1.2
// homepage: http://wiki.unity3d.com/index.php/CopyTransform
 
using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class TransformCopier : ScriptableObject {
 
	private static Vector3 position;
	private static Quaternion rotation;
	private static Vector3 scale;
 
	[MenuItem("CONTEXT/Transform/Copy Independent Values",false,151)]
	static void DoRecord () {
		position = Selection.activeTransform.localPosition;
		rotation = Selection.activeTransform.localRotation;
		scale = Selection.activeTransform.localScale;
	}
 
	// PASTE POSITION:
	[MenuItem ("CONTEXT/Transform/Paste Position",false,200)]
	static void DoApplyPositionXYZ () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections) {
			Undo.RecordObject(selection, "Paste Position" + selection.name);
			selection.localPosition = position;
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Position X",false,201)]
	static void DoApplyPositionX () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Position X" + selection.name);
			selection.localPosition = new Vector3(position.x, selection.localPosition.y, selection.localPosition.z);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Position Y",false,202)]
	static void DoApplyPositionY () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Position Y" + selection.name);
			selection.localPosition = new Vector3(selection.localPosition.x, position.y, selection.localPosition.z);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Position Z",false,203)]
	static void DoApplyPositionZ () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Position Z" + selection.name);
			selection.localPosition = new Vector3(selection.localPosition.x, selection.localPosition.y, position.z);
		}
	}
 
	// PASTE ROTATION:
	[MenuItem ("CONTEXT/Transform/Paste Rotation",false,250)]
	static void DoApplyRotationXYZ () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Rotation" + selection.name);
			selection.localRotation = rotation;
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Rotation X",false,251)]
	static void DoApplyRotationX () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Rotation X" + selection.name);
			selection.localRotation = Quaternion.Euler(rotation.eulerAngles.x, selection.localRotation.eulerAngles.y, selection.localRotation.eulerAngles.z);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Rotation Y",false,252)]
	static void DoApplyRotationY () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Rotation Y" + selection.name);
			selection.localRotation = Quaternion.Euler(selection.localRotation.eulerAngles.x, rotation.eulerAngles.y, selection.localRotation.eulerAngles.z);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Rotation Z",false,253)]
	static void DoApplyRotationZ () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Rotation Z" + selection.name);
			selection.localRotation = Quaternion.Euler(selection.localRotation.eulerAngles.x, selection.localRotation.eulerAngles.y, rotation.eulerAngles.z);
		}
	}
 
	// PASTE SCALE:
	[MenuItem ("CONTEXT/Transform/Paste Scale",false,300)]
	static void DoApplyScaleXYZ () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Scale" + selection.name);
			selection.localScale = scale;
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Scale X",false,301)]
	static void DoApplyScaleX () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Scale X" + selection.name);
			selection.localScale = new Vector3(scale.x, selection.localScale.y, selection.localScale.z);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Scale Y",false,302)]
	static void DoApplyScaleY () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Scale Y" + selection.name);
			selection.localScale = new Vector3(selection.localScale.x, scale.y, selection.localScale.z);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/Paste Scale Z",false,303)]
	static void DoApplyScaleZ () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "Paste Scale Z" + selection.name);
			selection.localScale = new Vector3(selection.localScale.x, selection.localScale.y, scale.z);
		}
	}
 
	// CHANGE LOCAL ROTATION :
	[MenuItem ("CONTEXT/Transform/localRotation.x + 90",false,350)]
	static void localRotateX90 () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "localRotation.x + 90" + selection.name);
			selection.localRotation = selection.localRotation*Quaternion.Euler(90f,0f,0f);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/localRotation.y + 90",false,351)]
	static void localRotateY90 () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections){
			Undo.RecordObject(selection, "localRotation.y + 90" + selection.name);
			selection.localRotation = selection.localRotation*Quaternion.Euler(0f,90f,0f);
		}
	}
 
	[MenuItem ("CONTEXT/Transform/localRotation.z + 90",false,352)]
	static void localRotateZ90 () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections) {
			Undo.RecordObject(selection, "localRotation.z + 90" + selection.name);
			selection.localRotation = selection.localRotation*Quaternion.Euler(0f,0f,90f);
		}
	}
 
	// SWAP:
	[MenuItem ("CONTEXT/Transform/Swap Y and Z Scale", false, 401)]
	static void SwapYZScale () {
		Transform[] selections  = Selection.transforms;
		foreach (Transform selection  in selections) {
			Undo.RecordObject(selection, "Swap Y and Z Scale" + selection.name);
			selection.localScale = new Vector3 (selection.localScale.x,selection.localScale.z,selection.localScale.y);
		}
	}
}
}
