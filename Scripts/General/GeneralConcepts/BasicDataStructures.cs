// Original url: http://wiki.unity3d.com/index.php/BasicDataStructures
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/GeneralConcepts/BasicDataStructures.cs
// File based on original modification date of: 19 October 2009, at 19:26. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
When using JavaScript, the basic data structures do not behave as you expect. There are other data structures you can use, but they have syntactic nuances. You can read the MSDN page about these data structures but the basics are collected here. 
Contents [hide] 
1 Static Typing 
1.1 Variable Creation/Assignment 
2 Data Structures 
2.1 Hash Tables 
2.2 ArrayLists 

Static TypingUnity uses Javascript's data typing features to help it figure out the types of the variables you use. You are often not required to use the static typing features because the Unity engine will try to figure out what type you mean. When you use dynamic typing, sometimes the error messages are less clear than when you use static typing - so if you get an incomprehensible error message, try adding type definitions to your variables, especially loop variables - eg for (var someVar: GameObject in anArray). 
variable creation function definition 
var someVar: VariableType; function fooBar(variable: VariableType) 
Variable Creation/AssignmentDynamic
someVar = "foo"; 
var someVar = "foo"; 
Static
var someVar: VariableType; 
var someVar: VariableType = "foo"; 
The two dynamic examples are more-or-less identical in browsers. However, Unity always interprets someVar = "foo"; as being assignment to an already existing variable, and var someVar = "foo"; as creating a variable in the current scope. This can mean that you're accidentally creating someVar if you use the var prefix when you mean to assign to an existing variable. Inspector Tip: Creating "blank" variables in the global namespace (outside of any function) using the var someVar: VariableType; syntax is necessary for it to show up in the Inspector. 
Data StructuresHash TablesHash tables are an important exception to all the suggestions above. You cannot initialize an empty hash-table using var aHash: Hash; . You must use var aHash = {}; to initialize a hash table. 
Significant functions/methods of the hash table: 
Count 
An integer number of elements in the hash table. 
Keys 
A collection (list-like data structure) of the keys in the hash table. 
Contains(aKey) 
Is aKey a key in the hash table? 
Remove(aKey) 
Removes a key/value pair. 
Add(aKey, aValue) 
Equivalent to aHash[aKey] = aValue; 
ArrayListsYou might be tempted to use new Array(); for your arrays. While this is understandable, the Array implementation has some significant limits, notably not supporting the indexOf method. The ArrayList has many of the convenience methods one might want. Warnings: 
You cannot create a null ArrayList using var someArray: ArrayList; and subsequently access that ArrayList's members without first creating it. Workaround: in your Awake function, do var someArray = new ArrayList(); 
You cannot access the ArrayList constructor - var someArray = new ArrayList(foo, bar, baz); doesn't work. Workaround: You must create the ArrayList and subsequently populate it. 

Significant functions/methods of the array list: 
Count 
An integer number of elements in the array. Use this instead of length. e.g. for (i=0; i < anArrayList.Count; i++) { ... } 
Contains(anObject) 
Is aMember an element in the hash table? e.g. if (anArrayList.Contains(anObject)) { ... }. 
Remove(aMember) 
Removes aMember. Does not raise an error if aMember doesn't exist in the array. 
Add(anObject) 
Equivalent to anArray[] = aMember; except that anArray.Add(anObject) works. 
}
