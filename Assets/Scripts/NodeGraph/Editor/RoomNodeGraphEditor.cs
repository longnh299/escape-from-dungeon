using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle = null;
    private float width = 160f;
    private float height = 80f;
    private int padding = 20;
    private int border = 10;

    private void OnEnable()
    {
        // define layout of editor
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.padding = new RectOffset(padding, padding, padding, padding);
        roomNodeStyle.border = new RectOffset(border, border, border, border);

    }
    // create room node graph editor on unity editor
    [MenuItem("Room Graph Editor", menuItem = "Window/Room Editor/Room Node Graph Editor")]

    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Editor");
    }

    // create editor gui
    private void OnGUI()
    {
        // create node rectangle 
        GUILayout.BeginArea(new Rect(new Vector2(100f, 100f), new Vector2(width, height)), roomNodeStyle);
        EditorGUILayout.LabelField("node 1");
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(new Vector2(200f, 200f), new Vector2(width, height)), roomNodeStyle);
        EditorGUILayout.LabelField("node 1");
        GUILayout.EndArea();
    }
}
