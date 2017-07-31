// Original url: http://wiki.unity3d.com/index.php/Headbobber
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/Headbobber.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: (Mr. Animator) 
DescriptionThis script makes an object bob up and down smoothly when you're pressing one or both of the horizontal/vertical axes. Attach it to a camera and play with the public variables to get the feel of your player's head bobbing as they walk. The midpoint variable is whatever value for the Y translate you want to be considered the middle of the camera's bobbing range. 
UsagePlace this script on a Camera. 
JavaScript - Headbobber.js private var timer = 0.0; 
 var bobbingSpeed = 0.18; 
 var bobbingAmount = 0.2; 
 var midpoint = 2.0; 
 
 function Update () { 
    waveslice = 0.0; 
    horizontal = Input.GetAxis("Horizontal"); 
    vertical = Input.GetAxis("Vertical"); 
    if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) { 
       timer = 0.0; 
    } 
    else { 
       waveslice = Mathf.Sin(timer); 
       timer = timer + bobbingSpeed; 
       if (timer > Mathf.PI * 2) { 
          timer = timer - (Mathf.PI * 2); 
       } 
    } 
    if (waveslice != 0) { 
       translateChange = waveslice * bobbingAmount; 
       totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical); 
       totalAxes = Mathf.Clamp (totalAxes, 0.0, 1.0); 
       translateChange = totalAxes * translateChange; 
       transform.localPosition.y = midpoint + translateChange; 
    } 
    else { 
       transform.localPosition.y = midpoint; 
    } 
 }
}
