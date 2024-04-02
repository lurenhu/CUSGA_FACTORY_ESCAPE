using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextNodeSO : NodeSO
{
    [Space(10)]
    [Header("文本节点")]
    [Tooltip("文本内容(支持HTML格式会让文本更加华丽优雅)")]
    public string text;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "TextNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
