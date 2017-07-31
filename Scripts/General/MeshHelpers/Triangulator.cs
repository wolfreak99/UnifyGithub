// Original url: http://wiki.unity3d.com/index.php/Triangulator
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MeshHelpers/Triangulator.cs
// File based on original modification date of: 23 August 2016, at 12:17. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
Author: runevision 
Contents [hide] 
1 Description 
2 Usage 
3 Troubleshooting 
4 C#- Triangulator.cs 
5 Javascript Triangulator Code 
6 javascript - Triangulator.js 

DescriptionThis script can be used to split a 2D polygon into triangles. The algorithm supports concave polygons, but not polygons with holes, or multiple polygons at once. 
Note: This is a naive triangulation implementation. For more well-distributed triangles, consider using Delaunay triangulation, such as with the script here [1] 
UsageCreate a new Triangulator object with a array of Vector2 points as the constructor parameter. Then get the indices by calling the Triangulate method on the Triangulator. You can now use the points and the indices to construct a mesh. Example use (attach this script to a game object): 
using UnityEngine;
 
public class PolygonTester : MonoBehaviour {
    void Start () {
        // Create Vector2 vertices
        Vector2[] vertices2D = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,50),
            new Vector2(50,50),
            new Vector2(50,100),
            new Vector2(0,100),
            new Vector2(0,150),
            new Vector2(150,150),
            new Vector2(150,100),
            new Vector2(100,100),
            new Vector2(100,50),
            new Vector2(150,50),
            new Vector2(150,0),
        };
 
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();
 
        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i=0; i<vertices.Length; i++) {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }
 
        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();
 
        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }
}TroubleshootingIf you can't see a polygon created with this utility, remember to check if the polygon is facing the opposite direction. If it is, you can change that by constructing your mesh with the vertex indices in reverse order. 
C#- Triangulator.csusing UnityEngine;
using System.Collections.Generic;
 
public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();
 
    public Triangulator (Vector2[] points) {
        m_points = new List<Vector2>(points);
    }
 
    public int[] Triangulate() {
        List<int> indices = new List<int>();
 
        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();
 
        int[] V = new int[n];
        if (Area() > 0) {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }
 
        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2; ) {
            if ((count--) <= 0)
                return indices.ToArray();
 
            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;
 
            if (Snip(u, v, w, nv, V)) {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }
 
        indices.Reverse();
        return indices.ToArray();
    }
 
    private float Area () {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++) {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }
 
    private bool Snip (int u, int v, int w, int n, int[] V) {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++) {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }
 
    private bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;
 
        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;
 
        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;
 
        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}Javascript Triangulator CodeThe steps for using this triangulation code are the same as for the c sharp example. You just need to create a Triangulator object like this: 
var tr:Triangulator = new Triangulator(); tr.initTriangulator(verts2d); var triangles:int[] = tr.Triangulate(); 
and then assign the triangles array to mesh.triangles of your desired mesh object. 


javascript - Triangulator.js#pragma strict
 
import System.Collections.Generic;
 
public class Triangulator {
 
	private var m_points:List.<Vector2> = new List.<Vector2>();
 
	public function Triangulator(points:Vector2[]) {
		m_points = new List.<Vector2>(points);
	}
 
	public function Triangulate() {
		var indices:List.<int> = new List.<int>();
 
		var n:int = m_points.Count;
		if (n < 3)
			return indices.ToArray();
 
		var V:int[] = new int[n];
		if (Area() > 0) 
		{
			for (var v:int = 0; v < n; v++)
				V[v] = v;
		}
		else {
			for (v = 0; v < n; v++)
				V[v] = (n - 1) - v;
		}
 
		var nv:int = n;
		var count:int = 2 * nv;
		var m=0;
		for (v = nv - 1; nv > 2; ) 
		{
			if ((count--) <= 0)
				return indices.ToArray();
 
			var u:int = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			var w:int = v + 1;
			if (nv <= w)
				w = 0;
 
			if (Snip(u, v, w, nv, V)) 
			{
				var a:int;
				var b:int;
				var c:int;
				var s:int;
				var t:int;
				a = V[u];
				b = V[v];
				c = V[w];
				indices.Add(a);
				indices.Add(b);
				indices.Add(c);
				m++;
				s = v;
				for (t = v + 1; t < nv; t++)
				{
					V[s] = V[t];
					s++;
				}
				nv--;
				count = 2 * nv;
			}
		}
 
		indices.Reverse();
		return indices.ToArray();
	}
 
	private function Area () {
		var n:int = m_points.Count;
		var A:float = 0.0f;
		var q:int=0;
		for (var p:int = n - 1; q < n; p = q++) {
			var pval:Vector2 = m_points[p];
			var qval:Vector2 = m_points[q];
			A += pval.x * qval.y - qval.x * pval.y;
		}
		return (A * 0.5);
	}
 
	private function Snip (u:int, v:int, w:int, n:int, V:int[]) {
		var p:int;
		var A:Vector2 = m_points[V[u]];
		var B:Vector2 = m_points[V[v]];
		var C:Vector2 = m_points[V[w]];
		if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
			return false;
		for (p = 0; p < n; p++) {
			if ((p == u) || (p == v) || (p == w))
				continue;
			var P:Vector2 = m_points[V[p]];
			if (InsideTriangle(A, B, C, P))
				return false;
		}
		return true;
	}
 
	private function InsideTriangle (A:Vector2, B:Vector2, C:Vector2, P:Vector2) {
		var ax:float;
		var ay:float;
		var bx:float;
		var by:float;
		var cx:float;
		var cy:float;
		var apx:float;
		var apy:float;
		var bpx:float;
		var bpy:float;
		var cpx:float;
		var cpy:float;
		var cCROSSap:float;
		var bCROSScp:float;
		var aCROSSbp:float;
 
		ax = C.x - B.x; ay = C.y - B.y;
		bx = A.x - C.x; by = A.y - C.y;
		cx = B.x - A.x; cy = B.y - A.y;
		apx = P.x - A.x; apy = P.y - A.y;
		bpx = P.x - B.x; bpy = P.y - B.y;
		cpx = P.x - C.x; cpy = P.y - C.y;
 
		aCROSSbp = ax * bpy - ay * bpx;
		cCROSSap = cx * apy - cy * apx;
		bCROSScp = bx * cpy - by * cpx;
 
		return ((aCROSSbp >= 0.0) && (bCROSScp >= 0.0) && (cCROSSap >= 0.0));
	}
}
}
