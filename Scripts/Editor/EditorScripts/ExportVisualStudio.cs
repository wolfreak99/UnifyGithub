// Original url: http://wiki.unity3d.com/index.php/ExportVisualStudio
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/ExportVisualStudio.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Lasse Järvensivu a.k.a. Statement 
Contents [hide] 
1 What it does 
2 Installation 
3 Usage 
4 C# - ExportVisualStudio.cs 

What it doesCreates Visual Studio projects (2005, 2008, 2010 CTP) to allow Unity3D coders to benefit from intellisense. 
Automatic documentation download for missing documentation (all you need to do is extract the contents). 
Launches Visual Studio project for you. 
Includes .cs .js .boo .shader files out of the box and easy to mod! 
InstallationAdd this script file into a folder called "Editor", and allow for unity to compile it. 
Usage1) In Unity, select from the the menu "File/New Visual Studio Project/" the version of Visual Studio C# project you want to create. Please note that 2005 and 2008 versions are untested at the time of writing. 
2) In case Xml documentation is missing, this script will attempt to download it. To successfully install the documentation you need to extract the Xml files into the opened directory. 
3) If all went well, the script will offer to open the project for you. 
C# - ExportVisualStudio.cs// ExportVisualStudio.cs
 
// Created by Lasse Järvensivu (no email please, but I might be on IRC as 'Statement').
// Feel free to modify code, make money from it, or claim it is yours.
// I don't care. 'I hold no ownership to this document'. I hope you enjoy.
 
 
// What it does:
// * Creates Visual Studio projects (2005, 2008, 2010 CTP) to allow Unity3D coders to benefit from intellisense.
// * Automatic documentation download for intellisense inline documentation (all you need to do is extract the contents).
// * Launches Visual Studio project for you.
 
 
// Installaton:
// 1) Add this script file into a folder called "Editor", and allow for unity to compile it.
 
 
// Usage:
// 1) In Unity, select from the the menu "File/New Visual Studio Project/" the version of
//    Visual Studio C# project you want to create (Note that 2005 and 2008 versions are untested).
// 2) In case Xml documentation is missing, this script will attempt to download it. To successfully
//    install the documentation you need to extract the Xml files into the opened directory.
// 3) If all went well, the script will offer to open the project for you.
 
 
// Troubleshooting:
// If you want to silence the nagging compability warning, simply remove the first two lines of code in
//   ExportVisualStudio.GenerateVisualC2005 and ExportVisualStudio.GenerateVisualC2008.
//
// If downloading the documentation locks up unity, remove the call to DocumentationHelper.CheckAndDownload
//   in ExportVisualStudio.GenerateVisualC2005, ExportVisualStudio.GenerateVisualC2008 and
//   ExportVisualStudio.GenerateVisualC2010.
//
// Finally, don't be afraid to change the code to suit your needs. Start in ExportVisualStudio functions
//   and then work your way around the calls to figure out how the code works. It is not complex, it just
//   contains a lot of xml generation.
 
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
using AssemblyReferences = System.Collections.Generic.List<VisualStudioProject.AssemblyReference>;
using FileInfos = System.Collections.Generic.List<System.IO.FileInfo>;
 
public sealed class ExportVisualStudio
{
    /// <summary>
    /// Creates projectname.csproj and attempts to conform to Visual C# 2005 specificatons.
    /// </summary>
    [MenuItem("File/New Visual Studio Project/Visual C# 2005")]
    public static void GenerateVisualC2005()
    {
        if (!EditorUtility.DisplayDialog("Compability warning.", "*Version 2005 of Visual C# project has not been tested.\nIt might produce an invalid project file and possibly overwrite a valid one.\nAre you sure you want to continue?", "Yes", "No"))
            return;
 
        DocumentationHelper.CheckAndDownload();
 
        string toolsVersion = "2.0";
        string targetFrameworkVersion = "v2.0";
        string productVersion = VisualStudioProject.VS2005ProjectVersion;
 
        GenerateProject(toolsVersion, targetFrameworkVersion, productVersion, true);
    }
 
    /// <summary>
    /// Creates projectname.csproj and attempts to conform to Visual C# 2008 specificatons.
    /// </summary>
    [MenuItem("File/New Visual Studio Project/Visual C# 2008")]
    public static void GenerateVisualC2008()
    {
        if (!EditorUtility.DisplayDialog("Compability warning.", "*Version 2008 of Visual C# project has not been tested.\nIt might produce an invalid project file and possibly overwrite a valid one.\nAre you sure you want to continue?", "Yes", "No"))
            return;
 
        DocumentationHelper.CheckAndDownload();
 
        string toolsVersion = "3.5";
        string targetFrameworkVersion = "v3.5";
        string productVersion = VisualStudioProject.VS2008ProjectVersion;
 
        GenerateProject(toolsVersion, targetFrameworkVersion, productVersion);
    }
 
    /// <summary>
    /// Creates projectname.csproj and attempts to conform to Visual C# 2010 CTP Beta specificatons.
    /// </summary>
    [MenuItem("File/New Visual Studio Project/Visual C# 2010 (CTP)")]
    public static void GenerateVisualC2010()
    {
        DocumentationHelper.CheckAndDownload();
 
        string toolsVersion = "4.0";
        string targetFrameworkVersion = "v3.5";
        string productVersion = VisualStudioProject.VS2010ProjectVersion;
 
        GenerateProject(toolsVersion, targetFrameworkVersion, productVersion);
    }
 
    /// <summary>
    /// Creates projectname.csproj.
    /// </summary>
    /// <param name="toolsVersion">The toolset version</param>
    /// <param name="targetFrameworkVersion">The intended target framework version of .net</param>
    /// <param name="productVersion">The version of visual studio to support</param>
    private static void GenerateProject(string toolsVersion, string targetFrameworkVersion, string productVersion)
    {
        GenerateProject(toolsVersion, targetFrameworkVersion, productVersion, false);
    }
 
    /// <summary>
    /// Creates projectname.csproj.
    /// </summary>
    /// <param name="toolsVersion">The toolset version</param>
    /// <param name="targetFrameworkVersion">The intended target framework version of .net</param>
    /// <param name="productVersion">The version of visual studio to support</param>
    /// <param name="exportFor2005">If true, Import directive is omitted from the build. Useful when exporting for 2005.</param>
    private static void GenerateProject(string toolsVersion, string targetFrameworkVersion, string productVersion, bool exportFor2005)
    {
        VisualStudioProject project = new VisualStudioProject();
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets"));
        project.AssemblyName = VisualStudioProject.GetProjectName();
        project.Compiled.AddRange(directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories));
        project.Contents.AddRange(directoryInfo.GetFiles("*.shader", SearchOption.AllDirectories));
        project.Contents.AddRange(directoryInfo.GetFiles("*.js", SearchOption.AllDirectories));
        project.Contents.AddRange(directoryInfo.GetFiles("*.boo", SearchOption.AllDirectories));
        project.ProductVersion = productVersion;
        project.ToolsVersion = toolsVersion;
        project.TargetFrameworkVersion = targetFrameworkVersion;
        project.ProjectGuid = VisualStudioProject.GetProjectGuid();
        project.References = VisualStudioProject.GetCommonReferences();
        project.RootNamespace = VisualStudioProject.GetProjectName();
        project.SkipImport = exportFor2005;
        project.Build();
        project.Save();
 
        if (EditorUtility.DisplayDialog("Generation complete.", "Would you like to open the project?", "Yes", "No"))
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), VisualStudioProject.GetProjectName() + ".csproj");
            FileInfo info = new FileInfo(path);
            System.Diagnostics.Process.Start(info.FullName);
        }
    }
}
 
internal static class DocumentationHelper
{
    /// <summary>
    /// Checks for documentation files and attempts to download these if not found.
    /// </summary>
    internal static void CheckAndDownload()
    {
        FileInfo unityEngineDll = new FileInfo(VisualStudioProject.GetAssemblyPath(typeof(UnityEngine.Object)));
        FileInfo unityEditorDll = new FileInfo(VisualStudioProject.GetAssemblyPath(typeof(UnityEditor.Editor)));
        FileInfo unityEngineXml = new FileInfo(Path.Combine(unityEngineDll.Directory.ToString(), "UnityEngine.xml"));
        FileInfo unityEditorXml = new FileInfo(Path.Combine(unityEditorDll.Directory.ToString(), "UnityEditor.xml"));
 
        if ((!unityEngineXml.Exists || !unityEditorXml.Exists))
        {
            string title = "Documentation missing.";
            string message = "Unity3D documentation not found.\nWould you attempt to download this now?\n(Documentation provided by unifycommuinty.com)";
            bool download = EditorUtility.DisplayDialog(title, message, "Download", "Skip");
            if (download)
            {
                DownloadDocumentationTo(unityEngineXml.Directory.FullName);
            }
        }
    }
 
    /// <summary>
    /// Downloads documentation to folder.
    /// </summary>
    /// <param name="destination">The folder where to place documentation zip to.</param>
    private static void DownloadDocumentationTo(string destination)
    {
        string url = @"http://www.unifycommunity.com/wiki/images/d/d2/Visual_Studio_docs_2.5.zip";
        string file = Path.Combine(destination, "Visual_Studio_docs_2.5.zip");
 
        try
        {
            WWW www = new WWW(url);
            while (!www.isDone)
            {
                EditorUtility.DisplayProgressBar("Downloading documentation...", "Downloading documentation for Visual Studio...", www.progress);
            }
            EditorUtility.DisplayProgressBar("Downloading documentation...", "Saving file...", 0.5f);
            File.WriteAllBytes(file, www.bytes);
            EditorUtility.ClearProgressBar();
            System.Diagnostics.Process.Start(destination);
            bool open = EditorUtility.DisplayDialog("Documentation downloaded.", "Would you like to open the zipped archive?", "Yes", "No");
            if (open)
            {
                System.Diagnostics.Process.Start(file);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(string.Format("File download error ({0})", e.Message));
        }
    }
}
 
/// <summary>
/// Handles building a visual studio project.
/// </summary>
internal sealed class VisualStudioProject
{
    public const string VS2005ProjectVersion = "8.0.50727";
    public const string VS2008ProjectVersion = "9.0.21022";
    public const string VS2010ProjectVersion = "10.0.20506"; // This is from the public beta build.
 
    /// <summary>
    /// See GetProjectGuid()
    /// </summary>
    public System.Guid ProjectGuid = System.Guid.Empty;
 
    /// <summary>
    /// The version of Visual Studio this project is targetted for
    /// </summary>
    public string ProductVersion = VS2010ProjectVersion;
 
    /// <summary>
    /// The projects root namespace (used when creating new files through visual studio)
    /// </summary>
    public string RootNamespace = string.Empty;
 
    /// <summary>
    /// The projects assembly name. Commonly the project name. See GetProjectName()
    /// </summary>
    public string AssemblyName = string.Empty;
 
    /// <summary>
    /// Some sort of version identifier for the tools in visual studio.
    /// </summary>
    public string ToolsVersion = "4.0";
 
    /// <summary>
    /// The target .net framework version.
    /// </summary>
    public string TargetFrameworkVersion = "v3.5";
 
    /// <summary>
    /// Relative path to the project where debug files are emitted during build events.
    /// </summary>
    public string DebugOutput = "Visual Studio/Debug/";
 
    /// <summary>
    /// Relative path to the project where release files are emitted during build events.
    /// </summary>
    public string ReleaseOutput = "Visual Studio/Release/";
 
    /// <summary>
    /// Mainly used for Visual Studio 2005 support.
    /// </summary>
    public bool SkipImport = false;
 
    /// <summary>
    /// List of references in the project
    /// </summary>
    public AssemblyReferences References = new AssemblyReferences();
 
    /// <summary>
    /// All content that is not to compiled and included in the project
    /// </summary>
    public FileInfos Compiled = new FileInfos();
 
    /// <summary>
    /// All content that is not to be compiled, but included in the project
    /// </summary>
    public FileInfos Contents = new FileInfos();
 
    /// <summary>
    /// Is set on Build() and contains the XmlDocument which makes up this project.
    /// </summary>
    public XmlDocument GeneratedDocument;
 
    /// <summary>
    /// References to Assemblies
    /// </summary>
    internal sealed class AssemblyReference
    {
        public string Name;
        public string HintPath;
 
        public AssemblyReference(string name)
            : this(name, string.Empty)
        {
        }
 
        public AssemblyReference(string name, string hintPath)
        {
            Name = name;
            HintPath = hintPath;
        }
    }
 
 
    /// <summary>
    /// Builds the project as defined through the member settings.
    /// Access GeneratedDocument if you need to process contents easily through Xml.
    /// </summary>
    public void Build()
    {
        GeneratedDocument = new XmlDocument();
        XmlDeclaration declaration = GeneratedDocument.CreateXmlDeclaration("1.0", "utf-8", null);
        GeneratedDocument.AppendChild(declaration);
        BuildProject(GeneratedDocument);
    }
 
    /// <summary>
    /// Saves the project to yourprojectname.csproj.
    /// </summary>
    public void Save()
    {
        if (GeneratedDocument != null)
            GeneratedDocument.Save(Path.Combine(Directory.GetCurrentDirectory(), GetProjectName() + ".csproj"));
        else
            Debug.LogError("Must build document before Save");
    }
 
 
    #region Helper functions
    /// <summary>
    /// Returns a list of assembly references commonly accessed, such as System.Core, System.Xml and the assemblies exposed by Unity3D.
    /// </summary>
    /// <returns>A list of common assembly references.</returns>
    public static AssemblyReferences GetCommonReferences()
    {
        AssemblyReferences references = new AssemblyReferences();
 
        AssemblyReference system = new AssemblyReference("System");
        references.Add(system);
        AssemblyReference systemCore = new AssemblyReference("System.Core");
        references.Add(systemCore);
        AssemblyReference systemXml = new AssemblyReference("System.XML");
        references.Add(systemXml);
        AssemblyReference unityEngine = new AssemblyReference("UnityEngine", GetAssemblyPath(typeof(UnityEngine.Object)));
        references.Add(unityEngine);
        AssemblyReference unityEditor = new AssemblyReference("UnityEditor", GetAssemblyPath(typeof(UnityEditor.Editor)));
        references.Add(unityEditor);
 
        return references;
    }
 
    /// <summary>
    /// Returns the path where the assembly which holds type exists.
    /// </summary>
    /// <param name="type">The type to discover its associated assemblys path.</param>
    /// <returns>The associated assemblys path.</returns>
    public static string GetAssemblyPath(Type type)
    {
        return Assembly.GetAssembly(type).Location;
    }
 
    /// <summary>
    /// Creates a deterministic Guid based on a string.
    /// </summary>
    /// <param name="input">The string used as input to generate a Guid.</param>
    /// <returns>The generated Guid based on input.</returns>
    public static System.Guid StringGuid(string input)
    {
        MD5 md5 = MD5.Create();
        Encoding encoding = Encoding.Default;
        byte[] bytes = encoding.GetBytes(input);
        byte[] hash = md5.ComputeHash(bytes);
        System.Guid guid = new System.Guid(hash);
        return guid;
    }
 
    /// <summary>
    /// Returns the project name based on the current directorys name (in lack of any API function).
    /// </summary>
    /// <returns>The current project name.</returns>
    public static string GetProjectName()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        string projectName = directoryInfo.Name;
        return projectName;
    }
 
    /// <summary>
    /// Creates a deterministic guid based on the MD5 hash of the current project name.
    /// </summary>
    /// <returns>A guid for this project name.</returns>
    public static System.Guid GetProjectGuid()
    {
        return StringGuid(GetProjectName());
    }
    #endregion
 
    #region Helper functions to build the project in steps
    private void BuildProject(XmlDocument document)
    {
        XmlElement project = document.CreateElement("Project");
        project.SetAttribute("ToolsVersion", ToolsVersion);
        project.SetAttribute("DefaultTargets", "Build");
        project.SetAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
 
        BuildProperties(project, document);
        BuildConfigurationDebug(project, document);
        BuildConfigurationRelease(project, document);
        BuildReferences(project, document);
        BuildFiles(project, document);
 
        // Visual Studio 2005 security fix.
        if (!SkipImport)
        {
            BuildImport(project, document);
        }
 
        document.AppendChild(project);
    }
 
    private void BuildProperties(XmlElement project, XmlDocument document)
    {
        XmlElement propertyGroup = document.CreateElement("PropertyGroup");
 
        XmlElement configuration = document.CreateElement("Configuration");
        configuration.SetAttribute("Condition", " '$(Configuration)' == '' ");
        configuration.InnerText = "Debug";
        propertyGroup.AppendChild(configuration);
 
        XmlElement platform = document.CreateElement("Platform");
        platform.SetAttribute("Condition", " '$(Platform)' == '' ");
        platform.InnerText = "AnyCPU";
        propertyGroup.AppendChild(platform);
 
        XmlElement productVersion = document.CreateElement("ProductVersion");
        productVersion.InnerText = ProductVersion;
        propertyGroup.AppendChild(productVersion);
 
        XmlElement schemaVersion = document.CreateElement("SchemaVersion");
        schemaVersion.InnerText = "2.0";
        propertyGroup.AppendChild(schemaVersion);
 
        XmlElement projectGuid = document.CreateElement("ProjectGuid");
        projectGuid.InnerText = ProjectGuid.ToString();
        propertyGroup.AppendChild(projectGuid);
 
        XmlElement outputType = document.CreateElement("OutputType");
        outputType.InnerText = "Library";
        propertyGroup.AppendChild(outputType);
 
        XmlElement appDesignerFolder = document.CreateElement("AppDesignerFolder");
        appDesignerFolder.InnerText = "Properties";
        propertyGroup.AppendChild(appDesignerFolder);
 
        XmlElement rootNamespace = document.CreateElement("RootNamespace");
        rootNamespace.InnerText = RootNamespace;
        propertyGroup.AppendChild(rootNamespace);
 
        XmlElement assemblyName = document.CreateElement("AssemblyName");
        assemblyName.InnerText = AssemblyName;
        propertyGroup.AppendChild(assemblyName);
 
        XmlElement targetFrameworkVersion = document.CreateElement("TargetFrameworkVersion");
        targetFrameworkVersion.InnerText = TargetFrameworkVersion;
        propertyGroup.AppendChild(targetFrameworkVersion);
 
        project.AppendChild(propertyGroup);
    }
 
    private void BuildConfigurationDebug(XmlElement project, XmlDocument document)
    {
        XmlElement propertyGroup = document.CreateElement("PropertyGroup");
        propertyGroup.SetAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ");
 
        XmlElement debugSymbols = document.CreateElement("DebugSymbols");
        debugSymbols.InnerText = "true";
        propertyGroup.AppendChild(debugSymbols);
 
        XmlElement debugType = document.CreateElement("DebugType");
        debugType.InnerText = "full";
        propertyGroup.AppendChild(debugType);
 
        XmlElement optimize = document.CreateElement("Optimize");
        optimize.InnerText = "false";
        propertyGroup.AppendChild(optimize);
 
        XmlElement outputPath = document.CreateElement("OutputPath");
        outputPath.InnerText = DebugOutput;
        propertyGroup.AppendChild(outputPath);
 
        XmlElement defineConstants = document.CreateElement("DefineConstants");
        defineConstants.InnerText = "DEBUG;TRACE";
        propertyGroup.AppendChild(defineConstants);
 
        XmlElement errorReport = document.CreateElement("ErrorReport");
        errorReport.InnerText = "prompt";
        propertyGroup.AppendChild(errorReport);
 
        XmlElement warningLevel = document.CreateElement("WarningLevel");
        warningLevel.InnerText = "4";
        propertyGroup.AppendChild(warningLevel);
 
        project.AppendChild(propertyGroup);
    }
 
    private void BuildConfigurationRelease(XmlElement project, XmlDocument document)
    {
        XmlElement propertyGroup = document.CreateElement("PropertyGroup");
        propertyGroup.SetAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ");
 
        XmlElement debugType = document.CreateElement("DebugType");
        debugType.InnerText = "pdbonly";
        propertyGroup.AppendChild(debugType);
 
        XmlElement optimize = document.CreateElement("Optimize");
        optimize.InnerText = "true";
        propertyGroup.AppendChild(optimize);
 
        XmlElement outputPath = document.CreateElement("OutputPath");
        outputPath.InnerText = ReleaseOutput;
        propertyGroup.AppendChild(outputPath);
 
        XmlElement defineConstants = document.CreateElement("DefineConstants");
        defineConstants.InnerText = "TRACE";
        propertyGroup.AppendChild(defineConstants);
 
        XmlElement errorReport = document.CreateElement("ErrorReport");
        errorReport.InnerText = "prompt";
        propertyGroup.AppendChild(errorReport);
 
        XmlElement warningLevel = document.CreateElement("WarningLevel");
        warningLevel.InnerText = "4";
        propertyGroup.AppendChild(warningLevel);
 
        project.AppendChild(propertyGroup);
    }
 
    private void BuildReferences(XmlElement project, XmlDocument document)
    {
        XmlElement itemGroup = document.CreateElement("ItemGroup");
 
        foreach (AssemblyReference assemblyReference in References)
        {
            XmlElement reference = document.CreateElement("Reference");
            reference.SetAttribute("Include", assemblyReference.Name);
            if (!string.IsNullOrEmpty(assemblyReference.HintPath))
            {
                XmlElement hintPath = document.CreateElement("HintPath");
                hintPath.InnerText = assemblyReference.HintPath;
                reference.AppendChild(hintPath);
            }
            itemGroup.AppendChild(reference);
        }
 
        project.AppendChild(itemGroup);
    }
 
    private void BuildFiles(XmlElement project, XmlDocument document)
    {
        XmlElement itemGroup = document.CreateElement("ItemGroup");
 
        foreach (FileInfo fileInfo in Compiled)
        {
            // For each compiled file
            XmlElement compile = document.CreateElement("Compile");
            compile.SetAttribute("Include", fileInfo.FullName);
            itemGroup.AppendChild(compile);
        }
 
        foreach (FileInfo fileInfo in Contents)
        {
            // For each .shader/.js/.boo
            XmlElement none = document.CreateElement("None");
            none.SetAttribute("Include", fileInfo.FullName);
            itemGroup.AppendChild(none);
        }
 
        project.AppendChild(itemGroup);
    }
 
    private void BuildImport(XmlElement project, XmlDocument document)
    {
        XmlElement import = document.CreateElement("Import");
        import.SetAttribute("Project", @"$(MSBuildToolsPath)/Microsoft.CSharp.targets");
        project.AppendChild(import);
    }
    #endregion
}--Statement 06:50, 24 October 2009 (PDT) 
}
