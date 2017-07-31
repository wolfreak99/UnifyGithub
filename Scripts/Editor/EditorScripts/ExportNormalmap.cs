// Original url: http://wiki.unity3d.com/index.php/ExportNormalmap
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/ExportNormalmap.cs
// File based on original modification date of: 19 March 2013, at 00:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{

Author: Eric Haines (Eric5h5) 
Description Exports a normalmap from your project as a .png file. When a grayscale texture is converted by Unity to a normalmap, opening it simply gives you the original texture. But you can use this script instead to export the actual converted texture. (Technically this will export any texture as a .png, not just normalmaps, if you wanted to do that for some reason.) 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work. 
To save a normalmap, click on the texture in your project, then select the Export Normalmap... item in the Assets menu. If the texture is not marked "Is Readable", this script will override that, but you'll get a dialog about unapplied import settings--choose "Apply" to keep the texture not readable, or Revert to change it to readable. The script will work either way. Then choose somewhere to save the normalmap. 
JavaScript - ExportNormalmap.js @MenuItem ("Assets/Export Normalmap...")
 
static function ExportNormalmap () {
	var tex = Selection.activeObject as Texture2D;
	if (tex == null) {
		EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
		return;
	}
 
	// Force the texture to be readable so that we can access its pixels
	var texPath = AssetDatabase.GetAssetPath(tex);
	var texImport : TextureImporter = AssetImporter.GetAtPath(texPath);
	if (!texImport.isReadable) {
		texImport.isReadable = true;
		AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
	}
 
	var bytes = tex.EncodeToPNG();
	var path = EditorUtility.SaveFilePanel("Save Texture", "", tex.name+"_normal.png", "png");
	if (path != "") {
		System.IO.File.WriteAllBytes(path, bytes);
		AssetDatabase.Refresh(); // In case it was saved to the Assets folder
	}
}
}
