// Original url: http://wiki.unity3d.com/index.php/UUnit
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/DebuggingScripts/UUnit.cs
// File based on original modification date of: 3 December 2013, at 19:06. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{
UUnit is a simple xUnit-style framework for unit testing inside Unity IDE. 
NUnit (a popular unit-testing framework for all .NET languages) may seem like a good fit for Unity, but doesn't mesh well. In particular, NUnit is designed to be called from the console or run on a windows GUI. NUnitLite is designed for IDEs like Unity but is not included in the Unity Package; to be run easily and portably between projects it needs to included in assets. Unfortunately, NUnitlite does not put each class in a file of the same name so Unity will not load the files. Thus, the need for UUnit. 
To run its own tests attach "RunnerTestCase" to an object and it's start method will run the test cases on UUnit. Output goes to Debug.Log. To make your own test cases use "TestCaseTest" as exmple. the "[Test]" attribute marks tests and if the class is added to the suite all methods with a Void return and no paramaters signature will be run. 
Example runner code is as follows: 
suite = TestSuite() # create a test suite
suite.AddAll(TestCaseTest()) # AddAll [Test] methods in TestCaseTest
result = suite.Run(null) # run all tests
Debug.Log(result.Summary()) # send results to Debug.Log
xUnit programs do not normally show successful tests only failures. So debug log show only 6 test run. To find out what is running you will have to check the code. In RunnerTestCase, the following is the line which adds tests to the test suite: 
suite.AddAll(TestCaseTest())If you look at TestCaseTest.boo you will see it has 6 methods with [Test] attribute in front of them. These are what is run. 
Also in the debug log you will 3 lines with Dummy Fail. This is what shows when a test fails. The reason the output says 0 fail even though there are 3 failures above it is that I am testing the framework to see that the failure mechanism works correctly and it produced fail trigger when it was supposed to so they do not show up in the final output as failures. 
Contents [hide] 
1 Releases 
1.1 August 21 2012 
1.2 July 1 2008 
1.3 August 2007 
2 Test Examples 
2.1 Javascript 
2.2 C# 
2.3 Downloadable example 
2.4 Javascript bool problems 

Releases August 21 2012 0.4 release by pboechat 
Translation to C# 
Added to SVN repository (googlecode) 
Added to Git repository (github) 
New assertions on UUnitAssert 
Now UUnitTestRunner run all tests (Ctrl + Shift + T on Unity) 
UUnit_04.unityPackage 
July 1 2008 0.3 release by hatsix 
standardized filenames 
separated the test files from the UUnit files to facilitate new user understanding 
added UUnitAssertException, to help standardize the output of failed assertions. 
changed the logging function for failed tests, much more readable now (no more InvocationException) 
UUnit_version0.3.unityPackage.zip 


August 2007 Original 0.2 release by ryuuguu 
Uunit.unityPackage.0.2.zip 
Test Examples With UUnit 0.4 it is possible to run tests in C# and in Javascript at the same time. 
Javascript Javascript looks like this: 
#pragma strict
 
class XAppleTest extends UUnitTestCase {
  @UUnitTest
  function firstTest() {
  	UUnitAssert.Equals( "one", XApple.returnOne() );
  }
}C# public class XAppleCTest : UUnitTestCase
{
	[UUnitTest]
	public void firstTest() {
	  	UUnitAssert.Equals( "one", XAppleC.returnOne() );
	}
}Downloadable example ZIP: UUnit04JavascriptAndCSharpExampleV01.zip 
Unity Asset: UUnit04JavascriptAndCSharpExampleV01.unitypackage 
Unity version 3.5.6f4. I changed method RunAllTests of class UUnitTestRunner to public. 
The tests will run using CTRL-SHIFT-T or by running the scene. 
Javascript bool problems The C# method UUnitAssert.Equals might have a problem with Javascript bool types. So instead of this 
  	UUnitAssert.Equals( true, XApple.returnTrue() );use this: 
  	UUnitAssert.True( XApple.returnTrue() );
}
