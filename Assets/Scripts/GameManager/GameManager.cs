using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Start,
    Playing,
    Pause,
    End
}

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
    public List<NodeLevelSO> nodeLevelSOs;
    private int nodeLevelIndex = 0;// 关卡索引
    private int nodeGraphIndex = 0;// 节点图索引
    private List<List<string>> nodeIdsInGraph = new List<List<string>>(); // 对应节点图索引的节点ID列表

    
    [HideInInspector] public bool haveNodeDrag = false;

    override protected void Awake() {
        base.Awake();
    }
    
    private void Start() {
        InitializeNodeList();

        NodeMapBuilder.Instance.GenerateNodeMap(nodeLevelSOs[nodeLevelIndex].levelGraphs[nodeGraphIndex]);

    }

    private void Update() {
        
    }

    private void InitializeNodeList()
    {
        nodeIdsInGraph.Clear();

        foreach (NodeGraphSO nodeGraph in nodeLevelSOs[nodeLevelIndex].levelGraphs)
        {
            nodeIdsInGraph.Add(new List<string>());
        }

    }

    /// <summary>
    /// 向右转换节点图，index++
    /// </summary>
    public void GetRightNodeGraph() {
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[nodeGraphIndex]);

        nodeGraphIndex++;
        if (nodeGraphIndex >= nodeLevelSOs.Count) {
            nodeGraphIndex = 0;
        }

        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(nodeLevelSOs[nodeLevelIndex].levelGraphs[nodeGraphIndex]);

        NodeMapBuilder.Instance.LoadNodeMap(nodeIdsInGraph[nodeGraphIndex]);
    }

    /// <summary>
    /// 向左转换节点图，Index--
    /// </summary>
    public void GetLeftNodeGraph() {
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[nodeGraphIndex]);

        nodeGraphIndex--;
        if (nodeGraphIndex < 0) {
            nodeGraphIndex = nodeLevelSOs.Count - 1;
        }

        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(nodeLevelSOs[nodeLevelIndex].levelGraphs[nodeGraphIndex]);
        
        NodeMapBuilder.Instance.LoadNodeMap(nodeIdsInGraph[nodeGraphIndex]);
    }


}
