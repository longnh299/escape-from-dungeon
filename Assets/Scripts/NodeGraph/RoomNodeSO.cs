using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIds;
    [HideInInspector] public List<string> childRoomNodeIds;
    [HideInInspector] public RoomNodeGraphSO roomNodeGraphSO;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypes;
}
