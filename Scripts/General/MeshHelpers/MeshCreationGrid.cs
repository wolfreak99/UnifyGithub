// Original url: http://wiki.unity3d.com/index.php/MeshCreationGrid
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MeshHelpers/MeshCreationGrid.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
The below script generates a mesh for use in a tile based game. In order to utilize it simply attach it t a gameobject, populate the public variables, and hit play. You should be greeted with a grid of the size you chose with randomly changing tiles. 
Make sure that under the properties of your Texture you have FilterMode: Point and WrapMode: Clamp otherwise you will see artifacts as you move the map around. Also ensure that your texture is Power of 2 height and width.. 256x256, 512x512, 512x256, 1024x512 etc.. 
In order for this to actually be useful you will need to write the logic that calls UpdateGrid() elsewhere in your code to render your level.. And obviously turn off the code in Update() that randomly sets tiles. :) 
There is a unitypackage attached to the forum thread linked here[1]; it shows an example modification of the code below to load a simple level file from resource and draw it to the grid. 


using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
    public int TileWidth = 16;
    public int TileHeight = 16;
    public int NumTilesX = 16;
    public int NumTilesY = 16;
    public int TileGridWidth = 100;
    public int TileGridHeight = 100;
    public int DefaultTileX;
    public int DefaultTileY;
    public Texture2D Texture;
 
    void OnEnable()
    {
        CreatePlane(TileWidth, TileHeight, TileGridWidth, TileGridHeight);
    }
 
    void Update()
    {
        var tileColumn = Random.Range(0, NumTilesX);
        var tileRow = Random.Range(0, NumTilesY);
 
        var x = Random.Range(0, TileGridWidth);
        var y = Random.Range(0, TileGridHeight);
 
        UpdateGrid(new Vector2(x, y), new Vector2(tileColumn, tileRow), TileWidth, TileHeight, TileGridWidth);
    }
 
    public void UpdateGrid(Vector2 gridIndex, Vector2 tileIndex, int tileWidth, int tileHeight, int gridWidth)
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var uvs = mesh.uv;
 
        var tileSizeX = 1.0f / NumTilesX;
        var tileSizeY = 1.0f / NumTilesY;
 
        mesh.uv = uvs;
 
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 0] = new Vector2(tileIndex.x * tileSizeX, tileIndex.y * tileSizeY);
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 1] = new Vector2((tileIndex.x + 1) * tileSizeX, tileIndex.y * tileSizeY);
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 2] = new Vector2((tileIndex.x + 1) * tileSizeX, (tileIndex.y + 1) * tileSizeY);
        uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 3] = new Vector2(tileIndex.x * tileSizeX, (tileIndex.y + 1) * tileSizeY);
 
        mesh.uv = uvs;
    }
 
    void CreatePlane(int tileHeight, int tileWidth, int gridHeight, int gridWidth)
    {
        var mesh = new Mesh();
        var mf = GetComponent<MeshFilter>();
        mf.renderer.material.SetTexture("_MainTex", Texture);
        mf.mesh = mesh;
 
        var tileSizeX = 1.0f / NumTilesX;
        var tileSizeY = 1.0f / NumTilesY;
 
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
 
        var index = 0;
        for (var x = 0; x < gridWidth; x++) {
            for (var y = 0; y < gridHeight; y++) {
                AddVertices(tileHeight, tileWidth, y, x, vertices);
                index = AddTriangles(index, triangles);
                AddNormals(normals);
                AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
            }
        }
 
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }
 
    private static void AddVertices(int tileHeight, int tileWidth, int y, int x, ICollection<Vector3> vertices)
    {
        vertices.Add(new Vector3((x * tileWidth), (y * tileHeight), 0));
        vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight), 0));
        vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight) + tileHeight, 0));
        vertices.Add(new Vector3((x * tileWidth), (y * tileHeight) + tileHeight, 0));
    }
 
    private static int AddTriangles(int index, ICollection<int> triangles)
    {
        triangles.Add(index + 2);
        triangles.Add(index + 1);
        triangles.Add(index);
        triangles.Add(index);
        triangles.Add(index + 3);
        triangles.Add(index + 2);
        index += 4;
        return index;
    }
 
    private static void AddNormals(ICollection<Vector3> normals)
    {
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
    }
 
    private static void AddUvs(int tileRow, float tileSizeY, float tileSizeX, ICollection<Vector2> uvs, int tileColumn)
    {
        uvs.Add(new Vector2(tileColumn * tileSizeX, tileRow * tileSizeY));
        uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, tileRow * tileSizeY));
        uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, (tileRow + 1) * tileSizeY));
        uvs.Add(new Vector2(tileColumn * tileSizeX, (tileRow + 1) * tileSizeY));
    }
}
}
