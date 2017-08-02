/*************************
 * Original url: http://wiki.unity3d.com/index.php/WithProgressBar
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/WithProgressBar.cs
 * File based on original modification date of: 10 January 2012, at 20:47. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    Description Put this script in an Editor/ folder and you'll now have the ability to add a Unity IDE *cancelable* progress bar to any enumerable expression. Anytime you have a really time-consuming "foreach" in your code, you can now throw in a quick progress bar anywhere something is being enumerated. 
    by Matt "Trip" Maker, Monstrous Company :: http://monstro.us 
    Usage Use it bare, or with arguments specifying the properties the progress bar should have. 
    enumerateAssets().WithProgressBar("Collecting Missing Refs").Where(x => hasMissingRefInChildren(x)).ToList();
     
    foreach (Object o in BigListOfObjects.WithProgressBar()) ...
     
    List<GameObject> myList = mySmallEnumerableThing.WithProgressBar(80, "Processing Small List of Enumerable Things", "This is the info section, where you can show a longer description of just why it is that the user is supposed to be waiting for something. All these arguments are optional.").ToList();Notes Currently, when the user cancels the progress bar, this simply ends the enumeration and your code proceeds as if it had reached the end of the enumerables. See the comments in the code for additional information. 
    // IEnumerableExtensionWithProgressBar - Unity-Specific Extension to LINQ to Objects
    // by Matt "Trip" Maker, Monstrous Company :: http://monstro.us
    //
    //
    // TODO would IQueryable<T> be useful also?
    // TODO could pass a delegate to be called if the user cancels. (would anyone need that?)
    // TODO could provide the option to show the ToString() of the current item in the info area of the progress bar dialog.
     
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
     
    public static class IEnumerableExtensionWithProgressBar
    {
    	// default values. Note that you can adjust these as desired earlier in your code, and then call WithProgressBar() with fewer or no arguments at all.
    	public static int defaultMax = 10000; // we just pick a big number for defaultMax. if we wanted to be silly, we could keep an actual calculated average stored somewhere.
    	public static string defaultTitle = "Progress";
    	public static string defaultInfo = string.Empty;
     
    	private static int defaultInterval = 50;
     
    	/// <summary>
    	/// Shows a progress bar for the items as they pass by; the bar size is proportional to the given max, if supplied.
    	/// </summary>
    	/// <param name="source">
    	/// A <see cref="IEnumerable<T>"/>
    	/// </param>
    	/// <param name="max">
    	/// A <see cref="System.Int32"/>
    	/// </param>
    	/// <param name="title">
    	/// A <see cref="System.String"/>
    	/// </param>
    	/// <param name="info">
    	/// A <see cref="System.String"/>
    	/// </param>
    	/// <returns>
    	/// A <see cref="IEnumerable<T>"/>
    	/// </returns>
    	public static IEnumerable<T> WithProgressBar<T>(this IEnumerable<T> source, int max, string title, string info)
    	{
    		if (source == null) throw new ArgumentNullException("source");
            return WithProgressBarImpl(source, max, title, info);
        }
        public static IEnumerable<T> WithProgressBar<T>(this IEnumerable<T> source, int max, string title) {
    		return WithProgressBar(source, max, title, defaultInfo);
    	}
        public static IEnumerable<T> WithProgressBar<T>(this IEnumerable<T> source, string title) {
            return WithProgressBar(source, defaultMax, title, defaultInfo);
        }
        public static IEnumerable<T> WithProgressBar<T>(this IEnumerable<T> source, int max) {
            return WithProgressBar(source, max, defaultTitle, defaultInfo);
        }
    	public static IEnumerable<T> WithProgressBar<T>(this IEnumerable<T> source) {
            return WithProgressBar(source, defaultMax, defaultTitle, defaultInfo);
        }
     
    	private static IEnumerable<T> WithProgressBarImpl<T>(this IEnumerable<T> source, int max, string title, string info)
        {
    		if (max < 1) max = 1;//or could throw exception
            int progress = 0;
    		int nth = max / defaultInterval;
    		if (nth < 1) nth = 1;
    		foreach (T element in source)
            {
    			progress+=1;
     
    	        if(progress < max) {
    				if (progress % nth == 0) { //only update every nth time, because updating a progressbar UI is heavy
    		            if(EditorUtility.DisplayCancelableProgressBar(
    		                title,
    					    info,
    		                progress/(float)max)) { //note, that cast to float is essential if you actually want to see the progress! :)
    		                Debug.Log("Progress bar canceled by the user");
    						EditorUtility.ClearProgressBar(); // if canceled, we must get rid of the progress bar!
    						yield break; // cancelling just stops the enumeration short, so make sure your code is aware this can occur
    		            }
    				}
    	        }
     
    			yield return element;
    		}
    		EditorUtility.ClearProgressBar(); // and this is mandatory; otherwise, if the progress remains less than max, the progress bar would remain.
    	}
    }
}
