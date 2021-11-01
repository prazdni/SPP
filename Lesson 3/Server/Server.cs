using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;

    private int port = 5805;

    private int hostID;
    private int reliableChannel;

    private bool isStarted = false;
    private byte error;

    List<int> connectionIDs = new List<int>();

    public void Start()
    {
        NetworkTransport.Init();
 
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
        hostID = NetworkTransport.AddHost(topology, port);

        isStarted = true;
        Debug.Log($"Server started");
    }

    public void ShutDownServer()
    {
        if (!isStarted) return;

        NetworkTransport.RemoveHost(hostID);
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    
    void Update()
    {
        if (!isStarted) return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    connectionIDs.Add(connectionId);
                    Debug.Log($"Player has been connected to server.");
                    break;

                case NetworkEventType.DataEvent:
                    RecieveMessage(recHostId, recBuffer, dataSize);
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log($"Player has been disconnected from server.");
                    break;

                case NetworkEventType.BroadcastEvent:

                    break;
            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    private void RecieveMessage(int recHostId, byte[] recBuffer, int dataSize)
    {
        using (MemoryStream serializedMessage = new MemoryStream(recBuffer))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            object msg = formatter.Deserialize(serializedMessage);
            if (msg is System.String stringMsg)
            {
                SendMessageToAll(recHostId, stringMsg);
            }

            if (msg is PlayerProfile playerProfile)
            {
                if (!LocalStorageContext.playersNames.TryGetValue(recHostId, out string name))
                {
                    LocalStorageContext.playersNames.Add(recHostId, playerProfile.name);
                }
                else if (name != playerProfile.name)
                {
                    LocalStorageContext.playersNames[recHostId] = playerProfile.name;
                }
            }
        }
    }

    public void SendMessageToAll(int recHostId, string message)
    {
        for (int i = 0; i < connectionIDs.Count; i++)
        {
            SendMessage(recHostId, message, connectionIDs[i]);
        }
    }

    public void SendMessage(int recHostId, string message, int connectionID)
    {
        if (LocalStorageContext.playersNames.TryGetValue(recHostId, out string name))
        {
            message = string.Concat($"{name}: ", message);
        }
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok) 
            Debug.LogError((NetworkError)error);
    }
}

