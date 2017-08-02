/*************************
 * Original url: http://wiki.unity3d.com/index.php/Perlin_Noise
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Perlin_Noise.cs
 * File based on original modification date of: 23 October 2012, at 04:01. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Description The following snippet shows how to use noise for smoothly randomly moving objects, rotating them or animated light intensity. 
    Usage Download the package. Open Unity with your own project folder. Double click on the unityPackage in the finder. [PerlinNoise] 
    This installs the perlin noise class and 3 scripts implementing: 
    Animated light intensity noise 
    Animated noise position 
    Animated noise rotation 
    To use one of the scripts, just drag them on a game object, to make them move, rotate, animate light based on noise. 
    Another function previous information above is out of date links here are 4 new functions including voronoi on a pretty free license UnityScript noises on forum 
    Also here is a nearly complete implementation of Perlin noise in UnityScript,although I didn't finish completing it because I didn't understand JavaScript this.noise function. I am sure I will finish it or else you can help if you want the original is here- JavaScript Perlin noise implementation 
    #pragma strict
     
     
    // This is a port of Ken Perlin's Java code. The
    // original Java code is at http://cs.nyu.edu/%7Eperlin/noise/.
    // Note that in this version, a number from 0 to 1 is returned.
    function PerlinNoise() {
    var x :int;
    var y :int; 
    var z :int;
    var t :int;
    var a :int;
    var b :int;
    var n :int;
    var hash :int;
    	 thisnoise (x, y, z);
    	 fade(t);
    	 lerp( t, a, b);
    	 grad(hash, x, y, z);
         scale(n);}
     
    function thisnoise (x:int, y:int, z:int) {
     
       var p : int[]; //array
       p = new int[512];
       var permutation : int[] = [ 151,160,137,91,90,15,
       131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
       190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
       88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
       77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
       102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
       135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
       5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
       223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
       129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
       251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
       49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
       138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
       ];
       for (var i=0; i < 256 ; i++) 
     p[256+i] = p[i] = permutation[i]; 
     
         var X = parseInt(Mathf.Floor(x)) & 255;                  // FIND UNIT CUBE THAT
    	 var Y = parseInt(Mathf.Floor(y)) & 255;                // CONTAINS POINT.
    	 var Z = parseInt(Mathf.Floor(z)) & 255;
          x -= Mathf.Floor(x);                                // FIND RELATIVE X,Y,Z
          y -= Mathf.Floor(y);                                // OF POINT IN CUBE.
          z -= Mathf.Floor(z);
          var    u = fade(x);                               // COMPUTE FADE CURVES
           var      v = fade(y);                               // FOR EACH OF X,Y,Z.
          var       w = fade(z);
          var A = p[X  ]+Y;var AA = p[A]+Z;var AB = p[A+1]+Z;      // HASH COORDINATES OF
             var B = p[X+1]+Y;var BA = p[B]+Z;var BB = p[B+1]+Z;      // THE 8 CUBE CORNERS,
     
          return scale(lerp(w, lerp(v, lerp(u, grad(p[AA  ], x  , y  , z   ),  // AND ADD
                                         grad(p[BA  ], x-1, y  , z   )), // BLENDED
                                 lerp(u, grad(p[AB  ], x  , y-1, z   ),  // RESULTS
                                         grad(p[BB  ], x-1, y-1, z   ))),// FROM  8
                         lerp(v, lerp(u, grad(p[AA+1], x  , y  , z-1 ),  // CORNERS
                                         grad(p[BA+1], x-1, y  , z-1 )), // OF CUBE
                                 lerp(u, grad(p[AB+1], x  , y-1, z-1 ),
                                         grad(p[BB+1], x-1, y-1, z-1 )))));
       }
       function fade(t:int) { return t * t * t * (t * (t * 6 - 15) + 10); }
       function lerp( t:int, a:int, b:int) { return a + t * (b - a); }
       function grad(hash:int, x:int, y:int, z:int) {
          var h = hash & 15;                      // CONVERT LO 4 BITS OF HASH CODE
          var u = h<8 ? x : y;                 // INTO 12 GRADIENT DIRECTIONS.
               var  v = h<4 ? y : h==12||h==14 ? x : z;
          return ((h&1) == 0 ? u : -u) + ((h&2) == 0 ? v : -v);
       } 
       function scale(n:int) { return (1 + n)/2; }
}
