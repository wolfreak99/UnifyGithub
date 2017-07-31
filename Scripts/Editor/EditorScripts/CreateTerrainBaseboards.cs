// Original url: http://wiki.unity3d.com/index.php/CreateTerrainBaseboards
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/CreateTerrainBaseboards.cs
// File based on original modification date of: 24 January 2013, at 17:34. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{

Author: Max70.

DescriptionThis editor script creates a baseboards for a selected terrain inside the editor. 
UsagePlace this script as "CreateTerrainBaseboards.cs" in YourProject/Assets/Editor and a menu item will automatically appear in the "Terrain/Create Terrain baseboards" menu after it is compiled. 
Detail define the density of the mesh created. The created mesh density is proportional to 'TerainSize/detail'. 
Global H is the overall height of the baseboards created.
Select Color is the color of material created.
UVFactor is a parameter for setting the repetition of the texture.
Select Texture is a texture chosen for the material created applied to the baseboards. 
It will create four mesh asset, child of selected terrain. 
 
C# - CreateTerrainBaseboards.csusing UnityEngine;
using UnityEditor;
using System.Collections;
 
 
public class CreateTerrainBaseboards : ScriptableWizard {
 
	// PUBLIC DATA
	public float detail = 2.0f;
	public float globalH = -40.0f;
	public Color selectColor = Color.white;
	public float UVFactor = 8.0f;
	public Texture selectTexture;
 
	// PRIVATE DATA
	private GameObject objterrain=null;
	private Terrain terrain;
	private Vector3 posTerrain;
	private GameObject edgeG;
	private MeshFilter meshFilter;
	private Vector3[] vertices;
	private Vector2[] uvs;
	private int[] triangles;		
 
 
    [MenuItem("Terrain/Create Terrain Baseboards...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create TerrainBaseboards",typeof(CreateTerrainBaseboards));
    }
 
 
    void OnWizardCreate()
    {
		// selected terrain
       	objterrain = Selection.activeGameObject;
 
		// try to get terrain 
		if(objterrain==null)	{	terrain = Terrain.activeTerrain; }
		else 					{	terrain = objterrain.GetComponent<Terrain>(); }
 
		// position terrain
		posTerrain = terrain.transform.position;
 
		// init tab
		int sizeTab = (int)( terrain.terrainData.size.x / detail )+1;
		float[] hh = new float[sizeTab];
 
		//-------------------------------------
		// new material for the four Baseboards
		Material newMat = new Material( Shader.Find("Diffuse"));
		newMat.color = selectColor;
		newMat.mainTexture = selectTexture;
		newMat.name = "BaseboardsMaterial";
 
 
		//**************************************************************************
		//****************** FIRST	BORDER
		//**************************************************************************		
		int id=0;
		Vector2 terPos = new Vector2(0,0.001f);
		for(float x=0; x<=terrain.terrainData.size.x; x+=detail)
		{	terPos.x = x / terrain.terrainData.size.x;
			hh[id++] = terrain.terrainData.GetInterpolatedHeight(terPos.x, terPos.y);
		}
		Mesh m = CreateEdgeObject("BBorderX0");
 
		int numVertices = sizeTab*2;
		int numTriangles = (sizeTab-1)*2*3;
		vertices = new Vector3[numVertices];
		uvs = new Vector2[numVertices];
		triangles = new int[numTriangles];		
		float uvFactor=1.0f/UVFactor;
 
		// VERTICES
		int index=0;
		for(int i=0; i<sizeTab; i++)
		{	float xx = i*detail;
			vertices[index] = new Vector3(xx,hh[i],0) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 1);
			vertices[index] = new Vector3(xx,globalH,0) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 0);
		}
		// TRIANGLES
		for(int i=0; i<numTriangles/3; i++)
		{	if( i-((i/2)*2)==0)
			{	//IMPAIR
				triangles[i*3]=i+1;
				triangles[i*3+1]=i;
				triangles[i*3+2]=i+2;				
			}else{ 
				//PAIR
				triangles[i*3]=i;
				triangles[i*3+1]=i+1;
				triangles[i*3+2]=i+2;
			}
		}
		FinalizeEdgeObject(m, newMat);
 
		//**************************************************************************
		//****************** SECOND BORDER
		//**************************************************************************		
		id=0;
		terPos.y = 0.9990f;
		for(float x=0; x<=terrain.terrainData.size.x; x+=detail)
		{	terPos.x = x / terrain.terrainData.size.x;
			hh[id++] = terrain.terrainData.GetInterpolatedHeight(terPos.x, terPos.y);
		}
		m = CreateEdgeObject("BBorderXZ");			
		// VERTICES
		index=0;
		for(int i=0; i<sizeTab; i++)
		{	float xx = i*detail;
			vertices[index] = new Vector3(xx,hh[i],terrain.terrainData.size.z) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 1);
			vertices[index] = new Vector3(xx,globalH,terrain.terrainData.size.z) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 0);
		}
		// TRIANGLES
		for(int i=0; i<numTriangles/3; i++)
		{	if( i-((i/2)*2)==0)
			{	//IMPAIR
				triangles[i*3]=i+1;
				triangles[i*3+1]=i+2;
				triangles[i*3+2]=i;				
			}else{ 
				//PAIR
				triangles[i*3]=i;
				triangles[i*3+1]=i+2;
				triangles[i*3+2]=i+1;
			}
		}
		FinalizeEdgeObject(m, newMat);
 
		//**************************************************************************
		//****************** THIRD BORDER
		//**************************************************************************		
		id=0;
		terPos.x = 0.001f;
		for(float x=0; x<=terrain.terrainData.size.z; x+=detail)
		{	terPos.y = x / terrain.terrainData.size.z;
			hh[id++] = terrain.terrainData.GetInterpolatedHeight(terPos.x, terPos.y);
		}		
		m = CreateEdgeObject("BBorderZ0");
		// VERTICES
		index=0;
		for(int i=0; i<sizeTab; i++)
		{	float xx = i*detail;
			vertices[index] = new Vector3(0,hh[i],xx) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 1);
			vertices[index] = new Vector3(0,globalH,xx) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 0);
		}
		// TRIANGLES
		for(int i=0; i<numTriangles/3; i++)
		{	if( i-((i/2)*2)==0)
			{	//IMPAIR
				triangles[i*3]=i+1;
				triangles[i*3+1]=i+2;
				triangles[i*3+2]=i;				
			}else{ 
				//PAIR
				triangles[i*3]=i;
				triangles[i*3+1]=i+2;
				triangles[i*3+2]=i+1;
			}
		}
		//......
		FinalizeEdgeObject(m, newMat);
 
		//**************************************************************************
		//****************** FOURTH BORDER
		//**************************************************************************		
		id=0;
		terPos.x = 0.999f;
		for(float x=0; x<=terrain.terrainData.size.z; x+=detail)
		{	terPos.y = x / terrain.terrainData.size.z;
			hh[id++] = terrain.terrainData.GetInterpolatedHeight(terPos.x, terPos.y);
		}
		m = CreateEdgeObject("BBorderZX");
 
		// VERTICES
		index=0;
		for(int i=0; i<sizeTab; i++)
		{	float xx = i*detail;
			vertices[index] = new Vector3(terrain.terrainData.size.x,hh[i],xx) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 1);
			vertices[index] = new Vector3(terrain.terrainData.size.x,globalH,xx) + posTerrain;
			uvs[index++] = new Vector2(i*uvFactor, 0);
		}
		// TRIANGLES
		for(int i=0; i<numTriangles/3; i++)
		{	if( i-((i/2)*2)==0)
			{	//IMPAIR
				triangles[i*3]=i+1;
				triangles[i*3+1]=i;
				triangles[i*3+2]=i+2;				
			}else{ 
				//PAIR
				triangles[i*3]=i;
				triangles[i*3+1]=i+1;
				triangles[i*3+2]=i+2;
			}
		}
		FinalizeEdgeObject(m, newMat);
 
	}
 
	//--------------------------------------
	// New Object for our mesh border
	Mesh CreateEdgeObject(string name)
	{
        edgeG = new GameObject();
		edgeG.name = name;		
		meshFilter = (MeshFilter)edgeG.AddComponent(typeof(MeshFilter));
		edgeG.AddComponent(typeof(MeshRenderer));		
		Mesh m = new Mesh();
        m.name = edgeG.name;	
		return m;
	}
	//--------------------------------------
	// finalize our border object
	void FinalizeEdgeObject(Mesh m, Material mat)
	{
		m.vertices = vertices;
		m.uv = uvs;
		m.triangles = triangles;
		m.RecalculateNormals();
 
		meshFilter.sharedMesh = m;
		m.RecalculateBounds();
		meshFilter.renderer.material = mat;
		edgeG.transform.parent = terrain.transform;	
		edgeG.isStatic = true;
	}
}
}
