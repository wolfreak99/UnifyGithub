// Original url: http://wiki.unity3d.com/index.php/DeepCopy
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/DataStructureUtils/DeepCopy.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.DataStructureUtils
{
Deep Copy This function should serve most purposes for deep-copying an array. Note that it does some type coercion (specifically, builtin array to Unity array, and built-in hashtables to Boo.Lang.Hash). It also does not handle the unsigned integers, etc. This is merely for MOST applications. Do not rely on it for industrial use or whatever. It is written in jscript.NET. 
function DeepCopyHash(hash) : Hashtable {
  var retHash = new Hashtable();
  for(pair in hash) {
 
    var val = pair.Value;
    var key = pair.Key;
 
    var kind = (typeof val).ToString();
 
    if(kind == "UnityScript.Lang.Array") {
      retHash[key]=DeepCopyUnityArr(val);
    }
    if(kind.Substring(kind.length-2, 2) == "[]") {
      retHash[key]=DeepCopyUnityArr(new UnityScript.Lang.Array( val ));
    }
    else if(kind == "System.Collections.Hashtable" || kind == "Boo.Lang.Hash") {
      retHash[key]=DeepCopyHash(val);
    }
    else if(kind == "System.Boolean" || kind == "System.String" || kind == "System.Int16" || kind == "System.Int32" || kind == "System.Int64" || kind == "System.Single" || kind == "System.Double" || kind == "System.Decimal") {
      retHash[key]=val;
    }
    else if(kind == "Null") {
      retHash[key]=null;
    }
 
  }
  return retHash;
}
 
function DeepCopyUnityArr(arr) : Array {
  var retArr = new Array();
  for(var i=0;i<arr.length;i++) {
    var kind = (typeof arr[i]).ToString();
    if(kind == "UnityScript.Lang.Array") {
      retArr.push(DeepCopyUnityArr(arr[i]));
    }
    if(kind.Substring(kind.length-2, 2) == "[]") {
      retArr.push(DeepCopyUnityArr(new UnityScript.Lang.Array( arr[i] )));
    }
    else if(kind == "System.Collections.Hashtable" || kind == "Boo.Lang.Hash") {
      retArr.push(DeepCopyHash( arr[i] ));
    }
    else if(kind == "System.Boolean" || kind == "System.String" || kind == "System.Int16" || kind == "System.Int32" || kind == "System.Int64" || kind == "System.Single" || kind == "System.Double" || kind == "System.Decimal") {
      retArr.push(arr[i]);
    }
    else if(kind == "Null") {
      retArr.push(null);
    }
  }
  return retArr;
}
}
