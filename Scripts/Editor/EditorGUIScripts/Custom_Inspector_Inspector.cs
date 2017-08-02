/*************************
 * Original url: http://wiki.unity3d.com/index.php/Custom_Inspector_Inspector
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/Custom_Inspector_Inspector.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    Put this line inside your custom inspector/editor script's OnInspector() somewhere: 
    EditorGUILayout.ObjectField(new GUIContent("Custom Editor Script"), AssetDatabase.LoadAssetAtPath("Assets/Editor/" + GetType().Name + ".cs", typeof(MonoScript)), typeof(MonoScript));
    Now, by clicking this control, you can go directly from looking at a custom inspector you are making, to editing the script of that custom inspector (which can otherwise require a lot of scrolling to find in your Project pane). 
}
