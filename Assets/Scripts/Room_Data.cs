using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room_Data
{
    private Room_Data(){}
    static readonly Room_Data _ = new Room_Data();
    public static Room_Data Instance => _;

    public List<Room> list_room = new List<Room>();
    public List<bool> map = new List<bool>();

    public int Map()
    {
        int i = 0;
        bool exist = false;

        while(i < map.Count())
        {
            if(!map[i])
            {
                exist = true;

                map[i] = true;

                break;
            }

            i ++;
        }

        if(!exist)
        {
            map.Add(true);
        }

        return i * 200;
    }

    public List<UI_Lobby.Room> Refresh()
    {
        List<UI_Lobby.Room> lr = new List<UI_Lobby.Room>();

        foreach(var i in list_room)
        {
            if(i.crr_player == 2) continue;

            lr.Add(new UI_Lobby.Room(){ID = i.ID,sv_name = i.sv_name});
        }

        return lr;
    }

    public void Exit_Room(int id)
    {
        Room room = list_room.FirstOrDefault(n => n.ID == id);

        if(room == null) return;

        room.crr_player --;

        if(room.crr_player == 0) list_room.Remove(room);
    }

    public Room Join_Room(int id, ref int option)
    {
        Room room = list_room.FirstOrDefault(n => n.ID == id);

        if(room == null) // Room doesn't exist
        {
            option = 0;

            return room;
        }

        if(room.crr_player >= 2)// Room is full
        {
            option = 1;
        }
        else
        {
            option = 2;

            room.crr_player ++;
        }

        return room;
    }

    public Room Host(string svname)
    {
        Room room = new Room(){ID = Random.Range(100000,999999), map = Map(), crr_player = 1, sv_name = svname};

        list_room.Add(room);

        return room;
    }

    [System.Serializable]
    public sealed class Room
    {
        public int ID;
        public int map;
        public int crr_player;
        public string sv_name;
        public DungeonBuilder.DataMapRandom mapdata = new DungeonBuilder.DataMapRandom();
    }
}