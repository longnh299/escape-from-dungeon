using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMode : MonoBehaviour
{
    public Instance instance;

    public void Multiplayer_btn()
    {
        instance.Client();
    }

    public void Singleplayer_btn()
    {
        instance.Host();
    }

    public void Quit_btn()
    {
        Application.Quit();
    }
}
