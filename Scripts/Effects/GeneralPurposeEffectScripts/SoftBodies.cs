/*************************
 * Original url: http://wiki.unity3d.com/index.php/SoftBodies
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/SoftBodies.cs
 * File based on original modification date of: 27 September 2007, at 22:34. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    SoftBodies  
    Softbodies of various shapes and usesRealtime SoftBodies using C#, Feel free to attempt to rewrite the code in Javascript, 
    To see some webplayers, unityPackage samples, and a CPP version go to [[1]] 
    To use the code, simply make an empty object, then attach a Mesh Filter and Renderer and the script. Then place some spheres below the object and call them SphereSoft or tag them with a tag called "SoftCollide". Place a Plane below the spheres, and call it "PlaneSoft". Change parameters accordingly. Modify the Starting functions to fix vertices if you want to, and a few more hidden parameters (such as friction and 'caucho'). 
    Currently the code is in development, and some parameter settings will give strange results. It can collide only against spheres and one plane, and can not be rotated or scaled. It would be pretty easy to extend to collide against other primitives and exert force on them, but for now it's here if you want to play with it and give feedback, and to track its development. 
    The Midpoint version does midpoint integration and is significantly more stable and precise, especially for stiffer materials, but runs at half the speed. 
    Hope you enjoy, I had fun writing it. 
    
    
    SoftBody.cs - Original CSharp version 
    SoftBodyMP.cs - CSharp version with Midpoint integration 
}
