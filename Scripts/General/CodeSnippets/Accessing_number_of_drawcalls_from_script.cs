// Original url: http://wiki.unity3d.com/index.php/Accessing_number_of_drawcalls_from_script
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/Accessing_number_of_drawcalls_from_script.cs
// File based on original modification date of: 19 January 2013, at 22:35. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
By: Alex Schwartz (GTJuggler) 
Note - When running in the editor, scene view cameras will be included in the draw call count! Also, if you instantiate new objects at runtime, this script will not take those into account, so you should deal with that case manually. 
FYI - This is NOT producing anything remotely like a "number of draw calls". It will tell you how many renderers will render something in a given frame, more or less. Any given object may produce 0..N draw calls depending on the number/kind of lights hitting it, shadowcasting settings, number of materials / sub-meshes, and so forth. On top of that, some objects such as terrains can vary considerably in the number of draw calls. And that doesn't address things like image effects, GUIs (both GUILayer-based and UnityGUI). 
Javascript - DrawCalls.js var drawcalls : int = 0;
var allObjects : GameObject[];
 
function Start(){
	allObjects = FindObjectsOfType (GameObject);
}
 
function Update(){
	for(var obj : GameObject in allObjects){
		var rend : Renderer = obj.GetComponent(Renderer);
		if(rend && rend.isVisible){
			drawcalls++;
		}
	}
 
	//print drawcalls
	Debug.Log(drawcalls);
 
	//reset drawcalls every update
	drawcalls = 0;
 
	//do some math to find average drawcall count here
}
}
