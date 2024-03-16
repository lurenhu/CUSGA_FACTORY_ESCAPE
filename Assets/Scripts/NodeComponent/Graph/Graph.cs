using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
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
        if (myNode.isPopping) return;
        if (myNode.isDragging) myNode.isDragging = false;

        if (myNode.isSelected)
        {
            // 节点交互内容
            PopUpGraph();
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);

            myNode.isSelected = true;
        }
    }

    private void PopUpGraph()
    {
        Image image = GameManager.Instance.graphTransform.Find("Image").GetComponent<Image>();

        image.sprite = this.image;

        GameManager.Instance.graphTransform.gameObject.SetActive(true);
    }
}