using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerToResult : MonoBehaviour
{
    [Header("观测参数")]
    public Node startNode;
    public Node myNode;
    private float duration = 1;
    private Coroutine timerCoroutine;

    private void Start() {
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {

            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }

            // 播放音频
            if (myNode.audios.Count != 0)
            {
                soundManager.Instance.PlayMusic(myNode.audios[0]);
            }
            // UIManager.Instance.StartDisplayNodeTextForShowRoutine(myNode.nodeTextForShow);
            UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }

    /// <summary>
    /// 传递计时器节点数据
    /// </summary>
    public void InitializeTimerToResult(NodeSO nodeSO)
    {
        TimerToResultNodeSO timingNodeSO = (TimerToResultNodeSO)nodeSO;

        duration = timingNodeSO.duration;
        TimeManager.Instance.startTimingNodeId = timingNodeSO.startNodeId;
        TimeManager.Instance.endTimingNodeId = timingNodeSO.stopNodeId;

        myNode = transform.GetComponent<Node>();
        TimeManager.Instance.timingNode = myNode;
    }

    public void StartTimerCoroutine()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(StartTimer());
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    private IEnumerator StartTimer()
    {
        float currentTime = duration; // 初始化当前时间为倒计时时间
        SetCurrentTimeText(currentTime);

        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f); // 等待一秒钟

            currentTime -= 1f; // 减去一秒钟

            SetCurrentTimeText(currentTime);
        }

        Debug.Log("时间到！");
        // 在此处执行计时结束后的操作
        StaticEventHandler.CallGetResult(GameManager.Instance.winCutScene);
        GameManager.Instance.levelIndex = -1;
        GameManager.Instance.gameState = GameState.Result;
    }

    private void SetCurrentTimeText(float currentTime)
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        transform.GetComponentInChildren<TMP_Text>().text = timerString;

        Debug.Log("剩余时间：" + timerString);
    }

}
