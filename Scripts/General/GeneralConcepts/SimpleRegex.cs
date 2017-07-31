// Original url: http://wiki.unity3d.com/index.php/SimpleRegex
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/SimpleRegex.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
Author: Jonathan Czeck (aarku) 
Description This script shows one way to use regular expressions in Unity. This script is not intended to do something useful, but open up ideas on how one might make something useful using regular expressions. It does not attempt to teach Mono's regular expression syntax. (Which seems very similar if not identical to bash's.) In this script, we take an inputString and extract specific properties out of it and turn them into useful things like string, float, int, and Vector3. 
Usage Place this script on any GameObject in your scene, set the inputString property to something like the default string or just leave it as-is. Then hit play and see the results of the script in the Console window. 
JavaScript - SimpleRegex.js import System.Text.RegularExpressions;
import System;
 
private var realNumberRegex = "([-+]?[0-9]*\\.?[0-9]+)"; // need double \ as \ is an escape character in javascript strings
private var integerRegex = "([-+]?[0-9]+)";
private var nameRegex = "([A-Za-z0-9_]+)";
private var unsignedIntegerRegex = "([0-9]+)";
private var startOfLineRegex = "^\\s*";
private var endOfLineRegex = "\\s*$";
private var spaceRegex = "\\s*";
private var restOfLineRegex = "(.*$)";
private var anythingRegex = ".*";
private var gameObjectNameRegex = "(.+) \\|";  // need a double \ to imply an escape in the regular expression syntax... wheee
private var propertyStringRegex = "\"(.+)\"";
private var vector3StringRegex = realNumberRegex + "," + realNumberRegex + "," + realNumberRegex;
var inputString = "rusty turret base | spawn=\"turret\" scale=2,2,2 raise=1 rotate=0,45,10";
 
function Start ()
{
	// extract name
	match = Regex.Match(inputString, startOfLineRegex + gameObjectNameRegex);
	if (match.Success)
	{
		gameObjectName = match.Groups[1].Value; // the groups are things matched inside the parentheses.  It starts at group 1, which is our gameObjectName
		Debug.Log("Name:" + gameObjectName);
	}
	else
	{
		Debug.Log("Name is required in :\"" + inputString + "\"");
	}
 
	// extract spawn
	match = Regex.Match(inputString, startOfLineRegex + gameObjectNameRegex + spaceRegex + anythingRegex + "spawn=" + propertyStringRegex);
	if (match.Success)
	{
		spawn = match.Groups[2].Value;  // we want the second group matched because the first one is the gameObjectName
		Debug.Log("Spawn:" + spawn);
	}
 
	// extract scale
	match = Regex.Match(inputString, startOfLineRegex + gameObjectNameRegex + spaceRegex + anythingRegex + "scale=" + vector3StringRegex);
	if (match.Success)
	{
		scale = Vector3(Convert.ToSingle(match.Groups[2].Value), Convert.ToSingle(match.Groups[3].Value), Convert.ToSingle(match.Groups[4].Value));
		Debug.Log("Scale:" + scale);
	}
 
	// extract raise
	match = Regex.Match(inputString, startOfLineRegex + gameObjectNameRegex + spaceRegex + anythingRegex + "raise=" + realNumberRegex);
	if (match.Success)
	{
		raise = Convert.ToInt32(match.Groups[2].Value);  // we want the second group matched because the first one is the gameObjectName
		Debug.Log("Raise:" + raise);
	}
 
	// extract rotate
	match = Regex.Match(inputString, startOfLineRegex + gameObjectNameRegex + spaceRegex + anythingRegex + "rotate=" + vector3StringRegex);
	if (match.Success)
	{
		rotate = Vector3(Convert.ToSingle(match.Groups[2].Value), Convert.ToSingle(match.Groups[3].Value), Convert.ToSingle(match.Groups[4].Value));
		Debug.Log("Rotate:" + rotate);
	}
}For those interested in learning about the System.Text.RegularExpressions namespace that brings in regular expressions functionality and an entire bunch of new types, here's the link for the relevant Microsoft .NET Framework documentation page: http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.aspx 
}
