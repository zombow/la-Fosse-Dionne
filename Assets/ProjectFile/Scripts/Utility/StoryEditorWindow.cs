using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class StoryEditorWindow : EditorWindow
{
    private List<StoryBlock> loadedBlocks = new();
    private List<StoryNode> nodes = new();

    private Vector2 dragOffset;
    private StoryNode selectedNode = null;
    private Vector2 panOffset = Vector2.zero;
    private Vector2 lastMousePos;
    private bool isPanning = false;

    private float zoom = 1.0f;
    private const float zoomMin = 0.3f;
    private const float zoomMax = 2.0f;

    private ConnectionMode currentConnectionMode = ConnectionMode.None;
    private StoryNode fromNode = null;
    private int selectedChoiceIndex = -1;

    public enum ConnectionMode
    {
        None,
        ChoiceNext,
        ChoiceSuccess,
        ChoiceFail
    }

    public class StoryNode
    {
        public Rect rect;
        public StoryBlock block;
        public bool isDragged;

        public StoryNode(StoryBlock block, Rect rect)
        {
            this.block = block;
            this.rect = rect;
        }

        public void Draw(StoryEditorWindow editor)
        {
            Rect drawRect = rect;
            drawRect.position += editor.panOffset;
            drawRect.position *= editor.zoom;
            drawRect.size *= editor.zoom;

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            GUI.Box(drawRect, "");
            GUI.Label(new Rect(drawRect.xMin, drawRect.yMin + 5, drawRect.width, 20), block.name, titleStyle);

            int offset = 30;
            for (int i = 0; i < block.choices.Count; i++)
            {
                GUI.Label(new Rect(drawRect.xMin + 10, drawRect.yMin + offset, 100, 20), $"Choice {i}");

                if (GUI.Button(new Rect(drawRect.xMax - 110, drawRect.yMin + offset, 20, 20), "N"))
                    editor.BeginConnection(this, ConnectionMode.ChoiceNext, i);

                if (GUI.Button(new Rect(drawRect.xMax - 85, drawRect.yMin + offset, 20, 20), "S"))
                    editor.BeginConnection(this, ConnectionMode.ChoiceSuccess, i);

                if (GUI.Button(new Rect(drawRect.xMax - 60, drawRect.yMin + offset, 20, 20), "F"))
                    editor.BeginConnection(this, ConnectionMode.ChoiceFail, i);

                if (GUI.Button(new Rect(drawRect.xMax - 35, drawRect.yMin + offset, 20, 20), "x"))
                {
                    block.choices[i].nextBlock = null;
                    block.choices[i].successBlock = null;
                    block.choices[i].failBlock = null;
                    EditorUtility.SetDirty(block);
                }

                offset += 25;
            }
        }

        public Rect GetDrawRect(Vector2 panOffset, float zoom)
        {
            Rect drawRect = rect;
            drawRect.position += panOffset;
            drawRect.position *= zoom;
            drawRect.size *= zoom;
            return drawRect;
        }

        public bool Contains(Vector2 screenPosition, Vector2 panOffset, float zoom)
        {
            return GetDrawRect(panOffset, zoom).Contains(screenPosition);
        }
    }

    [MenuItem("Tools/Story Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<StoryEditorWindow>("Story Editor");
        window.LoadLayout();
    }

    private void OnEnable()
    {
        LoadStoryBlocks();
        LoadLayout();
    }

    private void OnDisable()
    {
        SaveLayout();
    }

    private void OnGUI()
    {
        HandleZoom(Event.current);
        HandlePanning(Event.current);

        DrawConnections();

        foreach (var node in nodes)
            node.Draw(this);

        if (fromNode != null)
        {
            Handles.DrawBezier(
                (fromNode.rect.center + panOffset) * zoom,
                Event.current.mousePosition,
                (fromNode.rect.center + panOffset + Vector2.right * 50) * zoom,
                Event.current.mousePosition + Vector2.left * 50,
                Color.yellow, null, 2f);
            Repaint();
        }

        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void HandleZoom(Event e)
    {
        if (e.type == EventType.ScrollWheel)
        {
            Vector2 mousePos = e.mousePosition;
            float delta = -e.delta.y * 0.01f;
            float oldZoom = zoom;
            zoom = Mathf.Clamp(zoom + delta, zoomMin, zoomMax);

            Vector2 mouseWorld = (mousePos - panOffset) / oldZoom;
            panOffset = mousePos - mouseWorld * zoom;
            e.Use();
        }
    }

    private void HandlePanning(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 2)
        {
            isPanning = true;
            lastMousePos = e.mousePosition;
            e.Use();
        }
        else if (e.type == EventType.MouseDrag && isPanning)
        {
            Vector2 delta = e.mousePosition - lastMousePos;
            panOffset += delta;
            lastMousePos = e.mousePosition;
            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 2)
        {
            isPanning = false;
            e.Use();
        }
    }

    private void ProcessEvents(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            StoryNode clickedNode = GetNodeAtPosition(e.mousePosition);

            if (clickedNode != null)
            {
                if (fromNode != null && fromNode != clickedNode)
                {
                    StoryBlock targetBlock = clickedNode.block; // ★ 바로 block 참조 사용!

                    switch (currentConnectionMode)
                    {
                        case ConnectionMode.ChoiceNext:
                            fromNode.block.choices[selectedChoiceIndex].nextBlock = targetBlock;
                            break;
                        case ConnectionMode.ChoiceSuccess:
                            fromNode.block.choices[selectedChoiceIndex].successBlock = targetBlock;
                            break;
                        case ConnectionMode.ChoiceFail:
                            fromNode.block.choices[selectedChoiceIndex].failBlock = targetBlock;
                            break;
                    }

                    EditorUtility.SetDirty(fromNode.block);
                    fromNode = null;
                    currentConnectionMode = ConnectionMode.None;
                    e.Use();
                }
                else
                {
                    selectedNode = clickedNode;
                    Vector2 drawPos = selectedNode.GetDrawRect(panOffset, zoom).position;
                    dragOffset = e.mousePosition - drawPos;
                    selectedNode.isDragged = true;
                }
            }
            else
            {
                selectedNode = null;
                fromNode = null;
                currentConnectionMode = ConnectionMode.None;
            }
        }
        // 나머지 로직 동일
    
        else if (e.type == EventType.MouseUp)
        {
            if (selectedNode != null)
            {
                selectedNode.isDragged = false;
                selectedNode = null;
            }
        }
        else if (e.type == EventType.MouseDrag && e.button == 0)
        {
            if (selectedNode != null && selectedNode.isDragged)
            {
                Vector2 newPos = (e.mousePosition - dragOffset) / zoom - panOffset;
                selectedNode.rect.position = newPos;
                e.Use();
                GUI.changed = true;
            }
        }
    }

    private StoryNode GetNodeAtPosition(Vector2 position)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].Contains(position, panOffset, zoom))
                return nodes[i];
        }

        return null;
    }

    private void DrawConnections()
    {
        foreach (var node in nodes)
        {
            foreach (var choice in node.block.choices)
            {
                if (choice.requiresProbabilityCheck)
                {
                    DrawConnectionToNode(node, choice.successBlock, Color.green);
                    DrawConnectionToNode(node, choice.failBlock, Color.red);
                }
                else
                {
                    DrawConnectionToNode(node, choice.nextBlock, Color.cyan);
                }
            }
        }
    }

    
    private void DrawConnectionToNode(StoryNode fromNode, StoryBlock toBlock, Color color)
    {
        if (toBlock == null) return;

        // 노드 참조 찾기
        StoryNode toNode = nodes.Find(n => n.block == toBlock);
        if (toNode == null) return;

        // 아래 → 위 방향으로 연결
        Vector2 from = new Vector2(fromNode.rect.center.x, fromNode.rect.yMax);
        Vector2 to   = new Vector2(toNode.rect.center.x, toNode.rect.yMin);

        from = (from + panOffset) * zoom;
        to   = (to + panOffset) * zoom;

        Handles.DrawBezier(
            from,
            to,
            from + Vector2.up * 50,
            to + Vector2.down * 50,
            color, null, 3f
        );

        DrawArrowHead(to, Vector2.down, color);
    }


    
    private void DrawArrowHead(Vector2 position, Vector2 direction, Color color)
    {
        float arrowHeadAngle = 20.0f;
        float arrowHeadLength = 16.0f;

        Vector2 right = Quaternion.Euler(0, 0, arrowHeadAngle) * direction.normalized;
        Vector2 left = Quaternion.Euler(0, 0, -arrowHeadAngle) * direction.normalized;

        Vector2 p1 = position;
        Vector2 p2 = position + right * arrowHeadLength;
        Vector2 p3 = position + left * arrowHeadLength;

        Handles.color = color;
        Handles.DrawAAConvexPolygon(p1, p2, p3);
    }


    private void SaveLayout()
    {
        List<string> layoutData = new();
        foreach (var node in nodes)
        {
            string assetPath = AssetDatabase.GetAssetPath(node.block);
            layoutData.Add($"{assetPath}|{node.rect.x}|{node.rect.y}");
        }

        File.WriteAllLines("Assets/Editor/StoryEditorLayout.txt", layoutData);
    }

    private void LoadLayout()
    {
        string layoutFile = "Assets/Editor/StoryEditorLayout.txt";
        if (!File.Exists(layoutFile)) return;

        Dictionary<string, Vector2> savedPositions = new();
        foreach (var line in File.ReadAllLines(layoutFile))
        {
            var parts = line.Split('|');
            if (parts.Length == 3)
            {
                savedPositions[parts[0]] = new Vector2(float.Parse(parts[1]), float.Parse(parts[2]));
            }
        }

        foreach (var node in nodes)
        {
            string path = AssetDatabase.GetAssetPath(node.block);
            if (savedPositions.TryGetValue(path, out Vector2 pos))
            {
                node.rect.position = pos;
            }
        }
    }

    private void LoadStoryBlocks()
    {
        loadedBlocks.Clear();
        nodes.Clear();
        string[] guids = AssetDatabase.FindAssets("t:StoryBlock");
        int row = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var block = AssetDatabase.LoadAssetAtPath<StoryBlock>(path);
            if (block != null)
            {
                Rect rect = new Rect(50 + row * 300, 100, 250, 75);
                nodes.Add(new StoryNode(block, rect));
                row++;
            }
        }
    }

    public void BeginConnection(StoryNode fromNode, ConnectionMode mode, int choiceIndex = -1)
    {
        this.fromNode = fromNode;
        this.currentConnectionMode = mode;
        this.selectedChoiceIndex = choiceIndex;
    }

    public void ClearConnection(StoryNode fromNode, ConnectionMode mode, int choiceIndex, bool isDual = false)
    {
        switch (mode)
        {
            case ConnectionMode.ChoiceNext:
                fromNode.block.choices[choiceIndex].nextBlock = null;
                break;
            case ConnectionMode.ChoiceSuccess:
                fromNode.block.choices[choiceIndex].successBlock = null;
                if (isDual) fromNode.block.choices[choiceIndex].failBlock = null;
                break;
            case ConnectionMode.ChoiceFail:
                fromNode.block.choices[choiceIndex].failBlock = null;
                break;
        }

        EditorUtility.SetDirty(fromNode.block);
    }
}