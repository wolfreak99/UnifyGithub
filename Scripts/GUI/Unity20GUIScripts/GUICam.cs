/*************************
 * Original url: http://wiki.unity3d.com/index.php/GUICam
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/GUICam.cs
 * File based on original modification date of: 20 September 2011, at 18:16. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    Here is the script I attach to a camera that holds a Rect with the cameras pixels in GUI co-ordinate space. 
    <Boo> 
    import UnityEngine 
    
    class GUICam (MonoBehaviour): 
    
      public GUIRect as Rect 
      cam as Camera 
       
      def Start(): 
         cam = transform.camera 
    
      def Update (): 
         pr=cam.pixelRect 
         r = cam.rect 
         screenHeight = pr.height / r.height 
         GUIRect = Rect(pr.left,screenHeight-(pr.top+pr.height),pr.width,pr.height) 
    </Boo>      
    
    
    to use I put this on GUI generation script which is a camera component and draw a button in the top left of the camera viewport. <Boo> 
      aGUICam as GUICam 
    
      def Start (): 
         aGUICam = transform.camera.GetComponent(GUICam) 
    
      def ButtonGUI(): 
         r=aGUICam.GUIRect 
         GUI.Button ( Rect (r.xMin+10,r.yMin+50,50, 30), "button"))
    </Boo> 
}
