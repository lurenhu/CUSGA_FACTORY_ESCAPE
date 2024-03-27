using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Default : MonoBehaviour
{
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
    
                // 弹出子节点
                if (!myNode.hasPopUp)
                {
                    myNode.PopUpChildNode(myNode.nodeInfos);
                    myNode.hasPopUp = true;
                    return;
                }
                
                // 播放音频
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }
        }
        
        if (myNode.isDragging) myNode.isDragging = false;
    }
}
