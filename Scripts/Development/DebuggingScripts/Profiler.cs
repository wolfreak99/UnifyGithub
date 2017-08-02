/*************************
 * Original url: http://wiki.unity3d.com/index.php/Profiler
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Development/DebuggingScripts/Profiler.cs
 * File based on original modification date of: 20 January 2013, at 00:44. 
 *
 * Author: Michael Garforth 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Development.DebuggingScripts
{
    
    
    Contents [hide] 
    1 Overview 
    2 Use 
    3 Example - TestProfile.cs 
    3.1 Usage in js 
    4 C# - Profile.cs 
    5 C# - HRProfile.cs 
    
    OverviewThis C# class gives simple access to simple profiling of code 
    The script can be placed in an early compilation pass (e.g. Plugins directory) so that it can be used from any language 
    It has no dependencies besides .NET and Unity 
    
    
    UseProfile.StartProfile(tag) - Use this to start the profile timer for the specified tag 
    Profile.EndProfile(tag) - Use this to end the profile of the specified tag 
    Profile.Reset() - Remove all profile data and reset the timer, generally not needed 
    Profile.PrintResults() - Output all profile data to the console, usually called in OnApplicationQuit() from one location 
    
    Note: Match up every start tag with an end tag, otherwise the timing data will be wrong 
    Note: The profiler can work across scripts and across scenes by starting the tag in one, and ending it in another. (E.G.: Use: Profile.StartProfile("LoadLevel"); just before Application.LoadLevel(1); and Profile.EndProfile("LoadLevel"); on a script's function Awake(); in the new scene.) 
    Example - TestProfile.csusing UnityEngine;
     
    public class TestProfile : MonoBehaviour
    {
    	private float outputTest = 0;
     
    	void Start()
    	{
    		Profile.StartProfile("Start");
    	}
     
    	void Update ()
    	{
    		Profile.StartProfile("Update");
    		for (int i = 0; i < 100; ++i)
    			outputTest += Mathf.Sin(i * Time.time);
    		Profile.EndProfile("Update");
    	}
     
    	void OnApplicationQuit()
    	{
    		Profile.EndProfile("Start");
    		Debug.Log("outputTest is " + outputTest.ToString("F3"));
    		Profile.PrintResults();
    	}
    }
    Output: 
    ============================ 
    Profile results: 
    ============================ 
    
    Profile Update took 0.03267 seconds to complete over 1568 iterations, averaging 0.00002 seconds per call 
    Profile Start took 7.56886 seconds to complete over 1 iteration, averaging 7.56886 seconds per call 
    
    ============================ 
    Total runtime: 7.580 seconds 
    ============================ 
    
    Usage in jsTo measure the time a function takes to complete, e.g Start 
    function Start () 
    {
     
    	Profile.StartProfile("Start");
            //my code
    	Profile.EndProfile("Start");
    }
     
    function OnApplicationQuit()
    {
     
            Profile.PrintResults();
    }
    
    To view the results, you have to select the Profile results in the console
     
    C# - Profile.csThe script should be named Profile.cs 
    
    
    using System.Collections.Generic;
    using System;
    using UnityEngine;
     
    public class Profile
    {
    	public struct ProfilePoint
    	{
    		public DateTime lastRecorded;
    		public TimeSpan totalTime;
    		public int totalCalls;
    	}
     
    	private static Dictionary<string, ProfilePoint> profiles = new Dictionary<string, ProfilePoint>();
    	private static DateTime startTime = DateTime.UtcNow;
     
    	private Profile()
    	{
    	}
     
    	public static void StartProfile(string tag)
    	{
    		ProfilePoint point;
     
    		profiles.TryGetValue(tag, out point);
    		point.lastRecorded = DateTime.UtcNow;
    		profiles[tag] = point;
    	}
     
    	public static void EndProfile(string tag)
    	{
    		if (!profiles.ContainsKey(tag))
    		{
    			Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
    			return;
    		}
    		ProfilePoint point = profiles[tag];
    		point.totalTime += DateTime.UtcNow - point.lastRecorded;
    		++point.totalCalls;
    		profiles[tag] = point;
    	}
     
    	public static void Reset()
    	{
    		profiles.Clear();
    		startTime = DateTime.UtcNow;
    	}
     
    	public static void PrintResults()
    	{
    		TimeSpan endTime = DateTime.UtcNow - startTime;
    		System.Text.StringBuilder output = new System.Text.StringBuilder();
    		output.Append("============================\n\t\t\t\tProfile results:\n============================\n");
    		foreach(KeyValuePair<string, ProfilePoint> pair in profiles)
    		{
    			double totalTime = pair.Value.totalTime.TotalSeconds;
    			int totalCalls = pair.Value.totalCalls;
    			if (totalCalls < 1) continue;
    			output.Append("\nProfile ");
    			output.Append(pair.Key);
    			output.Append(" took ");
    			output.Append(totalTime.ToString("F5"));
    			output.Append(" seconds to complete over ");
    			output.Append(totalCalls);
    			output.Append(" iteration");
    			if (totalCalls != 1) output.Append("s");
    			output.Append(", averaging ");
    			output.Append((totalTime / totalCalls).ToString("F5"));
    			output.Append(" seconds per call");
    		}
    		output.Append("\n\n============================\n\t\tTotal runtime: ");
    		output.Append(endTime.TotalSeconds.ToString("F3"));
    		output.Append(" seconds\n============================");
    		Debug.Log(output.ToString());
    	}
    }
    
    C# - HRProfile.csThe script should be named HRProfile.cs and is a modified version of the above script which include ticks for higher resolution. 
    
    
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
     
    public class Profile
    {
        public struct ProfilePoint
        {
            public DateTime lastRecorded;
            public TimeSpan totalTime;
            public int totalCalls;
        }
     
        private static Dictionary<string, ProfilePoint> profiles = new Dictionary<string, ProfilePoint>();
        private static DateTime startTime = DateTime.UtcNow;
        private static DateTime _startTime;
        private static Stopwatch _stopWatch = null;
        private static TimeSpan _maxIdle = TimeSpan.FromSeconds(10);
     
        private Profile()
        {
        }
     
        public static DateTime UtcNow
        {
            get
            {
                if (_stopWatch == null || startTime.Add(_maxIdle) < DateTime.UtcNow)
                {
                    _startTime = DateTime.UtcNow;
                    _stopWatch = Stopwatch.StartNew();
                }
                return _startTime.AddTicks(_stopWatch.Elapsed.Ticks);
            }
        }
     
        public static void StartProfile(string tag)
        {
            ProfilePoint point;
     
            profiles.TryGetValue(tag, out point);
            point.lastRecorded = UtcNow;
            profiles[tag] = point;
        }
     
        public static void EndProfile(string tag)
        {
            if (!profiles.ContainsKey(tag))
            {
                UnityEngine.Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
                return;
            }
            ProfilePoint point = profiles[tag];
            point.totalTime += UtcNow - point.lastRecorded;
            ++point.totalCalls;
            profiles[tag] = point;
        }
     
        public static void Reset()
        {
            profiles.Clear();
            startTime = DateTime.UtcNow;
        }
     
        public static void PrintResults()
        {
            TimeSpan endTime = DateTime.UtcNow - startTime;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("============================\n\t\t\t\tProfile results:\n============================\n");
            foreach (KeyValuePair<string, ProfilePoint> pair in profiles)
            {
                double totalTime = pair.Value.totalTime.TotalSeconds;
                int totalCalls = pair.Value.totalCalls;
                if (totalCalls < 1) continue;
                output.Append("\nProfile ");
                output.Append(pair.Key);
                output.Append(" took ");
                output.Append(totalTime.ToString("F9"));
                output.Append(" seconds to complete over ");
                output.Append(totalCalls);
                output.Append(" iteration");
                if (totalCalls != 1) output.Append("s");
                output.Append(", averaging ");
                output.Append((totalTime / totalCalls).ToString("F9"));
                output.Append(" seconds per call");
            }
            output.Append("\n\n============================\n\t\tTotal runtime: ");
            output.Append(endTime.TotalSeconds.ToString("F3"));
            output.Append(" seconds\n============================");
            UnityEngine.Debug.Log(output.ToString());
        }
}
}
