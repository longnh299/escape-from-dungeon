using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{

    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion

    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS
    public static int maxChildCorridors = 4; // max child corridor 
    #endregion 

    #region ANIMATOR PARAMETERS
    // player animator parameter (convert to int because compare int is faster than compare string)
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollDown = Animator.StringToHash("rollDown");

    // Animator parameters - Door
    public static int open = Animator.StringToHash("open");

    #endregion

    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion
}
