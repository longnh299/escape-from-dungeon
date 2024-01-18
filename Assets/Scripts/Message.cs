using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Message : NetworkBehaviour
{
    public GameObject messobject;
    public TextMeshProUGUI text;

    [TargetRpc]
    public void Target_Message(string mes)
    {
        Send_Message(mes);
    }

    public void Send_Message(string mes)
    {
        messobject.SetActive(true);

        text.text = mes;

        Invoke(nameof(Off_Mess), 2);
    }

    public void Off_Mess()
    {
        messobject.SetActive(false);
    }
}
