// Original url: http://wiki.unity3d.com/index.php/CameraFog
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/CameraFog.cs
// File based on original modification date of: 3 April 2015, at 09:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Description This is a simple class that, when added to a GameObject with a camera, allows you to control the fog settings for that camera separately from the global ones. 
Unity 5 package demo is available here CameraFog.unitypackage (46.66 kb) and here File:CameraFog.zip 
 
CSharp - CameraFog.cs // Per-camera fog
//
// I'd love to hear from you if you do anything cool with this or have any suggestions :)
// www.tenebrous.co.uk
// =============
// Code updated and tested in unity 5 by: Dean aka (Created by: X) 
// Unity Wiki user: http://wiki.unity3d.com/index.php/User:Createdbyx
// Homepage: http://www.createdbyx.com/
 
    using UnityEngine;
 
    /// <summary>
    /// Modifies a camera to allows you to control the fog settings for that camera separately from the global scene fog or other cameras. 
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class CameraFog : MonoBehaviour
    {
        /// <summary>
        /// The enabled state weather or not fog will be visible.
        /// </summary>
        public bool Enabled;
 
        /// <summary>
        /// The start distance from the camera where the fog will be drawn.
        /// </summary>
        public float StartDistance;
 
        /// <summary>
        /// The end distance from the camera where the fog will be drawn.
        /// </summary>
        public float EndDistance;
 
        /// <summary>
        /// The fog mode that controls how the fog is rendered.
        /// </summary>
        public FogMode Mode;
 
        /// <summary>
        /// The density of the fog that is rendered.
        /// </summary>
        public float Density;
 
        /// <summary>
        /// The fog color.
        /// </summary>
        public Color Color;
 
        /// <summary>
        /// Stores the pre-render state of the start distance.
        /// </summary>
        private float _startDistance;
 
        /// <summary>
        /// Stores the pre-render state of the end  distance.
        /// </summary>
        private float _endDistance;
 
        /// <summary>
        /// Stores the pre-render state of the fog mode.
        /// </summary>
        private FogMode _mode;
 
        /// <summary>
        /// Stores the pre-render state of the density.
        /// </summary>
        private float _density;
 
        /// <summary>
        /// Stores the pre-render state of the fog color.
        /// </summary>
        private Color _color;
 
        /// <summary>
        /// Stores the pre-render state wheather or not the fog is enabled.
        /// </summary>
        private bool _enabled;
 
        /// <summary>
        /// Event that is fired before any camera starts rendering.
        /// </summary>
        private void OnPreRender()
        {
            this._startDistance = RenderSettings.fogStartDistance;
            this._endDistance = RenderSettings.fogEndDistance;
            this._mode = RenderSettings.fogMode;
            this._density = RenderSettings.fogDensity;
            this._color = RenderSettings.fogColor;
            this._enabled = RenderSettings.fog;
 
            RenderSettings.fog = this.Enabled;
            RenderSettings.fogStartDistance = this.StartDistance;
            RenderSettings.fogEndDistance = this.EndDistance;
            RenderSettings.fogMode = this.Mode;
            RenderSettings.fogDensity = this.Density;
            RenderSettings.fogColor = this.Color;
        }
 
        /// <summary>
        /// Event that is fired after any camera finishes rendering.
        /// </summary>
        private void OnPostRender()
        {
            RenderSettings.fog = this._enabled;
            RenderSettings.fogStartDistance = this._startDistance;
            RenderSettings.fogEndDistance = this._endDistance;
            RenderSettings.fogMode = this._mode;
            RenderSettings.fogDensity = this._density;
            RenderSettings.fogColor = this._color;
        }
    }
}
