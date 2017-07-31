// Original url: http://wiki.unity3d.com/index.php/ExpandoObject
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/ExpandoObject.cs
// File based on original modification date of: 10 January 2012, at 20:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Author: KeliHlodversson 
Contents [hide] 
1 Description 
2 Usage 
3 JavaScript - ExpandoObject.js 
4 Using ExpandoObject in other languages 
4.1 Boo 
4.2 C# 

Description I've written multiple times on the Unity forum that Unity's Javascript implementation does not support expando objects, i.e. you can not add fields to an object at runtime. The following is valid ECMAScript, but not possible in Unity: 
var a = new Object();
a.myField=22;This is not entirely correct. Unity's Javascript is based on Boo and when combining ducktyping with Boo's IQuackFu interface, one can in fact implement support for expando objects. 
Usage Add ExpandoObject.js to your project and you will be able to do the following: 
var a = new ExpandoObject();
a.myField=22;
 
// Or:
var template : Transform;
var things = new Array();
 
for(var i=0;i<10;i++) {
   things.push(new ExpandoObject());
   things[i].name = "Thing "+(i+1);
   things[i].score = 0;
   things[i].transform = Instantiate(template,transform.position + Vector3.up * 3, transform.rotation);
}
 
// And even:
class DerivedClass extends ExpandoObject {
  var staticallyDefined = 42;
  function AFunction() {
      // NOTE: you always have to qualify runtime fields inside methods by prepending "this." to them. 
     if(this.newField) {
        Debug.Log("I have a newField: "+this.newField);
     }
     else {
        Debug.Log(staticallyDefined);
     }
  }
}
 
var b = new DerivedClass();
b.AFunction() ; // prints "42"
b.newField="testing 123";
b.AFunction() ; // prints "I have a newField: testing 123"JavaScript - ExpandoObject.js import System.Reflection;
 
class ExpandoObject extends Boo.Lang.IQuackFu {
    private var _data : Hashtable;
    function QuackInvoke(name : String, args : Object[]) : Object {
        var t : System.Type = this.GetType();
        var at = new System.Type[args.Length];
        for (var i=0;i<args.Length;i++)
            at[i]=args[i].GetType();
 
        // First test if the method is defined statically
        var mi : MethodInfo = t.GetMethod(name, at);
        if(mi)
            return mi.Invoke(this, args);
        else {
            // If not -- throw an error. TODO: Support extending the object with closures.
            var s = "(";
            for (var j=0;j<at.Length;j++)
                s+=at[j]+((j<at.Length-1)?", ":"");
            s+=")";
 
            throw System.MissingMethodException("Method '"+name+s+"' not found in class "+this.GetType());
        }
    }
 
    function QuackGet( name : String, params : Object[] ) : Object {
        var t : System.Type = this.GetType();
        var fi : FieldInfo = t.GetField(name);
 
        // First test if the field is defined statically
        if (fi) {
            return fi.GetValue(this);
        }
        else {
            // If not, read the value from a local hash
            return _data[name];
         }
    }
 
    function QuackSet( name : String, params : Object[], value : Object ) : Object {
        var t : System.Type = this.GetType();
        var fi : FieldInfo = t.GetField(name);
 
        // First test if the field is defined statically
        if (fi) {
            fi.SetValue(this, value);
        }
        else {
            // If not, store the value in a local hash
            _data[name]=value;
        }
        return value;
    }
 
    // Constructor
    function ExpandoObject() {
        _data = new Hashtable();        
    }
 
}Using ExpandoObject in other languages Boo This will also work in Boo (just place the javascript inside Standard Assets to make it compile before the Boo script): 
expand_me as duck = ExpandoObject();
expand_me.expanded="QUACK!";C# C# Does not support ducktyping, so it will not see the added fields. You can however access the IQuackFu methods directly: 
ExpandoObject expand_me = new ExpandoObject();
expand_me.QuackSet("expanded","QUACK!");
Debug.Log(expand_me.QuackGet("expanded"));
}
