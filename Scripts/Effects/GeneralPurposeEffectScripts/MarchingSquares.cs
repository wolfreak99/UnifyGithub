/*************************
 * Original url: http://wiki.unity3d.com/index.php/MarchingSquares
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/MarchingSquares.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Description 
 *   
 * Related Work 
 *   
 * Usage 
 *   
 * MarchingSquares.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Description This Script renders a arbitrary 2D Mesh out of a Point / Voxel Lattice. It actually computes two Meshes, one for rendering (Purple) and one simple mesh for collisions (Blue). 
     
    Related Work Some of the Code is derived from "Polygonising a scalar field (Marching Cubes)" by Paul Bourke http://local.wasp.uwa.edu.au/~pbourke/geometry/polygonise/ A lot of inspiration of how to use it with Unity by Brian R. Cowan. See some of his Work here: http://www.briancowan.net/unity/fx 
    Usage Just attach the Script to an empty GameObject. Make sure you have a Camera in the Scene named in the standard way "Main Camera" (its just there for convenience - if you dont want it, just remove it from the script: lines 527ff). 
    
    
    MarchingSquares.cs ///   Marching Squares: 2D Surface Reconstruction
    ///
    ///   Derived from "Polygonising a scalar field (Marching Cubes)" by Paul Bourke
    ///   http://local.wasp.uwa.edu.au/~pbourke/geometry/polygonise/
    ///   And a lot of inspiration of how to use it with Unity by Brian R. Cowan.
    ///   See some of his Work here: http://www.briancowan.net/unity/fx
    ///
    ///   usage:
    ///   Attach this script to an empty GameObject. It will look for the "Main Camera"
    ///   GameObject to automatically adjust the Camera to the rendered Meshes.
    ///
    ///   This script is placed in public domain. The author takes no responsibility for any possible harm.
     
    using System;
    using System.Collections;
    using UnityEngine;
     
    public class MarchingSquares : MonoBehaviour {
     
    	#region Unity Singleton Pattern
    	private static MarchingSquares instance = null;
     
    	public static MarchingSquares Instance {
    		get { return instance; }
    	}
     
    	public void Awake() {
    		if (instance != null && instance != this) {
    			Destroy(this.gameObject);
    			return;
    		} else {
    			instance = this;
    		}
    		DontDestroyOnLoad(this.gameObject);
    	}
    	#endregion Unity Singleton Pattern
     
    	#region helper classes
     
    	///  A Triangle in 2D
    	///
    	public class Triangle {
    		// the triangles vertices
    		public Vector2[] p;
    		// saves "outside" lines index positions
    		public int[] outerline;
    		// constructor
    		public Triangle() {
    			p = new Vector2[3];
    			outerline = new int[2];
    			outerline[0] = -1;
    			outerline[1] = -1;
    		}
    	}
     
    	///  A Square has four Vertices. (p)
    	///  For each Vertice there is a float elem [0,1] (see val[] array)
    	///  2     3
    	///  +-----+
    	///  |     |
    	///  |     |
    	///  +-----+
    	///  0     1
    	///
    	public class SquareCell {
    		public Vector2[] p;
    		public float[] val;
     
    		public SquareCell() {
    			p = new Vector2[4];
    			val = new float[4];
    		}
    	}
    	#endregion helper classes
     
    	// render mesh arrays
    	ArrayList vert;
    	ArrayList uv;
    	ArrayList tri;
    	ArrayList norm;
    	// collider mesh arrays
    	ArrayList cvert;
    	ArrayList cuv;
    	ArrayList ctri;
    	ArrayList cnorm;
     
    	/// Linearly interpolate the position where an isosurface cuts
    	/// an edge between two vertices, each with their own scalar value
    	///
    	Vector2 VertexInterp(float isolevel, ref SquareCell cell, int pid1, int pid2) {
    		Vector2 p1 = cell.p[pid1];
    		Vector2 p2 = cell.p[pid2];
    		float valp1 = cell.val[pid1];
    		float valp2 = cell.val[pid2];
     
    		float mu;
    		Vector2 p = Vector2.zero;
     
    		if (Math.Abs(isolevel-valp1) < 0.00001)
    			return(p1);
    		if (Math.Abs(isolevel-valp2) < 0.00001)
    			return(p2);
    		if (Math.Abs(valp1-valp2) < 0.00001)
    			return(p1);
     
    		mu = (isolevel - valp1) / (valp2 - valp1);
    		p.x = p1.x + mu * (p2.x - p1.x);
    		p.y = p1.y + mu * (p2.y - p1.y);
    		return(p);
    	}
     
    	/// All cases
    	///
    	/// Case 0   Case 1   Case 2   Case 3   Case 4   Case 5   Case 6   Case 7
    	/// O-----O  O-----O  O-----O  O-----O  O-----#  O-----#  O-----#  O-----#
    	/// |     |  |     |  |     |  |     |  |    \|  |    \|  |  |  |  |/    |
    	/// |     |  |\    |  |    /|  |-----|  |     |  |\    |  |  |  |  |     |
    	/// O-----O  #-----O  O-----#  #-----#  O-----O  #-----O  O-----#  #-----#
    	///
    	/// Case 8   Case 9   Case 10  Case 11  Case 12  Case 13  Case 14  Case 15
    	/// #-----O  #-----O  #-----O  #-----O  #-----#  #-----#  #-----#  #-----#
    	/// |/    |  |  |  |  |/    |  |    \|  |-----|  |     |  |     |  |     |
    	/// |     |  |  |  |  |    /|  |     |  |     |  |    /|  |\    |  |     |
    	/// O-----O  #-----O  O-----#  #-----#  O-----O  #-----O  O-----#  #-----#
    	///
    	private int Polygonise(SquareCell cell, out Triangle[] triangles, float isoLevel) {
     
    		triangles = new Triangle[3]; // => Max 3 Triangles needed
     
    		// decide which case we have
     
    //		bool case_0  = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
    		bool case_1  = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
    		bool case_2  = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
    		bool case_3  = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] <  isoLevel;
    		bool case_4  = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
    		bool case_5  = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
    		bool case_6  = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
    		bool case_7  = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] <  isoLevel && cell.val[3] >= isoLevel;
    		bool case_8  = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
    		bool case_9  = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
    		bool case_10 = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
    		bool case_11 = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] <  isoLevel;
    		bool case_12 = cell.val[0] <  isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
    		bool case_13 = cell.val[0] >= isoLevel && cell.val[1] <  isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
    		bool case_14 = cell.val[0] <  isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
    		bool case_15 = cell.val[0] >= isoLevel && cell.val[1] >= isoLevel && cell.val[2] >= isoLevel && cell.val[3] >= isoLevel;
     
    		// make triangles
    		int ntriang = 0;
    		if (case_1) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 0);
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[0].p[2] = cell.p[0];
    			triangles[0].outerline[0] = 0;
    			triangles[0].outerline[1] = 1;
    			ntriang++;
    		}
    		if (case_2) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[0].p[2] = cell.p[1];
    			triangles[0].outerline[0] = 0;
    			triangles[0].outerline[1] = 1;
    			ntriang++;
    		}
    		if (case_3) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[0].p[1] = cell.p[1];
    			triangles[0].p[2] = cell.p[0];
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[1].p[2] = cell.p[1];
    			triangles[1].outerline[0] = 0;
    			triangles[1].outerline[1] = 1;
    			ntriang++;
    		}
    		if (case_4) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[0].p[2] = cell.p[3];
    			triangles[0].outerline[0] = 0;
    			triangles[0].outerline[1] = 1;
    			ntriang++;
    		}
    		if (case_5) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[0].p[2] = cell.p[3];
    			triangles[0].outerline[0] = 0;
    			triangles[0].outerline[1] = 1;
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = cell.p[0];
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[1].outerline[0] = 1;
    			triangles[1].outerline[1] = 2;
    			ntriang++;
    		}
    		if (case_6) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[0].p[1] = cell.p[3];
    			triangles[0].p[2] = cell.p[1];
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[1].p[2] = cell.p[1];
    			triangles[1].outerline[0] = 0;
    			triangles[1].outerline[1] = 1;
    			ntriang++;
    		}
    		if (case_7) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[0].p[1] = cell.p[3];
    			triangles[0].p[2] = cell.p[1];
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[1].p[2] = cell.p[1];
    			triangles[1].outerline[0] = 0;
    			triangles[1].outerline[1] = 1;
    			ntriang++;
     
    			triangles[2] = new Triangle();
    			triangles[2].p[0] = cell.p[0];
    			triangles[2].p[1] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[2].p[2] = cell.p[1];
    			// no outer line...
    			ntriang++;
    		}
    		if (case_8) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[0].p[2] = cell.p[2];
    			triangles[0].outerline[0] = 0;
    			triangles[0].outerline[1] = 1;
    			ntriang++;
    		}
    		if (case_9) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = cell.p[0];
    			triangles[0].p[1] = cell.p[2];
    			triangles[0].p[2] = VertexInterp(isoLevel, ref cell, 0, 1);
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = cell.p[2];
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[1].outerline[0] = 1;
    			triangles[1].outerline[1] = 2;
    			ntriang++;
    		}
    		if (case_10) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = cell.p[2];
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[0].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[0].outerline[0] = 1;
    			triangles[0].outerline[1] = 2;
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[1].p[2] = cell.p[1];
    			triangles[1].outerline[0] = 0;
    			triangles[1].outerline[1] = 1;
    			ntriang++;
    		}
    		if (case_11) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = cell.p[0];
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[0].p[2] = cell.p[1];
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[1].p[2] = cell.p[0];
    			triangles[1].outerline[0] = 0;
    			triangles[1].outerline[1] = 1;
    			ntriang++;
     
    			triangles[2] = new Triangle();
    			triangles[2].p[0] = cell.p[2];
    			triangles[2].p[1] = VertexInterp(isoLevel, ref cell, 2, 3);
    			triangles[2].p[2] = cell.p[0];
    			// no outer line...
    			ntriang++;
    		}
    		if (case_12) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = cell.p[2];
    			triangles[0].p[1] = cell.p[3];
    			triangles[0].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = cell.p[3];
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[1].outerline[0] = 1;
    			triangles[1].outerline[1] = 2;
    			ntriang++;
    		}
    		if (case_13) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[0].p[1] = cell.p[0];
    			triangles[0].p[2] = cell.p[2];
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[1].p[2] = cell.p[2];
    			triangles[1].outerline[0] = 0;
    			triangles[1].outerline[1] = 1;
    			ntriang++;
     
    			triangles[2] = new Triangle();
    			triangles[2].p[0] = VertexInterp(isoLevel, ref cell, 1, 3);
    			triangles[2].p[1] = cell.p[2];
    			triangles[2].p[2] = cell.p[3];
    			// no outer line...
    			ntriang++;
    		}
    		if (case_14) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = cell.p[1];
    			triangles[0].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[0].p[2] = cell.p[3];
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = cell.p[3];
    			triangles[1].p[1] = VertexInterp(isoLevel, ref cell, 0, 1);
    			triangles[1].p[2] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[1].outerline[0] = 1;
    			triangles[1].outerline[1] = 2;
    			ntriang++;
     
    			triangles[2] = new Triangle();
    			triangles[2].p[0] = VertexInterp(isoLevel, ref cell, 0, 2);
    			triangles[2].p[1] = cell.p[2];
    			triangles[2].p[2] = cell.p[3];
    			// no outer line...
    			ntriang++;
    		}
    		if (case_15) {
    			triangles[0] = new Triangle();
    			triangles[0].p[0] = cell.p[2];
    			triangles[0].p[1] = cell.p[1];
    			triangles[0].p[2] = cell.p[0];
    			// no outer line...
    			ntriang++;
     
    			triangles[1] = new Triangle();
    			triangles[1].p[0] = cell.p[1];
    			triangles[1].p[1] = cell.p[2];
    			triangles[1].p[2] = cell.p[3];
    			// no outer line...
    			ntriang++;
    		}
     
    		return ntriang;
    	}
     
    	/// this method renders two meshes:
    	/// cmesh the collision mesh (just the outline vertices extruded)
    	/// rmesh the render mesh (the 2d surface of the given voxel lattices)
    	///
    	public void MarchSquares(out Mesh cmesh, out Mesh rmesh, ref SquareCell[,] cells, float isolevel) {
     
    		Vector2 uvScale = new Vector2(1.0f / cells.GetLength(0), 1.0f / cells.GetLength(1));
    		// triangle index counter
    		int tricount = 0;
    		// collider triangle index counter
    		int ctricount = 0;
    		// mesh data arrays - just clear when reused
    		if (vert == null) vert = new ArrayList(); else vert.Clear();
    		if (uv == null)   uv = new ArrayList();   else uv.Clear();
    		if (tri == null)  tri = new ArrayList();  else tri.Clear();
    		if (norm == null) norm = new ArrayList(); else norm.Clear();
    		// collider mesh arrays
    		if (cvert == null) cvert = new ArrayList(); else cvert.Clear();
    		if (cuv == null)   cuv = new ArrayList();   else cuv.Clear();
    		if (ctri == null)  ctri = new ArrayList();  else ctri.Clear();
    		if (cnorm == null) cnorm = new ArrayList(); else cnorm.Clear();
     
    		for (int i = 0; i < cells.GetLength(0); i++) {
    			for (int j = 0; j < cells.GetLength(1); j++) {
     
    				SquareCell cell = cells[i,j];
     
    				Triangle[] triangles;
    				Polygonise(cell, out triangles, isolevel);
     
    				for (int k = 0; k < triangles.Length; k++) {
    					Triangle triangle = triangles[k];
    					if (triangle != null) {
     
    						Vector3 p0 = new Vector3(triangle.p[0].x, 0, triangle.p[0].y);
    						Vector3 p1 = new Vector3(triangle.p[1].x, 0, triangle.p[1].y);
    						Vector3 p2 = new Vector3(triangle.p[2].x, 0, triangle.p[2].y);
     
    						/// Start Vertices One ---------------------------------------
    						vert.Add(p0);
    						vert.Add(p1);
    						vert.Add(p2);
    						// Triangles
    						tri.Add(tricount);
    						tri.Add(tricount+1);
    						tri.Add(tricount+2);
    						// Normals
    						Vector3 vn1 = p0 - p1; Vector3 vn2 = p0 - p2;
    						Vector3 n = Vector3.Normalize ( Vector3.Cross(vn1,vn2) );
    						norm.Add(n); norm.Add(n); norm.Add(n);
    						uv.Add(Vector2.Scale(new Vector2 (p0.x, p0.z), new Vector2(uvScale.x, uvScale.y)));
    						uv.Add(Vector2.Scale(new Vector2 (p1.x, p1.z), new Vector2(uvScale.x, uvScale.y)));
    						uv.Add(Vector2.Scale(new Vector2 (p2.x, p2.z), new Vector2(uvScale.x, uvScale.y)));
    						tricount += 3;
    						/// END Vertices One ---------------------------------------
     
    						if (triangle.outerline[0] != -1) {
    							Vector3 o1 = new Vector3(triangle.p[triangle.outerline[0]].x, 0, triangle.p[triangle.outerline[0]].y);
    							Vector3 o2 = new Vector3(triangle.p[triangle.outerline[1]].x, 0, triangle.p[triangle.outerline[1]].y);
    							Vector3 bo1 = o1; o1.y = -1; // o1 transposed one unit down
    							Vector3 bo2 = o2; o2.y = -1; // o2 transposed one unit down
    							/// Start Vertices Two ---------------------------------------
    							cvert.Add(o1);
    							cvert.Add(o2);
    							cvert.Add(bo1);
    							// Triangles
    							ctri.Add(ctricount);
    							ctri.Add(ctricount+1);
    							ctri.Add(ctricount+2);
    							// Normals
    							Vector3 ovn1 = o1 - o2; Vector3 ovn2 = o1 - bo1;
    							Vector3 on = Vector3.Normalize ( Vector3.Cross(ovn1,ovn2) );
    							cnorm.Add(on); cnorm.Add(on); cnorm.Add(on);
    							cuv.Add(Vector2.zero); cuv.Add(Vector2.zero); cuv.Add(Vector2.zero);
    							ctricount += 3;
    							/// END Vertices Two ---------------------------------------
     
    							/// Start Vertices Three ---------------------------------------
    							cvert.Add(bo2);
    							cvert.Add(bo1);
    							cvert.Add(o2);
    							// Triangles
    							ctri.Add(ctricount);
    							ctri.Add(ctricount+1);
    							ctri.Add(ctricount+2);
    							// Normals
    							Vector3 oovn1 = o2 - bo1; Vector3 oovn2 = o2 - bo2;
    							Vector3 oon = Vector3.Normalize ( Vector3.Cross(oovn1,oovn2) )*-1;
    							cnorm.Add(oon); cnorm.Add(oon); cnorm.Add(oon);
    							cuv.Add(Vector2.zero); cuv.Add(Vector2.zero); cuv.Add(Vector2.zero);
    							ctricount += 3;
    							/// END Vertices Three ---------------------------------------
    						}
    					}
    				}
    			}
    		}
     
    		// prepare the collision mesh
    		cmesh = new Mesh();
    		cmesh.vertices = (Vector3[]) cvert.ToArray(typeof(Vector3));
    		cmesh.uv = (Vector2[]) cuv.ToArray(typeof(Vector2));
    		cmesh.triangles = (int[]) ctri.ToArray(typeof(int));
    		cmesh.normals = (Vector3[]) cnorm.ToArray(typeof(Vector3));
     
    		// prepare the render mesh
    		rmesh = new Mesh();
    		rmesh.vertices = (Vector3[]) vert.ToArray(typeof(Vector3));
    		rmesh.uv = (Vector2[]) uv.ToArray(typeof(Vector2));
    		rmesh.triangles = (int[]) tri.ToArray(typeof(int));
    		rmesh.normals = (Vector3[]) norm.ToArray(typeof(Vector3));
    	}
     
     
     
     
     
     
     
     
    	//////////////////////////////////////////////////////////////////////////////////////////////////////////
    	/////////// A simple Example /////////////////////////////////////////////////////////////////////////////
    	/// //////////////////////////////////////////////////////////////////////////////////////////////////////
     
    	GameObject Testg;
    	GameObject TestRenderg;
    	SquareCell[,] cells;
     
    	public void Start() {
     
    		// grab the camera
    		GameObject cam = GameObject.Find("Main Camera");
    		cam.transform.position = new Vector3(3,5,-1);
    		cam.transform.rotation = Quaternion.Euler(60,0,0);
     
    		// some 2d voxel data
    		float[,] data = {{0,0,0,0,0,0},{0,1,1,1,1,0},{0,0,0,1,1,0},{0,0,0,0,1,0},{0,1,0,1,1,0},{0,1,1,0,1,0},{0,1,1,1,1,0},{0,0,0,0,0,0},};
    		// prepare cell data
    		cells = new SquareCell[data.GetLength(0)-1,data.GetLength(1)-1];
    		// put data in cells
    		for (int i = 0; i < data.GetLength(0); i++) {
    			for (int j = 0; j < data.GetLength(1); j++) {
    				// do not process the edges of the data array since cell.dim + 1 == data.dim
    				if (i < data.GetLength(0)-1 && j < data.GetLength(1)-1) {
    					SquareCell cell = new SquareCell();
    					cell.p[0] = new Vector2(i,j);
    					cell.p[1] = new Vector2(i+1,j);
    					cell.p[2] = new Vector2(i,j+1);
    					cell.p[3] = new Vector2(i+1,j+1);
     
    					cell.val[0] = data[i,j];
    					cell.val[1] = data[i+1,j];
    					cell.val[2] = data[i,j+1];
    					cell.val[3] = data[i+1,j+1];
     
    					cells[i,j] = cell;
    				}
    			}
    		}
     
    		// create a gameobject
    		Testg = new GameObject();
    		Testg.name = "msquare";
    		Testg.transform.position = Vector3.zero;
    		Testg.transform.rotation = Quaternion.identity;
    		// collision meshfilter
    		MeshFilter mf = (MeshFilter) Testg.AddComponent(typeof(MeshFilter));
    		// normally you don't want to render the collision mesh
    		MeshRenderer mr = (MeshRenderer) Testg.AddComponent(typeof(MeshRenderer));
    		mr.material.color = new Color(.5f,.6f,1f, 1f);
    		// collider rigidbody...
    		Testg.AddComponent(typeof(Rigidbody));
    		Testg.rigidbody.isKinematic = true;
    		Testg.rigidbody.useGravity = true;
    		Testg.rigidbody.mass = 10;
    		Testg.rigidbody.drag = 0.1f;
    		Testg.rigidbody.angularDrag = 0.4f;
    		MeshCollider mc = (MeshCollider) Testg.AddComponent(typeof(MeshCollider));
    		mc.sharedMesh = mf.mesh;
    		mc.convex = true;
    		// create texture sub gameobject
    		TestRenderg = new GameObject();
    		TestRenderg.transform.parent = Testg.transform;
    		TestRenderg.name = "msquare__rendermesh";
    		TestRenderg.transform.position = Vector3.zero;
    		TestRenderg.transform.rotation = Quaternion.identity;
    		TestRenderg.AddComponent(typeof(MeshFilter));
    		MeshRenderer cmr = (MeshRenderer)  TestRenderg.AddComponent(typeof(MeshRenderer));
    		cmr.material.color = new Color(1f,.6f,1f, 1f);
     
    	}
     
    	public void FixedUpdate() {
    		// render
    		// for the sake of simplicity we are rendering every frame.
    		// obviously you should only render when the data of the cells has changed
    		Mesh mesh, cmesh;
    		MarchSquares(out mesh, out cmesh, ref cells, 0.5f);
    		// update the render mesh
    		MeshFilter mf = (MeshFilter) Testg.GetComponent(typeof(MeshFilter));
    		mf.mesh.Clear();
    		mf.mesh.vertices = mesh.vertices;
    		mf.mesh.uv = mesh.uv;
    		mf.mesh.triangles = mesh.triangles;
    		mf.mesh.normals = mesh.normals;
    		Destroy(mesh);
    		// update the collision mesh
    		MeshFilter cmf = (MeshFilter) TestRenderg.GetComponent(typeof(MeshFilter));
    		cmf.mesh.Clear();
    		cmf.mesh.vertices = cmesh.vertices;
    		cmf.mesh.uv = cmesh.uv;
    		cmf.mesh.triangles = cmesh.triangles;
    		cmf.mesh.normals = cmesh.normals;
    		Destroy(cmesh);
    		MeshCollider mc = (MeshCollider) Testg.GetComponent(typeof(MeshCollider));
    		if (mc != null)
    			mc.sharedMesh = mf.mesh;
    	}
     
    }
}
