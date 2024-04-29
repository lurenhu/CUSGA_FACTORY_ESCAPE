using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NodeLevel_",menuName = "ScriptableObjects/NodeLevel")]
public class NodeLevelSO : ScriptableObject
{
    public string levelName;
    public List<NodeGraphSO> levelGraphs;
    [Tooltip("进入该关卡的动画")]
    public AudioClip audioClip;

    [Space(5)]
    [Header("AI参数")]
    public float initialAnxietyValue;
    [Range(0,1)]
    public float rate;

    [Space(5)]
    [Header("进入该关卡时播放过场演出")]
    [SerializeField] public List<CutSceneCell> cutSceneList;
}
