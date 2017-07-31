// Original url: http://wiki.unity3d.com/index.php/UnityObject
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Networking/WWWScripts/UnityObject.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
DescriptionThis wraps all of UT's browser detection and communication in one javascript file and allows you to use minimal code to embed .unityweb's in your web pages. 
The code is based on the examples UT has given in the manual and the work Geoff Stearns has done with SWFObject 
Now Unity Developers can embed in style & comfort ;) 
Tested in Safari 2.0.4, FireFox 2.0.0.2 and Opera 9.01 
Compatible with CookieCutter 
UsageAll it takes to embed is the following; 
<script type="text/javascript" src="unityobject.js"></script>
<script type="text/javascript">
 var uniObj = new UnityObject("example.unity3d", "example", "640", "480", "000000", "000000");
 uniObj.write();
</script>
To send a message to unity all you need to do is 
uniObj.msg("MyObject", "MyFunction", "Hello Unity!");To use Pro Features; 
<script type="text/javascript">
 var uniObj = new UnityObject("proExample.unity3d", "proExample", "640", "480");
uniObj.addParam("logoimage", "MyLogo.png");
uniObj.addParam("progressbarimage", "MyProgressBar.png");
uniObj.addParam("progressframeimage", "MyProgressFrame.png");
uniObj.write();
</script>
If Plugin Not Found Write Alternate HTML; 
<script type="text/javascript">
 var uniObj = new UnityObject("example.unity3d", "example", "640", "480");
uniObj.setAttribute("altHTML", "<a href='http://unity3d.com/unitywebplayer.html' title='Go to unity3d.com to install the Unity Web Player'>Install the Unity Web Player</a>");
uniObj.write();
</script>
JavaScript - unityobject.js////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                                    //
//  unityobject.js - v0.5                                                                                             //
//                                                                                                                    //
//  Revision History:      
//
//  2010-APR-26 : Fixed Windows IE errors (ms@splinelab.de)
//  2009-JUL-29 : Added Chrome support for FindEar                                                                    //
//                and changed '_embed' to 'Embed' and '_object' to 'Object'                                           //
//                Modifcation by Alexander AT illustrata.no                                                           //
//                                                                                                                    //
//  2008-JAN-21 : Script updated to fix some Windows IE errors (tom@unity3d.com/joe@unity3d.com)                      //
//  2007-NOV-28 : Script updated to fix a few minor errors (tom@unity3d.com)                                          //
//  2007-NOV-15 : Script updated from milkytreat's v0.3 by Tom Higgins (tom@unity3d.com)                              //
//                - it's now compatible with both Unity 1.0 and 2.0 content                                           //
//                - it now uses 'ut' instead of 'otee' in the code                                                    //
//                - there are changes to the parameters that are provided when creating the UnityObject versus those  //
//                  that need to be added by calling addParam                                                         //
//                - for Unity 2.0 content this script now takes advantage of the fact that the user doesn't need to   //
//                  quit the browser when installing the Unity Web Player                                             //
//                                                                                                                    //
//  Special thanks to Geoff Sterns and 'milkytreat' (a member the Unity Community Forums).                            //
//                                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
var gAutomaticReloadForwardObject = null;
function automaticReloadForward () {
	if (gAutomaticReloadForwardObject != null)
		gAutomaticReloadForwardObject.automaticReload();
}
 
if (typeof ut == "undefined") { var ut = new Object(); }
ut.UnityObject = function (u, i, w, h, v, url) {
    if (!document.getElementById) { return; }
    this.attributes = new Array();
    this.params = new Object();
    if (u) { this.setAttribute("src", u); }
    if (i) { this.setAttribute("id", i); }
    if (w) { this.setAttribute("width", w); }
    if (h) { this.setAttribute("height", h); }
    if (v) { 
		this.playerVersion = v; 
    } else { 
		this.playerVersion = 2; 
    }
    if (url) { this.setAttribute("redirectUrl", url); }
}
 
ut.UnityObject.prototype = {	
    addParam: function (aName, aValue) {
		    this.params[aName] = aValue;
	  },
    automaticReload: function () {
		gAutomaticReloadForwardObject = null;
		navigator.plugins.refresh();
		if (this.detectUnityWebPlayer()) {
			window.location.reload();
		}
		else {
			gAutomaticReloadForwardObject = this;
			setTimeout('automaticReloadForward()', 500);
		}
    },
    detectUnityWebPlayer: function () {
    	var tInstalled = false;
        if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1) {
			tInstalled = DetectUnityWebPlayerActiveX();
        } else {
        	  if (this.playerVersion == 1) {
        	      if (navigator.mimeTypes && navigator.mimeTypes["application/x-unity"] && navigator.mimeTypes["application/x-unity"].enabledPlugin) {
                    if (navigator.plugins && navigator.plugins["Unity Web Player"]) {
                   	    tInstalled = true;	
            	      }
                }
        	  } else if (this.playerVersion >= 2) {
        	      if (navigator.mimeTypes && navigator.mimeTypes["application/vnd.unity"] && navigator.mimeTypes["application/vnd.unity"].enabledPlugin) {
                    if (navigator.plugins && navigator.plugins["Unity Player"]) {
                   	    tInstalled = true;	
            	      }
                }
        	  }
        }
        return tInstalled;	
    },
    findEar: function () {
		    this.unityEar = "";
 
			if (navigator.appVersion.indexOf("Chrome") != -1) {
				this.unityEar = document.getElementById(this.getAttribute("id") + "Embed");
			} else if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1) {
			      this.unityEar = document.getElementById(this.getAttribute("id") + "Object");
		    } else if (navigator.appVersion.toLowerCase().indexOf("safari") != -1) {
			      this.unityEar = document.getElementById(this.getAttribute("id") + "Object")
		    } else {
			      this.unityEar = document.getElementById(this.getAttribute("id") + "Embed");
		    }
    	  document.Unity = this.unityEar;
    },	  
	  getAttribute: function (aName) {
		    return this.attributes[aName];
	  },  
	  getParams: function () {
		    return this.params;
	  },	    
    getInstallerPath: function () {
		var tDownloadURL = "";
		if (this.playerVersion == 1) {
			tDownloadURL = "";
		}
		else if (this.playerVersion >= 2) {
			if (navigator.platform == "MacIntel") {
				tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/webplayer-i386.dmg";
			}
			else if (navigator.platform == "MacPPC") {
				tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/webplayer-ppc.dmg";
			}
			else if (navigator.platform.toLowerCase().indexOf("win") != -1) {
				tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayer.exe";
			}
		}
		return tDownloadURL;
    },   
    msg: function (aObj, aFunc, aVar) {
        this.unityEar.SendMessage(aObj, aFunc, aVar);
    },
    setAttribute: function (aName, aValue) {
		    this.attributes[aName] = aValue;
	  },	  
    write: function (aElementId) {
 
    	// Write the VB detection script once
		if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1) {
			document.write(" \n");
			document.write("<script language='VBscript'> \n");
			document.write("function DetectUnityWebPlayerActiveX \n");
			document.write("on error resume next \n");
			document.write("dim tControl \n");
			document.write("dim res \n");
			document.write("res = 0 \n");
			if (this.playerVersion == 1) {
				document.write("set tControl = CreateObject(\"UnityWebPlayer.UnityWebPlayerAXCtrl.1\") \n");
			} else if (this.playerVersion >= 2) {
				document.write("set tControl = CreateObject(\"UnityWebPlayer.UnityWebPlayer.1\") \n");
			}
			document.write("if IsObject(tControl) then \n");
			document.write("res = 1 \n");
			document.write("end if \n");
			document.write("DetectUnityWebPlayerActiveX = res\n");
			document.write("end function\n");
			document.write("</script>\n");
		}
 
		if (this.detectUnityWebPlayer()) {
       		  document.write(this.writeEmbedDOM());
       		  this.findEar();
        	  return true;
        } else {
            if (this.getAttribute("altHTML")) {
                document.write(this.getAttribute("altHTML"));
            }
            else if (this.getAttribute("redirectUrl")) {
                document.location.replace(this.getAttribute("redirectUrl"));
            }
            else {
				document.write("<div align='center' id='UnityPrompt' style=' width: " + this.getAttribute("width") + "px;");
				if (this.getAttribute("backgroundcolor")) {
					document.write(" background-color: #" + this.GetAttribute("backgroundcolor") + ";"); }
					document.write("'> \n");
					if (this.playerVersion == 1) {
						document.write("<a href='http://www.unity3d.com/unity-web-player-1.x'><img src='http://webplayer.unity3d.com/installation/getunity.png' border='0'/></a> \n");
 
					document.write("</div> \n");
				}
				else if (this.playerVersion >= 2) {
					var tInstallerPath = this.getInstallerPath();
					if (tInstallerPath != "") {
						document.write("<a href='" + tInstallerPath + "'><img src='http://webplayer.unity3d.com/installation/getunity.png' border='0'/></a> \n");
					}
					else {
						document.write("<a href='http://www.unity3d.com/unity-web-player-2.x'><img src='http://webplayer.unity3d.com/installation/getunity.png' border='0'/></a> \n");
					}
					document.write("</div> \n");
					this.automaticReload();
				}
            }
			return false;
        }
    },
    writeEmbedDOM: function() {
    	  var tUniSrc = "";
    	  if (this.playerVersion == 1) {
		        tUniSrc += "<object classid='clsid:36D04559-44B7-45E0-BA81-E1508FAB359F' codebase='http://otee.dk/download_webplayer/UnityWebPlayer.cab' ";
    	  } else if (this.playerVersion >= 2) {
    	  	  tUniSrc += "<object classid='clsid:444785F1-DE89-4295-863A-D46C3A781394' codebase='http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayer.cab#version=2,0,0,0' ";
    	  }
		    tUniSrc += "id='" + this.getAttribute("id") + "Object' width='" + this.getAttribute("width") + "' height='" + this.getAttribute("height") + "'><param name='src' value='" + this.getAttribute("src")+"' />";
		    var params = this.getParams();
    	  for(var key in params) {
        	  tUniSrc += "<param name='" + key + "' value='" + params[key] + "' />";
    	  }
    	  if (this.playerVersion == 1) {
    	      tUniSrc += "<embed type='application/x-unity' pluginspage='http://www.unity3d.com/unity-web-player-1.x' ";
    	  } else if (this.playerVersion >= 2) {
    	  	  tUniSrc += "<embed type='application/vnd.unity' pluginspage='http://www.unity3d.com/unity-web-player-2.x' ";
    	  } 
    	  tUniSrc += "id='" + this.getAttribute("id") + "Embed' width='" + this.getAttribute("width") + "' height='" + this.getAttribute("height") + "' src='" + this.getAttribute("src") + "' ";
     	  var params = this.getParams();
    	  for(var key in params){
        	  tUniSrc += [key] + "='" + params[key] + "' ";
    	  }
     	  tUniSrc += " /></object\>";
    	  return tUniSrc;
    }
}
if (!document.getElementById && document.all) { document.getElementById = function(id) { return document.all[id]; }}
var UnityObject = ut.UnityObject;
}
