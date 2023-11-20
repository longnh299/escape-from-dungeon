using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public string id;
    public string templateId;

    public GameObject prefab;
    //public RoomTemplateSO roomTemplate;
    public RoomNodeTypeSO roomNodeType;

    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;

    public Vector2Int templateLowerBounds;
    public Vector2Int templateUpperBounds;

    public Vector2Int[] spawnPositionArray;
    public List<string> childRoomIdList;
    public string parentRoomId;
    public List<Doorway> doorWayList;
    public bool isPositioned = false; 
    public InstantiatedRoom instantiatedRoom;
    public bool isLit = false; // room đã được thắp sáng chưa?
    public bool isClearedOfEnemies = false; // đã tiêu diệt hết quái trong room chưa?
    public bool isPreviouslyVisited = false;

    public Room()
    {
        childRoomIdList = new List<string>();
        doorWayList = new List<Doorway>();
    }

}
