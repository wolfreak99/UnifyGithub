/*************************
 * Original url: http://wiki.unity3d.com/index.php/WrapTextMesh
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/WrapTextMesh.cs
 * File based on original modification date of: 6 September 2013, at 17:05. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    Description Ever wanted to use a TextMesh with text that just fit in a certain width but your font didn't have uniform widths, so using character counts was pointless? 
    This solution uses actual kerning data from the imported font to determine the most appropriate place to break the line. 
    
    Important Note: This will not work with dynamic fonts. 
    Usage There are 2 ways you can use this script in it's current form: 
    1. Apply it to a TextMesh and use the WrapOnStart feature that wraps the text that's already on the mesh as soon as the scene is initialized. 
    2. If new text has been dynamically assigned to the TextMesh and wrapping needs to be done on a fresh string then just call the Wrap() function directly to the desired width: 
    textObj.GetComponent<WrapOnStart>().Wrap(fWrapWidth);Code using UnityEngine;
     
    public class WrapTextMesh : MonoBehaviour
    {
        public bool WrapOnStart;
        public float WrapWidth;
     
        public void Wrap(float MaxWidth)
        {
            TextMesh tm = GetComponent<TextMesh>();
            if (tm == null)
            {
                Debug.LogError("TextMesh component not found.");
                return;
            }
     
            Font f = tm.font;
            string str = tm.text;
            int nLastWordInd = 0;
            int nIter = 0;
            float fCurWidth = 0.0f;
            float fCurWordWidth = 0.0f;
            while (nIter < str.Length)
            {
                // get char info
                char c = str[nIter];
                CharacterInfo charInfo;
                if (!f.GetCharacterInfo(c, out charInfo))
                {
                    Debug.LogError("Unrecognized character encountered (" + (int)c + "): " + c);
                    return;
                }
     
                if (c == '\n')
                {
                    nLastWordInd = nIter;
                    fCurWidth = 0.0f;
                }
                else
                {
                    if (c == ' ')
                    {
                        nLastWordInd = nIter; // replace this character with '/n' if breaking here
                        fCurWordWidth = 0.0f;
                    }
     
                    fCurWidth += charInfo.width;
                    fCurWordWidth += charInfo.width;
                    if (fCurWidth >= MaxWidth)
                    {
                        str = str.Remove(nLastWordInd, 1);
                        str = str.Insert(nLastWordInd, "\n");
                        fCurWidth = fCurWordWidth;
                    }
                }
     
                ++nIter;
            }
     
            tm.text = str;
        }
     
        // Use this for initialization
        void Start()
        {
            if (WrapOnStart)
                Wrap(WrapWidth);
        }
    }
}
