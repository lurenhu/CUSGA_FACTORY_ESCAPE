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
    public Node startNode;
    public Node stopNode;
    public Node myNode;
    private float duration = 1;
    private Coroutine timerCoroutine;

    private void Awake() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnEnable() {
        timerCoroutine = StartCoroutine(StartTimer());

        StaticEventHandler.OnStopTiming += StaticEventHandler_OnStopTiming;
    }

    private void OnDisable() {
        StopTimer();

        StaticEventHandler.OnStopTiming -= StaticEventHandler_OnStopTiming;
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

    private void StaticEventHandler_OnStopTiming(StopTimingArgs args)
    {
        if(gameObject.activeSelf)
        {
            TimeOut();
        }
    }

    /// <summary>
    /// 传递计时器节点数据
    /// </summary>
    public void InitializeTiming(NodeSO nodeSO)
    {
        TimingNodeSO timingNodeSO = (TimingNodeSO)nodeSO;

        duration = timingNodeSO.duration;
        startNode = NodeMapBuilder.Instance.nodeHasCreated[timingNodeSO.startNodeId];
        stopNode = NodeMapBuilder.Instance.nodeHasCreated[timingNodeSO.stopNodeId];

        BeClocked beClocked = startNode.gameObject.AddComponent<BeClocked>();
        beClocked.InitializeBeClocked(this.myNode);
        
        StopClocked stopClocked = stopNode.gameObject.AddComponent<StopClocked>();
        stopClocked.InitializeStopClocked(this.myNode);
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    private IEnumerator StartTimer()
    {
        float currentTime = duration; // 初始化当前时间为倒计时时间

        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f); // 等待一秒钟

            currentTime -= 1f; // 减去一秒钟

            // 计算分钟和秒数
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // 将分钟和秒数格式化成00:00的形式
            string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
            transform.GetComponentInChildren<TMP_Text>().text = timerString;

            Debug.Log("剩余时间：" + timerString);
        }

        Debug.Log("时间到！");
        // 在此处执行计时结束后的操作
        TimeOut();
    }

    /// <summary>
    /// 停止计时
    /// </summary>
    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 执行时间到事件
    /// </summary>
    private void TimeOut()
    {

        Queue<Node> tempNodeQueue = new Queue<Node>();

        NodeMapBuilder.Instance.nodeHasCreated[startNode.parentID].hasPopUp = false;
        
        tempNodeQueue.Enqueue(startNode);

        while (tempNodeQueue.Count > 0)
        {
            Node currentNode = tempNodeQueue.Dequeue();

            currentNode.hasPopUp = false;
            currentNode.gameObject.SetActive(false);
            LineCreator.Instance.DeleteLine(currentNode);

            foreach (string childNodeId in currentNode.childIdList)
            {
                Node childNode = NodeMapBuilder.Instance.nodeHasCreated[childNodeId];

                if (childNode == stopNode)
                {
                    break;
                }

                tempNodeQueue.Enqueue(childNode);
            }
        }

        gameObject.SetActive(false);
    }


}
