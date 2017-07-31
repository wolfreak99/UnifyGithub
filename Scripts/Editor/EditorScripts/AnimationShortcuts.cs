// Original url: http://wiki.unity3d.com/index.php/AnimationShortcuts
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/AnimationShortcuts.cs
// File based on original modification date of: 25 April 2012, at 00:07. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Daniel Kim (dannyskim) 
Description Gives easy access to playing imported Animations on a Game Object. Utilizes the keyboard Ctrl + 1 - 0 ( Cmd on OSX ) keys for fast viewing. 
Usage This is a simple editor script. You must place this in the Editor folder of your project. 
Press Play. 
Select on a Game Object in the hierarchy. 
Press Ctrl + 1 - 0 ( cmd + 1 - 0 on OSX ) to play corresponding animation. 
C# - AnimationShortcuts.cs using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class AnimationShortcuts : Editor {
 
	[MenuItem ("Animation Shortcuts/Play Element 1 %1")]
	public static void playElement1() {
		playAnimInSelected(0);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 2 %2")]
	public static void playElement2() {
		playAnimInSelected(1);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 3 %3")]
	public static void playElement3() {
		playAnimInSelected(2);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 4 %4")]
	public static void playElement4() {
		playAnimInSelected(3);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 5 %5")]
	public static void playElement5() {
		playAnimInSelected(4);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 6 %6")]
	public static void playElement6() {
		playAnimInSelected(5);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 7 %7")]
	public static void playElement7() {
		playAnimInSelected(6);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 8 %8")]
	public static void playElement8() {
		playAnimInSelected(7);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 9 %9")]
	public static void playElement9() {
		playAnimInSelected(8);
	}
 
	[MenuItem ("Animation Shortcuts/Play Element 10 %0")]
	public static void playElement10() {
		playAnimInSelected(9);
	}
 
	private static Animation getSelectedGameObjectAnimation() {
		if( Selection.activeGameObject.animation != null )	return Selection.activeGameObject.animation;
		else 												return null;
	}
 
	private static void playAnimInSelected( int index ){
 
		AnimationClip[] clips;
		if( getSelectedGameObjectAnimation() != null )	clips = AnimationUtility.GetAnimationClips( getSelectedGameObjectAnimation() );
		else
		{
			Debug.LogError( "Animation Shortcut: GameObject does not have an Animation Component attached." );
			return;
		}
		Animation s = getSelectedGameObjectAnimation();
 
		if( (index + 1) <= s.GetClipCount() )		
		{
			Debug.Log( "Animation Shortcut Play: " + clips[index].name );
			s.clip = clips[index];
			s.Play();
		}
		else Debug.LogError( "Animation Shortcut: Animation Element " + index + " Does Not Exist. Index out of range." );
	}
}
}
