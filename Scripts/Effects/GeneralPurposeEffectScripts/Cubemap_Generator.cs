// Original url: http://wiki.unity3d.com/index.php/Cubemap_Generator
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/Cubemap_Generator.cs
// File based on original modification date of: 19 November 2014, at 13:51. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Will Traxler 
These two brief scripts will allow you to take the output from a camera and convert it into a cubemap which can be used to present reflections etc. The reflections are not realtime. This is based on reference examples provided by Unity. 
The two scripts are used as follows: 
Cubemapper.cs - Attaches to an empty gameobject. Responsible for generating the cubemap. Requires a camera which serves as the source for the cubemap and also an integer cubemap resolution (ie 128,256,512 etc). 
CubemapperUtil.cs - Attaches to an object containing the material which will receive the cubemap. Takes a cubemapper object (see above) input which supplies the cubemap texture. This arrangement is slightly complicated but allows the cubemap to be generated only once and shared to many objects. 
CubemapperUtil also takes an array of keywords which are used to seek out materials containing these keywords, and submit the cubemap as input. For example, submit the shader keyword '_Cubemap' and any shader containing this keyword will have the cubemap set. 


Cubemapper.csusing UnityEngine;
using System.Collections;
 
public class Cubemapper : MonoBehaviour {
	/*
	 * Creates a cubemap from a camera and feeds it to a material
	 */
 
 
	public Camera sourceCamera;
	public int cubeMapRes					= 128;
	public bool createMipMaps				= false;
 
	RenderTexture renderTex;
 
 
 
	public RenderTexture GetRenderTexture() {
 
                if (renderTex != null) return renderTex;
 
		renderTex = new RenderTexture(cubeMapRes, cubeMapRes, 16);
		renderTex.isCubemap = true;
		renderTex.hideFlags = HideFlags.HideAndDontSave;
		renderTex.generateMips = createMipMaps;
		sourceCamera.RenderToCubemap(renderTex);
		return renderTex;
	}
 
 
}

CubemapperUtil.csusing UnityEngine;
using System.Collections;
 
public class CubemapperUtil : MonoBehaviour {
	/*
	 * Attach to gameobject to set cubemap generated by cubemapper object
	 * 
	 * Scans each material for keywords and sets the cubemap accordingly
	 * 
	 */
 
	public Cubemapper thisCubemapper;			//Cubemapper reference
	public string[] cubemapKeywords;			//Shader keywords to set the cubemap
 
	bool isCubemapSet					= false;
 
 
	void Update() {
		if (!isCubemapSet) {
			SetCubemap();
			isCubemapSet = true;
		}
	}
 
	void SetCubemap() {
		//Loops the materials and sets the cubemap
		Material[] materials = renderer.materials;
 
		foreach(Material thisMaterial in materials) {
			foreach(string thisKeyword in cubemapKeywords) {
				if (thisMaterial.HasProperty(thisKeyword))
				thisMaterial.SetTexture(thisKeyword, thisCubemapper.GetRenderTexture());
			}
		}
	}
 
}
}
