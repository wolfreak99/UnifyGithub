/*************************
 * Original url: http://wiki.unity3d.com/index.php/JavascriptMultiDimArrays
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/JavascriptMultiDimArrays.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Eric Haines (Eric5h5) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.GeneralConcepts
{
    
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Jagged Arrays 
    4 C# - MultiDim.cs 
    
    Description NOTE: This is largely obsolete in Unity 3.2, since multidimensional arrays can be declared directly in JS now. Please upgrade if you haven't already! However, the syntax for directly declaring the type of jagged multi-dimensional arrays is still missing in JS, so if you need those, this script can still be useful (though you can delete the rectangular array functions and just leave the jagged array functions). 
    Multi-dimensional (AKA rectangular) arrays in Javascript are a bit of a conundrum, prior to Unity 3.2. Ideally it should be possible to write something like "var foo = new int[5,6];". Alas, this results in a compiler error. However, it is possible to write this: 
    var heights = Terrain.activeTerrain.terrainData.GetHeights(0, 0, 10, 10);
    print (heights);The output, assuming a terrain is present, is "System.Single[,]" (AKA "float[,]"). And you can use the array as expected with no issues. So, clearly these sorts of arrays are implemented in Javascript, with the exception of the ability to declare them. This makes them only marginally useful, unless you're working with terrain data. 
    Or are they? Javascript uses type inference, so it's not necessary to specify the type if the compiler can figure it out. For example, "var foo = 5;" results in "foo" being declared as an int, since an int value was specified. Likewise, GetHeights() returns a float[,], so "heights" is declared as a float[,] through type inference. We can use this feature, together with the fact that there's no problem declaring rectangular arrays in C#, to allow us to declare arrays like this in Javascript as well with minimal fuss. 
    Usage Put the MultiDim script below in your Standard Assets/Scripts folder (if you don't have one, make one). This way it can be accessed from Javascript easily without having to worry about compilation order problems. Now you can use the MultiDim class and type inference to declare rectangular 2D and 3D arrays of ints, floats, and strings. For example: 
    var foo = MultiDim.IntArray(100, 200);
    foo[52, 49] = 123;
    var foo2 = MultiDim.IntArray(100, 200, 300);
    foo2[99, 199, 299] = 1;
    var numbers = MultiDim.FloatArray(50, 60);
    numbers[10, 20] = Mathf.PI;
    var someStuff = MultiDim.StringArray(10, 10, 10);
    someStuff[0, 0, 7] = "w00t";If you need to use additional types or dimensions, it should be pretty obvious how to make new functions in the MultiDim script that will return these, even if you don't know C#...just use the existing functions as a template. If you're using Unity types such as GameObject, put "using UnityEngine;" at the top of the script so the namespace is imported. 
    Jagged Arrays Along the same lines, jagged arrays (where each row can have a different number of columns) can be made in Javascript by using an array of arrays. But if you would rather use jagged built-in arrays for speed instead of using dynamic arrays, the method for being able to declare them directly is missing, though they can still be used. The work-around uses the same technique as above, so you can use JaggedInt, JaggedFloat, and JaggedString to declare these sorts of arrays by passing in a value representing the number of rows. Each row is null by default and must be declared individually. For example: 
    var foo = MultiDim.JaggedInt(3);
    foo[0] = new int[5];
    foo[1] = new int[21];
    foo[2] = new int[7];
    foo[1][19] = 55;It's also possible to declare jagged arrays using type inference with initial values: 
    var foo = [ [1, 2, 9], [4, 5, 2], [0, 0, 7] ];The type of "foo" in this case is System.Int32[][] (AKA int[][]). If you use this method, you don't need the MultiDim script. 
    C# - MultiDim.cs public class MultiDim {
     
    	public static int[,] IntArray (int a, int b) {
    		return new int[a,b];
    	}
     
    	public static int[,,] IntArray (int a, int b, int c) {
    		return new int[a,b,c];
    	}
     
    	public static float[,] FloatArray (int a, int b) {
    		return new float[a,b];
    	}
     
    	public static float[,,] FloatArray (int a, int b, int c) {
    		return new float[a,b,c];
    	}
     
    	public static string[,] StringArray (int a, int b) {
    		return new string[a,b];
    	}
     
    	public static string[,,] StringArray (int a, int b, int c) {
    		return new string[a,b,c];
    	}
     
    	public static int[][] JaggedInt (int a) {
    		return new int[a][];
    	}
     
    	public static float[][] JaggedFloat (int a) {
    		return new float[a][];
    	}
     
    	public static string[][] JaggedString (int a) {
    		return new string[a][];
    	}
}
}
