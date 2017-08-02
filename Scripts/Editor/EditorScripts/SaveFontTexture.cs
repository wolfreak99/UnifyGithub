/*************************
 * Original url: http://wiki.unity3d.com/index.php/SaveFontTexture
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/SaveFontTexture.cs
 * File based on original modification date of: 24 June 2013, at 19:08. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 SaveFontTexture.js 
    4 SaveFontTexture.cs 
    
    DescriptionIf you've ever wanted to get at the auto-generated font bitmap that Unity creates when importing vector fonts, now's your chance. SaveFontTexture saves that bitmap as a .png file so you can edit it, add whatever whizbang effects you want to it, and then use it as a colored font, using the TexturedFont shader and a GUIText object. 
    Note: this script isn't necessary in Unity 4, since in that case you can click on the font and choose "Create Editable Copy" in the gear menu in the inspector. 
    Usage Script Setup 
    You must place the script in a folder named Editor in your project's Assets folder for it to work properly. When this is done, you will then have a Save Font Texture... menu item in the Assets menu. Find the TrueType font you want in Unity and open the root, which reveals the auto-generated material and texture. Click once on the texture (called "font Texture") and select the Save Font Texture menu item. If the texture appears black in the preview window then you must change the Character setting in the inspector from Dynamic to another, such as Unicode. Choose a file name and location in the file dialog, and hit save. (Technically this script will work for any texture in Alpha8 format, if for some reason you wanted to save it as a .png file.) Note that the character set must be something other than dynamic. 
    
    Customizing and Using Font Textures 
    1. Get the TrueType font that you want to modify in Unity at the desired Font Size (so that you can use Pixel Correct in the GUI_Text object that will use it for crisp rendering). 
    2. Use this script (installed as described above) to save the texture Unity generates automatically for the font. 
    3. Edit that texture file in Photoshop to use in your project. 
    4. Create a new Material in Unity. 
    5. Set the texture of the material to the modified texture. 
    6. Set the shader of the material to the TexturedFont Shader. 
    7. In any scene GUI_Text objects that will use the custom font, apply the original font to the object then change its material to the custom material created in steps 4-6. 
    8. In the Inspector, Wrap Mode: Clamp the texture and set its Texture Type to "GUI" (which disables MIP Maps). 
    SaveFontTexture.js import System.IO; 
    @MenuItem ("Assets/Save Font Texture...")
     
    static function SaveFontTexture () {
    	var tex = Selection.activeObject as Texture2D;
    	if (tex == null) {
    		EditorUtility.DisplayDialog("No texture selected", "Please select a texture", "Cancel");
    		return;
    	}
    	if (tex.format != TextureFormat.Alpha8) {
    		EditorUtility.DisplayDialog("Wrong format", "Texture must be in uncompressed Alpha8 format", "Cancel");
    		return;
    	}
     
    	// Convert Alpha8 texture to ARGB32 texture so it can be saved as a PNG
    	var texPixels = tex.GetPixels();
    	var tex2 = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
    	tex2.SetPixels(texPixels);
     
    	// Save texture (WriteAllBytes is not used here in order to keep compatibility with Unity iPhone)
    	var texBytes = tex2.EncodeToPNG();
    	var fileName = EditorUtility.SaveFilePanel("Save font texture", "", "font Texture", "png");
    	if (fileName.Length > 0) {
    		var f : FileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
    		var b : BinaryWriter = new BinaryWriter(f);
    		for (var i = 0; i < texBytes.Length; i++) b.Write(texBytes[i]);
    		b.Close(); 
    	}
     
    	DestroyImmediate(tex2);
    }
    
    SaveFontTexture.cs /* CSharp version of above JavaScript file by MicroEyes */
     
    using UnityEditor;  //Obviosly its an editor script.
    using UnityEngine;  //Unity base Texture, Debug, Color, etc.
    using System;       //Exception & Environment
    using System.IO;    //FileStream, BinaryWritter.
     
    public  class SaveFontTexture  {
     
        [MenuItem("Assets/Save Font Texture...")]
        static void Init()
        {
            Texture2D l_texture = null;
     
            try
            {
                l_texture = (Texture2D)Selection.activeObject;
            }
            catch (InvalidCastException e)
            {
                Debug.Log("Selected Object is not a texture: " + Environment.NewLine + e.Message);
            }
     
            if (l_texture == null)
            {
                EditorUtility.DisplayDialog("No texture selected", "Please select a texture", "Cancel");
                return;
            }
     
            if (l_texture.format != TextureFormat.Alpha8)
            {
                EditorUtility.DisplayDialog("Wrong format", "Texture must be in uncompressed Alpha8 format", "Cancel");
                return;
            }
     
            Color[] l_pixels = l_texture.GetPixels();
     
            Texture2D l_newTexture = new Texture2D(l_texture.width, l_texture.height, TextureFormat.ARGB32, false);
     
            l_newTexture.SetPixels(l_pixels);
     
            var texBytes = l_newTexture.EncodeToPNG();
     
            string fileName = EditorUtility.SaveFilePanel("Save font texture", "", "font Texture", "png");
     
            if (fileName.Length > 0) {
    		FileStream f = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
    		BinaryWriter b = new BinaryWriter(f);
    		for (var i = 0; i < texBytes.Length; i++) b.Write(texBytes[i]);
    		b.Close(); 
    	}
     
    	  UnityEngine.Object.DestroyImmediate(l_texture);
     
        }
}
}
