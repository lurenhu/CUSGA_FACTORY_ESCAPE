using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeSO : ScriptableObject
{
    public string nodeText;
    public string id;
    public List<string> parentNodeIdList = new List<string>();
    public List<string> childrenNodeIdList = new List<string>();
    [HideInInspector] public NodeGraphSO nodeGraph;
    [HideInInspector] public NodeTypeSO nodeType;
    [HideInInspector] public NodeTypeListSO nodeTypeList;

    #if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

#region 数据初始化
    public void Initialise(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "Node";
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    /// <summary>
    /// 将子节点的ID添加至子节点列表中
    /// </summary>
    /// <param name="childID">子节点ID</param>
    /// <returns>是否成功添加</returns>
    public bool AddChildNodeIDToNode(string childID)
    {
        if (!childrenNodeIdList.Contains(childID) && this.id != childID)
        {
            childrenNodeIdList.Add(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断子节点列表中是否已经存在该ID的子节点
    /// </summary>
    /// <param name="childID">子节点ID</param>
    /// <returns>true：该子节点可添加，false：反之</returns>
    public bool isChildNodeValid(string childID)
    {
        foreach (string id in childrenNodeIdList)
        {
            if (id == childID) return false;
        }
        return true;
    }

    /// <summary>
    /// 确定当前节点的父节点
    /// </summary>
    /// <param name="parentID">父节点ID</param>
    /// <returns>true：添加至父节点，false：反之</returns>
    public bool AddParentNodeIDToNode(string parentID)
    {
        if (!parentNodeIdList.Contains(parentID) && this.id != parentID && parentNodeIdList.Count < 1)
        {
            parentNodeIdList.Add(parentID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除子节点列表中的子节点
    /// </summary>
    /// <param name="childID">子节点ID</param>
    public bool RemoveChildNodeIDFromNode(string childID)
    {
        if (childrenNodeIdList.Contains(childID))
        {
            childrenNodeIdList.Remove(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除父节点列表中的父节点
    /// </summary>
    /// <param name="parentID">父节点ID</param>
    public bool RemoveParentNodeIDFromNode(string parentID)
    {
        if (parentNodeIdList.Contains(parentID))
        {
            parentNodeIdList.Remove(parentID);
            return true;
        }
        return false;
    }
#endregion 数据初始化

#region 绘制节点
    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect,nodeStyle);

        EditorGUI.BeginChangeCheck();

        if (parentNodeIdList.Count > 0)
        {
            EditorGUILayout.LabelField(nodeType.nodeTypeName + "_" + nodeText);
        }
        else
        {
            int selected = nodeTypeList.list.FindIndex(x => x == nodeType);

            int selection = EditorGUILayout.Popup("",selected,GetNodeTypesDisplay());
            this.nodeText = EditorGUILayout.TextField("","nodeText");

            nodeType = nodeTypeList.list[selection];
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    /// <summary>
    /// 获取所有节点类型的string数据
    /// </summary>
    private string[] GetNodeTypesDisplay()
    {
        string[] nodeArray = new string[nodeTypeList.list.Count];

        for (int i = 0; i < nodeTypeList.list.Count; i++)
        {
            nodeArray[i] = nodeTypeList.list[i].nodeTypeName;
        }

        return nodeArray;
    }
#endregion 绘制节点

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
#region 鼠标按键按下
    /// <summary>
    /// 鼠标按键按下事件
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    /// <summary>
    /// 鼠标左键按下事件
    /// </summary>
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        isSelected = !isSelected;
    }

    /// <summary>
    /// 鼠标右键按下事件
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        nodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }
#endregion 鼠标按键按下

#region  鼠标按键抬起
    /// <summary>
    /// 鼠标按键抬起事件
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// <summary>
    /// 鼠标左键按键抬起事件
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }
#endregion 鼠标按键抬起

#region 鼠标拖拽
    /// <summary>
    /// 鼠标拖拽事件
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessMouseLeftDragEvent(currentEvent);
        }
    }

    /// <summary>
    /// 鼠标左键拖拽事件
    /// </summary>
     private void ProcessMouseLeftDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    /// <summary>
    /// 拖拽节点
    /// </summary>
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
#endregion 鼠标拖拽

    

    


    #endif
}
