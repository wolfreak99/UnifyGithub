// Original url: http://wiki.unity3d.com/index.php/MaterialAnalyzer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/MaterialAnalyzer.cs
// File based on original modification date of: 23 July 2012, at 11:44. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Xadhoom 
Description Adds a new editor window at menu Custom→MaterialAnalyzer. The editor window allows analyzing and list all materials used by the current selection in scene. Results can be dumped as material list or hierarchy to text files. This window helps to keep track of material usage to optimize drawcalls. 
Background information can be found here: http://trianglestrip.blogspot.de/2012/07/material-analyzer.html 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Click on Custom→MaterialAnalyzer 
Select one or more game objects in scene. Press Analyze Selection. Now the list should show all materials used by the selected game objects. The small button to the left in the list select the material asset. Selecting one or more list items selects the game objects using this/these materials. 
Press Dump Hierarchy to dump the selection hierarchy including the used materials to text file. Press Dump List to dump the list of materials to text file including the game objects using each material. 
C# - MaterialAnalyzer.cs (Developed for Unity 3.5) using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
 
 
/// ---------------------------------------------------------------------------
/// <summary>
/// Analyzes the currently selected gameobjects in scene recursively and lists all materials in an EditorWindow.
/// The list allows to (mutli)select materials which automatically selects the scene game objects which use it.
/// Additionally every list item provides a button to jump to the material asset in project window.
/// </summary>
/// ---------------------------------------------------------------------------
public class MaterialAnalyzer : EditorWindow
{
	/// PRIVATE ===============================================================
 
	private static GUIStyle mListStyle;
	private static GUIStyle mItemStyle;
	private static GUIStyle mItemSelectedStyle;
	private Texture2D mListBackgroundTex;
	private Texture2D mItemBackgroundTex;
	private Texture2D mItemSelectedBackgroundTex;
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Defines a single list item encapsulating a set of game objects and a selection state.
	/// </summary>
	/// -----------------------------------------------------------------------
	private class ListItem
	{
		public ListItem( bool selected = false )
		{
			this.Selected = selected;
		}
 
		public HashSet<GameObject> GameObjects = new HashSet<GameObject>();
		public bool Selected;
	};
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Material comparer calls the material name comparer.
	/// </summary>
	/// -----------------------------------------------------------------------
	private class MaterialComp : IComparer<Material>
	{
		public int Compare( Material x, Material y )
		{
			return x.name.CompareTo( y.name );
		}
	}
	/// <summary>
	/// Stores list items by material instance.
	/// </summary>
	private SortedDictionary<Material, ListItem> mSelectionMaterials = new SortedDictionary<Material, ListItem>( new MaterialComp() );
 
	/// <summary>
	/// The current scroll position.
	/// </summary>
	private Vector2 mScrollPosition;
 
	/// <summary>
	/// A text dump of the material hierarchy.
	/// </summary>
	private string mMaterialHierarchyDump = string.Empty;
 
	/// METHODS ===============================================================
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Adds menu named "Analyze Scene" to the "Debug" menu which creates and initializes a new instance of this class.
	/// </summary>
	/// -----------------------------------------------------------------------
	[MenuItem("Custom/Analyze Materials")]
	public static void Init()
	{
		MaterialAnalyzer win = EditorWindow.GetWindow( typeof(MaterialAnalyzer) ) as MaterialAnalyzer;
		win.init();
		win.Show();
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Draws the GUI window.
	/// </summary>
	/// -----------------------------------------------------------------------
	private void OnGUI()
	{
		GUILayout.BeginVertical();
		if( GUILayout.Button( "Analyze Selection" ) )
			analyzeSelection();
		if( GUILayout.Button( "Dump Hierarchy" ) )
			dumpMaterialHierarchy();
		if( GUILayout.Button( "Dump List" ) )
			dumpMaterialList();
		GUILayout.EndVertical();
 
		GUILayout.Label( "Materials: " + mSelectionMaterials.Count.ToString(), EditorStyles.boldLabel );
 
		mScrollPosition = GUILayout.BeginScrollView( mScrollPosition, false, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, mListStyle, GUILayout.MinWidth( 400 ), GUILayout.MaxWidth( 1000 ), GUILayout.MaxHeight( 1000 ) );
		foreach( KeyValuePair<Material, ListItem> item in mSelectionMaterials )
		{
			GUILayout.BeginHorizontal();
 
			// select the material asset in project hierarchy
			if( GUILayout.Button( "<", EditorStyles.miniButton, GUILayout.Width( 20 ) ) )
			{
				// unselect all selected and select the material instance in project
				foreach( ListItem listItem in mSelectionMaterials.Values )
					listItem.Selected = false;
 
				Selection.activeObject = item.Key;
			}
			if( GUILayout.Button( item.Key.name, item.Value.Selected ? mItemSelectedStyle : mItemStyle, GUILayout.MinWidth( 200 ) ) )
				processListItemClick( item.Value );
 
			if( GUILayout.Button( item.Key.shader != null ? item.Key.shader.name : " <MISSING>", item.Value.Selected ? mItemSelectedStyle : mItemStyle, GUILayout.Width( 300 ) ) )
				processListItemClick( item.Value );
 
			GUILayout.EndHorizontal();
		}
 
		GUILayout.EndScrollView();
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Processes the list item click.
	/// </summary>
	/// <param name='itemClicked'>
	/// The item clicked.
	/// </param>
	/// -----------------------------------------------------------------------
	private void processListItemClick( ListItem itemClicked )
	{
		Event e = Event.current;
 
		// if shift/control is pressed just add this element
		if( e.control )
		{
			itemClicked.Selected = !itemClicked.Selected;
			updateSceneSelection();
		}
		else
		{
			// unselect all selected and select this
			foreach( ListItem listItem in mSelectionMaterials.Values )
				listItem.Selected = false;
 
			itemClicked.Selected = true;
			updateSceneSelection();
		}
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Starts recursive analyze process iterating through every selected GameObject.
	/// </summary>
	/// -----------------------------------------------------------------------
	private void analyzeSelection()
	{
		mSelectionMaterials.Clear();
 
		if( Selection.transforms.Length == 0 )
		{
			Debug.LogError( "Please select the object(s) you wish to analyze." );
			return;
		}
 
		StringBuilder dump = new StringBuilder();
		foreach( Transform transform in Selection.transforms )
			analyzeGameObject( transform.gameObject, dump, "" );
 
		mMaterialHierarchyDump = dump.ToString();
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Analyzes the given game object.
	/// </summary>
	/// <param name='gameObject'>
	/// The game object to analyze.
	/// </param>
	/// -----------------------------------------------------------------------
	private void analyzeGameObject( GameObject gameObject, StringBuilder dump, string indent )
	{
		dump.Append( indent + gameObject.name + "\n" );
 
		foreach( Component component in gameObject.GetComponents<Component>() )
			analyzeComponent( component, dump, indent + "    " );
 
		foreach( Transform child in gameObject.transform )
			analyzeGameObject( child.gameObject, dump, indent + "    " );
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Analyzes the given component.
	/// </summary>
	/// <param name='component'>
	/// The component to analyze.
	/// </param>
	/// -----------------------------------------------------------------------
	private void analyzeComponent( Component component, StringBuilder dump, string indent )
	{
		// early out if component is missing
		if( component == null )
			return;
 
		List<Material> materials = new List<Material>();
		switch( component.GetType().ToString() )
		{
		case "UnityEngine.MeshRenderer":
			{
				MeshRenderer mr = component as MeshRenderer;
				foreach( Material mat in mr.sharedMaterials )
					materials.Add( mat );
			}
			break;
		case "UnityEngine.ParticleRenderer":
			{
				ParticleRenderer pr = component as ParticleRenderer;
				foreach( Material mat in pr.sharedMaterials )
					materials.Add( mat );
			}
			break;
		default:
			break;
		}
 
		bool materialMissing = false;
		foreach( Material mat in materials )
		{
			if( mat == null )
			{
				materialMissing = true;
				dump.Append( indent + "> MISSING\n" );
			}
			else
			{
				ListItem item;
				mSelectionMaterials.TryGetValue( mat, out item );
				if( item == null )
				{
					item = new ListItem();
					mSelectionMaterials.Add( mat, item );
				}
				item.GameObjects.Add( component.gameObject );
 
				string matName = mat.shader != null ?
				mat.name + " <" + mat.shader.name + ">" :
				mat.name + " <MISSING>";
				dump.Append( indent + "> " + matName + "\n" );
			}
		}
 
		if( materialMissing )
			Debug.LogWarning( "Material(s) missing in game object '" + component.gameObject + "'!" );
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Dumps the current selection hierarchy to a file.
	/// </summary>
	/// -----------------------------------------------------------------------
	private void dumpMaterialHierarchy()
	{
		if( mMaterialHierarchyDump == string.Empty )
		{
			Debug.LogError( "There is nothing to dump yet." );
			return;
		}
 
		string path = EditorUtility.SaveFilePanel( "Hierarchy Dump File", "", "material_hierarchy.txt", "txt" );
		File.WriteAllText( path, mMaterialHierarchyDump );
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Dumps the current list of materials.
	/// </summary>
	/// -----------------------------------------------------------------------
	private void dumpMaterialList()
	{
		if( mSelectionMaterials.Count == 0 )
		{
			Debug.LogError( "The material list is empty. Dump aborted." );
			return;
		}
 
		// create string from current list
		StringBuilder materialItems = new StringBuilder();
		materialItems.Append( String.Format( "{0,-40}", "Material" ) ).Append( " | " );
		materialItems.Append( String.Format( "{0,-60}", "Shader" ) ).Append( " | " );
		materialItems.Append( String.Format( "{0,-9}", "Occurence" ) + " | " );
		materialItems.Append( "GameObject List\n" );
		materialItems.Append( "----------------------------------------------------------------------------" +
							  "----------------------------------------------------------------------------\n" );
 
		foreach( KeyValuePair<Material, ListItem> item in mSelectionMaterials )
		{
			materialItems.Append( String.Format( "{0,-40}", item.Key.name ) ).Append( " | " );
			materialItems.Append( String.Format( "{0,-60}", item.Key.shader != null ? item.Key.shader.name : " MISSING" ) ).Append( " | " );
			materialItems.Append( String.Format( "{0,-9}", "   " + item.Value.GameObjects.Count.ToString() ) + " | " );
 
			foreach( GameObject go in item.Value.GameObjects )
				materialItems.Append( " " + go.name + " |" );
 
			materialItems.Append( "\n" );
		}
 
		string path = EditorUtility.SaveFilePanel( "Material Dump File", "", "material_list.txt", "txt" );
		File.WriteAllText( path, materialItems.ToString() );
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Selects the game objects in scene stored in all selected list items.
	/// </summary>
	/// -----------------------------------------------------------------------
	private void updateSceneSelection()
	{
		HashSet<UnityEngine.Object> sceneObjectsToSelect = new HashSet<UnityEngine.Object>();
		foreach( ListItem item in mSelectionMaterials.Values )
			if( item.Selected )
				foreach( GameObject go in item.GameObjects )
					sceneObjectsToSelect.Add( go );
 
		UnityEngine.Object[] array = new UnityEngine.Object[sceneObjectsToSelect.Count];
		sceneObjectsToSelect.CopyTo( array );
		Selection.objects = array;
	}
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Initializes GUI styles and textures used.
	/// </summary>
	/// -----------------------------------------------------------------------
	private void init()
	{
		// list background
		mListBackgroundTex = new Texture2D( 1, 1 );
		mListBackgroundTex.SetPixel( 0, 0, new Color( 0.6f, 0.6f, 0.6f ) );
		mListBackgroundTex.Apply();
		mListBackgroundTex.wrapMode = TextureWrapMode.Repeat;
 
		// list style
		mListStyle = new GUIStyle();
		mListStyle.normal.background = mListBackgroundTex;
		mListStyle.margin = new RectOffset( 4, 4, 4, 4 );
		mListStyle.border = new RectOffset( 1, 1, 1, 1 );
 
		// item background
		mItemBackgroundTex = new Texture2D( 1, 1 );
		Color wBGColor = new Color( 0.0f, 0.0f, 0.0f );
		string wBGColorData = EditorPrefs.GetString( "Windows/Background" );
		string[] wBGColorArray = wBGColorData.Split( ';' );
		if( wBGColorArray.Length == 5 )
			wBGColor = new Color( float.Parse( wBGColorArray[1] ), float.Parse( wBGColorArray[2] ), float.Parse( wBGColorArray[3] ) );
		else
			Debug.LogError( "Invalid window color in EditorPref found." + wBGColorArray.Length );
		mItemBackgroundTex.SetPixel( 0, 0, wBGColor );
		mItemBackgroundTex.Apply();
		mItemBackgroundTex.wrapMode = TextureWrapMode.Repeat;
 
		// item selected background
		mItemSelectedBackgroundTex = new Texture2D( 1, 1 );
		mItemSelectedBackgroundTex.SetPixel( 0, 0, new Color( 0.239f, 0.376f, 0.568f ) );
		mItemSelectedBackgroundTex.Apply();
		mItemSelectedBackgroundTex.wrapMode = TextureWrapMode.Repeat;
 
		// item style
		mItemStyle = new GUIStyle( EditorStyles.textField );
		mItemStyle.normal.background = mItemBackgroundTex;
		mItemStyle.hover.textColor = Color.cyan;
		mItemStyle.padding = new RectOffset( 2, 2, 2, 2 );
		mItemStyle.margin = new RectOffset( 1, 2, 1, 1 );
		mItemSelectedStyle = new GUIStyle( mItemStyle );
		mItemSelectedStyle.normal.background = mItemSelectedBackgroundTex;
		mItemSelectedStyle.normal.textColor = Color.white;
	}
}
}
