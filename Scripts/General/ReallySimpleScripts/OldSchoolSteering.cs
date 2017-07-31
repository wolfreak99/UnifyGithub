// Original url: http://wiki.unity3d.com/index.php/OldSchoolSteering
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/OldSchoolSteering.cs
// File based on original modification date of: 29 December 2012, at 17:50. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{

Author: Lars-Erik Jakobsson (save) 
Description Assigns a new forward direction on a transform from horizontal- and vertical input to get eight rotation directions. 
JavaScript - OldSchoolSteering.js function Update () {
    if (Input.GetButton("Horizontal")||Input.GetButton("Vertical")) {
 
       var newForward : Vector3 = Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
       if (newForward!=Vector3.zero) transform.forward = newForward;
 
       transform.Translate(Vector3.forward*Time.deltaTime);
    }
}
}
