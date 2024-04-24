using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1AILock : MonoBehaviour
{
    private Node myNode;
    [SerializeField] private int submissionTimes;
    [SerializeField] private bool hasResult = false;

    public void InitializeLevel1AILock(NodeSO nodeSO)
    {
        Level1AILockSO level1AILockSO= (Level1AILockSO)nodeSO;

        submissionTimes = level1AILockSO.submissionTimes;

        myNode = GetComponent<Node>();
    }

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
            CheckAnxietyValue();
            hasResult = true;
            tongyi_AI.instance.input_field.SetActive(false);
        }
    }

    /// <summary>
    /// 检查焦虑值并给出相关的结局
    /// </summary>
    private void CheckAnxietyValue()
    {
        if (GameManager.Instance.level1GetResultTimes == 0)
        {
            if (GameManager.Instance.CheckAnxietyValue())
            {
                Debug.Log("out of area");
            }
            else
            {
                VideoManager.Instance.ShowCutScenes(GameManager.Instance.nodeLevelSOs[GameManager.Instance.levelIndex].cutSceneList);// 臭太tm臭啦
                GameManager.Instance.level1GetResultTimes++;

                UIManager.Instance.leftNodeGraphButton.gameObject.SetActive(true);
                UIManager.Instance.rightNodeGraphButton.gameObject.SetActive(true);
            }
        }
        else
        {
            GameManager.Instance.ShowCutScenes(GameManager.Instance.CheckAnxietyValue());
        }
    }
}
