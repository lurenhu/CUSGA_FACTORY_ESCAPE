using System;
using UnityEngine;

public class SynthesizableNodeSO : NodeSO
{
    [Space(10)]
    [Header("合成节点所需数据")]
    [Tooltip("合成目标的节点ID")]
    public string targetIdForMerge = Setting.stringDefaultValue;
    
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "SynthesizableNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
