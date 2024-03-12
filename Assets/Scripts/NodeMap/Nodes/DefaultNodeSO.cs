using System;
using UnityEngine;

public class DefaultNodeSO : NodeSO
{
    // this is DefaultNode
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "DefaultNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
