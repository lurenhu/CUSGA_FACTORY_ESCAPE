using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILocked : MonoBehaviour
{
    [Header("可调整参数")]
    public int submissionTimes;// 剩余提交次数
    [SerializeField] private bool hasResult = false;// 是否已经获取结果
    [SerializeField] private bool getCutScene = false;// 是否已经获取结果
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

    public void InitializeAILocked(NodeSO nodeSO)
    {
        AILockedNodeSO aiLockedNodeSO = (AILockedNodeSO)nodeSO;

        submissionTimes = aiLockedNodeSO.submissionTimes;
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
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
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        }        

    }

    private void StaticEventHandler_OnCommit(CommitArgs args)
    {
        GameManager.Instance.currentAnxiety += args.anxiety_change_value;
        submissionTimes--;

        if (submissionTimes == 0)
        {
            hasResult = true;
        }
    }

    private void Update() {
        if (hasResult && DialogSystem.Instance.textFinished && Input.GetMouseButtonDown(0) && DialogSystem.Instance.AIDialogPanel.gameObject.activeSelf)
            DialogSystem.Instance.AIDialogPanel.gameObject.SetActive(false);

        if (hasResult && !DialogSystem.Instance.AIDialogPanel.gameObject.activeSelf && !getCutScene)
        {
            CheckAnxietyValue();
            getCutScene = true;
        }

    }

    /// <summary>
    /// 检查焦虑值并给出相关的结局
    /// </summary>
    private void CheckAnxietyValue()
    {
        if (GameManager.Instance.CheckAnxietyValue())
        {
            // 前往下一关
            GameManager.Instance.levelIndex++;
            GameManager.Instance.gameState = GameState.Generating;
        }
        else
        {
            // 播放失败过场
        }
    }
}
