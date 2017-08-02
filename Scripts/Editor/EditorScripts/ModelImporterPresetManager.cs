/*************************
 * Original url: http://wiki.unity3d.com/index.php/ModelImporterPresetManager
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/ModelImporterPresetManager.cs
 * File based on original modification date of: 11 July 2012, at 19:23. 
 *
 * Author: Xadhoom 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Adds a new editor window at menu Custom→ModelImportManager. The editor window allows selecting/creating/editing/saving model import settings presets which are applied if a new model (e.g. from Fbx file) is imported into Unity. The script file contains two classes. The EditorWindow providing the functionality to manage the presets and the AssetPostProcessor which applies the selected settings. Presets and settings are stored in EditorPrefs. 
    Background information can be found here: http://trianglestrip.blogspot.de/2012/07/model-import-presets.html 
    Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
    Click on Custom->ModelImportManager 
    Change the import settings presented on the right as you wish and and press Save As providing a preset name on the right. By clicking on the presets at left they are loaded and will be applied on model import if the global Use Import Settings checkbox at the top is selected. 
    Note: The default preset cannot be overriden. Use this prefab to apply default import settings or save the changes made as a new preset. 
    
    
    C# - ModelImportManager.cs (Developed for Unity 3.5) using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
     
     
    /// <summary>
    /// Provides most of the settings of Unitys model import dialog in an editor window to define them before
    /// importing assets. Presets can be defined/selected/edited/saved and are stored in EditorPrefs.
    /// The saved parameters are applied on model import by the ModelImportProcessor script.
    /// </summary>
    public class ModelImportManager : EditorWindow
    {
    	/// PRIVATE ===============================================================
     
    	/// <summary>
    	/// Defines wether to use the selected import settings or not.
    	/// </summary>
    	private bool mUseImportSettings;
     
    	/// <summary>
    	/// Contains all preset names read from registry.
    	/// </summary>
    	private List<string> mPresetList;
     
    	/// <summary>
    	/// Name of the currently selected preset.
    	/// </summary>
    	private string mCurrentPresetName;
     
    	/// <summary>
    	/// Name of the new preset name when saving.
    	/// </summary>
    	private string mNewPresetName = "MySettings";
     
    	/// <summary>
    	/// Constant definition of the default preset name.
    	/// </summary>
    	private static readonly string mDefaultPresetName = "Default";
     
    	// Meshes //
     
    	/// <summary>
    	/// Global scale factor for importing.
    	/// </summary>
    	private float mGlobalScale;
     
    	/// <summary>
    	/// Mesh compression setting.
    	/// </summary>
    	private ModelImporterMeshCompression mMeshCompression;
     
    	/// <summary>
    	/// Vertex optimization setting
    	/// </summary>
    	private bool mOptimizeMesh;
     
    	/// <summary>
    	// Add mesh colliders to imported meshes.
    	/// </summary>
    	private bool mAddCollider;
     
    	/// <summary>
    	/// Swap primary and secondary UV channels when importing.
    	/// </summary>
    	private bool mSwapUVChannels;
     
    	/// <summary>
    	/// Generate secondary UV set for lightmapping.
    	/// </summary>
    	private bool mGenerateSecondaryUV ;
     
    	// Normals & Tangents //
     
    	/// <summary>
    	/// Normals import mode.
    	/// </summary>
    	private ModelImporterTangentSpaceMode mNormalImportMode;
     
    	/// <summary>
    	/// Tangents import mode.
    	/// </summary>
    	private ModelImporterTangentSpaceMode mTangentImportMode;
     
    	/// <summary>
    	// Smoothing angle for calculating normals.
    	/// </summary>
    	private float mNormalSmoothingAngle;
     
    	/// <summary>
    	/// Should tangents be split across UV seams.
    	/// </summary>
    	private bool mSplitTangentsAcrossSeams;
     
    	// Materials //
     
    	/// <summary>
    	/// Import materials from file.
    	/// </summary>
    	private bool mImportMaterials;
     
    	/// <summary>
    	/// Material naming setting.
    	/// </summary>
    	private ModelImporterMaterialName mMaterialName;
     
    	/// <summary>
    	/// Existing material search setting.
    	/// </summary>
    	private ModelImporterMaterialSearch mMaterialSearch;
     
    	/// METHODS ===============================================================
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Adds menu named "ModelImportManager" to the "Custom" menu which creates and initializes a new instance of this class.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	[MenuItem("Custom/ModelImportManager")]
    	public static void Init()
    	{
    		ModelImportManager win = EditorWindow.GetWindow( typeof(ModelImportManager) ) as ModelImportManager;
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
     
    		GUI.SetNextControlName( "ToggleGroup" );
    		bool useImportSettings = EditorGUILayout.BeginToggleGroup( "Use '" + mCurrentPresetName + "' Import Settings", mUseImportSettings );
    		if( useImportSettings != mUseImportSettings )
    		{
    			mUseImportSettings = useImportSettings;
    			EditorPrefs.SetBool( "ModelImportManager.UseImportSettings", mUseImportSettings );
    		}
     
    		EditorGUILayout.Space();
    		GUILayout.BeginHorizontal();
    		EditorGUILayout.Space();
    		drawPresetList();
    		EditorGUILayout.Space();
    		drawSettingDialog();
    		EditorGUILayout.Space();
    		GUILayout.EndHorizontal();
     
    		EditorGUILayout.EndToggleGroup();
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Draws the preset list.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void drawPresetList()
    	{
    		GUILayout.BeginVertical( GUILayout.Width( 120 ) );
     
    		GUILayout.Label( "Presets", EditorStyles.boldLabel );
     
    		// default
    		if( GUILayout.Button( mDefaultPresetName, mCurrentPresetName == mDefaultPresetName ? EditorStyles.toolbarButton : EditorStyles.miniButtonMid ) )
    		{
    			reset();
    			GUI.FocusControl( "ToggleGroup" );
    		}
     
    		// all custom presets
    		foreach( string presetName in mPresetList )
    		{
    			if( GUILayout.Button( presetName, presetName == mCurrentPresetName ? EditorStyles.toolbarButton : EditorStyles.miniButtonMid ) )
    			{
    				loadPreset( presetName );
    				GUI.FocusControl( "ToggleGroup" );
    			}
    		}
     
    		GUILayout.EndVertical();
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Draws the setting dialog.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void drawSettingDialog()
    	{
    		GUILayout.BeginVertical( GUILayout.MinWidth( 300 ), GUILayout.MaxWidth( 1000 ), GUILayout.ExpandWidth( true ) );
     
    		GUILayout.Label( "Meshes", EditorStyles.boldLabel );
     
    		// Global scale factor for importing.
    		mGlobalScale = EditorGUILayout.FloatField( "Scale Factor", mGlobalScale );
     
    		// Mesh compression setting.
    		mMeshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup( "Mesh Compression", mMeshCompression );
     
    		// Vertex optimization setting
    		mOptimizeMesh = EditorGUILayout.Toggle( "Optimize Mesh", mOptimizeMesh );
     
    		// Add mesh colliders to imported meshes.
    		mAddCollider = EditorGUILayout.Toggle( "Generate Colliders", mAddCollider );
     
    		// Swap primary and secondary UV channels when importing.
    		mSwapUVChannels = EditorGUILayout.Toggle( "Swap UVs", mSwapUVChannels );
     
    		// Generate secondary UV set for lightmapping.
    		mGenerateSecondaryUV = EditorGUILayout.Toggle( "Generate Lightmap UVs", mGenerateSecondaryUV );
     
     
    		EditorGUILayout.Space();
    		GUILayout.Label( "Normals & Tangents", EditorStyles.boldLabel );
     
    		// Normals import mode.
    		mNormalImportMode = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup( "Normals", mNormalImportMode );
     
    		// Tangents import mode.
    		mTangentImportMode = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup( "Tangents", mTangentImportMode );
     
    		EditorGUI.BeginDisabledGroup( mNormalImportMode != ModelImporterTangentSpaceMode.Calculate );
     
    		// Smoothing angle for calculating normals.
    		mNormalSmoothingAngle = (int)EditorGUILayout.IntSlider( "Smoothing Angle", (int)mNormalSmoothingAngle, 0, 180 );
     
    		EditorGUI.EndDisabledGroup();
     
    		// Should tangents be split across UV seams.
    		mSplitTangentsAcrossSeams = EditorGUILayout.Toggle( "Split Tangents", mSplitTangentsAcrossSeams );
     
     
    		EditorGUILayout.Space();
    		GUILayout.Label( "Materials", EditorStyles.boldLabel );
     
    		// Import materials from file.
    		mImportMaterials = EditorGUILayout.Toggle( "Import Materials", mImportMaterials );
     
    		EditorGUI.BeginDisabledGroup( !mImportMaterials );
     
    		// Material naming setting.
    		mMaterialName = (ModelImporterMaterialName)EditorGUILayout.EnumPopup( "Material Naming", mMaterialName );
     
    		// Existing material search setting.
    		mMaterialSearch = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup( "Material Search", mMaterialSearch );
     
    		EditorGUI.EndDisabledGroup();
     
    		EditorGUILayout.Space();
    		EditorGUILayout.BeginHorizontal();
    		if( mCurrentPresetName != mDefaultPresetName )
    		{
    			if( GUILayout.Button( "Save" ) )
    				savePreset( mCurrentPresetName );
     
    			if( GUILayout.Button( "Delete" ) )
    				deletePreset( mCurrentPresetName );
    		}
    		else
    		{
    			if( GUILayout.Button( "Save As", GUILayout.Width( 200 ) ) && ( mNewPresetName.Length > 0 ) && ( mNewPresetName != mDefaultPresetName ) )
    			{
    				savePreset( mNewPresetName );
    				loadPreset( mNewPresetName );
    			}
     
    			// only characters allowed
    			mNewPresetName = Regex.Replace( GUILayout.TextField( mNewPresetName ), @"[^\w]", string.Empty );
    		}
    		EditorGUILayout.EndHorizontal();
     
    		GUILayout.EndVertical();
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Loads the preset list from registry.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void loadPresetList()
    	{
    		mPresetList = new List<string>();
     
    		// try to read the list of presets
    		string presetListStr = EditorPrefs.GetString( "ModelImportManager.Presets", string.Empty );
    		if( presetListStr != string.Empty )
    		{
    			string[] presetList = presetListStr.Split( ';' );
    			mPresetList.AddRange( presetList );
    		}
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Saves the preset list to registry.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void savePresetList()
    	{
    		StringBuilder presetListStr = new StringBuilder();
     
    		if( mPresetList.Count > 0 )
    		{
    			foreach( string preset in mPresetList )
    				presetListStr.Append( preset ).Append( ';' );
     
    			EditorPrefs.SetString( "ModelImportManager.Presets", presetListStr.ToString( 0, presetListStr.Length - 1 ) );
    		}
    		else
    		{
    			EditorPrefs.SetString( "ModelImportManager.Presets", string.Empty );
    		}
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Save the current settings to Registry.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void savePreset( string presetName )
    	{
    		string prefix = "ModelImportManager." + presetName;
     
    		// Meshes
    		EditorPrefs.SetFloat( prefix + ".GlobalScale", mGlobalScale );
    		EditorPrefs.SetString( prefix + ".MeshCompression", mMeshCompression.ToString() );
    		EditorPrefs.SetBool( prefix + ".OptimizeMesh", mOptimizeMesh );
    		EditorPrefs.SetBool( prefix + ".AddCollider", mAddCollider );
    		EditorPrefs.SetBool( prefix + ".SwapUVChannels", mSwapUVChannels );
    		EditorPrefs.SetBool( prefix + ".GenerateSecondaryUV", mGenerateSecondaryUV );
     
    		// Normals & Tangents
    		EditorPrefs.SetString( prefix + ".NormalImportMode", mNormalImportMode.ToString() );
    		EditorPrefs.SetString( prefix + ".TangentImportMode", mTangentImportMode.ToString() );
    		EditorPrefs.SetFloat( prefix + ".NormalSmoothingAngle", mNormalSmoothingAngle );
    		EditorPrefs.SetBool( prefix + ".SplitTangentsAcrossSeams", mSplitTangentsAcrossSeams );
     
    		// Materials
    		EditorPrefs.SetBool( prefix + ".ImportMaterials", mImportMaterials );
    		EditorPrefs.SetString( prefix + ".MaterialName", mMaterialName.ToString() );
    		EditorPrefs.SetString( prefix + ".MaterialSearch", mMaterialSearch.ToString() );
     
    		// add preset to preset list if its a new one
    		if( !mPresetList.Contains( presetName ) )
    		{
    			mPresetList.Add( presetName );
    			savePresetList();
    		}
     
    		Debug.Log( "ModelImportManager::savePreset, Settings have been saved to preset '" + presetName + "'." );
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Load settings from Registry.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void loadPreset( string presetName )
    	{
    		string prefix = "ModelImportManager." + presetName;
     
    		// Meshes
    		mGlobalScale = EditorPrefs.GetFloat( prefix + ".GlobalScale", 1.0f );
    		mMeshCompression = (ModelImporterMeshCompression)System.Enum.Parse( typeof(ModelImporterMeshCompression),
    								EditorPrefs.GetString( prefix + ".MeshCompression", "Off" ) );
    		mOptimizeMesh = EditorPrefs.GetBool( prefix + ".OptimizeMesh", false );
    		mAddCollider = EditorPrefs.GetBool( prefix + ".AddCollider", false );
    		mSwapUVChannels = EditorPrefs.GetBool( prefix + ".SwapUVChannels", false );
    		mGenerateSecondaryUV = EditorPrefs.GetBool( prefix + ".GenerateSecondaryUV", false );
     
    		// Normals & Tangents
    		mNormalImportMode = (ModelImporterTangentSpaceMode)System.Enum.Parse( typeof(ModelImporterTangentSpaceMode),
    								EditorPrefs.GetString( prefix + ".NormalImportMode", "Import" ) );
    		mTangentImportMode = (ModelImporterTangentSpaceMode)System.Enum.Parse( typeof(ModelImporterTangentSpaceMode),
    								EditorPrefs.GetString( prefix + ".TangentImportMode", "Calculate" ) );
    		mNormalSmoothingAngle = EditorPrefs.GetFloat( prefix + ".NormalSmoothingAngle", 60.0f );
    		mSplitTangentsAcrossSeams = EditorPrefs.GetBool( prefix + ".SplitTangentsAcrossSeams", false );
     
    		// Materials
    		mImportMaterials = EditorPrefs.GetBool( prefix + ".ImportMaterials", false );
    		mMaterialName = (ModelImporterMaterialName)System.Enum.Parse( typeof(ModelImporterMaterialName),
    								EditorPrefs.GetString( prefix + ".MaterialName", "BasedOnTextureName" ) );
    		mMaterialSearch = (ModelImporterMaterialSearch)System.Enum.Parse( typeof(ModelImporterMaterialSearch),
    								EditorPrefs.GetString( prefix + ".MaterialSearch", "RecursiveUp" ) );
     
    		// remember current preset selection
    		mCurrentPresetName = presetName;
    		EditorPrefs.SetString( "ModelImportManager.CurrentPreset", mCurrentPresetName );
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Remove all setting information from registry.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void deletePreset( string presetName )
    	{
    		string prefix = "ModelImportManager." + presetName;
     
    		EditorPrefs.DeleteKey( prefix + ".GlobalScale" );
    		EditorPrefs.DeleteKey( prefix + ".MeshCompression" );
    		EditorPrefs.DeleteKey( prefix + ".OptimizeMesh" );
    		EditorPrefs.DeleteKey( prefix + ".AddCollider" );
    		EditorPrefs.DeleteKey( prefix + ".SwapUVChannels" );
    		EditorPrefs.DeleteKey( prefix + ".GenerateSecondaryUV" );
    		EditorPrefs.DeleteKey( prefix + ".NormalImportMode" );
    		EditorPrefs.DeleteKey( prefix + ".TangentImportMode" );
    		EditorPrefs.DeleteKey( prefix + ".NormalSmoothingAngle" );
    		EditorPrefs.DeleteKey( prefix + ".SplitTangentsAcrossSeams" );
    		EditorPrefs.DeleteKey( prefix + ".ImportMaterials" );
    		EditorPrefs.DeleteKey( prefix + ".MaterialName" );
    		EditorPrefs.DeleteKey( prefix + ".MaterialSearch" );
     
    		mPresetList.Remove( presetName );
    		savePresetList();
    		reset();
     
    		Debug.Log( "ModelImportManager::deletePreset, Preset '" + presetName + "' has been deleted." );
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Resets settings to default values.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void reset()
    	{
    		// Meshes
    		mGlobalScale = 1.0f;
    		mMeshCompression = ModelImporterMeshCompression.Off;
    		mOptimizeMesh = false;
    		mAddCollider = false;
    		mSwapUVChannels = false;
    		mGenerateSecondaryUV = false;
     
    		// Normals & Tangents
    		mNormalImportMode = ModelImporterTangentSpaceMode.Import;
    		mTangentImportMode = ModelImporterTangentSpaceMode.Calculate;
    		mNormalSmoothingAngle = 60.0f;
    		mSplitTangentsAcrossSeams = false;
     
    		// Materials
    		mImportMaterials = true;
    		mMaterialName = ModelImporterMaterialName.BasedOnTextureName;
    		mMaterialSearch = ModelImporterMaterialSearch.RecursiveUp;
     
     
    		// remember current preset selection
    		mCurrentPresetName = mDefaultPresetName;
    		EditorPrefs.SetString( "ModelImportManager.CurrentPreset", mCurrentPresetName );
    	}
     
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Init this instance.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void init()
    	{
    		loadPresetList();
     
    		mUseImportSettings = EditorPrefs.GetBool( "ModelImportManager.UseImportSettings", true );
     
    		string currentPresetName = EditorPrefs.GetString( "ModelImportManager.CurrentPreset", mDefaultPresetName );
    		loadPreset( currentPresetName );
    	}
    }
     
    /// ---------------------------------------------------------------------------
    /// <summary>
    /// Import script applies model import settings based on the settings made in the ModelImportManager window.
    /// </summary>
    /// ---------------------------------------------------------------------------
    public class ModelImportProcessor : AssetPostprocessor
    {
    	/// -----------------------------------------------------------------------
    	/// <summary>
    	/// Applies the import settings of the model based on ModelImportManager settings.
    	/// </summary>
    	/// -----------------------------------------------------------------------
    	private void OnPreprocessModel()
    	{
    		ModelImporter modelImporter = assetImporter as ModelImporter;
    		if( modelImporter != null )
    		{
    			bool useImportSettings = EditorPrefs.GetBool( "ModelImportManager.UseImportSettings", false );
    			if( useImportSettings )
    			{
    				string presetName = EditorPrefs.GetString( "ModelImportManager.CurrentPreset", string.Empty );
     
    				string prefix = "ModelImportManager." + presetName;
     
    				// Meshes
    				modelImporter.globalScale = EditorPrefs.GetFloat( prefix + ".GlobalScale", 1.0f );
    				modelImporter.meshCompression = (ModelImporterMeshCompression)System.Enum.Parse( typeof(ModelImporterMeshCompression),
    												EditorPrefs.GetString( prefix + ".MeshCompression", "Off" ) );
    				modelImporter.optimizeMesh = EditorPrefs.GetBool( prefix + ".OptimizeMesh", false );
    				modelImporter.addCollider = EditorPrefs.GetBool( prefix + ".AddCollider", false );
    				modelImporter.swapUVChannels = EditorPrefs.GetBool( prefix + ".SwapUVChannels", false );
    				modelImporter.generateSecondaryUV = EditorPrefs.GetBool( prefix + ".GenerateSecondaryUV", false );
     
    				// Normals & Tangents
    				modelImporter.normalImportMode = (ModelImporterTangentSpaceMode)System.Enum.Parse( typeof(ModelImporterTangentSpaceMode),
    								EditorPrefs.GetString( prefix + ".NormalImportMode", "Import" ) );
    				modelImporter.tangentImportMode = (ModelImporterTangentSpaceMode)System.Enum.Parse( typeof(ModelImporterTangentSpaceMode),
    								EditorPrefs.GetString( prefix + ".TangentImportMode", "Calculate" ) );
    				modelImporter.normalSmoothingAngle = EditorPrefs.GetFloat( prefix + ".NormalSmoothingAngle", 60.0f );
    				modelImporter.splitTangentsAcrossSeams = EditorPrefs.GetBool( prefix + ".SplitTangentsAcrossSeams", false );
     
    				// Materials
    				modelImporter.importMaterials = EditorPrefs.GetBool( prefix + ".ImportMaterials", false );
    				modelImporter.materialName = (ModelImporterMaterialName)System.Enum.Parse( typeof(ModelImporterMaterialName),
    								EditorPrefs.GetString( prefix + ".MaterialName", "BasedOnTextureName" ) );
    				modelImporter.materialSearch = (ModelImporterMaterialSearch)System.Enum.Parse( typeof(ModelImporterMaterialSearch),
    								EditorPrefs.GetString( prefix + ".MaterialSearch", "RecursiveUp" ) );
     
    				Debug.Log( "ModelImportProcessor::OnPreprocessModel, Using custom import settings of preset '" + presetName + "'." );
    			}
    		}
    	}
}
}
