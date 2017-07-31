// Original url: http://wiki.unity3d.com/index.php/BoxColliderUtilities
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/BoxColliderUtilities.cs
// File based on original modification date of: 22 February 2013, at 15:41. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
developed November 2010 
Update(22.02.2013) 
It seems that since Unity3.4 a similar function is already built into Unity. Just hold down "Shift" when you select a primitive collider (Sphere, Box, Capsule). This will show small green handles at the center of each face which can be dragged. 


DescriptionAllows you to scale or rotate a BoxCollider side-aligned 
This script extends the Unity editor when editing a BoxCollider. Depending on the transformation mode it shows 6 additional handles in the scene-view. In scaling mode you can extend or reduce the box at one end. In other words the box doesn't scale around the center pivot. The GOs center is adjusted to keep the opposite side where it is. In rotation mode it's quite similar. You can rotate the box around one of it's 6 side-centers. 
This can be useful when placing triggers or colliders in the scene. 
(I've also planned to rotate around the box corners but no time atm ;)) 
UsageYou must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
To use the additional scaling/rotating: select a GameObject with a BoxCollider-component attached and choose the transformation mode in the BoxCollider inspector. To hide the Unity default handle just switch to the camera mode (hotkey Q) 
watch out: Don't try to scale a Box that is nested in a GO which is already scaled. This script works only when the local scale is equal to the global scale. So nesting is possible, but all parents need to have all scaling factors set to 1. Parent rotation or movement will work without problems. 
C# - BoxColliderUtilities.cs /******************************************************************************
 *	2010 written by Bunny83
 *  Editor extention for BoxCollider-components to allow side-aligned scaling
 *  and rotating of the box
 *  
 *  Useful to place and modify BoxColliders (Triggers etc...)
 *  	
 *  ps. to hide the default Unity position/rotate/scale handle just switch to
 *  camera-mode (hotkey Q)
 ******************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;
 
[CustomEditor(typeof(BoxCollider))]
public class BoxColliderUtilities : Editor
{
    private enum ETransformMode
    {
        None,
        Scale,
        Rotate,
        LAST
    }
 
    private Vector3[] m_HandlePositions = null;
    private BoxCollider m_Collider = null;
 
    // this is static to keep the mode when you select another BoxCollider
    private static ETransformMode m_Mode = ETransformMode.None;
    private static int m_Side = -1;
 
    void OnEnable()
    {
        m_Collider = (BoxCollider)target;
        m_HandlePositions = new Vector3[6];
    }
 
    void OnDisable()
    {
        m_HandlePositions = null;
    }
 
    void OnSceneGUI()
    {   
        if (target.GetType() != typeof(BoxCollider)|| m_Collider == null)
            return;          
 
        Vector3 Pos = m_Collider.transform.position;  
        // Calculate the handle positions
        m_HandlePositions[0] = m_Collider.transform.TransformPoint(m_Collider.size.x/2,0,0);
        m_HandlePositions[1] = m_Collider.transform.TransformPoint(-m_Collider.size.x/2,0,0);
        m_HandlePositions[2] = m_Collider.transform.TransformPoint(0,m_Collider.size.y/2,0);
        m_HandlePositions[3] = m_Collider.transform.TransformPoint(0,-m_Collider.size.y/2,0);
        m_HandlePositions[4] = m_Collider.transform.TransformPoint(0,0,m_Collider.size.z/2);
        m_HandlePositions[5] = m_Collider.transform.TransformPoint(0,0,-m_Collider.size.z/2);
 
 
 
        switch (m_Mode)
        {
            case ETransformMode.Scale:
            {
                Undo.SetSnapshotTarget(m_Collider.transform,"Aligned Scale");
 
                Vector3[] NewHandlePositions = new Vector3[6];
                NewHandlePositions[0] = Handles.Slider(m_HandlePositions[0], m_Collider.transform.right,  HandleUtility.GetHandleSize(m_HandlePositions[0]) * 1.2f, Handles.ArrowCap, 0.1f);
                NewHandlePositions[1] = Handles.Slider(m_HandlePositions[1],-m_Collider.transform.right,  HandleUtility.GetHandleSize(m_HandlePositions[1]) * 1.2f, Handles.ArrowCap, 0.1f);
                NewHandlePositions[2] = Handles.Slider(m_HandlePositions[2], m_Collider.transform.up,     HandleUtility.GetHandleSize(m_HandlePositions[2]) * 1.2f, Handles.ArrowCap, 0.1f);
                NewHandlePositions[3] = Handles.Slider(m_HandlePositions[3],-m_Collider.transform.up,     HandleUtility.GetHandleSize(m_HandlePositions[3]) * 1.2f, Handles.ArrowCap, 0.1f);
                NewHandlePositions[4] = Handles.Slider(m_HandlePositions[4], m_Collider.transform.forward,HandleUtility.GetHandleSize(m_HandlePositions[4]) * 1.2f, Handles.ArrowCap, 0.1f);
                NewHandlePositions[5] = Handles.Slider(m_HandlePositions[5],-m_Collider.transform.forward,HandleUtility.GetHandleSize(m_HandlePositions[5]) * 1.2f, Handles.ArrowCap, 0.1f);
 
                Vector3 Change;
                Vector3 Scale = m_Collider.transform.localScale;
 
                Change = NewHandlePositions[0] - m_HandlePositions[0];
                if (Change.sqrMagnitude != 0.0f)
                {
                    m_Collider.transform.position = Pos + Change * 0.5f;
                    Scale.x = (m_Collider.transform.position - NewHandlePositions[0]).magnitude * 2.0f / m_Collider.size.x;
                }
                Change = NewHandlePositions[1] - m_HandlePositions[1];
                if (Change.sqrMagnitude != 0.0f)
                {
                    m_Collider.transform.position = Pos + Change * 0.5f;
                    Scale.x = (m_Collider.transform.position - NewHandlePositions[1]).magnitude * 2.0f / m_Collider.size.x;
                }
                Change = NewHandlePositions[2] - m_HandlePositions[2];
                if (Change.sqrMagnitude != 0.0f)
                {
                    m_Collider.transform.position = Pos + Change * 0.5f;
                    Scale.y = (m_Collider.transform.position - NewHandlePositions[2]).magnitude * 2.0f / m_Collider.size.y;
                }
                Change = NewHandlePositions[3] - m_HandlePositions[3];
                if (Change.sqrMagnitude != 0.0f)
                {
                    m_Collider.transform.position = Pos + Change * 0.5f;
                    Scale.y = (m_Collider.transform.position - NewHandlePositions[3]).magnitude * 2.0f / m_Collider.size.y;
                }
                Change = NewHandlePositions[4] - m_HandlePositions[4];
                if (Change.sqrMagnitude != 0.0f)
                {
                    m_Collider.transform.position = Pos + Change * 0.5f;
                    Scale.z = (m_Collider.transform.position - NewHandlePositions[4]).magnitude * 2.0f / m_Collider.size.z;
                }
                Change = NewHandlePositions[5] - m_HandlePositions[5];
                if (Change.sqrMagnitude != 0.0f)
                {
                    m_Collider.transform.position = Pos + Change * 0.5f;
                    Scale.z = (m_Collider.transform.position - NewHandlePositions[5]).magnitude * 2.0f / m_Collider.size.z;
                }
 
                if (m_Collider.transform.localScale != Scale)
                {
                    m_Collider.transform.localScale = Scale;
                }
                break;
            }
            case ETransformMode.Rotate:
            {
                Undo.SetSnapshotTarget(m_Collider.transform,"Aligned Rotate");
 
                for (int i = 0; i < 6; i++)
                {
                    Handles.color = (m_Side == i)?Color.green:Color.blue;
                    // Just show a sphere at the handle position to select the side you want to rotate
                    if (Handles.Button(m_HandlePositions[i], Quaternion.identity, HandleUtility.GetHandleSize(m_HandlePositions[i]) * 0.2f, 0.1f, Handles.SphereCap))
                    {
                        m_Side = i;
                    }
                }
 
                if (m_Side >= 0 && m_Side < 6)
                {
                    Quaternion newRotation = Handles.RotationHandle(m_Collider.transform.rotation, m_HandlePositions[m_Side]);
                    if (GUI.changed)
                    {
                        m_Collider.transform.rotation = newRotation;
                        Vector3 HandleDelta = Vector3.zero;
                        // Calculate the center-movement due to rotation
                        switch (m_Side)
                        {
                            case 0:HandleDelta = m_HandlePositions[0] - m_Collider.transform.TransformPoint(m_Collider.size.x/2,0,0);break;
                            case 1:HandleDelta = m_HandlePositions[1] - m_Collider.transform.TransformPoint(-m_Collider.size.x/2,0,0);break;
                            case 2:HandleDelta = m_HandlePositions[2] - m_Collider.transform.TransformPoint(0,m_Collider.size.y/2,0);break;
                            case 3:HandleDelta = m_HandlePositions[3] - m_Collider.transform.TransformPoint(0,-m_Collider.size.y/2,0);break;
                            case 4:HandleDelta = m_HandlePositions[4] - m_Collider.transform.TransformPoint(0,0,m_Collider.size.z/2);break;
                            case 5:HandleDelta = m_HandlePositions[5] - m_Collider.transform.TransformPoint(0,0,-m_Collider.size.z/2);break;
                        }
                        // Apply the movement. That keeps the handle at the original position
                        m_Collider.transform.position += HandleDelta;
                    }
                }
                break;
            }
        }
    } 
 
    public override void OnInspectorGUI()
    {
        // Implement new inspector buttons for our new features
        GUILayout.Label("Extra Gizmos");
        m_Mode = (ETransformMode)GUILayout.Toolbar((int)m_Mode,new string[3]{"None","Scale","Rotate"});
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        // After we drawn our stuff, draw the default inspector
        DrawDefaultInspector();
    }
}
}
