// Original url: http://wiki.unity3d.com/index.php/MixupGO
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/MixupGO.cs
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
3 JavaScript - MixupGO.js 
4 Using MixupGO in other languages 
4.1 Boo 
4.2 C# 

Description This class combines ducktyping with Boo's IQuackFu interface, to wrap GameObjects into a dynamic object that automatically forwards all method calls to the first Component attached to it that implements the method. 
Usage Add MixupGO.js to your project and you will be able to do the following: 
var me : Object;
me = new MixupGO(gameObject);
 
//...
 
me.position=Vector3(0,0,0) ; // same as transform.position=...
 
me.material.color=Color.red; // Uses the firs material property found inside components
me.MeshRenderer.material.color=Color.red; // Explicitly specify which component to use in case of ambiguity
 
me.child_object.position+=Vector3.forward; // Asuming a child object named "child_object"
me.AddForce(0,0,1); // same as rigidbody.AddForce(0,0,1)JavaScript - MixupGO.js //
// MixupGO - a wraper around GameObject that merges the methods of all member components
// as if they were defined in GameObject itself
//
 
import System.Reflection;
 
class MixupGO extends Boo.Lang.IQuackFu  {
    private var me : GameObject;
    private var t : System.Type;
 
    function QuackInvoke(name : String, args : Object[]) : Object {
        var at = new System.Type[args.Length];
        for (var i=0;i<args.Length;i++)
            at[i]=args[i].GetType();
 
        // First test if the method is defined statically on the GameObject
        var mi : MethodInfo = t.GetMethod(name, at);
        if(mi)
            return mi.Invoke(me, args);
 
        // Then loop through all components an invoke the first one found
        for (var c in me.GetComponents(Component) ){
            var lt=c.GetType();
            mi = lt.GetMethod(name, at);
            if(mi)
                return mi.Invoke(c, args);
        }
 
        // If not -- throw an error. 
        var s = "(";
        for (var j=0;j<at.Length;j++)
            s+=at[j]+((j<at.Length-1)?", ":"");
        s+=")";
 
        throw System.MissingMethodException("Method '"+name+s+"' not found on "+me.name);
 
    }
 
    function QuackGet(name : String) : Object {
 
        // First test if the field is defined statically
        var pi : PropertyInfo = t.GetProperty(name);
        if (pi) 
            return pi.GetValue(me,null);
        var fi : FieldInfo = t.GetField(name);
        if (fi) 
            return fi.GetValue(me);
 
        // special case for parent property -- return a MixupGO-wrapped gameObject instead of a Transform
        if(name == "parent")
            return new MixupGO(me.transform.parent.gameObject);
 
        // Then loop through all components an invoke the first one found
        for (var c in me.GetComponents(Component) ){
            var lt=c.GetType();
            pi = lt.GetProperty(name);
            if (pi) 
                return pi.GetValue(c,null);
            fi = lt.GetField(name);
            if(fi)
                return fi.GetValue(c);
 
        }
        // Then test if name is an attached component
        var c : Component = me.GetComponent(name);
        if( c )
            return c;
 
        // finally loop through all children an return the first one with the same name
        for(var child : Transform in me.transform) {
            if(child.name == name) 
                return new MixupGO(child.gameObject);
        }
 
        throw System.MissingMethodException("Property '"+name+"' not found on "+me.name);
 
    }
 
    function QuackSet(name : String, value : Object) : Object {
        //var vt : System.Type = value.GetType();
 
        // First test if the field is defined statically
        var pi : PropertyInfo = t.GetProperty(name);
        if (pi) {
            pi.SetValue(me,value,null);
            return value;
        }
        var fi : FieldInfo = t.GetField(name);
        if (fi) {
            fi.SetValue(me,value);
            return value;
        }
        // Then loop through all components an invoke the first one found
        for (var c in me.GetComponents(Component) ){
            var lt=c.GetType();
            pi = lt.GetProperty(name);
            if (pi) {
                pi.SetValue(c,value,null);
                return value;
            }
            fi = lt.GetField(name);
            if(fi) {
                fi.SetValue(c,value);
                return value;
            }
 
        }
 
        throw System.MissingMethodException("Property '"+name+"' not found on "+me.name);
    }
 
    // Constructor
    function MixupGO(go : GameObject) {
        me=go;
        t=me.GetType();
    }
 
}Using MixupGO in other languages Boo This will also work in Boo (just place the javascript inside Standard Assets to make it compile before the Boo script): 
wrapped as duck = MixupGO(gameObject);
wrapped.position += Vector3.left;C# C# Does not support ducktyping, so it will not see the added fields and methods. 
}
