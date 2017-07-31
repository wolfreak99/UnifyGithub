// Original url: http://wiki.unity3d.com/index.php/LoadAssetBundle
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/LoadAssetBundle.cs
// File based on original modification date of: 27 September 2012, at 21:56. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: DbIMok 
DescriptionThe purpose of this script is to load content of selected AssetBundle in Editor 
UsageLike all Editor scripts, this has to be put into a folder named Editor, somewhere in your Assets folder. Click on .unity3d file in Project window right mouse button and select Ext/Load Bundle. 
C# - LoadBundle.csusing UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
 
public class LoadBundle : EditorWindow
{
	/// <summary>
	/// Relative path to AssetBundle file
	/// </summary>
	private static string _pathBundle;
	/// <summary>
	/// Dictionary for loaded AssetBundles
	/// </summary>
	private static Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();
	/// <summary>
	/// Dictionary for hashes of loaded AssetBundles
	/// </summary>
	private static Dictionary<string, byte[]> _bundlesHash = new Dictionary<string, byte[]>();
	/// <summary>
	/// EditorWindow instance
	/// </summary>
	private static LoadBundle _wnd;
 
	/// <summary>
	/// URL to AssetBundle file
	/// </summary>
	private string _url;
	/// <summary>
	/// Check for Run
	/// </summary>
	private bool _isRun;
	/// <summary>
	/// Check for Loaded
	/// </summary>
	private bool _isLoaded;
	/// <summary>
	/// WWW loader
	/// </summary>
	private WWW _loader;
	/// <summary>
	/// Instance of MD5 hash class
	/// </summary>
	private MD5 _hash = MD5.Create();
 
	/// <summary>
	/// Menu validator Assets/Ext/Load Bundle
	/// </summary>
	[MenuItem("Assets/Ext/Load Bundle", true)]
	public static bool LoadBundleValidator()
	{
		_pathBundle = AssetDatabase.GetAssetPath(Selection.activeObject);
		return Path.GetExtension(_pathBundle) == ".unity3d";
	}
	/// <summary>
	/// Menu Assets/Ext/Load Bundle
	/// </summary>
	[MenuItem("Assets/Ext/Load Bundle")]
	public static void LoadBundleToScene()
	{
		BundleToScene();
	}
	/// <summary>
	/// Menu Assets/Ext/Unload Bundles
	/// </summary>
	[MenuItem("Assets/Ext/Unload Bundles")]
	public static void UnloadAll()
	{
		if (_wnd != null)
		{
			_wnd.Close();
			DestroyImmediate(_wnd);
		}
		foreach (AssetBundle assetBundle in _bundles.Values)
		{
			if (assetBundle != null) assetBundle.Unload(true);
		}
		_bundles.Clear();
		_bundlesHash.Clear();
	}
 
	/// <summary>
	/// Init EditorWindow
	/// </summary>
	static void BundleToScene()
	{
		GetWindowWithRect<LoadBundle>(new Rect(0,0,1,1));
	}
	/// <summary>
	/// Update for EditorWindow
	/// </summary>
	void Update()
	{
		if (!_isRun)
		{
			_isRun = true;
			if (IsLoaded()) Close();
		} else {
			while ((_loader == null) || (_loader.assetBundle == null) && string.IsNullOrEmpty(_loader.error))
			{
				return;
			}
			if (_loader.assetBundle != null)
			{
				Instantiate(_loader.assetBundle.mainAsset);
				_bundles.Add(_pathBundle, _loader.assetBundle);
				_bundlesHash.Add(_pathBundle, _hash.ComputeHash(_loader.bytes));
			}
			else
			{
				Debug.LogWarning(_loader.error);				
			}
			Close();
		}
	}
	/// <summary>
	/// Check for already loaded bundles
	/// </summary>
	/// <returns></returns>
	bool IsLoaded()
	{
		_url = string.Concat("file:///", Application.dataPath, "/../", _pathBundle);
		if (_bundles.ContainsKey(_pathBundle))
		{
			if (!CheckHash())
			{
				_bundles.Remove(_pathBundle);
				_bundlesHash.Remove(_pathBundle);
				_loader = new WWW(_url);
				return false;
			}
			Instantiate(_bundles[_pathBundle].mainAsset);
			return true;
		} 
		else
		{
			_loader = new WWW(_url);
			return false;
		}
	}
	/// <summary>
	/// Check hash for loaded AssetBundle and file
	/// </summary>
	/// <returns></returns>
	bool CheckHash()
	{
		StreamReader sr = new StreamReader(string.Concat(Application.dataPath, "/../", _pathBundle));
		byte[] _filehash = _hash.ComputeHash(sr.BaseStream);
		for (int i = 0; i < _filehash.Length; i++)
		{
			if (_filehash[i] != _bundlesHash[_pathBundle][i]) return false;
		}
		return true;
	}
}
}
