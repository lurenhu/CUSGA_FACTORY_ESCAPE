using System;
using UnityEngine;

public class AILockedNodeSO : NodeSO
{
    [Space(10)]
    [Header("AI锁节点数据")]
    [Tooltip("开始的焦虑值")]
    public float anxietyValue;
    [Tooltip("AI锁解锁成功比例")]
    public float rate;
    [Tooltip("可提交次数")]
    public int submissionTimes;


    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "AILockedNode";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
