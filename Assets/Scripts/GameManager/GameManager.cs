using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonMonobehaviour<GameManager>
{

    [Space(10)]
    [Header("动画参数")]
    [Tooltip("弹出动画的持续时间")]
    public float tweenDuring = 0.5f;// 弹出持续时间
    [Tooltip("弹出动画的弹出距离")]
    public float popUpForce = 3;// 弹出距离

    [Space(10)]
    [Header("节点图参数")]
    [Tooltip("所需生成节点图列表")]
    public List<NodeGraphSO> nodeGraphSOs;

    
    [HideInInspector] public bool haveNodeDrag = false;

    override protected void Awake() {
        base.Awake();
    }
    
    private void Start() {
        NodeMapBuilder.Instance.GenerateNodeMap(nodeGraphSOs[0]);

        Debug.Log(Application.dataPath);
    }

    
}
