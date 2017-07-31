// Original url: http://wiki.unity3d.com/index.php/ToolbarWrapped
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/GraphicalUserInterfaceScripts/ToolbarWrapped.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.GraphicalUserInterfaceScripts
{
Description ToolbarWrapped creates a toolbar or selection grid of text buttons that wraps horizontally. Thus, without changing its style, all its buttons fit within its given width in pixels (for example, the screen). The buttons will also be sized to fit the longest string in the array given so that everything is displayed in full. (GUI.Toolbar and GUI.SelectionGrid are currentlyunable to customize their individual buttons' styles to distinguish one from another.) Parameters and usage are the same as with GUI.Toolbar or GUI.SelectionGrid. 
Parameters position: Rectangle in screen coordinates, in which it will be wrapped horizontally and stretched vertically.
selected: The index of the selected button.
texts: An array of strings to show on the buttons.

Code function ToolbarWrapped(position:Rect,selected:int,texts:String[]):int{
  var longest:int=0;
  for(var i:int=1;i < texts.length;i++)if(texts[i].length>texts[longest].length)longest=i;
  var textWidth:float=GUI.skin.button.CalcSize(GUIContent(toolbarStrings[longest])).x;
  var paddingWidth:int=GUI.skin.button.padding.left+GUI.skin.button.padding.right;
  var margin:RectOffset=GUI.skin.button.margin;
  var xCount:int=Screen.width/(textWidth+paddingWidth+margin.horizontal);
  return GUI.SelectionGrid(position,selected,texts,xCount);
}Let me know if you'd like versions for textured buttons, GUIContent buttons, or ones in which you specify your own GUIStyle. 
}
