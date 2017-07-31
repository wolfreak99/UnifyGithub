// Original url: http://wiki.unity3d.com/index.php/Config_loading
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/CodeSnippets/Config_loading.cs
// File based on original modification date of: 2 April 2017, at 12:23. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
Author: Fredrik Ludvigsen (Steinbitglis) 
Contents [hide] 
1 Description 
2 Usage 
3 ConfigAsset.cs 
4 PlaymodeConfigLoader.cs 

DescriptionUsing the pattern means that you can have a global object, that is always loaded in playmode, but not otherwise. 
You should also put your asset in PlayerSettings.preloadedAssets, to get the same behaviour in builds. 
This is the best way to load global data, because they are loaded effectively, and before anything else. 
PS: Using Resources.Load, you can obtain something similar, but much less performant, and more complex to set up. 
Usage - Put PlaymodeConfigLoader.cs in some editor folder (Assets/Scripts/Editor, Assets/Editor, etc.).
- Put ConfigAsset.cs in any other folder.
- Create your config asset (from the menu, Assets->Create->ConfigAsset).
- Edit ConfigAsset.cs to suit your needs.
- Edit PlaymodeConfigLoader.cs to suit your needs. (Adjust the guid, or hard code paths).
- Populate the asset with data.
- Use ConfigAsset.config from other scripts to access your config data :-)
 
 
Enjoy effortless config loading! 
ConfigAsset.cs using UnityEngine;
 
[CreateAssetMenu]
public class ConfigAsset : ScriptableObject {
    public static ConfigAsset config;
 
    // ... stash global data here
 
#if !UNITY_EDITOR
    private void OnEnable() {
        config = this;
    }
#endif
}PlaymodeConfigLoader.cs using UnityEngine;
using UnityEditor;
 
[InitializeOnLoad]
public class PlaymodeConfigLoader {
 
    static PlaymodeConfigLoader() {
        EditorApplication.playmodeStateChanged += LoadOrUnloadIfPlaying;
    }
 
    private static void LoadOrUnloadIfPlaying() {
        if (EditorApplication.isPlaying)
            Load(EditorApplication.isPlayingOrWillChangePlaymode);
    }
 
    private static void Load(bool loadIn) {
        if (loadIn) {
            if (ConfigAsset.config == null) {
                // string guid = AssetDatabase.AssetPathToGUID("Assets/Configs/ConfigAsset.asset"); // GUID can also be found in the asset meta file
                string guid = "7758d97879949d349a517932ac1c4c6c";
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ConfigAsset.config = AssetDatabase.LoadAssetAtPath<ConfigAsset>(assetPath);
            }
        } else {
            if (ConfigAsset.config != null) {
                Resources.UnloadAsset(ConfigAsset.config);
                ConfigAsset.config = null;
            }
        }
    }
 
}
}
