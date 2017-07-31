// Original url: http://wiki.unity3d.com/index.php/SimpleTimer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/SimpleTimer.cs
// File based on original modification date of: 27 July 2016, at 10:24. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Contents [hide] 
1 SimpleTimer.cs 
1.1 Author 
1.2 Description 
2 SimpleTimer.cs code 
2.1 Example Usage 

SimpleTimer.csAuthorzombience aka Jason Araujo 
DescriptionSimpleTimer.cs is intended to provide a simple class that can be used to keep track of intervals and drive events. 
SimpleTimer constructor takes two arguments: 
 
public SimpleTimer(float _life, bool useFixedTime = false)a float indicating the lifetime in seconds of the timer 
a boolean (default is false) that indicates whether Time.time or Time.fixedTime should be used to run timing 
SimpleTimer.cs code 
using UnityEngine;
using System.Collections;
 
public class SimpleTimer
{
	public float life { get { return _life; } private set { _life = value; } }
	public float elapsed { get { return _curTime; } }
	public float normalized { get { return _curTime / life; } } // returns timer as a range between 0 and 1
	public float remaining { get { return life - elapsed; } }
	public bool isFinished { get { return elapsed >= life; } }
	public bool isPaused { get { return _isPaused; } private set { _isPaused = value; } }
 
	protected bool _fixedTime;
	protected bool _isPaused;
	protected float _life;
	protected float _startTime;
	protected float _pauseTime;
	protected float _curTime { get { return (isPaused ? _pauseTime : _getTime) - _startTime; } set { _pauseTime = value; } }
	protected float _getTime { get { return _fixedTime ? Time.fixedTime : Time.time; } }
 
 
	public SimpleTimer() { }
	/// <summary>
	/// timer is implicitly started on instantiation
	/// </summary>
	/// <param name="lifeSpan">length of the timer</param>
	/// <param name="useFixedTime">use fixed (physics) time or screen update time</param>
	public SimpleTimer(float lifeSpan, bool useFixedTime = false) { life = lifeSpan; _fixedTime = useFixedTime; _startTime = _getTime; }
 
	/// <summary>
	/// starts timer again using time remaining 
	/// </summary>
	public void Resume() { _startTime = (isPaused ? _getTime - elapsed : _getTime); isPaused = false; }
 
	/// <summary>
	/// stop pauses the timer and allows for resume at current elapsed time
	/// </summary>
	public void Stop()
	{
		if (!isPaused)
		{
			_curTime = _getTime;
			isPaused = true;
		}
	}
        /// <summary>
        /// Add time to the timer
        /// </summary>
        /// <param name="amt"></param>
        public void AddTime(float amt)
        {
            _life += amt;
        }
}Example UsageTo use this example, put TimerTester.cs on a gameObject in the scene. In the inspector, you can see the public variables update as the timer runs. Left mouse button resets timer, right mouse button pauses timer. 


 
 
using UnityEngine;
using System.Collections;
 
public class TimerTester : MonoBehaviour 
{
	public SimpleTimer timer;
	public float life = 10f;
	public float elapsed;
	public float normalized;
	public float timerLife;
	public bool isFinished;
	public bool isPaused;
 
	void Start () 
	{
		timer = new SimpleTimer(life);
	}
 
	void Update () 
	{
		elapsed = timer.elapsed;
		normalized = timer.normalized;
		isFinished = timer.isFinished;
		isPaused = timer.isPaused;
		timerLife = timer.life;
 
		if (Input.GetMouseButtonDown(0))
			timer = new Timer(life);
 
		if (Input.GetMouseButtonDown(1))
                {
			if(timer.isPaused)
                                timer.Stop();
                        else
                                timer.Resume();
                }
	}
}
}
