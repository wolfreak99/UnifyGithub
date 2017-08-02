/*************************
 * Original url: http://wiki.unity3d.com/index.php/ToggleCapsLock
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/ToggleCapsLock.cs
 * File based on original modification date of: 2 September 2016, at 03:58. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    Using caps lock as a key bind is fraught with some pitfalls but there are solutions available. 
    Unity doesn't easily let you know when it is or isn't pressed. KeyCode.Capslock and Event both let you know about instant presses, but not if it is toggled one way or the other. 
    Also, beware of the fact that many people disable or remap caps lock to another key like CONTROL so don't rely on it totally without the ability to remap or a default redundant alternative key. 
    You can access it reliably through the Widnows API according to this: 
    [1] 
    Answer by quicktom · May 10, 2013 at 03:35 PM
    You can solve this as following:
    
     using System.Runtime.InteropServices;
     [DllImport("user32.dll")]
     public static extern short GetKeyState(int keyCode);
     
     bool isCapsLockOn = false;
     void Start()
     {
         isCapsLockOn = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;//init stat
     }
     
     void Update()
     {
         if(Input.GetKeyDown(KeyCode.CapsLock))
         {
             isCapsLockOn  = !isCapsLockOn ;
             //......
         }
     }
    You can even CHANGE caps lock's state in Windows using the same API: 
    [2] 
     using UnityEngine;
     using System.Collections;
     using System.Runtime.InteropServices;
     
     public class ToggleCapsLock:MonoBehaviour {
     
     [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
     private static extern short GetKeyState(int keyCode);
     
     [DllImport("user32.dll")]
     private static extern int GetKeyboardState(byte [] lpKeyState);
     
     [DllImport("user32.dll", EntryPoint = "keybd_event")]
     private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
     
     private const byte VK_NUMLOCK = 0x90; 
     private const byte VK_CAPSLOCK = 0x14; 
     private const uint KEYEVENTF_EXTENDEDKEY = 1; 
     private const int KEYEVENTF_KEYUP = 0x2;
     private const int KEYEVENTF_KEYDOWN = 0x0;
     
     public static bool GetNumLock()
     {    
         return (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
     }
     
     public static bool GetCapsLock()
     {    
         return (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
     }
     
     public static void SetNumLock( bool bState )
     {
         if (GetNumLock() != bState)
         {
             keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0); 
             keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0); 
         }
     }
     
     public static void SetCapsLock( bool bState )
     {
         if (GetCapsLock() != bState)
         {
             keybd_event(VK_CAPSLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0); 
             keybd_event(VK_CAPSLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0); 
         }
     }}
    You can then call it like this: 
     ToggleCapsLock.SetCapsLock(false);
     // or
     ToggleCapsLock.SetCapsLock(true);
}
