// Original url: http://wiki.unity3d.com/index.php/Calculating_Lead_For_Projectiles
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/Calculating_Lead_For_Projectiles.cs
// File based on original modification date of: 19 March 2014, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
Author: Daniel Brauer 
These functions calculate an intercept point based on the current positions and constant velocities of a shooter and a target, along with a projectile speed. They're really good for shooting asteroids with machine guns, but won't work as well for targets that can dodge. 
How to use the intercept function This is an example of how to call FirstOrderIntercept. The result is that interceptPoint is the point in space where the projectile can hit the target. If you fire a projectile at that point, and the target doesn't accelerate, you'll hit it (except when hitting it is impossible). 
// === variables you need ===
//how fast our shots move
float shotSpeed;
//objects
GameObject shooter;
GameObject target;
 
// === derived variables ===
//positions
Vector3 shooterPosition = shooter.transform.position;
Vector3 targetPosition = target.transform.position;
//velocities
Vector3 shooterVelocity = shooter.rigidbody ? shooter.rigidbody.velocity : Vector3.zero;
Vector3 targetVelocity = target.rigidbody ? target.rigidbody.velocity : Vector3.zero;
 
//calculate intercept
Vector3 interceptPoint = FirstOrderIntercept
(
	shooterPosition,
	shooterVelocity,
	shotSpeed,
	targetPosition,
	targetVelocity
);
//now use whatever method to launch the projectile at the intercept pointCalculating the intercept point This code is based on solving the first order (velocity, no acceleration) version of the problem. It properly handles the situation where the target's relative velocity and the shot velocity are equal, which causes a singularity if not dealt with specially. Also, it returns the target's current position if there are no positive solutions. This is because it's not always possible to intercept a target. For example, the target may be moving away from the shooter with a velocity higher than the shot velocity. 
//first-order intercept using absolute target position
public static Vector3 FirstOrderIntercept
(
	Vector3 shooterPosition,
	Vector3 shooterVelocity,
	float shotSpeed,
	Vector3 targetPosition,
	Vector3 targetVelocity
)  {
	Vector3 targetRelativePosition = targetPosition - shooterPosition;
	Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
	float t = FirstOrderInterceptTime
	(
		shotSpeed,
		targetRelativePosition,
		targetRelativeVelocity
	);
	return targetPosition + t*(targetRelativeVelocity);
}
//first-order intercept using relative target position
public static float FirstOrderInterceptTime
(
	float shotSpeed,
	Vector3 targetRelativePosition,
	Vector3 targetRelativeVelocity
) {
	float velocitySquared = targetRelativeVelocity.sqrMagnitude;
	if(velocitySquared < 0.001f)
		return 0f;
 
	float a = velocitySquared - shotSpeed*shotSpeed;
 
	//handle similar velocities
	if (Mathf.Abs(a) < 0.001f)
	{
		float t = -targetRelativePosition.sqrMagnitude/
		(
			2f*Vector3.Dot
			(
				targetRelativeVelocity,
				targetRelativePosition
			)
		);
		return Mathf.Max(t, 0f); //don't shoot back in time
	}
 
	float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
	float c = targetRelativePosition.sqrMagnitude;
	float determinant = b*b - 4f*a*c;
 
	if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
		float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
				t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
		if (t1 > 0f) {
			if (t2 > 0f)
				return Mathf.Min(t1, t2); //both are positive
			else
				return t1; //only t1 is positive
		} else
			return Mathf.Max(t2, 0f); //don't shoot back in time
	} else if (determinant < 0f) //determinant < 0; no intercept path
		return 0f;
	else //determinant = 0; one intercept path, pretty much never happens
		return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
}License The MIT License (MIT) 
Copyright (c) 2008 Daniel Brauer 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
}
