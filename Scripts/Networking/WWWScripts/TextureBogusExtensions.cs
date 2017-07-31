// Original url: http://wiki.unity3d.com/index.php/TextureBogusExtensions
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/WWWScripts/TextureBogusExtensions.cs
// File based on original modification date of: 20 January 2013, at 00:08. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
Usage Drop this somewhere in your project as TextureBogusExtensions.cs, and you will be able to do things like: 
if (myTexture.isBogus()) { //code here that uses some other local default texture instead }TextureBogusExtensions.cs using UnityEngine;
using System.Collections;
using System.Text;
 
/// <summary>
/// 
/// Add a feature to the Texture class which allows you to detect the case when you have attempted to download a bogus WWW Texture.
///
/// by Matt "Trip" Maker, Monstrous Company :: http://monstro.us
/// 
/// TODO could also use the filesystem cache to keep the example error image between runs.
///
/// from http://unifycommunity.com/wiki/index.php?title=TextureBogusExtensions
/// 
/// </summary>
public static class TextureBogusExtensions
{
	public static bool ready = false;
	private static Texture _bogusTexture = null;
	public static Texture bogusTexture {
		get {
			return _bogusTexture;
		}
		set {
			_bogusTexture = value;	
		}
	}
 
	public static bool obtainExampleBogusTexture() {
		Debug.Log("obtaining an example bogus texture by trying to load an HTML page as a texture");
		bool keepgoing = true;
		float timeoutAt = Time.time + 10.0f;				
		_bogusTexture = new Texture();
 
		WWW www = new WWW("http://monstro.us");
 
//		Wait for download to complete
		float elapsed = 0.0f;
		while ( Time.time < timeoutAt && !www.isDone && keepgoing)
		{
			elapsed = elapsed + 0.01f;
		}
 
		if (www.error == null) {
			_bogusTexture = www.texture;
			Debug.Log("bogusTexture " + _bogusTexture.name +","+ _bogusTexture.height +","+ _bogusTexture.width +","+ _bogusTexture.filterMode +","+ _bogusTexture.anisoLevel +","+ _bogusTexture.wrapMode +","+ _bogusTexture.mipMapBias);
			return true;
		}
		return false;
	}
 
	/// <summary>
	/// The easy way: compare to a saved version of the question mark image, expressed here as an array of bytes.
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public static bool isBogus(this Texture tex) {
		if (!tex) return true;
 
		byte[] png1 = (tex as Texture2D).EncodeToPNG();
		byte[] questionMarkPNG = new byte[] {137,80,78,71,13,10,26,10,0,0,0,13,73,72,68,82,0,0,0,8,0,0,0,8,8,2,0,0,0,75,109,41,220,0,0,0,65,73,68,65,84,8,29,85,142,81,10,0,48,8,66,107,236,254,87,110,106,35,172,143,74,243,65,89,85,129,202,100,239,146,115,184,183,11,109,33,29,126,114,141,75,213,65,44,131,70,24,97,46,50,34,72,25,39,181,9,251,205,14,10,78,123,43,35,17,17,228,109,164,219,0,0,0,0,73,69,78,68,174,66,96,130,};
 
		return Equivalent(png1, questionMarkPNG);
	}
 
	/// <summary>
	/// The hard(er) way: compare this texture to the result of a deliberately failed texture download attempt.
	/// note that this may block during the first access, while the error image is downloaded and cached.
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public static bool matchesCurrentErrorImage(this Texture tex) {
		if (!tex) return false;
		if (!ready) {
			Debug.LogWarning("TextureBogusExtensions matchesCurrentErrorImage was called before the error image had been cached. To avoid this, call TextureBogusExtensions.Init() earlier in your code. If the error texture has been successfully cached, TextureBogusExtensions.ready will be true. We will now block for a moment while caching the error image.");
			Init();
		}
 
		byte[] png1 = (tex as Texture2D).EncodeToPNG();
		byte[] png2 = (bogusTexture as Texture2D).EncodeToPNG();
 
		return Equivalent(png1, png2);
	}
 
	/// <summary>
	/// Compare two byte arrays.
	/// </summary>
	/// <param name="bytes1">
	/// A <see cref="System.Byte[]"/>
	/// </param>
	/// <param name="bytes2">
	/// A <see cref="System.Byte[]"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public static bool Equivalent(byte[] bytes1, byte[] bytes2) {
		if (bytes1.Length != bytes2.Length) return false;
		for (int i=0;i<bytes1.Length;i++)
			if (!bytes1[i].Equals(bytes2[i])) return false;
		return true;
	}
 
	/// <summary>
	/// Express the texture in the form of the C# code that would be necessary to represent a PNG of it.
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string asPNGDeclaration(this Texture tex) {
		StringBuilder sb = new StringBuilder();
		byte[] pngBytes = (tex as Texture2D).EncodeToPNG();
		sb.Append("new byte[] {");
		for (int i=0;i<pngBytes.Length;i++) {
			sb.Append(pngBytes[i].ToString()).Append(",");
		}
		sb.Append("};");
		return sb.ToString();
	}
 
	/// <summary>
	/// Get an example of whatever the current "error" texture is. (At the time of this writing, an "upside down red question mark on a white background".)
	/// </summary>
	public static void Init() {
		//Debug.Log("Init");
		ready = obtainExampleBogusTexture();
	}
}
}
