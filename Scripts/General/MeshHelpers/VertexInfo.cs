// Original url: http://wiki.unity3d.com/index.php/VertexInfo
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MeshHelpers/VertexInfo.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MeshHelpers
{
How to useBake the scale and rotation of the mesh in your 3D modeling application before you import it. Scale has to be 1,1,1 in Unity. 
Scriptpublic var guiTextPrototype : Transform;
 
private var guiTextTransforms = new Array ();
private var baseVertices = new Array ();
private var meshObj : Mesh;
private var differentPos = new Array ();	//to store all different Positions of the Verteces only once
 
function Start ()
{
	if ( guiTextPrototype == null || guiTextPrototype.guiText == null )
	{
		Debug.Log ( "No Gameobject or GUIText Component attached in the Inspector !" );
		return;
	}
	//find the first mesh attached to the GameObject
	meshObj = GetComponent(MeshFilter).mesh;
	//the vertices of the first mesh  
	baseVertices = meshObj.vertices;
	//add the first vertex of the baseVertices into the diffPos Array that it has a length of 1 (for for)
	differentPos.Add ( baseVertices[0] );
	//create an array to store which elements of the baseVertex Array and the differentPos Array belong to eachother (there are a lot of vertexes with the same position) 
	var linkArray = new int[baseVertices.length];
	//seach the baseVertices Array if the Position of an special array is already recorded in the differentPos Array
	for (var i=0;i<baseVertices.length;i++)
	{
		var foundIt : int;
		foundIt = 0;
 
		for (var j=0;j<differentPos.length;j++)
		{
			if ( differentPos[j].x == baseVertices[i].x && differentPos[j].y == baseVertices[i].y && differentPos[j].z == baseVertices[i].z )
			{
				foundIt = 1;
			}
		}
		//if the Position is unknown Add the new element to the differentPos Array
		if ( !foundIt )
		{
				differentPos.Add ( baseVertices[i] );
		}
	}	
	//link the values of the baseVertices Array and the differentPos Array via the linkArray (linkArray is used that it is only once necassary to search)  
	for ( l=0;l<baseVertices.length;l++)
	{
		for ( m=0;m<differentPos.length;m++)
		{
			if ( baseVertices[l] == differentPos[m] ) linkArray[l] = m;
		}
	}
	//output the results
	/*
	for (var k=0;k<differentPos.length;k++)
	{
		Debug.Log ( "Vertex Number: " + k + " pos x: " + differentPos[k].x + " pos y: " + differentPos[k].y + " pos z: " + differentPos[k].z );
	}
	for (var n=0;n<linkArray.length;n++)
	{
		Debug.Log ( "Element #" + n + " of baseVertices[] is linked to Element #" + linkArray[n] + " of differentPos[]"  );
	}
	*/
 
	//clone GUITexts and add their Transforms into an Array
	for (var p=0;p<differentPos.length;p++)
	{
		var thisGuiTextInstance : Transform;
		thisGuiTextInstance = Instantiate(guiTextPrototype);
		guiTextTransforms[p] = thisGuiTextInstance;
 
		guiTextTransforms[p].name = "VertexInfoGUIText" + p;
		guiTextTransforms[p].guiText.text = ".\n"; 
	}
	//Debug.Log ( guiTextTransforms.length );
	//destroy the Prototype ...
	Destroy (guiTextPrototype.gameObject);
 
	//fill in the correct Text for each GUIText
	for (var q=0;q<differentPos.length;q++)
	{
		for (var r=0;r<linkArray.length;r++)
		{
			if ( linkArray[r] == q )
			{
				guiTextTransforms[q].guiText.text  += r + "\n" ;
			}
		}
	}
 
	for (var s=0;s<differentPos.length;s++)
	{
		guiTextTransforms[s].position = Camera.main.WorldToViewportPoint( differentPos[s] );
	}
}
 
 
function Update () 
{
	if ( differentPos.length == 0 )
	{
		Debug.Log ( "No Gameobject or GUIText Component attached in the Inspector ! --->#2" );
		return;
	}
	for (var t=0;t<differentPos.length;t++)
	{
		guiTextTransforms[t].position = Camera.main.WorldToViewportPoint( differentPos[t] );
	}
 
	//?
	//meshObj.RecalculateBounds();
	//meshObj.RecalculateNormals();
 
}
}
