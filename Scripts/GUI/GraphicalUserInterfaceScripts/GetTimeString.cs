// Original url: http://wiki.unity3d.com/index.php/GetTimeString
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/GetTimeString.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
function getTimeString(t : float) :String{
	var hours : int = t / 3600;
	var mins : int = (t % 3600 )/ 60;
	var secs = t % 60;
	return (hours.ToString("00") + ":" + mins.ToString("00") + ":" + secs.ToString("00.00"));
}An alternative version in C#: 
/// using System;
string FormatTime(float fTime)
{			
  TimeSpan t =  TimeSpan.FromSeconds( fTime );
 
  /// You can add more digits by adding more digits eg: {1:D2}:{2:D2}:{3:D2}:{4:D2} to also display milliseconds.
  return string.Format("{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
}Sample of usage in C#: 
using UnityEngine;
 
public class GetTimeString : MonoBehaviour {
    float startTime;
    float time = 0.0f;
 
    void Start () {
        startTime = Time.time;
    }
 
    void Update () {
        time = Time.time - startTime;
        Print(FormatTime(time));
    }
}
}
