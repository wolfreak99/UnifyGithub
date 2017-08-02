/*************************
 * Original url: http://wiki.unity3d.com/index.php/SVG
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/SVG.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Description 
 *   
 * Example File 
 *   
 * Usage 
 *   
 * Limitations 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Description This package is used render SVG Files. If you have an SVG file and want to get its texture, this SVG will help you do this. Input is an SVG File and Output is a Texture2D of it. 
    Example File  
    Usage Quick start: 
    1. Get the project on GitHub: http://github.com/MrJoy/UnitySVG 
    2. Open the scene named "Test" and click on "Plane". 
    
    In more depth: 
    1. Direct show GameObject's texture from an SVG File: Drag and drop file Invoke.cs (Hao_MrJoy > Use > Invoke.cs) to GameObject that you want to show it's texture from a SVG file. Drag and drop Material (Hao_MrJoy > Use > Material) to the same GameObject. 
    2. Get and use Texture2D from a SVG file: 
    Implement m_implement = new Implement(TextAssetObject);
    m_implement.f_StartProcess();
    Texture2D m_texture = m_implement.f_GetTexture();3. Use RenderingEngine for drawing. uSVGGraphics class (Hao_MrJoy > SVG > Resources > Implementation > RenderingEngine) provides some methods for you draw basic shapes, paths,... to a Texture2D. 
    
    
    Limitations While the version on GitHub is significantly improved with respect to memory usage (as much as 500x in some cases!), performance remains a weak point. Performance seems largely proportional to the resolution of the generated texture, so if you're having problems try cutting it down to a lower resolution. 
    We've also done very little to support artist workflow. 
    Animation / scripting are not supported, and we have no plans to do so in the foreseeable future. 
    Text rendering is not supported because we would not be able to produce desirable results: SVG fonts are often huge (80MB), using system fonts isn't feasible, and using Unity fonts would produce quite ugly and unexpected results due to them not being vector based. 
    In other words, consider this a rough draft, and set your expectations accordingly. 
}
