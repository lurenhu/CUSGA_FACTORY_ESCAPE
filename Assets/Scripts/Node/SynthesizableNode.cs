using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SynthesizableNode : Node
{
    [Space(10)]
    [Header("SYNTHESIZABLE NODE")]
    public bool hasSynthesized = false;
    public Node currentNode;

    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        if (isSelected)
        {
           // 节点交互内容
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(this);

            isSelected = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        Debug.Log("onTrigger");
        if (hasSynthesized || isPopping) return;
        currentNode = collision.GetComponent<Node>();

        if (!currentNode.isDragging && !currentNode.isPopping && !isDragging)
        {
            MergeTwoNode();

            hasSynthesized = true;
        }
    }

    /// <summary>
    /// 合成两个节点，生成子节点
    /// </summary>
    private void MergeTwoNode()
    {
        NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(nodeProperty.targetNodeID,out Node targetNode);

        if (targetNode == currentNode)
        {
            LineCreator.Instance.DeleteLine(currentNode);

            Destroy(targetNode.transform.gameObject);
    
            PopUpChildNode(nodeInfos);
        }
    }
}
