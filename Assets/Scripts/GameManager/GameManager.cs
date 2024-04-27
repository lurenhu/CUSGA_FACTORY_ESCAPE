using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
    Won,
    Fail,
}

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Header("AI参数")]
    [Tooltip("AI当前焦虑值")]
    public float currentAnxiety;
    [Tooltip("AI的最大焦虑值")]
    public float maxAnxiety;
    [Tooltip("AI解锁成功条件(胜利概率)")]
    public float rate;

    [Space(10)]
    [Header("弹出动画参数")]
    [Tooltip("弹出动画的持续时间")]
    public float tweenDuring = 0.5f;// 弹出持续时间
    [Tooltip("弹出动画的弹出距离")]
    public float popUpForce = 3;// 弹出距离

    [Space(10)]
    [Header("节点图参数")]
    [Tooltip("所需生成节点图列表")]
    public List<NodeLevelSO> nodeLevelSOs;
    [Tooltip("进入对应索引节点图的次数")]
    private List<int> enterNodeGraphTimesList = new List<int>();
    [SerializeField] public int levelIndex = 0;// 关卡索引
    [SerializeField] private int graphIndex = 0;// 节点图索引
    private List<List<string>> nodeIdsInGraph = new List<List<string>>(); // 对应节点图索引的节点ID列表，用于存取节点状态数据

    [Space(10)]
    [Header("游戏状态参数")]
    public GameState gameState = GameState.Start;

    public int level1GetResultTimes = 0;

    [HideInInspector] public bool haveNodeDrag = false;

    override protected void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start() {
    }   

    private void Update() {
        HandleGameState();
    }

    private void HandleGameState() {
        switch (gameState) {
            case GameState.Start:
                break;
            case GameState.Generating:
                if (NodeMapBuilder.Instance == null) return;

                GetGenerateNodeMap();                

                gameState = GameState.Playing;
                break;
            case GameState.Playing:
                break;
            case GameState.Pause:
                break;
            case GameState.Won:
                break;
            case GameState.Fail:
                break;
        }
    }

    /// <summary>
    /// 生成节点图并初始化相关参数
    /// </summary>
    private void GetGenerateNodeMap()
    {
        NodeLevelSO currentNodeLevel = nodeLevelSOs[levelIndex];
        NodeGraphSO currentNodeGraph = currentNodeLevel.levelGraphs[graphIndex];

        if (currentNodeLevel == null) return;
        
        GetCutScene(currentNodeLevel);
        InitializeReference(currentNodeLevel);

        maxAnxiety = currentNodeLevel.initialAnxietyValue;
        currentAnxiety = maxAnxiety;
        rate = currentNodeLevel.rate;

        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(currentNodeGraph,enterNodeGraphTimesList[graphIndex]);
        enterNodeGraphTimesList[graphIndex]++;

        if (currentNodeLevel.canNotTransitionForFirstTimes)
        {
            UIManager.Instance.leftNodeGraphButton.gameObject.SetActive(false);
            UIManager.Instance.rightNodeGraphButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 初始化相关参数与函数回调
    /// </summary>
    private void InitializeReference(NodeLevelSO currentNodeLevel)
    {
        nodeIdsInGraph.Clear();
        enterNodeGraphTimesList.Clear();

        foreach (NodeGraphSO nodeGraph in currentNodeLevel.levelGraphs)
        {
            nodeIdsInGraph.Add(new List<string>());
            enterNodeGraphTimesList.Add(0);
        }

        UIManager.Instance.rightNodeGraphButton.GetComponent<Button>().onClick.AddListener(GetRightNodeGraph);
        UIManager.Instance.leftNodeGraphButton.GetComponent<Button>().onClick.AddListener(GetLeftNodeGraph);
    }
    
    /// <summary>
    /// 获取过场动画
    /// </summary>
    private void GetCutScene(NodeLevelSO currentNodeLevel)
    {
        if (currentNodeLevel.cutSceneList.Count > 0)
        {
            VideoManager.Instance.ShowCutScenes(currentNodeLevel.cutSceneList);
        }
    }

    /// <summary>
    /// 向右转换节点图，index++
    /// </summary>
    public void GetRightNodeGraph() {
        // 保存当前节点图的节点数据
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[graphIndex]);

        // 节点图索引增加
        graphIndex++;
        if (graphIndex >= nodeLevelSOs[levelIndex].levelGraphs.Count) {
            graphIndex = 0;
        }

        // 生成当前索引的节点图
        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(
            nodeLevelSOs[levelIndex].levelGraphs[graphIndex],
            enterNodeGraphTimesList[graphIndex]
            );
        enterNodeGraphTimesList[graphIndex]++;

        // 根据读取的节点状态数据重新载入节点图
        if (enterNodeGraphTimesList[graphIndex] != 1)
            NodeMapBuilder.Instance.LoadNodeMap(nodeIdsInGraph[graphIndex]);
    }

    /// <summary>
    /// 向左转换节点图，Index--
    /// </summary>
    public void GetLeftNodeGraph() {
        // 保存当前节点图的节点数据
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[graphIndex]);

        // 节点图索引减少
        graphIndex--;
        if (graphIndex < 0) {
            graphIndex = nodeLevelSOs[levelIndex].levelGraphs.Count - 1;
        }

        // 生成当前索引的节点图
        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(
            nodeLevelSOs[levelIndex].levelGraphs[graphIndex],
            enterNodeGraphTimesList[graphIndex]
            );
        enterNodeGraphTimesList[graphIndex]++;
        
        // 根据读取的节点状态数据重新载入节点图
        if (enterNodeGraphTimesList[graphIndex] != 1)
            NodeMapBuilder.Instance.LoadNodeMap(nodeIdsInGraph[graphIndex]);
    }

    /// <summary>
    /// 检查当前的焦虑值是否处在焦虑值比例内
    /// </summary>
    public bool CheckAnxietyValue()
    {
        if (currentAnxiety/maxAnxiety < rate)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
