// Original url: http://wiki.unity3d.com/index.php/SharpUnit
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Development/DebuggingScripts/SharpUnit.cs
// File based on original modification date of: 15 April 2012, at 16:58. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{
SharpUnit is a unit testing framework written in C# specifically for Unity3D. It was inspired by and adapted from UUnit, which is written in Boo. It is my hope that SharpUnit will help Unity3D developers write and maintain solid code. Enjoy! 
Mark Gants | mgants[at]bigpoint[dot]net 
Contents [hide] 
1 Get SharpUnit 
2 Why SharpUnit? 
3 Getting Started: 
4 Integrate with Unity3D Using a DLL: 
5 Integrate with Unity3D Using the Scripts: 
6 Running Tests in Unity3D: 
7 Extending SharpUnit: 

Get SharpUnitSharpUnit has been added to GitHub (finally!). Get it here: 
https://github.com/mgants4/SharpUnit 
Each version of SharpUnit has been committed sequentially to GitHub to maintain the revision history. Feel free to contribute improvements. 
Why SharpUnit?Because NUnit is not so easy to integrate into Unity3D. 
UUnit works but, while I personally like Boo, not everyone is familiar/comfortable with programming in Boo. 
SharpUnit allows you to trap/expect exceptions, UUnit does not (as of May 20, 2010). 
SharpUnit can easily be integrated into Unity3D but is not dependant on the Unity3DEngine namespace. 
SharpUnit can easily extended and tested outside of Unity3D and even be used for other purposes. 
SharpUnit is straight-forward and easy to use. 
Getting Started:The Visual Studio solution contains two projects: SharpUnit and SharpUnitTest 
SharpUnit: This is the class library that can be built to a DLL. 
SharpUnitTest: This is a console app that runs the unit tests for SharpUnit itself. 
The Main() function in Program.cs is an example test runner, it illustrates how to get up and running with SharpUnit. 
The Unity3D_SharpUnitDemo.zip file contains an actual Unity3D 2.6 project using SharpUnit. 
Integrate with Unity3D Using a DLL:Build the SharpUnit.dll from source using Visual Studio. 
Create a folder named "SharpUnit" In your Unity3D project's "Assets" folder. 
Place the SharpUnit.dll in this folder. 
Copy the Unity3D_TestRunner.cs and Unity3D_TestReporter.cs files to this folder. 
Integrate with Unity3D Using the Scripts:Create a folder named "SharpUnit" In your Unity3D project's "Assets" folder. 
Copy the SharpUnit class files (*.cs) to this folder. 
Copy the Unity3D_TestRunner.cs and Unity3D_TestReporter.cs files to this folder. 
Running Tests in Unity3D:Create an empty GameObject within Unity, name it TestRunner. 
Drag the Unity3D_TestRunner.cs script onto the TestRunner game object. 
Write unit test scripts as classes derived from the TestCase class. 
Configure the TestSuite object in the Unity3D_TestRunner.cs script to run your tests. 
Press Play, you should see the test output in the Unity3D editor console. 
Extending SharpUnit:Derive a new class from TestReporter to modify how SharpUnit reports test results and errors. 
Feel free to extend this project any way you like. I have no problems if someone wants to host it somewhere and continue developing on it. Just respect the terms of the original license included with the project and make your improvements available to the rest of the Unity3D community. 
}
