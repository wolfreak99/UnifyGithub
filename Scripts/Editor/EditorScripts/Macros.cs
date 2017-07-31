// Original url: http://wiki.unity3d.com/index.php/Macros
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Macros.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Matthew Miner 
Description Allows execution of arbitrary code snippets in the editor. This makes use of the undocumented UnityEditor.Macros.MacroEvaluator class, so use at your own risk. 
Usage Place the script inside the Editor folder. Open the editor window by choosing Window > Macros. Write your code in the text area then click the Execute button. See this blog post for examples. 
C# - Macros.cs using UnityEditor;
using UnityEditor.Macros;
using UnityEngine;
 
/// <summary>
/// Allows use of the undocumented MacroEvaluator class. Use at your own risk.
/// </summary>
public class Macros : EditorWindow
{
	string macro = "";
 
	/// <summary>
	/// Adds a menu named "Macros" to the Window menu.
	/// </summary>
	[MenuItem ("Window/Macros")]
	static void Init () {
		CreateInstance<Macros>().ShowUtility();
	}
 
	void OnGUI () {
		macro = EditorGUILayout.TextArea(macro, GUILayout.ExpandHeight(true));
 
		if (GUILayout.Button("Execute")) {
			MacroEvaluator.Eval(macro);
		}
	}
}
}
