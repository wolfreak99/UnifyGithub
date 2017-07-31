// Original url: http://wiki.unity3d.com/index.php/AnimationStepSync
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/AnimationControllers/AnimationStepSync.cs
// File based on original modification date of: 10 January 2012, at 20:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.AnimationControllers
{
Author: Lasse Järvensivu (Statement) 
Contents [hide] 
1 Description 
2 Usage 
3 Notes 
4 C# - AnimationStepSync.cs 
5 C# - AnimationStepSync.cs(Edit don't animation clip init) 

DescriptionThis script allows animations to move the object to a new location at the end of the animation. 
This is useful if you perhaps have a jump animation, and want the character to end up standing where the character lands. 
UsageAttach script to object with animation. 
Set which clips should sync upon completion by filling in the clips array. 
Set reference to the object in your animation that the object should position itself at the end of the animation. 
NotesOnStepSync is called on the object when an animation ends and the object is repositioned. 
This script isn't tested with live animation content, so it might not work well with blended animations. 
C# - AnimationStepSync.csusing UnityEngine;
 
[RequireComponent(typeof(Animation))]
public class AnimationStepSync : MonoBehaviour
{
    public AnimationClip[] clips;
    public Transform reference;
 
    void Awake()
    {
        foreach (var clip in clips)
        {
            if (clip != null)
                 AddStepSyncEvent(clip);
        }
    }
 
    void AddStepSyncEvent(AnimationClip clip)
    {
        var stepSyncEvent = new AnimationEvent();
        stepSyncEvent.time = clip.length;
        stepSyncEvent.functionName = "OnStepSync";
        clip.AddEvent(stepSyncEvent);
    }
 
    void OnStepSync()
    {
        transform.position = reference.position;
        transform.rotation = reference.rotation;
    }
}

C# - AnimationStepSync.cs(Edit don't animation clip init)using UnityEngine;
 
[RequireComponent(typeof(Animation))]
public class AnimationStepSync : MonoBehaviour
{
	private AnimationClip[] clips;
	public Transform reference;
 
	void Awake()
	{
		foreach (AnimationState state in animation)
		{
			AddStepSyncEvent(state.clip);
		}
	}
 
	void AddStepSyncEvent(AnimationClip clip)
	{
		var stepSyncEvent = new AnimationEvent();
		stepSyncEvent.time = clip.length;
		stepSyncEvent.functionName = "OnStepSync";
		clip.AddEvent(stepSyncEvent);
	}
 
	void OnStepSync()
	{
		transform.position = reference.position;
		transform.rotation = reference.rotation;
	}
}
}
