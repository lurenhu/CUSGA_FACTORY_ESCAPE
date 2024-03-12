using System;
using System.Collections.Generic;
using UnityEngine;

public class AngleLockNodeSO : NodeSO
{
    [Space(10)]
    [Header("角度锁节点数据")]
    [Tooltip("解开角度锁的角度列表")]
    public List<float> angles = new List<float>();
    
    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "AngleLockNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
