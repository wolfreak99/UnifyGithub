// Original url: http://wiki.unity3d.com/index.php/RewriteReferences
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/RewriteReferences.cs
// File based on original modification date of: 16 August 2016, at 10:27. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Fredrik Ludvigsen (Steinbitglis) 
Contents [hide] 
1 Description 
2 Usage 
3 Example 
4 C# - ReferenceUtil.cs 

Description This script creates a command which rewrites a reference throughout a whole project. 
Usage GetRewriteCommand() creates a command, based on git, xargs and sed, which can effortlessly rewrite references in a Unity project. 
This saves a lot of pain associated with having to open scenes, reconnecting references and saving the scenes again. 
The patch becomes minimal, since you haven't even opened them in Unity. This makes the patch merge more easily too. 
Example This example shows how to update a whole list of references by writing all the commands to a file in the project root. 
To use this command file with git bash or your favorite unix-like terminal: 
   source database_update_command.sh
var sb = new StringBuilder();
for (int i = 0; i < migrationSources.Count; i++) {
    sb.AppendLine(ReferenceUtil.GetRewriteCommand(migrationSources[i], migrationTargets[i], "unity"));
    sb.AppendLine(ReferenceUtil.GetRewriteCommand(migrationSources[i], migrationTargets[i], "prefab"));
}
System.IO.File.WriteAllText("database_update_command.sh", sb.ToString());C# - ReferenceUtil.cs using System;
using UnityEngine;
using UnityEditor;
 
using Object = UnityEngine.Object;
 
public class ReferenceUtil {
    public static string GetRewriteCommand(Object oldObject, Object newObject, string fileType) {
        var oldGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(oldObject));
        var newGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newObject));
 
        var oldSerializedObject = new SerializedObject (oldObject);
        var newSerializedObject = new SerializedObject (newObject);
 
        var inspectorModeInfo = typeof(SerializedObject).GetProperty ("inspectorMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
 
        inspectorModeInfo.SetValue (oldSerializedObject, InspectorMode.Debug, null);
        inspectorModeInfo.SetValue (newSerializedObject, InspectorMode.Debug, null);
 
        var oldId = oldSerializedObject.FindProperty ("m_LocalIdentfierInFile").intValue;
        var newId = newSerializedObject.FindProperty ("m_LocalIdentfierInFile").intValue;
 
        return string.Format("git grep -z -l 'fileID: {0}, guid: {1}' -- '*.{2}' | xargs -r0 sed -i 's/fileID: {0}, guid: {1}/fileID: {3}, guid: {4}/g'", oldId, oldGUID, fileType, newId, newGUID);
    }
}
}
