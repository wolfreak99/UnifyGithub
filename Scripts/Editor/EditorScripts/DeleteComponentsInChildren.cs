/*************************
 * Original url: http://wiki.unity3d.com/index.php/DeleteComponentsInChildren
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/DeleteComponentsInChildren.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Timo (T) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description This Editor script helps to delete a special type of components which are attached to the children of the selected Gameobject. 
    Usage You must place the script in a folder named Editor in your projects Assets folder for it to work properly. 
    Open the script with Unitron and change the string "GrassOnMeshGenerator" to a string of the type you want to delete... 
    C# - DeleteComponentsInChildren.cs using UnityEngine;
    using UnityEditor;
    using System.Collections;
     
     
    public class DeleteComponentsInChildren : ScriptableObject
    {
        [MenuItem ( "GameObject/Delete Components in Children" )]
        static void MenuDeleteComponentsInChildren()
        {
            	Transform[] allTransforms = Selection.GetTransforms( SelectionMode.Deep ); //Get all Children
     
           	foreach( Transform eachTransform in allTransforms )
           	{
           		//Change GrassOnMeshGenerator string to delete other Types
           		if ( eachTransform.GetComponent ( "GrassOnMeshGenerator" ) != null )
           		{
           			//Change GrassOnMeshGenerator string to delete other Types
           			Object.DestroyImmediate ( eachTransform.GetComponent (  "GrassOnMeshGenerator" ) );
           		}
           	}
        }
}
}
