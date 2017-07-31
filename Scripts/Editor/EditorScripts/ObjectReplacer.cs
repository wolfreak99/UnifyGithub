// Original url: http://wiki.unity3d.com/index.php/ObjectReplacer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/ObjectReplacer.cs
// File based on original modification date of: 6 November 2012, at 22:40. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Xadhoom 
Contents [hide] 
1 Description 
2 Usage 
3 Known Issues 
4 C# - ObjectReplacer.cs (Developed for Unity 3.5) 

Description Adds a new editor window at menu Custom→Replace Objects. The editor window allows easy replacement of child objects by name by a selected prefab. The editor window expects the object to be dropped in the appropriate object field. A list of transitions can be configured. For each transition its possible to define how the source/replacement transition should be handled. A quick check list on the right side shows which objects will be replaced. The "save"/"load" button manages external transition files which can be saved or loaded here. Finally the Apply button changes replaces the prefab accordingly. 
Background information can be found here: http://trianglestrip.blogspot.de/2012/07/object-replacer-editor-script.html 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Click on Custom→Replace Objects 
Drop a prefab (not a model file) onto the given object field or select one in the object field. 
Define the object names you would like to replace (you can use wildcards) and choose by which prefab it should be replaced. The three buttons on the right of each transition allow to define how to compute the final transition of the replacement. The right most button calculates which child objects will be replaced. 
To save the transitions defined press the Save button in the window. 
To load transitions from a file press the Load button in the window. 
To eventually apply the replacements to the prefab press the Apply button. 
Known Issues The script applies each transition one after another from top to bottom. If an objects name would match with several of the transitions the uppermost transition in the list will be applied only. 
If an object replacement is attached for a matching object it might be matched by following transitions and replaced again. 
Solution for both is to use clearly distinguishable object names for your source and target object(s). 
If a matching object has children they are removed implicitly ignoring them for further processing. 
C# - ObjectReplacer.cs (Developed for Unity 3.5) using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
 
 
	/// -----------------------------------------------------------------------
/// <summary>
/// If an FBX file contains meshes which need to be replaced this can be achieved using the this editor class.
/// The script replaces the objects present in a given prefab or object instance. The object is given as
/// the input and as soon as it is placed in the object field. Its possible to create transitions defining
/// the source and target name where the target needs to be a prefab in your asset folder. The source name
/// supports wildcards. For every transition the user can specify the transform behaviour type. Either using
/// the source, target or combined source/target transformation.
/// Once transitions have been defined the following three operations can be done:
/// 1. Save - Saves the object transitions made into an XML file which can be reused later.
/// 2. Load - Update the transitions for the current object with the transitions loaded from the given transition file.
/// 3. Apply Changes - Replaces all objects (and their children) with the prefab defined in the uppermost matching transition.
/// </summary>
	/// -----------------------------------------------------------------------
public class ObjectReplacer : EditorWindow
{
	/// PRIVATE ===============================================================
 
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Encapsulates data of one transition.
	/// </summary>
	/// -----------------------------------------------------------------------
	private class Transition
	{
		/// PUBLIC ============================================================
 
		/// <summary>
		/// The object name to replace.
		/// </summary>
		public string Name;
 
		/// <summary>
		/// The replacement game object.
		/// </summary>
		public GameObject Replacement;
 
		/// <summary>
		/// The transformation type.
		/// </summary>
		public int TransformationType;
 
		/// METHODS ===========================================================
 
		/// -------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectReplacer.Transition"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name of the object to be replaced
		/// </param>
		/// <param name='replacement'>
		/// Prefab reference of the replacement.
		/// </param>
		/// <param name='transformation'>
		/// Transformation type. 0 = "source", 1 = "target", 2 = "combined"
		/// </param>
		/// -------------------------------------------------------------------
		public Transition( string name, GameObject replacement, int transformationType )
		{
			Name = name;
			Replacement = replacement;
			TransformationType = transformationType;
		}
	}
 
	/// <summary>
	/// The possible transformation calculation types.
	/// </summary>
	private readonly GUIContent[] mTransformationTypes = { new GUIContent("S", "Use source transformation."),
															new GUIContent( "T", "Use target transformation."),
															new GUIContent("C", "Combine source and target transformation.")};
 
	/// <summary>
	/// Stores all object transitions.
	/// </summary>
	private List< Transition > mTransitionList = new List< Transition >();
 
	/// <summary>
	/// The current object list.
	/// </summary>
	private List<string> mObjectList = new List<string>();
 
	/// <summary>
	/// The currently selected object.
	/// </summary>
	private GameObject mObject;
 
	/// <summary>
	/// Transition list scroll position.
	/// </summary>
	private Vector2 mTransitionListScrollPos = new Vector2( 0, 0 );
 
	/// <summary>
	/// Object list scroll position.
	/// </summary>
	private Vector2 mObjectListScrollPos = new Vector2( 0, 0 );
 
	/// METHODS ===============================================================
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Init this instance.
	/// </summary>
	// ------------------------------------------------------------------------
	[MenuItem ("Custom/Replace Objects")]
	public static void Init()
	{
		// Get existing open window or if none, make a new one:
		ObjectReplacer win = (ObjectReplacer)EditorWindow.GetWindow( typeof(ObjectReplacer) );
		win.position = new Rect( 200, 200, 750, 430 );
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// OnGUI renders the editor window.
	/// </summary>
	// ------------------------------------------------------------------------
	private void OnGUI()
	{
		//create an object field for the prefab/object
		GUILayout.BeginHorizontal();
		GameObject obj = (GameObject)EditorGUILayout.ObjectField( "Select or drop object:", mObject, typeof(GameObject), true );
 
		// if something changed
		if( obj != mObject )
		{
			// ignore model prefabs
			if( PrefabUtility.GetPrefabType( obj ) == PrefabType.ModelPrefab )
			{
				Debug.LogError( "ObjectReplacer, Object cannot be a model file. Please create a prefab first or select an instance from scene" );
				mObject = null;
			}
			else
			{
				mObject = obj;
			}
 
 
			mObjectList.Clear();
			mObjectListScrollPos = Vector2.zero;
		}
		GUILayout.EndHorizontal();
 
		GUILayout.Space( 7 );
 
		GUILayout.BeginHorizontal();
 
		GUILayout.BeginVertical();
 
		// Create the Headings for displaying the transition list
		GUILayout.BeginHorizontal();
		GUILayout.Label( "ObjectName", GUILayout.MaxWidth( 200 ), GUILayout.ExpandWidth( true ) );
		GUILayout.Label( "Replacement", GUILayout.MaxWidth( 200 ), GUILayout.ExpandWidth( true ) );
		GUILayout.Space( 10 );
		GUILayout.Label( "Transform", GUILayout.Width( 80 ) );
		GUILayout.Space( 50 );
		GUILayout.EndHorizontal();
 
		// Create scroll view for the transitions
		mTransitionListScrollPos = GUILayout.BeginScrollView( mTransitionListScrollPos, false, true, GUILayout.ExpandWidth( true ) );
		GUILayout.BeginVertical();
 
		for( int i = 0; i < mTransitionList.Count; i++ )
		{
			Transition transition = mTransitionList[i];
			GUILayout.BeginHorizontal();
			transition.Name = GUILayout.TextField( transition.Name, GUILayout.MaxWidth( 200 ), GUILayout.ExpandWidth( true ) );
			GameObject newReplacement = ( (GameObject)EditorGUILayout.ObjectField( new GUIContent( "", "Prefab replacing all objects mathing the name." ), transition.Replacement, typeof(GameObject), false, GUILayout.MaxWidth( 200 ), GUILayout.ExpandWidth( true ) ) );
			GUILayout.Space( 10 );
			transition.TransformationType = GUILayout.Toolbar( transition.TransformationType, mTransformationTypes, GUILayout.Width( 80 ) );
 
			if( GUILayout.Button( new GUIContent( ">", "List matching objects." ), GUILayout.Width( 25 ) ) && ( mObject != null ) )
			{
				Wildcard wildcard = new Wildcard( transition.Name, RegexOptions.IgnoreCase );
 
				HashSet<string> objectSet = new HashSet<string>();
				addObjectsToSet( mObject.transform, wildcard, objectSet );
 
				mObjectList.Clear();
				mObjectList.AddRange( objectSet );
				mObjectList.Sort();
			}
 
			GUILayout.EndHorizontal();
 
			if( newReplacement == transition.Replacement )
				continue;
 
			if( ( newReplacement != null ) && ( PrefabUtility.GetPrefabType( newReplacement ) != PrefabType.Prefab ) )
			{
				Debug.LogError( "ObjectReplacer, Given object needs to be a prefab." );
				continue;
			}
 
			transition.Replacement = newReplacement;
		}
 
		if( GUILayout.Button( "+" ) )
		{
			mTransitionList.Add( new Transition( "", null, 0 ) );
		}
 
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
 
		// object list
		GUILayout.BeginVertical( GUILayout.MinWidth( 200 ), GUILayout.ExpandWidth( true ) );
		GUILayout.Label( "Replaceable Objects", GUILayout.MinWidth( 200 ), GUILayout.ExpandWidth( true ) );
		mObjectListScrollPos = GUILayout.BeginScrollView( mObjectListScrollPos, false, true, GUILayout.MinWidth( 200 ), GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
		foreach( string objName in mObjectList )
			GUILayout.Label( objName );
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
 
		GUILayout.EndHorizontal();
 
		GUILayout.BeginHorizontal();
 
		// load previously saved transitions from xml file
		if( GUILayout.Button( "Load" ) )
			loadTransitionList();
 
		// save the current transition map to xml file
		if( GUILayout.Button( "Save" ) )
			saveTransitionList();
 
		GUILayout.EndHorizontal();
 
		// Buttons //
		if( GUILayout.Button( "Apply Changes" ) && ( mObject != null ) ) // Save the transition changes
			applyChanges();
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Save transition map to xml file.
	/// </summary>
	// ------------------------------------------------------------------------
	private void saveTransitionList()
	{
		string path = EditorUtility.SaveFilePanel( "Save Object Map File", "", "object_map.xml", "xml" );
		if( path != string.Empty )
		{
			StringBuilder sb = new StringBuilder().AppendLine( "Saved Transitions:" );
 
			//Create the xml file for storing transition changes
			XmlDocument doc = new XmlDocument();
			XmlElement transitionListNode = doc.CreateElement( "TransitionList" );
			doc.AppendChild( transitionListNode );
 
			// save object transitions to xml
			foreach( Transition transition in mTransitionList )
			{
				XmlElement transitionNode = doc.CreateElement( "Transition" );
 
				XmlElement nameNode = doc.CreateElement( "Name" );
				nameNode.InnerText = transition.Name;
 
				XmlElement replacementNode = doc.CreateElement( "Replacement" );
				replacementNode.InnerText = transition.Replacement != null ? transition.Replacement.name : "null";
 
				XmlElement transformationTypeNode = doc.CreateElement( "TransformationType" );
				transformationTypeNode.InnerText = transition.TransformationType.ToString();
 
				transitionNode.AppendChild( nameNode );
				transitionNode.AppendChild( replacementNode );
				transitionNode.AppendChild( transformationTypeNode );
				transitionListNode.AppendChild( transitionNode );
 
				sb.Append( "'" ).Append( nameNode.InnerText ).Append( "' -> '" ).Append( replacementNode.InnerText ).AppendLine( "'" );
			}
			doc.Save( path );
			Debug.Log( sb.ToString() );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Load transition map from xml file.
	/// </summary>
	// ------------------------------------------------------------------------
	private void loadTransitionList()
	{
		// list stores loaded transitions and the replacement object as string for later mapping
		List< KeyValuePair<Transition, string> > transitionList = new List< KeyValuePair<Transition, string> >();
 
		string path = EditorUtility.OpenFilePanel( "Load Object Map File", "", "xml" );
		if( path != string.Empty )
		{
			XmlDocument doc = new XmlDocument();
			doc.Load( path );
 
			XmlNodeList elemList = doc.GetElementsByTagName( "Transition" );
 
			foreach( XmlNode node in elemList )
			{
				if( ( node.ChildNodes.Count == 3 ) &&
					( node.ChildNodes[0].Name == "Name" ) &&
					( node.ChildNodes[1].Name == "Replacement" ) &&
					( node.ChildNodes[2].Name == "TransformationType" ) )
				{
					Transition transition = new Transition( node.ChildNodes[0].InnerText, null, int.Parse( node.ChildNodes[2].InnerText ) );
					transitionList.Add( new KeyValuePair<Transition, string>( transition, node.ChildNodes[1].InnerText ) );
				}
			}
 
			mapAvailablePrefabs( transitionList );
 
			// finally set the transitions in our transition list
			mTransitionList.Clear();
			foreach( KeyValuePair<Transition, string> transition in transitionList )
				mTransitionList.Add( transition.Key );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Apply object replacement settings to selected prefab.
	/// </summary>
	// ------------------------------------------------------------------------
	private void applyChanges()
	{
		foreach( Transition transition in mTransitionList )
		{
			Wildcard wildcard = new Wildcard( transition.Name, RegexOptions.IgnoreCase );
			replaceObject( mObject.transform, transition, wildcard );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// For each transition check if the replacement prefab asset with the given name is present in the projects asset directory.
	/// Load the prefab and add its reference to the transition object.
	/// </summary>
	// ------------------------------------------------------------------------
	private void mapAvailablePrefabs( List< KeyValuePair<Transition, string> > transitionList )
	{
		// retrieve all prefabs in our project asset folder
		string[] prefabsInProject = Directory.GetFiles( "Assets\\", "*.prefab", SearchOption.AllDirectories );
 
		Dictionary<string, string> prefabAssets = new Dictionary<string, string>();
		foreach( string prefabFile in prefabsInProject )
			prefabAssets[Path.GetFileNameWithoutExtension( prefabFile )] = prefabFile;
 
		// foreach transition check if a prefab exists and add it to the transition
		foreach( KeyValuePair<Transition, string> transition in transitionList )
		{
			string prefabName = transition.Value;
 
			if( prefabName == "null" )
				continue;
 
			// search for the prefab
			string prefabAssetPath;
			if( prefabAssets.TryGetValue( prefabName, out prefabAssetPath ) )
			{
				transition.Key.Replacement = (GameObject)AssetDatabase.LoadAssetAtPath( prefabAssetPath.Replace( "\\", "/" ), typeof(GameObject) );
			}
			else
				Debug.Log( "Prefab '" + prefabName + "' specified in the transition file could not be found." );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Replaces the object.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='transition'>
	/// Transition.
	/// </param>
	/// <param name='wildcard'>
	/// Wildcard.
	/// </param>
	// ------------------------------------------------------------------------
	private void replaceObject( Transform obj, Transition transition, Wildcard wildcard )
	{
		if( wildcard.IsMatch( obj.name ) )
		{
			// we have to replace the object
			if( transition.Replacement == null )
			{
				DestroyImmediate( obj.gameObject, true );
			}
			else
			{
				Quaternion localRotation = Quaternion.identity;
				Vector3 localPosition = Vector3.zero;
				Vector3 localScale = Vector3.zero;
 
				switch( transition.TransformationType )
				{
				case 0: // source
					{
						localRotation = obj.localRotation;
						localPosition = obj.localPosition;
						localScale = obj.localScale;
					}
					break;
				case 1: // target
					{
						localRotation = transition.Replacement.transform.localRotation;
						localPosition = transition.Replacement.transform.localPosition;
						localScale = transition.Replacement.transform.localScale;
					}
					break;
				case 2: // combine
					{
						localRotation = transition.Replacement.transform.localRotation * obj.localRotation;
						localPosition = obj.localRotation * transition.Replacement.transform.localPosition + obj.localPosition;
						localScale = Vector3.Scale( transition.Replacement.transform.localScale, obj.localScale );
					}
					break;
				}
 
				Transform parent = obj.parent;
				DestroyImmediate( obj.gameObject, true );
 
				GameObject replacement = Instantiate( transition.Replacement ) as GameObject;
				replacement.name = transition.Replacement.name;
				replacement.transform.parent = parent;
				replacement.transform.localRotation = localRotation;
				replacement.transform.localPosition = localPosition;
				replacement.transform.localScale = localScale;
			}
		}
		else
		{
			// we have to use a separat list here because the internal child list might change while iterating
			List<Transform> children = new List<Transform>( obj.childCount );
			foreach( Transform child in obj )
				children.Add( child );
 
			foreach( Transform child in children )
				replaceObject( child, transition, wildcard );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Recursive method adds objects to goList if their name matches the wildcard.
	/// </summary>
	/// <param name='obj'>
	/// The current object to check.
	/// </param>
	/// <param name='wildcard'>
	/// The wildcard used for matching.
	/// </param>
	/// <param name='goList'>
	/// Holds all matching object transform.
	/// </param>
	// ------------------------------------------------------------------------
	private void addObjectsToSet( Transform obj, Wildcard wildcard, HashSet<string> objectSet )
	{
		if( wildcard.IsMatch( obj.name ) )
			objectSet.Add( obj.name );
 
		foreach( Transform child in obj )
			addObjectsToSet( child, wildcard, objectSet );
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Represents a wildcard running on the
	/// <see cref="System.Text.RegularExpressions"/> engine.
	/// This code has been taken from here: http://www.codeproject.com/Articles/11556/Converting-Wildcards-to-Regexes
	/// </summary>
	// ------------------------------------------------------------------------
	private class Wildcard : Regex
	{
		/// METHODS ===========================================================
 
		// --------------------------------------------------------------------
		/// <summary>
		/// Initializes a wildcard with the given search pattern and options.
		/// </summary>
		/// <param name="pattern">The wildcard pattern to match.</param>
		/// <param name="options">A combination of one or more
		/// <see cref="System.Text.RegexOptions"/>.</param>
		// --------------------------------------------------------------------
		public Wildcard( string pattern, RegexOptions options = RegexOptions.None ) : base(WildcardToRegex(pattern), options)
		{
		}
 
		// --------------------------------------------------------------------
		/// <summary>
		/// Converts a wildcard to a regex.
		/// </summary>
		/// <param name="pattern">The wildcard pattern to convert.</param>
		/// <returns>A regex equivalent of the given wildcard.</returns>
		// --------------------------------------------------------------------
		public static string WildcardToRegex( string pattern )
		{
			return "^" + Regex.Escape( pattern ).Replace( "\\*", ".*" ).Replace( "\\?", "." ) + "$";
		}
	}
}
}
