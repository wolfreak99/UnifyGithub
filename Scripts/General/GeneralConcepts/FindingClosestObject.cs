// Original url: http://wiki.unity3d.com/index.php/FindingClosestObject
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/FindingClosestObject.cs
// File based on original modification date of: 28 December 2012, at 16:56. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{

Author: Lars-Erik Jakobsson (save) 
Description This script illustrates how to find the closest transform to another transform. Commonly used to check which object is closest to the player. Should be ready to go for mobile devices too. 
Usage Name this script ClosestCollider.js and assign it to the player transform. From Inspector tweak the radius, masked layers and update frequency. 
JavaScript - ClosestCollider.js #pragma strict
#pragma downcast
import System;
 
/*
    ClosestCollider.js
    Description: Finding closest collider without using a pool
    Last updated: 2012-12-28
    Author: save
*/
 
var sphereRadius : float = 10.0; //Radius of the OverlapSphere
var sphereMask : LayerMask; //What layers the OverlapSphere sees
var updateFrequency : float = 1.0; //Update frequency of FindClosestCollider-function
 
private var cachedUF : float; //Cached value of updateFrequency
static var pTransform : Transform; //Cache the Player Transform
 
function Start () {
    pTransform = transform;
    RestartInvokeRepeating();
}
 
//Finding the closest collider
function FindClosestCollider () {
    if (updateFrequency!=cachedUF) RestartInvokeRepeating();
    var pSphere : Collider[] = Physics.OverlapSphere(pTransform.position, sphereRadius, sphereMask);
    if (pSphere==null||pSphere.Length==0) return;
    System.Array.Sort(pSphere, new PositionSorter());
    DoSomethingWithClosestTransform(pSphere[0].transform);
}
 
//Do something with the closest transform, for instance print its name in Console
function DoSomethingWithClosestTransform (t : Transform) {
    Debug.Log(t.name);
}
 
//This is just a fancy extra if you want to update the frequency in play mode
function RestartInvokeRepeating () {
    updateFrequency = Mathf.Clamp(updateFrequency, .01, 10.0);
    CancelInvoke("FindClosestCollider");
    InvokeRepeating("FindClosestCollider", .0, updateFrequency);
    cachedUF = updateFrequency;
}
 
 
//Sort the closest transform
class PositionSorter implements IComparer {
    function Compare(a : System.Object, b : System.Object) : int {
        if ( !(a instanceof Collider) || !(b instanceof Collider)) return;
        var posA : Collider = a;
        var posB : Collider = b;
        return (ClosestCollider.pTransform.position-posA.transform.position).magnitude.CompareTo((ClosestCollider.pTransform.position-posB.transform.position).magnitude);
    }
}
}
