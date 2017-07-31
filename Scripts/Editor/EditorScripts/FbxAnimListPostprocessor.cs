// Original url: http://wiki.unity3d.com/index.php/FbxAnimListPostprocessor
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/FbxAnimListPostprocessor.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Pat AfterMoon
With the help of resources from the Unity wiki, forum and IRC channel. 
Contents [hide] 
1 Description 
2 Usage 
3 C# - FbxAnimListPostprocessor.cs 
4 Alternative for Multiple Models Using Same Animation List 

DescriptionThis script use an external text file to import a list of splitted animations for FBX 3D models.
When Importing or Reimporting a FBX file, the script will search a text file with the same name and the ".txt" extension. 
File format: one line per animation clip "firstFrame-lastFrame loopFlag animationName" The keyworks "loop" or "noloop" are optional.
Example:
0-50 loop Move forward
100-190 die 
UsagePlace this script in YourProject/Assets/Editor.
The Regex expression in the ParseAnimFile function can be modified to manage other file format (CSV files with TAB or comma ...etc.). 
C# - FbxAnimListPostprocessor.cs// FbxAnimListPostprocessor.cs : Use an external text file to import a list of 
// splitted animations for FBX 3D models.
//
// Put this script in your "Assets/Editor" directory. When Importing or 
// Reimporting a FBX file, the script will search a text file with the 
// same name and the ".txt" extension.
// File format: one line per animation clip "firstFrame-lastFrame loopFlag animationName"
// The keyworks "loop" or "noloop" are optional.
// Example:
// 0-50 loop Move forward
// 100-190 die
 
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;
 
public class FbxAnimListPostprocessor : AssetPostprocessor
{
    public void  OnPreprocessModel()
    {
        if (Path.GetExtension(assetPath).ToLower() == ".fbx"
            && !assetPath.Contains("@"))
        {
            try
            {
                // Remove 6 chars because dataPath and assetPath both contain "assets" directory
                string fileAnim = Application.dataPath + Path.ChangeExtension(assetPath, ".txt").Substring(6);
                StreamReader file = new StreamReader(fileAnim);
 
                string sAnimList = file.ReadToEnd();
                file.Close();
 
                if (EditorUtility.DisplayDialog("FBX Animation Import from file",
                    fileAnim, "Import", "Cancel"))
                {
                    System.Collections.ArrayList List = new ArrayList();
                    ParseAnimFile(sAnimList, ref List);
 
                    ModelImporter modelImporter = assetImporter as ModelImporter;
                    modelImporter.splitAnimations = true;
                    modelImporter.clipAnimations = (ModelImporterClipAnimation[])
                        List.ToArray(typeof(ModelImporterClipAnimation));
 
                    EditorUtility.DisplayDialog("Imported animations",
                        "Number of imported clips: "
                        + modelImporter.clipAnimations.GetLength(0).ToString(), "OK");
                }
            }
            catch {}
            // (Exception e) { EditorUtility.DisplayDialog("Imported animations", e.Message, "OK"); }
        }
    }
 
    void ParseAnimFile(string sAnimList, ref System.Collections.ArrayList List)
    {
        Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);
 
        Match match = regexString.Match(sAnimList, 0);
        while (match.Success)
        {
            ModelImporterClipAnimation clip = new ModelImporterClipAnimation();
 
            if (match.Groups["firstFrame"].Success)
            {
                clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);
            }
            if (match.Groups["lastFrame"].Success)
            {
                clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);
            }
            if (match.Groups["loop"].Success)
            {
                clip.loop = match.Groups["loop"].Value == "loop";
            }
            if (match.Groups["name"].Success)
            {
                clip.name = match.Groups["name"].Value;
            }
 
            List.Add(clip);
 
            match = regexString.Match(sAnimList, match.Index + match.Length);
        }
    }
}Alternative for Multiple Models Using Same Animation ListSmall change by Thomas Hentschel Lund 
Here is a small change that makes it easier to load animations for multiple models without having redundant copies of the animation list. 
A lot of the 3drt packs contain several versions of the "same" fbx. This fix takes a animation list file called animations.txt in the same folder as the fbx and uses that instead. 
To do so change 
string fileAnim = Application.dataPath + Path.ChangeExtension(assetPath, ".txt").Substring(6);to 
string fileAnim = Application.dataPath + Path.GetDirectoryName(assetPath).Substring(6) + "/animations.txt";Thats it. 
}
