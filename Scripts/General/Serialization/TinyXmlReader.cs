// Original url: http://wiki.unity3d.com/index.php/TinyXmlReader
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/Serialization/TinyXmlReader.cs
// File based on original modification date of: 19 January 2013, at 23:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
You can use Mono's System.Xml for handling XML files but this requires including the System.Xml dll into your Unity program which increases its file size by about 1 MB. Not to mention the lack of documentation for using System.Xml on UnityScript. 
I found rolling my own XML parser was easier. Note however that this is a really simple XML parser, it doesn't recognize attributes (I did not implement it simply because I don't use XML attributes). 
TinyXmlReader.csusing UnityEngine;
using System.Collections;
 
public class TinyXmlReader
{
    private string xmlString = "";
    private int idx = 0;
 
    public TinyXmlReader(string newXmlString)
    {
        xmlString = newXmlString;
    }
 
    public string tagName = "";
    public bool isOpeningTag = false;
    public string content = "";
 
 
	// properly looks for the next index of _c, without stopping at line endings, allowing tags to be break lines	
	int IndexOf(char _c, int _i)
	{
		int i = _i;
		while (i < xmlString.Length)
		{
			if (xmlString[i] == _c)
				return i;
 
			++i;
		}
 
		return -1;
	}
 
    public bool Read()
    {
		if (idx > -1)
        	idx = xmlString.IndexOf("<", idx);
 
        if (idx == -1)
        {
            return false;
        }
        ++idx;
 
		// skip attributes, don't include them in the name!
		int endOfTag = IndexOf('>', idx);
		int endOfName = IndexOf(' ', idx);
        if ((endOfName == -1) || (endOfTag < endOfName))
	    {
			endOfName = endOfTag;
		}
 
		if (endOfTag == -1)
        {
            return false;
        }
 
        tagName = xmlString.Substring(idx, endOfName - idx);
 
        idx = endOfTag;
 
        // check if a closing tag
        if (tagName.StartsWith("/"))
        {
            isOpeningTag = false;
            tagName = tagName.Remove(0, 1); // remove the slash
        }
        else
        {
            isOpeningTag = true;
        }
 
        // if an opening tag, get the content
        if (isOpeningTag)
        {
            int startOfCloseTag = xmlString.IndexOf("<", idx);
            if (startOfCloseTag == -1)
            {
                return false;
            }
 
            content = xmlString.Substring(idx+1, startOfCloseTag-idx-1);
            content = content.Trim();
        }
 
        return true;
    }
 
    // returns false when the endingTag is encountered
    public bool Read(string endingTag)
    {
        bool retVal = Read();
        if (tagName == endingTag && !isOpeningTag)
        {
            retVal = false;
        }
        return retVal;
    }
}TinyXmlReader.jsclass TinyXmlReader
{
 
private var xmlString = "";
private var idx = 0;
 
function TinyXmlReader(aXmlString : String)
{
	xmlString = aXmlString;
}
 
var tagName = "";
var isOpeningTag = false;
var content = "";
 
function Read() : boolean
{
	idx = xmlString.IndexOf("<", idx);
	if (idx == -1)
	{
		return false;
	}
	++idx;
 
	var endOfTag = xmlString.IndexOf(">", idx);
	if (endOfTag == -1)
	{
		return false;
	}
 
	tagName = xmlString.Substring(idx, endOfTag-idx);
 
	idx = endOfTag;
 
	// check if a closing tag
	if (tagName.StartsWith("/"))
	{
		isOpeningTag = false;
		tagName = tagName.Remove(0, 1); // remove the slash
	}
	else
	{
		isOpeningTag = true;
	}
 
	// if an opening tag, get the content
	if (isOpeningTag)
	{
		var startOfCloseTag = xmlString.IndexOf("<", idx);
		content = xmlString.Substring(idx+1, startOfCloseTag-idx-1);
		content = content.Trim();
	}
 
	return true;
}
 
// returns false when the endingTag is encountered
function Read(endingTag : String) : boolean
{
	var retVal = Read();
	if (tagName == endingTag && !isOpeningTag)
	{
		retVal = false;
	}
	return retVal;
}
 
}UsageHere I provide example code on how to use the TinyXmlReader. 
private var text : String;
var skin : GUISkin;
 
function OnGUI()
{
	GUILayout.Label(text, skin.label);
}
 
function Start()
{
	var xmlText = System.IO.File.ReadAllText(Application.dataPath + "/Rifleman.xml");
	var reader = TinyXmlReader(xmlText);
	while (reader.Read())
	{
		if (reader.isOpeningTag)
		{
			text += (reader.tagName + " \"" + reader.content + "\"\n");
		}
		if (reader.tagName == "Skills" && reader.isOpeningTag)
		{
			while(reader.Read("Skills")) // read as long as not encountering the closing tag for Skills
			{
 
				if (reader.isOpeningTag)
				{
					text += ("Skill: " + reader.tagName + " \"" + reader.content + "\"\n");
				}
			}
		}
	}
}Plus, here's the XML file used for the example code: <xml> <?xml version="1.0" encoding="UTF-8"?> <Unit> <Type>Rifleman</Type> <Label>ライフル銃兵 é (для Windows тоже!)</Label> <MaxHP>100</MaxHP> <SightRange>10</SightRange> <Skills> <UnitMovement> <Label>Move</Label> <Animation>move</Animation> <Range>10</Range> <Speed>6</Speed> </UnitMovement> <SimpleAttack> <Label>Attack</Label> <Animation>shoot</Animation> <Range>10</Range> <Damage>20</Damage> <AttackTime>0.348</AttackTime> <AttackType>Ranged</AttackType> </SimpleAttack> <Grenade> <Label>Throw Grenade</Label> <Animation>shoot</Animation> <Range>10</Range> <Damage>20</Damage> <AttackTime>0.348</AttackTime> <ExplosionRadius>5</ExplosionRadius> <NoOfUses>3</NoOfUses> </Grenade> </Skills> <Pahabol>oompa loompa doompity doo, we wouldn't hit that and neither should you</Pahabol> </Unit> </xml> 
It produces this output: 
 
}
