using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header Room Node Type List
    [Space(10)]
    [Header("Room node type list")]
    #endregion

    #region Tooltip
    [Tooltip("This list should be populated with all the RoomTypeSO for the game, it is used instead of enum")]
    #endregion
    public List<RoomNodeTypeSO> roomNodeTypes;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.IsEnumerableValues(this, nameof(roomNodeTypes), roomNodeTypes);
    }
#endif
    #endregion
}
