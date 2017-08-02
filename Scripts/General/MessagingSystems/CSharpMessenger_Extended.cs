/*************************
 * Original url: http://wiki.unity3d.com/index.php/CSharpMessenger_Extended
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MessagingSystems/CSharpMessenger_Extended.cs
 * File based on original modification date of: 3 October 2014, at 03:27. 
 *
 * Author: Magnus Wolffelt (with Rod Hyde quotations), Julie Iaccarino (Callback return extensions, cleanup, covariance, slowdown, then speedup) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.MessagingSystems
{
    Contents [hide] 
    1 Description 
    2 Usage 
    2.1 Writing an event listener 
    2.2 Registering an event listener 
    2.3 Unregistering an event listener 
    2.4 Registering a callback return 
    2.4.1 Warning 
    2.5 Broadcasting an event 
    3 Code 
    3.1 Messenger.cs 
    3.2 MessengerUnitTest.cs 
    
    Description This tries to be a programmer-mistake-resilient delegate-based event system for C#. It uses generics to achieve static typing. Based on and inspired by Rod Hyde's CSharpMessenger, please refer to it for more usage details. The use of generics means that parameters can be of any type and are not restricted to subclasses of a special Message class - if a callback only needs a float then supply it with a float. 
    Usage The main difference in usage from Hyde's implementation, is that with this messenger you can not have several events with same name, but different parameter signature. So for example, if you register a listener for "myEvent" with no parameters, and later try to register an event listener for "myEvent" that takes a float parameter, an exception will be thrown. Furthermore, there's an optional MessengerMode that allows for requiring at least one listener to exist when broadcasting an event. Generally, this Messenger will throw a lot of exceptions as soon as the programmer makes mistakes. Not all potential errors can be covered, but it tries to be strict in order to prevent silent, undetected bugs. 
    Writing an event listener     void OnSpeedChanged(float speed)
        {
            this.speed = speed;
        }Registering an event listener     void OnEnable()
        {
            Messenger<float>.AddListener("speed changed", OnSpeedChanged);
        }Unregistering an event listener     void OnDisable()
        {
            Messenger<float>.RemoveListener("speed changed", OnSpeedChanged);
        }Registering a callback return In some cases, a return value will be desired when sending out a message. Callback returns act in this capacity, allowing a broadcaster to receive a return value for every subscriber listening to the event type. 
    The listener might look like this: 
        void OnDisable()
        {
            Messenger.AddListener<int>("getScore", GetScore);
        }
     
        int GetScore()
        {
            return 0;
        }Where the <T> parameter on the AddListener function declares a return type. It is still possible to use one, two, or three types for parameters on the main Messenger class. 
    The broadcast may look like this: 
        void OnDisable()
        {
            Messenger.Broadcast<int>("getScore", OnGetScore);
        }
     
        void OnGetScore(int i)
        {
            Debug.Log(string.Format("Score is {0}...", i));
        }It's important the signature matches the listener and the broadcaster. 
    Warning RemoveListener should always be called on messages when loading a new level. Otherwise many MissingReferenceExceptions will be thrown, when invoking messages on destroyed objects. For example: 1. We registered a "speed changed" message in a Level1 scene. Afterwards the scene has been destroyed, but the "speed changed" message is still pointing to the OnSpeedChanged message handler in the destroyed class. 2. We loaded Level2 and registered another "speed changed" message, but the previous reference to the destroyed object hasn't been removed. We'll get a MissingReferenceException, because by invoking the "speed changed" message, the messaging system will first invoke the OnSpeedChanged handler of the destroyed object. 
    
    
    Broadcasting an event     if (speed != lastSpeed)
        {
            Messenger<float>.Broadcast("speed changed", speed);
        }Code There are two files, Messenger.cs and MessengerUnitTest.cs. The last one is not required for usage. 
    Messenger.cs // Messenger.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
    // Version 1.4 by Julie Iaccarino, biscuitWizard @ github.com
    //
    // Inspired by and based on Rod Hyde's Messenger:
    // http://www.unifycommunity.com/wiki/index.php?title=CSharpMessenger
    //
    // This is a C# messenger (notification center). It uses delegates
    // and generics to provide type-checked messaging between event producers and
    // event consumers, without the need for producers or consumers to be aware of
    // each other. The major improvement from Hyde's implementation is that
    // there is more extensive error detection, preventing silent bugs.
    //
    // Usage example:
    // Messenger<float>.AddListener("myEvent", MyEventHandler);
    // ...
    // Messenger<float>.Broadcast("myEvent", 1.0f);
    //
    // Callback example:
    // Messenger<float>.AddListener<string>("myEvent", MyEventHandler);
    // private string MyEventHandler(float f1) { return "Test " + f1; }
    // ...
    // Messenger<float>.Broadcast<string>("myEvent", 1.0f, MyEventCallback);
    // private void MyEventCallback(string s1) { Debug.Log(s1"); }
     
    using System;
    using System.Collections.Generic;
    using System.Linq;
     
    public enum MessengerMode {
    	DONT_REQUIRE_LISTENER,
    	REQUIRE_LISTENER,
    }
     
    static internal class MessengerInternal {
    	readonly public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
    	static public readonly MessengerMode DEFAULT_MODE = MessengerMode.REQUIRE_LISTENER;
     
     	static public void AddListener(string eventType, Delegate callback) {
    		MessengerInternal.OnListenerAdding(eventType, callback);
    		eventTable[eventType] = Delegate.Combine(eventTable[eventType], callback);
    	}
     
    	static public void RemoveListener(string eventType, Delegate handler) {
    		MessengerInternal.OnListenerRemoving(eventType, handler);	
    		eventTable[eventType] = Delegate.Remove(eventTable[eventType], handler);
    		MessengerInternal.OnListenerRemoved(eventType);
    	}
     
    	static public T[] GetInvocationList<T>(string eventType) {
    		Delegate d;
    		if(eventTable.TryGetValue(eventType, out d)) {
    			if(d != null) {
    				return d.GetInvocationList().Cast<T>().ToArray();
    			} else {
    				throw MessengerInternal.CreateBroadcastSignatureException(eventType);
    			}
    		}
    		return null;
    	}
     
    	static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded) {
    		if (!eventTable.ContainsKey(eventType)) {
    			eventTable.Add(eventType, null);
    		}
     
    		var d = eventTable[eventType];
    		if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
    			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
    		}
    	}
     
    	static public void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved) {
    		if (eventTable.ContainsKey(eventType)) {
    			var d = eventTable[eventType];
     
    			if (d == null) {
    				throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", eventType));
    			} else if (d.GetType() != listenerBeingRemoved.GetType()) {
    				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
    			}
    		} else {
    			throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", eventType));
    		}
    	}
     
    	static public void OnListenerRemoved(string eventType) {
    		if (eventTable[eventType] == null) {
    			eventTable.Remove(eventType);
    		}
    	}
     
    	static public void OnBroadcasting(string eventType, MessengerMode mode) {
    		if (mode == MessengerMode.REQUIRE_LISTENER && !eventTable.ContainsKey(eventType)) {
    			throw new MessengerInternal.BroadcastException(string.Format("Broadcasting message {0} but no listener found.", eventType));
    		}
    	}
     
    	static public BroadcastException CreateBroadcastSignatureException(string eventType) {
    		return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
    	}
     
    	public class BroadcastException : Exception {
    		public BroadcastException(string msg)
    			: base(msg) {
    		}
    	}
     
    	public class ListenerException : Exception {
    		public ListenerException(string msg)
    			: base(msg) {
    		}
    	}
    }
     
    // No parameters
    static public class Messenger { 
    	static public void AddListener(string eventType, Action handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void AddListener<TReturn>(string eventType, Func<TReturn> handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void RemoveListener(string eventType, Action handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void RemoveListener<TReturn>(string eventType, Func<TReturn> handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void Broadcast(string eventType) {
    		Broadcast(eventType, MessengerInternal.DEFAULT_MODE);
    	}
     
    	static public void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall) {
    		Broadcast(eventType, returnCall, MessengerInternal.DEFAULT_MODE);
    	}
     
     	static public void Broadcast(string eventType, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Action>(eventType);
     
    		foreach(var callback in invocationList)
    			callback.Invoke();
    	}
     
    	static public void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Func<TReturn>>(eventType);
     
    		foreach(var result in invocationList.Select(del => del.Invoke()).Cast<TReturn>()) {
    			returnCall.Invoke(result);
    		}
    	}
    }
     
    // One parameter
    static public class Messenger<T> {
    	static public void AddListener(string eventType, Action<T> handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void AddListener<TReturn>(string eventType, Func<T, TReturn> handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void RemoveListener(string eventType, Action<T> handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void RemoveListener<TReturn>(string eventType, Func<T, TReturn> handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void Broadcast(string eventType, T arg1) {
    		Broadcast(eventType, arg1, MessengerInternal.DEFAULT_MODE);
    	}
     
    	static public void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall) {
    		Broadcast(eventType, arg1, returnCall, MessengerInternal.DEFAULT_MODE);
    	}
     
    	static public void Broadcast(string eventType, T arg1, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Action<T>>(eventType);
     
    		foreach(var callback in invocationList)
    			callback.Invoke(arg1);
    	}
     
    	static public void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Func<T, TReturn>>(eventType);
     
    		foreach(var result in invocationList.Select(del => del.Invoke(arg1)).Cast<TReturn>()) {
    			returnCall.Invoke(result);
    		}
    	}
    }
     
     
    // Two parameters
    static public class Messenger<T, U> { 
    	static public void AddListener(string eventType, Action<T, U> handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void AddListener<TReturn>(string eventType, Func<T, U, TReturn> handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void RemoveListener(string eventType, Action<T, U> handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void RemoveListener<TReturn>(string eventType, Func<T, U, TReturn> handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void Broadcast(string eventType, T arg1, U arg2) {
    		Broadcast(eventType, arg1, arg2, MessengerInternal.DEFAULT_MODE);
    	}
     
    	static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, Action<TReturn> returnCall) {
    		Broadcast(eventType, arg1, arg2, returnCall, MessengerInternal.DEFAULT_MODE);
    	}
     
    	static public void Broadcast(string eventType, T arg1, U arg2, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Action<T, U>>(eventType);
     
    		foreach(var callback in invocationList)
    			callback.Invoke(arg1, arg2);
    	}
     
    	static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, Action<TReturn> returnCall, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Func<T, U, TReturn>>(eventType);
     
    		foreach(var result in invocationList.Select(del => del.Invoke(arg1, arg2)).Cast<TReturn>()) {
    			returnCall.Invoke(result);
    		}
    	}
    }
     
     
    // Three parameters
    static public class Messenger<T, U, V> { 
    	static public void AddListener(string eventType, Action<T, U, V> handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void AddListener<TReturn>(string eventType, Func<T, U, V, TReturn> handler) {
    		MessengerInternal.AddListener(eventType, handler);
    	}
     
    	static public void RemoveListener(string eventType, Action<T, U, V> handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void RemoveListener<TReturn>(string eventType, Func<T, U, V, TReturn> handler) {
    		MessengerInternal.RemoveListener(eventType, handler);
    	}
     
    	static public void Broadcast(string eventType, T arg1, U arg2, V arg3) {
    		Broadcast(eventType, arg1, arg2, arg3, MessengerInternal.DEFAULT_MODE);
    	}
     
    	static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, V arg3, Action<TReturn> returnCall) {
    		Broadcast(eventType, arg1, arg2, arg3, returnCall, MessengerInternal.DEFAULT_MODE);
    	}
     
    	static public void Broadcast(string eventType, T arg1, U arg2, V arg3, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Action<T, U, V>>(eventType);
     
    		foreach(var callback in invocationList)
    			callback.Invoke(arg1, arg2, arg3);
    	}
     
    	static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, V arg3, Action<TReturn> returnCall, MessengerMode mode) {
    		MessengerInternal.OnBroadcasting(eventType, mode);
    		var invocationList = MessengerInternal.GetInvocationList<Func<T, U, V, TReturn>>(eventType);
     
    		foreach(var result in invocationList.Select(del => del.Invoke(arg1, arg2, arg3)).Cast<TReturn>()) {
    			returnCall.Invoke(result);
    		}
    	}
    }MessengerUnitTest.cs // MessengerUnitTest.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
    // 
    // Some functionality testing of the classes in Messenger.cs.
    // A lot of attention is paid to proper exception throwing from the Messenger.
     
    using System;
     
    class MessengerUnitTest {
     
    	private readonly string eventType1 = "__testEvent1";
    	private readonly string eventType2 = "__testEvent2";
     
    	bool wasCalled = false;
     
    	public void RunTest() {
    		RunAddTests();
    		RunBroadcastTests();
    		RunRemoveTests();
    		Console.Out.WriteLine("All Messenger tests passed.");
    	}
     
     
    	private void RunAddTests() {
    		Messenger.AddListener(eventType1, TestCallback);
     
    		try {
    			// This should fail because we're adding a new event listener for same event type but a different delegate signature
    			Messenger<float>.AddListener(eventType1, TestCallbackFloat);
    			throw new Exception("Unit test failure - expected a ListenerException");
    		} catch (MessengerInternal.ListenerException e) {
    			// All good
    		}
     
    		Messenger<float>.AddListener(eventType2, TestCallbackFloat);
    	}
     
     
    	private void RunBroadcastTests() {
    		wasCalled = false;
    		Messenger.Broadcast(eventType1);
    		if (!wasCalled) { throw new Exception("Unit test failure - event handler appears to have not been called."); }
    		wasCalled = false;
    		Messenger<float>.Broadcast(eventType2, 1.0f);
    		if (!wasCalled) { throw new Exception("Unit test failure - event handler appears to have not been called."); }
     
    		// No listener should exist for this event, but we don't require a listener so it should pass
    		Messenger<float>.Broadcast(eventType2 + "_", 1.0f, MessengerMode.DONT_REQUIRE_LISTENER);
     
    		try {
    			// Broadcasting for an event there exists listeners for, but using wrong signature
    			Messenger<float>.Broadcast(eventType1, 1.0f, MessengerMode.DONT_REQUIRE_LISTENER);
    			throw new Exception("Unit test failure - expected a BroadcastException");
    		}
    		catch (MessengerInternal.BroadcastException e) {
    			// All good
    		}
     
    		try {
    			// Same thing, but now we (implicitly) require at least one listener
    			Messenger<float>.Broadcast(eventType2 + "_", 1.0f);
    			throw new Exception("Unit test failure - expected a BroadcastException");
    		} catch (MessengerInternal.BroadcastException e) {
    			// All good
    		}
     
    		try {
    			// Wrong generic type for this broadcast, and we implicitly require a listener
    			Messenger<double>.Broadcast(eventType2, 1.0);
    			throw new Exception("Unit test failure - expected a BroadcastException");
    		} catch (MessengerInternal.BroadcastException e) {
    			// All good
    		}
     
    	}
     
     
    	private void RunRemoveTests() {
     
    		try {
    			// Removal with wrong signature should fail
    			Messenger<float>.RemoveListener(eventType1, TestCallbackFloat);
    			throw new Exception("Unit test failure - expected a ListenerException");
    		}
    		catch (MessengerInternal.ListenerException e) {
    			// All good
    		}
     
    		Messenger.RemoveListener(eventType1, TestCallback);
     
    		try {
    			// Repeated removal should fail
    			Messenger.RemoveListener(eventType1, TestCallback);
    			throw new Exception("Unit test failure - expected a ListenerException");
    		}
    		catch (MessengerInternal.ListenerException e) {
    			// All good
    		}
     
     
     
    		Messenger<float>.RemoveListener(eventType2, TestCallbackFloat);
     
    		try {
    			// Repeated removal should fail
    			Messenger<float>.RemoveListener(eventType2, TestCallbackFloat);
    			throw new Exception("Unit test failure - expected a ListenerException");
    		}
    		catch (MessengerInternal.ListenerException e) {
    			// All good
    		}
    	}
     
     
    	void TestCallback() {
    		wasCalled = true;
    		Console.Out.WriteLine("TestCallback() was called.");
    	}
     
    	void TestCallbackFloat(float f) {
    		wasCalled = true;
    		Console.Out.WriteLine("TestCallbackFloat(float) was called.");
     
    		if (f != 1.0f) {
    			throw new Exception("Unit test failure - wrong value on float argument");
    		}
    	}
     
     
     
}
}
