/*************************
 * Original url: http://wiki.unity3d.com/index.php/CoroutineScheduler
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/CoroutineScheduler.cs
 * File based on original modification date of: 17 December 2016, at 03:55. 
 *
 * Author: Fernando Zapata (fernando@cpudreams.com) ported to CSharp by Frank Otto (http://fosion.de) Additional implementation for yield commands from the UnityEngine namespace by Gareth Williams (http://lonewolfwilliams.com) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.GeneralConcepts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Code 
    3.1 CoroutineScheduler.js 
    3.2 CoroutineNode.js 
    3.3 CoroutineSchedulerTest.cs 
    3.4 CoroutineScheduler.cs 
    3.5 CoroutineNode.cs 
    4 Additional Implementation C# 
    4.1 Sample Usage 
    4.2 Addition to CoroutineScheduler.cs 
    4.3 Addition to CoroutineNode.cs 
    4.4 IYieldWrapper.cs 
    4.5 Example Wrappers 
    4.5.1 UnityASyncOpWrapper 
    4.5.2 UnityWWWYieldWrapper 
    
    Description A simple coroutine scheduler. This coroutine scheduler allows for complete control over the execution regime of a set of coroutines. Reading the code will also help you understand how coroutines work behind the scenes. Understanding how coroutines are built on top of .Net generators will allow you to add coroutine support to a non-Unity project. 
    Coroutines can yield until the next update "yield;", until a given number of updates have passed "yield anInt;", until a given amount of seconds have passed "yield aFloat;", or until another coroutine has finished "yield scheduler.StartCoroutine(Coroutine());". 
    Multiple scheduler instances are supported and can be very useful. A coroutine running under one scheduler can yield (wait) for a coroutine running under a completely different scheduler instance. 
    Unity's YieldInstruction classes are not used because I cannot access their internal data needed for scheduling. Semantics are slightly different from Unity's scheduler. For example, in Unity if you start a coroutine it will run up to its first yield immediately, while in this scheduler it will not run until the next time UpdateAllCoroutines is called. This feature allows any code to start coroutines at any time, while making sure the started coroutines only run at specific times. 
    You should not depend on update order between coroutines running on the same update. 
    For a deeper understanding and to learn more about how coroutines are implemented as state machines by the compiler see the article quoted in this post. 
    Here's a link to a cached version of the original blog post which is way easier to read. 
    Usage Creating and driving CoroutineSchedulers is easy and is best shown by an example. 
    var scheduler = new CoroutineScheduler();
     
    function Update() {
      scheduler.UpdateAllCoroutines(Time.frameCount, Time.time);
    }You normally would not use Unity's own Time.frameCount or Time.time as in that case you may as well use Unity's built-in scheduler, but it makes for an easy to understand example. 
    Starting coroutines and yielding execution is also best shown by an example. 
    scheduler.StartCoroutine(MyCoroutine());
     
    function MyCoroutine() : IEnumerator {
      print("MyCoroutine: Begin");
      yield; // wait for next update
      print("MyCoroutine: next update;");
      yield 2; // wait for 2 updates, same as yield; yield;
      print("MyCoroutine: After yield 2;");
      yield 3.5; // wait for 3.5 seconds
      print("MyCoroutine: After 3.5 seconds;");
      // you can also yield for a coroutine running on a completely different scheduler instance
      yield scheduler.StartCoroutine(WaitForMe());
      print("MyCoroutine: After WaitForMe() finished;");
    }
     
    function WaitForMe() {
      yield 7.8; // wait for 7.8 seconds before finishing
    }For details on features not used in this particular example please read the doc comments. If you have any questions feel free to contact me directly at fernando@cpudreams.com. 
    Code CoroutineScheduler.js #pragma strict
     
    /**
     * A simple coroutine scheduler. Coroutines can yield until the next update
     * "yield;", until a given number of updates "yield anInt", until a given
     * amount of seconds "yield aFloat;", or until another coroutine has finished
     * "yield scheduler.StartCoroutine(Coroutine())".
     *
     * Multiple scheduler instances are supported and can be very useful. A
     * coroutine running under one scheduler can yield (wait) for a coroutine
     * running under a completely different scheduler instance.
     *
     * Unity's YieldInstruction classes are not used because I cannot
     * access their internal data needed for scheduling. Semantics are slightly
     * different from Unity's scheduler. For example, in Unity if you start a
     * coroutine it will run up to its first yield immediately, while in this
     * scheduler it will not run until the next time UpdateAllCoroutines is called.
     * This feature allows any code to start coroutines at any time, while
     * making sure the started coroutines only run at specific times.
     *
     * You should not depend on update order between coroutines running on the same
     * update. For example, StartCoroutine(A), StartCoroutine(B), StartCoroutine(C)
     * where A, B, C => while(true) { print(A|B|C); yield; }, do not expect "ABC" or
     * "CBA" or any other specific ordering.
     */
    class CoroutineScheduler {
      /**
       * The first node in list of the coroutines that are scheduled for execution.
       */
      var first : CoroutineNode = null;
      var currentFrame : int;
      var currentTime : float;
     
      /**
       * Starts a coroutine, the coroutine does not run immediately but on the
       * next call to UpdateAllCoroutines. The execution of a coroutine can
       * be paused at any point using the yield statement. The yield return value
       * specifies when the coroutine is resumed.
       */
      function StartCoroutine(fiber : IEnumerator) : CoroutineNode {
        // if function does not have a yield, fiber will be null and we no-op
        if (fiber == null) { return; }
        // create coroutine node and run until we reach first yield
        var coroutine = new CoroutineNode(fiber);
        AddCoroutine(coroutine);
        return coroutine;
      }
     
      /**
       * Stops all coroutines running on this behaviour. Use of this method is
       * discouraged, think of a natural way for your coroutines to finish
       * on their own instead of being forcefully stopped before they finish.
       * If you need finer control over stopping coroutines you can use multiple
       * schedulers.
       */
      function StopAllCoroutines() {
        first = null;
      }
     
      /**
       * Returns true if this scheduler has any coroutines. You can use this to
       * check if all coroutines have finished or been stopped.
       */
      function HasCoroutines() : boolean {
        return first != null;
      }
     
      /**
       * Runs all active coroutines until their next yield. Caller must provide
       * the current frame and time. This allows for schedulers to run under
       * frame and time regimes other than the Unity's main game loop.
       */
      function UpdateAllCoroutines(frame : int, time : float) {
        currentFrame = frame;
        currentTime = time;
        var coroutine = first;
        while (coroutine != null) {
          // store listNext before coroutine finishes and is removed from the list
          var listNext = coroutine.listNext;
          if (coroutine.waitForFrame > 0 && frame >= coroutine.waitForFrame) {
            coroutine.waitForFrame = -1;
            UpdateCoroutine(coroutine);
          } else if (coroutine.waitForTime > 0.0 && time >= coroutine.waitForTime) {
            coroutine.waitForTime = -1.0;
            UpdateCoroutine(coroutine);
          } else if (coroutine.waitForCoroutine &&
                     coroutine.waitForCoroutine.finished) {
            coroutine.waitForCoroutine = null;
            UpdateCoroutine(coroutine);
          } else if (coroutine.waitForFrame == -1 &&
                     coroutine.waitForTime == -1.0 &&
                     coroutine.waitForCoroutine == null) { // initial update
            UpdateCoroutine(coroutine);
          }
          coroutine = listNext;
        }
      }
     
      /**
       * Executes coroutine until next yield. If coroutine has finished, flags
       * it as finished and removes it from scheduler list.
       */
      private function UpdateCoroutine(coroutine : CoroutineNode) {
        var fiber = coroutine.fiber;
        if (coroutine.fiber.MoveNext()) {
          var yieldCommand : Object = (fiber.Current == null) ? 1 : fiber.Current;
          if (yieldCommand instanceof int) {
            coroutine.waitForFrame = yieldCommand;
            coroutine.waitForFrame += currentFrame;
          } else if (yieldCommand instanceof float) {
            coroutine.waitForTime = yieldCommand;
            coroutine.waitForTime +=  currentTime;
          } else if  (yieldCommand instanceof CoroutineNode) {
            coroutine.waitForCoroutine = yieldCommand;
          } else {
            throw "Unexpected coroutine yield type: " + yieldCommand.GetType();
          }
        } else { // coroutine finished
          coroutine.finished = true;
          RemoveCoroutine(coroutine);
        }
      }
     
      private function AddCoroutine(coroutine : CoroutineNode) {
        if (first != null) {
          coroutine.listNext = first;
          first.listPrevious = coroutine;
        }
        first = coroutine;
      }
     
      private function RemoveCoroutine(coroutine : CoroutineNode) {
        if (first == coroutine) { // remove first
          first = coroutine.listNext;
        } else { // not head of list
          if (coroutine.listNext != null) { // remove between
            coroutine.listPrevious.listNext = coroutine.listNext;
            coroutine.listNext.listPrevious = coroutine.listPrevious;
          } else if (coroutine.listPrevious != null) { // and listNext is null
            coroutine.listPrevious.listNext = null; // remove last
          }
        }
        coroutine.listPrevious = null;
        coroutine.listNext = null;
      }
    }CoroutineNode.js #pragma strict
     
    /**
     * Linked list node type used by coroutine scheduler to track scheduling of
     * coroutines.
     */
    class CoroutineNode {
      var listPrevious : CoroutineNode = null;
      var listNext : CoroutineNode = null;
      var fiber : IEnumerator;
      var finished = false;
      var waitForFrame = -1;
      var waitForTime = -1.0;
      var waitForCoroutine : CoroutineNode = null;
     
      function CoroutineNode(fiber : IEnumerator) {
        this.fiber = fiber;
      }
    }Ported to CSharp 
    CoroutineSchedulerTest.cs using UnityEngine;
    using System.Collections;
     
     
    /// <summary>
    /// CoroutineSchedulerTest.cs
    /// 
    /// Port of the Javascript version from 
    /// http://www.unifycommunity.com/wiki/index.php?title=CoroutineScheduler
    /// 
    /// Linked list node type used by coroutine scheduler to track scheduling of coroutines.
    ///  
    /// BMBF Researchproject http://playfm.htw-berlin.de
    /// PlayFM - Serious Games für den IT-gestützten Wissenstransfer im Facility Management 
    ///	Gefördert durch das bmb+f - Programm Forschung an Fachhochschulen profUntFH
    ///	
    ///	<author>Frank.Otto@htw-berlin.de</author>
    ///
    /// </summary>
     
    public class CoroutineSchedulerTest : MonoBehaviour
    {
     
    	CoroutineScheduler scheduler;
    	// Use this for initialization
    	void Start ()
    	{
    		scheduler = new CoroutineScheduler ();
    		scheduler.StartCoroutine (MyCoroutine ());
    	}
     
     
    	IEnumerator MyCoroutine ()
    	{
    		Debug.Log ("MyCoroutine: Begin");
    		yield return 0;
    		// wait for next update
    		Debug.Log ("MyCoroutine: next update;" + Time.time);
    		yield return 2;
    		// wait for 2 updates, same as yield; yield;
    		Debug.Log ("MyCoroutine: After yield 2;" + Time.time);
    		yield return 3.5f;
    		// wait for 3.5 seconds
    		Debug.Log ("MyCoroutine: After 3.5 seconds;" + Time.time);
    		// you can also yield for a coroutine running on a completely different scheduler instance
    		yield return scheduler.StartCoroutine (WaitForMe ());
    		Debug.Log ("MyCoroutine: After WaitForMe() finished;" + Time.time);
    	}
     
    	IEnumerator WaitForMe ()
    	{
    		yield return 7.8f;
    		// wait for 7.8 seconds before finishing
    	}
     
    	// Update is called once per 	
    	void Update ()
    	{
     
    		scheduler.UpdateAllCoroutines (Time.frameCount, Time.time);
    	}
     
     
    }CoroutineScheduler.cs using System.Collections;
    using UnityEngine;
     
    /// <summary>
    /// CoroutineScheduler.cs
    /// 
    /// Port of the Javascript version from 
    /// http://www.unifycommunity.com/wiki/index.php?title=CoroutineScheduler
    /// 
    /// Linked list node type used by coroutine scheduler to track scheduling of coroutines.
    /// 
    /// 
    /// BMBF Researchproject http://playfm.htw-berlin.de
    /// PlayFM - Serious Games für den IT-gestützten Wissenstransfer im Facility Management 
    ///	Gefördert durch das bmb+f - Programm Forschung an Fachhochschulen profUntFH
    ///	
    ///	<author>Frank.Otto@htw-berlin.de</author>
    ///
    /// 
    /// A simple coroutine scheduler. Coroutines can yield until the next update
    /// "yield;", until a given number of updates "yield anInt", until a given
    /// amount of seconds "yield aFloat;", or until another coroutine has finished
    /// "yield scheduler.StartCoroutine(Coroutine())".
    /// 
    /// Multiple scheduler instances are supported and can be very useful. A
    /// coroutine running under one scheduler can yield (wait) for a coroutine
    /// running under a completely different scheduler instance.
    /// 
    /// Unity's YieldInstruction classes are not used because I cannot
    /// access their internal data needed for scheduling. Semantics are slightly
    /// different from Unity's scheduler. For example, in Unity if you start a
    /// coroutine it will run up to its first yield immediately, while in this
    /// scheduler it will not run until the next time UpdateAllCoroutines is called.
    /// This feature allows any code to start coroutines at any time, while
    /// making sure the started coroutines only run at specific times.
    /// 
    /// You should not depend on update order between coroutines running on the same
    /// update. For example, StartCoroutine(A), StartCoroutine(B), StartCoroutine(C)
    /// where A, B, C => while(true) { print(A|B|C); yield; }, do not expect "ABC" or
    /// "CBA" or any other specific ordering.
    /// </summary>
    public class CoroutineScheduler : MonoBehaviour
    {
     
    	CoroutineNode first = null;
    	int currentFrame;
    	float currentTime;
     
    	/**
       * Starts a coroutine, the coroutine does not run immediately but on the
       * next call to UpdateAllCoroutines. The execution of a coroutine can
       * be paused at any point using the yield statement. The yield return value
       * specifies when the coroutine is resumed.
       */
     
    	public CoroutineNode StartCoroutine (IEnumerator fiber)
    	{
    		// if function does not have a yield, fiber will be null and we no-op
    		if (fiber == null) {
    			return null;
    		}
    		// create coroutine node and run until we reach first yield
    		CoroutineNode coroutine = new CoroutineNode (fiber);
    		AddCoroutine (coroutine);
    		return coroutine;
    	}
     
    	/**
       * Stops all coroutines running on this behaviour. Use of this method is
       * discouraged, think of a natural way for your coroutines to finish
       * on their own instead of being forcefully stopped before they finish.
       * If you need finer control over stopping coroutines you can use multiple
       * schedulers.
       */
    	public void StopAllCoroutines ()
    	{
    		first = null;
    	}
     
    	/**
       * Returns true if this scheduler has any coroutines. You can use this to
       * check if all coroutines have finished or been stopped.
       */
    	public bool HasCoroutines ()
    	{
    		return first != null;
    	}
     
    	/**
       * Runs all active coroutines until their next yield. Caller must provide
       * the current frame and time. This allows for schedulers to run under
       * frame and time regimes other than the Unity's main game loop.
       */
    	public void UpdateAllCoroutines (int frame, float time)
    	{
    		currentFrame = frame;
    		currentTime = time;
    		CoroutineNode coroutine = this.first;
    		while (coroutine != null) {
    			// store listNext before coroutine finishes and is removed from the list
    			CoroutineNode listNext = coroutine.listNext;
     
    			if (coroutine.waitForFrame > 0 && frame >= coroutine.waitForFrame) {
    				coroutine.waitForFrame = -1;
    				UpdateCoroutine (coroutine);
    			} else if (coroutine.waitForTime > 0.0f && time >= coroutine.waitForTime) {
    				coroutine.waitForTime = -1.0f;
    				UpdateCoroutine (coroutine);
    			} else if (coroutine.waitForCoroutine != null && coroutine.waitForCoroutine.finished) {
    				coroutine.waitForCoroutine = null;
    				UpdateCoroutine (coroutine);
    			} else if (coroutine.waitForFrame == -1 && coroutine.waitForTime == -1.0f && coroutine.waitForCoroutine == null) {
    				// initial update
    				UpdateCoroutine (coroutine);
    			}
    			coroutine = listNext;
    		}
    	}
     
    	/**
       * Executes coroutine until next yield. If coroutine has finished, flags
       * it as finished and removes it from scheduler list.
       */
    	private void UpdateCoroutine (CoroutineNode coroutine)
    	{
    		IEnumerator fiber = coroutine.fiber;
    		if (coroutine.fiber.MoveNext ()) {
    			System.Object yieldCommand = fiber.Current == null ? (System.Object) 1 : fiber.Current;
     
    			if (yieldCommand.GetType () == typeof(int)) {
    				coroutine.waitForFrame = (int) yieldCommand;
    				coroutine.waitForFrame += (int) currentFrame;
    			} else if (yieldCommand.GetType () == typeof(float)) {
    				coroutine.waitForTime = (float) yieldCommand;
    				coroutine.waitForTime += (float) currentTime;
    			} else if (yieldCommand.GetType () == typeof(CoroutineNode)) {
    				coroutine.waitForCoroutine = (CoroutineNode) yieldCommand;
    			} else {
    				throw new System.ArgumentException ("CoroutineScheduler: Unexpected coroutine yield type: " + yieldCommand.GetType ());
    			}
    		} else {
    			// coroutine finished
    			coroutine.finished = true;
    			RemoveCoroutine (coroutine);
    		}
    	}
     
    	private void AddCoroutine (CoroutineNode coroutine)
    	{
     
    		if (this.first != null) {
    			coroutine.listNext = this.first;
    			first.listPrevious = coroutine;
    		}
    		first = coroutine;
    	}
     
    	private void RemoveCoroutine (CoroutineNode coroutine)
    	{
    		if (this.first == coroutine) {
    			// remove first
    			this.first = coroutine.listNext;
    		} else {
    			// not head of list
    			if (coroutine.listNext != null) {
    				// remove between
    				coroutine.listPrevious.listNext = coroutine.listNext;
    				coroutine.listNext.listPrevious = coroutine.listPrevious;
    			} else if (coroutine.listPrevious != null) {
    				// and listNext is null
    				coroutine.listPrevious.listNext = null;
    				// remove last
    			}
    		}
    		coroutine.listPrevious = null;
    		coroutine.listNext = null;
    	}
     
    }//classCoroutineNode.cs using System.Collections;
    using UnityEngine;
     
     
    /// <summary>
    /// CoroutineNode.cs
    /// 
    /// Port of the Javascript version from 
    /// http://www.unifycommunity.com/wiki/index.php?title=CoroutineScheduler
    /// 
    /// Linked list node type used by coroutine scheduler to track scheduling of coroutines.
    ///  
    /// BMBF Researchproject http://playfm.htw-berlin.de
    /// PlayFM - Serious Games für den IT-gestützten Wissenstransfer im Facility Management 
    ///	Gefördert durch das bmb+f - Programm Forschung an Fachhochschulen profUntFH
    ///	
    ///	<author>Frank.Otto@htw-berlin.de</author>
    ///
    /// </summary>
     
    public class CoroutineNode
    {
    	public CoroutineNode listPrevious = null;
    	public CoroutineNode listNext = null;
    	public IEnumerator fiber;
    	public bool finished = false;
    	public int waitForFrame = -1;
    	public float waitForTime = -1.0f;
    	public CoroutineNode waitForCoroutine;
     
    	public CoroutineNode (IEnumerator _fiber)
    	{
    		this.fiber = _fiber;
    	}
    }Additional Implementation C# If you want to use CoroutineScheduler with Yield Commands from within the UnityEngine namespace here is an addition to the implementation, I found it useful for consuming api's in classes defined outside the UnityEngine namespace. 
    Sample Usage m_scheduler = new CoroutineScheduler();
    m_scheduler.StartCoroutine(testAPI());
     
    IEnumerator testAPI()
    {
       // ...set up request
     
       var www = new UnityEngine.WWW(requestURL);
       yield return new UnityWWWYieldWrapper(www);
     
       // ...loading complete do some stuff
    }Addition to CoroutineScheduler.cs //line 110
     
    public void UpdateAllCoroutines(int frame, float time)
    {
       currentFrame = frame;
       currentTime = time;
       CoroutineNode coroutine = this.first;
       while (coroutine != null)
       {
          // store listNext before coroutine finishes and is removed from the list
          CoroutineNode listNext = coroutine.listNext;
     
          if (coroutine.waitForFrame > 0 && frame >= coroutine.waitForFrame)
          {
             coroutine.waitForFrame = -1;
             UpdateCoroutine(coroutine);
          }
          else if (coroutine.waitForTime > 0.0f && time >= coroutine.waitForTime)
          {
             coroutine.waitForTime = -1.0f;
             UpdateCoroutine(coroutine);
          }
          else if (coroutine.waitForCoroutine != null && coroutine.waitForCoroutine.finished)
          {
             coroutine.waitForCoroutine = null;
             UpdateCoroutine(coroutine);
          }
          else if (coroutine.waitForUnityObject != null && coroutine.waitForUnityObject.finished)//lonewolfwilliams
          {
             coroutine.waitForUnityObject = null;
             UpdateCoroutine(coroutine);
          }
          else if (coroutine.waitForFrame == -1 && coroutine.waitForTime == -1.0f 
                   && coroutine.waitForCoroutine == null && coroutine.waitForUnityObject == null)
          {
             // initial update
             UpdateCoroutine(coroutine);
          }
          coroutine = listNext;
       }
    }
     
    //line 154
     
    private void UpdateCoroutine(CoroutineNode coroutine)
    {
       IEnumerator fiber = coroutine.fiber;
       if (coroutine.fiber.MoveNext())
       {
          System.Object yieldCommand = fiber.Current == null ? (System.Object)1 : fiber.Current;
     
          if (yieldCommand.GetType() == typeof(int))
          {
             coroutine.waitForFrame = (int)yieldCommand;
             coroutine.waitForFrame += (int)currentFrame;
          }
          else if (yieldCommand.GetType() == typeof(float))
          {
             coroutine.waitForTime = (float)yieldCommand;
             coroutine.waitForTime += (float)currentTime;
          }
          else if (yieldCommand.GetType() == typeof(CoroutineNode))
          {
             coroutine.waitForCoroutine = (CoroutineNode)yieldCommand;
          }
          else if (yieldCommand is IYieldWrapper) //lonewolfwilliams
          {
             coroutine.waitForUnityObject = yieldCommand as IYieldWrapper;
          }
          else
          {
             throw new System.ArgumentException("CoroutineScheduler: Unexpected coroutine yield type: " + yieldCommand.GetType());
     
             //this is an alternative if you don't have access to the function passed to the couroutineScheduler - maybe it's                      
             //precompiled in a dll for example - remember you will have to add a case every time you add a wrapper :/
             /*
             var commandType = yieldCommand.GetType();
    	 if(commandType == typeof(UnityEngine.WWW))
             {
    	    coroutine.waitForUnityObject = 
                   new UnityWWWWrapper(yieldCommand as UnityEngine.WWW);
    	 }
    	 else if(commandType == typeof(UnityEngine.AsyncOperation))
    	 {
    	    coroutine.waitForUnityObject = 
    	       new UnityASyncOpWrapper(yieldCommand as UnityEngine.AsyncOperation);
    	 }
    	 else if(commandType == typeof(UnityEngine.AssetBundleRequest))
    	 {
    	    coroutine.waitForUnityObject = 
    	       new UnityAssetBundleRequestWrapper(yieldCommand as UnityEngine.AssetBundleRequest);
    	 }
    	 else
    	 {
                throw new System.ArgumentException("CoroutineScheduler: Unexpected coroutine yield type: " + yieldCommand.GetType());
    	 }
             */
          }
       }
       else
       {
          // coroutine finished
          coroutine.finished = true;
          RemoveCoroutine(coroutine);
       }
    }Addition to CoroutineNode.cs public class CoroutineNode
    {
       public CoroutineNode listPrevious = null;
       public CoroutineNode listNext = null;
       public IEnumerator fiber;
       public bool finished = false;
       public int waitForFrame = -1;
       public float waitForTime = -1.0f;
       public CoroutineNode waitForCoroutine;
       public IYieldWrapper waitForUnityObject; //lonewolfwilliams
     
       public CoroutineNode(IEnumerator _fiber)
       {
          this.fiber = _fiber;
       }
    }IYieldWrapper.cs you will need to write your own implementations for yield commands from within the UnityEngine namespace, this interface should help facilitate that. 
    /*
     * gareth williams 
     * http://www.lonewolfwilliams.com
     */
     
    public interface IYieldWrapper
    {
       bool finished { get; }
    }Example Wrappers Below are some examples of wrappers I have used, in fact they have almost identical signatures so a more generic implementation could probably be written ^_^ 
    UnityASyncOpWrapper using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
     
    /*
    *   Gareth Williams
    *   http://www.lonewolfwilliams.com
    */
     
    class UnityASyncOpWrapper : IYieldWrapper
    {
       private UnityEngine.AsyncOperation m_UnityObject;
       public bool finished
       {
          get
          {
             return m_UnityObject.isDone;
          }
       }
     
       public UnityASyncOpWrapper(UnityEngine.AsyncOperation wraps)
       {
          m_UnityObject = wraps;
       }
    }UnityWWWYieldWrapper using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
     
    /*
     * Gareth Williams
     * http://www.lonewolfwilliams.com
     */
     
    public class UnityWWWYieldWrapper : IYieldWrapper
    {
       private UnityEngine.WWW m_UnityObject;
       public bool finished
       {
          get
          {
             return m_UnityObject.isDone;
          }
       }
     
       public UnityWWWYieldWrapper(UnityEngine.WWW wraps)
       {
          m_UnityObject = wraps;
       }
}
}
