// Original url: http://wiki.unity3d.com/index.php/ScreenWipes
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/ScreenWipes.cs
// File based on original modification date of: 31 October 2013, at 05:02. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{

Author: Eric Haines (Eric5h5) 
Contents [hide] 
1 Description 
2 Usage 
3 History 
4 JavaScript - ScreenWipe.js 

DescriptionMakes various animated screen wipes: cross-fade (two versions, for Unity and Unity Pro), rectangular zoom (in or out), arbitrary shape zoom and spin, a squish scroll (four directions), and a wavy "dream" wipe (requires Unity Pro). Requires Unity 2.6 or later. 
UsageSee CrossFade, CrossFadePro, DreamWipe, RectangleWipe, ShapeWipe, and SquishWipe for usage directions. This is the base script that contains the functions for all the wipes. It must be attached to some object in the scene in order for it to work, and should be located in your Standard Assets folder (if you don't have one, make one), so it can be used easily by C# and Boo scripts. It has a static variable called "use" that points to this script, so getting a reference isn't necessary. For example, CrossFade would be called like this: 
yield ScreenWipe.use.CrossFade (camera1, camera2, fadeTime);or 
yield ScreenWipe.use.CrossFade (camera1, camera2, fadeTime, curve);You can call the functions without "yield", in which case they will return immediately. They'll still run the specified length of time, but you won't know when they're done, and should be careful about calling them multiple times simultaneously with the same cameras, because strange behavior will of course result. 
See the file below for a UnityPackage that contains example scenes of usage, so you can see how they work in practice. Import the UnityPackage into an empty project for best results. 
ScreenWipes.zip‎ (211KB, MIME TYPE: application/zip) Eric's one. 
ScreenWipes2.zip‎ (211KB, MIME TYPE: application/zip) Berenger's one. (16-02-12) 
History16 Feb 2012 Possibility to use animation curves to control the wipes velocity. Function have been overloaded, so everything done before that update will work, with the exact same result. Pro parts not done, I don't have Unity pro. - Berenger 
JavaScript - ScreenWipe.jsprivate var tex : Texture;
private var renderTex : RenderTexture;
private var tex2D : Texture2D;
private var alpha : float;
private var reEnableListener : boolean;
private var shapeMaterial : Material;
private var shape : Transform;
private var curve : AnimationCurve;
private var useCurve : boolean;
static var use : ScreenWipe;
enum ZoomType {Grow, Shrink}
enum TransitionType {Left, Right, Up, Down}
 
function Awake () 
{
	if (use) {
		Debug.LogWarning("Only one instance of ScreenCrossFadePro is allowed");
		return;
	}
	use = this;
 
	this.enabled = false;
}
 
function OnGUI () {
	GUI.depth = -9999999;
	GUI.color.a = alpha;
	GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), tex);
}
 
function AlphaTimer (time : float) {
	var rate = 1.0/time;
 
	if( useCurve ){
		var t : float = 0.0;
		while( t < 1.0 ){
			alpha = curve.Evaluate( t );
			t += Time.deltaTime * rate;
			yield;
		}
	}
	else{
		for (alpha = 1.0; alpha > 0.0; alpha -= Time.deltaTime * rate)
			yield;
	}
 
}
 
function CameraSetup (cam1 : Camera, cam2 : Camera, cam1Active : boolean, enableThis : boolean) {
	if (enableThis) {
		this.enabled = true;
	}
	cam1.gameObject.active = cam1Active;
	cam2.gameObject.active = true;
	var listener = cam2.GetComponent(AudioListener);
	if (listener) {
		reEnableListener = listener.enabled? true : false;
		listener.enabled = false;
	}
}
 
function CameraCleanup (cam1 : Camera, cam2 : Camera) {
	var listener = cam2.GetComponent(AudioListener);
	if (listener && reEnableListener) {
		listener.enabled = true;
	}
	cam1.gameObject.active = false;
	this.enabled = false;
}
 
function CrossFadePro (cam1 : Camera, cam2 : Camera, time : float) {
	if (!renderTex) {
		renderTex = new RenderTexture(Screen.width, Screen.height, 24);
	}
	cam1.targetTexture = renderTex;
	tex = renderTex;
	CameraSetup (cam1, cam2, true, true);
 
	yield AlphaTimer(time);
 
	cam1.targetTexture = null;
	renderTex.Release();
	CameraCleanup (cam1, cam2);
}
 
function CrossFade (cam1 : Camera, cam2 : Camera, time : float ) {
	yield CrossFade( cam1, cam2, time, null );
}
function CrossFade (cam1 : Camera, cam2 : Camera, time : float, _curve : AnimationCurve ) {
	curve = _curve;
	useCurve = curve != null;
	if (!tex2D) {
		tex2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
	}
	tex2D.ReadPixels(Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
	tex2D.Apply();
	tex = tex2D;
	yield;
	CameraSetup (cam1, cam2, false, true);
 
	yield AlphaTimer(time);
 
	CameraCleanup (cam1, cam2);
}
 
function RectWipe (cam1 : Camera, cam2 : Camera, time : float, zoom : ZoomType) {
	yield RectWipe( cam1, cam2, time, zoom, null );
}
function RectWipe (cam1 : Camera, cam2 : Camera, time : float, zoom : ZoomType, _curve : AnimationCurve) {
	curve = _curve;
	useCurve = curve != null;
	CameraSetup (cam1, cam2, true, false);
	var useCam = (zoom == ZoomType.Shrink)? cam1 : cam2;
	var otherCam = (zoom == ZoomType.Shrink)? cam2 : cam1;
	var originalRect = useCam.rect;
	var originalDepth = useCam.depth;
	useCam.depth = otherCam.depth+1;
 
	if( useCurve )
	{
		var rate = 1.0/(time);
		if (zoom == ZoomType.Shrink) {
			for (i = 0.0; i < 1.0; i += Time.deltaTime * rate) {
				var t = curve.Evaluate( i )*0.5;
				cam1.rect = Rect(t, t, 1.0-t*2, 1.0-t*2);
				yield;
			}
		}
		else {
			for (i = 0.0; i < 1.0; i += Time.deltaTime * rate) {
				t = curve.Evaluate( i )*0.5;
				cam2.rect = Rect(.5-t, .5-t, t*2, t*2);
				yield;
			}
		}
	}
	else{
		rate = 1.0/(time*2);
		if (zoom == ZoomType.Shrink) {
			for (i = 0.0; i < .5; i += Time.deltaTime * rate) {
				t = Mathf.Lerp(0.0, .5, Mathf.Sin(i * Mathf.PI));	// Slow down near the end
				cam1.rect = Rect(t, t, 1.0-t*2, 1.0-t*2);
				yield;
			}
		}
		else {
			for (i = 0.0; i < .5; i += Time.deltaTime * rate) {
				t = Mathf.Lerp(.5, 0.0, Mathf.Sin((.5-i) * Mathf.PI));	// Start out slower
				cam2.rect = Rect(.5-t, .5-t, t*2, t*2);
				yield;
			}
		}
	}
 
	useCam.rect = originalRect;
	useCam.depth = originalDepth;
	CameraCleanup (cam1, cam2);
}
 
function ShapeWipe (cam1 : Camera, cam2 : Camera, time : float, zoom : ZoomType, mesh : Mesh, rotateAmount : float) {
	yield ShapeWipe( cam1, cam2, time, zoom, mesh, rotateAmount, null );
}
function ShapeWipe (cam1 : Camera, cam2 : Camera, time : float, zoom : ZoomType, mesh : Mesh, rotateAmount : float, _curve : AnimationCurve) {
	curve = _curve;
	useCurve = curve != null;
	if (!shapeMaterial) {
		shapeMaterial = new Material (
			"Shader \"DepthMask\" {" +
			"   SubShader {" +
			"	   Tags { \"Queue\" = \"Background\" }" +
			"	   Lighting Off ZTest LEqual ZWrite On Cull Off ColorMask 0" +
			"	   Pass {}" +
			"   }" +
			"}"
		);
	}
	if (!shape) {
		shape = new GameObject("Shape", MeshFilter, MeshRenderer).transform;
		shape.renderer.material = shapeMaterial;
	}
 
	CameraSetup (cam1, cam2, true, false);
	var useCam = (zoom == ZoomType.Shrink)? cam1 : cam2;
	var otherCam = (zoom == ZoomType.Shrink)? cam2 : cam1;
	var originalDepth = otherCam.depth;
	var originalClearFlags = otherCam.clearFlags;
	otherCam.depth = useCam.depth+1;
	otherCam.clearFlags = CameraClearFlags.Depth;
 
	shape.gameObject.active = true;
	(shape.GetComponent(MeshFilter) as MeshFilter).mesh = mesh;
	shape.position = otherCam.transform.position + otherCam.transform.forward * (otherCam.nearClipPlane+.01);
	shape.parent = otherCam.transform;
	shape.localRotation = Quaternion.identity;
 
	if( useCurve ){
		var rate = 1.0/time;
		if (zoom == ZoomType.Shrink) {
			for (i = 1.0; i > 0.0; i -= Time.deltaTime * rate) {
				var t = curve.Evaluate( i );
				shape.localScale = Vector3(t, t, t);
				shape.localEulerAngles = Vector3(0.0, 0.0, i * rotateAmount);
				yield;
			}
		}
		else {
			for (i = 0.0; i < 1.0; i += Time.deltaTime * rate) {
				t = curve.Evaluate( i );
				shape.localScale = Vector3(t, t, t);
				shape.localEulerAngles = Vector3(0.0, 0.0, -i * rotateAmount);
				yield;
			}   
		}
	}
	else{
		rate = 1.0/time;
		if (zoom == ZoomType.Shrink) {
			for (i = 1.0; i > 0.0; i -= Time.deltaTime * rate) {
				t = Mathf.Lerp(1.0, 0.0, Mathf.Sin((1.0-i) * Mathf.PI * 0.5));	// Slow down near the end
				shape.localScale = Vector3(t, t, t);
				shape.localEulerAngles = Vector3(0.0, 0.0, i * rotateAmount);
				yield;
			}
		}
		else {
			for (i = 0.0; i < 1.0; i += Time.deltaTime * rate) {
				t = Mathf.Lerp(1.0, 0.0, Mathf.Sin((1.0-i) * Mathf.PI * 0.5));		// Start out slower
				shape.localScale = Vector3(t, t, t);
				shape.localEulerAngles = Vector3(0.0, 0.0, -i * rotateAmount);
				yield;
			}   
		}
	}
 
	otherCam.clearFlags = originalClearFlags;
	otherCam.depth = originalDepth;
	CameraCleanup (cam1, cam2);
	shape.parent = null;
	shape.gameObject.active = false;
}
 
function SquishWipe (cam1 : Camera, cam2 : Camera, time : float, transitionType : TransitionType) {
	yield SquishWipe( cam1, cam2, time, transitionType, null );
}
function SquishWipe (cam1 : Camera, cam2 : Camera, time : float, transitionType : TransitionType, _curve : AnimationCurve) {
	curve = _curve;
	useCurve = curve != null;
	var originalCam1Rect = cam1.rect;
	var originalCam2Rect = cam2.rect;
	var cam1Matrix = cam1.projectionMatrix;
	var cam2Matrix = cam2.projectionMatrix;
	CameraSetup (cam1, cam2, true, false);
 
	var rate = 1.0/time;
	var t = 0.0;
	var i = 0.0;
	while( i < 1.0 ){
		if( useCurve ){ 
			i = curve.Evaluate(t);
			t += Time.deltaTime * rate;
		} 
		else
			i += Time.deltaTime * rate;
 
		switch (transitionType) {
			case TransitionType.Right:
				cam1.rect = Rect(i, 0, 1.0, 1.0);
				cam2.rect = Rect(0, 0, i, 1.0);
				break;
			case TransitionType.Left:
				cam1.rect = Rect(0, 0, 1.0-i, 1.0);
				cam2.rect = Rect(1.0-i, 0, 1.0, 1.0);
				break;
			case TransitionType.Up:
				cam1.rect = Rect(0, i, 1.0, 1.0);
				cam2.rect = Rect(0, 0, 1.0, i);
				break;
			case TransitionType.Down:
				cam1.rect = Rect(0, 0, 1.0, 1.0-i);
				cam2.rect = Rect(0, 1.0-i, 1.0, 1.0);
				break;
		}
		cam1.projectionMatrix = cam1Matrix;
		cam2.projectionMatrix = cam2Matrix;
		yield;
	}
 
	cam1.rect = originalCam1Rect;
	cam2.rect = originalCam2Rect;
	CameraCleanup (cam1, cam2);
}
 
var planeResolution = 90;	// Higher numbers make the DreamWipe effect smoother, but take more CPU time
private var baseVertices : Vector3[];
private var newVertices : Vector3[];
private var planeMaterial : Material;
private var plane : GameObject;
private var renderTex2 : RenderTexture;
 
function InitializeDreamWipe () {
	if (planeMaterial && plane) return;
 
	planeMaterial = new Material (
		"Shader \"Unlit2Pass\" {" +
		"Properties {" +
		"	_Color (\"Main Color\", Color) = (1,1,1,1)" +
		"	_Tex1 (\"Base\", Rect) = \"white\" {}" +
		"	_Tex2 (\"Base\", Rect) = \"white\" {}" +
		"}" +
		"Category {" +
		"	ZWrite Off Alphatest Greater 0 ColorMask RGB Lighting Off" +
		"	Tags {\"Queue\"=\"Transparent\" \"IgnoreProjector\"=\"True\" \"RenderType\"=\"Transparent\"}" +
		"	Blend SrcAlpha OneMinusSrcAlpha" +
		"	SubShader {" +
		"		Pass {SetTexture [_Tex2]}" +
		"		Pass {SetTexture [_Tex1] {constantColor [_Color] Combine texture * constant, texture * constant}}" +
		"	}" +
		"}}"
	);
 
	// Set up plane object
	plane = new GameObject("Plane", MeshFilter, MeshRenderer);
	plane.renderer.material = planeMaterial;
	plane.renderer.castShadows = false;
	plane.renderer.receiveShadows = false;
	plane.renderer.enabled = false;
 
	// Create the mesh used for the distortion effect
	var planeMesh = new Mesh();
	(plane.GetComponent(MeshFilter) as MeshFilter).mesh = planeMesh;
 
	planeResolution = Mathf.Clamp(planeResolution, 1, 16380);
	baseVertices = new Vector3[4*planeResolution + 4];
	newVertices = new Vector3[baseVertices.Length];
	var newUV = new Vector2[baseVertices.Length];
	var newTriangles = new int[18*planeResolution];
 
	var idx = 0;
	for (i = 0; i <= planeResolution; i++) {
		var add : float = 1.0/planeResolution*i;
		newUV[idx] = Vector2(0.0, 1.0-add);
		baseVertices[idx++] = Vector3(-1.0, .5-add, 0.0);
		newUV[idx] = Vector2(0.0, 1.0-add);
		baseVertices[idx++] = Vector3(-.5, .5-add, 0.0);
		newUV[idx] = Vector2(1.0, 1.0-add);
		baseVertices[idx++] = Vector3(.5, .5-add, 0.0);
		newUV[idx] = Vector2(1.0, 1.0-add);
		baseVertices[idx++] = Vector3(1.0, .5-add, 0.0);
	}
 
	idx = 0;
	for (y = 0; y < planeResolution; y++) {
		for (x = 0; x < 3; x++) {
			newTriangles[idx++] = (y*4	  )+x;
			newTriangles[idx++] = (y*4	  )+x+1;
			newTriangles[idx++] = ((y+1)*4)+x;
 
			newTriangles[idx++] = ((y+1)*4)+x;
			newTriangles[idx++] = (y	*4)+x+1;
			newTriangles[idx++] = ((y+1)*4)+x+1;
		}
	}
 
	planeMesh.vertices = baseVertices;
	planeMesh.uv = newUV;
	planeMesh.triangles = newTriangles;
 
	// Set up rendertextures
	renderTex = new RenderTexture(Screen.width, Screen.height, 24);
	renderTex2 = new RenderTexture(Screen.width, Screen.height, 24);
	renderTex.filterMode = renderTex2.filterMode = FilterMode.Point;
	planeMaterial.SetTexture("_Tex1", renderTex);
	planeMaterial.SetTexture("_Tex2", renderTex2);
}
 
function DreamWipe (cam1 : Camera, cam2 : Camera, time : float) {
	yield DreamWipe (cam1, cam2, time, .07, 25.0);
}
 
function DreamWipe (cam1 : Camera, cam2 : Camera, time : float, waveScale : float, waveFrequency : float) {
	if (!plane) {
		InitializeDreamWipe();
	}
	waveScale = Mathf.Clamp(waveScale, -.5, .5);
	waveFrequency = .25/(planeResolution/waveFrequency);
 
	CameraSetup (cam1, cam2, true, false);
 
	// Make a camera that will show a plane with the combined rendertextures from cam1 and cam2,
	// and make it have the highest depth so it's always on top
	var cam2Clone : Camera = Instantiate(cam2, cam2.transform.position, cam2.transform.rotation);
	cam2Clone.depth = cam1.depth+1;
	if (cam2Clone.depth <= cam2.depth) {
		cam2Clone.depth = cam2.depth+1;
	}
	// Get screen coodinates of 0,0 in local spatial coordinates, so we know how big to scale the plane (make sure clip planes are reasonable)
	cam2Clone.nearClipPlane = .5;
	cam2Clone.farClipPlane = 1.0;
	var p = cam2Clone.transform.InverseTransformPoint(cam2.ScreenToWorldPoint(Vector3(0.0, 0.0, cam2Clone.nearClipPlane)));
	plane.transform.localScale = Vector3(-p.x*2.0, -p.y*2.0, 1.0);
	plane.transform.parent = cam2Clone.transform;
	plane.transform.localPosition = plane.transform.localEulerAngles = Vector3.zero;
	// Must be a tiny bit beyond the nearClipPlane, or it might not show up
	plane.transform.Translate(Vector3.forward * (cam2Clone.nearClipPlane+.00005));
	// Move the camera back so cam2 won't see the renderPlane, and parent it to cam2 so that if cam2 is moving, it won't see the plane
	cam2Clone.transform.Translate(Vector3.forward * -1.0);
	cam2Clone.transform.parent = cam2.transform;
 
	// Initialize some stuff
	plane.renderer.enabled = true;
	var scale = 0.0;
	var planeMesh = plane.GetComponent(MeshFilter).mesh;
	cam1.targetTexture = renderTex;
	cam2.targetTexture = renderTex2;
 
	// Do the cross-fade
	var rate = 1.0/time;
	for (i = 0.0; i < 1.0; i += Time.deltaTime * rate) {
		planeMaterial.color.a = Mathf.SmoothStep(0.0, 1.0, Mathf.InverseLerp(.75, .15, i));
		// Make plane undulate
		for (var j = 0; j < newVertices.Length; j++) {
			newVertices[j] = baseVertices[j];
			newVertices[j].x += Mathf.Sin(j*waveFrequency + i*time) * scale;
		}
		planeMesh.vertices = newVertices;
		scale = Mathf.Sin(Mathf.PI * Mathf.SmoothStep(0.0, 1.0, i)) * waveScale;
		yield;
	}
 
	// Clean up
	CameraCleanup (cam1, cam2);
	plane.renderer.enabled = false;
	plane.transform.parent = null;
	Destroy(cam2Clone.gameObject);
	cam1.targetTexture = cam2.targetTexture = null;
	renderTex.Release();
	renderTex2.Release();
}
}
