using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room_Button : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ID;
    [HideInInspector] public UI_Lobby.Room room;

    void OnEnable()
    {
        Show();
    }

    public void Show()
    {
        ID.text = room.sv_name;
    }

    public void Join()
    {
        C_Data.Instance.player.Join(room.ID);
    }
}
