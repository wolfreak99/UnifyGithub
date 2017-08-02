/*************************
 * Original url: http://wiki.unity3d.com/index.php/TubeRenderer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/TubeRenderer.cs
 * File based on original modification date of: 8 November 2013, at 20:19. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Created by StarManta 
    TubeRenderer creates a cylindrical tube much like a LineRenderer creates a flat line. It's great for ropes and the like. 
    You can use SetPoints with and array of Vector3's, or access the array of vertices manually. 
    /*
    TubeRenderer.js
     
    This script is created by Ray Nothnagel of Last Bastion Games. It is 
    free for use and available on the Unify Wiki.
     
    For other components I've created, see:
    http://lastbastiongames.com/middleware/
     
    (C) 2008 Last Bastion Games
    */
     
     
    class TubeVertex {
    	var point : Vector3 = Vector3.zero;
    	var radius : float = 1.0;
    	var color : Color = Color.white;
     
    	function TubeVertex(pt : Vector3, r : float, c : Color) {
    		point=pt;
    		radius=r;
    		color=c;
    	}
    }
     
    var vertices : TubeVertex[];
    var material : Material;
     
    var crossSegments : int = 3;
    private var crossPoints : Vector3[];
    private var lastCrossSegments : int;
    var flatAtDistance : float=-1;
     
    private var lastCameraPosition1 : Vector3;
    private var lastCameraPosition2 : Vector3;
    var movePixelsForRebuild = 6;
    var maxRebuildTime = 0.1;
    private var lastRebuildTime = 0.00;
     
    function Reset() {
    	vertices = [TubeVertex(Vector3.zero, 1.0, Color.white), TubeVertex(Vector3(1,0,0), 1.0, Color.white)];
    }
    function Start() {
    	gameObject.AddComponent(MeshFilter);
    	var mr : MeshRenderer = gameObject.AddComponent(MeshRenderer);
    	mr.material = material;
    }
     
    function LateUpdate () {
    	if (!vertices || vertices.length <= 1) {
    		renderer.enabled=false;
    		return;
    	}
    	renderer.enabled=true;
     
    	//rebuild the mesh?
    	var re : boolean=false;
    	var distFromMainCam : float;
    	if(vertices.length > 1)
    	{
    		cur1 = Camera.main.WorldToScreenPoint(vertices[0].point);
    		distFromMainCam = lastCameraPosition1.z;
    		lastCameraPosition1.z = 0;
    		cur2 = Camera.main.WorldToScreenPoint(vertices[vertices.length - 1].point);
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
     
    	if (re) {
    		//draw tube
     
    		if (crossSegments != lastCrossSegments) {
    			crossPoints = new Vector3[crossSegments];
    			var theta : float = 2.0*Mathf.PI/crossSegments;
    			for (c=0;c<crossSegments;c++) {
    				crossPoints[c] = Vector3(Mathf.Cos(theta*c), Mathf.Sin(theta*c), 0);
    			}
    			lastCrossSegments = crossSegments;
    		}
     
    		var meshVertices : Vector3[] = new Vector3[vertices.length*crossSegments];
    		var uvs : Vector2[] = new Vector2[vertices.length*crossSegments];
    		var colors : Color[] = new Color[vertices.length*crossSegments];
    		var tris : int[] = new int[vertices.length*crossSegments*6];
    		var lastVertices : int[] = new int[crossSegments];
    		var theseVertices : int[] = new int[crossSegments];
    		var rotation : Quaternion;
     
    		for (p=0;p<vertices.length;p++) {
    			if (p<vertices.length-1) rotation = Quaternion.FromToRotation(Vector3.forward, vertices[p+1].point-vertices[p].point);
     
    			for (c=0;c<crossSegments;c++) {
    				var vertexIndex : int = p*crossSegments+c;
    				meshVertices[vertexIndex] = vertices[p].point + rotation * crossPoints[c] * vertices[p].radius;
    				uvs[vertexIndex] = Vector2((0.0+c)/crossSegments,(0.0+p)/vertices.length);
    				colors[vertexIndex] = vertices[p].color;
     
    //				print(c+" - vertex index "+(p*crossSegments+c) + " is " + meshVertices[p*crossSegments+c]);
    				lastVertices[c]=theseVertices[c];
    				theseVertices[c] = p*crossSegments+c;
    			}
    			//make triangles
    			if (p>0) {
    				for (c=0;c<crossSegments;c++) {
    					var start : int= (p*crossSegments+c)*6;
    					tris[start] = lastVertices[c];
    					tris[start+1] = lastVertices[(c+1)%crossSegments];
    					tris[start+2] = theseVertices[c];
    					tris[start+3] = tris[start+2];
    					tris[start+4] = tris[start+1];
    					tris[start+5] = theseVertices[(c+1)%crossSegments];
    //					print("Triangle: indexes("+tris[start]+", "+tris[start+1]+", "+tris[start+2]+"), ("+tris[start+3]+", "+tris[start+4]+", "+tris[start+5]+")");
    				}
    			}
    		}
     
    		var mesh : Mesh =  GetComponent(MeshFilter).mesh;
           	if (!mesh)
           	{
            	mesh = new Mesh();
        	}
    		mesh.vertices = meshVertices;
    		mesh.triangles = tris;
    		mesh.RecalculateNormals();
    		mesh.uv = uvs;
    	}
    }
     
    //sets all the points to points of a Vector3 array, as well as capping the ends.
    function SetPoints(points : Vector3[], radius : float, col : Color) {
    	if (points.length < 2) return;
    	vertices = new TubeVertex[points.length+2];
     
    	var v0offset : Vector3 = (points[0]-points[1])*0.01;
    	vertices[0] = TubeVertex(v0offset+points[0], 0.0, col);
    	var v1offset : Vector3 = (points[points.length-1] - points[points.length-2])*0.01;
    	vertices[vertices.length-1] = TubeVertex(v1offset+points[points.length-1], 0.0, col);
     
    	for (p=0;p<points.length;p++) {
    		vertices[p+1] = TubeVertex(points[p], radius, col);
    	}
    }
    
    
    
    using System;
    using UnityEngine;
     
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class TubeRenderer : MonoBehaviour
    {
        /*
        TubeRenderer.cs
     
        This script is created by Ray Nothnagel of Last Bastion Games. It is 
        free for use and available on the Unify Wiki.
     
        For other components I've created, see:
        http://lastbastiongames.com/middleware/
     
        (C) 2008 Last Bastion Games
        */
     
        [Serializable]
        public class TubeVertex
        {
            public Vector3 point = Vector3.zero;
            public float radius = 1.0f;
            public Color color = Color.white;
     
            public TubeVertex(Vector3 pt, float r, Color c)
            {
                point = pt;
                radius = r;
                color = c;
            }
        }
     
        public TubeVertex[] vertices;
        public Material material;
     
        public int crossSegments = 3;
        private Vector3[] crossPoints;
        private int lastCrossSegments;
        public float flatAtDistance = -1;
     
        private Vector3 lastCameraPosition1;
        private Vector3 lastCameraPosition2;
        public int movePixelsForRebuild = 6;
        public float maxRebuildTime = 0.1f;
        private float lastRebuildTime = 0.00f;
     
        void Reset()
        {
     
            vertices = new TubeVertex[]
            {
                new TubeVertex(Vector3.zero, 1.0f, Color.white), 
                new TubeVertex(new Vector3(1,0,0), 1.0f, Color.white),
            };
        }
        void Start()
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            mr.material = material;
        }
     
        void LateUpdate()
        {
            if (null == vertices ||
                vertices.Length <= 1)
            {
                renderer.enabled = false;
                return;
            }
            renderer.enabled = true;
     
            //rebuild the mesh?
            bool re = false;
            float distFromMainCam;
            if (vertices.Length > 1)
            {
                Vector3 cur1 = Camera.main.WorldToScreenPoint(vertices[0].point);
                distFromMainCam = lastCameraPosition1.z;
                lastCameraPosition1.z = 0;
                Vector3 cur2 = Camera.main.WorldToScreenPoint(vertices[vertices.Length - 1].point);
                lastCameraPosition2.z = 0;
     
                float distance = (lastCameraPosition1 - cur1).magnitude;
                distance += (lastCameraPosition2 - cur2).magnitude;
     
                if (distance > movePixelsForRebuild || Time.time - lastRebuildTime > maxRebuildTime)
                {
                    re = true;
                    lastCameraPosition1 = cur1;
                    lastCameraPosition2 = cur2;
                }
            }
     
            if (re)
            {
                //draw tube
     
                if (crossSegments != lastCrossSegments)
                {
                    crossPoints = new Vector3[crossSegments];
                    float theta = 2.0f * Mathf.PI / crossSegments;
                    for (int c = 0; c < crossSegments; c++)
                    {
                        crossPoints[c] = new Vector3(Mathf.Cos(theta * c), Mathf.Sin(theta * c), 0);
                    }
                    lastCrossSegments = crossSegments;
                }
     
                Vector3[] meshVertices = new Vector3[vertices.Length * crossSegments];
                Vector2[] uvs = new Vector2[vertices.Length * crossSegments];
                Color[] colors = new Color[vertices.Length * crossSegments];
                int[] tris = new int[vertices.Length * crossSegments * 6];
                int[] lastVertices = new int[crossSegments];
                int[] theseVertices = new int[crossSegments];
                Quaternion rotation = Quaternion.identity;
     
                for (int p = 0; p < vertices.Length; p++)
                {
                    if (p < vertices.Length - 1) rotation = Quaternion.FromToRotation(Vector3.forward, vertices[p + 1].point - vertices[p].point);
     
                    for (int c = 0; c < crossSegments; c++)
                    {
                        int vertexIndex = p * crossSegments + c;
                        meshVertices[vertexIndex] = vertices[p].point + rotation * crossPoints[c] * vertices[p].radius;
                        uvs[vertexIndex] = new Vector2((0.0f + c) / crossSegments, (0.0f + p) / vertices.Length);
                        colors[vertexIndex] = vertices[p].color;
     
                        //				print(c+" - vertex index "+(p*crossSegments+c) + " is " + meshVertices[p*crossSegments+c]);
                        lastVertices[c] = theseVertices[c];
                        theseVertices[c] = p * crossSegments + c;
                    }
                    //make triangles
                    if (p > 0)
                    {
                        for (int c = 0; c < crossSegments; c++)
                        {
                            int start = (p * crossSegments + c) * 6;
                            tris[start] = lastVertices[c];
                            tris[start + 1] = lastVertices[(c + 1) % crossSegments];
                            tris[start + 2] = theseVertices[c];
                            tris[start + 3] = tris[start + 2];
                            tris[start + 4] = tris[start + 1];
                            tris[start + 5] = theseVertices[(c + 1) % crossSegments];
                            //					print("Triangle: indexes("+tris[start]+", "+tris[start+1]+", "+tris[start+2]+"), ("+tris[start+3]+", "+tris[start+4]+", "+tris[start+5]+")");
                        }
                    }
                }
     
                Mesh mesh = GetComponent<MeshFilter>().mesh;
                if (!mesh)
                {
                    mesh = new Mesh();
                }
                mesh.vertices = meshVertices;
                mesh.triangles = tris;
                mesh.RecalculateNormals();
                mesh.uv = uvs;
            }
        }
     
        //sets all the points to points of a Vector3 array, as well as capping the ends.
        void SetPoints(Vector3[] points, float radius, Color col)
        {
            if (points.Length < 2) return;
            vertices = new TubeVertex[points.Length + 2];
     
            Vector3 v0offset = (points[0] - points[1]) * 0.01f;
            vertices[0] = new TubeVertex(v0offset + points[0], 0.0f, col);
            Vector3 v1offset = (points[points.Length - 1] - points[points.Length - 2]) * 0.01f;
            vertices[vertices.Length - 1] = new TubeVertex(v1offset + points[points.Length - 1], 0.0f, col);
     
            for (int p = 0; p < points.Length; p++)
            {
                vertices[p + 1] = new TubeVertex(points[p], radius, col);
            }
        }
    }
}
