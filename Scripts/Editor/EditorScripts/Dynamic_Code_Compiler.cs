/*************************
 * Original url: http://wiki.unity3d.com/index.php/Dynamic_Code_Compiler
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/Dynamic_Code_Compiler.cs
 * File based on original modification date of: 19 November 2013, at 21:13. 
 *
 * Author: Benjamin Schaaf (Benjamin Schaaf) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Description Allows you to compile a C# script right to a dll within the editor. It uses the CodeDomProvider from .NET to compile a C# source. No other languages are supported up to now until we get access to Unity's other compilers. 
    Usage To compile a C# script it (and only it) needs to be selected in the project folder. Then you can either go to Compiler -> Compile Selected or you can press Shift - U and the script will be automatically compiled and placed into Assets/Plugins folder. 
    If the file already exists, it will be overridden. 
    If the folder doesn't exist it will be created. 
    Any compiler errors will be put in the console log as errors. 
    For the compiler to work, you must have the .NET version set to 2.0 not 2.0 Subset. 
    DynamicCodeCompiler.js #pragma strict
    #pragma downcast
     
    import System;
    import System.IO;
    import System.Reflection;
    import System.CodeDom.Compiler;
     
    static class DynamicCodeCompiler {
        //Adds a menu item with a shortcut
        @MenuItem ("Compiler/Compile Selected #C")
        function CompileSelected():void {
            //only compile Script objects
            if (Selection.activeObject && Selection.activeObject.GetType().IsAssignableFrom(MonoScript)) {
                var target:MonoScript = Selection.activeObject as MonoScript;
     
                //reference directory separator
                var sep:String = Path.DirectorySeparatorChar.ToString();
                var path:String = Application.dataPath + sep + "Plugins" + sep + 
                    Path.GetFileName(target.name) + ".dll";
     
                GenerateDll(target.text, path);
            }
            else {
                Debug.Log("Selected object is not compilable");
            }
        }
     
        function GenerateDll(source:String, destination:String):void {
            var results:CompilerResults = CreateAssemblyFromSource("C#", source);
     
            //debug compiler errors
            if (results.Errors.HasErrors) {
                for (var error:CompilerError in results.Errors) {
                    Debug.LogError("Dynamic Code Compiler ERROR: " + error);
                }
                Debug.Log("Could not generate dll");
            }
            else {
                //Create Plugins directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
     
                //Move dll to Plugins folder, delete duplicates
                if (File.Exists(destination))
                    File.Delete(destination);
                File.Move(results.PathToAssembly, destination);
     
                //notify of completion and import new dll
                Debug.Log("Success! Code was compiled and moved to: " + destination);
                AssetDatabase.Refresh();
            }
        }
     
        function CreateAssemblyFromSource(type:String, source:String):CompilerResults {
            //Create default provider parameters
            var parameters:CompilerParameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = "temp_build.dll";
     
            //reference ALL assemblies!!
            for (var assembly:Assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                parameters.ReferencedAssemblies.Add(assembly.Location);
            }
     
            //compile and return results
            return CodeDomProvider.CreateProvider(type).CompileAssemblyFromSource(parameters, source);
        }
}
}
