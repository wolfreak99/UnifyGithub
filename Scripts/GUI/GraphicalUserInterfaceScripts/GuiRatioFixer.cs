/*************************
 * Original url: http://wiki.unity3d.com/index.php/GuiRatioFixer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/GuiRatioFixer.cs
 * File based on original modification date of: 10 January 2012, at 20:57. 
 *
 * Author: Jonathan Czeck (aarku) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 C# - GuiRatioFixer.cs 
    4 Boo - GuiRatioFixer.boo 
    5 JavaScript - GuiRatioFixer.js 
    
    DescriptionThis script will adjust the aspect ratio of a GUIText or GUITexture object by scaling it horizontally to fit the proportions. 
    It does not know how to align groups of GUI objects after it adjusts their aspect ratio. 
    UsagePlace this script on a GameObject with a GUIText and/or GUITexture component. Then scale the object to the correct screen size in a Game View that has aspect set to 4:3. When you run the game, the object's scale will be changed to match the actual screen ratio. 
    If you prefer working in a different aspect ratio, adjust the Native Ratio value to your native aspect ratio desired. This is horizontal / vertical. 
    C# - GuiRatioFixer.csusing UnityEngine;
    using System.Collections;
     
    // Use this on a guiText or guiTexture object to automatically have them
    // adjust their aspect ratio when the game starts.
     
    public class GuiRatioFixer : MonoBehaviour
    {
        public float m_NativeRatio = 1.3333333333333F;
     
        void Start ()
        {
            float currentRatio = (float)Screen.width / (float)Screen.height;
            Vector3 scale = transform.localScale;
            scale.x *= m_NativeRatio / currentRatio;
            transform.localScale = scale;
        }
     
    }Boo - GuiRatioFixer.booimport UnityEngine
     
    # Use this on a guiText or guiTexture object to automatically have them
    # adjust their aspect ratio when the game starts.
     
    class GuiRatioFixer (MonoBehaviour):
        public m_NativeRatio = 1.3333333333333
     
        def Start ():
            currentRatio = (Screen.width+0.0) / Screen.height
            transform.localScale.x *= m_NativeRatio / currentRatioeJavaScript - GuiRatioFixer.js// Use this on a guiText or guiTexture object to automatically have them
    // adjust their aspect ratio when the game starts.
    var m_NativeRatio = 1.3333333333333;
     
    currentRatio = (Screen.width+0.0) / Screen.height;
transform.localScale.x *= m_NativeRatio / currentRatio;
}
