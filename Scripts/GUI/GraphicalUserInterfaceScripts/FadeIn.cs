/*************************
 * Original url: http://wiki.unity3d.com/index.php/FadeIn
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/FadeIn.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Alejandro Gonzalez (zerofractal) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
    DescriptionThis is a script that allows you to show/hide a GUIText or GUITexture object when pressing a button. 
    It smoothly fades in and out the map based on the fade duration determined by the user. The default button is set to "Jump" You can easily edit the name of the button from the editor, just make sure you have created a button axis with that name. 
    The script also allows the image to stay visible for some time at the beginning of the level. If you want it to start invisible, simply edit the object's alpha value to 0 in the color setting. 
    UsagePlace this script on a GameObject with a GUIText and/or GUITexture component. Choose the appropriate fade in speed and button name. 
    JavaScript - FadeIn.js//FadeIn Script for Unity - Zerofractal 2006 
    var buttonName="Jump";
    var fadeDuration:float=0.5;
    var initialDelay:float=5;
    private var timeLeft:float=0.5;
     
    function Awake () {
       timeLeft = fadeDuration;
    }
     
    function Update () {
       if (initialDelay > 0){
          initialDelay = initialDelay-Time.deltaTime;
       } else {
          if (Input.GetButton (buttonName))
             fade(true);   
          else
             fade(false);
       }
    }
     
    function fade(direction:boolean){
       var alpha;
       if (direction){
          if (guiElement.color.a < 0.5){
             timeLeft = timeLeft - Time.deltaTime;
             alpha = (timeLeft/fadeDuration);
             guiElement.color.a=0.5-(alpha/2);
          } else {
             timeLeft = fadeDuration;
          }
       } else {
          if (guiElement.color.a > 0){
             timeLeft = timeLeft - Time.deltaTime;
             alpha = (timeLeft/fadeDuration);
             guiElement.color.a=alpha/2;
          } else {
             timeLeft = fadeDuration;
          }
       }
}
}
