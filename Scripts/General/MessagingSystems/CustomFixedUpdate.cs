// Original url: http://wiki.unity3d.com/index.php/CustomFixedUpdate
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MessagingSystems/CustomFixedUpdate.cs
// File based on original modification date of: 5 July 2014, at 23:21. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MessagingSystems
{
Contents [hide] 
1 Description 
1.1 MaxAllowedTimeStep 
1.2 Update() / Update(dt) 
1.3 OnFixedUpdateCallback 
2 Examples 
3 CustomFixedUpdate.cs 

Description This helper class does something similar to what Unity does behind the scenes for FixedUpdate. It allows you to "create" as many FixedUpdate callbacks you like, each with it's own rate. You can create callbacks that run 10 times per second or 10000 times per second, independent from the visual Update rate and independent from Unity's FixedUpdate. 
MaxAllowedTimeStep The "MaxAllowedTimeStep" works similar to Unity's Time.maximumDeltaTime, however by default it's disabled (set to 0). Keep in mind that Unity will already limit deltaTime to maximumDeltaTime which is 1/3 by default. If a frame takes longer than these 0.3333 seconds, Unity will clamp the deltaTime to that value so you're not get caught in a "downward spiral". 
MaxAllowedTimeStep on the other hand only monitors it's own callback to ensure the (real) time doesn't exeed this value during one "update". if it does, it simply terminates the update without calling all required callbacks. 
Update() / Update(dt) You have to call Update() to actually invoke the callback(s). Usually you would call it once per frame (inside Update of a MonoBehaviour for example). You can pass a custom "deltaTime" to the method, if omitted Time.deltaTime is used. 
OnFixedUpdateCallback When you create an instance of the CustomFixedUpdate class, you have to provide a callback function which will be called at the desired rate. The callback has the "fixedDeltaTime" as parameter. 
Examples // C#
// inside a MonoBehaviour
private CustomFixedUpdate FU_instance;
 
void Awake()
{
    FU_instance = new CustomFixedUpdate(0.1f, OnFixedUpdate);
}
 
void Update()
{
    FU_instance.Update();
}
 
// this method will be called 10 times per second
void OnFixedUpdate(float dt)
{
 
}
Some constructor variants: 
 
    new CustomFixedUpdate(10f, OnFixedUpdate);  // OnFixedUpdate is called once every 10 seconds
 
    new CustomFixedUpdate(0.01f, OnFixedUpdate);  // OnFixedUpdate is called 100 times per second
 
    // You can also specify the desired FPS like that:
 
    new CustomFixedUpdate(OnFixedUpdate, 10000);  // OnFixedUpdate is called 10000 times per second
 
    // Since it's a delegate you can also provide a closure / lambda
    new CustomFixedUpdate(60, (dt)=>{
        Debug.Log("This will be printed every " + dt + " seconds")
    });An example with custom delta time: 
FU_instance = new CustomFixedUpdate(OnFixedUpdate, 10);
 
//[...]
 
void Update()
{
    // This will execute "OnFixedUpdate" 5 times per frame
    FU_instance.Update(0.5f);
}

CustomFixedUpdate.cs using UnityEngine;
using System.Collections;
 
public class CustomFixedUpdate
{
	public delegate void OnFixedUpdateCallback(float aDeltaTime);
	private float m_FixedTimeStep;
	private float m_Timer = 0;
 
	private OnFixedUpdateCallback m_Callback;
 
	private float m_MaxAllowedTimeStep = 0f;
	public float MaxAllowedTimeStep
	{
		get { return m_MaxAllowedTimeStep; }
		set { m_MaxAllowedTimeStep = value;}
	}
 
	public float deltaTime
	{
		get { return m_FixedTimeStep; }
		set	{m_FixedTimeStep = Mathf.Max (value, 0.000001f); } // max rate: 1000000
	}
	public float updateRate
	{
		get { return 1.0f / deltaTime; }
		set { deltaTime = 1.0f / value; }
	}
 
	public CustomFixedUpdate(float aTimeStep, OnFixedUpdateCallback aCallback, float aMaxAllowedTimestep)
	{
		if (aCallback == null)
			throw new System.ArgumentException("CustomFixedUpdate needs a valid callback");
		if (aTimeStep <= 0f)
			throw new System.ArgumentException("TimeStep needs to be greater than 0");
		deltaTime = aTimeStep;
		m_Callback = aCallback;
		m_MaxAllowedTimeStep = aMaxAllowedTimestep;
	}
	public CustomFixedUpdate(float aTimeStep, OnFixedUpdateCallback aCallback) : this(aTimeStep, aCallback, 0f) {}
	public CustomFixedUpdate(OnFixedUpdateCallback aCallback) : this(0.01f, aCallback, 0f) {}
	public CustomFixedUpdate(OnFixedUpdateCallback aCallback, float aFPS, float aMaxAllowedTimestep) : this(1f/aFPS, aCallback, aMaxAllowedTimestep){}
	public CustomFixedUpdate(OnFixedUpdateCallback aCallback, float aFPS) : this(aCallback, aFPS, 0f){}
 
 
	public void Update(float aDeltaTime)
	{
		m_Timer -= aDeltaTime;
		if (m_MaxAllowedTimeStep > 0)
		{
			float timeout = Time.realtimeSinceStartup + m_MaxAllowedTimeStep;
			while(m_Timer < 0f && Time.realtimeSinceStartup < timeout)
			{
				m_Callback(m_FixedTimeStep);
				m_Timer += m_FixedTimeStep;
			}
		}
		else
		{
			while(m_Timer < 0f)
			{
				m_Callback(m_FixedTimeStep);
				m_Timer += m_FixedTimeStep;
			}
		}
	}
 
	public void Update()
	{
		Update(Time.deltaTime);
	}
}
}
