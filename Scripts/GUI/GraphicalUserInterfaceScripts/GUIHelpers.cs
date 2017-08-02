/*************************
 * Original url: http://wiki.unity3d.com/index.php/GUIHelpers
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/GUIHelpers.cs
 * File based on original modification date of: 24 November 2013, at 03:08. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    GUIHELPERSThis is a simple static class that returns you a rect aligned to the screen or to an input rect, according to the parameters you use. 
    History Update by Berenger, 27/04/2012 
    Changed input rect into width and height, added possibility to align to a parent rect and to use offsets. Added mouse utility functions. 
    (28/04/2012) More function for rects, using .net extensions. 
    Update by Bunny93, 19.01.2013: Added some extension functions for the Event and Touch class. 
    C# - GUIHelpers.csusing UnityEngine;
     
    //simple static class to do some basic GUI stuff.
    public static class GUIHelpers 
    {
            //AlignRect takes a width and a height returned a rect snapped to the edge of the screen as defined by the alignment
            //If you want a different rect than the screen, use the parameter parentRect
            //You can adjust the result position with xOffset and yOffset
            //usage:
            //winRect = GUIHelpers.AlignRect(32f, 32f, GUIHelpers.ALIGN.TOPCENTER);
            //winRect = GUIHelpers.AlignRect(32f, 32f, GUIHelpers.ALIGN.TOPCENTER, 5f, -10f);
            //winRect = GUIHelpers.AlignRect(32f, 32f, inRect, GUIHelpers.ALIGN.TOPCENTER);
            //winRect = GUIHelpers.AlignRect(32f, 32f, inRect, GUIHelpers.ALIGN.TOPCENTER, 5f, -10f);
            public enum Alignment {
                TOPLEFT,TOPCENTER,TOPRIGHT,RIGHT,BOTTOMRIGHT,BOTTOMCENTER,BOTTOMLEFT,LEFT,CENTER
            }
     
            public static Rect AlignRect(float width, float height, Alignment alignment)
            { return AlignRect(width, height, new Rect(0, 0, Screen.width, Screen.height), alignment, 0f, 0f); }
            public static Rect AlignRect(float width, float height, Alignment alignment, float xOffset, float yOffset)
            { return AlignRect(width, height, new Rect(0, 0, Screen.width, Screen.height), alignment, xOffset, yOffset); }
     
            public static Rect AlignRect(float width, float height, Rect parentRect, Alignment alignment)
            { return AlignRect(width, height, parentRect, alignment, 0f, 0f); }
            public static Rect AlignRect(float width, float height, Rect parentRect, Alignment alignment, float xOffset, float yOffset)
            {
                Rect oRect;
     
                switch (alignment)
                {
                    case Alignment.TOPLEFT:
                        oRect = new Rect(0f, 0f, width, height);
                        break;
                    case Alignment.TOPRIGHT:
                        oRect = new Rect( parentRect.width - width, 0f, width, height);
                        break;
                    case Alignment.TOPCENTER:
                        oRect = new Rect( parentRect.width * 0.5F - width * 0.5F, 0f, width, height);
                        break;
                    case Alignment.CENTER:
                        oRect = new Rect( parentRect.width * 0.5F - width * 0.5F, parentRect.height * 0.5F - height * 0.5F, width, height);
                        break;
                    case Alignment.RIGHT:
                        oRect = new Rect( parentRect.width - width, parentRect.height * 0.5F - height * 0.5F, width, height);
                        break;
                    case Alignment.BOTTOMRIGHT:
                        oRect = new Rect( parentRect.width - width, parentRect.height - height, width, height);
                        break;
                    case Alignment.BOTTOMCENTER:
                        oRect = new Rect( parentRect.width * 0.5F - width * 0.5F,  parentRect.height- height, width, height);
                        break;
                    case Alignment.BOTTOMLEFT:
                        oRect = new Rect( 0f, parentRect.y + parentRect.height - height, width, height);
                        break;
                    case Alignment.LEFT:
                        oRect = new Rect( 0f, parentRect.height * 0.5F - height * 0.5F, width, height);
                        break;
                    default:
                        oRect = new Rect( 0f, 0f, width, height );
                        break;
                }
     
                oRect.x += parentRect.x + xOffset;
                oRect.y += parentRect.y + yOffset;
                return oRect;
            }
     
    // Rect extensions
            // Reduce the size of the rect from the center
            public static Rect Shrink(this Rect r, float nbPixels) { return r.Shrink(nbPixels, nbPixels); }
            public static Rect Shrink(this Rect r, float nbPixelX, float nbPixelY)
            {
                return new Rect(r.x + nbPixelX, r.y + nbPixelY, r.width - nbPixelX * 2f, r.height - nbPixelY * 2f);
            }
     
            // Enhance the size of the rect from the center
            public static Rect Grow(this Rect r, float nbPixels) { return r.Shrink(-nbPixels, -nbPixels); }
            public static Rect Grow(this Rect r, float nbPixelX, float nbPixelY) { return r.Shrink(-nbPixelX, -nbPixelY); }
     
            // Make sure the rect is contained inside another rect. It's size isn't changed.
            public static Rect ClampPosition(this Rect r, Rect borderRect)
            {
                return new Rect(Mathf.Clamp(r.x, borderRect.x, borderRect.x + borderRect.width  - r.width),
                                Mathf.Clamp(r.y, borderRect.y, borderRect.y + borderRect.height - r.height),
                                r.width, r.height);
            }
     
            // Translate the rect
            public static Rect MoveX(this Rect r, float xMovement) { return r.Move(xMovement, 0f); }
            public static Rect MoveY(this Rect r, float yMovement) { return r.Move(0f, yMovement); }
            public static Rect Move(this Rect r, Vector2 movement) { return r.Move(movement.x, movement.y); }
            public static Rect Move(this Rect r, float xMovement, float yMovement) 
            {
                return new Rect(r.x + xMovement, r.y + yMovement, r.width, r.height);
            }
     
            // Return the rect relatively to another one, like a groupe would do.
            public static Rect RelativeTo(this Rect r, Rect to)
            {
                return new Rect(r.x - to.x, r.y - to.y, r.width, r.height);
            }
     
            // Return the rect as one level of group higher.
            public static Rect InverseTransform(this Rect r, Rect from)
            {
                return new Rect(r.x + from.x, r.y + from.y, r.width, r.height);
            }
     
            // Quick access to the screen rect
            public static Rect screenRect { get { return new Rect(0f, 0f, Screen.width, Screen.height); } }
     
     
            // MOUSE FUNCTIONS
            // Easy access to the gui mouse pos
            public static Vector2 mousePos { get { return Event.current.mousePosition; } }
            // gui mouse pos with the y inverted
            public static Vector2 mousePosInvertY { get { return FlipY(mousePos); } }
            public static Vector2 FlipY(Vector2 inPos) { inPos.y = Screen.height - inPos.y; return inPos; }
            public static Vector2 MouseRelativePos(Rect rect) { return RelativePos(rect, mousePos.x, mousePos.y); }
            // Give the inPos relative to the input rect
            public static Vector2 RelativePos(Rect rect, Vector2 inPos) { return RelativePos(rect, inPos.x, inPos.y); }
            public static Vector2 RelativePos(Rect rect, Vector3 inPos) { return RelativePos(rect, inPos.x, inPos.y); }
            public static Vector2 RelativePos(Rect rect, float x, float y) 
            {
                return new Vector2(x - rect.x, y - rect.y);
            }
            // Give the inPos as one group higher
            public static Vector2 InverseTransformPoint(Rect rect, Vector3 inPos)
            {
                return new Vector2(rect.x + inPos.x, rect.y + inPos.y);
            }
     
     
    // Touch extensions
            // returns the given touch in GUI coordinates
            public static Vector2 GetGUIPosition(this Touch aTouch)
            {
                var tmp = aTouch.position;
                tmp.y = Screen.height - tmp.y;
                return tmp;
            }
     
            // returns the true touch delta in pixels
            public static Vector2 FixedTouchDelta(this Touch aTouch)
            {
                float dt = Time.deltaTime / aTouch.deltaTime;
                if (dt == 0 || float.IsNaN(dt) || float.IsInfinity(dt))
                    dt = 1.0f;
                return aTouch.deltaPosition * dt;
            }
     
    // GUI Event extensions
            // Checks for Key Down / Up event of a certain key
            public static bool GetKeyDown(this Event aEvent, KeyCode aKey)
            {
                return (aEvent.type == EventType.KeyDown && aEvent.keyCode == aKey);
            }
            public static bool GetKeyUp(this Event aEvent, KeyCode aKey)
            {
                return (aEvent.type == EventType.KeyUp && aEvent.keyCode == aKey);
            }
     
            // Checks for mouse Down / Up event of a certain mouse button
            public static bool GetMouseDown(this Event aEvent, int aButton)
            {
                return (aEvent.type == EventType.MouseDown && aEvent.button == aButton);
            }
            public static bool GetMouseUp(this Event aEvent, int aButton)
            {
                return (aEvent.type == EventType.MouseUp && aEvent.button == aButton);
            }
     
            // Checks for mouse Down / Up event of a certain mouse button inside the given Rect
            public static bool GetMouseDown(this Event aEvent, int aButton, Rect aRect)
            {
                return (aEvent.type == EventType.MouseDown && aEvent.button == aButton && aRect.Contains(aEvent.mousePosition));
            }
            public static bool GetMouseUp(this Event aEvent, int aButton, Rect aRect)
            {
                return (aEvent.type == EventType.MouseUp && aEvent.button == aButton && aRect.Contains(aEvent.mousePosition));
            }
     
     
    }
}
