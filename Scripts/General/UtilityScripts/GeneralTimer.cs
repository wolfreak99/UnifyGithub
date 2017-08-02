/*************************
 * Original url: http://wiki.unity3d.com/index.php/GeneralTimer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/GeneralTimer.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Stumbling Coder 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    
    
    OverviewThis simple C# class is a timer component that times down from an editor set duration to 0. Upon ending the countdown, the timer will either reset and pause, destroy itself, or destroy the gameObject it is attached to. Several method's have been provided to get the time passed and time remaining in float, or int, along with methods to pause, and unpause the timer. 
    
    
    C# - Timer.csThe script should be named Timer.cs 
    
    
    using UnityEngine;
    using System.Collections;
     
    /// <summary>
    /// Timer.cs
    /// 10/16/2011
    /// TheStumblingCoder
    /// <para>
    /// A timer that countdowns from the duration to zero.  On reaching zero it
    /// it can will pause and reset itself, destroy itself, or kill its parent GameObject 
    /// </para>
    /// </summary>
    public class Timer : MonoBehaviour {
    	private float finish;
    	private float remains;
    	private float passed;
    	private float start;
    	private bool isPaused;
    	/// <value> duration of timer </value>
    	public float duration;
    	/// <value> determins whether to delete component on finish </value>
    	public bool delOnFinish;
    	/// <value> determines whether to delete gameObject on finish </value>
    	public bool delParentOnFinish;
     
    	/// <summary>
    	/// Sets timer and starts it
    	/// </summary>
    	void Start () {
    		Set();
    		UnPause();
    	}
    	/// <summary>
    	/// Returns the time Commenced since timer started as float.
    	/// </summary>
    	public float Passed(){
    		return remains;
    	}
    	/// <summary>
    	/// Returns the time Commenced since timer started in seconds as int.
    	/// </summary>
    	public int SecPassed(){
    		return Mathf.CeilToInt(remains);
    	}
    	/// <summary>
    	/// Returns the time Ellapsed since timer started as float.
    	/// </summary>
    	public float Remaining(){
    		return passed;
    	}
    	/// <summary>
    	/// Returns the time ellapsed since timer started in seconds as int.
    	/// </summary>
    	public int SecRemaining(){
    		return Mathf.CeilToInt(passed);
    	}
    	/// <summary>
    	/// Finishes the timer, reseting, destroying itself, or destroying its parent gameObject.
    	/// </summary>
    	private void Finish(){
    		if (delOnFinish){
    			Destroy(this);
    		}
    		if (delParentOnFinish){
    			Destroy(gameObject);
    		}
    		Pause();
    		Set();
    	}
    	/// <summary>
    	/// Returns whether or not the timer is paused
    	/// </summary>
    	public bool IsPaused(){
    		return isPaused;
    	}
    	/// <summary>
    	/// Pauses the timer.
    	/// </summary>
    	public void Pause(){
    		isPaused = true;
    	}
    	/// <summary>
    	/// Sets the timer.
    	/// </summary>
    	public void Set(){
    		start = Time.time;
    		finish = start + duration;
    		remains = start - Time.time;
    		passed = duration - remains;
    	}
    	/// <summary>
    	/// Unpauses the timer.
    	/// </summary>
    	public void UnPause() {
    		isPaused = false;
    	}
    	/// <summary>
    	/// Unity3D's update, updates timer and Finishes if complete.
    	/// </summary>
    	void Update () {
    		if (!isPaused){
    			remains = finish - Time.time;
    			passed = duration - remains;
    			if (remains < start) {
    				Finish();
    			}
    		}
    	}
}
}
