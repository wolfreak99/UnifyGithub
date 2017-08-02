/*************************
 * Original url: http://wiki.unity3d.com/index.php/Flashlight
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/Flashlight.cs
 * File based on original modification date of: 8 September 2012, at 20:53. 
 *
 * // Author: Garth "Corrupted Heart" de Wet <mydeathofme[at]gmail[dot]com>
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.ReallySimpleScripts
{
    This is a super simple script that allows a flash light to be switched on and off. 
    Set Up Simply set up an input key named: Flashlight 
    Place the light you wish to use as a flashlight in the place provided within the inspector. 
    And you're done. 
    Script: FlashLight.cs ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Filename: FlashLight.cs
    // Website: www.CorruptedHeart.co.cc
    // 
    // Copyright (c) 2010 Garth "Corrupted Heart" de Wet
    // 
    // Permission is hereby granted, free of charge (a donation is welcome), to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
    using UnityEngine;
     
    public class FlashLight : MonoBehaviour
    {
    	public Light FlashLightObject;
    	private bool LightEnabled = false;
     
    	void Update ()
    	{
    		if(Input.GetButtonDown("Flashlight"))
    		{
    			LightEnabled = !LightEnabled;
    			FlashLightObject.enabled = LightEnabled;
    		}
    	}
    }Script: FlashLight.js //Translated to Javascript by William Stott reserving the guidelines stated below.
    //Added audio for when turning on or off the flashlight.
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Filename: FlashLight.cs (Now .js)
    // Author: Garth "Corrupted Heart" de Wet <mydeathofme[at]gmail[dot]com>
    // Website: www.CorruptedHeart.co.cc
    // 
    // Copyright (c) 2010 Garth "Corrupted Heart" de Wet
    // 
    // Permission is hereby granted, free of charge (a donation is welcome), to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
    var FlashLightObject : Light;
    private var LightEnabled : boolean = false;
    var lightonoroff: AudioClip;
     
    function Update ()
    {
    	if (Input.GetButtonDown("Flashlight")) {
    		LightEnabled = !LightEnabled;
    		FlashLightObject.enabled = LightEnabled;
    	}
    	if (Input.GetButtonDown("Flashlight")) {
    		audio.PlayOneShot (lightonoroff);
    	}
}
}
