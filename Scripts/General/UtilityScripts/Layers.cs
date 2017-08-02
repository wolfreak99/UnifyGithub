/*************************
 * Original url: http://wiki.unity3d.com/index.php/Layers
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/Layers.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    by Ray Nothnagel 
    DescriptionA static class designed to ease layer usage. 
    Usage//set the layer to 2 (ignore raycast)
    gameObject.layer = Layers.ignoreRaycast;
    //make the camera render only objects in the ignore raycast layer
    camera.cullingMask = Layers.CreateInclusiveMask([Layers.ignoreRaycast]);
    //make the camera render everyting but that layer
    camera.cullingMask = Layers.CreateExclusiveMask([Layers.ignoreRaycast]);If you add more layers, this becomes even more handy as you no longer need to think about bitwise math to create complex masks. 
    //this assumes you've added "player" and "landscape" layer variables to Layers.js
    //the camera will render only objects in those two layers
    camera.cullingMask = Layers.CreateInclusiveMask([Layers.player, Layers.landscape]);Layers.jsstatic var ignoreRaycast = 2;
    //any other layers you would like to access can go here
    //static var player = 8;
     
    static function CreateInclusiveMask(layers : int[]) : LayerMask{
    	var m : int = 0;
    	for (l=0;l<layers.length;l++) {
    		m |= (1<<layers[l]);
    	}
    	return m;
    }
     
    static function CreateExclusiveMask(layers : int[]) : LayerMask{
    	var m : int = 0;
    	for (l=0;l<layers.length;l++) {
    		m |= (1<<layers[l]);
    	}
    	return ~m;
    }
}
