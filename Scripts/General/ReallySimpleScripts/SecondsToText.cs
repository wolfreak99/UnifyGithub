/*************************
 * Original url: http://wiki.unity3d.com/index.php/SecondsToText
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/SecondsToText.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.ReallySimpleScripts
{
    Contents [hide] 
    1 Summary 
    2 Script (Javascript) 
    3 Script (CSharp) 
    4 Usage 
    
    Summary This is a simple script which can be used to convert a time value in seconds into a readable text string. e.g. 15 => 15 seconds, or 3601 => 1 hour etc... 
    Script (Javascript) function textTime (seconds : int)
    {
        if (seconds < 0) {
            return '0';
        }
        if (seconds < 60) {
            n = seconds;
            return n + ' second' + s(n);
        }
        if (seconds < 60 * 60) {
            n = Mathf.Floor(seconds/60);
            return n + ' minute' + s(n);
        }
        if (seconds < 60 * 60 * 24) {
            n = Mathf.Floor(seconds/60/60);
            return n + ' hour' + s(n);
        }
        if (seconds < 60 * 60 * 24 * 7) {
            n = Mathf.Floor(seconds/60/60/24);
            return n + ' day' + s(n);
        }
        if (seconds < 60 * 60 * 24 * 31) {
            n = Mathf.Floor(seconds/60/60/24/7);
            return n + ' week' + s(n);
        }
        if (seconds < 60 * 60 * 24 * 365) {
            n = Mathf.Floor(seconds/60/60/24/31);
            return n + ' month' + s(n);
        }
        n = Mathf.Floor(seconds/60/60/24/365);
        return n + ' year' + s(n);
    }
     
    function s (n : float) {
        return n == 1 ? '' : 's';
    }Script (CSharp) string s (int n) {
        	if (n == 1) {
        		return (" ");
        	} else {
        		return ("s");
        	}
    }
     
    string textTime (float seconds) {
    		int n = 0;
    		string outString = "error";
        	if (seconds < 0) {
            	return "0 seconds";
        	}
        	if (seconds < 60) {
        	    n = (int)Mathf.Floor(seconds);
        	    outString = n + " second" + s(n);
        	    return outString;
        	}
        	if (seconds < 60 * 60) {
        	    n = (int)Mathf.Floor(seconds/60);
        	    outString = n + " minute" + s(n);
        	    return outString;
        	}
        	if (seconds < 60 * 60 * 24) {
        	    n = (int)Mathf.Floor(seconds/60/60);
        	    outString = n + " hour" + s(n);
        	    return outString;
        	}
        	if (seconds < 60 * 60 * 24 * 7) {
         	    n = (int)Mathf.Floor(seconds/60/60/24);
        	    outString = n + " day" + s(n);
        	    return outString;
        	}
        	if (seconds < 60 * 60 * 24 * 31) {
        	    n = (int)Mathf.Floor(seconds/60/60/24/7);
        	    outString = n + " week" + s(n);
        	    return outString;
        	}
        	if (seconds < 60 * 60 * 24 * 365) {
        	    n = (int)Mathf.Floor(seconds/60/60/24/31);
        	    outString = n + " month" + s(n);
        	    return outString;
        	}
        	n = (int)Mathf.Floor(seconds/60/60/24/365);
        	outString = n + " year" + s(n);
        	return outString;
    }Usage Place anywhere in your monobehaviour and call using something similar to: 
    JS: 
    var timeAsText : String = textTime(Time.time);C# 
    string timeAsText = textTime(Time.time);Time.time being replaced by your own variable in seconds. 
}
