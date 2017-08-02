/*************************
 * Original url: http://wiki.unity3d.com/index.php/ArrayTools
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/DataStructureUtils/ArrayTools.cs
 * File based on original modification date of: 20 February 2012, at 17:08. 
 *
 * Author: Berenger 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.DataStructureUtils
{
    Contents [hide] 
    1 Description 
    1.1 Usage 
    1.2 Supported Operations 
    2 Installation 
    3 History 
    4 Code (ArrayTools.cs) 
    
    Description This script provide functions to use built-in arrays in a more natural way, like an ArrayList. 
    Usage To use ArrayTools functions, you need to specify the type of the elements of the array. The modified array is returned by the function. 
    int[] array = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
     
    	ArrayTools.InsertAt( array, 999, 5 );//0 1 2 3 4 999 5 6 7 8 9 
    	ArrayTools.InsertAt( array, new int[]{ 77, 88, 99}, 2 );//0 1 77 88 99 2 3 4 5 6 7 8 9 
    	ArrayTools.Push( array, 999 );//999 0 1 2 3 4 5 6 7 8 9 
    	ArrayTools.PushLast( array, 999 );//0 1 2 3 4 5 6 7 8 9 999 
    	ArrayTools.RemoveRange( array, 3, 7 );//0 1 2 8 9 
    	ArrayTools.RemoveAt( array, 2 );//0 1 3 4 5 6 7 8 9 
    	ArrayTools.RemoveAt( array, 2, 3 );//0 1 5 6 7 8 9 
    	ArrayTools.Pop( array );//1 2 3 4 5 6 7 8 9 
    	ArrayTools.Pop( array, 3 );//3 4 5 6 7 8 9 
    	ArrayTools.PopLast( array ); //0 1 2 3 4 5 6 7 8 
    	ArrayTools.PopLast( array, 3 ); //0 1 2 3 4 5 6 
    	ArrayTools.Remove( new int[]{ 0, 1, 5, 2, 5, 5 }, 5 ); //0 1 2 5 5 
    	ArrayTools.RemoveAll( new int[]{ 0, 2, 5, 6, 4, 5, 5, 2, 5, 5}, 5 ); //0 2 6 4 2 
    	ArrayTools.Shift( array, 2, 3, 3 ); //0 1 5 6 7 2 3 4 8 9 
    	ArrayTools.Shr( array, 5 ); //0 1 2 3 4 6 5 7 8 9 
    	ArrayTools.Shl( array, 5 ); //0 1 2 3 5 4 6 7 8 9
    	ArrayTools.Concat( new int[]{ 0, 1, 2 },
    			new int[]{ 3, 4, 5 },
    			new int[]{ 777, 888, 999 } );//0 1 2 3 4 5 777 888 999 
    	ArrayTools.SubArray( array, 3, 7 );//3 4 5 6 7
    	ArrayTools.Shuffle( array );//2 8 9 1 4 3 7 6 0 5 (random)
    	ArrayTools.Sow( array, 999, 5 );//999 0 1 2 3 4 5 999 999 6 999 7 8 999 9 (random)  
    	ArrayTools.CreateRepeat( 5, 10 );//5 5 5 5 5 5 5 5 5 5 
    	ArrayTools.CreateRandom( 10, 0, 15 );//13 6 14 13 12 8 1 12 1 9 (random)
    	ArrayTools.Create( 10, index => index );//0 1 2 3 4 5 6 7 8 9
    	ArrayTools.Create( 3, () => new Object() );//Object1 Object2 Object3
    	ArrayTools.Update( array, x => x*x );//0 1 4 9 16 25 36 49 64 81
    	ArrayTools.Update( array, (x, index) => x+index );//0 2 4 6 8 10 12 14 16 18Supported Operations Following operations are supported. 
    	// Insert an element at a given index
    	public static T[] InsertAt<T>( T[] array, T value, int index );
     
    	// Insert an array of elements at a given index
    	public static T[] InsertAt<T>( T[] array, T[] value, int index );
     
    	// Insert an element at the first index
    	public static T[] Push<T>( T[] array, T value );
     
    	// Insert an element at the last index
    	public static T[] PushLast<T>( T[] array, T value );
     
    	// Remove all elements between start and end indexes
    	public static T[] RemoveRange<T>( T[] array, int start, int end );
     
    	// Remove an element at a given index
    	public static T[] RemoveAt<T>( T[] array, int index );
     
    	// Remove all elements from start to start+count indexes
    	public static T[] RemoveAt<T>( T[] array, int start, int count );
     
    	// Remove first element
    	public static T[] Pop<T>( T[] array );
     
    	// Remove count elements at the beginning
    	public static T[] Pop<T>( T[] array, int count );
     
    	// Remove last element
    	public static T[] PopLast<T>( T[] array );
     
    	// Remove count elements at the end
    	public static T[] PopLast<T>( T[] array, int count );
     
    	// Find and remove an element
    	public static T[] Remove<T>( T[] array, T value );
     
    	// Find and remove all occurrences of the element
    	public static T[] RemoveAll<T>( T[] array, T value );
     
    	// Move an element inside the array, from the index indice to the index indice+decalage
    	// move count elements.
    	public static T[] Shift<T>( T[] array, int indice, int count, int decalage );
     
    	// Move one element to the right
    	public static T[] Shr<T>( T[] array, int indice );
     
    	// Move one element to the left
    	public static T[] Shl<T>( T[] array, int indice );
     
    	// Concats all the array in parameters.
    	public static T[] Concat<T>( params T[][] arrays );
     
    	// Return a subarray of array within the specified bounds.
    	public static T[] SubArray<T>( T[] source, int start, int end );
     
    	//http://www.codeproject.com/Articles/35114/Shuffling-arrays-in-C
    	// Change randomly the order of the array.
    	public static T[] Shuffle<T>( T[] array );
     
    	// Change randomly the order of a part of the array.
    	public static T[] Shuffle<T>( T[] array, int start, int end );
     
    	// Insert count elements randomly all over the array.
    	public static T[] Sow<T>( T[] array, T value, int count );
     
    	// Insert count elements randomly between the specified bounds.
    	public static T[] Sow<T>( T[] array, T value, int count, int lowerBound, int upperBound, bool includeBounds );
     
    	// Create an array of size count with every element == value.
    	public static T[] CreateRepeat<T>( T value, int count );
     
    	// Create an array of random integer and of size count. The numbers are between min and max.
    	public static int[] CreateRandom( int count, int min, int max );
     
    	//http://msdn.microsoft.com/en-us/library/bb397687.aspx
    	// Create an array of T, size count.
    	// Each element will be determined by the lambda function in argument(See link above)
    	// The first value is start, then it's start+1, start +2 ... 
    	// Create(5, () => new MyClass()) will give you an array of 5 MyClass unique instances.
    	public static T[] Create<T>( int count, Func<T> constructor );
     
    	// This overload provides the possibility to access the index of the element created.
    	// Create(5, i => i) will give you an increasing sequence of 5 integers. 0 1 2 3 4
    	// Create(5, x => x*x) a squarre function. 0 1 4 9 16
    	// Create(5, () => new MyClass(i)) will give you an array of 5 MyClass unique instances.
    	public static T[] Create<T>( int count, Func<int, T> constructor );
     
    	// Apply a function upon all members of the array. The function take a T in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, T> selectFunc );
     
    	// Apply a function upon all members of the array between start and end included. The function take a T in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, T> selectFunc, int start, int end );
     
    	// Apply a function upon all members of the array. The function take a T and the index in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, int, T> selectFunc );
     
    	// Apply a function upon all members of the array between start and end included. The function take a T and the index in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, int, T> selectFunc, int start, int end );Installation To be able to use ArrayTools from all your classes (JavaScript, Boo and C#), it's best to put it into your "Standard Assets" folder. 
    History 15 February 2012 
    Initial release 
    19 February 2012 
    Update, new functions (RemoveAll, Concat, SubArray, Shuffle, Sow, CreateRepeat, CreateRandom, Create) 
    Code (ArrayTools.cs) using System;
    using System.Collections;
    using System.Linq;
     
    public class ArrayTools
    {		
    	// Insert an element at a given index.
    	public static T[] InsertAt<T>( T[] array, T value, int index )
    		{
    		T[] tmp = array;
    		array = new T[ array.Length+1 ];
    		Array.Copy( tmp, 0, array, 0, index );
    		array[index] = value;
    		Array.Copy( tmp, index, array, index+1, tmp.Length-index );
     
    		return array;
    		// After 25 tests on 100k calls, this technique takes 43% more time
    		//InsertAt( ref array, new T[]{value}, index ); 
    	}
     
    	// Insert an array of elements at a given index.
    	public static T[] InsertAt<T>( T[] array, T[] value, int index )
    	{
    		T[] tmp = array;
    		array = new T[ array.Length+value.Length ];
    		Array.Copy( tmp, 0, array, 0, index );			
    		Array.Copy( value, 0, array, index, value.Length );			
    		Array.Copy( tmp, index, array, index+value.Length, tmp.Length-index );
     
    		return array;
    	}		
    	// Insert an element at the first index.
    	public static T[] Push<T>( T[] array, T value ){ return InsertAt<T>( array, value, 0 ); }
    	// Insert an element at the last index.
    	public static T[] PushLast<T>( T[] array, T value ){ return InsertAt<T>( array, value, array.Length ); }
     
    	// Remove all elements between start and end indexes.
    	public static T[] RemoveRange<T>( T[] array, int start, int end ){ return RemoveAt<T>( array, start, end-start+1 ); }
    	// Remove an element at a given index.
    	public static T[] RemoveAt<T>( T[] array, int index ){ return RemoveAt<T>( array, index, 1 ); }
    	// Remove all elements from start to start+count indexes.
    	public static T[] RemoveAt<T>( T[] array, int start, int count )
    	{			
    		T[] tmp = array;
    		array = new T[ array.Length-count >= 0 ? array.Length-count : 0 ];
    		Array.Copy( tmp, array, start );
    		int index = start+count;
    		if( index < tmp.Length) 
    			Array.Copy( tmp, index, array, start, tmp.Length-index );			
     
    		return array;
    	}
     
    	// Remove first element.
    	public static T[] Pop<T>( T[] array ){ return RemoveAt<T>( array, 0, 1 ); }
    	// Remove count elements at the beginning.
    	public static T[] Pop<T>( T[] array, int count ){ return RemoveAt<T>( array, 0, count ); }
    	// Remove last element.
    	public static T[] PopLast<T>( T[] array ){ return RemoveAt<T>( array, array.Length-1, 1 ); }
    	// Remove count elements at the end.
    	public static T[] PopLast<T>( T[] array, int count ){ return RemoveAt<T>( array, array.Length-count, count ); }
     
    	// Find and remove an element.
    	public static T[] Remove<T>( T[] array, T value )
    	{
    		int index = Array.IndexOf<T>( array, value );			
    		if( index >= 0 ) 
    			return RemoveAt<T>( array, index );
    		return array;
    	}
    	// Find and remove all occurrences of the element.
    	public static T[] RemoveAll<T>( T[] array, T value )
    	{
    		int index = 0;
    		do
    		{
    			index = Array.IndexOf<T>( array, value );
    			if( index >= 0 ) 
    				array = RemoveAt<T>( array, index );
    		}
    		while( index >= 0 && array.Length > 0 );			
    		return array;
    	}		
     
    	// Move an element inside the array, from the index indice to the index indice+decalage
    	// move count elements.
    	// It's possible to optimize that function by affecting directly the array in argument,
    	// thus avoiding a Clone(). However, for coherence with the other non-destructive functions
    	// the copy is performed. The same goes for Shuffle.
    	public static T[] Shift<T>( T[] array, int indice, int count, int decalage )
    	{
    		if( array == null ) return null;
    		T[] result = (T[])array.Clone();
     
    		indice = indice < 0 ? 0 : (indice >= result.Length ? result.Length-1 : indice );
    		count = count < 0 ? 0 : (indice+count >= result.Length ? result.Length-indice-1 : count );
    		decalage = indice+decalage < 0 ? -indice : (indice+count+decalage >= result.Length ? result.Length-indice-count : decalage );
     
    		int absDec = Math.Abs(decalage);
    		T[] items = new T[count]; // What we want to move
    		T[] dec = new T[absDec]; // What is going to replace the thing we move
    		Array.Copy( array, indice, items, 0, count );
    		Array.Copy( array, indice + (decalage >= 0 ? count : decalage), dec, 0, absDec );
    		Array.Copy( dec, 0, result, indice + (decalage >= 0 ? 0 : decalage+count), absDec );
    		Array.Copy( items, 0, result, indice+decalage, count );		
     
    		return result;
    	}
     
    	// Move one element to the right.
    	public static T[] Shr<T>( T[] array, int indice ){ return Shift<T>( array, indice, 1, 1 ); }
    	// Move one element to the left.
    	public static T[] Shl<T>( T[] array, int indice ){ return Shift<T>( array, indice, 1, -1 ); }		
     
    	// Concats all the array in parameters.
    	public static T[] Concat<T>( params T[][] arrays )
    	{
    		int count = 0;
    		foreach( T[] t in arrays ) count += t.Length;
    		T[] result = new T[count];
     
    		count = 0;
    		for( int i = 0; i < arrays.Length; i++ )
    		{
    			Array.Copy( arrays[i], 0, result, count, arrays[i].Length );
    			count += arrays[i].Length;
    		}
     
    		return result;	
    	}
     
    	// Return a subarray of array within the specified bounds.
    	public static T[] SubArray<T>( T[] source, int start, int end )
    	{
    		int count = end - start + 1;
    		T[] result = new T[ count ];
    		Array.Copy( source, start, result, 0, count);		
     
    		return result;
    	}
     
    	//http://www.codeproject.com/Articles/35114/Shuffling-arrays-in-C
    	// Change randomly the order of the array.
    	public static T[] Shuffle<T>( T[] array ){ return Shuffle<T>( array, 0, array.Length-1 ); }
    	// Change randomly the order of a part of the array.
    	public static T[] Shuffle<T>( T[] array, int start, int end )
    	{
    		int count = end - start + 1;
    	    T[] shuffledPart = new T[count];
    		Array.Copy( array, start, shuffledPart, 0, count );
     
    	    var matrix = new SortedList();
    	    var r = new System.Random();		
     
    	    for (var x = 0; x <= shuffledPart.GetUpperBound(0); x++)
    	    {
    	        var i = r.Next();
    	        while (matrix.ContainsKey(i)) { i = r.Next(); }
    	        matrix.Add(i, shuffledPart[x]);
    	    }
     
    	    matrix.Values.CopyTo(shuffledPart, 0);
    		T[] result = (T[])array.Clone();
    		Array.Copy( shuffledPart, 0, result, start, count );	
     
    	    return result;
    	}
     
    	// Insert count elements randomly all over the array.
    	public static T[] Sow<T>( T[] array, T value, int count ){ return Sow<T>(array, value, count, 0, array.Length-1, true ); }
    	// Insert count elements randomly between the specified bounds.
    	public static T[] Sow<T>( T[] array, T value, int count, int lowerBound, int upperBound, bool includeBounds )
    	{			
    		T[] result = (T[])array.Clone();
    	    var r = new System.Random();
    		lowerBound += includeBounds ? 0 : 1;
    		upperBound += includeBounds ? 2 : 1;
     
    		for( int i = 0; i < count; i++ )
    			result = InsertAt<T>( result, value, r.Next( lowerBound, upperBound++ ) );
     
    		return result;
    	}
     
    	// Create an array of size count with every element == value.
    	public static T[] CreateRepeat<T>( T value, int count )
    	{
    		return Enumerable.Repeat( value, count ).ToArray();
    	}
     
    	// Create an array of random integer and of size count. The numbers are between min and max.
    	public static int[] CreateRandom( int count, int min, int max )
    	{
    		Random rand = new Random();
    		return Enumerable.Range( 0, count ).Select( i => rand.Next(min, max) ).ToArray();
    	}
     
     
    	// Create an array of T, size count.
    	// Each element will be determined by the lambda function in argument(See link above)
    	// The first value is start, then it's start+1, start +2 ... 
    	// Create(5, () => new MyClass()) will give you an array of 5 MyClass unique instances.
    	public static T[] Create<T>( int count, Func<T> constructor )
    	{
    		T[] instance = new T[count];
    		for( int i = 0; i < count; i++ )
    			instance[i] = constructor();
     
    		return instance;
    	}
     
    	// This overload provides the possibility to access the index of the element created.
    	// Create(5, i => i) will give you an increasing sequence of 5 integers. 0 1 2 3 4
    	// Create(5, x => x*x) a squarre function. 0 1 4 9 16
    	// Create(5, () => new MyClass(i)) will give you an array of 5 MyClass unique instances.
    	public static T[] Create<T>( int count, Func<int, T> constructor )
    	{
    		T[] instance = new T[count];
    		for( int i = 0; i < count; i++ )
    			instance[i] = constructor(i);			
     
    		return instance;
    	}
     
    	// Apply a function upon all members of the array. The function take a T in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, T> selectFunc ){ return Update( array, selectFunc, 0, array.Length-1 ); }
    	// Apply a function upon all members of the array between start and end included. The function take a T in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, T> selectFunc, int start, int end )
    	{
    		T[] result = (T[])array.Clone();
    		for( int i = start; i <= end; i++ )
    			result[i] = selectFunc(array[i]);			
     
    		return result;
    	}
    	// Apply a function upon all members of the array. The function take a T and the index in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, int, T> selectFunc ){ return Update( array, selectFunc, 0, array.Length-1 ); }
    	// Apply a function upon all members of the array between start and end included. The function take a T and the index in input and return a T
    	public static T[] Update<T>( T[] array, Func<T, int, T> selectFunc, int start, int end )
    	{
    		T[] result = (T[])array.Clone();
    		for( int i = start; i <= end; i++ )
    			result[i] = selectFunc(array[i], i);
     
    		return result;
    	}
}
}
