/*************************
 * Original url: http://wiki.unity3d.com/index.php/Secure_UnitySingleton
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/Secure_UnitySingleton.cs
 * File based on original modification date of: 11 February 2016, at 16:46. 
 *
 * 	Author: Jon Kenkel (nonathaj)
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.GeneralConcepts
{
    Contents [hide] 
    1 Description 
    2 Usage: Different UnitySingleton Types 
    2.1 Exists In Scene 
    2.2 Loaded From Resources 
    2.3 Create On New GameObject 
    3 Usage: Unity type inheritance 
    4 Secret Singletons (and how to avoid them) 
    5 Code 
    5.1 UnitySingletonAttribute.cs 
    5.2 UnitySingleton.cs 
    
    Description This singleton differs from others on this wiki in that the creator must specifically define how the singleton creates using specific methods, and when that singleton is destroyed. This allows the creator to know the potential lifecycle of the singleton when creating it. 
    Using this script assumes you understand the Singleton Pattern and when to use it in a general program. 
    This singleton is not a replacement for a generic C# singleton. Instead this singleton is intended for using Unity specific functionality: 
    Access to MonoBehaviour Functions (Update, Start, OnLevelWasLoaded) 
    Data that is convenient to modify using the prefab inspector (Allow designers a nice UI without custom scripting an editor extension) 
    Object requires that gameobjects have a certain parent/child relationship or other scripts attached to the object. 
    Is this object creating secret singletons 
    Usage: Different UnitySingleton Types These types are the options for loading the singleton when Instance is called and a new instance must be generated. 
    When selecting a type for your singleton the most important question is "What kind of data/state does this singleton store?" and that should be used to determine how that singleton is generated and managed. 
    Exists In Scene This type searches the current scene for an object with the singleton component attached to it. 
    This is useful for objects that store data about a specific level, such as enemy generator counts, or level timers. As it allows a designer to modify the object in that scene, and the settings will only be referenced in that scene. 
    //Enemy generator stores rate of enemy creation, already exists in the level, and should be destroyed when a new level is loaded (as that level should have it's own generator with different settings).
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene, true)]
    public class EnemyGenerator : UnitySingleton<EnemyGenerator>
    {
    	//code here
    }Loaded From Resources This type creates an instance of a prefab with the singleton component attached to it from a Resources folder. 
    This is useful for storing custom configuration that is exactly the same across the entire life of the object (most likely the entire project itself), such as an input manager that stores the sensitivity of input, or network manager with configurations settings like port numbers. 
    //Input module is loaded from "Resources/Input Manager" prefab, and is NOT destroyed when new scenes load.
    [UnitySingleton(UnitySingletonAttribute.Type.LoadedFromResources, false, "Input Manager")]
    public class InputModule : UnitySingleton<InputModule>
    {
    	//code here
    }Create On New GameObject This type creates a new GameObject and attaches a new instance of the singleton component to it. 
    This is useful for objects that do not need any custom configuration, such as a performance tracker for framerate. Such a script would need to exist for the entirety of the project and need to be easy to reference, but does not require any designer input to configure properly. 
    //Stat Tracker tracks the min/max/average framerate across the length of the game.  Does not have any configuration, so it simply created on a new GameObject and is NOT destroyed when a new scene is loaded.
    [UnitySingleton(UnitySingletonAttribute.Type.CreateOnNewGameObject, false)]
    public class StatTracker : UnitySingleton<StatTracker>
    {
    	//code here
    }Usage: Unity type inheritance Not every script you need to create a singleton for will always inherit from will be directly from MonoBehaviour. The best example of this is the NetworkBehaviour and NetworkLobbyManager added with the 5.1 networking update. Both of these types extend MonoBehaviour, but they also have other functions that you want your singleton to be able to override. You can achieve this goal by creating a singleton "wrapper" for your inherited class as follows: 
    public class MyNetworkManager : UnitySingleton<MyNM>
    {
    	//Class that wrappers and gets references to the singleton of type MyNM
    	//    It is advisable to put static functions here, simply for clarity for the caller
    }
     
    [UnitySingleton(UnitySingletonAttribute.Type.LoadedFromResources, false, "Network Manager")]
    public class MyNM : NetworkLobbyManager
    {
    	//Code within singleton here that allows override of parent functions
    }Secret Singletons (and how to avoid them) A common problem that many users have when using Unity is creating 'Secret Singletons' or objects that there are only 1 of, and are referenced in that way, but are not getting the full advantages of using a singleton pattern. It is not always apparent that a script might be creating/using secret singletons, however this is more common in Unity due to the prevalence of functions such as these: 
    MyImportandScript myImportantObject;
     
    void Awake()
    {
    	myImportantObject = GameObject.FindObjectOfType<MyImportantScript>();
    	//OR
    	myImportantObject = GameObject.Find("My Important Object").GetComponent<MyImportantScript>();
    }Both of these functions call an object with the assumption that only one exist, and that it is actually in the scene. However they do not guarantee that either of those is the case, or warn when they are not. Once these problems are found, they can be easily replaced with a singleton component that both adds clarity to how that script should be used, and allows easier/efficient access to it. 
    Code UnitySingletonAttribute.cs Attribute used to store singleton configuration 
    /*
    	Date: 2/9/16
    */
    using System;
     
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class UnitySingletonAttribute : Attribute
    {
    	/// <summary>
    	/// What kind of singleton is this and how should it be generated?
    	/// </summary>
    	public enum Type
    	{
    		ExistsInScene,                  ///already exists in the scene, just look for it
    		LoadedFromResources,            ///load from the Resources folder, at the given path
    		CreateOnNewGameObject,          ///Create a new gameobject and create this singleton on it
    	}
     
    	public readonly Type[] singletonTypePriority;
    	public readonly bool destroyOnLoad;
    	public readonly string resourcesLoadPath;
    	public readonly bool allowSetInstance;
     
    	public UnitySingletonAttribute(Type singletonCreateType, bool destroyInstanceOnLevelLoad = true, string resourcesPath = "", bool allowSet = false)
    	{
    		singletonTypePriority = new Type[] { singletonCreateType };
    		destroyOnLoad = destroyInstanceOnLevelLoad;
    		resourcesLoadPath = resourcesPath;
    		allowSetInstance = allowSet;
        }
     
    	public UnitySingletonAttribute(Type[] singletonCreateTypePriority, bool destroyInstanceOnLevelLoad = true, string resourcesPath = "", bool allowSet = false)
    	{
    		singletonTypePriority = singletonCreateTypePriority;
    		destroyOnLoad = destroyInstanceOnLevelLoad;
    		resourcesLoadPath = resourcesPath;
    		allowSetInstance = allowSet;
    	}
    }UnitySingleton.cs Singleton abstract class 
    /*
    	Author: Jon Kenkel (nonathaj)
    	Date: 2/9/16
    */
    using UnityEngine;
    using System;
     
    /// <summary>
    /// Class for holding singleton component instances in Unity.
    /// </summary>
    /// <example>
    /// [UnitySingleton(UnitySingletonAttribute.Type.LoadedFromResources, false, "test")]
    /// public class MyClass : UnitySingleton&lt;MyClass&gt; { }
    /// </example>
    /// <example>
    /// [UnitySingleton(UnitySingletonAttribute.Type.CreateOnNewGameObject)]
    /// public class MyOtherClass : UnitySingleton&lt;MyOtherClass&gt; { }
    /// </example>
    /// <typeparam name="T">The type of the singleton</typeparam>
    public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
    	/// <summary>
    	/// Is there an instance active of this singleton?
    	/// </summary>
    	public static bool InstanceExists { get { return instance != null; } }
     
    	private static T instance = null;
    	/// <summary>
    	/// Returns an instance of this singleton (if it does not exist, generates one based on T's UnitySingleton Attribute settings)
    	/// </summary>
    	public static T Instance 
    	{
    		get 
    		{
    			TouchInstance();
    			return instance;
    		}
    		set 
    		{
    			UnitySingletonAttribute attribute = Attribute.GetCustomAttribute(typeof(T), typeof(UnitySingletonAttribute)) as UnitySingletonAttribute;
    			if (attribute == null)
    				Debug.LogError("Cannot find UnitySingleton attribute on " + typeof(T).Name);
     
    			if (attribute.allowSetInstance)
    				instance = value;
    			else
    				Debug.LogError(typeof(T).Name + " is not allowed to set instances.  Please set the allowSetInstace flag to true to enable this feature.");
    		}
    	}
     
    	/// <summary>
    	/// Destroy the current static instance of this singleton
    	/// </summary>
    	/// <param name="destroyGameObject">Should we destroy the gameobject of the instance too?</param>
    	public static void DestroyInstance(bool destroyGameObject = true)
    	{
    		if (InstanceExists)
    		{
    			if (destroyGameObject)
    				Destroy(instance.gameObject);
    			else
    				Destroy(instance);
    			instance = null;
    		}
    	}
     
    	/// <summary>
    	/// Called when this object is created.  Children should call this base method when overriding.
    	/// </summary>
    	protected virtual void Awake()
    	{
    		if (InstanceExists && instance != this)
    			Destroy(gameObject);
    	}
     
    	/// <summary>
    	/// Ensures that an instance of this singleton is generated
    	/// </summary>
    	public static void TouchInstance()
    	{
    		if (!InstanceExists)
    			Generate();
    	}
     
    	/// <summary>
    	/// Generates this singleton
    	/// </summary>
    	private static void Generate()
    	{
    		UnitySingletonAttribute attribute = Attribute.GetCustomAttribute(typeof(T), typeof(UnitySingletonAttribute)) as UnitySingletonAttribute;
    		if (attribute == null)
    		{
    			Debug.LogError("Cannot find UnitySingleton attribute on " + typeof(T).Name);
    			return;
    		}
     
    		for (int x = 0; x < attribute.singletonTypePriority.Length; x++)
    		{
    			if (TryGenerateInstance(attribute.singletonTypePriority[x], attribute.destroyOnLoad, attribute.resourcesLoadPath, x == attribute.singletonTypePriority.Length - 1))
    				break;
    		}
    	}
     
    	/// <summary>
    	/// Attempts to generate a singleton with the given parameters
    	/// </summary>
    	/// <param name="type"></param>
    	/// <param name="resourcesLoadPath"></param>
    	/// <param name="warn"></param>
    	/// <returns></returns>
    	private static bool TryGenerateInstance(UnitySingletonAttribute.Type type, bool destroyOnLoad, string resourcesLoadPath, bool warn)
    	{
    		if (type == UnitySingletonAttribute.Type.ExistsInScene)
    		{
    			instance = GameObject.FindObjectOfType<T>();
    			if (instance == null)
    			{
    				if(warn)
    					Debug.LogError("Cannot find an object with a " + typeof(T).Name + " .  Please add one to the scene.");
    				return false;
    			}
    		}
    		else if (type == UnitySingletonAttribute.Type.LoadedFromResources)
    		{
    			if (string.IsNullOrEmpty(resourcesLoadPath))
    			{
    				if(warn)
    					Debug.LogError("UnitySingletonAttribute.resourcesLoadPath is not a valid Resources location in " + typeof(T).Name);
    				return false;
    			}
    			T pref = Resources.Load<T>(resourcesLoadPath);
    			if (pref == null)
    			{
    				if(warn)
    					Debug.LogError("Failed to load prefab with " + typeof(T).Name + " component attached to it from folder Resources/" + resourcesLoadPath + ".  Please add a prefab with the component to that location, or update the location.");
    				return false;
    			}
    			instance = Instantiate<T>(pref);
    			if (instance == null)
    			{
    				if (warn)
    					Debug.LogError("Failed to create instance of prefab " + pref + " with component " + typeof(T).Name + ".  Please check your memory constraints");
    				return false;
    			}
    		}
    		else if (type == UnitySingletonAttribute.Type.CreateOnNewGameObject)
    		{
    			GameObject go = new GameObject(typeof(T).Name + " Singleton");
    			if (go == null)
    			{
    				if (warn)
    					Debug.LogError("Failed to create gameobject for instance of " + typeof(T).Name + ".  Please check your memory constraints.");
    				return false;
    			}
    			instance = go.AddComponent<T>();
    			if (instance == null)
    			{
    				if (warn)
    					Debug.LogError("Failed to add component of " + typeof(T).Name + "to new gameobject.  Please check your memory constraints.");
    				Destroy(go);
    				return false;
    			}
    		}
     
    		if (!destroyOnLoad)
    			DontDestroyOnLoad(instance.gameObject);
     
    		return true;
    	}
}
}
