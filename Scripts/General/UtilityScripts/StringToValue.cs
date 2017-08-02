/*************************
 * Original url: http://wiki.unity3d.com/index.php/StringToValue
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/StringToValue.cs
 * File based on original modification date of: 10 January 2012, at 20:53. 
 *
 * Author: Danny Lawrence 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    Functions for getting a value in string input between strings startFlag and endFlag, with an optional starting point index. Useful for dynamic import logic. 
    Usage function GetValue (input : String, startFlag : String, endFlag : String, startPos : int) : string 
    DescriptionReturns the value in string input between startFlag and endFlag, beginning at startPos. 
    var value : String = GetValue ("[12]gibberish[17]gibberish", "[", "]", 6); // returns 17
    
    
    function GetValue(input : String, startFlag : String, endFlag : String) : String 
    DescriptionReturns the value in string input between startFlag and endFlag. 
    var value : String = GetValue ("[12]gibberish[17]gibberish", "[", "]"); // returns 12
    
    
    function GetValue(input : String, startFlag : String, endFlag : String, startPos : int) {
    	startIndex = input.IndexOf(startFlag,startPos);
    	endIndex = input.IndexOf(endFlag,startPos);
    	valueLength = endIndex - startIndex - 1;
    	print (input);
    	output = input.Substring(startIndex+1,valueLength);
    	return output;
    };
     
    function GetValue(input : String, startFlag : String, endFlag : String) {
    	startIndex = input.IndexOf(startFlag);
    	endIndex = input.IndexOf(endFlag);
    	valueLength = endIndex - startIndex - 1;
    	print (input);
    	output = input.Substring(startIndex+1,valueLength);
    	return output;
};
}
