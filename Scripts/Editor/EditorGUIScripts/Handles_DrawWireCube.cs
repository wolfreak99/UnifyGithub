/*************************
 * Original url: http://wiki.unity3d.com/index.php/Handles_DrawWireCube
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/Handles_DrawWireCube.cs
 * File based on original modification date of: 9 December 2012, at 21:49. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    Author Created by: X - http://www.createdbyx.com 
    Description This method allows you to draw a wire cube inside your OnSceneGUI methods. it is intended to be used as a counter part to Gizmos.DrawWireCube 
     
    C# Code        public static void DrawWireCube(Vector3 position, Vector3 size)
           {
               var half = size / 2;
               // draw front
               Handles.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
               Handles.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
               Handles.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
               Handles.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
               // draw back
               Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
               Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
               Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
               Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
               // draw corners
               Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, -half.y, half.z));
               Handles.DrawLine(position + new Vector3(half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, half.z));
               Handles.DrawLine(position + new Vector3(-half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, half.z));
               Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, half.y, half.z));
           }
}
