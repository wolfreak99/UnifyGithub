// Original url: http://wiki.unity3d.com/index.php/MetadataSystem
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/Serialization/MetadataSystem.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.Serialization
{
Contents [hide] 
1 Features 
2 Get the Code 
3 Key use cases 
4 Usage 

Features A mechanism for storing preferences that compresses and (weakly) encrypts them, to make tampering more difficult. (NOTE: This is NOT an exceptionally strong mechanism, and will only make it harder to tamper with individual properties; the user can still simply delete the registry entries / .plist file to reset all the preferences at once...) 
A system for declaratively specifying properties that can be "patched" at runtime via data stored in the ciphered preferences, and which can be looked up by objects at runtime easily, even from early-lifecycle events such as Awake() or OnEnable(). 
Get the Code File:MetadataSystem.zip 


Key use cases Make it harder for players to tamper with their high scores. 
Make it easier to manage game balance by collecting balance-related parameters in one central place, facilitating easy review and manipulation. 
Make it easy to tweak game balance by downloading tweaks from a server and applying them to an installed copy of the game without having to download an actual patch to the game. 
Usage To use the ciphered preferences mechanism, use the static methods on CipheredPlayerPrefs, notably GetPref(key, default) which has variants for bool, int, and string and the corresponding SetPref(key, value). 
To use the metadata system, create a .js file under Plugins with a static hash variable like so: 
static var data = {
	"_patchset" : "MyGameProperties",
	"foo": "bar",
	"baz": ["a", "b", "c"],
	"bleah": {
		"foooo" : "Wheeeeee!",
		"baaaaaaaarrrrrrr" : "Whaaaaateeeeeeevvvvvveeeeerrrr!!!"
	}
};To access a property, call Properties.Get(classname.data, key) and cast the result to the correct type. (Where "classname" is whatever you called your .js file of course) The syntax for the key name is a simple dotted notation for accessing both hash keys and array subscripts. In the above example, valid keys are "foo", "baz.Count", "baz.0", "baz.1", "baz.2", "bleah.Keys", "bleah.foooo", and "bleah.baaaaaaaarrrrrrr". Note that the ".Keys" and ".Count" special properties are created automatically to allow iteration but they are NOT updated automatically when patching metadata from preferences. 
As you might have guessed, it's possible to have multiple such files. Also, it's very important that you not modify the data structure in the .js file at runtime directly -- the changes won't be recognized. 
The last key feature of this system is the ability to stuff values in the ciphered preferences that "patch" or "override" these properties. 
The "_patchset" key is important to allow overriding of these properties at runtime from the preferences. 
To override a key, set a preference in the ciphered preferences whose key matches the value in "_patchset" above, and whose value is a set of key=value pairs, one per line, using the same naming convention as is used to read properties via Properties.Get(). 
Note that it's not easy to grow/shrink an array, or add/remove keys from a hash via this patching mechanism, as it is not intended as a general-purpose data store. 
See the ShowProperties.cs and EditPatch.cs scripts for examples. 
}
