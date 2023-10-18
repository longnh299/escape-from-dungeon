using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypes;
    [HideInInspector] public List<RoomNodeSO> roomNodes = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    #region Editor Code
#if UNITY_EDITOR
    [HideInInspector] public RoomNodeSO roomNodeDrawLineStart = null;
    [HideInInspector] public Vector2 linePostion;

    public void SetNodeToDrawConnectionLineStart(RoomNodeSO roomNode, Vector2 position)
    {
        roomNodeDrawLineStart = roomNode;
        linePostion = position;
    }
#endif
    #endregion Editor Code
}
