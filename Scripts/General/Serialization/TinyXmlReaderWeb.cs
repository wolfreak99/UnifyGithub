// Original url: http://wiki.unity3d.com/index.php/TinyXmlReaderWeb
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/Serialization/TinyXmlReaderWeb.cs
// File based on original modification date of: 16 March 2012, at 17:07. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
Author: Berenger 
Contents [hide] 
1 Remark 
2 Description 
3 Usage 
4 CSharp - TinyXmlReaderWeb.cs 

Remark That script is an extension of http://www.unifycommunity.com/wiki/index.php?title=TinyXmlReader. 
Description OP's description : 
"You can use Mono's System.Xml for handling XML files but this requires including the System.Xml dll into your Unity program which increases its file size by about 1 MB. Not to mention the lack of documentation for using System.Xml on UnityScript. 
I found rolling my own XML parser was easier. Note however that this is a really simple XML parser, it doesn't recognize attributes (I did not implement it simply because I don't use XML attributes). " 

This extension's purpose is to be used with the web player. Basically, the main difference is the way the xml string is obtained, with www instead of System.IO. 
Usage Once the file was loaded and parsed, you can access the result array, a hashtable or custom class FromXML (see comments). The data is stored as follow, with i an even integer : 
* xml.strResult[i] => tag
* xml.strResult[i+1] => content
* xml.hashtableResult[ key ] => content
* xml.fromXmlResult[i].sVal => content as string
* xml.fromXmlResult[i].iVal => content as int
* xml.fromXmlResult[i].bVal => content as bool


using UnityEngine;
using System.Collections;
 
public class XmlLoaderTest : MonoBehaviour 
{
	void Awake (){ 
		StartCoroutine( LoadXML() );
	}
 
	IEnumerator LoadXML()
	{		
		TinyXmlReaderWeb xml = new TinyXmlReaderWeb(this, "http://somewhere/somename.xml");
 
		yield return xml.routine;
 
		string[] array = xml.strResult;
	}
}

CSharp - TinyXmlReaderWeb.cs using UnityEngine;
using System.Collections;
 
public class TinyXmlReaderWeb
{
    public XmlParserWeb(MonoBehaviour _sender, string url)
        : this(_sender, url, System.Text.Encoding.ASCII) { }
    public XmlParserWeb(MonoBehaviour _sender, string url, System.Text.Encoding encoding) 
    {
        // routine can be used later with a yield to wait the end of the parsing, that way :
        // yield return myXml.routine;
        routine = _sender.StartCoroutine(Parse(url, encoding));
    }
 
    public Coroutine routine { get; private set; } // Can be used with a yield instruction
    public string[] strResult { get; private set; } // The result of the parsing, array[i] is the tag, array[i+1] is the content
    public string wwwError { get; private set; } // If something goes wrong, you can check the log
 
    public FromXML[] fromXmlResult { get { return FromXML.Format(strResult); } } // The result stored in FromXML format, see the end of the code
    public Hashtable hashtableResult // The result in a hastable (key, value)
    { 
        get 
        {
            Hashtable result = new Hashtable();
            for (int i = 0; i < strResult.Length; i += 2)
            {
                if (!result.ContainsKey(strResult[i]))
                    result.Add(strResult[i], strResult[i + 1]);
                else
                    Debug.LogWarning("The key [" + strResult[i] + "] already exist and will be ignored.");
            }
            return result;
        } 
    }
 
    private IEnumerator Parse(string url, System.Text.Encoding encoding)
	{
        WWW www = new WWW(url);
 
		yield return www;
 
        wwwError = www.error;
 
		byte[] bytes = www.bytes;
        string xmlStr = encoding.GetString(bytes);
 
        ArrayList xmlList = new ArrayList();
	    TinyXmlReader reader = new TinyXmlReader(xmlStr);
	    while (reader.Read())
	    {
	        if (reader.isOpeningTag)
	        {				
				xmlList.Add(reader.tagName);
				xmlList.Add(reader.content);
	        }
	    }
        strResult = (string[])xmlList.ToArray(typeof(string));
 
        yield break;
	}
 
    // source : http://www.unifycommunity.com/wiki/index.php?title=TinyXmlReader
    private class TinyXmlReader
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
    }        
}
 
// This is a class to retrieve the data in a user friendly format. With a .xml such as :
// <Fruit>Apple</Fruit>
// <Count>2</Count>
// <Tasty>1</Tasty>
// You can then use the data like that :
// string fruitName = fromXml[0].sVal;
// int c = fromXml[1].iVal;
// etc ...
public class FromXML
{
    private string m_key;
    private string m_val;
 
    public FromXML(string k, string v) { m_key = k; m_val = v; }
 
    // That way seems to be the fastest to compare strings
    public bool IsKey(string _key) { return string.Equals(m_key, _key, System.StringComparison.OrdinalIgnoreCase); }
    public bool IsVal(string _val) { return string.Equals(m_val, _val, System.StringComparison.OrdinalIgnoreCase); }
    public string sVal { get { return m_val; } }
    public int iVal { get { return int.Parse(m_val); } }
    public bool bVal { get { return !IsVal("0"); } }
 
    public override string ToString()
    {
        return string.Format("[FromXML: Key={0}, Val={1}]", m_key, m_val);
    }
 
    public static FromXML[] Format(string[] _from)
    {
        FromXML[] result = new FromXML[_from.Length / 2];
        for (int i = 0, p = 0; i < _from.Length; i += 2, p++)
            result[p] = new FromXML(_from[i], _from[i + 1]);
 
        return result;
    }
    public static void Debug(FromXML[] array)
    {
        string s = "";
        foreach (FromXML F in array)
            s += F.ToString() + "\n";
        UnityEngine.Debug.Log(s);
    }
}
}
