// Original url: http://wiki.unity3d.com/index.php/QuaternionExtensions
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MathHelpers/QuaternionExtensions.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MathHelpers
{

Author: Kieren Wallace (kdw.weebly.com) 


Contents [hide] 
1 Overview 
2 Usage 
3 Example - RotateObject.cs 
4 Code 

OverviewThis script provide a few useful extensions to the inbuilt 'Quaternion' struct. The Exponent, Magnitude, and Scalar Multiplication methods are useful for writing specialised Quaternion math functions, and the Power method is used to rotate a given quaternion an exact multiple of iteslf. 


UsageTo use these methods, simply append them to a given Quaternion. 
quaternion.Exp() - returns euler's number raised to quaternion 
quaternion.Magnitude() - returns the float magnitude of quaternion 
quaternion.ScalarMultipy(float scalar) - returns quaternion multiplied by scalar 
quaternion.Pow(float pow) - returns quaternion raised to the power pow. This is useful for smoothly multiplying a Quaternion by a given floating-point value. 
Example - RotateObject.csThis script will rotate the gameObject by rotateOffset's local rotation once per second. 
public class RotateObject : MonoBehaviour
{
    public Transform rotateOffset;
 
    void Update()
    {
        transform.rotation = rotateOffset.localRotation.Pow(Time.time);
    }
}Codepublic static class QuaternionExtensions
{
	public static Quaternion Pow(this Quaternion input, float power)
	{
		float inputMagnitude = input.Magnitude();
		Vector3 nHat = new Vector3(input.x, input.y, input.z).normalized;
		Quaternion vectorBit = new Quaternion(nHat.x, nHat.y, nHat.z, 0)
			.ScalarMultiply(power * Mathf.Acos(input.w / inputMagnitude))
				.Exp();
		return vectorBit.ScalarMultiply(Mathf.Pow(inputMagnitude, power));
	}
 
	public static Quaternion Exp(this Quaternion input)
	{
		float inputA = input.w;
		Vector3 inputV = new Vector3(input.x, input.y, input.z);
		float outputA = Mathf.Exp(inputA) * Mathf.Cos(inputV.magnitude);
		Vector3 outputV = Mathf.Exp(inputA) * (inputV.normalized * Mathf.Sin(inputV.magnitude));
		return new Quaternion(outputV.x, outputV.y, outputV.z, outputA);
	}
 
	public static float Magnitude(this Quaternion input)
	{
		return Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z + input.w * input.w);
	}
 
	public static Quaternion ScalarMultiply(this Quaternion input, float scalar)
	{
		return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
	}
}
}
