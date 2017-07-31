// Original url: http://wiki.unity3d.com/index.php/AddComponentRecursively
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/AddComponentRecursively.cs
// File based on original modification date of: 11 December 2012, at 16:19. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Daniel Brauer 
DescriptionThis editor script adds the option to add a component to an object and all its children. It checks for existing components, so no duplicates will be added. 
UsagePlace this script in YourProject/Assets/Editor and the menu items will appear in the GameObject menu once it is compiled. Please note that you have to push return after entering the name of the component, otherwise it will not be registered when you push Add. 
AddComponentRecursively.jsclass AddComponentRecursively extends ScriptableWizard {
 
	var componentName : String = "";
 
	@MenuItem ("GameObject/Add Component Recursively...")
 
	static function AddComponentsRecursivelyItem() {
		ScriptableWizard.DisplayWizard("Add Component Recursively", AddComponentRecursively, "Add", "");
	}
 
	//Main function
	function OnWizardCreate() {
		var total : int = 0;
		for (var currentTransform : Transform in Selection.transforms) { 
	      total += RecurseAndAdd(currentTransform, componentName);
		}
		if (total == 0)
			Debug.Log("No components added.");
		else
			Debug.Log(total + " components of type \"" + componentName + "\" created.");
	}
 
	function RecurseAndAdd(parent : Transform, componentToAdd : String) : int {
		//keep count
		var total : int = 0;
		//add components to children
		for (var child : Transform in parent) {
			total += RecurseAndAdd(child, componentToAdd);
		}
		//add component to parent
		var existingComponent : Component = parent.GetComponent(componentToAdd);
		if (!existingComponent) {
			parent.gameObject.AddComponent(componentToAdd);
			total++;
		}
 
		return total;
	}
	//Set the help string
	function OnWizardUpdate () {  
	    helpString = "Specify the exact name of the component you wish to add:";
	}
 
	// The menu item will be disabled if no transform is selected. 
	@MenuItem ("GameObject/Add Component Recursively...", true) 
 
	static function ValidateMenuItem() : boolean { 
	   return Selection.activeTransform; 
	}
}
}
