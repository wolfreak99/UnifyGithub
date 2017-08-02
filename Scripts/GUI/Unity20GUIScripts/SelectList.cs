/*************************
 * Original url: http://wiki.unity3d.com/index.php/SelectList
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/SelectList.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    By AngryAnt. 
    Description Visualises a list of strings or objects overriding ToString and returns the item selected by the user. Clicking a selected item will deselect it. 
    Optionally, the OnListItemGUI delegate can be used to do custom GUI rendering of list items. 
    Component code 		public static object SelectList( ICollection list, object selected, GUIStyle defaultStyle, GUIStyle selectedStyle )
    		{			
    			foreach( object item in list )
    			{
    				if( GUILayout.Button( item.ToString(), ( selected == item ) ? selectedStyle : defaultStyle ) )
    				{
    					if( selected == item )
    					// Clicked an already selected item. Deselect.
    					{
    						selected = null;
    					}
    					else
    					{
    						selected = item;
    					}
    				}
    			}
     
    			return selected;
    		}
     
     
     
    		public delegate bool OnListItemGUI( object item, bool selected, ICollection list );
     
     
     
    		public static object SelectList( ICollection list, object selected, OnListItemGUI itemHandler )
    		{
    			ArrayList itemList;
     
    			itemList = new ArrayList( list );
     
    			foreach( object item in itemList )
    			{
    				if( itemHandler( item, item == selected, list ) )
    				{
    					selected = item;
    				}
    				else if( selected == item )
    				// If we *were* selected, but aren't any more then deselect
    				{
    					selected = null;
    				}
    			}
     
    			return selected;
    		}Usage 	currentSeletion = SelectList( myList, currentSelection, GUI.skin.GetStyle( "Button" ), GUI.skin.GetStyle( "Label" ) );In this example, myList could be an ArrayList of strings or instances of your own type, returning the desired list label in ToString. 
    	currentSeletion = SelectList( myList, currentSelection, OnCheckboxItemGUI );
     
    	private bool OnCheckboxItemGUI( object item, bool selected, ICollection list )
    	{
    		return GUILayout.Toggle( selected, item.ToString() );
    	}In this example, the same list is used, only this time it is visualised using a custom list item rendering function. 
    Further advancing the use of custom list item rendering functions, you could implement nested lists similar to this: 
     
}
