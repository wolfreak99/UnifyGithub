// Original url: http://wiki.unity3d.com/index.php/MeshSerializer2
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/WWWScripts/MeshSerializer2.cs
// File based on original modification date of: 20 April 2015, at 19:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
Author: Aras Pranckevicius 
Contents [hide] 
1 Description 
2 Usage 
3 The package 
4 File format used 
5 Notes and ideas 
6 Changelog 
7 Source code 
7.1 JavaScript - MeshSerializer.js 
7.2 JavaScript - SaveMeshForWeb.js 
7.3 JavaScript - LoadMeshFromWeb.js 
8 Source code - C# version (slightly modified) 
8.1 C# - MeshSerializer.cs 

DescriptionA set of scrtipts for saving/loading meshes to a simple compact binary format. Possible use case is saving them to files, uploading them somewhere and then using WWW interface to download meshes dynamically. 
Of course since Unity 2.1 Asset Bundles provide a superset of this functionality (but they are only available in Unity Pro). This script itself is an improvement from MeshSerializer script, with 5x-6x reduction in file size. 
The package contains an example scene; where one mesh is saved to file and the other is downloaded. 
UsageMeshSerializer.js is an utility class to do the actual mesh serialization. Has comments inside. 
SaveMeshForWeb.js and LoadMeshFromWeb.js are example usage scripts. 
The packageZipped Unity package: Media:MeshSerializer2.zip (for Unity 2.x) 
File format usedNote that such a file format is pretty fragile to changes - it does not have any concept of "format version" built in. It's fully enough if you save and load meshes yourself (you know which format are you using), but if you want a generic mesh-loading-format, you'd have to implement something more complex. 
The file format used now: 
2 bytes vertex count 
2 bytes triangle count 
1 bytes vertex format (bits: 0=vertices, 1=normals, 2=tangents, 3=uvs) 
After that come vertex component arrays, each optional except for positions. Which ones are present depends on vertex format: 
Positions. 3D bounding box, followed by vertices encoded as two bytes per vertex component (so it's 6 bytes/vertex, plus 24 bytes for the bounding box). 
Normals. 3 bytes per normal. 
Tangents. 4 bytes per tangent. 
UVs. 2D bounding box, followed by UVs encoded as two bytes per UV component (so it's 4 bytes/UV, plus 16 bytes for the bounding box). 
Finally the triangle indices array: 6 bytes per triangle (3 two byte indices) 
Notes and ideasIf bump-mapping is not used, then storing tangents is a waste of space. By default tangents are not stored. 
There is a related article about saving unity meshes in .obj format: ObjExporter. 
Like mentioned above, this incorporates some of size reduction ideas from MeshSerializer article. Positions and UVs are encoded as 16 bit integers, quantized over the bounding box range. Normals and tangents are encoded at one byte per component, under assumption that components will always be in -1..1 range. 
The encoding is not exactly right, I think; proper way would be to round to integers instead of truncate. An exercise for the reader! 
Changelog2009 Feb 24 - Initial version. 
Source codeAll the source code is included in the provided Unity Package above. This is only for easy-browsing purposes. 
JavaScript - MeshSerializer.jsimport System.IO;
 
// A simple mesh saving/loading functionality.
// This is an utility script, you don't need to add it to any objects.
// See SaveMeshForWeb and LoadMeshFromWeb for example of use.
//
// Uses a custom binary format:
//
//    2 bytes vertex count
//    2 bytes triangle count
//    1 bytes vertex format (bits: 0=vertices, 1=normals, 2=tangents, 3=uvs)
//
//    After that come vertex component arrays, each optional except for positions.
//    Which ones are present depends on vertex format:
//        Positions
//            Bounding box is before the array (xmin,xmax,ymin,ymax,zmin,zmax)
//            Then each vertex component is 2 byte unsigned short, interpolated between the bound axis
//        Normals
//            One byte per component
//        Tangents
//            One byte per component
//        UVs (8 bytes/vertex - 2 floats)
//            Bounding box is before the array (xmin,xmax,ymin,ymax)
//            Then each UV component is 2 byte unsigned short, interpolated between the bound axis
//
//    Finally the triangle indices array: 6 bytes per triangle (3 unsigned short indices)
 
 
// Reads mesh from an array of bytes. Can return null
// if the bytes seem invalid.
static function ReadMesh( bytes : byte[] ) : Mesh
{
    if( !bytes || bytes.Length < 5 )
    {
        print( "Invalid mesh file!" );
        return null;
    }
 
    var buf = new BinaryReader( new MemoryStream( bytes ) );
 
    // read header
    var vertCount = buf.ReadUInt16();
    var triCount = buf.ReadUInt16();
    var format = buf.ReadByte();
 
    // sanity check
    if (vertCount < 0 || vertCount > 64000)
    {
        print ("Invalid vertex count in the mesh data!");
        return null;
    }
    if (triCount < 0 || triCount > 64000)
    {
        print ("Invalid triangle count in the mesh data!");
        return null;
    }
    if (format < 1 || (format&1) == 0 || format > 15)
    {
        print ("Invalid vertex format in the mesh data!");
        return null;
    }
 
    var mesh = new Mesh();
    var i : int;
 
    // positions
    var verts = new Vector3[vertCount];
    ReadVector3Array16bit (verts, buf);
    mesh.vertices = verts;
 
    if( format & 2 ) // have normals
    {
        var normals = new Vector3[vertCount];
        ReadVector3ArrayBytes (normals, buf);
        mesh.normals = normals;
    }
 
    if( format & 4 ) // have tangents
    {
        var tangents = new Vector4[vertCount];
        ReadVector4ArrayBytes (tangents, buf);
        mesh.tangents = tangents;
    }
 
    if( format & 8 ) // have UVs
    {
        var uvs = new Vector2[vertCount];
        ReadVector2Array16bit (uvs, buf);
        mesh.uv = uvs;
    }
 
    // triangle indices
    var tris = new int[triCount * 3];
    for( i = 0; i < triCount; ++i )
    {
        tris[i*3+0] = buf.ReadUInt16();
        tris[i*3+1] = buf.ReadUInt16();
        tris[i*3+2] = buf.ReadUInt16();
    }
    mesh.triangles = tris;
 
    buf.Close();
 
    return mesh;
}
 
static function ReadVector3Array16bit (arr : Vector3[], buf : BinaryReader) : void
{
    var n = arr.Length;
    if (n == 0)
        return;
 
    // Read bounding box
    var bmin : Vector3;
    var bmax : Vector3;
    bmin.x = buf.ReadSingle ();
    bmax.x = buf.ReadSingle ();
    bmin.y = buf.ReadSingle ();
    bmax.y = buf.ReadSingle ();
    bmin.z = buf.ReadSingle ();
    bmax.z = buf.ReadSingle ();
 
    // Decode vectors as 16 bit integer components between the bounds
    for (var i = 0; i < n; ++i) {
        var ix : System.UInt16 = buf.ReadUInt16 ();
        var iy : System.UInt16 = buf.ReadUInt16 ();
        var iz : System.UInt16 = buf.ReadUInt16 ();
        var xx : float = ix / 65535.0 * (bmax.x - bmin.x) + bmin.x;
        var yy : float = iy / 65535.0 * (bmax.y - bmin.y) + bmin.y;
        var zz : float = iz / 65535.0 * (bmax.z - bmin.z) + bmin.z;
        arr[i] = Vector3 (xx,yy,zz);
    }
}
 
static function WriteVector3Array16bit (arr : Vector3[], buf : BinaryWriter) : void
{
    if (arr.Length == 0)
        return;
 
    // Calculate bounding box of the array
    var bounds = Bounds (arr[0], Vector3(0.001,0.001,0.001));
    for (var v in arr)
        bounds.Encapsulate (v);
 
    // Write bounds to stream
    var bmin = bounds.min;
    var bmax = bounds.max;
    buf.Write (bmin.x);
    buf.Write (bmax.x);
    buf.Write (bmin.y);
    buf.Write (bmax.y);
    buf.Write (bmin.z);
    buf.Write (bmax.z);
 
    // Encode vectors as 16 bit integer components between the bounds
    for (var v in arr) {
        var xx = Mathf.Clamp ((v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0, 0.0, 65535.0);
        var yy = Mathf.Clamp ((v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0, 0.0, 65535.0);
        var zz = Mathf.Clamp ((v.z - bmin.z) / (bmax.z - bmin.z) * 65535.0, 0.0, 65535.0);
        var ix : System.UInt16 = xx;
        var iy : System.UInt16 = yy;
        var iz : System.UInt16 = zz;
        buf.Write (ix);
        buf.Write (iy);
        buf.Write (iz);
    }
}
 
 
static function ReadVector2Array16bit (arr : Vector2[], buf : BinaryReader) : void
{
    var n = arr.Length;
    if (n == 0)
        return;
 
    // Read bounding box
    var bmin : Vector2;
    var bmax : Vector2;
    bmin.x = buf.ReadSingle ();
    bmax.x = buf.ReadSingle ();
    bmin.y = buf.ReadSingle ();
    bmax.y = buf.ReadSingle ();
 
    // Decode vectors as 16 bit integer components between the bounds
    for (var i = 0; i < n; ++i) {
        var ix : System.UInt16 = buf.ReadUInt16 ();
        var iy : System.UInt16 = buf.ReadUInt16 ();
        var xx : float = ix / 65535.0 * (bmax.x - bmin.x) + bmin.x;
        var yy : float = iy / 65535.0 * (bmax.y - bmin.y) + bmin.y;
        arr[i] = Vector2 (xx,yy);
    }
}
 
static function WriteVector2Array16bit (arr : Vector2[], buf : BinaryWriter) : void
{
    if (arr.Length == 0)
        return;
 
    // Calculate bounding box of the array
    var bmin : Vector2 = arr[0] - Vector2(0.001,0.001);
    var bmax : Vector2 = arr[0] + Vector2(0.001,0.001);
    for (var v in arr) {
        bmin.x = Mathf.Min (bmin.x, v.x);
        bmin.y = Mathf.Min (bmin.y, v.y);
        bmax.x = Mathf.Max (bmax.x, v.x);
        bmax.y = Mathf.Max (bmax.y, v.y);
    }
 
    // Write bounds to stream
    buf.Write (bmin.x);
    buf.Write (bmax.x);
    buf.Write (bmin.y);
    buf.Write (bmax.y);
 
    // Encode vectors as 16 bit integer components between the bounds
    for (var v in arr) {
        var xx = (v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0;
        var yy = (v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0;
        var ix : System.UInt16 = xx;
        var iy : System.UInt16 = yy;
        buf.Write (ix);
        buf.Write (iy);
    }
}
 
static function ReadVector3ArrayBytes (arr : Vector3[], buf : BinaryReader) : void
{
    // Decode vectors as 8 bit integers components in -1.0 .. 1.0 range
    var n = arr.Length;
    for (var i = 0; i < n; ++i) {
        var ix : byte = buf.ReadByte ();
        var iy : byte = buf.ReadByte ();
        var iz : byte = buf.ReadByte ();
        var xx : float = (ix - 128.0) / 127.0;
        var yy : float = (iy - 128.0) / 127.0;
        var zz : float = (iz - 128.0) / 127.0;
        arr[i] = Vector3(xx,yy,zz);
    }
}
 
static function WriteVector3ArrayBytes (arr : Vector3[], buf : BinaryWriter) : void
{
    // Encode vectors as 8 bit integers components in -1.0 .. 1.0 range
    for (var v in arr) {
        var ix : byte = Mathf.Clamp (v.x * 127.0 + 128.0, 0.0, 255.0);
        var iy : byte = Mathf.Clamp (v.y * 127.0 + 128.0, 0.0, 255.0);
        var iz : byte = Mathf.Clamp (v.z * 127.0 + 128.0, 0.0, 255.0);
        buf.Write (ix);
        buf.Write (iy);
        buf.Write (iz);
    }
}
 
static function ReadVector4ArrayBytes (arr : Vector4[], buf : BinaryReader) : void
{
    // Decode vectors as 8 bit integers components in -1.0 .. 1.0 range
    var n = arr.Length;
    for (var i = 0; i < n; ++i) {
        var ix : byte = buf.ReadByte ();
        var iy : byte = buf.ReadByte ();
        var iz : byte = buf.ReadByte ();
        var iw : byte = buf.ReadByte ();
        var xx : float = (ix - 128.0) / 127.0;
        var yy : float = (iy - 128.0) / 127.0;
        var zz : float = (iz - 128.0) / 127.0;
        var ww : float = (iw - 128.0) / 127.0;
        arr[i] = Vector4(xx,yy,zz,ww);
    }
}
 
static function WriteVector4ArrayBytes (arr : Vector4[], buf : BinaryWriter) : void
{
    // Encode vectors as 8 bit integers components in -1.0 .. 1.0 range
    for (var v in arr) {
        var ix : byte = Mathf.Clamp (v.x * 127.0 + 128.0, 0.0, 255.0);
        var iy : byte = Mathf.Clamp (v.y * 127.0 + 128.0, 0.0, 255.0);
        var iz : byte = Mathf.Clamp (v.z * 127.0 + 128.0, 0.0, 255.0);
        var iw : byte = Mathf.Clamp (v.w * 127.0 + 128.0, 0.0, 255.0);
        buf.Write (ix);
        buf.Write (iy);
        buf.Write (iz);
        buf.Write (iw);
    }
}
 
// Writes mesh to an array of bytes.
static function WriteMesh( mesh : Mesh, saveTangents : boolean ) : byte[]
{
    if( !mesh )
    {
        print( "No mesh given!" );
        return null;
    }
 
    var verts = mesh.vertices;
    var normals = mesh.normals;
    var tangents = mesh.tangents;
    var uvs = mesh.uv;    
    var tris = mesh.triangles;
 
    // figure out vertex format
    var format : byte = 1;
    if( normals.Length > 0 )
        format |= 2;
    if( saveTangents && tangents.Length > 0 )
        format |= 4;
    if( uvs.Length > 0 )
        format |= 8;
 
    var stream = new MemoryStream();
    var buf = new BinaryWriter( stream );
 
    // write header
    var vertCount : System.UInt16 = verts.Length;
    var triCount : System.UInt16 = tris.Length / 3;
    buf.Write( vertCount );
    buf.Write( triCount );
    buf.Write( format );
    // vertex components
    WriteVector3Array16bit (verts, buf);
    WriteVector3ArrayBytes (normals, buf);
    if (saveTangents)
        WriteVector4ArrayBytes (tangents, buf);
    WriteVector2Array16bit (uvs, buf);
    // triangle indices
    for( var idx in tris ) {
        var idx16 : System.UInt16 = idx;
        buf.Write( idx16 );
    }
    buf.Close();
 
    return stream.ToArray();
}
 
 
// Writes mesh to a local file, for loading with WWW interface later.
static function WriteMeshToFileForWeb( mesh : Mesh, name : String, saveTangents : boolean )
{
    // Write mesh to regular bytes
    var bytes = WriteMesh( mesh, saveTangents );
 
    // Write to file
    var fs = new FileStream( name, FileMode.Create );
    fs.Write( bytes, 0, bytes.Length );
    fs.Close();
}
 
 
// Reads mesh from the given WWW (that is finished downloading already)
static function ReadMeshFromWWW( download : WWW ) : Mesh
{
    if (download.error) {
        print ("Error downloading mesh: " + download.error);
        return null;
    }
 
    if (!download.isDone) {
        print ("Download must be finished already");
        return null;
    }
 
    var bytes = download.bytes;
 
    // Read and return the mesh from regular bytes.
    return ReadMesh( bytes );
}JavaScript - SaveMeshForWeb.js// Add this to any object. This will save it's mesh
// to a local file for the Web.
 
var fileName = "SerializedMesh.data";
var saveTangents = false;
 
function Start()
{
    var inputMesh = GetComponent(MeshFilter).mesh;
    var fullFileName = Application.dataPath + "/" + fileName;
    MeshSerializer.WriteMeshToFileForWeb (inputMesh, fullFileName, saveTangents);
    print ("Saved " + name + " mesh to " + fullFileName );
}JavaScript - LoadMeshFromWeb.js// Add this to any object. This will load it's
// mesh from the given URL.
 
var url = "http://files.unity3d.com/aras/SerializedMesh.data";
 
function Start()
{
    print ("Loading mesh from " + url);
    var download = WWW(url);
    yield download;
    var mesh = MeshSerializer.ReadMeshFromWWW( download );
    if (!mesh)
    {
        print ("Failed to load mesh");
        return;
    }
    print ("Mesh loaded");
 
    var meshFilter : MeshFilter = GetComponent(MeshFilter);
    if( !meshFilter ) {
        meshFilter = gameObject.AddComponent(MeshFilter);
        gameObject.AddComponent("MeshRenderer");
        renderer.material.color = Color.white;
    }
    meshFilter.mesh = mesh;
}Source code - C# version (slightly modified)C# - MeshSerializer.csusing System;
using System.IO;
using UnityEngine;
 
public class MeshSerializer
{
	// A simple mesh saving/loading functionality.
	// This is a utility script, you don't need to add it to any objects.
	// See SaveMeshForWeb and LoadMeshFromWeb for example of use.
	//
	// Uses a custom binary format:
	//
	//    2 bytes vertex count
	//    2 bytes triangle count
	//    1 bytes vertex format (bits: 0=vertices, 1=normals, 2=tangents, 3=uvs)
	//
	//    After that come vertex component arrays, each optional except for positions.
	//    Which ones are present depends on vertex format:
	//        Positions
	//            Bounding box is before the array (xmin,xmax,ymin,ymax,zmin,zmax)
	//            Then each vertex component is 2 byte unsigned short, interpolated between the bound axis
	//        Normals
	//            One byte per component
	//        Tangents
	//            One byte per component
	//        UVs (8 bytes/vertex - 2 floats)
	//            Bounding box is before the array (xmin,xmax,ymin,ymax)
	//            Then each UV component is 2 byte unsigned short, interpolated between the bound axis
	//
	//    Finally the triangle indices array: 6 bytes per triangle (3 unsigned short indices)
	// Reads mesh from an array of bytes. [old: Can return null if the bytes seem invalid.]
	public static Mesh ReadMesh(byte[] bytes)
	{
		if (bytes == null || bytes.Length < 5)
			throw new Exception("Invalid mesh file!");
 
		var buf = new BinaryReader(new MemoryStream(bytes));
 
		// read header
		var vertCount = buf.ReadUInt16();
		var triCount = buf.ReadUInt16();
		var format = buf.ReadByte();
 
		// sanity check
		if (vertCount < 0 || vertCount > 64000)
			throw new Exception("Invalid vertex count in the mesh data!");
		if (triCount < 0 || triCount > 64000)
			throw new Exception("Invalid triangle count in the mesh data!");
		if (format < 1 || (format & 1) == 0 || format > 15)
			throw new Exception("Invalid vertex format in the mesh data!");
 
		var mesh = new Mesh();
		int i;
 
		// positions
		var verts = new Vector3[vertCount];
		ReadVector3Array16Bit(verts, buf);
		mesh.vertices = verts;
 
		if ((format & 2) != 0) // have normals
		{
			var normals = new Vector3[vertCount];
			ReadVector3ArrayBytes(normals, buf);
			mesh.normals = normals;
		}
 
		if ((format & 4) != 0) // have tangents
		{
			var tangents = new Vector4[vertCount];
			ReadVector4ArrayBytes(tangents, buf);
			mesh.tangents = tangents;
		}
 
		if ((format & 8) != 0) // have UVs
		{
			var uvs = new Vector2[vertCount];
			ReadVector2Array16Bit(uvs, buf);
			mesh.uv = uvs;
		}
 
		// triangle indices
		var tris = new int[triCount * 3];
		for (i = 0; i < triCount; ++i)
		{
			tris[i * 3 + 0] = buf.ReadUInt16();
			tris[i * 3 + 1] = buf.ReadUInt16();
			tris[i * 3 + 2] = buf.ReadUInt16();
		}
		mesh.triangles = tris;
 
		buf.Close();
 
		return mesh;
	}
 
	static void ReadVector3Array16Bit(Vector3[] arr, BinaryReader buf)
	{
		var n = arr.Length;
		if (n == 0)
			return;
 
		// read bounding box
		Vector3 bmin;
		Vector3 bmax;
		bmin.x = buf.ReadSingle();
		bmax.x = buf.ReadSingle();
		bmin.y = buf.ReadSingle();
		bmax.y = buf.ReadSingle();
		bmin.z = buf.ReadSingle();
		bmax.z = buf.ReadSingle();
 
		// decode vectors as 16 bit integer components between the bounds
		for (var i = 0; i < n; ++i)
		{
			ushort ix = buf.ReadUInt16();
			ushort iy = buf.ReadUInt16();
			ushort iz = buf.ReadUInt16();
			float xx = ix / 65535.0f * (bmax.x - bmin.x) + bmin.x;
			float yy = iy / 65535.0f * (bmax.y - bmin.y) + bmin.y;
			float zz = iz / 65535.0f * (bmax.z - bmin.z) + bmin.z;
			arr[i] = new Vector3(xx, yy, zz);
		}
	}
	static void WriteVector3Array16Bit(Vector3[] arr, BinaryWriter buf)
	{
		if (arr.Length == 0)
			return;
 
		// calculate bounding box of the array
		var bounds = new Bounds(arr[0], new Vector3(0.001f, 0.001f, 0.001f));
		foreach (var v in arr)
			bounds.Encapsulate(v);
 
		// write bounds to stream
		var bmin = bounds.min;
		var bmax = bounds.max;
		buf.Write(bmin.x);
		buf.Write(bmax.x);
		buf.Write(bmin.y);
		buf.Write(bmax.y);
		buf.Write(bmin.z);
		buf.Write(bmax.z);
 
		// encode vectors as 16 bit integer components between the bounds
		foreach (var v in arr)
		{
			var xx = Mathf.Clamp((v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0f, 0.0f, 65535.0f);
			var yy = Mathf.Clamp((v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0f, 0.0f, 65535.0f);
			var zz = Mathf.Clamp((v.z - bmin.z) / (bmax.z - bmin.z) * 65535.0f, 0.0f, 65535.0f);
			var ix = (ushort)xx;
			var iy = (ushort)yy;
			var iz = (ushort)zz;
			buf.Write(ix);
			buf.Write(iy);
			buf.Write(iz);
		}
	}
	static void ReadVector2Array16Bit(Vector2[] arr, BinaryReader buf)
	{
		var n = arr.Length;
		if (n == 0)
			return;
 
		// Read bounding box
		Vector2 bmin;
		Vector2 bmax;
		bmin.x = buf.ReadSingle();
		bmax.x = buf.ReadSingle();
		bmin.y = buf.ReadSingle();
		bmax.y = buf.ReadSingle();
 
		// Decode vectors as 16 bit integer components between the bounds
		for (var i = 0; i < n; ++i)
		{
			ushort ix = buf.ReadUInt16();
			ushort iy = buf.ReadUInt16();
			float xx = ix / 65535.0f * (bmax.x - bmin.x) + bmin.x;
			float yy = iy / 65535.0f * (bmax.y - bmin.y) + bmin.y;
			arr[i] = new Vector2(xx, yy);
		}
	}
	static void WriteVector2Array16Bit(Vector2[] arr, BinaryWriter buf)
	{
		if (arr.Length == 0)
			return;
 
		// Calculate bounding box of the array
		Vector2 bmin = arr[0] - new Vector2(0.001f, 0.001f);
		Vector2 bmax = arr[0] + new Vector2(0.001f, 0.001f);
		foreach (var v in arr)
		{
			bmin.x = Mathf.Min(bmin.x, v.x);
			bmin.y = Mathf.Min(bmin.y, v.y);
			bmax.x = Mathf.Max(bmax.x, v.x);
			bmax.y = Mathf.Max(bmax.y, v.y);
		}
 
		// Write bounds to stream
		buf.Write(bmin.x);
		buf.Write(bmax.x);
		buf.Write(bmin.y);
		buf.Write(bmax.y);
 
		// Encode vectors as 16 bit integer components between the bounds
		foreach (var v in arr)
		{
			var xx = (v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0f;
			var yy = (v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0f;
			var ix = (ushort)xx;
			var iy = (ushort)yy;
			buf.Write(ix);
			buf.Write(iy);
		}
	}
 
	static void ReadVector3ArrayBytes(Vector3[] arr, BinaryReader buf)
	{
		// decode vectors as 8 bit integers components in -1.0f .. 1.0f range
		var n = arr.Length;
		for (var i = 0; i < n; ++i)
		{
			byte ix = buf.ReadByte();
			byte iy = buf.ReadByte();
			byte iz = buf.ReadByte();
			float xx = (ix - 128.0f) / 127.0f;
			float yy = (iy - 128.0f) / 127.0f;
			float zz = (iz - 128.0f) / 127.0f;
			arr[i] = new Vector3(xx, yy, zz);
		}
	}
	static void WriteVector3ArrayBytes(Vector3[] arr, BinaryWriter buf)
	{
		// encode vectors as 8 bit integers components in -1.0f .. 1.0f range
		foreach (var v in arr)
		{
			var ix = (byte)Mathf.Clamp(v.x * 127.0f + 128.0f, 0.0f, 255.0f);
			var iy = (byte)Mathf.Clamp(v.y * 127.0f + 128.0f, 0.0f, 255.0f);
			var iz = (byte)Mathf.Clamp(v.z * 127.0f + 128.0f, 0.0f, 255.0f);
			buf.Write(ix);
			buf.Write(iy);
			buf.Write(iz);
		}
	}
 
	static void ReadVector4ArrayBytes(Vector4[] arr, BinaryReader buf)
	{
		// Decode vectors as 8 bit integers components in -1.0f .. 1.0f range
		var n = arr.Length;
		for (var i = 0; i < n; ++i)
		{
			byte ix = buf.ReadByte();
			byte iy = buf.ReadByte();
			byte iz = buf.ReadByte();
			byte iw = buf.ReadByte();
			float xx = (ix - 128.0f) / 127.0f;
			float yy = (iy - 128.0f) / 127.0f;
			float zz = (iz - 128.0f) / 127.0f;
			float ww = (iw - 128.0f) / 127.0f;
			arr[i] = new Vector4(xx, yy, zz, ww);
		}
	}
	static void WriteVector4ArrayBytes(Vector4[] arr, BinaryWriter buf)
	{
		// Encode vectors as 8 bit integers components in -1.0f .. 1.0f range
		foreach (var v in arr)
		{
			var ix = (byte)Mathf.Clamp(v.x * 127.0f + 128.0f, 0.0f, 255.0f);
			var iy = (byte)Mathf.Clamp(v.y * 127.0f + 128.0f, 0.0f, 255.0f);
			var iz = (byte)Mathf.Clamp(v.z * 127.0f + 128.0f, 0.0f, 255.0f);
			var iw = (byte)Mathf.Clamp(v.w * 127.0f + 128.0f, 0.0f, 255.0f);
			buf.Write(ix);
			buf.Write(iy);
			buf.Write(iz);
			buf.Write(iw);
		}
	}
 
	// Writes mesh to an array of bytes.
	public static byte[] WriteMesh(Mesh mesh, bool saveTangents)
	{
		if (!mesh)
			throw new Exception("No mesh given!");
 
		var verts = mesh.vertices;
		var normals = mesh.normals;
		var tangents = mesh.tangents;
		var uvs = mesh.uv;
		var tris = mesh.triangles;
 
		// figure out vertex format
		byte format = 1;
		if (normals.Length > 0)
			format |= 2;
		if (saveTangents && tangents.Length > 0)
			format |= 4;
		if (uvs.Length > 0)
			format |= 8;
 
		var stream = new MemoryStream();
		var buf = new BinaryWriter(stream);
 
		// write header
		var vertCount = (ushort)verts.Length;
		var triCount = (ushort)(tris.Length / 3);
		buf.Write(vertCount);
		buf.Write(triCount);
		buf.Write(format);
		// vertex components
		WriteVector3Array16Bit(verts, buf);
		WriteVector3ArrayBytes(normals, buf);
		if (saveTangents)
			WriteVector4ArrayBytes(tangents, buf);
		WriteVector2Array16Bit(uvs, buf);
		// triangle indices
		foreach (var idx in tris)
		{
			var idx16 = (ushort)idx;
			buf.Write(idx16);
		}
		buf.Close();
 
		return stream.ToArray();
	}
}
}
