// NOT_CODE
/*************************
 * Original url: http://wiki.unity3d.com/index.php/ClassTypeReference
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/ClassTypeReference.cs
 * File based on original modification date of: 20 November 2014, at 16:18. 
 *
 * A class which provides serializable references to System.Type of classes with an accompanying custom property
 * drawer which allows class selection from drop-down. This source code is licensed under the open source MIT license 
 * and is hosted in a Git repository on Bitbucket: https://bitbucket.org/rotorz/classtypereference-for-unity 
 * Whilst we have not encountered any platform specific issues yet, the source code in this repository might not 
 * necessarily work for all of Unity's platforms or build configurations. It would be greatly appreciated if people 
 * would report issues using the issue tracker. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

/*
// Usage Examples Type references can be made using the inspector simply by using ClassTypeReference: 
using UnityEngine;
using TypeReferences;
     
public class ExampleBehaviour : MonoBehaviour {
    public ClassTypeReference greetingLoggerType;
}

// You can apply one of two attributes to drastically reduce the number of types presented when using the drop-down field. 
using UnityEngine;
using TypeReferences;
     
public class ExampleBehaviour : MonoBehaviour {
    // Allow selection of classes that implement an interface.
    [ClassImplements(typeof(IGreetingLogger))]
    public ClassTypeReference greetingLoggerType;
     
    // Allow selection of classes that extend a specific class.
    [ClassExtends(typeof(MonoBehaviour))]
    public ClassTypeReference someBehaviourType;
}

// To create an instance at runtime you can use the System.Activator class from the .NET / Mono library: 
using System;
using UnityEngine;
using TypeReferences;
     
public class ExampleBehaviour : MonoBehaviour {
    [ClassImplements(typeof(IGreetingLogger))]
    public ClassTypeReference greetingLoggerType = typeof(DefaultGreetingLogger);
     
    private void Start() {
    	if (greetingLoggerType.Type == null) {
    		Debug.LogWarning("No type of greeting logger was specified.");
    	}
    	else {
    		var greetingLogger = Activator.CreateInstance(greetingLoggerType) as IGreetingLogger;
    		greetingLogger.LogGreeting();
    	}
    }
}
    
// Presentation of drop-down list can be customized by supplying a ClassGrouping value to either of the 
//      attributes ClassImplements or ClassExtends. 
// ClassGrouping.None - No grouping, just show type names in a list; 
//      for instance, "Some.Nested.Namespace.SpecialClass". 
// ClassGrouping.ByNamespace - Group classes by namespace and show foldout menus for nested namespaces; 
//      for instance, "Some > Nested > Namespace > SpecialClass". 
// ClassGrouping.ByNamespaceFlat (default) - Group classes by namespace; 
//      for instance, "Some.Nested.Namespace > SpecialClass". 
// ClassGrouping.ByAddComponentMenu - Group classes in the same way as Unity does for its component menu. 
//      This grouping method must only be used for MonoBehaviour types. 
// For instance, 
using UnityEngine;
using TypeReferences;
     
public class ExampleBehaviour : MonoBehaviour {
    [ClassImplements(typeof(IGreetingLogger), Grouping = ClassGrouping.ByAddComponentMenu)]
    public ClassTypeReference greetingLoggerType;
}
*/