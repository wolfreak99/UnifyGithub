// Original url: http://wiki.unity3d.com/index.php/IsoFrame
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorGUIScripts/IsoFrame.cs
// File based on original modification date of: 2 November 2012, at 15:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorGUIScripts
{
Contents [hide] 
1 Author 
2 Description 
3 Usage 
4 UnityScript - IsoFrame.cs 

AuthorHayden Scott-Baron (Dock) - http://starfruitgames.com 
DescriptionThis adds an isometric frame visible in the editor. 
UsageDrag this onto a gameObject, and adjust the public variables. Add multiple aspect ratios to show different sizes. 


UnityScript - IsoFrame.cs// IsoFrame.cs
// Hayden Scott-Baron (Dock) - http://starfruitgames.com
// Draws a gizmo frame for orthographic cameras.
public class IsoFrame : MonoBehaviour 
{
	public Vector3 offset = Vector3.zero; 
	public float scaleFactor = 64.0f; 
	public Vector2[] frameSizes = new Vector2[] { new Vector2(1.333f, 1.0f), new Vector2(1.7777f, 1.0f) };
	public Color[] colors = new Color[]
	{
		Color.red, 
		Color.blue, 
		Color.green,
		Color.yellow,
		Color.grey,
		Color.cyan,
	};
 
	void OnDrawGizmos()
	{
		//foreach (Vector3 frameSize in frameSizes)
		for (int i = 0; i < frameSizes.Length; i++) 
		{
			Vector2 frameSize = frameSizes[i] * scaleFactor; 
			if (i < colors.Length)
				Gizmos.color = colors[i];
 
			Vector3[] corners = new Vector3[]
			{
				offset + new Vector3( transform.position.x - (frameSize.x * 0.5f),  transform.position.y - (frameSize.y * 0.5f), transform.position.z),
				offset + new Vector3( transform.position.x + (frameSize.x * 0.5f),  transform.position.y - (frameSize.y * 0.5f), transform.position.z),
				offset + new Vector3( transform.position.x + (frameSize.x * 0.5f),  transform.position.y + (frameSize.y * 0.5f), transform.position.z),
				offset + new Vector3( transform.position.x - (frameSize.x * 0.5f),  transform.position.y + (frameSize.y * 0.5f), transform.position.z)		
			};
 
			Gizmos.DrawLine (corners[0], corners[1]);
			Gizmos.DrawLine (corners[1], corners[2]);
			Gizmos.DrawLine (corners[2], corners[3]);
			Gizmos.DrawLine (corners[3], corners[0]);
		}
	}
}
}
