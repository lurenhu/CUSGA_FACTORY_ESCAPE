using System;
using UnityEngine;

public class QuickClickNodeSO : NodeSO
{
    [Space(10)]
    [Header("快速点击节点所需数据")]
    [Tooltip("点击次数")]
    public int ClickNumber;
    
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "QuickClick";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
