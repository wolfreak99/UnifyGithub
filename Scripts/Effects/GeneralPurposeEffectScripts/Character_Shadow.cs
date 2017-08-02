/*************************
 * Original url: http://wiki.unity3d.com/index.php/Character_Shadow
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Character_Shadow.cs
 * File based on original modification date of: 10 January 2012, at 20:44. 
 *
 * Author: Aras Pranckevicius 
 *
 * Description 
 *   
 * Usage 
 *   
 * The package 
 *   
 * Behind the scenes 
 *   
 * History 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Description 
    Character Shadows in actionThis package enables casting shadows from a single object. Good ole' projected shadows, for use on the important characters. Requires Unity Pro as it uses render textures. 
    Note that in Unity 2.0 and up there are easier to use built-in shadows. But this technique still works if you want projected shadows. 
    Usage 
    Typical script setupThe provided unity package contains the example scene inside. In general, the usage is this: 
    Have CharacterShadow.cs, CharacterShadowHelper.cs and the fadeout texture (CharacterShadowFadeout.png) in the project. Import from the unity package or just copy from somewhere. 
    Assign a non-default layer to your "character" object. Most often you'd create a layer named like "Shadow caster" and use it. 
    Add CharacterShadow.cs script to the object from where shadows are cast. This can be just an empty game object, or the light source, anything. Just the position of this object is important. 
    Assign your character to the "target" slot of the script, fadeout texture to the corresponding slot. The other properties should be intuitive enough. Texture size needs to be a power-of-two. 
    Internally shadow rendering uses one layer (default is user layer 31, defined in CharacterShadow.cs). This layer should not be used for anything else in your project! 
    Hit play, move your object or the shadow-cast-position object and enjoy. 
    This script uses render textures and therefore requires Unity Pro. It should run on about any hardware. 
    The packageFor Unity 3.0b5 (at least), you must add a line to CharacterShadow.cs to get correct shadows: 
    child.camera.isOrthoGraphic = true; // This is now called "orthographic", but the backwards-compatible name still works
    child.camera.aspect = 1.0f; // ADD THIS LINEZipped Unity package for Unity 2.x: Media:CharacterShadow2.unityPackage.zip 
    Zipped Unity package for Unity 1.x: Media:CharacterShadow.unityPackage.zip (not updated anymore) 
    Behind the scenesThe script creates a child game object with a Camera and Projector components. It also creates a RenderTexture for the shadow and sets everything up. 
    Then each frame it tracks the "character" object and modifies camera/projector so that their view fully encloses the object. It always uses orthogonal projection for the shadow because it was easier to implement :) and that's what you want most often anyway. 
    The rendering happens by rendering only the character object, with setting pixel light count to zero before and restoring it afterwards. Then a fullscreen quad is drawn on the shadow rendertexture to actually get the uniform dark color for the shadow (this is done by using a ZTest Greater in the shader). 
    History2008 Feb 20 Shadow casting object can be a hierarchy of objects (all have to use same layer though) 
    2008 Feb 14 Fix issues with multiple character shadows in the scene and with overlapping shadows 
    2007 Nov 7 Add package for Unity 2.0, with material/shader leaks fixed 
    2006 Aug 27 Fixed when used on more than one character 
2006 Aug 8 Initial version 
}
