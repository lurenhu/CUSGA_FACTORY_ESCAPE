using System;
using UnityEngine;

[Serializable]
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class QTENodeSO : NodeSO
{
    [Space(10)]
    [Header("QTE节点所需数据")]
    [Tooltip("方向")]
    public Direction direction;
    public float dragDistance;
    
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "QTE";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
