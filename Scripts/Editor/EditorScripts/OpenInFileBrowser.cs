// Original url: http://wiki.unity3d.com/index.php/OpenInFileBrowser
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/OpenInFileBrowser.cs
// File based on original modification date of: 9 April 2014, at 15:12. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Use `OpenInFileBrowser.Open()` for a cross-platform way of opening any file/folder. This gives your code a "Reveal in Finder" or "Open in Explorer" functionality. Can be useful for editor scripts but probably can be used for non-editor scripts for desktop builds also (for in-game mod editors perhaps?). 
If you specify a path with a file at the end, the code will open your file browser with that file selected/highlighted. If you specify a folder instead, it will open the contents of that folder with nothing in it selected. 
I place this code in the public domain. Feel free to use it! 
public static class OpenInFileBrowser
{
	public static bool IsInMacOS
	{
		get
		{
			return UnityEngine.SystemInfo.operatingSystem.IndexOf("Mac OS") != -1;
		}
	}
 
	public static bool IsInWinOS
	{
		get
		{
			return UnityEngine.SystemInfo.operatingSystem.IndexOf("Windows") != -1;
		}
	}
 
	[UnityEditor.MenuItem("Window/Test OpenInFileBrowser")]
	public static void Test()
	{
		Open(UnityEngine.Application.dataPath);
	}
 
	public static void OpenInMac(string path)
	{
		bool openInsidesOfFolder = false;
 
		// try mac
		string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes
 
		if ( System.IO.Directory.Exists(macPath) ) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}
 
		if ( !macPath.StartsWith("\"") )
		{
			macPath = "\"" + macPath;
		}
 
		if ( !macPath.EndsWith("\"") )
		{
			macPath = macPath + "\"";
		}
 
		string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
 
		try
		{
			System.Diagnostics.Process.Start("open", arguments);
		}
		catch ( System.ComponentModel.Win32Exception e )
		{
			// tried to open mac finder in windows
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}
 
	public static void OpenInWin(string path)
	{
		bool openInsidesOfFolder = false;
 
		// try windows
		string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes
 
		if ( System.IO.Directory.Exists(winPath) ) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}
 
		try
		{
			System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
		}
		catch ( System.ComponentModel.Win32Exception e )
		{
			// tried to open win explorer in mac
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}
 
	public static void Open(string path)
	{
		if ( IsInWinOS )
		{
			OpenInWin(path);
		}
		else if ( IsInMacOS )
		{
			OpenInMac(path);
		}
		else // couldn't determine OS
		{
			OpenInWin(path);
			OpenInMac(path);
		}
	}
}
}
