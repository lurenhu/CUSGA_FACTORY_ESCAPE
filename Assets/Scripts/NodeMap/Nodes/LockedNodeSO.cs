using System;
using System.Collections.Generic;
using UnityEngine;

public class LockedNodeSO : NodeSO
{
    [Space(10)]
    [Header("锁节点所需数据")]
    [Tooltip("解开密码锁的密码值列表")]
    public List<int> cipherValues = new List<int>();
    
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "LockedNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
