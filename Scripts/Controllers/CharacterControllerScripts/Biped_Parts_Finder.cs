// Original url: http://wiki.unity3d.com/index.php/Biped_Parts_Finder
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/Biped_Parts_Finder.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
This is obsolete part of the Antares Project. 
CsBiped.cs and CsBiredEditor is Editor scripts for search simplification of parts of a biped body. Biped hierarchy contain to many objects and search of the necessary part of a biped body to become the difficult task. Having changed these scripts according to your desire, you can strongly facilitate this work. 
This scripts is only the base. Use it for build your own. 
Import the package from this arch to the Unity. Good luck. 
NeoBipedFinder.unitypackage 
 
CsBiped.cs //Created by Neodrop
//neodrop@unity3d.ru
using UnityEngine;
 
// Put this script on the character object with biped hierarchy
 
public class CsBiped  : MonoBehaviour 
{
    public Transform head, handLeft, handRight, footLeft, footRight;
}CsBipedEditor.cs using UnityEditor;
//Created by Neodrop
//neodrop@unity3d.ru
using UnityEngine;
 
[CustomEditor(typeof(CsBiped))]
public class CsBipedEditor : Editor 
{
    public Texture bodyTexture;
    CsBiped targetObject;
    SerializedObject serializedObject;
    static int w = 339, h = 341;
 
	void OnEnable () 
    {
        targetObject = target as CsBiped;
        serializedObject = new SerializedObject(target);
        serializedObject.Update();
        if (bodyTexture == null)
        {
            bodyTexture = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/gold.jpg", typeof(Texture)) as Texture;
            string bipName = "";
            foreach (Transform tr in targetObject.gameObject.GetComponentsInChildren<Transform>())
            {
                bipName = tr.gameObject.name;
                switch(bipName)
                {
                    case "Bip01 R Hand" : 
                        targetObject.handRight = tr;
                        break;
                    case "Bip01 L Hand" : 
                        targetObject.handLeft = tr;
                        break;
                    case "Bip01 Head" : 
                        targetObject.head = tr;
                        break;
                    case "Bip01 R Toe0" : 
                        targetObject.footRight = tr;
                        break;
                    case "Bip01 L Toe0" : 
                        targetObject.footLeft = tr;
                        break;
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
	}
 
	public override void OnInspectorGUI () 
    {
        if (!bodyTexture) return; 
        Rect lRect = GUILayoutUtility.GetLastRect();
        float middle = lRect.width / 2, half = 170f;
        Vector2 leftUp = new Vector2(middle - half, lRect.y + 25);
        GUI.Box(new Rect(leftUp.x, leftUp.y, w, h), bodyTexture);
 
        EditorGUILayout.BeginHorizontal();
        float yHand = leftUp.y + 107;
        if (GUI.Button(new Rect(leftUp.x + 45, yHand, 15, 15), new GUIContent(":", "Right hand"))) 
            EditorGUIUtility.PingObject(targetObject.handRight.gameObject);
        if(GUI.Button(new Rect(leftUp.x + 279, yHand, 15, 15), new GUIContent(":", "Left hend"))) 
           EditorGUIUtility.PingObject(targetObject.handLeft.gameObject);
        float xFoot = lRect.y + h - 10;
        if (GUI.Button(new Rect(middle - 80, xFoot, 15, 15), new GUIContent(".", "Right foot"))) 
           EditorGUIUtility.PingObject(targetObject.footRight.gameObject);
        if (GUI.Button(new Rect(middle + 80 - 6, xFoot, 15, 15), new GUIContent(".", "Left foot"))) 
           EditorGUIUtility.PingObject(targetObject.footLeft.gameObject);
        EditorGUILayout.EndHorizontal();
 
        if(GUI.Button(new Rect(middle - 7.5f, lRect.y + 85, 15, 15), new GUIContent("+", "Head"))) 
           EditorGUIUtility.PingObject(targetObject.head.gameObject);
 
        GUILayout.Space(h + 20);
	}
}
}
