/*************************
 * Original url: http://wiki.unity3d.com/index.php/AbortableEnumerator
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/AbortableEnumerator.cs
 * File based on original modification date of: 25 December 2013, at 23:03. 
 *
 * Description
 *   Provides an abort mechanism to a coroutine. 
 *   Wrap the call to the coroutine with this AbortableEnumerator, and call Abort() when you want to abort it. 
 * Usage
 *   See this answer for usage example
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/
// TODO when site server is fixed, retrieve link for Usage's "See this answer" context

namespace UnifyGithub.General.CodeSnippets
{
    using System.Collections;

    public class AbortableEnumerator : IEnumerator
    {
    	protected IEnumerator enumerator;
    	protected bool isAborted;
     
    	public AbortableEnumerator(IEnumerator enumerator)
    	{
    		this.enumerator = enumerator;
    	}
     
    	public void Abort()
    	{
    		isAborted = true;
    	}
     
    	bool IEnumerator.MoveNext ()
    	{
    		if (isAborted)
    			return false;
    		else
    			return enumerator.MoveNext ();
    	}
     
    	void IEnumerator.Reset ()
    	{
    		isAborted = false;
    		enumerator.Reset ();
    	}
     
    	object IEnumerator.Current 
    	{
    		get { return enumerator.Current; }
    	}
    }
}
