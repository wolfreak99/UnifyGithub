/*************************
 * Original url: http://wiki.unity3d.com/index.php/GUI_Keyboard
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/HeadsUpDisplayScripts/GUI_Keyboard.cs
 * File based on original modification date of: 11 December 2012, at 22:53. 
 *
 * Authors 
 *   
 * Description 
 *   
 * Usage 
 *   
 * Download 
 *   
 * C# Source Code 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.HeadsUpDisplayScripts
{
    AuthorsOriginal Author : james barrett -- james 170482
    Edited: CorrodedSoul [March,03,2011] 
    DescriptionThis creates a scrolling GUI keyboard. 
    UsageProvides a very simple on screen gui virtual keyboard for inputting text. Add to any GameObject, assign the textures and GUISkin. All the output is stored in the "character" variable as a string, which can be accessed from other scripts, the "reset" button erases the content of said variable. 
    DownloadMedia:VirtualKeyboard.zip 
     
    C# Source Codeusing UnityEngine;
     
    public class Virtual_Keyboard : MonoBehaviour
    {
     
        public GUISkin MySkin;
     
        #region Scrolling Keyboard Textures
        public Texture2D Letter_A;
        public Texture2D Letter_B;
        public Texture2D Letter_C;
        public Texture2D Letter_D;
        public Texture2D Letter_E;
        public Texture2D Letter_F;
        public Texture2D Letter_G;
        public Texture2D Letter_H;
        public Texture2D Letter_I;
        public Texture2D Letter_J;
        public Texture2D Letter_K;
        public Texture2D Letter_L;
        public Texture2D Letter_M;
        public Texture2D Letter_N;
        public Texture2D Letter_O;
        public Texture2D Letter_P;
        public Texture2D Letter_Q;
        public Texture2D Letter_R;
        public Texture2D Letter_S;
        public Texture2D Letter_T;
        public Texture2D Letter_U;
        public Texture2D Letter_V;
        public Texture2D Letter_W;
        public Texture2D Letter_X;
        public Texture2D Letter_Y;
        public Texture2D Letter_Z;
        public Texture2D Number_0;
        public Texture2D Number_1;
        public Texture2D Number_2;
        public Texture2D Number_3;
        public Texture2D Number_4;
        public Texture2D Number_5;
        public Texture2D Number_6;
        public Texture2D Number_7;
        public Texture2D Number_8;
        public Texture2D Number_9;
        public Texture2D Add;
        public Texture2D Subtract;
        public Texture2D Divide;
        public Texture2D Multiply;
        public Texture2D equals;
     
        #endregion
     
        //reset button texture
        public Texture2D ResetButtonTexture;
     
        //keyboard position floats
        private float offset = 10f;
     
        //text input stuff
        public string character;   // used  as the string holder for textfield etc
     
        //KeyBoard
        private bool displayKeyBoard = true;
        private const int KEYBOARD_WINDOW_ID = 0;
        private Rect KeyBoardRect = new Rect(0, 0, 0, 0);
        private Vector2 KeyBoardSlider = Vector2.zero;
     
        //reset button
        private bool ResetString = true;
     
     
        //amount of buttons to have dependant on alphabet , numbers , symbols you want etc
        private float amtButtons = 41f;
     
        //keyboard button sizes
        private float buttonWidth = 70f;
        private float buttonHeight = 70f;
     
     
        // Use this for initialization
        void Start()
        {
     
        }
     
        // Update is called once per frame
        void Update()
        {
     
        }
     
        void OnGUI()
        {
            GUI.skin = MySkin;
     
            if (displayKeyBoard)
            {
                KeyBoardRect = GUI.Window(KEYBOARD_WINDOW_ID, new Rect(offset, Screen.height - (offset + 125f), Screen.width - (offset * 2), 125f), KeyBoardWindow, "");
            }
     
            if (ResetString)
            {
                if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, buttonWidth, buttonHeight), ResetButtonTexture))
                {
                    character = "";
                }
            }
     
        }
     
        private void KeyBoardWindow(int id)
        {
            KeyBoardSlider = GUI.BeginScrollView(new Rect(5, 15, KeyBoardRect.width - 10, 100f), KeyBoardSlider, 
                                                 new Rect(-5f, -6f, buttonWidth * amtButtons + offset * 3, buttonHeight + offset));
            {
                Populate();
            }
            GUI.EndScrollView();
        }
     
     
     
        // populate buttons with textures and desired input functions if you add or delete change amtButtons float to required number of buttons
        #region populate
        void Populate()
        {
     
            //alphabet buttons
     
     
            if (GUI.Button(new Rect(offset, 0, buttonWidth, buttonHeight), Letter_A))
            {
                character = character + "A";
            }
            if (GUI.Button(new Rect(buttonWidth * 1 + offset, 0, buttonWidth, buttonHeight), Letter_B))
            {
                character = character + "B";
            }
            if (GUI.Button(new Rect(buttonWidth * 2 + offset, 0, buttonWidth, buttonHeight), Letter_C))
            {
                character = character + "C";
            }
            if (GUI.Button(new Rect(buttonWidth * 3 + offset, 0, buttonWidth, buttonHeight), Letter_D))
            {
                character = character + "D";
            }
            if (GUI.Button(new Rect(buttonWidth * 4 + offset, 0, buttonWidth, buttonHeight), Letter_E))
            {
                character = character + "E";
            }
            if (GUI.Button(new Rect(buttonWidth * 5 + offset, 0, buttonWidth, buttonHeight), Letter_F))
            {
                character = character + "F";
            }
            if (GUI.Button(new Rect(buttonWidth * 6 + offset, 0, buttonWidth, buttonHeight), Letter_G))
            {
                character = character + "G";
            }
            if (GUI.Button(new Rect(buttonWidth * 7 + offset, 0, buttonWidth, buttonHeight), Letter_H))
            {
                character = character + "H";
            }
            if (GUI.Button(new Rect(buttonWidth * 8 + offset, 0, buttonWidth, buttonHeight), Letter_I))
            {
                character = character + "I";
            }
            if (GUI.Button(new Rect(buttonWidth * 9 + offset, 0, buttonWidth, buttonHeight), Letter_J))
            {
                character = character + "J";
            }
            if (GUI.Button(new Rect(buttonWidth * 10 + offset, 0, buttonWidth, buttonHeight), Letter_K))
            {
                character = character + "K";
            }
            if (GUI.Button(new Rect(buttonWidth * 11 + offset, 0, buttonWidth, buttonHeight), Letter_L))
            {
                character = character + "L";
            }
            if (GUI.Button(new Rect(buttonWidth * 12 + offset, 0, buttonWidth, buttonHeight), Letter_M))
            {
                character = character + "M";
            }
            if (GUI.Button(new Rect(buttonWidth * 13 + offset, 0, buttonWidth, buttonHeight), Letter_N))
            {
                character = character + "N";
            }
            if (GUI.Button(new Rect(buttonWidth * 14 + offset, 0, buttonWidth, buttonHeight), Letter_O))
            {
                character = character + "O";
            }
            if (GUI.Button(new Rect(buttonWidth * 15 + offset, 0, buttonWidth, buttonHeight), Letter_P))
            {
                character = character + "P";
            }
            if (GUI.Button(new Rect(buttonWidth * 16 + offset, 0, buttonWidth, buttonHeight), Letter_Q))
            {
                character = character + "Q";
            }
            if (GUI.Button(new Rect(buttonWidth * 17 + offset, 0, buttonWidth, buttonHeight), Letter_R))
            {
                character = character + "R";
            }
            if (GUI.Button(new Rect(buttonWidth * 18 + offset, 0, buttonWidth, buttonHeight), Letter_S))
            {
                character = character + "S";
            }
            if (GUI.Button(new Rect(buttonWidth * 19 + offset, 0, buttonWidth, buttonHeight), Letter_T))
            {
                character = character + "T";
            }
            if (GUI.Button(new Rect(buttonWidth * 20 + offset, 0, buttonWidth, buttonHeight), Letter_U))
            {
                character = character + "U";
            }
            if (GUI.Button(new Rect(buttonWidth * 21 + offset, 0, buttonWidth, buttonHeight), Letter_V))
            {
                character = character + "V";
            }
            if (GUI.Button(new Rect(buttonWidth * 22 + offset, 0, buttonWidth, buttonHeight), Letter_W))
            {
                character = character + "W";
            }
            if (GUI.Button(new Rect(buttonWidth * 23 + offset, 0, buttonWidth, buttonHeight), Letter_X))
            {
                character = character + "X";
            }
            if (GUI.Button(new Rect(buttonWidth * 24 + offset, 0, buttonWidth, buttonHeight), Letter_Y))
            {
                character = character + "Y";
            }
            if (GUI.Button(new Rect(buttonWidth * 25 + offset, 0, buttonWidth, buttonHeight), Letter_Z))
            {
                character = character + "Z";
            }
            if (GUI.Button(new Rect(buttonWidth * 26 + offset, 0, buttonWidth, buttonHeight), Number_0))
            {
                character = character + "0";
            }
            if (GUI.Button(new Rect(buttonWidth * 27 + offset, 0, buttonWidth, buttonHeight), Number_1))
            {
                character = character + "1";
            }
            if (GUI.Button(new Rect(buttonWidth * 28 + offset, 0, buttonWidth, buttonHeight), Number_2))
            {
                character = character + "2";
            }
            if (GUI.Button(new Rect(buttonWidth * 29 + offset, 0, buttonWidth, buttonHeight), Number_3))
            {
                character = character + "3";
            }
            if (GUI.Button(new Rect(buttonWidth * 30 + offset, 0, buttonWidth, buttonHeight), Number_4))
            {
                character = character + "4";
            }
            if (GUI.Button(new Rect(buttonWidth * 31 + offset, 0, buttonWidth, buttonHeight), Number_5))
            {
                character = character + "5";
            }
            if (GUI.Button(new Rect(buttonWidth * 32 + offset, 0, buttonWidth, buttonHeight), Number_6))
            {
                character = character + "6";
            }
            if (GUI.Button(new Rect(buttonWidth * 33 + offset, 0, buttonWidth, buttonHeight), Number_7))
            {
                character = character + "7";
            }
            if (GUI.Button(new Rect(buttonWidth * 34 + offset, 0, buttonWidth, buttonHeight), Number_8))
            {
                character = character + "8";
            }
            if (GUI.Button(new Rect(buttonWidth * 35 + offset, 0, buttonWidth, buttonHeight), Number_9))
            {
                character = character + "9";
            }
            if (GUI.Button(new Rect(buttonWidth * 36 + offset, 0, buttonWidth, buttonHeight), Add))
            {
                character = character + "+";
            }
            if (GUI.Button(new Rect(buttonWidth * 37 + offset, 0, buttonWidth, buttonHeight), Subtract))
            {
                character = character + "-";
            }
            if (GUI.Button(new Rect(buttonWidth * 38 + offset, 0, buttonWidth, buttonHeight), Divide))
            {
                character = character + "/";
            }
            if (GUI.Button(new Rect(buttonWidth * 39 + offset, 0, buttonWidth, buttonHeight), Multiply))
            {
                character = character + "*";
            }
            if (GUI.Button(new Rect(buttonWidth * 40 + offset, 0, buttonWidth, buttonHeight), equals))
            {
                character = character + "=";
            }
        }
     
        #endregion
    }
}
