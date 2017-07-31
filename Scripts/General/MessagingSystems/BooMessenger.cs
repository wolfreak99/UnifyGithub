// Original url: http://wiki.unity3d.com/index.php/BooMessenger
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MessagingSystems/BooMessenger.cs
// File based on original modification date of: 1 March 2013, at 07:50. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MessagingSystems
{
Author: Ofer Reichman (tortoise) 
Contents [hide] 
1 Description 
2 Usage 
2.1 Creating and using the messenger 
2.2 Dispatching a message 
2.3 Listening to messages 
2.4 Defining messages 
2.5 Grouping messages 
3 Performance 
4 Integrating with JavaScript/C# 
5 Code 
5.1 BooMessenger.boo 
5.2 Message.boo 
5.3 God.boo 

Description BooMessenger is an efficient and simple to use messaging platform. It facilitates lightweight dispatching of messages (events) between script components without the need of coupling. It is written in Boo and can be used by any scripting language. Messages are properly defined so that errors are caught during compilation. Messages can also be grouped hierarchially using inheritance for easy subscription. 
Usage Creating and using the messenger You don't need to create the messenger. It is created implicitly by the God class, which is bundled and implements the singleton pattern. To access the single messenger simply use: 
        God.Inst.HermesThe singleton mechanics were decoupled to provide a single place for all single instances. 
Tip: Feel free to use the God class as a container for any classes of yours of which you only need one instance. 
Dispatching a message To create and send a message simply instantiate a message class, provided with parameters, and voila! For example, if you have a message called MessageText you can write: 
        MessageText("Hello!")The message will be sent right away. 
Listening to messages You can subscribe to receive a message or a group of messages using the Listen function. It receives the message type and the listener, which is the component calling (self): 
    def OnEnable ():
        God.Inst.Hermes.Listen(MessageText, self)Note: The listening script must be attached to a game object. 
You can unsubscribe using the StopListening function. It receives the same parameters: 
    def OnDisable ():
        God.Inst.Hermes.StopListening(MessageText, self)Any received message will be handled by a function named OnMsgName, where Name is the name of the message class, without the Message prefix: 
    def OnMsgText (msg as MessageText):
        Debug.Log("Received a text message: ${msg.Text}")Yes, naming is important. You should give all your message class names that start with Message. 
Listening to a group of messages will be explained later in the Grouping messages section. 
Defining messages Messages are defined by inheriting from the Message class: 
class MessageGamePaused (Message):
    passYou can define a message with additional properties by adding properties to your message class. For example, you might want to attach a string: 
class MessageText (Message):
 
    Text:
        get:
            return _text
    _text as string
 
    def constructor (text):
 
        _text = text
 
        # send the message
        super()Make sure that the base class constructor is called last, since it actually sends the message. 
Tip: Create a folder for all of your message scripts to keep your workspace organized. 
Grouping messages You can group your messages hierarchially using multiple levels of inheritance. For example, you can have a group of messages that have something in common: 
class MessageBall (Message):
    passInherit from it to create individual messages: 
class MessageBallKicked (MessageBall):
    passclass MessageBallInGoal (MessageBall):
    passclass MessageBallOutOfBounds (MessageBall):
    passEasily subscribe to receive all of them: 
import UnityEngine
 
class Referee (MonoBehaviour): 
 
    def OnEnable ():
        God.Inst.Hermes.Listen(MessageBall, self)
 
    def OnDisable ():
        God.Inst.Hermes.StopListening(MessageBall, self)
 
    def OnMsgBallKicked (msg as MessageBallKicked):
        Debug.Log("Ball was kicked.")
 
    def OnMsgBallInGoal (msg as MessageBallInGoal):
        Debug.Log("Ball is in goal.")
 
    def OnMsgBallOutOfBounds (msg as MessageBallOutOfBounds):
        Debug.Log("Ball is out of bounds.")In a similar fashion you can also listen to all messages by subscribing to the Message base class. 
Performance BooMessenger is quite efficient when it comes to speed. While it is not as fast as delegates, it is slightly faster than using SendMessage. The reason for this is that it does not iterate through all scripts attached to a game object. It keeps a direct reference to the listening script. 
Still, it should be used moderately. It is ill-advised to use it inside Update or FixedUpdate. 
Integrating with JavaScript/C# It is possible to use BooMessenger together with any JavaScript/C# scripts you may have. The topic of scripting languages interoperability is already covered by Unity in: Script compilation. Simply place all the Boo code inside the "Standard Assets" folder (for example under "Standard Assets/Scripts/Messenger") and leave the code that uses it outside. 
Code BooMessenger.boo import UnityEngine
 
class BooMessenger:
 
    _listeners = {}
    """ Enlisted listeners. """
 
    def Listen (msgType as System.Type, listener as MonoBehaviour):
    """ Starts listening to messages derived from the specified type. """
 
        # verify type inherits from Message
        unless msgType.IsSubclassOf(Message) or msgType == Message:
            raise "Listened type is not a Message"
 
        # get list (create if necessary)
        if msgType not in _listeners:
            _listeners[msgType] = []
        list as List = _listeners[msgType]
 
        # add listener
        if listener not in list:
            list.Add(listener)
 
    def StopListening (msgType as System.Type, listener as MonoBehaviour):
    """ Stops listening to messages derived from the specified type. """
 
        # get list
        list as List = _listeners[msgType]
        return unless list
 
        # remove listener
        list.Remove(listener)
 
    def Send (msg as Message):
    """ Dispatches a message. """
 
        # send message (to listeners of base classes too)
        for msgType in msg.BaseClasses:
            # get list
            list = _listeners[msgType]
            continue unless list
 
            # send to all listeners
            for i in range(len(list) - 1, -1, -1):
                listener as MonoBehaviour = list[i]
                # invoke component method by name
                cb = listener.GetType().GetMethod(msg.FunctionName)
                if cb: cb.Invoke(listener, (msg,))Message.boo class Message:
 
    FunctionName:
    """ Method name in listener components """
        get:
            return _functionName
    _functionName as string
 
    BaseClasses:
    """ Inheritance route """
        get:
            return _baseClasses
    _baseClasses as (System.Type)
 
    def constructor ():
    """ Creates and dispatches a message. """
 
        # replace 'Message' with 'OnMsg'
        _functionName = 'OnMsg' + self.GetType().ToString()[7:]
 
        _baseClasses = array(System.Type, GetBaseClasses())
 
        God.Inst.Hermes.Send(self)
 
    protected def GetBaseClasses ():
    """ Generates inheritance route """
 
        msgType = self.GetType()
        while msgType != Message:
            yield msgType
            msgType = msgType.BaseType
        yield MessageGod.boo class God:
 
    static Inst as God:
    """ Calls upon God """
        get:
            God() unless _instance
            return _instance
    static _instance as God
 
    Hermes:
    """ The messenger """
        get:
            return _hermes
    _hermes as BooMessenger
 
    private def constructor ():
    """ Wakes up God """
 
        _instance = self
        _hermes = BooMessenger()
}
