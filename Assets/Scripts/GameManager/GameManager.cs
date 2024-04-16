using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum GameState
{
    Start,
    Generating,
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
    public List<NodeGraphSO> nodeGraphSOs;
    private int nodeLevelIndex = 0;// 关卡索引
    private int nodeGraphIndex = 0;// 节点图索引
    private List<List<string>> nodeIdsInGraph = new List<List<string>>(); // 对应节点图索引的节点ID列表，用于存取节点状态数据

    [Space(10)]
    [Header("游戏状态参数")]
    public GameState gameState = GameState.Start;


    [HideInInspector] public bool haveNodeDrag = false;

    override protected void Awake() {
        base.Awake();
    }
    
    private void Start() {
        NodeMapBuilder.Instance.GenerateNodeMap(nodeGraphSOs[nodeGraphIndex]);
    }   

    private void Update() {
        HandleGameState();
    }

    private void HandleGameState() {
        switch (gameState) {
            case GameState.Start:
                break;
            case GameState.Generating:
                InitializeNodeIdList();
                NodeMapBuilder.Instance.GenerateNodeMap(nodeLevelSOs[nodeLevelIndex].levelGraphs[nodeGraphIndex]);
                gameState = GameState.Playing;
                break;
            case GameState.Playing:
                break;
            case GameState.Pause:
                break;
            case GameState.End:
                break;
        }
    }

    /// <summary>
    /// 初始化关卡内节点图的节点ID列表
    /// </summary>
    private void InitializeNodeIdList()
    {
        if (nodeLevelSOs.Count == 0) return;

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
        // 保存当前节点图的节点数据
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[nodeGraphIndex]);

        // 节点图索引增加
        nodeGraphIndex++;
        if (nodeGraphIndex >= nodeLevelSOs.Count) {
            nodeGraphIndex = 0;
        }

        // 生成当前索引的节点图
        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(nodeLevelSOs[nodeLevelIndex].levelGraphs[nodeGraphIndex]);

        // 根据读取的节点状态数据重新载入节点图
        NodeMapBuilder.Instance.LoadNodeMap(nodeIdsInGraph[nodeGraphIndex]);
    }

    /// <summary>
    /// 向左转换节点图，Index--
    /// </summary>
    public void GetLeftNodeGraph() {
        // 保存当前节点图的节点数据
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[nodeGraphIndex]);

        // 节点图索引减少
        nodeGraphIndex--;
        if (nodeGraphIndex < 0) {
            nodeGraphIndex = nodeLevelSOs.Count - 1;
        }

        // 生成当前索引的节点图
        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(nodeLevelSOs[nodeLevelIndex].levelGraphs[nodeGraphIndex]);
        
        // 根据读取的节点状态数据重新载入节点图
        NodeMapBuilder.Instance.LoadNodeMap(nodeIdsInGraph[nodeGraphIndex]);
    }


}
