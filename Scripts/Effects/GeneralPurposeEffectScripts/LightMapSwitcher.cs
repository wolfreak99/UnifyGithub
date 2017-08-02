/*************************
 * Original url: http://wiki.unity3d.com/index.php/LightMapSwitcher
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/LightMapSwitcher.cs
 * File based on original modification date of: 3 December 2013, at 09:04. 
 *
 * Author: Kspr 
 *
 * Description 
 *   
 * Usage 
 *   
 * LightMapSwitcher 
 *   
 * NaturalSortComparer 
 *   
 * Notes 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Description This is a MonoBehavior that takes four arrays of LightMaps, and allows you to switch them at runtime. It allows you to blindly drag-and drop the file list into the array, as the code will sort it by name of the file. 
    Usage In this example, LightMapSwitcher is a MonoBehavior which should be attached to a GameObject in the scene. 
    LightMapSwitcherusing UnityEngine;
     
    using System.Linq;
     
    public class LightMapSwitcher : MonoBehaviour
    {
        public Texture2D[] DayNear;
        public Texture2D[] DayFar;
        public Texture2D[] NightNear;
        public Texture2D[] NightFar;
     
        private LightmapData[] dayLightMaps;
        private LightmapData[] nightLightMaps;
     
        void Start ()
        {
            if ((DayNear.Length != DayFar.Length) || (NightNear.Length != NightFar.Length))
            {
                Debug.Log("In order for LightMapSwitcher to work, the Near and Far LightMap lists must be of equal length");
                return;
            }
     
            // Sort the Day and Night arrays in numerical order, so you can just blindly drag and drop them into the inspector
            DayNear = DayNear.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
            DayFar = DayFar.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
            NightNear = NightNear.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
            NightFar = NightFar.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
     
            // Put them in a LightMapData structure
            dayLightMaps = new LightmapData[DayNear.Length];
            for (int i=0; i<DayNear.Length; i++)
            {
                dayLightMaps[i] = new LightmapData();
                dayLightMaps[i].lightmapNear = DayNear[i];
                dayLightMaps[i].lightmapFar = DayFar[i];
            }
     
            nightLightMaps = new LightmapData[NightNear.Length];
            for (int i=0; i<NightNear.Length; i++)
            {
                nightLightMaps[i] = new LightmapData();
                nightLightMaps[i].lightmapNear = NightNear[i];
                nightLightMaps[i].lightmapFar = NightFar[i];
            }
        }
     
        #region Publics
        public void SetToDay()
        {
            LightmapSettings.lightmaps = dayLightMaps;
        }
     
        public void SetToNight()
        {
            LightmapSettings.lightmaps = nightLightMaps;
        }
        #endregion
     
        #region Debug
        [ContextMenu ("Set to Night")]
        void Debug00()
        {
            SetToNight();
        }
     
        [ContextMenu ("Set to Day")]
        void Debug01()
        {
            SetToDay();
        }
        #endregion
    }NaturalSortComparer using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
     
    // From http://zootfroot.blogspot.dk/2009/09/natural-sort-compare-with-linq-orderby.html
    public class NaturalSortComparer<T> : IComparer<string>, IDisposable
    {
        private readonly bool isAscending;
     
        public NaturalSortComparer(bool inAscendingOrder = true)
        {
            this.isAscending = inAscendingOrder;
        }
     
        #region IComparer<string> Members
        public int Compare(string x, string y)
        {
            throw new NotImplementedException();
        }
        #endregion
     
        #region IComparer<string> Members
        int IComparer<string>.Compare(string x, string y)
        {
            if (x == y)
                return 0;
     
            string[] x1, y1;
     
            if (!table.TryGetValue(x, out x1))
            {
                x1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
                table.Add(x, x1);
            }
     
            if (!table.TryGetValue(y, out y1))
            {
                y1 = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
                table.Add(y, y1);
            }
     
            int returnVal;
     
            for (int i = 0; i < x1.Length && i < y1.Length; i++)
            {
                if (x1[i] != y1[i])
                {
                    returnVal = PartCompare(x1[i], y1[i]);
                    return isAscending ? returnVal : -returnVal;
                }
            }
     
            if (y1.Length > x1.Length)
            {
                returnVal = 1;
            }
            else if (x1.Length > y1.Length)
            {
                returnVal = -1;
            }
            else
            {
                returnVal = 0;
            }
     
            return isAscending ? returnVal : -returnVal;
        }
     
        private static int PartCompare(string left, string right)
        {
            int x, y;
            if (!int.TryParse(left, out x))
                return left.CompareTo(right);
     
            if (!int.TryParse(right, out y))
                return left.CompareTo(right);
     
            return x.CompareTo(y);
        }
        #endregion
     
        private Dictionary<string, string[]> table = new Dictionary<string, string[]>();
     
        public void Dispose()
        {
            table.Clear();
            table = null;
        }
}Notes Per the documentation, the LightMap array can be at max 253 elements. (http://docs.unity3d.com/Documentation/ScriptReference/LightmapSettings.html) 
}
