/*************************
 * Original url: http://wiki.unity3d.com/index.php/Tile_Material_Creation_Window
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/Tile_Material_Creation_Window.cs
 * File based on original modification date of: 15 December 2012, at 20:57. 
 *
 * Author 
 *   
 * Description 
 *   
 * How to use 
 *   
 * Source Code 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    Author Dean Lunz (aka Created by: X)
    createdbyx@createdbyx.com
    http://www.createdbyx.com/
    Tile Material Creation Window 
    Description Provides a tool for selecting a tile from a tile set and creating a material from that tile. 
    How to use Download the TileMaterialCreationWindow.zip file and extract the *.unitypackage file within it. 
    Import the *.unitypackage into a new unity project 
    A new "Tools" menu should appear in the unity menu. 
    Select "Tools-Tile Mapping->Tile Material Editor" 
    Select a tile set texture. 
    Specify the tile width and height dimensions and specify a spacing value if the tiles are spaced. 
    Select one or more tiles from the tile set by left clicking and dragging a selection rectangle. 
    Specify an output folder where the material file will be placed. 
    Specify any labels you wish to attach to the material after it's created. Each label must be separated by a space. 
    Give the material a name and tweak the edges using inset. Inset shrinks the selection rectangle by a small amount to help clip off any unwanted pixels bleeding over from adjacent tiles. 
    Click the create button to create the material 
    Unity Package Download
    TileMaterialCreationWindowR2.zip Release 2 - Dec 15, 2012
    Media:TileMaterialCreationWindow.zip - Dec 13, 2012
    
     
    Source Code Code for TileMaterialCreationWindow.cs ...
    NOTE: The fallowing code relies on "Utilities.Helpers.DrawRect" which is provided in a separate code file which is included in the download. 
    namespace CBX.Unity.Editor.Windows
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
     
        using UnityEditor;
     
        using UnityEngine;
     
        /// <summary>
        /// Provides a window for selecting tiles from a tile set and creating materials from them
        /// </summary>
        public class TileMaterialCreationWindow : EditorWindow
        {
            /// <summary>
            /// Holds a list of selected tile locations.
            /// </summary>
            private readonly List<Vector2> selectedTiles = new List<Vector2>();
     
            /// <summary>
            /// Holds a predefined list of shader names.
            /// </summary>
            private readonly string[] shaderNames = new[] { "Diffuse", "Transparent/Diffuse" };
     
            /// <summary>
            /// Holds the scroll values for the main preview.
            /// </summary>
            private Vector2 mainPreviewScroll;
     
            /// <summary>
            /// Holds the value for the tile width.
            /// </summary>
            private int tileWidth = 32;
     
            /// <summary>
            /// Holds the value for the tile height.
            /// </summary>
            private int tileHeight = 32;
     
            /// <summary>
            /// Stores the index into the scaleValues field.
            /// </summary>
            private int previewScaleIndex;
     
            /// <summary>
            /// Holds an array of scale values.
            /// </summary>
            private string[] scaleValues = { "1", "2", "3", "4", "5", "6" };
     
            /// <summary>
            /// Holds a reference to the texture asset.
            /// </summary>
            private Texture2D textureAsset;
     
            /// <summary>
            /// Holds a reference to the output path where materials will be generated.
            /// </summary>
            private string outputPath = "Assets/";
     
            /// <summary>
            /// Holds the value for spacing between tiles.
            /// </summary>
            private int spacing = 1;
     
            /// <summary>
            /// Holds the value for spacing between tiles.
            /// </summary>
            private float inset = 0.001f;
     
            /// <summary>
            /// Determines if the tile set starts with spaces.
            /// </summary>
            private bool startSpacing;
     
            /// <summary>
            /// Used to record when the last auto repaint occurred.
            /// </summary>
            private DateTime lastUpdateTime;
     
            /// <summary>
            /// Stores the location of the first tile location where dragging began.
            /// </summary>
            private Vector2 firstTileLocation;
     
            /// <summary>
            /// Used to determine if the user is dragging a selection rectangle.
            /// </summary>
            private bool isSelectingTiles;
     
            /// <summary>
            /// Holds the scroll values for the selection preview.
            /// </summary>
            private Vector2 selectionPreviewScroll;
     
            /// <summary>
            /// Used to store the material name.
            /// </summary>
            private string txtName = string.Empty;
     
            /// <summary>
            /// Used to store the names of labels.
            /// </summary>
            private string txtLabels = string.Empty;
     
            /// <summary>
            /// Holds the selection index for the specified shader.
            /// </summary>
            private int shaderSelectionIndex;
     
            /// <summary>
            /// Used to determine weather the error text should be drawn.
            /// </summary>
            private bool showErrorText;
     
            /// <summary>
            /// Used to store what error message to report.
            /// </summary>
            private string errorText = string.Empty;
     
            /// <summary>
            /// Is called when the window need to be re-drawn.
            /// </summary>
            private void OnGUI()
            {
                GUILayout.BeginHorizontal();
     
                // draw the main preview 
                this.DrawMainPreview();
     
                // draw the side controls
                this.DrawSideControls();
     
                GUILayout.EndHorizontal();
            }
     
            /// <summary>
            /// Draws the main preview texture.
            /// </summary>
            private void DrawMainPreview()
            {
                // setup a scroll view so the user can scroll large textures
                this.mainPreviewScroll = GUILayout.BeginScrollView(this.mainPreviewScroll, true, true);
     
                // check if a texture asset is available
                if (this.textureAsset != null)
                {
                    // we draw a label here with the same dimensions of the texture so scrolling will work
                    GUILayout.Label(
                        string.Empty, GUILayout.Width(this.textureAsset.width), GUILayout.Height(this.textureAsset.height));
     
                    // draw the preview texture
                    GUI.DrawTexture(
                        new Rect(4, 4, this.textureAsset.width, this.textureAsset.height),
                        this.textureAsset,
                        ScaleMode.StretchToFill,
                        true,
                        1);
     
                    // draw the selection rectangles
                    this.DrawSelection();
                }
     
                GUILayout.EndScrollView();
            }
     
            /// <summary>
            /// Used to select a output path where materials will be saved to.
            /// </summary>
            private void DoSelectOutputPath()
            {
                // prompt the user to select a path
                this.outputPath = EditorUtility.OpenFolderPanel("Select output path", this.outputPath, string.Empty);
            }
     
            /// <summary>
            /// Draws controls on the side for creating materials.
            /// </summary>
            private void DrawSideControls()
            {
                GUILayout.BeginVertical(GUILayout.Width(200), GUILayout.MinWidth(200));
     
                // draw tile size controls
                var size = EditorGUILayout.Vector2Field("Tile Size", new Vector2(this.tileWidth, this.tileHeight));
                this.tileWidth = (int)size.x;
                this.tileHeight = (int)size.y;
     
                // draw a texture selection control
                this.textureAsset =
                    (Texture2D)EditorGUILayout.ObjectField("Texture", this.textureAsset, typeof(Texture2D), true);
     
                // draw control for specifying if starts with spacing 
                this.startSpacing = GUILayout.Toggle(this.startSpacing, "Starts with spacing");
     
                // draw control for specifying spacing
                this.spacing = EditorGUILayout.IntField("Spacing", this.spacing, GUILayout.MaxWidth(200));
                this.spacing = this.spacing < 0 ? 0 : this.spacing;
     
                // draw control for specifying inset
                this.inset = EditorGUILayout.FloatField("Inset", this.inset, GUILayout.MaxWidth(200));
                this.inset = this.inset < 0 ? 0 : this.inset;
     
                // place some spacing of a few pixels
                GUILayout.Space(4);
     
                // provide controls for selecting a output path
                GUILayout.Label("Output Path");
                this.outputPath = GUILayout.TextField(this.outputPath, GUILayout.MaxWidth(200));
                if (GUILayout.Button("Set", GUILayout.MaxWidth(40)))
                {
                    this.DoSelectOutputPath();
                }
     
                // place some spacing of a few pixels
                GUILayout.Space(4);
     
                // draw a popup that allows for shader type selection
                GUILayout.Label("Shader type");
                this.shaderSelectionIndex = EditorGUILayout.Popup(this.shaderSelectionIndex, this.shaderNames);
     
                // place some spacing of a few pixels
                GUILayout.Space(4);
     
                // provide a text field for specifying labels
                GUILayout.Label("Labels to apply");
                this.txtLabels = GUILayout.TextField(this.txtLabels);
     
                // place some spacing of a few pixels
                GUILayout.Space(4);
     
                // provide a text field for specifying a material name
                GUILayout.Label("Material Name");
                this.txtName = GUILayout.TextField(this.txtName);
     
                // place some spacing of a few pixels
                GUILayout.Space(4);
     
                // provide a create button
                if (GUILayout.Button("Create"))
                {
                    this.CreateNewMaterial();
                }
     
                // place some spacing of a few pixels
                GUILayout.Space(4);
     
                // if there was an error draw it here
                if (this.showErrorText)
                {
                    var style = new GUIStyle(GUI.skin.label);
     
                    // NOTE: changing the text color may conflict with the over all skin style be we will assume that the default skin is being used.
                    style.normal.textColor = Color.red;
                    GUILayout.Label(this.errorText, style);
                }
     
                // draw a small selection preview so the user can see what they selected
                this.DrawSelectionPreview();
     
                GUILayout.EndVertical();
            }
     
            /// <summary>
            /// Used to create a new material using the provided information from the user
            /// </summary>
            private void CreateNewMaterial()
            {
                // trim material name and labels
                this.txtName = this.txtName.Trim();
                this.txtLabels = this.txtLabels.Trim();
     
                // make sure we don't display any error. We assume up front that there will not be any
                this.showErrorText = false;
     
                // if no material name specified just exit
                if (string.IsNullOrEmpty(this.txtName))
                {
                    // report error
                    this.showErrorText = true;
                    this.errorText = "No material name!";
                    return;
                }
     
                // check if a texture has been specified and if not just exit
                if (this.textureAsset == null)
                {
                    // report error
                    this.showErrorText = true;
                    this.errorText = "No texture selected!";
                    return;
                }
     
                // attempt to create a a new material 
                Material mat;
                try
                {
                    mat = new Material(Shader.Find(this.shaderNames[this.shaderSelectionIndex])) { mainTexture = this.textureAsset };
                }
                catch (Exception ex)
                {
                    // report error
                    this.showErrorText = true;
                    this.errorText = ex.Message;
                    return;
                }
     
                // get texture co-ordinates for the material
                var coords = this.GetTextureCoords();
                mat.mainTextureOffset = new Vector2(coords.xMin, coords.yMin);
                mat.mainTextureScale = new Vector2(coords.width, coords.height);
     
                // attempt to construct a file path
                string file;
                try
                {
                    // try to build a file path
                    file = System.IO.Path.Combine(this.outputPath, this.txtName);
     
                    // include the *.mat file extension
                    file = System.IO.Path.ChangeExtension(file, ".mat");
                }
                catch (Exception ex)
                {
                    // report error
                    this.showErrorText = true;
                    this.errorText = ex.Message;
                    return;
                }
     
                // attempt to split any labels by spaces
                var labelParts =
                    this.txtLabels.Split(new[] { " " }, StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x.Trim()))
                        .ToArray();
     
                // check if the file exists already and if the file exists ask the user if they want to override the file or cancel    
                if (System.IO.File.Exists(file)
                    &&
                    !EditorUtility.DisplayDialog(
                        "Warning", "A material with that file name already exists!", "Override", "Cancel"))
                {
                    // create the material asset and override any file that may already exist
                    this.CreateMaterialFile(mat, file, labelParts);
                }
                else
                {
                    // create the material asset and override any file that may already exist
                    this.CreateMaterialFile(mat, file, labelParts);
                }
            }
     
            /// <summary>
            /// Creates a material file.
            /// </summary>
            /// <param name="mat">A reference to the material to save.</param>
            /// <param name="file">The filename of the material.</param>
            /// <param name="labelParts">An array of labels to apply to the material asset file.</param>
            private void CreateMaterialFile(Material mat, string file, string[] labelParts)
            {
                // get directory
                var directory = System.IO.Path.GetDirectoryName(file);
     
                // check if directory was found
                if (directory == null)
                {
                    // report error
                    this.showErrorText = true;
                    this.errorText = "No output directory!";
                    return;
                }
     
                // check if directory exists and if not create it
                if (!System.IO.Directory.Exists(directory))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }
                    catch (Exception ex)
                    {
                        // report error
                        this.showErrorText = true;
                        this.errorText = "Could not create directory!";
                        Debug.LogException(ex);
                        return;
                    }
                }
     
                try
                {
                    // create the material asset and override any file that may already exist
                    AssetDatabase.CreateAsset(mat, file);
                }
                catch (Exception ex)
                {
                    // report error
                    this.showErrorText = true;
                    this.errorText = ex.Message;
                    return;
                }
     
                // if labels were specified then set the labels
                if (labelParts.Any())
                {
                    AssetDatabase.SetLabels(mat, labelParts);
                }
            }
     
            /// <summary>
            /// Draw the selection preview showing what the user has selected.
            /// </summary>
            private void DrawSelectionPreview()
            {
     
                GUILayout.BeginHorizontal();
     
                // draw a label indicating that this is a preview
                GUILayout.Label("Preview");
     
                GUILayout.FlexibleSpace();
     
                // draw scale values selector
                this.previewScaleIndex = EditorGUILayout.Popup(this.previewScaleIndex, this.scaleValues);
     
                GUILayout.EndHorizontal();
     
                // if there is no texture asset or selection we can just exit
                if (this.textureAsset == null)
                {
                    return;
                }
     
                // wrap preview in a scroll view incase preview selection is larger
                this.selectionPreviewScroll = GUILayout.BeginScrollView(this.selectionPreviewScroll, true, true);
     
                // get the texture co-ordinates
                var texCoords = this.GetTextureCoords();
     
                // get the selection size
                var size = this.GetSelectionSize();
     
                // setup a rectangle the same size as the selection
                var rect = new Rect(0, 0, (size.x * (this.tileWidth + this.spacing)) - this.spacing, (size.y * (this.tileHeight + this.spacing)) - this.spacing);
     
                // scale output rectangle by the selected scale value index plus one
                rect.width *= this.previewScaleIndex + 1;
                rect.height *= this.previewScaleIndex + 1;
     
                // we draw a label here with the same dimensions of the texture so scrolling will work
                GUILayout.Label(string.Empty, GUILayout.Width(rect.width), GUILayout.Height(rect.height));
     
                // draw the selection texture
                GUI.DrawTextureWithTexCoords(rect, this.textureAsset, texCoords, true);
     
                GUILayout.EndScrollView();
            }
     
            /// <summary>
            /// Calculates the texture co-ordinates for the selection.
            /// </summary>
            /// <returns>Returns the textures co-ordinates for the selection in UV space.</returns>
            private Rect GetTextureCoords()
            {
                // calc selection rectangle size
                var size = this.GetSelectionSize();
     
                // get min X/Y position of the selection rectangle
                var minX = float.MaxValue;
                var minY = float.MaxValue;
                foreach (var tile in this.selectedTiles)
                {
                    if (tile.x < minX)
                    {
                        minX = tile.x;
                    }
     
                    if (tile.y < minY)
                    {
                        minY = tile.y;
                    }
                }
     
                var startOffset = this.startSpacing ? this.spacing : 0;
     
                // setup a rect containing the US co-ordinates
                var texCoords = new Rect(
                   (startOffset + (minX * (this.tileWidth + this.spacing))) / this.textureAsset.width,
                   (startOffset + (minY * (this.tileHeight + this.spacing))) / this.textureAsset.height,
                   ((size.x * (this.tileWidth + this.spacing)) - startOffset) / this.textureAsset.width,
                   ((size.y * (this.tileHeight + this.spacing)) - startOffset) / this.textureAsset.height);
     
                texCoords.x += inset;
                texCoords.y += inset;
                texCoords.width -= (inset * 2);
                texCoords.height -= (inset * 2);
     
                // textures co-ordinates originate from the lower left corner of the texture so adjust y to accommodate
                texCoords.y = 1 - (texCoords.y / 1) - texCoords.height;
                return texCoords;
            }
     
            /// <summary>
            /// Calculates the size of the selection rectangle in tile co-ordinates.
            /// </summary>
            /// <returns>Returns the size of the selection rectangle in tile co-ordinates as a <see cref="Vector2"/> type.</returns>
            private Vector2 GetSelectionSize()
            {
                // calculate the min and max values of the selection rectangle
                var minX = float.MaxValue;
                var maxX = float.MinValue;
                var minY = float.MaxValue;
                var maxY = float.MinValue;
                foreach (var tile in this.selectedTiles)
                {
                    if (tile.x < minX)
                    {
                        minX = tile.x;
                    }
     
                    if (tile.x > maxX)
                    {
                        maxX = tile.x;
                    }
     
                    if (tile.y < minY)
                    {
                        minY = tile.y;
                    }
     
                    if (tile.y > maxY)
                    {
                        maxY = tile.y;
                    }
                }
     
                // store the width and height
                var selectionWidth = maxX - minX + 1;
                var selectionHeight = maxY - minY + 1;
     
                // return the size
                return new Vector2(selectionWidth, selectionHeight);
            }
     
            /// <summary>
            /// Restricts the values of a <see cref="Vector2"/> to the dimensions of the selected texture.
            /// </summary>
            /// <param name="value">The value to be restricted.</param>
            /// <returns>Returns the restricted <see cref="Vector2"/> value.</returns>
            private Vector2 CapValues(Vector2 value)
            {
                // prevent values less then 0
                value.x = value.x < 0 ? 0 : value.x;
                value.y = value.y < 0 ? 0 : value.y;
     
                // prevent values greater then the texture dimensions
                value.x = value.x > (this.textureAsset.width / this.tileWidth) - 1 ? (this.textureAsset.width / this.tileWidth) - 1 : value.x;
                value.y = value.y > (this.textureAsset.height / this.tileHeight) - 1 ? (this.textureAsset.height / this.tileHeight) - 1 : value.y;
     
                return value;
            }
     
            /// <summary>
            /// Draw the selection rectangle in the preview.
            /// </summary>
            private void DrawSelection()
            {
                // get mouse position
                var input = Event.current;
                var mousePosition = input.mousePosition;
     
                var startOffset = this.startSpacing ? this.spacing : 0;
     
                // convert the mouse position into a tile position
                mousePosition.x = (int)Math.Round(mousePosition.x / (this.tileWidth + this.spacing), 1, MidpointRounding.ToEven);
                mousePosition.y = (int)Math.Round(mousePosition.y / (this.tileHeight + this.spacing), 1, MidpointRounding.ToEven);
     
                // determine if user is selecting tiles
                if (this.isSelectingTiles)
                {
                    // clear any previous selection
                    this.selectedTiles.Clear();
     
                    // calc the min and max positions based on the location of the mouse and the first selected tile location
                    var min = new Vector2(Math.Min(this.firstTileLocation.x, mousePosition.x), Math.Min(this.firstTileLocation.y, mousePosition.y));
                    var max = new Vector2(Math.Max(this.firstTileLocation.x, mousePosition.x), Math.Max(this.firstTileLocation.y, mousePosition.y));
     
                    // cap the min max values to the dimensions of the texture
                    min = this.CapValues(min);
                    max = this.CapValues(max) + Vector2.one;
     
                    // add tile entries for the selection
                    for (var idx = min.x; idx < max.x; idx++)
                    {
                        for (var idy = min.y; idy < max.y; idy++)
                        {
                            this.selectedTiles.Add(new Vector2(idx, idy));
                        }
                    }
                }
     
                // draw selected tiles
                int x;
                int y;
                foreach (var tile in this.selectedTiles)
                {
                    // calc rectangle Top Left for tile location
                    x = startOffset + ((int)tile.x * (this.tileWidth + this.spacing));
                    y = startOffset + ((int)tile.y * (this.tileHeight + this.spacing));
     
                    // draw the selected tile rectangle
                    Utilities.Helpers.DrawRect(new Rect(x, y, this.tileWidth + this.spacing - startOffset, this.tileHeight + this.spacing - startOffset), Color.red);
                }
     
                // draw blue rectangle indicating what tile the mouse is hovering over
                x = startOffset + ((int)mousePosition.x * (this.tileWidth + this.spacing));
                y = startOffset + ((int)mousePosition.y * (this.tileHeight + this.spacing));
                Utilities.Helpers.DrawRect(new Rect(x, y, this.tileWidth + this.spacing - startOffset, this.tileHeight + this.spacing - startOffset), Color.blue);
     
                // only repaint every so often to save cpu cycles
                if (DateTime.Now > this.lastUpdateTime + TimeSpan.FromMilliseconds(50))
                {
                    // record the time that we are repainting
                    this.lastUpdateTime = DateTime.Now;
                    this.Repaint();
                }
     
                // check if were beginning to select tiles with the left mouse button
                if (!this.isSelectingTiles && input.isMouse && input.type == EventType.MouseDown && input.button == 0)
                {
                    this.isSelectingTiles = true;
                    this.firstTileLocation = new Vector2(mousePosition.x, mousePosition.y);
     
                    // clear any previously selected tiles and add the first tile location
                    this.selectedTiles.Clear();
                    this.selectedTiles.Add(this.firstTileLocation);
                }
     
                // check if the user has stopped selecting tiles
                if (input.isMouse && input.type == EventType.MouseUp && input.button == 0)
                {
                    this.isSelectingTiles = false;
                }
            }
     
            /// <summary>
            /// Used to initialize the window.
            /// </summary>
            [MenuItem("Tools/Tile Mapping/Tile Material Editor")]
            private static void Init()
            {
                // get the window, show it, and hand it focus
                try
                {
                    var window = EditorWindow.GetWindow<TileMaterialCreationWindow>();
                    window.title = "Tile Materials";
                    window.Show();
                    window.Focus();
                    window.Repaint();
                }
                catch (System.Exception ex)
                {
                    // log error in the console if something went wrong
                    Debug.LogError(ex.Message);
                }
            }
        }
    }
}
