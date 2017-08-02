// UNITYSCRIPT
/*************************
 * Original url: http://wiki.unity3d.com/index.php/CoUpdate
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/CoUpdate.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Lasse Järvensivu (Statement) 
 *
 * Summary 
 *   Provides an alternative to Update method that support yield instructions. 
 * Problem 
 *   The Update method in Unity3D can't be a Coroutine. In some cases it can be beneficial to allow yielding 
 *   in Update to allow step-wise logic to be executed. For example waiting for a key stroke and then play an 
 *   animation that must finish before remaining code is executed. 
 * Solution 
 *   In Start method, start a new Coroutine called CoStart which handles an internal loop. This internal loop 
 *   in turn call upon the CoUpdate Coroutine which then can be treated as Update but with added yield 
 *   instruction support. 
 *   
 * Example usage (JS) 
 *   
 * This file has only been partially formatted, feel free to contribute!
 *
 *************************/

#if USES_JAVASCRIPT

#pragma strict
     
    function Start()
    {
        StartCoroutine("CoStart");
    }
     
    function CoStart() : IEnumerator
    {
        while (true)
            yield CoUpdate();
    }
     
    function CoUpdate() : IEnumerator
    {
        // Place your update code here.
    }
    
    // Example usage (JS) 
    /*
    Let's assume the following example: 
    You want to create a basic player controller that allow movement and has the capability of performing a
    taunt animation. While the taunt is playing, movement should be disabled. 
    Traditionally one could either add a boolean flag that stop movement code from being run while it is set
    for the duration of the animation, or stop movement code if an animation is playing. This can quickly
    lead to hard to maintain code. In this scenario we haven't done nothing yet to stop the taunt animation
    from being triggered again before it completes. We could add another if-test to see if the flag is set or
    if the animation is playing. However using Coroutines we can eliminate much of the boiler plate state code. 
    Let's try this new approach with a CoUpdate and see what we can do with it. We could make the taunt a coroutine
    to block move code execution while it is playing. We don't need to take special care to check if an animation is
    playing or not, since no other code will run while it is playing our taunt. 
    The following script uses arrow keys for movement input and the G key to trigger the taunt animation. 
    */
    // CoUpdate usage example.
    // Assumes an Animation is used.
     
    #pragma strict
     
    @script RequireComponent(Animation)
     
    function Start()
    {
        StartCoroutine("CoStart");
    }
     
    function CoStart() : IEnumerator
    {
        while (true)
            yield CoUpdate();
    }
     
    function CoUpdate() : IEnumerator
    {
        // Notice how easily we can taunt 
        // and render movement disabled
        // for the duration since Taunt is
        // a Coroutine.
     
        if (Input.GetKey(KeyCode.G))
            yield Taunt();
     
        Move();
    }
     
    function Taunt() : IEnumerator
    {
        animation.Play(animation.clip.name);
        yield WaitForSeconds(animation.clip.length);
    }
     
    function Move()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            Move(Vector3.left);
        if (Input.GetKey(KeyCode.RightArrow))
            Move(Vector3.right);
        if (Input.GetKey(KeyCode.UpArrow))
            Move(Vector3.forward);
        if (Input.GetKey(KeyCode.DownArrow))
            Move(Vector3.back);
    }
     
    function Move(direction : Vector3)
    {
        transform.Translate(direction * Time.deltaTime, Space.Self);
    }

#endif