/*************************
 * Original url: http://wiki.unity3d.com/index.php/Windows_Saved_Game_Directory
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/Windows_Saved_Game_Directory.cs
 * File based on original modification date of: 4 April 2013, at 21:18. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.CodeSnippets
{
    If you want to store your saved games in the file system instead of using Unity's PlayerPrefs, there are some conventions for where saved games should be stored on the Windows platform, but figuring out where the appropriate directories are is not trivial. 
    On Windows XP, saved game files should go in C:\Users\Name\My Documents\My Games (examples: Borderlands, Skyrim, Torchlight). As far as I know this isn't an official recommendation, but it is established practice. 
    Windows Vista and later provide the special C:\Users\Name\Saved Games directory. Not many games are using this yet, probably due to tech support worries if the directory changes depending on the OS used. 
    Code The relevant method to call is getSaveGameDirectory() (Ctrl+F to jump to where it's used). 
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
     
    using UnityEngine;
     
    /// <summary>Example component that looks up the saved game folder</summary>
    public class SaveGameLocation : MonoBehaviour {
     
      /// <summary>Maximum buffer size most Win32 API functions expect</summary>
      private const int MAX_PATH = 260;
      /// <summary>Special folder id of the user's 'My Documents' folder</summary>
      private const int CSIDL_PERSONAL = 0x0005;
      /// <summary>COM ClassID of the KnownFolderManager proxy coclass</summary>
      private const string CLSID_KnownFolderManager = "4df0c730-df9d-4ae3-9153-aa6b82e9795a";
      /// <summary>COM InterfaceID of the IKnownFolder interface</summary>
      private const string IID_IKnownFolder = "3aa7af7e-9b36-420c-a8e3-f77d4674a488";
      /// <summary>COM InterfaceID of the IKnownFolderManager interface</summary>
      private const string IID_IKnownFolderManager = "8be2d872-86aa-4d47-b776-32cca40c7018";
      /// <summary>ID of the 'Saved Games' folder in the known folder manager</summary>
      private static readonly Guid FOLDERID_SavedGames = new Guid(
        0x4c5c32ff, 0xbb9d, 0x43b0, 0xb5, 0xb4, 0x2d, 0x72, 0xe5, 0x4e, 0xaa, 0xa4
      );
     
      /// <summary>Retrieves the path of a special folder, identified by its CSIDL</summary>
      /// <param name="ownerWindowHandle">Not used, always set to IntPtr.Zero</param>
      /// <param name="path">Received the drive and path o the specified folder</param>
      /// <param name="folderId">
      ///   CSIDL that identifies that folder whose path will be retrieved
      /// </param>
      /// <param name="createFlag">
      ///   Whether the folder should be created if it doesn't exist
      /// </param>
      /// <returns>Any nonzero value if successful, otherwise 0</returns>
      [DllImport("Shell32", CharSet = CharSet.Unicode)]
      private static extern int SHGetSpecialFolderPath(
        IntPtr ownerWindowHandle,
        StringBuilder path,
        int folderId,
        int createFlag
      );
     
      #region interface IKnownFolder
     
      /// <summary>Accesses informations about a known folder, including its path</summary>
      [
        ComImport,
        Guid(IID_IKnownFolder),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
      ]
      private interface IKnownFolder {
     
        /// <summary>GetId() is not provided</summary>
        void GetId_Stub();
     
        /// <summary>GetCategory() is not provided</summary>
        void GetCategory_Stub();
     
        /// <summary>GetShellItem() is not provided</summary>
        void GetShellItem_Stub();
     
        /// <summary>Retrieves the path of a known folder as a string</summary>
        /// <param name="flags">
        ///   Flags that specify special retrieval options. This value can be 0;
        ///   otherwise, one or more of the KNOWN_FOLDER_FLAG values
        /// </param>
        /// <param name="path">
        ///   Receives a pointer to a null-terminated buffer that contains the path
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPath([In] uint flags, [MarshalAs(UnmanagedType.LPWStr)] out string path);
     
        /// <summary>SetPath() is not provided</summary>
        void SetPath_Stub();
     
        /// <summary>GetLocation() is not provided</summary>
        void GetLocation_Stub();
     
        /// <summary>GetFolderType() is not provided</summary>
        void GetFolderType_Stub();
     
        /// <summary>GetRedirectionCapabilities() is not provided</summary>
        void GetRedirectionCapabilities_Stub();
     
        /// <summary>GetFolderDefinition() is not provided</summary>
        void GetFolderDefinition_Stub();
     
      }
     
      #endregion // interface IKnownFolder
     
      #region interface IKnownFolderManager
     
      /// <summary>Enumerates, creates and manages known folders</summary>
      [
        ComImport,
        Guid(IID_IKnownFolderManager),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
      ]
      private interface IKnownFolderManager {
     
        /// <summary>FolderIdFromCsidl() is not provided</summary>
        void FolderIdFromCsidl_Stub();
     
        /// <summary>FolderIdToCsidl() is not provided</summary>
        void FolderIdToCsidl_Stub();
     
        /// <summary>GetFolderIds() is not provided</summary>
        void GetFolderIds_Stub();
     
        /// <summary>Retrieves informations about a known folder by its id</summary>
        /// <param name="folderId">
        ///   Id of the known folder whose informations will be retrieved
        /// </param>
        /// <param name="knownFolder">Receives the known folder information instance</param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder(
          [In] ref Guid folderId,
          [MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder
        );
     
        /// <summary>GetFolderByName() is not provided</summary>
        void GetFolderByName_Stub();
     
        /// <summary>RegisterFolder() is not provided</summary>
        void RegisterFolder_Stub();
     
        /// <summary>UnregisterFolder() is not provided</summary>
        void UnregisterFolder_Stub();
     
        /// <summary>FindFolderFromPath() is not provided</summary>
        void FindFolderFromPath_Stub();
     
        /// <summary>FindFolderFromIDList() is not provided</summary>
        void FindFolderFromIDList_Stub();
     
        /// <summary>Redirect() is not provided</summary>
        void Redirect_Stub();
     
      }
     
      #endregion // interface IKnownFolderManager
     
      #region class KnownFolderManagerImpl
     
      /// <summary>KnownFolderManager proxy coclass</summary>
      [ComImport, Guid(CLSID_KnownFolderManager)]
      internal class KnownFolderManagerImpl { }
     
      #endregion // class KnownFolderManagerImpl
     
      /// <summary>Directory, on Windows, where saved games should be stored</summary>
      /// <remarks>
      ///   This public field is an example for quick testing and should be filled with
      ///   the saved game directory (eg. C:\Users\Me\Saved Games) if everything worked.
      /// </remarks>
      public string SaveGameDirectory;
     
      void Awake() {
        SaveGameDirectory = getSaveGameDirectory();
      }
     
      /// <summary>Looks up the preferred save game directory for the current OS</summary>
      /// <returns>The preferred save game directory</returns>
      private static string getSaveGameDirectory() {
        bool isVistaOrLater = isAtLeastWindowsVersion(6, 0);
        if(isVistaOrLater) {
          try {
            string savedGameFolder = getKnownFolder(FOLDERID_SavedGames);
            if(!string.IsNullOrEmpty(savedGameFolder)) {
              return savedGameFolder;
            }
          }
          catch(Exception) {
            // Something went wrong querying the folder, fall back to other means
          }
        }
     
        try {
          string myDocumentsFolder = getSpecialFolder(CSIDL_PERSONAL);
          if(!string.IsNullOrEmpty(myDocumentsFolder)) {
            string myGamesPath = Path.Combine(myDocumentsFolder, "My Games");
            if(!Directory.Exists(myGamesPath)) {
              Directory.CreateDirectory(myGamesPath);
            }
     
            return myGamesPath;
          }
        }
        catch(Exception) {
          // Something went wrong querying the folder, fall back to other means
        }
     
        // Add more fallbacks here...
     
        return null;
      }
     
      /// <summary>Retrieves the path of a special folder by its CSIDL</summary>
      /// <param name="folderId">
      ///   CSIDL of the special folder whose path will be retrieved
      /// </param>
      /// <returns>The path of the special folder with the specified CSIDL</returns>
      private static string getSpecialFolder(int folderId) {
        var path = new StringBuilder(MAX_PATH);
     
        int result = SHGetSpecialFolderPath(IntPtr.Zero, path, folderId, 1);
        if(result == 0) {
          throw new Win32Exception("Could not query special folder path");
        }
     
        return path.ToString();
      }
     
      /// <summary>Retrieves the path of a known folder</summary>
      /// <param name="knownFolderId">GUID of the known folderparam>
      /// <returns>The path of the known folder with the specified GUID</returns>
      private static string getKnownFolder(Guid knownFolderId) {
        var instance = new KnownFolderManagerImpl();
        if(instance == null) {
          throw new COMException("Could not create instance of known folder manager coclass");
        }
     
        IKnownFolderManager knownFolderManager = instance as IKnownFolderManager;
        if(knownFolderManager == null) {
          throw new COMException("Could not query known folder manager interface");
        }
     
        IKnownFolder knownFolder;
        knownFolderManager.GetFolder(ref knownFolderId, out knownFolder);
        if(knownFolder == null) {
          throw new COMException("Could not query known folder");
        }
     
        string path;
        knownFolder.GetPath(0, out path);
        return path;
      }
     
      /// <summary>
      ///   Determines if at least the specified operating system version is running
      /// </summary>
      /// <param name="major">Major version number of the required OS</param>
      /// <param name="minor">Minor version number of the required OS</param>
      /// <returns>True if at least the specified OS version is running</returns>
      private static bool isAtLeastWindowsVersion(int major, int minor) {
        Version osVersion = Environment.OSVersion.Version;
     
        return
          (osVersion.Major > major) ||
          ((osVersion.Major == major) && (osVersion.Minor >= minor));
      }
     
    }
}
