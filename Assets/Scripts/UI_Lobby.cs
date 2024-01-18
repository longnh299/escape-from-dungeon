using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Lobby : MonoBehaviour
{
    [SerializeField] TMP_InputField hostname;
    [SerializeField] GameObject room_template;
    List<GameObject> room_btns = new List<GameObject>();

    void Awake()
    {
        C_Data.Instance.UI_Lobby = this;

        Refesh();
    }
    
    public void Refesh()
    {
        C_Data.Instance.player.Refresh();
    }

    public void Show_Room(List<UI_Lobby.Room> rooms)
    {
        foreach(var i in room_btns)
        {
            Destroy(i);
        }

        GameObject g;

        room_btns = new List<GameObject>();

        foreach(var i in rooms)
        {
            g = Instantiate(room_template, room_template.transform.parent);

            g.GetComponent<Room_Button>().room = i;

            room_btns.Add(g);

            g.SetActive(true);
        }
    }

    public void Host()
    {
        C_Data.Instance.player.Host(hostname.text);
    }

    public void Quit()
    {
        Application.Quit();
    }

    [System.Serializable]
    public sealed class Room
    {
        public int ID;
        public string sv_name;
    }
}
