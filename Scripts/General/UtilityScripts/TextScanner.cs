// Original url: http://wiki.unity3d.com/index.php/TextScanner
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/TextScanner.cs
// File based on original modification date of: 10 January 2012, at 20:53. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Author: KeliHlodversson 
DescriptionThis is a reimplementation of the sscanf C standard library function for .Net, which can be very handy for parsing data in strings without needing to write your own parsers and suchlike. 
Usage    values = TextScanner.Scan(formatString, stringToParse);
        or
    scanner = new TextScanner(formatString);
    values = scanner.Scan(stringToParse);The format string is similar to the one accepted by the C standard library functions: 
    %% skips a % litteral
    %d matches any optionally signed decimal number
    %o matches any optionally signed octal number
    %x matches any optionally signed hexadecimal number
    %i matches any optionally signed integer. 
        If prefixed with 0, it's parsed in base 8 and 0x, base 16, else base 10
    %u is an unsigned integer.
    %f float
    %s word - matches up to the next whitespace
    %c a list of characters. If no size is specified only 1 character will be matched
    %n returns an int containing the number of characters consumed sofar does not 
        consume characters from the text string and ignores any flags except *.
    any other text is matched verbatim
    
    You can add extra options between the % sign and the format specifier:
    *    - the matching data will be ignored
    hh   - tiny size, the parsed data will be returned as a SByte (or Byte for %u)
    h    - half size, the parsed data will be returned as a Int16 (or UInt16 for %u, 
            System.Single for %f, the default)
    l    - long, the default for ints. Results in an Int32 (UInt32 for %u, 
            System.Double for %f)
    ll,L - long long. Results in an Int64 (UInt64 for %u, System.Decimal for %f)
You can also specify the maximum number of characters to match after the % sign, default is Infinite, except for %c, which is 1. Whitespace at the start of each pattern is skipped, except for %c. 
The Scan method will return a list of matched values. It will stop at the first pattern that fails to match and return any previously matched patterns. 
See the man page for sscanf(3) for more information on the original. 
TextScanner.csusing System.Text;
using System.Text.RegularExpressions;
using System.Collections;
 
public class TextScanner
{
 
    public static ArrayList Scan(string format, string data) {
        TextScanner scanner= new TextScanner(format);
        return scanner.Scan(data);
    }
 
 
    private delegate object ParserDelegate (Match m, PatternElement e);
    private struct PatternElement {     
        public Regex re; // should always be anchored with \G
        public int maxWidth; // 0 is same as infinite (or whatever the Regex does)
        public ParserDelegate parser;
        public bool skipWS;
        public bool ignoreMatch;
        public int numBase; 
        public int size; // size of converted type 4 huge, 3 large, 2 default, 1 less, 0 tiny. Ignored for s and c.
    }
 
    private ArrayList compiledPattern;
 
    private static Regex eatString = new Regex( 
             @"\G(%(?<width>\d*)(?<flag>[\*hlL]*)(?<specifier>[diouxXaAeEfFgGscn])|(?<verbatim>(%%|[^%])+)|(?<error>%[^diouxXaAeEfFgGscCn]))", 
             RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    public TextScanner (string format) {
 
        compiledPattern=new ArrayList();
        int location = 0;
        Match m = eatString.Match(format,location);
        while(m.Success) {
            location += m.Length;
 
            PatternElement e ;
            e.maxWidth=0;
            e.skipWS=true;
            e.re=null;
            e.parser=null;
            e.ignoreMatch=false;
            e.size=2;
            e.numBase=10;
 
            if(m.Groups["flag"].Success) {
                if (m.Groups["flag"].Value.IndexOf("*") >= 0)
                    e.ignoreMatch=true;
                if (m.Groups["flag"].Value.IndexOf("hh") >= 0)
                    e.size=0;
                else if (m.Groups["flag"].Value.IndexOf("h") >= 0)
                    e.size=1;
                else if (m.Groups["flag"].Value.IndexOf("ll") >= 0 || m.Groups["flags"].Value.IndexOf("L") >= 0 )
                    e.size=4;
                else if (m.Groups["flag"].Value.IndexOf("l") >= 0 )
                    e.size=3;
            }
 
 
            if(m.Groups["verbatim"].Success) {
                e.re=new Regex(string.Format(@"\G{0}",QuoteMeta(m.Groups["verbatim"].Value)));
                e.ignoreMatch=true;
            }
            else if (m.Groups["error"].Success) {
                throw new System.ArgumentException(string.Format("Unknown format specifier: {0}", m.Groups["error"].Value));
            }
            else {
 
                if(m.Groups["width"].Success && m.Groups["width"].Value != "") 
                    e.maxWidth=int.Parse(m.Groups["width"].Value);
 
                e.skipWS=true;
 
                switch (m.Groups["specifier"].Value) {
                    case "d":
                        e.re=res[0];
                        e.parser=new ParserDelegate(ParseDOX);
                        break;
                    case "i":
                        e.re=res[1];
                        e.parser=new ParserDelegate(ParseI);
                        break;
                    case "o":
                        e.re=res[2];
                        e.parser=new ParserDelegate(ParseDOX);
                        e.numBase=8;
                        break;
                    case "u":
                        e.re=res[3];
                        e.parser=new ParserDelegate(ParseU);
                        break;
                    case "x":
                        e.re=res[4];
                        e.parser=new ParserDelegate(ParseDOX);
                        e.numBase=16;
                        break;
                    case "f":
                        e.re=res[5];
                        e.parser=new ParserDelegate(ParseF);
                        break;
                    case "s":
                        e.re=res[6];
                        e.parser=new ParserDelegate(ParseSC);
                        break;
                    case "c":
                        e.re=res[7];
                        e.parser=new ParserDelegate(ParseSC);
                        e.skipWS=false;
                        if(!m.Groups["width"].Success || m.Groups["width"].Value == "")
                            e.maxWidth=1;
                        break;
                    case "n":
                        e.re=null;
                        e.parser=null; // special case
                        e.skipWS=false;
                        break;
                    case "X":
                        goto case "x";
                    default:
                        goto case "f";
                }
            }
            compiledPattern.Add(e);
 
            m = eatString.Match(format,location);
        }
 
    }
 
    private static Regex[] res = {
        new Regex( @"\G([+\-]?\d+)" ), // d
        new Regex( @"\G((?<sign>[+\-])?(?:(?:0x(?<hex>[0-9a-f]+)|(?<oct>0[0-7]*)|(?<dec>[1-9]\d*))))" , RegexOptions.IgnoreCase), // i 
        new Regex( @"\G([+\-]?[0-7]+)"), // o
        new Regex( @"\G((?<sign>[+])?(?:(?:0x(?<hex>[0-9a-f]+)|(?<oct>0[0-7]*)|(?<dec>[1-9]\d*))))" , RegexOptions.IgnoreCase), // u
        new Regex( @"\G([+\-]?(?:0x)?[0-9a-f]+)", RegexOptions.IgnoreCase), // x
        new Regex( @"\G([+\-]?(?:\d*\.\d+|\d+)(?:e[+\-]?\d+)?)", RegexOptions.IgnoreCase), //  a, A, e, E, f, F, g, G  
        new Regex( @"\G(\w+)"), // s
        new Regex( @"\G(.+)"), // c
    };
 
    private object ParseSC(Match m, PatternElement e) {
        return m.Value;
    }
    private object ParseDOX(Match m, PatternElement e) {
        switch(e.size) {
            case 0:
                return System.Convert.ToSByte(m.Value,e.numBase);
            case 1:
                return System.Convert.ToInt16(m.Value,e.numBase);
            case 3:
                return System.Convert.ToInt32(m.Value,e.numBase);
            case 4:
                return System.Convert.ToInt64(m.Value,e.numBase);
            default:
                return System.Convert.ToInt32(m.Value,e.numBase);
        }
    }
    private object ParseI(Match m, PatternElement e) {
        string sign="";
        if( m.Groups["sign"].Success && m.Groups["sign"].Value == "-")
            sign="-";
        string val;
        int fromBase;
        if( m.Groups["hex"].Success ) {
            val = sign+m.Groups["hex"].Value;
            fromBase = 16;
        }
        else if( m.Groups["oct"].Success ){
            val = sign+m.Groups["oct"].Value;
            fromBase = 8;
        }
        else if( m.Groups["dec"].Success ) {
            val = sign+m.Groups["dec"].Value;
            fromBase = 10;
        }
        else
            throw new System.ArgumentException(string.Format("Could not convert '{0}' to an int", m.Value));
 
        switch(e.size) {
            case 0:
                return System.Convert.ToSByte(val,fromBase);
            case 1:
                return System.Convert.ToInt16(val,fromBase);
            case 3:
                return System.Convert.ToInt32(val,fromBase);
            case 4:
                return System.Convert.ToInt64(val,fromBase);
            default:
                return System.Convert.ToInt32(val,fromBase);
        }
    }
 
    private object ParseU(Match m, PatternElement e) {
        string val;
        int fromBase;
        if( m.Groups["hex"].Success ) {
            val = m.Groups["hex"].Value;
            fromBase = 16;
        }
        else if( m.Groups["oct"].Success ){
            val = m.Groups["oct"].Value;
            fromBase = 8;
        }
        else if( m.Groups["dec"].Success ) {
            val = m.Groups["dec"].Value;
            fromBase = 10;
        }
        else
            throw new System.ArgumentException(string.Format("Could not convert '{0}' to an unsigned int", m.Value));
 
        switch(e.size) {
            case 0:
                return System.Convert.ToByte(val,fromBase);
            case 1:
                return System.Convert.ToUInt16(val,fromBase);
            case 3:
                return System.Convert.ToUInt32(val,fromBase);
            case 4:
                return System.Convert.ToUInt64(val,fromBase);
            default:
                return System.Convert.ToUInt32(val,fromBase);
        }
    }
    private object ParseF(Match m, PatternElement e) {
        switch(e.size) {
            case 0:
                return float.Parse(m.Value);
            case 1:
                return float.Parse(m.Value);
            case 3:
                return double.Parse(m.Value);
            case 4:
                return System.Decimal.Parse(m.Value);
            default:
                return float.Parse(m.Value);
        }
    }
 
 
    private static Regex metaChars = new Regex( @"([\\\[\].\(\)\|\*\+]|%%)" ,RegexOptions.Compiled);
    private static string ReplaceMeta(Match m) {
        if(m.Value == "%%") return "%";
        else return "\\" + m.Value;
    }
    private static string QuoteMeta (string unquoted) {
        return metaChars.Replace(unquoted, new MatchEvaluator(ReplaceMeta));
    }
 
    private static Regex whitespace = new Regex( @"\G\s*" ,RegexOptions.Compiled);
 
    public ArrayList Scan(string data) {
        ArrayList res = new ArrayList ();
        int location=0;
        foreach (PatternElement e in compiledPattern) {
            Match m;
            if(e.skipWS) {
                m=whitespace.Match(data, location);
                location += m.Length;
            }
 
            if(e.re == null && !e.ignoreMatch) { // special case for %n
                res.Add(location); // %n stores the number of characters matched sofar.
                continue;
            }
 
            if(e.maxWidth > 0) 
                m=e.re.Match(data, location, e.maxWidth);
            else
                m=e.re.Match(data, location);
 
            if (!m.Success)
                return res; // If the regex doesn't match, just return what we've got sofar
            location += m.Length;
            if(! e.ignoreMatch && e.parser != null)
                res.Add(e.parser(m, e));
        }
 
        return res;
    }
 
 
}
}
