using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using Codice.CM.WorkspaceServer.DataStore;
using System;

public class NodeGraphEditor : EditorWindow
{
    private GUIStyle nodeStyle;
    private GUIStyle nodeSelectedStyle;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private NodeTypeListSO nodeTypeList;
    private NodeSO currentNode = null;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 100f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    private static NodeGraphSO currentNodeGraph;

    /// <summary>
    /// 打开编辑器窗口
    /// </summary>
    [MenuItem("Node Graph Editor", menuItem = "Window/Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<NodeGraphEditor>("Node Graph Editor");
    }

    /// <summary>
    /// 双击打开节点编辑器资产
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        NodeGraphSO NodeGraph = EditorUtility.InstanceIDToObject(instanceID) as NodeGraphSO;

        if (NodeGraph != null)
        {
            OpenWindow();

            currentNodeGraph = NodeGraph;

            return true;
        }

        return false;
    }

    private void OnEnable() {
        Selection.selectionChanged += InspectorSelectionChanged;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        nodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        nodeSelectedStyle = new GUIStyle();
        nodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        nodeSelectedStyle.normal.textColor = Color.white;
        nodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        nodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    private void OnDisable() {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    private void OnGUI() {
        if (currentNodeGraph != null)
        {
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.2f, Color.gray);

            DrawDraggedLine();

            ProcessEvent(Event.current);

            DrawConnections();

            DrawNodes();
        }

        if(GUI.changed)
            Repaint();
    }

    /// <summary>
    /// 绘制被拽出的线条
    /// </summary>
    private void DrawDraggedLine()
    {
        if (currentNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentNodeGraph.nodeToDrawLineFrom.rect.center, currentNodeGraph.linePosition, 
                currentNodeGraph.nodeToDrawLineFrom.rect.center, currentNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    

    /// <summary>
    /// 给编辑器赋予网格背景
    /// </summary>
    /// <param name="gridSize">网格大小</param>
    /// <param name="gridOpacity">网格透明度</param>
    /// <param name="gridColor">网格颜色</param>
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) /gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0) + gridOffset);
        }
        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0) + gridOffset);
        }

        Handles.color = Color.white;
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    private void ProcessEvent(Event currentEvent)
    {
        graphDrag = Vector2.zero;

        if (currentNode == null || currentNode.isLeftClickDragging == false)
        {
            currentNode = IsMouseOverNode(currentEvent);
        }

        if (currentNode == null || currentNodeGraph.nodeToDrawLineFrom != null)
        {
            ProcessNodeGraphEvent(currentEvent);
        }
        else
        {
            currentNode.ProcessEvents(currentEvent);
        }

    }

    /// <summary>
    /// 获取当前鼠标所在节点
    /// </summary>
    private NodeSO IsMouseOverNode(Event currentEvent)
    {
        for (int i = currentNodeGraph.nodeList.Count - 1; i >= 0 ; i--)
        {
            if (currentNodeGraph.nodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentNodeGraph.nodeList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 触发节点地图内的事件
    /// </summary>
    private void ProcessNodeGraphEvent(Event currentEvent)
    {
        switch(currentEvent.type)
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
        }
    }

#region 鼠标拖拽
    /// <summary>
    /// 触发鼠标拖拽事件
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentNodeGraph.nodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentNodeGraph.nodeList.Count; i++)
        {
            currentNodeGraph.nodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    public void DragConnectingLine(Vector2 detal)
    {
        currentNodeGraph.linePosition += detal;
    }
#endregion 鼠标拖拽

#region 鼠标按键抬起
    /// <summary>
    /// 触发鼠标按键抬起事件
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentNodeGraph.nodeToDrawLineFrom != null)
        {
            NodeSO Node = IsMouseOverNode(currentEvent);

            if (Node != null)
            {
                if (currentNodeGraph.nodeToDrawLineFrom.AddChildNodeIDToNode(Node.id))
                {
                    Node.AddParentNodeIDToNode(currentNodeGraph.nodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ClearLineDrag()
    {
        currentNodeGraph.nodeToDrawLineFrom = null;
        currentNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }
#endregion 鼠标按键抬起

#region 鼠标按键按下
    /// <summary>
    /// 触发鼠标按键按下事件
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedNodes();
        }else if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    /// <summary>
    /// 在编辑器内展示菜单栏
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("创建普通节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isDefault)));

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("创建密码锁节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isLocked)));
        menu.AddItem(new GUIContent("创建AI锁节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isAI)));
        menu.AddItem(new GUIContent("创建角度锁节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isAngleLock)));

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("创建图节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isGraph)));
        menu.AddItem(new GUIContent("创建探测节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isProbe)));
        menu.AddItem(new GUIContent("创建合成节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isSynthetic)));
        menu.AddItem(new GUIContent("创建可控制节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isControllable)));
        menu.AddItem(new GUIContent("创建合成图片节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isSyntheticPicture)));
        menu.AddItem(new GUIContent("创建计时节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isTiming)));
        menu.AddItem(new GUIContent("创建快速点击节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isQuickClick)));
        menu.AddItem(new GUIContent("创建QTE节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isQTE)));
        menu.AddItem(new GUIContent("创建对话节点"),false,() => CreateNode(mousePosition, nodeTypeList.list.Find(x => x.isDialog)));

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Select All Node"), false, SelectAllNode);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Delete Selected Node Links"), false, DeleteSelectedNodeLinks);

        menu.AddItem(new GUIContent("Delete Selected Nodes"), false, DeleteSelectedNodes);

        menu.ShowAsContext();
    }

    /// <summary>
    /// 选择所有节点
    /// </summary>
    private void SelectAllNode()
    {
        foreach (NodeSO roomNode in currentNodeGraph.nodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }


    /// <summary>
    /// 删除所有被选择的节点
    /// </summary>
    private void DeleteSelectedNodes()
    {
        Queue<NodeSO> nodeDeletionQueue = new Queue<NodeSO>();

        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            if (node.isSelected == true)
            {
                nodeDeletionQueue.Enqueue(node);

                foreach (string childNodeID in node.childrenNodeIdList)
                {
                    NodeSO childNode = currentNodeGraph.GetNode(childNodeID);

                    if (childNode != null)
                    {
                        childNode.RemoveParentNodeIDFromNode(node.id);
                    }
                }

                foreach (string parentNodeID in node.parentNodeIdList)
                {
                    NodeSO parentNode = currentNodeGraph.GetNode(parentNodeID);

                    if (parentNode != null)
                    {
                        parentNode.RemoveChildNodeIDFromNode(node.id);
                    }
                }
            }
        }
        
        while (nodeDeletionQueue.Count > 0)
        {
            NodeSO nodeToDelete = nodeDeletionQueue.Dequeue();

            currentNodeGraph.nodeDictionary.Remove(nodeToDelete.id);

            currentNodeGraph.nodeList.Remove(nodeToDelete);

            DestroyImmediate(nodeToDelete, true);

            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// 删除与所选节点相关联的所有链接线
    /// </summary>
    private void DeleteSelectedNodeLinks()
    {
        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            if (node.isSelected && node.childrenNodeIdList.Count > 0)
            {
                for (int i = node.childrenNodeIdList.Count - 1; i >= 0; i--)
                {
                    NodeSO childNode = currentNodeGraph.GetNode(node.childrenNodeIdList[i]);

                    if (childNode != null && childNode.isSelected)
                    {
                        node.RemoveChildNodeIDFromNode(childNode.id);

                        childNode.RemoveParentNodeIDFromNode(node.id);
                    }
                }
            }
        }

        ClearAllSelectedNodes();
    }

    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param name="mousePositionObject">节点创建位置</param>
    private void CreateNode(object mousePositionObject,NodeTypeSO nodeType)
    {
        if (currentNodeGraph.nodeList.Count == 0)
        {
            CreateNodes(new Vector2(200,200),nodeTypeList.list.Find(x => x.isEntrance));
            CreateNodes(new Vector2(200,200),nodeTypeList.list.Find(x => x.isExit));
        }

        CreateNodes(mousePositionObject,nodeType);
    }

    /// <summary>
    /// 创建并初始化节点
    /// </summary>
    /// <param name="mousePositionObject">节点创建位置</param>
    /// <param name="nodeType">节点类型</param>
    private void CreateNodes(object mousePositionObject, NodeTypeSO nodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        GetNodeTypeSO(nodeType, mousePosition);

        AssetDatabase.SaveAssets();

        currentNodeGraph.OnValidate();
    }

    /// <summary>
    /// 根据所创建的节点类型创建对应的节点序列对象
    /// </summary>
    private void GetNodeTypeSO(NodeTypeSO nodeType, Vector2 mousePosition)
    {
        if (nodeType.isDefault || nodeType.isEntrance || nodeType.isExit)
        {
            DefaultNodeSO node = ScriptableObject.CreateInstance<DefaultNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isLocked)
        {
            LockedNodeSO node = ScriptableObject.CreateInstance<LockedNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isAI)
        {
            AILockedNodeSO node = ScriptableObject.CreateInstance<AILockedNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isAngleLock)
        {
            AngleLockNodeSO node = ScriptableObject.CreateInstance<AngleLockNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isGraph)
        {
            GraphNodeSO node = ScriptableObject.CreateInstance<GraphNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isSynthetic)
        {
            SynthesizableNodeSO node = ScriptableObject.CreateInstance<SynthesizableNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isProbe)
        {
            ProbeNodeSO node = ScriptableObject.CreateInstance<ProbeNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isControllable)
        {
            ControllableNodeSO node = ScriptableObject.CreateInstance<ControllableNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isSyntheticPicture)
        {
            SyntheticPictureNodeSO node = ScriptableObject.CreateInstance<SyntheticPictureNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isTiming)
        {
            TimingNodeSO node = ScriptableObject.CreateInstance<TimingNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isQuickClick)
        {
            QuickClickNodeSO node = ScriptableObject.CreateInstance<QuickClickNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isQTE)
        {
            QTENodeSO node = ScriptableObject.CreateInstance<QTENodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        else if (nodeType.isDialog)
        {
            DialogNodeSO node = ScriptableObject.CreateInstance<DialogNodeSO>();
            currentNodeGraph.nodeList.Add(node);
            node.Initialize(new Rect(mousePosition,new Vector2(nodeWidth,nodeHeight)),currentNodeGraph,nodeType);
            AssetDatabase.AddObjectToAsset(node,currentNodeGraph);
        }
        
    }

    /// <summary>
    /// 取消选择所有选择的节点
    /// </summary>
    private void ClearAllSelectedNodes()
    {
        foreach (NodeSO roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }
#endregion 鼠标按键按下

    /// <summary>
    /// 绘制节点图内包含的所有节点
    /// </summary>
    private void DrawNodes()
    {
        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            if (node.isSelected)
            {
                node.Draw(nodeSelectedStyle);
            }
            else
            {
                node.Draw(nodeStyle);
            }
        }

        GUI.changed = true;
    }

    /// <summary>
    /// 建立连接并绘制线条
    /// </summary>
     private void DrawConnections()
    {
        foreach (NodeSO Node in currentNodeGraph.nodeList)
        {
            if (Node.childrenNodeIdList.Count > 0)
            {
                foreach (string childNodeID in Node.childrenNodeIdList)
                {
                    if (currentNodeGraph.nodeDictionary.ContainsKey(childNodeID))
                    {
                        DrawConnectionLine(Node, currentNodeGraph.nodeDictionary[childNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 绘制线条
    /// </summary>
    private void DrawConnectionLine(NodeSO parentNode, NodeSO childNode)
    {
        Vector2 startPosition = parentNode.rect.center;
        Vector2 endPosition = childNode.rect.center;

        #region 绘制箭头
        Vector2 midPosition = (startPosition + endPosition) / 2;

        Vector2 direction = endPosition - startPosition;

        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);
        #endregion 绘制箭头

        Handles.DrawBezier(startPosition,endPosition,startPosition,endPosition,Color.white,null,connectingLineWidth);

        GUI.changed = true;
    }

    /// <summary>
    /// 编辑器内选择发生改变事件
    /// </summary>
    private void InspectorSelectionChanged()
    {
        NodeGraphSO NodeGraph = Selection.activeObject as NodeGraphSO;

        if (NodeGraph != null)
        {
            currentNodeGraph = NodeGraph;
            GUI.changed = true;
        }
    }
}
