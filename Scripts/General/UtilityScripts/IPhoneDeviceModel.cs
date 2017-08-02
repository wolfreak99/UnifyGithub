/*************************
 * Original url: http://wiki.unity3d.com/index.php/IPhoneDeviceModel
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/IPhoneDeviceModel.cs
 * File based on original modification date of: 27 September 2013, at 11:46. 
 *
 * // Author: Charles M. Barros
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    The following plugin is an alternative for users of Unity 3.5 that need to identify new Apple mobile devices like the iPhone5S and iPhone5C. Using the device model as a string is also a good ideia if your application can download config files from a server or if you are using AssetBundles. This way you do not need to upload a new binary to Apple only to set some settings based on the user device. 
    ObjC - HwMachine.m Warning: This file must be placed at Plugins\iOS 
    // charlesbarros@gmail.com
     
    #include <sys/types.h>
    #include <sys/sysctl.h>
     
    const char * GetIPhoneHWMachine()
    {
    	size_t size;
    	sysctlbyname("hw.machine", NULL, &size, NULL, 0);
     
    	char *machine = (char *)malloc(size + 1);
     
    	sysctlbyname("hw.machine", machine, &size, NULL, 0);
    	machine[size] = 0;
     
    	return machine;
    }C# - DeviceUtils.cs // Author: Charles M. Barros
    // charlesbarros@gmail.com
     
    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
     
    public static class DeviceUtils
    {
    	private static IphoneModel _model = DeviceUtils.IphoneModel.Unknown;
     	private static string _modelString = string.Empty;
     
    	// Iphone Model
    	public enum IphoneModel
    	{
    		iPhone1,
    		iPhone3G,
    		iPhone3GS,
    		iPhone4,
    		iPhone4S,
    		iPhone5,
    		iPhone5C,
    		iPhone5S,
    		iPhoneUnknown,
     
    		iPod1Gen,
    		iPod2Gen,
    		iPod3Gen,
    		iPod4Gen,
    		iPod5Gen,
    		iPodUnknown,
     
    		iPad1,
    		iPad2,
    		iPad3, // The New iPad
    		iPad4, // iPad Retina
    		iPadMini,
    		iPadUnknown,
     
    		Unknown
    	}
     
    	#if UNITY_IPHONE
    	[DllImport ("__Internal")]
    	private static extern string GetIPhoneHWMachine();
    	#else
    	private static string GetIPhoneHWMachine() { return string.Empty; }
    	#endif
     
    	public static string GetIphoneModelString()
    	{
    		if (string.IsNullOrEmpty(_modelString) == true)
    		{
    			_modelString = GetIPhoneHWMachine();
    		}
     
    		return _modelString;
    	}
     
    	// Identifiers:
    	// http://theiphonewiki.com/wiki/Models
    	public static IphoneModel GetIphoneModel()
    	{
    		if (_model == DeviceUtils.IphoneModel.Unknown)
    		{
    			string deviceHWMachine = GetIphoneModelString();
     
    			if (deviceHWMachine.StartsWith("iPhone"))
    			{
    				_model = GetIphoneModel(deviceHWMachine);
    			}
    			else if (deviceHWMachine.StartsWith("iPod"))
    			{
    				_model = GetIpodModel(deviceHWMachine);
    			}
    			else if (deviceHWMachine.StartsWith("iPad"))
    			{
    				_model = GetIpadModel(deviceHWMachine);
    			}
    		}
     
    		return _model;
    	}
     
    	private static IphoneModel GetIphoneModel(string iPhoneHWMachine)
    	{
    		if (GetIphoneModelString().StartsWith("iPhone1,1") == true)
    		{
    			return DeviceUtils.IphoneModel.iPhone1;
    		}
    		else if (GetIphoneModelString().StartsWith("iPhone1,2") == true)
    		{
    			return DeviceUtils.IphoneModel.iPhone3G;
    		}
    		else if (GetIphoneModelString().StartsWith("iPhone2") == true)
    		{
    			return DeviceUtils.IphoneModel.iPhone3GS;
    		}
    		else if (GetIphoneModelString().StartsWith("iPhone3") == true)
    		{
    			return DeviceUtils.IphoneModel.iPhone4;
    		}
    		else if (GetIphoneModelString().StartsWith("iPhone4") == true)
    		{
    			return DeviceUtils.IphoneModel.iPhone4S;
    		}
    		else if (GetIphoneModelString().StartsWith("iPhone5,1") == true ||
    			 GetIphoneModelString().StartsWith("iPhone5,2") == true )
    		{
    			return DeviceUtils.IphoneModel.iPhone5;
    		}
    		else if (GetIphoneModelString().StartsWith("iPhone5,3") == true ||
    			 GetIphoneModelString().StartsWith("iPhone5,4") == true )
    		{
    			return DeviceUtils.IphoneModel.iPhone5C;
    		}
    		else if (GetIphoneModelString().StartsWith("iPhone6") == true)
    		{
    			return DeviceUtils.IphoneModel.iPhone5S;
    		}	
     
    		return DeviceUtils.IphoneModel.iPhoneUnknown;
    	}
     
    	private static IphoneModel GetIpodModel(string iPhoneHWMachine)
    	{
    		if (GetIphoneModelString().StartsWith("iPod1") == true)
    		{
    			return DeviceUtils.IphoneModel.iPod1Gen;
    		}
    		else if (GetIphoneModelString().StartsWith("iPod2") == true)
    		{
    			return DeviceUtils.IphoneModel.iPod2Gen;
    		}
    		else if (GetIphoneModelString().StartsWith("iPod3") == true)
    		{
    			return DeviceUtils.IphoneModel.iPod3Gen;
    		}
    		else if (GetIphoneModelString().StartsWith("iPod4") == true)
    		{
    			return DeviceUtils.IphoneModel.iPod4Gen;
    		}
    		else if (GetIphoneModelString().StartsWith("iPod5") == true)
    		{
    			return DeviceUtils.IphoneModel.iPod5Gen;
    		}
     
    		return DeviceUtils.IphoneModel.iPodUnknown;
    	}
     
    	private static IphoneModel GetIpadModel(string iPhoneHWMachine)
    	{
    		if (GetIphoneModelString().StartsWith("iPad1") == true)
    		{
    			return DeviceUtils.IphoneModel.iPad1;
    		}
    		else if (GetIphoneModelString().StartsWith("iPad2,5") == true ||
    			 GetIphoneModelString().StartsWith("iPad2,6") == true ||
    			 GetIphoneModelString().StartsWith("iPad2,7") == true)
    		{
    			return DeviceUtils.IphoneModel.iPadMini;
    		}
    		else if (GetIphoneModelString().StartsWith("iPad2") == true)
    		{
    			return DeviceUtils.IphoneModel.iPad2;
    		}
    		else if (GetIphoneModelString().StartsWith("iPad3,4") == true)
    		{
    			return DeviceUtils.IphoneModel.iPad4;
    		}
    		else if (GetIphoneModelString().StartsWith("iPad3") == true)
    		{
    			return DeviceUtils.IphoneModel.iPad3;
    		}
     
    		return DeviceUtils.IphoneModel.iPadUnknown;
    	}
}
}
