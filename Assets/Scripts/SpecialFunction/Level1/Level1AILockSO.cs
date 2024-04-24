using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Level1AILockSO : NodeSO
{
    [Space(10)]
    [Header("Level1AILock的参数")]
    [Tooltip("AI对话次数")]
    public int submissionTimes;
    public List<CutSceneCell> firstFailResult;
    public List<CutSceneCell> secondFailResult;

    #if UNITY_EDITOR

    public void Initialize(Rect rect, NodeGraphSO nodeGraph, NodeTypeSO NodeType)
    {
        this.name = "Level1AILock";
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.nodeGraph = nodeGraph;
        this.nodeType = NodeType;

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    #endif
}
