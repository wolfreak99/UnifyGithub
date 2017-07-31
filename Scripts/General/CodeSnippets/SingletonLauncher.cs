// Original url: http://wiki.unity3d.com/index.php/SingletonLauncher
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/SingletonLauncher.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
Contents [hide] 
1 Yet another singleton work around (C#) 
1.1 Requirements 
1.1.1 Notes 
1.2 The Script 
2 Example 
3 = More Notes 
3.1 More Options 
3.2 Important 
4 Finally 

Yet another singleton work around (C#) This implementation of a singletons for unity results in the following: 
No need to place a prefab in every scene 
Awake gets called on any singleton prior to it being used ( guaranteed ) 
Any script used for a singleton can be configured completely in the inspector. 
One game object slipped into the scene for all singletons ( single ). 
Does not do a null/exists check every time a singleton is accessed. 
Requirements One prefab with each singleton type component on it named "Singletons" and placed in a folder named "Resources" anywhere in the Assets folder. 
If using JS or boo with this script, you should place this script into the Standard Assets or Plugins folder so it is compiled first. 
Notes I'm not fluent in non-C# languages in Unity. The main thing you'll need to be able to do is inherit from a generic type. 
The Script Filename should be SingletonEnsure.cs 
public abstract class SingletonMonoBehaviour : MonoBehaviour
{
    // name this after the prefab in the Resources folder.
    public const string resourceName = "Singletons";
 
    // this will be the object containing all components which are singletons.
    static GameObject singletonsObject = null;
 
    protected static class Initializer {
 
        public static readonly int singletonCount;
 
        static Initializer()
        {
            singletonCount = 0;
 
            // load in the prefab from resources.
            var singletonsObjectPrefab = (GameObject)Resources.Load(resourceName, typeof(GameObject));
 
            // see if we loaded/got it.
            if (!singletonsObjectPrefab)
            {
                Debug.LogError("There was no singleton prefab : Resources/" + resourceName);
                return;
            }
 
            // instantiate it
            singletonsObject = (GameObject)Instantiate(singletonsObjectPrefab);
 
            // mark it as do not destroy.
            DontDestroyOnLoad(singletonsObject);
 
            // run through each mono behaviour on the object
            foreach (MonoBehaviour behaviour in singletonsObject.GetComponents<MonoBehaviour>())
            {
                // get the type once.
                var type = behaviour.GetType();
 
                // check that the type is a singleton type.
                if ( typeof(SingletonMonoBehaviour).IsAssignableFrom(type) )
                {
                    // call the SetSingletonFromLoad static private.
                    typeof(SingletonMonoBehaviour<>)
                        .MakeGenericType(type)
                        .GetMethod("SetSingletonFromLoad", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                        .Invoke(null, new object[]{ behaviour} );
                    ++singletonCount;
                }
            }
 
            // send messages optionally
 
            // SingletonAwake is where all singletons have been configured and set. but not neccisarily configured.
            singletonsObjectPrefab.SendMessage("SingletonAwake", SendMessageOptions.DontRequireReceiver);
 
            // SingletonStart is where all singletons have been awoke at minimum
            singletonsObjectPrefab.SendMessage("SingletonStart", SendMessageOptions.DontRequireReceiver);
 
            // optionally add more messages if you need more layers of initialization
        }
 
        // Never call this in a static initializer of a MonoScript or ScriptableObject.
        public static void Bind() {
            // does nothing but ensures the static Initializer is called.
            // should be very low-performance use.
        }
    }
 
    public static GameObject singletonGameObject { get { Initializer.Bind(); return singletonsObject; } }
 
    public static T GetSingleton<T>() where T : SingletonMonoBehaviour<T>
    {
        return SingletonMonoBehaviour<T>.singleton;
    }
 
    // private: Takes a component with given type sees if the type is assinable to SingletonMonoBehaviour return it if it is otherwise null.
    // checks for null
    static SingletonMonoBehaviour SingletonCast(Component component, System.Type type)
    {
        if (typeof(SingletonMonoBehaviour).IsAssignableFrom(type) && component)
        {
            return (SingletonMonoBehaviour)component;
        }
        else
        {
            return null;
        }
    }
 
    // private: Takes a component with unknown type, sees if the typ eis assinable to SingletonMonoBehaviour return it if it is otherwise null.
    // checks for null
    static SingletonMonoBehaviour SingletonCast(Component component)
    {
        if (component && typeof(SingletonMonoBehaviour).IsAssignableFrom(component.GetType()))
            return (SingletonMonoBehaviour)component;
        else
            return null;
    }
 
    // Only for singleton types. Use singletonGameObject.GetComponent for non singleton types
    public static SingletonMonoBehaviour GetSingleton(System.Type type)
    {
        Initializer.Bind();
 
        return singletonsObject ? SingletonCast( singletonsObject.GetComponent(type), type) : null;
    }
 
    // Only for singleton types. Use singletonGameObject.GetComponent for non singleton types
    public static SingletonMonoBehaviour GetSingleton(string type)
    {
        Initializer.Bind();
 
        return singletonsObject ? SingletonCast( singletonsObject.GetComponent(type) ) : null;
    }
 
    public static bool TryGetSingleton<T>(out T singleton) where T : SingletonMonoBehaviour<T>
    {
        singleton = GetSingleton<T>();
        return singleton != null;
    }
 
    public static int singletonCount { get { return Initializer.singletonCount; } }
 
    public static void Ensure() { Initializer.Bind(); }
}
 
// to use this instead of inheriting MonoBehaviour, instead inherit SingletonMonoBehaviour<> with the type your making
public abstract class SingletonMonoBehaviour<T> : SingletonMonoBehaviour where T : SingletonMonoBehaviour<T>
{
    static T _singleton = null;
 
    private static void SetSingletonFromLoad(T singleton)
    {
        if (_singleton)
        {
            Debug.LogWarning("Setting singleton, but already had one:", _singleton);
            Debug.LogWarning(" --> Incoming", singleton);
        }
        _singleton = singleton;
    }
 
    // access the singleton for this type ( readonly )
    public static T singleton { get { Initializer.Bind(); return _singleton; } }
    // optionally one could change this to protected and expose the singleton in the class itself how they would like or not at all.
}
 
public class SingletonEnsure : MonoBehaviour {
    void Awake() { SingletonMonoBehaviour.Ensure(); }
}Example [AddComponentMenu("Singletons/Scoreboard")]
public class Scoreboard : SingletonMonoBehaviour<Scoreboard> {
 
    public int teamBlueScore, teamRedScore;
 
    public static void AddPoints(bool redTeam, int points) { 
        if( !redTeam ) singleton.teamBlueScore += points;
        else singleton.teamRedScore += points;
    }
}Then making a prefab gameobject in Resources named Singletons and adding the above example to the game object inside the prefab. 
Then in other parts of the game you would access this by the following ( snippet ) 
void GoalScored( bool redGoal ) {
    // add to the other team
    Scoreboard.AddPoints(!redGoal, 1);
}or alternatively accessing the singleton directly 
void GoalScored( bool redGoal ) {
    if( redGoal ) Scoreboard.singleton.teamBlueScore += 1;
    else Scoreboard.singleton.teamRedScore += 1;
}= More Notes To explain it more, once any singleton script's .singleton gets requested if the entire singleton prefab has not been loaded from resources it does so prior to returning the loaded in value. This way you can make sure that the system will be there as long as it is in the prefab itself. 
The generics system allows for easy upfront access to the singleton loaded via prefab, and structure. The fact that the prefab is loaded from resources gives you the benefit of configuring the scripts inside the editor as well. 
The way that SingletonMonBehaviour<T> works theres also no sort of lookup involved, and any calls to SingletonMonoBehavior.Initializer.Bind() is simply a empty void function which ensures the static constructor loads in the resources needed when it is needed, and never lets go. 
More Options Sometimes you may not even need to actually instantiate the prefab which would be a valid way to go as well. But this approach in general i think supports most cases and does not tie into any messages a script may choose to. With that said you could remove the Instantiate line and use the prefab itself as long as you do not modify any members off it and do not require Awake, Start, OnDestroy, Update or other messages that may be sent. 
You're allowed to put scripts that are not singleton types onto the prefab which do not need a public singleton accessor. These will init like normal and if you wanted the singleton off of the prefab in game you have the option of doing the following 
SingletonMonoBehaviour.singletonGameObject.GetComponent<TypeHere>();this may be more ideal to some. Important This system is based off strong static initialization. Unity does not like some things done in static initialization such as loading from resources, using Random, or instantiating. The trick to this script is that the static initialization is not that of a object unity would create but rather a subclass of one which will not static initialize until something on it is called ( in this case Initializer.Bind() ). This approach is safe as long as you do not call any singletons or functions which use singletons in a static constructor(initializer) of a unity type such as MonoBehaviour or ScriptableObject. 
so these are not okay: 
public class Game : MonoBehaviour {
    static Scoreboard scoreboard;
    static Game() { scoreboard = Scoreboard.singleton; }
}public class Game : MonoBehaviour {
    static Scoreboard scoreboard = Scoreboard.singleton;
}public class Game : MonoBehaviour {
    Scoreboard scoreboard = Scoreboard.singleton;
}public class Game : ScriptableObject {
    static Scoreboard scoreboard;
    static Game() { scoreboard= Scoreboard.singleton; }
}where these are okay: 
public class Game : MonoBehaviour {
    Scoreboard scoreboard;
    void Awake() { scoreboard = Scoreboard.singleton; }
}// not a UnityEngine.Object
public class Game {
    Scoreboard scoreboard;
    static Game() { scoreboard = Scoreboard.singleton; }
}Lastly as the singletons are loaded as needed, if you do need or want the system to be loaded prior like if you needed to log into some service. At the title insert a gameobject with the SingletonEnsure component. This will make sure that the singletons will be initialized. 
If using Unity3.4 or above you'll want to set the calling order on SingletonEnsure to a lower value so that it gets called prior to others if you plan on using it. 
With all that said, all singletons load together so if you access one singleton all singletons are loaded by the time you get the singleton back which again if that turns out to be intensive based on how you initialize your singleton types SingletonEnsure is a good idea to place in the intro / first loaded scene so you can avoid a hit later on while playing. 
FinallyAll in all i think its pretty flexible. The few restrictions are in my opinion worth it and structurally sound. 
Feel free to edit this article to support the other languages. 
}
