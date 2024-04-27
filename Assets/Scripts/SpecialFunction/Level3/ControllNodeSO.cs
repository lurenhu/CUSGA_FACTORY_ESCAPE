using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControllNodeSO : NodeSO
{
    [Space(5)]
    [Header("移动节点参数")]
    [Tooltip("当该节点的锤子撞击目标节点是触发事件")]
    public string targetNodeID;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "ControlNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
