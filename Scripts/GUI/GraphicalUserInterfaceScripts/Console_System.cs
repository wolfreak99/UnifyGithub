/*************************
 * Original url: http://wiki.unity3d.com/index.php/Console_System
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/Console_System.cs
 * File based on original modification date of: 30 June 2014, at 20:28. 
 *
 * Author: Nicholas Ventimiglia (nventimiglia) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    
    File : Media:GUIConsole.zip 
    
    NOTE: Nicholas Ventimiglia's website is : http://www.avariceonline.com/ . I plan to update it shortly with more goodies. 
    Description I had a need for a console application for testing Unity3d services, so I made one. 
    It has a text input 
    You can register "interpreters" to process the input (ie: chat interpreter) 
    It has a command menu (buttons) on the right... to make testing things easier 
    Separation of the model (ConsoleContext) from the UI (ConsoleGUI) 
    Support for toggling visibility 
    Support for listening to Application Log Events. 
    http://i.imgur.com/Z2wGlpn.png 
    http://wiki.unity3d.com/index.php/Console_System 
    Mobile Friendly 
    Usage Unzip the package. Run the scene. ConsoleGUI.cs is the needed view script. The datamodel (ConsoleContext.cs) does not inherit from monobehaviour. 
    If you want to change the default settings (like colors) there is a ConsoleSetup.cs monobehaviour. It makes any desired changes to the static datamodel on Awake. 
    
    Using the Console script from your scripts: 
    ConsoleContext.Instance is a public static accessor 
    
    
    Log (string, type) 
    Adds a message to the list. 
    
    LogWarning() (string) 
    Adds a warning message to the list. 
    
    LogError() (string) 
    Adds a error message to the list. 
    
    LogSuccess (string) 
    Adds a success message to the list. 
    
    LogInput() (string) 
    Adds a input message to the list. 
    
    LogOutput() (string) 
    Adds a output message to the list. 
    
    Clear() 
    Clears all of the messages from the list and the screen. 
    
    Submit() (string[]) 
Submits a argument for evaluation by the command parser. 
}
