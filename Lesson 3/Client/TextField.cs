using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextField : MonoBehaviour
{
    [SerializeField] private GameObject _textObjectTemplate;
    [SerializeField] private Transform _messageParent;


    public void ReceiveMessage(string message)
    {
        var gameObject = Instantiate(_textObjectTemplate, _messageParent);
        gameObject.GetComponent<MessageTemplate>().Init(message);
        gameObject.SetActive(true);
    }
}