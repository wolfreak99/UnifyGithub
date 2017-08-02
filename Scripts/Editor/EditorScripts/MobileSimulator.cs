/*************************
 * Original url: http://wiki.unity3d.com/index.php/MobileSimulator
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/MobileSimulator.cs
 * File based on original modification date of: 12 March 2014, at 09:29. 
 *
 * Author: Bit Barrel Media 
 *
 * Description 
 *   
 * Usage 
 *   
 * C# - StoredEditorData.cs 
 *   
 * C# - GameWindowSize.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Credits: jspease 
    DescriptionThis script undocks the Game window and sets it to a size which matches the physical size of the mobile device targeted. This way you can test for example if the size of text is not too small on HD devices. 
    Usage-Place the GameWindowSize.cs script in YourProject/Assets/Editor. 
    -Place the StoredEditorData.cs in YourProject/Assets/Scripts. 
    -Set the variable called monitorInchX in GameWindowSize.cs to the physical horizontal size of the monitor you use to view the Editor. 
    -A menu item will appear in Window\Game Window Size\. Select the mobile target device and the Game window will undock and resize to match the physical size of the mobile. 
    -Add this line to the Start() function of the script where you set the scale of your game world: 
    StoredEditorData storedEditorData = Resources.LoadAssetAtPath("Assets/storedEditorData.asset", (typeof(StoredEditorData))) as StoredEditorData;-When running in the Editor only, multiply the scale of your game world by this: 
    storedEditorData.scaleFactor;-Note that you need to set the Game window size via the menu first before you can access storedEditorData.scaleFactor 
    -If the monitor resolution is not large enough, the Game window will be clipped. 
    -mobilePPI must be set to the ppi (dpi) reported by Screen.dpi, not the actual ppi. 
    -If any changes are made to GameWindowSize.cs, the menu has to be selected again in order to update StoredEditorData. 
    C# - StoredEditorData.cs using UnityEngine;
    using System.Collections;
     
    public class StoredEditorData : ScriptableObject{
     
    	public float scaleFactor;
    	public Vector2 mobileRes;
    	public float mobilePPI;
    }C# - GameWindowSize.cs using System;
    using UnityEditor;
    using UnityEngine;
     
    public class GameWindowSize{
     
    	//Set this variable to the physical horizontal size (inch) of the monitor you use to view the Editor.
    	private static float monitorInchX = 12f;
     
    	private static Vector2 position = new Vector2(100f, 100f);
    	private static int menuOffset = 17;
     
    	private static float scaleFactor;
    	private static Vector2 mobileOnMonitorRes;
    	private static Vector2 mobileRes;
    	private static float mobilePPI;
     
    	private static class MobileDevice{
     
    		public const int IPHONE123	= 0;
    		public const int IPHONE4	= 1;
    		public const int IPHONE5	= 2;
    		public const int IPAD12		= 3;
    		public const int IPAD34		= 4;
    		public const int HTCONE		= 5;
    		public const int TF201		= 6;
    	}
     
    	private static void GetMobileData(out Vector2 mobileRes, out float mobilePPI, int mobileDevice){
     
    		switch(mobileDevice){
     
    			case MobileDevice.IPHONE123:
    				mobileRes.x = 320f;
    				mobileRes.y = 480f;
    				mobilePPI = 163f;
    			break;
     
    			case MobileDevice.IPHONE4:
    				mobileRes.x = 640f;
    				mobileRes.y = 960f;
    				mobilePPI = 320f; //320 reported, 326 official, 329 calculated
    			break;
     
    			case MobileDevice.IPHONE5:
    				mobileRes.x = 640f;
    				mobileRes.y = 1136f;
    				mobilePPI = 326f;
    			break;
     
    			case MobileDevice.IPAD12:
    				mobileRes.x = 768f;
    				mobileRes.y = 1024f;
    				mobilePPI = 132f;
    			break;
     
    			case MobileDevice.IPAD34:
    				mobileRes.x = 1536f;
    				mobileRes.y = 2048f;
    				mobilePPI = 264f;
    			break;
     
    			case MobileDevice.HTCONE:
    				mobileRes.x = 1080f;
    				mobileRes.y = 1920f;
    				mobilePPI = 468f;
    			break;
     
    			case MobileDevice.TF201:
    				mobileRes.x = 800f;
    				mobileRes.y = 1280f;
    				mobilePPI = 150f;
    			break;
     
    			default:
    				mobileRes.x = 0f;
    				mobileRes.y = 0f;
    				mobilePPI = 0f;
    			break;
    		}
    	}
     
    	private static void GetMobileOnMonitorData(out Vector2 mobileOnMonitorRes, out float scaleFactor, out Vector2 mobileRes, out float mobilePPI, int mobileDevice){
     
    		Vector2 mobileInch;
     
    		GetMobileData(out mobileRes, out mobilePPI, mobileDevice);
     
    		mobileInch.x = mobileRes.x / mobilePPI;
    		mobileOnMonitorRes.x = (mobileInch.x * Screen.currentResolution.width) / monitorInchX;
    		float aspect = mobileRes.y / mobileRes.x;
    		mobileOnMonitorRes.y = mobileOnMonitorRes.x * aspect;
    		scaleFactor = mobileOnMonitorRes.x / mobileRes.x;
    	}
     
     
    	private static void StoreEditorData(float scaleFactor, Vector2 mobileRes, float mobilePPI){
     
    		//create an instance of your StoredData class
    		StoredEditorData storedEditorData = (StoredEditorData)ScriptableObject.CreateInstance("StoredEditorData");
     
    		//create an asset file at given path
    		AssetDatabase.CreateAsset(storedEditorData, "Assets/storedEditorData.asset");
     
    		//this basically tells Unity that our scriptable object variable has changed and needs to be saved
    		EditorUtility.SetDirty(storedEditorData);
     
    		//finally assign value to stored variable
    		storedEditorData.scaleFactor = scaleFactor;
    		storedEditorData.mobileRes = mobileRes;
    		storedEditorData.mobilePPI = mobilePPI;
    	}
     
    	[MenuItem("Window/Game Window Size/iPhone 1, 2, 3 (320x480)", false, 99)]
    	public static void Size1(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.IPHONE123);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y, mobileOnMonitorRes.x, mobileOnMonitorRes.y+menuOffset); // left,top,width,height
    	}
     
    	[MenuItem("Window/Game Window Size/iPhone 4 (640x960)", false, 99)]
    	public static void Size2(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.IPHONE4);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y,mobileOnMonitorRes.x,mobileOnMonitorRes.y+menuOffset); // left,top,width,height
    	}
     
    	[MenuItem("Window/Game Window Size/iPhone 5 (640x1136)", false, 99)]
    	public static void Size3(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.IPHONE5);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y,mobileOnMonitorRes.x,mobileOnMonitorRes.y+menuOffset); // left,top,width,height
    	}
     
    	[MenuItem("Window/Game Window Size/iPad 1, 2 (768x1024)", false, 99)]
    	public static void Size4(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.IPAD12);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y,mobileOnMonitorRes.x,mobileOnMonitorRes.y+menuOffset); // left,top,width,height
    	}
     
    	[MenuItem("Window/Game Window Size/iPad 3, 4 (1536x2048)", false, 99)]
    	public static void Size5(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.IPAD34);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y,mobileOnMonitorRes.x,mobileOnMonitorRes.y+menuOffset); // left,top,width,height
    	}
     
    	[MenuItem("Window/Game Window Size/TF201 (800x1280)", false, 99)]
    	public static void Size6(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.TF201);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y,mobileOnMonitorRes.x,mobileOnMonitorRes.y+menuOffset); // left,top,width,height
    	}
     
    	[MenuItem("Window/Game Window Size/HTC One (1080x1920)", false, 99)]
    	public static void Size7(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.HTCONE);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y,mobileOnMonitorRes.x,mobileOnMonitorRes.y+menuOffset); // left,top,width,height
    	}
     
    	[MenuItem("Window/Game Window Size/Custom (640x480)", false, 99)]
    	public static void Size8(){
     
    		GetMobileOnMonitorData(out mobileOnMonitorRes, out scaleFactor, out mobileRes, out mobilePPI, MobileDevice.IPHONE4);
    		StoreEditorData(scaleFactor, mobileRes, mobilePPI);
     
    		// initialize window-specific title data and such (this can be cached)
    		WindowInfos windows = new WindowInfos();
    		windows.game.isOpen = false; //workaround for setting free aspect ratio
    		windows.game.position = new Rect(position.x,position.y, 640, 480 + menuOffset); // left,top,width,height
    	}
     
     
        class WindowInfos
        {
            // note: some of this data might need to change a little between different versions of Unity
            public WindowInfo game = new WindowInfo("UnityEditor.GameView", "Game", "Window/Game");
        }
     
        class WindowInfo
        {
            string defaultTitle;
            string menuPath;
            Type type;
     
            public WindowInfo(string typeName, string defaultTitle=null, string menuPath=null, System.Reflection.Assembly assembly=null)
            {
                this.defaultTitle = defaultTitle;
                this.menuPath = menuPath;
     
                if(assembly == null)
                    assembly = typeof(UnityEditor.EditorWindow).Assembly;
                type = assembly.GetType(typeName);
                if(type == null)
                    Debug.LogWarning("Unable to find type \"" + typeName + "\" in assembly \"" + assembly.GetName().Name + "\".\nYou might want to update the data in WindowInfos.");
            }
     
            public EditorWindow[] FindAll()
            {
                if(type == null)
                    return new EditorWindow[0];
                return (EditorWindow[])(Resources.FindObjectsOfTypeAll(type));
            }
     
            public EditorWindow FindFirst()
            {
                foreach(EditorWindow window in FindAll())
                    return window;
                return null;
            }
     
            public EditorWindow FindFirstOrCreate()
            {
                EditorWindow window = FindFirst();
                if(window != null)
                    return window;
                if(type == null)
                    return null;
                if(menuPath != null && menuPath.Length != 0)
                    EditorApplication.ExecuteMenuItem(menuPath);
                window = EditorWindow.GetWindow(type, false, defaultTitle);
                return window;
            }
     
            // shortcut for setting/getting the position and size of the first window of this type.
            // when setting the position, if the window doesn't exist it will also be created.
            public Rect position
            {
                get
                {
                    EditorWindow window = FindFirst();
                    if(window == null)
                        return new Rect(0,0,0,0);
                    return window.position;
                }
                set
                {
                    EditorWindow window = FindFirstOrCreate();
                    if(window != null)
                        window.position = value;
                }
            }
     
            // shortcut for deciding if any windows of this type are open,
            // or for opening/closing windows
            public bool isOpen
            {
                get
                {
                    return FindAll().Length != 0;
                }
                set
                {
                    if(value)
                        FindFirstOrCreate();
                    else
                        foreach(EditorWindow window in FindAll())
                            window.Close();
                }
            }
        }
}
}
