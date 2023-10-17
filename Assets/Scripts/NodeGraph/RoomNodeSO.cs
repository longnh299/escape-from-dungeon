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

    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

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

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    // process mouse down event
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // left click down
        if (currentEvent.button == 0) // left mouse press
        {
            ProcessLeftClickDownEvent();
        }
    }

    // process left click down event
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        // toggle node selection
        if (isSelected == true)
            isSelected = false;
        else
            isSelected = true;
    }

    // process mouse up event
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // left mouse up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent(); 
        }
    }

    // process left click up event
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    // process mouse drag event
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // mouse drag
        if (currentEvent.button == 0) // use left mouse drag room node
        {
            ProcessLeftMouseDragEvent(currentEvent);
        } 
    }

    // process left mouse drag
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    // process drag room node
    private void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }



#endif
    #endregion Editor Code
}
