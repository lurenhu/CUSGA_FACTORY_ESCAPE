using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIAnxietyChangedSO : NodeSO
{
    [Space(10)]
    [Header("AI焦虑值变化节点参数")]
    [Tooltip("AI锁成功比例变化目标值")]
    public float changeRate;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "AIAnxietyChangedNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
