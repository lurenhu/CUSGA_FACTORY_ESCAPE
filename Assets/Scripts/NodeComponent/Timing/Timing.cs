using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Timing : MonoBehaviour
{
    [Header("观测参数")]
    public Node myNode;
    private float duration = 1;
    private Coroutine timerCoroutine;

    private void OnEnable() {
        StaticEventHandler.OnStopTiming += StaticEventHandler_OnStopTiming;
    }

    private void OnDisable() {
        StaticEventHandler.OnStopTiming -= StaticEventHandler_OnStopTiming;
    }

    public void StartTimerCoroutine()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(StartTimer());
    }

    private void StaticEventHandler_OnStopTiming(StopTimingArgs args)
    {
        if(gameObject.activeSelf)
        {
            Restart();
        }
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
    public void InitializeTiming(NodeSO nodeSO)
    {
        TimingNodeSO timingNodeSO = (TimingNodeSO)nodeSO;

        duration = timingNodeSO.duration;
        TimeManager.Instance.startTimingNodeId = timingNodeSO.startNodeId;
        TimeManager.Instance.endTimingNodeId = timingNodeSO.stopNodeId;

        myNode = transform.GetComponent<Node>();
        TimeManager.Instance.timingNode = myNode;
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
        Restart();
    }

    private void SetCurrentTimeText(float currentTime)
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        transform.GetComponentInChildren<TMP_Text>().text = timerString;

        Debug.Log("剩余时间：" + timerString);
    }

    /// <summary>
    /// 停止计时
    /// </summary>
    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        gameObject.SetActive(false);
    }


    /// <summary>
    /// 执行时间到事件
    /// </summary>
    private void Restart()
    {

        Queue<Node> tempNodeQueue = new Queue<Node>();

        NodeMapBuilder.Instance.GetNode(TimeManager.Instance.startTimingNode.parentID).hasPopUp = false;
        
        tempNodeQueue.Enqueue(TimeManager.Instance.startTimingNode);

        while (tempNodeQueue.Count > 0)
        {
            Node currentNode = tempNodeQueue.Dequeue();

            currentNode.hasPopUp = false;
            currentNode.gameObject.SetActive(false);
            LineCreator.Instance.DeleteLine(currentNode);

            foreach (string childNodeId in currentNode.childIdList)
            {
                Node childNode = NodeMapBuilder.Instance.nodeHasCreated[childNodeId];

                if (childNode == TimeManager.Instance.endTimingNode)
                {
                    break;
                }

                tempNodeQueue.Enqueue(childNode);
            }
        }

        StopTimer();
    }


}
