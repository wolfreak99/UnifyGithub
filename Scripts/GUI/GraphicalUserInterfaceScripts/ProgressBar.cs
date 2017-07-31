// Original url: http://wiki.unity3d.com/index.php/ProgressBar
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/ProgressBar.cs
// File based on original modification date of: 17 February 2014, at 01:17. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Contents [hide] 
1 What is it 
2 Demo 
3 Code 
4 Instructions 
5 Resources 

What is it A simple circular progress bar for 2D Sprites, or on a 3D quad, by Opless. 
Demo http://www.youtube.com/watch?v=y8RLVuoq0AU 
Code void Update() {
	float revealOffset = (float)(Time.timeSinceLevelLoad % 10) / 10.1F; 
 
	gameObject.renderer.material.SetFloat ("_Cutoff", revealOffset);
}Instructions Create your circular progress bar image with an alpha gradient (N.B. it can be ANY shape) 
Set the import to be a texture (not a sprite) and DO NOT USE COMPRESSION. 
Create a material with the shader being "Transparent Cutout Vertex Lit" 
Use progress texture in the material 
Use code above, or something like it in a script, attach it to the Quad/Sprite. 
2D Only - Use material and set it on the sprite, remember it'll ignore all but the size of the sprite. 
REMEMBER - This is using a cutout shader, so you'll need something behind it if you want it coloured. 
Resources Transparent Progress Bar Slider 
}
