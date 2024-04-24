using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NodeLevel_",menuName = "ScriptableObjects/NodeLevel")]
public class NodeLevelSO : ScriptableObject
{
    public string levelName;
    public List<NodeGraphSO> levelGraphs;

    [Space(5)]
    [Header("AI参数")]
    public float initialAnxietyValue;
    public float rate;

    [Space(5)]
    [Header("进入该关卡时播放过场演出")]
    [SerializeField] public List<CutSceneCell> cutSceneList;

    [Space(5)]
    [Header("线性叙述相关参数")]
    [Tooltip("是否在第一次进入该关卡时关闭关卡内节点图之间的转换")]
    public bool canNotTransitionForFirstTimes;

}
