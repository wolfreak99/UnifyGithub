/*************************
 * Original url: http://wiki.unity3d.com/index.php/CreatePlane
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CreatePlane.cs
 * File based on original modification date of: 21 June 2015, at 10:00. 
 *
 * Author: Michael Garforth.
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Extended by Juan Manuel Palacios to include the Anchor parameter for diverse plane pivoting. Extended by Frogsbo to make hexagon lattice planes. Extended by Jason MacRae to add the two-sided plane option. 
    Contents [hide] 
    1 Description 
    2 Usage 
    3 C# - CreatePlane.cs 
    4 C# - CreateHexLattice.cs 
    
    DescriptionThis editor script creates a plane with the specified orientation, pivot point, number of segments for width & length, among others. 
    There is a second create plane script to make hexagon planes: 
    /\/\/\ 
    \/\/\/
    /\/\/\
    UsagePlace this script as "CreatePlane.cs" in YourProject/Assets/Editor and a menu item will automatically appear in the "GameObject/Create Other" menu after it is compiled. 
    Width Segments is the number of quads the plane will have in the X direction.
    Length Segments is the number of quads the plane will have in the Z direction (or Y direction, for a vertically-oriented plane).
    Width is the number of world units wide the plane will be. (Generally it's best to create objects at the correct scale natively, rather than scaling them afterward.)
    Length is the number of world units long the plane will be (or how high, for a vertically-oriented plane).
    Orientation is either Horizontal (flat) or Vertical (standing up), which can avoid having to rotate the plane afterward.
    Anchor is the mesh point in the plane that will act as its pivot.
    Add Collider creates a box collider for the plane if checked.
    Two Sided creates a two sided plane. Can be useful for collision detection and for very thin objects that need to render both sides.
    Create At Origin creates the plane at the origin (0,0,0) if checked; otherwise it's created a little ways in front of the editor camera. 
    It will create a mesh asset named either "Plane" or "OptionalName" in the "YourProject/Assets/Editor" folder. Appended to the name is the number of segments wide and long, plus the width, length and orientation. 
    C# - CreatePlane.csusing UnityEngine;
    using UnityEditor;
    using System.Collections;
     
     
    public class CreatePlane : ScriptableWizard
    {
     
        public enum Orientation
        {
            Horizontal,
            Vertical
        }
     
        public enum AnchorPoint
        {
            TopLeft,
            TopHalf,
            TopRight,
            RightHalf,
            BottomRight,
            BottomHalf,
            BottomLeft,
            LeftHalf,
            Center
        }
     
        public int widthSegments = 1;
        public int lengthSegments = 1;
        public float width = 1.0f;
        public float length = 1.0f;
        public Orientation orientation = Orientation.Horizontal;
        public AnchorPoint anchor = AnchorPoint.Center;
        public bool addCollider = false;
        public bool createAtOrigin = true;
        public bool twoSided = false;
        public string optionalName;
     
        static Camera cam;
        static Camera lastUsedCam;
     
     
        [MenuItem("GameObject/Create Other/Custom Plane...")]
        static void CreateWizard()
        {
            cam = Camera.current;
            // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
            if (!cam)
                cam = lastUsedCam;
            else
                lastUsedCam = cam;
            ScriptableWizard.DisplayWizard("Create Plane",typeof(CreatePlane));
        }
     
     
        void OnWizardUpdate()
        {
            widthSegments = Mathf.Clamp(widthSegments, 1, 254);
            lengthSegments = Mathf.Clamp(lengthSegments, 1, 254);
        }
     
     
        void OnWizardCreate()
        {
            GameObject plane = new GameObject();
     
            if (!string.IsNullOrEmpty(optionalName))
                plane.name = optionalName;
            else
                plane.name = "Plane";
     
            if (!createAtOrigin && cam)
                plane.transform.position = cam.transform.position + cam.transform.forward*5.0f;
            else
                plane.transform.position = Vector3.zero;
     
    		Vector2 anchorOffset;
    		string anchorId;
    		switch (anchor)
    		{
    		case AnchorPoint.TopLeft:
    			anchorOffset = new Vector2(-width/2.0f,length/2.0f);
    			anchorId = "TL";
    			break;
    		case AnchorPoint.TopHalf:
    			anchorOffset = new Vector2(0.0f,length/2.0f);
    			anchorId = "TH";
    			break;
    		case AnchorPoint.TopRight:
    			anchorOffset = new Vector2(width/2.0f,length/2.0f);
    			anchorId = "TR";
    			break;
    		case AnchorPoint.RightHalf:
    			anchorOffset = new Vector2(width/2.0f,0.0f);
    			anchorId = "RH";
    			break;
    		case AnchorPoint.BottomRight:
    			anchorOffset = new Vector2(width/2.0f,-length/2.0f);
    			anchorId = "BR";
    			break;
    		case AnchorPoint.BottomHalf:
    			anchorOffset = new Vector2(0.0f,-length/2.0f);
    			anchorId = "BH";
    			break;
    		case AnchorPoint.BottomLeft:
    			anchorOffset = new Vector2(-width/2.0f,-length/2.0f);
    			anchorId = "BL";
    			break;			
    		case AnchorPoint.LeftHalf:
    			anchorOffset = new Vector2(-width/2.0f,0.0f);
    			anchorId = "LH";
    			break;			
    		case AnchorPoint.Center:
    		default:
    			anchorOffset = Vector2.zero;
    			anchorId = "C";
    			break;
    		}
     
            MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
            plane.AddComponent(typeof(MeshRenderer));
     
            string planeAssetName = plane.name + widthSegments + "x" + lengthSegments + "W" + width + "L" + length + (orientation == Orientation.Horizontal? "H" : "V") + anchorId + ".asset";
            Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + planeAssetName,typeof(Mesh));
     
            if (m == null)
            {
                m = new Mesh();
                m.name = plane.name;
     
                int hCount2 = widthSegments+1;
                int vCount2 = lengthSegments+1;
                int numTriangles = widthSegments * lengthSegments * 6;
                if (twoSided) {
                    numTriangles *= 2;
                }
                int numVertices = hCount2 * vCount2;
     
                Vector3[] vertices = new Vector3[numVertices];
                Vector2[] uvs = new Vector2[numVertices];
                int[] triangles = new int[numTriangles];
                Vector4[] tangents = new Vector4[numVertices];
                Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
     
                int index = 0;
                float uvFactorX = 1.0f/widthSegments;
                float uvFactorY = 1.0f/lengthSegments;
                float scaleX = width/widthSegments;
                float scaleY = length/lengthSegments;
                for (float y = 0.0f; y < vCount2; y++)
                {
                    for (float x = 0.0f; x < hCount2; x++)
                    {
                        if (orientation == Orientation.Horizontal)
                        {
                            vertices[index] = new Vector3(x*scaleX - width/2f - anchorOffset.x, 0.0f, y*scaleY - length/2f - anchorOffset.y);
                        }
                        else
                        {
                            vertices[index] = new Vector3(x*scaleX - width/2f - anchorOffset.x, y*scaleY - length/2f - anchorOffset.y, 0.0f);
                        }
                        tangents[index] = tangent;
                        uvs[index++] = new Vector2(x*uvFactorX, y*uvFactorY);
                    }
                }
     
                index = 0;
                for (int y = 0; y < lengthSegments; y++)
                {
                    for (int x = 0; x < widthSegments; x++)
                    {
                        triangles[index]   = (y     * hCount2) + x;
                        triangles[index+1] = ((y+1) * hCount2) + x;
                        triangles[index+2] = (y     * hCount2) + x + 1;
     
                        triangles[index+3] = ((y+1) * hCount2) + x;
                        triangles[index+4] = ((y+1) * hCount2) + x + 1;
                        triangles[index+5] = (y     * hCount2) + x + 1;
                        index += 6;
                    }
                    if (twoSided) {
                        // Same tri vertices with order reversed, so normals point in the opposite direction
                        for (int x = 0; x < widthSegments; x++)
                        {
                            triangles[index]   = (y     * hCount2) + x;
                            triangles[index+1] = (y     * hCount2) + x + 1;
                            triangles[index+2] = ((y+1) * hCount2) + x;
     
                            triangles[index+3] = ((y+1) * hCount2) + x;
                            triangles[index+4] = (y     * hCount2) + x + 1;
                            triangles[index+5] = ((y+1) * hCount2) + x + 1;
                            index += 6;
                        }
                    }
                }
     
                m.vertices = vertices;
                m.uv = uvs;
                m.triangles = triangles;
                m.tangents = tangents;
                m.RecalculateNormals();
     
                AssetDatabase.CreateAsset(m, "Assets/Editor/" + planeAssetName);
                AssetDatabase.SaveAssets();
            }
     
            meshFilter.sharedMesh = m;
            m.RecalculateBounds();
     
            if (addCollider)
                plane.AddComponent(typeof(BoxCollider));
     
            Selection.activeObject = plane;
        }
    }
    
    HEXAGON VERSION: 
    C# - CreateHexLattice.cs	/*
    	Unify wiki script, it's included for conveniently making a HexLattice of any detail level
    	Author: Michael Garforth.
    	Extended by Juan Manuel Palacios
    	Extended by Frogsbo, to make hexagon lattice.
    	/\/\/\
    	\/\/\/ 
    	/\/\/\ Isoceles triangle height is sqrt(1^2 - .5^2), 
    	sqrt(.75), 0.8660254037844386.  ~ 100/86.6
    	so to have a square HexLattice plane, use rows/columns like 50/43 and 32/28 38/33 35/30
    	(see table addendum)
    	*/
     
     
     
    	using UnityEngine;
    	using UnityEditor;
    	using System.Collections;
     
     
    	public class CreateHexLattice : ScriptableWizard
    	{
     
    		public enum Orientation
    		{
    			Horizontal,
    			Vertical
    		}
     
    		public enum AnchorPoint
    		{
    			TopLeft,
    			TopHalf,
    			TopRight,
    			RightHalf,
    			BottomRight,
    			BottomHalf,
    			BottomLeft,
    			LeftHalf,
    			Center
    		}
     
    		public int widthSegments = 1;
    		public int lengthSegments = 1;
    		public float width = 1.0f;
    		public float length = 1.0f;
    		public Orientation orientation = Orientation.Horizontal;
    		public AnchorPoint anchor = AnchorPoint.Center;
    		public bool addCollider = false;
    		public bool createAtOrigin = true;
    		public string optionalName;
     
    		static Camera cam;
    		static Camera lastUsedCam;
     
     
    		[MenuItem("GameObject/Create Other/Custom HexLattice...")]
    		static void CreateWizard()
    		{
    			cam = Camera.current;
    			// Hack because camera.current doesn't return editor camera if scene view doesn't have focus
    			if (!cam)
    				cam = lastUsedCam;
    			else
    				lastUsedCam = cam;
    			ScriptableWizard.DisplayWizard("Create HexLattice",typeof(CreateHexLattice));
    		}
     
     
    		void OnWizardUpdate()
    		{
    			widthSegments = Mathf.Clamp(widthSegments, 1, 254);
    			lengthSegments = Mathf.Clamp(lengthSegments, 1, 254);
    		}
     
     
    		void OnWizardCreate()
    		{
    			GameObject HexLattice = new GameObject();
     
    			if (!string.IsNullOrEmpty(optionalName))
    				HexLattice.name = optionalName;
    			else
    				HexLattice.name = "HexLattice";
     
    			if (!createAtOrigin && cam)
    				HexLattice.transform.position = cam.transform.position + cam.transform.forward*5.0f;
    			else
    				HexLattice.transform.position = Vector3.zero;
     
    			Vector2 anchorOffset;
    			string anchorId;
    			switch (anchor)
    			{
    			case AnchorPoint.TopLeft:
    				anchorOffset = new Vector2(-width/2.0f,length/2.0f);
    				anchorId = "TL";
    				break;
    			case AnchorPoint.TopHalf:
    				anchorOffset = new Vector2(0.0f,length/2.0f);
    				anchorId = "TH";
    				break;
    			case AnchorPoint.TopRight:
    				anchorOffset = new Vector2(width/2.0f,length/2.0f);
    				anchorId = "TR";
    				break;
    			case AnchorPoint.RightHalf:
    				anchorOffset = new Vector2(width/2.0f,0.0f);
    				anchorId = "RH";
    				break;
    			case AnchorPoint.BottomRight:
    				anchorOffset = new Vector2(width/2.0f,-length/2.0f);
    				anchorId = "BR";
    				break;
    			case AnchorPoint.BottomHalf:
    				anchorOffset = new Vector2(0.0f,-length/2.0f);
    				anchorId = "BH";
    				break;
    			case AnchorPoint.BottomLeft:
    				anchorOffset = new Vector2(-width/2.0f,-length/2.0f);
    				anchorId = "BL";
    				break;			
    			case AnchorPoint.LeftHalf:
    				anchorOffset = new Vector2(-width/2.0f,0.0f);
    				anchorId = "LH";
    				break;			
    			case AnchorPoint.Center:
    			default:
    				anchorOffset = Vector2.zero;
    				anchorId = "C";
    				break;
    			}
     
    			MeshFilter meshFilter = (MeshFilter)HexLattice.AddComponent(typeof(MeshFilter));
    			HexLattice.AddComponent(typeof(MeshRenderer));
     
    			string HexLatticeAssetName = HexLattice.name + widthSegments + "x" + lengthSegments + "W" + width + "L" + length + (orientation == Orientation.Horizontal? "H" : "V") + anchorId + ".asset";
    			Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + HexLatticeAssetName,typeof(Mesh));
     
    			if (m == null)
    			{
    				m = new Mesh();
    				m.name = HexLattice.name;
     
    				int hCount2 = widthSegments+1;
    				int vCount2 = lengthSegments+1;
    				int numTriangles = widthSegments * lengthSegments * 6;
    				int numVertices = hCount2 * vCount2;
     
    				Vector3[] vertices = new Vector3[numVertices];
    				Vector2[] uvs = new Vector2[numVertices];
    				int[] triangles = new int[numTriangles];
     
    				int index = 0;
    				float uvFactorX = 1.0f/widthSegments;
    				float uvFactorY = 1.0f/lengthSegments;
    				float scaleX = width/widthSegments;
    				float scaleY = length/lengthSegments;
    				for (float y = 0.0f; y < vCount2; y++)
    				{
    					for (float x = 0.0f; x < hCount2; x++)
    					{
    						if (orientation == Orientation.Horizontal)
    						{
    							vertices[index] = new Vector3(x*scaleX - width/2f - anchorOffset.x -(y%2-1)*scaleX*.5f, 0.0f, y*scaleY - length/2f - anchorOffset.y);
    						}
    						else
    						{
    							vertices[index] = new Vector3(x*scaleX - width/2f - anchorOffset.x -(y%2-1)*scaleX*.5f, y*scaleY - length/2f - anchorOffset.y, 0.0f);
    						}
    						uvs[index++] = new Vector2(x*uvFactorX, y*uvFactorY);
    					}
    				}
     
    				index = 0;
    				for (int y = 0; y < lengthSegments; y++)
    				{
    					if (y%2 == 0)
    					{
    						for (int x = 0; x < widthSegments; x++)
    						{
    							triangles[index]   = (y     * hCount2) + x;
    							triangles[index+1] = ((y+1) * hCount2) + x;
    							triangles[index+2] = ((y+1) * hCount2) + x + 1;
     
     
    							triangles[index+3] = (y     * hCount2) + x;
    							triangles[index+4] = ((y+1) * hCount2) + x + 1;
    							triangles[index+5] = (y     * hCount2) + x + 1;
    							index += 6;
    						}
    					}
    					else			
    					{
    						for (int x = 0; x < widthSegments; x++)
    						{
    							triangles[index]   = (y     * hCount2) + x;
    							triangles[index+1] = ((y+1) * hCount2) + x;
    							triangles[index+2] = (y     * hCount2) + x + 1;
     
    							triangles[index+3] = ((y+1) * hCount2) + x;
    							triangles[index+4] = ((y+1) * hCount2) + x + 1;
    							triangles[index+5] = (y     * hCount2) + x + 1;
    							index += 6;
    						}
    					}
    				}
     
    				m.vertices = vertices;
    				m.uv = uvs;
    				m.triangles = triangles;
    				m.RecalculateNormals();
    				m.Optimize();
     
    				AssetDatabase.CreateAsset(m, "Assets/Editor/" + HexLatticeAssetName);
    				AssetDatabase.SaveAssets();
    			}
     
    			meshFilter.sharedMesh = m;
    			m.RecalculateBounds();
     
    			if (addCollider)
    				HexLattice.AddComponent(typeof(BoxCollider));
     
    			Selection.activeObject = HexLattice;
    		}
    	}
    	/* LIST OF EXACT RATIOS OF LATTICE HEIGHT FOR GIVEN NUM OF CELLS:
    	1	 0,866
    	2	 1,732
    	3	 2,598
    	4	 3,464
    	5	 4,33
    	6	 5,196
    	7	 6,062------
    	8	 6,928-----
    	9	 7,794
    	10	 8,66
    	11	 9,526
    	12	 10,392
    	13	 11,258
    	14	 12,124---
    	15	 12,99-------------
    	16	 13,856
    	17	 14,722
    	18	 15,588
    	19	 16,454
    	20	 17,32
    	21	 18,186
    	22	 19,052-----------
    	23	 19,918-----------
    	24	 20,784
    	25	 21,65
    	26	 22,51
    	27	 23,382
    	28	 24,248
    	29	 25,114
    	30	 25,98------------
    	31	 26,846
    	32	 27,712
    	33	 28,578
    	34	 29,444
    	35	 30,31
    	36	 31,176
    	37	 32,042-----------
    	38	 32,908----------
    	39	 33,774
    	40	 34,64
    	41	 35,506
    	42	 36,372
    	43	 37,238
    	44	 38,104
    	45	 38,97-----------
    	46	 39,836
    	47	 40,702
    	48	 41,568
    	49	 42,434
    	50	 43,3
    	51	 44,166
    	52	 45,032----------
    	53	 45,898----------
    	54	 46,764
    	55	 47,63
    	56	 48,496
    	57	 49,362
    	58	 50,228
    	59	 51,094---------
    	60	 51,96---------
    	61	 52,826
    	62	 53,692
    	63	 54,558
    	64	 55,424
    	65	 56,29
    	66	 57,156
    	67	 58,022-----------
    	68	 58,888
    	69	 59,754
	*/
}
