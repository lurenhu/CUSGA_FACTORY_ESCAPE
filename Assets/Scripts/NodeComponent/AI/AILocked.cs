using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILocked : MonoBehaviour
{
    [Header("可调整参数")]
    public int anxietyValue = 100;// 焦虑值
    public int submissionTimes = 10;// 剩余提交次数
    private bool hasResult = false;// 是否已经获取结果

    private Node myNode;

    private void OnEnable() {
        StaticEventHandler.OnCommit += StaticEventHandler_OnCommit;
    }

    private void OnDisable() {
        StaticEventHandler.OnCommit -= StaticEventHandler_OnCommit;
    }

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping) return;
        if (myNode.isDragging) myNode.isDragging = false;

        if (myNode.isSelected)
        {
            // 节点交互内容
            if(!hasResult)
                tongyi_AI.instance.input_field.SetActive(true);
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);

            myNode.isSelected = true;
        }
    }

    private void StaticEventHandler_OnCommit(CommitArgs args)
    {
        anxietyValue += args.anxiety_change_value;
        submissionTimes--;

        CheckAnxietyValue();

        if (submissionTimes == 0)
            tongyi_AI.instance.input_field.SetActive(false);
    }

    /// <summary>
    /// 检查焦虑值并弹出对应子节点
    /// </summary>
    private void CheckAnxietyValue()
    {
        if (submissionTimes != 0)
            return;
        
        if (anxietyValue >= 80 && anxietyValue < 100)
        {
            myNode.PopUpChildNode(myNode.nodeInfos[0]);
        }
        else if (anxietyValue >= 30 && anxietyValue < 80)
        {
            myNode.PopUpChildNode(myNode.nodeInfos[1]);
        }
        else if (anxietyValue >= 0 && anxietyValue < 30)
        {
            myNode.PopUpChildNode(myNode.nodeInfos[2]);
        }

        hasResult = true;
    }
}
