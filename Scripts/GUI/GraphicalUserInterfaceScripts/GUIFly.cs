// Original url: http://wiki.unity3d.com/index.php/GUIFly
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/GraphicalUserInterfaceScripts/GUIFly.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Jonathan Czeck (aarku) 
DescriptionThis script can be used ot make your GUI objects fly in and off the screen pleasingly. Simply use GameObject.SendMessage to tell all your GUI objects to move when you'd like them to. 
UsagePlace this script on a GameObject. Adjust m_InPosition to be where you want the object located when "in" and similarly for m_OutPosition. If you reset the script, m_InPosition will update. 
From another script, use GameObject.SendMessage to send a Fly message with a boolean variable indicating whether it should fly towards the in or out position. 
Here is an example JavaScript. Note that you could also use Boo or C#. 
//Attach this to the same as the GUIFly is attached to.
 
// Fly in
gameObject.SendMessage("Fly", true);
 
// Wait 5 seconds
yield new WaitForSeconds(5);
 
// Fly out
gameObject.SendMessage("Fly", false);C# - GUIFly.csusing UnityEngine;
using System.Collections;
 
public class GUIFly : MonoBehaviour
{
    public enum InterpolationType
    {
        Linear,
        Sinusoidal,
        Hermite
    }
 
    public Vector3 m_InPosition;
    public Vector3 m_OutPosition;
    public float m_TravelTime = 0.5f;
    public float m_DelayToStartTravelingAfterMessageReceived = 0.1f;
    public bool m_StartWithInPosition = false;
    public InterpolationType m_InterpolationType = InterpolationType.Sinusoidal;
 
    void Start ()
    {
        transform.position = (m_StartWithInPosition) ? m_InPosition : m_OutPosition;
    }
 
    IEnumerator Fly(bool flyIn)
    {
        yield return new WaitForSeconds(m_DelayToStartTravelingAfterMessageReceived);
 
        Vector3 targetPosition = (flyIn) ? m_InPosition : m_OutPosition;
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
 
        while (Time.time < startTime + m_TravelTime)
        {
            switch (m_InterpolationType)
            {
                case InterpolationType.Linear:
                    transform.position = Vector3.Lerp(startPosition, targetPosition, (Time.time - startTime) / m_TravelTime);
                break;
                case InterpolationType.Sinusoidal:
                    transform.position = Sinerp(startPosition, targetPosition, (Time.time - startTime) / m_TravelTime);
                break;
                case InterpolationType.Hermite:
                    transform.position = Hermite(startPosition, targetPosition, (Time.time - startTime) / m_TravelTime);
                break;
            }
            yield return 0;
        }
 
        transform.position = targetPosition;
    }
 
    void Reset()
    {
        m_InPosition = transform.position;
    }
 
    private static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Sinerp(start.x, end.x, value), Sinerp(start.y, end.y, value), Sinerp(start.z, end.z, value));
    }
 
    private static Vector3 Hermite(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value), Hermite(start.z, end.z, value));
    }
 
    /* The following functions are also in the Mathfx script on the UnifyWiki, but are included here so the script is self sufficient. */
 
    private static float Sinerp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
    }
 
    private static float Hermite(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }
}
}
