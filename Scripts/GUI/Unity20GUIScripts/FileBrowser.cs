/*************************
 * Original url: http://wiki.unity3d.com/index.php/FileBrowser
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/GUI/Unity20GUIScripts/FileBrowser.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.GUI.Unity20GUIScripts
{
    By AngryAnt. A different file browser can be found here: ImprovedFileBrowser. 
    Description Implements a crude file browser in unity 2.0 GUI. This code uses the SelectList script. 
    Note that by crude I mean you might want to have a GUILayout pretty it up and at some point someone (me perhaps?) should pull the script around a bit so the file and directory representations in the lists aren't by complete path. Also the root check only works for unix environments at the moment. That should be an easy rewrite though. 
    This is how the browser could look: 
     
    Component code public static bool FileBrowser( ref string location, ref Vector2 directoryScroll, ref Vector2 fileScroll )
    {
        bool complete;
        DirectoryInfo directoryInfo;
        DirectoryInfo directorySelection;
        FileInfo fileSelection;
        int contentWidth;
     
     
        // Our return state - altered by the "Select" button
        complete = false;
     
        // Get the directory info of the current location
        fileSelection = new FileInfo( location );
        if( (fileSelection.Attributes & FileAttributes.Directory) == FileAttributes.Directory )
        {
        	directoryInfo = new DirectoryInfo( location );
        }
        else
        {
        	directoryInfo = fileSelection.Directory;
        }
     
     
        if( location != "/" && GUI.Button( new Rect( 10, 20, 410, 20 ), "Up one level" ) )
        {
            directoryInfo = directoryInfo.Parent;
            location = directoryInfo.FullName;
        }
     
     
        // Handle the directories list
        GUILayout.BeginArea( new Rect( 10, 40, 200, 300 ) );
            GUILayout.Label( "Directories:" );
            directoryScroll = GUILayout.BeginScrollView( directoryScroll );
        	    directorySelection = BehaveLibrary.Resources.SelectList( directoryInfo.GetDirectories(), null ) as DirectoryInfo;
        	GUILayout.EndScrollView();
        GUILayout.EndArea();
     
        if( directorySelection != null )
        // If a directory was selected, jump there
        {
            location = directorySelection.FullName;
        }
     
     
        // Handle the files list
        GUILayout.BeginArea( new Rect( 220, 40, 200, 300 ) );
            GUILayout.Label( "Files:" );
            fileScroll = GUILayout.BeginScrollView( fileScroll );
            	fileSelection = BehaveLibrary.Resources.SelectList( directoryInfo.GetFiles(), null ) as FileInfo;
        	GUILayout.EndScrollView();
        GUILayout.EndArea();
     
        if( fileSelection != null )
        // If a file was selected, update our location to it
        {
            location = fileSelection.FullName;
        }
     
     
        // The manual location box and the select button
        GUILayout.BeginArea( new Rect( 10, 350, 410, 20 ) );
        GUILayout.BeginHorizontal();		
        	location = GUILayout.TextArea( location );
     
        	contentWidth = ( int )GUI.skin.GetStyle( "Button" ).CalcSize( new GUIContent( "Select" ) ).x;
        	if( GUILayout.Button( "Select", GUILayout.Width( contentWidth ) ) )
        	{
        		complete = true;
        	}
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
     
     
        return complete;
    }Usage 	public void FileBrowserWindow( int idx )
    	{
    		if( FileBrowser( ref location, ref directoryScroll, ref fileScroll ) )
    		{
    			fileBrowser = false;
    		}
    	}
     
    	public void OnGUI()
    	{
    		if( fileBrowser )
    		{
    			GUI.Window( 0, new Rect( ( Screen.width - 430 ) / 2, ( Screen.height - 380 ) / 2, 430, 380 ), FileBrowserWindow, "Browse" );
    			return;
    		}
    	}
}
