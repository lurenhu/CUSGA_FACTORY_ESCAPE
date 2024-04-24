using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level1AILock : MonoBehaviour
{
    private Node myNode;
    [SerializeField] private int submissionTimes;
    [SerializeField] private bool hasResult = false;
    private List<CutSceneCell> firstFailResult;
    private List<CutSceneCell> secondFailResult;
    private bool getCutScene = false;

    public void InitializeLevel1AILock(NodeSO nodeSO)
    {
        Level1AILockSO level1AILockSO= (Level1AILockSO)nodeSO;

        submissionTimes = level1AILockSO.submissionTimes;
        firstFailResult = level1AILockSO.firstFailResult;
        secondFailResult = level1AILockSO.secondFailResult;

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
            hasResult = true;
            tongyi_AI.instance.input_field.SetActive(false);
        }
    }

    private void Update() {
        if (hasResult && !DialogSystem.Instance.dialogPanel.activeSelf && !getCutScene)
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
        if (GameManager.Instance.level1GetResultTimes == 0)
        {
            if (GameManager.Instance.CheckAnxietyValue())
            {
                Debug.Log("out of area");
            }
            else
            {
                VideoManager.Instance.ShowCutScenes(firstFailResult);

                UIManager.Instance.leftNodeGraphButton.gameObject.SetActive(true);
                UIManager.Instance.rightNodeGraphButton.gameObject.SetActive(true);

                GameManager.Instance.level1GetResultTimes++;
            }
        }
        else
        {
            if (GameManager.Instance.CheckAnxietyValue())
            {
                GameManager.Instance.levelIndex++;
                GameManager.Instance.gameState = GameState.Generating;
            }
            else
            {
                VideoManager.Instance.ShowCutScenes(secondFailResult);
            }
        }
    }
}
