/*************************
 * Original url: http://wiki.unity3d.com/index.php/Textmeshextension
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/Textmeshextension.cs
 * File based on original modification date of: 7 April 2014, at 20:55. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    Description In the current version of Unity (4.3.4) the component TextMesh contains a text field that does not accept enter as new line. This script creates a new field, above the standard, that allows new lines to be entered by pressing enter. 
    NOTE: This class is only necessary if you are unable to create new lines by pressing shift+enter; See the Textmeshextension Talk page for details. 
    Script and Usage Create a script named "TextMeshExtension.js" with the contents as shown below and place it inside the "Editor" folder. 
     
    @CustomEditor (TextMesh)
    class TextMeshExtension extends Editor{
        function OnInspectorGUI () {
            target.text = EditorGUILayout.TextArea(target.text);
            DrawDefaultInspector();
        }
    }
}
