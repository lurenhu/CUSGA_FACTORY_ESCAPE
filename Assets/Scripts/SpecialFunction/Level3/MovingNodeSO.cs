using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingNodeSO : NodeSO
{
    [Space(5)]
    [Header("移动节点参数")]
    [Tooltip("触发条件:移动移动距离")]
    public float triggerDistance;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "MovingNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
