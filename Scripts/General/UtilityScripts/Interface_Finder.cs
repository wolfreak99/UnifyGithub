/*************************
 * Original url: http://wiki.unity3d.com/index.php/Interface_Finder
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/Interface_Finder.cs
 * File based on original modification date of: 15 January 2015, at 14:13. 
 *
 * Author: Ilya Suzdalnitski 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    Object.FindObjectsOfType() is great, but it works only if a concrete type (inherited from Component) is given. What if we want to find objects that implement some interface? 
    It's possible to achieve the desired functionality using reflection. The solution makes a lookup table of types at initialization, and is almost as fast as Unity's FindObjectsOfType() method. 
    Usage example     var stuff = InterfaceHelper.FindObject<IIrresistible>();
        var component = transform.GetInterfaceComponent<IIrresistible>();
    
    Class itself using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Component = UnityEngine.Component;
    using Object = UnityEngine.Object;
     
        public static class InterfaceHelper
        {
            private static Dictionary<Type, List<Type>> _interfaceToComponentMapping;
            private static Type[] _allTypes;
     
            static InterfaceHelper()
            {
                InitInterfaceToComponentMapping();
            }
     
            private static void InitInterfaceToComponentMapping()
            {
                _interfaceToComponentMapping = new Dictionary<Type, List<Type>>();
     
                _allTypes = GetAllTypes();
     
                foreach (var curInterface in _allTypes)
                {
                    //We're interested only in interfaces
                    if (!curInterface.IsInterface)
                        continue;
     
                    var typeName = curInterface.ToString().ToLower();
     
                    //Skip system interfaces
                    if ( typeName.Contains( "unity" ) || typeName.Contains( "system." )
                         || typeName.Contains( "mono." ) || typeName.Contains( "mono." ) || typeName.Contains( "icsharpcode." )
                         || typeName.Contains( "nsubstitute" ) || typeName.Contains( "nunit." ) || typeName.Contains( "microsoft." )
                         || typeName.Contains( "boo." ) || typeName.Contains( "serializ" ) || typeName.Contains( "json" )
                         || typeName.Contains( "log." ) || typeName.Contains( "logging" ) || typeName.Contains( "test" )
                         || typeName.Contains( "editor" ) || typeName.Contains( "debug" ) )
                        continue;
     
                    var typesInherited = GetTypesInheritedFromInterface( curInterface );
     
                    if ( typesInherited.Count <= 0 )
                        continue;
     
                    var componentsList = new List<Type>();
     
                    foreach (var curType in typesInherited)
                    {
                        //Skip interfaces
                        if (curType.IsInterface)
                            continue;
     
                        //Ignore non-component classes
                        if ( !( typeof(Component) == curType || curType.IsSubclassOf( typeof(Component) ) ) )
                            continue;
     
                        if ( !componentsList.Contains( curType ) )
                            componentsList.Add( curType );
                    }
     
                    _interfaceToComponentMapping.Add( curInterface, componentsList );
                }
            }
     
            private static Type[] GetAllTypes()
            {
                var res = new List<Type>();
                foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
                {
                    res.AddRange( assembly.GetTypes() );
                }
     
                return res.ToArray();
            }
     
            private static IEnumerable<Type> GetTypesInheritedFromInterface<T>() where T : class
            {
                return GetTypesInheritedFromInterface( typeof(T) );
            }
     
            private static IList<Type> GetTypesInheritedFromInterface( Type type )
            {
                //Caching
                if ( null == _allTypes )
                {
                    _allTypes = GetAllTypes();
                }
     
                var res = new List<Type>();
     
                foreach (var curType in _allTypes)
                {
                    if ( !( type.IsAssignableFrom( curType ) && curType.IsSubclassOf( typeof( Component ) ) ) )
                        continue;
     
                    res.Add( curType );
     
                }
     
                return res;
            }
     
            public static IList<T> FindObjects<T>(bool firstOnly = false) where T : class
            {
                var resList = new List<T>();
     
                var types = _interfaceToComponentMapping[ typeof(T) ];
     
                if ( null == types || types.Count <= 0 )
                {
                    Debug.LogError( "No descendants found for type " + typeof(T) );
                    return null;
                }
     
                foreach (var curType in types)
                {
                    Object[] objects = firstOnly ?
                        new[] { Object.FindObjectOfType( curType ) }
                        : Object.FindObjectsOfType( curType );
     
                    if ( null == objects || objects.Length <= 0 )
                        continue;
     
                    var tList = new List<T>();
     
                    foreach ( var curObj in objects )
                    {
                        var curObjAsT = curObj as T;
     
                        if ( null == curObjAsT )
                        {
                            Debug.LogError( "Unable to cast '" + curObj.GetType() + "' to '" + typeof( T ) + "'" );
                            continue;
                        }
     
                        tList.Add( curObjAsT );
                    }
     
                    resList.AddRange( tList );
                }
     
                return resList;
            }
     
            public static T FindObject<T>() where T : class
            {
                var list = FindObjects<T>();
     
                return list[0];
            }
     
            public static IList<T> GetInterfaceComponents<T>( this Component component, bool firstOnly = false ) where T : class
            {
                var types = _interfaceToComponentMapping[typeof( T )];
     
                if ( null == types || types.Count <= 0 )
                {
                    Debug.LogError( "No descendants found for type " + typeof( T ) );
                    return null;
                }
     
                var resList = new List<T>();
     
                foreach ( var curType in types )
                {
                    //Optimization - don't get all objects if we need only one
                    Component[] components = firstOnly ?
                        new []{ component.GetComponent( curType ) }
                        : component.GetComponents( curType );
     
                    if (null == components || components.Length <=0)
                        continue;
     
                    var tList = new List<T>();
     
                    foreach (var curComp in components)
                    {
                        var curCompAsT = curComp as T;
     
                        if ( null == curCompAsT )
                        {
                            Debug.LogError( "Unable to cast '" + curComp.GetType() + "' to '" + typeof (T) + "'" );
                            continue;
                        }
     
                        tList.Add( curCompAsT );
                    }
     
                    resList.AddRange( tList );
                }
     
                return resList;
            }
     
            public static T GetInterfaceComponent<T>( this Component component ) where T : class
            {
                var list = GetInterfaceComponents<T>( component, true );
     
                return list[0];
            }
    }
}
