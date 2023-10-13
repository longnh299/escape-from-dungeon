using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string name;

    #region Header
    [Header("Only flag the RoomNodeType that should be visible in the editor")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;

    #region Header
    [Header("One type should be a corridor")]
    #endregion Header
    public bool isCorridor;

    #region Header
    [Header("One type should be a vertical corridor")]
    #endregion Header
    public bool isCorridorVertical;

    #region Header
    [Header("One type should be a horizontal corridor")]
    #endregion Header
    public bool isCorridorHorizontal;

    #region Header
    [Header("One type should be an entrance")]
    #endregion Header
    public bool isEntrance;

    #region Header
    [Header("One type should be a boss room")]
    #endregion Header
    public bool isBossRoom;

    #region Header
    [Header("One type should be none (unassigned)")]
    #endregion Header
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.IsEmptyString(this, nameof(name), name);
    }
#endif
    #endregion
}
