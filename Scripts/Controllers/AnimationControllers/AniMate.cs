// Original url: http://wiki.unity3d.com/index.php/AniMate
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/AnimationControllers/AniMate.cs
// File based on original modification date of: 10 January 2012, at 20:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.AnimationControllers
{
Author: Adrian 
Contents [hide] 
1 Description 
1.1 Features 
2 Installation 
2.1 Download 
2.2 Version History 
2.2.1 v2.0 (9. October 2009) 
2.2.2 v1.7.1 (9. October 2009) 
2.2.3 v1.7 (8. October 2009) 
2.2.4 v1.6 (28. June 2009) 
2.2.5 v1.5 (24. February 2009) 
2.2.6 v1.0.1 (16. July 2008) 
2.2.7 v1 (4. July 2008) 
3 Quick Introduction 
4 Methods 
4.1 To 
4.2 From 
4.3 By 
4.4 Has 
4.5 Stop 
4.6 Cancel 
4.7 Finish 
5 Options 
5.1 Easing 
5.2 Easing Direction 
5.3 Delay 
5.4 Rigidbody 
5.5 Physics 
5.6 ColorName 
5.7 Callback 
5.8 Frames per second 
5.9 Replace animations 
6 Easing Classes 
7 Animation Drive Classes 
8 Special Cases 
8.1 Animating Rigidbodies 
8.2 Animating Rotation 
8.3 Chaining with Coroutines 
8.4 Animating Named Colors of Materials 

Description Complex animations are best done using Unity's animation system and bones. But there are times when you just need to move a block or during rapid prototyping when you don't want to bother a 3d application. Unity provides some functions for those cases like Lerp, Repeat, PingPong etc. They are fine and can be very powerful using coroutines. However, inspired by TweenLite library from Flash, I wanted a simpler approach that would allow me to dispatch an animation and not have to bother with it further, while also making my code a bit simpler. 
Features Animate any field/property that supports +/-/* mathematical operations. 
Apply easing to animation to create more pleasing transitions. 
Chain / delay animations. 
Animate rigidbodies 
Animate rotation 
Installation To install AniMate, simply import the Ani.boo script into your Standard Assets/Scripts folder. AniMate is then available from any script outside of the Standard Assets folder and from any Boo script inside it. 
Download Download AniMate here: [1] v2.0
WARNING: Version 2.0 changed easing, drive and direction class names and will very likely not work without changing your code! 
There's also an older version available which still uses the old easing/drive class names (e.g. LinearEasing, SlerpDrive): [2] v1.7.1 
Version History v2.0 (9. October 2009) Lots of class name changes: 
Easings: CircularEasing becomes Ani.Easing.Circular 
Easing directions: EasingType.In becomes Ani.Easing.In 
Drives: SlerpDrive becomes Ani.Drive.Slerp 
Improved error reporting now lets you set the message level so you can go from quiet to verbose to find out what's going on (or wrong) 
Improved delay option now takes the values from when the animation is actually started instead of from when AniMate was called 
v1.7.1 (9. October 2009) Fix a bug which kept AniMate from applying any easing to animations 
v1.7 (8. October 2009) Fix Ani.Mate not doing implicit type conversions where possible 
Add a warning when an animation is stopped because of an error 
New Cancel/Finish methods to stop animations and set them to the beginning/end of the animation 
StopAll has been deprecated in favor of Stop(object) 
v1.6 (28. June 2009) Animate named colors on materials with the colorName option 
New Easings: Back, Bounce and Elastic 
Stop animation when an object is deleted 
Work around for unused variable warning 
Few small bug fixes error reporting improvements 
v1.5 (24. February 2009) Create dynamic methods instead of using reflection for setting values (50% - 100% faster) 
New fps option to update slower if the animation doesn't need to fluent (like for particle systems) 
New replace option to remove all existing animations when creating a new one 
New Has() method to check for existing animations 
Switch to LGPL 
v1.0.1 (16. July 2008) Fixed compatibility with C# ("Cannot convert type `System.Collections.Hashtable' to `Boo.Lang.Hash'") 
v1 (4. July 2008) Initial release 
Quick Introduction Animations are created with To, From and By. All three methods take three arguments: 
The object you want to animate properties on (this for current object) 
The duration of the animation in seconds 
A Hash containing the properties to be animated as keys and the target as value. The Hash can also contain AniMate options. 
For example, to move a game object to another position: 
Ani.Mate.To(gameObject.transform, 2, {"position": new Vector3(0,20,0)});It's also possible to animate multiple properties with one call: 
Ani.Mate.To(gameObject.transform, 2, {"position": new Vector3(0,20,0), "localScale": new Vector3(2,2,2)});AniMate can also be used with C# and Boo: 
# Boo:
Ani.Mate.To(transform, 2, {"position": Vector3(0,20,0)})

// C#:
Hashtable props = new Hashtable();
props.Add("position", new Vector3(0,20,0));
Ani.Mate.To(transform, 2, props);Methods AniMate is available from any script using Ani.Mate. It provides following methods: 
To To(object :Object, duration :float, properties/options :Hashtable) :WaitForSeconds(duration) 
To(object :Object, duration :float, properties :Hashtable, options :Hashtable) :WaitForSeconds(duration) 
Create an animation from the current value to the given value. 
// Move object from it's current position to (10,0,0)
Ani.Mate.To(transform, 3, {"position": new Vector3(10,0,0)});From From(object :Object, duration :float, properties/options :Hashtable) :WaitForSeconds(duration) 
From(object :Object, duration :float, properties :Hashtable, options :Hashtable) :WaitForSeconds(duration) 
Create an animation from the given value to the current value. 
// Move object from (0,0,0) to it's current position
Ani.Mate.From(transform, 3, {"localScale": new Vector3(0,0,0)});By By(object :Object, duration :float, properties/options :Hashtable) :WaitForSeconds(duration) 
By(object :Object, duration :float, properties :Hashtable, options :Hashtable) :WaitForSeconds(duration) 
Create an animation from the current value and animate by the given value. 
// Move object by 10 units on the x axis
Ani.Mate.By(transform, 3, {"localScale": new Vector3(10,0,0)});Has Has(object :Object) :bool 
Has(object :Object, name :string) :bool 
Check if there are any animations for given object or field/proeprty. 
// Only start new animation if there isn't already one
if (!Ani.Mate.Has(transform, "position")
    Ani.Mate.To(transform, 2, {"position": Vector3.zero});Stop Stop(object :Object) 
Stop(object :Object, propertyName :string) 
Stop an animation or all animations on an object if you supply no property name. The animation will stop at whatever position it's currently at. 
Ani.Mate.Stop(transform);
Ani.Mate.Stop(transform, "position");Cancel Cancel(object :Object) 
Cancel(object :Object, name :string) 
Cancel an animation or all animations on an object if you supply no property name. The animation will be reset to its start value (note that when you use Ani.Mate.From, this will set it to the supplied value, not the original value). 
Ani.Mate.Cancel(transform);
Ani.Mate.Cancel(transform, "position");

Finish Finish(object :Object) 
Finish(object :Object, name :string) 
Finish an animation or all animations on an object if you supply no property name. The animation will be set to its end value. 
Ani.Mate.Finish(transform);
Ani.Mate.Finish(transform, "position");Options The hash that is given to To, From and By can contain AniMate options. Options can also be passed to those methods as separate hash for applying the same options to multiple animations or to avoid name conflicts. 
Easing {"easing": AnimationEasing class} 
Easing allows to make an animation non-linear. Linear is the default and means that the change will be constant over the whole animation. Different easings can be chosen to, for example, make the animation start fast and the slow down when it approaches it's target. See Easing Classes for what different kinds of easing types are available. 
Easing Direction {"direction": Ani.Easing.In / Ani.Easing.Out / Ani.Easing.InOut} 
Easing also has a direction that's either In, Out or InOut. In generally means the animation starts fast and then slows down, Out means the animation starts slow and and ends fast and InOut means the animation starts slow, is fastest at halfway and then slows down again. 
Delay {"delay": delay in seconds} 
If you don't want an animation to start immediately, you can delay the animation with this option. 
Rigidbody {"rigidbody": Rigidbody instance} 
Shorthand for animating rigidbodies. This makes sure the rigidbody is animated so that it correctly interacts with other physics objects. This option only applies to "position" and "rotation" properties. This is equivalent to {physics: true, callback: rigidbody.MoveXX}. 
Physics {"physics": true / false} 
Usually animations are processed in the Update() loop. Setting physics to true will instead process the animation in the FixedUpdate() function. This is necessary if you want to animate a rigidbody that interacts with physics. 
ColorName {"colorName": "Name of color on material"} 
It's simple to animate the main color of a material by using AniMate with material.color and the individual color properties. For named colors, it's not that straight-forward, however. AniMate provides a built-in callback for this case, which is triggered by setting the colorName option. In this case, the material has to be passed as the main object and the name of the color (as used with the GetColor() SetColor() functions) has to be supplied with colorName. It's then possible to animate the r, g, b and a properties as usual. 
Callback {"callback": callback, callable or delegate} 
Setting the callback option to a function (callable, delegate or what you want to call it) will not directly apply the animated value to the object but instead pass it to the callback function. This can be used together with RigidbodyMover and RigidbodyRotator to pass the position/rotation of a rigidbody through MovePosition() and MovePosition() to improve interaction of the rigidbody with other objects. 
Frames per second {"fps": float} 
By default, AniMate updates the animation each frame (or physics frame) but it's not always necessary to update that often. If you animate the maxEmission property of a particle emitter, it won't be visible if it's only animated with 10fps or even less. Note that AniMate can only approximate the fps and it will not update more often than the actual fps. 
Replace animations {"replace": true / false} 
Remove all existing animations on this object for each field/property before creating a new one. Concurrent opposing animation can lead to strange results, especially with rigidbodies. 
Easing Classes AniMate bundles following easing classes. You can roll your own by extending the AnimationEasing interface. 
Ani.Easing.Linear 
Ani.Easing.Quadratic 
Ani.Easing.Cubic 
Ani.Easing.Quartic 
Ani.Easing.Quintic 
Ani.Easing.Sinusoidal 
Ani.Easing.Exponential 
Ani.Easing.Circular 
Ani.Easing.Back 
Ani.Easing.Bounce 
Ani.Easing.Elastic 
You can see a demonstration of those easing functions here: [3] or here: [4] 
Default is Linear and any other easing is chosen by adding "easing" to the options: 
Ani.Mate.To(transform, 3, {"position": Vector3(10,10,10), "easing": Ani.Easing.Quartic});Animation Drive Classes An animation drive calculates the current value of an animated object. The default is Regular and calculates the current value like (startValue + currentTime * change). 
Ani.Drive.Regular 
Ani.Drive.Slerp (for Quaternion) 
Ani.Drive.Lerp (for Vector3) 
Slerp only works with Quaternions and can be used to animate rotation so that the object rotates the shortest distance possible. 
Ani.Mate.To(transform, 3, {"rotation": Quaternion.Euler(0,0,180), "drive": Ani.Drive.Slerp});Special Cases Animating Rigidbodies To correctly animate rigidbodies, the animation has to be done in FixedUpdate and the position/rotation applied through MovePosition and MoveRotation. This can be done with AniMate using the "physics" (to use FixedUpdate) and "callback" (to call the Move* methods). 
Ani.Mate.To(transform, 3, {"position": Vector3(10,10,10), "physics": true, "callback": rigidbody.MovePosition});There's also a shorthand to make this call simpler: 
Ani.Mate.To(transform, 3, {"position": Vector3(10,10,10), "rigidbody": rigidbody});For this to work correctly, isKinematic should be enabled on the rigidbody. 
Animating Rotation There are two possible ways to animate rotation. Either by using By and eulerAngles to create a rotation without target or by using rotation and Slerp to create a rotation from two positions that will rotate with as little torque as possible. 
// Make two full circles around the y-axis:
Ani.Mate.By(transform, 3, {"eulerAngles": new Vector3(0,720,0)});
// Rotate to a value:
Ani.Mate.To(transform, 3, {"rotation": Quaternion.Euler(30,90,20), "drive": Ani.Drive.Slerp});Chaining with Coroutines To, From and By all conveniently return a WaitForSeconds objects with the duration of the animation. This allows to use Coroutines to create a sequence of successive animations. 
// Make an object move back an forth
function AnimationCoroutine() {
    while true {
        yield Ani.Mate.By(transform, 5, {"position": new Vector3(0,20,0)});
        yield Ani.Mate.By(transform, 5, {"position": new Vector3(0,-20,0)});
    }
}Animating Named Colors of Materials Animating named colors of materials, which are accessed with the GetColor and SetColor methods, is not straight-forward since GetColor returns a copy of the color and not the color itself. It's therefore necessary to use a callback that feeds the color back to SetColor. 
For convenience AniMate provides a built-in callback for this case. While the main color of a material has to be animated like this: 
// Pass the color object directly and make the object disappear (requires a transparent shader)
Ani.Mate.To(renderer.material.color, 5, {"a": 0.0});A named color has to be animated like this: 
// Pass the material object instead of the color object and specify the color name with the
// colorName option. Turns the material's specular color to red.
Ani.Mate.To(renderer.material, 5, {"colorName": "_SpecColor", "r": 1.0, "g": 0.0, "b": 0.0});
}
