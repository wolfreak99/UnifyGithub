// Original url: http://wiki.unity3d.com/index.php/SKUManager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SKUManager.cs
// File based on original modification date of: 23 May 2014, at 15:29. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
This is an Editor script for managing SKUs inside Unity. It allows to create different SKUs for the same target build groups, changing compile time directives that would affect which code got compiled even for the same platform. 
You need to change the values in the SKU enum, and how are they associated in the constructor. I've left a couple as examples. 
 
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
 
public class SKUManager : EditorWindow {
 
    public enum SKU
    {
        WindowsFull,
        AndroidGooglePlay,
        AndroidAmazon,
        AndroidHumbleStore,
    }
 
    public SKU sku;
    private Dictionary<SKU, string> symbols = new Dictionary<SKU,string>();
    private Dictionary<SKU, BuildTargetGroup> targetGroups = new Dictionary<SKU, BuildTargetGroup>();
    private Dictionary<SKU, BuildTarget> targets = new Dictionary<SKU, BuildTarget>();
 
    public SKUManager()
    {
        // Associate preprocessor directives with the SKU
        symbols.Add(SKU.WindowsFull, "FULL_VERSION;NO_RATING");
	symbols.Add(SKU.AndroidGooglePlay, "UPGRADABLE;USE_OPENIAB");
	symbols.Add(SKU.AndroidAmazon, "UPGRADABLE;USE_OPENIAB");
        symbols.Add(SKU.AndroidHumbleStore, "NO_RATING");
 
        // Associate target groups with the SKU
        targetGroups.Add(SKU.WindowsFull, BuildTargetGroup.Standalone);
        targetGroups.Add(SKU.AndroidGooglePlay, BuildTargetGroup.Android);
        targetGroups.Add(SKU.AndroidAmazon, BuildTargetGroup.Android);
        targetGroups.Add(SKU.AndroidHumbleStore, BuildTargetGroup.Android);
 
        // Associate targets with the SKU
        targets.Add(SKU.WindowsFull, BuildTarget.StandaloneWindows);
        targets.Add(SKU.AndroidGooglePlay, BuildTarget.Android);
        targets.Add(SKU.AndroidAmazon, BuildTarget.Android);
        targets.Add(SKU.AndroidHumbleStore, BuildTarget.Android);
    }
 
    [MenuItem("Window/SKU Manager")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SKUManager));
    }
 
    void OnGUI()
    {
        GUILayout.Label("SKU Settings", EditorStyles.boldLabel);
        sku = (SKU)EditorGUILayout.EnumPopup("SKU", sku);
 
        if (GUI.Button(new Rect(15, 45, 100, 20), "Switch"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroups[sku], symbols[sku]);
            EditorUserBuildSettings.SwitchActiveBuildTarget(targets[sku]);
        }
 
        if (GUI.Button(new Rect(15, 70, 120, 20), "Set Defines only"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroups[sku], symbols[sku]);
        }
 
    }
}It could be improved by allowing to edit the SKUs from the editor itself, instead of setting them in code. 
}
