using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIds = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIds = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypes;

    #region Editor Code
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;

    public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString(); // generate new guid
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;

        // load room node type list
        roomNodeTypes = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle roomNodeStyle)
    {
        // draw node box using begin area
        GUILayout.BeginArea(rect, roomNodeStyle);

        EditorGUI.BeginChangeCheck();

        // display popup
        int selected = roomNodeTypes.roomNodeTypes.FindIndex(x => x == roomNodeType);

        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

        roomNodeType = roomNodeTypes.roomNodeTypes[selection];

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        
        GUILayout.EndArea();
    }

    // get name of room node types to dispaly
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomNames = new string[roomNodeTypes.roomNodeTypes.Count];

        for(int i = 0; i < roomNodeTypes.roomNodeTypes.Count; i++)
        {
            if (roomNodeTypes.roomNodeTypes[i].displayInNodeGraphEditor)
            {
                roomNames[i] = roomNodeTypes.roomNodeTypes[i].name;
            }
        }

        return roomNames;
    }
#endif
    #endregion Editor Code
}
