// Original url: http://wiki.unity3d.com/index.php/Animating_Tiled_texture_-_Extended
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Animating_Tiled_texture_-_Extended.cs
// File based on original modification date of: 23 March 2013, at 17:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Joachim Ante - Extended by TomLong74 
Contents [hide] 
1 Description 
2 Usage 
3 Example 
4 JavaScript - AnimatedTextureExtendedUV.js 
5 CSharp - AnimatedTextureExtendedUV.cs 
6 Boo - AnimatedTextureExtendedUV.boo 

Description This script extends the capabilities of the original AnimatedTexureUV.js. This allows many animation skins or animation states to be stored in the same texture. They can be updated at run time to play the new skin or new animation state via SetSpriteAnimation(); 
Usage Attach this script to the object that has a material with the animation cell-sheet texture. From your other script call this script's SetSpriteAnimation() function with the new parameters: 
colCount: the total number of columns in the animation cell-sheet; 
rowCount: the total number of rows in the animation cell-sheet; 
rowNumber: the row where this animation will start; 
colNumber: the column where this animation will start; 
totalCells: the number of cells in this animation; 
fps: the number of cells (frames) per second the animation will play; 
Example  
Example function call: SetSpriteAnimation(4,4,1,0,4,12); Should animate the 4 green cells starting with the left most cell and at a speed of 12 cells per second. 
Here is an example scene : File:AnimatedTex.zip 
JavaScript - AnimatedTextureExtendedUV.js //vars for the whole sheet
var colCount	: int =  4;
var rowCount	: int =  4;
 
//vars for animation
var rowNumber	: int =  0; //Zero Indexed
var colNumber	: int =  0; //Zero Indexed
var totalCells	: int =  4;
var fps		: int = 10;
var offset	: Vector2;  //Maybe this should be a private var
 
//Update
function Update () { SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells,fps);  }
 
//SetSpriteAnimation
function SetSpriteAnimation(colCount : int,rowCount : int,rowNumber : int,colNumber : int,totalCells : int,fps : int){
 
	// Calculate index
	var index : int = Time.time * fps;
	// Repeat when exhausting all cells
	index = index % totalCells;
 
	// Size of every cell
	var size = Vector2 (1.0 / colCount, 1.0 / rowCount);
 
	// split into horizontal and vertical index
	var uIndex = index % colCount;
	var vIndex = index / colCount;
 
	// build offset
	// v coordinate is the bottom of the image in opengl so we need to invert.
	offset = Vector2 ((uIndex+colNumber) * size.x, (1.0 - size.y) - (vIndex+rowNumber) * size.y);
 
	renderer.material.SetTextureOffset ("_MainTex", offset);
	renderer.material.SetTextureScale  ("_MainTex", size);
}CSharp - AnimatedTextureExtendedUV.cs This is just a CSharp version of the AnimatedTextureExtendedUV.js script above. 
public class AnimatedTextureExtendedUV : MonoBehaviour
{
 
	//vars for the whole sheet
public int colCount =  4;
public int rowCount =  4;
 
//vars for animation
public int  rowNumber  =  0; //Zero Indexed
public int colNumber = 0; //Zero Indexed
public int totalCells = 4;
public int  fps     = 10;
  //Maybe this should be a private var
    private Vector2 offset;
//Update
void Update () { SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells,fps);  }
 
//SetSpriteAnimation
void SetSpriteAnimation(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells,int fps ){
 
    // Calculate index
    int index  = (int)(Time.time * fps);
    // Repeat when exhausting all cells
    index = index % totalCells;
 
    // Size of every cell
    float sizeX = 1.0f / colCount;
    float sizeY = 1.0f / rowCount;
    Vector2 size =  new Vector2(sizeX,sizeY);
 
    // split into horizontal and vertical index
    var uIndex = index % colCount;
    var vIndex = index / colCount;
 
    // build offset
    // v coordinate is the bottom of the image in opengl so we need to invert.
    float offsetX = (uIndex+colNumber) * size.x;
    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
    Vector2 offset = new Vector2(offsetX,offsetY);
 
    renderer.material.SetTextureOffset ("_MainTex", offset);
    renderer.material.SetTextureScale  ("_MainTex", size);
}
}Boo - AnimatedTextureExtendedUV.boo This is just a Boo version of the AnimatedTextureExtendedUV.js script above. 
import UnityEngine
 
class AnimatedTextureExtendedUV (MonoBehaviour): 
	# Vars for the Whole Sheet
	public colCount as int = 24 # Total Columns
	public rowCount as int = 1 # Total Rows
	#Vars for specific Animation
	public rowNumber as int = 0 # Start Row from the top zero indexed
	public colNumber as int = 0 # Start Column from the left zero indexed
	public totalCells as int = 24 # Total frames to run
	public fps as single = 10
	private offset as Vector2
 
	def Update ():	
		SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells,fps)
 
	def SetSpriteAnimation(colCount as int, rowCount as int, rowNumber as int, colNumber as int, totalCells as int, fps as int):
		# Calculate index
		index as int = Time.time * fps
		# Repeat when exhausting all frames
		index = index % totalCells
 
		# Size of every Cell
		sizeX = 1.0f / colCount
		sizeY = 1.0f / rowCount
		size = Vector2(sizeX, sizeY)
 
		# Split into horizontal and vertical index
		uIndex = index % colCount
		vIndex = index / colCount
 
		# Build offset
		# v coordinate is the bottom of the image in opengl so we need to invert.
		offsetX = (uIndex+colNumber) * size.x
		offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y
		offset = Vector2(offsetX, offsetY)
 
		renderer.material.SetTextureOffset("_MainTex", offset)
		renderer.material.SetTextureScale("_MainTex", size)
}
