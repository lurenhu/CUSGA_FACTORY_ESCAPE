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
        if (myNode.isPopping) return;
        if (myNode.isDragging) myNode.isDragging = false;

        if (myNode.isSelected)
        {
            // 关卡结束
            Debug.Log("游戏结束");
                       
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);

            myNode.isSelected = true;
        }
    }
}
