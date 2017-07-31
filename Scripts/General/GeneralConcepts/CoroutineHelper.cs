// Original url: http://wiki.unity3d.com/index.php/CoroutineHelper
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/GeneralConcepts/CoroutineHelper.cs
// File based on original modification date of: 13 February 2014, at 16:46. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
Contents [hide] 
1 Description 
2 Examples 
2.1 Run.EachFrame 
2.2 Run.Every 
2.3 Run.After 
2.4 Run.Lerp 
2.5 Run.OnDelegate 
2.6 Run.OnGUI 
2.7 Run.Coroutine 
2.8 Run.CreateGUIWindow 
3 Files 
3.1 CoroutineHelper.cs 
3.2 Run.cs 
3.3 MonoBehaviourSingleton.cs 
3.4 GUIHelper.cs 
3.5 SimpleEvent.cs 
4 Zip archive 

Description This is a set of helper classes which allows you to run a delegate at specific times. You can simply delay a call, call it repeatedly or even schedule it as OnGUI call. 
The supported methods are: 
* Run.EachFrame(delegate)
* Run.Every(initialDelay, repeatDelay, delegate)
* Run.After(delay, delegate)
* Run.Lerp(duration, float-delegate)
* Run.OnDelegate(SimpleEvent, delegate)
* Run.OnGUI(duration, delegate)
* Run.Coroutine(someUnityIEnumerator)
Each of these calls retun a Run instance which can be used to abort the scheduled task or to wait for it to complete. 
To wait for a task you can use the "WaitFor" property inside a coroutine or use "ExecuteWhenDone(delegate)" to run a delegate when the task is finished. 
    yield return someRunTask.WaitFor;
    // [...]
    someRunTask.ExecuteWhenDone(()=>{
        // some code
    });Additionally there's another little helperclass to create a GUI window on-the-fly: 
* Run.CreateGUIWindow(rect, title, CTempWindow-delegate)
Examples Run.EachFrame     Run.EachFrame(()=>{
        Debug.Log("This is executed every frame like Update");
    });Run.Every     Run.Every(10,2,()=>{
        Debug.Log("Something");
    });After running this code once after 10 seconds it will print "Something" every 2 seconds. 
Run.After     Run.After(4, ()=>{
        Debug.Log("4 Seconds later");
    });
    Debug.Log("See you in 4 seconds");This will print "See you in 4 seconds" and 4 seconds later it prints "4 Seconds later" 
Run.Lerp     Run.Lerp(3,(t)=>{
        Debug.Log("lerp value: " + t);
    });This will execute the Debug.Log every frame for 3 seconds. The value t is increasing linearly from 0.0f to 1.0f during this time. 


Run.OnDelegate     SimpleEvent someEvent = new SimpleEvent();
    Run.OnDelegate(someEvent, ()=>{
        Debug.Log("Hello World");
    });
    someEvent.Run();when "someEvent.Run();" is executed it will print "Hello World" 
Run.OnGUI     Run.OnGUI(5, ()=>{
        if (GUI.Button(new Rect(10,10,100,30),"Test"))
        {
            Debug.Log("Test clicked");
        }
    });If you execute this it will display the GUI button for 5 seconds. The passed delegate will be executed inside OnGUI. If you pass 0 as duration it will run "forever" or until you call Abort on the returned Run instance. 
    Run instance = null;
    instance = Run.OnGUI(0, ()=>{
        if (GUI.Button(new Rect(10,10,100,30),"remove me"))
        {
            instance.Abort();
        }
    });This will display the button at the given coordinates until you click the button. Note: Since the delegate in this example is a closure the "instance" variable has to be declared before you create the closure. So this is not possible: 
    // this doesn't work!!
    Run instance = Run.OnGUI(0, ()=>{
        if (GUI.Button(new Rect(10,10,100,30),"remove me"))
        {
            instance.Abort(); // you can't use "instance" inside the closure
        }
    });

Run.Coroutine     Run.Coroutine(SomeCoroutine())This is basically a replacement for StartCoroutine. It can be used everywhere since it's static. It also returns a Run instance which can be used to wait for the coroutine even multiple times or let you execute something when it's finished. Note: The passed coroutine will be run on the CoroutineHelper gameobject. 
Run.CreateGUIWindow     Run.CreateGUIWindow(new Rect(10,10,200,100), "WindowTitle", (windowInstance)=>
    {
        GUILayout.Label("some text");
        if (GUILayout.Button("close"))
            windowInstance.Close();
    });This creates an instance of the Run.CTempWindow class which just holds the basic information required for a GUI.Window like title, windowid, position and a Run instance which displays the window utilizing Run.OnGUI. The CTempWindow instance is passed to the delegate and also returned by the CreateGUIWindow function. 


Files CoroutineHelper.cs using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class CoroutineHelper : MonoBehaviourSingleton<CoroutineHelper>
{
	private List<Run> m_OnGUIObjects = new List<Run>();
	public int ScheduledOnGUIItems
	{
		get {return m_OnGUIObjects.Count;}
	}
	public Run Add(Run aRun)
	{
		if (aRun != null)
			m_OnGUIObjects.Add(aRun);
		return aRun;
	}
	void OnGUI()
	{
		for(int i = 0; i < m_OnGUIObjects.Count; i++)
		{
			Run R = m_OnGUIObjects[i];
			if (!R.abort && !R.isDone && R.onGUIaction != null)
				R.onGUIaction();
			else
				R.isDone = true;
		}
	}
	void Update()
	{
		for(int i = m_OnGUIObjects.Count-1; i >= 0; i--)
		{
			if (m_OnGUIObjects[i].isDone)
				m_OnGUIObjects.RemoveAt(i);
		}
	}
}Run.cs using UnityEngine;
using System.Collections;
 
public class Run
{
	public bool isDone;
	public bool abort;
	private IEnumerator action;
	public System.Action onGUIaction = null;
 
	#region Run.EachFrame
	public static Run EachFrame(System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunEachFrame(tmp, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunEachFrame(Run aRun, System.Action aAction)
	{
		aRun.isDone = false;
		while (true)
		{
			if (!aRun.abort && aAction != null)
				aAction();
			else
				break;
			yield return null;
		}
		aRun.isDone = true;
	}
	#endregion Run.EachFrame
 
	#region Run.Every
	public static Run Every(float aInitialDelay, float aDelay, System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunEvery(tmp,aInitialDelay, aDelay, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunEvery(Run aRun, float aInitialDelay, float aSeconds, System.Action aAction)
	{
		aRun.isDone = false;
		if (aInitialDelay > 0f)
			yield return new WaitForSeconds(aInitialDelay);
		else
		{
			int FrameCount = Mathf.RoundToInt(-aInitialDelay);
			for (int i = 0; i < FrameCount; i++)
				yield return null;
		}
		while (true)
		{
			if (!aRun.abort && aAction != null)
				aAction();
			else
				break;
			if (aSeconds > 0)
				yield return new WaitForSeconds(aSeconds);
			else
			{
				int FrameCount = Mathf.Max(1,Mathf.RoundToInt(-aSeconds));
				for (int i = 0; i < FrameCount; i++)
					yield return null;
			}
		}
		aRun.isDone = true;
	}
	#endregion Run.Every
 
	#region Run.After
	public static Run After(float aDelay, System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunAfter(tmp, aDelay, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunAfter(Run aRun, float aDelay, System.Action aAction)
	{
		aRun.isDone = false;
		yield return new WaitForSeconds(aDelay);
		if (!aRun.abort && aAction != null)
			aAction();
		aRun.isDone = true;
	}
	#endregion Run.After
 
	#region Run.Lerp
	public static Run Lerp(float aDuration, System.Action<float> aAction)
	{
		var tmp = new Run();
		tmp.action = _RunLerp(tmp, aDuration, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunLerp(Run aRun, float aDuration, System.Action<float> aAction)
	{
		aRun.isDone = false;
		float t = 0f;
		while (t < 1.0f)
		{
			t = Mathf.Clamp01(t + Time.deltaTime / aDuration);
			if (!aRun.abort && aAction != null)
				aAction(t);
			yield return null;
		}
		aRun.isDone = true;
 
	}
	#endregion Run.Lerp
 
	#region Run.OnDelegate
	public static Run OnDelegate(SimpleEvent aDelegate, System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunOnDelegate(tmp, aDelegate, aAction);
		tmp.Start();
		return tmp;
	}
 
	private static IEnumerator _RunOnDelegate(Run aRun, SimpleEvent aDelegate, System.Action aAction)
	{
		aRun.isDone = false;
		System.Action action = ()=>{
			aAction();
		};
		aDelegate.Add(action);
		while (!aRun.abort && aAction != null)
		{
			yield return null;
		}
		aDelegate.Remove(action);
		aRun.isDone = true;
	}
	#endregion Run.OnDelegate
 
	#region Run.Coroutine
	public static Run Coroutine(IEnumerator aCoroutine)
	{
		var tmp = new Run();
		tmp.action = _Coroutine(tmp, aCoroutine);
		tmp.Start();
		return tmp;
	}
 
	private static IEnumerator _Coroutine(Run aRun, IEnumerator aCoroutine)
	{
		yield return CoroutineHelper.Instance.StartCoroutine(aCoroutine);
		aRun.isDone = true;
	}
	#endregion Run.Coroutine
 
	public static Run OnGUI(float aDuration, System.Action aAction)
	{
		var tmp = new Run();
		tmp.onGUIaction = aAction;
		if (aDuration > 0.0f)
			tmp.action = _RunAfter(tmp, aDuration, null);
		else
			tmp.action = null;
		tmp.Start();
		CoroutineHelper.Instance.Add(tmp);
		return tmp;
	}
 
	public class CTempWindow
	{
		public Run inst;
		public Rect pos;
		public string title;
		public int winID = GUIHelper.GetFreeWindowID();
		public void Close(){ inst.Abort();}
	}
 
	public static CTempWindow CreateGUIWindow(Rect aPos, string aTitle, System.Action<CTempWindow> aAction)
	{
		CTempWindow tmp = new CTempWindow();
		tmp.pos = aPos;
		tmp.title = aTitle;
		tmp.inst = OnGUI(0,()=>{
			tmp.pos = GUI.Window(tmp.winID, tmp.pos, (id)=>{
				aAction(tmp);
			},tmp.title);
		});
		return tmp;
	}
 
 
	private void Start()
	{
		if (action != null)
		CoroutineHelper.Instance.StartCoroutine(action);
	}
 
	public Coroutine WaitFor
	{
		get
		{
			return CoroutineHelper.Instance.StartCoroutine(_WaitFor(null));
		}
	}
	public IEnumerator _WaitFor(System.Action aOnDone)
	{
		while(!isDone)
			yield return null;
		if (aOnDone != null)
			aOnDone();
	}
	public void Abort()
	{
		abort = true;
	}
	public Run ExecuteWhenDone(System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _WaitFor(aAction);
		tmp.Start();
		return tmp;
	}
}MonoBehaviourSingleton.cs public class MonoBehaviourSingleton< TSelfType > : MonoBehaviour where TSelfType : MonoBehaviour
{
	private static TSelfType m_Instance = null;
	public static TSelfType Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = (TSelfType)FindObjectOfType(typeof(TSelfType));
				if (m_Instance == null)
				{
					m_Instance = (new GameObject(typeof(TSelfType).Name)).AddComponent<TSelfType>();
				}
				DontDestroyOnLoad(m_Instance.gameObject);
			}
			return m_Instance;
		}
	}
}GUIHelper.cs public static class GUIHelper
{
	private static int m_WinIDCounter = 2000;
	public static int GetFreeWindowID()
	{
		return m_WinIDCounter++;
	}
}SimpleEvent.cs public class SimpleEvent
{
	private System.Action m_Delegate = ()=>{};
	public void Add(System.Action aDelegate)
	{
		m_Delegate += aDelegate;
	}
	public void Remove(System.Action aDelegate)
	{
		m_Delegate -= aDelegate;
	}
	public void Run()
	{
		m_Delegate();
	}
}Zip archive Media:CoroutineHelperPack.zip 
}
