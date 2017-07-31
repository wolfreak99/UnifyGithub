// Original url: http://wiki.unity3d.com/index.php/MeshMorpher
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/MeshMorpher.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Morph.zip 
Javascript - MeshMorpher.js The script included with the project is C#; if you'd rather use Javascript, the script below is a direct translation. 
/// REALLY IMPORTANT NOTE. 
/// When using the mesh morpher you should absolutely make sure that you turn 
/// off generate normals automatically in the importer, or set the normal angle to 180 degrees. 
/// When importing a mesh Unity automatically splits vertices based on normal creases. 
/// However the mesh morpher requires that you use the same amount of vertices for each mesh and that 
/// those vertices are laid out in the exact same way. Thus it won't work if Unity autosplits vertices based on normals. 
var m_Meshes : Mesh[]; 
var m_AnimateAutomatically = true; 
var m_OneLoopLength : float = 1.0;   /// The time it takes for one loop to complete 
var m_WrapMode = WrapMode.Loop; 
private var m_AutomaticTime : float = 0; 
 
private var m_SrcMesh : int = -1; 
private var m_DstMesh : int = -1; 
private var m_Weight : float = -1; 
private var m_Mesh: Mesh; 
 
/// Set the current morph in 
function SetComplexMorph (srcIndex : int, dstIndex : int, t : float) { 
   if (m_SrcMesh == srcIndex && m_DstMesh == dstIndex && Mathf.Approximately(m_Weight, t)) 
      return; 
   var v0 = m_Meshes[srcIndex].vertices; 
   var v1 = m_Meshes[dstIndex].vertices; 
   var vdst = new Vector3[m_Mesh.vertexCount]; 
   for (i=0; i<vdst.Length; i++) 
      vdst[i] = Vector3.Lerp(v0[i], v1[i], t); 
 
   m_Mesh.vertices = vdst; 
   m_Mesh.RecalculateBounds(); 
} 
 
/// t is between 0 and m_Meshes.Length - 1. 
/// 0 means the first mesh, m_Meshes.Length - 1 means the last mesh. 
/// 0.5 means half of the first mesh and half of the second mesh.
function SetMorph (t : float) { 
   var floor : int = t; 
   floor = Mathf.Clamp(floor, 0, m_Meshes.Length - 2); 
   var fraction : float = t - floor; 
   fraction = Mathf.Clamp(t - floor, 0.0, 1.0); 
   SetComplexMorph (floor, floor + 1, fraction); 
} 
 
function Awake () { 
   enabled = m_AnimateAutomatically; 
   var filter : MeshFilter = GetComponent(MeshFilter); 
   // Make sure all meshes are assigned! 
   for (i=0; i<m_Meshes.Length; i++) { 
      if (m_Meshes[i] == null) {    
         Debug.Log("MeshMorpher mesh  " + i + " has not been setup correctly"); 
         m_AnimateAutomatically = false; 
         return; 
      } 
   } 
 
   // At least two meshes 
   if (m_Meshes.Length < 2) { 
      Debug.Log ("The mesh morpher needs at least 2 source meshes"); 
      m_AnimateAutomatically = false; 
      return; 
   } 
 
   filter.sharedMesh = m_Meshes[0]; 
   m_Mesh = filter.mesh; 
   var vertexCount = m_Mesh.vertexCount; 
   for (i=0; i<m_Meshes.Length; i++) { 
      if (m_Meshes[i].vertexCount != vertexCount) {    
         Debug.Log("Mesh " + i + " doesn't have the same number of vertices as the first mesh"); 
         m_AnimateAutomatically = false; 
         return; 
      } 
   } 
} 
 
function Update () { 
   if (m_AnimateAutomatically) { 
      var deltaTime = Time.deltaTime * (m_Meshes.Length - 1) / m_OneLoopLength; 
      m_AutomaticTime += deltaTime; 
      var time : float; 
      if (m_WrapMode == WrapMode.Loop) 
         time = Mathf.Repeat(m_AutomaticTime, m_Meshes.Length - 1); 
      else if (m_WrapMode == WrapMode.PingPong) 
         time = Mathf.PingPong(m_AutomaticTime, m_Meshes.Length - 1); 
      else 
         time = Mathf.Clamp(m_AutomaticTime, 0, m_Meshes.Length - 1); 
 
      SetMorph (time); 
   } 
} 
 
@script RequireComponent (MeshFilter)
}
