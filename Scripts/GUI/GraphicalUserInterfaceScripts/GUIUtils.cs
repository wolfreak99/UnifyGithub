// Original url: http://wiki.unity3d.com/index.php/GUIUtils
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/GUIUtils.cs
// File based on original modification date of: 20 January 2013, at 00:27. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Contents [hide] 
1 What is it? 
2 How to use GUIUtils ? 
3 Who do I contact for help / questions? 
4 The code 
4.1 The plugin script 
4.2 The testing script 
5 Legacy 
5.1 The plugin script 
5.2 The testing script 

What is it? It's a GUI helper written in UnityScript ("UnityJS") for creating rectangles with size and position absolute/relative to any side of the screen. 
This plugin was created for Unity 3.4 Compatible Unity 3.5 
How to use GUIUtils ? Mainly you just put a Vector4 in the function doing what you want (which point of the rectangle your working with) Vector4ToRect_XYWH where X = (L)eft/(C)enter/(R)ight et Y = (T)op/(M)iddle/(B)ottom. 
The first two coordinates are the position of the point of the rectangle you want to place. If coordinates are positive, the position is calculated from Top/Left side of the screen, else the position is calculated from Bottom/Right side of the screen. 
If coordinates/sizes have an absolute value higher than 1, then it is calculated with pixels, else it is calculated in Screen %. 
For example, using Vector4ToRect_CM( Vector4( 0.5, -0.3, 1.0, 200 ) ) create a Rectange with its center at 50% of the screen from left side of the screen (horizontally centered), at 30% of the screen from bottom side of the screen, with a width of 100% of the screen and a height of 200 pixels. 
Note : I used Vector4 so the variables are grouped together in the Inspector and you dont have to declare 4 different attributes. 
Who do I contact for help / questions? Feel free to email me if you upgrade/clean/adapt/translate this code. 
Indiana MAGNIEZ 
The code The plugin script #pragma strict
 
/*
 
GUIUtils.js
A GUI helper for the Unity Game Engine
 
Created by Indiana MAGNIEZ
 
*/
 
class GUIUtils
{
	private static var defaultDotPerInch:int = 130; // If Screen.dpi == 0 we use defaultDotPerInch insteed ( 130 is the actuel dpi for iPad2 )
 
	private var originalResolution:Vector2; // Resolution used for positionning the GUI
	private var fakeResolution:Vector2; // Resolution with dimensionToKeep tha same of originalResolution but with the actual ratio of the screen
	private var lastFrameResolution:Vector2; // Actual resolution of the screen last time GUIUtils.InitGUIScale() was called
 
	private var scaleToApplyToGUI:float = 1.0; // Scale applied to GUI.matrix
 
	private var dimensionToKeep:DimensionToKeep; // dimension to keep when creating a fakeResolution with a different ration than originalResolution
 
	public enum DimensionToKeep {
		Height,
		Width
	};
 
	public enum PointForPositionGUI {
		LeftTop,
		CenterTop,
		RightTop,
		LeftMiddle,
		CenterMiddle,
		RightMiddle,
		LeftBottom,
		CenterBottom,
		RightBottom
	};
 
	// Le ratio est toujours exprimé en Width/Height
	public enum Vector2UseDataType {
		WidthHeight, // Coordonnées X et Y
		WidthRatio, // Coordonnée X et Ratio X/Y
		HeightRatio // Coordonnée Y et Ratio X/Y
	};
 
	public class GuiDataStructure {
		public var positionData:Vector2;
		public var positionDataType:Vector2UseDataType;
		public var pointForPositionGUI:PointForPositionGUI;
		public var sizeData:Vector2;
		public var sizeDataType:Vector2UseDataType;
		public var pointReferenceOnTheScreen:PointForPositionGUI;
	};
 
	// Functions to call in Start and in OnGUI
 
	public function InitGUIScale( _originalResolution:Vector2, _dimensionToKeep:DimensionToKeep ):void
	{
		originalResolution = _originalResolution;
		lastFrameResolution = Vector2( Screen.width, Screen.height );
 
		var currentResolution:Vector2 = Vector2( Screen.width, Screen.height );
		var currentRatio:float = currentResolution.x / currentResolution.y;
 
		dimensionToKeep = _dimensionToKeep;
 
		if( dimensionToKeep == DimensionToKeep.Height )
		{
			scaleToApplyToGUI = currentResolution.y / originalResolution.y;
			fakeResolution = Vector2( originalResolution.y * currentRatio , originalResolution.y );
		}
		else if( dimensionToKeep == DimensionToKeep.Width )
		{
			scaleToApplyToGUI = currentResolution.x / originalResolution.x;
			fakeResolution = Vector2( originalResolution.x, originalResolution.x / currentRatio );
		}
		else
		{
			// ERROR
			Debug.LogError("dimensionToKeep is incorrect in GUIUtils");
			return;
		}
//		Debug.Log("GUIUtils :"
//			+"\n- originalResolution : "+originalResolution
//			+"\n- currentResolution : "+currentResolution
//			+"\n- currentRatio : "+currentRatio
//			+"\n- fakeResolution : "+fakeResolution);
	}
 
	public function HasResolutionChanged():boolean
	{
		return ( lastFrameResolution.x != Screen.width || lastFrameResolution.y != Screen.height );
	}
 
	public function GetScaleToApplyToGUI():float
	{
		return scaleToApplyToGUI;
	}
 
	public function ApplyGuiTransform():void
	{
		GUI.matrix = Matrix4x4.Scale( Vector3.one * GetScaleToApplyToGUI() );
	}
 
	// Functions to call in CalculateGUI()
 
	public function GetRectangleForGUI( guiData:GUIUtils.GuiDataStructure ):Rect
	{
		return GetRectangleForGUI( guiData.positionData, guiData.positionDataType, guiData.pointForPositionGUI, guiData.sizeData, guiData.sizeDataType, guiData.pointReferenceOnTheScreen );
	}
 
	public function GetRectangleForGUI( positionData:Vector2, positionDataType:Vector2UseDataType, pointForPositionGUI:PointForPositionGUI, sizeData:Vector2, sizeDataType:Vector2UseDataType ):Rect
	{
		return GetRectangleForGUI( positionData, positionDataType, pointForPositionGUI, sizeData, sizeDataType, PointForPositionGUI.LeftTop );
	}
 
	public function GetRectangleForGUI( positionData:Vector2, positionDataType:Vector2UseDataType, pointForPositionGUI:PointForPositionGUI, sizeData:Vector2, sizeDataType:Vector2UseDataType, pointReferenceOnTheScreen:PointForPositionGUI ):Rect
	{
		var xPos:float = 0.0;
		var yPos:float = 0.0;
		var ratioPos:float = 0.0;
		var xSize:float = 0.0;
		var ySize:float = 0.0;
		var ratioSize:float = 0.0;
 
//		var xPosFinal:float = 0.0;
//		var yPosFinal:float = 0.0;
//		var xSizeFinal:float = 0.0;
//		var ySizeFinal:float = 0.0;
 
 
		// On calcule la position du point référence pour placer l'élément de GUI
		switch( positionDataType )
		{
			case Vector2UseDataType.WidthHeight :
				xPos = positionData.x;
				yPos = positionData.y;
				if( Mathf.Abs(xPos) <= 1 )
				{
				    xPos = xPos * fakeResolution.x;
				}
				if( Mathf.Abs(yPos) <= 1 )
				{
				    yPos = yPos * fakeResolution.y;
				}
				break;
			case Vector2UseDataType.WidthRatio :
				xPos = positionData.x;
				ratioPos = positionData.y;
				if( Mathf.Abs(xPos) <= 1 )
				{
				    xPos = xPos * fakeResolution.x;
				}
				if( ratioPos != 0 )
				{
					yPos = xPos / ratioPos;
				}
				break;
			case Vector2UseDataType.HeightRatio :
				yPos = positionData.x; // La première coordonnée de positionData est bel et bien yPos
				ratioPos = positionData.y;
				if( Mathf.Abs(yPos) <= 1 )
				{
				    yPos = yPos * fakeResolution.y;
				}
				if( ratioPos != 0 )
				{
					xPos = yPos * ratioPos;
				}
				break;
			default :
				Debug.LogWarning("Incorrect positionDataType in GUIUtils.GetRectangleForGUI : "+positionDataType);
				return Rect(0,0,0,0);
//				break;
		}
 
		// On calcule la taille de l'élément de GUI
		switch( sizeDataType )
		{
			case Vector2UseDataType.WidthHeight :
				xSize = sizeData.x;
				ySize = sizeData.y;
				if( Mathf.Abs(xSize) <= 1 )
				{
				    xSize = Mathf.Abs(xSize) * fakeResolution.x;
				}
				if( Mathf.Abs(ySize) <= 1 )
				{
				    ySize = Mathf.Abs(ySize) * fakeResolution.y;
				}
				break;
			case Vector2UseDataType.WidthRatio :
				xSize = sizeData.x;
				ratioSize = sizeData.y;
				if( Mathf.Abs(xSize) <= 1 )
				{
				    xSize = Mathf.Abs(xSize) * fakeResolution.x;
				}
				if( ratioSize != 0 )
				{
					ySize = xSize / ratioSize;
				}
				break;
			case Vector2UseDataType.HeightRatio :
				ySize = sizeData.x; // La première coordonnée de sizeData est bel et bien ySize
				ratioSize = sizeData.y;
				if( Mathf.Abs(ySize) <= 1 )
				{
				    ySize = ySize * fakeResolution.y;
				}
				if( ratioSize != 0 )
				{
					xSize = ySize * ratioSize;
				}
				break;
			default :
				Debug.LogWarning("Incorrect sizeDataType in GUIUtils.GetRectangleForGUI : "+sizeDataType);
				return Rect(0,0,0,0);
//				break;
		}
 
		// On applique une translation aux coordonnées pour placer l'élément relativement à un point de l'écran
		switch( pointReferenceOnTheScreen )
		{
			case PointForPositionGUI.LeftTop :
				// Do nothing, already the good coordinates
				break;
			case PointForPositionGUI.CenterTop :
				xPos = xPos + fakeResolution.x/2.0;
				break;
			case PointForPositionGUI.RightTop :
				xPos = xPos + fakeResolution.x;
				break;
			case PointForPositionGUI.LeftMiddle :
				yPos = yPos + fakeResolution.y/2.0;
				break;
			case PointForPositionGUI.CenterMiddle :
				xPos = xPos + fakeResolution.x/2.0;
				yPos = yPos + fakeResolution.y/2.0;
				break;
			case PointForPositionGUI.RightMiddle :
				xPos = xPos + fakeResolution.x;
				yPos = yPos + fakeResolution.y/2.0;
				break;
			case PointForPositionGUI.LeftBottom :
				yPos = yPos + fakeResolution.y;
				break;
			case PointForPositionGUI.CenterBottom :
				xPos = xPos + fakeResolution.x/2.0;
				yPos = yPos + fakeResolution.y;
				break;
			case PointForPositionGUI.RightBottom :
				xPos = xPos + fakeResolution.x;
				yPos = yPos + fakeResolution.y;
				break;
			default :
				Debug.LogWarning("Incorrect pointReferenceOnTheScreen in GUIUtils.GetRectangleForGUI : "+pointReferenceOnTheScreen);
				return Rect(0,0,0,0);
//				break;
		}
 
		// On applique le positionnement par rapport au côté opposé pour les positions négatives
		if( xPos < 0 )
		{
		    xPos = fakeResolution.x + xPos; // La valeur est déjà négative, pas la peine de mettre un signe moins.
		}
		if( yPos < 0 )
		{
		    yPos = fakeResolution.y + yPos; // La valeur est déjà négative, pas la peine de mettre un signe moins.
		}
 
		// Calcul des coordonnées du point en haut à gauche du rect : le point utilisé par le constructeur Rect ne peut pas être choisi.
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		switch( pointForPositionGUI )
		{
			case PointForPositionGUI.LeftTop :
				// Do nothing, already the good coordinates
				break;
			case PointForPositionGUI.CenterTop :
				xPos = xPos - xSize/2.0;
				break;
			case PointForPositionGUI.RightTop :
				xPos = xPos - xSize;
				break;
			case PointForPositionGUI.LeftMiddle :
				yPos = yPos - ySize/2.0;
				break;
			case PointForPositionGUI.CenterMiddle :
				xPos = xPos - xSize/2.0;
				yPos = yPos - ySize/2.0;
				break;
			case PointForPositionGUI.RightMiddle :
				xPos = xPos - xSize;
				yPos = yPos - ySize/2.0;
				break;
			case PointForPositionGUI.LeftBottom :
				yPos = yPos - ySize;
				break;
			case PointForPositionGUI.CenterBottom :
				xPos = xPos - xSize/2.0;
				yPos = yPos - ySize;
				break;
			case PointForPositionGUI.RightBottom :
				xPos = xPos - xSize;
				yPos = yPos - ySize;
				break;
			default :
				Debug.LogWarning("Incorrect pointForPositionGUI in GUIUtils.GetRectangleForGUI : "+pointForPositionGUI);
				return Rect(0,0,0,0);
//				break;
		}
 
		return Rect(xPos,yPos,xSize,ySize);
	}
 
 
	/**
	 * @Name : CreateRectangleFromLimits
	 * @Arguments :
	 * - xMin:float : Left coordinate of the rectangle
	 * - yMin:float : Top coordinate of the rectangle
	 * - xMax:float : Right coordinate of the rectangle
	 * - yMax:float : Bottom coordinate of the rectangle
	 * @Description :
	 *      This function return a rectangle whose xMin, xMax, yMin and yMax are these given in argument.
	 * @Return : Rect
	 **/
	public static function CreateRectangleFromLimits(xMin:float, xMax:float, yMin:float, yMax:float):Rect {
	    var temp:float;
	    if( xMin > xMax )
	    {
	        temp = xMin;
	        xMin = xMax;
	        xMax = temp;
	    }
	    if( yMin > yMax )
	    {
	        temp = yMin;
	        yMin = yMax;
	        yMax = temp;
	    }
	    // Rect (left, top, width, height)
	    return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
	}
}The testing script You can use this script adding it on an object in your scene after addition of GUIUtils.js to your project (ideally in Plugins). 
#pragma strict
 
class TestGUIUtils extends MonoBehaviour
{
	public var originalResolution:Vector2 = Vector2(750,500);
 
	public var guiDimensionToKeep:GUIUtils.DimensionToKeep;
 
	public var guiData:GUIUtils.GuiDataStructure;
 
	public var guiRectCalculated:Rect = Rect(0,0,0,0);
 
	public var dynamicGUICalculation:boolean = true;
 
	private var guiUtils:GUIUtils;
 
	// Fonction de GUIUtils
	// InitGUIScale( _originalResolution:Vector2, _dimensionToKeep:DimensionToKeep ):void
	// GetScaleToApplyToGUI():float
	// GetRectangleForGUI( positionData:Vector2, positionDataType:Vector2UseDataType, pointForPositionGUI:PointForPositionGUI, sizeData:Vector2, sizeDataType:Vector2UseDataType, guiPointReferenceOnScreen ):Rect
 
	function Start ()
	{
		if( guiUtils == null )
		{
			guiUtils = new GUIUtils();
		}
		CalculateGUI();
	}
 
	function CalculateGUI():void
	{
		guiUtils.InitGUIScale( originalResolution, guiDimensionToKeep );
 
		guiRectCalculated = guiUtils.GetRectangleForGUI( guiData );
	}
 
	function OnGUI()
	{
	   	if( guiUtils.HasResolutionChanged() == true )
		{
			CalculateGUI();
		}
		guiUtils.ApplyGuiTransform();
 
		GUI.Button( guiRectCalculated, "Ceci est un bouton." );
 
		OnGUITestMenu();
	}
 
	public function OnGUITestMenu()
	{
		GUI.matrix = Matrix4x4.Scale( Vector3.one );
 
		GUILayout.BeginArea(Rect(0,50,300,500));
		GUILayout.BeginVertical();
 
		if( GUILayout.Button("Reinitialize GUIUtils") )
		{
			CalculateGUI();
		}
		if( 
			( !dynamicGUICalculation && GUILayout.Button("\nActivate dynamicGUICalculation\n") )
			|| ( dynamicGUICalculation && GUILayout.Button("\nDesactivate dynamicGUICalculation\n") )
		)
		{
			dynamicGUICalculation = !dynamicGUICalculation;
		}
 
		if( dynamicGUICalculation )
		{
			CalculateGUI();
		}
 
		GUILayout.EndVertical();
		GUILayout.EndArea();
	};
}Legacy The plugin script /*
 
GUIUtils.js
A GUI helper for the Unity Game Engine
 
Created by Indiana MAGNIEZ
 
*/
 
class GUIUtils 
{
	/**
	 * @Name : CreateRectangleFromLimits
	 * @Arguments :
	 * - xMin:float : Left coordinate of the rectangle
	 * - yMin:float : Top coordinate of the rectangle
	 * - xMax:float : Right coordinate of the rectangle
	 * - yMax:float : Bottom coordinate of the rectangle
	 * @Description :
	 * 		This function return a rectangle whose xMin, xMax, yMin and yMax are these given in argument.
	 * @Return : Rect
	 **/
	public static function CreateRectangleFromLimits(xMin:float, xMax:float, yMin:float, yMax:float):Rect {
		var temp:float;
		if ( xMin > xMax ) {
			temp = xMin;
			xMin = xMax;
			xMax = temp;
		}
		if ( yMin > yMax ) {
			temp = yMin;
			yMin = yMax;
			yMax = temp;
		}
		// Rect (left, top, width, height)
		return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
	}
 
	private static function ConvertCoordInPixels(coord:Vector4):Vector4
	{
		var xPos:float = coord[0];
		var yPos:float = coord[1];
		var xSize:float = coord[2];
		var ySize:float = coord[3];
 
		// On multiplie les valeurs relatives par la taille de l'écran
		if( Mathf.Abs(xPos) <= 1 )
		{
			xPos = xPos * Screen.width;
		}
		if( Mathf.Abs(xSize) <= 1 )
		{
			xSize = Mathf.Abs(xSize) * Screen.width;
		}
		if( Mathf.Abs(yPos) <= 1 )
		{
			yPos = yPos * Screen.height;
		}
		if( Mathf.Abs(ySize) <= 1 )
		{
			ySize = Mathf.Abs(ySize) * Screen.height;
		}
 
		// On applique le positionnement par rapport au côté opposé pour les positions négatives
		if( xPos < 0 )
		{
			xPos = Screen.width + xPos; // La valeur est déjà négative, pas la peine de mettre un signe moins.
		}
		if( yPos < 0 )
		{
			yPos = Screen.height + yPos; // La valeur est déjà négative, pas la peine de mettre un signe moins.
		}
 
		return Vector4(xPos,yPos,xSize,ySize);
	}
 
	// Liste des fonctions de conversions pour GUI dynamique Vector4ToRect_XYWH où X = (L)eft/(C)enter/(R)ight et Y = (T)op/(M)iddle/(B)ottom
 
	/**
	 * Nom : Vector4ToRect_LTHW (LeftTop)
	 * Description :
	 * 		Cette fonction permet de créer un rectangle utilisable en GUI à partir de quatre composantes
	 * @param coord:Vector4
	 * 		Vecteur dont les composantes sont la position par rapport au bord gauche, la position par rapport au bord haut, la largeur et la hauteur du coin haut gauche du rectangle créé
	 * 		Pour les quatre composantes, si la valeur absolue est comprise entre 0 et 1, la position est relative à la taille de l'écran, sinon, elle est en pixels.
	 * 		Pour les deux premières composantes, une valeur négative se basera sur le bord opposé (bas au lieu de haut, droit au lieu de gauche).
	 * @return Rect
	 * 		Le rectangle qui occupe la place correspondant aux quatre composantes.
	 **/
	public static function Vector4ToRect_LTWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
 
		var xPos:float = coord[0];
		var yPos:float = coord[1];
		var xSize:float = coord[2];
		var ySize:float = coord[3];
		return Rect(xPos,yPos,xSize,ySize);
	}
 
	/**
	 * Nom : Vector4ToRect_CMHW (CenterMiddle)
	 * Description :
	 * 		Cette fonction permet de créer un rectangle utilisable en GUI à partir de quatre composantes
	 * @param coord:Vector4
	 * 		Vecteur dont les composantes sont la position par rapport au bord gauche, la position par rapport au bord haut, la largeur et la hauteur du centre du rectangle créé
	 * 		Pour les quatre composantes, si la valeur absolue est comprise entre 0 et 1, la position est relative à la taille de l'écran, sinon, elle est en pixels.
	 * 		Pour les deux premières composantes, une valeur négative se basera sur le bord opposé (bas au lieu de haut, droit au lieu de gauche).
	 * @return Rect
	 * 		Le rectangle qui occupe la place correspondant aux quatre composantes.
	 **/
	public static function Vector4ToRect_CMWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
 
		var xPos:float = coord[0];
		var yPos:float = coord[1];
		var xSize:float = coord[2];
		var ySize:float = coord[3];
 
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		xPos = xPos - xSize/2;
		yPos = yPos - ySize/2;
		return Rect(xPos,yPos,xSize,ySize);
	}
 
	public static function Vector4ToRect_CTWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		return Rect(
			coord[0] - coord[2]/2,
			coord[1],
			coord[2],
			coord[3]
			);
	}
 
	public static function Vector4ToRect_RTWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		return Rect(
			coord[0] - coord[2],
			coord[1],
			coord[2],
			coord[3]
			);
	}
 
	public static function Vector4ToRect_LMWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		return Rect(
			coord[0],
			coord[1] - coord[3]/2,
			coord[2],
			coord[3]
			);
	}
 
	public static function Vector4ToRect_RMWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		return Rect(
			coord[0] - coord[2],
			coord[1] - coord[3]/2,
			coord[2],
			coord[3]
			);
	}
 
	public static function Vector4ToRect_LBWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		return Rect(
			coord[0],
			coord[1] - coord[3],
			coord[2],
			coord[3]
			);
	}
 
	public static function Vector4ToRect_CBWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		return Rect(
			coord[0] - coord[2]/2,
			coord[1] - coord[3],
			coord[2],
			coord[3]
			);
	}
 
	public static function Vector4ToRect_RBWH(coord:Vector4):Rect
	{
		coord = ConvertCoordInPixels(coord);
		// On place le rectangle dont la position est TOUJOURS celle du coin en haut à gauche correctement par rapport aux valeurs du vecteur
		return Rect(
			coord[0] - coord[2],
			coord[1] - coord[3],
			coord[2],
			coord[3]
			);
	}
}The testing script var size:int = 100;
var temoin:boolean = false;
 
function OnGUI()
{
	if( temoin )
	{
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
 
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
 
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
		GUI.Box( Rect(size,size,size,size) , "size" );
	}
	else
	{
		GUI.Box( GUIUtils.Vector4ToRect_LTWH( Vector4( 1.0/6.0 , 1.0/6.0 , size , size ) ), "LT" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 1.0/6.0 , 1.0/6.0 , size , size ) ), "CM" );
		GUI.Box( GUIUtils.Vector4ToRect_CTWH( Vector4( 3.0/6.0 , 1.0/6.0 , size , size ) ), "CT" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 3.0/6.0 , 1.0/6.0 , size , size ) ), "CM" );
		GUI.Box( GUIUtils.Vector4ToRect_RTWH( Vector4( -1.0/6.0 , 1.0/6.0 , size , size ) ), "RT" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( -1.0/6.0 , 1.0/6.0 , size , size ) ), "CM" );
 
		GUI.Box( GUIUtils.Vector4ToRect_LMWH( Vector4( 1.0/6.0 , 3.0/6.0 , size , size ) ), "LM" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 1.0/6.0 , 3.0/6.0 , size , size ) ), "CM" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 3.0/6.0 , 3.0/6.0 , size , size ) ), "CM" );
		GUI.Box( GUIUtils.Vector4ToRect_RMWH( Vector4( 5.0/6.0 , 3.0/6.0 , size , size ) ), "RM" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 5.0/6.0 , 3.0/6.0 , size , size ) ), "CM" );
 
		GUI.Box( GUIUtils.Vector4ToRect_LBWH( Vector4( 1.0/6.0 , 5.0/6.0 , size , size ) ), "LB" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 1.0/6.0 , 5.0/6.0 , size , size ) ), "CM" );
		GUI.Box( GUIUtils.Vector4ToRect_CBWH( Vector4( 3.0/6.0 , 5.0/6.0 , size , size ) ), "CB" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 3.0/6.0 , 5.0/6.0 , size , size ) ), "CM" );
		GUI.Box( GUIUtils.Vector4ToRect_RBWH( Vector4( 5.0/6.0 , 5.0/6.0 , size , size ) ), "RB" );
		GUI.Box( GUIUtils.Vector4ToRect_CMWH( Vector4( 5.0/6.0 , 5.0/6.0 , size , size ) ), "CM" );
	}
}
}
