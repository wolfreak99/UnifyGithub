/*************************
 * Original url: http://wiki.unity3d.com/index.php/MaterialReplacer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/MaterialReplacer.cs
 * File based on original modification date of: 30 September 2013, at 12:29. 
 *
 * Author: Xadhoom 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Adds a new editor window at menu Custom→Replace Materials. The editor window allows easy replacement of all materials currently used by a prefab or scene object in one window. The editor window expects the object to be dropped (or selected) in the appropriate object field. After that a list of all materials is shown indicating the current material and its replacement. The material replacements can be dropped or set onto the appropriate object fields. The "save"/"load" button manages external transition files which can be saved or loaded here. Finally the Apply button changes the materials in the prefab or object instance accordingly. 
    Background information can be found here: http://trianglestrip.blogspot.de/2012/07/material-replacer.html 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
    Click on Custom→MaterialReplacer 
    Drop a prefab (not a model file) onto the given object field or select one in the object field. 
    A list shows up with all materials used by the prefab or scene object. Drop or select materials as replacements in the object fields on the right side for particular materials. 
    To save the transitions defined press the Save button in the window. 
    To load transitions from a file press the Load button in the window. 
    To eventually apply the material replacements to the prefab or scene object press the Apply button. 
    C# - MaterialReplacer.cs (Developed for Unity 3.5) using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.Xml;
    using System.IO;
    using System.Text;
     
     
    /// <summary>
    /// The script modifies the materials present in the given prefab or object instance. The object is given
    /// as the input and as soon as it is placed in the object field, the script processes the
    /// materials present and creates a dictionary storing the materials and the new material as key
    /// and value pairs respectively. In the initial case the old and new materials will be the same and
    /// the they will be displayed in the corresponding labels and object fields. We can edit the entry
    /// in object field for "New Material" and put a new material in its place.
    /// Once we have changed the required materials then we can do the following three functions.
    /// a) Save - By clicking this button we go the saveMaterialMap method which saves the material changes 
    /// that we have made into an XML file which can then be used later on if we have to make the changes again at a later time
    ///
    /// b) Load - Using this method we can specify the path of a previously saved XML file and use it to update
    /// the transitions for another object. The method used is the "loadMaterialMap" method which reads data 
    /// from the XML file and correspondingly updates the object fields with the values from the XML file.
    ///
    /// c) Apply Changes - By clicking this button we call the "applyChanges" method which updates the materials
    /// of the object with the new materials we have specified.
    /// </summary>
    public class MaterialReplacer : EditorWindow
    {
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Material comparer calls the material name comparer.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private class MaterialComp : Comparer<Material>
    	{
    		public override int Compare( Material x, Material y )
    		{
    			return x.name.CompareTo( y.name );
    		}
    	}
     
    	/// <summary>
    	/// Stores the material transition for all materials.
    	/// </summary>
    	SortedDictionary<Material, Material> mMaterialMap = new SortedDictionary<Material, Material>( new MaterialComp() );
     
    	/// <summary>
    	/// The currently selected object.
    	/// </summary>
    	GameObject mObject;
     
    	/// <summary>
    	/// Material list scroll position.
    	/// </summary>
    	Vector2 mScrollPosition = new Vector2( 0, 0 );
     
    	// ------------------------------------------------------------------------
    	/// <summary>
    	/// Init this instance.
    	/// </summary>
    	// ------------------------------------------------------------------------
    	[MenuItem ("Custom/Replace Materials")]
    	static void Init()
    	{
    		// Get existing open window or if none, make a new one:
    		MaterialReplacer materialWindow = (MaterialReplacer)EditorWindow.GetWindow( typeof(MaterialReplacer) );
    		materialWindow.position = new Rect( 200, 200, 750, 430 );
    	}
     
    	// ------------------------------------------------------------------------
    	/// <summary>
    	/// OnGUI renders the editor window.
    	/// </summary>
    	// ------------------------------------------------------------------------
    	void OnGUI()
    	{
    		//create an object field for the prefab/object
    		GUILayout.BeginHorizontal();
    		GameObject obj = (GameObject)EditorGUILayout.ObjectField( "Select or drop object:", mObject, typeof(GameObject), true );
    		// if something changed
    		if( obj != mObject )
    		{
    			// ignore model prefabs or
    			if( PrefabUtility.GetPrefabType( obj ) == PrefabType.ModelPrefab )
    			{
    				Debug.LogError( "ProcessMaterials, Object cannot be a model file. Please create a prefab first or select an instance from scene" );
    				mObject = null;
    			}
    			else
    			{
    				mObject = obj;
    			}
     
    			resetMaterialMap();
    		}
    		GUILayout.EndHorizontal();
     
    		GUILayout.Space( 7 );
     
    		// Material map //
    		if( mMaterialMap.Count > 0 )
    		{
    			// Create the Headings for displaying the material map
    			GUILayout.BeginHorizontal();
    			GUILayout.Label( "Current Material", GUILayout.MaxWidth( 250 ) );
    			GUILayout.Label( "Replacement" );
    			GUILayout.EndHorizontal();
     
    			//Create scroll view for the materials
    			mScrollPosition = GUILayout.BeginScrollView( mScrollPosition );
    			GUILayout.BeginVertical();
     
    			// remember user change and apply it after drawing
    			KeyValuePair<Material, Material> transition = new KeyValuePair<Material, Material>( null, null );
     
    			foreach( KeyValuePair<Material, Material> pair in mMaterialMap )
    			{
    				GUILayout.BeginHorizontal();
    				GUILayout.Label( pair.Key.name, GUILayout.MaxWidth( 250 ) );
    				Material newValue = ( (Material)EditorGUILayout.ObjectField( "", pair.Key != pair.Value ? pair.Value : null, typeof(Material), false ) );
    				GUILayout.EndHorizontal();
     
    				// although this would override previous changes in the list only one change is expected per update
    				if( ( newValue != null ) && ( newValue != pair.Value ) )
    					transition = new KeyValuePair<Material, Material>( pair.Key, newValue );
    			}
     
    			// update material map with new transition
    			if( transition.Key != null )
    				mMaterialMap[transition.Key] = transition.Value;
     
    			GUILayout.EndScrollView();
    			GUILayout.EndVertical();
     
    			GUILayout.BeginHorizontal();
     
    			// load previously saved transitions from xml file
    			if( GUILayout.Button( "Load" ) )
    				loadMaterialMap();
     
    			// save the current material map to xml file
    			if( GUILayout.Button( "Save" ) )
    				saveMaterialMap();
     
    			GUILayout.EndHorizontal();
     
    			// Buttons //
    			if( GUILayout.Button( "Apply Changes" ) ) // Save the material changes
    			{
    				applyChanges();
    				resetMaterialMap();
    			}
    		}
    	}
     
    	// ------------------------------------------------------------------------
    	/// <summary>
    	/// Resets the material map.
    	/// </summary>
    	// ------------------------------------------------------------------------
    	private void resetMaterialMap()
    	{
    		mMaterialMap.Clear();
     
    		if( mObject != null )
    		{
    			GameObject instance;
    			if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
    				instance = (GameObject)PrefabUtility.InstantiatePrefab( mObject );
    			else
    				instance = mObject;
     
    			MeshRenderer [] meshRenderer = instance.GetComponentsInChildren<MeshRenderer>();
    			foreach( MeshRenderer renderer in meshRenderer )
    				foreach( Material mat in renderer.sharedMaterials )
    					if( mat != null )
    					{
    						mMaterialMap[mat] = mat;
    					}
    					else
    					{
    						Debug.Log( "Missing material in game object '" + renderer.name + "'" );
    					}
     
    			if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
    				UnityEngine.Object.DestroyImmediate( instance );
    		}
    	}
     
    	// ------------------------------------------------------------------------
    	/// <summary>
    	/// Save material map to xml file.
    	/// </summary>
    	// ------------------------------------------------------------------------
    	private void saveMaterialMap()
    	{
    		string path = EditorUtility.SaveFilePanel( "Save Material Map File", "", "material_map.xml", "xml" );
    		if( path != string.Empty )
    		{
    			StringBuilder sb = new StringBuilder().AppendLine("Saved Transitions:");
     
    			//Create the xml file for storing material changes
    			XmlDocument doc = new XmlDocument();
    			XmlElement transitionsNode = doc.CreateElement( "Transitions" );
    			doc.AppendChild( transitionsNode );
     
    			// save material changes to xml
    			foreach( KeyValuePair<Material,Material> pair in mMaterialMap )
    			{
    				if( pair.Key != pair.Value )
    				{
    					XmlElement materialNode = doc.CreateElement( "Material" );
     
    					XmlElement sourceNode = doc.CreateElement( "Original" );
    					sourceNode.InnerText = pair.Key.name;
     
    					XmlElement targetNode = doc.CreateElement( "Replacement" );
    					targetNode.InnerText = pair.Value.name;
     
    					materialNode.AppendChild( sourceNode );
    					materialNode.AppendChild( targetNode );
    					transitionsNode.AppendChild( materialNode );
     
    					sb.Append( "'" ).Append( sourceNode.InnerText ).Append( "' -> '" ).Append( targetNode.InnerText ).AppendLine("'");
    				}
    			}
    			doc.Save( path );
    			Debug.Log( sb.ToString() );
    		}
    	}
     
    	// ------------------------------------------------------------------------
    	/// <summary>
    	/// Load material map from xml file.
    	/// </summary>
    	// ------------------------------------------------------------------------
    	private void loadMaterialMap()
    	{
    		Dictionary<string,string> transitions = new Dictionary<string, string>();
     
    		string path = EditorUtility.OpenFilePanel( "Load Material Map File", "", "xml" );
    		if( path != string.Empty )
    		{
    			XmlDocument doc = new XmlDocument();
    			doc.Load( path );
     
    			XmlNodeList elemList = doc.GetElementsByTagName( "Material" );
     
    			foreach( XmlNode node in elemList )
    			{
    				if( ( node.ChildNodes.Count == 2 ) &&
    					( node.ChildNodes[0].Name == "Original" ) &&
    					( node.ChildNodes[1].Name == "Replacement" ) )
    				{
    					transitions.Add( node.ChildNodes[0].InnerText, node.ChildNodes[1].InnerText );
    				}
    			}
     
    			mapApplicableTransitions( transitions );
    		}
    	}
     
    	// ------------------------------------------------------------------------
    	/// <summary>
    	/// Apply material map settings to selected prefab.
    	/// </summary>
    	// ------------------------------------------------------------------------
    	public void applyChanges()
    	{
    		GameObject instance;
    		if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
    			instance = (GameObject)PrefabUtility.InstantiatePrefab( mObject );
    		else
    			instance = mObject;
     
    		MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();
     
    		// change the materials for all renderer in the Game Object
    		foreach( MeshRenderer renderer in renderers )
    		{
    			Material[] sharedMaterials = renderer.sharedMaterials;
    			for( int i = 0; i < sharedMaterials.Length; i++ )
    				if( sharedMaterials[i] != null )
    					sharedMaterials[i] = mMaterialMap[sharedMaterials[i]];
     
    			renderer.sharedMaterials = sharedMaterials;
    		}
     
    		if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
    		{
    			//Save the changes into the prefab
    			PrefabUtility.ReplacePrefab( instance, mObject, ReplacePrefabOptions.ConnectToPrefab );
     
    			//destroy the previously created instance
    			DestroyImmediate( instance );
    		}
    	}
     
    	// ------------------------------------------------------------------------
    	/// <summary>
    	/// For each transition check if the source material is used by the object and the target material exists.
    	/// If both is true the transition is applied in the material map.
    	/// </summary>
    	// ------------------------------------------------------------------------
    	private void mapApplicableTransitions( Dictionary<string,string> transitions )
    	{
    		// retrieve all materials in our project asset folder
    		string[] materialsInProject = Directory.GetFiles( "Assets\\", "*.mat", SearchOption.AllDirectories );
     
    		Dictionary<string, string> materialAssets = new Dictionary<string, string>();
    		foreach( string materialFile in materialsInProject )
    			materialAssets[Path.GetFileNameWithoutExtension( materialFile )] = materialFile;
     
    		// retrieve all substance archives in our project asset folder
    		string[] substancesInProject = Directory.GetFiles( "Assets\\", "*.sbsar", SearchOption.AllDirectories );
     
    		// cache all substance instances assets found in the dictionary since we had to load them anyway
    		Dictionary<string, ProceduralMaterial> substanceAssets = new Dictionary<string, ProceduralMaterial>();
    		foreach( string substanceFile in substancesInProject )
    		{
    			UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath( substanceFile.Replace( "\\", "/" ) );
    			foreach( UnityEngine.Object obj in objects )
    			{
    				ProceduralMaterial substance = obj as ProceduralMaterial;
    				if( substance != null )
    				{
    					try
    					{
    						substanceAssets.Add( substance.name, substance );
    					}
    					catch( ArgumentException )
    					{
    						Debug.LogError("Multiple occurences of substance '" + substance.name + "' found which will be ignored.");
    					}
    				}
    			}
    		}
     
    		// foreach object material check if a material transition with the material as source exist
    		StringBuilder sb = new StringBuilder().AppendLine( "Mapped Transitions:" );
    		foreach( Material mat in mMaterialMap.Keys )
    		{
    			string targetName;
    			if( transitions.TryGetValue( mat.name, out targetName ) )
    			{
    				Material targetMaterial = null;
     
    				// search for the target material
    				string materialAssetPath;
    				if( materialAssets.TryGetValue( targetName, out materialAssetPath ) )
    				{
    					targetMaterial = (Material)AssetDatabase.LoadAssetAtPath( materialAssetPath.Replace( "\\", "/" ), typeof(Material) );
    				}
    				else
    				{
    					// try to retrieve substance with the given target name
    					ProceduralMaterial substance;
    					if( substanceAssets.TryGetValue( targetName, out substance ) )
    						targetMaterial = substance;
    				}
     
    				if( targetMaterial != null )
    				{
    					mMaterialMap[mat] = targetMaterial;
    					sb.Append( "'" ).Append( mat.name ).Append( "' -> '" ).Append( targetMaterial.name ).AppendLine( "'" );
    				}
    				else
    					Debug.Log( "Material '" + targetName + "' specified in the transition file could not be found." );
    			}
    		}
    		Debug.Log( sb.ToString() );
    	}
}
}
