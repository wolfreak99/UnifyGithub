/*************************
 * Original url: http://wiki.unity3d.com/index.php/QuickTimer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/QuickTimer.cs
 * File based on original modification date of: 26 January 2012, at 07:07. 
 *
 * Author: Mihai Cozma 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    
    
    OverviewThis C# class will allow you to mark a certain point in time and then poll it whenever you need to find out the difference between the current time and the marked time. Very useful in "called on each frame" methods to measure certain actions that take more than one frame. 
    
    
    C# - QuickTimer.csThe script should be named QuickTimer.cs 
    
    
    using UnityEngine;
    using System;
     
    public class QuickTimer
     
    {
    	/// <summary>
    	/// Constructor 
    	/// </summary>
        public QuickTimer()
        {
            On = false;
            RecTime = 0.0f;
        }
     
    	/// <summary>
    	/// Checks if timer is running 
    	/// </summary>
        public bool On { get; set; }
     
    	/// <summary>
    	/// Startup time 
    	/// </summary>
        public float RecTime { get; set; }
     
    	/// <summary>
    	/// Start timer 
    	/// </summary>
     
        public void Reset()
        {
            RecTime = Time.time;
            On = true;
        }
     
    	/// <summary>
    	/// Stop timer 
    	/// </summary>
        public void Stop()
        {
            On = false;
            RecTime = 0.0f;
        }
     
    	/// <summary>
    	/// Check difference between current time and startup time 
    	/// </summary>
        public float Difference()
        {
            return Time.time - RecTime;
        }
}
}
