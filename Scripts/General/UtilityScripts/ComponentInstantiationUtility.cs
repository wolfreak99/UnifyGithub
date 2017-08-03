/*************************
 * Original url: http://wiki.unity3d.com/index.php/ComponentInstantiationUtility
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/ComponentInstantiationUtility.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Description 
 *   This is a static helper class that will create a new, parented GameObject with a specified component type attached 
 *   using just one line of code. 
 * Usage 
     // Create a new game object with component AIPlayer and set the player's mood using setMood() function within my custom component
     AIPlayer myNewAIPlayer = Static_Utils.newScriptedGO<AIPlayer>(this);
     
     // Static_Utils returns a reference to the component so we can now call our own methods and functions within AIPlayer directly...
     myNewAIPlayer.setMood("Happy");
     // and because of inheritance we can set the name of the object easily...
     myNewAIPlayer.name = "Steve";
     // and position it
     myNewAIPlayer.transform.position = new Vector3(0,1,0);Code CSharp - Static_Utils.cs 
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
     
    public static class Static_Utils
    {
     
        // USAGE:
        // EXAMPLE 1 - [ComponentType] NewGOVar = Static_Utils.newScriptedGO<[ComponentType]>(this);
        // EXAMPLE 2 - AIPlayer newAIPlayer = Static_Utils.newScriptedGO<AIPlayer>(this);
     
        public static T newScriptedGO<T>(Component Caller) where T : MonoBehaviour
        {
     
            GameObject NewGO = new GameObject();
     
            string StrComponentName = typeof(T).ToString(); // get the component name
     
            NewGO.name = StrComponentName;
     
            NewGO.transform.parent = Caller.transform;
     
            //UnityEngine.MonoBehaviour.print(StrComponentName); // tell the world
     
            NewGO.AddComponent(typeof(T));
     
            Component NewGOComponent = NewGO.GetComponent(StrComponentName);
     
            return (T)NewGOComponent;
     
        }
    }
}
