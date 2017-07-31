// Original url: http://wiki.unity3d.com/index.php/Teleporter
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/Teleporter.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{

This script works like the halo Teleporters, but you can shoot rockets in to it and it come out the other teleporter Have fun with it. BY Ryan Davis. 
Simply add two objects to your scene. Place this script onto the object you wish to be the entrance teleporter (must have a collider attached). Then drag the object from the Hierarchy, which will be the exit, into the slot named "teleportTo" in the entrances inspector, and voila. 
Javascript - Teleporter.js var teleportTo : Transform;
 
function OnCollisionEnter (col : Collision) {
    col.transform.position = teleportTo.position;	
}CSharp - Teleporter.cs using UnityEngine;
 
public class Teleporter : MonoBehaviour
{
    Transform teleportTo;
 
    void OnCollisionEnter (col : Collision) {
        col.transform.position = teleportTo.position;	
    }
}
}
