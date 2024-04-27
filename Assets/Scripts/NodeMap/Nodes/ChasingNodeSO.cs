using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChasingNodeSO : NodeSO
{
    [Space(10)]
    [Header("追逐节点参数")]
    [Tooltip("追逐目标")]
    public string chasingTargetId;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "ChasingNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
