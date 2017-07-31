// Original url: http://wiki.unity3d.com/index.php/Averaging_Quaternions_and_Vectors
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/Averaging_Quaternions_and_Vectors.cs
// File based on original modification date of: 4 May 2013, at 09:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
Quaternion Averaging This code will calculate the average rotation of various separate quaternions. If the total amount of rotations are unknown, the algorithm will still work. The foreach loop will output a valid average value with each loop cycle. 
Note: this code will only work if the separate quaternions are relatively close to each other. Also note that it might not be necessary to normalize the end result. 
This method is based on a simplified procedure described in this document: http://ntrs.nasa.gov/archive/nasa/casi.ntrs.nasa.gov/20070017872_2007014421.pdf Special thanks to "Titos" for doing the maths. 
//Get an average (mean) from more then two quaternions (with two, slerp would be used).
//Note: this only works if all the quaternions are relatively close together.
//Usage: 
//-Cumulative is an external Vector4 which holds all the added x y z and w components.
//-newRotation is the next rotation to be added to the average pool
//-firstRotation is the first quaternion of the array to be averaged
//-addAmount holds the total amount of quaternions which are currently added
//This function returns the current average quaternion
public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount){
 
	float w = 0.0f;
	float x = 0.0f;
	float y = 0.0f;
	float z = 0.0f;
 
	//Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
	//q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
	if(!Math3d.AreQuaternionsClose(newRotation, firstRotation)){
 
		newRotation = Math3d.InverseSignQuaternion(newRotation);	
	}
 
	//Average the values
	float addDet = 1f/(float)addAmount;
	cumulative.w += newRotation.w;
	w = cumulative.w * addDet;
	cumulative.x += newRotation.x;
	x = cumulative.x * addDet;
	cumulative.y += newRotation.y;
	y = cumulative.y * addDet;
	cumulative.z += newRotation.z;
	z = cumulative.z * addDet;		
 
	//note: if speed is an issue, you can skip the normalization step
	return NormalizeQuaternion(x, y, z, w);
}
 
public static Quaternion NormalizeQuaternion(float x, float y, float z, float w){
 
	float lengthD = 1.0f / (w*w + x*x + y*y + z*z);
	w *= lengthD;
	x *= lengthD;
	y *= lengthD;
	z *= lengthD;
 
	return new Quaternion(x, y, z, w);
}
 
//Changes the sign of the quaternion components. This is not the same as the inverse.
public static Quaternion InverseSignQuaternion(Quaternion q){
 
	return new Quaternion(-q.x, -q.y, -q.z, -q.w);
}
 
//Returns true if the two input quaternions are close to each other. This can
//be used to check whether or not one of two quaternions which are supposed to
//be very similar but has its component signs reversed (q has the same rotation as
//-q)
public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2){
 
	float dot = Quaternion.Dot(q1, q2);
 
	if(dot < 0.0f){
 
		return false;					
	}
 
	else{
 
		return true;
	}
}Vector Averaging This code will calculate the average vector (which can be a position) of various separate vectors. If the total amount of vectors are unknown, the algorithm will still work. The foreach loop will output a valid average value with each loop cycle. 
//Global variable which holds the amount of rotations which 
//need to be averaged.
int addAmount = 0;
 
//Global variable which represents the additive vector
Vector3 addedVector = Vector3.zero;
 
//The averaged vector
Vector3 averageVector;
 
//multipleRotations is an array which holds all the vectors
//which need to be averaged.
Vector3[] multipleVectors new Vector3[totalAmount];
 
//Loop through all the vectors
foreach(Vector3 singleVector in multipleVectors){
 
	//Amount of separate rotational values so far
	addAmount++;
 
	addedVector += singleVector;
 
	//The result is valid right away, without 
	//first going through the entire array.
	averageVector = addedVector / (float)addAmount;
}
}
