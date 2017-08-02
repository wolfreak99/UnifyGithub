/*************************
 * Original url: http://wiki.unity3d.com/index.php/FindSceneObjectsWithTag
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/FindSceneObjectsWithTag.cs
 * File based on original modification date of: 17 August 2012, at 06:19. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    A dockable editor window to find all scene objects with a particular tag. 
    Contents [hide] 
    1 Description 
    2 Screenshots 
    3 Code 
    3.1 FindSceneObjectsWithTag.cs 
    
    DescriptionA dockable editor window to find all scene objects with a particular tag. Displays all results in a scrollable window. Can set an optional upper limit on results. 
    ScreenshotsFindByTag00.png FindByTag01.png FindByTag02.png FindByTag03.png FindByTag04.png FindByTag05.png 
    CodeFindSceneObjectsWithTag.csusing UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
     
    public class FindSceneObjectsWithTag : EditorWindow
    {
    	public string TagToSearchFor = "Player";
     
    	public bool LimitResultCount = false;
    	public int MaxResults = 1;
     
    	public List<GameObject> Results;
    	private Vector2 ResultScrollPos;
     
    	void OnGUI()
    	{
    		EditorGUILayout.BeginVertical();
    		{
    			EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
    			{
    				TagToSearchFor = EditorGUILayout.TextField("Tag:", TagToSearchFor);
     
    				if (GUILayout.Button("Find"))
    				    Find();
     
    				if (LimitResultCount = EditorGUILayout.Foldout(LimitResultCount, "Limit Result Count (Limit:"
    						+ (LimitResultCount ? MaxResults.ToString() : "None") + ")"))
    					MaxResults = EditorGUILayout.IntField("Result Max:", MaxResults);
    			}
     
    			EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);
    			{
    				if (Results != null)
    				{
    					EditorGUILayout.LabelField("Scene objects found:", Results.Count.ToString(), EditorStyles.boldLabel);
     
    					ResultScrollPos = EditorGUILayout.BeginScrollView(ResultScrollPos);
    					{
    						if (LimitResultCount)
    						{
    							for (int i = 0; i < Mathf.Min(MaxResults, Results.Count); i++)
    								EditorGUILayout.ObjectField(Results[i], typeof(GameObject), false);
    						}
    						else
    						{
    							foreach (GameObject go in Results)
    								EditorGUILayout.ObjectField(go, typeof(GameObject), false);
    						}
    					}
    					EditorGUILayout.EndScrollView();
    				}
    			}
    		}
    		EditorGUILayout.EndVertical();
    	}
     
    	void Find()
    	{
    		//if it's not a preexisting tag, return null
    		//if (GameObject.
    		//{
    			//Results = null;
    			//return;
    		//}
     
    		if (TagToSearchFor != null && TagToSearchFor != "")
    			Results = new List<GameObject>(GameObject.FindGameObjectsWithTag(TagToSearchFor));
    	}
     
    	[MenuItem("Edit/Find By Tag...")]
    	static void Init()
    	{
    		FindSceneObjectsWithTag window = EditorWindow.GetWindow<FindSceneObjectsWithTag>("Find By Tag");
    		window.ShowPopup();
    		//window.ShowAuxWindow();
    	}
    }
}
