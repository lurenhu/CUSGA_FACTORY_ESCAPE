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
                    tongyi_AI.instance.input_field.SetActive(true);
                    DialogSystem.Instance.PopUpAIDialogPanel();
                    DialogSystem.Instance.submitText.text = "剩余对话次数:" + submissionTimes;
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
        DialogSystem.Instance.SubmitTimer[submissionTimes].color = green;


        DialogSystem.Instance.submitText.text = "剩余对话次数:" + submissionTimes;
        DialogSystem.Instance.anxietyValue.localScale = new Vector3(GameManager.Instance.currentAnxiety/GameManager.Instance.maxAnxiety, 1, 1);
        DialogSystem.Instance.value.text = (GameManager.Instance.currentAnxiety/GameManager.Instance.maxAnxiety * 100).ToString("F0") + "%";

        if (submissionTimes == 0)
        {
            hasResult = true;
        }
    }

    private void Update() {
        if (hasResult && DialogSystem.Instance.textFinished && Input.GetMouseButtonDown(0) && DialogSystem.Instance.AIDialogPanel.gameObject.activeSelf)
        {
            DialogSystem.Instance.AIDialogPanel.gameObject.SetActive(false);
            UIManager.Instance.UIShow = false;
        }

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
            GameManager.Instance.canvasGroup.blocksRaycasts = true;
            StartCoroutine(GameManager.Instance.Fade(0,1,2,Color.black));
            soundManager.Instance.PlaySFX("ChangeScene");
            GameManager.Instance.levelIndex++;
            GameManager.Instance.gameState = GameState.Generating;
        }
        else
        {
            // 播放失败过场
            GameManager.Instance.canvasGroup.blocksRaycasts = true;
            StartCoroutine(GameManager.Instance.Fade(0,1,2,Color.black));
            soundManager.Instance.PlaySFX("ChangeScene");
            VideoManager.Instance.ShowCutScenes(failCutScene);

        }
    }
}
