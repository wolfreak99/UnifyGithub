// Original url: http://wiki.unity3d.com/index.php/Set
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/DataStructureUtils/Set.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.DataStructureUtils
{
A Set Data TypeSet data types provide a fast way to do, well, set operations - union, intersection, and the like. They're much faster than lists for these types of operations, and are particularly well suited to situations where your objects don't have an ordinal or keyed relationship. 
Codeusing UnityEngine;
using System;
using System.Collections;
 
/// <summmary>
/// Fairly decent sets class implemented using hashtables.
/// from http://www.codeproject.com/csharp/set.asp ,
/// assumed to be under an open source license by Ethan Fremen <i mindlace.net>
/// who also adapted it for unity.
/// </summmary>
/// <remarks>
/// Authors: Richard Bothne, Jim Showalter
/// </remarks>
/// <remarks>
/// All sets used with an instance of this class have to use
/// the same hashcode provider and comparer.
/// </remarks>
/// <remarks>
/// The constructors duplicate the Hashtable constructors. For
/// documentation, see the corresponding Hashtable documentation.
/// </remarks>
/// <exception>
/// Throws no exceptions, and propagates untouched all exceptions thrown by callees.
/// </exception>
public class Set : Hashtable {
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set() : base()
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(Set otherSet) : base(otherSet)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(int capacity) : base(capacity)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(Set otherSet, float loadFactor) : base(otherSet, loadFactor)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(IHashCodeProvider iHashCodeProvider, IComparer iComparer) : base(iHashCodeProvider, iComparer)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(int capacity, float loadFactor) : base(capacity, loadFactor)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(Set otherSet, IHashCodeProvider iHashCodeProvider, IComparer iComparer) : base(otherSet, iHashCodeProvider, iComparer)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(int capacity, IHashCodeProvider iHashCodeProvider, IComparer iComparer) : base(capacity, iHashCodeProvider, iComparer)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(Set otherSet, float loadFactor, IHashCodeProvider iHashCodeProvider, IComparer iComparer) : base(otherSet, loadFactor, iHashCodeProvider, iComparer)
	{
	}
 
	/// <summary>
	/// Refer to Hashtable constructor documentation.
	/// </summary>
	public Set(int capacity, float loadFactor, IHashCodeProvider iHashCodeProvider, IComparer iComparer) : base(capacity, loadFactor, iHashCodeProvider, iComparer)
	{
	}
 
	/// <summary>
	///  Adds an item to the set. Items are stored as keys, with no associated values.
	/// </summary>
	public void Add(System.Object entry)
	{
		base.Add(entry, null);
	}
 
	/// <summary>
	/// Helper function that does most of the work in the class.
	/// </summary>
	private static Set Generate(
		Set iterSet,
		Set containsSet,
		Set startingSet,
		bool containment)
	{
		// Returned set either starts out empty or as copy of the starting set.
		Set returnSet = startingSet == null ? new Set(iterSet.hcp, iterSet.comparer) : startingSet;
 
		foreach(object key in iterSet.Keys)
		{
			// (!containment && !containSet.ContainsKey) ||
			//  (containment &&  containSet.ContainsKey)
			if (!(containment ^ containsSet.ContainsKey(key)))
			{
				returnSet.Add(key);
			}
		}
 
		return returnSet;
	}
 
	/// <summary>
	/// Union of set1 and set2.
	/// </summary>
	public static Set operator | (Set set1, Set set2)
	{
		// Copy set1, then add items from set2 not already in set 1.
		Set unionSet = new Set(set1, set1.hcp, set1.comparer);
		return Generate(set2, unionSet, unionSet, false);
	}
 
	/// <summary>
	/// Union of this set and otherSet.
	/// </summary>
	public Set Union(Set otherSet)
	{
		return this | otherSet;
	}
 
	/// <summary>
	/// Intersection of set1 and set2.
	/// </summary>
	public static Set operator & (Set set1, Set set2) 
	{
		// Find smaller of the two sets, iterate over it
		// to compare to other set.
		return Generate(
			set1.Count > set2.Count ? set2 : set1,
			set1.Count > set2.Count ? set1 : set2,
			null,
			true);
	}
 
	/// <summary>
	/// Intersection of this set and otherSet.
	/// </summary>
	public Set Intersection(Set otherSet) 
	{
		return this & otherSet;
	}
 
	/// <summary>
	/// Exclusive-OR of set1 and set2.
	/// </summary>
	public static Set operator ^ (Set set1, Set set2) 
	{
		// Find items in set1 that aren't in set2. Then find
		// items in set2 that aren't in set1. Return combination
		// of those two subresults.
		return Generate(set2, set1, Generate(set1, set2, null, false), false);
	}
 
	/// <summary>
	/// Exclusive-OR of this set and otherSet.
	/// </summary>
	public Set ExclusiveOr(Set otherSet) 
	{
		return this ^ otherSet;
	}
 
	/// <summary>
	/// The set1 minus set2. This is not associative.
	/// </summary>
	public static Set operator - (Set set1, Set set2) 
	{
		return Generate(set1, set2, null, false);
	}
 
	/// <summary>
	/// This set minus otherSet. This is not associative.
	/// </summary>
	public Set Difference(Set otherSet) 
	{
		return this - otherSet;
	}
 
}
}
