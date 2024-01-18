using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class Instance : MonoBehaviour
{
    [SerializeField] GameObject nw_mng;
    [SerializeField] GameObject select_mode_prefab;
    [SerializeField] bool is_server = false;
    NetworkManager nw;

    private void Awake() 
    {
        Application.targetFrameRate = 60;

        nw = Instantiate(nw_mng).GetComponent<NetworkManager>();

        if(is_server)
        {
            nw.StartServer();
        }
        else
        {
            Instantiate(select_mode_prefab).GetComponent<SelectMode>().instance = this;
        }
    }

    public void Host()
    {
        C_Data.Instance.is_single = true;

        nw.StartHost();
    }

    public void Client()
    {
        C_Data.Instance.is_single = false;

        nw.StartClient();
    }
}