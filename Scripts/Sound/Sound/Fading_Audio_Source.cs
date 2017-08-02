/*************************
 * Original url: http://wiki.unity3d.com/index.php/Fading_Audio_Source
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Sound/Sound/Fading_Audio_Source.cs
 * File based on original modification date of: 23 May 2014, at 15:40. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Sound.Sound
{
    Attaching the following script to your AudioSource allows you to fade between audio clips instead of changing them instantly. Set FadeSpeed to the desired volume change per second when fading, and FadeOutThreshold to the volume to end the previous clip at. 
    Calling the Fade method of the FadingAudioSource will 
    fade out the current clip and fade in the specified one, after, if the audio source is enabled and playing. 
    fade in the specified clip immediately, if the audio source is enabled and not playing. 
    fade in the specified clip as soon as the audio source gets enabled, if it is not enabled. 
    Source Code     using UnityEngine;
     
        /// <summary>
        ///   Audio source that fades between clips instead of playing them immediately.
        /// </summary>
        [RequireComponent(typeof(AudioSource))]
        public class FadingAudioSource : MonoBehaviour
        {
            #region Fields
     
            /// <summary>
            ///   Volume to end the previous clip at.
            /// </summary>
            public float FadeOutThreshold = 0.05f;
     
            /// <summary>
            ///   Volume change per second when fading.
            /// </summary>
            public float FadeSpeed = 0.05f;
     
            /// <summary>
            ///   Actual audio source.
            /// </summary>
            private AudioSource audioSource;
     
            /// <summary>
            ///   Whether the audio source is currently fading, in or out.
            /// </summary>
            private FadeState fadeState = FadeState.None;
     
            /// <summary>
            ///   Next clip to fade to.
            /// </summary>
            private AudioClip nextClip;
     
            /// <summary>
            ///   Whether to loop the next clip.
            /// </summary>
            private bool nextClipLoop;
     
            /// <summary>
            ///   Target volume to fade the next clip to.
            /// </summary>
            private float nextClipVolume;
     
            #endregion
     
            #region Enums
     
            public enum FadeState
            {
                None,
     
                FadingOut,
     
                FadingIn
            }
     
            #endregion
     
            #region Public Properties
     
            /// <summary>
            ///   Current clip of the audio source.
            /// </summary>
            public AudioClip Clip
            {
                get
                {
                    return this.audioSource.clip;
                }
            }
     
            /// <summary>
            ///   Whether the audio source is currently playing a clip.
            /// </summary>
            public bool IsPlaying
            {
                get
                {
                    return this.audioSource.isPlaying;
                }
            }
     
            /// <summary>
            ///   Whether the audio source is looping the current clip.
            /// </summary>
            public bool Loop
            {
                get
                {
                    return this.audioSource.loop;
                }
            }
     
            /// <summary>
            ///   Current volume of the audio source.
            /// </summary>
            public float Volume
            {
                get
                {
                    return this.audioSource.volume;
                }
            }
     
            #endregion
     
            #region Public Methods and Operators
     
            /// <summary>
            ///   If the audio source is enabled and playing, fades out the current clip and fades in the specified one, after.
            ///   If the audio source is enabled and not playing, fades in the specified clip immediately.
            ///   If the audio source is not enalbed, fades in the specified clip as soon as it gets enabled.
            /// </summary>
            /// <param name="clip">Clip to fade in.</param>
            /// <param name="volume">Volume to fade to.</param>
            /// <param name="loop">Whether to loop the new clip, or not.</param>
            public void Fade(AudioClip clip, float volume, bool loop)
            {
                if (clip == null || clip == this.audioSource.clip)
                {
                    return;
                }
     
                this.nextClip = clip;
                this.nextClipVolume = volume;
                this.nextClipLoop = loop;
     
                if (this.audioSource.enabled)
                {
                    if (this.IsPlaying)
                    {
                        this.fadeState = FadeState.FadingOut;
                    }
                    else
                    {
                        this.FadeToNextClip();
                    }
                }
                else
                {
                    this.FadeToNextClip();
                }
            }
     
            /// <summary>
            ///   Continues fading in the current audio clip.
            /// </summary>
            public void Play()
            {
                this.fadeState = FadeState.FadingIn;
                this.audioSource.Play();
            }
     
            /// <summary>
            ///   Stop playing the current audio clip immediately.
            /// </summary>
            public void Stop()
            {
                this.audioSource.Stop();
                this.fadeState = FadeState.None;
            }
     
            #endregion
     
            #region Methods
     
            private void Awake()
            {
                this.audioSource = this.audio;
                this.audioSource.volume = 0f;
            }
     
            private void FadeToNextClip()
            {
                this.audioSource.clip = this.nextClip;
                this.audioSource.loop = this.nextClipLoop;
     
                this.fadeState = FadeState.FadingIn;
     
                if (this.audioSource.enabled)
                {
                    this.audioSource.Play();
                }
            }
     
            private void OnDisable()
            {
                this.audioSource.enabled = false;
                this.Stop();
            }
     
            private void OnEnable()
            {
                this.audioSource.enabled = true;
                this.Play();
            }
     
            private void Update()
            {
                if (!this.audioSource.enabled)
                {
                    return;
                }
     
                if (this.fadeState == FadeState.FadingOut)
                {
                    if (this.audioSource.volume > this.FadeOutThreshold)
                    {
                        // Fade out current clip.
                        this.audioSource.volume -= this.FadeSpeed * Time.deltaTime;
                    }
                    else
                    {
                        // Start fading in next clip.
                        this.FadeToNextClip();
                    }
                }
                else if (this.fadeState == FadeState.FadingIn)
                {
                    if (this.audioSource.volume < this.nextClipVolume)
                    {
                        // Fade in next clip.
                        this.audioSource.volume += this.FadeSpeed * Time.deltaTime;
                    }
                    else
                    {
                        // Stop fading in.
                        this.fadeState = FadeState.None;
                    }
                }
            }
     
            #endregion
        }
}
