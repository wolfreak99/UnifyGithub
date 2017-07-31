// Original url: http://wiki.unity3d.com/index.php/UVTransfer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MeshHelpers/UVTransfer.cs
// File based on original modification date of: 5 October 2008, at 21:07. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
Here is a unityPackage that contains two Editor scripts, one that "merge" two OBJ meshes and transfers the UVs from one to the other and creates a text file with the UV-coordinates. The other script reapplies the UVs when reimporting a mesh. 
It also sets all materials to a lightmap material automatically, so you don't need to manually go over the object and change the material type and drag the lightmap texture onto it. 
The only weird "side effect" to it, is that the lightmap model gets a color map texture applied as its lightmap. This isn't really a problem as the lightmap OBJ can be deleted after the UVs have been transferred. 
For the transfer script to work, you must select two mesh objects as shown in the screenshot, and rightclick to select TransferUVs. 
This is an update to Unity 2.1 of the earlier script i posted, and it has only been tested with OBJs exported from LW 9.5. It should work for OBJs from other applications as well. 
Media:UVTransfer.unityPackage.zip 
 
}
