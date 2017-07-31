// Original url: http://wiki.unity3d.com/index.php/PropertyListSerializer
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/Serialization/PropertyListSerializer.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
Author: capnbishop 
Description The PropertyListSerializer.js script is used to load and save an XML property list file to and from a hierarchical hashtable (in which the root hashtable can contain child hashtables and arrays). This can provide a convenient and dynamic means of serializing a complex hierarchy of game data into XML files. 
When loading, the resulting hashtable can include 8 different types of values: string, integer, real, date, data, boolean, dictionary, and array. Data elements are loaded as strings. Dictionaries are loaded as hashtables. Arrays are loaded as arrays. Each value is loaded with an associating key, except for elements of an array. Thus, each child hashtable and array also have associating keys, and can be combined to create a complex hierarchy of key value pairs and arrays. 
When saving, the resulting XML file will contain the same hierarchy of data. All data will end up being stored as a string, but with an associated value type. Strings, integers, and decimals values are stored as such. Dates are stored in ISO 8601 format. Hashtables are stored as a plist key/value dictionary, and arrays as a series of keyless values. 
The loader passes a lot of values by reference, and performs a considerable amount of recursion. Primitive values had to be passed by reference. Unity's JavaScript only passes objects by reference, and cannot explicitly pass a primitive by reference. As such, we've had to create a special ValueObject, which is just an abstract object that holds a single value. This object is then passed by reference, and the primitive value is set to its val property. 
This plist loader conforms to Apple's plist DOCTYPE definition: http://www.apple.com/DTDs/PropertyList-1.0.dtd 
Usage To load a plist, call the LoadPlistFromFile(String, Hashtable) function and pass it the path to a plist file and a hashtable that will be populated with the plist data. LoadPlistFromFile(String, Hashtable) will return true if the operation was successful. The plist passed must not be null, as it has to be passed by reference. 
The easiest way to create a plist is with Apple's Property List Editor, which is a part of the Xcode developer tools. 
LoadPlistFromFile() needs to be passed an already instantiated hashtable, because that hashtable is passed by reference. Because of this, it is also able to be passed an already populated plist hashtable. This is fine, and the script will mesh the two hashtables by overwriting the values of existing keys along the hashtable tree structure. For sub-hashtables, the overwriting will follow the structure, and only overwrite the existing keys within the sub-hashtables. Essentially, sub-hashtables themselves aren't overwritten, but the key/value pairs within them can be. Elements of arrays are simply appended to the existing array, and not overwritten at all. 
import System;
import System.IO;
 
function Start () {
    var xmlFiles = Directory.GetFiles((Application.dataPath), "*.plist");
    if (xmlFiles.length < 1) { Debug.Log("Unable to find any plist files."); }
    else {
        var plist = new Hashtable();
        if (PropertyListSerializer.LoadPlistFromFile(xmlFiles[0], plist)) {
            for (var i in plist.Keys) {
                Debug.Log(plist[i]);
            }
        }
        else { Debug.Log("Unable to open plist file."); }
    }
}To save a hashtable to file, call the SavePlistToFile(String, Hashtable) function and pass it the path to the destination plist file, and the hashtable to be saved. SavePlistToFile(String, Hashtable) will return true if the operation was successful. 
var playerData : Hashtable = new Hashtable();
 
function Start () {
    // init the player data
    playerData["Health"] = 100;
    playerData["Guns"] = new Array();
    playerData["Guns"].Add("Knife");
    playerData["Guns"].Add("Pistol");
    playerData["Grenades"] = new Hashtable();
    playerData["Grenades"]["FragmentationCount"] = 1;
    playerData["Grendaes"]["IncendiaryCount"] = 1;
}
 
function Update () {
    // if the player presses the quick save button, serialize the savegame data
    if (Input.GetButtonDown("Save")) {
        var xmlFile = Application.dataPath + "/" + "SaveFile.plist";
        PropertyListSerializer.SavePlistToFile(xmlFile, playerData);
    }
}Javascript - PropertyListSerializer.js // We use the .Net XML and DateTime libraries in this script
import System;
import System.IO;
import System.Xml;
import System.Text;
import System.DateTime;
import System.Globalization;
 
// We need to be able to pass values by reference fluidly, including primatives.  UnityScript doesn't support passing non objects by reference.  Thus, we've had to create an object that simple stores a single value.  This way, we can pass the ValueObject, and just use its val value.
class ValueObject { function ValueObject(aVal) { val = aVal; } function ValueObject() {} var val; };
 
// LoadPlistFromFile(String, Hashtable) is the root public function for loading a plist file into memory.  The plist is loaded into the hashtable passed.  Return true/false for success/failure
static public function LoadPlistFromFile(xmlFile : String, plist : Hashtable) : boolean {
    // Unless plist has already been initiated, it can't be passed by reference, which it has to be
    if (!plist) { Debug.LogError("Cannot pass null plist value by reference to LoadPlistFromFile."); return false; }
 
    // If the file doesn't exist, return a false
    if (!File.Exists(xmlFile)) { Debug.LogError("File doesn't exist: " + xmlFile); return false; }
 
    // Load the file into an XML data object
    var sr = new StreamReader(xmlFile);
    var txt = sr.ReadToEnd();
    sr.Close();
    var xml = new XmlDocument();
    xml.LoadXml(txt);
 
    // Find the root plist object.  If it doesn't exist or isn't a plist node, state so and return null.
    var plistNode = xml.LastChild;
    if (plistNode.Name != "plist") { Debug.LogError("This is not a plist file: " + xmlFile); return false; }
 
    // Get the version number of this plist file.  This script was designed to work with version 1.0.  If this is an incorrect version, state so and return null
    var plistVers = plistNode.Attributes["version"].Value;
    var plistVersSupported = "1.0";
    if (plistVers != plistVersSupported) { Debug.LogError("This is an unsupported plist version: " + plistVers + ". Require version " + plistVersSupported); return false; }
 
    // Get the root plist dict object.  This is the root object that contains all the data in the plist file.  This object will be the hashtable.
    var dictNode = plistNode.FirstChild;
    if (dictNode.Name != "dict") { Debug.LogError("Missing root dict from plist file: " + xmlFile); return false; }
 
    // Using the root dict node, load the plist into a hashtable and return the result.
    // If successful, this will return true, and the plist object will be populated with all the appropriate information.
    return LoadDictFromPlistNode(dictNode, plist);
}
 
// LoadDictFromPlistNode(XmlNode, Hashtable) takes an XML node and loads it as a hashtable.  Return true/false for success/failure
static private function LoadDictFromPlistNode(node : XmlNode, dict : Hashtable) : boolean {
    // If we were passed a null object, return false
    if (!node) { Debug.LogError("Attempted to load a null plist dict node."); return false; }
    // If we were passed a non dict node, then post an error stating so and return false
    if (node.Name != "dict") { Debug.LogError("Attempted to load an dict from a non-array node type: " + node + ", " + node.Name); return false; }
 
    // We could be passed an null hashtable.  If so, initialize it.
    if (!dict) { dict = new Hashtable(); }
 
    // Identify how many child nodes there are in this dict element and itterate through them.
    // A dict element will contain a series of key/value pairs.  As such, we're going through the child nodes in pairs.
    var cnodeCount = node.ChildNodes.Count;
    for (var i = 0; i+1 < cnodeCount; i = i+2) {
        // Select the key and value child nodes
        var keynode = node.ChildNodes.Item(i);
        var valuenode = node.ChildNodes.Item(i+1);
 
        // If this node isn't a 'key'
        if (keynode.Name == "key") {
            // Establish our variables to hold the key and value.
            var key = keynode.InnerText;
            var value : ValueObject = new ValueObject();
 
            // Load the value node.
            // If the value node loaded successfully, add the key/value pair to the dict hashtable.
            if (LoadValueFromPlistNode(valuenode, value)) {
                // This could be one of several different possible data types, including another dict.
                // AddKeyValueToDict() handles this by replacing existing key values that overlap, and doing so recursively for dict values.
                // If this not successful, post a message stating so and return false.
                if (!AddKeyValueToDict(dict, key, value)) { Debug.LogError("Failed to add key value to dict when loading plist from dict"); return false; }
            }
            // If the value did not load correctly, post a message stating so and return false.
            else { Debug.LogError("Did not load plist value correctly for key in node: " + key + ", " + node); return false; }
        }
        // Because the plist was formatted incorrectly, post a message stating so and return false.
        else { Debug.LogError("The plist being loaded may be corrupt."); return false; }
    }
 
    // If we got this far, the dict was loaded successfully.  Return true
    return true;
}
 
// LoadValueFromPlistNode(XmlNode, Object) takes an XML node and loads its value into the passed value object.
// The value for this node can be one of several different possible types.  Return true/false for success/failure
static private function LoadValueFromPlistNode(node : XmlNode, value : ValueObject) : boolean {
    // If passed a null node, post an error stating so and return false
    if (!node) { Debug.LogError("Attempted to load a null plist value node."); return false; }
 
    // Identify the data type for the value node and assign it accordingly
    if      (node.Name == "string")   { value.val = node.InnerText; }
    else if (node.Name == "integer")  { value.val = parseInt(node.InnerText); }
    else if (node.Name == "real")     { value.val = parseFloat(node.InnerText); }
    else if (node.Name == "date")     { value.val = DateTime.Parse(node.InnerText, null, DateTimeStyles.None); } // Date objects are in ISO 8601 format
    else if (node.Name == "data")     { value.val = node.InnerText; } // Data objects are just loaded as a string
    else if (node.Name == "true")     { value.val = true; } // Boollean values are empty objects, simply identified with a name being "true" or "false"
    else if (node.Name == "false")    { value.val = false; }
    // The value can be an array or dict type.  In this case, we need to recursively call the appropriate loader functions for dict and arrays.
    // These functions will in turn return a boolean value for their success, so we can just return that.
    // The val value also has to be instantiated, since it's being passed by reference.
    else if (node.Name == "dict")     { value.val = new Hashtable(); return LoadDictFromPlistNode(node, value.val); }
    else if (node.Name == "array")    { value.val = new Array(); return LoadArrayFromPlistNode(node, value.val); }
    else                              { Debug.LogError("Attempted to load a value from a non value type node: " + node + ", " + node.Name); return false; }
 
    // If we made it this far, then we had success.  Return true.
    return true;
}
 
// LoadArrayFromPlistNode(XmlNode, Array) takes an XML node and loads it as an Array.  A plist array is just a series of value objects without keys.
static private function LoadArrayFromPlistNode(node : XmlNode, array : Array) : boolean {
    // If we were passed a null node object, then post an error stating so and return false
    if (!node) { Debug.LogError("Attempted to load a null plist array node."); return false; }
    // If we were passed a non array node, then post an error stating so and return false
    if (node.Name != "array") { Debug.LogError("Attempted to load an array from a non-array node type: " + node + ", " + node.Name); return false; }
 
    // We can be passed an empty array object.  If so, initialize it
    if (!array) { array = new Array(); }
 
    // Itterate through the child nodes for this array object
    var nodeCount = node.ChildNodes.Count;
    for (var i = 0; i < nodeCount; i++) {
        // Establish variables to hold the child node of the array, and it's value
        var cnode = node.ChildNodes.Item(i);
        var element = new ValueObject();
        // Attempt to load the value from the current array node.
        // If successful, add it as an element of the array.  If not, post and error stating so and return false.
        if (LoadValueFromPlistNode(cnode, element)) { array.Add(element.val); }
        else { return false; }
    }
 
    // If we made it through the array without errors, return true
    return true;
}
 
// AddKeyValueToDict(Hashtable, String, Object) handles adding new or existing values to a hashtable.
// A hashtable can already contain the key that we're trying to add.  If it's a regular value, we can just replace the existing one with the new one.
// If trying to add a hashtable value to another hashtable that already has a hashtable for that key, then we need to recursively add new values.
// This allows us to load two plists with overlapping values so that they will be combined into one big plist hashtable, and the new values will replace the old ones with the same keys
static private function AddKeyValueToDict(dict : Hashtable, key : String, value : ValueObject) : boolean {
    // Make sure that we have values that we can work with.
    if (!dict || !key || key == "" || !value) { Debug.LogError("Attempted to AddKeyValueToDict() with null objects."); return false; }
 
    // If the hashtabel doesn't already contain the key, they we can just go ahead and add it.
    if (!dict.ContainsKey(key)) { dict.Add(key, value.val); return true; }
 
    // At this point, the dict contains already contains the key we're trying to add.
    // If the value for this key is of a different type between the dict and the new value, then we have a type mismatch.
    // Post an error stating so, but go ahead and overwrite the existing key value.
    if (typeof(value.val) != typeof(dict[key])) {
        Debug.LogWarning("Value type mismatch for overlapping key (will replace old value with new one): " + value.val + ", " + dict[key] + ", " + key);
        dict[key] = value.val;
    }
    // If the value for this key is a hashtable, then we need to recursively add the key values of each hashtable.
    else if (typeof(value.val) == Hashtable) {
        // Itterate through the elements of the value's hashtable.
        for (element in value.val.Keys) {
            // Recursively attempt to add/repalce the elements of the value hashtable to the dict's value hashtable.
            // If this fails, post a message stating so and return false.
            if (!AddKeyValueToDict(dict[key], element, new ValueObject(value.val[element]))) {
                Debug.LogError("Failed to add key value to dict: " + element + ", " + value.val[element] + ", " + dict[key]);
                return false;
            }
        }
    }
    // If the value is an array, then there's really no way we can tell which elements to overwrite, because this is done based on the congruent keys.
    // Thus, we'll just add the elements of the array to the existing array.
    else if (typeof(value.val) == Array) {
        for (element in value.val) { dict[key].Add(element); }
    }
    // If the key value is not an array or a hashtable, then it's a primitive value that we can easily write over.
    else { dict[key] = value.val; }
 
    // If we've gotten this far, then we were successful.  Return true.
    return true;
}
 
// SavePlistToFile(String, Hashtable) is the root public function for saving a hashtable to a plist file.  The hashtable is saved to the file location passed as a plist.  Return true/false for success/failure.
static function SavePlistToFile (xmlFile : String, plist : Hashtable) : boolean {
    // If the hashtable is null, then there's apparently an issue; fail out.
    if (!plist) { Debug.LogError("Passed a null plist hashtable to SavePlistToFile."); return false; }
 
    // Create the base xml document that we will use to write the data
    var xml = new XmlDocument();
 
    // Create the root XML declaration
    // This, and the DOCTYPE, below, are standard parts of a XML property list file
    var xmldecl = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
    xml.PrependChild(xmldecl);
 
    // Create the DOCTYPE
    var doctype = xml.CreateDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
    xml.AppendChild(doctype);
 
    // Create the root plist node, with a version number attribute.
    // Every plist file has this as the root element.  We're using version 1.0 of the plist scheme
    var plistNode = xml.CreateNode(XmlNodeType.Element, "plist", null);
    var plistVers = xml.CreateNode(XmlNodeType.Attribute, "version", null);
    plistVers.Value = "1.0";
    plistNode.Attributes.Append(plistVers);
    xml.AppendChild(plistNode);
 
    // Now that we've created the base for the XML file, we can add all of our information to it.
    // Pass the plist data and the root dict node to SaveDictToPlistNode, which will write the plist data to the dict node.
    // This function will itterate through the hashtable hierarchy and call itself recursively for child hashtables.
    if (!SaveDictToPlistNode(plistNode, plist)) {
        // If for some reason we failed, post an error and return false.
        Debug.LogError("Failed to save plist data to root dict node: " + plist);
        return false;
    } else { // We were successful
        // Create a StreamWriter and write the XML file to disk.
        // (do not append and UTF-8 are default, but we're defining it explicitly just in case)
        var sw = new StreamWriter(xmlFile, false, System.Text.Encoding.UTF8);
        xml.Save(sw);
        sw.Close();
    }
 
    // We're done here.  If there were any failures, they would have returned false.
    // Return true to indicate success.
    return true;
}
 
// SaveDictToPlistNode(XmlNode, Hashtable) takes the hashtable and stores it into the XML node.  Return true/false for success/failure.
// If the hashtable contains more hashtables, this function may call itself recursively.
static private function SaveDictToPlistNode(node : XmlNode, dict : Hashtable) : boolean {
    // If we were passed a null object, return false
    if (!node) { Debug.LogError("Attempted to save a null plist dict node."); return false; }
 
    var dictNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "dict", null);
    node.AppendChild(dictNode);
 
    // We could be passed an null hashtable.  This isn't necessarily an error.
    if (!dict) { Debug.LogWarning("Attemped to save a null dict: " + dict); return true; }
 
    // Itterate through the keys in the hashtable
    for (var key in dict.Keys) {
        // Since plists are key value pairs, save the key to the plist as a new XML element
        var keyNode = node.OwnerDocument.CreateElement("key");
        keyNode.InnerText = key;
        dictNode.AppendChild(keyNode);
 
        // The name of the value element is based on the datatype of the value.  We need to serialize it accordingly.  Pass the XML node and the hash value to SaveValueToPlistNode to handle this.
        if (!SaveValueToPlistNode(dictNode, dict[key])) {
            // If SaveValueToPlistNode() returns false, that means there was an error.  Return false to indicate this up the line.
            Debug.LogError("Failed to save value to plist node: " + key);
            return false;
        }
    }
 
    // If we got this far then all is well.  Return true to indicate success.
    return true;
}
 
// SaveValueToPlistNode(XmlNode, Object) takes a value and saves it as a plist value in the XmlNode passed.
// A plist value is an XML element with a name based on the data type, and an inner text of the actual value.
private static function SaveValueToPlistNode(node : XmlNode, value) : boolean {
    // The node passed will be the parent node to the new value node.
    var valNode : XmlNode;
 
    // Identify the data type for the value and serialize it accordingly
    if      (typeof(value) == String)       { valNode = node.OwnerDocument.CreateElement("string"); }
    else if (IsInteger(value))              { valNode = node.OwnerDocument.CreateElement("integer"); }
    else if (IsDecimal(value))              { valNode = node.OwnerDocument.CreateElement("real"); }
    else if (typeof(value) == DateTime) {
        // Dates need to be stored in ISO 8601 format
        valNode = node.OwnerDocument.CreateElement("date");
        valNode.InnerText = value.ToUniversalTime().ToString("o");
        node.AppendChild(valNode);
        return true;
    }
    else if (typeof(value) == Boolean) {
        // Boolean values are empty elements, simply being stored as an elemement with a name of true or false
        if (value == true) { valNode = node.OwnerDocument.CreateElement("true"); }
        else               { valNode = node.OwnerDocument.CreateElement("false"); }
        node.AppendChild(valNode);
        return true;
    }
    // Hashtables and arrays require special functions to save their values in an itterative and recursive manner.
    // The functions will return true/false to indicate success/failure, so pass those on.
    else if (typeof(value) == Hashtable)    { return SaveDictToPlistNode(node, value); }
    else if (typeof(value) == Array) { return SaveArrayToPlistNode(node, value); }
    // Anything that doesn't fit the defined data types will just be stored as "data", which is effectively a string.
    else { valNode = node.OwnerDocument.CreateElement("data"); }
 
    // Some of the values (strings, numbers, data) basically get stored as a string.  The rest will store their values in their special format and return true for success.  If we made it this far, then the value in valNode must be stored as a string.
    valNode.InnerText = value.ToString();
    node.AppendChild(valNode);
 
    // We're done.  Return true for success.
    return true;
}
 
// SaveArrayToPlistNode(XmlNode, Array) takes an arry and stores it as an XML plist array.  A plist array is just a series of value objects without keys.
private static function SaveArrayToPlistNode (node : XmlNode, array : Array) : boolean {
    // Create the value node as an "array" element.
    var arrayNode = node.OwnerDocument.CreateElement("array");
    node.AppendChild(arrayNode);
 
    // Each element in the array can be any data type.  Itterate through the array and send each element to SaveValueToPlistNode(), where it can be stored accordingly based on its data type.
    for (var element in array) {
        // If SaveValueToPlistNode() returns false, then there was a problem.  Return false in that case.
        if (!SaveValueToPlistNode(arrayNode, element)) { return false; }
    }
 
    // If we made it this far then all is well.  Return true for success.
    return true;
}
 
// Property list files store numbers as either an "integer" or "real" (decimal) number.  There's no simple way of identifying a numeric variable like this, other than to compare it to every single datatype of that kind.  Thus, we have the IsInteger() and IsDecimal() functions that serve this purpose.  These will simple return true if the variable is of an integer or decimal based datatype (respectively), and false otherwise.
static private function IsDecimal (num) {
    var type = typeof(num);
    if (type == System.Single ||
        type == System.Double ||
        type == System.Decimal)
    { return true; }
 
    return false;
}
static private function IsInteger (num) {
    var type = typeof(num);
    if (type == System.Int16 ||
        type == System.Int32 ||
        type == System.Int64)
    { return true; }
 
    return false;
}
}
