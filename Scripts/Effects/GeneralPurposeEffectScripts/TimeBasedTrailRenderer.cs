// Original url: http://wiki.unity3d.com/index.php/TimeBasedTrailRenderer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/TimeBasedTrailRenderer.cs
// File based on original modification date of: 23 April 2016, at 14:51. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{

Author: Forest Johnson (Yoggy) 
iPhone version by Niels Vanspauwen (nvsp) 
C# version by xyber from Unity Forums 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - TimeTrail.js 
4 JavaScript - iPhoneTimeTrail.js 
5 C# - TimeTrail.cs 

Description I created this time based line renderer because I don't like how Unity's built in one works. As well as being time based instead of position based it also has some other features, like arbitrarily and independently sized color and scale fades, built in time out if it is needed, the ability to start and stop emitting at any time, and more optimization features so the line only has exactly as many segments as you want it to have or only as many as it needs to look good. 
No doubt this is less WSIWYG and probably slower than the trail renderer or even line renderer but it generates better results in game. 
Usage Attach the script to any object that moves and should have a trail behind it. 

What all the variables do: 
Emit: Works just like on a particle emitter 
Emit Time: automatically stop emitting after this many seconds, or leave at 0 for infinite 
Material: the material to use on the line 
Life Time: how long it takes for a line segment created at the front end of the line to fade out completely 
Colors: Set the color fade with any number of colors 
Sizes: Set the size fade with any number of sizes 
Uv Length Scale: Set it real small unless your line has details along the x axis in the texture 
Higher Quality UVs: this is the difference between using .magnitude and .sqrMagnitude. Doesn't matter much. 
Move Pixels For Rebuild: The line mesh will not be rebuilt unless the line moves more than this many pixels on the screen. Can save a bit by not rebuilding on big lines that are not moving, and the camera is not moving much 
Min Vertex Distance: Never make segments shorter than this 
Max Vertex Distance: Never make segments longer than this 
Max Angle: Never make angles between segments greater than this. Increasing it will make a faster but uglier line. 
Auto Destruct: Destroy this object and the render object if we are not emitting and have no segments left . 
JavaScript - TimeTrail.js var points = Array();
var emit = true;
var emitTime = 0.00;
var material : Material;
 
var lifeTime = 1.00;
 
var colors : Color[];
var sizes : float[];
 
var uvLengthScale = 0.01;
var higherQualityUVs = true;
 
var movePixelsForRebuild = 6;
var maxRebuildTime = 0.1;
 
var minVertexDistance = 0.10;
 
var maxVertexDistance = 10.00;
var maxAngle = 3.00;
 
var autoDestruct = false;
 
private var o : GameObject;
private var lastPosition : Vector3;
private var lastCameraPosition1 : Vector3;
private var lastCameraPosition2 : Vector3;
private var lastRebuildTime = 0.00;
private var lastFrameEmit = true;
 
class Point
{
   var timeCreated = 0.00;
   var position : Vector3;
   var lineBreak = false;
}
 
function Start ()
{
   lastPosition = transform.position;
   o = new GameObject("Trail");
   o.transform.parent = null;
   o.transform.position = Vector3.zero;
   o.transform.rotation = Quaternion.identity;
   o.transform.localScale = Vector3.one;
   o.AddComponent(MeshFilter);
   o.AddComponent(MeshRenderer);
   o.renderer.material = material;
}
 
function OnEnable ()
{
   lastPosition = transform.position;
   o = new GameObject("Trail");
   o.transform.parent = null;
   o.transform.position = Vector3.zero;
   o.transform.rotation = Quaternion.identity;
   o.transform.localScale = Vector3.one;
   o.AddComponent(MeshFilter);
   o.AddComponent(MeshRenderer);
   o.renderer.material = material;
}
 
function OnDisable ()
{
   Destroy(o);   
}
 
function Update ()
{
   if(emit && emitTime != 0)
   {
      emitTime -= Time.deltaTime;
      if(emitTime == 0) emitTime = -1;
      if(emitTime < 0) emit = false;
   }
 
   if(!emit && points.length == 0 && autoDestruct)
   {
      Destroy(o);
      Destroy(gameObject);
   }
 
   // early out if there is no camera
   if(!Camera.main) return;
 
   re = false;
 
   // if we have moved enough, create a new vertex and make sure we rebuild the mesh
   theDistance = (lastPosition - transform.position).magnitude;
   if(emit)
   {
      if(theDistance > minVertexDistance)
      {
         make = false;
         if(points.length < 3)
         {
            make = true;
         }
         else
         {
            l1 = points[points.length - 2].position - points[points.length - 3].position;
            l2 = points[points.length - 1].position - points[points.length - 2].position;
            if(Vector3.Angle(l1, l2) > maxAngle || theDistance > maxVertexDistance) make = true;
         }
 
         if(make)
         {
            p = new Point();
            p.position = transform.position;
            p.timeCreated = Time.time;
            points.Add(p);
            lastPosition = transform.position;
         }
         else
         {
            points[points.length - 1].position = transform.position;
            points[points.length - 1].timeCreated = Time.time;
         }
      }
      else if(points.length > 0)
      {
         points[points.length - 1].position = transform.position;
         points[points.length - 1].timeCreated = Time.time;
      }
   }
 
   if(!emit && lastFrameEmit && points.length > 0) points[points.length - 1].lineBreak = true;
   lastFrameEmit = emit;
 
   // approximate if we should rebuild the mesh or not
   if(points.length > 1)
   {
      cur1 = Camera.main.WorldToScreenPoint(points[0].position);
      lastCameraPosition1.z = 0;
      cur2 = Camera.main.WorldToScreenPoint(points[points.length - 1].position);
      lastCameraPosition2.z = 0;
 
      distance = (lastCameraPosition1 - cur1).magnitude;
      distance += (lastCameraPosition2 - cur2).magnitude;
 
      if(distance > movePixelsForRebuild || Time.time - lastRebuildTime > maxRebuildTime)
      {
         re = true;
         lastCameraPosition1 = cur1;
         lastCameraPosition2 = cur2;
      }
   }
   else
   {
      re = true;   
   }
 
 
   if(re)
   {
      lastRebuildTime = Time.time;
 
      i = 0;
      for(var p : Point in points)
      {
         // cull old points first
         if(Time.time - p.timeCreated > lifeTime)
         {
            points.RemoveAt(i);
         }
         i++;
      }
 
      if(points.length > 1)
      {
         var newVertices : Vector3[] = new Vector3[points.length * 2];
         var newUV : Vector2[] = new Vector2[points.length * 2];
         var newTriangles : int[] = new int[(points.length - 1) * 6];
         var newColors : Color[] = new Color[points.length * 2];
 
         i = 0;
         var curDistance = 0.00;
 
         for(var p : Point in points)
         {
            time = (Time.time - p.timeCreated) / lifeTime;
 
            if(colors.length > 0)
            {
               colorTime = time * (colors.length - 1);
               min = Mathf.Floor(colorTime);
               max = Mathf.Clamp(Mathf.Ceil(colorTime), 1, colors.length - 1);
               lerp = Mathf.InverseLerp(min, max, colorTime);
               color = Color.Lerp(colors[min], colors[max], lerp);
            }
            else
            {
               color = Color.Lerp(Color.white, Color.clear, time);
            }
 
            if(sizes.length > 0)
            {
               sizeTime = time * (sizes.length - 1);
               min = Mathf.Floor(sizeTime);
               max = Mathf.Clamp(Mathf.Ceil(sizeTime), 1, sizes.length - 1);
               lerp = Mathf.InverseLerp(min, max, sizeTime);
               size = Mathf.Lerp(sizes[min], sizes[max], lerp);
            }
            else
            {
               size = 1;   
            }
 
            if(i == 0) lineDirection = p.position - points[i + 1].position;
            else lineDirection = points[i - 1].position - p.position;
            vectorToCamera = Camera.main.transform.position - p.position;
 
            perpendicular = Vector3.Cross(lineDirection, vectorToCamera).normalized;
 
            newVertices[i * 2] = p.position + (perpendicular * (size * 0.5));
            newVertices[(i * 2) + 1] = p.position + (-perpendicular * (size * 0.5));
 
            newColors[i * 2] = newColors[(i * 2) + 1] = color;
 
            newUV[i * 2] = Vector2(curDistance * uvLengthScale, 0);
            newUV[(i * 2) + 1] = Vector2(curDistance * uvLengthScale, 1);
 
            if(i > 0 && !points[i - 1].lineBreak)
            {
               if(higherQualityUVs) curDistance += (p.position - points[i - 1].position).magnitude;
               else curDistance += (p.position - points[i - 1].position).sqrMagnitude;
 
               newTriangles[(i - 1) * 6] = (i * 2) - 2;
               newTriangles[((i - 1) * 6) + 1] = (i * 2) - 1;
               newTriangles[((i - 1) * 6) + 2] = i * 2;
 
               newTriangles[((i - 1) * 6) + 3] = (i * 2) + 1;
               newTriangles[((i - 1) * 6) + 4] = i * 2;
               newTriangles[((i - 1) * 6) + 5] = (i * 2) - 1;
            }
 
            i ++;
         }
 
         var mesh : Mesh = o.GetComponent(MeshFilter).mesh;
         mesh.Clear();
         mesh.vertices = newVertices;
         mesh.colors = newColors;
         mesh.uv = newUV;
         mesh.triangles = newTriangles;
      }
   }
}JavaScript - iPhoneTimeTrail.js The version above doesn't work on Unity iPhone. This one does. I did a couple of things to optimize it, but I'm sure there's more room for improvement. 
#pragma strict
 
class Point
{
   var timeCreated = 0.00;
   var position : Vector3;
   var lineBreak = false;
}
 
class PointArray
{
  private var arr : Point[];
  private var head : int = 0;
  private var tail : int = -1;
  private var nextFreePos : int = 0;
  private var size : int = 0;
 
  function PointArray(s : int) {
  	arr = new Point[s];
  	for (i=0; i < s; ++i) {
  		arr[i] = new Point();
  	}
  }
 
  function Reset() {
  	head = 0;
  	tail = -1;
  	nextFreePos = 0;
  	size = 0;
  }	
 
	function Add(pos : Vector3) {
		if (IsFull()) {
			var new_arr : Point[] = new Point[arr.length * 2];
			for (i=0; i < arr.length; ++i) {
				new_arr[i] = arr[i];
			}
			for (i=arr.length; i < new_arr.length; ++i) {
				new_arr[i] = new Point();
			}
			nextFreePos = arr.length;
			arr = new_arr;
		}
 
		arr[nextFreePos].timeCreated = Time.time;
		arr[nextFreePos].position = pos;
		arr[nextFreePos].lineBreak = false;
		tail = nextFreePos;
		nextFreePos = (nextFreePos + 1) % arr.length;
		++size;
	}
 
	function Length() {
		return size;
	}
 
	function IsEmpty() {
	  return size == 0;
	}
 
	function NotEmpty() {
		return size != 0;
	}
 
	function IsFull() {
		return size == (arr.length-1);
	}
 
	// Use positive relInx to start counting from the head of the circular array
	// Use negative relInx to start counter from the tail of the circular array
	// (The relative index must always be in the range 0..size-1)
	function At(relInx : int) {
		return arr[RelativeToAbsoluteIndex(relInx)];
	}
 
	function RelativeToAbsoluteIndex(relInx : int) {
		if (relInx >= 0) {
			return (head + relInx) % arr.length;
		} else {
			return tail + relInx + 1;
		}
	}
 
	function RemoveDeadPoints(lifeTime: float) {
		if (IsEmpty()) return;
 
		// Skip over dead points to find the new head
		for (i = 0; i < size; ++i) {
			var age : float = (Time.time - At(i).timeCreated) * 1.0;
			if (age <= lifeTime) {
				head = RelativeToAbsoluteIndex(i);
				size -= i;
				return;
			}
		}
 
		// If we get here, it means all points are dead
		Reset();
	}
}
 
var emit = true;
var emitTime = 0.00;
var material : Material;
 
var lifeTime : float = 1.00;
 
var colors : Color[];
var sizes : float[];
 
var uvLengthScale = 0.01;
var higherQualityUVs = true;
 
var movePixelsForRebuild = 6;
var maxRebuildTime = 0.1;
 
var minVertexDistance = 0.10;
 
var maxVertexDistance = 10.00;
var maxAngle = 3.00;
 
var autoDestruct = false;
var initialNumberOfPoints : int = 50;
 
private var o : GameObject;
private var points : PointArray;
private var lastPosition : Vector3;
private var lastCameraPosition1 : Vector3;
private var lastCameraPosition2 : Vector3;
private var lastRebuildTime = 0.00;
private var lastFrameEmit = true;
 
 
function Start()
{
  InitTrail(); 
}
 
function OnEnable()
{
   InitTrail();
}
 
function InitTrail() 
{
  lastPosition = transform.position;
  o = new GameObject("Trail");
  o.transform.parent = null;
  o.transform.position = Vector3.zero;
  o.transform.rotation = Quaternion.identity;
  o.transform.localScale = Vector3.one;
  o.AddComponent(MeshFilter);
  o.AddComponent(MeshRenderer);
  o.renderer.material = material;
  if (points) {
    points.Reset();
  } else {
	  points = new PointArray(initialNumberOfPoints);
  }
}
 
function OnDisable()
{
   Destroy(o);   
}
 
function Update()
{
   if (emit && emitTime != 0) {
      emitTime -= Time.deltaTime;
      if (emitTime == 0) emitTime = -1;
      if (emitTime < 0) emit = false;
   }
 
   if (!emit && points.IsEmpty() && autoDestruct) {
      Destroy(o);
      Destroy(gameObject);
   }
 
   // early out if there is no camera
   if (!Camera.main) return;
 
   re = false;
 
   // if we have moved enough, create a new vertex and make sure we rebuild the mesh
   theDistance = (lastPosition - transform.position).magnitude;
   if (emit) {
      if (theDistance > minVertexDistance) {
         make = false;
         if (points.Length() < 3) {
            make = true;
         } else {
            l1 = points.At(-2).position - points.At(-3).position;
            l2 = points.At(-1).position - points.At(-2).position;
            if (Vector3.Angle(l1, l2) > maxAngle || theDistance > maxVertexDistance) 
            	make = true;
         }
 
         if (make) {
            points.Add(transform.position);
            lastPosition = transform.position;
         } else {
            points.At(-1).position = transform.position;
            points.At(-1).timeCreated = Time.time;
         }
 
      } else if (points.NotEmpty()) {
         points.At(-1).position = transform.position;
         points.At(-1).timeCreated = Time.time;
      }
   }
 
   if (!emit && lastFrameEmit && points.NotEmpty()) 
   	 points.At(-1).lineBreak = true;
 
   lastFrameEmit = emit;
 
   // approximate if we should rebuild the mesh or not
   if (points.Length() > 1) {
      cur1 = Camera.main.WorldToScreenPoint(points.At(0).position);
      lastCameraPosition1.z = 0;
      cur2 = Camera.main.WorldToScreenPoint(points.At(-1).position);
      lastCameraPosition2.z = 0;
 
      distance = (lastCameraPosition1 - cur1).magnitude;
      distance += (lastCameraPosition2 - cur2).magnitude;
 
      if (distance > movePixelsForRebuild || Time.time - lastRebuildTime > maxRebuildTime) {
         re = true;
         lastCameraPosition1 = cur1;
         lastCameraPosition2 = cur2;
      }
   } else {
      re = true;   
   }
 
 
   if (re) {
      lastRebuildTime = Time.time;
      points.RemoveDeadPoints(lifeTime);
 
      var l : int = points.Length();
      if (l > 1) { 	
         var newVertices : Vector3[] = new Vector3[l * 2];
         var newUV : Vector2[] = new Vector2[l * 2];
         var newTriangles : int[] = new int[(l - 1) * 6];
         var newColors : Color[] = new Color[l * 2];
         var curDistance = 0.00;
 
         for (i = 0; i < l; ++i) {
         		p = points.At(i);
            time = (Time.time - p.timeCreated) / lifeTime;
 
            if(colors.length > 0) {
               colorTime = time * (colors.length - 1);
               min = Mathf.Floor(colorTime);
               max = Mathf.Clamp(Mathf.Ceil(colorTime), 0, colors.length - 1);
               lerp = Mathf.InverseLerp(min, max, colorTime);
               color = Color.Lerp(colors[min], colors[max], lerp);
            } else {
               color = Color.Lerp(Color.white, Color.clear, time);
            }
 
            if(sizes.length > 0) {
               sizeTime = time * (sizes.length - 1);
               min = Mathf.Floor(sizeTime);
               max = Mathf.Clamp(Mathf.Ceil(sizeTime), 0, sizes.length - 1);
               lerp = Mathf.InverseLerp(min, max, sizeTime);
               size = Mathf.Lerp(sizes[min], sizes[max], lerp);
            } else {
               size = 1;   
            }
 
            if (i == 0) {
            	lineDirection = p.position - points.At(i + 1).position;
            } else {
            	lineDirection = points.At(i - 1).position - p.position;
            }
 
            vectorToCamera = Camera.main.transform.position - p.position;
 
            perpendicular = Vector3.Cross(lineDirection, vectorToCamera).normalized;
 
            newVertices[i * 2] = p.position + (perpendicular * (size * 0.5));
            newVertices[(i * 2) + 1] = p.position + (-perpendicular * (size * 0.5));
 
            newColors[i * 2] = newColors[(i * 2) + 1] = color;
 
            newUV[i * 2] = Vector2(curDistance * uvLengthScale, 0);
            newUV[(i * 2) + 1] = Vector2(curDistance * uvLengthScale, 1);
 
            if (i > 0 && !points.At(-1).lineBreak) {           
               if (higherQualityUVs) {
               		curDistance += (p.position - points.At(-1).position).magnitude;
               } else {
               		curDistance += (p.position - points.At(-1).position).sqrMagnitude;
               }
 
               newTriangles[(i - 1) * 6] = (i * 2) - 2;
               newTriangles[((i - 1) * 6) + 1] = (i * 2) - 1;
               newTriangles[((i - 1) * 6) + 2] = i * 2;
 
               newTriangles[((i - 1) * 6) + 3] = (i * 2) + 1;
               newTriangles[((i - 1) * 6) + 4] = i * 2;
               newTriangles[((i - 1) * 6) + 5] = (i * 2) - 1;
            }
         }
 
         var mesh : Mesh = (o.GetComponent(MeshFilter) as MeshFilter).mesh;
         mesh.Clear();
         mesh.vertices = newVertices;
         mesh.colors = newColors;
         mesh.uv = newUV;
         mesh.triangles = newTriangles;
      }
   }
}

C# - TimeTrail.cs "Converted to C# and tested on iPhone." -xyber 
using UnityEngine;
using System.Collections;
 
public class TimedTrailRenderer : MonoBehaviour
{
 
   public bool emit = true;
   public float emitTime = 0.00f;
   public Material material;
 
   public float lifeTime = 1.00f;
 
   public Color[] colors;
   public float[] sizes;
 
   public float uvLengthScale = 0.01f;
   public bool higherQualityUVs = true;
 
   public int movePixelsForRebuild = 6;
   public float maxRebuildTime = 0.1f;
 
   public float minVertexDistance = 0.10f;
 
   public float maxVertexDistance = 10.00f;
   public float maxAngle = 3.00f;
 
   public bool autoDestruct = false;
 
   private ArrayList points = new ArrayList();
   private GameObject o;
   private Vector3 lastPosition;
   private Vector3 lastCameraPosition1;
   private Vector3 lastCameraPosition2;
   private float lastRebuildTime = 0.00f;
   private bool lastFrameEmit = true;
 
   public class Point
   {
      public float timeCreated = 0.00f;
      public Vector3 position;
      public bool lineBreak = false;
   }
 
   void Start()
   {
      lastPosition = transform.position;
      o = new GameObject("Trail");
      o.transform.parent = null;
      o.transform.position = Vector3.zero;
      o.transform.rotation = Quaternion.identity;
      o.transform.localScale = Vector3.one;
      o.AddComponent(typeof(MeshFilter));
      o.AddComponent(typeof(MeshRenderer));
      o.GetComponent<Renderer>().sharedMaterial = material;
   }
 
   void OnEnable ()
   {
      lastPosition = transform.position;
      o = new GameObject("Trail");
      o.transform.parent = null;
      o.transform.position = Vector3.zero;
      o.transform.rotation = Quaternion.identity;
      o.transform.localScale = Vector3.one;
      o.AddComponent(typeof(MeshFilter));
      o.AddComponent(typeof(MeshRenderer));
      o.GetComponent<Renderer>().sharedMaterial = material;
   }
 
   void OnDisable ()
   {
      Destroy(o);   
   }
 
   void Update ()
   {
      if(emit && emitTime != 0)
      {
        emitTime -= Time.deltaTime;
        if(emitTime == 0) emitTime = -1;
        if(emitTime < 0) emit = false;
      }
 
      if(!emit && points.Count == 0 && autoDestruct)
      {
        Destroy(o);
        Destroy(gameObject);
      }
 
      // early out if there is no camera
      if(!Camera.main) return;
 
      bool re = false;
 
      // if we have moved enough, create a new vertex and make sure we rebuild the mesh
      float theDistance = (lastPosition - transform.position).magnitude;
      if(emit)
      {
        if(theDistance > minVertexDistance)
        {
          bool make = false;
          if(points.Count < 3)
          {
            make = true;
          }
          else
          {
            Vector3 l1 = ((Point)points[points.Count - 2]).position - ((Point)points[points.Count - 3]).position;
            Vector3 l2 = ((Point)points[points.Count - 1]).position - ((Point)points[points.Count - 2]).position;
            if(Vector3.Angle(l1, l2) > maxAngle || theDistance > maxVertexDistance) make = true;
          }
 
          if(make)
          {
            Point p = new Point();
            p.position = transform.position;
            p.timeCreated = Time.time;
            points.Add(p);
            lastPosition = transform.position;
          }
          else
          {
            ((Point)points[points.Count - 1]).position = transform.position;
            ((Point)points[points.Count - 1]).timeCreated = Time.time;
          }
        }
        else if(points.Count > 0)
        {
          ((Point)points[points.Count - 1]).position = transform.position;
          ((Point)points[points.Count - 1]).timeCreated = Time.time;
        }
      }
 
      if(!emit && lastFrameEmit && points.Count > 0) ((Point)points[points.Count - 1]).lineBreak = true;
      lastFrameEmit = emit;
 
      // approximate if we should rebuild the mesh or not
      if(points.Count > 1)
      {
        Vector3 cur1 = Camera.main.WorldToScreenPoint(((Point)points[0]).position);
        lastCameraPosition1.z = 0;
        Vector3 cur2 = Camera.main.WorldToScreenPoint(((Point)points[points.Count - 1]).position);
        lastCameraPosition2.z = 0;
 
        float distance = (lastCameraPosition1 - cur1).magnitude;
        distance += (lastCameraPosition2 - cur2).magnitude;
 
        if(distance > movePixelsForRebuild || Time.time - lastRebuildTime > maxRebuildTime)
        {
          re = true;
          lastCameraPosition1 = cur1;
          lastCameraPosition2 = cur2;
        }
      }
      else
      {
        re = true;   
      }
 
 
      if(re)
      {
        lastRebuildTime = Time.time;
 
        ArrayList remove = new ArrayList();
        int i = 0;
        foreach (Point p in points)
        {
          // cull old points first
           if (Time.time - p.timeCreated > lifeTime) remove.Add(p);
          i++;
        }
 
        foreach (Point p in remove) points.Remove(p);
        remove.Clear();
 
        if(points.Count > 1)
        {
          Vector3[] newVertices = new Vector3[points.Count * 2];
          Vector2[] newUV = new Vector2[points.Count * 2];
          int[] newTriangles = new int[(points.Count - 1) * 6];
          Color[] newColors = new Color[points.Count * 2];
 
          i = 0;
          float curDistance = 0.00f;
 
          foreach (Point p in points)
          {
            float time = (Time.time - p.timeCreated) / lifeTime;
 
            Color color = Color.Lerp(Color.white, Color.clear, time);
            if (colors != null && colors.Length > 0)
            {
               float colorTime = time * (colors.Length - 1);
               float min = Mathf.Floor(colorTime);
               float max = Mathf.Clamp(Mathf.Ceil(colorTime), 1, colors.Length - 1);
               float lerp = Mathf.InverseLerp(min, max, colorTime);
               if (min >= colors.Length) min = colors.Length - 1; if (min < 0) min = 0;
               if (max >= colors.Length) max = colors.Length - 1; if (max < 0) max = 0;
               color = Color.Lerp(colors[(int)min], colors[(int)max], lerp);
            }
 
            float size = 1f;
            if (sizes != null && sizes.Length > 0)
            {
               float sizeTime = time * (sizes.Length - 1);
               float min = Mathf.Floor(sizeTime);
               float max = Mathf.Clamp(Mathf.Ceil(sizeTime), 1, sizes.Length - 1);
               float lerp = Mathf.InverseLerp(min, max, sizeTime);
               if (min >= sizes.Length) min = sizes.Length - 1; if (min < 0) min = 0;
               if (max >= sizes.Length) max = sizes.Length - 1; if (max < 0) max = 0;
               size = Mathf.Lerp(sizes[(int)min], sizes[(int)max], lerp);
            }
 
            Vector3 lineDirection = Vector3.zero;
            if(i == 0) lineDirection = p.position - ((Point)points[i + 1]).position;
            else lineDirection = ((Point)points[i - 1]).position - p.position;
 
            Vector3 vectorToCamera = Camera.main.transform.position - p.position;
            Vector3 perpendicular = Vector3.Cross(lineDirection, vectorToCamera).normalized;
 
            newVertices[i * 2] = p.position + (perpendicular * (size * 0.5f));
            newVertices[(i * 2) + 1] = p.position + (-perpendicular * (size * 0.5f));
 
            newColors[i * 2] = newColors[(i * 2) + 1] = color;
 
            newUV[i * 2] = new Vector2(curDistance * uvLengthScale, 0);
            newUV[(i * 2) + 1] = new Vector2(curDistance * uvLengthScale, 1);
 
            if(i > 0 && !((Point)points[i - 1]).lineBreak)
            {
               if(higherQualityUVs) curDistance += (p.position - ((Point)points[i - 1]).position).magnitude;
               else curDistance += (p.position - ((Point)points[i - 1]).position).sqrMagnitude;
 
               newTriangles[(i - 1) * 6] = (i * 2) - 2;
               newTriangles[((i - 1) * 6) + 1] = (i * 2) - 1;
               newTriangles[((i - 1) * 6) + 2] = i * 2;
 
               newTriangles[((i - 1) * 6) + 3] = (i * 2) + 1;
               newTriangles[((i - 1) * 6) + 4] = i * 2;
               newTriangles[((i - 1) * 6) + 5] = (i * 2) - 1;
            }
 
            i++;
          }
 
          Mesh mesh = (o.GetComponent(typeof(MeshFilter)) as MeshFilter).mesh;
          mesh.Clear();
          mesh.vertices = newVertices;
          mesh.colors = newColors;
          mesh.uv = newUV;
          mesh.triangles = newTriangles;
        }
      }
   }
}
}
