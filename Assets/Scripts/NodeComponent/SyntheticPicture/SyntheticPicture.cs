using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyntheticPicture : MonoBehaviour
{
    public Node targetNode;
    private string targetId;
    private Sprite image;
    private Node myNode;
    private bool hasSynthesized = false;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    /// <summary>
    /// 传递数据至合成图片功能组件
    /// </summary>
    public void InitializeSyntheticPicture(NodeSO nodeSO)
    {
        SyntheticPictureNodeSO syntheticPictureNodeSO = (SyntheticPictureNodeSO)nodeSO;

        targetId = syntheticPictureNodeSO.targetIdForMerge;
        image = syntheticPictureNodeSO.image;
    }

    private void OnTriggerStay2D(Collider2D collision) 
    {
        if (targetNode == null && NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(targetId, out Node targetNodeTemp))
            targetNode = targetNodeTemp;

        if (targetNode != null)
        {
            if (hasSynthesized || targetNode.isPopping || myNode.isPopping || targetNode.isDragging || myNode.isDragging) return;

            if (collision.transform.GetComponent<Node>() == targetNode)
            {
                MergeTowNode();

                hasSynthesized = true;
            }
        }
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

    /// <summary>
    /// 合成两个节点
    /// </summary>
    private void MergeTowNode()
    {
        targetNode.gameObject.SetActive(false);
        LineCreator.Instance.DeleteLine(targetNode);

        StartCoroutine(myNode.PopUpChildNode(myNode.nodeInfos));
    }

    /// <summary>
    /// 弹出图片
    /// </summary>
    private void PopUpGraph()
    {
        Image image = UIManager.Instance.graphNodeUI.Find("Image").GetComponent<Image>();

        image.sprite = this.image;

        UIManager.Instance.graphNodeUI.gameObject.SetActive(true);
    }
}
