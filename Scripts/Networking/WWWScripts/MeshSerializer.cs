/*************************
 * Original url: http://wiki.unity3d.com/index.php/MeshSerializer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/WWWScripts/MeshSerializer.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Aras Pranckevicius 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Networking.WWWScripts
{
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
    
    DescriptionNote: You should probably use MeshSerializer2 now. It produces much smaller file sizes! 
    A set of scrtipts for saving/loading meshes to a simple binary format. Possible use case is saving them to files, uploading them somewhere and then using WWW interface to download meshes dynamically. 
    The package contains an example scene; where one mesh is saved to file and the other is downloaded. 
    UsageMeshSerializer.js is an utility class to do the actual mesh serialization. Has comments inside. 
    SaveMeshForWeb.js and LoadMeshFromWeb.js are example usage scripts. 
    The packageZipped Unity package: Media:MeshSerializer.zip (should work both in Unity 1.x and 2.x) 
    File format usedNote that such a file format is pretty fragile to changes - it does not have any concept of "format version" built in. It's fully enough if you save and load meshes yourself (you know which format are you using), but if you want a generic mesh-loading-format, you'd have to implement something more complex. 
    The file format used now: 
    4 bytes vertex count 
    4 bytes triangle count 
    4 bytes vertex format (bits: 0=vertices, 1=normals, 2=tangents, 3=uvs) 
    After that come vertex component arrays, each optional except for positions. Which ones are present depends on vertex format: 
    Positions (12 bytes/vertex - 3 floats) 
    Normals (12 bytes/vertex - 3 floats) 
    Tangents (16 bytes/vertex - 4 floats) 
    UVs (8 bytes/vertex - 2 floats) 
    Finally the triangle indices array: 12 bytes per triangle (3 int indices) 
    Notes and ideasThere is a related article about saving unity meshes in .obj format: ObjExporter. 
    For smaller file sizes, a whole range of ideas can be tried. Actually, some of them are implemented in MeshSerializer2, so go check it out! 
    Normals and tangents could be encoded in a more compact way: 
    One byte per component (vs. 4 bytes/component); with 0 meaning -1.0; 128 meaning 0.0; 255 meaning 1.0. That's four times smaller than it uses now for a slight accuracy reduction. 
    3 bytes per normal, using 11 bits for two components, and storing a sign of the third component. At load time, recalculate the third from the first two and the sign. The quality should be very good. 
    Same for tangents: 3 bytes for tangent: 11 bits for two components, sign of the third, and sign of the fourth (4th is always either +1 or -1). 
    Positions components could be stored as 2 byte integers, quantizing to the bounding box size. 
    Just drop the 4th byte from floats. A slight accuracy reduction, but a 25% space savings (a trick used by 64KB demo coders a lot :)). 
    When this script was originally written (Unity 1.5.0), one limitation of WWW interface is that it only supported textual data. So this package encodes binary format into a 2-characters-per-byte textual format. This restriction was lifted in a later Unity release, so nowadays the encoding is not actually necessary. 
    Changelog2008 Mar 14 - Bugfix, where in some cases the encode-to-text-characters step would produce out of range results. Interface change, now has to yield the download before creating mesh. 
    2006 Aug 23 - Initial version 
    Source codeAll the source code is included in the provided Unity Package above. This is only for easy-browsing purposes. 
    JavaScript - MeshSerializer.jsimport System.IO;
     
    // A simple mesh saving/loading functionality.
    // This is an utility script, you don't need to add it to any objects.
    // See SaveMeshForWeb and LoadMeshFromWeb for example of use.
    //
    // Uses a custom binary format:
    //
    //    4 bytes vertex count
    //    4 bytes triangle count
    //    4 bytes vertex format (bits: 0=vertices, 1=normals, 2=tangents, 3=uvs)
    //
    //    After that come vertex component arrays, each optional except for positions.
    //    Which ones are present depends on vertex format:
    //        Positions (12 bytes/vertex - 3 floats)
    //        Normals (12 bytes/vertex - 3 floats)
    //        Tangents (16 bytes/vertex - 4 floats)
    //        UVs (8 bytes/vertex - 2 floats)
    //
    //    Finally the triangle indices array: 12 bytes per triangle (3 int indices)
     
     
    // Reads mesh from an array of bytes. Can return null
    // if the bytes seem invalid.
    static function ReadMesh( bytes : byte[] ) : Mesh
    {
        if( !bytes || bytes.Length < 12 )
        {
            print( "Invalid mesh file!" );
            return null;
        }
     
        var buf = new BinaryReader( new MemoryStream( bytes ) );
     
        // read header
        var vertCount = buf.ReadInt32();
        var triCount = buf.ReadInt32();
        var format = buf.ReadInt32();
     
        // sanity check
        if (vertCount < 0 || vertCount > 65535)
        {
            print ("Invalid vertex count in the mesh data!");
            return null;
        }
        if (triCount < 0 || triCount > 65535)
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
        for( i = 0; i < vertCount; ++i )
        {
            verts[i] = Vector3( buf.ReadSingle(), buf.ReadSingle(), buf.ReadSingle() );
        }
        mesh.vertices = verts;
     
        if( format & 2 ) // have normals
        {
            var normals = new Vector3[vertCount];
            for( i = 0; i < vertCount; ++i )
            {
                normals[i] = Vector3( buf.ReadSingle(), buf.ReadSingle(), buf.ReadSingle() );
            }
            mesh.normals = normals;
        }
     
        if( format & 4 ) // have tangents
        {
            var tangents = new Vector4[vertCount];
            for( i = 0; i < vertCount; ++i )
            {
                tangents[i] = Vector4( buf.ReadSingle(), buf.ReadSingle(), buf.ReadSingle(), buf.ReadSingle() );
            }
            mesh.tangents = tangents;
        }
     
        if( format & 8 ) // have UVs
        {
            var uvs = new Vector2[vertCount];
            for( i = 0; i < vertCount; ++i )
            {
                uvs[i] = Vector2( buf.ReadSingle(), buf.ReadSingle() );
            }
            mesh.uv = uvs;
        }
     
        // triangle indices
        var tris = new int[triCount * 3];
        for( i = 0; i < triCount; ++i )
        {
            tris[i*3+0] = buf.ReadInt32();
            tris[i*3+1] = buf.ReadInt32();
            tris[i*3+2] = buf.ReadInt32();
        }
        mesh.triangles = tris;
     
        buf.Close();
     
        return mesh;
    }
     
     
    // Writes mesh to an array of bytes.
    static function WriteMesh( mesh : Mesh ) : byte[]
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
        var format = 1;
        if( normals.Length > 0 )
            format |= 2;
        if( tangents.Length > 0 )
            format |= 4;
        if( uvs.Length > 0 )
            format |= 8;
     
        var stream = new MemoryStream();
        var buf = new BinaryWriter( stream );
     
        // write header
        buf.Write( verts.Length );
        buf.Write( tris.Length / 3 );
        buf.Write( format );
        // vertex components
        for( var v in verts ) {
            buf.Write( v.x );
            buf.Write( v.y );
            buf.Write( v.z );
        }
        for( var n in normals ) {
            buf.Write( n.x );
            buf.Write( n.y );
            buf.Write( n.z );
        }
        for( var t in tangents ) {
            buf.Write( t.x );
            buf.Write( t.y );
            buf.Write( t.z );
            buf.Write( t.w );
        }
        for( var uv in uvs ) {
            buf.Write( uv.x );
            buf.Write( uv.y );
        }
        // triangle indices
        for( var idx in tris ) {
            buf.Write( idx );
        }
        buf.Close();
     
        return stream.ToArray();
    }
     
     
    // Writes mesh to a local file, for loading with WWW interface later.
    //
    // The tricky part is that currently (Unity 1.5.0) the WWW interface
    // can only return textual data. So we convert the mesh bytes to
    // a text-only-characters format.
    //
    // If you don't use WWW interface (e.g. load files locally), you don't need
    // to do this trickery. Just save/load byte arrays.
    static function WriteMeshToFileForWeb( mesh : Mesh, name : String )
    {
        // Write mesh to regular bytes
        var bytes = WriteMesh( mesh );
     
        // Convert to kind-of-hex characters: 2 characters for each
        // input byte.
        var textBytes = new byte[bytes.Length*2];
        for( var i = 0; i < bytes.Length; ++i )
        {
            var bb = bytes[i];
            var b1 = bb / 16;
            var b2 = bb % 16;
            textBytes[i*2+0] = b1 + 33;
            textBytes[i*2+1] = b2 + 33;
        }
     
        // Write to file
        var fs = new FileStream( name, FileMode.Create );
        fs.Write( textBytes, 0, textBytes.Length );
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
     
        var data = download.data;
     
        // The mesh was written in a text-only-characters format
        // (see WriteMeshToFileForWeb). Convert that to original
        // byte array.
        var bytes = new byte[data.Length/2];
        for( var i = 0; i < bytes.Length; ++i )
        {
            var b1 = System.Convert.ToByte(data[i*2+0]) - 33;
            var b2 = System.Convert.ToByte(data[i*2+1]) - 33;
            bytes[i] = b1 * 16 + b2;
        }
     
        // Read and return the mesh from regular bytes.
        return ReadMesh( bytes );
    }JavaScript - SaveMeshForWeb.js// Add this to any object. This will save its mesh
    // to a local file for the Web.
     
    var fileName = "SerializedMesh.txt";
     
    function Start()
    {
        var inputMesh = GetComponent(MeshFilter).mesh;
        var fullFileName = Application.dataPath + "/" + fileName;
        MeshSerializer.WriteMeshToFileForWeb( inputMesh, fullFileName );
        print ("Saved " + name + " mesh to " + fullFileName );
    }JavaScript - LoadMeshFromWeb.js// Add this to any object. This will load its
    // mesh from the given URL.
     
    var url = "http://unity3d.com/aras/SerializedMesh.txt";
     
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
}
}
