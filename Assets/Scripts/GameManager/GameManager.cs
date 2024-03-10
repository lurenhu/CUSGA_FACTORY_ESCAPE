using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Tooltip("节点图列表")]
    public List<NodeGraphSO> nodeGraphSOs;

    public Transform Canvas;
    public Transform graphTransform;

    override protected void Awake() {
        base.Awake();
        
        Canvas = GameObject.FindWithTag("Canvas").transform;    
    }
    
    private void Start() {
        NodeMapBuilder.Instance.GenerateNodeMap(nodeGraphSOs[0]);
    }

    public void CloseGraph()
    {
        graphTransform.gameObject.SetActive(false);
    }
}
