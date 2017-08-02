/*************************
 * Original url: http://wiki.unity3d.com/index.php/Ocean
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/Ocean.cs
 * File based on original modification date of: 1 November 2013, at 12:45. 
 *
 * Author: Donitz 
 *
 * Description 
 *   
 * Usage 
 *   
 * Ocean.cs 
 *   
 * Floater.cs 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.SimulationScripts
{
    DescriptionThe Ocean component simulates an endless scrolling ocean by morphing a water mesh centered on the camera. The water mesh reacts to wave emitters and the movement of the camera, giving it the illusion that the ocean is scrolling. Floater components can be used to provide simple buoyancy physics for boats and other objects. 
    UsageAttach the Ocean component to an object with a MeshFilter and Renderer. The mesh will be automatically created when the game starts. To make the water material scroll as the camera moves around you need to set the texture offset manually in the SetTextureOffset method. 
    Attach the Floaters to empty objects and add them to several points in an object with a RigidBody to give that point lift as it moves beneath the water surface. 
    Ocean.csusing UnityEngine;
    using System.Collections.Generic;
     
    /* -------------------------------------------------------------------------------
    	Ocean
     
    	This component is added to a GameObject with a MeshFilter and Renderer.
    	When the component is started it creates a conical ocean Mesh based on
    	the ViewDistance and TileSize. The ocean mesh is added to the MeshFilter.
     
    	An Ocean.WaveEmitter is an object which releases a sinus wave on the ocean
    	with a specific frequency, wavelength and amplitude.
     
    	The Ocean GameObject will always be centered on the FollowCamera. If the
    	FollowCamera moves under the Ocean height the Mesh normals are flipped to
    	provide an underwater view. The functions SetOverwater and SetUnderwater
    	are called when the FollowCamera switches between being over and underwater.
     
    	Since the Ocean GameObject remains centered, the Ocean textures needs to be
    	moved manually in the SetTextureOffset method to provide the illusion of movement.
     ------------------------------------------------------------------------------ */
    [RequireComponent(typeof(MeshFilter))]
    public class Ocean : MonoBehaviour {
     
    	[System.Serializable]
    	public class WaveEmitter {
     
    		public Vector2 Position;
    		public float Lifetime;
    		public float CurrentTime;
    		public float Frequency;
    		public float WaveLength;
    		public float Amplitude;
    		public float DecayDistance;
    		private float speed;
    		private float frequencyRadians;
    		private float minRadiusSquared;
    		private float maxRadiusSquared;
     
    		public float MaxRadius { get; private set; }
     
    		public bool HasWaves { get; private set; }
     
    		public void Update(float deltaTime) {
    			CurrentTime += deltaTime;
    			speed = WaveLength * Frequency;
    			frequencyRadians = Frequency * Mathf.PI * 2;
    			minRadiusSquared = Mathf.Pow(Mathf.Max(0, (CurrentTime - Lifetime) * speed), 2);
    			maxRadiusSquared = Mathf.Pow(Mathf.Min(DecayDistance, CurrentTime * speed), 2);
    			MaxRadius = Mathf.Sqrt(maxRadiusSquared);
    			HasWaves = minRadiusSquared < maxRadiusSquared;
    		}
     
    		public float GetWaveHeight(float x, float y) {
    			float dx = x - Position.x;
    			float dy = y - Position.y;
    			float distanceSquared = dx * dx + dy * dy;
    			if (distanceSquared < minRadiusSquared || distanceSquared > maxRadiusSquared)
    				return 0;
    			float distance = Mathf.Sqrt(distanceSquared);
    			float localTime = CurrentTime - distance / speed;
    			return Mathf.Sin(localTime * frequencyRadians) * Amplitude
    				* (1 - (distance / DecayDistance)) * (1 - (localTime / Lifetime));
    		}
     
    	}
     
    	private const float NormalTriangleSize = 0.2f;
    	public float TileSize;
    	public int ViewDistance;
    	public List<WaveEmitter> WaveEmitters;
    	public Transform FollowCamera;
    	private int[,] vertexGrid;
    	private float gridCenter;
    	private int[,] gridEdges;
    	private Mesh mesh;
    	private bool isUnderwater;
     
    	void Start() {
    		// Create a 2D grid of vertices contained within the ViewDistance
    		int size = (int)Mathf.Ceil((ViewDistance / TileSize) * 2);
    		gridCenter = (size - 1) * TileSize / 2;
    		vertexGrid = new int[size, size];
    		for (int x = 0; x < size; x++)
    			for (int z = 0; z < size; z++)
    				vertexGrid[x, z] = Mathf.Sqrt(Mathf.Pow(x * TileSize - gridCenter, 2) +
    					Mathf.Pow(z * TileSize - gridCenter, 2)) < ViewDistance ? 0 : -1;
     
    		// Create a list of vertices and uv-coordinates from the grid
    		gridEdges = new int[size, 2];
    		List<Vector3> vertices = new List<Vector3> { new Vector3(0, -ViewDistance * 2, 0) };
    		List<Vector2> uvCoords = new List<Vector2> { new Vector2(0, 0) };
    		for (int z = 0; z < size; z++) {
    			gridEdges[z, 0] = size - 1;
    			gridEdges[z, 1] = 0;
    			for (int x = 0; x < size; x++) {
    				if (vertexGrid[x, z] > -1) {
    					if (x == 0 || vertexGrid[x - 1, z] == -1 || x == size - 1 || vertexGrid[x + 1, z] == -1 ||
    						z == 0 || vertexGrid[x, z - 1] == -1 || z == size - 1 || vertexGrid[x, z + 1] == -1) {
    						if (x == 0 || vertexGrid[x - 1, z] == -1)
    							gridEdges[z, 0] = x;
    						else if (x == size - 1 || vertexGrid[x + 1, z] == -1)
    							gridEdges[z, 1] = x;
    						continue;
    					} else
    						vertexGrid[x, z] = vertices.Count;
    					Vector3 v = new Vector3(x * TileSize - gridCenter, 0, z * TileSize - gridCenter);
    					vertices.Add(v);
    					uvCoords.Add(new Vector2(v.x, v.z));
    				}
    			}
    		}
     
    		// Create a list of triangles from the grid
    		List<int> indices = new List<int>();
    		for (int x = 0; x < size - 1; x++)
    			for (int z = 0; z < size - 1; z++)
    				if (vertexGrid[x, z] > -1 && vertexGrid[x + 1, z] > -1 &&
    					vertexGrid[x, z + 1] > -1 && vertexGrid[x + 1, z + 1] > -1) {
    					indices.Add(vertexGrid[x, z]);
    					indices.Add(vertexGrid[x + 1, z + 1]);
    					indices.Add(vertexGrid[x + 1, z]);
    					indices.Add(vertexGrid[x, z]);
    					indices.Add(vertexGrid[x, z + 1]);
    					indices.Add(vertexGrid[x + 1, z + 1]);
    				}
     
    		mesh = new Mesh();
    		mesh.vertices = vertices.ToArray();
    		mesh.uv = uvCoords.ToArray();
    		mesh.triangles = indices.ToArray();
    		mesh.RecalculateBounds();
     
    		GetComponent<MeshFilter>().mesh = mesh;
    	}
     
    	void Update() {
    		// Keep the Ocean centered on the FollowCamera
    		transform.position = new Vector3(FollowCamera.position.x, 0, FollowCamera.position.z);
     
    		// Reset waves
    		Vector3[] vertices = mesh.vertices;
    		for (int i = 1; i < vertices.Length; i++)
    			vertices[i].y = 0;
     
    		for (int i = WaveEmitters.Count - 1; i > -1; i--) {
    			WaveEmitter e = WaveEmitters[i];
     
    			e.Update(Time.deltaTime);
    			if (!e.HasWaves)
    				WaveEmitters.RemoveAt(i);
     
    			// Is the WaveEmitter outside the ViewDistance?
    			if (Mathf.Sqrt(Mathf.Pow(e.Position.x - transform.position.x, 2) +
    				Mathf.Pow(e.Position.y - transform.position.z, 2)) > e.MaxRadius + ViewDistance)
    				continue;
     
    			// Loop through each vertex in the two intersecting circles formed from the grid and emitter MaxRadius
    			float offsetX = e.Position.x - transform.position.x;
    			float offsetY = e.Position.y - transform.position.z;
    			int zMin = Mathf.Max(0, (int)((offsetY - e.MaxRadius + gridCenter) / TileSize));
    			int zMax = Mathf.Min(vertexGrid.GetLength(0) - 1, (int)((offsetY + e.MaxRadius + gridCenter) / TileSize));
    			for (int z = zMin; z <= zMax; z++) {
    				float width = Mathf.Sqrt(Mathf.Pow(e.MaxRadius, 2) - Mathf.Pow(-offsetY + z * TileSize - gridCenter, 2));
    				int xMin = Mathf.Max(gridEdges[z, 0], (int)((offsetX - width + gridCenter) / TileSize));
    				int xMax = Mathf.Min(gridEdges[z, 1], (int)((offsetX + width + gridCenter) / TileSize));
    				for (int x = xMin; x <= xMax; x++) {
    					int index = vertexGrid[x, z];
    					if (index > 0)
    						vertices[index].y += e.GetWaveHeight(vertices[index].x + transform.position.x, vertices[index].z + transform.position.z);
    				}
    			}
    		}
    		mesh.vertices = vertices;
     
    		// Flip surface normals if the FollowCamera moves through the water plane
    		bool oldUnderwater = isUnderwater;
    		isUnderwater = GetHeightAtLocation(transform.position.x, transform.position.y) > FollowCamera.position.y;
    		if (isUnderwater != oldUnderwater) {
    			for (int m = 0; m < mesh.subMeshCount; m++) {
    				int[] triangles = mesh.GetTriangles(m);
    				for (int i = 0; i < triangles.Length; i+=3) {
    					int temp = triangles[i + 0];
    					triangles[i + 0] = triangles[i + 1];
    					triangles[i + 1] = temp;
    				}
    				mesh.SetTriangles(triangles, m);
    			}
    			if (isUnderwater)
    				SetUnderwater();
    			else
    				SetOverwater();
    		}
    		mesh.RecalculateNormals();
     
    		SetTextureOffset(new Vector2(transform.position.x, transform.position.z));
    	}
     
    	public float GetHeightAtLocation(float x, float z) {
    		float height = 0;
    		foreach (WaveEmitter emitter in WaveEmitters)
    			height += emitter.GetWaveHeight(x, z);
    		return height;
    	}
     
    	public Vector3 GetNormalAtLocation(float x, float z) {
    		Vector3 a = new Vector3(x, GetHeightAtLocation(x, z + NormalTriangleSize), z + NormalTriangleSize);
    		Vector3 b = new Vector3(x + NormalTriangleSize, GetHeightAtLocation(x + NormalTriangleSize, z - NormalTriangleSize), z - NormalTriangleSize);
    		Vector3 c = new Vector3(x - NormalTriangleSize, GetHeightAtLocation(x - NormalTriangleSize, z - NormalTriangleSize), z - NormalTriangleSize);
    		Vector3 dir = Vector3.Cross(b - a, c - a);
    		return dir / dir.magnitude;
    	}
     
    	public void SetTextureOffset(Vector2 offset) {
    		// TODO: Set the texture offset in the material
    	}
     
    	public void SetOverwater() {
    		// TODO: Set overwater effects
    	}
     
    	public void SetUnderwater() {
    		// TODO: Set underwater effects
    	}
     
    }Floater.csusing UnityEngine;
    using System.Collections;
     
    /* -------------------------------------------------------------------------------
    	Floater
     
    	This component works with the Ocean component to make a RigidBody float.
    	When this GameObject moves beneath the Ocean surface it pushes the
    	referenced RigidBody towards the Ocean Normal with an acceleration
    	based on the depth.
     
    	Tip: Use multiple Floaters spread far apart to make a RigidBody stable.
     ------------------------------------------------------------------------------ */
    public class Floater : MonoBehaviour {
     
    	public float LiftAcceleration = 1;
    	public float MaxDistance = 3;
    	public Rigidbody Body;
    	private Ocean ocean;
     
    	void Start() {
    		ocean = (Ocean)GameObject.FindObjectOfType(typeof(Ocean));
    	}
     
    	void FixedUpdate() {
    		Vector3 p = transform.position;
    		float waterHeight = ocean.GetHeightAtLocation(p.x, p.z);
    		Vector3 waterNormal = ocean.GetNormalAtLocation(p.x, p.z);
    		float forceFactor = Mathf.Clamp(1f - (p.y - waterHeight) / MaxDistance, 0, 1);
    		transform.parent.rigidbody.AddForceAtPosition(waterNormal * forceFactor * LiftAcceleration * Body.mass, p);
     
    		if (!Debug.isDebugBuild)
    			return;
    		Debug.DrawLine(p, p + waterNormal * forceFactor * 5, Color.green);
    		Debug.DrawLine(p + waterNormal * forceFactor * 5, p + waterNormal * 5, Color.red);
    	}
     
}
}
