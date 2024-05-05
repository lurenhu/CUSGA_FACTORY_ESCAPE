using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AILocked : MonoBehaviour
{
    [Header("可调整参数")]
    public int submissionTimes;// 剩余提交次数
    [SerializeField] private bool hasResult = false;// 是否已经获取结果
    [SerializeField] private bool getCutScene = false;// 是否已经获取结果
    private List<CutSceneCell> failCutScene= new List<CutSceneCell>();
    private string openingRemark;
    private string AIName;
    private Node myNode;

    private Color orange = new Color(0.9137256f, 0.5647059f, 0.2078432f);
    private Color green = new Color(0.360404f, 0.8396226f, 0.3699884f);

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
        failCutScene = aiLockedNodeSO.failCutScene;
        openingRemark = aiLockedNodeSO.openingRemark;   
        AIName = aiLockedNodeSO.AIName;
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
                {
                    tongyi_AI.instance.SubmitTimer = submissionTimes;
                    DialogSystem.Instance.AICharacter_1.sprite = GameResources.Instance.characters.Find(x => x.name == "8DE").sprite;
                    UIManager.Instance.UIShow = true;
                    DialogSystem.Instance.AIDialogPanel.gameObject.SetActive(true);
                    DialogSystem.Instance.AINameText.text = AIName;
                    DialogSystem.Instance.AIDialogText.text = openingRemark;
                    DialogSystem.Instance.PopUpAIDialogPanel();
                    DialogSystem.Instance.anxietyValue.localScale = new Vector3(GameManager.Instance.currentAnxiety/GameManager.Instance.maxAnxiety, 1, 1);
                    DialogSystem.Instance.value.text = (GameManager.Instance.currentAnxiety/GameManager.Instance.maxAnxiety * 100).ToString("F0") + "%";
                    foreach (Image image in DialogSystem.Instance.SubmitTimer)
                    {
                        image.color = orange;
                    }
                }
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
        tongyi_AI.instance.SubmitTimer--;

        if (DialogSystem.Instance.SubmitTimer[tongyi_AI.instance.SubmitTimer] != null)
        {
            DialogSystem.Instance.SubmitTimer[tongyi_AI.instance.SubmitTimer].color = green;
        }
        DialogSystem.Instance.anxietyValue.localScale = new Vector3(GameManager.Instance.currentAnxiety/GameManager.Instance.maxAnxiety, 1, 1);
        DialogSystem.Instance.value.text = (GameManager.Instance.currentAnxiety/GameManager.Instance.maxAnxiety * 100).ToString("F0") + "%";

        if (tongyi_AI.instance.SubmitTimer == 0)
        {
            if (myNode.nodeInfos.Count > 0)
            {
                DialogSystem.Instance.get_text_in_other_ways("823", "感谢你的努力，陶特","8N");
            }
            else
            {
                if (GameManager.Instance.CheckAnxietyValue())
                {
                    DialogSystem.Instance.get_text_in_other_ways("823", "感谢你的努力，陶特","8N");
                }
                else
                {
                    DialogSystem.Instance.get_text_in_other_ways("823","陶特，我感觉不太好......","8AN");
                }
            }

            hasResult = true;
        }
    }

    private void Update() {
        if (hasResult && DialogSystem.Instance.textFinished && Input.GetMouseButtonDown(0) && DialogSystem.Instance.AIDialogPanel.gameObject.activeSelf)
        {
            DialogSystem.Instance.AIDialogPanel.gameObject.SetActive(false);
            UIManager.Instance.UIShow = false;
            if (myNode.nodeInfos.Count > 0)
            {
                StartCoroutine(myNode.PopUpChildNodes(myNode.nodeInfos));
            }
            else
            {
                CheckAnxietyValue();
            }
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
            StaticEventHandler.CallGetNextNodeLevel();
        }
        else
        {
            // 播放失败过场
            StaticEventHandler.CallGetResult(failCutScene);
            GameManager.Instance.gameState = GameState.Fail;
        }
    }
}
