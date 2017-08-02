/*************************
 * Original url: http://wiki.unity3d.com/index.php/NotificationCenter
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MessagingSystems/NotificationCenter.cs
 * File based on original modification date of: 8 June 2012, at 17:48. 
 *
 * Author: capnbishop 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.MessagingSystems
{
    Description NotificationCenter.js allows for easy and powerful messaging between objects in Unity. An object can register as an observer and receive notifications of a certain type when they occur. When that notification type is posted elsewhere, all objects that registered as an observer for that notification type will receive a message that it has occurred, along with the associated data. 
    This system allows for objects to communicate their status and events with one another, without having to directly reference each object that receives the notification. An object can simply post an event when it occurs, and any interested party can register to receive notification when it happens. 
    Using a notification center will allow for dynamic and flexible coding practices. For example, a pinball game can have a bumper that posts a notification when the ball collides with it. An object that keeps track of score can register to receive a notification when this event occurs, and increment the score accordingly without having to communicate directly with or keep track of the bumper. Another object can play a sound when it receives notification of a bumper impact. Another object can display a particle effect when it receives notification of a bumper impact. Other features can be added to respond to this event later on without having to modify the existing objects, because they independently choose to receive and act on the notification event. 
    This script was inspired by Blurst Technology's Messenger system, with a convention twist to make it easier to use and a little more similar to the Cocoa framework's NSNotificationCenter. 
    This script breaks with #pragma strict or deployment to Android or iOS. For a version that works, look at NotificationCenterGenerics. An updated version for Unity 3.5 is available here. 
    Usage This is an auto-instantiating singleton class. There is no need to attach it to an object. Simply place it in a project's assets and use it by calling to the DefaultCenter. The static method will automatically create a game object, add a NotificationCenter component, and create a reference to that component. 
    Post a notification by calling the PostNotification method: 
    NotificationCenter.DefaultCenter().PostNotification(this, "OnBumperCollision");
    NotificationCenter.DefaultCenter().PostNotification(this, "OnBumperCollision", anyObject);
    NotificationCenter.DefaultCenter().PostNotification(new Notification(this, "OnBumperCollision"));
    NotificationCenter.DefaultCenter().PostNotification(new Notification(this, "OnBumperCollision", anyObject));The PostNotification() method comes in several varieties, all of which are valid. It only requires two parameters: the sender, and the notification name. Optionally it can accept a miscellaneous data object. This can be anything that needs to be communicated. For instance, the OnBumperCollision notification could send information about the collision, or even send the Collision object itself. Often the data object is best used as a Hashtable, so that it can include a variety of keyed information. 
    Notice that PostNotification() can be called by either passing individual parameters, or by passing a Notification object. The Notification object can be subclassed to make a streamlined set of notification data. The Notification object also must hold the sender, the notification name, and (optionally) a data object. 
    Register to receive notifications by calling the AddObserver method: 
    NotificationCenter.DefaultCenter().AddObserver(this, "OnBumperCollision");DefaultCenter() is a static method that returns an instance of the default notification center. If it doesn't exist yet, it will be automatically instantiated. AddObserver() accepts two parameters for the object that wishes to receive notifications, and the notification to be received. The notification that's being registered is the function call that will be executed for the event. 
    Remove an observer with the RemoveObserver() method: 
    NotificationCenter.DefaultCenter().RemoveObserver(this, "OnBumperCollision");RemoveObserver() simply removes the object from the list of observers for the specified notification event. 
    To receive notification, simply implement a method with the notification name: 
    function OnBumperCollision (notification: Notification) {
        Debug.Log("Received notification from: " + notification.sender);
        if (notification.data == null)
            Debug.Log("And the data object was null!");
        else
            Debug.Log("And it included a data object: " + notification.data);
    }Notification methods must accept the Notification object. It contains the three properties for sender, name (of the notification), and data (optional object). 
    Javascript - NotificationCenter.js //    NotificationCenter is used for handling messages between GameObjects.
     
    //    GameObjects can register to receive specific notifications.  When another objects sends a notification of that type, all GameObjects that registered for it and implement the appropriate message will receive that notification.
     
    //    Observing GameObjetcs must register to receive notifications with the AddObserver function, and pass their selves, and the name of the notification.  Observing GameObjects can also unregister themselves with the RemoveObserver function.  GameObjects must request to receive and remove notification types on a type by type basis.
     
    //    Posting notifications is done by creating a Notification object and passing it to PostNotification.  All receiving GameObjects will accept that Notification object.  The Notification object contains the sender, the notification type name, and an option hashtable containing data.
     
    //    To use NotificationCenter, either create and manage a unique instance of it somewhere, or use the static NotificationCenter.
     
     
    // We need a static method for objects to be able to obtain the default notification center.
    // This default center is what all objects will use for most notifications.  We can of course create our own separate instances of NotificationCenter, but this is the static one used by all.
    private static var defaultCenter : NotificationCenter;
    static function DefaultCenter () {
        // If the defaultCenter doesn't already exist, we need to create it
        if (!defaultCenter) {
            // Because the NotificationCenter is a component, we have to create a GameObject to attach it to.
            var notificationObject: GameObject = new GameObject("Default Notification Center");
            // Add the NotificationCenter component, and set it as the defaultCenter
            defaultCenter = notificationObject.AddComponent(NotificationCenter);
        }
     
        return defaultCenter;
    }
     
    // Our hashtable containing all the notifications.  Each notification in the hash table is an ArrayList that contains all the observers for that notification.
    var notifications: Hashtable = new Hashtable();
     
    // AddObserver includes a version where the observer can request to only receive notifications from a specific object.  We haven't implemented that yet, so the sender value is ignored for now.
    function AddObserver (observer, name: String) { AddObserver(observer, name, null); }
    function AddObserver (observer, name: String, sender) {
        // If the name isn't good, then throw an error and return.
        if (name == null || name == "") { Debug.Log("Null name specified for notification in AddObserver."); return; }
        // If this specific notification doens't exist yet, then create it.
        if (!notifications[name]) {
            notifications[name] = new ArrayList();
        }
     
        var notifyList: ArrayList = notifications[name];
     
        // If the list of observers doesn't already contains the one that's registering, then add it.
        if (!notifyList.Contains(observer)) { notifyList.Add(observer); }
    }
     
    // RemoveObserver removes the observer from the notification list for the specified notification type
    function RemoveObserver (observer, name: String) {
        var notifyList: ArrayList = notifications[name];
     
        // Assuming that this is a valid notification type, remove the observer from the list.
        // If the list of observers is now empty, then remove that notification type from the notifications hash.  This is for housekeeping purposes.
        if (notifyList) {
            if (notifyList.Contains(observer)) { notifyList.Remove(observer); }
            if (notifyList.Count == 0) { notifications.Remove(name); }
        }
    }
     
    // PostNotification sends a notification object to all objects that have requested to receive this type of notification.
    // A notification can either be posted with a notification object or by just sending the individual components.
    function PostNotification (aSender, aName: String) { PostNotification(aSender, aName, null); }
    function PostNotification (aSender, aName: String, aData) { PostNotification(new Notification(aSender, aName, aData)); }
    function PostNotification (aNotification: Notification) {
        // First make sure that the name of the notification is valid.
        if (aNotification.name == null || aNotification.name == "") { Debug.Log("Null name sent to PostNotification."); return; }
        // Obtain the notification list, and make sure that it is valid as well
        var notifyList: ArrayList = notifications[aNotification.name];
        if (!notifyList) { Debug.Log("Notify list not found in PostNotification."); return; }
     
        // Create an array to keep track of invalid observers that we need to remove
        var observersToRemove = new Array();
     
        // Itterate through all the objects that have signed up to be notified by this type of notification.
        for (var observer in notifyList) {
            // If the observer isn't valid, then keep track of it so we can remove it later.
            // We can't remove it right now, or it will mess the for loop up.
            if (!observer) { observersToRemove.Add(observer);
            } else {
                // If the observer is valid, then send it the notification.  The message that's sent is the name of the notification.
                observer.SendMessage(aNotification.name, aNotification, SendMessageOptions.DontRequireReceiver);
            }
        }
     
        // Remove all the invalid observers
        for (observer in observersToRemove) {
            notifyList.Remove(observer);
        }
    }
     
    // The Notification class is the object that is send to receiving objects of a notification type.
    // This class contains the sending GameObject, the name of the notification, and optionally a hashtable containing data.
    class Notification {
        var sender;
        var name : String;
        var data;
        function Notification (aSender,  aName: String) { sender = aSender; name = aName; data = null; }
        function Notification (aSender, aName: String, aData) { sender = aSender; name = aName; data = aData; }
}
}
