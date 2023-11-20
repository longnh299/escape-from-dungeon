using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypes;
    [HideInInspector] public List<RoomNodeSO> roomNodes = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    /* public RoomNodeSO GetRoomNodeById(string id)
     {
         foreach (RoomNodeSO roomNode in roomNodes)
         {
             if (roomNode.id == id)
             { return roomNode; }
         }
         return null;
     } */

    public RoomNodeSO GetRoomNodeById(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }

    // get child room node
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomNodeIds)
        {
            yield return GetRoomNodeById(childNodeID);
        }
    }

    public void Awake()
    {
        // init room node dictionary
        LoadRoomNodeDictionary();
    }

    // load room node dictionary from room node list
    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();
        foreach(RoomNodeSO room in roomNodes)
        {
            roomNodeDictionary[room.id] = room;
        }

    }

    // get room node by room type
    public RoomNodeSO getRoomNode(RoomNodeTypeSO roomType)
    {
        foreach(RoomNodeSO roomNode in roomNodes)
        {
            if(roomNode.roomNodeType == roomType)
            {
                return roomNode;
            }
        }
        return null;
    }

    #region Editor Code
#if UNITY_EDITOR
    [HideInInspector] public RoomNodeSO roomNodeDrawLineStart = null;
    [HideInInspector] public Vector2 linePostion;

    // recaculate dictionary every time has change in room node grah editor
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineStart(RoomNodeSO roomNode, Vector2 position)
    {
        roomNodeDrawLineStart = roomNode;
        linePostion = position;
    }
#endif
    #endregion Editor Code
}
