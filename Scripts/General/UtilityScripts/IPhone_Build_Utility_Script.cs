/*************************
 * Original url: http://wiki.unity3d.com/index.php/IPhone_Build_Utility_Script
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/IPhone_Build_Utility_Script.cs
 * File based on original modification date of: 19 October 2009, at 19:07. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    This script copies the icon and splash screen every time the project is built. It also sets the "Bundle Display Name" for your app, and let's you define if you want a prerendered icon (no gloss), and if your game is in landscape orientation (so OS popups are correctly oriented). 
    In addition you can adjust some of the values that are defined in AppController.mm (kFPS, kAccelerometerFrequency, ENABLE_INTERNAL_PROFILER) 
    Create a file called PostprocessBuildPlayer (no file extension) in the Assets\Editor directory. Create the Editor directory if it does not already exist. This is easiest to do from the Finder, not within Unity. 
    Paste the code below into the file and save. 
    Files to be copied should be in your project's root directory. This is the directory that contains the Assets folder. Icon.png is the icon. Default.png is the start up screen. 
    You can not replace the splash screen (Default.png) in the basic version on Unity iPhone so comment out the line "copy($defaultFilename, $dst) or die "Default file can not be copied "; " by putting a # at the beginning of the line. 
    
    
       #!/usr/bin/perl 
       use File::Copy; 
       
       my $installPath = $ARGV[0]; 
       
       #the name that displays on the iPhone 
       my $bundleDisplayName = "New App"; 
       # prerendered icons don't have the glossy effect applied over them. 
       my $prerenderedIcon = 1; 
       # determines orientation of OS popups (text messages, volume controls) 
       my $landscapeOrientation = 0; 
       
       # these three are values defined in AppController.m 
       my $fpsRate = "60.0"; 
       my $accelerometerRate = "60.0"; 
       my $profilerOn = "0"; 
       
       #go through the info.plist file line by line until you find this one: 
       my $findLine = "CFBundleDisplayName"; 
       my $endOfPlist = "</dict>"; 
       
       #copy Default.png and Icon.png from Asset to installPath 
       my $iconFilename = "Icon.png"; 
       my $defaultFilename = "Default.png"; 
       
       # The type of player built: 
       # "dashboard", "standaloneWin32", "standaloneOSXIntel", "standaloneOSXPPC", "standaloneOSXUniversal", "webplayer", "iPhone" 
       my $target = $ARGV[1]; 
       
       print ("\n*** PostprocessBuildPlayer - Building at '$installPath' with target: $target ***\n"); 
       
       my $dst = $installPath . "/" . $iconFilename; 
       print ("Copying Icon.png [$iconFilename -> $dst]\n"); 
       copy($iconFilename, $dst) or die "Icon file can not be copied "; 
       
       my $dst = $installPath . "/" . $defaultFilename; 
       print ("Copying Default.png [$defaultFilename -> $dst]\n"); 
       copy($defaultFilename, $dst) or die "Default file can not be copied "; 
       
       ################################################################ 
       # This modifies info.plist so you don't have to                # 
       # set BundleDisplayName manually                               # 
       ################################################################ 
       
       #open this file 
       
       $oplistPath = $installPath."/Info.plist"; 
       $nplistPath = $installPath."/Info.plist.tmp"; 
       
       open OLDPLIST, "<", $oplistPath or die("Cannot open Info.plist"); 
       open NEWPLIST, ">", $nplistPath or die("Cannot create new Info.plist"); 
       
       my $nextLine = 0;    
           
       while(<OLDPLIST>) 
       {     
          if ($nextLine == 1) 
          { 
             $_ =~ s/\${PRODUCT_NAME}/$bundleDisplayName/; #swap the product name for display name 
           
             $nextLine = 0; 
          } 
       
          if ($_ =~ m/$findLine/) 
          { 
             $nextLine = 1; 
          } 
           
          ################################################################ 
          # Add any key/value pairs you want at the end of Info.plist    # 
          ################################################################ 
       
          if ($_ =~ m/$endOfPlist/) 
          { 
             my $keys = ""; 
              
             if ($prerenderedIcon) 
             { 
                $keys .= "   <key>UIPrerenderedIcon</key>\n"; 
                $keys .= "   <true/>\n"; 
             } 
              
             if ($landscapeOrientation) 
             { 
                $keys .= "   <key>UIInterfaceOrientation</key>\n"; 
                $keys .= "   <string>UIInterfaceOrientationLandscapeRight</string>\n";       
             } 
              
             $_ = $keys . $_; 
          } 
           
          print NEWPLIST $_; 
       } 
       
       close OLDPLIST; 
       close NEWPLIST; 
       
       `mv \'$nplistPath\' \'$oplistPath\'`; 
       
       ################################################################ 
       # Change default Profiler & kFPS rates                         # 
       ################################################################ 
       
       $oacmPath = $installPath."/Classes/AppController.mm"; 
       $nacmPath = $installPath."/Classes/AppController.mm.tmp"; 
       
       open OLDACM, "<", $oacmPath or die("Cannot open AppController.mm"); 
       open NEWACM, ">", $nacmPath or die("Cannot create new AppController.mm"); 
       
       while(<OLDACM>) 
       { 
          if ($_ =~ m/ENABLE_INTERNAL_PROFILER/) 
          { 
             $_ =~ s/0/$profilerOn/; 
          } 
          if ($_ =~ m/kFPS/) 
          { 
             $_ =~ s/60.0/$fpsRate/; 
          } 
          if ($_ =~ m/kAccelerometerFrequency/) 
          { 
             $_ =~ s/60.0/$accelerometerRate/; 
          } 
          print NEWACM $_; 
       } 
       
       close OLDACM; 
       close NEWACM; 
       
       `mv \'$nacmPath\' \'$oacmPath\'`;
}
