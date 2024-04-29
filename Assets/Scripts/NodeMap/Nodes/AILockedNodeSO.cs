using System;
using System.Collections.Generic;
using UnityEngine;

public class AILockedNodeSO : NodeSO
{
    [Space(10)]
    [Header("AI锁节点数据")]
    [Tooltip("可提交次数")]
    public int submissionTimes = 5;
    [Tooltip("AI对话失败时播放过场")]
    public List<CutSceneCell> failCutScene;


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
