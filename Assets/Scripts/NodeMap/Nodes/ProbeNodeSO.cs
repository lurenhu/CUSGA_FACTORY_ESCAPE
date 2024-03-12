using System;
using UnityEngine;

public class ProbeNodeSO : NodeSO
{
    [Space(10)]
    [Header("探测节点数据")]
    [Tooltip("探测目标的节点ID")]
    public string targetIDForDetection = Setting.stringDefaultValue;
    
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "ProbeNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
