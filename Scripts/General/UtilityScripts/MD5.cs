/*************************
 * Original url: http://wiki.unity3d.com/index.php/MD5
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/MD5.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * Author: Matthew Wegner 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    Contents [hide] 
    1 Overview 
    1.1 C# 
    1.2 JavaScript 
    2 The server side 
    2.1 Perl 
    2.2 PHP 
    2.3 Python 
    2.4 Ruby 
    2.5 Shell 
    
    OverviewThis C# code snippet generates an MD5 hash for an input string. The formatting will match the output of PHP's md5() function. 
    C#Best placed in your static-only utility class. 
    public  string Md5Sum(string strToEncrypt)
    {
    	System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
    	byte[] bytes = ue.GetBytes(strToEncrypt);
     
    	// encrypt bytes
    	System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    	byte[] hashBytes = md5.ComputeHash(bytes);
     
    	// Convert the encrypted bytes back to a string (base 16)
    	string hashString = "";
     
    	for (int i = 0; i < hashBytes.Length; i++)
    	{
    		hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
    	}
     
    	return hashString.PadLeft(32, '0');
    }JavaScript... and just in case anyone was wondering. This is also possible using JavaScript. 
    To use, name your Javascript file something like "md5functions.js". In your code, access the function using "hash = md5functions.Md5Sum("string");", where the prefix of the function matches the name of the .js file you created for it. 
    #pragma strict
     
    static function Md5Sum(strToEncrypt: String)
    {
    	var encoding = System.Text.UTF8Encoding();
    	var bytes = encoding.GetBytes(strToEncrypt);
     
    	// encrypt bytes
    	var md5 = System.Security.Cryptography.MD5CryptoServiceProvider();
    	var hashBytes:byte[] = md5.ComputeHash(bytes);
     
    	// Convert the encrypted bytes back to a string (base 16)
    	var hashString = "";
     
    	for (var i = 0; i < hashBytes.Length; i++)
    	{
    		hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, "0"[0]);
    	}
     
    	return hashString.PadLeft(32, "0"[0]);
    }You can use SHA1CryptoServiceProvider instead of MD5CryptoServiceProvider if you want to create SHA1 hashes instead of MD5 hashes. 
    The server sideAs noted above, the above unity snippets will return a hash matching the one returned from PHP's md5() function. In case you are using another language on the server side, here are some examples: 
    Perl<perl>use Digest::MD5 qw(md5_hex); 
    my $hashString = md5_hex($stringToHash); </perl> 
    PHP<perl> $hashString = md5($stringToHash); </perl> 
    Python<python>import hashlib 
    def md5Sum(inputString): 
      return hashlib.md5(inputString).hexdigest()
    </python> 
    Rubyrequire 'digest/md5' 
    def md5Sum(inputString) 
       Digest::MD5.hexdigest(inputString)
    end 
ShellRequires that you have the md5sum program installed on the server. <bash>HASH = `echo "$STRING_TO_HASH" | md5sum | cut -f 1 -d' '` </bash> 
}
