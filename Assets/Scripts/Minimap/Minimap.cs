using Cinemachine;
using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the child MinimapPlayer gameobject")]
    #endregion Tooltip

    [SerializeField] GameObject miniPlayerTemp;
    [SerializeField] private List<GameObject> miniMapPlayer = new List<GameObject>();

    private List<Transform> playerTransform = new List<Transform>();

    void Start()
    {
        foreach (var i in FindObjectsOfType<Player>())
        {
            AddPlayer(i, i == C_Data.Instance.character);
        }
    }

    public void AddPlayer(Player chr, bool def = false)
    {
        playerTransform.Add(chr.transform);

        if (def)
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = chr.transform;
        }

        GameObject m = Instantiate(miniPlayerTemp, miniPlayerTemp.transform.parent);

        m.SetActive(true);

        miniMapPlayer.Add(m);

        // Set minimap player icon
        SpriteRenderer spriteRenderer = m.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GameResources.Instance.playerDetailsList[chr.chr_index].playerMiniMapIcon; ;
        }
    }

    int i = 0;
    private void Update()
    {
        // Move the minimap player to follow the player
        for (i = 0; i < playerTransform.Count; i++)
        {
            if (playerTransform[i] != null && miniMapPlayer[i] != null)
            {
                miniMapPlayer[i].transform.position = playerTransform[i].position;
            }
            else
            {
                Destroy(miniMapPlayer[i]);

                playerTransform.Remove(null);

                miniMapPlayer.Remove(null);
            }
        }
    }
}