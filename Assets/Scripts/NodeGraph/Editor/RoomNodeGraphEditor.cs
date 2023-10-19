using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;

    // room node style attributes
    private float width = 160f;
    private float height = 80f;
    private int padding = 20;
    private int border = 10;

    // line style attributes
    private float lineWidth = 5f;
    private float arrowSize = 6f;

    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;
    private RoomNodeSO currentRoomNode;

    private void OnEnable()
    {
        // define layout of node
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.padding = new RectOffset(padding, padding, padding, padding);
        roomNodeStyle.border = new RectOffset(border, border, border, border);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

    }
    // create room node graph editor on unity editor
    [MenuItem("Room Graph Editor", menuItem = "Window/Room Editor/Room Node Graph Editor")]

    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Editor");
    }

    //double click on room node graph asset in inspector => open room node graph editor 
    [OnOpenAsset(0)]
    private static bool OnDoubleClickAsset(int id, int line)
    {
        RoomNodeGraphSO roomNodeGraphSO = EditorUtility.InstanceIDToObject(id) as RoomNodeGraphSO; // ko hieu

        if (roomNodeGraphSO != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraphSO;
            return true;
        }

        return false;
    }

    // create editor gui
    private void OnGUI()
    {

        if (currentRoomNodeGraph != null)
        {

            // draw line if being dragged
            DrawDraggedLine();

            // process event
            ProcessEvents(Event.current);

            // draw room node connection
            DrawRoomNodeConnection();

            // draw room node
            DrawRoomNodes();

            if (GUI.changed)
            {
                Repaint();
            }
        }
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePostion != Vector2.zero)
        {
            // draw line from current room node to line position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeDrawLineStart.rect.center, currentRoomNodeGraph.linePostion, currentRoomNodeGraph.roomNodeDrawLineStart.rect.center, currentRoomNodeGraph.linePostion, Color.white, null, lineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // get room node that mouse is over if it is null of not currently being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // if mouse is not over on a room node or dragging line from room node
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeDrawLineStart != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
            // process room node event
            currentRoomNode.ProcessEvents(currentEvent);
        
    }

    // check is mouse over on room node?
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for(int i = (currentRoomNodeGraph.roomNodes.Count - 1); i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodes[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodes[i];
            }
        }

        return null;
    }

    // process room node graph events
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            default: 
                break;
        }
    }

    // mouse down event on rng editor
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // right click, 1 is right mouse, 0 is left mouse
        if (currentEvent.button == 1) 
        {
            ShowMenu(currentEvent.mousePosition);
        } 
    }

    // show choose menu
    private void ShowMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create room node"), false, CreateRoomNode, mousePosition);
        menu.ShowAsContext();
    }

    // mouse drag event
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // right click to draw line
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }

    // process right mouse drag event
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeDrawLineStart != null)
        {
           DragLine(currentEvent.delta);
           GUI.changed = true;
        }
    }

    // drag line method
    private void DragLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePostion += delta;
    }

    // process mouse up event
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // if right mouse up and dragging a line
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeDrawLineStart != null)
        {
            // check if mouse over a room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                // add room node is overed mouse to be a child of room node line start
                if (currentRoomNodeGraph.roomNodeDrawLineStart.AddChildIdToRoomNode(roomNode.id))
                {
                    // add room node start line to be a parent of room node 
                    roomNode.AddParentIdToRoomNode(currentRoomNodeGraph.roomNodeDrawLineStart.id);
                }
            }

            ClearLineDrag();
        }
    }

    // reset drag line when mouse up
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeDrawLineStart = null;
        currentRoomNodeGraph.linePostion = Vector2.zero;

        GUI.changed = true;
    }

    // draw coonection of room node to other
    private void DrawRoomNodeConnection()
    {
        //loop all room nodes
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodes)
        {
            if (roomNode.childRoomNodeIds.Count > 0)
            {
                // loop all child node of roomNode
                foreach(string childId in roomNode.childRoomNodeIds)
                {
                    // get child room node from dictionary
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childId))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childId]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    // draw line between parent node to child node
    private void DrawConnectionLine(RoomNodeSO parent, RoomNodeSO child)
    {
        // get start and end position of line
        Vector2 startLine = parent.rect.center;
        Vector2 endLine = child.rect.center;

        // mid point
        Vector2 midPoint = (startLine + endLine) / 2;

        // vector from start to end of line
        Vector2 direction = endLine - startLine;

        // caculate normalised perpendicular position from mid point
        Vector2 arrowTail1 = midPoint - new Vector2(-direction.y, direction.x).normalized * arrowSize;
        Vector2 arrowTail2 = midPoint + new Vector2(-direction.y, direction.x).normalized * arrowSize;

        // caculate mid point offset for arrow head
        Vector2 arrowHead = midPoint + direction.normalized * arrowSize;

        // draw arrow
        Handles.DrawBezier(arrowHead, arrowTail1, arrowHead, arrowTail1, Color.white, null, lineWidth);
        Handles.DrawBezier(arrowHead, arrowTail2, arrowHead, arrowTail2, Color.white, null, lineWidth);

        // draw line
        Handles.DrawBezier(startLine, endLine, startLine, endLine, Color.white, null, lineWidth);

        GUI.changed = true;
    }

    // create room node at mouse position
    private void CreateRoomNode(object mousePosition)
    {
        CreateRoomNode(mousePosition, roomNodeTypeList.roomNodeTypes.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create room node so asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // add room node to room node list of current room node graph
        currentRoomNodeGraph.roomNodes.Add(roomNode);

        // init room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(width, height)), currentRoomNodeGraph, roomNodeType);

        // add room node to room node graph so asset database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        // refresh room node graph dictionary
        currentRoomNodeGraph.OnValidate();
    }

    // draw all room nodes in room node list of current room node graph
    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodes)
        {
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;
    }
}
