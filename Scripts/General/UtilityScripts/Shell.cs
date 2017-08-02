/*************************
 * Original url: http://wiki.unity3d.com/index.php/Shell
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/Shell.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: KeliHlodversson 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    Description This is a replacement for the built-in shell command that was removed from Unity 1.6.1. It is based on the original C# source of the Boo internals in the Boo project. 
    Usage To use it place Shell.js into your project and add an "import Shell;" at the top of your code. Then use it the same way as you used the old shell function: 
    import Shell;
     
    var output_of_ls = shell("ls", "-l");JavaScript - Shell.js import System.Diagnostics;
     
    static function shellp(filename : String, arguments : String) : Process  {
        var p = new Process();
        p.StartInfo.Arguments = arguments;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.FileName = filename;
        p.Start();
        return p;
    }
     
    static function shell( filename : String, arguments : String) : String {
        var p = shellp(filename, arguments);
        var output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        return output;
}
}
