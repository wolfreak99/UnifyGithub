// Original url: http://wiki.unity3d.com/index.php/FingerManager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/FingerManager.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Author: Rozgo 
Description I created this FingerManager.cs script to handle multiple touches, moves, ends and cancels. Just drop it on only one master object and listen to FingerBegin, FingerMove, FingerEnd and FingerCancel messages on all objects you want to be interactive. The target objects need a collider. 
// Author: alex@rozgo.com
// License: enjoy
 
using UnityEngine;
using System.Collections;
 
public class Finger {
   public iPhoneTouch touch;
   public bool moved = false;
   public ArrayList colliders = new ArrayList();
}
 
public class FingerManager : MonoBehaviour {
 
   public ArrayList fingers = new ArrayList();
 
   void Update () {
      RaycastHit[] hits;
      foreach (iPhoneTouch evt in iPhoneInput.touches) {
         if (evt.phase==iPhoneTouchPhase.Began) {
            Finger finger = new Finger();
            finger.touch = evt;
            Ray ray = Camera.main.ScreenPointToRay(evt.position);
            hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits) {
               finger.colliders.Add(hit.collider);
               GameObject to = hit.collider.gameObject;
               to.SendMessage("FingerBegin",evt,SendMessageOptions.DontRequireReceiver);
            }
            fingers.Add(finger);
         }
         else if (evt.phase==iPhoneTouchPhase.Moved) {
            for (int i=0;i<fingers.Count;++i) {
               Finger finger = (Finger)fingers[i];
               if (finger.touch.fingerId==evt.fingerId) {
                  finger.moved = true;
                  foreach (Collider collider in finger.colliders) {
                     if (collider==null) {continue;}
                     GameObject to = collider.gameObject;
                     to.SendMessage("FingerMove",evt,SendMessageOptions.DontRequireReceiver);
                  }
               }
            }
         }
         else if (evt.phase==iPhoneTouchPhase.Ended || evt.phase==iPhoneTouchPhase.Canceled) {
            Ray ray = Camera.main.ScreenPointToRay(evt.position);
            hits = Physics.RaycastAll(ray);
            for (int i=0;i<fingers.Count;) {
               Finger finger = (Finger)fingers[i];
               if (finger.touch.fingerId==evt.fingerId) {
                  foreach (Collider collider in finger.colliders) {
                     if (collider==null) {continue;}
                     bool canceled = true;
                     foreach (RaycastHit hit in hits) {
                        if (hit.collider==collider) {
                           canceled = false;
                           GameObject to = collider.gameObject;
                           to.SendMessage("FingerEnd",evt,SendMessageOptions.DontRequireReceiver);
                        }
                     }
                     if (canceled) {
                        GameObject to = collider.gameObject;
                        to.SendMessage("FingerCancel",evt,SendMessageOptions.DontRequireReceiver);
                     }
                  }
                  fingers[i] = fingers[fingers.Count-1];
                  fingers.RemoveAt(fingers.Count-1);
               }
               else {
                  ++i;
               }
            }
         }
      }
   }
}Write message handlers like this, on each of your interactive objects. 
	void FingerBegin (iPhoneTouch evt) {
	}
 
	void FingerMove (iPhoneTouch evt) {
	}
 
	void FingerEnd (iPhoneTouch evt) {
	}
 
	void FingerCancel (iPhoneTouch evt) {
	}Note from UnityQA: Wozik 
If you want maximal preciseness, handle touches in FixedUpdate function, not in OnGUI. 
And I can share a little bug, we're fixing now - after 250-300 continuous (both multiple and single) touches you may see 1-2 more TouchEnded states then TouchStarted. Closer to 400 touches, delta may reach 5-6 touches. 
}
