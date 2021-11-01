using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Button buttonConnectClient;
    [SerializeField]
    private Button buttonDisconnectClient;
    [SerializeField]
    private Button buttonSendMessage;
    [SerializeField]
    private Button buttonSetName;

    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TMP_InputField inputNameField;


    [SerializeField]
    private TextField textField;

    [SerializeField]
    private Client client;
    private int sessionChangedNames;

    private void Start()
    {
        buttonConnectClient.onClick.AddListener(() => Connect());
        buttonDisconnectClient.onClick.AddListener(() => Disconnect());
        buttonSendMessage.onClick.AddListener(() => 
        {
            SendMessage(inputField.text);
            inputField.text = "";
        });
        buttonSetName.onClick.AddListener(() => 
        {
            sessionChangedNames++;
            SendMessage(new PlayerProfile{ name = inputNameField.text, sessionChangedNames = sessionChangedNames });
            inputNameField.text = "";
        });
        client.onMessageReceive += ReceiveMessage;
    }

    private void Connect()
    {
        client.Connect();
    }

    private void Disconnect()
    {
        client.Disconnect();
    }

    private void SendMessage(object msg)
    {
        client.SendMessage(msg);
    }

    public void ReceiveMessage(string message)
    {
        textField.ReceiveMessage(message);
    }
}