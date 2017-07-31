// Original url: http://wiki.unity3d.com/index.php/DirectionFinder
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MathHelpers/DirectionFinder.cs
// File based on original modification date of: 27 March 2014, at 15:10. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MathHelpers
{
Author: Will Traxler 
This is a short script to determine the general two-dimensional direction between two vector3 points. 
The script takes the two vector3 points and returns one of eight directions: Up, Down, Left, Right, UpLeft, UpRight, DownLeft and DownRight. 
 
 
public static DirectionType GetGeneralDirection(Vector3 startingVector, Vector3 endingVector) {
 
 
		float bestDotProduct = -Mathf.Infinity;
 
		DirectionType closestDirection = DirectionType.None;
		Vector3 normalizedDirection = (endingVector - startingVector).normalized;
 
		//Loops the direction types and returns a quadrant (upleft,upright,downleft,downright)
		foreach (DirectionType thisDirection in System.Enum.GetValues(typeof(DirectionType))) {
			Vector3 vectorDirection = GetVector3(thisDirection);
			float dotProduct = Vector3.Dot(normalizedDirection, vectorDirection);
			if (dotProduct > bestDotProduct ) {
				closestDirection = thisDirection;
				bestDotProduct = dotProduct;
			}
		}
 
		//Manually adjust for direct angles (u,d,l,r) which are not reported above
		float allowedAngle = 0.25f;
 
		float absX = Mathf.Abs(normalizedDirection.x);
		float absY = Mathf.Abs(normalizedDirection.y);
 
		switch(closestDirection) {
 
		case(DirectionType.UpLeft):
			if (absY < allowedAngle) return DirectionType.Left;
			if (absX < allowedAngle) return DirectionType.Up;
			break;
		case(DirectionType.UpRight):
			if (absY < allowedAngle) return DirectionType.Right;
			if (absX < allowedAngle) return DirectionType.Up;
			break;
		case(DirectionType.DownLeft):
			if (absY < allowedAngle) return DirectionType.Left;
			if (absX < allowedAngle) return DirectionType.Down;
			break;
		case(DirectionType.DownRight):
			if (absY < allowedAngle) return DirectionType.Right;
			if (absX < allowedAngle) return DirectionType.Down;
			break;
 
		}
		return closestDirection;
 
	}Here is the direction type enum which is returned: 
public enum DirectionType { Left, Right, Up, UpLeft, UpRight, Down, DownLeft, DownRight, None }
}
