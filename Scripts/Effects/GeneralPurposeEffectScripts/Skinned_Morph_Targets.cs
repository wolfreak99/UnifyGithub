/*************************
 * Original url: http://wiki.unity3d.com/index.php/Skinned_Morph_Targets
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Skinned_Morph_Targets.cs
 * File based on original modification date of: 7 October 2010, at 14:47. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Download : Media:MayaPipeline.zip , Media:SkinnedMorphTargets.zip 
    While working on our next game, the company I work for (Omek Interactive) needed to use blend shapes / morph targets in our animation system. Unity lacks support for them, and the user-posted solutions in the forums don't have support for doing so in skinned meshes, and they don't have support for animating these values. They also don't support meshes with multiple submeshes. 
    We implemented a system that supports : 
    Blend shapes for skinned targets (only skinned targets for now) 
    The blend weights can be animated or controlled manually 
    Support for meshes with multiple submeshes. 
    Fast import pipeline from Maya. 
    Runtime system The attached unity package contains everything you need to use the system. There are three ways to use it : 
    Manual mode : Attach the "Skinned Morph Targets" script to the object that should be animated, and populate the list of morph targets with the morph meshes. 
    Semi-automatic mode : (This relies on the pipeline) - For an object exported after running our maya scripts (see the attached Cube.fbx/mb), click the object on the scene and go to Morph->Prepare Maya Blend Shapes. It will add the script component and link the submeshes automatically. 
    Fully-automatic mode : (Also relies on the pipeline) - Add the MayaMorphPreparer script to an empty gameobject, and set the object as the Target Object. The "Link Animations" flag toggles whether animations will be controlled automatically or not. 
    Demo The demo attached as part of the unity package contains method #2, but all three are possible in it. The two animations that are valid in this demo are RotAnim and TransAnim, which demonstrate that the blend shapes are compatible with different types of animations. If you disable the "Blend Weight Animation Connector" script on the cube object, you will be able to manually control the blend shapes in the demo. 
    Animation import system The attached MayaPipeline.zip file contains our solution to the Maya->Unity bridge. It automatically creates meshes for each blend shape and nodes containing the blend weight for the animations. To use it, run the two functions createMorphTargetMeshes and createMorphTargetLocationAnimations with the designated object selected, and it will create all the data needed in Maya. You can then use the normal .FBX exported to create the FBX. I attached the cube from the demo (with its blend shapes and animations) in the zip file as well so you can test on that. The Cube.fbx from the unity package is just the cube.mb after both functions have been applied to it and it was exported to Maya. This means that the export process takes just 10 seconds longer than usual and you get full blend shape animations. Notice that if you play the animation in Maya and in the demo you get the exact same visual result - true WYSIWYG. 
    
    We chose to use the BSD license for this contribution to the community, I don't think there should be any problems with it. (If there are, feel free to comment about it). 
    Of course, you can modify the runtime/export scripts to fit your needs. I didn't explain all the naming conventions, but the code is documented well enough to understand. 
    Enjoy! 
    Download : Media:MayaPipeline.zip , Media:SkinnedMorphTargets.zip 
}
