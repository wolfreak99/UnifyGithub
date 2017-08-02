/*************************
 * Original url: http://wiki.unity3d.com/index.php/AssetMover
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/AssetMover.cs
 * File based on original modification date of: 9 September 2013, at 07:31. 
 *
 * Author: Unicorn 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description It's a program for convenient move assets from one folder to another. 
    Usage You must place the script in a folder named Plugins in your project's Assets folder for it to work properly. Navigate to "Window/uAssetMover" Select asset you want to move/copy, Get selected asset name list. The name list is remembered, then you can select target folder, Move/Copy to it. 
    C# - uAssetMover.cs //Author : Unicorn Jim
    //Webpage : http://blog.unicoea.com/
     
    //uAssetMover by Unicorn Jim is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
     
     
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
     
    public class uAssetMover : EditorWindow {
     
    	string ToFolder = "";
    	string findObString = "";
    	Vector2 scrollPosition1;
    	ArrayList resultList = new ArrayList();
    	bool useSelect = true;
     
        // Add menu named "uAssetMover" to the Window menu
        [MenuItem ("Window/uAssetMover")]
        static void Init ()
    	{
            // Get existing open window or if none, make a new one:
            uAssetMover window = (uAssetMover)EditorWindow.GetWindow (typeof (uAssetMover));
        }
     
        void OnGUI () 
    	{
    		GUILayout.Label ("uAssetMover v1.0", EditorStyles.boldLabel);
     
    		GUILayout.BeginHorizontal();
    			if (GUILayout.Button("Get Selected Asset List"))
    			{
    				findObString = FindAssetName();
    			}
    		GUILayout.EndHorizontal();
     
    		scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, GUILayout.Height(100));
    			GUILayout.TextArea(findObString);
    		GUILayout.EndScrollView();
     
    		if (GUILayout.Button("Move Assets To"))
    		{
    			MoveAssetTo(ToFolder, 0);
    		}
    		if (GUILayout.Button("Copy Assets To"))
    		{
    			MoveAssetTo(ToFolder, 1);
    		}
     
    		useSelect = GUILayout.Toggle(useSelect, "To Selected Folder");
    		if (!useSelect)
    		{
    			ToFolder = EditorGUILayout.TextField ("ToFolder", ToFolder);
    		}
     
        }
     
    	string FindAssetName()
    	{
    		int i;
    		Object[] selectObs = Selection.objects;
    		Debug.Log("selectObs's count= "+selectObs.Length);
     
    		resultList.Clear();
    		for (i=0;i<selectObs.Length;i++)
    		{
    			string assetPath = AssetDatabase.GetAssetPath(selectObs[i]);
    			resultList.Add(assetPath);
    			Debug.Log("obj find: "+assetPath);
    		}
     
    		string result = "";
    		for (i=0;i<resultList.Count;i++)
    		{
    			if (i==0)
    			{
    				result=(string)resultList[i];
    			}
    			else
    			{
    				result = result +"\n"+ (string)resultList[i];
    			}
    		}
    		return result;
    	}
     
    	void MoveAssetTo(string ToFolder, int mode)
    	{
    		if (resultList==null) return;
    		if (useSelect)
    		{
    			if (Selection.objects.Length>0)
    				ToFolder=AssetDatabase.GetAssetPath(Selection.objects[Selection.objects.Length-1]);
    		}
    		else
    		{
    			if (ToFolder=="") return;
    		}
    //		Debug.Log("ToFolder= "+ToFolder);
    		string folderStr = GetFileName(ToFolder, '/');
    		if (IncludeChar(folderStr, '.'))
    		{
    			Debug.LogWarning("Select or Input a path");
    			return;
    		}
    		if (folderStr!="") ToFolder=ToFolder+'/';
    		Debug.Log("resultList.Count= "+resultList.Count);
    		for (int i=0;i<resultList.Count;i++)
    		{
    			string toFileStr = ToFolder+GetFileName((string)resultList[i], '/');
    			Debug.Log("toFileStr= "+toFileStr);
    			if (mode==0)
    			{
    				string resultStr = AssetDatabase.MoveAsset((string)resultList[i], toFileStr);
    				if (resultStr!="")
    				{
    					Debug.LogWarning(resultStr);
    				}
    			}
    			else if (mode==1)
    			{
    				bool resultBool = AssetDatabase.CopyAsset((string)resultList[i], toFileStr);
    				if (resultBool==false)
    				{
    					Debug.LogWarning("Copy Assets Fail");
    				}
    			}
    		}
     
    		resultList.Clear();
    		findObString="";
    		Selection.objects=new Object[0];
    		AssetDatabase.Refresh();
    	}
     
     
    	string GetFileName (string fileStr, char separator)
    	{
    		ArrayList resultList = new ArrayList();
    		for (int i=0;i<fileStr.Length;i++)
    		{
    			if (fileStr[i]!=separator)
    			{
    				resultList.Add(fileStr[i]);
    			}
    			else
    			{
    				resultList.Clear();
    			}
    		}
    		string result = "";
    		for (int j=0;j<resultList.Count;j++)
    		{
    			result=result+(char)resultList[j];
    		}
    		return result;
    	}
     
    	bool IncludeChar (string fileStr, char checkChar)
    	{
    		bool result=false;
    		for (int i=0;i<fileStr.Length;i++)
    		{
    			if (fileStr[i]==checkChar)
    			{
    				result=true;
    				break;
    			}
    		}
    		return result;
    	}
     
}
}
