// Original url: http://wiki.unity3d.com/index.php/Distance_from_a_point_to_a_rectangle
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/MathGeometryUtilities/Distance_from_a_point_to_a_rectangle.cs
// File based on original modification date of: 30 March 2013, at 01:48. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.MathGeometryUtilities
{
Contents [hide] 
1 Function 
2 Background 
3 Usage 
4 Operation 
5 Original Author 
6 Code 
7 License 

Function This script computes the distance between a point and a rectangle. It's a 2D computation so it's assumed that the point and rectangle lie in a plane. If the point is inside the rectangle, the distance returned is 0. 
Background This is part of a larger framework I worked on called the Cygnet Engine. It's a math function, so I put it in a class called CygnetMath. I may release other parts of Cygnet in the future, but for now, this is the only part. 
Usage The class is designed for use in Unity's GUI coordinates. With this coordinate system, the origin is at the top left of the screen, x values increase from left to right, and y values increase as you go DOWN the screen. If you do some careful coordinate transformations, you probably should be able to use the code with an arbitrary coordinate system, but this was not the usage I designed it for, so beware. 
Operation There are several ways to compute the distance. This one determines if the point is either (a) within the rectangle (b) closest to an edge (c) closest to a corner. This is done by splitting the area around the rectangle into 9 regions, and doing a few branches. It may not be the most efficient way. If not, let me know or edit this page. 
Original Author Philip Peterson (i.e. who to send bug reports to) 
Code It's written in C#. Sorry. 
partial class CygnetMath {
    public static float DistancePointToRectangle(Vector2 point, Rect rect) {
        //  Calculate a distance between a point and a rectangle.
        //  The area around/in the rectangle is defined in terms of
        //  several regions:
        //
        //  O--x
        //  |
        //  y
        //
        //
        //        I   |    II    |  III
        //      ======+==========+======   --yMin
        //       VIII |  IX (in) |  IV
        //      ======+==========+======   --yMax
        //       VII  |    VI    |   V
        //
        //
        //  Note that the +y direction is down because of Unity's GUI coordinates.
 
        if (point.x < rect.xMin) { // Region I, VIII, or VII
            if (point.y < rect.yMin) { // I
                Vector2 diff = point - new Vector2(rect.xMin, rect.yMin);
                return diff.magnitude;
            }
            else if (point.y > rect.yMax) { // VII
                Vector2 diff = point - new Vector2(rect.xMin, rect.yMax);
                return diff.magnitude;
            }
            else { // VIII
                return rect.xMin - point.x;
            }
        }
        else if (point.x > rect.xMax) { // Region III, IV, or V
            if (point.y < rect.yMin) { // III
                Vector2 diff = point - new Vector2(rect.xMax, rect.yMin);
                return diff.magnitude;
            }
            else if (point.y > rect.yMax) { // V
                Vector2 diff = point - new Vector2(rect.xMax, rect.yMax);
                return diff.magnitude;
            }
            else { // IV
                return point.x - rect.xMax;
            }
        }
        else { // Region II, IX, or VI
            if (point.y < rect.yMin) { // II
                return rect.yMin - point.y;
            }
            else if (point.y > rect.yMax) { // VI
                return point.y - rect.yMax;
            }
            else { // IX
                return 0f;
            }
        }
    }
}License This code is released under the MIT license. Please provide attribution to the original author, Philip Peterson. 
}
