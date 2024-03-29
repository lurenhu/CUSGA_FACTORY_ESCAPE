using System;
using UnityEngine;

public class ControllableNodeSO : NodeSO
{
    [Space(10)]
    [Header("可控制节点数据")]
    [Tooltip("当控制节点的速度达到该值并与被碰撞节点发生碰撞时弹出被碰撞节点的子节点")]
    public float speedToPop = 10f;
    [Tooltip("被碰撞节点ID")]
    public string collidedNodeId = Setting.stringDefaultValue;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "ControllableNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
