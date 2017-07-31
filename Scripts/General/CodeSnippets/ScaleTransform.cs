// Original url: http://wiki.unity3d.com/index.php/ScaleTransform
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/ScaleTransform.cs
// File based on original modification date of: 15 February 2013, at 09:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
The ScaleTransform method allows you to scale a GameObject transform to a specific size along the x & z axis by taking into account the GameObjects renderer bounds if a renderer component is attached. Also takes into account any children of the transform. 
        /// <summary>
        /// Scales a transform to specific dimensions along the x & z axis.
        /// </summary>
        /// <param name="transform">
        /// Reference to the transform to scale.
        /// </param>
        /// <param name="width">The width along the x axis that represents the target size.</param>
        /// <param name="height">The height along the z axis that represents the target size.</param>
        public static void ScaleTransform(Transform transform, float width, float height)
        {
            // get bounds of the prefab
            var bounds = new Bounds();
            var encapsulate = false;
            if (!Utilities.Helpers.GetBoundWithChildren(transform, ref bounds, ref encapsulate))
            {
                return;
            }
 
            // get minimum size from the size dimensions
            var min = Mathf.Min(width, height);
 
            // get the maximum x or z size of the transform
            var max = Mathf.Max(bounds.size.x, bounds.size.z);
 
            // calculate the scale factor 
            var scaleFactor = min / max;
 
            // apply scaling to the transform
            transform.localScale *= scaleFactor;
        }
 
        /// <summary>
        /// Gets the rendering bounds of the transform.
        /// </summary>
        /// <param name="transform">The game object to get the bounding box for.</param>
        /// <param name="pBound">The bounding box reference that will </param>
        /// <param name="encapsulate">Used to determine if the first bounding box to be 
        /// calculated should be encapsulated into the <see cref="pBound"/> argument.</param>
        /// <returns>Returns true if at least one bounding box was calculated.</returns>
        public static bool GetBoundWithChildren(Transform transform, ref Bounds pBound, ref bool encapsulate)
        {
            var didOne = false;
 
            // get 'this' bound
            if (transform.gameObject.renderer != null)
            {
                var bound = transform.gameObject.renderer.bounds;
                if (encapsulate)
                {
                    pBound.Encapsulate(bound.min);
                    pBound.Encapsulate(bound.max);
                }
                else
                {
                    pBound.min = bound.min; 
                    pBound.max = bound.max; 
                    encapsulate = true;
                }
 
                didOne = true;
            }
 
            // union with bound(s) of any/all children
            foreach (Transform child in transform)
            {
                if (GetBoundWithChildren(child, ref pBound, ref encapsulate))
                {
                    didOne = true;
                }
            }
 
            return didOne;
        }
}
