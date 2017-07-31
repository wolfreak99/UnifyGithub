// Original url: http://wiki.unity3d.com/index.php/NUnit
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Development/DebuggingScripts/NUnit.cs
// File based on original modification date of: 25 April 2014, at 06:21. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{
You may see how to adapt Unity to run NUnity test in this demo-project: 
https://github.com/downloads/zoon/NUnitLiteUnityRunner/nunit-lite-demo.unitypackage 
code repository: http://github.com/zoon/NUnitLiteUnityRunner 
NUnitLite Test Runner for Unity3D 3.0 Project Overview This is a test runner for NUnitLite, that redirects test results to Unity3D console. 
After compilation of C# files Unity gives you two assemblies: 
Assembly-CSharp-firstpass.dll for 'Plugins' and '[Pro] Standard Assets' 
Assembly-CSharp.dll for other scripts 
(Note, that Unity uses criptic names like '9cda786f9571f9a4d863974e5a5a9142') 
Then, if you want to have tests in both places - you should call NUnitLiteUnityRunner.RunTests() from both places. One call per assembly is enough, but you can call it as many times as you want - all calls after first are ignored. 
All of the above is correct for Js scripts too. 
You can use 'MonoBahavior' classes for tests, but Unity gives you one harmless warning per class. Using special Test classes would be a better idea. 
Running on the Mac/Linux If you set this up on the Mac version of Unity, you will see a warning complaining about "/nologo". This is trying to invoke the nologo option, but that format only works in Windows. 
In NUnitLiteUnityRunner.cs, you can modify the line 
runner.Execute(new[] {"/nologo", assembly.FullName});
to 
	    //Execute based on the OS version
		    int osversion = (int) Environment.OSVersion.Platform;
         	string nologoOption;
       //Windows
    		nologoOption = "/nologo";
    	//Linux (and Mac)
	      	if ((osversion == 4) || (osversion == 6) || (osversion == 128))
	 	   	    nologoOption = "-nologo";
           runner.Execute(new[] {nologoOption, assembly.FullName});
}
