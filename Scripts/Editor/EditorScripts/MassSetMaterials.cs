/*************************
 * Original url: http://wiki.unity3d.com/index.php/MassSetMaterials
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/MassSetMaterials.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Original Author: Stelimar 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    
    DescriptionAllows you to set the materials of multiple objects at once. All of the selected objects will have their materials set to the same as the current active object (the object shown in the Inspector Window). 
    Usage1.Place the MassSetMaterials.js script in YourProject/Assets/Editor.
    2.Set the materials of one object to the desired materials.
    3.Ctrl + click on the other objects to select them, making sure that the first object is still show in the Inspector Window.
    4.Click Scripts > Mass Set Materials from the menu bar at the top of the screen.
    
    Javascript - MassSetMaterials.js@MenuItem("Scripts/Mass Set Materials")
    static function MassSetMaterials() {
        Undo.RegisterSceneUndo("Mass Set Materials");
     
        var mats : Material[] = Selection.activeGameObject.renderer.sharedMaterials;
     
        for (var obj : GameObject in Selection.gameObjects) {
            obj.renderer.sharedMaterials = mats;
        }
}
}
