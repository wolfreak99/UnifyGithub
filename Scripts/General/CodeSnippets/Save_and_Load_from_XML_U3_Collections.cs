// Original url: http://wiki.unity3d.com/index.php/Save_and_Load_from_XML_U3_Collections
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/Save_and_Load_from_XML_U3_Collections.cs
// File based on original modification date of: 14 August 2012, at 00:38. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.CodeSnippets
{
Save and Load from XML U3 CollectionsThis script will allow you to save and load data collections into an XML file. To use, add an empty game object to the scene and attache the script to the object. When you start the scene, you will see a save and load button, these allow you to save and load the information from the xml file. 
The purpose to this code is to allow you to collect objects in a given scene and save there state to the XML file, for demonstration purposes, the information that is stored in the XML file is the name of the object, the instanceID, transform positions x,y and z, you can expand on this as much as you want. Two files exist in this code, the first is the class structure that is the serializable object which we will be populating to save, the second is the process to do the actual saving and loading. 
Author: Zumwalt
Date: 09/30/2010 
SaveStructure.csusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
 
[Serializable()]
public class SaveStructure : ISerializable
{
    [Serializable()]
    public class GameItems : ISerializable
    {
        public string ID;
        public string Name;
        public float posx;
        public float posy;
        public float posz;
 
        public GameItems()
        {
            ID = string.Empty;
            Name = string.Empty;
            posx = 0;
            posy = 0;
            posz = 0;
        }
 
        // Deserialization
        public GameItems(SerializationInfo info, StreamingContext ctxt)
        {
            ID = (String)info.GetValue("ID", typeof(string));
            Name = (String)info.GetValue("Name", typeof(string));
            posx = (float)info.GetValue("posx", typeof(float));
            posy = (float)info.GetValue("posy", typeof(float));
            posz = (float)info.GetValue("posz", typeof(float));
        }
 
        // Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("ID", ID);
            info.AddValue("Name", Name);
            info.AddValue("posx", posx);
            info.AddValue("posy", posy);
            info.AddValue("posz", posz);
        }
    }
 
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
    }
}_GameSaveLoad.csusing UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;
 
public class _GameSaveLoad : MonoBehaviour
{
    // An example where the encoding can be found is at 
    // http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
    // We will just use the KISS method and cheat a little and use 
    // the examples from the web page since they are fully described 
 
    // This is our local private members 
    Rect _Save, _Load, _SaveMSG, _LoadMSG;
    bool _ShouldSave, _ShouldLoad, _SwitchSave, _SwitchLoad;
    string _FileLocation, _FileName;
 
    Vector3 VPosition;
 
    List<SaveStructure.GameItems> _GameItems;
    public GameObject[] bodies;
    string _data = string.Empty;
 
    void Awake()
    {
        _GameItems = new List<SaveStructure.GameItems>();
    }
 
    // When the EGO is instansiated the Start will trigger 
    // so we setup our initial values for our local members 
    void Start()
    {
        // We setup our rectangles for our messages 
        _Save = new Rect(10, 80, 100, 20);
        _Load = new Rect(10, 100, 100, 20);
        _SaveMSG = new Rect(10, 120, 400, 40);
        _LoadMSG = new Rect(10, 140, 400, 40);
 
        // Where we want to save and load to and from 
        _FileLocation = Application.dataPath+"/";
        _FileName = "SaveData.xml";
    }
 
    void Update() { }
 
    bool isSaving = false;
    bool isLoading = false;
    void OnGUI()
    {
        //*************************************************** 
        // Loading The Player... 
        // **************************************************       
        if (GUI.Button(_Load, "Load") && !isLoading)
        {
            try
            {
                isLoading = true;
                GUI.Label(_LoadMSG, "Loading from: " + _FileLocation);
                LoadXML();
                Debug.Log("Data loaded");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            finally
            {
                isLoading = false;
            }
 
        }
 
        //*************************************************** 
        // Saving The Player... 
        // **************************************************    
        if (GUI.Button(_Save, "Save") && !isSaving)
        {
            try
            {
                isSaving = true;
                GUI.Label(_SaveMSG, "Saving to: " + _FileLocation);
 
                bodies = FindObjectsOfType(typeof(GameObject)) as GameObject[];
                _GameItems = new List<SaveStructure.GameItems>();
                SaveStructure.GameItems itm;
                foreach (GameObject body in bodies)
                {
                    itm = new SaveStructure.GameItems();
                    itm.ID = body.name + "_" + body.GetInstanceID();
                    itm.Name = body.name;
                    itm.posx = body.transform.position.x;
                    itm.posy = body.transform.position.y;
                    itm.posz = body.transform.position.z;
                    _GameItems.Add(itm);
                }
 
                // Time to creat our XML! 
                _data = SerializeObject(_GameItems);
 
                CreateXML();
                Debug.Log("Data Saved");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            finally
            {
                isSaving = false;
            }
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
        XmlSerializer xs = new XmlSerializer(typeof(List<SaveStructure.GameItems>));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }
 
    // Here we deserialize it back into its original form 
    object DeserializeObject(string pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(List<SaveStructure.GameItems>));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        return xs.Deserialize(memoryStream);
    }
 
    // Finally our save and load methods for the file itself 
    void CreateXML()
    {
        StreamWriter writer;
        FileInfo t = new FileInfo(_FileLocation + "/" + _FileName);
        if (!t.Exists)
        {
            writer = t.CreateText();
        }
        else
        {
            //t.Delete();
            writer = t.CreateText();
        }
        writer.Write(_data);
        writer.Close();
        Debug.Log("File written.");
    }
 
    void LoadXML()
    {
        if (File.Exists(_FileLocation + "/" + _FileName))
        {
            StreamReader r = File.OpenText(_FileLocation + "/" + _FileName);
            string _info = r.ReadToEnd();
            r.Close();
            if(_data.ToString() != "")
	    {
	        // notice how I use a reference to type (UserData) here, you need this
	        // so that the returned object is converted into the correct type
	        _GameItems = (List<SaveStructure.GameItems>)DeserializeObject(_info);
		for(int i = 0; i < _GameItems.Count; i++)
		{
		     VPosition = new Vector3(_GameItems[i].posx, _GameItems[i].posy, _GameItems[i].posz);
		     bodies[i].transform.position=VPosition;
		}
	        Debug.Log("File Read with item count: " + _GameItems.Count);
	    }
        }
        else
        {
            Debug.Log("Files does not exist: " + _FileLocation + "/" + _FileName);
        }
    }
}
}
