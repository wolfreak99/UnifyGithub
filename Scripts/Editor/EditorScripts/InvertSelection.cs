/*************************
 * Original url: http://wiki.unity3d.com/index.php/InvertSelection
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/InvertSelection.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Mift (mift) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description This script inverts the editors hierarchy selection. 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
    Unity main menu->Selection->Invert 
    C# - InvertSelection.cs using System;
    using UnityEngine;
    using UnityEditor;
     
    using System.Collections.Generic;
     
    public class InvertSelection : ScriptableWizard {
     
     
        [MenuItem ("Selection/Invert")]
        static void static_InvertSelection() { 
     
    		List< GameObject > oldSelection = new List< GameObject >();
    		List< GameObject > newSelection = new List< GameObject >();
     
     
    		foreach( GameObject obj in Selection.GetFiltered( typeof( GameObject ), SelectionMode.ExcludePrefab ) )
    			oldSelection.Add( obj );
     
    		foreach( GameObject obj in FindObjectsOfType( typeof( GameObject ) ) )
    		{
    			if ( !oldSelection.Contains( obj ) )
    				newSelection.Add( obj );
    		}
     
    		Selection.objects = newSelection.ToArray();
     
        }
     
}
}
