/*************************
 * Original url: http://wiki.unity3d.com/index.php/LoadSceneAdditive
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/LoadSceneAdditive.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Found this in forums, added a little extra. Copies one scene into another. Originally by cyb3rmaniak and kino, thanks guys. 
    @MenuItem("File/Load Scene [Additive]") 
    static function Apply () 
    { 
    	var strScenePath : String = AssetDatabase.GetAssetPath(Selection.activeObject); 
    	if (strScenePath == null) 
    	{
    		EditorUtility.DisplayDialog("Select Scene", "You Must Select a Scene first!", "Ok"); 
    		return; 
    	} 
    	if (!strScenePath.Contains(".unity"))
    	{
    		EditorUtility.DisplayDialog("Select Scene","You Must Select a SCENE first, you selected "+strScenePath, "Ok"); 
    		return; 
    	}
     
    	Debug.Log("Opening " + strScenePath + " additively"); 
    	EditorApplication.OpenSceneAdditive(strScenePath); 
    	return; 
    }
}
