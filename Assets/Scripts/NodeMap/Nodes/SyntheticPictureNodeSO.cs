using System;
using UnityEngine;

public class SyntheticPictureNodeSO : NodeSO
{
    [Space(10)]
    [Header("合成图片节点所需数据")]
    public Sprite image;
    public string targetIdForMerge;

    #if UNITY_EDITOR
    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "SyntheticPictureNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }
    #endif
}
