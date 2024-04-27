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

    private void Start() {
        StartCoroutine(StartTimer());
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
        TimingNodeSO timingNodeSO = (TimingNodeSO)nodeSO;

        duration = timingNodeSO.duration;
        startNode = NodeMapBuilder.Instance.GetNode(timingNodeSO.startNodeId);
        myNode = transform.GetComponent<Node>();

        BeClocked beClocked = startNode.gameObject.AddComponent<BeClocked>();
        beClocked.InitializeBeClocked(this.myNode);
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
        GameManager.Instance.gameState = GameState.Won;
    }

}
