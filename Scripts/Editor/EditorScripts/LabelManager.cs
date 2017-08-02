/*************************
 * Original url: http://wiki.unity3d.com/index.php/LabelManager
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/LabelManager.cs
 * File based on original modification date of: 9 September 2013, at 07:32. 
 *
 * Author: Unicorn 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description It's a program for find assets who has special component, then you can add/remove label on them. 
    Usage You must place the script in a folder named Plugins in your project's Assets folder for it to work properly. Navigate to "Window/uLabelManager" 
    Select a asset folder, enter the name of component you want to find,Click on "Find Asset" Button,The result show as list below. Then you can add label to them/del label from them. 
    
    
    C# - uLabelManager.cs //Author : Unicorn Jim
    //Webpage : http://blog.unicoea.com/
     
    //uLabelManager by Unicorn Jim is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
     
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
     
    public class uLabelManager : EditorWindow {
     
    	string componentName = "Animation";
        string addString = "Hello World";
    	string delString = "Hello World";
    	string findString = "";
    	string findObString = "";
    	Vector2 scrollPosition1;
    	Vector2 scrollPosition2;
     
    	bool updateAtOnce=true;
    //  bool myBool = true;
    //  float myFloat = 1.23f;
     
        // Add menu named "uLabelManager" to the Window menu
        [MenuItem ("Window/uLabelManager")]
        static void Init ()
    	{
            // Get existing open window or if none, make a new one:
            uLabelManager window = (uLabelManager)EditorWindow.GetWindow (typeof (uLabelManager));
        }
     
        void OnGUI () 
    	{
    		GUILayout.Label ("uLabelManager v1.0", EditorStyles.boldLabel);
     
    		GUILayout.BeginHorizontal();
    			componentName = EditorGUILayout.TextField ("Find Component Name", componentName);
    			if (GUILayout.Button("Find Asset"))
    			{
    				findObString = FindAssetName(componentName);
    				Debug.Log("findObString= "+findObString);
    			}
    		GUILayout.EndHorizontal();
     
    		scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, GUILayout.Height(100));
    			GUILayout.TextArea(findObString);
    		GUILayout.EndScrollView();
     
    		GUILayout.BeginHorizontal();
    			addString = EditorGUILayout.TextField ("Add Label Name", addString);
    			if (GUILayout.Button("Add Label"))
    			{
    				RefreshAssetLabel(componentName, addString, true);
    			}
    		GUILayout.EndHorizontal();
     
    		GUILayout.BeginHorizontal();
    			delString = EditorGUILayout.TextField ("Del Label Name", delString);
    			if (GUILayout.Button("Del Label"))
    			{
    				RefreshAssetLabel(componentName, delString, false);
    			}
    		GUILayout.EndHorizontal();
     
    		updateAtOnce = GUILayout.Toggle(updateAtOnce, "Refresh Immediately");
     
    		if (GUILayout.Button("Find All Labels"))
    		{
    			findString = FindAssetLabel(componentName);
    		}
     
    		scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, GUILayout.Height(100));
    			GUILayout.TextArea(findString);
    		GUILayout.EndScrollView();
     
    		if (GUILayout.Button("Clear All Labels"))
    		{
    			RefreshAssetLabel(componentName, "", true);
    			findString="";
    		}
     
    //      myBool = EditorGUILayout.Toggle ("Toggle", myBool);
    //      myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
    //      EditorGUILayout.EndToggleGroup ();
        }
     
    	string FindAssetName (string findName)
    	{
    		int i;
    		Object[] selectObs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Editable);
    		ArrayList resultList = new ArrayList();
    		Debug.Log("selectObs's count= "+selectObs.Length);
     
    		for (i=0;i<selectObs.Length;i++)
    		{
    			if (selectObs[i].GetType()==typeof(GameObject) || selectObs[i].GetType().IsSubclassOf(typeof(GameObject)))
    			{
    				bool findFlag=false;
    				FindInAllChilds(((GameObject)selectObs[i]).transform, findName, out findFlag);
    				if (findFlag)
    				{
    					resultList.Add(selectObs[i]);
    					Debug.Log("obj find: "+selectObs[i].name);
    				}
    			}
    		}
     
    		Object[] resultObs = new Object[resultList.Count];
    		for (i=0;i<resultObs.Length;i++)
    		{
    			resultObs[i]=(Object)resultList[i];
    		}
    		Selection.objects=resultObs;
     
     
    		string result = "";
    		for (i=0;i<resultList.Count;i++)
    		{
    			if (i==0)
    			{
    				result=((GameObject)resultList[i]).name;
    			}
    			else
    			{
    				result = result +"\n"+ ((GameObject)resultList[i]).name;
    			}
    		}
    		return result;
    	}
     
    	void RefreshAssetLabel(string findName, string mySetName, bool addFlag)
    	{
    		int i;
    		Object[] selectObs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Editable);
    		ArrayList resultList = new ArrayList();
    		Debug.Log("selectObs's count= "+selectObs.Length);
     
    		for (i=0;i<selectObs.Length;i++)
    		{
    			if (selectObs[i].GetType()==typeof(GameObject) || selectObs[i].GetType().IsSubclassOf(typeof(GameObject)))
    			{
    				bool findFlag=false;
    				FindInAllChilds(((GameObject)selectObs[i]).transform, findName, out findFlag);
    				if (findFlag)
    				{
    					resultList.Add(selectObs[i]);
    					Debug.Log("obj find: "+selectObs[i].name);
    				}
    			}
    		}
     
    		for (i=0;i<resultList.Count;i++)
    		{
    			Object resultObj = (Object)resultList[i];
    			//add/del
    			if (mySetName!="")
    			{
    				string[] lastString = AssetDatabase.GetLabels(resultObj);
    				//add label
    				if (addFlag)
    				{
    					if (!CheckExist(lastString, mySetName))
    					{
    						AssetDatabase.SetLabels(resultObj, AddStrings(lastString, mySetName));
    						if (updateAtOnce)
    							AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(resultObj));
    					}
    				}
    				//del label
    				else
    				{
    					if (CheckExist(lastString, mySetName))
    					{
    						AssetDatabase.SetLabels(resultObj, DelStrings(lastString, mySetName));
    						if (updateAtOnce)
    							AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(resultObj));
    					}
    				}
    			}
    			//clear all
    			else
    			{
    				AssetDatabase.ClearLabels(resultObj);
    				if (updateAtOnce)
    					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(resultObj));
    			}
    		}
    	}
     
    	string FindAssetLabel (string findName)
    	{
    		int i;
    		Object[] selectObs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Editable);
    		ArrayList resultList = new ArrayList();
    		Debug.Log("selectObs's count= "+selectObs.Length);
     
    		for (i=0;i<selectObs.Length;i++)
    		{
    			if (selectObs[i].GetType()==typeof(GameObject) || selectObs[i].GetType().IsSubclassOf(typeof(GameObject)))
    			{
    				bool findFlag=false;
    				FindInAllChilds(((GameObject)selectObs[i]).transform, findName, out findFlag);
    				if (findFlag)
    				{
    					resultList.Add(selectObs[i]);
    					Debug.Log("obj find: "+selectObs[i].name);
    				}
    			}
    		}
     
    		string[] resultString=new string[]{};
    		for (i=0;i<resultList.Count;i++)
    		{
    			string[] lastString = AssetDatabase.GetLabels((Object)resultList[i]);
    			resultString=AddToStrings(resultString, lastString);
    		}
     
    		string result="";
    		for (i=0;i<resultString.Length;i++)
    		{
    			if (i==0)
    			{
    				result=resultString[i];
    			}
    			else
    			{
    				result = result +"\n"+ resultString[i];
    			}
    		}
     
    		return result;
    	}
     
    	bool CheckExist (string[] lastString, string addString)
    	{
    		bool existed = false;
    		for (int i=0;i<lastString.Length;i++)
    		{
    			if (lastString[i]==addString)
    			{
    				existed=true;
    				break;
    			}
    		}
    		return existed;
    	}
     
    	string[] AddStrings(string[] lastString, string addString)
    	{
    		string[] newString = new string[lastString.Length+1];
    		for (int i=0;i<newString.Length;i++)
    		{
    			if (i<newString.Length-1)
    			{
    				newString[i]=lastString[i];
    			}
    			else
    			{
    				newString[i]=addString;
    			}
    		}
    		return newString;
    	}
     
    	string[] DelStrings(string[] lastString, string delString)
    	{
    		ArrayList resultStringList = new ArrayList();
    		for (int i=0;i<lastString.Length;i++)
    		{
    			if (lastString[i]!=delString)
    			{
    				resultStringList.Add(lastString[i]);
    			}
    		}
    		string[] resultString=new string[resultStringList.Count];
    		for (int j=0;j<resultString.Length;j++)
    		{
    			resultString[j]=(string) resultStringList[j];
    		}
    		return resultString;
    	}
     
    	string[] AddToStrings (string[] originString, string[] newString)
    	{
    		ArrayList resultStringList = new ArrayList();
    		for (int i=0;i<originString.Length;i++)
    		{
    			if (CheckExist(newString, originString[i]))
    			{
    				newString=DelStrings(newString, originString[i]);
    			}
    			resultStringList.Add(originString[i]);
    		}
    		for (int j=0;j<newString.Length;j++)
    		{
    			resultStringList.Add(newString[j]);
    		}
    		string[] resultString=new string[resultStringList.Count];
    		for (int k=0;k<resultString.Length;k++)
    		{
    			resultString[k]=(string) resultStringList[k];
    		}
    		return resultString;
    	}
     
    	void FindInAllChilds (Transform treeSource, string findname, out bool findFlag)
    	{
    		findFlag=false;
    		if (treeSource.GetComponent(findname))
    		{
    			findFlag=true;
    			return;
    		}
    		else if (treeSource.childCount>0)
    		{
    			int i;
    			for (i=0;i<treeSource.childCount;i++)
    			{
    				FindInAllChilds(treeSource.GetChild(i), findname, out findFlag);
    				if(findFlag==true)
    				{
    					break;
    				}
    			}
    		}
    	}
     
     
}
}
