/*************************
 * Original url: http://wiki.unity3d.com/index.php/Gesture_Recognizer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/Gesture_Recognizer.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Description 
 *   
 * Usage 
 *   
 * JavaScript - gesture.js 
 *   
 * JavaScript - gestureRecognizer.js 
 *   
 * JavaScript - gestureTemplates.js 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
    DescriptionI developed a Gesture Recognizer script with the purpose of creating a new interaction method for a game at viezel studios [1] and now release for the community. It is completely a math based recognizer concept including tweakable parameters to adjust the CPU usage vs recognizer accuracy. 
    The gesture recognizer consists of three script files: 
    - gesture.js 
    - gestureRecognizer.js 
    - gestureTemplates.js 
    
    I used the TronTrail [2] to display the gesture path while drawing it. Apply it to the FPS controller. 
    UsageYou can download a sample project with the complete setup here: GestureReg.zip‎ 
    
    UPDATE UPDATE UPDATE 
    Gabriel Cirera, from CosmonautGames contacted me today with a C# edition of my code.. Thanks for that, I guess alot could use this instead of the javascript! GestureRecognizerC#‎ 
    UPDATE UPDATE UPDATE 
    JavaScript - gesture.jsGesture.js is the main script file. It contains the functionality of controlling the events and recording the user’s inputs to the system. Add this script to an empty game object. Within Start() the MouseLook script in the camera component is disabled, for optimal control with the purpose of gesture recognition. 
    The user right clicks (held down) in order to draw a gesture in the 3D scene. When released the recognizer starts. The gesture will be recorded and added to the gesture template bank if the user hold down CTRL while right clicking. 
    
    
    // gesture.js - Add this to a empty game object
     
    static var avatar : GameObject;
    static var avatarCam : GameObject;
    static var gestureDrawing : GameObject;
    static var GuiText : GameObject;
    static var enterText : GUIText;
     
    var p : Vector2;
    var pointArr : Array;
    static var mouseDown;
     
    function Start () {
     
    	pointArr = new Array ();
     
    	gestureDrawing = GameObject.Find("gesture");
    	GuiText = GameObject.Find("GUIText");
    	GuiText.guiText.text = GuiText.guiText.text + "\n Templates loaded: " + gestureTemplates.Templates.length;
    	avatar = GameObject.FindWithTag("Player");
     
    	// Find the main camera and disable the default mouselook script.
    	avatarCam = GameObject.FindWithTag("MainCamera");
    	avatarCam.GetComponent (MouseLook).enabled=false;
    }
     
     
    function worldToScreenCoordinates() {
    	// fix world coordinate to the viewport coordinate
    	var screenSpace = Camera.main.WorldToScreenPoint(gestureDrawing.transform.position);
     
    	while (Input.GetMouseButton(1))
    	{
    		var curScreenSpace = Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
    		var curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace); 
    		gestureDrawing.transform.position = curPosition;
    		yield;
    	}
    }
     
    function Update() {
     
    	if (Input.GetMouseButtonDown(1)) {
    		// run one time - click the right click button
    		avatar.GetComponent (MouseLook).enabled=false;
    		avatar.GetComponent (FPSWalker).enabled=false;
    		mouseDown = 1;
    	}
     
    	if (mouseDown == 1) {
    		p = Vector2(Input.mousePosition.x , Input.mousePosition.y);
    		pointArr.Add(p);
    		worldToScreenCoordinates();
    	}
     
     
    	if (Input.GetMouseButtonUp(1)) {
    		if (Input.GetKey (KeyCode.LeftControl)) {
    			// if CTRL is held down, the script will record a gesture. 
    			mouseDown = 0;
    			gestureRecognizer.recordTemplate(pointArr);
     
    		} else {
    			avatar.GetComponent (MouseLook).enabled=true;
    			avatar.GetComponent (FPSWalker).enabled=true;
    			mouseDown = 0;
     
    			// start recognizing! 
    			gestureRecognizer.startRecognizer(pointArr);
     
    			pointArr.Clear();
     
    		}
     
    	}
     
    } 
     
    function OnGUI () {
    	if(gestureRecognizer.recordDone == 1) { 
    		windowRect = GUI.Window (0, Rect (350, 220, 300, 100), DoMyWindow, "Save the template?");
    	}
    }
     
    function DoMyWindow (windowID : int) {
     
    	gestureRecognizer.stringToEdit = GUILayout.TextField (gestureRecognizer.stringToEdit);
     
    	if (GUI.Button (Rect (100,50,50,20), "Save")) {
    		gestureTemplates.Templates.Push(gestureRecognizer.newTemplateArr);
    		gestureTemplates.TemplateNames.Push(gestureRecognizer.stringToEdit);
    		gestureRecognizer.recordDone = 0;
    		gestureRecognizer.newTemplateArr.Clear();
     
    		GuiText.guiText.text = "TEMPLATE: " + gestureRecognizer.stringToEdit +  "\n STATUS: SAVED";
    	}
     
    	if (GUI.Button (Rect (160,50,50,20), "Cancel")) {
    		gestureRecognizer.recordDone = 0;
    		GuiText.guiText.text = "";
    	}
    }
    
    JavaScript - gestureRecognizer.jsThe gestureRecognizer.js script is based on the three steps: 
    Step 1. Optimize gesture 
    Step 2. Rotate & scale gesture 
    Step 3. Match gesture against a set of templates 
    
    As seen underneath there are four settings variables for the recognizer adjusting the accuracy and the CPU consuming the technique requires. From the gestures.js file the function startRecognizer is executed. This is the main function of the entire project. startRecognizer() follows the four steps of the article and returns a result if the gesture drawn by the user matches any of the templates. 
    
    
    static var pointArray : Array;				// The current gesture saved in this array
     
    // recognizer settings
    static var maxPoints = 64;				// Max number of point in the gesture
    static var sizeOfScaleRect = 500;			// The size of the bounding box
    static var compareDetail = 15;				// Number of matching iterations (CPU consuming) 
    static var angleRange = 45;				// Angle detail level of when matching with templates 
     
    static var recordDone = 0;
    static var recognitionDone = 0;
    static var gestureChosen = -1;
    static var stringToEdit = "Enter a Template name";
    static var newTemplateArr : Array;
     
     
    static function startRecognizer (pointArray) {
    	// main recognizer function
    	pointArray = optimizeGesture(pointArray, maxPoints);
    	var center = calcCenterOfGesture(pointArray);
    	var radians = Mathf.Atan2(center.y - pointArray[0].y, center.x - pointArray[0].x);
    	pointArray = RotateGesture(pointArray, -radians, center);
    	pointArray = ScaleGesture(pointArray, sizeOfScaleRect);
    	pointArray = TranslateGestureToOrigin(pointArray);
    	gestureMatch(pointArray); 
    	recognitionDone = 1;
    }
     
    static function recordTemplate (pointArray) {
    	// record function
     
    	pointArray = optimizeGesture(pointArray, maxPoints);
    	var center = calcCenterOfGesture(pointArray);
    	var radians = Mathf.Atan2(center.y - pointArray[0].y, center.x - pointArray[0].x);
    	pointArray = RotateGesture(pointArray, -radians, center);
    	pointArray = ScaleGesture(pointArray, sizeOfScaleRect);
    	pointArray = TranslateGestureToOrigin(pointArray);
     
    	newTemplateArr = new Array ();
    	newTemplateArr = pointArray;
     
    	recordDone = 1;
     
    }
     
     
    static function optimizeGesture(pointArray, maxPoints) {
    	// take all the points in the gesture and finds the correct points compared with distance and the maximun value of points
     
    	// calc the interval relative the length of the gesture drawn by the user
    	var interval = calcTotalGestureLength(pointArray) / (maxPoints - 1);
     
    	// use the same starting point in the new array from the old one. 
    	var optimizedPoints = new Array(pointArray[0]);
     
    	var tempDistance = 0.0;
     
    	// run through the gesture array. Start at i = 1 because we compare point two with point one)
    	for (var i : int = 1; i < pointArray.length; i++)	{
    		var currentDistanceBetween2Points = calcDistance(pointArray[i - 1] , pointArray[i]);
     
    		if ((tempDistance + currentDistanceBetween2Points) >= interval) {
     
    			// the calc is: old pixel + the differens of old and new pixel multiply  
    			var newX = pointArray[i - 1].x + ((interval - tempDistance) / currentDistanceBetween2Points) * (pointArray[i].x - pointArray[i - 1].x);
    			var newY = pointArray[i - 1].y + ((interval - tempDistance) / currentDistanceBetween2Points) * (pointArray[i].y - pointArray[i - 1].y);
     
    			// create new point
    			var newPoint = new Vector2(newX, newY);
     
    			// set new point into array
    			optimizedPoints[optimizedPoints.length] = newPoint;
     
    			pointArray.splice(i, 0, newPoint); 
     
    			tempDistance = 0.0;
    		} else {
    			// the point was too close to the last point compared with the interval,. Therefore the distance will be stored for the next point to be compared.
    			tempDistance += currentDistanceBetween2Points;
    		}
    	}
     
    	// Rounding-errors might happens. Just to check if all the points are in the new array
    	if (optimizedPoints.length == maxPoints - 1) {
    		optimizedPoints[optimizedPoints.length] = new Vector2(pointArray[pointArray.length - 1].x, pointArray[pointArray.length - 1].y);
    	}
    	return optimizedPoints;
     
    }
     
     
     
    static function RotateGesture(pointArray, radians, center)  {
    	// loop through original array, rotate each point and return the new array
    	var newArray = new Array();
    	var cos = Mathf.Cos(radians);
    	var sin = Mathf.Sin(radians);
     
    	for (var i = 0; i < pointArray.length; i++) {
    		var newX = (pointArray[i].x - center.x) * cos - (pointArray[i].y - center.y) * sin + center.x;
    		var newY = (pointArray[i].x - center.x) * sin + (pointArray[i].y - center.y) * cos + center.y;
    		newArray[newArray.length] = new Vector2(newX, newY);
    	}
    	return newArray;
    }
     
    static function ScaleGesture(pointArray, size) {
     
    	// equal min and max to the opposite infinity, such that every gesture size can fit the bounding box.
    	var minX = Mathf.Infinity;
    	var maxX = Mathf.NegativeInfinity; 
    	var minY = Mathf.Infinity;
    	var maxY = Mathf.NegativeInfinity;
     
    	// loop through array. Find the minimum and maximun values of x and y to be able to create the box
    	for (var i = 0; i < pointArray.length; i++) {
    		if (pointArray[i].x < minX) {
    			minX = pointArray[i].x;
    		}
    		if (pointArray[i].x > maxX) {
    			maxX = pointArray[i].x;
    		}
    		if (pointArray[i].y < minY) {
    			minY = pointArray[i].y;
    		}
    		if (pointArray[i].y > maxY) {
    			maxY = pointArray[i].y;
    		}
    	}
     
    	// create a rectangle surronding the gesture as a bounding box.
    	var BoundingBox = Rect(minX, minY, maxX - minX, maxY - minY);
     
     
    	var newArray = new Array();
     
    	for (i = 0; i < pointArray.length; i++) {
    		var newX = pointArray[i].x * (size / BoundingBox.width);
    		var newY = pointArray[i].y * (size / BoundingBox.height);
    		newArray[newArray.length] = new Vector2(newX, newY);
    	}
    	return newArray;
    }
     
     
    static function TranslateGestureToOrigin(pointArray) {
    	var origin = Vector2(0,0);
    	var center = calcCenterOfGesture(pointArray);
    	var newArray = new Array();
     
    	for (var i = 0; i < pointArray.length; i++){
    		var newX = pointArray[i].x + origin.x - center.x;
    		var newY = pointArray[i].y + origin.y - center.y;
    		newArray[newArray.length] = new Vector2(newX, newY);
    	}
    	return newArray;
    }
     
     
    // --------------------------------  		     GESTURE OPTIMIZING DONE   		--------------------------------------------------------
    // -------------------------------- 		START OF THE MATCHING PROCESS	----------------------------------------------------------------
     
    static function gestureMatch(pointArray) {
    	var tempDistance = Mathf.Infinity;
    	var count = 0;
     
    	for (var i = 0; i < gestureTemplates.Templates.length; i++) {
    		var distance = calcDistanceAtOptimalAngle(pointArray, gestureTemplates.Templates[i], -angleRange, angleRange);
     
    		if (distance < tempDistance)	{
    			tempDistance = distance;
    			count = i;
    		}
     
    	}
    	var HalfDiagonal = 0.5 * Mathf.Sqrt(Mathf.Pow(sizeOfScaleRect, 2) + Mathf.Pow(sizeOfScaleRect, 2));
    	var score = 1.0 - (tempDistance / HalfDiagonal);
     
    	// print the result
     
    	if (score < 0.7) {
    		print("NO MATCH " + score );
    		gesture.GuiText.guiText.text = "RESULT: NO MATCH " +  "\n" + "SCORE: " + Mathf.Round(100 * score) +"%";
    		gestureChosen = -1;
    	} else {
    		print("RESULT: " + gestureTemplates.TemplateNames[count] + " SCORE: " + score);
    		gesture.GuiText.guiText.text = "RESULT: " + gestureTemplates.TemplateNames[count] + "\n" + "SCORE: " + Mathf.Round(100 * score) +"%";
    		gestureChosen = count;
    	}
     
    }
     
     
    // --------------------------------  		    GESTURE RECOGNIZER DONE   		----------------------------------------------------------------
    // -------------------------------- 		START OF THE HELP CALC FUNCTIONS	----------------------------------------------------------------
     
     
    static function calcCenterOfGesture(pointArray) {
    	// finds the center of the drawn gesture
     
    	var averageX : float = 0.0;
    	var averageY : float = 0.0;
     
    	for (var i : int = 0; i < pointArray.length; i++) {
    		averageX += pointArray[i].x;
    		averageY += pointArray[i].y;
    	}
     
    	averageX = averageX / pointArray.length;
    	averageY = averageY / pointArray.length;
     
    	return new Vector2(averageX, averageY);
    }	
     
    static function calcDistance(point1, point2) {
    	// distance between two vector points.
    	var dx = point2.x - point1.x;
    	var dy = point2.y - point1.y;
     
    	return Mathf.Sqrt(dx * dx + dy * dy);
    }
     
    static function calcTotalGestureLength(pointArray) { 
    	// total length of gesture path
    	var length = 0.0;
    	for (var i : int = 1; i < pointArray.length; i++) {
    		length += calcDistance(pointArray[i - 1], pointArray[i]);
    	}
    	return length;
    }
     
     
    static function calcDistanceAtOptimalAngle(pointArray, T, negativeAngle, positiveAngle) {
    	// Create two temporary distances. Compare while running through the angles. 
    	// Each time a lower distace between points and template points are foound store it in one of the temporary variables. 
     
    	radian1 = Mathf.PI * negativeAngle + (1.0 - Mathf.PI ) * positiveAngle;
    	tempDistance1 = calcDistanceAtAngle(pointArray, T, radian1);
     
    	radian2 = (1.0 - Mathf.PI ) * negativeAngle + Mathf.PI  * positiveAngle;
    	tempDistance2 = calcDistanceAtAngle(pointArray, T, radian2);
     
    	// the higher the number compareDetail is, the better recognition this system will perform. 
    	for (i = 0; i < compareDetail; i++) {
    		if (tempDistance1 < tempDistance2)	{
    			positiveAngle = radian2;
    			radian2 = radian1;
    			tempDistance2 = tempDistance1;
    			radian1 = Mathf.PI * negativeAngle + (1.0 - Mathf.PI) * positiveAngle;
    			tempDistance1 = calcDistanceAtAngle(pointArray, T, radian1);
    		} else {
    			negativeAngle = radian1;
    			radian1 = radian2;
    			tempDistance1 = tempDistance2;
    			radian2 = (1.0 - Mathf.PI) * negativeAngle + Mathf.PI * positiveAngle;
    			tempDistance2 = calcDistanceAtAngle(pointArray, T, radian2);
    		}
    	}
     
    	return Mathf.Min(tempDistance1, tempDistance2);
    }
     
    static function calcDistanceAtAngle(pointArray, T, radians) {
    	// calc the distance of template and user gesture at 
    	var center = calcCenterOfGesture(pointArray);
    	var newpoints = RotateGesture(pointArray, radians, center);
    	return calcGestureTemplateDistance(newpoints, T);
    }	
     
    static function calcGestureTemplateDistance(newRotatedPoints, templatePoints) {
    	// calc the distance between gesture path from user and the template gesture
    	var distance = 0.0;
    	for (var i = 0; i < newRotatedPoints.length; i++) { // assumes newRotatedPoints.length == templatePoints.length
    		distance += calcDistance(newRotatedPoints[i], templatePoints[i]);
    	}
    	return distance / newRotatedPoints.length;
    }
    
    JavaScript - gestureTemplates.jsThe gestureTemplates.js script is just holding the templates and names of all the gestures. 
    The best approach is to create your own templates for two reasons. These templates are optimized for resolution 1000 x 600. The second reason is that your game/application etc might find square, circle, triangle, cross, a, z, line and wave gesture a bit out of scope what you want. 
    
    You need at least two versions of each gesture. clockwise and counter clockwise, due to different users draws gesture differently. For optimal recognition record three or four versions of each gesture is recommended. 
    // gesture templates
     
    static var Templates : Array;
    static var TemplateNames : Array;
     
    	var square = new Array(Vector2(-252 , 0),Vector2(-233 , -13),Vector2(-215 , -26),Vector2(-199 , -42),Vector2(-181 , -56),Vector2(-166 , -73),Vector2(-151 , -90),Vector2(-137 , -108),Vector2(-122 , -126),Vector2(-107 , -143),Vector2(-95 , -162),Vector2(-85 , -182),Vector2(-72 , -201),Vector2(-59 , -220),Vector2(-46 , -239),Vector2(-32 , -252),Vector2(-18 , -234),Vector2(-2 , -218),Vector2(15 , -202),Vector2(31 , -186),Vector2(47 , -169),Vector2(63 , -154),Vector2(80 , -138),Vector2(97 , -123),Vector2(114 , -108),Vector2(131 , -92),Vector2(148 , -78),Vector2(166 , -64),Vector2(183 , -49),Vector2(201 , -35),Vector2(218 , -19),Vector2(235 , -5),Vector2(248 , 14),Vector2(241 , 32),Vector2(223 , 47),Vector2(207 , 62),Vector2(191 , 79),Vector2(176 , 96),Vector2(160 , 112),Vector2(146 , 130),Vector2(130 , 147),Vector2(115 , 164),Vector2(100 , 181),Vector2(84 , 198),Vector2(69 , 214),Vector2(54 , 231),Vector2(39 , 248),Vector2(29 , 234),Vector2(15 , 216),Vector2(-1 , 200),Vector2(-18 , 185),Vector2(-35 , 170),Vector2(-52 , 154),Vector2(-69 , 139),Vector2(-86 , 124),Vector2(-102 , 108),Vector2(-119 , 93),Vector2(-136 , 78),Vector2(-153 , 62),Vector2(-170 , 47),Vector2(-186 , 31),Vector2(-203 , 16),Vector2(-221 , 3),Vector2(-233 , -10) );
     
    	var circle = new Array( Vector2(-247 , 0) , Vector2(-246 , 26) , Vector2(-238 , 51) , Vector2(-224 , 73) , Vector2(-208 , 93) , Vector2(-190 , 111) , Vector2(-172 , 129) , Vector2(-154 , 146) , Vector2(-136 , 163) , Vector2(-115 , 177) , Vector2(-94 , 189) , Vector2(-73 , 201) , Vector2(-51 , 211) , Vector2(-29 , 222) , Vector2(-7 , 231) , Vector2(15 , 240) , Vector2(39 , 242) , Vector2(62 , 239) , Vector2(85 , 232) , Vector2(109 , 227) , Vector2(130 , 215) , Vector2(149 , 200) , Vector2(168 , 185) , Vector2(187 , 169) , Vector2(203 , 148) , Vector2(218 , 128) , Vector2(232 , 106) , Vector2(245 , 83) , Vector2(252 , 58) , Vector2(253 , 31) , Vector2(253 , 4) , Vector2(250 , -23) , Vector2(244 , -49) , Vector2(234 , -74) , Vector2(222 , -97) , Vector2(209 , -120) , Vector2(194 , -140) , Vector2(178 , -160) , Vector2(164 , -182) , Vector2(146 , -199) , Vector2(126 , -212) , Vector2(104 , -222) , Vector2(82 , -232) , Vector2(59 , -240) , Vector2(36 , -246) , Vector2(12 , -250) , Vector2(-11 , -255) , Vector2(-34 , -258) , Vector2(-57 , -250) , Vector2(-78 , -237) , Vector2(-97 , -221) , Vector2(-112 , -201) , Vector2(-128 , -181) , Vector2(-146 , -163) , Vector2(-161 , -143) , Vector2(-175 , -120) , Vector2(-187 , -97) , Vector2(-195 , -72) , Vector2(-204 , -47) , Vector2(-207 , -20) , Vector2(-210 , 7) , Vector2(-217 , 32) , Vector2(-225 , 58) , Vector2(-232 , 83) );
     
    	var triangle = new Array( Vector2(-278 , 0) , Vector2(-259 , -13) , Vector2(-241 , -30) , Vector2(-222 , -46) , Vector2(-203 , -62) , Vector2(-184 , -78) , Vector2(-164 , -93) , Vector2(-144 , -108) , Vector2(-124 , -123) , Vector2(-102 , -137) , Vector2(-80 , -150) , Vector2(-58 , -163) , Vector2(-37 , -177) , Vector2(-16 , -191) , Vector2(6 , -205) , Vector2(29 , -217) , Vector2(52 , -229) , Vector2(74 , -242) , Vector2(93 , -257) , Vector2(103 , -251) , Vector2(107 , -229) , Vector2(113 , -207) , Vector2(118 , -185) , Vector2(121 , -163) , Vector2(125 , -140) , Vector2(130 , -118) , Vector2(134 , -96) , Vector2(138 , -74) , Vector2(142 , -52) , Vector2(146 , -29) , Vector2(150 , -7) , Vector2(154 , 15) , Vector2(159 , 37) , Vector2(163 , 59) , Vector2(167 , 82) , Vector2(170 , 104) , Vector2(174 , 126) , Vector2(178 , 148) , Vector2(180 , 171) , Vector2(183 , 193) , Vector2(187 , 215) , Vector2(190 , 237) , Vector2(171 , 243) , Vector2(146 , 236) , Vector2(121 , 226) , Vector2(97 , 215) , Vector2(73 , 205) , Vector2(51 , 192) , Vector2(28 , 180) , Vector2(5 , 168) , Vector2(-18 , 156) , Vector2(-40 , 143) , Vector2(-61 , 129) , Vector2(-82 , 114) , Vector2(-102 , 99) , Vector2(-125 , 87) , Vector2(-149 , 76) , Vector2(-173 , 66) , Vector2(-197 , 55) , Vector2(-219 , 43) , Vector2(-242 , 31) , Vector2(-265 , 18) , Vector2(-286 , 5) , Vector2(-310 , -2)  );
     
    	var cross = new Array( Vector2(-265 , 0) , Vector2(-244 , 8) , Vector2(-222 , 12) , Vector2(-199 , 14) , Vector2(-176 , 16) , Vector2(-153 , 17) , Vector2(-130 , 18) , Vector2(-107 , 18) , Vector2(-84 , 17) , Vector2(-61 , 16) , Vector2(-38 , 17) , Vector2(-16 , 17) , Vector2(7 , 17) , Vector2(30 , 18) , Vector2(53 , 19) , Vector2(76 , 19) , Vector2(99 , 20) , Vector2(122 , 22) , Vector2(145 , 24) , Vector2(167 , 27) , Vector2(190 , 28) , Vector2(213 , 23) , Vector2(235 , 18) , Vector2(233 , 3) , Vector2(220 , -15) , Vector2(208 , -32) , Vector2(196 , -50) , Vector2(183 , -68) , Vector2(171 , -85) , Vector2(158 , -103) , Vector2(147 , -121) , Vector2(134 , -138) , Vector2(121 , -156) , Vector2(107 , -173) , Vector2(94 , -190) , Vector2(80 , -206) , Vector2(63 , -220) , Vector2(59 , -204) , Vector2(52 , -184) , Vector2(43 , -165) , Vector2(34 , -146) , Vector2(22 , -128) , Vector2(10 , -110) , Vector2(-1 , -91) , Vector2(-11 , -73) , Vector2(-21 , -54) , Vector2(-30 , -35) , Vector2(-39 , -15) , Vector2(-49 , 4) , Vector2(-58 , 23) , Vector2(-66 , 43) , Vector2(-73 , 63) , Vector2(-81 , 82) , Vector2(-87 , 103) , Vector2(-94 , 122) , Vector2(-103 , 142) , Vector2(-112 , 161) , Vector2(-124 , 179) , Vector2(-137 , 196) , Vector2(-151 , 213) , Vector2(-164 , 230) , Vector2(-178 , 247) , Vector2(-192 , 263) , Vector2(-206 , 280)  );
     
    	var a = new Array(  Vector2(-341 , 0) , Vector2(-323 , 17) , Vector2(-300 , 29) , Vector2(-276 , 39) , Vector2(-251 , 48) , Vector2(-227 , 58) , Vector2(-202 , 67) , Vector2(-178 , 76) , Vector2(-154 , 86) , Vector2(-129 , 96) , Vector2(-106 , 107) , Vector2(-82 , 118) , Vector2(-59 , 130) , Vector2(-35 , 141) , Vector2(-11 , 151) , Vector2(13 , 162) , Vector2(37 , 171) , Vector2(62 , 180) , Vector2(86 , 189) , Vector2(111 , 199) , Vector2(135 , 207) , Vector2(159 , 210) , Vector2(151 , 186) , Vector2(144 , 162) , Vector2(138 , 138) , Vector2(132 , 113) , Vector2(125 , 89) , Vector2(118 , 65) , Vector2(111 , 41) , Vector2(104 , 17) , Vector2(98 , -8) , Vector2(92 , -32) , Vector2(84 , -56) , Vector2(74 , -79) , Vector2(63 , -102) , Vector2(52 , -125) , Vector2(44 , -149) , Vector2(37 , -173) , Vector2(33 , -198) , Vector2(27 , -222) , Vector2(18 , -245) , Vector2(6 , -268) , Vector2(-7 , -290) , Vector2(-9 , -289) , Vector2(-2 , -265) , Vector2(5 , -241) , Vector2(15 , -218) , Vector2(23 , -194) , Vector2(30 , -169) , Vector2(37 , -145) , Vector2(45 , -121) , Vector2(53 , -98) , Vector2(61 , -74) , Vector2(68 , -49) , Vector2(74 , -25) , Vector2(78 , 0) , Vector2(71 , 21) , Vector2(49 , 35) , Vector2(27 , 47) , Vector2(4 , 60) , Vector2(-18 , 74) , Vector2(-40 , 87) , Vector2(-62 , 101) , Vector2(-84 , 115)  );
     
    	var z = new Array( Vector2(-252 , 0) , Vector2(-235 , 13) , Vector2(-219 , 27) , Vector2(-203 , 41) , Vector2(-187 , 56) , Vector2(-171 , 70) , Vector2(-155 , 84) , Vector2(-139 , 98) , Vector2(-122 , 112) , Vector2(-106 , 126) , Vector2(-90 , 140) , Vector2(-75 , 155) , Vector2(-59 , 169) , Vector2(-43 , 183) , Vector2(-27 , 197) , Vector2(-10 , 211) , Vector2(9 , 222) , Vector2(27 , 233) , Vector2(43 , 242) , Vector2(44 , 221) , Vector2(43 , 200) , Vector2(42 , 180) , Vector2(39 , 159) , Vector2(35 , 138) , Vector2(30 , 118) , Vector2(25 , 97) , Vector2(21 , 77) , Vector2(17 , 56) , Vector2(14 , 36) , Vector2(10 , 15) , Vector2(6 , -5) , Vector2(2 , -26) , Vector2(-1 , -47) , Vector2(-5 , -67) , Vector2(-8 , -88) , Vector2(-11 , -109) , Vector2(-17 , -129) , Vector2(-21 , -149) , Vector2(-27 , -170) , Vector2(-33 , -190) , Vector2(-39 , -210) , Vector2(-45 , -230) , Vector2(-48 , -251) , Vector2(-42 , -258) , Vector2(-31 , -240) , Vector2(-21 , -222) , Vector2(-10 , -203) , Vector2(1 , -185) , Vector2(14 , -168) , Vector2(27 , -152) , Vector2(41 , -136) , Vector2(56 , -120) , Vector2(71 , -105) , Vector2(86 , -90) , Vector2(102 , -76) , Vector2(118 , -61) , Vector2(133 , -47) , Vector2(149 , -32) , Vector2(166 , -19) , Vector2(183 , -5) , Vector2(200 , 8) , Vector2(216 , 21) , Vector2(232 , 35) , Vector2(248 , 49)  );
     
    	var line = new Array( Vector2(-250 , 0) , Vector2(-242 , 6) , Vector2(-234 , 66) , Vector2(-227 , 117) , Vector2(-219 , 200) , Vector2(-211 , 184) , Vector2(-203 , 203) , Vector2(-195 , 218) , Vector2(-187 , 202) , Vector2(-179 , 186) , Vector2(-171 , 170) , Vector2(-163 , 154) , Vector2(-155 , 138) , Vector2(-147 , 122) , Vector2(-139 , 106) , Vector2(-131 , 90) , Vector2(-123 , 74) , Vector2(-115 , 57) , Vector2(-107 , 41) , Vector2(-99 , 25) , Vector2(-91 , 9) , Vector2(-84 , -7) , Vector2(-76 , -20) , Vector2(-68 , -9) , Vector2(-60 , 1) , Vector2(-52 , 10) , Vector2(-44 , 18) , Vector2(-36 , 26) , Vector2(-28 , 13) , Vector2(-20 , -3) , Vector2(-12 , -16) , Vector2(-4 , -13) , Vector2(4 , -10) , Vector2(12 , -7) , Vector2(20 , -1) , Vector2(28 , 7) , Vector2(36 , 15) , Vector2(44 , 16) , Vector2(52 , 18) , Vector2(60 , 19) , Vector2(68 , 18) , Vector2(76 , 2) , Vector2(84 , -14) , Vector2(91 , -30) , Vector2(99 , -40) , Vector2(107 , -31) , Vector2(115 , -21) , Vector2(123 , -28) , Vector2(131 , -44) , Vector2(139 , -60) , Vector2(147 , -76) , Vector2(155 , -92) , Vector2(163 , -108) , Vector2(171 , -124) , Vector2(179 , -140) , Vector2(187 , -156) , Vector2(195 , -172) , Vector2(203 , -175) , Vector2(211 , -142) , Vector2(219 , -154) , Vector2(227 , -170) , Vector2(235 , -186) , Vector2(243 , -202) , Vector2(250 , -282)  );
     
    	var wave = new Array( Vector2(-245 , 0) , Vector2(-241 , 4) , Vector2(-234 , 37) , Vector2(-227 , 75) , Vector2(-221 , 113) , Vector2(-215 , 152) , Vector2(-209 , 190) , Vector2(-201 , 225) , Vector2(-192 , 243) , Vector2(-182 , 251) , Vector2(-172 , 237) , Vector2(-165 , 205) , Vector2(-157 , 170) , Vector2(-150 , 138) , Vector2(-142 , 108) , Vector2(-133 , 79) , Vector2(-125 , 52) , Vector2(-117 , 19) , Vector2(-110 , -15) , Vector2(-102 , -45) , Vector2(-94 , -77) , Vector2(-86 , -106) , Vector2(-77 , -133) , Vector2(-68 , -156) , Vector2(-58 , -168) , Vector2(-48 , -172) , Vector2(-38 , -165) , Vector2(-28 , -147) , Vector2(-20 , -123) , Vector2(-12 , -93) , Vector2(-4 , -59) , Vector2(3 , -23) , Vector2(9 , 15) , Vector2(14 , 57) , Vector2(19 , 98) , Vector2(25 , 138) , Vector2(32 , 171) , Vector2(42 , 193) , Vector2(52 , 200) , Vector2(62 , 191) , Vector2(70 , 161) , Vector2(76 , 124) , Vector2(85 , 95) , Vector2(92 , 63) , Vector2(100 , 33) , Vector2(108 , 3) , Vector2(116 , -29) , Vector2(125 , -58) , Vector2(132 , -91) , Vector2(138 , -129) , Vector2(144 , -170) , Vector2(150 , -208) , Vector2(157 , -241) , Vector2(167 , -249) , Vector2(177 , -239) , Vector2(186 , -216) , Vector2(195 , -187) , Vector2(203 , -157) , Vector2(211 , -129) , Vector2(219 , -100) , Vector2(228 , -72) , Vector2(237 , -47) , Vector2(246 , -29) , Vector2(255 , -7)  );
     
     
    	// 2nd iteration of templates 	--------------------------------------------------------------------------------------------------------
     
     
    	var square2 = new Array(  Vector2(-244 , 0) , Vector2(-231 , 19) , Vector2(-218 , 38) , Vector2(-207 , 58) , Vector2(-194 , 77) , Vector2(-181 , 95) , Vector2(-168 , 114) , Vector2(-155 , 133) , Vector2(-141 , 152) , Vector2(-129 , 171) , Vector2(-116 , 190) , Vector2(-103 , 209) , Vector2(-90 , 228) , Vector2(-77 , 246) , Vector2(-62 , 251) , Vector2(-43 , 238) , Vector2(-26 , 224) , Vector2(-8 , 210) , Vector2(10 , 197) , Vector2(28 , 183) , Vector2(46 , 169) , Vector2(63 , 154) , Vector2(80 , 140) , Vector2(98 , 126) , Vector2(115 , 112) , Vector2(133 , 98) , Vector2(151 , 84) , Vector2(169 , 70) , Vector2(186 , 56) , Vector2(204 , 42) , Vector2(221 , 28) , Vector2(238 , 13) , Vector2(253 , -4) , Vector2(241 , -23) , Vector2(228 , -41) , Vector2(214 , -60) , Vector2(201 , -78) , Vector2(187 , -96) , Vector2(173 , -114) , Vector2(159 , -133) , Vector2(146 , -151) , Vector2(133 , -170) , Vector2(119 , -188) , Vector2(105 , -206) , Vector2(92 , -225) , Vector2(77 , -242) , Vector2(59 , -249) , Vector2(41 , -236) , Vector2(23 , -222) , Vector2(5 , -208) , Vector2(-13 , -194) , Vector2(-30 , -180) , Vector2(-48 , -167) , Vector2(-66 , -153) , Vector2(-84 , -139) , Vector2(-102 , -125) , Vector2(-119 , -111) , Vector2(-137 , -98) , Vector2(-155 , -84) , Vector2(-173 , -70) , Vector2(-191 , -57) , Vector2(-210 , -45) , Vector2(-229 , -34) , Vector2(-247 , -22)  );	
     
    	var circle2 = new Array(  Vector2(-179 , 0) , Vector2(-188 , 50) , Vector2(-187 , 102) , Vector2(-179 , 153) , Vector2(-161 , 200) , Vector2(-126 , 228) , Vector2(-83 , 233) , Vector2(-41 , 224) , Vector2(0 , 209) , Vector2(41 , 191) , Vector2(80 , 168) , Vector2(117 , 143) , Vector2(152 , 112) , Vector2(187 , 81) , Vector2(219 , 47) , Vector2(245 , 6) , Vector2(268 , -39) , Vector2(280 , -88) , Vector2(275 , -139) , Vector2(260 , -188) , Vector2(238 , -232) , Vector2(202 , -259) , Vector2(159 , -267) , Vector2(116 , -267) , Vector2(73 , -262) , Vector2(32 , -246) , Vector2(-9 , -228) , Vector2(-46 , -203) , Vector2(-82 , -174) , Vector2(-117 , -144) , Vector2(-151 , -111) , Vector2(-182 , -75) , Vector2(-203 , -30) , Vector2(-181 , -41) , Vector2(-148 , -74) , Vector2(-128 , -96) , Vector2(-165 , -69) , Vector2(-199 , -38) , Vector2(-220 , 7) , Vector2(-220 , 59) , Vector2(-210 , 109) , Vector2(-195 , 158) , Vector2(-165 , 194) , Vector2(-125 , 213) , Vector2(-83 , 224) , Vector2(-40 , 226) , Vector2(3 , 224) , Vector2(46 , 217) , Vector2(87 , 202) , Vector2(126 , 181) , Vector2(162 , 151) , Vector2(190 , 113) , Vector2(197 , 62) , Vector2(186 , 12) , Vector2(167 , -34) , Vector2(141 , -76) , Vector2(111 , -113) , Vector2(73 , -137) , Vector2(31 , -150) , Vector2(-11 , -161) , Vector2(-54 , -161) , Vector2(-95 , -155) , Vector2(-134 , -133) , Vector2(-160 , -106)  );
     
    	var triangle2 = new Array(  Vector2(-244 , 0) , Vector2(-177 , 23) , Vector2(-110 , 44) , Vector2(-43 , 66) , Vector2(24 , 87) , Vector2(91 , 108) , Vector2(159 , 128) , Vector2(226 , 149) , Vector2(224 , 108) , Vector2(202 , 42) , Vector2(182 , -24) , Vector2(164 , -92) , Vector2(155 , -161) , Vector2(140 , -229) , Vector2(116 , -264) , Vector2(63 , -218) , Vector2(5 , -179) , Vector2(-53 , -139) , Vector2(-109 , -97) , Vector2(-166 , -56) , Vector2(-228 , -23) , Vector2(-256 , 34) , Vector2(-254 , 90) , Vector2(-188 , 115) , Vector2(-122 , 138) , Vector2(-55 , 160) , Vector2(11 , 184) , Vector2(77 , 210) , Vector2(144 , 233) , Vector2(161 , 201) , Vector2(140 , 135) , Vector2(123 , 67) , Vector2(107 , -1) , Vector2(90 , -68) , Vector2(73 , -136) , Vector2(57 , -203) , Vector2(15 , -163) , Vector2(-30 , -110) , Vector2(-76 , -57) , Vector2(-127 , -9) , Vector2(-185 , 30) , Vector2(-251 , 47) , Vector2(-274 , 57) , Vector2(-207 , 79) , Vector2(-140 , 100) , Vector2(-73 , 122) , Vector2(-7 , 146) , Vector2(59 , 172) , Vector2(125 , 196) , Vector2(186 , 207) , Vector2(185 , 138) , Vector2(172 , 69) , Vector2(153 , 2) , Vector2(135 , -65) , Vector2(117 , -132) , Vector2(109 , -201) , Vector2(89 , -267) , Vector2(44 , -264) , Vector2(-6 , -215) , Vector2(-48 , -159) , Vector2(-93 , -105) , Vector2(-143 , -56) , Vector2(-197 , -12) , Vector2(-261 , 14)  );	
     
    	var cross2 = new Array(  Vector2(-258 , 0) , Vector2(-236 , -7) , Vector2(-213 , -11) , Vector2(-190 , -13) , Vector2(-167 , -16) , Vector2(-144 , -18) , Vector2(-121 , -21) , Vector2(-98 , -24) , Vector2(-75 , -28) , Vector2(-53 , -33) , Vector2(-30 , -37) , Vector2(-7 , -41) , Vector2(16 , -45) , Vector2(39 , -49) , Vector2(62 , -53) , Vector2(84 , -57) , Vector2(107 , -61) , Vector2(130 , -63) , Vector2(154 , -64) , Vector2(177 , -66) , Vector2(200 , -70) , Vector2(222 , -74) , Vector2(242 , -71) , Vector2(228 , -53) , Vector2(214 , -36) , Vector2(201 , -17) , Vector2(186 , 0) , Vector2(172 , 18) , Vector2(158 , 35) , Vector2(144 , 53) , Vector2(130 , 70) , Vector2(116 , 88) , Vector2(102 , 106) , Vector2(88 , 124) , Vector2(75 , 142) , Vector2(61 , 159) , Vector2(48 , 178) , Vector2(36 , 197) , Vector2(25 , 216) , Vector2(10 , 233) , Vector2(-2 , 219) , Vector2(-9 , 198) , Vector2(-15 , 177) , Vector2(-21 , 156) , Vector2(-26 , 134) , Vector2(-33 , 113) , Vector2(-40 , 92) , Vector2(-46 , 71) , Vector2(-53 , 50) , Vector2(-61 , 29) , Vector2(-69 , 8) , Vector2(-77 , -13) , Vector2(-84 , -34) , Vector2(-90 , -55) , Vector2(-94 , -77) , Vector2(-98 , -99) , Vector2(-105 , -120) , Vector2(-110 , -141) , Vector2(-116 , -163) , Vector2(-122 , -184) , Vector2(-129 , -205) , Vector2(-136 , -227) , Vector2(-143 , -247) , Vector2(-153 , -267)  );
     
    	var a2 = new Array(  Vector2(-289 , 0) , Vector2(-262 , 7) , Vector2(-233 , -13) , Vector2(-205 , -33) , Vector2(-176 , -53) , Vector2(-147 , -73) , Vector2(-119 , -93) , Vector2(-90 , -113) , Vector2(-62 , -133) , Vector2(-33 , -154) , Vector2(-5 , -174) , Vector2(24 , -194) , Vector2(52 , -214) , Vector2(81 , -234) , Vector2(110 , -254) , Vector2(130 , -253) , Vector2(134 , -222) , Vector2(138 , -190) , Vector2(142 , -158) , Vector2(145 , -126) , Vector2(147 , -94) , Vector2(156 , -63) , Vector2(163 , -32) , Vector2(166 , 0) , Vector2(168 , 32) , Vector2(173 , 64) , Vector2(180 , 95) , Vector2(188 , 126) , Vector2(195 , 158) , Vector2(203 , 189) , Vector2(211 , 220) , Vector2(209 , 246) , Vector2(177 , 230) , Vector2(144 , 217) , Vector2(113 , 200) , Vector2(81 , 184) , Vector2(50 , 166) , Vector2(20 , 148) , Vector2(-8 , 128) , Vector2(-38 , 109) , Vector2(-65 , 88) , Vector2(-93 , 67) , Vector2(-123 , 48) , Vector2(-150 , 27) , Vector2(-178 , 6) , Vector2(-206 , -14) , Vector2(-233 , -35) , Vector2(-262 , -55) , Vector2(-280 , -61) , Vector2(-248 , -48) , Vector2(-214 , -35) , Vector2(-181 , -20) , Vector2(-150 , -3) , Vector2(-120 , 15) , Vector2(-92 , 35) , Vector2(-62 , 54) , Vector2(-35 , 76) , Vector2(-3 , 74) , Vector2(29 , 58) , Vector2(62 , 45) , Vector2(95 , 30) , Vector2(127 , 15) , Vector2(160 , 1) , Vector2(191 , -12)   );
     
    	var z2 = new Array(  Vector2(-238 , 0) , Vector2(-222 , 14) , Vector2(-206 , 27) , Vector2(-190 , 41) , Vector2(-174 , 54) , Vector2(-159 , 68) , Vector2(-143 , 81) , Vector2(-127 , 95) , Vector2(-111 , 109) , Vector2(-95 , 122) , Vector2(-79 , 136) , Vector2(-63 , 149) , Vector2(-47 , 163) , Vector2(-32 , 176) , Vector2(-16 , 190) , Vector2(0 , 203) , Vector2(16 , 217) , Vector2(32 , 231) , Vector2(48 , 244) , Vector2(58 , 241) , Vector2(52 , 219) , Vector2(44 , 198) , Vector2(36 , 176) , Vector2(30 , 154) , Vector2(22 , 132) , Vector2(16 , 110) , Vector2(9 , 88) , Vector2(3 , 66) , Vector2(-4 , 44) , Vector2(-10 , 22) , Vector2(-16 , -1) , Vector2(-23 , -23) , Vector2(-30 , -45) , Vector2(-37 , -67) , Vector2(-44 , -88) , Vector2(-51 , -111) , Vector2(-58 , -132) , Vector2(-65 , -154) , Vector2(-71 , -176) , Vector2(-72 , -200) , Vector2(-72 , -223) , Vector2(-70 , -246) , Vector2(-63 , -256) , Vector2(-47 , -242) , Vector2(-33 , -226) , Vector2(-18 , -211) , Vector2(-3 , -196) , Vector2(12 , -181) , Vector2(27 , -167) , Vector2(43 , -154) , Vector2(59 , -139) , Vector2(74 , -125) , Vector2(90 , -111) , Vector2(106 , -97) , Vector2(121 , -83) , Vector2(137 , -69) , Vector2(152 , -55) , Vector2(168 , -41) , Vector2(183 , -27) , Vector2(199 , -12) , Vector2(214 , 1) , Vector2(230 , 15) , Vector2(246 , 28) , Vector2(262 , 42)  );
     
    	var line2 = new Array(  Vector2(-250 , 0) , Vector2(-242 , -11) , Vector2(-234 , -23) , Vector2(-227 , -34) , Vector2(-219 , -46) , Vector2(-211 , -57) , Vector2(-203 , -69) , Vector2(-195 , -80) , Vector2(-187 , -91) , Vector2(-179 , -103) , Vector2(-171 , -114) , Vector2(-163 , -126) , Vector2(-155 , -137) , Vector2(-147 , -149) , Vector2(-139 , -160) , Vector2(-131 , -159) , Vector2(-123 , -154) , Vector2(-115 , -150) , Vector2(-107 , -145) , Vector2(-99 , -135) , Vector2(-91 , -123) , Vector2(-83 , -111) , Vector2(-75 , -85) , Vector2(-68 , -57) , Vector2(-60 , -29) , Vector2(-52 , -21) , Vector2(-44 , -32) , Vector2(-36 , -44) , Vector2(-28 , -55) , Vector2(-20 , -67) , Vector2(-12 , -78) , Vector2(-4 , -90) , Vector2(4 , -101) , Vector2(12 , -112) , Vector2(20 , -121) , Vector2(28 , -115) , Vector2(36 , -109) , Vector2(44 , -102) , Vector2(52 , -85) , Vector2(60 , -52) , Vector2(68 , -20) , Vector2(76 , 12) , Vector2(83 , 44) , Vector2(91 , 60) , Vector2(99 , 74) , Vector2(107 , 90) , Vector2(115 , 112) , Vector2(123 , 135) , Vector2(131 , 157) , Vector2(139 , 168) , Vector2(147 , 157) , Vector2(155 , 156) , Vector2(163 , 170) , Vector2(171 , 184) , Vector2(179 , 177) , Vector2(187 , 166) , Vector2(195 , 154) , Vector2(203 , 143) , Vector2(211 , 147) , Vector2(219 , 178) , Vector2(227 , 208) , Vector2(234 , 240) , Vector2(242 , 284) , Vector2(250 , 340)  );
     
    	var wave2 = new Array(  Vector2(-262 , 0) , Vector2(-256 , 27) , Vector2(-249 , 58) , Vector2(-241 , 81) , Vector2(-233 , 102) , Vector2(-224 , 119) , Vector2(-215 , 136) , Vector2(-207 , 152) , Vector2(-198 , 168) , Vector2(-189 , 180) , Vector2(-179 , 191) , Vector2(-170 , 200) , Vector2(-161 , 207) , Vector2(-151 , 209) , Vector2(-142 , 207) , Vector2(-132 , 201) , Vector2(-123 , 195) , Vector2(-113 , 183) , Vector2(-105 , 164) , Vector2(-97 , 143) , Vector2(-88 , 120) , Vector2(-81 , 93) , Vector2(-74 , 66) , Vector2(-66 , 39) , Vector2(-59 , 10) , Vector2(-52 , -20) , Vector2(-45 , -46) , Vector2(-38 , -75) , Vector2(-30 , -104) , Vector2(-23 , -133) , Vector2(-17 , -163) , Vector2(-10 , -192) , Vector2(-1 , -209) , Vector2(8 , -215) , Vector2(18 , -206) , Vector2(27 , -190) , Vector2(35 , -171) , Vector2(44 , -151) , Vector2(52 , -132) , Vector2(61 , -113) , Vector2(69 , -94) , Vector2(78 , -73) , Vector2(86 , -53) , Vector2(95 , -33) , Vector2(103 , -15) , Vector2(112 , 2) , Vector2(121 , 17) , Vector2(130 , 31) , Vector2(139 , 43) , Vector2(149 , 48) , Vector2(158 , 51) , Vector2(168 , 52) , Vector2(177 , 46) , Vector2(187 , 40) , Vector2(196 , 27) , Vector2(204 , 8) , Vector2(211 , -22) , Vector2(216 , -58) , Vector2(221 , -95) , Vector2(226 , -132) , Vector2(230 , -172) , Vector2(233 , -211) , Vector2(236 , -251) , Vector2(238 , -291)  );
     
     
     
    // add all templates
    Templates = new Array (square, circle, triangle, cross, a, z, line, wave, square2, circle2, triangle2, cross2, a2, z2, line2, wave2 );
    TemplateNames = new Array ("square", "circle", "triangle", "cross", "a", "z", "line", "wave", "square2", "circle2", "triangle2", "cross2", "a2", "z2", "line2", "wave2" );
    Thats all... Hope some will benefit from this 
    Mads Møller 
    viezel studios 
}
