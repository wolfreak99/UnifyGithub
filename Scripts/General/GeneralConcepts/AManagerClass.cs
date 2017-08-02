/*************************
 * Original url: http://wiki.unity3d.com/index.php/AManagerClass
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/AManagerClass.cs
 * File based on original modification date of: 18 September 2013, at 19:57. 
 *
 * Author: Keli Hlodversson (freyr) 
 *
 * Warning 
 *   
 * Description 
 *   
 * Usage 
 *   
 * C# - AManager.cs 
 *   
 * Boo - AManager.boo 
 *   
 * Javascript - AManager.js 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.GeneralConcepts
{
    Warning This article is quite dated and missing a bit of explaining. Read in conjunction with Singleton. 
    Description The following snippet shows how to add a static property called instance that will automatically find an instance of the class in the scene and return it. 
    This is useful for managers and other behaviours that only have one instance in the scene and need to be accessed from other classes, as it avoids having each class to keep a reference to the manager object. 
    Hint: A nice place to put game managers in a scene hierarchy is to create an empty game object called Managers and attach all manager behaviours to it. 
    Usage Use the code example as a template when creating a manager-type script. Remember to replace all occurrences of AManager with the name of your class. To access a function, say, Foo() in the manager you call it with (Where "AManager" again should be replaced with the name of your class): 
    AManager.instance.Foo();C# - AManager.cs  
    /// Contribution Create Missing Instance 10/2010: Daniel P. Rossi (DR9885)
     
    using UnityEngine;
     using System.Collections; 
     
     /// AManager is a singleton.
     /// To avoid having to manually link an instance to every class that needs it, it has a static property called
     /// instance, so other objects that need to access it can just call:
     ///        AManager.instance.DoSomeThing();
     ///
     public class AManager : MonoBehaviour {
        // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
        private static AManager s_Instance = null;
     
        // This defines a static instance property that attempts to find the manager object in the scene and
        // returns it to the caller.
        public static AManager instance {
            get {
                if (s_Instance == null) {
                    // This is where the magic happens.
                    //  FindObjectOfType(...) returns the first AManager object in the scene.
                    s_Instance =  FindObjectOfType(typeof (AManager)) as AManager;
                }
     
                // If it is still null, create a new instance
                if (s_Instance == null) {
                    GameObject obj = new GameObject("AManager");
                    s_Instance = obj.AddComponent(typeof (AManager)) as AManager;
                    Debug.Log ("Could not locate an AManager object. \ AManager was Generated Automaticly.");
                }
     
                return s_Instance;
            }
        }
     
        // Ensure that the instance is destroyed when the game is stopped in the editor.
        void OnApplicationQuit() {
            s_Instance = null;
        }
     
        // Add the rest of the code here...
        public void DoSomeThing() {
            Debug.Log("Doing something now", this);
        }
     
     }Boo - AManager.boo I haven't tested that one btw... so please fix it if you find any errors -- Keli 
     import UnityEngine
     
     class AManager (MonoBehaviour) :
     """
     AManager is a singleton.
     To avoid having to manually link an instance to every class that needs it, it has a static property called
     instance, so other objects that need to access it can just call:
            AManager.instance.DoSomeThing()
     """
     
        # s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
        private static s_Instance as AManager
     
        #This defines a static instance property that attempts to find the manager object in the scene and
        # returns it to the caller.
        public static instance :
            get :
                if s_Instance == null :
                    # This is where the magic happens.
                    #  FindObjectOfType(...) returns the first AManager object in the scene.
                    s_Instance =  FindObjectOfType(AManager) 
                    if s_Instance == null:
                        Debug.Log ("Could not locate an AManager object. \n" +
                                            "You have to have exactly one AManager in the scene.");
     
                return s_Instance
     
        # Ensure that the instance is destroyed when the game is stopped in the editor.
        def OnApplicationQuit() :
            s_Instance = null
     
        # Add the rest of the code here...
        def DoSomeThing() :
            Debug.Log("Doing something now", self)Javascript - AManager.js Note: Unity Javascript does not have a way to declare "static" properties (but, non-static properties is declarable), so the instance property has been replaced with a static variable that gets initialised upon start. This also means that one can not be sure that instance has been initialised if it is accessed during initialisation. 
     /// AManager is a singleton.
     /// To avoid having to manually link an instance to every class that needs it, it has a static variable called
     /// instance, so other objects that need to access it can just call:
     ///        AManager.instance.DoSomeThing();
     ///
     static var instance : AManager;
     
     // This is where the magic happens.
     //  FindObjectOfType(...) returns the first AManager object in the scene.
     instance =  FindObjectOfType(AManager);
     if (instance == null)
         Debug.Log ("Could not locate an AManager object. \n" +
                         "You have to have exactly one AManager in the scene.");
     
     // Ensure that the instance is destroyed when the game is stopped in the editor.
     function OnApplicationQuit() {
         instance = null;
     }
     
     // Add the rest of the code here...
     
     function DoSomeThing() {
         Debug.Log("Doing something now", this);
 }
}
