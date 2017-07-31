// Original url: http://wiki.unity3d.com/index.php/MainMenu
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/MainMenu.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
The system is based around delegates in C#. Simply put, I have a single delegate defined which gets called by my OnGUI method. By simply assigning different methods to that delegate, I can easily switch menus on the fly without any if statements, loading new levels or whatever else people do. 
If you're unsure what a delegate is, MSDN states that it "is a type that defines a method signature, and can be associated with any method with a compatible signature. You can invoke (or call) the method through the delegate. Delegates are used to pass methods as arguments to other methods. Event handlers are nothing more than methods that are invoked through delegates. You create a custom method and a class such as a windows control can call your method when a certain event occurs" 
So in order to harness this, we first create the delegate method signature. This is just a normal method signature with the word delegate added onto it after the access modifier. 
private delegate void GUIMethod();Now that just defines what any delegate that you create should look like as far as its signature. In order to use one in your class, you'll have to create an instance of one. That may seem weird since you define a method but then create an instance. Creating an instance generally means you create an object. Underneath the hood, a delegate is actually a whole new class. We don't need to dig that deep into it though. So, create an instance of your delegate like so: 
private GUIMethod currentGUIMethod;So now that we have that, we can easily assign methods to that delegate and it will switch on the fly. Let me first show you the OnGUI Unity method that you all know and love using our new delegate object. 
public void OnGUI () 
{ 
    this.currentGUIMethod(); 
}As you can see, we're simply callling currentGUIMethod as if it were a normal method. So now we get to actually show the power of delgates. I'll start with a simple example and then show you the entire script for your cut and paste pleasure. 
Let's say we have 2 menus we want to show, a main menu and an options menu. Normally you might have to different objects and switch between those somehow or have some logic that encapsulates everything with bool's and conditional statements. For large menu items this can get cumbersome. First off, let's create our 2 new delegate methods. Recall that they must have the exact same signature os our GUIMethod we created earlier. 
private void MainMenu()
{
    // add your menu GUI code here
}
 
private void OptionsMenu()
{
    // add your menu GUI code here
}If I want to switch to the MainMenu method, all I have to do is say: 
this.currentGUIMethod = MainMenu;Likewise, if I want the options menu, I say: 
this.currentGUIMethod = OptionsMenu;Therefore, in my OnGUI method from above, anytime this.currentGUIMethod() is called, the appropriate menu method is invoked. Let's see how you can switch to the options menu from the main menu real easy using a button from the main menu: 
private void MainMenu()
{
    if (GUI.Button (new Rect (10,25,Screen.width - 220,40), "Options")) 
    {
        // options button clicked, switch to new menu
        this.currentGUIMethod = OptionsMenu;
    } 
}
 
private void OptionsMenu()
{
    if (GUI.Button (new Rect (10,25,Screen.width - 220,40), "Main Menu")) 
    {
        // go back to the main menu
        this.currentGUIMethod = MainMenu;
    } 
}As you can see this really simplifies things and actually makes it a bit more readable. If we put everything together we get: 
using UnityEngine; 
using System.Collections; 
 
public class MainMenu : MonoBehaviour 
{ 
    // define and create our GUI delegate
    private delegate void GUIMethod(); 
    private GUIMethod currentGUIMethod; 
 
    void Start () 
    { 
        // start with the main menu GUI
        this.currentGUIMethod = MainMenu; 
    } 
 
    public void MainMenu() 
    { 
        if (GUI.Button (new Rect (10,25,Screen.width - 220,40), "Options")) 
        {
            // options button clicked, switch to new menu
            this.currentGUIMethod = OptionsMenu;
        } 
    } 
 
    private void OptionsMenu()
    {
        if (GUI.Button (new Rect (10,25,Screen.width - 220,40), "Main Menu")) 
        {
            // go back to the main menu
            this.currentGUIMethod = MainMenu;
        } 
    }    
 
    // Update is called once per frame 
    public void OnGUI () 
    { 
        this.currentGUIMethod(); 
    } 
}Here is the javascript version 
#pragma strict
 
// define and create our GUI delegate
var currentGUIMethod : Function;
 
function Start ()
{
    // start with the main menu GUI
    this.currentGUIMethod = MainMenu;
}
 
function MainMenu()
{
    if (GUI.Button(Rect(10,10,Screen.width/2,40), "Options"))
    {
        // options button clicked, switch to new menu
        this.currentGUIMethod = OptionsMenu;
    }
}
 
function OptionsMenu()
{
    if (GUI.Button (Rect(10,10,Screen.width/2,40), "Main Menu"))
    {
        // go back to the main menu
        this.currentGUIMethod = MainMenu;
    }
}   
 
// Update is called once per frame
function OnGUI ()
{
    this.currentGUIMethod();
}
}
