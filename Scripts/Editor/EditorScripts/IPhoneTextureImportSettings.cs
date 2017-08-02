/*************************
 * Original url: http://wiki.unity3d.com/index.php/IPhoneTextureImportSettings
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/IPhoneTextureImportSettings.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Martin Schultz (MartinSchultz) 
 *
 * Description 
 *   
 * Usage 
 *   
 * Screenshot 
 *   
 * C# - ChangeTextureImportSettings.cs 
 *   
 * C# - ChangeTextureImportSettings.cs for Unity 3 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Adds under the menu Custom→iPhone a way to change for multiple selected textures the import settings in one step. Idea was to have the same choices for multiple texture files as you would have if you open the import settings of a single texture. Currently the most often used import settings are editable: Texture Format (same amount and order as in Unity), enable/disable MipMap and changing the maximum texture size. 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
    Select some textures in the project window and select from the Custom→iPhone menu the modification you want to apply to the selected textures. 
    Screenshot  
    
    
    C# - ChangeTextureImportSettings.cs using UnityEngine;
    using UnityEditor;
     
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // iPhone specific Texture import settings modifier.
    //
    // Modifies all selected textures in the project window and applies the requested modification on the 
    // textures. Idea was to have the same choices for multiple files as you would have if you open the 
    // import settings of a single texture. Put this into Assets/Editor and once compiled by Unity you find
    // the new functionality in Custom -> iPhone. Enjoy! :-)
    // 
    // Based on the great work of benblo in this thread: 
    // http://forum.unity3d.com/viewtopic.php?t=16079&start=0&postdays=0&postorder=asc&highlight=textureimporter
    // 
    // Developed by Martin Schultz, Decane in January 2009
    // e-mail: ms@decane.net
    //
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ChangeTextureImportSettings : ScriptableObject {
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/Auto (RGBA Compressed PVRTC 4 bits)")]
        static void ChangeTextureFormat_Automatic() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.Automatic);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed PVRTC 2 bits")]
        static void ChangeTextureFormat_RGB_PVRTC_2bits() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_2BPP_RGB);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGBA Compressed PVRTC 2 bits")]
        static void ChangeTextureFormat_RGBA_PVRTC_2bits() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_2BPP_RGBA);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed PVRTC 4 bits")]
        static void ChangeTextureFormat_RGB_PVRTC_4bits() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_4BPP_RGB);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGBA Compressed PVRTC 4 bits")]
        static void ChangeTextureFormat_RGBA_PVRTC_4bits() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_4BPP_RGBA);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed DXT1")]
        static void ChangeTextureFormat_RGB_DXT1() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.DXT1);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed DXT5")]
        static void ChangeTextureFormat_RGB_DXT5() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.DXT5);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGB 16 bit")]
        static void ChangeTextureFormat_RGB_16bit() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.RGB16);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGB 24 bit")]
        static void ChangeTextureFormat_RGB_24bit() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.RGB24);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/Alpha 8 bit")]
        static void ChangeTextureFormat_Alpha_8bit() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.Alpha8);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGBA 16 bit")]
        static void ChangeTextureFormat_RGBA_16bit() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.ARGB16);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Texture Format/RGBA 32 bit")]
        static void ChangeTextureFormat_RGBA_32bit() { 
    		SelectedChangeTextureFormatSettings(TextureImporterFormat.ARGB32);
    	}
     
    	// ----------------------------------------------------------------------------
     
    	[MenuItem ("Custom/iPhone/Change Max Texture Size/32")]
        static void ChangeTextureSize_32() { 
    		SelectedChangeMaxTextureSize(32);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Max Texture Size/64")]
        static void ChangeTextureSize_64() { 
    		SelectedChangeMaxTextureSize(64);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Max Texture Size/128")]
        static void ChangeTextureSize_128() { 
    		SelectedChangeMaxTextureSize(128);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Max Texture Size/256")]
        static void ChangeTextureSize_256() { 
    		SelectedChangeMaxTextureSize(256);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Max Texture Size/512")]
        static void ChangeTextureSize_512() { 
    		SelectedChangeMaxTextureSize(512);
    	}
     
    	[MenuItem ("Custom/iPhone/Change Max Texture Size/1024")]
        static void ChangeTextureSize_1024() { 
    		SelectedChangeMaxTextureSize(1024);
    	}
     
        // ----------------------------------------------------------------------------
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/None")]
        static void SelectedChangeNonePowerOfTwo_None() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.None);
        }   
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/Nearest")]
        static void SelectedChangeNonePowerOfTwo_Nearest() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.ToNearest);
        } 
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/Larger")]
        static void SelectedChangeNonePowerOfTwo_Larger() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.ToLarger);
        } 
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/Smaller")]
        static void SelectedChangeNonePowerOfTwo_Smaller() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.ToSmaller);
        }  
     
    	// ----------------------------------------------------------------------------
     
    	[MenuItem ("Custom/iPhone/Change MipMap/Enable MipMap")]
        static void ChangeMipMap_On() { 
    		SelectedChangeMimMap(true);
    	}
     
    	[MenuItem ("Custom/iPhone/Change MipMap/Disable MipMap")]
        static void ChangeMipMap_Off() { 
    		SelectedChangeMimMap(false);
    	}
     
    	// ----------------------------------------------------------------------------
     
        static void SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale newScale) {
     
            Object[] textures = GetSelectedTextures();
            foreach (Texture2D texture in textures)  {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                textureImporter.npotScale = newScale; 
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate );
            }
        } 
     
    	static void SelectedChangeMimMap(bool enabled) { 
     
    		Object[] textures = GetSelectedTextures(); 
    		Selection.objects = new Object[0];
    		foreach (Texture2D texture in textures)  {
    			string path = AssetDatabase.GetAssetPath(texture); 
    			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
    			textureImporter.mipmapEnabled = enabled;	
    			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate ); 
    		}
    	}
     
    	static void SelectedChangeMaxTextureSize(int size) { 
     
    		Object[] textures = GetSelectedTextures(); 
    		Selection.objects = new Object[0];
    		foreach (Texture2D texture in textures)  {
    			string path = AssetDatabase.GetAssetPath(texture); 
    			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
    			textureImporter.maxTextureSize = size;	
    			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate ); 
    		}
    	}
     
    	static void SelectedChangeTextureFormatSettings(TextureImporterFormat newFormat) { 
     
    		Object[] textures = GetSelectedTextures(); 
    		Selection.objects = new Object[0];		
    		foreach (Texture2D texture in textures)  {
    			string path = AssetDatabase.GetAssetPath(texture); 
    			//Debug.Log("path: " + path);
    			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
    			textureImporter.textureFormat = newFormat;	
    			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate ); 
    		}
    	}
     
    	static Object[] GetSelectedTextures() 
    	{ 
    		return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets); 
    	}
    }
    
    C# - ChangeTextureImportSettings.cs for Unity 3 using UnityEngine;
    using UnityEditor;
     
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // iPhone specific Texture import settings modifier.
    //
    // Modifies all selected textures in the project window and applies the requested modification on the 
    // textures. Idea was to have the same choices for multiple files as you would have if you open the 
    // import settings of a single texture. Put this into Assets/Editor and once compiled by Unity you find
    // the new functionality in Custom -> iPhone. Enjoy! :-)
    // 
    // Based on the great work of benblo in this thread: 
    // http://forum.unity3d.com/viewtopic.php?t=16079&start=0&postdays=0&postorder=asc&highlight=textureimporter
    // 
    // Developed by Martin Schultz, Decane in January 2009
    // e-mail: ms@decane.net
    //
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ChangeTextureImportSettings : ScriptableObject {
     
        [MenuItem ("Custom/iPhone/Change Texture Format/Auto (RGBA Compressed PVRTC 4 bits)")]
        static void ChangeTextureFormat_Automatic() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.AutomaticCompressed);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed PVRTC 2 bits")]
        static void ChangeTextureFormat_RGB_PVRTC_2bits() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGB2);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGBA Compressed PVRTC 2 bits")]
        static void ChangeTextureFormat_RGBA_PVRTC_2bits() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGBA2);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed PVRTC 4 bits")]
        static void ChangeTextureFormat_RGB_PVRTC_4bits() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGB4);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGBA Compressed PVRTC 4 bits")]
        static void ChangeTextureFormat_RGBA_PVRTC_4bits() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGBA4);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed DXT1")]
        static void ChangeTextureFormat_RGB_DXT1() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.DXT1);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGB Compressed DXT5")]
        static void ChangeTextureFormat_RGB_DXT5() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.DXT5);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGB 16 bit")]
        static void ChangeTextureFormat_RGB_16bit() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.RGB16);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGB 24 bit")]
        static void ChangeTextureFormat_RGB_24bit() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.RGB24);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/Alpha 8 bit")]
        static void ChangeTextureFormat_Alpha_8bit() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.Alpha8);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGBA 16 bit")]
        static void ChangeTextureFormat_RGBA_16bit() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.ARGB16);
        }
     
        [MenuItem ("Custom/iPhone/Change Texture Format/RGBA 32 bit")]
        static void ChangeTextureFormat_RGBA_32bit() { 
            SelectedChangeTextureFormatSettings(TextureImporterFormat.ARGB32);
        }
     
        // ----------------------------------------------------------------------------
     
        [MenuItem ("Custom/iPhone/Change Max Texture Size/32")]
        static void ChangeTextureSize_32() { 
            SelectedChangeMaxTextureSize(32);
        }
     
        [MenuItem ("Custom/iPhone/Change Max Texture Size/64")]
        static void ChangeTextureSize_64() { 
            SelectedChangeMaxTextureSize(64);
        }
     
        [MenuItem ("Custom/iPhone/Change Max Texture Size/128")]
        static void ChangeTextureSize_128() { 
            SelectedChangeMaxTextureSize(128);
        }
     
        [MenuItem ("Custom/iPhone/Change Max Texture Size/256")]
        static void ChangeTextureSize_256() { 
            SelectedChangeMaxTextureSize(256);
        }
     
        [MenuItem ("Custom/iPhone/Change Max Texture Size/512")]
        static void ChangeTextureSize_512() { 
            SelectedChangeMaxTextureSize(512);
        }
     
        [MenuItem ("Custom/iPhone/Change Max Texture Size/1024")]
        static void ChangeTextureSize_1024() { 
            SelectedChangeMaxTextureSize(1024);
        }
     
        // ----------------------------------------------------------------------------
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/None")]
        static void SelectedChangeNonePowerOfTwo_None() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.None);
        }   
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/Nearest")]
        static void SelectedChangeNonePowerOfTwo_Nearest() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.ToNearest);
        } 
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/Larger")]
        static void SelectedChangeNonePowerOfTwo_Larger() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.ToLarger);
        } 
     
        [MenuItem ("Custom/iPhone/Change None Power of Two/Smaller")]
        static void SelectedChangeNonePowerOfTwo_Smaller() {
            SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale.ToSmaller);
        }  
     
        // ----------------------------------------------------------------------------
     
        [MenuItem ("Custom/iPhone/Change MipMap/Enable MipMap")]
        static void ChangeMipMap_On() { 
            SelectedChangeMimMap(true);
        }
     
        [MenuItem ("Custom/iPhone/Change MipMap/Disable MipMap")]
        static void ChangeMipMap_Off() { 
            SelectedChangeMimMap(false);
        }
     
        // ----------------------------------------------------------------------------
     
        static void SelectedChangeNonePowerOfTwo(TextureImporterNPOTScale newScale) {
     
            Object[] textures = GetSelectedTextures();
            foreach (Texture2D texture in textures)  {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                textureImporter.npotScale = newScale; 
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate );
            }
        } 
     
        static void SelectedChangeMimMap(bool enabled) { 
     
            Object[] textures = GetSelectedTextures(); 
            Selection.objects = new Object[0];
            foreach (Texture2D texture in textures)  {
                string path = AssetDatabase.GetAssetPath(texture); 
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
                textureImporter.mipmapEnabled = enabled;    
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate ); 
            }
        }
     
        static void SelectedChangeMaxTextureSize(int size) { 
     
            Object[] textures = GetSelectedTextures(); 
            Selection.objects = new Object[0];
            foreach (Texture2D texture in textures)  {
                string path = AssetDatabase.GetAssetPath(texture); 
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
                textureImporter.maxTextureSize = size;  
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate ); 
            }
        }
     
        static void SelectedChangeTextureFormatSettings(TextureImporterFormat newFormat) { 
     
            Object[] textures = GetSelectedTextures(); 
            Selection.objects = new Object[0];    
            foreach (Texture2D texture in textures)  {
                string path = AssetDatabase.GetAssetPath(texture); 
                //Debug.Log("path: " + path);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
                textureImporter.textureFormat = newFormat;  
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate ); 
            }
        }
     
        static Object[] GetSelectedTextures() 
        { 
            return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets); 
        }
}
}
