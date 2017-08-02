/*************************
 * Original url: http://wiki.unity3d.com/index.php/PrintExtended
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/PrintExtended.cs
 * File based on original modification date of: 23 March 2013, at 20:10. 
 *
 * Author: Jake Bayer (BakuJake14) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    
    Contents [hide] 
    1 Description 
    2 Usage 
    2.1 Printing a warning 
    2.2 Printing an error 
    3 PrintExtended.cs 
    
    DescriptionAdditional methods that go with the print function. 
    UsageWorks the same way as the print function that inherits from MonoBehaviour. 
    Printing a warning	public void TestExample() {
    		float dist = Vector3.Distance(transform.position, light.transform.position);
     
    		if(dist < 3) {
    			PrintExtended.printWarning("my position from the light is " + dist);
    		}
    	}Printing an error	public void FollowAI() {
    		Transform target = transform.parent.transform;
     
    		if(target == null) {
    			PrintExtended.printError(target.name + " could not be found");
    		}
    	}PrintExtended.csusing UnityEngine;
    using System.Collections;
     
    public class PrintExtended : MonoBehaviour {
     
    	public static void printWarning(object message) {
    		Debug.LogWarning(message);
    	}
    	public static void printError(object message) {
    		Debug.LogError(message);
    	}
}
}
