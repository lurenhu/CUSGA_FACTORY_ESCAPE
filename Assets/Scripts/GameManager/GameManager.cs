using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public enum GameState
{
    Start,
    Generating,
    Playing,
    Pause,
    Won,
    Fake,
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
    public float tweenDuring = 0.2f;// 弹出持续时间
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
    Coroutine ChangeNodeGraph;
    Coroutine GameSceneChanged;
    Coroutine ResultCoroutine;

    [Space(10)]
    [Header("场景过度")]
    public CanvasGroup canvasGroup;

    [Space(10)]
    [Header("游戏状态参数")]
    public GameState gameState = GameState.Start;
    [HideInInspector] public int level1GetResultTimes = 0;
    [HideInInspector] public bool haveNodeDrag = false;

    [Space(10)]
    [Header("游戏结局过场动画")]
    public List<CutSceneCell> winCutScene;
    public List<CutSceneCell> fakeCutScene;

    override protected void Awake() {
        base.Awake();

        SceneManager.LoadScene("MainMenu",LoadSceneMode.Additive);
        StartCoroutine(Fade(1,0,2,Color.black));
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable() {
        StaticEventHandler.OnGetNextNodeLevel += StaticEventHandler_OnGetNextNodeLevel;

        StaticEventHandler.OnGetResult += StaticEventHandler_OnGetFailResult;
    }

    private void OnDisable() {
        StaticEventHandler.OnGetNextNodeLevel -= StaticEventHandler_OnGetNextNodeLevel;

        StaticEventHandler.OnGetResult -= StaticEventHandler_OnGetFailResult;
    }

    private void StaticEventHandler_OnGetFailResult(GetResult result)
    {
        if (ResultCoroutine != null)
        {
            StopCoroutine(ResultCoroutine);
        }
        ResultCoroutine = StartCoroutine(GetResult(result.cutSceneCells));
    }

    IEnumerator GetResult(List<CutSceneCell> cutSceneCells)
    {
        canvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(0,1,2,Color.black));

        soundManager.Instance.StopMusicInFade();
        soundManager.Instance.PlaySFX("ChangeScene");

        VideoManager.Instance.ShowCutScenes(cutSceneCells);

        canvasGroup.blocksRaycasts = false;
        yield return StartCoroutine(Fade(1,0,2,Color.black));
    }

    private void StaticEventHandler_OnGetNextNodeLevel(GetNextNodeLevel args)
    {
        Debug.Log("触发进入下一关卡事件");
        if (GameSceneChanged != null)
        {
            StopCoroutine(GameSceneChanged);
        }
        GameSceneChanged = StartCoroutine(GetNextNodeLevel());
    }

    IEnumerator GetNextNodeLevel()
    {
        canvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(0,1,2,Color.black));

        soundManager.Instance.StopMusicInFade();
        soundManager.Instance.PlaySFX("ChangeScene");

        levelIndex++;
        gameState = GameState.Generating;

        canvasGroup.blocksRaycasts = false;
        yield return StartCoroutine(Fade(1,0,2,Color.black));

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
                VideoManager.Instance.ShowCutScenes(winCutScene);
                break;
            case GameState.Fake:
                VideoManager.Instance.ShowCutScenes(fakeCutScene);
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

        InitializeReference(currentNodeLevel);

        maxAnxiety = currentNodeLevel.initialAnxietyValue;
        currentAnxiety = maxAnxiety;
        rate = currentNodeLevel.rate;

        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(currentNodeGraph,enterNodeGraphTimesList[graphIndex]);
        enterNodeGraphTimesList[graphIndex]++;

        if (levelIndex == 6)
        {
            UIManager.Instance.SkyUI.gameObject.SetActive(true);
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

        MatchRightAndLeftNodeGraphName(currentNodeLevel);

        if (currentNodeLevel.cutSceneList.Count > 0)
        {
            VideoManager.Instance.ShowCutScenes(currentNodeLevel.cutSceneList);
        }
    }

    /// <summary>
    /// 匹配左右两侧的切换节点图按钮
    /// </summary>
    private void MatchRightAndLeftNodeGraphName(NodeLevelSO currentNodeLevel)
    {
        if (currentNodeLevel.levelGraphs.Count > 1)
        {
            UIManager.Instance.rightNodeGraphButton.gameObject.SetActive(true);
            UIManager.Instance.leftNodeGraphButton.gameObject.SetActive(true);
        }
        else
        {
            UIManager.Instance.rightNodeGraphButton.gameObject.SetActive(false);
            UIManager.Instance.leftNodeGraphButton.gameObject.SetActive(false);
        }

        int rightGraphIndex = graphIndex + 1;
        if (rightGraphIndex >= currentNodeLevel.levelGraphs.Count)
        {
            rightGraphIndex = 0;
        }
        UIManager.Instance.rightNodeGraphButton.GetComponentInChildren<TMP_Text>().text = currentNodeLevel.levelGraphs[rightGraphIndex].graphName;

        int leftGraphIndex = graphIndex - 1;
        if (leftGraphIndex < 0)
        {
            leftGraphIndex = currentNodeLevel.levelGraphs.Count - 1;
        }
        UIManager.Instance.leftNodeGraphButton.GetComponentInChildren<TMP_Text>().text = currentNodeLevel.levelGraphs[leftGraphIndex].graphName;

        UIManager.Instance.rightNodeGraphButton.GetComponent<Button>().onClick.AddListener(ChangeToRightGraph);
        UIManager.Instance.leftNodeGraphButton.GetComponent<Button>().onClick.AddListener(ChangeToLeftGraph);
    }
    
    private void ChangeToRightGraph()
    {
        if (ChangeNodeGraph != null)
        {
            StopCoroutine(ChangeNodeGraph);
        }

        ChangeNodeGraph = StartCoroutine(ChangeToRightGraphCoroutine());
    }

    IEnumerator ChangeToRightGraphCoroutine()
    {
        yield return StartCoroutine(Fade(0,1,2,Color.black));

        soundManager.Instance.PlaySFX("ChangeScene");

        GetRightNodeGraph();

        yield return StartCoroutine(Fade(1,0,2,Color.black));
    }

    /// <summary>
    /// 向右转换节点图，index++
    /// </summary>
    private void GetRightNodeGraph() {
        NodeLevelSO currentNodeLevel = nodeLevelSOs[levelIndex];
        // 保存当前节点图的节点数据
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[graphIndex]);

        // 节点图索引增加
        graphIndex++;
        if (graphIndex >= currentNodeLevel.levelGraphs.Count) {
            graphIndex = 0;
        }
        
        MatchRightAndLeftNodeGraphName(currentNodeLevel);

        // 生成当前索引的节点图
        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(
            currentNodeLevel.levelGraphs[graphIndex],
            enterNodeGraphTimesList[graphIndex]
            );
        enterNodeGraphTimesList[graphIndex]++;

        // 根据读取的节点状态数据重新载入节点图
        if (enterNodeGraphTimesList[graphIndex] != 1)
            NodeMapBuilder.Instance.LoadNodeMap(nodeIdsInGraph[graphIndex]);
    }

    private void ChangeToLeftGraph()
    {
        if (ChangeNodeGraph != null)
        {
            StopCoroutine(ChangeNodeGraph);
        }

        ChangeNodeGraph = StartCoroutine(ChangeToLeftGraphCoroutine());
    }

    IEnumerator ChangeToLeftGraphCoroutine()
    {
        yield return StartCoroutine(Fade(0,1,2,Color.black));

        soundManager.Instance.PlaySFX("ChangeScene");

        GetLeftNodeGraph();

        yield return StartCoroutine(Fade(1,0,2,Color.black));
    }

    /// <summary>
    /// 向左转换节点图，Index--
    /// </summary>
    private void GetLeftNodeGraph() {
        NodeLevelSO currentNodeLevel = nodeLevelSOs[levelIndex];
        // 保存当前节点图的节点数据
        NodeMapBuilder.Instance.SaveNodeMap(nodeIdsInGraph[graphIndex]);

        // 节点图索引减少
        graphIndex--;
        if (graphIndex < 0) {
            graphIndex = currentNodeLevel.levelGraphs.Count - 1;
        }

        MatchRightAndLeftNodeGraphName(currentNodeLevel);

        // 生成当前索引的节点图
        NodeMapBuilder.Instance.DeleteNodeMap();
        NodeMapBuilder.Instance.GenerateNodeMap(
            currentNodeLevel.levelGraphs[graphIndex],
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

    /// <summary>
    /// 淡入淡出
    /// </summary>
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backGround)
    {
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backGround;

        float time = 0;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time/fadeSeconds);
            yield return null;
        }
    }

    public IEnumerator ChangeSceneToMainMenu()
    {
        canvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(0,1,2,Color.black));

        SceneManager.LoadSceneAsync("MainMenu",LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("GameScene");

        yield return StartCoroutine(Fade(1,0,2,Color.black));
        canvasGroup.blocksRaycasts = false;
    }

    public IEnumerator ChangeSceneToGameScene()
    {
        canvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(0,1,2,Color.black));

        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadSceneAsync("GameScene",LoadSceneMode.Additive);

        soundManager.Instance.StopMusicInFade();
        soundManager.Instance.PlaySFX("ChangeScene");

        levelIndex = 0;
        gameState = GameState.Generating;

        canvasGroup.blocksRaycasts = false;
        yield return StartCoroutine(Fade(1,0,2,Color.black));
    }

    public void PlayCurrentLevelAudio()
    {
        NodeLevelSO currentLevel = nodeLevelSOs[levelIndex];

        soundManager.Instance.StopMusicInFade();
        soundManager.Instance.PlayMusicInFade(currentLevel.audioClip);

        StartCoroutine(Fade(1,0,2,Color.black));
    }

}
