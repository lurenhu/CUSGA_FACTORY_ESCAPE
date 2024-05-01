using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogNodeSO : NodeSO
{
    [Space(10)]
    [Header("对话节点所需数据")]
    [Tooltip("对话文本列表")]
    public List<TextAsset> textAssets = new List<TextAsset>();
    [Tooltip("是否在对话结束后替换其子节点")]
    public bool stopAfterDialog = false;
    public string DisplayNodeID;
    public Sprite changeBackGroundImage;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "DialogNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
