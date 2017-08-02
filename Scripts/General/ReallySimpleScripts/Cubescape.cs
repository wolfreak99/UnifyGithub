/*************************
 * Original url: http://wiki.unity3d.com/index.php/Cubescape
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/Cubescape.cs
 * File based on original modification date of: 2 February 2013, at 04:02. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.ReallySimpleScripts
{
    JavaScript- Cubescape.js//simple code for learning, makes a 100x100 cube landscape using 2 unity perlin noise functions, to add depth you should add perlins of different 
    //octaves/scales, size and periods. /30 is the hill closeness, and 40 is the height. 76 is the seed randomizer. 
     
    private var rot : Quaternion;
    rot.eulerAngles = Vector3(0, 0, 0);
    var terraincube: GameObject;
     
    function Start (){
    for (var px:float = 0; px < 100; px ++) {
        for (var py:float = 0; py< 100; py ++) {
     
    var Perlin1 = Mathf.PerlinNoise(px/30, 76);
    var Perlin2 = Mathf.PerlinNoise(py/30, 22);
    Instantiate(terraincube, Vector3(py-50, Perlin1*40*Perlin2, px-50), rot);
     
     
    }
     
    }
    }
}
