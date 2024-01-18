using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetGameManager : NetworkBehaviour
{
    NetPlayer player;
    PlayerDetailsSO playerDetails;

    void Awake()
    {
        player = GetComponent<NetPlayer>();
    }

    public void Spawn_Character()
    {
        if (isOwned)
        {
            int i;

            for (i = 0; i < GameResources.Instance.playerDetailsList.Count; i++)
            {
                if (GameResources.Instance.currentPlayer.playerDetails == GameResources.Instance.playerDetailsList[i]) break;
            }

            Cmd_Spawn_Chr(i);
        }
    }

    [Command]
    void Cmd_Spawn_Chr(int i)
    {
        GetComponent<NetworkTeam>().ChangeID(player.data.ID.ToString());

        playerDetails = GameResources.Instance.playerDetailsList[i];

        GameObject chr = Instantiate(playerDetails.playerPrefab, new Vector3(Random.Range(8, 12), 0, 0), Quaternion.identity);

        chr.GetComponent<NetworkTeam>().ChangeID(player.data.ID.ToString());

        chr.GetComponent<Player>().Sv_Initialize(playerDetails);

        chr.GetComponent<Player>().chr_index = i;

        NetworkServer.Spawn(chr, connectionToClient);
    }
}