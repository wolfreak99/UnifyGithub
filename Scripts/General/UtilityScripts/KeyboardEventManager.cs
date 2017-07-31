// Original url: http://wiki.unity3d.com/index.php/KeyboardEventManager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/KeyboardEventManager.cs
// File based on original modification date of: 24 April 2012, at 17:38. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{

Author: Berenger 
Contents [hide] 
1 Description 
2 Usage 
3 Example 
4 C# - KeyboardEventManager .cs 

Description An event based keyboard input mager. You can associate one or several functions with a KeyCode. That way, you can detect several keys at a time. 
Usage KeyboardEventManager inherit from MonoBehaviour so you need to put it on a game object. It uses a singleton pattern so you can access it's instance from anywhere. To add an event, use the functions RegisterKeyDown or RegisterKeyUp. To remove an event, use the functions UnregisterKeyDown or UnregisterKeyUp. To remove a key completely, use RemoveKey. Remark : Only the keys with a least one event on up or on down are checked with Input.GetKey... 
Example public class KeyEventMgrTest : MonoBehaviour 
{
	public bool ctrl, shift, a;
 
	void Start () 
	{
		KeyboardEventManager.instance.RegisterKeyDown( KeyCode.LeftControl, new KeyEvent( ComboDown ) );
		KeyboardEventManager.instance.RegisterKeyDown( KeyCode.LeftShift, new KeyEvent( ComboDown ) );
		KeyboardEventManager.instance.RegisterKeyDown( KeyCode.A, new KeyEvent( ComboDown ) );
 
		KeyboardEventManager.instance.RegisterKeyUp( KeyCode.LeftControl, new KeyEvent( ComboUp ) );
		KeyboardEventManager.instance.RegisterKeyUp( KeyCode.LeftShift, new KeyEvent( ComboUp ) );
		KeyboardEventManager.instance.RegisterKeyUp( KeyCode.A, new KeyEvent( ComboUp ) );
	}
 
	void ComboDown( KeyCode K)
	{
		print( "Key : " + K );
		if( K == KeyCode.LeftControl ) ctrl = true;
		if( K == KeyCode.LeftShift ) shift = true;
		if( K == KeyCode.A ) a = true;
 
		if( ctrl && shift && a ) MEGACOMBO();
	}
	void ComboUp( KeyCode K)
	{
		print( "Key : " + K );
		if( K == KeyCode.LeftControl ) ctrl = false;
		if( K == KeyCode.LeftShift ) shift = false;
		if( K == KeyCode.A ) a = false;
	}
 
	void MEGACOMBO()
	{
		print( "MEGACOMBO" );
	}
}

C# - KeyboardEventManager .cs using UnityEngine;
using System.Collections.Generic;
 
public delegate void KeyEvent( KeyCode K );
 
public class KeyboardEventManager : MonoBehaviour 
{
	#region Singleton
	private static KeyboardEventManager m_Instance;
	public static KeyboardEventManager instance
	{
		get
		{
			if( m_Instance == null )
			{
				m_Instance = GameObject.FindObjectOfType( typeof( KeyboardEventManager ) ) as KeyboardEventManager;
				if( m_Instance == null )
					m_Instance = new GameObject( "KeyboardEventManager Temporary Instance", typeof( KeyboardEventManager ) ).GetComponent< KeyboardEventManager >();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}
 
	void Awake() 
	{
		if( m_Instance == null )
		{
			m_Instance = this;
			m_Instance.Init();
		}
	}
	#endregion
 
	private List< KeyCode > keys;
	private Dictionary< KeyCode, KeyEvent > keyDownEvents, keyUpEvents;
 
	private void Init()
	{
		keyDownEvents = new Dictionary<KeyCode, KeyEvent>();
		keyUpEvents = new Dictionary<KeyCode, KeyEvent>();
		keys = new List<KeyCode>();
	}
 
	#region Registration
	public void RegisterKeyDown( KeyCode K, KeyEvent kEvent )
	{
		if( keyDownEvents.ContainsKey(K) ) 
			keyDownEvents[K] += kEvent;
		else{
			if( !keys.Contains(K) ) keys.Add( K );
			keyDownEvents.Add( K, kEvent );
		}
	}
 
	public void RegisterKeyUp( KeyCode K, KeyEvent kEvent )
	{
		if( keyUpEvents.ContainsKey(K) ) 
			keyUpEvents[K] += kEvent;
		else{
			if( !keys.Contains(K) ) keys.Add( K );
			keyUpEvents.Add( K, kEvent );
		}
	}
 
	public void UnregisterKeyDown( KeyCode K, KeyEvent kEvent, bool removeKey )
	{
		if( keyDownEvents.ContainsKey(K) ){
			keyDownEvents[K] -= kEvent;
			if( keyDownEvents[K] == null )
				keyDownEvents.Remove(K);
		}
		if( removeKey ) RemoveKey( K );
	}
 
	public void UnregisterKeyUp( KeyCode K, KeyEvent kEvent, bool removeKey )
	{
		if( keyUpEvents.ContainsKey(K) ){
			keyUpEvents[K] -= kEvent;
			if( keyUpEvents[K] == null )
				keyUpEvents.Remove(K);
		}
		if( removeKey ) RemoveKey( K );
	}
 
	public void RemoveKey( KeyCode K )
	{
		if( keyDownEvents.ContainsKey( K ) ) keyDownEvents.Remove( K );
		if( keyUpEvents.ContainsKey( K ) ) keyUpEvents.Remove( K );
		if( keys.Contains( K ) ) keys.Remove( K );
	}
	#endregion
 
	#region Key detection
	private void Update () 
	{
        foreach (KeyCode key in keys) 
		{
            if( Input.GetKeyDown(key) )
				OnKeyDown( key );
 
            if( Input.GetKeyUp(key) )
				OnKeyUp( key );
        }
	}
 
	private void OnKeyDown( KeyCode K )
	{
		KeyEvent E = null;
		if( keyDownEvents.TryGetValue(K, out E) ) 
			if( E != null )
				E(K);
	}
	private void OnKeyUp( KeyCode K )
	{
		KeyEvent E = null;
		if( keyUpEvents.TryGetValue(K, out E) )
			if( E != null )
				E(K);
	}
	#endregion
}
}
