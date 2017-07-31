// Original url: http://wiki.unity3d.com/index.php/DelayedDelegates
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MessagingSystems/DelayedDelegates.cs
// File based on original modification date of: 29 May 2012, at 03:39. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MessagingSystems
{
Author: David O'Donoghue (Trooper) 
Contents [hide] 
1 Description 
2 Changes 
3 Usage 
4 Code 

Description DelayedDelegates is a centralised way of calling a function at a specific time whilst passing through a parameter. 
This gets rid of the messy practice of storing a global variable when using Invoke(). 
Changes 29/05/2012 
Fixed issue with DelayedDelegates.Add(DelayedDelegateInt method, int param) throwing Index Out of Range error... caused by previous change :) 
26/05/2012 
Added DelayedDelegates.Skip(DelayedDelegateInt method, int param, int skipFrames) 
22/05/2012 
Added DelayedDelegates.RemoveAll() 
01/03/2012 
Added DelayedDelegates.Skip(DelayedDelegate method, int skipFrames) which lets you skip a number of frames instead of seconds, only made this for non parameter delegates as I'm lazy. 
Added DelayedDelegates.RunAll() function which runs every delayed delegate immediately 
Added DelayedDelegates.Add(DelayedDelegate meoth, float delay, bool realTime) to use realtimesincestartup instead of fixed time (i reversed the previous change because it did let me pause correctly). 
Usage Create a function with either a int, float, string, Vector3, Vector2 or GameObject parameter and then call the following: 
void MyFunction(int param)
{
 
   // Do somethign with param
 
}
 
// Replace '1' with your int, float, etc....
DelayedDelegates.Add(MyFunction, 1, 2)You can pass back your own parameters by inheriting from DelegateParameters like this: 
public class MyParams : DelegateParameters
{
   public int myParam1;
   public string myParam2;
 
   public MyParams(int myParam1, string myParam2)
   {
      this.myParam1 = myParam1;
      this.myParam2 = myParam2;
   }
}
 
void MyFunction(DelegateParameters param)
{
   MyParams myParam = (MyParams) param;
 
   Debug.Log(myParam.myParam1 + " : " + myParam.myParam2);
 
}
 
 
DelayedDelegates.Add(MyFunction, new MyParams(1,"HELLOW WORLD"), 2);Code using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
 
public delegate void DelayedDelegate();
public delegate void DelayedDelegateParameters(DelegateParameters param);
public delegate void DelayedDelegateInt(int param);
public delegate void DelayedDelegateString(string param);
public delegate void DelayedDelegateFloat(float param);
public delegate void DelayedDelegateVector3(Vector3 param);
public delegate void DelayedDelegateVector2(Vector3 param);
public delegate void DelayedDelegateGameObject(GameObject param);
public delegate void DelayedDelegateCollider(Collider param);
 
 
public abstract class DelegateParameters { public Object param; }
 
public static class DelayedDelegates
{
 
	static List<DelayedDelegate> delayedDelegates = new List<DelayedDelegate>();
	static List<float> delayedDelegatesTime = new List<float>();
	static List<int> delayedDelegatesFrame = new List<int>();
	static List<bool> delayedDelegatesRealtime = new List<bool>();
 
    static List<DelayedDelegateParameters> delayedDelegatesParams = new List<DelayedDelegateParameters>();
	static List<DelegateParameters> delegateParameters = new List<DelegateParameters>();
	static List<float> delayedDelegatesTimeParams = new List<float>();
 
	static List<DelayedDelegateInt> delayedDelegatesInt = new List<DelayedDelegateInt>();
	static List<int> delegateParametersInt = new List<int>();
	static List<int> delayedDelegatesIntFrame = new List<int>();
	static List<float> delayedDelegatesTimeInt = new List<float>();
	static List<bool> delayedDelegatesIntRealtime = new List<bool>();
 
 
	static List<DelayedDelegateString> delayedDelegatesString = new List<DelayedDelegateString>();
	static List<string> delegateParametersString = new List<string>();
	static List<float> delayedDelegatesTimeString = new List<float>();
 
	static List<DelayedDelegateFloat> delayedDelegatesFloat = new List<DelayedDelegateFloat>();
	static List<float> delegateParametersFloat = new List<float>();
	static List<float> delayedDelegatesTimeFloat = new List<float>();
 
	static List<DelayedDelegateVector3> delayedDelegatesVector3 = new List<DelayedDelegateVector3>();
	static List<Vector3> delegateParametersVector3 = new List<Vector3>();
	static List<float> delayedDelegatesTimeVector3 = new List<float>();
 
	static List<DelayedDelegateVector2> delayedDelegatesVector2 = new List<DelayedDelegateVector2>();
	static List<Vector2> delegateParametersVector2 = new List<Vector2>();
	static List<float> delayedDelegatesTimeVector2 = new List<float>();
 
	static List<DelayedDelegateGameObject> delayedDelegatesGameObject = new List<DelayedDelegateGameObject>();
	static List<GameObject> delegateParametersGameObject = new List<GameObject>();
	static List<float> delayedDelegatesTimeGameObject = new List<float>();
 
	static List<DelayedDelegateCollider> delayedDelegatesCollider = new List<DelayedDelegateCollider>();
	static List<Collider> delegateParametersCollider = new List<Collider>();
	static List<float> delayedDelegatesTimeCollider = new List<float>();
 
 
	static DelayedDelegateRun delegateRunner;
 
	static bool hasDelegates;
 
	public static int Count
	{
		get { return delayedDelegates.Count+delayedDelegatesFloat.Count+delayedDelegatesGameObject.Count+delayedDelegatesInt.Count+ delayedDelegatesParams.Count+delayedDelegatesString.Count+delayedDelegatesVector2.Count+delayedDelegatesTimeVector3.Count; }
	}
 
	public static bool HasDelegates
	{
		get { return hasDelegates; }
	}
 
	public static void RemoveAll(DelayedDelegate method)
	{
 
 
		for (int x = delayedDelegates.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegates[x] == method)
            {
 
                delayedDelegates.RemoveAt(x);
				delayedDelegatesTime.RemoveAt(x);
				delayedDelegatesFrame.RemoveAt(x);
				delayedDelegatesRealtime.RemoveAt(x);
            }
 
        }
 
	}
 
 
	public static void RemoveAll(DelayedDelegateParameters method) 
	{
 
		for (int x = delayedDelegatesParams.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesParams[x] == method)
            {
 
                delayedDelegatesParams.RemoveAt(x);
				delayedDelegatesTimeParams.RemoveAt(x);
				delegateParameters.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RemoveAll(DelayedDelegateInt method)
	{
 
		for (int x = delayedDelegatesInt.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesInt[x] == method)
            {
 
                delayedDelegatesInt.RemoveAt(x);
				delayedDelegatesTimeInt.RemoveAt(x);
				delegateParametersInt.RemoveAt(x);
				delayedDelegatesIntFrame.RemoveAt(x);
				delayedDelegatesIntRealtime.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RemoveAll(DelayedDelegateString method)
	{
 
		for (int x = delayedDelegatesString.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesString[x] == method)
            {
 
                delayedDelegatesString.RemoveAt(x);
				delayedDelegatesTimeString.RemoveAt(x);
				delegateParametersString.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RemoveAll(DelayedDelegateFloat method)
	{
 
		for (int x = delayedDelegatesFloat.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesFloat[x] == method)
            {
 
                delayedDelegatesFloat.RemoveAt(x);
				delayedDelegatesTimeFloat.RemoveAt(x);
				delegateParametersFloat.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RemoveAll(DelayedDelegateVector3 method)
	{
 
		for (int x = delayedDelegatesVector3.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesVector3[x] == method)
            {
 
                delayedDelegatesVector3.RemoveAt(x);
				delayedDelegatesTimeVector3.RemoveAt(x);
				delegateParametersVector3.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RemoveAll(DelayedDelegateVector2 method)
	{
 
		for (int x = delayedDelegatesVector2.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesVector2[x] == method)
            {
 
                delayedDelegatesVector2.RemoveAt(x);
				delayedDelegatesTimeVector2.RemoveAt(x);
				delegateParametersVector2.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RemoveAll(DelayedDelegateGameObject method)
	{
 
		for (int x = delayedDelegatesGameObject.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesGameObject[x] == method)
            {
 
                delayedDelegatesGameObject.RemoveAt(x);
				delayedDelegatesTimeGameObject.RemoveAt(x);
				delegateParametersGameObject.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RemoveAll(DelayedDelegateCollider method)
	{
 
		for (int x = delayedDelegatesCollider.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesCollider[x] == method)
            {
 
                delayedDelegatesCollider.RemoveAt(x);
				delayedDelegatesTimeCollider.RemoveAt(x);
				delegateParametersCollider.RemoveAt(x);
 
			}
 
 
        }
 
	}
 
	public static void RunAll()
	{
 
		for (int x = delayedDelegates.Count-1; x >= 0; x --)
		{
				delayedDelegatesTime[x] = Time.fixedTime;
				delayedDelegatesRealtime[x] = false;
				delayedDelegatesFrame[x] = 0;
		}
 
		for (int x = delayedDelegatesParams.Count-1; x >= 0; x --)
				delayedDelegatesTimeParams[x] = Time.fixedTime;
 
		for (int x = delayedDelegatesInt.Count-1; x >= 0; x --)
			delayedDelegatesTimeInt[x] = Time.fixedTime;
 
		for (int x = delayedDelegatesString.Count-1; x >= 0; x --)
			delayedDelegatesTimeString[x] = Time.fixedTime;
 
		for (int x = delayedDelegatesFloat.Count-1; x >= 0; x --)
			delayedDelegatesTimeFloat[x] = Time.fixedTime;
 
		for (int x = delayedDelegatesVector3.Count-1; x >= 0; x --)
			delayedDelegatesTimeVector3[x] = Time.fixedTime;
 
		for (int x = delayedDelegatesVector2.Count-1; x >= 0; x --)
			delayedDelegatesTimeVector2[x] = Time.fixedTime;
 
		for (int x = delayedDelegatesGameObject.Count-1; x >= 0; x --)
			delayedDelegatesTimeGameObject[x] = Time.fixedTime;
 
		for (int x = delayedDelegatesCollider.Count-1; x >= 0; x --)
			delayedDelegatesTimeCollider[x] = Time.fixedTime;
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RemoveAll()
	{
 
		delayedDelegates = new List<DelayedDelegate>();
		delayedDelegatesTime = new List<float>();
		delayedDelegatesFrame = new List<int>();
		delayedDelegatesRealtime = new List<bool>();
 
		delayedDelegatesParams = new List<DelayedDelegateParameters>();
		delegateParameters = new List<DelegateParameters>();
		delayedDelegatesTimeParams = new List<float>();
 
		delayedDelegatesInt = new List<DelayedDelegateInt>();
		delegateParametersInt = new List<int>();
		delayedDelegatesTimeInt = new List<float>();
 
		delayedDelegatesString = new List<DelayedDelegateString>();
		delegateParametersString = new List<string>();
		delayedDelegatesTimeString = new List<float>();
 
		delayedDelegatesFloat = new List<DelayedDelegateFloat>();
		delegateParametersFloat = new List<float>();
		delayedDelegatesTimeFloat = new List<float>();
 
		delayedDelegatesVector3 = new List<DelayedDelegateVector3>();
		delegateParametersVector3 = new List<Vector3>();
		delayedDelegatesTimeVector3 = new List<float>();
 
		delayedDelegatesVector2 = new List<DelayedDelegateVector2>();
		delegateParametersVector2 = new List<Vector2>();
		delayedDelegatesTimeVector2 = new List<float>();
 
		delayedDelegatesGameObject = new List<DelayedDelegateGameObject>();
		delegateParametersGameObject = new List<GameObject>();
		delayedDelegatesTimeGameObject = new List<float>();
 
		delayedDelegatesCollider = new List<DelayedDelegateCollider>();
		delegateParametersCollider = new List<Collider>();
		delayedDelegatesTimeCollider = new List<float>();
 
 
	}
 
	public static void RunAll(DelayedDelegate method)
	{
 
 
		for (int x = delayedDelegates.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegates[x] == method)
            {
 
				delayedDelegatesTime[x] = Time.fixedTime;
				delayedDelegatesRealtime[x] = false;
				delayedDelegatesFrame[x] = 0;
            }
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
 
	public static void RunAll(DelayedDelegateParameters method) 
	{
 
		for (int x = delayedDelegatesParams.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesParams[x] == method)
            {
 
				delayedDelegatesTimeParams[x] = Time.fixedTime;
 
			}
 
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RunAll(DelayedDelegateInt method)
	{
 
		for (int x = delayedDelegatesInt.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesInt[x] == method)
            {
 
				delayedDelegatesTimeInt[x] = Time.fixedTime;
				delayedDelegatesIntRealtime[x] = false;
				delayedDelegatesIntFrame[x] = 0;
 
			}
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RunAll(DelayedDelegateString method)
	{
 
		for (int x = delayedDelegatesString.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesString[x] == method)
            {
 
				delayedDelegatesTimeString[x] = Time.fixedTime;
 
			}
 
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RunAll(DelayedDelegateFloat method)
	{
 
		for (int x = delayedDelegatesFloat.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesFloat[x] == method)
            {
 
				delayedDelegatesTimeFloat[x] = Time.fixedTime;
 
			}
 
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RunAll(DelayedDelegateVector3 method)
	{
 
		for (int x = delayedDelegatesVector3.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesVector3[x] == method)
            {
 
				delayedDelegatesTimeVector3[x] = Time.fixedTime;
 
			}
 
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RunAll(DelayedDelegateVector2 method)
	{
 
		for (int x = delayedDelegatesVector2.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesVector2[x] == method)
            {
 
				delayedDelegatesTimeVector2[x] = Time.fixedTime;
 
			}
 
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RunAll(DelayedDelegateGameObject method)
	{
 
		for (int x = delayedDelegatesGameObject.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesGameObject[x] == method)
            {
 
				delayedDelegatesTimeGameObject[x] = Time.fixedTime;
 
			}
 
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
	public static void RunAll(DelayedDelegateCollider method)
	{
 
		for (int x = delayedDelegatesCollider.Count-1; x >= 0; x --)
        {
 
            if (delayedDelegatesCollider[x] == method)
            {
 
				delayedDelegatesTimeCollider[x] = Time.fixedTime;
 
			}
 
 
        }
 
		DelayedDelegates.RunDelegates();
 
	}
 
 
	public static void Add(DelayedDelegate method, float delay) { PreAdd(); delayedDelegates.Add(method); delayedDelegatesTime.Add(Time.fixedTime + delay); delayedDelegatesRealtime.Add(false); delayedDelegatesFrame.Add(0); }
 
	/// <summary>
	/// Delays the function until the next frame
	/// </summary>
	/// <param name='method'>
	/// Method.
	/// </param>
	public static void Skip(DelayedDelegate method, int skipFrames) { PreAdd(); delayedDelegates.Add(method); delayedDelegatesTime.Add(0); delayedDelegatesRealtime.Add(false); delayedDelegatesFrame.Add(Time.frameCount+skipFrames); }
 
	public static void Add(DelayedDelegate method, float delay, bool realTime) { PreAdd(); delayedDelegates.Add(method); delayedDelegatesTime.Add(Time.fixedTime + delay);  delayedDelegatesRealtime.Add(true); delayedDelegatesFrame.Add(0); }
 
	public static void Add(DelayedDelegateParameters method, DelegateParameters param, float delay) { PreAdd(); delayedDelegatesParams.Add(method); delayedDelegatesTimeParams.Add(Time.fixedTime + delay); delegateParameters.Add(param); }
	public static void Add(DelayedDelegateString method, string param,  float delay) { PreAdd(); delayedDelegatesString.Add(method); delayedDelegatesTimeString.Add(Time.fixedTime + delay); delegateParametersString.Add(param);  }
 
	public static void Add(DelayedDelegateInt method, int param, float delay) { PreAdd(); delayedDelegatesInt.Add(method); delayedDelegatesTimeInt.Add(Time.fixedTime + delay); delayedDelegatesIntRealtime.Add(false); delayedDelegatesIntFrame.Add(0); delegateParametersInt.Add(param); }
 
	public static void Skip(DelayedDelegateInt method, int param, int skipFrames) { PreAdd(); delayedDelegatesInt.Add(method); delayedDelegatesTimeInt.Add(0); delayedDelegatesIntRealtime.Add(false); delayedDelegatesIntFrame.Add(Time.frameCount+skipFrames); delegateParametersInt.Add(param); }
 
	public static void Add(DelayedDelegateFloat method, float param, float delay) { PreAdd(); delayedDelegatesFloat.Add(method); delayedDelegatesTimeFloat.Add(Time.fixedTime + delay); delegateParametersFloat.Add(param); }
	public static void Add(DelayedDelegateVector3 method, Vector3 param, float delay) { PreAdd(); delayedDelegatesVector3.Add(method); delayedDelegatesTimeVector3.Add(Time.fixedTime + delay); delegateParametersVector3.Add(param); }
	public static void Add(DelayedDelegateVector2 method, Vector2 param, float delay) { PreAdd(); delayedDelegatesVector2.Add(method); delayedDelegatesTimeVector2.Add(Time.fixedTime + delay); delegateParametersVector2.Add(param); }
	public static void Add(DelayedDelegateGameObject method, GameObject param, float delay) { PreAdd(); delayedDelegatesGameObject.Add(method); delayedDelegatesTimeGameObject.Add(Time.fixedTime + delay); delegateParametersGameObject.Add(param); }
    public static void Add(DelayedDelegateCollider method, Collider param, float delay) { PreAdd(); delayedDelegatesCollider.Add(method); delayedDelegatesTimeCollider.Add(Time.fixedTime + delay); delegateParametersCollider.Add(param); }
 
	static void PreAdd()
	{
 
		if (delegateRunner == null)
		{
 
			GameObject go = new GameObject("DelayedDelegates");
			delegateRunner = (DelayedDelegateRun) go.AddComponent<DelayedDelegateRun>();
 
		}
 
		hasDelegates = true;
 
	}
 
    public static void RunDelegates()
    {
 
		if (hasDelegates)
		{
 
			for (int x = delayedDelegates.Count-1; x >= 0; x --)
	        {
 
	            if ((Time.fixedTime >= delayedDelegatesTime[x] && delayedDelegatesFrame[x] == 0 && !delayedDelegatesRealtime[x]) ||  
					(Time.frameCount >= delayedDelegatesFrame[x] && delayedDelegatesTime[x] == 0 && !delayedDelegatesRealtime[x]) ||
					(Time.realtimeSinceStartup >= delayedDelegatesTime[x] && delayedDelegatesFrame[x] == 0 && delayedDelegatesRealtime[x]))
	            {
 
					delayedDelegates[x]();
	                delayedDelegates.RemoveAt(x);
					delayedDelegatesTime.RemoveAt(x);
					delayedDelegatesFrame.RemoveAt(x);
 
	            }
 
 
	        }
 
			for (int x = delayedDelegatesParams.Count-1; x >= 0; x --)
	        {
 
	            if (Time.fixedTime >= delayedDelegatesTimeParams[x])
	            {
 
					delayedDelegatesParams[x](delegateParameters[x]);
	                delayedDelegatesParams.RemoveAt(x);
					delayedDelegatesTimeParams.RemoveAt(x);
					delegateParameters.RemoveAt(x);
 
				}
 
 
	        }
 
 
			for (int x = delayedDelegatesInt.Count-1; x >= 0; x --)
	        {
 
	            if ((Time.fixedTime >= delayedDelegatesTimeInt[x] && delayedDelegatesIntFrame[x] == 0 && !delayedDelegatesIntRealtime[x]) ||  
					(Time.frameCount >= delayedDelegatesIntFrame[x] && delayedDelegatesTimeInt[x] == 0 && !delayedDelegatesIntRealtime[x]) ||
					(Time.realtimeSinceStartup >= delayedDelegatesTimeInt[x] && delayedDelegatesIntFrame[x] == 0 && delayedDelegatesIntRealtime[x]))
	            {
 
					delayedDelegatesInt[x](delegateParametersInt[x]);
	                delayedDelegatesInt.RemoveAt(x);
					delayedDelegatesTimeInt.RemoveAt(x);
					delegateParametersInt.RemoveAt(x);
					delayedDelegatesIntFrame.RemoveAt(x);
					delayedDelegatesIntRealtime.RemoveAt(x);
 
				}
 
 
	        }
 
			for (int x = delayedDelegatesFloat.Count-1; x >= 0; x --)
	        {
 
	            if (Time.fixedTime >= delayedDelegatesTimeFloat[x])
	            {
 
					delayedDelegatesFloat[x](delegateParametersFloat[x]);
	                delayedDelegatesFloat.RemoveAt(x);
					delayedDelegatesTimeFloat.RemoveAt(x);
					delegateParametersFloat.RemoveAt(x);
 
				}
 
	        }
 
			for (int x = delayedDelegatesString.Count-1; x >= 0; x --)
	        {
 
	            if (Time.fixedTime >= delayedDelegatesTimeString[x])
	            {
 
					delayedDelegatesString[x](delegateParametersString[x]);
	                delayedDelegatesString.RemoveAt(x);
					delayedDelegatesTimeString.RemoveAt(x);
					delegateParametersString.RemoveAt(x);
 
				}
 
	        }
 
			for (int x = delayedDelegatesVector3.Count-1; x >= 0; x --)
	        {
 
	            if (Time.fixedTime >= delayedDelegatesTimeVector3[x])
	            {
 
					delayedDelegatesVector3[x](delegateParametersVector3[x]);
	                delayedDelegatesVector3.RemoveAt(x);
					delayedDelegatesTimeVector3.RemoveAt(x);
					delegateParametersVector3.RemoveAt(x);
 
				}
 
	        }
 
			for (int x = delayedDelegatesVector2.Count-1; x >= 0; x --)
	        {
 
	            if (Time.fixedTime >= delayedDelegatesTimeVector2[x])
	            {
 
					delayedDelegatesVector2[x](delegateParametersVector2[x]);
	                delayedDelegatesVector2.RemoveAt(x);
					delayedDelegatesTimeVector2.RemoveAt(x);
					delegateParametersVector2.RemoveAt(x);
 
				}
 
	        }
 
			for (int x = delayedDelegatesGameObject.Count-1; x >= 0; x --)
	        {
 
	            if (Time.fixedTime >= delayedDelegatesTimeGameObject[x])
	            {
 
					delayedDelegatesGameObject[x](delegateParametersGameObject[x]);
	                delayedDelegatesGameObject.RemoveAt(x);
					delayedDelegatesTimeGameObject.RemoveAt(x);
					delegateParametersGameObject.RemoveAt(x);
 
				}
 
 
	        }
 
			for (int x = delayedDelegatesCollider.Count-1; x >= 0; x --)
	        {
 
	            if (Time.fixedTime >= delayedDelegatesTimeCollider[x])
	            {
 
					delayedDelegatesCollider[x](delegateParametersCollider[x]);
	                delayedDelegatesCollider.RemoveAt(x);
					delayedDelegatesTimeCollider.RemoveAt(x);
					delegateParametersCollider.RemoveAt(x);
 
				}
 
 
	        }
 
			if (delayedDelegates.Count == 0 && delayedDelegatesFloat.Count == 0 && delayedDelegatesGameObject.Count == 0 && delayedDelegatesParams.Count == 0
			    && delayedDelegatesInt.Count == 0 && delayedDelegatesString.Count == 0 && delayedDelegatesVector2.Count == 0 && delayedDelegatesVector3.Count == 0)
			{
				hasDelegates = false;
			}
 
		}
 
 
    }
 
}
 
public class DelayedDelegateRun : MonoBehaviour 
{
 
	void Update()
	{
 
		DelayedDelegates.RunDelegates();
 
	}
 
}
}
