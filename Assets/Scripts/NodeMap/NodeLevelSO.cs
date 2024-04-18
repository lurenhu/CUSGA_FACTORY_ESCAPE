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
    [Header("过场演出")]
    public VideoClip videoClip;
    public List<GraphicsAndText> graphicsAndTextList;

}
