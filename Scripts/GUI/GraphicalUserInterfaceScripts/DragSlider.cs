// Original url: http://wiki.unity3d.com/index.php/DragSlider
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/DragSlider.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Author: Spaniard 
Contents [hide] 
1 Description 
2 Usage 
3 C# - MoveSlider.cs 
4 JS - MoveSlider.js 

DescriptionThis script uses OnMouseDrag() to create a draggable GUI slider element. 
UsageAttach this script to the GUITexture that you want to be the sider widget (the little ball or box that people click on and drag around). You will need a second GUITexture that is the slider bar. Set the min, max and current values for the slider in the inspector and enjoy! 
When positioning the two GUITextures, they both must have the same transform, as the pixel insets for the slider widget will be set based on the pixel insets from the slider bar. 
C# - MoveSlider.cs// MoveSlider.cs by spaniard 2006-07-19
 
using UnityEngine;
using System.Collections;
 
public class MoveSlider : MonoBehaviour {
   // This script requires that the sliderBar and sliderWidget have the same transform
   // Positioning is done using pixelOffsets
   public GUITexture sliderBar = null;
   public GUITexture sliderWidget = null;
   public float minValue = 0;
   public float maxValue = 100;
   public float currentValue = 0;
 
   private bool connectedToMouse = false;
   private float originalX = 0;
   private float originalMouseX = 0;
   private float currentX = 0;
   private float halfWidgetWidth = 0;      // used to centre the widget at either end
 
   void Start () {
      halfWidgetWidth = (sliderWidget.pixelInset.xMax - sliderWidget.pixelInset.xMin) / 2;
      currentX = sliderWidget.pixelInset.xMin + halfWidgetWidth;
   }
 
   void OnMouseDown () {
      connectedToMouse = true;
      originalMouseX = Input.mousePosition.x;
      originalX = sliderWidget.pixelInset.xMin + halfWidgetWidth;
   }
 
   void OnMouseUp () {
      connectedToMouse = false;
   }
 
   void Update () {
      if (connectedToMouse == true) {
         // calculate currentX based on mouse position
         currentX = originalX - (originalMouseX - Input.mousePosition.x);
         // translate from the pixel values to the value
         currentValue = (((currentX - sliderBar.pixelInset.xMin) / (sliderBar.pixelInset.xMax - sliderBar.pixelInset.xMin)) * (maxValue - minValue)) + minValue;
      }
      // make sure the value isn't out of bounds
      if (currentValue < minValue) {
         currentValue = minValue;
      } else if (currentValue > maxValue) {
         currentValue = maxValue;
      }
      // update where the sliderWidget is drawn from currentValue (in case the value is changed externally)
      currentX = (((currentValue - minValue) / (maxValue - minValue)) * (sliderBar.pixelInset.xMax - sliderBar.pixelInset.xMin)) + sliderBar.pixelInset.xMin;
      sliderWidget.pixelInset = new Rect ((currentX - halfWidgetWidth), sliderWidget.pixelInset.yMin, (currentX + halfWidgetWidth) - (currentX - halfWidgetWidth), sliderWidget.pixelInset.yMax - sliderWidget.pixelInset.yMin);
 
      // now do something with that new value!
      AudioListener.volume = currentValue;
   }
}JS - MoveSlider.js// MoveSlider.cs by spaniard 2006-07-19
// Modified by Ippokratis 19-April-2011
// Changelog: Translated to UnityScript
 
// This script requires that the sliderBar and sliderWidget have the same transform
// Positioning is done using pixelOffsets
var sliderBar : GUITexture = null;
var sliderWidget : GUITexture = null;
var minValue:float = 0.0;
var maxValue:float = 100.0;
var currentValue:float = 0.0;
 
private var connectedToMouse :boolean = false;
private var originalX : float = 0.0;
private var originalMouseX :float = 0.0;
private var currentX : float = 0.0;
private var halfWidgetWidth : float = 0.0;      // used to centre the widget at either end
 
function Start ()
{
	halfWidgetWidth = (sliderWidget.pixelInset.xMax - sliderWidget.pixelInset.xMin) * 0.5;
	currentX = sliderWidget.pixelInset.xMin + halfWidgetWidth;
}
 
function OnMouseDown ()
{
	connectedToMouse = true;
	originalMouseX = Input.mousePosition.x;
	originalX = sliderWidget.pixelInset.xMin + halfWidgetWidth;
}
 
function OnMouseUp () 
{
	connectedToMouse = false;
}
 
function Update ()
{
	if (connectedToMouse == true) 
	{
		// calculate currentX based on mouse position
		currentX = originalX - (originalMouseX - Input.mousePosition.x);
		// translate from the pixel values to the value
		currentValue = (((currentX - sliderBar.pixelInset.xMin) / 
		(sliderBar.pixelInset.xMax - sliderBar.pixelInset.xMin)) * 
		(maxValue - minValue)) + minValue;
	}
	// make sure the value isn't out of bounds
	currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
	// update where the sliderWidget is drawn from currentValue (in case the value is changed externally)
	currentX = (((currentValue - minValue) / (maxValue - minValue)) * 
	(sliderBar.pixelInset.xMax - sliderBar.pixelInset.xMin)) + 
	sliderBar.pixelInset.xMin;
 
	sliderWidget.pixelInset = Rect ((currentX - halfWidgetWidth), sliderWidget.pixelInset.yMin, 
	(currentX + halfWidgetWidth) - (currentX - halfWidgetWidth), sliderWidget.pixelInset.yMax - 
	sliderWidget.pixelInset.yMin);
 
	// now do something with that new value!
	AudioListener.volume = currentValue;
}
}
