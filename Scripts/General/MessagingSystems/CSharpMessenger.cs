// Original url: http://wiki.unity3d.com/index.php/CSharpMessenger
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MessagingSystems/CSharpMessenger.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MessagingSystems
{
Author: Rod Hyde (badlydrawnrod) 
Contents [hide] 
1 Description 
2 Usage 
2.1 Writing an event listener 
2.2 Registering an event listener 
2.3 Unregistering an event listener 
2.3.1 Warning 
2.4 Invoking an event 
3 Code 
3.1 Callback.cs 
3.2 Messenger.cs 

Description This is a delegate-based event system for C#. It uses generics to achieve static typing. As with other messaging systems, such as CSharpEventManager it allows for messaging between event producers and event consumers, without the need for producers or consumers to be aware of each other. The messengers are implemented as static classes, so there is never any need to instantiate a Messenger object. The use of generics means that parameters can be of any type and are not restricted to subclasses of a special Message class - if a callback only needs a float then supply it with a float. 
Usage Writing an event listener To create an event listener, just write a method with the appropriate parameters. Here's an example of a listener that takes a float parameter. Note that although this example uses a method called OnSpeedChanged, this is merely convention and is not enforced. It could equally be called HandleSpeedChanged or any other valid identifier. 
    void OnSpeedChanged(float speed)
    {
        this.speed = speed;
    }Registering an event listener Of course, the listener will never be invoked unless it is registered with the messenger. This is done with the AddListener method. If the listener is part of a MonoBehaviour then it is good practice to register it when the behaviour is enabled. 
    void OnEnable()
    {
        Messenger<float>.AddListener("speed changed", OnSpeedChanged);
    }Unregistering an event listener If the listener should not be invoked when an event is fired then it must be unregistered with the RemoveListener method. As before, if the listener is part of a MonoBehaviour then it is good practice to unregister it when the behaviour is disabled. 
    void OnDisable()
    {
        Messenger<float>.RemoveListener("speed changed", OnSpeedChanged);
    }Warning RemoveListener should always be called on messages when loading a new level. Otherwise many MissingReferenceExceptions will be thrown, when invoking messages on destroyed objects. For example: 1. We registered a "speed changed" message in a Level1 scene. Afterwards the scene has been destroyed, but the "speed changed" message is still pointing to the OnSpeedChanged message handler in the destroyed class. 2. We loaded Level2 and registered another "speed changed" message, but the previous reference to the destroyed object hasn't been removed. We'll get a MissingReferenceException, because by invoking the "speed changed" message, the messaging system will first invoke the OnSpeedChanged handler of the destroyed object. 


Invoking an event To fire all the listeners for an event type, call the messenger's Invoke method, passing in the event type followed by the arguments. 
    if (speed != lastSpeed)
    {
        Messenger<float>.Invoke("speed changed", speed);
    }Code There are two files, Callback.cs and Messenger.cs. The code below contains callbacks and messengers for delegates that take no arguments, delegates that take one argument and delegates that take two arguments. It is straightforward to extend the code to take more arguments if needed. 
Callback.cs // Callback.cs v0.1 (20090925) by Rod Hyde (badlydrawnrod).
//
// These are callbacks (delegates) that can be used by the messengers defined
// in Messenger.cs.
 
public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);Messenger.cs // Messenger.cs v0.1 (20090925) by Rod Hyde (badlydrawnrod).
//
// This is a C# messenger (notification center) for Unity. It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other.
 
using System;
using System.Collections.Generic;
 
 
/**
 * A messenger for events that have no parameters.
 */
static public class Messenger
{
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
 
    static public void AddListener(string eventType, Callback handler)
    {
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Create an entry for this event type if it doesn't already exist.
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            // Add the handler to the event.
            eventTable[eventType] = (Callback)eventTable[eventType] + handler;
        }
    }
 
    static public void RemoveListener(string eventType, Callback handler)
    {
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Only take action if this event type exists.
            if (eventTable.ContainsKey(eventType))
            {
                // Remove the event handler from this event.
                eventTable[eventType] = (Callback)eventTable[eventType] - handler;
 
                // If there's nothing left then remove the event type from the event table.
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }
 
    static public void Invoke(string eventType)
    {
        Delegate d;
        // Invoke the delegate only if the event type is in the dictionary.
        if (eventTable.TryGetValue(eventType, out d))
        {
            // Take a local copy to prevent a race condition if another thread
            // were to unsubscribe from this event.
            Callback callback = (Callback) d;
 
            // Invoke the delegate if it's not null.
            if (callback != null)
            {
                callback();
            }
        }
    }
}
 
 
/**
 * A messenger for events that have one parameter of type T.
 */
static public class Messenger<T>
{
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
 
    static public void AddListener(string eventType, Callback<T> handler)
    {
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Create an entry for this event type if it doesn't already exist.
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            // Add the handler to the event.
            eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
        }
    }
 
    static public void RemoveListener(string eventType, Callback<T> handler)
    {
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Only take action if this event type exists.
            if (eventTable.ContainsKey(eventType))
            {
                // Remove the event handler from this event.
                eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
 
                // If there's nothing left then remove the event type from the event table.
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }
 
    static public void Invoke(string eventType, T arg1)
    {
        Delegate d;
        // Invoke the delegate only if the event type is in the dictionary.
        if (eventTable.TryGetValue(eventType, out d))
        {
            // Take a local copy to prevent a race condition if another thread
            // were to unsubscribe from this event.
            Callback<T> callback = (Callback<T>)d;
 
            // Invoke the delegate if it's not null.
            if (callback != null)
            {
                callback(arg1);
            }
        }
    }
}
 
 
/**
 * A messenger for events that have two parameters of types T and U.
 */
static public class Messenger<T, U>
{
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
 
    static public void AddListener(string eventType, Callback<T, U> handler)
    {
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Create an entry for this event type if it doesn't already exist.
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            // Add the handler to the event.
            eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
        }
    }
 
    static public void RemoveListener(string eventType, Callback<T, U> handler)
    {
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Only take action if this event type exists.
            if (eventTable.ContainsKey(eventType))
            {
                // Remove the event handler from this event.
                eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;
 
                // If there's nothing left then remove the event type from the event table.
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }
 
    static public void Invoke(string eventType, T arg1, U arg2)
    {
        Delegate d;
        // Invoke the delegate only if the event type is in the dictionary.
        if (eventTable.TryGetValue(eventType, out d))
        {
            // Take a local copy to prevent a race condition if another thread
            // were to unsubscribe from this event.
            Callback<T, U> callback = (Callback<T, U>)d;
 
            // Invoke the delegate if it's not null.
            if (callback != null)
            {
                callback(arg1, arg2);
            }
        }
    }
}
}
