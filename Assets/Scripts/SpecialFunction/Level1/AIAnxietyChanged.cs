using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnxietyChanged : MonoBehaviour
{
    private Node myNode;
    private float targetRate = 0;

    public void InitializeAiAnxietyChanged(NodeSO nodeSO)
    {
        AIAnxietyChangedSO aIAnxietyChangedSO= (AIAnxietyChangedSO)nodeSO;

        targetRate = aIAnxietyChangedSO.changeRate;

        myNode = GetComponent<Node>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
                GetAIAnxietyRateChanged();
                
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

    private void GetAIAnxietyRateChanged()
    {
        GameManager.Instance.rate += targetRate;

        gameObject.SetActive(false);
        LineCreator.Instance.DeleteLine(myNode);
    }
}
