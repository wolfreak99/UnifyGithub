// Original url: http://wiki.unity3d.com/index.php/CSharpEventManager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MessagingSystems/CSharpEventManager.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MessagingSystems
{
Author: Billy "brian" Fletcher 
Description This is an iPhone friendly implementation of a non-delegate driven event system for C#, similar to the javascript messaging systems in that you can subscribe to an event purely by a string identifier, regardless of whether it exists or not at that time. It comes in two versions, one iPhone friendly version using ArrayLists and one optimised version by AngryAnt using C# generics. There is an option to limit the amount of time spent processing events per frame and priority designation may be added in future although the processing spent sorting can easily outweigh the gain of having some events processed faster. 
Usage The event manager uses a Singleton design pattern that ensures it is self instantiating and globally accessible through the static EventManager.instance reference. 
The system consists of two base interfaces for interacting and creating events, the IEvent interface that declares the base functionality of all events and the IEventListener interface that allows any class to receive events. 
To create an event, simply create a new CS file and inherit from the IEvent interface and implement the two functions declared within it: GetName() and GetData(). GetData() returns a C# object which can then be casted to any data type such as ArrayLists or predefined structures. 
Here is an example event, note that the GetName() code can be reused for nearly every event. 
public class TestEvent : IEvent
{
    public TestEvent()
    {
    }
 
    string IEvent.GetName()
    {
        return this.GetType().ToString();
    }
 
    object IEvent.GetData()
    {
        return "TestEvent Data goes here!";
    }
}To create an event listener, follow a similar process and simply inherit from the IEventListener interface and implement the HandleEvent() function. Note that an object can inherit from any number of interfaces, but only one defined class, this allows any MonoBehaviour or other object to receive events very easily. 
To subscribe to events, simple call the EventManager.instance.AddListener() function and provide a reference to the event listener (typically using the 'this' keyword) and the name of the event it wishes to subscribe to. Note that if you use the above GetName() code it is simply the class name of the event, although the event does not have to exist in order to subscribe to it. The main benefit of this is that when working in multiple person teams, provided the events are designed beforehand, components can be developed completely independently. 
The HandleEvent function takes in any event the object is subscribed to and a boolean value must be returned. This value indicates whether or not the event has been consumed by the object and therefore should not be passed to other listeners of the same event. A listener can subscribe to multiple events and differentiate them by using the GetName() method. 
Here is an example of an event listener that reacts to the TestEvent above: 
using UnityEngine;
using System.Collections;
 
public class TestListener : MonoBehaviour, IEventListener
{
    void Start()
    {
        EventManager.instance.AddListener(this as IEventListener, "TestEvent");
    }
 
    bool IEventListener.HandleEvent(IEvent evt)
    {
        string txt = evt.GetData() as string;
        Debug.Log("Event received: " + evt.GetName() + " with data: " + txt);
        return true;
    }
}To queue or trigger an event, merely call the EventManager.instance.QueueEvent() function with a new instance of the event you wish to pass, if the event must be sent immediately call the TriggerEvent() function. Note that the queue event function is typically fast enough for the vast vast majority of uses. 
C# - EventManager.cs - iPhone friendly version //C# Unity event manager that uses strings in a hashtable over delegates and events in order to
//allow use of events without knowing where and when they're declared/defined.
//by Billy Fletcher of Rubix Studios
using UnityEngine;
using System.Collections;
 
public interface IEventListener
{
    bool HandleEvent(IEvent evt);
}
 
public interface IEvent
{
    string GetName();
    object GetData();
}
 
public class EventManager : MonoBehaviour
{
    public bool LimitQueueProcesing = false;
    public float QueueProcessTime = 0.0f;
 
    private static EventManager s_Instance = null;
    public static EventManager instance 
    {
        get 
        {
            if (s_Instance == null) 
            {
                GameObject go = new GameObject("EventManager");
                s_Instance = (EventManager)go.AddComponent(typeof(EventManager));
            }
 
            return s_Instance;
        }
    }
 
    private Hashtable m_listenerTable = new Hashtable();
    private Queue m_eventQueue = new Queue();
 
 
    //Add a listener to the event manager that will receive any events of the supplied event name.
    public bool AddListener(IEventListener listener, string eventName)
    {
        if (listener == null || eventName == null)
        {
            Debug.Log("Event Manager: AddListener failed due to no listener or event name specified.");
            return false;
        }
 
        if (!m_listenerTable.ContainsKey(eventName))
            m_listenerTable.Add(eventName, new ArrayList());
 
        ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
        if (listenerList.Contains(listener))
        {
            Debug.Log("Event Manager: Listener: " + listener.GetType().ToString() + " is already in list for event: " + eventName);
            return false; //listener already in list
        }
 
        listenerList.Add(listener);
        return true;
    }
 
    //Remove a listener from the subscribed to event.
    public bool DetachListener(IEventListener listener, string eventName)
    {
        if (!m_listenerTable.ContainsKey(eventName))
            return false;
 
        ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
        if (!listenerList.Contains(listener))
            return false;
 
        listenerList.Remove(listener);
        return true;
    }
 
    //Trigger the event instantly, this should only be used in specific circumstances,
    //the QueueEvent function is usually fast enough for the vast majority of uses.
    public bool TriggerEvent(IEvent evt)
    {
        string eventName = evt.GetName();
        if (!m_listenerTable.ContainsKey(eventName))
        {
            Debug.Log("Event Manager: Event \"" + eventName + "\" triggered has no listeners!");
            return false; //No listeners for event so ignore it
        }
 
        ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
        foreach (IEventListener listener in listenerList)
        {
            if (listener.HandleEvent(evt))
                return true; //Event consumed.
        }
 
        return true;
    }
 
    //Inserts the event into the current queue.
    public bool QueueEvent(IEvent evt)
    {
        if (!m_listenerTable.ContainsKey(evt.GetName()))
        {
            Debug.Log("EventManager: QueueEvent failed due to no listeners for event: " + evt.GetName());
            return false;
        }
 
        m_eventQueue.Enqueue(evt);
        return true;
    }
 
    //Every update cycle the queue is processed, if the queue processing is limited,
    //a maximum processing time per update can be set after which the events will have
    //to be processed next update loop.
    void Update()
    {
        float timer = 0.0f;
        while (m_eventQueue.Count > 0)
        {
            if (LimitQueueProcesing)
            {
                if (timer > QueueProcessTime)
                    return;
            }
 
            IEvent evt = m_eventQueue.Dequeue() as IEvent;
            if (!TriggerEvent(evt))
                Debug.Log("Error when processing event: " + evt.GetName());
 
            if (LimitQueueProcesing)
                timer += Time.deltaTime;
        }
    }
 
    public void OnApplicationQuit()
    {
        m_listenerTable.Clear();
        m_eventQueue.Clear();
        s_Instance = null;
    }
}
}
