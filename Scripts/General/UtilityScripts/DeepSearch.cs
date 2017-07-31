// Original url: http://wiki.unity3d.com/index.php/DeepSearch
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/DeepSearch.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
This article is a stub. 
You can help UnifyWiki by expanding it. 

JS Scriptstatic function deepSearch (t : Transform, s : String) : Transform
{
	var	dt : Transform = t.Find (s);
	if (dt)	
		return dt;
	else
	{
		for (var child : Transform in t) {
			dt = deepSearch (child, s);
			if (dt) 
				return dt;
		}
	}	
	return null;
}C# Script   static public Transform deepSearch(Transform t, string s)
   {
       Transform dt = t.Find(s);
       if (dt != null)
           return dt;
       else
       {
           foreach (Transform child in t)
           {
               dt = deepSearch(child, s);
               if (dt != null)
                   return dt;
           }
       }
       return null;
   }
}
