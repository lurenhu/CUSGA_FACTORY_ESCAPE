using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 关卡结束
                Debug.Log("游戏结束");
                           
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
}
