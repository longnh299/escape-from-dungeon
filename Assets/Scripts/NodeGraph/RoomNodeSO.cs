using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIds = new List<string>();
    public List<string> childRoomNodeIds = new List<string>();
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

        // if room nodes has parent or room node is entrance => it can not change type.
        if (parentRoomNodeIds.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.name);

        } else
        {
            // display popup
            int selected = roomNodeTypes.roomNodeTypes.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypes.roomNodeTypes[selection];

            // If the room type selection has changed making child connections potentially invalid
            if (roomNodeTypes.roomNodeTypes[selected].isCorridor && !roomNodeTypes.roomNodeTypes[selection].isCorridor 
                || !roomNodeTypes.roomNodeTypes[selected].isCorridor && roomNodeTypes.roomNodeTypes[selection].isCorridor 
                || !roomNodeTypes.roomNodeTypes[selected].isBossRoom && roomNodeTypes.roomNodeTypes[selection].isBossRoom)
            {
                // vẫn tạo được 2 boss room -> cần fix
                // if a room node type has been changed and it already has children then delete child connect line since revalidate any
                if (childRoomNodeIds.Count > 0)
                {
                    // remove all child ids and parent ids
                    for (int i = childRoomNodeIds.Count - 1; i >= 0; i--)
                    {
                        // get child
                        RoomNodeSO childRoom = roomNodeGraph.GetRoomNodeById(childRoomNodeIds[i]);

                        if (childRoom != null)
                        {
                            // parent remove child id
                            RemoveChildIdFromRoomNode(childRoom.id);
                            // child remove parent id
                            childRoom.RemoveParentIdFromRoomNode(id);
                        }
                    }
                }
            }

        }

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

        // right click down
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
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

    // process right click down event
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineStart(this, currentEvent.mousePosition);
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
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    // add child id to room node
    public bool AddChildIdToRoomNode(string childId)
    {
        if (IsChildRoomValid(childId)) // valid child room node
        {
            childRoomNodeIds.Add(childId);
            return true;
        }

        return false;

    }

    // Check the child node can be validly added to the parent node - return true if it can otherwise return false
    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        // Check if there is there already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodes)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIds.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        // if the child node has a type of boss room and there is already a connected boss room node then return false
        if (roomNodeGraph.GetRoomNodeById(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;

        // If the child node has a type of none then return false
        if (roomNodeGraph.GetRoomNodeById(childID).roomNodeType.isNone)
            return false;

        // If the node already has a child with this child ID return false
        if (childRoomNodeIds.Contains(childID))
            return false;

        // If this node ID and the child ID are the same return false
        if (id == childID)
            return false;

        // If this childID is already in the parentID list return false
        if (parentRoomNodeIds.Contains(childID))
            return false;

        // If the child node already has a parent return false
        if (roomNodeGraph.GetRoomNodeById(childID).parentRoomNodeIds.Count > 0)
            return false;

        // If child is a corridor and this node is a corridor return false
        if (roomNodeGraph.GetRoomNodeById(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        // If child is not a corridor and this node is not a corridor return false
        if (!roomNodeGraph.GetRoomNodeById(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        // If adding a corridor check that this node has < the maximum permitted child corridors
        if (roomNodeGraph.GetRoomNodeById(childID).roomNodeType.isCorridor && childRoomNodeIds.Count >= Settings.maxChildCorridors)
            return false;

        // if the child room is an entrance return false - the entrance must always be the top level parent node
        if (roomNodeGraph.GetRoomNodeById(childID).roomNodeType.isEntrance)
            return false;

        // If adding a room to a corridor check that this corridor node doesn't already have a room added
        if (!roomNodeGraph.GetRoomNodeById(childID).roomNodeType.isCorridor && childRoomNodeIds.Count > 0)
            return false;

        return true;
    }

    // add parent id to room node
    public bool AddParentIdToRoomNode(string parentId)
    {
        parentRoomNodeIds.Add(parentId);
        return true;
    }

    // remove child id
    public bool RemoveChildIdFromRoomNode(string childId)
    {
        //if child id list of room node has childId => remove it
        if (childRoomNodeIds.Contains(childId))
        {
            childRoomNodeIds.Remove(childId);
            return true;
        } 

        return false;
    }

    // remove parent id
    public bool RemoveParentIdFromRoomNode(string parentId)
    {
        // if parent id list of room node has parentId => remove it
        if (parentRoomNodeIds.Contains(parentId))
        {
            parentRoomNodeIds.Remove(parentId);
            return true;
        }

        return false;
    }




#endif
    #endregion Editor Code
}
