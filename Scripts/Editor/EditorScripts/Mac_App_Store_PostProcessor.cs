/*************************
 * Original url: http://wiki.unity3d.com/index.php/Mac_App_Store_PostProcessor
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Mac_App_Store_PostProcessor.cs
 * File based on original modification date of: 21 March 2014, at 12:29. 
 *
 * Author: Martin Schultz / Decane 
 *
 * Update 
 *   
 * Description 
 *   
 * Usage 
 *   
 * Assets/Editor/PostprocessBuildPlayer.sh 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Update An in-editor version is now available and should be used instead of manual scripting: http://u3d.as/content/jemast-software/mac-app-store-toolset/4TN 
    A simplified version of the tool is also available for free: http://jemast.com/unity/unity-entitlements-tool 
    DescriptionThis PostprocessBuildPlayer script takes your Mac binary and auto-signs and auto-packages it for uploading it to the Mac App Store. No need anymore for manually opening the terminal app to sign your game and then create a package out of it to be able to upload it to the Mac App Store - this script does the hard work for you - automatically! 
    Usage Put this script into a file named "PostprocessBuildPlayer.sh" and save that to your /Assets/Editor directory. 
    If you want to code sign your game, set this variable to true, otherwise to false for not signing it (saves roughly 1 second in the build process): "my $doCodeSigning = true;" 
    If you want to auto-package your signed game, set "my $doCreatePackage = true;" and it will create the required package file for uploading to the Mac App Store. 
    Modify the certificate names for the DEVELOPER and INSTALLER certificates to match the name of your certificate! 
    NOTE: When you use this for signing a Mac App with In App Purchasing, you might need to manually sign the App. I've run into an issue (not yet clear why) that the auto-signing fails ONLY when using additional plugins like Prime31's IAP Mac plugin. I had to manually sign the app (and use the Distribution cert then for testing!). Furthermore, don't use Build & Run for In App Purchase testing as the Apple Tech note 2259 says, that the first time an IAP app has to be started from the Finder so that the Mac App Store login dialog can come up and _MASReceipt can be downloaded into your app. 
    Assets/Editor/PostprocessBuildPlayer.sh #!/usr/bin/perl 
    use File::Copy; 
    use Cwd;
    
    my $installPath = $ARGV[0]; 
    
    # The type of player built: 
    # "dashboard", "standaloneWin32", "standaloneOSXIntel", "standaloneOSXPPC", "standaloneOSXUniversal", "webplayer", "iPhone" 
    my $target = $ARGV[1]; 
    
    print ("\n*** PostprocessBuildPlayer - Building at '$installPath' with target: $target ***\n"); 
    
    # This script assumes you have the following directories created:
    # <Unity Project folder>/Assets/AppData 
    # <Unity Project folder>/Builds
    # The AppData contains the files Info.plist, UnityPlayer.icns and UnityPlayerIcon.png
    # The Builds directory is where your generated game is being saved to.
    
    # Auto-sign the binary?
    my $doCodeSigning = true;
    # Auto create a package file?
    my $doCreatePackage = true;
    # This is the source directory where to copy from
    my $srcAssetPath = "/Assets/AppData/";
    # This is the directory where you set in Unity in the Build Settings where you output your game to.
    my $outputDirectory = "/Builds/";
    # This is the name of the package file that this scripts creates out of your game
    my $packageName = "HardRockRacing.pkg";
    # This is your DEVELOPER certificate you create with Apple. The name has to match exactly the name in the certificate!
    my $certificateApplication = "3rd Party Mac Developer Application: Martin Schultz";
    # This is the INSTALLER certificate. The name has to match exactly the name in the certificate!
    my $certificateInstaller = "3rd Party Mac Developer Installer: Martin Schultz";
    # src and dest are temp variables. Just ignore them... :-)
    my $src = "";
    my $dest = "";
    
    # this copies your own Assets/AppData/Info.plist to the generated game
    $plist = getcwd . $srcAssetPath . "Info.plist";
    $dest = $installPath . "/Contents/Info.plist";
    copy($plist, $dest) or die "File can not be copied: " . $plist;
    print ("\n*** Copying " . $plist . " to " . $dest);
    
    # this copies Assets/AppData/UnityPlayer.icns to your generated game replacing the original app icon by your own
    $icons = getcwd . $srcAssetPath . "UnityPlayer.icns";
    $dest = $installPath . "/Contents/Resources/UnityPlayer.icns";
    copy($icons, $dest) or die "File can not be copied: " . $icons;
    print ("\n*** Copying " . $icons . " to " . $dest);
    
    # this copies Assets/AppData/UnityPlayerIcon.png to your generated game replacing the original Unity Player Icon by your own
    $playericon = getcwd . $srcAssetPath . "UnityPlayerIcon.png";
    $dest = $installPath . "/Contents/Resources/UnityPlayerIcon.png";
    copy($playericon, $dest) or die "File can not be copied: " . $playericon;
    print ("\n*** Copying " . $playericon . " to " . $dest);
    
    #fail safe check: If accidentially signing is set to false, but packaging set to true, signing is turned on to create a valid package.
    if ($doCreatePackage eq "true") {
    	$doCodeSigning = true;
    }
    
    # Auto code signing?
    if ($doCodeSigning eq "true") {
    	system("codesign -f -s \"" . $certificateApplication . "\" \"" . $installPath . "\"");
    }
    
    # Auto creating a package file?
    if ($doCreatePackage eq "true") {
    	system("productbuild --component \"" . $installPath . "\" /Applications --sign \"". $certificateInstaller . "\" \"" . getcwd . $outputDirectory . $packageName . "\"");
}
}
