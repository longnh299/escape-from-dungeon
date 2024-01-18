using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetPlayer : NetworkBehaviour
{
    public DungeonBuilder.DataMapRandom mapdata = new DungeonBuilder.DataMapRandom();// Client var
    public Room_Data.Room data;
    public AllEnemySO allenemy;
    public bool isHost = false;

    public override void OnStartAuthority()
    {
        C_Data.Instance.player = this;

        if(C_Data.Instance.is_single)
        {
            Host(string.Empty);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    [ClientCallback]
    void Update()
    {
        if(!isOwned) return;

        if(Input.GetKeyDown(KeyCode.H))
        {
            NextLevel();
        }
    }
    
    public void SpawnBot(int i, Vector3 position)
    {
        if(isOwned)
        {
            Cmd_SpawnBot(i, position);
        }
    }

    [Command]
    void Cmd_SpawnBot(int i, Vector3 position)
    {
        // if(isHost)
        // {
        //     if(roomid != data.hostid)
        //     {
        //         data.hostid = roomid;
        //     }
        // }
        // else
        // {
        //     if(roomid != data.clientid)
        //     {
        //         data.clientid = roomid;
        //     }

        //     if(roomid == data.hostid) return;
        // }
        if(!isHost) return;

        GameObject enemy = Instantiate(allenemy.enemyList[i].enemyPrefab, position, Quaternion.identity);

        enemy.GetComponent<NetworkTeam>().ChangeID(data.ID.ToString());

        enemy.GetComponent<Enemy>().ChangeIndex(i);

        foreach (Collider collider in enemy.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        NetworkServer.Spawn(enemy, connectionToClient);
    }

    public void NextLevel()
    {
        if(isOwned) Cmd_NextLevel();
    }

    [Command]
    void Cmd_NextLevel()
    {
        Target_NextLevel();
    }

    [TargetRpc]
    void Target_NextLevel()
    {
        GameManager.Instance.NextLevel(false);
    }

    public void NextLevelNonAuth()
    {
        Cmd_NextLevelNonAuth();
    }

    [Command(requiresAuthority = false)]
    void Cmd_NextLevelNonAuth()
    {
        Target_NextLevelNonAuth(data.mapdata);
    }

    [TargetRpc]
    void Target_NextLevelNonAuth(DungeonBuilder.DataMapRandom dt)
    {
        mapdata = dt;

        DungeonBuilder.Instance.mapdata = dt;

        GameManager.Instance.NextLevel(true);
    }

    public void PushMap(DungeonBuilder.DataMapRandom dt)
    {
        if(isOwned)
        {
            Cmd_PushMap(dt);
        }
    }

    [Command]
    void Cmd_PushMap(DungeonBuilder.DataMapRandom dt)
    {
        data.mapdata = dt;

        Target_PushMap();
    }

    [TargetRpc]
    void Target_PushMap()
    {
        foreach(var i in FindObjectsOfType<NetPlayer>())
        {
            if(i == this) continue;
            else
            {
                i.NextLevelNonAuth();

                break;
            }
        }
    }

    public void Refresh()
    {
        if(isOwned)
        {
            Cmd_Refresh();
        }
    }

    [Command]
    void Cmd_Refresh()
    {
        Target_Refresh(Room_Data.Instance.Refresh());
    }

    [TargetRpc]
    void Target_Refresh(List<UI_Lobby.Room> rooms)
    {
        C_Data.Instance.UI_Lobby.Show_Room(rooms);
    }

    public void Host(string svname)
    {
        if(isOwned)
        {
            Cmd_Host(svname);
        }
    }

    [Command]
    void Cmd_Host(string svname)
    {
        data = Room_Data.Instance.Host(svname);

        isHost = true;

        Sv_StartGame();
    }

    public override void OnStopServer()
    {
        Room_Data.Instance.Exit_Room(data.ID);
    }

    public void Join(int id)
    {
        if(isOwned)
        {
            Cmd_Join(id);
        }
    }

    [Command]
    void Cmd_Join(int id)
    {
        int i = 0;

        data = Room_Data.Instance.Join_Room(id, ref i);

        if(i == 0)
        {
            GetComponent<Message>().Target_Message("Room doesn't exist");
        }
        else if(i == 1)
        {
            GetComponent<Message>().Target_Message("Room is full");
        }
        else
        {
            Sv_StartGame();
        }
    }

    [Server]
    void Sv_StartGame()
    {
        Target_StartGame(data.mapdata);
    }

    [TargetRpc]
    void Target_StartGame(DungeonBuilder.DataMapRandom dt)
    {
        mapdata = dt;

        SceneManager.LoadScene("MainMenuScene");
    }
}