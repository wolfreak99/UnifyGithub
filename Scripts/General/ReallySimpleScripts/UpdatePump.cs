// Original url: http://wiki.unity3d.com/index.php/UpdatePump
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/UpdatePump.cs
// File based on original modification date of: 24 January 2013, at 19:06. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Brandon Edmark 
Description This is a MonoBehaviour that allows non-MonoBehaviour objects to hook into Unity's Update, FixedUpdate, and LateUpdate loops. It is useful if you have objects outside the Unity MonoBehaviour/GameObject system that need to update like MonoBehaviours can. It can also be used to boost performance vs Unity's automatic Update calls if there are hundreds of MonoBehaviours updating in a scene. 
Usage Call UpdatePump.Register, inputting the name of the method you want to run and the type of Update you want to run it on. The update type, fittingly, is specified by the UpdateType enum. The method must have no parameters and must return void. 
As an UpdatePump is just a MonoBehaviour, you can use its "enabled" property to pause updating of all the objects registered to it. 
UpdatePump.cs using UnityEngine;
using System;
using System.Collections.Generic;
 
/// <summary>
/// A way of executing Unity's update messages on non-Monobehaviour objects.
/// </summary>
public class UpdatePump : MonoBehaviour {
 
    public static bool hideGameObject = true;
 
	private event Action fixedUpdateTarget;
	private event Action updateTarget;
	private event Action lateUpdateTarget;
 
	public void Register (Action action, UpdateType updateType)
	{
		switch (updateType)
		{
		case UpdateType.FixedUpdate:
			fixedUpdateTarget += action;
			return;
		case UpdateType.Update:
			updateTarget += action;
			return;
		case UpdateType.LateUpdate:
			lateUpdateTarget += action;
			return;
		}
	}
 
	public void UnRegister (Action action, UpdateType updateType)
	{
		switch (updateType) 
		{
		case UpdateType.FixedUpdate:
			fixedUpdateTarget -= action;
			return;
		case UpdateType.Update:
			updateTarget -= action;
			return;
		case UpdateType.LateUpdate:
			lateUpdateTarget -= action;
			return;
		}
	}
 
	void FixedUpdate ()
	{
		DoUpdating(fixedUpdateTarget);
	}
 
	void Update ()
	{
		DoUpdating (updateTarget);
	}
 
	void LateUpdate ()
	{
		DoUpdating (lateUpdateTarget);
	}
 
	void DoUpdating (Action currentEvent)
	{
		if (currentEvent != null)
		{
			currentEvent ();
		}
	}
 
	void OnDestroy ()
	{
		RemoveAll (fixedUpdateTarget);
		RemoveAll (updateTarget);
		RemoveAll (lateUpdateTarget);
	}
 
	void RemoveAll (Action deadEvent)
	{
		if (deadEvent != null) 
		{
			Delegate[] clientList = deadEvent.GetInvocationList ();
			foreach (Delegate d in clientList) {
				deadEvent -= (d as Action);
			}
		}
	}
}
 
public enum UpdateType
{
	FixedUpdate,
	Update,
	LateUpdate
}
}
