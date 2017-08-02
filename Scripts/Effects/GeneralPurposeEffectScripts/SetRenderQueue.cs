/*************************
 * Original url: http://wiki.unity3d.com/index.php/SetRenderQueue
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/SetRenderQueue.cs
 * File based on original modification date of: 26 March 2016, at 01:06. 
 *
 * Author: Alex Schwartz (GTJuggler) and Daniel Brauer 
 *
 * Description 
 *   
 * C# (By Daniel Brauer) 
 *   
 * License 
 *   
 * JS (By Alex Schwartz) 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    DescriptionAllows you to set the render order for objects to avoid transparent sort issues. JS version has been extended to allow an option to affect all children of a GameObject but removes the option to affect materials other than sharedMaterial, as that was not necessary in my project. Feel free to modify. 
    Notes 
    The default transparent queue is 3000. 
    Lower queues will be rendered first, higher ones later. 
    This change will affect the shared material, so you have to make separate materials for separate render queues. 
    In the editor, this change will persist across runs. However, the scripts still need to be attached to the correct objects in a build in order to work there. 
    C# (By Daniel Brauer)using UnityEngine;
     
    [AddComponentMenu("Effects/SetRenderQueue")]
    [RequireComponent(typeof(Renderer))]
     
    public class SetRenderQueue : MonoBehaviour {
       [Tooltip("Background=1000, Geometry=2000, AlphaTest=2450, Transparent=3000, Overlay=4000")]
       public int queue = 1;
     
       public int[] queues;
     
       void Start() {
          Renderer renderer = GetComponent<Renderer>();
          if (!renderer || !renderer.sharedMaterial || queues == null)
             return;
          renderer.sharedMaterial.renderQueue = queue;
          for (int i = 0; i < queues.Length && i < renderer.sharedMaterials.Length; i++)
             renderer.sharedMaterials[i].renderQueue = queues[i];
       }
     
       void OnValidate() {
          Start();
       }
     
    }License The MIT License (MIT) 
    Copyright (c) 2009 Daniel Brauer 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
    JS (By Alex Schwartz)var queue : int = 3000; //3000 is default in Unity
    var applyToChildren : boolean = false;
     
    function Awake(){
    	SetRenderQueue();
    }
     
    function SetRenderQueue(){
    	if (!renderer || !renderer.sharedMaterial || applyToChildren){
    		if(applyToChildren){
    			for(var child : Transform in transform){
    				child.renderer.sharedMaterial.renderQueue = queue;
    			}
    		} else { 
    			print("No renderer found on this GameObject. Check the applyToChildren box to apply settings to children"); 
    		} 	 
    	} else {renderer.sharedMaterial.renderQueue = queue;}
}
}
