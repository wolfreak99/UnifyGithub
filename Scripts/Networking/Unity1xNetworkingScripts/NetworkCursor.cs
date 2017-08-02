/*************************
 * Original url: http://wiki.unity3d.com/index.php/NetworkCursor
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/Unity1xNetworkingScripts/NetworkCursor.cs
 * File based on original modification date of: 19 January 2012, at 19:03. 
 *
 * Author: (Joachim Ante) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Networking.Unity1xNetworkingScripts
{
    Contents [hide] 
    1 Description 
    2 Usage 
    3 JavaScript - NetworkCursor.js 
    4 JavaScript - SendNetworkCursor.js 
    5 C# - Plugins/Client.cs 
    6 C# - Plugins/Server.cs 
    7 C# - Plugins/MessageData.cs 
    
    DescriptionA set of scripts to make one Unity instance control another. This example takes mouse input one one Unity instance and sends it to control a cursor on another. This code could be developed further into a good networking implementation for many types of games. 
    UsageDownload this project: Networking Example Project 
    JavaScript - NetworkCursor.jsfunction Update () {
        while (true)
        {
            var msg : MessageData = Server.PopMessage();
            if (msg == null)
                break;
     
            transform.position.x = msg.mousex;
            transform.position.y = msg.mousey;
        }
    }JavaScript - SendNetworkCursor.jsfunction Update ()
    {
        // Create the message it and send it off!
        var msgData = new MessageData();
        msgData.mousex = Input.mousePosition.x / Screen.width;
        msgData.mousey = Input.mousePosition.y / Screen.height;
        msgData.stringData = "Hello World";
     
        Client.Send(msgData);
    }C# - Plugins/Client.csusing UnityEngine;
    using System.Collections;
    using System.Net.Sockets;
    using System.Net;
     
    public class Client : MonoBehaviour {
     
        public string m_IPAdress = "127.0.0.1";
        public const int kPort = 10253;
     
        private static Client singleton;
     
     
        private Socket m_Socket;
        void Awake ()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
     
            // System.Net.PHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
            // System.Net.IPAddress remoteIPAddress = ipHostInfo.AddressList[0];
            System.Net.IPAddress    remoteIPAddress  = System.Net.IPAddress.Parse(m_IPAdress);
     
            System.Net.IPEndPoint   remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, kPort);
     
            singleton = this;
            m_Socket.Connect(remoteEndPoint);
        }
     
        void OnApplicationQuit ()
        {
            m_Socket.Close();
            m_Socket = null;
        }
     
        static public void Send(MessageData msgData)
        {
            if (singleton.m_Socket == null)
                return;
     
            byte[] sendData = MessageData.ToByteArray(msgData);
            byte[] prefix = new byte[1];
            prefix[0] = (byte)sendData.Length;
            singleton.m_Socket.Send(prefix);
            singleton.m_Socket.Send(sendData);
        }
    }C# - Plugins/Server.csusing UnityEngine;
    using System.Collections;
    using System.Net.Sockets;
    using System.Net;
    using System.Text;
     
    public class Server : MonoBehaviour {
     
        static Server singleton;
     
        private Socket m_Socket;
     
        ArrayList m_Connections = new ArrayList ();
     
        ArrayList m_Buffer = new ArrayList ();
        ArrayList m_ByteBuffer = new ArrayList ();
     
        void Awake ()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);     
            IPEndPoint ipLocal = new IPEndPoint ( IPAddress.Any , Client.kPort);
     
            m_Socket.Bind( ipLocal );
     
            //start listening...
            m_Socket.Listen (100);
            singleton = this;
        }
     
        void OnApplicationQuit ()
        {
            Cleanup();
        }
     
        void Cleanup ()
        {
            if (m_Socket != null)
                m_Socket.Close();
            m_Socket = null;
     
            foreach (Socket con in m_Connections)
                con.Close();
            m_Connections.Clear();
        }   
        ~Server ()
        {
            Cleanup();      
        }
     
        void Update ()
        {
            // Accept any incoming connections!
            ArrayList listenList = new ArrayList();
            listenList.Add(m_Socket);
            Socket.Select(listenList, null, null, 1000);
            for( int i = 0; i < listenList.Count; i++ )
            {
                Socket newSocket = ((Socket)listenList[i]).Accept();
                m_Connections.Add(newSocket);
                m_ByteBuffer.Add(new ArrayList());
                Debug.Log("Did connect");
            }
     
            // Read data from the connections!
            if (m_Connections.Count != 0)
            {
                ArrayList connections = new ArrayList (m_Connections);
                Socket.Select(connections, null, null, 1000);
                // Go through all sockets that have data incoming!
                foreach (Socket socket in connections)
                {
                    byte[] receivedbytes = new byte[512];
     
                    ArrayList buffer = (ArrayList)m_ByteBuffer[m_Connections.IndexOf(socket)];
                    int read = socket.Receive(receivedbytes);
                    for (int i=0;i<read;i++)
                        buffer.Add(receivedbytes[i]);
     
                    while (true && buffer.Count > 0)
                    {
                        int length = (byte)buffer[0];
     
                        if (length < buffer.Count)
                        {
                            ArrayList thismsgBytes = new ArrayList(buffer);
                            thismsgBytes.RemoveRange(length + 1, thismsgBytes.Count - (length + 1));
                            thismsgBytes.RemoveRange(0, 1);
                            if (thismsgBytes.Count != length)
                                Debug.Log("Bug");
     
                            buffer.RemoveRange(0, length + 1);
                            byte[] readbytes = (byte[])thismsgBytes.ToArray(typeof(byte));
     
                            MessageData readMsg = MessageData.FromByteArray(readbytes);
                            m_Buffer.Add(readMsg);
     
                            //Debug.Log(System.String.Format("Message {0}: {1}, {2}", readMsg.stringData, readMsg.mousex, readMsg.mousey));
     
                            if (singleton != this)
                                Debug.Log("Bug");   
                        }
                        else
                            break;
                    }
     
                    // string output = Encoding.UTF8.GetString(bytes);
                }           
            }
        }
     
        static public MessageData PopMessage ()
        {
            if (singleton.m_Buffer.Count == 0)
            {
                return null;
            }
            else
            {
                MessageData readMsg = (MessageData)singleton.m_Buffer[0];
                singleton.m_Buffer.RemoveAt(0);
                // Debug.Log(System.String.Format("Message {0}: {1}, {2}", readMsg.stringData, readMsg.mousex, readMsg.mousey));
                return readMsg;
            }
        }
    }
    
    C# - Plugins/MessageData.csusing UnityEngine;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
     
    [System.Serializable]
    public class MessageData {
     
        public string stringData = "";
        public float  mousex = 0;
        public float  mousey = 0;
        public int    type = 0;
     
        public static MessageData FromByteArray(byte[] input)
        {
            // Create a memory stream, and serialize.
            MemoryStream stream = new MemoryStream(input);
            // Create a binary formatter.
            BinaryFormatter formatter = new BinaryFormatter();
     
            MessageData data = new MessageData();
            data.stringData = (string)formatter.Deserialize(stream);
            data.mousex = (float)formatter.Deserialize(stream);
            data.mousey = (float)formatter.Deserialize(stream);
            data.type = (int)formatter.Deserialize(stream);
     
            return data;
        }
     
        public static byte[] ToByteArray (MessageData msg)
        {
            // Create a memory stream, and serialize.
            MemoryStream stream = new MemoryStream();
            // Create a binary formatter.
            BinaryFormatter formatter = new BinaryFormatter();
     
            // Serialize.
            formatter.Serialize(stream, msg.stringData);
            formatter.Serialize(stream, msg.mousex);
            formatter.Serialize(stream, msg.mousey);
            formatter.Serialize(stream, msg.type);
     
            // Now return the array.
            return stream.ToArray();
        }
}
}
