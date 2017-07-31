// Original url: http://wiki.unity3d.com/index.php/MessageRouter
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MessagingSystems/MessageRouter.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MessagingSystems
{
Author: Fernando Zapata (fernando@cpudreams.com) 
Contents [hide] 
1 Description 
2 Usage 
3 Code 
3.1 MessageRouter.js 
3.2 CompactList.js 

Description This subscription based messaging/event system provides an efficient method for broadcasting game events to interested parties while maintaining loose coupling. It has several features which may make it a good fit for your game: 
Delayed Notification: messages can arrive immediately or delayed by n seconds or n frames 
Delivery Stages: messages can be sent immediately or delayed until the next Update, FixedUpdate, LateUpdate, or EndOfFrame 
Message Filtering: subscriptions can be as broad or as narrow as you need them to be, you can subscribe to all messages sent to you, to anyone, sent from someone, sent from anyone, of a particular type/class, of any type/class, with any tags, or with particular tags 
Tags: messages can have zero or more tags that categorize it and can be filtered by those tags 
Receiver Assertions: messages can be marked as requiring a receiver and subscribers can be marked as hidden receivers the message router will generate an error if a message requiring a receiver is not received by at least one non-hidden receiver 
Usage The MessageRouter is flexible and can handle many different scenarios. As an example, imagine an air combat game in which pilots communicate through our MessageRouter. A pilot might broadcast (shout) an "Under attack!" message if they feel threatened. 
First you will need to add the MessageRouter.js component to at least one GameObject. The MessageRouter is usually a singleton but you are free to have instances specific to particular sub-systems. 
Next lets create a "Under attack!" message to send from pilots under attack. 
class UnderAttackMessage extends Message {
  var fromCallSign : String;
 
  function UnderAttackMessage(header : MessageHeader) {
    super(header);
  }
}To create a new message type create a sub-class of the MessageRouter's Message class. A message can contain any additional data that is specific to this new message type. It can also contain any additional convenience constructors and methods that you deem necessary. In this example I've kept things simple and have only added a single attribute with the call-sign of the pilot that has sent this distress message. 
After creating the message type we need to broadcast it to any interested parties. Hopefully there is a friendly nearby that is willing and able to react. 
var messageRouter : MessageRouter;
var messageHeader : MessageHeader;
 
function SendUnderAttackMessage() {
  var message = new UnderAttackMessage(messageHeader)
  message.fromCallSign = "Red Baron";
  messageRouter.SendMessage(message);
}To send a message create an instance of the previously defined UnderAttackMessage and assign it a re-usable message header. A single message header can be shared by multiple message instances and types. A message header is like the front of an envelope and is used to route the message to subscribers. By convention I've exposed the message header as an attribute of this script so I can configure it using the Unity Inspector. There I can specify that a receiver is not required (header.requireReceiver = false) and that the message should be delivered 0.2 seconds after it was sent (header.deliveryTime = 0.2). The message is delayed as a way to simulate the pilots reaction time. 
Once a message has been created and initialized sending it is easy. Just call SendMessage() on the message router with the newly created message. After a message has been sent it will be discarded if there are no interested subscribers. In order to receive messages you must create a subscription. 
var messageRouter : MessageRouter;
var subscription : MessageSubscription;
 
function Awake() {
  subscription.handler = OnUnderAttackMessage;
  subscription.type = UnderAttackMessage;
}
 
function OnEnable() {
  messageRouter.Subscribe(subscription);
}
 
function OnDisable() {
  messageRouter.Unsubscribe(subscription);
}
 
function OnUnderAttackMessage(message : UnderAttackMessage) {
  if (message.fromCallSign == "Red Baron") {
    print("Curse you, Red Baron! You won't get away this time!");
}To subscribe to a message you need a subscription object. By convention I've exposed the subscription as an attribute of this script so I can configure it using the Unity Inspector. I also set the subscription's handler to a method that takes an UnderAttackMessage as its only parameter. I set the subscription type to UnderAttackMessage so that the handler is only called for messages of this type. 
Once a subscription has been created and initialized subscribing is easy. Just call Subscribe() on the message router with the newly created subscription (use Unsubscribe() to remove a subscription at any time). After a subscription has been made any messages matching the subscription will be sent to the OnUnderAttackMessage method. In this case, the "Under Attack!" message is heard by an opponent of the Red Baron and unless a friendly subscriber also received this message the Red Baron may have finally met his match. 
This example shows an idiomatic way of using the MessageRouter. By exposing the message header and subscription objects as attributes you can easily configure the message routing in many different ways. For example, you could add an AI tag to "Under Attack!" message and similar messages and have a subscriber subscribe to all AI tagged messages to produce an AI log for debugging. For details on features not used in this particular example please read the doc comments of the relevant class (ie. MessageHeader, Message, MessageSubscription, and MessageRouter). If you have any questions feel free to contact me directly at fernando@cpudreams.com. 
Code There are two files MessageRouter.js and CompactList.js. MessageRouter is the only MonoBeaviour component and is implemented at the end of MessageRouter.js. MessageHeader, Message, and MessageSubscription are plain helper classes and are also implemented in MessageRouter.js. The enum MessageTag is also included in MessageRouter.js and should be modified to add additional user defined tags (you may prefer to keep the enum in your own separate Javascript file). CompactList.js implements a custom data structure that is well suited for storing delayed messages and is used internally by MessageRouter. MessageRouter.js also includes DelayedMessageList which likewise is for internal use by MessageRouter. 
MessageRouter.js #pragma strict
 
/**
 * A subscription based messaging/event system that provides an efficient method
 * for broadcasting game events to interested parties while maintaining loose
 * coupling.
 *
 * @author Fernando Zapata (fernando@cpudreams.com)
 */
 
/**
 * A message can have zero or more tags describing the kind of message it is.
 * These are sample tags and you are free to use or remove them.
 */
enum MessageTag {
  Network, // A message sent over the network
  PlayerCommand, // A game command generated by a human or AI player
  AI // An AI related message such as a transition in a state machine
}
 
/**
 * A message can be delivered during the following stages of the Unity
 * game loop.
 */
enum DeliveryStage {
  Immediate, // Send message right away regardless of current stage
  FixedUpdate, // Send message during earliest possible FixedUpdate
  Update, // Send message during earliest possible Update
  LateUpdate, // Send message during earliest possible LateUpdate
  EndOfFrame // Send message during earliest possible EndOfFrame
}
 
/**
 * Base class for all messages that can be sent through MessageRouter.
 * Extend this class to create your own message types and include additional
 * message data. This is an abstract class, only send subtypes of this class as
 * messages.
 */
class Message {
  /**
   * The header for this message. A message must have a header for it to be
   * sent. You can share the same header instance among multiple Message
   * instances.
   */
  var header : MessageHeader;
  /**
   * Set by MessageRouter.SendMessage to Time.time
   */
  var sentTime = -1.0;
  /**
   * Set by MessageRouter.SendMessage to Time.frameCount
   */
  var sentFrame = -1;
  /**
   * Set by MessageRouter.SendMessage to header.deliveryTime + Time.time or
   * Time.time if deliveryTime is -1.
   */
  var desiredArrivalTime : float;
  /**
   * Set by MessageRouter.SendMessage to header.deliveryFrame + Time.frameCount
   * or Time.frameCount if deliveryFrame is -1.
   */
  var desiredArrivalFrame : int;
 
  /**
   * A message is not complete without a header. In non-abstract base classes
   * you can create convenience constructors to initialize any member data
   * specific to a particular message type.
   */
  function Message(newHeader : MessageHeader) {
    header = newHeader;
  }
 
  virtual function ToString() {
    var s = GetType().ToString();
    if (header) {
      if (header.from) {
        s += " From: " + header.from.name;
      }
      if (header.to) {
        s += " To: " + header.to.name;
      }
    }
    return s;
  }
}
 
/**
 * MessageHeader contains message meta data used for routing messages to
 * message subscribers. A message must have a header for it to be
 * sent. You can and should share the same header instance among multiple
 * Message instances. But you should not modify a message header instance
 * after any message referencing that instance has been sent.
 */
class MessageHeader {
  /** Who is sending the message, this is mandatory. */
  var from : MonoBehaviour;
  /** Optional intended recipient of the message. */
  var to : MonoBehaviour;
  /**
   * If requireReceiver is set to true an error is printed when the message
   * is not consumed by at least one non-hidden subscriber.
   */
  var requireReceiver = true;
  /**
   * Tags help describe the kind of message this is (similar to categories).
   * A message can have zero or more tags. Tags can be used to filter
   * subscriptions, for example you could subscribe to all messages with the
   * Network tag.
   */
  var tags : MessageTag[];
  /**
   * Stage in the game loop at which to deliver the message. If set to Immediate
   * the message is sent right away, other wise it is sent when the message
   * router next enters the specified DeliveryStage and the specified time or
   * frame delay has elapsed.
   */
  var deliveryStage = DeliveryStage.Immediate;
  /**
   * Relative time in the future at which to deliver the message.
   * Example: header.deliveryTime = 3.0; // deliver 3 seconds from now
   * Actual delivery time will be as close to three seconds as possible, and
   * depends on game loop state.
   * If <= 0 sent as soon as posible (depends on deliveryStage). If set to -1.0
   * or anything else < 0 consider undefined and ASAP.
   */
  var deliveryTime = -1.0;
  /**
   * Deliver this many frames from now. Example: header.deliveryFrame = 10;
   * // deliver 10 frames from now
   * If set to zero delivery will be as soon as possible but might not happen
   * until next frame if deliveryStage is not set to Immediate.
   * deliveryFrame is only used if deliveryTime is -1.0 and deliveryStage is
   * not set to Immediate.
   */
  var deliveryFrame = -1;
}
 
/**
 * Components must subscribe if they wish to receive any messages.
 * You can subscribe to messages sent to you, sent to anyone, sent from someone,
 * sent from anyone, of a particular type/class, of any type/class, with any
 * tags, or with at least one of the given tags.
 *
 * The messages sent to the handler for a particular subscription are those
 * that match all of the filter conditions in the subscription. If any of the
 * filter conditions do not match for a message it will not be sent to the
 * handler method.
 *
 * Subscriptions can be as narrow or as broad as you want, you can even
 * subscribe to all messages sent through the MessageRouter.
 */
class MessageSubscription {
  /**
   * Handler method invoked in order to deliver a message to the subscriber.
   * The handler method should take one parameter handler(messageSubclass).
   */
  @HideInInspector
  var handler : Function;
 
  //
  //  filtering options
  //
 
  /**
   * If hiddenReceiver is set to true it is not counted as a receiver when
   * a message with requireReceiver set to true is sent. In other words
   * a requireReceiver message must be consumed by at least one subscriber
   * that is not a hiddenReceiver or an error message will be generated.
   */
  var hiddenReceiver = false;
  /**
   * Set to the object you are interested in receiving messages from. Set
   * to null and you will receive the message regardless of whom it is from.
   */
  var from : MonoBehaviour;
  /**
   * Messages sent to this object will also be sent to you. Set to null
   * if you want to revieve messages sent to anyone.
   */
  var to : MonoBehaviour;
  /**
   * The type (class) of messages you want to receive.
   * Example: subscription.type = PlayerHitMessage; // use class name
   * Example 2: subscription.type = message.GetType();
   * Set to Message if you want to receive messages of any type (class).
   */
  var type : System.Type = Message;
  /**
   * A message must have at least one of the tags listed in order for it to
   * be delivered to this subscriber. If you want to enfoce a match all tags
   * behvaviour instead of a match at least one tag behaviour you can check
   * the header.tags in your handler method and filter there.
   * Set to null if you want to receive messages regardless of their tags.
   */
  var tags : MessageTag[];
}
 
/**
 * List of messages with delayed deliveries. This class keeps track of the next
 * delivery time and frame to help avoid unnecessary list iterations. This
 * helper class for MessageRouter is purely a private implementation detail.
 */
class DelayedMessageList {
  /**
   * Messages checked for delivery during the next delivery stage.
   */
  private var list : CompactList;
  /**
   * The absolute delivery time of the next message to be delivered.
   * Set to Mathf.Infinity when no messages are waiting for their delivery time.
   * Used to avoid iterating fixedUpdateMessages when no deliveries are due.
   */
  private var nextMessageTime : float;
  /**
   * The absolute delivery frame of the next message to be delivered.
   * Set to -1 when no messages are waiting for their delivery time.
   * Used to avoid iterating fixedUpdateMessages when no deliveries are due.
   */
  private var nextMessageFrame : int;
  /**
   * The route message method to call when sending delayed messages.
   */
  private var routeMessage : Function;
 
  /**
   * Creates a delayed message list with the given message starting capacity
   * that will grow by current capacity * growth rate as needed.
   */
  function DelayedMessageList(startingCapacity : int, growthRate : float,
                              routeMessage : Function) {
    list = CompactList(startingCapacity, growthRate);
    nextMessageTime = Mathf.Infinity;
    nextMessageFrame = -1;
    this.routeMessage = routeMessage;
  }
 
  /**
   * Store's the dealyed message in the this DelayedMessageList and updates
   * nextMessageTime and nextMessageFrame if necessary.
   */
  function Add(message : Message) {
    UpdateNextMessageInfo(message);
    list.Add(message);
  }
 
  /**
   * Route delayed messages who's delivery time has arrived or past.
   */
  function SendDelayedMessages() {
    if (nextMessageTime <= Time.time ||
        (nextMessageFrame != -1 && nextMessageFrame <= Time.frameCount)) {
      nextMessageTime = Mathf.Infinity;
      nextMessageFrame = -1;
      list.Iterate(ProcessDelayedMessage);
    }
  }
 
  /**
   * Update nextMessageTime and/or nextMessageFrame if this message's
   * deliveryTime or deliveryFrame is earlier.
   */
  private function UpdateNextMessageInfo(message : Message) {
    if (message.header.deliveryTime != -1.0) {
      nextMessageTime = (message.desiredArrivalTime < nextMessageTime) ?
        message.desiredArrivalTime : nextMessageTime;
    } else {
      // deliveryFrame is only used if deliveryTime is -1
      nextMessageFrame = (nextMessageFrame == -1 ||
                          message.desiredArrivalFrame < nextMessageFrame) ?
        message.desiredArrivalFrame : nextMessageFrame;
    }
    /*
    Debug.Log("after: " + message + " arrivalTime: " +
              message.desiredArrivalTime +
              " arrivalFrame: " + message.desiredArrivalFrame + " nextTime: " +
              nextMessageTime + " nextFrame: " + nextMessageFrame);
    */
  }
 
  /**
   * Routes the delayed message if it is deliverable otherwise updates
   * next message info if necessary.
   */
  private function ProcessDelayedMessage(message : Message) : boolean {
    var removeMessage = false;
    if (MessageIsDeliverable(message)) {
      routeMessage(message);
      removeMessage = true;
    } else {
      UpdateNextMessageInfo(message);
    }
    return removeMessage;
  }
 
  /**
   * Returns true if a delayed message is deliverable because we are at or
   * past its scheduled delivery time or frame.
   */
  private function MessageIsDeliverable(message : Message) : boolean {
    var isDeliverable = false;
    if (message.header.deliveryTime != -1.0) {
      isDeliverable = (message.desiredArrivalTime <= Time.time);
    } else {
      // deliveryFrame is only used if deliveryTime is -1
      isDeliverable = (message.desiredArrivalFrame <= Time.frameCount);
    }
    return isDeliverable;
  }
}
 
/**
 * Starting capacity for delayed message lists. There is one delayed message
 * list for each DeliveryStage.
 */
var startingCapacity = 16;
/**
 * Growth rate for delayed message lists. Used to grow lists as they reach
 * capacity. Must be greater than 1.0.
 */
var growthRate = 2.0;
 
/**
 * Messages are sent to all matching subscribers. A subscription must be made
 * if an object wants to receive messages even if a message is sent "to" them.
 * Subscriptions are kept in a hashtable of subscription arrays, the arrays
 * are keyed by the message type.
 */
private var subscriptions = {};
 
/**
 * Messages checked for delivery during the next fixed update.
 */
private var fixedUpdateMessages = DelayedMessageList(startingCapacity,
                                                     growthRate,
                                                     RouteMessage);
/**
 * Messages checked for delivery during the next update.
 */
private var updateMessages = DelayedMessageList(startingCapacity, growthRate,
                                                RouteMessage);
/**
 * Messages checked for delivery during the next late update.
 */
private var lateUpdateMessages = DelayedMessageList(startingCapacity,
                                                    growthRate,
                                                    RouteMessage);
/**
 * Messages checked for delivery during the end of frame.
 */
private var endOfFrameMessages = DelayedMessageList(startingCapacity,
                                                    growthRate,
                                                    RouteMessage);
 
/**
 * Send a message based on header options.
 */
function SendMessage(message : Message) {
  if (!message.header || !message.header.from) {
    Debug.LogError("Message must include header and header.from", this);
    return;
  }
 
  message.sentTime = Time.time;
  message.sentFrame = Time.frameCount;
  message.desiredArrivalTime = (message.header.deliveryTime != -1) ?
    message.header.deliveryTime + Time.time : Time.time;
  message.desiredArrivalFrame = (message.header.deliveryFrame != -1) ?
    message.header.deliveryFrame + Time.frameCount : Time.frameCount;
 
  switch (message.header.deliveryStage) {
  case DeliveryStage.Immediate:
    RouteMessage(message); break;
  case DeliveryStage.FixedUpdate:
    fixedUpdateMessages.Add(message); break;
  case DeliveryStage.Update:
    updateMessages.Add(message); break;
  case DeliveryStage.LateUpdate:
    lateUpdateMessages.Add(message); break;
  case DeliveryStage.EndOfFrame:
    endOfFrameMessages.Add(message); break;
  }
}
 
/**
 * Subscribe to messages matching this subscription.
 */
function Subscribe(subscription : MessageSubscription) {
  if (!(subscription.handler && subscription.type)) {
    Debug.LogError("Subscription must include handler and type",
                   this);
    return;
  }
 
  var list : Array = subscriptions[subscription.type];
  if (!list) {
    list = Array();
    subscriptions[subscription.type] = list;
  }
  list.Add(subscription);
}
 
/**
 * Unsubscribe this existing subscription (must be exact same instance that
 * was susbscribed and not a copy). Returns true if subscription was
 * unsubscribed and false if a matching subscription was not found.
 */
function Unsubscribe(subscription : MessageSubscription) : boolean {
  var subscriptionFound = false;
  var list : Array = subscriptions[subscription.type];
  if (list) {
    for (var i = 0; i < list.length; i++) {
      if (list[i] == subscription) {
        list.RemoveAt(i);
        subscriptionFound = true;
        break;
      }
    }
  }
  return subscriptionFound;
}
 
/**
 * Start EndOfFrame coroutine that sends end of frame delivery stage messages
 * at the end of each frame by using yield WaitForEndOfFrame.
 */
function Awake() {
  EndOfFrame();
}
 
function FixedUpdate() {
  fixedUpdateMessages.SendDelayedMessages();
}
 
function Update() {
  updateMessages.SendDelayedMessages();
}
 
function LateUpdate() {
  lateUpdateMessages.SendDelayedMessages();
}
 
private function EndOfFrame() : IEnumerator {
  while (true) {
    yield WaitForEndOfFrame;
    endOfFrameMessages.SendDelayedMessages();
  }
}
 
/**
 * Routes message to any interested subscribers.
 */
private function RouteMessage(message : Message) {
  // First route message to subscriptions for this message subclass
  // then route message to catch all subscriptions for the Message baseclass
  var receivedByOne =
    RouteMessageWithList(message, subscriptions[message.GetType()]);
  var receivedByTwo =
    RouteMessageWithList(message, subscriptions[Message]);
  var receivedByVisibleReceiver = receivedByOne || receivedByTwo;
 
  if (message.header.requireReceiver && !receivedByVisibleReceiver) {
    Debug.LogError("Message routed without the required receiver: " + message,
                   this);
  }
}
 
/**
 * Helper method for RouteMessage(). Routes message to subscribers in list
 * and returns true if message was received by at least one visible receiver.
 */
private function RouteMessageWithList(message : Message, list : Array) : boolean {
  var receivedByVisibleReceiver = false;
  if (list) {
    for (var subscription : MessageSubscription in list) {
      if (SubscriptionIncludesMessage(subscription, message)) {
        subscription.handler(message);
        if (!subscription.hiddenReceiver) receivedByVisibleReceiver = true;
      }
    }
  }
  return receivedByVisibleReceiver;
}
 
/**
 * Returns true if the subscription includes messages of this kind. See the
 * MessageSubscription class for details on matching rules.
 */
private function SubscriptionIncludesMessage(subscription : MessageSubscription,
                                             message : Message) : boolean {
  var header = message.header;
  return (!subscription.from || subscription.from == header.from) &&
         (!subscription.to || subscription.to == header.to) &&
         HeaderHasSubscriptionTag(header, subscription);
}
 
/**
 * Returns true if at least one of the subscription.tags is also a tag
 * in the header or if the subscription.tags list is empty.
 */
private function HeaderHasSubscriptionTag(header : MessageHeader,
                   subscription : MessageSubscription) : boolean {
  if (subscription.tags && subscription.tags.length != 0) {
    for (var tag : MessageTag in subscription.tags) {
      for (var headerTag : MessageTag in header.tags) {
        if (headerTag == tag) {
          return true;
        }
      }
    }
    return false; // no matching tags found
  }
  return true; // an empty subscription.tags list always matches
}CompactList.js /**
 * A special case list implementation with useful performance characteristics
 * over Array() and ArrayList() in some circumstances. A compact list cannot
 * hold null references and elements can only be accessed sequentially and
 * in an all or nothing matter. Also the capacity of the native array used
 * to hold list elements may increase as length increases but it will never
 * decrease as length decreases.
 *
 * @author Fernando Zapata (fernando@cpudreams.com)
 */
class CompactList {
  /**
   * Read only. Number of elemnts in a compact list. Not guaranteed to be up
   * to date while Iterate is in process.
   */
  var length : int;
  private var data : Object[];
  private var capacity : int;
  private var growthRate : float;
 
  /**
   * Set the starting capacity of the array used to store the CompactList's
   * elements. When Add is called and the internal array is at capacity
   * a new array of size Mathf.CeilToInt(capacity * growthRate) is allocated.
   * Growth rate must be greater than 1.0.
   */
  function CompactList(startingCapacity : int, growthRate : float) {
    if (growthRate <= 1.0) {
      Debug.LogError("Invalid CompactList growthRate, must be > 1.0");
    }
    data = new Object[startingCapacity];
    capacity = startingCapacity;
    this.growthRate = growthRate;
    length = 0;
  }
 
  /**
   * Adds object to the end of the list. The object cannot be null.
   */
  function Add(o : Object) {
    if (!o) {
      Debug.LogError("Cannot add null reference to CompactList");
    }
 
    if (length == capacity) {
      // allocate new larger array and copy elements from old array
      capacity = Mathf.CeilToInt(capacity * growthRate);
      var newData = new Object[capacity];
      for (var i = 0; i < length; i++) {
        newData[i] = data[i];
      }
      data = newData;
    }
    data[length] = o;
    length++;
  }
 
  /**
   * Iterates through each element in the list in the order they where added
   * to the list and calls the process function once for each element.
   * The current element is passed as the first parameter of the function.
   * The process function should return true if the processed element should be
   * removed from this CompactList. Returning true from the process function
   * is the only method of removing elements from a compact list.
   */
  function Iterate(process : Function) {
    var nullIndex = -1;
    for (var i = 0; i < length; i++) {
      var o = data[i];
      if (process(o)) {
        // remove current element
        data[i] = null;
        if (nullIndex == -1) {
          nullIndex = i;
        }
      } else {
        // move current element in order to compact the array
        if (nullIndex != -1) {
          data[nullIndex] = o;
          data[i] = null;
          // since elements are removed in order the next nullIndex is always
          // nullIndex + 1
          nullIndex++;
        }
      }
    }
    if (nullIndex != -1) {
      length = nullIndex;
    }
  }
}
}
