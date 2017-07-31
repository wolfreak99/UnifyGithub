// Original url: http://wiki.unity3d.com/index.php/AdvancedRepeatButton
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/AdvancedRepeatButton.cs
// File based on original modification date of: 19 January 2013, at 23:06. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Bérenger 
Description Display a repeat button as usual, and gives you control over the repeat start and speed. Because times must be stored, you instances of that class. 
Usage Initialize your buttons then use call the Draw or DrawLayout functions inside the GUI callback. When you click the button the first time it returns true. If you hold it longer than delayBeforeRepeat, it will return true every repeatSteps seconds. 
Here is an example : 
using UnityEngine;
 
public class RBTest : MonoBehaviour
{
    public Rect rect = new Rect( 100, 100, 100, 100 );
    AdvancedRepeatButton advRB;
    public void Start()
    {
        advRB= new AdvancedRepeatButton();
    }
 
    private void OnGUI()
    {
        if( advRB.Draw( rect, "Adv Repeat Button", .5f, .1f ) )
            print( "Hi, it is " + Time.time );
    }
}

C# - AdvancedRepeatButton.cs using UnityEngine;
 
public class AdvancedRepeatButton
{
    private float downTime, lastStep;
    private bool wasClicked;
    public AdvancedRepeatButton() { downTime = -1f; lastStep = -1f; wasClicked = false; }
 
    public bool Draw(Rect r, string txt, float delayBeforeRepeat, float repeatSteps) { return Draw(r, new GUIContent(txt), GUI.skin.button, delayBeforeRepeat, repeatSteps); }
    public bool Draw(Rect r, Texture2D tex, float delayBeforeRepeat, float repeatSteps) { return Draw(r, new GUIContent(tex), GUI.skin.button, delayBeforeRepeat, repeatSteps); }
    public bool Draw(Rect r, GUIContent content, float delayBeforeRepeat, float repeatSteps) { return Draw(r, content, GUI.skin.button, delayBeforeRepeat, repeatSteps); }
    public bool Draw(Rect r, string txt, GUIStyle style, float delayBeforeRepeat, float repeatSteps) { return Draw(r, new GUIContent(txt), style, delayBeforeRepeat, repeatSteps); }
    public bool Draw(Rect r, Texture2D tex, GUIStyle style, float delayBeforeRepeat, float repeatSteps) { return Draw(r, new GUIContent(tex), style, delayBeforeRepeat, repeatSteps); }
    public bool Draw(Rect r, GUIContent content, GUIStyle style, float delayBeforeRepeat, float repeatSteps)
    {
        return ARBLogic( GUI.RepeatButton(r, content, style), delayBeforeRepeat, repeatSteps );
 
    }
 
    public bool DrawLayout(string txt, float delayBeforeRepeat, float repeatSteps, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(txt), GUI.skin.button, delayBeforeRepeat, repeatSteps, options); }
    public bool DrawLayout(Texture2D tex, float delayBeforeRepeat, float repeatSteps, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(tex), GUI.skin.button, delayBeforeRepeat, repeatSteps, options); }
    public bool DrawLayout(GUIContent content, float delayBeforeRepeat, float repeatSteps, params GUILayoutOption[] options) { return DrawLayout(content, GUI.skin.button, delayBeforeRepeat, repeatSteps, options); }
    public bool DrawLayout(string txt, GUIStyle style, float delayBeforeRepeat, float repeatSteps, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(txt), style, delayBeforeRepeat, repeatSteps, options); }
    public bool DrawLayout(Texture2D tex, GUIStyle style, float delayBeforeRepeat, float repeatSteps, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(tex), style, delayBeforeRepeat, repeatSteps, options); }
    public bool DrawLayout(GUIContent content, GUIStyle style, float delayBeforeRepeat, float repeatSteps, params GUILayoutOption[] options)
    {
        return ARBLogic(GUILayout.RepeatButton(content, style, options), delayBeforeRepeat, repeatSteps);
    }
 
    private bool ARBLogic(bool clic, float delayBeforeRepeat, float repeatSteps)
    {
        bool shallPass = false;
        if( Event.current.type == EventType.Repaint )
        {
            if( clic )
            {
                if( !wasClicked )
                {
                    wasClicked = true;
                    downTime = Time.time;
                    lastStep = Time.time;
                    shallPass = true;
                }
                else if( Time.time - downTime > delayBeforeRepeat )
                {
                    float currStep = Time.time - lastStep;
                    if( currStep > repeatSteps )
                    {
                        currStep = -1f;
                        lastStep = Time.time;
                        shallPass = true;
                    }
                }
            }
            else
            {
                wasClicked = false;
                downTime = -1f;
                lastStep = -1f;
            }
        }
 
        return clic && shallPass;
    }
}
}
