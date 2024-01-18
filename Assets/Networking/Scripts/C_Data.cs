using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Data
{
    private C_Data(){}
    static readonly C_Data _ = new C_Data();
    public static C_Data Instance => _;

    public NetPlayer player;
    public UI_Lobby UI_Lobby;
    public Player character;
    public bool is_single = false;
}
