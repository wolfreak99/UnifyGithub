/*************************
 * Original url: http://wiki.unity3d.com/index.php/GetXMLHack
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/WWWScripts/GetXMLHack.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Networking.WWWScripts
{
    I've been having trouble getting the Web Player to download XML properly using built in WWW() calls so I've come up with this work around. ( Please excuse the sparse nature of the page, feel free to tidy it up more if you like to do such things. :) ) 
    You really should use WWW() where possible but it's been giving me back corrupt data and suffering from what appears to be a buffer overflow bug somewhere between the browser and the unity plugin. This page offers a work-around. 
    There is no facility for pushing data to the server demonstrated here. 
    
    Attach this code to the Unity Object you want to use as a button, I've been using a GUITexture. 
    function OnMouseUp(){
     
        var url : String = "http://boots.local/ggmm/state_proxy.php";
        Application.ExternalCall(    "getXmlForUnity", 
                                    url, //which url to get data from
                                    "UnityEmbed", //value from id attrib of html embed tag
                                    "BrowserMarshal", //return object name
                                    "xmlcb_positionData" //function to handle incoming data
                                    );
     
    }I have created an object called BrowserMarshal that I use to deal with any incoming messages from my browser side scripts. You can pass-out any object name in the function call above, the script on the other side will respect it. 
    
    
    Use this function in the browser to handle your call for XML data: (This script depends on Googles Mapping API objects, replace GXmlHttp below with XmlHttpRequest(?)...) 
    function getXmlForUnity( url, cb_domid, cb_o, cb_f ){
     
        //url - URL to get data from
        //cb_domid - the dom id of the embedded unity player we want to send a message to
        //cb_o - unity call back handler object
        //cb_f - unity call back handler object member function
     
        var xml = GXmlHttp.create();
     
            xml.open("GET", url, true);
     
                xml.onreadystatechange = 
                function() {
     
                        if (xml.readyState == 4) { //wait for data to download async
     
                            var xmlr = xml.responseText;
     
                            if( xmlr != null ){
     
                                document.getElementById( cb_domid ).SendMessage( cb_o, cb_f, xmlr );
     
                            }//end if not null payload
     
                        }//end if data has come back to us
                };//end function as value, semi colon important!
     
            xml.send(null); //initiate transfer now that callback behaviour is established.
    }//end function getXmlForUnity
    
    
    This is the script that resides in BrowserMarshal: 
    import System.Xml;
     
    function xmlcb_positionData( data : String ){
     
        var xml : XmlDocument = new XmlDocument();
     
        if( data != "" ){
     
            xml.LoadXml( data );
     
            var lat = xml.GetElementsByTagName("world").Item(0).Attributes.ItemOf["lat"].Value;
            var lon = xml.GetElementsByTagName("world").Item(0).Attributes.ItemOf["lon"].Value;
     
        }
     
    }
    
    
    Rough XML example. All branch tag names are unique. <xml><?xml version="1.0" encoding="ISO-8859-1"?> <world_state> <vehicle> <position> <world lat="-28.30857" lon="153.64496" alt_msl="3269.747" alt_agl="3269.070" /> <opengl x="0" y="0" z="0"/> <heading true="350.016" mag="339.667" track="349.987" /> <speed tas="213.797" ias="128.577" gs_kts="128.577" gs_kmh="" gs_mps="" gs_fps="" gs_mph="" /> </position> </vehicle> </world_state> </xml> 
    
    
    Example HTML example. Note the value of the <embed> id parameter. <xml><object id="UnityObject" classid="clsid:36D04559-44B7-45E0-BA81-E1508FAB359F" codebase="http://otee.dk/download_webplayer/UnityWebPlayer.cab"> 
       <param name="src" value="Unity Player.unityweb" />
       <embed id="UnityEmbed" src="Unity Player.unityweb" width="800" height="100" type="application/x-unity" pluginspage="http://www.otee.dk/getunityplayer.html" />
       <noembed>
               The browser does not have Unity Web Player installed.
               <a href="http://otee.dk/getunityplayer.html">Get Unity Web Player</a>
    
       </noembed>
    </object> </xml> 
}
