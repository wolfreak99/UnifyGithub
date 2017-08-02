/*************************
 * Original url: http://wiki.unity3d.com/index.php/Low_Pass_Filter
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/Low_Pass_Filter.cs
 * File based on original modification date of: 7 March 2012, at 04:36. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.CodeSnippets
{
    Usage This function filters out high frequency jitter caused by hardware sampling such as accelerometer output or Augmented reality marker pose output. In most cases a generic low pass filter can be used to get rid of jitter but this will not work for rotations because of the transition between 360 and 1 degrees. If this transition is not taken into account, violent movements are the result when operating at this range. To counteract this, a Quaternion.Lerp function is used. 
    Note that the modified output needs to be stored outside of the function as a global variable. Also, the previous value should be initialized to the current value at the first iteration to prevent jumps. 
    Rotational Low Pass Filter //Initialization done once:
    float lowPassFactor = 0.8f; //Value should be between 0.01f and 0.99f. Smaller value is more damping.
    bool init = true;
     
    //Called every frame:
    gameObjectFiltered.transform.rotation = lowPassFilterQuaternion(gameObjectFiltered.transform.rotation , gameObject.transform.rotation, lowPassFactor, init);
    init = false
     
    //Function:
    Quaternion lowPassFilterQuaternion(Quaternion intermediateValue, Quaternion targetValue, float factor, bool init){
     
    	//intermediateValue needs to be initialized at the first usage.
    	if(init){		
    		intermediateValue = targetValue;
    	}
     
    	intermediateValue = Quaternion.Lerp(intermediateValue, targetValue, factor);
    	return intermediateValue;
    }Generic Low Pass Filter The output of this function is the modified intermediate value. The input is "targetValue", and "intermediateValueBuf" needs to be stored outside of the function as a global variable. 
    Vector3 lowPassFilter(Vector3 targetValue, ref Vector3 intermediateValueBuf, float factor, bool init){
     
    	Vector3 intermediateValue;
     
    	//intermediateValue needs to be initialized at the first usage.
    	if(init){
    		intermediateValueBuf = targetValue;
    	}
     
    	intermediateValue.x = (targetValue.x * factor) + (intermediateValueBuf.x * (1.0f - factor)); 
    	intermediateValue.y = (targetValue.y * factor) + (intermediateValueBuf.y * (1.0f - factor));
    	intermediateValue.z = (targetValue.z * factor) + (intermediateValueBuf.z * (1.0f - factor));
     
    	intermediateValueBuf = intermediateValue;
     
    	return intermediateValue;
    }
}
