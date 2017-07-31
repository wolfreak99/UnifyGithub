// Original url: http://wiki.unity3d.com/index.php/BoneDebug
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/BoneDebug.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Author: Jesper Taxbøl 
Description This is a simple C# script I am using when debugging animations. 
It allows you to see the bones of a character, revealing details that can be hard to see through skinning. 
 using UnityEngine;
 using System.Collections;
 [ExecuteInEditMode]
 public class BoneDebug : MonoBehaviour
 {
    void drawbone(Transform t)
    {
        foreach (Transform child in t)
        {
            float len = 0.05f;
            Vector3 loxalX = new Vector3(len, 0, 0);
            Vector3 loxalY = new Vector3(0, len, 0);
            Vector3 loxalZ = new Vector3(0, 0, len);
            loxalX = child.rotation * loxalX;
            loxalY = child.rotation * loxalY;
            loxalZ = child.rotation * loxalZ;
            Debug.DrawLine(t.position * 0.1f + child.position * 0.9f, t.position * 0.9f + child.position * 0.1f, Color.white);
            Debug.DrawLine(child.position, child.position + loxalX, Color.red);
            Debug.DrawLine(child.position, child.position + loxalY, Color.green);
            Debug.DrawLine(child.position, child.position + loxalZ, Color.blue);
            drawbone(child);
        }
    }
    void Update()
    {
        drawbone(transform);
    }
 }
}
