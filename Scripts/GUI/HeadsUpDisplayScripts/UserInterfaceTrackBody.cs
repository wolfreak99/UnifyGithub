// Original url: http://wiki.unity3d.com/index.php/UserInterfaceTrackBody
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/HeadsUpDisplayScripts/UserInterfaceTrackBody.cs
// File based on original modification date of: 7 March 2016, at 21:00. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.HeadsUpDisplayScripts
{


UserInterfaceTrackBody Originally by Opless 
Initialise this script with your GUI camera and the gameobject you're following 
Remember Unity5's new UI may get obscured by the object it's tracking so you probably need a separate camera for the UI. 
I assume you'll parent this to a regular "Screen Space Overlay" canvas. 
Code using UnityEngine;
using System.Collections;
 
public class UserInterfaceTrackBody : MonoBehaviour
{
    RectTransform canvRect;
    GameObject go;
    Camera cam;
 
    RectTransform rectTransform;
 
    // Use this for initialization
    void Start()
    {
        this.rectTransform = GetComponent<RectTransform>();
    }
 
    bool IsTargetVisible()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var point = body.GO.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }
 
 
 
    // Update is called once per frame
    void Update()
    {
        bool show = IsTargetVisible();
        if (!show)
        {
            this.rectTransform.anchoredPosition = new Vector2(-Screen.width, -Screen.height);
            return;
        }
 
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 newPosn = cam.WorldToViewportPoint(go.transform.position);
        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
        newPosn.x *= this.canvRect.sizeDelta.x;
        newPosn.y *= this.canvRect.sizeDelta.y;
        //The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
        //But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
        //We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
        //returned value will still be correct.
 
        newPosn.x -= this.canvRect.sizeDelta.x * this.canvRect.pivot.x;
        newPosn.y -= this.canvRect.sizeDelta.y * this.canvRect.pivot.y;
 
        this.rectTransform.anchoredPosition = newPosn;
    }
 
    public void Initialise(Canvas canv, GameObject go, Camera cam)
    {
        this.canvRect = canv.GetComponent<RectTransform>();
        this.go = go;
        this.cam = cam;
    }
}
}
