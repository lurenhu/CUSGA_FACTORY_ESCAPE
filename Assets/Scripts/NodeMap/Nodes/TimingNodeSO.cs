using System;
using UnityEngine;

public class TimingNodeSO : NodeSO
{
    [Space(10)]
    [Header("计时器节点所需数据")]
    public string targetIdForStop;
    public float duration;

    #if UNITY_EDITOR
    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        nodeText = "计时器";

        this.name = "Timing";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }
    #endif
}
