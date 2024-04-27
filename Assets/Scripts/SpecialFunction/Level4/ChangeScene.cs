using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    private Node myNode;

    private void Start() {
        myNode = GetComponent<Node>();
    }

    private void OnMouseUp() 
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;
        
        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                GameManager.Instance.levelIndex++;
                GameManager.Instance.gameState = GameState.Generating;
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
