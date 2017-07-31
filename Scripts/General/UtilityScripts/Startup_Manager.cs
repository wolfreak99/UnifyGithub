// Original url: http://wiki.unity3d.com/index.php/Startup_Manager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/Startup_Manager.cs
// File based on original modification date of: 15 October 2012, at 11:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
There is currently no built-in way to manage the order in which scripts start up. To a certain extent you can get around this by putting more important or prerequisite code into Awake(), but this doesn't solve the problem and if you're entirely likely to run into the same problem again. 
The startup manager is a singleton class that calls an initialization function on participating objects created in the last frame (or at startup) in the order dictated by their priority variable. Participating classes must be derived from the provided BaseBehaviour class (which in turn derives from MonoBehaviour as per usual). This class registers with the startup manager singleton in Start() which then calls Initialize() on it at the begining of the next frame (again, in the appropriate order). 
The BaseBehaviour.priority member is a float. This was a design decision to allow for plenty of tweaking. 
BaseBehaviour derived objects which do not find the StartManager instance (e.g. because it is not present in the scene) will call Initialize themselves in Start() 
Contents [hide] 
1 Basic Usage 
2 C# - StartupManager.cs 
3 C# - BaseBehaviour.cs 
4 C# - TestClass.cs 

Basic Usage 1. Put StartupManager on a game object in your scene somewhere 
2. Derive your scripts (or at least the ones which you need to control the order they start in) from BaseBehaviour instead of MonoBehaviour 
3. Profit 
C# - StartupManager.csusing UnityEngine;
using System.Collections;
 
public class StartupManager : MonoBehaviour {
 
    public static StartupManager Instance;
 
    public bool debug = false;
 
    ArrayList componentList = new ArrayList();
 
    void Awake() {
        Instance = this;
    }
 
    void Update () {
        if( componentList.Count <= 0 )
            return;
 
        if( debug )
            Debug.Log( "StartupManager: Update()" );
 
        // Call Initialize() on everything
        componentList.Sort();
 
        foreach( BaseBehaviour behaviour in componentList ) {
            if( behaviour != null && behaviour._initialized == false )
                initializeObject( behaviour );
        }
 
        componentList.Clear();
    }
 
    public virtual void registerStartup( BaseBehaviour behaviour ) {
        if( debug )
            Debug.Log( "Registering " + behaviour + " @ priority " + behaviour._StartupPriority );
 
        componentList.Add( behaviour );
    }
 
    public void initializeObject( BaseBehaviour behaviour ) {
        behaviour.Initialize();
        behaviour._initialized = true;
    }
 
}C# - BaseBehaviour.csusing UnityEngine;
using System.Collections;
 
public class BaseBehaviour : MonoBehaviour, System.IComparable {
 
    public float _StartupPriority = 0f;
 
    public int CompareTo( object obj ) {
        if( obj is BaseBehaviour ) {
            BaseBehaviour thisObj = (BaseBehaviour) obj;
            return _StartupPriority.CompareTo( thisObj._StartupPriority );
        }
 
        return 0;
    }
 
    [HideInInspector]
    public bool _initialized = false;
 
    public virtual void Initialize() {
        //Debug.Log( _StartupPriority + ": In Inititialize()" );
        _initialized = true;
    }
 
    public virtual void Start () {
        if( StartupManager.Instance != null )
            StartupManager.Instance.registerStartup( this );
        else
            Initialize();
    }
 
    public virtual void LateUpdate() {
        if( StartupManager.Instance == null ) {
            _initialized = true;
        }
    }
}Here is an example of a BaseBehaviour derived class. 
C# - TestClass.csusing UnityEngine;
using System.Collections;
 
public class NetworkGame : BaseBehaviour {
 
    void Awake() {
        _StartupPriority = 10f;
    }
 
    override public void Initialize() {
       Debug.Log( "Starting @ Priority " + _StartupPriority );
    }
}A few things of note: 
If you define start you'll need to use override in the normal way 
The member BaseBehaviour._initialized can be checked to see if the object has been initialized 
_StartupPriority is exposed in the editor so you can change it there for scene/object specific startup priorities 
Groups of BaseBehaviour derived objects created in a particular frame will be dealt with by StartupManager in the same way as at the start of the scene 

Feel free to ask me any questions regarding this script 
 -Ryan Scott-
}
