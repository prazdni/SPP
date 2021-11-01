using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    public void Init(string message)
    {
        _text.text = message;
    }
}
