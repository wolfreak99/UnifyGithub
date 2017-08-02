/*************************
 * Original url: http://wiki.unity3d.com/index.php/LookAtCameraYonly
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/LookAtCameraYonly.cs
 * File based on original modification date of: 2 February 2015, at 17:27. 
 *
 * Author: Neil Carter (NCarter) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    DescriptionThis script will rotate a GameObject on its Y axis so that it is always facing the selected camera. It is useful for making camera facing billboards. 
    UsagePlace this script on a GameObject that you want to face the camera. Then, with the object selected, use the inspector to select the Camera you want the object to face. 
    C# - LookAtCameraYonly.cs 
    using UnityEngine;
     using System.Collections;
     
     public class LookAtCameraYonly : MonoBehaviour
     {
        public Camera cameraToLookAt;
     
        void Update() 
        {
            Vector3 v = cameraToLookAt.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(cameraToLookAt.transform.position - v); 
        }
 }
}
