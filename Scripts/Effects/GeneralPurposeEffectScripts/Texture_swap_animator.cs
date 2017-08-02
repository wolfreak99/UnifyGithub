/*************************
 * Original url: http://wiki.unity3d.com/index.php/Texture_swap_animator
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Texture_swap_animator.cs
 * File based on original modification date of: 14 December 2014, at 21:57. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 TextureSwapAnimator.js 
    4 TextureSwapAnimator2.js 
    
    DescriptionThis script animates a texture on an object by using a series of frames. You can give it a framerate to determine the speed of the animation. 
    Usage Attach this script to an object, then set the size of the Frames array and drag all the frames of the animation onto the appropriate slots. (Note that you can click the lock icon in the inspector and drag all the frames at once, rather than one at a time.) Set FramesPerSecond to the desired framerate. 
    TextureSwapAnimator.js var frames : Texture2D[];
    var framesPerSecond = 10.0;
     
    function Update () {
    	var index : int = Time.time * framesPerSecond;
    	index = index % frames.Length;
    	GetComponent(Renderer).material.mainTexture = frames[index];
    }Here's another version, which uses sprites instead of Texture2D, and therefore requires Unity 4.3 or later. Also, it doesn't use Time.time, which eventually runs out of precision if you leave it going long enough. So this version can be left running forever (or until the power is cut) without any problems. 
    TextureSwapAnimator2.js var frames : Sprite[];
    var framesPerSecond = 10.0;
    private var timer = 0.0;
     
    function Update () {
    	timer = (timer + Time.deltaTime*framesPerSecond) % frames.Length;
    	GetComponent(SpriteRenderer).sprite = frames[timer];
}
}
