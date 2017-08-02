/*************************
 * Original url: http://wiki.unity3d.com/index.php/ScrollingBG
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/ScrollingBG.cs
 * File based on original modification date of: 18 August 2014, at 14:48. 
 *
 * Author: Luke Davenport 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    DescriptionHandles automated scrolling backgrounds that use the Sprite system 
    Usage Place all the sprites you want to scroll inside an empty GameObject. 
    ScrollingBG.cs // 
     
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
     
    public class ScrollingBG : MonoBehaviour
    {
     
        public float scrollSpeed = 1f;
     
        //example value, for this I had 7 sprites that were 2 world units each
        public float repeatLimit = -14;
        private List<Transform> tiles;
     
        //populate list on startup
        void Start()
        {
            tiles = new List<Transform>();
     
            foreach (Transform child in transform)
            {
                tiles.Add(child);
            }
     
            tiles = tiles.OrderBy(t => t.position.x).ToList();
        }
     
        void Update()
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].position.x < repeatLimit)
                {
                    Transform firstTile = tiles[0];
                    Transform lastTile = tiles[tiles.Count -1];
     
                    firstTile.transform.position = new Vector3(lastTile.transform.position.x + firstTile.renderer.bounds.size.x,
                                                               firstTile.transform.position.y, 
                                                               firstTile.transform.position.z);
     
                    tiles.Remove(firstTile);
                    tiles.Add(firstTile);
                }
     
                tiles[i].Translate(new Vector3(-scrollSpeed * Time.deltaTime, 0, 0));
            }
        }
     
}
}
