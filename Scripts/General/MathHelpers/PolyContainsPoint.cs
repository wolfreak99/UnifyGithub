// Original url: http://wiki.unity3d.com/index.php/PolyContainsPoint
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MathHelpers/PolyContainsPoint.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MathHelpers
{
Author: Eric Haines (Eric5h5) 
DescriptionRect.Contains got you down? You're feeling the need for more points in your polygons? Fear not, PolyContainsPoint is here to save the day! That's right, now you can test to see if a given point is inside any kind of polygon you can think of. Say goodbye to boring old rectangles! 
Usage Have this script somewhere in your project. Call the function by the name of the script (let's call it "Poly") plus ".ContainsPoint". It takes an array of Vector2s describing the polygon and a single Vector2 containing the point you want to test, and returns a boolean. A code example: 
// Array of points making up polygon
var polygonArray = [Vector2(0.0, 1.0), Vector2(0.0, 2.0), Vector2(2.0, 2.0), Vector2(2.0, 1.0), Vector2(1.0, 0.0)];
var point = Vector2(0.5, 0.5);   // See if this point is inside the polygon
 
function Start () {
   if (Poly.ContainsPoint(polygonArray, point)) {
      print ("Inside! Yay, I'm safe!");
   }
   else {
      print ("Outside! Brr, cold out here!");
   }
}Poly.js static function ContainsPoint (polyPoints : Vector2[], p : Vector2) : boolean { 
   var j = polyPoints.Length-1; 
   var inside = false; 
   for (i = 0; i < polyPoints.Length; j = i++) { 
      if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
         (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
         inside = !inside; 
   } 
   return inside; 
}
}
