using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickClick : MonoBehaviour
{
    [Header("观测数据")]
    private Node myNode;
    public int ClickNumber;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    /// <summary>
    /// 传递快速点击节点数据
    /// </summary>
    public void InitializeQuickClick(NodeSO nodesSO) 
    {
        QuickClickNodeSO quickClickNodeSO = (QuickClickNodeSO)nodesSO;

        ClickNumber = quickClickNodeSO.ClickNumber;
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                if (ClickNumber > 0)
                    ClickNumber--;
    
                // 节点交互内容
                if (!myNode.hasPopUp && ClickNumber == 0)
                {
                    StartCoroutine(myNode.PopUpChildNodes(myNode.nodeInfos));
                    myNode.hasPopUp = true;
                }
    
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }
            UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }

}
