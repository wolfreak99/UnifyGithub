/*************************
 * Original url: http://wiki.unity3d.com/index.php/DeepSearch
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/DeepSearch.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This article is a stub. 
 * You can help UnifyWiki by expanding it. 
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    using UnityEngine;

    public class DeepSearch
    {
        static public Transform deepSearch(Transform t, string s)
        {
            Transform dt = t.Find(s);
            if (dt != null)
                return dt;
            else {
                foreach (Transform child in t) {
                    dt = deepSearch(child, s);
                    if (dt != null)
                        return dt;
                }
            }
            return null;
        }
    }
}
