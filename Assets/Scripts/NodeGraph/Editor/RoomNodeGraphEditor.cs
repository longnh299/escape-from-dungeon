using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private float width = 160f;
    private float height = 80f;
    private int padding = 20;
    private int border = 10;

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
            // process event
            ProcessEvents(Event.current);

            // draw room node
            DrawRoomNodes();

            if (GUI.changed)
            {
                Repaint();
            }
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // get room node that mouse is over if it is null of not currently being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // if mouse is not over on a room node
        if (currentRoomNode == null)
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
