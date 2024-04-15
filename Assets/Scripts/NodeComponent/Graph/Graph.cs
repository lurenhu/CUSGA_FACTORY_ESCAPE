using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    [Header("观测数据")]
    public Sprite image;
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    public void InitializeGraph(NodeSO nodeSO)
    {
        GraphNodeSO graphNode = (GraphNodeSO)nodeSO;

        image = graphNode.image;
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
                PopUpGraph();
    
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

    private void PopUpGraph()
    {
        Image image = UIManager.Instance.graphNodeUI.Find("Close/Graph").GetComponent<Image>();

        image.sprite = this.image;

        UIManager.Instance.graphNodeUI.gameObject.SetActive(true);
    }
}
