/*************************
 * Original url: http://wiki.unity3d.com/index.php/NetworkSerializationHelpers
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/Networking/NetworkSerializationHelpers.cs
 * File based on original modification date of: 6 November 2014, at 08:17. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Networking.Networking
{
    Contents [hide] 
    1 Description 
    1.1 SerializableNetworkViewID 
    1.2 SerializableNetworkPlayer 
    1.3 BinaryReader / BinaryWriter extension 
    1.4 NetworkSerializationHelpers class 
    2 Usage 
    2.1 IMPORTANT 
    2.2 C# example 
    3 NetworkSerializationHelpers.cs 
    
    Description This file contiains a set of helper methods and structs which allows you to serialize NetworkPlayer and NetworkViewID values. 
    SerializableNetworkViewID The SerializableNetworkViewID struct simply holds the same raw values as the NetworkViewID struct but is marked as "Serializable". That allows you to serialize NetworkViewIDs for example in a BinaryFormatter. The struct has two implicit casting operators which makes it easy to convert one to the other. 
    SerializableNetworkPlayer The SerializableNetworkPlayer struct does the same as the SerializableNetworkViewID struct but for serialiting NetworkPlayer values. A NetworkPlayer consists actually just of a single int value, however this class makes it easier to use the conversion methods. 
    BinaryReader / BinaryWriter extension Besides the two structs there are also extension methods for the BinaryReader and BinareWriter class to allow the serializing / deserializing of NetworkPlayer and NetworkViewID values. 
    NetworkSerializationHelpers class This class contains all the extension methods mentioned above and all the conversion helpers which use reflection to read and write the internal field(s) of NetworkPlayer and NetworkViewID values. 
    Usage All classes / structs are placed in a namespace called "NetworkSerialization". To use the extension methods or serializable versions of NetworkPlayer and NetworkViewID you have to add a using statement at the top (import for UnityScript) 
    
    
    IMPORTANT This class uses reflection to read and write the internal fields of Unity's NetworkPlayer and NetworkViewID struct. The internal naming of those fields or the way those values are handled can change at any time. So use at your own risk. 
    
    
    C# example     // [...]
        using NetworkSerialization;
        using System.Collections.Generic;
        using System.Runtime.Serialization.Formatters.Binary;
     
        // [...]
     
        [System.Serializable]
        public class Player
        {
            public SerializableNetworkPlayer player;
            public string name;
        }
     
        void SendPlayerUpdate(List<Player> aPlayers)
        {
            using (var m = new System.IO.MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(m,aPlayers);
                networkView.RPC("UpdatePlayers", RPCMode.Others, m.ToArray());
            }
        }
     
        [RPC]
        void UpdatePlayers(byte[] aData)
        {
            using (var m = new System.IO.MemoryStream(aData))
            {
                var formatter = new BinaryFormatter();
                List<Player> players = (List<Player>)formatter.Deserialize(m);
                // Do something with the players list
            }
        }
    
    NetworkSerializationHelpers.cs /* * * * * * * * * * * * *
     * Serialization support for Unity's NetworkPlayer and NetworkViewID values
     * -----------------------
     * 
     * IMPORTANT: This class uses reflection to read and write the internal fields of Unity's
     * NetworkPlayer and NetworkViewID struct. The internal naming of those fields or the way
     * those values are handled can change at any time. So use at your own risk.
     * 
     * Written by Bunny83
     * 2014-11-06
     * 
     * 
     * * * * * * * * * * * * */
    using UnityEngine;
    using System.Collections;
     
    namespace NetworkSerialization
    {
        [System.Serializable]
        public struct SerializableNetworkViewID
        {
            public int a;
            public int b;
            public int c;
            public SerializableNetworkViewID(int aA, int aB, int aC) { a = aA; b = aB; c = aC; }
            public SerializableNetworkViewID(NetworkViewID aViewID)
            {
                var tmp = NetworkSerializationHelpers.ToSerializableNetworkViewID(aViewID);
                a = tmp.a;
                b = tmp.b;
                c = tmp.c;
            }
            public static implicit operator NetworkViewID(SerializableNetworkViewID aID)
            {
                return NetworkSerializationHelpers.ToViewID(aID);
            }
            public static implicit operator SerializableNetworkViewID(NetworkViewID aViewID)
            {
                return NetworkSerializationHelpers.ToSerializableNetworkViewID(aViewID);
            }
        }
     
        [System.Serializable]
        public struct SerializableNetworkPlayer
        {
            public int index;
            public SerializableNetworkPlayer(int aIndexA) { index = aIndexA; }
            public SerializableNetworkPlayer(NetworkPlayer aPlayer)
            {
                var tmp = NetworkSerializationHelpers.ToSerializableNetworkPlayer(aPlayer);
                index = tmp.index;
            }
            public static implicit operator NetworkPlayer(SerializableNetworkPlayer aPlayer)
            {
                return NetworkSerializationHelpers.ToNetworkPlayer(aPlayer);
            }
            public static implicit operator SerializableNetworkPlayer(NetworkPlayer aPlayer)
            {
                return NetworkSerializationHelpers.ToSerializableNetworkPlayer(aPlayer);
            }
        }
     
        public static class NetworkSerializationHelpers
        {
            #region reflection magic
            private static System.Reflection.FieldInfo[] m_NetworkViewID;
            private static System.Reflection.FieldInfo m_NetworkPlayerIndex;
            static NetworkSerializationHelpers()
            {
                var t = typeof(NetworkViewID);
                m_NetworkViewID = new System.Reflection.FieldInfo[]
            {
                t.GetField("a", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic),
                t.GetField("b", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic),
                t.GetField("c", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            };
                t = typeof(NetworkPlayer);
                m_NetworkPlayerIndex = t.GetField("index", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }
            #endregion
            #region BinaryWriter / BinaryReader extension
            public static void Write(this System.IO.BinaryWriter aContext, NetworkViewID aViewID)
            {
                for (int i = 0; i < m_NetworkViewID.Length; i++)
                    aContext.Write((int)m_NetworkViewID[i].GetValue(aViewID));
            }
            public static NetworkViewID ReadNetworkViewID(this System.IO.BinaryReader aContext)
            {
                object id = new NetworkViewID();
                for (int i = 0; i < m_NetworkViewID.Length; i++)
                    m_NetworkViewID[i].SetValue(id, aContext.ReadInt32());
                return (NetworkViewID)id;
            }
            public static void Write(this System.IO.BinaryWriter aContext, NetworkPlayer aPlayer)
            {
                aContext.Write((int)m_NetworkPlayerIndex.GetValue(aPlayer));
            }
            public static NetworkPlayer ReadNetworkPlayer(this System.IO.BinaryReader aContext)
            {
                object player = new NetworkPlayer();
                m_NetworkPlayerIndex.SetValue(player, aContext.ReadInt32());
                return (NetworkPlayer)player;
            }
            #endregion
     
            #region Serializable classes helpers
            public static SerializableNetworkViewID ToSerializableNetworkViewID(NetworkViewID aViewID)
            {
                object o = aViewID;
                return new SerializableNetworkViewID(
                    (int)m_NetworkViewID[0].GetValue(o),
                    (int)m_NetworkViewID[1].GetValue(o),
                    (int)m_NetworkViewID[2].GetValue(o)
                );
            }
            public static NetworkViewID ToViewID(SerializableNetworkViewID aID)
            {
                object id = new NetworkViewID();
                m_NetworkViewID[0].SetValue(id, aID.a);
                m_NetworkViewID[1].SetValue(id, aID.b);
                m_NetworkViewID[2].SetValue(id, aID.c);
                return (NetworkViewID)id;
            }
     
            public static SerializableNetworkPlayer ToSerializableNetworkPlayer(NetworkPlayer aPlayer)
            {
                return new SerializableNetworkPlayer((int)m_NetworkPlayerIndex.GetValue(aPlayer));
            }
     
            public static NetworkPlayer ToNetworkPlayer(SerializableNetworkPlayer aPlayer)
            {
                object player = new NetworkPlayer();
                m_NetworkPlayerIndex.SetValue(player, aPlayer.index);
                return (NetworkPlayer)player;
            }
            #endregion
        }
    }
}
