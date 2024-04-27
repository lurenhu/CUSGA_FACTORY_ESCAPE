using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerToResultNodeSO : NodeSO
{
    [Space(10)]
    [Header("计时器触发结局节点所需数据")]
    public string startNodeId;
    public string stopNodeId;
    public float duration;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "ChangeSceneNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
