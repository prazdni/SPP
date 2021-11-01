﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class Client : MonoBehaviour
{
    public delegate void OnMessageReceive(string message);
    public event OnMessageReceive onMessageReceive;

    private const int MAX_CONNECTION = 10;

    private int port = 0;
    private int serverPort = 5805;

    private int hostID;

    private int reliableChannel;
    private int connectionID;

    private bool isConnected = false;
    private byte error;

    

    public void Connect()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable); 

        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);

        hostID = NetworkTransport.AddHost(topology, port);
        connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", serverPort, 0, out error);

        if ((NetworkError)error == NetworkError.Ok) 
            isConnected = true;
        else 
            Debug.Log((NetworkError)error);
    }

    public void Disconnect()
    {
        if (!isConnected) return;

        NetworkTransport.Disconnect(hostID, connectionID, out error); 
        isConnected = false;
    }

    void Update()
    {
        if (!isConnected) return;

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
                    onMessageReceive?.Invoke($"You have been connected to server.");
                    Debug.Log($"You have been connected to server.");
                    break;

                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    onMessageReceive?.Invoke(message);
                    Debug.Log(message);
                    break;

                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    onMessageReceive?.Invoke($"You have been disconnected from server.");
                    Debug.Log($"You have been disconnected from server.");
                    break;

                case NetworkEventType.BroadcastEvent:

                    break;
            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    public void SendMessage(object message)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            byte[] buffer = null;
            if(message != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, message);
                buffer = stream.ToArray();
            }
            NetworkTransport.Send(hostID, connectionID, reliableChannel, stream.ToArray(), 1024, out error);
            if ((NetworkError)error != NetworkError.Ok) 
                Debug.Log((NetworkError)error);
        }
    }
}

