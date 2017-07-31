// Original url: http://wiki.unity3d.com/index.php/AdvancedButton
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/AdvancedButton.cs
// File based on original modification date of: 19 January 2013, at 23:05. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Bérenger 
Description Display a a button the way you're used to, but it returns SimpleClick, DoubleClick, Drag, Drop or None, depending on the delays you set in the constructor. 
Usage First, initialiaze an AdvancedButton object. Then in OnGUI, draw and get the result. Let's take a look at the constructor : 
public AdvancedButton(float delayMin, float delayMax, float delayDrag)delayMin and delayMax control the double click. It must occurs at least delayMin seconds after the first click and before delayMax seconds after. 
delayDrag is the delay before the button starts returning Drag. As soon as you leave the rect once you've clicked, this gets restarted. 
Here is an example : 
using UnityEngine;
 
public class AdvButtonTest : MonoBehaviour
{
    private AdvancedButton advButton;
 
    void Awake()
    {
        advButton = new AdvancedButton( 0.1f, 0.3f, 1f );
    }
 
    void OnGUI()
    {
        AdvancedButtonResult result = advButton.Draw( new Rect(100, 100, 200, 100), "Advanced Button" );
 
        if( result == AdvancedButtonResult.SimpleClick )
            print( "Simple click" );
        else if( result == AdvancedButtonResult.DoubleClick )
            print( "Double click !" );
        else if( result == AdvancedButtonResult.Drag )
            print( "Dragging !!" );
        else if( result == AdvancedButtonResult.Drop)
            print( "DROP !!!" );
    }
}C# - AdvancedButton.cs using UnityEngine;
 
public enum AdvancedButtonResult
{
    SimpleClick,
    DoubleClick,
    Drag,
    Drop,
    None
}
 
public class AdvancedButton
{
    public AdvancedButton(float delayMin, float delayMax, float delayDrag )
    {
        hitMin = delayMin;
        hitMax = delayMax;
        dragDelay = delayDrag;
        isHover = false;
    }
 
    private float hitMin, hitMax, dragDelay;
    private float lastHit, lastMouseDown;
    private bool isHover;
    private Rect rect;
 
    // Count the number of click
    private AdvancedButtonResult HandleClickCount(float t)
    {
        // First hit
        if (lastHit < 0f || t > hitMax)
        {
            lastHit = Time.time;
            return AdvancedButtonResult.SimpleClick;
        }
        // Second hit, we test the delay
        else if (t >= hitMin && t < hitMax)
        {
            lastHit = -1f;
            return AdvancedButtonResult.DoubleClick;
        }
 
        return AdvancedButtonResult.None;
    }
 
    // Return true when you press the mouse over a button a stay over it long enough whilst pressing.
    private AdvancedButtonResult HandleDragAndDrop(float t)
    {
        Event e = Event.current;
        if( e.isMouse )
        {
            if( e.button == 0 )
            {
                // Mouse Up, cancel drag
                if( e.type == EventType.mouseUp )
                {
                    bool wasPressed = lastMouseDown > 0f;
                    lastMouseDown = -1f;
                    if( !wasPressed && isHover ) 
                        return AdvancedButtonResult.Drop;
                }
                // Mouse Down
                else if( e.type == EventType.mouseDown )
                {
                    // Over the button
                    if( isHover )
                    {
                        t = -dragDelay;
                        lastMouseDown = Time.time;
                    }
                }
            }
        }
 
        // If we are holding the mouse
        if (lastMouseDown > 0f)
        {
            // Make sure we don't leave the button
            if( isHover )
            {
                // We've hold long enough
                if (t > dragDelay)
                    return AdvancedButtonResult.Drag;
            }
            // We left the button, cancel
            else
                lastMouseDown = -1f;
        }
 
        return AdvancedButtonResult.None;
    }
 
    // Usual GUI functions
    public AdvancedButtonResult Draw(Rect r, string txt) { return Draw(r, new GUIContent(txt), GUI.skin.button); }
    public AdvancedButtonResult Draw(Rect r, Texture2D tex) { return Draw(r, new GUIContent(tex), GUI.skin.button); }
    public AdvancedButtonResult Draw(Rect r, GUIContent content) { return Draw(r, content, GUI.skin.button); }
    public AdvancedButtonResult Draw(Rect r, string txt, GUIStyle style) { return Draw(r, new GUIContent(txt), style); }
    public AdvancedButtonResult Draw(Rect r, Texture2D tex, GUIStyle style) { return Draw(r, new GUIContent(tex), style); }
    public AdvancedButtonResult Draw(Rect r, GUIContent content, GUIStyle style)
    {
        // The drag test must be performed before the drawing, or events will be eaten.
        isHover = r.Contains(Event.current.mousePosition);
        AdvancedButtonResult dragoDropResult = HandleDragAndDrop(Time.time - lastMouseDown);
 
        // The usual button
        bool click = GUI.Button(r, content, style);
 
        if( dragoDropResult != AdvancedButtonResult.None )
            return dragoDropResult;
 
        if( click )
            return HandleClickCount(Time.time - lastHit);
 
        return AdvancedButtonResult.None;
    }
 
 
    // Usual GUILayout functions
    public AdvancedButtonResult DrawLayout(string txt, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(txt), GUI.skin.button, options); }
    public AdvancedButtonResult DrawLayout(Texture2D tex, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(tex), GUI.skin.button, options); }
    public AdvancedButtonResult DrawLayout(GUIContent content, params GUILayoutOption[] options) { return DrawLayout(content, GUI.skin.button, options); }
    public AdvancedButtonResult DrawLayout(string txt, GUIStyle style, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(txt), style, options); }
    public AdvancedButtonResult DrawLayout(Texture2D tex, GUIStyle style, params GUILayoutOption[] options) { return DrawLayout(new GUIContent(tex), style, options); }
    public AdvancedButtonResult DrawLayout(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
    {
        // The drag test must be performed before the drawing, or events will be eaten.
        AdvancedButtonResult dragoDropResult = dragoDropResult = HandleDragAndDrop(Time.time - lastMouseDown);
 
        // The usual button
        bool click = GUILayout.Button(content, style, options);
 
        if( Event.current.type == EventType.Repaint )
        {
            // If repaint, we update the isHover
            rect = GUILayoutUtility.GetLastRect();
            isHover = rect.Contains(Event.current.mousePosition);
        }
 
        if( dragoDropResult != AdvancedButtonResult.None )
            return dragoDropResult;
 
        if( click )
            return HandleClickCount(Time.time - lastHit);
 
        return AdvancedButtonResult.None;
    }
}
}
