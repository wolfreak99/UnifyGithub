// Original url: http://wiki.unity3d.com/index.php/Save_and_Load_from_XML
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/CodeSnippets/Save_and_Load_from_XML.cs
// File based on original modification date of: 16 October 2012, at 02:01. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
Save and Load Info to XMLThis script will allow you to save and load data about an object into an XML file. To use, add an empty game object to the scene and attache the script to the object. Change the Player on that game object property to the item you wish to save properties about. When you start the scene, you will see a save and load button, these allow you to save and load the information from the xml file. The guts of what you save is located in the UserData class, change this as you see fit to allow you to save what you want. You will also need to update the save method to store the information that you are looking to store. At the moment, the code is only setup to store the postition of the object in world space. 
Author: Zumwalt 
using UnityEngine; 
using System.Collections; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 
 
public class _GameSaveLoad: MonoBehaviour { 
 
   // An example where the encoding can be found is at 
   // http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
   // We will just use the KISS method and cheat a little and use 
   // the examples from the web page since they are fully described 
 
   // This is our local private members 
   Rect _Save, _Load, _SaveMSG, _LoadMSG; 
   bool _ShouldSave, _ShouldLoad,_SwitchSave,_SwitchLoad; 
   string _FileLocation,_FileName; 
   public GameObject _Player; 
   UserData myData; 
   string _PlayerName; 
   string _data; 
 
   Vector3 VPosition; 
 
   // When the EGO is instansiated the Start will trigger 
   // so we setup our initial values for our local members 
   void Start () { 
      // We setup our rectangles for our messages 
      _Save=new Rect(10,80,100,20); 
      _Load=new Rect(10,100,100,20); 
      _SaveMSG=new Rect(10,120,400,40); 
      _LoadMSG=new Rect(10,140,400,40); 
 
      // Where we want to save and load to and from 
      _FileLocation=Application.dataPath; 
      _FileName="SaveData.xml"; 
 
      // for now, lets just set the name to Joe Schmoe 
      _PlayerName = "Joe Schmoe"; 
 
      // we need soemthing to store the information into 
      myData=new UserData(); 
   } 
 
   void Update () {} 
 
   void OnGUI() 
   {    
 
   //*************************************************** 
   // Loading The Player... 
   // **************************************************       
   if (GUI.Button(_Load,"Load")) { 
 
      GUI.Label(_LoadMSG,"Loading from: "+_FileLocation); 
      // Load our UserData into myData 
      LoadXML(); 
      if(_data.ToString() != "") 
      { 
        // notice how I use a reference to type (UserData) here, you need this 
        // so that the returned object is converted into the correct type 
        myData = (UserData)DeserializeObject(_data); 
		// set the players position to the data we loaded 
        VPosition=new Vector3(myData._iUser.x,myData._iUser.y,myData._iUser.z);              
        _Player.transform.position=VPosition; 
        // just a way to show that we loaded in ok 
        Debug.Log(myData._iUser.name); 
      } 
 
   } 
 
   //*************************************************** 
   // Saving The Player... 
   // **************************************************    
   if (GUI.Button(_Save,"Save")) { 
 
     GUI.Label(_SaveMSG,"Saving to: "+_FileLocation); 
     myData._iUser.x=_Player.transform.position.x; 
     myData._iUser.y=_Player.transform.position.y; 
     myData._iUser.z=_Player.transform.position.z; 
     myData._iUser.name=_PlayerName;    
 
     // Time to creat our XML! 
     _data = SerializeObject(myData); 
     // This is the final resulting XML from the serialization process 
     CreateXML(); 
     Debug.Log(_data); 
   } 
 
 
   } 
 
   /* The following metods came from the referenced URL */ 
   string UTF8ByteArrayToString(byte[] characters) 
   {      
      UTF8Encoding encoding = new UTF8Encoding(); 
      string constructedString = encoding.GetString(characters); 
      return (constructedString); 
   } 
 
   byte[] StringToUTF8ByteArray(string pXmlString) 
   { 
      UTF8Encoding encoding = new UTF8Encoding(); 
      byte[] byteArray = encoding.GetBytes(pXmlString); 
      return byteArray; 
   } 
 
   // Here we serialize our UserData object of myData 
   string SerializeObject(object pObject) 
   { 
      string XmlizedString = null; 
      MemoryStream memoryStream = new MemoryStream(); 
      XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
      xs.Serialize(xmlTextWriter, pObject); 
      memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
      XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
      return XmlizedString; 
   } 
 
   // Here we deserialize it back into its original form 
   object DeserializeObject(string pXmlizedString) 
   { 
      XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
      MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
      return xs.Deserialize(memoryStream); 
   } 
 
   // Finally our save and load methods for the file itself 
   void CreateXML() 
   { 
      StreamWriter writer; 
      FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
      if(!t.Exists) 
      { 
         writer = t.CreateText(); 
      } 
      else 
      { 
         t.Delete(); 
         writer = t.CreateText(); 
      } 
      writer.Write(_data); 
      writer.Close(); 
      Debug.Log("File written."); 
   } 
 
   void LoadXML() 
   { 
      StreamReader r = File.OpenText(_FileLocation+"\\"+ _FileName); 
      string _info = r.ReadToEnd(); 
      r.Close(); 
      _data=_info; 
      Debug.Log("File Read"); 
   } 
} 
 
// UserData is our custom class that holds our defined objects we want to store in XML format 
 public class UserData 
 { 
    // We have to define a default instance of the structure 
   public DemoData _iUser; 
    // Default constructor doesn't really do anything at the moment 
   public UserData() { } 
 
   // Anything we want to store in the XML file, we define it here 
   public struct DemoData 
   { 
      public float x; 
      public float y; 
      public float z; 
      public string name; 
   } 
}Here's a javascript version of the same c# script: 
import System;
import System.Collections;
import System.Xml;
import System.Xml.Serialization;
import System.IO;
import System.Text;
 
// Anything we want to store in the XML file, we define it here
class DemoData
{
	var x : float;
	var y : float;
	var z : float;
	var name : String;
}
 
// UserData is our custom class that holds our defined objects we want to store in XML format
 class UserData
 {
    // We have to define a default instance of the structure
   public var _iUser : DemoData = new DemoData();
    // Default constructor doesn't really do anything at the moment
   function UserData() { }
}
 
//public class GameSaveLoad: MonoBehaviour {
 
// An example where the encoding can be found is at
// http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp
// We will just use the KISS method and cheat a little and use
// the examples from the web page since they are fully described
 
// This is our local private members
private var _Save : Rect;
private var _Load : Rect;
private var _SaveMSG : Rect;
private var _LoadMSG : Rect;
//var _ShouldSave : boolean;
//var _ShouldLoad : boolean;
//var _SwitchSave : boolean;
//var _SwitchLoad : boolean;
private var _FileLocation : String;
private var _FileName : String = "SaveData.xml";
 
//public GameObject _Player;
var _Player : GameObject;
var _PlayerName : String = "Joe Schmoe";
 
private var myData : UserData;
private var _data : String;
 
private var VPosition : Vector3;
 
// When the EGO is instansiated the Start will trigger
// so we setup our initial values for our local members
//function Start () {
function Awake () {	
      // We setup our rectangles for our messages
      _Save=new Rect(10,80,100,20);
      _Load=new Rect(10,100,100,20);
      _SaveMSG=new Rect(10,120,200,40);
      _LoadMSG=new Rect(10,140,200,40);
 
      // Where we want to save and load to and from
      _FileLocation=Application.dataPath;
 
 
      // we need soemthing to store the information into
      myData=new UserData();
   }
 
function Update () {}
 
function OnGUI()
{   
 
   // ***************************************************
   // Loading The Player...
   // **************************************************       
   if (GUI.Button(_Load,"Load")) {
 
      GUI.Label(_LoadMSG,"Loading from: "+_FileLocation);
      // Load our UserData into myData
      LoadXML();
      if(_data.ToString() != "")
      {
         // notice how I use a reference to type (UserData) here, you need this
         // so that the returned object is converted into the correct type
         //myData = (UserData)DeserializeObject(_data);
         myData = DeserializeObject(_data);
         // set the players position to the data we loaded
         VPosition=new Vector3(myData._iUser.x,myData._iUser.y,myData._iUser.z);             
         _Player.transform.position=VPosition;
         // just a way to show that we loaded in ok
         Debug.Log(myData._iUser.name);
      }
 
   }
 
   // ***************************************************
   // Saving The Player...
   // **************************************************   
   if (GUI.Button(_Save,"Save")) {
 
      GUI.Label(_SaveMSG,"Saving to: "+_FileLocation);
      //Debug.Log("SaveLoadXML: sanity check:"+ _Player.transform.position.x);
 
      myData._iUser.x = _Player.transform.position.x;
      myData._iUser.y = _Player.transform.position.y;
      myData._iUser.z = _Player.transform.position.z;
      myData._iUser.name = _PlayerName;   
 
      // Time to creat our XML!
      _data = SerializeObject(myData);
      // This is the final resulting XML from the serialization process
      CreateXML();
      Debug.Log(_data);
   }
 
 
}
 
/* The following metods came from the referenced URL */
//string UTF8ByteArrayToString(byte[] characters)
function UTF8ByteArrayToString(characters : byte[] )
{     
   var encoding : UTF8Encoding  = new UTF8Encoding();
   var constructedString : String  = encoding.GetString(characters);
   return (constructedString);
}
 
//byte[] StringToUTF8ByteArray(string pXmlString)
function StringToUTF8ByteArray(pXmlString : String)
{
   var encoding : UTF8Encoding  = new UTF8Encoding();
   var byteArray : byte[]  = encoding.GetBytes(pXmlString);
   return byteArray;
}
 
   // Here we serialize our UserData object of myData
   //string SerializeObject(object pObject)
function SerializeObject(pObject : Object)
{
   var XmlizedString : String  = null;
   var memoryStream : MemoryStream  = new MemoryStream();
   var xs : XmlSerializer = new XmlSerializer(typeof(UserData));
   var xmlTextWriter : System.Xml.XmlTextWriter  = new System.Xml.XmlTextWriter(memoryStream, Encoding.UTF8);
   xs.Serialize(xmlTextWriter, pObject);
   memoryStream = xmlTextWriter.BaseStream; // (MemoryStream)
   XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
   return XmlizedString;
}
 
   // Here we deserialize it back into its original form
   //object DeserializeObject(string pXmlizedString)
function DeserializeObject(pXmlizedString : String)   
{
   var xs : XmlSerializer  = new XmlSerializer(typeof(UserData));
   var memoryStream : MemoryStream  = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
   var xmlTextWriter : System.Xml.XmlTextWriter  = new System.Xml.XmlTextWriter(memoryStream, Encoding.UTF8);
   return xs.Deserialize(memoryStream);
}
 
   // Finally our save and load methods for the file itself
function CreateXML()
{
   var writer : StreamWriter;
   //FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName);
   var t : FileInfo = new FileInfo(_FileLocation+"/"+ _FileName);
   if(!t.Exists)
   {
      writer = t.CreateText();
   }
   else
   {
      t.Delete();
      writer = t.CreateText();
   }
   writer.Write(_data);
   writer.Close();
   Debug.Log("File written.");
}
 
function LoadXML()
{
   //StreamReader r = File.OpenText(_FileLocation+"\\"+ _FileName);
   var r : StreamReader = File.OpenText(_FileLocation+"/"+ _FileName);
   var _info : String = r.ReadToEnd();
   r.Close();
   _data=_info;
   Debug.Log("File Read");
}Modify to static Those are scripts modified to static for easy extensions. Just drag GameSateData.cs script to GameObject for use. 
Links : http://unifycommunity.com/wiki/index.php?title=File:XML.zip 
}
