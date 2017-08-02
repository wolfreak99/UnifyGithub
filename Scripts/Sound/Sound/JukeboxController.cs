/*************************
 * Original url: http://wiki.unity3d.com/index.php/JukeboxController
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Sound/Sound/JukeboxController.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Unknown (69.174.143.210) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Sound.Sound
{
    DescriptionThis is a simple jukebox-style audio manager that lets you load multiple sound clips and have them played/mixed on the fly. The intent was to gradually grow this class into something that would easilly allow crossfades, ducking, etc. 
    This class is meant to be used in a main game controller script. For example, a game might have multiple levels, a score, lives remaining, etc. All of this state would be kept in a main controller class. This controller might also hold a reference to a JukeboxController object, which would make all the registered sound clips available to all game components, regardless of what scene they are in. 
    C# - JukeboxController.cs using UnityEngine;
    using System.Collections;
     
    /*
        Usage:
        JukeboxController jukebox = new JukeboxController();
        jukebox.AddClip("mysong", myclip);
        jukebox.PlayClip("mysong");
        jukebox.StopClip();
    */
    public class JukeboxController {
     
        Hashtable jukebox;
        string current_clip;
     
        public JukeboxController()
        {
            jukebox = new Hashtable();
            current_clip = null;
        } // constructor
     
        public void AddClip(string name, AudioClip clip)
        {
            if (jukebox == null)
            {
                jukebox = new Hashtable();
            } // if
     
            GameObject obj;
            obj = new GameObject();
            obj.AddComponent("AudioSource");
            obj.audio.clip = clip;
            obj.audio.ignoreListenerVolume = true;
            DontDestroyOnLoad(obj);
            jukebox.Add(name, obj);
        } // AddClip()
     
        /*
            Play a named audio clip.
            Does not restart the clip if it is played twice in a row.
            Will stop a previously playing clip to play this new clip.
        */
        public void PlayClip(string name)
        {
            if (name == current_clip)
            {
                return;
            } // if
            if (current_clip != null)
            {
                StopClip();
            } // if
     
            ((GameObject)jukebox[name]).audio.Play();
            current_clip = name;
        } // PlayClip()
     
        public void StopClip()
        {
            ((GameObject)jukebox[current_clip]).audio.Stop();
        } // StopClip()
     
}
}
