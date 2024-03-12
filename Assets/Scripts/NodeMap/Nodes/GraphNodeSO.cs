using System;
using UnityEngine;

public class GraphNodeSO : NodeSO
{
    [Space(10)]
    [Header("图片节点所需数据")]
    [Tooltip("需要展示的图片")]
    public Sprite image;
    
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "GraphNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
