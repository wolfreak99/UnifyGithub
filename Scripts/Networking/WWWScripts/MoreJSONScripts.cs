// Original url: http://wiki.unity3d.com/index.php/MoreJSONScripts
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Networking/WWWScripts/MoreJSONScripts.cs
// File based on original modification date of: 20 January 2013, at 00:15. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
Contents [hide] 
1 Author 
2 Code 
3 Serialise To Objects, base class 
4 JSON Attribute 
5 Example 
5.1 Example Data Object 
5.2 Example Usage 

AuthorOPless 


CodeIf I remember correctly I pilfered this script from here [[1]] 
This converts JSON to HashTables and ArrayLists 
using System;
//using System.Data;
using System.Collections;
using System.Globalization;
using System.Text;
 
 
	/// <summary>
	/// This class encodes and decodes JSON strings.
	/// Spec. details, see http://www.json.org/
	/// 
	/// JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and Hashtable.
	/// All numbers are parsed to doubles.
	/// </summary>
	public class JSON
	{
		public const int TOKEN_NONE = 0;
		public const int TOKEN_CURLY_OPEN = 1;
		public const int TOKEN_CURLY_CLOSE = 2;
		public const int TOKEN_SQUARED_OPEN = 3;
		public const int TOKEN_SQUARED_CLOSE = 4;
		public const int TOKEN_COLON = 5;
		public const int TOKEN_COMMA = 6;
		public const int TOKEN_STRING = 7;
		public const int TOKEN_NUMBER = 8;
		public const int TOKEN_TRUE = 9;
		public const int TOKEN_FALSE = 10;
		public const int TOKEN_NULL = 11;
 
		private const int BUILDER_CAPACITY = 2000;
 
		/// <summary>
		/// Parses the string json into a value
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		public static object JsonDecode(string json)
		{
			bool success = true;
 
			return JsonDecode(json, ref success);
		}
 
		/// <summary>
		/// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <param name="success">Successful parse?</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		public static object JsonDecode(string json, ref bool success)
		{
			success = true;
			if (json != null) {
				char[] charArray = json.ToCharArray();
				int index = 0;
				object value = ParseValue(charArray, ref index, ref success);
				return value;
			} else {
				return null;
			}
		}
 
		/// <summary>
		/// Converts a Hashtable / ArrayList object into a JSON string
		/// </summary>
		/// <param name="json">A Hashtable / ArrayList</param>
		/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
		public static string JsonEncode(object json)
		{
			StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
			bool success = SerializeValue(json, builder);
			return (success ? builder.ToString() : null);
		}
 
		protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			Hashtable table = new Hashtable();
			int token;
 
			// {
			NextToken(json, ref index);
 
			bool done = false;
			while (!done) {
				token = LookAhead(json, index);
				if (token == JSON.TOKEN_NONE) {
					success = false;
					return null;
				} else if (token == JSON.TOKEN_COMMA) {
					NextToken(json, ref index);
				} else if (token == JSON.TOKEN_CURLY_CLOSE) {
					NextToken(json, ref index);
					return table;
				} else {
 
					// name
					string name = ParseString(json, ref index, ref success);
					if (!success) {
						success = false;
						return null;
					}
 
					// :
					token = NextToken(json, ref index);
					if (token != JSON.TOKEN_COLON) {
						success = false;
						return null;
					}
 
					// value
					object value = ParseValue(json, ref index, ref success);
					if (!success) {
						success = false;
						return null;
					}
 
					table[name] = value;
				}
			}
 
			return table;
		}
 
		protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			ArrayList array = new ArrayList();
 
			// [
			NextToken(json, ref index);
 
			bool done = false;
			while (!done) {
				int token = LookAhead(json, index);
				if (token == JSON.TOKEN_NONE) {
					success = false;
					return null;
				} else if (token == JSON.TOKEN_COMMA) {
					NextToken(json, ref index);
				} else if (token == JSON.TOKEN_SQUARED_CLOSE) {
					NextToken(json, ref index);
					break;
				} else {
					object value = ParseValue(json, ref index, ref success);
					if (!success) {
						return null;
					}
 
					array.Add(value);
				}
			}
 
			return array;
		}
 
		protected static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (LookAhead(json, index)) {
				case JSON.TOKEN_STRING:
					return ParseString(json, ref index, ref success);
				case JSON.TOKEN_NUMBER:
					return ParseNumber(json, ref index);
				case JSON.TOKEN_CURLY_OPEN:
					return ParseObject(json, ref index, ref success);
				case JSON.TOKEN_SQUARED_OPEN:
					return ParseArray(json, ref index, ref success);
				case JSON.TOKEN_TRUE:
					NextToken(json, ref index);
					return Boolean.Parse("TRUE");
				case JSON.TOKEN_FALSE:
					NextToken(json, ref index);
					return Boolean.Parse("FALSE");
				case JSON.TOKEN_NULL:
					NextToken(json, ref index);
					return null;
				case JSON.TOKEN_NONE:
					break;
			}
 
			success = false;
			return null;
		}
 
		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
			char c;
 
			EatWhitespace(json, ref index);
 
			// "
			c = json[index++];
 
			bool complete = false;
			while (!complete) {
 
				if (index == json.Length) {
					break;
				}
 
				c = json[index++];
				if (c == '"') {
					complete = true;
					break;
				} else if (c == '\\') {
 
					if (index == json.Length) {
						break;
					}
					c = json[index++];
					if (c == '"') {
						s.Append('"');
					} else if (c == '\\') {
						s.Append('\\');
					} else if (c == '/') {
						s.Append('/');
					} else if (c == 'b') {
						s.Append('\b');
					} else if (c == 'f') {
						s.Append('\f');
					} else if (c == 'n') {
						s.Append('\n');
					} else if (c == 'r') {
						s.Append('\r');
					} else if (c == 't') {
						s.Append('\t');
					} else if (c == 'u') {
						int remainingLength = json.Length - index;
						if (remainingLength >= 4) {
							// fetch the next 4 chars
							char[] unicodeCharArray = new char[4];
							Array.Copy(json, index, unicodeCharArray, 0, 4);
							// parse the 32 bit hex into an integer codepoint
							uint codePoint = UInt32.Parse(new string(unicodeCharArray), NumberStyles.HexNumber);
							// convert the integer codepoint to a unicode char and add to string
							s.Append(Char.ConvertFromUtf32((int)codePoint));
							// skip 4 chars
							index += 4;
						} else {
							break;
						}
					}
 
				} else {
					s.Append(c);
				}
 
			}
 
			if (!complete) {
				success = false;
				return null;
			}
 
			return s.ToString();
		}
 
		protected static double ParseNumber(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);
 
			int lastIndex = GetLastIndexOfNumber(json, index);
			int charLength = (lastIndex - index) + 1;
			char[] numberCharArray = new char[charLength];
 
			Array.Copy(json, index, numberCharArray, 0, charLength);
			index = lastIndex + 1;
			return Double.Parse(new string(numberCharArray), CultureInfo.InvariantCulture);
		}
 
		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			int lastIndex;
 
			for (lastIndex = index; lastIndex < json.Length; lastIndex++) {
				if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}
 
		protected static void EatWhitespace(char[] json, ref int index)
		{
			for (; index < json.Length; index++) {
				if (" \t\n\r".IndexOf(json[index]) == -1) {
					break;
				}
			}
		}
 
		protected static int LookAhead(char[] json, int index)
		{
			int saveIndex = index;
			return NextToken(json, ref saveIndex);
		}
 
		protected static int NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);
 
			if (index == json.Length) {
				return JSON.TOKEN_NONE;
			}
 
			char c = json[index];
			index++;
			switch (c) {
				case '{':
					return JSON.TOKEN_CURLY_OPEN;
				case '}':
					return JSON.TOKEN_CURLY_CLOSE;
				case '[':
					return JSON.TOKEN_SQUARED_OPEN;
				case ']':
					return JSON.TOKEN_SQUARED_CLOSE;
				case ',':
					return JSON.TOKEN_COMMA;
				case '"':
					return JSON.TOKEN_STRING;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
					return JSON.TOKEN_NUMBER;
				case ':':
					return JSON.TOKEN_COLON;
			}
			index--;
 
			int remainingLength = json.Length - index;
 
			// false
			if (remainingLength >= 5) {
				if (json[index] == 'f' &&
					json[index + 1] == 'a' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 's' &&
					json[index + 4] == 'e') {
					index += 5;
					return JSON.TOKEN_FALSE;
				}
			}
 
			// true
			if (remainingLength >= 4) {
				if (json[index] == 't' &&
					json[index + 1] == 'r' &&
					json[index + 2] == 'u' &&
					json[index + 3] == 'e') {
					index += 4;
					return JSON.TOKEN_TRUE;
				}
			}
 
			// null
			if (remainingLength >= 4) {
				if (json[index] == 'n' &&
					json[index + 1] == 'u' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 'l') {
					index += 4;
					return JSON.TOKEN_NULL;
				}
			}
 
			return JSON.TOKEN_NONE;
		}
 
		protected static bool SerializeValue(object value, StringBuilder builder)
		{
			bool success = true;
 
			if (value is string) {
				success = SerializeString((string)value, builder);
			} else if (value is Hashtable) {
				success = SerializeObject((Hashtable)value, builder);
			} else if (value is ArrayList) {
				success = SerializeArray((ArrayList)value, builder);
			} else if (IsNumeric(value)) {
				success = SerializeNumber(Convert.ToDouble(value), builder);
			} else if ((value is Boolean) && ((Boolean)value == true)) {
				builder.Append("true");
			} else if ((value is Boolean) && ((Boolean)value == false)) {
				builder.Append("false");
			} else if (value == null) {
				builder.Append("null");
			} else {
				success = false;
			}
			return success;
		}
 
		protected static bool SerializeObject(Hashtable anObject, StringBuilder builder)
		{
			builder.Append("{");
 
			IDictionaryEnumerator e = anObject.GetEnumerator();
			bool first = true;
			while (e.MoveNext()) {
				string key = e.Key.ToString();
				object value = e.Value;
 
				if (!first) {
					builder.Append(", ");
				}
 
				SerializeString(key, builder);
				builder.Append(":");
				if (!SerializeValue(value, builder)) {
					return false;
				}
 
				first = false;
			}
 
			builder.Append("}");
			return true;
		}
 
		protected static bool SerializeArray(ArrayList anArray, StringBuilder builder)
		{
			builder.Append("[");
 
			bool first = true;
			for (int i = 0; i < anArray.Count; i++) {
				object value = anArray[i];
 
				if (!first) {
					builder.Append(", ");
				}
 
				if (!SerializeValue(value, builder)) {
					return false;
				}
 
				first = false;
			}
 
			builder.Append("]");
			return true;
		}
 
		protected static bool SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");
 
			char[] charArray = aString.ToCharArray();
			for (int i = 0; i < charArray.Length; i++) {
				char c = charArray[i];
				if (c == '"') {
					builder.Append("\\\"");
				} else if (c == '\\') {
					builder.Append("\\\\");
				} else if (c == '\b') {
					builder.Append("\\b");
				} else if (c == '\f') {
					builder.Append("\\f");
				} else if (c == '\n') {
					builder.Append("\\n");
				} else if (c == '\r') {
					builder.Append("\\r");
				} else if (c == '\t') {
					builder.Append("\\t");
				} else {
					int codepoint = Convert.ToInt32(c);
					if ((codepoint >= 32) && (codepoint <= 126)) {
						builder.Append(c);
					} else {
						builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
					}
				}
			}
 
			builder.Append("\"");
			return true;
		}
 
		protected static bool SerializeNumber(double number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
			return true;
		}
 
		/// <summary>
		/// Determines if a given object is numeric in any way
		/// (can be integer, double, null, etc). 
		/// 
		/// Thanks to mtighe for pointing out Double.TryParse to me.
		/// </summary>
		protected static bool IsNumeric(object o)
		{
			double result;
 
			return (o == null) ? false : Double.TryParse(o.ToString(), out result);
		}
	}

Serialise To Objects, base class This is untidied code, complete with debugging code left about. You'll need the attribute class in the next section 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
 
using UnityEngine;
 
public class JSONSerialised
{
	protected void D (string s)
	{
		//Debug.Log ("JSON: " + s);
	}
	protected void Parser (Hashtable ht)
	{
		Dictionary<string, PropertyInfo> pia = new Dictionary<string, PropertyInfo> ();
		Dictionary<string, JSONStructAttribute> jsa = new Dictionary<string, JSONStructAttribute> ();
 
		string c = this.GetType ().FullName;
 
		if( ht == null)
		{
			D(c+" ... passed null. abort.");
			return;
		}
 
		D ("Parser: ht size:" + ht.Count);
 
 
		D ("scanning properties... " + c + " / " + this.GetType ().GetProperties ().Length);
 
		foreach (PropertyInfo pi in this.GetType ().GetProperties ()) {
			string name = pi.Name;
			D ("####property: " + name);
			foreach (JSONStructAttribute sa in pi.GetCustomAttributes (typeof(JSONStructAttribute), true)) {
				D ("########struct attribute: " + sa.ToString());
				name = sa.Name;
				jsa.Add (name, sa);
			}
 
			pia.Add (name, pi);
		}
 
		D ("scanning hashtable...");
		foreach (string key in ht.Keys) {
			D ("####key:" + key);
			if (pia.ContainsKey (key)) {
				D ("########key found" + key);
				PropertyInfo pi = pia[key];
				MethodInfo mi = pi.GetSetMethod ();
				JSONStructAttribute ji = null;
				if (jsa.ContainsKey (key))
					ji = jsa[key];
 
				Type t = ht[key].GetType ();
				if (t == typeof(Hashtable)) {
					D ("############ struct");
					D ("############ struct count: "+ ((Hashtable)ht[key]).Count);
					//hashtable = new class, convention adds to property
					mi.Invoke (this, new object[] { ConvertCtor (ht[key], ji) });
				} else if (t == typeof(ArrayList)) {
 
					D ("############ array ");
					D ("############ array count "+ ((ArrayList)ht[key]).Count);
					mi = pi.GetGetMethod();
					object target = mi.Invoke(this,null);
					MethodInfo mAdd = pi.GetGetMethod().ReturnType.GetMethod ("Add");
					D("KEY:"+key);
					D("PI::"+pi.Name);
					D("PIGT:"+pi.GetGetMethod().ReturnType.Name);
					D("TGT::"+target.ToString());
					if(mAdd == null)
						throw new NotSupportedException("add method, not found.");
					int index = 0;
					foreach (object oo in (ArrayList)ht[key]) {
						object x = Convert ( oo, ji);
						D("applying "+key+" index#"+ index);
						D("adding "+x);
						D("to ... "+mAdd.ToString());
						mAdd.Invoke (target, new object[] { x });
						index++;
					}
				} else {
					D ("############ "+t.FullName);
					mi.Invoke (this, new object[] { Convert (ht[key], ji) });
 
				}
			}
		}
	}
 
	protected object Convert (object o, JSONStructAttribute ji)
	{
		D ("############ converting... "+o);
 
		Type t = o.GetType ();
		if (t == typeof(double))
			return o;
		if (t == typeof(string))
			return o;
		if (t == typeof(bool))
			return o;
		if(t == typeof(Hashtable))
			return ConvertCtor(o,ji);
 
		throw new NotSupportedException ("convert:" + o);
	}
 
	protected object ConvertCtor (object o, JSONStructAttribute ji)
	{
		D ("############ creating new object ");
		if(o == null)
			throw new NotSupportedException("passing null as object?");
		if(ji == null)
			throw new NotSupportedException("You forgot an attribute!");
		D ("############ creating new object "+ji.Target.FullName);
		ConstructorInfo ctor = ji.Target.GetConstructor (new Type[] { typeof(Hashtable) });
		if (ctor != null)
			return ctor.Invoke (new object[] { o });
		else
			throw new NotSupportedException ("ctor ");
	}
 
	protected string DDX(object o)
	{
		if(o == null)
			return "<null>\n";
		if (o.GetType () == typeof(bool))
			return DD ((bool)o);
		if (o.GetType () == typeof(double))
			return DD ((double)o);
		if (o.GetType () == typeof(string))
			return DD ((string)o);
		if (o.GetType () == typeof(ArrayList))
			return DD ((ArrayList)o);
		if (o.GetType () == typeof(Hashtable))
			return DD ((Hashtable)o);
 
		throw new NotSupportedException("eh? "+o);
	}
	protected string DD(Hashtable ht)
	{
		if(ht == null)
			return "<null>\n";
		string ret = "{ ";
		foreach(string key in ht.Keys)
		{
			ret+="\n\t["+key+"]="+DDX( ht[key]);
		}
		return ret+"\n}";
	}
 
	protected string DD(ArrayList al)
	{
		if (al == null)
			return "<null>\n";
		string ret = "[ ";
		int index =0;
		foreach (object o in al) {
			ret += "\n\t[" + index + "]=" + DDX (o);
			index++;
		}
		return ret + "\n]";
 
	}
 
	protected string DD(string s)
	{
		if(s==null)
			return "<null>\n";
		return "\""+s+"\"\n";
	}
 
	protected string DD(double d)
	{
		return d.ToString()+"\n";
	}
 
	protected string DD(bool b)
	{
		return b.ToString()+"\n";
	}
}JSON Attribute using System;
using System.Collections.Generic;
 
public class JSONStructAttribute : Attribute
{
	private string sName;
	private Type cTarget;
	public JSONStructAttribute (string structName) 
	{
		sName = structName;
	}
	public JSONStructAttribute(string structName, Type target) : this(structName)
	{
		cTarget = target;
	}
 
	public string Name { get { return sName; } }
 
	public Type Target { get {return cTarget;}}
 
	public override string ToString ()
	{
		return string.Format ("[JSONStructAttribute: Name={0}, Target={1}]", Name, Target);
	}	
}Example Terrible example code. YMMV 
Example Data Object public class Example : JSONSerialised
{
	List<SomeOtherClass> eList = new List<SomeOtherClass>();
	public Example (Hashtable ht)
	{
		Parser(ht);
	}
 
	public double A { get; set; }
 
	public string B { get; set; }
 
	[JSONStruct("C", typeof(SomeClass))]
	public SomeClass C { get; set; }
 
	public string D {get;set;}
 
	[JSONStruct("E", typeof(SomeOtherClass))]
	public List<SomeOtherClass> SomeList { get { return eList;} }
 
	public override string ToString ()
	{
		return string.Format ("[Example: A={0}, B={1}, C={2}, D={3}]", A, B, C, D);
	}
}Example Usage 		WWW www = new WWW (URL);
 
		yield return www;
 
		bool ok = false;
 
		Hashtable ht = (Hashtable)JSON.JsonDecode (www.text, ref ok);
 
		if(ok)
			return new Example(ht);
		else
			return null;
}
