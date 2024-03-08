using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILockedNode : Node
{
    [Space(10)]
    [Header("AI LOCK NODE")]
    public int anxietyValue = 100;// 焦虑值
    public int submissionTimes = 10;// 剩余提交次数

    private void OnEnable() {
        StaticEventHandler.OnCommit += StaticEventHandler_OnCommit;
    }

    private void OnDisable() {
        StaticEventHandler.OnCommit -= StaticEventHandler_OnCommit;
    }

    private void StaticEventHandler_OnCommit(CommitArgs args)
    {
        anxietyValue += args.anxiety_change_value;
        submissionTimes--;
    }

    protected override void OnMouseUp() {
        base.OnMouseUp();
        
        if (isSelected)
        {
            // 节点交互内容

            if (!tongyi_AI.instance.input_field.activeSelf)
            {
                tongyi_AI.instance.input_field.SetActive(true);
            }

            CheckAnxietyValue();
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(this);

            isSelected = true;
        }
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
            PopUpChildNode(nodeInfos[0]);
        }
        else if (anxietyValue >= 30 && anxietyValue < 80)
        {
            PopUpChildNode(nodeInfos[1]);
        }
        else if (anxietyValue >= 0 && anxietyValue < 30)
        {
            PopUpChildNode(nodeInfos[2]);
        }
    }
}
