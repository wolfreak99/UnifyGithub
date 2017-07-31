// Original url: http://wiki.unity3d.com/index.php/SpriteManager
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Development/PerformanceScripts/SpriteManager.cs
// File based on original modification date of: 22 October 2012, at 13:37. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.PerformanceScripts
{
Contents [hide] 
1 Terms of Use 
2 Overview 
3 Usage 
4 The Sprite Class 
4.1 drawLayer 
4.2 SetDrawLayer() 
4.2.1 Arguments 
4.3 SetColor() 
4.3.1 Arguments 
4.4 SetSizeXY(), SetSizeXZ(), SetSizeYZ() 
4.5 SetAnimCompleteDelegate() 
4.5.1 Arguments 
4.6 AddAnimation() 
4.6.1 Arguments 
4.7 PlayAnim() 
4.8 PlayAnimInReverse() 
4.9 PauseAnim() 
4.10 UnpauseAnim() 
5 The UVAnimation class 
5.1 Public members 
5.2 BuildUVAnim() 
5.2.1 Arguments 
5.2.2 Return value 
6 The SpriteManager class 
6.1 Memory management 
6.2 Editor Settings 
6.2.1 allocBlockSize 
6.2.2 material 
6.2.3 plane 
6.2.4 winding 
6.3 PixelSpaceToUVSpace() 
6.4 PixelCoordToUVCoord() 
6.5 AddSprite() 
6.5.1 Arguments 
6.5.2 Return value 
6.6 SetBillboarded() 
6.6.1 Arguments 
6.7 RemoveSprite() 
6.7.1 Arguments 
6.8 HideSprite() 
6.8.1 Arguments 
6.9 ShowSprite() 
6.9.1 Arguments 
6.10 MoveToFront() 
6.11 MoveToBack() 
6.12 MoveInFrontOf() 
6.12.1 Arguments 
6.13 MoveBehind() 
6.13.1 Arguments 
6.14 SortDrawingOrder() 
6.15 GetSprite() 
6.15.1 Arguments 
6.16 Transform() 
6.16.1 Arguments 
6.17 TransformBillboarded() 
6.17.1 Arguments 
6.18 UpdatePositions() 
6.18.1 Arguments 
6.19 UpdateUV() 
6.19.1 Arguments 
6.20 UpdateColors() 
6.20.1 Arguments 
6.21 UpdateBounds() 
6.22 ScheduleBoundsUpdate() 
6.22.1 Arguments 
6.23 CancelBoundsUpdate() 
7 The LinkedSpriteManager class 
8 Misc 
9 Possible features to be implemented by the community in the future 
10 The Code 
10.1 SpriteManager.cs 
10.2 LinkedSpriteManager.cs 
10.3 Sprite.cs 
11 SpriteManager.cs Change Log 
11.1 v0.64 
11.2 v0.633 
11.3 v0.632 
11.4 v0.631 
11.5 v0.63 
11.6 v0.622 
11.7 v0.621 
11.8 v0.62 
11.9 v0.61 
12 LinkedSpriteManager.cs Change Log 
12.1 v0.64 
12.2 v0.632 
12.3 v0.631 
12.4 v0.52 
13 Sprite.cs Change Log 
13.1 v0.64 
13.2 v0.633 
13.3 v0.632 
13.4 v0.631 
13.5 v0.63 

Terms of Use This code is provided for all to use on one condition: that the notice at the top of each script is kept intact and unmodified, and that if you make any improvements to the code, that you share them with the Unity community so everyone can benefit from them (please post to this thread). This revision has not yet been thoroughly tested for stability, but the code is pretty simple and it should be pretty stable. 
Overview Drawing lots of simple, independently-moving sprites for a 2D game can be performance prohibitive in Unity iPhone because the engine was designed with 3D in mind. For each object that has its own transform, another draw call is normally required. The significant overhead of a draw call quickly adds up and will cause framerate problems with only a modest number of objects on-screen. To address this, my SpriteManager class builds a single mesh containing the sprite "quads" to be displayed, and then "manually" transforms the vertices of these quads at runtime to create the appearance of multiple, independently moving objects - all in a single draw call! This dramatically increases the number of independently moving objects allowed on-screen at a time while maintaining a decent framerate. 
While these classes were designed as a solution to performance limitations on the iPhone, they should work perfectly well in reducing draw calls using regular Unity as well. 
Usage 1. Create an empty GameObject (or you may use any other GameObject so long as it is located at the origin (0,0,0) with no rotations or scaling) and attach the SpriteManager or LinkedSpriteManager script to it. (NOTE: It is vital that the object containing the SpriteManager script be at the origin and have no rotations or scaling or else the sprites will be drawn out of alignment with the positions of the GameObjects they are intended to represent! This gets forced in the Awake() method of SpriteManager so that you don't have to worry about it in the editor. But do not relocate the object containing SpriteManager at run-time unless you have a very good reason for doing so!) Fill in the allocBlockSize and material values in the Unity editor. The SpriteManager is now ready to use. 
2. To use it, create GameObjects which you want to represent using sprites at run-time. Add a script to each of these objects that contains a reference to the instance of the SpriteManager script you created in step 1. 
3. In Start() of each such GameObject, place code calling the appropriate initialization routines of the SpriteManager object to add the sprite you want to represent this GameObject to the SpriteManager. Depending on the animation techniques used, you may also need to add code to Update() to manually inform the SpriteManager of changes you have made to the sprite at run-time. (In a later revision, all the necessary update calls could be made automatically to the SpriteManager through the Sprite class's own property accessors.) 
The Sprite Class The Sprite class contains all the relevant information to describe a single quad sprite (two coplanar triangles that form a quadrilateral). Each sprite has a width and height to indicate world-space dimensions. It also has the location of the lower-left UV offset (which can be changed at runtime to create UV animations) as well as the width and height of the UV (m_UVDimensions). 
Each sprite contains four vertices which define the shape of its "quad" in local space. These vertices will be transformed by the SpriteManager class at runtime to orient the quad in world-space. 
Finally, each sprite is associated with a GameObject referred to as the "client". This client object is the object to be represented by the quad. The quad will be transformed according to the client's transform. So when the client moves, the quad will follow, exactly as if the quad were simply part of the client GameObject. 
drawLayer This is an integer value that indicates roughly where in the drawing order the sprite should be drawn relative to other sprites in the same SpriteManager. Lower values are drawn first, higher values are drawn later. For sprites with equal drawLayer values, the order is undefined, unless MoveBehind() or MoveInFrontOf() are used to manually place one in front of the other in the drawing order. 
SetDrawLayer() Sets the drawing layer for the sprite as described above for drawLayer, but also automatically calls the associated SpriteManager's SortDrawingOrder(). This should only be used if you are only sorting a single sprite in a single game cycle. If you are assigning drawLayer values to multiple sprites in a single frame/update cycle, you should assign the values directly to their drawLayer members, then make a single call to the SpriteManager's SortDrawingOrder(). The reason is that SortDrawingOrder() can be very expensive. 
Argumentsv - The value indicating the relative drawing order. 
SetColor() Causes all four vertices of the sprite to be rendered using the specified color and transparency (alpha). (Automatically instructs SpriteManager to update the color entries for the sprite.) In addition to changing the color tint of the sprite, this is also useful for fading a sprite in or out. 
Argumentsc - The color and transparency to use for the sprite. 
SetSizeXY(), SetSizeXZ(), SetSizeYZ() Sets the dimensions of the sprite in the indicated plane (XY, XZ, or YZ). 
SetAnimCompleteDelegate() Sets the delegate routine to be called when an animation is finished playing. 
Argumentsdel - The delegate to be called upon animation completion. 
AddAnimation() Adds a UV animation to the Sprite's list of animations. 
Argumentsanim - A variable of type UVAnimation, describing the UV animation to add. 
PlayAnim() Begins playing the specified UV animation. 
PlayAnimInReverse() Plays the specified animation in reverse - starting at the end. 
PauseAnim() Pauses the currently-playing animation mid-stream, if any. Has no effect if no animation is playing. 
UnpauseAnim() Unpauses the previously playing animation. 
The UVAnimation class This class describes a series of UV coordinates as well as a few parameters that describe a basic UV animation. That is, animation that uses multiple UV coordinates, displayed in sequence, to create the appearance of animation, very similar to stop-motion animation. This would typically be accomplished by creating a series of sprites placed at regular intervals in a row or column on a single texture. Then the coordinates of each individual sprite image are given, in sequence, to define the animation sequence. While you may simply manually define an array of Vector2 UV coordinates to setup your animation, some methods are provided to automate the process as much as possible. 
Public members name - A string containing the name of the animation. Should be unique from other animations associated with a given sprite object. 
loopCycles - The number of times to loop the animation before stopping. 0 results in a "one-shot" animation. 1 would cause the animation to loop over once, etc. -1 causes the animation to loop infinitely. 
loopReverse - If true, the play direction is reversed once the end of the animation sequence is reached. If set to false, a loop iteration is counted each time the end of the animation sequence is reached. If set to true, a loop iteration isn't counted until the animation returns to the beginning. 
framerate - The rate at which to advance from frame to frame, in frames per second. i.e. a value of 15 would cause 15 frames of animation to be advanced every second. 
BuildUVAnim() This method automatically builds a UV animation sequence from a few pieces of information. The layout of an animation sequence grid is assumed to be left-to-right, top-to-bottom. 
Argumentsstart - The UV coordinates of the lower-left corner of the first sprite in the sequence 
cellSize - The size, in UV space, of each sprite animation "cell" (to use BuildUVAnim(), all sprite images in the sequence must have the same dimensions). 
cols - The number of columns in the animation sequence grid. 
rows - The number of rows in the animation sequence grid. 
totalCells - The total number of cells in the animation sequence grid. 
fps - The number of frames of animation that should advance per second (the framerate of the animation). 
Return valueAn array of Vector2 UV coordinates representing the animation sequence. 
The SpriteManager class This class manages a list of sprites and associated GameObjects. 
Memory management Currently, as sprites are added, the list (and associated vertex, uv, and triangle buffers) increase in size. As sprites are removed from the manager, the lists remain the same size, but the "open slots" are flagged and are re-used when new sprites are added again, removing the performance penalty of re-allocating all the buffers and copying their contents over again. This approach was taken not only for the aformentioned performance reasons, but also because it would add significant complexity to reduce the size of the buffers since client GameObjects hold the indices of their associated sprites, and if the buffers were sized down, those indices could then point to invalid offsets. The only way to resolve this would be to add either additional complexity to the design, or less performant ways of keeping track of sprites, or both. 
Editor Settings allocBlockSize Since allocating large new buffers and copying their contents can be a big performance hit at runtime, SpriteManager allows the developer to choose how many sprites should be pre-allocated at a time. If, for example, you expect your game to never use more than 100 sprites, you should probably set this value to 100, resulting in a one-time allocation of sprites so the player does not experience a "hiccup" mid-game as the buffer is re-allocated and new contents are copied over during gameplay. If you pre-allocate 100 sprites and have filled up the sprite buffer, then find yourself having to create one more sprite (for a total of 101), if you have set allocBlockSize to 100, then another 100 sprites will be allocated even though you have added only 1. So use caution in the value you assign to allocBlockSize. Try to balance memory waste with frequency of having to re-allocate new buffers at runtime. In the above case, using an allocBlockSize of 25, if you created 101 sprites, you would only have an "overage" of 24 sprites, but the buffers would have to be re-allocated and re-copied 5 times. 
material The material with which to render the sprites. Simply assign the materal you wish to use for your sprites here. It is strongly advised that for sprites, you use one of the particle shaders so that backface culling is not an issue. All the sprites for this SpriteManager will use this material. So for a typical application, you would want to combine as many of your sprites as possible into a single texture atlas and assign that material to the SpriteManager. 
plane The plane in which the sprites are to be created. The options are XY, XZ, or YZ. For example, an Asteroids type game might typically use sprites created in the XZ plane, while a Tetris-like game would probably use the XY plane. 
winding Which way to wind the generated polygons. The possible values are "CCW" (counter-clockwise) and "CW" (clockwise). This affects the direction the polygons are considered to be "facing". If you can't see the sprites at runtime with your particular setup, try changing the winding order. 
PixelSpaceToUVSpace() This utility method will convert values from pixel space to UV space according to the material currently assigned to the SpriteManager object. For example, to use 256x256 pixels of a 512x512 texture, you would normally use the UV value 0.5,0.5. However, PixelSpaceToUVSpace() will allow you to specify 256,256 and will perform the conversion. There is a version that accepts Vector2s and one that accepts ints. Returns the converted UV-space value in a Vector2. 
NOTE: Do not use PixelSpaceToUVSpace() to get UV coordinates. PixelSpaceToUVSpace() is intended to convert widths and heights from pixel space to UV space. For coordinates, please use PixelCoordToUVCoord()! 
PixelCoordToUVCoord() This utility method will convert coordinates from pixel space to UV space according to the material currently assigned to the SpriteManager object. For example, to specify a point at the center of a 512x512 texture you would normally use the UV value 0.5,0.5. However, PixelCoordToUVCorrd() will allow you to specify 256,256 and will perform the conversion. There is a version that accepts Vector2s and one that accepts ints. Returns the converted UV-space coordinate in a Vector2. 
NOTE: Do not use PixelCoordToUVCoord() to convert widths or heights (dimensions). PixelCoordToUVCoord() is only intended for actual coordinates and it inverts the Y-component since UV coordinates are given in the opposite Y-direction from how pixel coordinates are typically represented. To convert heights and widths, please use PixelSpaceToUVSpace()! 
AddSprite() This method will add a sprite to the SpriteManager's list and will associate it with the specified GameObject. The sprite list as well as the vertex, UV, and triangle buffers will all be reallocated and copied if no available "slots" can be found. The buffers will be increased according to allocBlockSize. Performance note: Will cause the vertex, UV, and triangle buffers to be re-copied to the mesh object. NOTE: a versions also exist that allow you to specify the UV coordinates using pixel-space values that is not documented here. 
Arguments client - The GameObject that is to be associated with this sprite. The sprite will be transformed using this object's transform. 
width and height - The width and height of the sprite in world space units. (This assumes that you have not applied scaling to the object containing the SpriteManager script - which you probably should not do unless you really know why you're doing it.) 
lowerLeftUV - The UV coordinate of the lower-left corner of the quad. 
UVDimensions - The width and height of how much of the texture to use. This is a scalar value. Ex: if lowerLeftUV is 0.5,0.5 and UVDimensions is 0.5,0.5, the quad will display the associated texture from the center extending out to the extreme top and right edges. 
offset - the sprite will be offseted by the specified Vector3 relative to the client transform. For example, applying an offset of (0.5, 0, -0.5) might help aligning a mouse cursor sprite so that the arrow tip corresponds to the mouse position. If ignored, Vector3.zero will be used. 
billboarded - Whether or not the sprite should be rendered so that it always faces toward the camera (actually, it only faces in the direction from which the camera is looking, but it's close enough in almost all cases). Currently, this incurs a noticeable performance hit if many such billboards are used, so only use billboarding when necessary. 
Return valueA reference to the sprite added. 
SetBillboarded() Causes the specified sprite to be rendered so that it is oriented facing the direction from which the camera is looking. This type of orientation is somewhat more performance demanding, so be sure to only use it when necessary. 
Argumentssprite - A reference to the sprite to be billboarded. 
RemoveSprite() "Removes" the sprite specified. (It actually just flags the sprite as available and reduces its dimensions to 0 so that it is invisible when rendered.) 
Argumentssprite - A reference to the sprite to remove. This should be the value returned by AddSprite(). 
Performance note: Will cause the vertex buffer to be re-copied to the mesh object. 
HideSprite() Hides the sprite specified. This is accomplished by setting the sprite's associated vertices to 0. In addition, the sprite is removed from active lists so that it does not needlessly incur the overhead of being transformed. This method can be invoked indirectly by simply writing something such as: 
sprite.hidden = true;NOTE: The sprite will not remain hidden if Transform() is called on it, as Transform() will cause its vertices to be re-copied to the mesh's vertex buffer, meaning they will no longer be set to zero (which is how they were "hidden" in the first place). So if you wish to keep a sprite hidden, be sure not to manually call Transform() on it. 
Argumentssprite - A reference to the sprite to hide. This should be the value returned by AddSprite(). 
Performance note: Will cause the vertex buffer to be re-copied to the mesh object. 
ShowSprite() Unhides the sprite specified which was previously hidden. This method can be invoked indirectly by simply writing something such as: 
sprite.hidden = false;Argumentssprite - A reference to the sprite to unhide. This should be the value returned by AddSprite(). 
Performance note: Will cause the vertex buffer to be re-copied to the mesh object. 
MoveToFront()Moves the specified sprite to the front visually by moving it to the end of the drawing order, meaning it will be drawn on top of all other sprites in the SpriteManager. 
MoveToBack()Moves the specified sprite to the back visually by moving it to the beginning of the drawing order, meaning it will be drawn first and all other sprites in the SpriteManager will be drawn intop of it. 
MoveInFrontOf()Moves the first sprite in front of the second sprite by placing it later in the drawing order. If the sprite is already in front, nothing is changed. 
ArgumentstoMove - The sprite to move 
reference - The sprite we wish "toMove" to appear in front of. 
MoveBehind()Moves the first sprite behind the second sprite by placing it earlier in the drawing order. If the sprite is already behind, nothing is done. 
ArgumentstoMove - The sprite to move 
reference - The sprite we wish "toMove" to appear behind. 
SortDrawingOrder()Sorts all sprites' in the drawing order according to their respective drawLayer values with lower values being drawn first, and higher values being drawn later. NOTE: This routine is expensive and should only be called after setting the drawLayer of all the sprites you intend to sort in a given update cycle. It is automatically called when a sprite's SetDrawLayer() method is called. Also note that if MoveToFront(), MoveToBack(), MoveInFrontOf(), or MoveBehind() have been used on a sprite but it has not had the appropriate drawLayer value set, calling SortDrawingOrder() may undo these operations. So when using drawLayer values to order your sprites, be sure that their respective drawLayer values do not conflict with the ordering you specify through these other routines. 
GetSprite()This method returns a reference to the specified sprite so that the sprite can be directly manipulated if need be. Use of this routine is now discouraged since the preferred way of referring to sprites is by reference. This is largely because in the future, the internal buffers may be resized at any time, at which point any indices obtained earlier may be invalid. 
Argumentsi - Index of the sprite in question. 
Transform()This method transforms the vertices associated with the specified sprite by the transform of its client GameObject. In plain English, if a GameObject wants to manually synch a sprite up with its current orientation, it should call this method. This method will transform that sprite, and that sprite alone, leaving all the other sprites un-updated. Performance note: Will cause the vertex buffer to be re-copied to the mesh object in the next LateUpdate(). 
Argumentssprite - A reference to the sprite to transform. 
TransformBillboarded()This method operates identically to Transform() except that it orients the quad so that it faces the direction from which the camera is looking (billboarding). Performance note: Will cause the vertex buffer to be re-copied to the mesh object in the next LateUpdate(). 
Argumentssprite - A reference to the sprite to transform. 
UpdatePositions()Informs the SpriteManager that vertices have changed and they need to be updated (copied to the mesh object) at the earliest opportunity (usually the end of the current frame when LateUpdate is called). This is used if a GameObject has made changes to a sprite (such as changing its dimensions) and its vertices should be re-copied to the mesh to reflect these changes. Performance note: Will cause the vertex buffer to be re-copied to the mesh object in the next LateUpdate(). NOTE: Under normal circumstances, caling SetSizeXX() or Transform() on a Sprite object will automatically call this routine. You should only have need to call it directly if you have manually modified the vertices directly. 
ArgumentsNone 
UpdateUV()Updates the UVs of the sprite in the local UV buffer (which mirrors that of the mesh object), and forces the UVs of the entire mesh to be re-copied to the mesh object. Use this when you manually change the UV offset or dimensions of the sprite between frames and want to inform the SpriteManager of the change so that it may update its UV buffer. Performance note: Will cause the UV buffer to be re-copied to the mesh object in the next LateUpdate(). NOTE: Under normal circumstances, modifying lowerLeftUV or uvDimensions of a Sprite object will automatically call this routine. You should only have need to call it directly if you have manually modified one of the UV coordinates directly. 
Argumentssprite - The index of the sprite to update. 
UpdateColors()Informs the SpriteManager to copy over the color values of the specified sprite. Performance note: Will cause the UV buffer to be re-copied to the mesh object in the next LateUpdate(). NOTE: Under normal circumstances, modifying color of a Sprite object using SetColor() will automatically call this routine. You should only have need to call it directly if you have manually modified a sprite's color directly. 
Argumentssprite - The index of the sprite to update. 


UpdateBounds()Forces SpriteManager to recalculate the bounding volume of the mesh in the next LateUpdate(). This is important to do periodically to ensure that Unity's visibility culling can cull the mesh when not visible, and also so that the mesh is not culled erroneously when it is visible. If your objects all remain pretty much in the same area, it is less important to call this frequently. However, if your objects move around quite a bit and the bounds are not recalculated, it is very easy for them to extend beyond the bounds of the previously updated bounding volume, causing Unity to cull the entire mesh based on the earlier boundaries, resulting in parts of your mesh which should be visible to disappear. Performance note: Will cause Unity to recalculate the bounds of the mesh, which can be very performance intensive with a large number of sprites. Consider using ScheduleBoundsUpdate() to keep from recalculating bounds each frame. 
ScheduleBoundsUpdate()Instructs SpriteManager to call UpdateBounds() at a regular interval. This can be useful if you have a large number of sprites that move around quite a bit so that recalculating the mesh's bounds each frame would be performance prohibitive. This way the bounds are only recalculated periodically. 
Argumentsseconds - The interval in seconds. 
CancelBoundsUpdate()Cancels any previously scheduled bounds update created by ScheduleBoundsUpdate(). 
The LinkedSpriteManager class This class inherits from SpriteManager and adds the functionality of automatically transforming the vertices of all sprites each frame, removing the need to call "Transform()" whenever the position of a GameObject is changed. The trade-off is that if you have lots of sprites that do not move most of the time, you will be transforming the vertices of these sprites needlessly each frame. If you have lots of sprites, this could impact performance noticably. If, however, the typical case is that your sprites will be in almost constant motion, it will be faster to use LinkedSpriteManager since all transformations are handled under a single function call (TransformSprites()) rather than having each GameObject call Transform() separately, thereby reducing call overhead. 
MiscPlease report all bugs you find or improvements you make to these classes by posting to this thread. I put a bit of work into creating these and am sharing with everyone in the hope that more brains working on this will result in a more robust, efficient, and stable solution than I have time to commit to making happen here by myself. I truly believe that this approach will unlock game types and other possibilities that have, until now, been out of the question for Unity iPhone because of the overhead of the required draw calls. 
Possible features to be implemented by the community in the futureDevise a method for reducing the size of the sprite and other buffers without adding significant complexity or performance overhead and without compromising stability. 
Create a 3D version of SpriteManager for use with fully 3D objects. 
Anything else you can think of! 
The CodeSpriteManager.cs//-----------------------------------------------------------------
//  SpriteManager v0.64 (21-10-2012)
//  Copyright 2012 Brady Wright and Above and Beyond Software
//  All rights reserved
//-----------------------------------------------------------------
// A class to allow the drawing of multiple "quads" as part of a
// single aggregated mesh so as to achieve multiple, independently
// moving objects using a single draw call.
//-----------------------------------------------------------------
 
 
using UnityEngine;
using System.Collections;
 
 
//-----------------------------------------------------------------
// Describes a UV animation
//-----------------------------------------------------------------
//	NOTE: Currently, you should assign at least two frames to an
//	animation, or else you can expect problems!
//-----------------------------------------------------------------
public class UVAnimation
{
	protected Vector2[] frames;						// Array of UV coordinates (for quads) defining the frames of an animation
 
	// Animation state vars:
	protected int curFrame = 0;						// The current frame
	protected int stepDir = 1;						// The direction we're currently playing the animation (1=forwards (default), -1=backwards)
	protected int numLoops = 0;						// Number of times we've looped since last animation
 
	public string name;								// The name of the 
	public int loopCycles = 0;						// How many times to loop the animation (-1 loop infinitely)
	public bool loopReverse = false;				// Reverse the play direction when the end of the animation is reached? (if true, a loop iteration isn't counted until we return to the beginning)
	public float framerate;							// The rate in frames per second at which to play the animation
 
 
	// Resets all the animation state vars to ready the object
	// for playing anew:
	public void Reset()
	{
		curFrame = 0;
		stepDir = 1;
		numLoops = 0;
	}
 
	// Sets the stepDir to -1 and sets the current frame to the end
	// so that the animation plays in reverse
	public void PlayInReverse()
	{
		stepDir = -1;
		curFrame = frames.Length - 1;
	}
 
	// Stores the UV of the next frame in 'uv', returns false if
	// we've reached the end of the animation (this will never
	// happen if it is set to loop infinitely)
	public bool GetNextFrame(ref Vector2 uv)
	{
		// See if we can advance to the next frame:
		if((curFrame + stepDir) >= frames.Length || (curFrame + stepDir) < 0)
		{
			// See if we need to loop (if we're reversing, we don't loop until we get back to the beginning):
			if( stepDir>0 && loopReverse )
			{
				stepDir = -1;	// Reverse playback direction
				curFrame += stepDir;
 
				uv = frames[curFrame];
			}else
			{
				// See if we can loop:
				if (numLoops + 1 > loopCycles && loopCycles != -1)
					return false;
				else
				{	// Loop the animation:
					++numLoops;
 
					if (loopReverse)
					{
						stepDir *= -1;
						curFrame += stepDir;
					}
					else
						curFrame = 0;
 
					uv = frames[curFrame];
				}
			}
		}else
		{
			curFrame += stepDir;
			uv = frames[curFrame];
		}
 
		return true;
	}
 
	// Constructs an array of UV coordinates based upon the info
	// supplied.
	//
	// start	-	The UV of the lower-left corner of the first
	//				cell
	// cellSize	-	width and height, in UV space, of each cell
	// cols		-	Number of columns in the grid
	// rows		-	Number of rows in the grid
	// totalCells-	Total number of cells in the grid (left-to-right,
	//				top-to-bottom ordering is assumed, just like reading
	//				English).
	// fps		-	Framerate (frames per second)
	public Vector2[] BuildUVAnim(Vector2 start, Vector2 cellSize, int cols, int rows, int totalCells, float fps)
	{
		int cellCount = 0;
 
		frames = new Vector2[totalCells];
		framerate = fps;
 
		frames[0] = start;
 
		for(int row=0; row < rows; ++row)
		{
			for(int col=0; col<cols && cellCount < totalCells; ++col)
			{
				frames[cellCount].x = start.x + cellSize.x * ((float)col);
				frames[cellCount].y = start.y - cellSize.y * ((float)row);
 
				++cellCount;
			}
		}
 
		return frames;
	}
 
	// Assigns the specified array of UV coordinates to the
	// animation, replacing its current contents
	public void SetAnim(Vector2[] anim)
	{
		frames = anim;
	}
 
	// Appends the specified array of UV coordinates to the
	// existing animation
	public void AppendAnim(Vector2[] anim)
	{
		Vector2[] tempFrames = frames;
 
		frames = new Vector2[frames.Length + anim.Length];
		tempFrames.CopyTo(frames, 0);
		anim.CopyTo(frames, tempFrames.Length);
	}
}
 
 
 
 
//-----------------------------------------------------------------
// Holds a single mesh object which is composed of an arbitrary
// number of quads that all use the same material, allowing
// multiple, independently moving objects to be drawn on-screen
// while using only a single draw call.
//-----------------------------------------------------------------
public class SpriteManager : MonoBehaviour 
{
	// In which plane should we create the sprites?
	public enum SPRITE_PLANE
	{
		XY,
		XZ,
		YZ
	};
 
	// Which way to wind polygons?
	public enum WINDING_ORDER
	{
		CCW,		// Counter-clockwise
		CW			// Clockwise
	};
 
	public Material material;				// The material to use for the sprites
	public int allocBlockSize;				// How many sprites to allocate space for at a time. ex: if set to 10, 10 new sprite blocks will be allocated at a time. Once all of these are used, 10 more will be allocated, and so on...
	public SPRITE_PLANE plane;				// The plane in which to create the sprites
	public WINDING_ORDER winding=WINDING_ORDER.CCW;	// Which way to wind polygons
	public bool autoUpdateBounds = false;	// Automatically recalculate the bounds of the mesh when vertices change?
 
	protected ArrayList availableBlocks = new ArrayList(); // Array of references to sprites which are currently not in use
	protected bool vertsChanged = false;	// Have changes been made to the vertices of the mesh since the last frame?
	protected bool uvsChanged = false;		// Have changes been made to the UVs of the mesh since the last frame?
	protected bool colorsChanged = false;	// Have the colors changed?
	protected bool vertCountChanged = false;// Has the number of vertices changed?
	protected bool updateBounds = false;	// Update the mesh bounds?
	protected Sprite[] sprites;				// Array of all sprites (the offset of the vertices corresponding to each sprite should be found simply by taking the sprite's index * 4 (4 verts per sprite).
	protected ArrayList activeBlocks = new ArrayList();	// Array of references to all the currently active (non-empty) sprites
	protected ArrayList activeBillboards = new ArrayList(); // Array of references to all the *active* sprites which are to be rendered as billboards
	protected ArrayList playingAnimations = new ArrayList();// Array of references to all the sprites that are currently playing animation
	protected ArrayList spriteDrawOrder = new ArrayList();	// Array of indices of sprite objects stored in the order they are to be drawn (corresponding to the position of their vertex indices in the triIndices list)  Allows us to keep track of where a given Sprite is in the drawing order (triIndices)
	protected SpriteDrawLayerComparer drawOrderComparer = new SpriteDrawLayerComparer(); // Used to sort our draw order array
	protected float boundUpdateInterval;	// Interval, in seconds, to update the mesh bounds
 
 
	protected MeshFilter meshFilter;
	protected MeshRenderer meshRenderer;
	protected Mesh mesh;					// Reference to our mesh (contained in the MeshFilter)
 
	protected Vector3[] vertices;			// The vertices of our mesh
	protected int[] triIndices;				// Indices into the vertex array
	protected Vector2[] UVs;				// UV coordinates
	protected Color[] colors;				// Color values
	protected Vector3[] normals;			// Normals
 
	// Working vars:
	protected int i;
	protected Sprite tempSprite = null;
	protected float animTimeElapsed;
 
	//--------------------------------------------------------------
	// Utility functions:
	//--------------------------------------------------------------
 
	// Converts pixel-space values to UV-space scalar values
	// according to the currently assigned material.
	// NOTE: This is for converting widths and heights-not
	// coordinates (which have reversed Y-coordinates).
	// For coordinates, use PixelCoordToUVCoord()!
	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		Texture t = material.GetTexture("_MainTex");
 
		return new Vector2(xy.x / ((float)t.width), xy.y / ((float)t.height));
	}
 
	// Converts pixel-space values to UV-space scalar values
	// according to the currently assigned material.
	// NOTE: This is for converting widths and heights-not
	// coordinates (which have reversed Y-coordinates).
	// For coordinates, use PixelCoordToUVCoord()!
	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return PixelSpaceToUVSpace(new Vector2((float)x, (float)y));
	}
 
	// Converts pixel coordinates to UV coordinates according to
	// the currently assigned material.
	// NOTE: This is for converting coordinates and will reverse
	// the Y component accordingly.  For converting widths and
	// heights, use PixelSpaceToUVSpace()!
	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		Vector2 p = PixelSpaceToUVSpace(xy);
		p.y = 1.0f - p.y;
		return p;
	}
 
	// Converts pixel coordinates to UV coordinates according to
	// the currently assigned material.
	// NOTE: This is for converting coordinates and will reverse
	// the Y component accordingly.  For converting widths and
	// heights, use PixelSpaceToUVSpace()!
	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return PixelCoordToUVCoord(new Vector2((float)x, (float)y));
	}
 
	//--------------------------------------------------------------
	// End utility functions
	//--------------------------------------------------------------
 
	void Awake()
	{
		gameObject.AddComponent("MeshFilter");
		gameObject.AddComponent("MeshRenderer");
 
		meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
		meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
 
		meshRenderer.renderer.material = material;
		mesh = meshFilter.mesh;
 
		// Create our first batch of sprites:
		EnlargeArrays(allocBlockSize);
 
		// Move the object to the origin so the objects drawn will not
		// be offset from the objects they are intended to represent.
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
	}
 
	// Allocates initial arrays
	protected void InitArrays()
	{
		sprites = new Sprite[1];
		sprites[0] = new Sprite();
		vertices = new Vector3[4];
		UVs = new Vector2[4];
		colors = new Color[4];
		normals = new Vector3[4];
		triIndices = new int[6];
	}
 
	// Enlarges the sprite array by the specified count and also resizes
	// the UV and vertex arrays by the necessary corresponding amount.
	// Returns the index of the first newly allocated element
	// (ex: if the sprite array was already 10 elements long and is 
	// enlarged by 10 elements resulting in a total length of 20, 
	// EnlargeArrays() will return 10, indicating that element 10 is the 
	// first of the newly allocated elements.)
	protected int EnlargeArrays(int count)
	{
		int firstNewElement;
 
		if (sprites == null)
		{
			InitArrays();
			firstNewElement = 0;
			count = count - 1;	// Allocate one less since InitArrays already allocated one sprite for us
		}
		else
			firstNewElement = sprites.Length;
 
		// Resize sprite array:
		Sprite[] tempSprites = sprites;
		sprites = new Sprite[sprites.Length + count];
		tempSprites.CopyTo(sprites, 0);
 
		// Vertices:
		Vector3[] tempVerts = vertices;
		vertices = new Vector3[vertices.Length + count*4];
		tempVerts.CopyTo(vertices, 0);
 
		// UVs:
		Vector2[] tempUVs = UVs;
		UVs = new Vector2[UVs.Length + count*4];
		tempUVs.CopyTo(UVs, 0);
 
		// Colors:
		Color[] tempColors = colors;
		colors = new Color[colors.Length + count * 4];
		tempColors.CopyTo(colors, 0);
 
		// Normals:
		Vector3[] tempNormals = normals;
		normals = new Vector3[normals.Length + count*4];
		tempNormals.CopyTo(normals, 0);
 
		// Triangle indices:
		int[] tempTris = triIndices;
		triIndices = new int[triIndices.Length + count*6];
		tempTris.CopyTo(triIndices, 0);
 
		// Inform existing sprites of the new vertex and UV buffers:
		for (int i = 0; i < firstNewElement; ++i)
		{
			sprites[i].SetBuffers(vertices, UVs);
		}
 
		// Setup the newly-added sprites and Add them to the list of available 
		// sprite blocks. Also initialize the triangle indices while we're at it:
		for (int i = firstNewElement; i < sprites.Length; ++i)
		{
			// Create and setup sprite:
 
			sprites[i] = new Sprite();
			sprites[i].index = i;
			sprites[i].manager = this;
 
			sprites[i].SetBuffers(vertices, UVs);
 
			// Setup indices of the sprite's vertices in the vertex buffer:
			sprites[i].mv1 = i * 4 + 0;
			sprites[i].mv2 = i * 4 + 1;
			sprites[i].mv3 = i * 4 + 2;
			sprites[i].mv4 = i * 4 + 3;
 
			// Setup the indices of the sprite's UV entries in the UV buffer:
			sprites[i].uv1 = i * 4 + 0;
			sprites[i].uv2 = i * 4 + 1;
			sprites[i].uv3 = i * 4 + 2;
			sprites[i].uv4 = i * 4 + 3;
 
			// Setup the indices to the color values:
			sprites[i].cv1 = i * 4 + 0;
			sprites[i].cv2 = i * 4 + 1;
			sprites[i].cv3 = i * 4 + 2;
			sprites[i].cv4 = i * 4 + 3;
 
			// Setup the indices to the normal values:
			sprites[i].nv1 = i * 4 + 0;
			sprites[i].nv2 = i * 4 + 1;
			sprites[i].nv3 = i * 4 + 2;
			sprites[i].nv4 = i * 4 + 3;
 
			// Setup the default color:
			sprites[i].SetColor(Color.white);
 
			// Add as an available sprite:
			availableBlocks.Add(sprites[i]);
 
			// Init triangle indices:
			if(winding == WINDING_ORDER.CCW)
			{	// Counter-clockwise winding
				triIndices[i * 6 + 0] = i * 4 + 0;	//	0_ 2			0 ___ 3
				triIndices[i * 6 + 1] = i * 4 + 1;	//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = i * 4 + 3;	// 1|/				1|/__|2
 
				triIndices[i * 6 + 3] = i * 4 + 3;	//	  3
				triIndices[i * 6 + 4] = i * 4 + 1;	//   /|
				triIndices[i * 6 + 5] = i * 4 + 2;	// 4/_|5
			}
			else
			{	// Clockwise winding
				triIndices[i * 6 + 0] = i * 4 + 0;	//	0_ 1			0 ___ 3
				triIndices[i * 6 + 1] = i * 4 + 3;	//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = i * 4 + 1;	// 2|/				1|/__|2
 
				triIndices[i * 6 + 3] = i * 4 + 3;	//	  3
				triIndices[i * 6 + 4] = i * 4 + 2;	//   /|
				triIndices[i * 6 + 5] = i * 4 + 1;	// 5/_|4
			}
 
			// Add the index of this sprite to the draw order list
			spriteDrawOrder.Add(sprites[i]);
		}
 
		vertsChanged = true;
		uvsChanged = true;
		colorsChanged = true;
		vertCountChanged = true;
 
		return firstNewElement;
	}
 
	// Adds a sprite to the manager at the location and rotation of the client 
	// GameObject and with its transform.  Returns a reference to the new sprite
	// Width and height are in world space units
	// leftPixelX and bottomPixelY- the bottom-left position of the desired portion of the texture, in pixels
	// pixelWidth and pixelHeight - the dimensions of the desired portion of the texture, in pixels
	public Sprite AddSprite(GameObject client, float width, float height, int leftPixelX, int bottomPixelY, int pixelWidth, int pixelHeight, bool billboarded)
	{
		return AddSprite(client, width, height, PixelCoordToUVCoord(leftPixelX, bottomPixelY), PixelSpaceToUVSpace(pixelWidth, pixelHeight), Vector3.zero, billboarded);
	}
 
	// Same as the previous, but allows the use of a Vector3 offset for the sprite
	// Adds a sprite to the manager at the location and rotation of the client 
	// GameObject and with its transform.  Returns a reference to the new sprite
	// Width and height are in world space units
	// leftPixelX and bottomPixelY- the bottom-left position of the desired portion of the texture, in pixels
	// pixelWidth and pixelHeight - the dimensions of the desired portion of the texture, in pixels
	// offset - the sprite will be offseted by the specified Vector3 relative to the client transform
	public Sprite AddSprite(GameObject client, float width, float height, int leftPixelX, int bottomPixelY, int pixelWidth, int pixelHeight, Vector3 offset, bool billboarded)
	{
		return AddSprite(client, width, height, PixelCoordToUVCoord(leftPixelX, bottomPixelY), PixelSpaceToUVSpace(pixelWidth, pixelHeight), offset, billboarded);
	}
 
	// Adds a sprite to the manager at the location and rotation of the client 
	// GameObject and with its transform.  Returns a reference to the new sprite
	// Width and height are in world space units
	// lowerLeftUV - the UV coordinate for the upper-left corner
	// UVDimensions - the distance from lowerLeftUV to place the other UV coords
	// offset - the sprite will be offseted by the specified Vector3 relative to the client transform
	public Sprite AddSprite(GameObject client, float width, float height, Vector2 lowerLeftUV, Vector2 UVDimensions, Vector3 offset, bool billboarded)
	{
		int spriteIndex;
 
		// Get an available sprite:
		if (availableBlocks.Count < 1)
			EnlargeArrays(allocBlockSize);	// If we're out of available sprites, allocate some more:
 
		// Use a sprite from the list of available blocks:
		spriteIndex = ((Sprite)availableBlocks[0]).index;
		availableBlocks.RemoveAt(0);	// Now that we're using this one, remove it from the available list
 
		// Assign the new sprite:
		Sprite newSprite = sprites[spriteIndex];
		newSprite.client = client;
		newSprite.offset = offset;
		newSprite.lowerLeftUV = lowerLeftUV;
		newSprite.uvDimensions = UVDimensions;
 
 
		switch(plane)
		{
			case SPRITE_PLANE.XY:
				newSprite.SetSizeXY(width, height);
				break;
			case SPRITE_PLANE.XZ:
				newSprite.SetSizeXZ(width, height);
				break;
			case SPRITE_PLANE.YZ:
				newSprite.SetSizeYZ(width, height);
				break;
			default:
				newSprite.SetSizeXY(width, height);
				break;
		}
 
		// Save this to an active list now that it is in-use:
		if(billboarded)			
		{
			newSprite.billboarded = true;
			activeBillboards.Add(newSprite);
		}
		else
			activeBlocks.Add(newSprite);
 
		// Transform the sprite:
		newSprite.Transform();
 
		// Setup the UVs:
		UVs[newSprite.uv1] = lowerLeftUV + Vector2.up * UVDimensions.y;	 // Upper-left
		UVs[newSprite.uv2] = lowerLeftUV;								 // Lower-left
		UVs[newSprite.uv3] = lowerLeftUV + Vector2.right * UVDimensions.x;// Lower-right
		UVs[newSprite.uv4] = lowerLeftUV + UVDimensions;				// Upper-right
 
		// Calculate the normals
		Vector3 normal = CalculateNormal(newSprite);
 
		normals[newSprite.nv1] = normal;
		normals[newSprite.nv2] = normal;
		normals[newSprite.nv3] = normal;
		normals[newSprite.nv4] = normal;
 
		// Set our flags:
		vertsChanged = true;
		uvsChanged = true;
 
		return newSprite;
	}
 
	Vector3 CalculateNormal( Sprite sprite )
	{
 
		// Setup the normals
		// The normal of a triangle is found by calculating the cross product of two of its vectors.
		// We know both triangles of the generated mesh are coplanar, and as such have the same normal vector,
		// so we only have to calculate it once.
 
		// The winding order of the triangle must be followed, so we have to use triIndices to know the correct vertex order
 
		int[] indices = new int[3];
		int offset = spriteDrawOrder.IndexOf(sprite) * 6;
 
		if (offset < 0)
			return Vector3.zero;
 
		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
 
		Vector3 v1 = vertices[indices[1]] - vertices[indices[0]];
		Vector3 v2 = vertices[indices[2]] - vertices[indices[0]];
 
		Vector3 normal = Vector3.Cross(v1, v2);
 
		normal.Normalize();
 
		return normal;
 
	}
 
	public void SetBillboarded(Sprite sprite)
	{
		// Make sure the sprite isn't in the active list
		// or else it'll get handled twice:
		activeBlocks.Remove(sprite);
		activeBillboards.Add(sprite);
	}
 
	public void RemoveSprite(Sprite sprite)
	{
		sprite.SetSizeXY(0,0);
		sprite.v1 = Vector3.zero;
		sprite.v2 = Vector3.zero;
		sprite.v3 = Vector3.zero;
		sprite.v4 = Vector3.zero;
 
		vertices[sprite.mv1] = sprite.v1;
		vertices[sprite.mv2] = sprite.v2;
		vertices[sprite.mv3] = sprite.v3;
		vertices[sprite.mv4] = sprite.v4;
 
		// Remove the sprite from the billboarded list
		// since that list should only contain active
		// sprites:
		if (sprite.billboarded)
			activeBillboards.Remove(sprite);
		else
			activeBlocks.Remove(sprite);
 
		// Clean the sprite's settings:
		sprite.Clear();		
 
		availableBlocks.Add(sprite);
 
		vertsChanged = true;
	}
 
	public void HideSprite(Sprite sprite)
	{
		// Remove the sprite from the billboarded list
		// since that list should only contain sprites
		// we intend to transform:
		if (sprite.billboarded)
			activeBillboards.Remove(sprite);
		else
			activeBlocks.Remove(sprite);
 
		sprite.m_hidden___DoNotAccessExternally = true;
 
		vertices[sprite.mv1] = Vector3.zero;
		vertices[sprite.mv2] = Vector3.zero;
		vertices[sprite.mv3] = Vector3.zero;
		vertices[sprite.mv4] = Vector3.zero;
 
		vertsChanged = true;
	}
 
	public void ShowSprite(Sprite sprite)
	{
		// Only show the sprite if it has a client:
		if(sprite.client == null)
			return;
 
		if (!sprite.m_hidden___DoNotAccessExternally)
			return;
 
		sprite.m_hidden___DoNotAccessExternally = false;
 
		// Update the vertices:
		sprite.Transform();
 
		if (sprite.billboarded)
			activeBillboards.Add(sprite);
		else
			activeBlocks.Add(sprite);
 
		vertsChanged = true;
	}
 
 
	// Moves the specified sprite to the end of the drawing order
	public void MoveToFront(Sprite s)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(s) * 6;
 
		if (offset < 0)
			return;
 
		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];
 
		// Shift all indices from here forward down 6 slots (each sprite occupies 6 index slots):
		for (int i = offset; i < triIndices.Length - 6; i += 6)
		{
			triIndices[i] = triIndices[i+6];
			triIndices[i+1] = triIndices[i+7];
			triIndices[i+2] = triIndices[i+8];
			triIndices[i+3] = triIndices[i+9];
			triIndices[i+4] = triIndices[i+10];
			triIndices[i+5] = triIndices[i+11];
 
			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 + 1];
		}
 
		// Place our desired index value at the end:
		triIndices[triIndices.Length - 6] = indices[0];
		triIndices[triIndices.Length - 5] = indices[1];
		triIndices[triIndices.Length - 4] = indices[2];
		triIndices[triIndices.Length - 3] = indices[3];
		triIndices[triIndices.Length - 2] = indices[4];
		triIndices[triIndices.Length - 1] = indices[5];
 
		// Update the sprite's index offset:
		spriteDrawOrder[spriteDrawOrder.Count - 1] = s.index;
 
		vertCountChanged = true;
	}
 
	// Moves the specified sprite to the start of the drawing order
	public void MoveToBack(Sprite s)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(s) * 6;
 
		if (offset < 0)
			return;
 
		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];
 
		// Shift all indices from here back up 6 slots (each sprite occupies 6 index slots):
		for(int i=offset; i>5; i-=6)
		{
			triIndices[i] = triIndices[i-6];
			triIndices[i+1] = triIndices[i-5];
			triIndices[i+2] = triIndices[i-4];
			triIndices[i+3] = triIndices[i-3];
			triIndices[i+4] = triIndices[i-2];
			triIndices[i+5] = triIndices[i-1];
 
			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 - 1];
		}
 
		// Place our desired index value at the beginning:
		triIndices[0] = indices[0];
		triIndices[1] = indices[1];
		triIndices[2] = indices[2];
		triIndices[3] = indices[3];
		triIndices[4] = indices[4];
		triIndices[5] = indices[5];
 
		// Update the sprite's index offset:
		spriteDrawOrder[0] = s.index;
 
		vertCountChanged = true;
	}
 
	// Moves the first sprite in front of the second sprite by
	// placing it later in the draw order. If the sprite is already
	// in front of the reference sprite, nothing is changed:
	public void MoveInfrontOf(Sprite toMove, Sprite reference)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(toMove) * 6;
		int refOffset = spriteDrawOrder.IndexOf(reference) * 6;
 
		if (offset < 0)
			return;
 
		// Check to see if the sprite is already in front:
		if(offset > refOffset)
			return;
 
		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];
 
		// Shift all indices from here to the reference sprite down 6 slots (each sprite occupies 6 index slots):
		for (int i = offset; i < refOffset; i += 6)
		{
			triIndices[i] = triIndices[i+6];
			triIndices[i+1] = triIndices[i+7];
			triIndices[i+2] = triIndices[i+8];
			triIndices[i+3] = triIndices[i+9];
			triIndices[i+4] = triIndices[i+10];
			triIndices[i+5] = triIndices[i+11];
 
			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 + 1];
		}
 
		// Place our desired index value at the destination:
		triIndices[refOffset] = indices[0];
		triIndices[refOffset+1] = indices[1];
		triIndices[refOffset+2] = indices[2];
		triIndices[refOffset+3] = indices[3];
		triIndices[refOffset+4] = indices[4];
		triIndices[refOffset+5] = indices[5];
 
		// Update the sprite's index offset:
		spriteDrawOrder[refOffset/6] = toMove.index;
 
		vertCountChanged = true;
	}
 
	// Moves the first sprite behind the second sprite by
	// placing it earlier in the draw order. If the sprite
	// is already behind, nothing is done:
	public void MoveBehind(Sprite toMove, Sprite reference)
	{
		int[] indices = new int[6];
		int offset = spriteDrawOrder.IndexOf(toMove) * 6;
		int refOffset = spriteDrawOrder.IndexOf(reference) * 6;
 
		if (offset < 0)
			return;
 
		// Check to see if the sprite is already behind:
		if(offset < refOffset)
			return;
 
		// Save our indices:
		indices[0] = triIndices[offset];
		indices[1] = triIndices[offset + 1];
		indices[2] = triIndices[offset + 2];
		indices[3] = triIndices[offset + 3];
		indices[4] = triIndices[offset + 4];
		indices[5] = triIndices[offset + 5];
 
		// Shift all indices from here to the reference sprite up 6 slots (each sprite occupies 6 index slots):
		for (int i = offset; i > refOffset; i -= 6)
		{
			triIndices[i] = triIndices[i-6];
			triIndices[i+1] = triIndices[i-5];
			triIndices[i+2] = triIndices[i-4];
			triIndices[i+3] = triIndices[i-3];
			triIndices[i+4] = triIndices[i-2];
			triIndices[i+5] = triIndices[i-1];
 
			spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 - 1];
		}
 
		// Place our desired index value at the destination:
		triIndices[refOffset] = indices[0];
		triIndices[refOffset+1] = indices[1];
		triIndices[refOffset+2] = indices[2];
		triIndices[refOffset+3] = indices[3];
		triIndices[refOffset+4] = indices[4];
		triIndices[refOffset+5] = indices[5];
 
		// Update the sprite's index offset:
		spriteDrawOrder[refOffset/6] = toMove.index;
 
		vertCountChanged = true;
	}
 
	// Rebuilds the drawing order based upon the drawing order buffer
	public void SortDrawingOrder()
	{
 
		Sprite s;
 
		spriteDrawOrder.Sort(drawOrderComparer);
 
		// Now reconstitute the triIndices in the order we want:
		if (winding == WINDING_ORDER.CCW)
		{
			for (int i = 0; i < spriteDrawOrder.Count; ++i)
			{
				s = (Sprite) spriteDrawOrder[i];
 
				// Counter-clockwise winding
				triIndices[i * 6 + 0] = s.mv1;		//	0_ 2			1 ___ 4
				triIndices[i * 6 + 1] = s.mv2;		//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = s.mv4;		// 1|/				2|/__|3
 
				triIndices[i * 6 + 3] = s.mv4;		//	  3
				triIndices[i * 6 + 4] = s.mv2;		//   /|
				triIndices[i * 6 + 5] = s.mv3;		// 4/_|5
			}
		}
		else
		{
			for (int i = 0; i < spriteDrawOrder.Count; ++i)
			{
				s = (Sprite)spriteDrawOrder[i];
 
				// Clockwise winding
				triIndices[i * 6 + 0] = s.mv1;		//	0_ 1			0 ___ 3
				triIndices[i * 6 + 1] = s.mv4;		//  | /		Verts:	 |	/|
				triIndices[i * 6 + 2] = s.mv2;		// 2|/				1|/__|2
 
				triIndices[i * 6 + 3] = s.mv4;		//	  3
				triIndices[i * 6 + 4] = s.mv3;		//   /|
				triIndices[i * 6 + 5] = s.mv2;		// 5/_|4
			}
		}
 
		vertCountChanged = true;
	}
 
	public void AnimateSprite(Sprite s)
	{
		// Add this sprite to our playingAnimation list:
		playingAnimations.Add(s);
	}
 
	public void StopAnimation(Sprite s)
	{
		playingAnimations.Remove(s);
	}
 
	public Sprite GetSprite(int i)
	{
		if (i < sprites.Length)
			return sprites[i];
		else
			return null;
	}
 
	// Updates the vertices of a sprite based on the transform
	// of its client GameObject
	public void Transform(Sprite sprite)
	{
		sprite.Transform();
 
		vertsChanged = true;
	}
 
	// Updates the vertices of a sprite such that it is oriented
	// more or less toward the camera
	public void TransformBillboarded(Sprite sprite)
	{
		Vector3 pos = sprite.clientTransform.position;
		Transform t = Camera.main.transform;
 
		vertices[sprite.mv1] = pos + t.TransformDirection(sprite.v1);
		vertices[sprite.mv2] = pos + t.TransformDirection(sprite.v2);
		vertices[sprite.mv3] = pos + t.TransformDirection(sprite.v3);
		vertices[sprite.mv4] = pos + t.TransformDirection(sprite.v4);
 
		vertsChanged = true;
	}
 
	// Informs the SpriteManager that some vertices have changed position
	// and the mesh needs to be reconstructed accordingly
	public void UpdatePositions()
	{
		vertsChanged = true;
	}
 
	// Updates the UVs of the specified sprite and copies the new values
	// into the mesh object.
	public void UpdateUV(Sprite sprite)
	{
		UVs[sprite.uv1] = sprite.lowerLeftUV + Vector2.up * sprite.uvDimensions.y;	// Upper-left
		UVs[sprite.uv2] = sprite.lowerLeftUV;										// Lower-left
		UVs[sprite.uv3] = sprite.lowerLeftUV + Vector2.right * sprite.uvDimensions.x;// Lower-right
		UVs[sprite.uv4] = sprite.lowerLeftUV + sprite.uvDimensions;					// Upper-right
 
		uvsChanged = true;
	}
 
	// Updates the color values of the specified sprite and copies the
	// new values into the mesh object.
	public void UpdateColors(Sprite sprite)
	{
		colors[sprite.cv1] = sprite.color;
		colors[sprite.cv2] = sprite.color;
		colors[sprite.cv3] = sprite.color;
		colors[sprite.cv4] = sprite.color;
 
		colorsChanged = true;
	}
 
	// Instructs the manager to recalculate the bounds of the mesh
	public void UpdateBounds()
	{
		updateBounds = true;
	}
 
	// Schedules a recalculation of the mesh bounds to occur at a
	// regular interval (given in seconds):
	public void ScheduleBoundsUpdate(float seconds)
	{
		boundUpdateInterval = seconds;
		InvokeRepeating("UpdateBounds", seconds, seconds);
	}
 
	// Cancels any previously scheduled bounds recalculations:
	public void CancelBoundsUpdate()
	{
		CancelInvoke("UpdateBounds");
	}
 
	// Use this for initialization
	void Start () 
	{
 
	}
 
	// LateUpdate is called once per frame
	virtual public void LateUpdate () 
	{
		// See if we have any active animations:
		if(playingAnimations.Count > 0)
		{
			animTimeElapsed = Time.deltaTime;
 
			for(i=0; i<playingAnimations.Count; ++i)
			{
				tempSprite = (Sprite)playingAnimations[i];
 
				// Step the animation, and if it has finished
				// playing, remove it from the playing list:
				if (!tempSprite.StepAnim(animTimeElapsed))
					playingAnimations.Remove(tempSprite);
			}
 
			uvsChanged = true;
		}
 
		// Were changes made to the mesh since last time?
		if (vertCountChanged)
		{
			vertCountChanged = false;
			colorsChanged = false;
			vertsChanged = false;
			uvsChanged = false;
			updateBounds = false;
 
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.normals = normals;
			mesh.triangles = triIndices;
		}
		else
		{
			if (vertsChanged)
			{
				vertsChanged = false;
 
				if (autoUpdateBounds)
					updateBounds = true;
 
				mesh.vertices = vertices;
				mesh.normals = normals;
			}
 
			if (updateBounds)
			{
				mesh.RecalculateBounds();
				updateBounds = false;
			}
 
			if (colorsChanged)
			{
				colorsChanged = false;
				mesh.colors = colors;
			}
 
			if (uvsChanged)
			{
				uvsChanged = false;
				mesh.uv = UVs;
			}
		}
	}
}You can also change the Reset() function on the SpriteManager to make sure the first frame is played in the first loop: 
public void Reset()
{
	curFrame = -1; //this is the change
	stepDir = 1;
	numLoops = 0;
}LinkedSpriteManager.cs//-----------------------------------------------------------------
//  LinkedSpriteManager v0.64 (21-10-2012)
//  Copyright 2012 Brady Wright and Above and Beyond Software
//  All rights reserved
//-----------------------------------------------------------------
// A class to allow the drawing of multiple "quads" as part of a
// single aggregated mesh so as to achieve multiple, independently
// moving objects using a single draw call.
//-----------------------------------------------------------------
 
 
using UnityEngine;
using System.Collections;
 
// A variation on the SpriteManager that automatically links all
// translations and rotations of the client GameObjects to the
// associated sprite - meaning the client need not worry about
// micromanaging all transformations:
public class LinkedSpriteManager : SpriteManager 
{
	Transform t;
	Vector3 pos;
	Sprite s;
 
 
	// Use this for initialization
	void Start () 
	{
 
	}
 
	// Transforms all sprites by their associated GameObject's
	// transforms:
	void TransformSprites()
	{
		for(int i=0; i<activeBlocks.Count; ++i)
		{
			((Sprite)activeBlocks[i]).Transform();
		}
 
		// Handle any billboarded sprites:
		if(activeBillboards.Count > 0)
		{
			t = Camera.main.transform;
 
			for(int i=0; i<activeBillboards.Count; ++i)
			{
				s = (Sprite)activeBillboards[i];
				pos = s.clientTransform.position;
 
				vertices[s.mv1] = pos + t.TransformDirection(s.v1);
				vertices[s.mv2] = pos + t.TransformDirection(s.v2);
				vertices[s.mv3] = pos + t.TransformDirection(s.v3);
				vertices[s.mv4] = pos + t.TransformDirection(s.v4);
			}
		}
	}
 
	// LateUpdate is called once per frame
	new void LateUpdate() 
	{
		// Transform all sprites according to their
		// client GameObject's transforms:
		TransformSprites();
 
		// Copy over the changes:
		mesh.vertices = vertices;
 
		// See if we have any active animations:
		if (playingAnimations.Count > 0)
		{
			animTimeElapsed = Time.deltaTime;
 
			for (i = 0; i < playingAnimations.Count; ++i)
			{
				tempSprite = (Sprite)playingAnimations[i];
 
				// Step the animation, and if it has finished
				// playing, remove it from the playing list:
				if (!tempSprite.StepAnim(animTimeElapsed))
					playingAnimations.Remove(tempSprite);
			}
 
			uvsChanged = true;
		}
 
		if (vertCountChanged)
		{
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.normals = normals;
			mesh.triangles = triIndices;
 
			vertCountChanged = false;
			uvsChanged = false;
			colorsChanged = false;
		}
		else
		{
			if (uvsChanged)
			{
				mesh.uv = UVs;
				uvsChanged = false;
			}
 
			if (colorsChanged)
			{
				colorsChanged = false;
 
				mesh.colors = colors;
			}
 
			// Explicitly recalculate bounds since
			// we didn't assign new triangles (which
			// implicitly recalculates bounds)
			if (updateBounds || autoUpdateBounds)
			{
				mesh.RecalculateBounds();
				updateBounds = false;
			}
		}
	}
}Sprite.cs//-----------------------------------------------------------------
//  Sprite (part of SpriteManager) v0.64 (21-10-2012)
//  Copyright 2012 Brady Wright and Above and Beyond Software
//  All rights reserved
//-----------------------------------------------------------------
// A class to allow the drawing of multiple "quads" as part of a
// single aggregated mesh so as to achieve multiple, independently
// moving objects using a single draw call.
//-----------------------------------------------------------------
 
 
using UnityEngine;
using System.Collections;
 
//-----------------------------------------------------------------
// Describes a sprite
//-----------------------------------------------------------------
public class Sprite
{
	protected float m_width;					// Width and Height of the sprite in worldspace units
	protected float m_height;
	protected Vector2 m_lowerLeftUV;			// UV coordinate for the upper-left corner of the sprite
	protected Vector2 m_UVDimensions;			// Distance from the upper-left UV to place the other UVs
	protected GameObject m_client;				// Reference to the client GameObject
	protected SpriteManager m_manager;			// Reference to the sprite manager in which this sprite resides
	protected bool m_billboarded = false;		// Is the sprite to be billboarded?
	public bool m_hidden___DoNotAccessExternally = false;	// Indicates whether this sprite is currently hidden (has to be public because C# has no "friend" feature, just don't access directly from outside)
 
	protected Vector3[] meshVerts;				// Pointer to the array of vertices in the mesh
	protected Vector2[] UVs;					// Pointer to the array of UVs in the mesh
	protected Vector3[] normals;				// Pointer to the array of normals in the mesh
 
	public Transform clientTransform;			// Transform of the client GameObject
	public Vector3 offset = new Vector3();		// Offset of sprite from center of client GameObject
	public Color color;							// The color to be used by all four vertices
 
	public int index;							// Index of this sprite in its SpriteManager's list
	public int drawLayer;						// The draw layer indicating the order in which this sprite should be rendered relative to other sprites
 
	public Vector3 v1 = new Vector3();			// The sprite's vertices in local space
	public Vector3 v2 = new Vector3();
	public Vector3 v3 = new Vector3();
	public Vector3 v4 = new Vector3();
 
	public int mv1;							// Indices of the associated vertices in the actual mesh (this just provides a quicker way for the SpriteManager to get straight to the right vertices in the vertex array)
	public int mv2;
	public int mv3;
	public int mv4;
 
	public int uv1;							// Indices of the associated UVs in the mesh
	public int uv2;
	public int uv3;
	public int uv4;
 
	public int cv1;							// Indices of the associated color values in the mesh
	public int cv2;
	public int cv3;
	public int cv4;
 
	public int nv1;							// Indices of the associated normal values in the mesh
	public int nv2;
	public int nv3;
	public int nv4;
 
	// Animation-related vars and types:
	public delegate void AnimCompleteDelegate();		// Definition of delegate to be called upon animation completion
 
	protected ArrayList animations = new ArrayList();	// Array of available animations
	protected UVAnimation curAnim = null;				// The current animation
	protected AnimCompleteDelegate animCompleteDelegate = null;	// Delegate to be called upon animation completion
	protected float timeSinceLastFrame = 0;				// The total time since our last animation frame change
	protected float timeBetweenAnimFrames;				// The amount of time we want to pass before moving to the next frame of animation
	protected int framesToAdvance;						// (working) The number of animation frames to advance given the time elapsed
 
	~Sprite()
	{
	}
 
 
	public Sprite()
	{
		m_width = 0;
		m_height = 0;
		m_client = null;
		m_manager = null;
		clientTransform = null;
		index = 0;
		drawLayer = 0;
		color = Color.white;
 
		offset = Vector3.zero;
	}
 
	public SpriteManager manager
	{
		get { return m_manager; }
		set { m_manager = value; }
	}
 
	public GameObject client
	{
		get { return m_client; }
		set
		{
			m_client = value;
			if (m_client != null)
				clientTransform = m_client.transform;
			else
				clientTransform = null;
		}
	}
 
	public Vector2 lowerLeftUV
	{
		get { return m_lowerLeftUV; }
		set
		{
			m_lowerLeftUV = value;
			m_manager.UpdateUV(this);
		}
	}
 
	public Vector2 uvDimensions
	{
		get { return m_UVDimensions; }
		set
		{
			m_UVDimensions = value;
			m_manager.UpdateUV(this);
		}
	}
 
	public float width
	{
		get { return m_width; }
	}
 
	public float height
	{
		get { return m_height; }
	}
 
	public bool billboarded
	{
		get { return m_billboarded; }
		set
		{
			m_billboarded = value;
		}
	}
 
	public bool hidden
	{
		get { return m_hidden___DoNotAccessExternally; }
		set
		{
			// No need to do anything if we're
			// already in this state:
			if (value == m_hidden___DoNotAccessExternally)
				return;
 
			if (value)
				m_manager.HideSprite(this);
			else
				m_manager.ShowSprite(this);
		}
	}
 
	// Resets all sprite values to defaults for reuse:
	public void Clear()
	{
		client = null;
		billboarded = false;
		hidden = false;
		SetColor(Color.white);
		offset = Vector3.zero;
 
		PauseAnim();
		animations.Clear();
		curAnim = null;
		animCompleteDelegate = null;
	}
 
	// Does the same as assigning the drawLayer value, except that
	// SortDrawingOrder() is called automatically.
	// The draw layer indicates the order in which this sprite should be 
	// rendered relative to other sprites. Higher values result in a later
	// drawing order relative to sprites with lower values:
	public void SetDrawLayer(int v)
	{
		drawLayer = v;
		m_manager.SortDrawingOrder();
	}
 
	// Sets the physical dimensions of the sprite in the XY plane:
	public void SetSizeXY(float width, float height)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3(-m_width / 2, m_height / 2, 0);	// Upper-left
		v2 = offset + new Vector3(-m_width / 2, -m_height / 2, 0);	// Lower-left
		v3 = offset + new Vector3(m_width / 2, -m_height / 2, 0);	// Lower-right
		v4 = offset + new Vector3(m_width / 2, m_height / 2, 0);	// Upper-right
 
		Transform();
	}
 
	// Sets the physical dimensions of the sprite in the XZ plane:
	public void SetSizeXZ(float width, float height)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3(-m_width / 2, 0, m_height / 2);	// Upper-left
		v2 = offset + new Vector3(-m_width / 2, 0, -m_height / 2);	// Lower-left
		v3 = offset + new Vector3(m_width / 2, 0, -m_height / 2);	// Lower-right
		v4 = offset + new Vector3(m_width / 2, 0, m_height / 2);	// Upper-right
 
		Transform();
	}
 
	// Sets the physical dimensions of the sprite in the YZ plane:
	public void SetSizeYZ(float width, float height)
	{
		m_width = width;
		m_height = height;
		v1 = offset + new Vector3(0, m_height / 2, -m_width / 2);	// Upper-left
		v2 = offset + new Vector3(0, -m_height / 2, -m_width / 2);	// Lower-left
		v3 = offset + new Vector3(0, -m_height / 2, m_width / 2);	// Lower-right
		v4 = offset + new Vector3(0, m_height / 2, m_width / 2);	// Upper-right
 
		Transform();
	}
 
	// Sets the vertex and UV buffers
	public void SetBuffers(Vector3[] v, Vector2[] uv)
	{
		meshVerts = v;
		UVs = uv;
	}
 
	// Applies the transform of the client GameObject and stores
	// the results in the associated vertices of the overall mesh:
	public void Transform()
	{
		meshVerts[mv1] = clientTransform.TransformPoint(v1);
		meshVerts[mv2] = clientTransform.TransformPoint(v2);
		meshVerts[mv3] = clientTransform.TransformPoint(v3);
		meshVerts[mv4] = clientTransform.TransformPoint(v4);
 
		m_manager.UpdatePositions();
	}
 
	// Applies the transform of the client GameObject and stores
	// the results in the associated vertices of the overall mesh:
	public void TransformBillboarded(Transform t)
	{
		Vector3 pos = clientTransform.position;
 
		meshVerts[mv1] = pos + t.InverseTransformDirection(v1);
		meshVerts[mv2] = pos + t.InverseTransformDirection(v2);
		meshVerts[mv3] = pos + t.InverseTransformDirection(v3);
		meshVerts[mv4] = pos + t.InverseTransformDirection(v4);
 
		m_manager.UpdatePositions();
	}
 
	// Sets the specified color and automatically notifies the
	// SpriteManager to update the colors:
	public void SetColor(Color c)
	{
		color = c;
		m_manager.UpdateColors(this);
	}
 
	//-----------------------------------------------------------------
	// Animation-related routines:
	//-----------------------------------------------------------------
 
	// Sets the delegate to be called upon animation completion:
	public void SetAnimCompleteDelegate(AnimCompleteDelegate del)
	{
		animCompleteDelegate = del;
	}
 
	// Adds an animation to the sprite
	public void AddAnimation(UVAnimation anim)
	{
		animations.Add(anim);
	}
 
	// Steps to the next frame of sprite animation
	public bool StepAnim(float time)
	{
		if (curAnim == null)
			return false;
 
		timeSinceLastFrame += time;
 
		framesToAdvance = (int) (timeSinceLastFrame / timeBetweenAnimFrames);
 
		// If there's nothing to do, return:
		if (framesToAdvance < 1)
			return true;
 
		while(framesToAdvance > 0)
		{
			if (curAnim.GetNextFrame(ref m_lowerLeftUV))
				--framesToAdvance;
			else
			{
				// We reached the end of our animation
				if (animCompleteDelegate != null)
					animCompleteDelegate();
 
				m_manager.UpdateUV(this);
 
				return false;
			}
		}
 
		m_manager.UpdateUV(this);
		timeSinceLastFrame = 0;
 
		return true;
	}
 
	// Starts playing the specified animation
	// (Note: this doesn't resume from a pause,
	// it completely restarts the animation. To
	// unpause, use UnpauseAnim):
	public void PlayAnim(UVAnimation anim)
	{
		// First stop any currently playing animation:
		m_manager.StopAnimation(this);
 
		curAnim = anim;
		curAnim.Reset();
		timeBetweenAnimFrames = 1f / anim.framerate;
		timeSinceLastFrame = timeBetweenAnimFrames;
		StepAnim(0);
 
		m_manager.AnimateSprite(this);
	}
 
	// Starts playing the specified animation:
	public void PlayAnim(string name)
	{
		for (int i = 0; i < animations.Count; ++i)
		{
			if (((UVAnimation)animations[i]).name == name)
				PlayAnim((UVAnimation)animations[i]);
		}
	}
 
	// Like PlayAnim but plays in reverse:
	public void PlayAnimInReverse(UVAnimation anim)
	{
		// First stop any currently playing animation:
		m_manager.StopAnimation(this);
 
		curAnim = anim;
		curAnim.Reset();
		curAnim.PlayInReverse();
		timeBetweenAnimFrames = 1f / anim.framerate;
		timeSinceLastFrame = timeBetweenAnimFrames;
		StepAnim(0);
 
		m_manager.AnimateSprite(this);
	}
 
	// Starts playing the specified animation in reverse:
	public void PlayAnimInReverse(string name)
	{
		for (int i = 0; i < animations.Count; ++i)
		{
			if (((UVAnimation)animations[i]).name == name)
			{
				((UVAnimation)animations[i]).PlayInReverse();
				PlayAnimInReverse((UVAnimation)animations[i]);
			}
		}
	}
 
	// Pauses the currently-playing animation:
	public void PauseAnim()
	{
		m_manager.StopAnimation(this);
	}
 
	// Unpauses the currently-playing animation:
	public void UnpauseAnim()
	{
		if (curAnim == null) return;
 
		m_manager.AnimateSprite(this);
	}
}
 
 
// Compares drawing layers of sprites
public class SpriteDrawLayerComparer : IComparer
{
	static Sprite s1;
	static Sprite s2;
 
	int IComparer.Compare(object a, object b)
	{
		s1 = (Sprite)a;
		s2 = (Sprite)b;
 
		if (s1.drawLayer > s2.drawLayer)
			return 1;
		else if (s1.drawLayer < s2.drawLayer)
			return -1;
		else
			return 0;
	}
}SpriteManager.cs Change Log v0.64 Corrected a bug in the "SortDrawingOrder()" function: CW winding polygons were being incorrectly drawn. 
Added rudimentary support for calculating normals. The mesh produced by the SpriteManager should be able to be correctly affected by Unity's lighting system (with the use of appropriate shaders, like Diffuse, for example). 
Exposed the Sprite "offset" property so it can be used in the "SpriteManager.AddSprite()" function. 
v0.633 Fixed bug where when a sprite grid was used, BuildUVAnim() was incrementing the y-coordinate in the wrong direction (changed + to -). 
v0.632 Added animation-handling facilities to allow the Sprite class to not derive from Monobehavior anymore, leading to better performance. 
Added protection in some of the draw order management routines so that if calling IndexOf() returns -1 because no matching sprite was found in the list, we don't attempt to access the array out of bounds. 
v0.631 Added features allowing the sorting of the drawing order of sprites. 
v0.63 Moved the Sprite class to its own file since it needs monoBehavior to use Invoke for animation. 
Implemented UVAnimation class 
Fixed problems where various properties of a removed sprite weren't getting reset, such as color, flags, etc. 
Reordered some sprite code to make sure the proper settings were set before adding/removing sprites from active lists. 
v0.622 Added ability to hide sprites without removing them. 
v0.621 Fixed bug in TransformBillboarded() where the camera transform was not being obtained. 
v0.62 Added ability to specify a color (and alpha) for each sprite. 
Added code to produce billboarded sprites 
Added code to ensure mesh bounds are recalculated when the user desires 
v0.61 Fixed bug where removing a sprite would not automatically result in the updating of the sprite's vertices to reflect that it had been removed (zeroed out). 
Sprite objects now have direct access to their associated vertices and UVs in the SpriteManager's buffers (not the same as the actual buffers held by the mesh object). This should reduce overhead when transforming sprite vertices and updating UVs. 
Interfacing with the SpriteManager is now reference-based rather than index based. In other words, info going in and out of SpriteManager that pertains to a particular sprite now uses references to the sprite in question rather than its index. For example, AddSprite() now returns a reference to the new Sprite, and RemoveSprite() accepts a reference to the sprite to remove rather than its index. 
Optimized transforming by storing cached reference to each sprite's client GameObject's transform locally. 
Added utility function(s) PixelSpaceToUVSpace() and PixelCoordToUVCoord() that convert pixel space and coordinates to UV space and coordinates for the currently assigned material's texture. 
There is now less of a need to manually inform SpriteManager when UVs are changed or when the sprite's dimensions change. Sprite objects, upon setting the size or changing UVs, will take care of informing the SpriteManager. 
Added an offset property to the Sprite class. This can be used to offset all of the vertices by the same amount from its client gameObject. 
Added the ability to specify texture coordinates using pixel-space values when calling AddSprite(). 
LinkedSpriteManager.cs Change Log v0.64 Slight changes to support polygon normals. 
v0.632 Added animation-handling facilities to allow the Sprite class to not derive from Monobehavior anymore, leading to better performance. 
v0.631 Removed vestigial vars from TransformSprites() 
Moved temporary vars to class scope to avoid re-allocation each frame. 
v0.52 Fixed a bug where if no sprite was added, the UV coordinates did not get copied to the mesh object. 
Added code to produce billboarded sprites. 
Added code to ensure mesh bounds are recalculated when the user desires 
Sprite.cs Change Log v0.64 Slight changes to support polygon normals. 
v0.633 Fixed problem where animation wasn't getting reset when a sprite is supposed to be cleared of its states for re-use. 
Fixed problem where calling PlayAnim() while an animation is already playing results in the animation getting added to the playingAnimation list in the SpriteManager more than once, causing it to appear to play more rapidly, since its StepAnim() gets called more than once per frame. 
v0.632 Moved Sprite references in SpriteDrawLayerComparer so that they are statically allocated once for the entire class. 
Sprite is no longer derived from Monobehavior to improve performance. Instead of needing Invoke(), etc, a list of actively animating sprites is maintained in the SpriteManager and the animation gets updated (if need be) each frame from there. This also means that there is no more need for dummy GameObjets to host each Sprite object. 
v0.631 Removed line in "hidden" property assigning the value to m_hidden___DoNotAccessExternally since this value is already set in SpriteManager::Hide/ShowSprite(), and otherwise, these latter routines would be confused and exit early since they would mistakenly think this had already been set (well, the flag would have been set, but none of the operations performed) 
Added drawLayer member which indicates roughly where in the drawing order the sprite should be drawn relative to other sprites. Higher drawLayer values result in a later drawing order. 
Added class to compare sprite drawing layers (used for sorting) 
v0.63 Moved sprite code from SpriteManager.cs to Sprite.cs so that Sprite can inherit from monoBehavior to allow use of Invoke() for animation. 
Changed SetSize*() routines to call Transform instead of UpdatePositions() so that when not using LinkedSpriteManager, you don't have to manually call Transform after calling SetSize*(). 
}
