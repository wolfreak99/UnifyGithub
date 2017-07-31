// Original url: http://wiki.unity3d.com/index.php/VersionCheck
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Networking/WWWScripts/VersionCheck.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
Author: (Kevin Lindeman) 
DescriptionThis script will take the version variable, and check it against a file on your web server. The file just contains the version number. If the version variable is greater than or equal to what the text file shows, then there is no update needed. Otherwise you can have it open up a URL you specify. 
UsagePlace this script on a GameObject. 
JavaScript - VersionCheck.jsvar version : int = 1;
 
function CheckVersion ()
{
    var update_url = "http://mysite.com/myGame/version.txt";
    update_post = WWW(update_url);
    yield update_post; // Wait until the download is done
    if(update_post.error)
    {
        print("There was an error loading the update URL: " + update_post.error);
    }
    else
    {
        var latestVersion : int;
        latestVersion = int.Parse(update_post.data);
        if (latestVersion > version)
        {
            // Put something to tell the user they should update
        }
    }
}
 
function openUpdateWebsite ()
{
    // Replace this url with wherever the user can download the update.
    System.Diagnostics.Process.Start("open","http://mysite.com/myGame/download.html");
}
}
