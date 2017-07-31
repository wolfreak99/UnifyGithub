// Original url: http://wiki.unity3d.com/index.php/SetGOFlags
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/SetGOFlags.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
By: DaveA 
Description Editor Utility that lets you set or clear the 'static', 'active', 'cast shadows' and 'receive shadows' properties on all selected objects and their children. Undoable! 
Put this script into your project in the Editor folder. You will get the menu option GameObject->Static->Yes/No GameObject->Active->Yes/No, etc. 
Code Javascript - SetGOFlags.js @MenuItem("GameObject/Static/Yes", false, 4) 
static function setStaticPicked() 
{ 
	Undo.RegisterSceneUndo ("Set Static");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setStatic (trs, true);
} 
 
@MenuItem("GameObject/Static/No", false, 4) 
static function clearStaticPicked() 
{
	Undo.RegisterSceneUndo ("Clear Static");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setStatic (trs, false);
} 
 
static function setStatic(trs, state) 
{ 
	for (var tr in trs) 
	{ 
		tr.gameObject.isStatic = state;
	} 
} 
 
 
 
@MenuItem("GameObject/Active/Yes", false, 4) 
static function setActivePicked() 
{ 
	Undo.RegisterSceneUndo ("Set Active");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setActive (trs, true);
} 
 
@MenuItem("GameObject/Active/No", false, 4) 
static function clearActivePicked() 
{
	Undo.RegisterSceneUndo ("Clear Active");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setActive (trs, false);
} 
 
static function setActive(trs, state) 
{ 
	for (var tr in trs) 
	{ 
		tr.gameObject.active = state;
	} 
} 
 
 
 
 
@MenuItem("GameObject/CastShadows/Yes", false, 4) 
static function setCastShadowsPicked() 
{ 
	Undo.RegisterSceneUndo ("Set CastShadows");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setCastShadows (trs, true);
} 
 
@MenuItem("GameObject/CastShadows/No", false, 4) 
static function clearCastShadowsPicked() 
{
	Undo.RegisterSceneUndo ("Clear CastShadows");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setCastShadows (trs, false);
} 
 
static function setCastShadows(trs, state) 
{ 
	for (var tr in trs) 
	{
		if (tr.gameObject.renderer)
			tr.gameObject.renderer.castShadows = state;
	} 
} 
 
@MenuItem("GameObject/ReceiveShadows/Yes", false, 4) 
static function setReceiveShadowsPicked() 
{ 
	Undo.RegisterSceneUndo ("Set ReceiveShadows");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setReceiveShadows (trs, true);
} 
 
@MenuItem("GameObject/ReceiveShadows/No", false, 4) 
static function clearReceiveShadowsPicked() 
{
	Undo.RegisterSceneUndo ("Clear ReceiveShadows");
	var trs = Selection.GetTransforms (SelectionMode.Deep);
	setReceiveShadows (trs, false);
} 
 
static function setReceiveShadows(trs, state) 
{ 
	for (var tr in trs) 
	{
		if (tr.gameObject.renderer)
			tr.gameObject.renderer.receiveShadows = state;
	} 
}
}
