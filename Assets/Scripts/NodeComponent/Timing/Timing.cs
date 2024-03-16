using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timing : MonoBehaviour
{
    [Header("观测参数")]
    public Node targetNode;
    private Node myNode;
    private float duration;
    private string targetId;
    private Coroutine timerCoroutine;

    private void Start() {
        timerCoroutine = StartCoroutine(StartTimer());
    }

    private void Update() 
    {
        if (targetNode != null && !targetNode.gameObject.activeSelf)
        {
            StopTimer();
        }

        if (targetNode != null && targetNode.gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartTimer();
        }
    }

    public void InitializeTiming(NodeSO nodeSO)
    {
        myNode = transform.GetComponent<Node>();

        TimingNodeSO timingNodeSO = (TimingNodeSO)nodeSO;

        duration = timingNodeSO.duration;
        targetId = timingNodeSO.targetIdForStop;

        targetNode = NodeMapBuilder.Instance.nodeHasCreated[targetId];

        BeClocked beClocked = targetNode.gameObject.AddComponent<BeClocked>();
        beClocked.InitializeBeClocked(myNode);
    }


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
            myNode.nodeText += " " + timerString;

            Debug.Log("剩余时间：" + timerString);
        }

        Debug.Log("时间到！");
        // 在此处执行计时结束后的操作
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            gameObject.SetActive(false);
        }
    }


}
